using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("TextFields")]  
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text worldText;

    [Header("Timer")]
    [SerializeField] private float startingTime;
    [SerializeField] private int timerSpeed = 3;

    [Header("World")]
    [SerializeField] private string world;

    private int coinsCollected = 0;
    private int score = 0;
    private float currentTime;

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
    }

    private void Start()
    {
        currentTime = startingTime;
        worldText.text = world;
    }

    private void Update()
    {
        RunTimer();
    }

    private void RunTimer()
    {
        currentTime -= timerSpeed * Time.deltaTime;
        timerText.text = Mathf.RoundToInt(currentTime).ToString();
        if (currentTime <= 0)
        {
            currentTime = 0;
            //TODO: Display gameover screen
        }
    }

    public void AddToScore(int amountToIncrease)
    {
        int newScore = score + amountToIncrease;
        score = newScore;
        scoreText.text = newScore.ToString("D6");
    }

    public int GetScore() { return score; }


    public void IncrementCoinCount(int amountToIncrease = 1)
    {
        int newCoinsCollected = coinsCollected + amountToIncrease;
        if (newCoinsCollected == 100)
        {
            //TODO: Add life
            newCoinsCollected = 0;
        }
        coinsCollected = newCoinsCollected;
        coinText.text = coinsCollected.ToString("D2");
    }

    public int GetCoinsCollected() { return coinsCollected; }
}
