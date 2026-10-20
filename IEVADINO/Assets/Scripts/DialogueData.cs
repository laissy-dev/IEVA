using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "IEVA/Dialogue")]
public class DialogueData : ScriptableObject
{
    public string SpeakerName = "Peintre";
    [TextArea] public string[] Questions = { "Tu veux de l'aide ?" };
    public string Answer = "Oui";
}
