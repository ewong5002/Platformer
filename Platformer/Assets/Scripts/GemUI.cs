using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemUI : MonoBehaviour
{
	public Image gemPrefab;
	public Sprite emptyGemSprite;
	public Sprite collectedGemSprite;
	public float animationDuration = 0.5f;

	List<Image> m_gems = new List<Image>();

	public void ResetGems(int gems)
	{
		foreach (Image gem in m_gems)
		{
			Destroy(gem.gameObject);
		}

		m_gems.Clear();

		for (int i = 0; i < gems; i++)
		{
			Image newGem = Instantiate(gemPrefab, transform);
			newGem.sprite = emptyGemSprite;
			newGem.color = Color.white;
			m_gems.Add(newGem);
		}
	}

	public void UpdateGems(int currentGems)
	{
		for (int i = 0; i < m_gems.Count; i++)
		{
			if (i < currentGems)
			{
				if (i == currentGems - 1)
				{
					StartCoroutine(AnimateGemFill(m_gems[i]));
				}
				else
				{
					m_gems[i].sprite = collectedGemSprite;
				}
				m_gems[i].color = Color.yellow;
			}
			else
			{
				m_gems[i].sprite = emptyGemSprite;
				m_gems[i].color = Color.white;
			}
		}
	}
	
	IEnumerator AnimateGemFill(Image gem)
    {
        float elapsed = 0f;
		gem.sprite = collectedGemSprite;
        RectTransform rt = gem.rectTransform;

        Vector2 startScale = Vector2.one * 1.1f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;

            rt.localScale = Vector2.Lerp(startScale, Vector2.one, t * t * (3f - 2f * t));

            yield return null;
        }

        rt.localScale = Vector2.one;
    }
}
