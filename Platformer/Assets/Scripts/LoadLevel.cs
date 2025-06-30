using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour
{
    public static event Action OnHoldComplete;
    public float holdDuration = 1f; // How long to hold to load level
    public Image fillCircle;

    float holdTimer = 0;
    bool m_isHolding = false;
    public Exit exitTile;

    void Update()
    {
        if (m_isHolding && Exit.CanExit)
        {
            holdTimer += Time.deltaTime;
            fillCircle.fillAmount = holdTimer / holdDuration;
            if (holdTimer >= holdDuration)
            {
                exitTile.ExitAnimation();

                StartCoroutine(WaitForAnimation());
                ResetHold();
            }
        }
        else if (!Exit.CanExit && m_isHolding)
        {
            ResetHold();
        }
    }

    public void OnHold(InputAction.CallbackContext context)
    {
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
        OnHoldComplete.Invoke();
    }
}
