using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour
{
    public static event Action OnHoldComplete;
    public float holdDuration = 1f;
    public Image fillCircle;

    float holdTimer = 0;
    bool m_isHolding = false;
    Exit m_exitTile;

    void Start()
    {
        m_exitTile = FindAnyObjectByType<Exit>();
        GameManager.OnReset += HandleGameReset;
    }

    void OnDestroy()
    {
        GameManager.OnReset -= HandleGameReset;
    }

    void Update()
    {
        if (!m_isHolding || m_exitTile == null) return;

        if (Exit.CanExit)
        {
            holdTimer += Time.deltaTime;
            fillCircle.fillAmount = holdTimer / holdDuration;
            
            if (holdTimer >= holdDuration)
            {
                if (m_exitTile != null)
                {
                    m_exitTile.ExitAnimation();
                    StartCoroutine(WaitForAnimation());
                }
                ResetHold();
            }
        }
        else if (m_isHolding)
        {
            ResetHold();
        }
    }

    public void OnHold(InputAction.CallbackContext context)
    {
        if (m_exitTile == null) return;

        if (context.started && Exit.CanExit)
        {
            m_isHolding = true;
        }
        else if (context.canceled)
        {
            ResetHold();
        }
    }

    void ResetHold()
    {
        m_isHolding = false;
        holdTimer = 0;
        fillCircle.fillAmount = 0;
    }

    IEnumerator WaitForAnimation()
    {
        yield return new WaitForSeconds(1f);
        OnHoldComplete?.Invoke();
    }

    void HandleGameReset()
    {
        m_exitTile = FindAnyObjectByType<Exit>();
        ResetHold();
    }
}