using System;
using UnityEngine;

[RequireComponent(typeof(PlayerController), typeof(PlayerHiding))]
public class Survivor : MonoBehaviour, ICatchable
{
    public bool IsControlled { get; private set; } = true;
    public bool IsCaught { get; private set; }
    public PlayerHiding Hiding { get; private set; }

    public bool CanBeCaught => !IsCaught && !Hiding.IsHidden;

    public event Action<Survivor> OnCaught;

    private PlayerController controller;
    private Interactor interactor;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        interactor = GetComponent<Interactor>();
        Hiding = GetComponent<PlayerHiding>();
    }

    public void SetControlled(bool controlled)
    {
        IsControlled = controlled && !IsCaught;
        controller.enabled = IsControlled;
        if (interactor != null) interactor.enabled = IsControlled;
    }

    [ContextMenu("Attraper (test)")]
    public void Catch()
    {
        if (IsCaught || !Application.isPlaying) return;

        IsCaught = true;
        SetControlled(false);
        OnCaught?.Invoke(this);
        gameObject.SetActive(false);
    }
}
