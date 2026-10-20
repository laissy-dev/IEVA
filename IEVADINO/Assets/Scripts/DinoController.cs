using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DinoController : MonoBehaviour
{
    public Transform Target;
    public float RepathInterval = 0.2f;

    private NavMeshAgent navMeshAgent;
    private Survivor targetSurvivor;
    private Coroutine chase;
    private Vector3 home;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        home = transform.position;
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

    // Cible perdue (cachée ou au refuge) : le dinosaure retourne à son point de départ.
    void HandleTargetVisibility(bool visible)
    {
        if (visible)
        {
            StartChase();
            return;
        }
        StopChase();
        if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh) navMeshAgent.SetDestination(home);
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
