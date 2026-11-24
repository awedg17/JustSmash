using UnityEditor.SceneManagement;
using UnityEngine;
public static class BootSetup
{
    public static void Run()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Boot.unity");
        if (Object.FindFirstObjectByType<JustSmash.BootDiagnostics>() == null)
            new GameObject("BootDiagnostics").AddComponent<JustSmash.BootDiagnostics>();
        EditorSceneManager.SaveScene(scene);
    }
}
