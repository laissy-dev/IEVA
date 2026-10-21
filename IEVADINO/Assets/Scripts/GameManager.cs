using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameRules Rules;
    public AntidoteFragment[] Fragments;
    public SurvivorManager Survivors;

    public int CollectedFragments { get; private set; }
    public bool IsOver { get; private set; }

    public event Action<int> OnFragmentCollected;
    public event Action OnVictory;
    public event Action OnDefeat;

    void OnEnable()
    {
        foreach (AntidoteFragment fragment in Fragments)
        {
            fragment.OnCollected += HandleFragmentCollected;
        }
        if (Survivors != null) Survivors.OnAllCaught += HandleAllCaught;
    }

    void OnDisable()
    {
        foreach (AntidoteFragment fragment in Fragments)
        {
            fragment.OnCollected -= HandleFragmentCollected;
        }
        if (Survivors != null) Survivors.OnAllCaught -= HandleAllCaught;
    }

    void HandleFragmentCollected(AntidoteFragment fragment)
    {
        CollectedFragments++;
        OnFragmentCollected?.Invoke(CollectedFragments);

        if (CollectedFragments >= Rules.FragmentsToWin && !IsOver)
        {
            EndGame();
            OnVictory?.Invoke();
        }
    }

    void HandleAllCaught()
    {
        if (IsOver) return;

        EndGame();
        OnDefeat?.Invoke();
    }

    void EndGame()
    {
        IsOver = true;
        Time.timeScale = 0.0f;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(20, 80, 500, 30), "Fragments d'antidote : " + CollectedFragments + " / " + Rules.FragmentsToWin);
    }
}
