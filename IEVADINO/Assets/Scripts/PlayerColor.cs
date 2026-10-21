using System;
using UnityEngine;

public class PlayerColor : MonoBehaviour
{
    public Color NeutralColor = Color.white;

    public ColorData Current { get; private set; }

    public event Action<ColorData> OnColorChanged;

    public void ApplyColor(ColorData data)
    {
        Current = data;
        Color color = data != null ? data.Color : NeutralColor;

        foreach (MeshRenderer part in GetComponentsInChildren<MeshRenderer>())
        {
            part.material.color = color;
        }

        OnColorChanged?.Invoke(data);
    }
}
