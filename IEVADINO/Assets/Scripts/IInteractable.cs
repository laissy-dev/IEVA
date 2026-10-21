using UnityEngine;

public interface IInteractable
{
    bool CanInteract(GameObject interactor);

    string GetPrompt(GameObject interactor);

    void Interact(GameObject interactor);
}
