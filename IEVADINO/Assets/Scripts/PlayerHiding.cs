using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerColor))]
public class PlayerHiding : MonoBehaviour
{
    public float HideDuration = 20.0f;

    public bool IsHidden { get; private set; }
    public float TimeLeft { get; private set; }

    public event Action<bool> OnHiddenChanged;

    private PlayerColor playerColor;
    private readonly TriggerSet<ColorZone> zones = new TriggerSet<ColorZone>();
    private Coroutine countdown;

    void Awake()
    {
        playerColor = GetComponent<PlayerColor>();
    }

    void OnEnable()
    {
        playerColor.OnColorChanged += HandleColorChanged;
    }

    void OnDisable()
    {
        playerColor.OnColorChanged -= HandleColorChanged;
    }

    void HandleColorChanged(ColorData color)
    {
        TimeLeft = color != null ? HideDuration : 0.0f;
        RefreshHidden();
    }

    void OnTriggerEnter(Collider other)
    {
        ColorZone zone = other.GetComponentInParent<ColorZone>();
        if (zone != null && zones.Add(zone)) RefreshHidden();
    }

    void OnTriggerExit(Collider other)
    {
        ColorZone zone = other.GetComponentInParent<ColorZone>();
        if (zone != null && zones.Remove(zone)) RefreshHidden();
    }

    void RefreshHidden()
    {
        bool hidden = TimeLeft > 0.0f && IsOnZoneOfColor(playerColor.Current);
        if (hidden == IsHidden) return;

        IsHidden = hidden;
        if (hidden)
        {
            countdown = StartCoroutine(Countdown());
        }
        else if (countdown != null)
        {
            StopCoroutine(countdown);
            countdown = null;
        }
        OnHiddenChanged?.Invoke(hidden);
    }

    bool IsOnZoneOfColor(ColorData color)
    {
        if (color == null) return false;
        foreach (ColorZone zone in zones.Items)
        {
            if (zone.Data == color) return true;
        }
        return false;
    }

    IEnumerator Countdown()
    {
        while (TimeLeft > 0.0f)
        {
            yield return null;
            TimeLeft -= Time.deltaTime;
        }
        TimeLeft = 0.0f;
        countdown = null;
        playerColor.ApplyColor(null);
    }
}
