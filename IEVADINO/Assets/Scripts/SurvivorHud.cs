using UnityEngine;

[RequireComponent(typeof(SurvivorManager))]
public class SurvivorHud : MonoBehaviour
{
    private SurvivorManager manager;

    void Awake()
    {
        manager = GetComponent<SurvivorManager>();
    }

    void OnGUI()
    {
        Survivor active = manager.Active;
        if (active == null) return;

        PlayerHiding hiding = active.Hiding;
        string state = hiding.IsHidden ? "Caché (" + Mathf.CeilToInt(hiding.TimeLeft) + " s)" : "Visible";
        string text = active.name + " : " + state
            + "   |   Survivants : " + manager.RemainingCount + " / " + manager.Survivors.Length
            + "   [" + manager.SwitchKey + "] changer";
        GUI.Label(new Rect(20, 50, 700, 30), text);
    }
}
