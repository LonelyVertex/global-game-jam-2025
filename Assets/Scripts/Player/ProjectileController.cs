using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public Rigidbody2D rb;

    public float speed = 5f;

    public PlayerController playerController;

    void Start()
    {
        rb.AddForce(rb.transform.up * speed, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerController.IncremeantScore();
        }
        Destroy(gameObject);
    }
}
