using UnityEngine;

public class RocketController : ProjectileController
{
    public GameObject explosionPrefab;
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.isTrigger) return;
        
        SpawnExplosion();
        
        trail.transform.SetParent(null);
        Destroy(trail, particleSystem.main.duration);

        Destroy(gameObject);
    }

    void SpawnExplosion()
    {
        var explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.GetComponent<RocketExplosionController>().playerController = playerController;
    }
}
