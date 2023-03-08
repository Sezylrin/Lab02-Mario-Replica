using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private PlayerManager _playerManager;
    private bool handledDeath;
    private void Update()
    {
        if (!_playerManager) return;
        if (handledDeath) return;
        if (_playerManager.AudioSource.isPlaying) return;
        GameManager.Instance.HandleMarioDeath();
        handledDeath = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        _playerManager = col.GetComponent<PlayerManager>();
        col.GetComponent<PlayerMovement>().enabled = false;
        FindObjectOfType<LevelManager>().GetComponent<AudioSource>().Pause();
        _playerManager.AudioSource.clip = _playerManager.marioDieClip;
        _playerManager.AudioSource.Play();
    }
}