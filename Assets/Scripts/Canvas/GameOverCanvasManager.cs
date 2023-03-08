using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverCanvasManager : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(Wait5Seconds());
    }

    private IEnumerator Wait5Seconds()
    {
        yield return new WaitForSeconds(5);
        Loader.Load(Loader.Scene.MainMenu);
    }
}
