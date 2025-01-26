using System.Collections.Generic;
using UnityEngine;

public class RocketExplosionController : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject explosionDetergent;
    public CircleCollider2D circleCollider;
    public AnimationCurve colliderRadius;
    public float destroyAfter;
    public float maxColliderRadius;
    public float force;
    public AudioSource audioSource;

    float _startTime;
    HashSet<Rigidbody2D> _rigidbodies = new();

    void Start()
    {
        _startTime = Time.time;
        audioSource.Play();

        Instantiate(explosionDetergent, transform.position, Quaternion.identity);

        Destroy(gameObject, destroyAfter);
    }

    void Update()
    {
        var t = (Time.time - _startTime) / destroyAfter;
        var radius = colliderRadius.Evaluate(t) * maxColliderRadius;
        circleCollider.radius = radius;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var collisionPlayerController = other.gameObject.GetComponentInParent<PlayerController>();
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

        var rb = other.gameObject.GetComponentInParent<Rigidbody2D>();

        if (!rb || _rigidbodies.Contains(rb)) return;

        var closestPoint = other.ClosestPoint(transform.position);
        var direction = (closestPoint - (Vector2)transform.position).normalized;
        rb.AddForceAtPosition(closestPoint, force * direction);
        rb.AddForce(force * direction, ForceMode2D.Impulse);

        _rigidbodies.Add(rb);
    }
}
