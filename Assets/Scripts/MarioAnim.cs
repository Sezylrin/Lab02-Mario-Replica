using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class MarioAnim : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator MarioAnimator;
    private Rigidbody2D Rigid2D;
    private PlayerManager MarioState;
    private PlayerMovement MarioMove;
    [SerializeField] private PlayerData data;
    private bool IsRight = true;
    public bool NeedTurn = false;

    private string[] Idle = new string[] { "Idle-Small", "Idle-Big", "Idle-Fire" };
    private string[] Walk = new string[] { "Walk-Small", "Walk-Big", "Walk-Fire" };
    private string[] Jump = new string[] { "Jump-Small", "Jump-Big", "Jump-Fire" };
    private string[] Turn = new string[] { "Turn-Small", "Turn-Big", "Turn-Fire" };
    private bool Falling = false;
    void Start()
    {
        MarioAnimator = GetComponentInChildren<Animator>();
        Rigid2D = GetComponent<Rigidbody2D>();
        MarioState = GetComponent<PlayerManager>();
        MarioMove = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(Rigid2D.velocity.x) > data.groundMaxSpeed * 0.4f)
        {
            Debug.Log("running1");
            NeedTurn = true;
        }
        if (Rigid2D.velocity == Vector2.zero)
        {
            MarioAnimator.Play(Idle[MarioState.PlayerState - 1]);
        }
        else if (Rigid2D.velocity.y < 0 && !MarioMove.IsJumping)
        {
            MarioAnimator.speed = 0;
        }
        else if (MarioMove.IsJumping || Rigid2D.velocity.y != 0)
        {
            IsRight = MarioMove.IsFacingRight;
            MarioAnimator.applyRootMotion = true;
            MarioAnimator.speed = 1;
            MarioAnimator.Play(Jump[MarioState.PlayerState - 1]);
        }
        else if (IsRight != MarioMove.IsFacingRight && NeedTurn == true && Mathf.Abs(Rigid2D.velocity.x) > data.groundMaxSpeed * 0.1f)
        {
            MarioAnimator.applyRootMotion = false;
            MarioAnimator.speed = 1;
            MarioAnimator.Play(Turn[MarioState.PlayerState - 1]);
        }
        else
        {
            if (Mathf.Abs(Rigid2D.velocity.x) < data.groundMaxSpeed * 0.1f)
            {
                NeedTurn = false;
            }
            MarioAnimator.applyRootMotion = false;
            IsRight = MarioMove.IsFacingRight;
            MarioAnimator.speed = 1;
            MarioAnimator.Play(Walk[MarioState.PlayerState - 1]);
        }
    }
}
