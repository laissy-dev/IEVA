using System;
using UnityEngine;

[RequireComponent(typeof(PlayerController), typeof(PlayerHiding))]
public class Survivor : MonoBehaviour, ICatchable
{
    public bool IsControlled { get; private set; } = true;
    public bool IsCaught { get; private set; }
    public bool IsSafe { get; private set; }
    public bool IsVisible { get; private set; } = true;
    public PlayerHiding Hiding { get; private set; }

    public bool CanBeCaught => IsVisible;

    public event Action<Survivor> OnCaught;
    public event Action<bool> OnVisibilityChanged;

    private PlayerController controller;
    private Interactor interactor;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        interactor = GetComponent<Interactor>();
        Hiding = GetComponent<PlayerHiding>();
    }

    void OnEnable()
    {
        Hiding.OnHiddenChanged += HandleHiddenChanged;
    }

    void OnDisable()
    {
        Hiding.OnHiddenChanged -= HandleHiddenChanged;
    }

    public void SetControlled(bool controlled)
    {
        IsControlled = controlled && !IsCaught;
        controller.enabled = IsControlled;
        if (interactor != null) interactor.enabled = IsControlled;
    }

    public void SetSafe(bool safe)
    {
        IsSafe = safe;
        RefreshVisibility();
    }

    [ContextMenu("Attraper (test)")]
    public void Catch()
    {
        if (IsCaught || !Application.isPlaying) return;

        IsCaught = true;
        SetControlled(false);
        RefreshVisibility();
        OnCaught?.Invoke(this);
        gameObject.SetActive(false);
    }

    void HandleHiddenChanged(bool hidden)
    {
        RefreshVisibility();
    }

    void RefreshVisibility()
    {
        bool visible = !IsCaught && !IsSafe && !Hiding.IsHidden;
        if (visible == IsVisible) return;

        IsVisible = visible;
        OnVisibilityChanged?.Invoke(visible);
    }
}
