using System;
using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;

public class BootTests
{
    [Test] public void MobileBuildHasSeparateArtifactEntrypoint()
    {
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("AndroidBuild")).First(t => t != null);
        Assert.That(type.GetMethod("RunMobile"), Is.Not.Null, "Do not overwrite historical APKs");
    }
    [Test] public void ConfigDefaultsMatchThreeDifficultyGameAndZeroWalkingCost()
    {
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.BootDiagnostics+Config")).First(t => t != null);
        var defaults = Activator.CreateInstance(type);
        var serialized = JsonUtility.FromJson(Resources.Load<TextAsset>("prototype-config").text, type);
        foreach (var config in new[] { defaults, serialized })
        {
            Assert.That(type.GetField("difficulty").GetValue(config), Is.EqualTo("Normal"));
            Assert.That(type.GetField("moveDrain").GetValue(config), Is.EqualTo(0f));
        }
    }
    [Test] public void BootLoadsDiagnosticsAndConfig()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Boot.unity");
        Assert.That(scene.isLoaded, Is.True);
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.BootDiagnostics")).FirstOrDefault(t => t != null);
        Assert.That(type, Is.Not.Null, "BootDiagnostics missing");
        Assert.That(UnityEngine.Object.FindFirstObjectByType(type), Is.Not.Null, "Boot scene must contain diagnostics");
        var config = Resources.Load<TextAsset>("prototype-config");
        Assert.That(config, Is.Not.Null, "Versioned config missing");
        Assert.That(config.text, Does.Contain("targetFps"));
    }
}
