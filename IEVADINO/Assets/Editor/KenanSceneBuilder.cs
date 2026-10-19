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

        AssetDatabase.DeleteAsset(KenanScenePath);
        if (!AssetDatabase.CopyAsset(SourceScenePath, KenanScenePath))
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

        ConvertChomperMaterialsToURP();
        ReplaceDinoWithChomper(dino);

        CreatePainter("Rouge", Rouge, new Vector3(-12.0f, 0.0f, 12.0f));
        CreatePainter("Jaune", Jaune, new Vector3(12.0f, 0.0f, 12.0f));
        CreatePainter("Vert", Vert, new Vector3(-12.0f, 0.0f, -12.0f));
        CreatePainter("Bleu", Bleu, new Vector3(12.0f, 0.0f, -12.0f));

        CreateZone("Rouge", Rouge, new Vector3(-8.0f, 0.0f, 6.0f), 4.0f, 3.0f);
        CreateZone("Rouge", Rouge, new Vector3(2.0f, 0.0f, -5.0f), 3.5f, 2.5f);
        CreateZone("Bleu", Bleu, new Vector3(1.0f, 0.0f, 7.5f), 3.0f, 2.5f);
        CreateZone("Bleu", Bleu, new Vector3(11.0f, 0.0f, -6.0f), 3.0f, 3.5f);
        CreateZone("Jaune", Jaune, new Vector3(9.0f, 0.0f, 8.0f), 4.0f, 2.5f);
        CreateZone("Jaune", Jaune, new Vector3(-6.0f, 0.0f, 0.0f), 3.0f, 2.0f);
        CreateZone("Vert", Vert, new Vector3(6.0f, 0.0f, 1.0f), 3.5f, 3.0f);
        CreateZone("Vert", Vert, new Vector3(-8.0f, 0.0f, -6.0f), 4.5f, 3.5f);

        if (player.GetComponent<PlayerColor>() == null)
        {
            player.AddComponent<PlayerColor>();
        }
        if (player.GetComponent<PlayerHiding>() == null)
        {
            player.AddComponent<PlayerHiding>();
        }

        BakeNavMesh(scene);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        Selection.activeGameObject = player;
        Debug.Log("[Kenan] Scène créée : " + KenanScenePath);
        EditorUtility.DisplayDialog("Kenan", "Ta scène est prête : " + KenanScenePath + "\n\nAppuie sur Play, prends une couleur (E près d'un PNJ) puis va sur une zone de la même couleur.", "OK");
    }

    static void ReplaceDinoWithChomper(GameObject dino)
    {
        RemoveComponent<MeshRenderer>(dino);
        RemoveComponent<MeshFilter>(dino);
        RemoveComponent<Collider>(dino);

        Vector3 position = dino.transform.position;
        dino.transform.position = new Vector3(position.x, 0.0f, position.z);
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

    static void CreatePainter(string colorName, Color color, Vector3 groundPosition)
    {
        GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        npc.name = "PNJ_" + colorName;
        npc.transform.position = groundPosition + Vector3.up;
        npc.transform.LookAt(new Vector3(0.0f, 1.0f, 0.0f));
        npc.GetComponent<MeshRenderer>().sharedMaterial = GetOrCreateColorMaterial(colorName, color);

        SphereCollider interactionZone = npc.AddComponent<SphereCollider>();
        interactionZone.isTrigger = true;
        interactionZone.radius = 2.5f;

        Rigidbody body = npc.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;

        ColorSource source = npc.AddComponent<ColorSource>();
        source.ColorName = colorName;
        npc.AddComponent<ColorInteraction>();
    }

    static void CreateZone(string colorName, Color color, Vector3 center, float radiusX, float radiusZ)
    {
        GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        zone.name = "Zone_" + colorName;
        zone.transform.position = center + new Vector3(0.0f, 0.025f, 0.0f);
        zone.transform.localScale = new Vector3(radiusX * 2.0f, 0.025f, radiusZ * 2.0f);
        zone.GetComponent<MeshRenderer>().sharedMaterial = GetOrCreateColorMaterial(colorName, color);

        RemoveComponent<Collider>(zone);
        zone.AddComponent<MeshCollider>().sharedMesh = zone.GetComponent<MeshFilter>().sharedMesh;

        ColorSource source = zone.AddComponent<ColorSource>();
        source.ColorName = colorName;
    }

    static Material GetOrCreateColorMaterial(string colorName, Color color)
    {
        string path = ColorMaterialsFolder + "/" + colorName + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null) return material;

        EnsureFolder(ColorMaterialsFolder);
        material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.color = color;
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
