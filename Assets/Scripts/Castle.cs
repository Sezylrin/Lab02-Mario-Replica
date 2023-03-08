using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Castle : MonoBehaviour
{
    private LevelManager _levelManager;
    private bool _loadVictory;

    private void Awake()
    {
        _levelManager = FindObjectOfType<LevelManager>();
    }

    private void Update()
    {
        if (!_loadVictory) return;
        if (_levelManager.GetComponent<AudioSource>().isPlaying) return;
        GameManager.Instance.Victory();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        col.gameObject.SetActive(false);
        _loadVictory = true;
    }
}