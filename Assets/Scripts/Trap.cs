using UnityEngine;

public class Trap : MonoBehaviour
{
    public float bounceForce = 10f;
    public int damage = 1;
    public Animator animator;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerBounce(collision.gameObject);
        }
    }

    void HandlePlayerBounce(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Trap trap = GetComponent<Trap>();

        if (rb)
        {
            if (trap.CompareTag("Spring"))
            {
                animator.SetTrigger("Bounce");
                SoundEffectManager.Play("Bounce");
            }

            rb.linearVelocity = new Vector2(rb.linearVelocityX, 0);

            rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
        }
    }
}
