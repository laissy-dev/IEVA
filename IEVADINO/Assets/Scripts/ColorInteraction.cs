using UnityEngine;

[RequireComponent(typeof(ColorSource))]
public class ColorInteraction : MonoBehaviour
{
    public KeyCode InteractionKey = KeyCode.E;

    private ColorSource colorSource;
    private PlayerColor playerInRange;

    void Start()
    {
        colorSource = GetComponent<ColorSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = other.GetComponent<PlayerColor>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = null;
        }
    }

    void Update()
    {
        if (playerInRange != null && Input.GetKeyDown(InteractionKey))
        {
            Color color = colorSource.GetColor();
            playerInRange.ApplyColor(color, colorSource.ColorName);
        }
    }

    void OnGUI()
    {
        if (playerInRange == null) return;
        GUI.Label(new Rect(20, 20, 500, 30), "[" + InteractionKey + "] Prendre la couleur " + colorSource.ColorName);
    }
}
