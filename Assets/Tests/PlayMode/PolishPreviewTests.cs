using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class PolishPreviewTests : PreferenceFixture
{
    [UnityEngine.TestTools.UnityTest, NUnit.Framework.Category("Capture")]
    public System.Collections.IEnumerator CaptureProposedCompositionFromActualAssets()
    {
        UnityEngine.TestTools.LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        var hidden = BindingFlags.Instance | BindingFlags.NonPublic;
        void Act(string action) => type.GetMethod("ActOnUi", hidden).Invoke(court, new object[] { action });
        void Call(string name, params object[] args) => type.GetMethod(name).Invoke(court, args);
        void Pose(string name, Vector3 position, float yaw)
        {
            var root = GameObject.Find(name).transform; root.SetPositionAndRotation(position, Quaternion.Euler(0, yaw, 0));
            var driver = root.Find("AthleteVisual").GetComponent(AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.AthleteVisual")).First(t => t != null));
            driver.GetType().GetMethod("SetTravel").Invoke(driver, new object[] { 0f });
            driver.GetType().GetMethod("Present").Invoke(driver, new object[] { "Ready", 1f, Vector3.zero, Vector3.zero, 0f, 0f });
        }
        void Capture(string name)
        {
            var camera = Camera.main; var previous = camera.targetTexture; var active = RenderTexture.active;
            var target = RenderTexture.GetTemporary(1280, 720, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 4);
            try
            {
                camera.targetTexture = target; camera.Render(); RenderTexture.active = target;
                var image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
                image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); image.Apply();
                File.WriteAllBytes(Path.Combine(EvidenceDirectory, name + ".png"), image.EncodeToPNG()); UnityEngine.Object.Destroy(image);
                Assert.That(new FileInfo(Path.Combine(EvidenceDirectory, name + ".png")).Length, Is.GreaterThan(10000));
            }
            finally { camera.targetTexture = previous; RenderTexture.active = active; RenderTexture.ReleaseTemporary(target); }
        }
        // Concept staging modifies scene instances only, never source assets or live match rules.
#if UNITY_EDITOR
        string shaderPath = System.Environment.GetEnvironmentVariable("JUSTSMASH_PREVIEW_SHADER");
        Assert.That(File.Exists(shaderPath), Is.True, "Explicit local preview shader source required");
        var toon = UnityEditor.ShaderUtil.CreateShaderAsset(File.ReadAllText(shaderPath), true);
        Assert.That(UnityEditor.ShaderUtil.ShaderHasError(toon), Is.False);
        Assert.That(toon.isSupported, Is.True);
        void CartoonScene()
        {
            foreach (var renderer in UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
            {
                if (renderer is LineRenderer || renderer is TrailRenderer || renderer.GetComponent<TextMesh>() != null) continue;
                var materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    var material = materials[i]; material.shader = toon;
                    bool clothing = renderer.name.Contains("tshirt") || renderer.name.Contains("pants") || renderer.name.Contains("jeans");
                    material.SetFloat("_Flat", clothing ? 1 : 0);
                    material.SetFloat("_Outline", renderer is SkinnedMeshRenderer ? .008f : 0);
                    if (renderer.name.Contains("pants") || renderer.name.Contains("jeans")) material.color = new Color(.04f, .13f, .30f);
                }
                renderer.materials = materials;
                if (renderer is SkinnedMeshRenderer && renderer.name.Contains("tshirt"))
                {
                    var block = new MaterialPropertyBlock(); renderer.GetPropertyBlock(block);
                    bool near = renderer.transform.root.name == "Player" || renderer.transform.root.name == "Partner";
                    block.SetColor("_Color", near ? new Color(.12f,.40f,1) : new Color(1,.30f,.13f)); renderer.SetPropertyBlock(block);
                }
            }
        }
        CartoonScene();
#endif
        ((Behaviour)court).enabled = false;
        var light = GameObject.Find("CourtLight").GetComponent<Light>(); light.intensity = 1.25f;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(.58f, .63f, .66f);
        GameObject.Find("HallFloor").GetComponent<Renderer>().material.color = new Color(.25f, .39f, .64f);
        GameObject.Find("HallBackWall").GetComponent<Renderer>().material.color = new Color(.11f, .24f, .48f);
        foreach (var label in UnityEngine.Object.FindObjectsByType<TextMesh>(FindObjectsSortMode.None))
        {
            var renderer = label.GetComponent<Renderer>();
            var material = new Material(Shader.Find("Sprites/Default")) { mainTexture = label.font.material.mainTexture };
            renderer.material = material;
        }
        GameObject.Find("Court").GetComponent<Renderer>().material.color = new Color(.04f, .30f, .78f);
        var shuttleObject = GameObject.Find("Shuttle"); var shuttleShadow = GameObject.Find("ShuttleShadow");
        var landingMarker = GameObject.Find("ShuttleLandingShadow");
        shuttleObject.SetActive(false); shuttleShadow.SetActive(false); if (landingMarker != null) landingMarker.SetActive(false);
        Camera.main.fieldOfView = 42;
        Pose("Player", new Vector3(1.0f, 0, -4.5f), 145);
        Camera.main.transform.position = new Vector3(-.4f, 1.35f, -8.4f); Camera.main.transform.LookAt(new Vector3(-.4f, 1, -4.5f));
        yield return null; Capture("concept-home-scene");
        Act("Main");
        Pose("Player", new Vector3(.3f, 0, -4.3f), 160); Pose("Opponent", new Vector3(1.65f, 0, -3.5f), 195);
        Camera.main.transform.position = new Vector3(-1.1f, 1.5f, -9.6f); Camera.main.transform.LookAt(new Vector3(-1.1f, .9f, -4));
        type.GetMethod("UpdateArena", hidden).Invoke(court, null); yield return null; Capture("concept-setup-singles-scene");
        Act("Ganda");
#if UNITY_EDITOR
        CartoonScene();
#endif
        Pose("Player", new Vector3(-.1f, 0, -4.6f), 160); Pose("Partner", new Vector3(1.3f, 0, -4.4f), 175);
        Pose("Opponent", new Vector3(-1.4f, 0, -1.0f), 180); Pose("OpponentPartner", new Vector3(2.9f, 0, .3f), 195);
        Camera.main.transform.position = new Vector3(-1.2f, 1.7f, -10.2f); Camera.main.transform.LookAt(new Vector3(-1.2f, .9f, -3.8f));
        type.GetMethod("UpdateArena", hidden).Invoke(court, null); yield return null; Capture("concept-setup-doubles-scene");
        shuttleObject.SetActive(true); shuttleShadow.SetActive(true);
        Act("Tunggal"); Act("Mulai"); Act("Teruskan"); Act("Teruskan");
        Camera.main.fieldOfView = 48;
        Camera.main.transform.position = new Vector3(0, 5.2f, -14.2f); Camera.main.transform.LookAt(new Vector3(0, 1, -1));
        ((Behaviour)court).enabled = true;
        Call("Serve", true); yield return new WaitForSeconds(.35f);
        ((Behaviour)court).enabled = false; Capture("concept-rally-scene");
        Act("Pause"); Capture("concept-pause-scene");
        Act("Lanjut");
        var match = type.GetProperty("Match").GetValue(court);
        while ((int)match.GetType().GetField("Winner").GetValue(match) < 0) match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
        Pose("Player", new Vector3(1.0f, 0, -4.5f), 145);
        var visual = GameObject.Find("Player").transform.Find("AthleteVisual").GetComponent(AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.AthleteVisual")).First(t => t != null));
        visual.GetType().GetMethod("PresentReaction").Invoke(visual, new object[] { true, 1f });
        Camera.main.fieldOfView = 42; Camera.main.transform.position = new Vector3(-.4f, 1.35f, -8.4f); Camera.main.transform.LookAt(new Vector3(-.4f, 1, -4.5f));
        type.GetMethod("UpdateArena", hidden).Invoke(court, null); yield return null; Capture("concept-result-scene");
        File.WriteAllText(Path.Combine(EvidenceDirectory, "concept-state.json"), "{\"concept\":true,\"runtimeRedesign\":false,\"asset\":\"AthleteNPC\",\"resultGames\":[" + string.Join(",", (int[])match.GetType().GetField("Games").GetValue(match)) + "]}");
        Act("Menu");
    }
    [Test] public void FixtureRestoresSnapshotsBackupsAndNestedFiles()
    {
        string root = Path.GetFullPath(Path.Combine(Application.dataPath, "../.hermes/local-game-data"));
        string path = Path.Combine(root, "polish-fixture-sentinel.json");
        Assert.That(File.Exists(path), Is.False);
        var nested = new PreferenceFixture();
        try
        {
            File.WriteAllBytes(path, new byte[] { 1, 7, 9 });
            File.WriteAllBytes(path + ".bak", new byte[] { 4, 2 });
            nested.IsolatePreferences();
            File.WriteAllText(path, "changed"); File.Delete(path + ".bak");
            Directory.CreateDirectory(path + ".dir"); File.WriteAllText(path + ".dir/new", "temporary");
            nested.RestorePreferences();
            Assert.That(File.ReadAllBytes(path), Is.EqualTo(new byte[] { 1, 7, 9 }), "Fixture protects prefs but destroys match snapshot bytes");
            Assert.That(File.ReadAllBytes(path + ".bak"), Is.EqualTo(new byte[] { 4, 2 }));
            Assert.That(Directory.Exists(path + ".dir"), Is.False, "Fixture leaks newly created directories");
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
            if (File.Exists(path + ".bak")) File.Delete(path + ".bak");
            if (Directory.Exists(path + ".dir")) Directory.Delete(path + ".dir", true);
        }
    }

    [Test] public void EvidenceDirectoryRefusesExistingOutput()
    {
        var method = typeof(PreferenceFixture).GetMethod("CreateEvidenceDirectory", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, "QA has no guard against historical evidence overwrite");
        string path = Path.Combine(Application.dataPath, "../.utmp/polish-evidence-" + Guid.NewGuid().ToString("N"));
        try
        {
            method.Invoke(null, new object[] { path });
            File.WriteAllText(Path.Combine(path, "keep.txt"), "keep");
            var error = Assert.Throws<TargetInvocationException>(() => method.Invoke(null, new object[] { path }));
            Assert.That(error.InnerException, Is.TypeOf<IOException>());
            Assert.That(File.ReadAllText(Path.Combine(path, "keep.txt")), Is.EqualTo("keep"));
        }
        finally { if (Directory.Exists(path)) Directory.Delete(path, true); }
    }
}
