using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public Rigidbody2D rb;

    public float speed = 5f;

    void Start()
    {
        rb.AddForce(rb.transform.up * speed, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
