using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    [Header("Progress")]
    public Slider progressSlider;
    public int levelCompleteRequirement = 200;

    private int progressAmount;
    public static int CurrentProgress { get; private set; }

    [Header("Timer")]
    public float levelTime = 60f;
    private float timeRemaining;
    private bool timerRunning = true;
    public TMP_Text timerText;

    [Header("Timer Warning")]
    public float warningThreshold = 10f;
    public Color normalTimerColor = Color.white;
    public Color warningTimerColor = Color.red;
    public float flashSpeed = 8f;

    [Header("Scene References")]
    public GameObject player;
    public GameObject LoadCanvas;
    public List<GameObject> levels;
    private int currentLevelIndex = 0;
    public GameObject gameOverScreen;

    public static event Action OnReset;

    void Start()
    {
        progressAmount = 0;
        CurrentProgress = 0;

        if (progressSlider != null)
            progressSlider.value = 0;

        timeRemaining = levelTime;
        timerRunning = true;
        UpdateTimerUI();

        Crystal.OnCrystalCollect += IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;

        if (LoadCanvas != null)
            LoadCanvas.SetActive(false);

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);
    }

    void Update()
    {
        HandleTimer();
    }

    void OnDestroy()
    {
        Crystal.OnCrystalCollect -= IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete -= LoadNextLevel;
    }

    void HandleTimer()
    {
        if (!timerRunning)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            UpdateTimerUI();
            GameOverScreen();
            return;
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";

        if (timeRemaining <= warningThreshold)
        {
            float pulse = Mathf.Abs(Mathf.Sin(Time.unscaledTime * flashSpeed));
            timerText.color = Color.Lerp(normalTimerColor, warningTimerColor, pulse);
        }
        else
        {
            timerText.color = normalTimerColor;
        }
    }

    void GameOverScreen()
    {
        timerRunning = false;

        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ResetGame()
    {
        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        LoadLevel(currentLevelIndex);
        Time.timeScale = 1f;
    }

    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        CurrentProgress = progressAmount;

        if (progressSlider != null)
            progressSlider.value = progressAmount;

        Debug.Log("Progress: " + progressAmount);

        if (progressAmount >= levelCompleteRequirement)
        {
            if (LoadCanvas != null)
                LoadCanvas.SetActive(true);

            timerRunning = false;
            Debug.Log("Level Complete!");
        }
    }

    void LoadLevel(int level)
    {
        if (LoadCanvas != null)
            LoadCanvas.SetActive(false);

        if (levels.Count > 0)
        {
            levels[currentLevelIndex].SetActive(false);
            levels[level].SetActive(true);
        }

        currentLevelIndex = level;

        if (player != null)
            player.transform.position = Vector3.zero;

        progressAmount = 0;
        CurrentProgress = 0;

        if (progressSlider != null)
            progressSlider.value = 0;

        timeRemaining = levelTime;
        timerRunning = true;
        UpdateTimerUI();

        OnReset?.Invoke();
    }

    void LoadNextLevel()
    {
        int nextLevelIndex = (currentLevelIndex == levels.Count - 1) ? 0 : currentLevelIndex + 1;
        LoadLevel(nextLevelIndex);
    }
}