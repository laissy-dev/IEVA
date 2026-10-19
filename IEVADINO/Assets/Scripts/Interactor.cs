using UnityEngine;

public class Interactor : MonoBehaviour
{
    public KeyCode InteractionKey = KeyCode.E;

    private readonly TriggerSet<IInteractable> inRange = new TriggerSet<IInteractable>();
    private IInteractable current;

    void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();
        if (interactable != null && inRange.Add(interactable))
        {
            current = interactable;
        }
    }

    void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();
        if (interactable != null && inRange.Remove(interactable) && current == interactable)
        {
            current = null;
            foreach (IInteractable remaining in inRange.Items) current = remaining;
        }
    }

    // Lire une touche impose de tester chaque frame : Update ne sert qu'à ça ici.
    void Update()
    {
        if (current != null && Input.GetKeyDown(InteractionKey))
        {
            current.Interact(gameObject);
        }
    }

    void OnGUI()
    {
        if (current == null) return;
        GUI.Label(new Rect(20, 20, 500, 30), "[" + InteractionKey + "] " + current.Prompt);
    }
}
