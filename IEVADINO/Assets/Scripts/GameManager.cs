using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Antidote Antidote;

    public bool IsOver { get; private set; }

    private string endMessage = "";

    void OnEnable()
    {
        if (Antidote != null) Antidote.OnCollected += HandleAntidoteCollected;
    }

    void OnDisable()
    {
        if (Antidote != null) Antidote.OnCollected -= HandleAntidoteCollected;
    }

    void HandleAntidoteCollected()
    {
        EndGame("Victoire ! Antidote récupéré");
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
        if (!IsOver) return;
        GUI.Label(new Rect(20, 80, 500, 30), endMessage);
    }
}
