using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Components
        [SerializeField] private PlayerData data;
        public InputManager Input { get; private set; }
        public Rigidbody2D rb { get; private set; }
        public PlayerManager PlayerManager { get; private set; }
        public AudioSource AudioSource { get; private set; }
        [SerializeField] private List<AudioClip> AudioClips;
    #endregion

    #region State Parameters
        public bool IsFacingRight { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsJumpCutting { get; private set; }
        public bool IsRunning { get; private set; }
        public float LastOnGroundTime { get; private set; }
    #endregion

    #region Input Parameters
        public float LastPressedJumpTime { get; private set; }
    #endregion

    #region Check Parameters
        [Header("Checks")]
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private Vector2 groundCheckSize;
    #endregion

    #region Layers & Tags
        [Header("Layers & Tags")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask ObsticalLayer;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Input = new InputManager();
        PlayerManager = GetComponent<PlayerManager>();
        AudioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        Input.Player.Jump.started += ctx => OnJumpInput();
        Input.Player.Jump.canceled += ctx => OnJumpUpInput();
        Input.Player.Run.started += ctx => OnRunInput();
        Input.Player.Run.canceled += ctx => OnRunUpInput();
        SetGravityScale(data.gravityScale);
        IsFacingRight = true;
    }

    private void OnEnable()
    {
        Input.Enable();
    }

    private void OnDisable()
    {
        Input.Disable();
    }

    void Update()
    {
        #region Physics Checks
            if (!IsJumping)
            {
                //Ground Check
                if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0,
                        groundLayer)) //Checks if set box overlaps with ground
                    LastOnGroundTime = data.coyoteTime;//If so sets the lastGrounded to coyoteTime
            }
        #endregion

        #region General Checks
            if (Input.Player.Move.ReadValue<Vector2>().x != 0)
                if (LastOnGroundTime >= data.coyoteTime)
                {
                    CheckDirectionToFace(Input.Player.Move.ReadValue<Vector2>().x > 0);
                }
        #endregion

        #region Gravity
            if (rb.velocity.y >= 0)
                SetGravityScale(data.gravityScale);
            else
                SetGravityScale(data.gravityScale * data.fallGravityMult);
        #endregion

        #region Jump Checks
            if (IsJumping && rb.velocity.y <= 0)
                IsJumping = false;

            //Jump
            if (CanJump())
            {
                IsJumping = true;
                Jump();
            }

            if (IsJumpCutting && CanJumpCut())
            {
                JumpCut();
            }
        #endregion
        #region Timer
            LastOnGroundTime -= Time.deltaTime;
            LastPressedJumpTime -= Time.deltaTime;
        #endregion
    }

    private void FixedUpdate()
    {
        #region Drag
            Drag(LastOnGroundTime <= data.coyoteTime ? data.dragAmount : data.frictionAmount);
        #endregion

        #region Walk
            Walk(1);
        #endregion

        #region Clamped Fall Speed
            if (rb.velocity.y < -data.maxFallSpeed) rb.velocity = new Vector2(rb.velocity.x, -data.maxFallSpeed);
        #endregion
    }

    #region Input Callbacks
        private void OnJumpInput()
        {
            LastPressedJumpTime = data.jumpBufferTime;
            IsJumpCutting = false;
            PlayJumpSound();
        }

        private void OnJumpUpInput()
        {
            IsJumpCutting = true;
        }

        private void OnRunInput()
        {
            IsRunning = true;
        }

        private void OnRunUpInput()
        {
            IsRunning = false;
        }
    #endregion

    #region Movement Methods
        private void SetGravityScale(float scale)
        {
            rb.gravityScale = scale;
        }

        private void Drag(float amount)
        {
            var velocity = rb.velocity;
            Vector2 force = amount * velocity.normalized;
            force.x = Mathf.Min(Mathf.Abs(velocity.x), Mathf.Abs(force.x)); //Ensures we only slow player down
            force.y = Mathf.Min(Mathf.Abs(velocity.y), Mathf.Abs(force.y)); //and apply a small force if they are moving slowly.
            force.x *= Mathf.Sign(velocity.x); //Find direction to apply force.
            force.y *= Mathf.Sign(velocity.y);

            rb.AddForce(-force, ForceMode2D.Impulse);
        }

        private void Walk(float lerpAmount)
        {
            float runMultiplier = (!IsRunning) ? 1 : data.runMultiplier;
            float targetSpeed = Input.Player.Move.ReadValue<Vector2>().x * (data.groundMaxSpeed * runMultiplier); //Calculate direction and target velocity
            float speedDif = targetSpeed - rb.velocity.x; //Calculate difference between current and target velocity.

            #region Acceleration Rate
                float accelRate;

                //Gets accel value based on if we are accelerating or trying to decelerate and applying multiplier if currently airborne.
                if (LastOnGroundTime > 0) //If on ground
                    accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.groundAccel * runMultiplier : data.groundDecel * runMultiplier;
                else
                    accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.groundAccel * data.accelInAir : data.groundDecel * data.decelInAir;

                //If we want to walk but are already going faster than max walk speed
                if ((rb.velocity.x > targetSpeed && targetSpeed > 0.01f ||
                     rb.velocity.x < targetSpeed && targetSpeed < -0.01f) && data.doKeepWalkMomentum)
                    accelRate = 0; //Prevent any deceleration from happening / conserve current momentum.
            #endregion

            #region Velocity Power
                float velPower;
                if (Mathf.Abs(targetSpeed) < 0.01f)
                    velPower = data.stopPower;
                //If moving, but the direction we're moving is not the same as the direction we're trying to move, set velocity to turningPower
                else if (Mathf.Abs(rb.velocity.x) > 0 && Mathf.Sign(targetSpeed) != Mathf.Sign(rb.velocity.x))
                    velPower = data.turnPower;
                else
                    velPower = data.accelPower;
            #endregion

            //Applies acceleration to speed difference, raise to set power so acceleration increases, multiply by sign to preserve direction
            float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
            movement = Mathf.Lerp(rb.velocity.x, movement, lerpAmount); //Lerp to prevent Walk from immediately slowing down playing in situations.

            rb.AddForce(Vector2.right * movement);
        }

        private void Turn()
        {
            var transformVar = transform;
            Vector3 scale = transformVar.localScale;
            scale.x *= -1;
            transformVar.localScale = scale;

            IsFacingRight = !IsFacingRight;
        }

        private void Jump()
        {
            //Ensures we can't  call a jump multiple times from one press
            LastPressedJumpTime = 0;
            LastOnGroundTime = 0;

            #region Perform Jump
                float force = data.jumpForce;

                if (rb.velocity.y < 0)
                    force -= rb.velocity.y;

                rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            #endregion
        }

        private void JumpCut()
        {
            IsJumpCutting = false;
            rb.AddForce(Vector2.down * (rb.velocity.y * (1 - data.jumpCutMultiplier)), ForceMode2D.Impulse);
        }
    #endregion

    #region Check Methods
        private void CheckDirectionToFace(bool isMovingRight)
        {
            if (isMovingRight != IsFacingRight)
                Turn();
        }

        private bool CanJump()
        {
            return LastOnGroundTime > 0 && !IsJumping && LastPressedJumpTime > 0;
        }

        private bool CanJumpCut()
        {
            return IsJumping && rb.velocity.y > 0;
        }
    #endregion

    #region Other Methods
        private void PlayJumpSound()
        {
            AudioSource.clip = (PlayerManager.PlayerState > 1) ? AudioClips[0] : AudioClips[1];
            AudioSource.Play();
        }
    #endregion
}