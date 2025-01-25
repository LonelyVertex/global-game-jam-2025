using UnityEngine;

public class PlayerVisualController : MonoBehaviour
{
    public Sprite[] normalSprites;
    public Sprite[] underwaterSprites;

    [Space]
    public SpriteRenderer normalSpriteRenderer;
    public SpriteRenderer underwaterSpriteRenderer;

    public void SetVisuals(int idx)
    {
        normalSpriteRenderer.sprite = normalSprites[idx];
        underwaterSpriteRenderer.sprite = underwaterSprites[idx];
    }

    public void SetType(bool underwater)
    {
        normalSpriteRenderer.gameObject.SetActive(!underwater);
        underwaterSpriteRenderer.gameObject.SetActive(underwater);
    }
}
