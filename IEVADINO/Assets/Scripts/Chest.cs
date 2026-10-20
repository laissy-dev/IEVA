using System;
using System.Collections;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public Transform Lid;
    public ColorData RequiredColor;
    public float OpenAngle = 110.0f;
    public float OpenDuration = 0.6f;

    public AntidoteFragment Content { get; set; }
    public bool IsOpen { get; private set; }

    public event Action<Chest> OnOpened;

    public bool CanInteract(GameObject interactor) => !IsOpen && HasRequiredColor(interactor);

    public string GetPrompt(GameObject interactor)
    {
        if (IsOpen) return null;
        if (HasRequiredColor(interactor)) return "Ouvrir le coffre";
        return "Coffre verrouillé : il faut la couleur " + RequiredColor.DisplayName;
    }

    bool HasRequiredColor(GameObject interactor)
    {
        if (RequiredColor == null) return true;

        PlayerColor playerColor = interactor.GetComponent<PlayerColor>();
        return playerColor != null && playerColor.Current == RequiredColor;
    }

    public void Interact(GameObject interactor)
    {
        if (!CanInteract(interactor)) return;

        IsOpen = true;
        OnOpened?.Invoke(this);
        StartCoroutine(OpenLid());
    }

    IEnumerator OpenLid()
    {
        Quaternion closed = Lid.localRotation;
        Quaternion open = closed * Quaternion.Euler(-OpenAngle, 0.0f, 0.0f);

        for (float time = 0.0f; time < OpenDuration; time += Time.deltaTime)
        {
            Lid.localRotation = Quaternion.Slerp(closed, open, time / OpenDuration);
            yield return null;
        }
        Lid.localRotation = open;

        if (Content != null)
        {
            Content.Reveal(transform.position + Vector3.up * 1.2f);
        }
    }
}
