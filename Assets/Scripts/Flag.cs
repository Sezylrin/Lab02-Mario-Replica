using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    private AudioSource _audioSource;
    private PlayerMovement _playerMovement;
    private GameManager _gameManager;
    [SerializeField] private Transform flagTop;
    private struct Tween
    {
        public Transform Target;
        public Vector2 StartPos;
        public Vector2 EndPos;
        public float Duration;
        public float Time;
    };
    private Tween _flagSlideTween;
    private bool _isSliding;
    private MarioAnim _marioAnim;

    public bool IsExiting { get; private set; }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player"))
            return;
        _playerMovement = col.GetComponentInParent<PlayerMovement>();
        if (!_playerMovement)
            return;
        _audioSource.Play();

        StopPlayer();

        AddScore(col);

        SlideDown(col);
    }


    private void Update()
    {
        if (!_isSliding)
            return;
        if (Vector2.Distance(_flagSlideTween.Target.position, _flagSlideTween.EndPos) > 0.1f)
        {
            _flagSlideTween.Target.position = Vector2.Lerp(_flagSlideTween.StartPos, _flagSlideTween.EndPos,
                _flagSlideTween.Time / _flagSlideTween.Duration);
            _flagSlideTween.Time += Time.deltaTime;
        }
        else
        {
            _flagSlideTween.Target.position = _flagSlideTween.EndPos;
            _isSliding = false;
            LeaveFlag(_flagSlideTween.Target);
            ExitRight();
        }
    }

    private void FixedUpdate()
    {
        if (!IsExiting)
            return;
        _playerMovement.Walk(1, 1);
    }

    private void SlideDown(Collider2D col)
    {
        col.transform.Translate(flagTop.position.x - col.transform.position.x, 0.0f, 0.0f);
        if (!_playerMovement.IsFacingRight) _playerMovement.Turn();
        _marioAnim = col.gameObject.GetComponentInChildren<MarioAnim>();
        _marioAnim.flagSliding = true;
        _marioAnim.Play = false;
        _flagSlideTween.Target = col.transform;
        _flagSlideTween.StartPos = col.transform.position;
        _flagSlideTween.EndPos = this.transform.position;
        _flagSlideTween.Duration = 2 * (Vector2.Distance(col.transform.position, this.transform.position) /
                                        Vector2.Distance(flagTop.position, this.transform.position));
        _flagSlideTween.Time = 0.0f;
        _isSliding = true;
    }

    private void LeaveFlag(Transform player)
    {
        _playerMovement.Turn();
        player.Translate(Vector3.right);
        _marioAnim.flagSliding = false;
        _marioAnim.Play = true;
    }

    private void ExitRight()
    {
        _playerMovement.rb.isKinematic = false;
        _playerMovement.Turn();
        IsExiting = true;
    }

    private void StopPlayer()
    {
        _playerMovement.Input.Disable();
        _playerMovement.rb.velocity = Vector2.zero;
        _playerMovement.rb.isKinematic = true;
    }

    private void AddScore(Collider2D col)
    {
        int amountToAdd;
        float range = 100 * (col.transform.position.y - this.transform.position.y) /
                      (flagTop.position.y - this.transform.position.y);
        switch (range)
        {
            case > 80:
                amountToAdd = 5000;
                break;
            case > 60:
                amountToAdd = 2000;
                break;
            case > 40:
                amountToAdd = 800;
                break;
            case > 20:
                amountToAdd = 400;
                break;
            default:
                amountToAdd = 100;
                break;
        }

        if (!_gameManager)
            return;
        _gameManager.AddToScore(amountToAdd);
        _gameManager.DisplayFloatingText(amountToAdd.ToString(), this.gameObject.transform.position);
    }
}