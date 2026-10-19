using UnityEditor;
using UnityEditor.SceneManagement;

public static class AlexandreSceneTools
{
    const string AlexandreScenePath = "Assets/Scenes/Alexandre.unity";

    [MenuItem("Alexandre/Ouvrir ma scène")]
    public static void OpenAlexandreScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        EditorSceneManager.OpenScene(AlexandreScenePath, OpenSceneMode.Single);
    }
}
