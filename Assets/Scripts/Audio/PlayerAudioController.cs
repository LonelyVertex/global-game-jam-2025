using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    public AudioSource movement;
    public AudioSource pistol;
    public AudioSource shotgun;
    public AudioSource bazooka;
    public AudioSource dive;
    public AudioSource death;
    public AudioSource rise;
    public AudioSource collectiblePickup;
    public AudioSource duckSpawned;

    public void ShootPistol()
    {
        pistol.Play();
    }

    public void ShootShotgun()
    {
        shotgun.Play();
    }

    public void ShootBazooka()
    {
        bazooka.Play();
    }

    public void Movement()
    {
        if (movement.isPlaying)
        {
            return;
        }

        movement.Play();
    }

    public void Dive()
    {
        dive.Play();
    }

    public void Death()
    {
        death.Play();
    }

    public void Rise()
    {
        rise.Play();
    }

    public void CollectiblePickup()
    {
        collectiblePickup.Play();
    }

    public void DuckSpawned()
    {
        duckSpawned.Play();
    }
}
