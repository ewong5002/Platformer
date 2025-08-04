using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Sprites")]
    public Image heartPrefab;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    [Header("Settings")]
    public float animationDuration = 0.5f;

    List<Image> m_hearts = new List<Image>();
    int m_previousHealth;
    List<Coroutine> m_activeCoroutines = new List<Coroutine>();

    public void SetMaxHearts(int maxHearts)
    {
        StopAllActiveCoroutines();

        foreach (Image heart in m_hearts)
        {
            if (heart != null && heart.gameObject != null)
            {
                Destroy(heart.gameObject);
            }
        }

        m_hearts.Clear();

        for (int i = 0; i < maxHearts; i++)
        {
            Image newHeart = Instantiate(heartPrefab, transform);
            newHeart.sprite = fullHeartSprite;
            newHeart.color = Color.red;
            m_hearts.Add(newHeart);
        }

        m_previousHealth = maxHearts;
    }

    public void UpdateHearts(int currentHealth)
    {
        if (currentHealth > m_previousHealth)
        {
            if (currentHealth - 1 < m_hearts.Count && m_hearts[currentHealth - 1] != null)
            {
                var coroutine = StartCoroutine(AnimateHeartGain(m_hearts[currentHealth - 1]));
                m_activeCoroutines.Add(coroutine);
            }
        }
        else if (currentHealth < m_previousHealth)
        {
            if (currentHealth >= 0 && currentHealth < m_hearts.Count && m_hearts[currentHealth] != null)
            {
                var coroutine = StartCoroutine(AnimateHeartLoss(m_hearts[currentHealth]));
                m_activeCoroutines.Add(coroutine);
            }
        }

        for (int i = 0; i < m_hearts.Count; i++)
        {
            if (m_hearts[i] == null) continue;

            if (i < currentHealth)
            {
                m_hearts[i].sprite = fullHeartSprite;
                m_hearts[i].color = Color.red;
            }
            else
            {
                m_hearts[i].sprite = emptyHeartSprite;
                m_hearts[i].color = Color.white;
            }
        }

        m_previousHealth = currentHealth;
    }

    IEnumerator AnimateHeartGain(Image heart)
    {
        if (heart == null || heart.rectTransform == null) yield break;

        float elapsed = 0f;
        RectTransform rt = heart.rectTransform;
        Vector2 startScale = Vector2.one * 1.2f;

        while (elapsed < animationDuration)
        {
            if (heart == null || rt == null) yield break;

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            rt.localScale = Vector2.Lerp(startScale, Vector2.one, t);
            yield return null;
        }

        if (heart != null && rt != null)
        {
            rt.localScale = Vector2.one;
        }
    }

    IEnumerator AnimateHeartLoss(Image heart)
    {
        if (heart == null || heart.rectTransform == null) yield break;

        float elapsed = 0f;
        RectTransform rt = heart.rectTransform;
        Vector2 endScale = Vector2.one * 0.8f;

        while (elapsed < animationDuration)
        {
            if (heart == null || rt == null) yield break;

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            rt.localScale = Vector2.Lerp(Vector2.one, endScale, t);
            yield return null;
        }

        if (heart != null && rt != null)
        {
            rt.localScale = Vector2.one;
        }
    }

    void StopAllActiveCoroutines()
    {
        foreach (var coroutine in m_activeCoroutines)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        m_activeCoroutines.Clear();
    }

    public void ResetHealthUI()
    {
        StopAllActiveCoroutines();
    }
}