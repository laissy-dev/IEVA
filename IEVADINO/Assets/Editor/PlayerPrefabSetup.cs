using UnityEditor;
using UnityEngine;

// Le survivant est assemblé par composition dans son prefab : les deux scènes en profitent.
public static class PlayerPrefabSetup
{
    const string PlayerPrefabPath = "Assets/Prefabs/Player.prefab";

    public static void Configure()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);

        Rigidbody body = GetOrAdd<Rigidbody>(root);
        body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.Continuous;

        GetOrAdd<PlayerController>(root);
        GetOrAdd<PlayerColor>(root);
        GetOrAdd<PlayerHiding>(root);
        GetOrAdd<Interactor>(root);

        PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
        PrefabUtility.UnloadPrefabContents(root);
    }

    static T GetOrAdd<T>(GameObject target) where T : Component
    {
        T component = target.GetComponent<T>();
        return component != null ? component : target.AddComponent<T>();
    }
}
