using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutoSaveBeforePlay
{
    static AutoSaveBeforePlay() => EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
            SaveAllOpenScenes();

        if (state == PlayModeStateChange.EnteredPlayMode)
            Logging.Log("Auto-saved all open scenes before entering play mode at " + System.DateTime.Now);
    }

    private static void SaveAllOpenScenes() => EditorSceneManager.SaveOpenScenes();
}