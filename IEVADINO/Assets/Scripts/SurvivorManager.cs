using System;
using UnityEngine;

public class SurvivorManager : MonoBehaviour
{
    public Survivor[] Survivors;
    public KeyCode SwitchKey = KeyCode.Tab;

    public Survivor Active { get; private set; }
    public int RemainingCount { get; private set; }

    public event Action<Survivor> OnActiveSurvivorChanged;
    public event Action<Survivor> OnSwitchedByPlayer;
    public event Action OnAllCaught;

    void OnEnable()
    {
        foreach (Survivor survivor in Survivors)
        {
            survivor.OnCaught += HandleCaught;
        }
    }

    void OnDisable()
    {
        foreach (Survivor survivor in Survivors)
        {
            survivor.OnCaught -= HandleCaught;
        }
    }

    void Start()
    {
        RemainingCount = Survivors.Length;
        foreach (Survivor survivor in Survivors)
        {
            survivor.SetControlled(false);
        }
        SelectNext();
    }

    void Update()
    {
        if (Input.GetKeyDown(SwitchKey) && SelectNext()) OnSwitchedByPlayer?.Invoke(Active);
    }

    bool SelectNext()
    {
        int current = Array.IndexOf(Survivors, Active);
        for (int step = 1; step <= Survivors.Length; step++)
        {
            Survivor candidate = Survivors[(current + step) % Survivors.Length];
            if (!candidate.IsCaught) return Select(candidate);
        }
        return false;
    }

    bool Select(Survivor survivor)
    {
        if (survivor == Active) return false;

        if (Active != null) Active.SetControlled(false);
        Active = survivor;
        Active.SetControlled(true);
        OnActiveSurvivorChanged?.Invoke(Active);
        return true;
    }

    void HandleCaught(Survivor survivor)
    {
        RemainingCount--;
        if (RemainingCount <= 0)
        {
            Active = null;
            Debug.Log("[Survivants] Les " + Survivors.Length + " survivants ont été attrapés.");
            OnAllCaught?.Invoke();
            return;
        }
        if (survivor == Active) SelectNext();
    }
}
