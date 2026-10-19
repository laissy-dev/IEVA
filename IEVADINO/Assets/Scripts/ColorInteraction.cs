using UnityEngine;

[RequireComponent(typeof(ColorSource))]
public class ColorInteraction : MonoBehaviour, IInteractable
{
    private ColorSource colorSource;

    public string Prompt => "Prendre la couleur " + colorSource.Data.DisplayName;

    void Awake()
    {
        colorSource = GetComponent<ColorSource>();
    }

    public void Interact(GameObject interactor)
    {
        PlayerColor playerColor = interactor.GetComponent<PlayerColor>();
        if (playerColor != null)
        {
            playerColor.ApplyColor(colorSource.Data);
        }
    }
}
