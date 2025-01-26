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
            var collisionPlayerController = collision.gameObject.GetComponent<PlayerController>();
            if (!collisionPlayerController.killed)
            {
                //Check for self kill
                if (collisionPlayerController.gameObject == playerController.gameObject)
                {
                    playerController.ReduceScore();
                    collisionPlayerController.Kill();
                }
                else
                {
                    playerController.IncrementScore();
                    collisionPlayerController.Kill();
                }
            }
        }

        DestroyProjectile();
    }

    public void DestroyProjectile()
    {
        trail.transform.SetParent(null);
        Destroy(trail, particleSystem.main.startLifetime.constant);

        Destroy(gameObject);
    }
}
