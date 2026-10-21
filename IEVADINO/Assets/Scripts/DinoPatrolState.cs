using UnityEngine;
using UnityEngine.AI;

public class DinoPatrolState : StateMachineBehaviour
{
    private DinoController dino;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        dino = animator.GetComponentInParent<DinoController>();
        dino.Agent.speed = dino.Settings.PatrolSpeed;
        if (dino.Agent.isOnNavMesh) dino.Agent.isStopped = false;
        GoToRandomPoint();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        NavMeshAgent agent = dino.Agent;
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
        {
            GoToRandomPoint();
        }
    }

    void GoToRandomPoint()
    {
        if (!dino.Agent.isOnNavMesh) return;

        float half = dino.Settings.PatrolHalfSize;
        Vector3 point = dino.Settings.PatrolCenter + new Vector3(Random.Range(-half, half), 0.0f, Random.Range(-half, half));
        if (NavMesh.SamplePosition(point, out NavMeshHit hit, 3.0f, NavMesh.AllAreas))
        {
            dino.Agent.SetDestination(hit.position);
        }
    }
}
