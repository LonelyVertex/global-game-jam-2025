using System.Collections.Generic;
using UnityEngine;

public class RocketExplosionController : MonoBehaviour
{
    public PlayerController playerController;
    public CircleCollider2D circleCollider;
    public AnimationCurve colliderRadius;
    public float destroyAfter;
    public float maxColliderRadius;
    public float force;

    float _startTime;
    HashSet<Rigidbody2D> _rigidbodies = new();
    
    void Start()
    {
        _startTime = Time.time;
        
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
            playerController.IncremeantScore();
            
            // TODO kill player
        }
        
        var rb = other.gameObject.GetComponentInParent<Rigidbody2D>();
        
        if (!rb || _rigidbodies.Contains(rb)) return;
        
        var closestPoint = other.ClosestPoint(transform.position);
        var direction = (closestPoint - (Vector2) transform.position).normalized;
        rb.AddForceAtPosition(closestPoint, force * direction);
        rb.AddForce(force * direction, ForceMode2D.Impulse);
        
        _rigidbodies.Add(rb);
    }
}
