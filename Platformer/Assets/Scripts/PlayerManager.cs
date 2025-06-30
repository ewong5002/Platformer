using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public ParticleSystem smokeFX;
    bool m_isFacingRight = true;

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
    bool m_isGrounded = true;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask wallLayer;

    [Header("WallMovement")]
    public float wallSlideSpeed = 2f;
    bool m_isWallSliding = false;
    bool m_isWallJumping = false;
    float m_wallJumpDirection;
    float m_wallJumpTime = 0.5f;
    float m_wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        Gravity();
        WallSlide();
        WallJump();

        if (!m_isWallJumping)
        {
            rb.linearVelocity = new Vector2(m_horizontalMovement * moveSpeed, rb.linearVelocityY);
            Flip();
        }

        animator.SetFloat("yVelocity", rb.linearVelocityY);
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude);
        animator.SetBool("isWallSliding", m_isWallSliding);
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
                JumpFX();
            }
            else if (context.canceled)
            {
                // Half jump height
                rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.5f);
                m_jumpsRemaining--;
                JumpFX();
            }
        }

        // Wall Jump
        if (context.performed && m_wallJumpTimer > 0f)
        {
            m_isWallJumping = true;
            rb.linearVelocity = new Vector2(m_wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            m_wallJumpTimer = 0;
            JumpFX();

            // Force Flip
            if (transform.localScale.x != m_wallJumpDirection)
            {
                m_isFacingRight = !m_isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }

            Invoke(nameof(CancelWallJump), m_wallJumpTime + 0.1f);
        }
    }

    private void JumpFX()
    {
        animator.SetTrigger("jump");
        smokeFX.Play();
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            m_jumpsRemaining = maxJumps;
            m_isGrounded = true;
        }
        else
        {
            m_isGrounded = false;
        }
    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
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

    private void WallSlide()
    {
        if (!m_isGrounded && WallCheck() && m_horizontalMovement != 0)
        {
            m_isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, Mathf.Max(rb.linearVelocityY, -wallSlideSpeed));
        }
        else
        {
            m_isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (m_isWallSliding)
        {
            m_isWallJumping = false;
            m_wallJumpDirection = -transform.localScale.x;
            m_wallJumpTimer = m_wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (m_wallJumpTimer > 0f)
        {
            m_wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        m_isWallJumping = false;
    }

    private void Flip()
    {
        if (m_isFacingRight && m_horizontalMovement < 0 || !m_isFacingRight && m_horizontalMovement > 0)
        {
            m_isFacingRight = !m_isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;

            if (m_isGrounded)
            {
                smokeFX.Play();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
}
