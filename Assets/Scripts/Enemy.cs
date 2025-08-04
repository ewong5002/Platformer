using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int damage = 1;
    public float chaseSpeed = 2f;
    public float jumpHeight = 2f;
    public LayerMask groundLayer;

    Rigidbody2D m_rb;
    bool m_isGrounded;
    bool m_shouldJump;
    Transform m_player;

    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        m_isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);

        float direction = Mathf.Sign(m_player.position.x - transform.position.x);

        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 5f, 1 <<m_player.gameObject.layer);

        if (m_isGrounded)
        {
            m_rb.linearVelocity = new Vector2(direction * chaseSpeed, m_rb.linearVelocityY);

            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2f, groundLayer);
            RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, groundLayer);
            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 5f, groundLayer);

            if (!groundInFront.collider && !gapAhead.collider)
            {
                m_shouldJump = true;
            }
            else if (isPlayerAbove && platformAbove.collider)
            {
                m_shouldJump = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (m_isGrounded && m_shouldJump)
        {
            m_shouldJump = false;
            Vector2 direction = (m_player.position - transform.position).normalized;

            Vector2 jumpDirection = direction * jumpHeight;

            m_rb.AddForce(new Vector2(jumpDirection.x, jumpHeight), ForceMode2D.Impulse);
        }
    }

}
