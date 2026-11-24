using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

public class PreferenceFixture
{
    GameObject inputIsolation;
#if UNITY_EDITOR
    UnityEditor.EditorWindow captureView;
    object captureSizeGroup;
    int originalSize, addedSize = -1;
    protected System.Collections.IEnumerator CaptureAt1280x720() => CaptureAtResolution(1280, 720);
    protected System.Collections.IEnumerator CaptureAtResolution(int width, int height)
    {
        if (addedSize >= 0)
        {
            captureView.GetType().GetMethod("SizeSelectionCallback").Invoke(captureView, new object[] { originalSize, null });
            captureSizeGroup.GetType().GetMethod("RemoveCustomSize").Invoke(captureSizeGroup, new object[] { addedSize }); addedSize = -1;
        }
        var editor = typeof(UnityEditor.Editor).Assembly;
        var sizesType = editor.GetType("UnityEditor.GameViewSizes");
        var sizes = sizesType.BaseType.GetProperty("instance").GetValue(null);
        captureSizeGroup = sizesType.GetProperty("currentGroup").GetValue(sizes);
        var groupType = captureSizeGroup.GetType();
        captureView = UnityEditor.EditorWindow.GetWindow(editor.GetType("UnityEditor.GameView"));
        originalSize = (int)captureView.GetType().GetProperty("selectedSizeIndex").GetValue(captureView);
        addedSize = (int)groupType.GetMethod("GetTotalCount").Invoke(captureSizeGroup, null);
        var fixedSize = System.Enum.Parse(editor.GetType("UnityEditor.GameViewSizeType"), "FixedResolution");
        groupType.GetMethod("AddCustomSize").Invoke(captureSizeGroup, new[] { System.Activator.CreateInstance(editor.GetType("UnityEditor.GameViewSize"), fixedSize, width, height, "JustSmash recording") });
        captureView.GetType().GetMethod("SizeSelectionCallback").Invoke(captureView, new object[] { addedSize, null }); captureView.Repaint();
        for (int i = 0; i < 10 && (Screen.width != width || Screen.height != height); i++) yield return null;
        Assert.That(new Vector2Int(Screen.width, Screen.height), Is.EqualTo(new Vector2Int(width, height)));
    }
#endif
    string directory;
    HashSet<string> originalDirectories;
    Dictionary<string, byte[]> original;
    protected string EvidenceDirectory { get; private set; }

    static void CreateEvidenceDirectory(string path)
    {
        if (Directory.Exists(path) || File.Exists(path)) throw new IOException("Refusing to overwrite QA evidence: " + path);
        Directory.CreateDirectory(path);
    }

    [SetUp] public void IsolatePreferences()
    {
        Assert.That(Application.isEditor, Is.True, "Host fixtures must not change device preferences");
        if (TestContext.CurrentContext.Test.Name != "NativeMouseAndKeyboardUseGameView")
        {
            inputIsolation = new GameObject("QA input isolation");
            inputIsolation.AddComponent<QaInputIsolation>();
            Object.DontDestroyOnLoad(inputIsolation);
            Input.ResetInputAxes();
        }
        directory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".hermes", "local-game-data"));
        Directory.CreateDirectory(directory);
        originalDirectories = new HashSet<string>(Directory.GetDirectories(directory, "*", SearchOption.AllDirectories));
        original = new Dictionary<string, byte[]>();
        foreach (string file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories)) original.Add(file, File.ReadAllBytes(file));
        string output = System.Environment.GetEnvironmentVariable("JUSTSMASH_QA_RUN");
        if (string.IsNullOrEmpty(output)) output = Path.Combine(Application.dataPath, "../.utmp/qa-" + System.Guid.NewGuid().ToString("N"));
        EvidenceDirectory = Path.Combine(output, TestContext.CurrentContext.Test.Name + "-" + System.Guid.NewGuid().ToString("N").Substring(0, 8));
        CreateEvidenceDirectory(EvidenceDirectory);
        string prefs = Path.Combine(directory, "prefs"); Directory.CreateDirectory(prefs);
        foreach (var pair in new Dictionary<string, string> {
            { "assist", "Manual" }, { "joystickMode", "Fixed" }, { "difficulty", "Normal" },
            { "replayMode", "1" }, { "targetFps", "30" }, { "volume", "0.7" },
            { "trailBrightness", "0.85" }, { "shadowBrightness", "0.9" },
            { "trailTint", "FFFFFF" }, { "shadowTint", "1A2424" }
        }) File.WriteAllText(Path.Combine(prefs, pair.Key + ".txt"), pair.Value);
    }

    [TearDown] public void RestorePreferences()
    {
        if (inputIsolation != null) Object.DestroyImmediate(inputIsolation);
#if UNITY_EDITOR
        if (addedSize >= 0)
        {
            captureView.GetType().GetMethod("SizeSelectionCallback").Invoke(captureView, new object[] { originalSize, null });
            captureSizeGroup.GetType().GetMethod("RemoveCustomSize").Invoke(captureSizeGroup, new object[] { addedSize }); addedSize = -1;
        }
#endif
        if (original == null) return;
        Time.timeScale = 1;
        foreach (string file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
            if (!original.ContainsKey(file)) File.Delete(file);
        foreach (var pair in original) { Directory.CreateDirectory(Path.GetDirectoryName(pair.Key)); File.WriteAllBytes(pair.Key, pair.Value); }
        var folders = Directory.GetDirectories(directory, "*", SearchOption.AllDirectories);
        System.Array.Sort(folders, (a, b) => b.Length.CompareTo(a.Length));
        foreach (string folder in folders) if (!originalDirectories.Contains(folder) && Directory.GetFileSystemEntries(folder).Length == 0) Directory.Delete(folder);
    }
}

[DefaultExecutionOrder(-32000)]
public sealed class QaInputIsolation : MonoBehaviour
{
    void Update() => Input.ResetInputAxes();
}
