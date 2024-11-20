using UnityEngine;

public class ColorImage : MonoBehaviour {

    public SpriteRenderer spriteRenderer;

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void SetOrderInLayer(int layer)
    {
        spriteRenderer.sortingOrder = layer;
    }
}