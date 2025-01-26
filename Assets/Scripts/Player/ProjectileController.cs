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
            //Check for self kill
            if (collision.gameObject.GetComponentInParent<PlayerController>().gameObject == playerController.gameObject)  {
                playerController.ReduceScore();
                collision.gameObject.GetComponent<PlayerController>().Kill();
            } else {
                playerController.IncrementScore();
                collision.gameObject.GetComponent<PlayerController>().Kill();
            }
        }

        trail.transform.SetParent(null);
        Destroy(trail, particleSystem.main.startLifetime.constant);

        Destroy(gameObject);
    }
}
