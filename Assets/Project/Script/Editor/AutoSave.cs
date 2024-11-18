using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutoSave
{
    private static double nextSaveTime = 0;
    private const double saveInterval = 10.0; // 10 seconds

    static AutoSave()
    {
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if (EditorApplication.timeSinceStartup >= nextSaveTime)
        {
            if (EditorSceneManager.GetActiveScene().isDirty)
                SaveScene();

            nextSaveTime = EditorApplication.timeSinceStartup + saveInterval;
        }
    }

    private static void SaveScene()
    {
        if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorSceneManager.SaveOpenScenes();
            Logging.Log("Auto-saved all open scenes at " + System.DateTime.Now);
        }
    }
}