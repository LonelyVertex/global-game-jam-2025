using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    public AudioSource movement;
    public AudioSource pistol;
    public AudioSource shotgun;
    public AudioSource bazooka;
    public AudioSource nextWeapon;
    public AudioSource dive;
    public AudioSource death;
    public AudioSource rise;
    public AudioSource collectiblePickup;
    public AudioSource obstacleHit;

    public void ShootPistol() {}

    public void ShootShotgun() { }

    public void ShootBazooka() {}

    public void SetNextWeapon() { }

    public void Movement()
    {
        if (movement.isPlaying)
        {
            return;
        }

        movement.Play();
    }

    public void Dive() { }

    public void Death() { }

    public void Rise() { }

    public void CollectiblePickup() { }

    public void ObstacleHit() { }
}
