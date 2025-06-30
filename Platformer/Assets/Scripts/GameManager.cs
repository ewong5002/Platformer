using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject Player;
    public GameObject LoadCanvas;
    public List<GameObject> Levels;

    int m_currentLevelIndex = 0;
    int m_gemsCollected;

    [Header("GemUI")]
    public Image[] gemIcons;
    public Sprite emptyGemSprite;
    public Sprite collectedGemSprite;
    public float animationDuration = 0.5f;

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
    }

    void ResetLevel()
    {
        LoadCanvas.SetActive(false);
        m_gemsCollected = 0;

        foreach (Image gem in gemIcons)
        {
            gem.sprite = emptyGemSprite;
            gem.rectTransform.localScale = Vector2.one;
        }
    }

    void LoadNextLevel()
    {
        int nextLevelIndex = (m_currentLevelIndex == Levels.Count - 1) ? 0 : m_currentLevelIndex + 1;

        Levels[m_currentLevelIndex].gameObject.SetActive(false);
        Levels[nextLevelIndex].gameObject.SetActive(true);
        Player.transform.position = new Vector3(0, 0, 0);
        m_currentLevelIndex = nextLevelIndex;

        ResetLevel();
    }

    void GemCollect()
    {
        if (m_gemsCollected < 3)
        {
            StartCoroutine(AnimateGemFill(gemIcons[m_gemsCollected]));
            m_gemsCollected++;
        }
    }

    IEnumerator AnimateGemFill(Image gemImage)
    {
        float elapsed = 0f;
        gemImage.sprite = collectedGemSprite;
        RectTransform rectTransform = gemImage.rectTransform;

        Vector2 startScale = Vector2.one * 1.1f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;

            rectTransform.localScale = Vector2.Lerp(startScale, Vector2.one, t * t * (3f - 2f * t));

            yield return null;
        }

        rectTransform.localScale = Vector2.one;
    }
}
