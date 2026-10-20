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
    const string ColorDataFolder = "Assets/Data/Couleurs";

    const float ChestHeight = 0.05f;
    const int NotWalkableArea = 1;
    const float PlaneSize = 10.0f;

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

        Scene scene = EditorSceneManager.OpenScene(AlexandreScenePath, OpenSceneMode.Single);
        BuildMap(scene);
        BakeNavMesh(scene);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[Alexandre] Scène construite : " + AlexandreScenePath);
    }

    // Reconstruit la map dans la scène ouverte 
    public static void BuildMap(Scene scene)
    {
        GameRules rules = GetOrCreateGameRules();

        DestroyRootObject(scene, MapRootName);
        DestroyRootObject(scene, "Antidote");
        ResizeGround(scene);

        Transform map = new GameObject(MapRootName).transform;
        BuildWalls(CreateGroup("Murs", map));
        BuildObstacles(CreateGroup("Obstacles", map));
        BuildSpawn(CreateGroup("Refuge", map));
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

        GameObject player = FindRootObject(scene, "Player");
        if (player != null)
        {
            Vector3 spawn = ArenaLayout.SurvivorSpawns[0];
            player.transform.position = new Vector3(spawn.x, player.transform.position.y, spawn.z);
        }

        GameObject dino = FindRootObject(scene, "Dino");
        IgnoreFromNavMesh(dino);
        SetupDinoCatcher(dino);
    }

    static void ResizeGround(Scene scene)
    {
        GameObject ground = FindRootObject(scene, "Ground");
        if (ground == null) return;

        float scale = ArenaLayout.HalfSize * 2.0f / PlaneSize;
        ground.transform.localScale = new Vector3(scale, 1.0f, scale);
    }

    static void BuildWalls(Transform parent)
    {
        float length = ArenaLayout.HalfSize * 2.0f + 1.0f;
        float offset = ArenaLayout.HalfSize + 0.25f;
        CreateBlock("Mur_Nord", parent, new Vector3(0.0f, 0.75f, offset), new Vector3(length, 1.5f, 0.5f), 0.0f, Pierre);
        // Ouverture du mur sud vers la passerelle du refuge.
        float segment = (length - ArenaLayout.BridgeWidth) * 0.5f;
        float segmentCenter = (ArenaLayout.BridgeWidth + segment) * 0.5f;
        CreateBlock("Mur_Sud_Ouest", parent, new Vector3(-segmentCenter, 0.75f, -offset), new Vector3(segment, 1.5f, 0.5f), 0.0f, Pierre);
        CreateBlock("Mur_Sud_Est", parent, new Vector3(segmentCenter, 0.75f, -offset), new Vector3(segment, 1.5f, 0.5f), 0.0f, Pierre);
        CreateBlock("Mur_Est", parent, new Vector3(offset, 0.75f, 0.0f), new Vector3(0.5f, 1.5f, length), 0.0f, Pierre);
        CreateBlock("Mur_Ouest", parent, new Vector3(-offset, 0.75f, 0.0f), new Vector3(0.5f, 1.5f, length), 0.0f, Pierre);
    }

    static void BuildObstacles(Transform parent)
    {
        foreach (ArenaLayout.ObstacleSpot spot in ArenaLayout.Obstacles)
        {
            Color color = spot.Name.StartsWith("Rocher") ? Rocher : Pierre;
            CreateBlock(spot.Name, parent, spot.Position, spot.Size, spot.Yaw, color);
        }
    }

    static void BuildSpawn(Transform parent)
    {
        Vector3 center = ArenaLayout.SpawnCenter;
        Vector2 size = ArenaLayout.SpawnSize;
        float bridge = ArenaLayout.BridgeWidth;
        float wallOuter = ArenaLayout.HalfSize + 0.5f;
        float spawnNorth = center.z + size.y * 0.5f;
        float spawnSouth = center.z - size.y * 0.5f;
        float bridgeLength = -ArenaLayout.HalfSize - spawnNorth;
        float bridgeCenter = -ArenaLayout.HalfSize - bridgeLength * 0.5f;
        float railLength = (size.x - bridge) * 0.5f;

        CreateBlock("Plateforme", parent, new Vector3(center.x, -0.25f, center.z), new Vector3(size.x, 0.5f, size.y), 0.0f, Pierre);
        CreateBlock("Passerelle", parent, new Vector3(0.0f, -0.25f, bridgeCenter), new Vector3(bridge, 0.5f, bridgeLength), 0.0f, Bois);

        CreateBlock("GardeCorps_Sud", parent, new Vector3(center.x, 0.3f, spawnSouth), new Vector3(size.x, 0.6f, 0.2f), 0.0f, Bois);
        CreateBlock("GardeCorps_Ouest", parent, new Vector3(center.x - size.x * 0.5f, 0.3f, center.z), new Vector3(0.2f, 0.6f, size.y), 0.0f, Bois);
        CreateBlock("GardeCorps_Est", parent, new Vector3(center.x + size.x * 0.5f, 0.3f, center.z), new Vector3(0.2f, 0.6f, size.y), 0.0f, Bois);
        CreateBlock("GardeCorps_Nord_Ouest", parent, new Vector3(-(bridge + railLength) * 0.5f, 0.3f, spawnNorth), new Vector3(railLength, 0.6f, 0.2f), 0.0f, Bois);
        CreateBlock("GardeCorps_Nord_Est", parent, new Vector3((bridge + railLength) * 0.5f, 0.3f, spawnNorth), new Vector3(railLength, 0.6f, 0.2f), 0.0f, Bois);

        float railLengthBridge = -wallOuter - spawnNorth;
        float railCenterBridge = -wallOuter - railLengthBridge * 0.5f;
        float railOffset = bridge * 0.5f + 0.1f;
        CreateBlock("GardeCorps_Passerelle_Ouest", parent, new Vector3(-railOffset, 0.3f, railCenterBridge), new Vector3(0.2f, 0.6f, railLengthBridge), 0.0f, Bois);
        CreateBlock("GardeCorps_Passerelle_Est", parent, new Vector3(railOffset, 0.3f, railCenterBridge), new Vector3(0.2f, 0.6f, railLengthBridge), 0.0f, Bois);

        GameObject safeZone = new GameObject("ZoneSure");
        safeZone.transform.SetParent(parent, false);
        float safeLength = -ArenaLayout.HalfSize - spawnSouth;
        safeZone.transform.position = new Vector3(center.x, 1.0f, -ArenaLayout.HalfSize - safeLength * 0.5f);
        safeZone.AddComponent<BoxCollider>().size = new Vector3(size.x, 2.0f, safeLength);
        safeZone.AddComponent<SafeZone>();

        // Le dinosaure ne peut pas entrer dans le refuge.
        NavMeshModifier modifier = parent.gameObject.AddComponent<NavMeshModifier>();
        modifier.overrideArea = true;
        modifier.area = NotWalkableArea;
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

            Chest component = chest.GetComponent<Chest>();
            component.RequiredColor = AssetDatabase.LoadAssetAtPath<ColorData>(ColorDataFolder + "/" + spot.ColorName + ".asset");
            chests.Add(component);
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

    // Trigger porté par un enfant : le builder de la scène fusionnée retire le collider du dinosaure.
    static void SetupDinoCatcher(GameObject dino)
    {
        if (dino == null || dino.GetComponentInChildren<DinoCatcher>() != null) return;

        GameObject capture = new GameObject("Capture");
        capture.transform.SetParent(dino.transform, false);
        capture.transform.localPosition = Vector3.up * 0.5f;
        capture.AddComponent<Rigidbody>().isKinematic = true;
        capture.AddComponent<DinoCatcher>();
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
