using System.Collections.Generic;
using System.IO;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public static class KenanSceneBuilder
{
    const string SourceScenePath = "Assets/Scenes/Alexandre.unity";
    const string KenanScenePath = "Assets/Scenes/Kenan.unity";
    const string KenanSceneFolder = "Assets/Scenes/Kenan";
    const string ColorMaterialsFolder = "Assets/Materials/Couleurs";
    const string ColorDataFolder = "Assets/Data/Couleurs";
    const string PlayerPrefabPath = "Assets/Prefabs/Player.prefab";

    const string ChomperFolder = "Assets/Chomper";
    const string ChomperModelPath = ChomperFolder + "/Models/Chomper.FBX";
    const string ChomperControllerPath = ChomperFolder + "/Animation/Chompercontroller.controller";

    static readonly Color Rouge = new Color(0.90f, 0.15f, 0.15f);
    static readonly Color Jaune = new Color(0.95f, 0.80f, 0.10f);
    static readonly Color Vert = new Color(0.20f, 0.75f, 0.25f);
    static readonly Color Bleu = new Color(0.15f, 0.35f, 0.90f);

    [MenuItem("Kenan/Créer ma scène")]
    public static void CreateKenanScene()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(KenanScenePath) != null &&
            !EditorUtility.DisplayDialog("Kenan", "La scène Kenan existe déjà.\nLa recréer à partir de la scène d'Alexandre ?\n(les modifications faites dans Kenan.unity seront perdues)", "Recréer", "Annuler"))
        {
            return;
        }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        PlayerPrefabSetup.Configure();

        if (!CopySourceScene())
        {
            Debug.LogError("[Kenan] Impossible de copier " + SourceScenePath);
            return;
        }
        Scene scene = EditorSceneManager.OpenScene(KenanScenePath, OpenSceneMode.Single);

        GameObject player = FindRootObject(scene, "Player", true);
        GameObject dino = FindRootObject(scene, "Dino", false);
        if (player == null || dino == null)
        {
            Debug.LogError("[Kenan] Joueur (tag Player) ou objet 'Dino' introuvable dans la scène.");
            return;
        }

        AlexandreMapBuilder.BuildMap(scene);

        ConvertChomperMaterialsToURP();
        ReplaceDinoWithChomper(dino);

        ColorData rouge = GetOrCreateColorData("Rouge", Rouge);
        ColorData jaune = GetOrCreateColorData("Jaune", Jaune);
        ColorData vert = GetOrCreateColorData("Vert", Vert);
        ColorData bleu = GetOrCreateColorData("Bleu", Bleu);

        float corner = ArenaLayout.PainterCorner;
        CreatePainter(rouge, new Vector3(-corner, 0.0f, corner));
        CreatePainter(jaune, new Vector3(corner, 0.0f, corner));
        CreatePainter(vert, new Vector3(-corner, 0.0f, -corner));
        CreatePainter(bleu, new Vector3(corner, 0.0f, -corner));

        Dictionary<string, ColorData> colors = new Dictionary<string, ColorData>
        {
            { "Rouge", rouge }, { "Jaune", jaune }, { "Vert", vert }, { "Bleu", bleu },
        };
        foreach (ArenaLayout.ZoneSpot spot in ArenaLayout.Zones)
        {
            CreateZone(colors[spot.ColorName], spot.Center, spot.RadiusX, spot.RadiusZ);
        }

        SurvivorManager survivors = CreateSurvivors(player);
        SetupCamera(scene, survivors);
        dino.AddComponent<DinoFollowActiveSurvivor>().Survivors = survivors;

        BakeNavMesh(scene);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        Selection.activeGameObject = survivors.gameObject;
        Debug.Log("[Kenan] Scène créée : " + KenanScenePath);
        EditorUtility.DisplayDialog("Kenan", "Ta scène est prête : " + KenanScenePath + "\n\nAppuie sur Play : Tab pour changer de survivant, E près d'un PNJ pour prendre sa couleur.", "OK");
    }

    static bool CopySourceScene()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(KenanScenePath) == null)
        {
            return AssetDatabase.CopyAsset(SourceScenePath, KenanScenePath);
        }

        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        File.Copy(SourceScenePath, KenanScenePath, true);
        AssetDatabase.ImportAsset(KenanScenePath, ImportAssetOptions.ForceUpdate);
        return true;
    }

    static SurvivorManager CreateSurvivors(GameObject firstPlayer)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
        Transform group = new GameObject("Survivants").transform;
        float height = firstPlayer.transform.position.y;
        Vector3 scale = firstPlayer.transform.localScale;
        Survivor[] survivors = new Survivor[ArenaLayout.SurvivorSpawns.Length];

        for (int i = 0; i < survivors.Length; i++)
        {
            GameObject survivor = i == 0 ? firstPlayer : (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            survivor.name = "Survivant_" + (i + 1);
            survivor.transform.SetParent(group, false);
            survivor.transform.SetPositionAndRotation(ArenaLayout.SurvivorSpawns[i] + Vector3.up * height, Quaternion.identity);
            survivor.transform.localScale = scale;
            survivors[i] = survivor.GetComponent<Survivor>();
        }

        SurvivorManager manager = group.gameObject.AddComponent<SurvivorManager>();
        manager.Survivors = survivors;
        group.gameObject.AddComponent<SurvivorHud>();
        return manager;
    }

    static void SetupCamera(Scene scene, SurvivorManager survivors)
    {
        GameObject cameraObject = FindRootObject(scene, "Main Camera", false);
        if (cameraObject == null) return;

        CameraFollow follow = cameraObject.GetComponent<CameraFollow>();
        if (follow == null) follow = cameraObject.AddComponent<CameraFollow>();
        follow.Survivors = survivors;

        Vector3 start = survivors.Survivors[0].transform.position;
        cameraObject.transform.SetPositionAndRotation(start + follow.Offset, Quaternion.LookRotation(-follow.Offset));
    }

    static void ReplaceDinoWithChomper(GameObject dino)
    {
        RemoveComponent<MeshRenderer>(dino);
        RemoveComponent<MeshFilter>(dino);
        RemoveComponent<Collider>(dino);

        dino.transform.position = ArenaLayout.DinoSpawn;
        NavMeshAgent agent = dino.GetComponent<NavMeshAgent>();
        agent.baseOffset = 0.0f;
        agent.radius = 1.0f;
        agent.height = 2.5f;

        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(ChomperModelPath);
        GameObject chomper = (GameObject)PrefabUtility.InstantiatePrefab(model, dino.scene);
        chomper.name = "Chomper";
        chomper.transform.SetParent(dino.transform, false);
        chomper.transform.localPosition = Vector3.zero;
        chomper.transform.localRotation = Quaternion.identity;
        chomper.transform.localScale = Vector3.one * 2.0f;

        Animator animator = chomper.GetComponent<Animator>();
        animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(ChomperControllerPath);
        animator.applyRootMotion = false;

        dino.AddComponent<DinoAnimation>();
    }

    static void ConvertChomperMaterialsToURP()
    {
        Shader lit = Shader.Find("Universal Render Pipeline/Lit");
        Texture albedo = AssetDatabase.LoadAssetAtPath<Texture>(ChomperFolder + "/Textures/Chomper_Albedo.tif");
        Texture normal = AssetDatabase.LoadAssetAtPath<Texture>(ChomperFolder + "/Textures/Chomper_Normal.tif");
        Texture metallic = AssetDatabase.LoadAssetAtPath<Texture>(ChomperFolder + "/Textures/Chomper_MetallicSmooth.tif");
        Texture occlusion = AssetDatabase.LoadAssetAtPath<Texture>(ChomperFolder + "/Textures/Chomper_Occlusion.tif");

        ConvertMaterial(ChomperFolder + "/Materials/Chomper_Body_Mat.mat", lit, albedo, Color.white, normal, metallic, occlusion);
        ConvertMaterial(ChomperFolder + "/Materials/Chomper_Internals_Mat.mat", lit, null, new Color(0.55f, 0.15f, 0.15f), normal, metallic, occlusion);
    }

    static void ConvertMaterial(string path, Shader lit, Texture baseMap, Color baseColor, Texture normal, Texture metallic, Texture occlusion)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null || material.shader == lit) return;

        material.shader = lit;
        material.shaderKeywords = new string[0];
        material.SetTexture("_BaseMap", baseMap);
        material.SetColor("_BaseColor", baseColor);
        material.SetTexture("_BumpMap", normal);
        material.EnableKeyword("_NORMALMAP");
        material.SetTexture("_MetallicGlossMap", metallic);
        material.EnableKeyword("_METALLICSPECGLOSSMAP");
        material.SetTexture("_OcclusionMap", occlusion);
        material.EnableKeyword("_OCCLUSIONMAP");
        EditorUtility.SetDirty(material);
    }

    static void CreatePainter(ColorData data, Vector3 groundPosition)
    {
        GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        npc.name = "PNJ_" + data.DisplayName;
        npc.transform.position = groundPosition + Vector3.up;
        npc.transform.LookAt(new Vector3(0.0f, 1.0f, 0.0f));
        npc.GetComponent<MeshRenderer>().sharedMaterial = GetOrCreateColorMaterial(data);

        SphereCollider interactionZone = npc.AddComponent<SphereCollider>();
        interactionZone.isTrigger = true;
        interactionZone.radius = 2.5f;

        Rigidbody body = npc.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;

        ColorSource source = npc.AddComponent<ColorSource>();
        source.Data = data;
        npc.AddComponent<ColorInteraction>();
    }

    static void CreateZone(ColorData data, Vector3 center, float radiusX, float radiusZ)
    {
        GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        zone.name = "Zone_" + data.DisplayName;
        zone.transform.position = center + new Vector3(0.0f, 0.025f, 0.0f);
        zone.transform.localScale = new Vector3(radiusX * 2.0f, 0.025f, radiusZ * 2.0f);
        zone.GetComponent<MeshRenderer>().sharedMaterial = GetOrCreateColorMaterial(data);
        RemoveComponent<Collider>(zone);

        ColorSource source = zone.AddComponent<ColorSource>();
        source.Data = data;
        zone.AddComponent<ColorZone>();

        // Trigger plus haut que le disque pour détecter le joueur qui se tient dessus.
        GameObject detection = new GameObject("Detection");
        detection.transform.SetParent(zone.transform, false);
        detection.transform.localScale = new Vector3(1.0f, 40.0f, 1.0f);
        MeshCollider trigger = detection.AddComponent<MeshCollider>();
        trigger.sharedMesh = zone.GetComponent<MeshFilter>().sharedMesh;
        trigger.convex = true;
        trigger.isTrigger = true;
    }

    static ColorData GetOrCreateColorData(string colorName, Color color)
    {
        string path = ColorDataFolder + "/" + colorName + ".asset";
        ColorData data = AssetDatabase.LoadAssetAtPath<ColorData>(path);
        if (data != null) return data;

        EnsureFolder(ColorDataFolder);
        data = ScriptableObject.CreateInstance<ColorData>();
        data.DisplayName = colorName;
        data.Color = color;
        AssetDatabase.CreateAsset(data, path);
        return data;
    }

    static Material GetOrCreateColorMaterial(ColorData data)
    {
        string path = ColorMaterialsFolder + "/" + data.DisplayName + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null) return material;

        EnsureFolder(ColorMaterialsFolder);
        material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.color = data.Color;
        AssetDatabase.CreateAsset(material, path);
        return material;
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
            Debug.LogWarning("[Kenan] Pas de NavMeshSurface dans la scène : NavMesh non recalculé.");
            return;
        }

        surface.BuildNavMesh();
        if (surface.navMeshData == null) return;

        EnsureFolder(KenanSceneFolder);
        string navMeshPath = KenanSceneFolder + "/NavMesh-" + surface.gameObject.name + ".asset";
        AssetDatabase.DeleteAsset(navMeshPath);
        AssetDatabase.CreateAsset(surface.navMeshData, navMeshPath);
        EditorUtility.SetDirty(surface);
    }

    static void RemoveComponent<T>(GameObject target) where T : Component
    {
        T component = target.GetComponent<T>();
        if (component != null) Object.DestroyImmediate(component);
    }

    static GameObject FindRootObject(Scene scene, string nameOrTag, bool byTag)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (byTag ? root.CompareTag(nameOrTag) : root.name == nameOrTag) return root;
        }
        return null;
    }

    static void EnsureFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath)) return;
        string parent = Path.GetDirectoryName(folderPath).Replace('\\', '/');
        EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, Path.GetFileName(folderPath));
    }
}
