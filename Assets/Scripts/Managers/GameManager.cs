using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject floatingTextPrefab;

    [Header("Timer")]
    [SerializeField] private int timerSpeed = 3;

    private int coinsCollected = 0;
    private int score = 0;
    private float currentTime;
    private int lives = 3;
    private bool levelStarted = false;
    private string world;
    private int highScore = 0;
    private bool respawnCalled = false;

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
        respawnCalled = false;
        coinsCollected = 0;
        score = 0;

        switch (scene)
        {
            case Loader.Scene.OneOne:
                currentTime = 400;
                GameCanvasManager.Instance.SetTimerText(currentTime.ToString());
                break;
        }

        levelStarted = true;
    }

    public void Respawn()
    {
        if (!respawnCalled)
        {
            respawnCalled = true;
            if (score > highScore)
            {
                highScore = score;
            }
            levelStarted = false;
            lives--;
            GameCanvasManager.Instance.SetTimerText("");
            currentTime = 0;
            Loader.Load(Loader.Scene.PreLevel);
        }
    }

    private void RunTimer()
    {
        currentTime -= timerSpeed * Time.deltaTime;
        GameCanvasManager.Instance.SetTimerText(Mathf.RoundToInt(currentTime).ToString());
        if (currentTime <= 0)
        {
            currentTime = 0;
            HandleMarioDeath();
        }
    }

    public void AddToScore(int amountToIncrease)
    {
        int newScore = score + amountToIncrease;
        score = newScore;
        GameCanvasManager.Instance.SetScoreText(newScore.ToString("D6"));
    }

    public void DisplayFloatingText(string displayText, Vector3 positionToSpawn)
    {
        Transform worldSpaceCanvasTransform = GameObject.Find("WorldCanvas").GetComponent<Transform>();
        GameObject textObj = Instantiate(floatingTextPrefab);
        textObj.transform.SetParent(worldSpaceCanvasTransform, false);
        textObj.transform.position = positionToSpawn + new Vector3(0.5f, 0, 0);
        textObj.GetComponent<TMP_Text>().text = displayText;
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

    public void SetLevelStarted(bool started)
    {
        levelStarted = started;
    }

    public int GetLives() { return lives; }

    public void SetWorld(string world)
    {
        this.world = world;
    }

    public string GetWorld() { return world; }

    public void GameOver()
    {
        if (!respawnCalled)
        {
            respawnCalled = true;
            if (score > highScore)
            {
                highScore = score;
            }
            levelStarted = false;
            GameCanvasManager.Instance.SetTimerText("");
            currentTime = 0;
            lives = 3;
            Loader.Load(Loader.Scene.GameOver);
        }
    }

    public void Victory()
    {
        levelStarted = false;
        lives = 3;
        if (score > highScore)
        {
            highScore = score;
        }
        Loader.Load(Loader.Scene.Victory);
    }

    public void HandleMarioDeath()
    {
        if (lives == 0)
        {
            GameOver();
        }
        else
        {
            Respawn();
        }
    }

    public int GetHighScore() { return highScore; }
}
