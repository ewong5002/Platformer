using System;
using UnityEngine;

public class Gem : MonoBehaviour, IItem
{
    public static event Action OnGemCollect;

    [Header("Effects")]
    public ParticleSystem m_collectEffect;
    public AudioClip m_collectSound;

    public void Collect()
    {
        // GameManager.Instance.CollectGem();

        // if (m_collectEffect != null)
        // {
        //     Instantiate(m_collectEffect, transform.position, Quaternion.identity);
        // }

        // if (m_collectSound != null)
        // {
        //     AudioSource.PlayClipAtPoint(m_collectSound, transform.position);
        // }

        OnGemCollect.Invoke();
        Destroy(gameObject);
    }
}
