using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AlexandreSceneTools
{
    const string AlexandreScenePath = "Assets/Scenes/Alexandre.unity";

    [MenuItem("Alexandre/Ouvrir ma scène")]
    public static void OpenAlexandreScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        EditorSceneManager.OpenScene(AlexandreScenePath, OpenSceneMode.Single);
    }

    [MenuItem("Alexandre/Configurer ma scène")]
    public static void ConfigureAlexandreScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        PlayerPrefabSetup.Configure();

        Scene scene = EditorSceneManager.OpenScene(AlexandreScenePath, OpenSceneMode.Single);

        GameManager manager = Object.FindFirstObjectByType<GameManager>();
        if (manager == null)
        {
            manager = new GameObject("GameManager").AddComponent<GameManager>();
        }
        manager.Antidote = Object.FindFirstObjectByType<Antidote>();
        if (manager.Antidote == null)
        {
            Debug.LogWarning("[Alexandre] Pas d'Antidote dans la scène : la victoire ne sera pas déclenchée.");
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[Alexandre] Scène configurée : " + AlexandreScenePath);
    }
}
