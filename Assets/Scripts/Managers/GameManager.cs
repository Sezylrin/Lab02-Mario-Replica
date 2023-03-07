using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Timer")]
    [SerializeField] private int timerSpeed = 3;

    private int coinsCollected = 0;
    private int score = 0;
    private float currentTime;
    private int lives = 0;
    private bool levelStarted = false;

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

    private void Update()
    {
        if (levelStarted)
        {
            RunTimer();
        }
    }

    public void NewGame(Loader.Scene scene)
    {
        coinsCollected = 0;
        score = 0;
        levelStarted = true;

        switch (scene)
        {
            case Loader.Scene.OneOne:
                currentTime = 400;
                GameCanvasManager.Instance.SetWorldText("1-1");
                break;
        }
    }

    private void RunTimer()
    {
        currentTime -= timerSpeed * Time.deltaTime;
        GameCanvasManager.Instance.SetTimerText(Mathf.RoundToInt(currentTime).ToString());
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
        GameCanvasManager.Instance.SetScoreText(newScore.ToString("D6"));
    }

    public int GetScore() { return score; }


    public void IncrementCoinCount(int amountToIncrease = 1)
    {
        int newCoinsCollected = coinsCollected + amountToIncrease;
        if (newCoinsCollected == 100)
        {
            AddLives();
            newCoinsCollected = 0;
        }
        coinsCollected = newCoinsCollected;
        GameCanvasManager.Instance.SetCoinText(coinsCollected.ToString("D2"));
    }

    public int GetCoinsCollected() { return coinsCollected; }

    public void AddLives(int livesToAdd = 1)
    {
        int newLives = lives + livesToAdd;
        lives = newLives;
        //TODO: Update UI
    }

    public int GetLives() { return lives; }
}
