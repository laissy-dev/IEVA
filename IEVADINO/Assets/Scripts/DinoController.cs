using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DinoController : MonoBehaviour
{
    public Transform Target;
    public float RepathInterval = 0.2f;

    private NavMeshAgent navMeshAgent;
    private PlayerHiding targetHiding;
    private Coroutine chase;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void OnEnable()
    {
        WatchTarget();
    }

    void OnDisable()
    {
        UnwatchTarget();
        StopChase();
    }

    public void SetTarget(Transform newTarget)
    {
        if (newTarget == Target) return;

        if (isActiveAndEnabled)
        {
            UnwatchTarget();
            StopChase();
        }
        Target = newTarget;
        if (isActiveAndEnabled) WatchTarget();
    }

    void WatchTarget()
    {
        if (Target == null) return;

        targetHiding = Target.GetComponent<PlayerHiding>();
        if (targetHiding != null)
        {
            targetHiding.OnHiddenChanged += HandleTargetHidden;
        }
        if (targetHiding == null || !targetHiding.IsHidden)
        {
            StartChase();
        }
    }

    void UnwatchTarget()
    {
        if (targetHiding != null)
        {
            targetHiding.OnHiddenChanged -= HandleTargetHidden;
        }
        targetHiding = null;
    }

    void HandleTargetHidden(bool hidden)
    {
        if (hidden) StopChase();
        else StartChase();
    }

    void StartChase()
    {
        if (chase == null) chase = StartCoroutine(Chase());
    }

    void StopChase()
    {
        if (chase != null)
        {
            StopCoroutine(chase);
            chase = null;
        }
        if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.ResetPath();
        }
    }

    IEnumerator Chase()
    {
        WaitForSeconds wait = new WaitForSeconds(RepathInterval);
        while (true)
        {
            if (navMeshAgent.isOnNavMesh) navMeshAgent.SetDestination(Target.position);
            yield return wait;
        }
    }
}
