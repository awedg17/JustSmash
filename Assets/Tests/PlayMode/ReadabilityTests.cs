using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class ReadabilityTests : PreferenceFixture
{
    [UnityTest, Category("Capture")] public IEnumerator CaptureShuttleLightingComparison()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null); ((Behaviour)court).enabled = false;
        var shuttle = GameObject.Find("Shuttle").transform;
        var renderers = shuttle.GetComponentsInChildren<MeshRenderer>();
        var materials = renderers.Select(x => x.sharedMaterial).ToArray();
        var unlit = new Material(Shader.Find("Unlit/Color")) { color = Color.white };
        var camera = Camera.main; var oldTarget = camera.targetTexture; var oldActive = RenderTexture.active;
        var target = RenderTexture.GetTemporary(1280, 720, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default, Mathf.Max(1, QualitySettings.antiAliasing));
        var image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
        var root = shuttle.position; var rotation = shuttle.rotation;
        var poses = new[] { new Vector3(0, 1.7f, 0), new Vector3(.25f, 1.4f, -3.5f), new Vector3(.25f, 1.4f, 4), new Vector3(0, 5, 2) };
        try
        {
            camera.targetTexture = target;
            System.IO.File.WriteAllText(EvidenceDirectory + "/camera.json", JsonUtility.ToJson(new CameraEvidence { position = camera.transform.position, rotation = camera.transform.rotation, fov = camera.fieldOfView, msaa = target.antiAliasing }));
            using (var csv = new System.IO.StreamWriter(EvidenceDirectory + "/projected.csv"))
            {
                csv.WriteLine("pose,tilt,corkX,corkY,skirtX,skirtY");
                for (int p = 0; p < poses.Length; p++) foreach (int tilt in new[] { 0, 90 })
                {
                    shuttle.SetPositionAndRotation(poses[p], Quaternion.Euler(tilt, 0, 0));
                    shuttle.GetComponent<TrailRenderer>().Clear();
                    var cork = camera.WorldToViewportPoint(shuttle.position);
                    var skirt = camera.WorldToViewportPoint(shuttle.TransformPoint(Vector3.up * .15f));
                    csv.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2:R},{3:R},{4:R},{5:R}", p, tilt, cork.x * 1280, (1 - cork.y) * 720, skirt.x * 1280, (1 - skirt.y) * 720));
                    foreach (string mode in new[] { "background", "baseline", "unlit" })
                    {
                        for (int i = 0; i < renderers.Length; i++) { renderers[i].enabled = mode != "background"; renderers[i].sharedMaterial = mode == "unlit" ? unlit : materials[i]; }
                        camera.Render(); RenderTexture.active = target;
                        image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); image.Apply();
                        System.IO.File.WriteAllBytes(EvidenceDirectory + "/" + p + "-" + tilt + "-" + mode + ".png", image.EncodeToPNG());
                    }
                }
            }
            Assert.That(shuttle.localScale, Is.EqualTo(Vector3.one));
            TestContext.WriteLine("Eight controlled poses; native unlit material is diagnostic only. No gameplay/visual acceptance implied.");
        }
        finally
        {
            shuttle.SetPositionAndRotation(root, rotation);
            for (int i = 0; i < renderers.Length; i++) { renderers[i].sharedMaterial = materials[i]; renderers[i].enabled = true; }
            camera.targetTexture = oldTarget; RenderTexture.active = oldActive;
            RenderTexture.ReleaseTemporary(target); UnityEngine.Object.Destroy(image); UnityEngine.Object.Destroy(unlit);
        }
    }
    [UnityTest] public IEnumerator ShuttleWhiteFacesStayVisibleUnderShadow()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null); ((Behaviour)court).enabled = false;
        var shuttle = GameObject.Find("Shuttle").transform;
        var renderers = shuttle.GetComponentsInChildren<MeshRenderer>();
        var materials = renderers.Select(x => x.sharedMaterial).ToArray();
        var root = shuttle.position; var rotation = shuttle.rotation;
        float outline = materials[0].GetFloat("_Outline");
        var camera = Camera.main; var oldTarget = camera.targetTexture; var oldActive = RenderTexture.active;
        var target = RenderTexture.GetTemporary(1280, 720, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default, Mathf.Max(1, QualitySettings.antiAliasing));
        var image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
        var white = new Material(Shader.Find("Unlit/Color")) { color = Color.white };
        try
        {
            camera.targetTexture = target;
            // Isolate from body/lettering occlusion; use real camera, light and shuttle geometry.
            shuttle.position = new Vector3(-2, 3.2f, -3.5f);
            int total = 0, dark = 0;
            foreach (int tilt in new[] { 0, 90, 180, 270 })
            {
                shuttle.rotation = Quaternion.Euler(tilt, 0, 0); shuttle.GetComponent<TrailRenderer>().Clear();
                foreach (var renderer in renderers) renderer.enabled = false;
                camera.Render(); RenderTexture.active = target; image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); image.Apply();
                var background = image.GetPixels32();
                foreach (var renderer in renderers) { renderer.enabled = true; renderer.sharedMaterial = white; }
                camera.Render(); image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); image.Apply();
                var reference = image.GetPixels32();
                for (int i = 0; i < renderers.Length; i++) renderers[i].sharedMaterial = materials[i];
                camera.Render(); image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); image.Apply();
                System.IO.File.WriteAllBytes(EvidenceDirectory + "/white-" + tilt + ".png", image.EncodeToPNG());
                // Native white mask has no outline. Isolate face lighting from intentional cork/skirt outline overlap.
                materials[0].SetFloat("_Outline", 0);
                camera.Render(); image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); image.Apply();
                var actual = image.GetPixels32();
                System.IO.File.WriteAllBytes(EvidenceDirectory + "/face-only-" + tilt + ".png", image.EncodeToPNG());
                materials[0].SetFloat("_Outline", outline);
                int count = 0, dim = 0;
                for (int i = 0; i < actual.Length; i++)
                {
                    if (reference[i].r < 250 || reference[i].g < 250 || reference[i].b < 250 ||
                        Mathf.Max(background[i].r, Mathf.Max(background[i].g, background[i].b)) > 180) continue;
                    count++;
                    if (Mathf.Min(actual[i].r, Mathf.Min(actual[i].g, actual[i].b)) < 200) dim++;
                }
                Assert.That(count, Is.GreaterThan(12), "Reference must identify fully covered shuttle face pixels");
                TestContext.WriteLine("tilt=" + tilt + " facePixels=" + count + " darkPixels=" + dim);
                total += count; dark += dim;
            }
            Assert.That(dark, Is.Zero, "Fully covered white shuttle faces must not turn dark with lighting/rotation; sampled=" + total);
            Assert.That(materials[0].shader.name, Is.EqualTo("JustSmash/CelCourt"));
            Assert.That(materials[0].GetFloat("_Outline"), Is.EqualTo(.012f));
            Assert.That(GameObject.Find("Court").GetComponent<Renderer>().sharedMaterial.GetFloat("_Unlit"), Is.Zero, "Court lighting must remain unchanged");
        }
        finally
        {
            shuttle.SetPositionAndRotation(root, rotation);
            for (int i = 0; i < renderers.Length; i++) { renderers[i].sharedMaterial = materials[i]; renderers[i].enabled = true; }
            camera.targetTexture = oldTarget; RenderTexture.active = oldActive;
            materials[0].SetFloat("_Outline", outline);
            RenderTexture.ReleaseTemporary(target); UnityEngine.Object.Destroy(image); UnityEngine.Object.Destroy(white);
        }
    }
    [Serializable] class CameraEvidence { public Vector3 position; public Quaternion rotation; public float fov; public int msaa; }
    [UnityTest] public IEnumerator ShuttleHasSolidOutlinedSilhouetteWithoutChangingPhysicsRoot()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
        ((Behaviour)court).enabled = false;
        var shuttle = GameObject.Find("Shuttle").transform;
        Assert.That(shuttle.localScale, Is.EqualTo(Vector3.one));
        Assert.That(shuttle.GetComponentsInChildren<Collider>().Length, Is.Zero);
        Assert.That(shuttle.Find("Cork").localScale, Is.EqualTo(Vector3.one * .075f));
        var skirt = shuttle.Find("FeatherSkirt");
        Assert.That(skirt, Is.Not.Null, "Eight thin spokes do not form a whole shuttle silhouette");
        Assert.That(skirt.localPosition, Is.EqualTo(Vector3.zero));
        Assert.That(skirt.localScale, Is.EqualTo(Vector3.one));
        var mesh = skirt.GetComponent<MeshFilter>().sharedMesh;
        Assert.That(mesh.triangles.Length, Is.EqualTo(96));
        Assert.That(mesh.bounds.min.y, Is.EqualTo(.025f).Within(.0001f));
        Assert.That(mesh.bounds.max.y, Is.EqualTo(.17f).Within(.0001f));
        Assert.That(mesh.bounds.size.x, Is.EqualTo(.16f).Within(.0001f));
        Assert.That(mesh.bounds.size.z, Is.EqualTo(.16f).Within(.0001f));
        Assert.That(mesh.normals.All(n => n.sqrMagnitude > .99f), Is.True);
        var renderer = skirt.GetComponent<MeshRenderer>();
        var material = renderer.sharedMaterial;
        Assert.That(renderer.enabled, Is.True);
        Assert.That(material.shader.name, Is.EqualTo("JustSmash/CelCourt"));
        Assert.That(material.GetFloat("_Outline"), Is.EqualTo(.012f));
        Assert.That(material.color.a, Is.EqualTo(1));
        Assert.That(shuttle.Find("Cork").GetComponent<Renderer>().sharedMaterial, Is.SameAs(material));
        Assert.That(Camera.main.transform.position, Is.EqualTo(new Vector3(.2f, 4f, -14.2f)));
        Assert.That(Camera.main.fieldOfView, Is.EqualTo(48));
        type.GetMethod("ActOnUi", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(court, new object[] { "Menu" });
        Assert.That(renderer.enabled, Is.False, "Menu must still hide whole shuttle");
        type.GetMethod("EnterTraining").Invoke(court, null);
        Assert.That(renderer.enabled, Is.True);
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        Assert.That(mesh == null, Is.True, "Generated skirt mesh must be released on reload");
        Assert.That(material == null, Is.True, "Shared shuttle material must be released on reload");
    }
}
