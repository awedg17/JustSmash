using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AndroidBuild
{
    public static void RunMobile() { Build("builds/JustSmash-Mobile.apk", "evidence/mobile/build", BuildOptions.None); }
    public static void RunShare() { Build("builds/JustSmash-Share.apk", "evidence/M2/share-build", BuildOptions.None); }
    public static void RunProduction() { Build("builds/JustSmash-Production.apk", "evidence/M2/production", BuildOptions.None); }
    public static void RunM2() { Build("builds/JustSmash-M2.apk", "evidence/M2"); }
    public static void RunM1() { Build("builds/JustSmash-M1.apk", "evidence/M1"); }
    public static void Run() { Build("builds/JustSmash-M0.apk", "evidence/M0"); }
    static void Build(string output, string evidence, BuildOptions options = BuildOptions.Development)
    {
        RuntimeShaders.Retain();
        Directory.CreateDirectory(evidence);
        Directory.CreateDirectory("builds");
        Directory.CreateDirectory("Assets/Scenes");
        if (!File.Exists("Assets/Scenes/Boot.unity"))
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Boot.unity");
        }
        PlayerSettings.companyName = "JustSmash";
        PlayerSettings.productName = "JustSmash";
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.justsmash.prototype");
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.forceInternetPermission = false;
        PlayerSettings.Android.forceSDCardPermission = false;
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/Boot.unity", true) };
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = new[] { "Assets/Scenes/Boot.unity" },
            locationPathName = output,
            target = BuildTarget.Android,
            options = options
        });
        File.WriteAllText(evidence + "/build-result.txt", $"result={report.summary.result}\nbytes={report.summary.totalSize}\nerrors={report.summary.totalErrors}\nwarnings={report.summary.totalWarnings}\n");
        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("Android development build failed: " + report.summary.result);
    }
}
