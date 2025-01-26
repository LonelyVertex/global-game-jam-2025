using UnityEngine;

public class PlayOnHit : MonoBehaviour
{
    public PlayerAudioController audioController;

    private void OnCollisionEnter2D(Collision2D other)
    {
        audioController.ObstacleHit();
    }
}
