using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SafeZone : MonoBehaviour
{
    private readonly TriggerSet<Survivor> inside = new TriggerSet<Survivor>();

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        Survivor survivor = other.GetComponentInParent<Survivor>();
        if (survivor != null && inside.Add(survivor)) survivor.SetSafe(true);
    }

    void OnTriggerExit(Collider other)
    {
        Survivor survivor = other.GetComponentInParent<Survivor>();
        if (survivor != null && inside.Remove(survivor)) survivor.SetSafe(false);
    }
}
