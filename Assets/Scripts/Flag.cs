using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    private AudioSource _audioSource;
    private PlayerMovement _playerMovement;
    [SerializeField] private GameManager gameManager;

    public bool IsExiting { get; private set; }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Time.timeScale = 0;
        _audioSource.Play();
        var playerPos = col.transform.position;
        if (!col.gameObject.CompareTag("Player"))
            return;
        _playerMovement = col.GetComponentInParent<PlayerMovement>();
        if (!_playerMovement)
            return;
        StopPlayer(_playerMovement);

        if (gameManager) gameManager.AddToScore(100);
        // Or
        if (gameManager) gameManager.AddToScore(5000); //TODO: Score points / Initiate end sequence

        SlideDown(col);

        _playerMovement.Turn();
        col.gameObject.transform.position = new Vector3(playerPos.x, 0.0f, playerPos.z); //Move to other side of Flag

        StartPlayer(_playerMovement);

        IsExiting = true; //TODO: Exit Stage Right
    }

    private void FixedUpdate()
    {
        if (!IsExiting)
            return;
        _playerMovement.Walk(1);
    }

    private void SlideDown(Collider2D col)
    {
        col.gameObject.transform.position = new Vector3(col.gameObject.transform.position.x, 0.0f, col.gameObject.transform.position.z); //TODO: Tween Down
    }

    private void StopPlayer(PlayerMovement playerMovement)
    {
        playerMovement.Input.Disable();
        playerMovement.rb.isKinematic = true;
    }
    private void StartPlayer(PlayerMovement playerMovement)
    {
        playerMovement.rb.isKinematic = false;
    }
}