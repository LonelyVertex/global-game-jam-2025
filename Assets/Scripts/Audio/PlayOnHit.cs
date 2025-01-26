using UnityEngine;

public class PlayOnHit : MonoBehaviour
{
    public AudioSource audioSource;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (audioSource.isPlaying)
        {
            return;
        }
        
        audioSource.Play();
    }
}
