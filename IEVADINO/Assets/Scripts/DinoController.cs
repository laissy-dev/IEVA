using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DinoController : MonoBehaviour
{
    public DinoSettings Settings;
    public Transform Target;

    public NavMeshAgent Agent { get; private set; }
    public bool TargetVisible { get; private set; }

    private static readonly int TargetVisibleParameter = Animator.StringToHash("TargetVisible");
    private static readonly int AttackParameter = Animator.StringToHash("Attack");

    private Animator animator;
    private DinoCatcher catcher;
    private Survivor targetSurvivor;

    void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        catcher = GetComponentInChildren<DinoCatcher>();
    }

    void OnEnable()
    {
        WatchTarget();
        if (catcher != null) catcher.OnCatch += HandleCatch;
    }

    void OnDisable()
    {
        UnwatchTarget();
        if (catcher != null) catcher.OnCatch -= HandleCatch;
    }

    void Start()
    {
        animator.SetBool(TargetVisibleParameter, TargetVisible);
    }

    public void SetTarget(Transform newTarget)
    {
        if (newTarget == Target) return;

        if (isActiveAndEnabled) UnwatchTarget();
        Target = newTarget;
        if (isActiveAndEnabled) WatchTarget();
    }

    void WatchTarget()
    {
        if (Target == null)
        {
            HandleTargetVisibility(false);
            return;
        }

        targetSurvivor = Target.GetComponent<Survivor>();
        if (targetSurvivor != null)
        {
            targetSurvivor.OnVisibilityChanged += HandleTargetVisibility;
        }
        HandleTargetVisibility(targetSurvivor == null || targetSurvivor.IsVisible);
    }

    void UnwatchTarget()
    {
        if (targetSurvivor != null)
        {
            targetSurvivor.OnVisibilityChanged -= HandleTargetVisibility;
        }
        targetSurvivor = null;
    }

    void HandleTargetVisibility(bool visible)
    {
        TargetVisible = visible;
        if (animator != null && animator.isInitialized) animator.SetBool(TargetVisibleParameter, visible);
    }

    void HandleCatch()
    {
        animator.SetTrigger(AttackParameter);
    }
}
