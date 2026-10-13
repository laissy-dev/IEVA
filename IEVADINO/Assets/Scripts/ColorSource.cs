using UnityEngine;

public class ColorSource : MonoBehaviour
{
    public string ColorName = "Rouge";

    public Color GetColor()
    {
        Renderer objectRenderer = GetComponentInChildren<Renderer>();
        Color color = objectRenderer.sharedMaterial.color;
        return color;
    }
}
