using UnityEngine;

[RequireComponent(typeof(ColorSource))]
public class ColorZone : MonoBehaviour
{
    private ColorSource colorSource;

    public ColorData Data => colorSource.Data;

    void Awake()
    {
        colorSource = GetComponent<ColorSource>();
    }
}
