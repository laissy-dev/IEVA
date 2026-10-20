using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public SurvivorManager Survivors;
    public Vector3 Offset = new Vector3(0.0f, 16.0f, -14.0f);
    public float SmoothTime = 0.25f;

    private Transform target;
    private Vector3 velocity;

    void OnEnable()
    {
        if (Survivors == null) return;

        Survivors.OnActiveSurvivorChanged += Follow;
        if (Survivors.Active != null) Follow(Survivors.Active);
    }

    void OnDisable()
    {
        if (Survivors != null) Survivors.OnActiveSurvivorChanged -= Follow;
    }

    void Follow(Survivor survivor)
    {
        target = survivor.transform;
    }

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = Vector3.SmoothDamp(transform.position, target.position + Offset, ref velocity, SmoothTime);
    }
}
