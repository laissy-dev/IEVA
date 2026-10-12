using UnityEngine;
using UnityEngine.AI;

// Based on SingleAgentController from the TP: instead of going where we click,
// the dinosaur goes towards the player on the NavMesh.
[RequireComponent(typeof(NavMeshAgent))]
public class DinoController : MonoBehaviour
{
    // The player to chase
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
