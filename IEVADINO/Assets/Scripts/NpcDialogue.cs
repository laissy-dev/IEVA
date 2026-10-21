using System;
using UnityEngine;

public class NpcDialogue : MonoBehaviour, IInteractable
{
    public DialogueData Dialogue;

    public bool IsOpen => speaker != null;

    public event Action<GameObject> OnAccepted;

    private GameObject speaker;
    private Interactor speakerInteractor;
    private PlayerController speakerController;
    private int questionIndex;

    public bool CanInteract(GameObject interactor) => speaker == null || speaker == interactor;

    public string GetPrompt(GameObject interactor) => IsOpen ? null : "Parler à " + Dialogue.SpeakerName;

    public void Interact(GameObject interactor)
    {
        if (!IsOpen) Open(interactor);
        else if (interactor == speaker) Answer();
    }

    void Open(GameObject interactor)
    {
        speaker = interactor;
        questionIndex = 0;

        speakerInteractor = interactor.GetComponent<Interactor>();
        if (speakerInteractor != null) speakerInteractor.OnDisabled += HandleSpeakerDisabled;

        speakerController = interactor.GetComponent<PlayerController>();
        if (speakerController != null) speakerController.enabled = false;

        if (Dialogue.Questions.Length == 0) Accept();
    }

    void Answer()
    {
        questionIndex++;
        if (questionIndex >= Dialogue.Questions.Length) Accept();
    }

    void Accept()
    {
        GameObject accepted = speaker;
        Close(true);
        OnAccepted?.Invoke(accepted);
    }

    void HandleSpeakerDisabled()
    {
        Close(false);
    }

    void Close(bool restoreControl)
    {
        if (speakerInteractor != null) speakerInteractor.OnDisabled -= HandleSpeakerDisabled;
        if (restoreControl && speakerController != null) speakerController.enabled = true;

        speaker = null;
        speakerInteractor = null;
        speakerController = null;
    }

    void OnGUI()
    {
        if (!IsOpen) return;

        Rect box = new Rect(Screen.width * 0.5f - 300.0f, Screen.height - 170.0f, 600.0f, 140.0f);
        GUI.Box(box, Dialogue.SpeakerName);
        GUI.Label(new Rect(box.x + 20.0f, box.y + 30.0f, box.width - 40.0f, 60.0f), Dialogue.Questions[questionIndex]);
        if (GUI.Button(new Rect(box.center.x - 60.0f, box.yMax - 45.0f, 120.0f, 30.0f), Dialogue.Answer))
        {
            Answer();
        }
    }
}
