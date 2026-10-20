using System;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public KeyCode InteractionKey = KeyCode.E;

    public event Action OnDisabled;

    private readonly TriggerSet<IInteractable> inRange = new TriggerSet<IInteractable>();
    private IInteractable current;

    void OnDisable()
    {
        OnDisabled?.Invoke();
    }

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

    void Update()
    {
        if (current != null && current.CanInteract(gameObject) && Input.GetKeyDown(InteractionKey))
        {
            current.Interact(gameObject);
        }
    }

    void OnGUI()
    {
        if (current == null) return;

        string prompt = current.GetPrompt(gameObject);
        if (prompt == null) return;

        string key = current.CanInteract(gameObject) ? "[" + InteractionKey + "] " : "";
        GUI.Label(new Rect(20, 20, 500, 30), key + prompt);
    }
}
