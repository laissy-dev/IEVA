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

    private string endMessage = "";

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

        if (CollectedFragments >= Rules.FragmentsToWin)
        {
            OnVictory?.Invoke();
            EndGame("Victoire ! Antidote reconstitué");
        }
    }

    void HandleAllCaught()
    {
        EndGame("Défaite ! Tous les survivants ont été attrapés");
    }

    void EndGame(string message)
    {
        if (IsOver) return;
        IsOver = true;
        endMessage = message;
        Time.timeScale = 0.0f;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(20, 80, 500, 30), "Fragments d'antidote : " + CollectedFragments + " / " + Rules.FragmentsToWin);
        if (IsOver)
        {
            GUI.Label(new Rect(20, 110, 500, 30), endMessage);
        }
    }
}
