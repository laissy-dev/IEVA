using UnityEngine;

[RequireComponent(typeof(DinoController))]
public class DinoFollowActiveSurvivor : MonoBehaviour
{
    public SurvivorManager Survivors;

    private DinoController dino;

    void Awake()
    {
        dino = GetComponent<DinoController>();
    }

    void OnEnable()
    {
        if (Survivors == null) return;

        Survivors.OnActiveSurvivorChanged += HandleActiveSurvivorChanged;
        if (Survivors.Active != null) HandleActiveSurvivorChanged(Survivors.Active);
    }

    void OnDisable()
    {
        if (Survivors != null) Survivors.OnActiveSurvivorChanged -= HandleActiveSurvivorChanged;
    }

    void HandleActiveSurvivorChanged(Survivor survivor)
    {
        dino.SetTarget(survivor.transform);
    }
}
