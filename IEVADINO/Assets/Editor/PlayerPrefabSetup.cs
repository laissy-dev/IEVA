using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;

public static class PlayerPrefabSetup
{
    const string PlayerPrefabPath = "Assets/Prefabs/Player.prefab";
    const float SurvivorScale = 0.6f;

    public static void Configure()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);

        root.transform.localScale = Vector3.one * SurvivorScale;
        // "Sphere" porte les quatre membres.
        DestroyChild(root, "Sphere");
        DestroyChild(root, "Directional Light");
        TrailRenderer trail = root.GetComponentInChildren<TrailRenderer>();
        if (trail != null) trail.widthMultiplier = SurvivorScale * 0.5f;

        Rigidbody body = GetOrAdd<Rigidbody>(root);
        body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.Continuous;

        GetOrAdd<PlayerController>(root);
        GetOrAdd<PlayerColor>(root);
        GetOrAdd<PlayerHiding>(root);
        GetOrAdd<Interactor>(root);
        GetOrAdd<Survivor>(root);

        // Un objet mobile ne doit pas creuser de trou dans le NavMesh.
        GetOrAdd<NavMeshModifier>(root).ignoreFromBuild = true;

        PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
        PrefabUtility.UnloadPrefabContents(root);
    }

    static void DestroyChild(GameObject parent, string childName)
    {
        Transform child = parent.transform.Find(childName);
        if (child != null) Object.DestroyImmediate(child.gameObject);
    }

    static T GetOrAdd<T>(GameObject target) where T : Component
    {
        T component = target.GetComponent<T>();
        return component != null ? component : target.AddComponent<T>();
    }
}
