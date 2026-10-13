using UnityEngine;

public class PlayerColor : MonoBehaviour
{
    public Color CurrentColor = Color.white;
    public string CurrentColorName = "";

    public void ApplyColor(Color color, string colorName)
    {
        CurrentColor = color;
        CurrentColorName = colorName;

        foreach (MeshRenderer part in GetComponentsInChildren<MeshRenderer>())
        {
            part.material.color = color;
        }

        Debug.Log(name + " a pris la couleur " + colorName);
    }
}
