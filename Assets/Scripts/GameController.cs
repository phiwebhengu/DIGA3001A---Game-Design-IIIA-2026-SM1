using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    int progressAmount;
    public Slider progressSlider;

    public static int CurrentProgress { get; private set; }

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
        progressSlider.value = 0;

        Crystal.OnCrystalCollect += IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayerDied += GameOverScreen;

        LoadCanvas.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResetGame()
    {
        gameOverScreen.SetActive(false);
        LoadLevel(0);
        OnReset?.Invoke();
        Time.timeScale = 1f;
    }

    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        CurrentProgress = progressAmount;

        progressSlider.value = progressAmount;

        if (progressAmount >= 200)
        {
            LoadCanvas.SetActive(true);
        }
    }

    void LoadLevel(int level)
    {
        LoadCanvas.SetActive(false);

        levels[currentLevelIndex].SetActive(false);
        levels[level].SetActive(true);

        player.transform.position = Vector3.zero;

        currentLevelIndex = level;

        progressAmount = 0;
        CurrentProgress = 0;
        progressSlider.value = 0;
    }

    void LoadNextLevel()
    {
        int nextLevelIndex = (currentLevelIndex == levels.Count - 1) ? 0 : currentLevelIndex + 1;
        LoadLevel(nextLevelIndex);
    }
}