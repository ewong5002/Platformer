using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static event Action OnPlayerDied;
    public int maxHealth = 3;
    int m_currentHealth;

    public HealthUI healthUI;

    SpriteRenderer m_spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetHealth();

        m_spriteRenderer = GetComponent<SpriteRenderer>();
        GameManager.OnReset += ResetHealth;
        HealthItem.OnHealthCollect += Heal;
    }

    void ResetHealth()
    {
        m_currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy)
        {
            TakeDamage(enemy.damage);
        }
    }

    void Heal(int amount)
    {
        m_currentHealth += amount;
        if (m_currentHealth > maxHealth)
        {
            m_currentHealth = maxHealth;
        }

        healthUI.UpdateHearts(m_currentHealth);
    }

    void TakeDamage(int damage)
    {
        m_currentHealth -= damage;
        healthUI.UpdateHearts(m_currentHealth);

        StartCoroutine(FlashRed());
        if (m_currentHealth <= 0)
        {
            OnPlayerDied.Invoke();
        }
    }

    IEnumerator FlashRed()
    {
        m_spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        m_spriteRenderer.color = Color.white;
    }
}
