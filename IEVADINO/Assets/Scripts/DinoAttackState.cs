using UnityEngine;

public class DinoAttackState : StateMachineBehaviour
{
    private static readonly int AttackParameter = Animator.StringToHash("Attack");

    private DinoController dino;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        dino = animator.GetComponentInParent<DinoController>();
        animator.ResetTrigger(AttackParameter);
        if (dino.Agent.isOnNavMesh) dino.Agent.isStopped = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (dino.Agent.isOnNavMesh) dino.Agent.isStopped = false;
    }
}
