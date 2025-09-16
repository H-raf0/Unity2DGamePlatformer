using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement; // <-- Add this

[InitializeOnLoad]
public static class SimpleEditorUtils
{
    [MenuItem("Edit/Play-Unplay, But From Prelaunch Scene %0")]
    public static void PlayFromPrelaunchScene()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
            return;
        }
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Scenes/_Manager.unity");
        EditorApplication.isPlaying = true;
    }
}
