using System.Collections.Generic;
using System.IO;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AlexandreMapBuilder
{
    const string AlexandreScenePath = "Assets/Scenes/Alexandre.unity";
    const string AlexandreSceneFolder = "Assets/Scenes/Alexandre";
    const string MapMaterialsFolder = "Assets/Materials/Map";
    const string GameRulesPath = "Assets/Data/GameRules.asset";
    const string ChestPrefabPath = "Assets/Prefabs/Coffre.prefab";
    const string FragmentPrefabPath = "Assets/Prefabs/FragmentAntidote.prefab";
    const string MapRootName = "Map";

    const float ArenaHalfSize = 15.0f;
    const float ChestHeight = 0.05f;

    static readonly Color Pierre = new Color(0.45f, 0.43f, 0.40f);
    static readonly Color Rocher = new Color(0.35f, 0.32f, 0.28f);
    static readonly Color Bois = new Color(0.45f, 0.28f, 0.12f);
    static readonly Color Metal = new Color(0.75f, 0.65f, 0.30f);
    static readonly Color Fragment = new Color(0.20f, 1.00f, 0.60f);

    [MenuItem("Alexandre/Construire ma scène")]
    public static void BuildAlexandreScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        PlayerPrefabSetup.Configure();
        GameRules rules = GetOrCreateGameRules();

        Scene scene = EditorSceneManager.OpenScene(AlexandreScenePath, OpenSceneMode.Single);
        DestroyRootObject(scene, MapRootName);
        DestroyRootObject(scene, "Antidote");

        Transform map = new GameObject(MapRootName).transform;
        BuildWalls(CreateGroup("Murs", map));
        BuildObstacles(CreateGroup("Obstacles", map));
        List<Chest> chests = BuildChests(CreateGroup("Coffres", map));
        List<AntidoteFragment> fragments = BuildFragments(CreateGroup("Fragments", map), rules.FragmentsToWin);

        ChestLoot loot = map.gameObject.AddComponent<ChestLoot>();
        loot.Chests = chests.ToArray();
        loot.Fragments = fragments.ToArray();

        GameManager manager = Object.FindFirstObjectByType<GameManager>();
        if (manager == null)
        {
            manager = new GameObject("GameManager").AddComponent<GameManager>();
        }
        manager.Rules = rules;
        manager.Fragments = fragments.ToArray();

        IgnoreFromNavMesh(FindRootObject(scene, "Dino"));
        BakeNavMesh(scene);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[Alexandre] Scène construite : " + AlexandreScenePath);
    }

    static void BuildWalls(Transform parent)
    {
        float length = ArenaHalfSize * 2.0f + 1.0f;
        float offset = ArenaHalfSize + 0.25f;
        CreateBlock("Mur_Nord", parent, new Vector3(0.0f, 0.75f, offset), new Vector3(length, 1.5f, 0.5f), 0.0f, Pierre);
        CreateBlock("Mur_Sud", parent, new Vector3(0.0f, 0.75f, -offset), new Vector3(length, 1.5f, 0.5f), 0.0f, Pierre);
        CreateBlock("Mur_Est", parent, new Vector3(offset, 0.75f, 0.0f), new Vector3(0.5f, 1.5f, length), 0.0f, Pierre);
        CreateBlock("Mur_Ouest", parent, new Vector3(-offset, 0.75f, 0.0f), new Vector3(0.5f, 1.5f, length), 0.0f, Pierre);
    }

    // Placés dans les espaces libres entre les zones de couleur et les PNJ des coins.
    static void BuildObstacles(Transform parent)
    {
        CreateBlock("Rocher_Centre", parent, new Vector3(-0.3f, 0.75f, 1.3f), new Vector3(2.0f, 1.5f, 2.0f), 15.0f, Rocher);
        CreateBlock("Rocher_Ouest", parent, new Vector3(-13.0f, 0.6f, 0.5f), new Vector3(1.5f, 1.2f, 2.5f), -10.0f, Rocher);
        CreateBlock("Rocher_Est", parent, new Vector3(13.0f, 0.6f, 1.5f), new Vector3(1.5f, 1.2f, 2.5f), 10.0f, Rocher);

        CreateBlock("Muret_Nord", parent, new Vector3(0.0f, 0.6f, 11.25f), new Vector3(12.0f, 1.2f, 0.5f), 0.0f, Pierre);
        CreateBlock("Muret_Sud", parent, new Vector3(0.0f, 0.6f, -11.25f), new Vector3(12.0f, 1.2f, 0.5f), 0.0f, Pierre);
    }

    static List<Chest> BuildChests(Transform parent)
    {
        GameObject prefab = GetOrCreatePrefab(ChestPrefabPath, CreateChestModel);
        List<Chest> chests = new List<Chest>();

        for (int i = 0; i < ArenaLayout.Zones.Length; i++)
        {
            ArenaLayout.ZoneSpot spot = ArenaLayout.Zones[i];
            GameObject chest = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            chest.name = "Coffre_" + (i + 1) + "_" + spot.ColorName;
            chest.transform.position = new Vector3(spot.Center.x, ChestHeight, spot.Center.z);
            chest.transform.LookAt(new Vector3(0.0f, ChestHeight, 0.0f));
            chests.Add(chest.GetComponent<Chest>());
        }
        return chests;
    }

    static List<AntidoteFragment> BuildFragments(Transform parent, int count)
    {
        GameObject prefab = GetOrCreatePrefab(FragmentPrefabPath, CreateFragmentModel);
        List<AntidoteFragment> fragments = new List<AntidoteFragment>();

        for (int i = 0; i < count; i++)
        {
            GameObject fragment = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            fragment.name = "Fragment_" + (i + 1);
            fragment.SetActive(false);
            fragments.Add(fragment.GetComponent<AntidoteFragment>());
        }
        return fragments;
    }

    // Le modèle n'est généré qu'une fois : ensuite le prefab se modifie dans Unity et n'est plus écrasé.
    static GameObject GetOrCreatePrefab(string path, System.Func<GameObject> createModel)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab != null) return prefab;

        EnsureFolder(Path.GetDirectoryName(path).Replace('\\', '/'));
        GameObject model = createModel();
        prefab = PrefabUtility.SaveAsPrefabAsset(model, path);
        Object.DestroyImmediate(model);
        return prefab;
    }

    static GameObject CreateChestModel()
    {
        GameObject chest = new GameObject("Coffre");

        CreatePart("Caisse", chest.transform, new Vector3(0.0f, 0.3f, 0.0f), new Vector3(1.0f, 0.6f, 0.7f), Bois);
        CreatePart("Serrure", chest.transform, new Vector3(0.0f, 0.5f, 0.36f), new Vector3(0.15f, 0.2f, 0.04f), Metal);

        // Pivot sur l'arête arrière du couvercle pour qu'il bascule en s'ouvrant.
        Transform lidPivot = new GameObject("Charniere").transform;
        lidPivot.SetParent(chest.transform, false);
        lidPivot.localPosition = new Vector3(0.0f, 0.6f, -0.35f);
        CreatePart("Couvercle", lidPivot, new Vector3(0.0f, 0.1f, 0.35f), new Vector3(1.05f, 0.2f, 0.75f), Bois);

        BoxCollider body = chest.AddComponent<BoxCollider>();
        body.center = new Vector3(0.0f, 0.4f, 0.0f);
        body.size = new Vector3(1.05f, 0.8f, 0.75f);

        SphereCollider interactionZone = chest.AddComponent<SphereCollider>();
        interactionZone.isTrigger = true;
        interactionZone.center = new Vector3(0.0f, 0.5f, 0.0f);
        interactionZone.radius = 1.6f;

        chest.AddComponent<Chest>().Lid = lidPivot;
        return chest;
    }

    static GameObject CreateFragmentModel()
    {
        GameObject fragment = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        fragment.name = "FragmentAntidote";
        fragment.transform.localScale = Vector3.one * 0.4f;
        fragment.GetComponent<MeshRenderer>().sharedMaterial = GetOrCreateMaterial("Fragment", Fragment, true);

        Rigidbody body = fragment.AddComponent<Rigidbody>();
        body.mass = 0.2f;
        body.collisionDetectionMode = CollisionDetectionMode.Continuous;

        SphereCollider pickup = fragment.AddComponent<SphereCollider>();
        pickup.isTrigger = true;
        pickup.radius = 2.0f;

        fragment.AddComponent<AntidoteFragment>().PickupTrigger = pickup;
        return fragment;
    }

    static GameObject CreateBlock(string name, Transform parent, Vector3 position, Vector3 size, float yaw, Color color)
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = name;
        block.transform.SetParent(parent, false);
        block.transform.position = position;
        block.transform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
        block.transform.localScale = size;
        block.GetComponent<MeshRenderer>().sharedMaterial = GetOrCreateMaterial(name.Split('_')[0], color, false);
        GameObjectUtility.SetStaticEditorFlags(block, StaticEditorFlags.BatchingStatic);
        return block;
    }

    // Pièce visuelle sans collider : la collision est portée par l'objet parent.
    static void CreatePart(string name, Transform parent, Vector3 localPosition, Vector3 size, Color color)
    {
        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
        part.name = name;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = localPosition;
        part.transform.localScale = size;
        Object.DestroyImmediate(part.GetComponent<Collider>());
        part.GetComponent<MeshRenderer>().sharedMaterial = GetOrCreateMaterial(name, color, false);
    }

    static Material GetOrCreateMaterial(string materialName, Color color, bool emissive)
    {
        string path = MapMaterialsFolder + "/" + materialName + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null) return material;

        EnsureFolder(MapMaterialsFolder);
        material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.color = color;
        if (emissive)
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * 2.0f);
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    static GameRules GetOrCreateGameRules()
    {
        GameRules rules = AssetDatabase.LoadAssetAtPath<GameRules>(GameRulesPath);
        if (rules != null) return rules;

        EnsureFolder(Path.GetDirectoryName(GameRulesPath).Replace('\\', '/'));
        rules = ScriptableObject.CreateInstance<GameRules>();
        AssetDatabase.CreateAsset(rules, GameRulesPath);
        return rules;
    }

    static Transform CreateGroup(string name, Transform parent)
    {
        Transform group = new GameObject(name).transform;
        group.SetParent(parent, false);
        return group;
    }

    static void IgnoreFromNavMesh(GameObject target)
    {
        if (target == null || target.GetComponent<NavMeshModifier>() != null) return;
        target.AddComponent<NavMeshModifier>().ignoreFromBuild = true;
    }

    static void BakeNavMesh(Scene scene)
    {
        NavMeshSurface surface = null;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            surface = root.GetComponentInChildren<NavMeshSurface>();
            if (surface != null) break;
        }
        if (surface == null)
        {
            Debug.LogWarning("[Alexandre] Pas de NavMeshSurface dans la scène : NavMesh non recalculé.");
            return;
        }

        surface.BuildNavMesh();
        if (surface.navMeshData == null) return;

        EnsureFolder(AlexandreSceneFolder);
        string navMeshPath = AlexandreSceneFolder + "/NavMesh-" + surface.gameObject.name + ".asset";
        AssetDatabase.DeleteAsset(navMeshPath);
        AssetDatabase.CreateAsset(surface.navMeshData, navMeshPath);
        EditorUtility.SetDirty(surface);
    }

    static GameObject FindRootObject(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == name) return root;
        }
        return null;
    }

    static void DestroyRootObject(Scene scene, string name)
    {
        GameObject target = FindRootObject(scene, name);
        if (target != null) Object.DestroyImmediate(target);
    }

    static void EnsureFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath)) return;
        string parent = Path.GetDirectoryName(folderPath).Replace('\\', '/');
        EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, Path.GetFileName(folderPath));
    }
}
