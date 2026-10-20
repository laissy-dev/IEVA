using UnityEngine;

[RequireComponent(typeof(ColorSource), typeof(NpcDialogue))]
public class ColorInteraction : MonoBehaviour
{
    private ColorSource colorSource;
    private NpcDialogue dialogue;

    void Awake()
    {
        colorSource = GetComponent<ColorSource>();
        dialogue = GetComponent<NpcDialogue>();
    }

    void OnEnable()
    {
        dialogue.OnAccepted += GiveColor;
    }

    void OnDisable()
    {
        dialogue.OnAccepted -= GiveColor;
    }

    void GiveColor(GameObject interactor)
    {
        PlayerColor playerColor = interactor.GetComponent<PlayerColor>();
        if (playerColor != null)
        {
            playerColor.ApplyColor(colorSource.Data);
        }
    }
}
