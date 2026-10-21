using UnityEngine;

public class DinoChaseState : StateMachineBehaviour
{
    private DinoController dino;
    private float nextRepath;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        dino = animator.GetComponentInParent<DinoController>();
        dino.Agent.speed = dino.Settings.ChaseSpeed;
        if (dino.Agent.isOnNavMesh) dino.Agent.isStopped = false;
        nextRepath = 0.0f;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Time.time < nextRepath || dino.Target == null || !dino.Agent.isOnNavMesh) return;

        dino.Agent.SetDestination(dino.Target.position);
        nextRepath = Time.time + dino.Settings.RepathInterval;
    }
}
