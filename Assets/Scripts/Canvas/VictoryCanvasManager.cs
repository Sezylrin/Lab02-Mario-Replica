using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryCanvasManager : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(Wait5Seconds());
    }

    private IEnumerator Wait5Seconds()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        Loader.Load(Loader.Scene.MainMenu);
    }
}
