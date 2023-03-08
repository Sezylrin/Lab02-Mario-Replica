using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClear : MonoBehaviour
{
    private LevelManager _levelManager;
    private AudioSource _bgMusic;

    private void Awake()
    {
        _levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        _bgMusic = _levelManager.GetComponent<AudioSource>();
        if (_levelManager.GetComponent<AudioSource>().isPlaying) return;
        _levelManager.GetComponent<AudioSource>().Play();
        _levelManager.GetComponent<AudioSource>().loop = false;
    }
}