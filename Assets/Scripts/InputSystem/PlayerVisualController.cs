using UnityEngine;

public class PlayerVisualController : MonoBehaviour
{
    public Sprite[] normalSprites;
    public Sprite[] underwaterSprites;

    [Space]
    public SpriteRenderer normalSpriteRenderer;
    public SpriteRenderer underwaterSpriteRenderer;

    [Space]
    public ParticleSystem particleSystem;

    public void SetVisuals(int idx)
    {
        normalSpriteRenderer.sprite = normalSprites[idx];
        underwaterSpriteRenderer.sprite = underwaterSprites[idx];
    }

    public void SetType(bool underwater)
    {

        if (underwater)
        {
            particleSystem.Stop();
        }
        else
        {
            particleSystem.Play();
        }

        normalSpriteRenderer.gameObject.SetActive(!underwater);
        underwaterSpriteRenderer.gameObject.SetActive(underwater);
    }
}
