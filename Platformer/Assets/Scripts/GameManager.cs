using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action OnReset;
    public GameObject Player;
    public GameObject LoadCanvas;
    public List<GameObject> Levels;

    public GemUI gemUI;

    public GameObject gameOverScreen;
    public TMP_Text survivedText;
    int m_survivedLevelsCount;

    int m_currentLevelIndex = 0;
    int m_gemsCollected;
    const int MAX_GEMS = 3;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        ResetLevel();
        Gem.OnGemCollect += GemCollect;
        LoadLevel.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayerDied += GameOverScreen;
    }

    void ResetLevel()
    {
        LoadCanvas.SetActive(false);
        gameOverScreen.SetActive(false);
        m_gemsCollected = 0;
        gemUI.ResetGems(MAX_GEMS);
    }

    public void ResetGame()
    {
        m_survivedLevelsCount = 0;

        var currentExits = Levels[m_currentLevelIndex].GetComponentsInChildren<Exit>();
        foreach (var exit in currentExits)
        {
            exit.ResetExit();
        }

        LoadNewLevel(0, false);
        OnReset.Invoke();
        Time.timeScale = 1;
    }

    void LoadNewLevel(int level, bool wantSurvivedIncreased)
    {
        Levels[m_currentLevelIndex].gameObject.SetActive(false);
        Levels[level].gameObject.SetActive(true);
        Player.transform.position = new Vector3(0, 0, 0);
        m_currentLevelIndex = level;

        if (wantSurvivedIncreased) m_survivedLevelsCount++;

        ResetLevel();
    }

    void LoadNextLevel()
    {
        int nextLevelIndex = (m_currentLevelIndex == Levels.Count - 1) ? 0 : m_currentLevelIndex + 1;

        LoadNewLevel(nextLevelIndex, true);
    }

    void GemCollect()
    {
        if (m_gemsCollected < MAX_GEMS)
        {
            m_gemsCollected++;
            gemUI.UpdateGems(m_gemsCollected);
        }
    }

    void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        survivedText.text = $"YOU SURVIVED {m_survivedLevelsCount} LEVEL";
        if (m_survivedLevelsCount != 1) survivedText.text += "S";
        Time.timeScale = 0;
    }
}
