using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PreLevelCanvasManager : MonoBehaviour
{
    [SerializeField] private TMP_Text worldText;
    [SerializeField] private TMP_Text livesText;

    private void Awake()
    {
        worldText.text = GameManager.Instance.GetWorld().ToString();
        livesText.text = GameManager.Instance.GetLives().ToString();
        StartCoroutine(Wait5Seconds());
    }

    private IEnumerator Wait5Seconds()
    {
        yield return new WaitForSeconds(5);
        Loader.Load(Loader.Scene.OneOne);
    }
}
