using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public Rigidbody2D rb;

    public float speed = 5f;

    public PlayerController playerController;

    public GameObject trail;
    public ParticleSystem particleSystem;

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

        trail.transform.SetParent(null);
        Destroy(trail, particleSystem.main.duration);

        Destroy(gameObject);
    }
}
