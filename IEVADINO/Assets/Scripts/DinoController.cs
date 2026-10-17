using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DinoController : MonoBehaviour
{
    public Transform Target;

    private NavMeshAgent navMeshAgent;
    private PlayerHiding targetHiding;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (Target != null)
        {
            targetHiding = Target.GetComponent<PlayerHiding>();
        }
    }

    void Update()
    {
        if (Target != null)
        {
            if (targetHiding != null && targetHiding.IsHidden)
            {
                navMeshAgent.ResetPath();
            }
            else
            {
                navMeshAgent.SetDestination(Target.position);
            }
        }
    }
}
