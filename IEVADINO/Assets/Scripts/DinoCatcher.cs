using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SphereCollider))]
public class DinoCatcher : MonoBehaviour
{
    public float Reach = 0.6f;

    void Awake()
    {
        SphereCollider trigger = GetComponent<SphereCollider>();
        trigger.isTrigger = true;

        NavMeshAgent agent = GetComponentInParent<NavMeshAgent>();
        if (agent != null) trigger.radius = agent.radius + Reach;
    }

    void OnTriggerEnter(Collider other)
    {
        TryCatch(other);
    }

    // Un survivant caché au contact est attrapé dès que son camouflage prend fin.
    void OnTriggerStay(Collider other)
    {
        TryCatch(other);
    }

    void TryCatch(Collider other)
    {
        if (other.attachedRigidbody == null) return;

        ICatchable target = other.attachedRigidbody.GetComponent<ICatchable>();
        if (target != null && target.CanBeCaught) target.Catch();
    }
}
