using UnityEngine;

public class NodeVisual : MonoBehaviour
{
    private SpriteRenderer sr;

    public void Init(Color defaultColor)
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = defaultColor;
    }

    public void SetColor(Color color)
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        sr.color = color;
    }

    public void SetAsObstacle() => SetColor(Color.black);
    public void ResetVisual(Color originalColor) => SetColor(originalColor);
}
