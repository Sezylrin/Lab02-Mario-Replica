using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    private enum Direction { Up, Left };
    [SerializeField] private Direction pipeFacing;
    [SerializeField] private Transform connectedPoint;
    private bool _stopWarp;
    private bool _tweening;
    private bool _finishedWarp;
    private struct Tween
    {
        public Transform Target;
        public Vector2 StartPos;
        public Vector2 EndPos;
        public float Duration;
        public float Time;
    };
    private Tween _warpTween;
    private AudioSource _audioSource;
    public Camera playerCamera;
    public Vector3 undergroundCameraPosition;
    public Vector3 surfaceCameraPosition;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player"))
            return;
        PlayerMovement playerMovement = col.GetComponentInParent<PlayerMovement>();
        Vector2 playerMoveInput = playerMovement.Input.Player.Move.ReadValue<Vector2>();
        switch (pipeFacing)
        {
            case Direction.Up when playerMoveInput.y >= 0:
            case Direction.Left when (playerMoveInput.x < 0 || col.GetComponentInParent<Rigidbody2D>().velocity.x <= 0):
                return;
            default:
                break;
        }
        Time.timeScale = 0;
        playerMovement.rb.velocity = Vector2.zero;
        playerMovement.enabled = false;
        _audioSource.Play();
        EnterPipe(col);
    }

    private void Update()
    {
        if (!_tweening)
            return;
        if (Vector2.Distance(_warpTween.Target.position, _warpTween.EndPos) > 0.1f)
        {
            _warpTween.Target.position =
                Vector2.Lerp(_warpTween.StartPos, _warpTween.EndPos, _warpTween.Time / _warpTween.Duration);
            _warpTween.Time += Time.unscaledDeltaTime;
        }
        else
        {
            _warpTween.Target.position = _warpTween.EndPos;
            _tweening = false;
            if (!_finishedWarp)
                Warp(_warpTween.Target);
            else ResumeTime(_warpTween.Target);
        }
    }

    private void Warp(Transform player)
    {
        player.position = connectedPoint.position;
        playerCamera.transform.position = undergroundCameraPosition;
        if (pipeFacing == Direction.Left)
        {
            ExitUpPipe(player);
            playerCamera.transform.position = surfaceCameraPosition;
            _finishedWarp = true;
            return;
        }
        ResumeTime(player);
    }

    private void EnterPipe(Collider2D player)
    {
        var other = player.gameObject;
        _warpTween.Target = other.transform;
        _warpTween.StartPos = other.transform.position;
        _warpTween.EndPos = this.transform.position;
        _warpTween.Duration = 1.0f;
        _warpTween.Time = 0.0f;
        _tweening = true;
    }

    private void ExitUpPipe(Transform player)
    {
        _warpTween.Target = player;
        _warpTween.StartPos = player.position;
        _warpTween.EndPos = connectedPoint.position + Vector3.up;
        _warpTween.Duration = 1.0f;
        _warpTween.Time = 0.0f;
        _tweening = true;
    }

    private void ResumeTime(Transform player)
    {
        Time.timeScale = 1;
        player.GetComponentInParent<PlayerMovement>().enabled = true;
    }
}