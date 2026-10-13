using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DinoController : MonoBehaviour
{
    public Transform Target;

    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Target != null)
        {
            navMeshAgent.SetDestination(Target.position);
        }
    }
}
