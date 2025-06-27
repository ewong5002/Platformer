using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    [Header("Movement")]
    public float moveSpeed = 5f;
    float m_horizontalMovement;

    [Header("Jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 2;
    int m_jumpsRemaining;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(m_horizontalMovement * moveSpeed, rb.linearVelocityY);
        GroundCheck();
        Gravity();
    }

    public void Move(InputAction.CallbackContext context)
    {
        m_horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (m_jumpsRemaining > 0)
        {
            if (context.performed)
            {
                // Full jump height
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpPower);
                m_jumpsRemaining--;
            }
            else if (context.canceled)
            {
                // Half jump height
                rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.5f);
                m_jumpsRemaining--;
            }
        }
    }

    private void Gravity()
    {
        if (rb.linearVelocityY < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, Mathf.Max(rb.linearVelocityY, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            m_jumpsRemaining = maxJumps;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}
