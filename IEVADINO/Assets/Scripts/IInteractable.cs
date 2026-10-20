using UnityEngine;

public interface IInteractable
{
    bool CanInteract(GameObject interactor);

    // null : rien à afficher pour cet interacteur.
    string GetPrompt(GameObject interactor);

    void Interact(GameObject interactor);
}
