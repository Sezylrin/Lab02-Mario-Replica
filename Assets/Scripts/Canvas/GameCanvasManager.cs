using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameCanvasManager : MonoBehaviour
{
    public static GameCanvasManager Instance { get; private set; }

    [Header("TextFields")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text worldText;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        worldText.text = GameManager.Instance.GetWorld();
    }

    public void SetScoreText(string text)
    {
        scoreText.text = text;
    }

    public void SetCoinText(string text)
    {
        coinText.text = text;
    }

    public void SetTimerText(string text)
    {
        timerText.text = text;
    }

    public void SetWorldText(string text)
    {
        worldText.text = text;
    }
}
