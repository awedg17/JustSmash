using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MobilePresentationTests : PreferenceFixture
{
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    string Evidence => EvidenceDirectory;
    Type type; UnityEngine.Object court;
    object Call(string name, params object[] args) => type.GetMethod(name).Invoke(court, args);
    IEnumerator Boot()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type);
    }
    void Capture(string name, int samples = 0)
    {
        Camera camera = Camera.main; var old = camera.targetTexture; var active = RenderTexture.active;
        var target = RenderTexture.GetTemporary(1280, 720, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default, samples == 0 ? Mathf.Max(1, QualitySettings.antiAliasing) : samples);
        try
        {
            camera.targetTexture = target; camera.Render(); RenderTexture.active = target;
            var image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); image.Apply();
            File.WriteAllBytes(Evidence + "/" + name + ".png", image.EncodeToPNG()); UnityEngine.Object.Destroy(image);
        }
        finally { camera.targetTexture = old; RenderTexture.active = active; RenderTexture.ReleaseTemporary(target); }
    }
    [UnityTest] public IEnumerator AuditCloseupStaysOnSubjectsSideOfNet()
    {
        yield return Boot(); Call("EnterTraining"); ((Behaviour)court).enabled = false;
        var player = GameObject.Find("Player").transform; var camera = Camera.main;
        var audit = camera.gameObject.AddComponent<PilotAuditCamera>(); audit.subject = player;
        var failures = new System.Collections.Generic.List<string>();
        try
        {
            foreach (float z in new[] { -.9f, -3.5f, .9f, 3.5f })
            {
                player.position = new Vector3(0, 0, z);
                typeof(PilotAuditCamera).GetMethod("LateUpdate", Hidden).Invoke(audit, null);
                if (Mathf.Sign(camera.transform.position.z) != Mathf.Sign(z) || Mathf.Abs(camera.transform.position.z) < .75f)
                    failures.Add("subject=" + z + " camera=" + camera.transform.position.z);
                Assert.That(Vector3.Angle(camera.transform.forward, player.position + Vector3.up - camera.transform.position), Is.LessThan(.01f));
            }
        }
        finally { UnityEngine.Object.Destroy(audit); }
        Assert.That(failures, Is.Empty, "Closeup lens must not cross net toward near-contact athlete: " + string.Join("; ", failures));
    }
    [UnityTest, Category("Capture")] public IEnumerator ProfileUnrecordedDoublesAt30And60()
    {
        Assert.That(Application.isBatchMode, Is.False);
        yield return CaptureAt1280x720(); yield return Boot();
        Call("ConfigureDiscipline", true); Call("BeginExhibition"); Call("AdvanceFlow"); Call("AdvanceFlow");
        var rows = new System.Collections.Generic.List<string> { "targetFps,frames,seconds,medianFrameMs,p95FrameMs,cpuCoreEquivalent,unityAllocatedBytes,maxDrawCalls,maxTriangles" };
        var process = System.Diagnostics.Process.GetCurrentProcess();
        foreach (int fps in new[] { 30, 60 })
        {
            Application.targetFrameRate = fps; yield return new WaitForSecondsRealtime(1);
            var gaps = new System.Collections.Generic.List<float>();
            float start = Time.realtimeSinceStartup, prior = start;
            double cpu = process.TotalProcessorTime.TotalSeconds;
            long memory = 0; int calls = 0, triangles = 0;
            while (Time.realtimeSinceStartup - start < 6)
            {
                if ((string)type.GetProperty("MatchState").GetValue(court) == "Serving") Call("Serve", true);
                yield return null;
                float now = Time.realtimeSinceStartup; gaps.Add((now - prior) * 1000); prior = now;
                memory = Math.Max(memory, UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong());
#if UNITY_EDITOR
                calls = Math.Max(calls, UnityEditor.UnityStats.drawCalls);
                triangles = Math.Max(triangles, UnityEditor.UnityStats.triangles);
#endif
            }
            double elapsed = Time.realtimeSinceStartup - start;
            double used = process.TotalProcessorTime.TotalSeconds - cpu;
            gaps.Sort(); Assert.That(gaps.Count, Is.GreaterThan(20)); Assert.That(memory, Is.GreaterThan(0));
            rows.Add(string.Join(",", new object[] { fps, gaps.Count, elapsed, gaps[gaps.Count / 2], gaps[(int)((gaps.Count - 1) * .95f)], used / elapsed, memory, calls, triangles }.Select(v => Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture))));
        }
        File.WriteAllLines(Evidence + "/pc-profile.csv", rows);
    }
    [UnityTest] public IEnumerator CaptureCurrentCourtAndContactPoses()
    {
        yield return Boot(); Call("EnterTraining"); ((Behaviour)court).enabled = false;
        Capture("court-before");
        var player = GameObject.Find("Player").transform;
        var driver = player.Find("AthleteVisual").GetComponent(AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.AthleteVisual")).First(t => t != null));
        Camera.main.transform.position = new Vector3(3.5f, 2.8f, -7.2f); Camera.main.transform.LookAt(player.position + Vector3.up);
        foreach (string shot in new[] { "Smash", "Clear", "Net", "Lift" })
        foreach (float side in new[] { -.4f, .4f })
        {
            var contact = player.TransformPoint(new Vector3(side, shot == "Net" || shot == "Lift" ? 1 : 2.1f, .35f));
            GameObject.Find("Shuttle").transform.position = contact;
            driver.GetType().GetMethod("Present").Invoke(driver, new object[] { shot, 0f, contact, Vector3.zero, 0f, 0f });
            yield return null; // Let Unity update skinned render matrices before manual camera capture.
            Capture("contact-" + shot + (side < 0 ? "-back" : "-fore"));
        }
    }
    [UnityTest] public IEnumerator CourtMatMeetsSurroundFloorWithoutRaisedPlatform()
    {
        yield return Boot(); Call("EnterTraining"); ((Behaviour)court).enabled = false;
        Capture("mat-edge");
        float courtTop = GameObject.Find("Court").GetComponent<Renderer>().bounds.max.y;
        float hallTop = GameObject.Find("HallFloor").GetComponent<Renderer>().bounds.max.y;
        Assert.That(courtTop - hallTop, Is.InRange(0, .01f), "Court must not be a 10cm platform above hall");
    }
    [UnityTest] public IEnumerator CaptureCoreCourtSinglesDoublesAtBothEnds()
    {
        yield return Boot();
        foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("BeginExhibition");
            var match = type.GetProperty("Match").GetValue(court); match.GetType().GetField("EndsSwapped").SetValue(match, swapped);
            Call("NextPoint"); Call("AdvanceFlow"); Call("AdvanceFlow");
            yield return null;
            string name = (doubles ? "doubles" : "singles") + (swapped ? "-far" : "-near");
            Assert.That(GameObject.Find("ArenaScoreText").GetComponent<TextMesh>().text, Does.Contain("GAME 1"));
            Capture(name);
        }
        ((Behaviour)court).enabled = false;
        Camera.main.orthographic = true; Camera.main.orthographicSize = 8.5f;
        Camera.main.transform.position = new Vector3(0, 20, 0); Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0); Capture("court-top");
        Camera.main.orthographic = false; Camera.main.transform.position = new Vector3(9, 4, -8); Camera.main.transform.LookAt(Vector3.zero); Capture("court-side");
    }
    [UnityTest] public IEnumerator ContactShadowsHaveSharedSoftEdgesAndReleaseOnReload()
    {
        yield return Boot(); Call("ConfigureDiscipline", true); Call("BeginExhibition"); Call("AdvanceFlow"); Call("AdvanceFlow");
        ((Behaviour)court).enabled = false;
        Camera.main.transform.position = new Vector3(2, 3, -6); Camera.main.transform.LookAt(new Vector3(0, 0, -4)); Capture("contact-shadow");
        var roots = new[] { "Player", "Opponent", "Partner", "OpponentPartner" }.Select(n => GameObject.Find(n).transform).ToArray();
        var material = roots[0].Find("ContactShadow").GetComponent<Renderer>().sharedMaterial;
        var texture = material.mainTexture as Texture2D;
        Assert.That(texture, Is.Not.Null, "Opaque flattened sphere cannot give a soft contact edge");
        Assert.That(texture.GetPixel(texture.width / 2, texture.height / 2).a, Is.GreaterThan(.2f));
        Assert.That(texture.GetPixel(0, 0).a, Is.Zero);
        Assert.That(texture.GetPixels().Select(c => c.a).Distinct().Count(), Is.GreaterThan(8));
        foreach (var root in roots)
        {
            var renderer = root.Find("ContactShadow").GetComponent<Renderer>();
            Assert.That(renderer.sharedMaterial, Is.SameAs(material));
            Assert.That(renderer.bounds.size.y, Is.LessThan(.001f));
            Assert.That(renderer.bounds.center.y, Is.InRange(.001f, .008f));
            Assert.That(renderer.shadowCastingMode, Is.EqualTo(UnityEngine.Rendering.ShadowCastingMode.Off));
        }
        yield return Boot(); yield return null;
        Assert.That(texture == null && material == null, Is.True, "Shadow resources leaked on reload");
    }
    [UnityTest] public IEnumerator CourtSurfaceIsMatteDetailedAndSurroundingsStayDark()
    {
        yield return Boot(); Call("EnterTraining"); ((Behaviour)court).enabled = false;
        Capture("material-gameplay");
        Camera.main.transform.position = new Vector3(0, 15, 0); Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0); Capture("material-top");
        var material = GameObject.Find("Court").GetComponent<Renderer>().sharedMaterial;
        Assert.That(material.shader.name, Is.EqualTo("JustSmash/CelCourt"), "Retain existing non-specular shader, not a pipeline migration");
        var color = material.color; Assert.That(color.g, Is.GreaterThan(color.r)); Assert.That(color.g, Is.GreaterThan(color.b));
        var texture = material.mainTexture as Texture2D;
        Assert.That(texture, Is.Not.Null, "Court needs subtle real sampled surface detail");
        var values = texture.GetPixels().Select(c => c.grayscale).ToArray();
        Assert.That(values.Max() - values.Min(), Is.InRange(.01f, .08f), "Detail must not compete with lines/shuttle");
        Assert.That(texture.mipmapCount, Is.GreaterThan(1)); Assert.That(material.GetFloat("_Flat"), Is.Zero);
        foreach (string name in new[] { "HallFloor", "HallBackWall", "HallSideWall" })
            Assert.That(GameObject.Find(name).GetComponent<Renderer>().sharedMaterial.color.grayscale, Is.LessThan(color.grayscale * .65f), name);
        var paint = GameObject.Find("CourtLine").GetComponent<Renderer>().sharedMaterial.color;
        Assert.That(paint.grayscale, Is.GreaterThan(.85f)); Assert.That(paint.g - paint.r, Is.LessThan(.03f), "Off-white paint, not mint");
        yield return Boot(); yield return null;
        Assert.That(texture == null, Is.True, "Owned court texture must be released on reload");
    }
    [UnityTest] public IEnumerator NetRendersContinuousCordsAtCloseView()
    {
        yield return Boot(); Call("EnterTraining"); ((Behaviour)court).enabled = false;
        var mesh = GameObject.Find("NetMesh").GetComponent<MeshFilter>().sharedMesh;
        Assert.That(mesh.normals.All(n => n.sqrMagnitude > .99f), Is.True, "Opposite triangles sharing vertices cancel net normals");
        Camera.main.transform.position = new Vector3(0, 1.15f, -3); Camera.main.transform.LookAt(new Vector3(0, 1.15f, 0));
        Capture("net-front-close");
        Camera.main.transform.position = new Vector3(0, 2.2f, -5); Camera.main.transform.LookAt(new Vector3(0, 1.2f, 0));
        Capture("net-near-no-msaa", 1); Capture("net-near-runtime-msaa");
        File.WriteAllText(Evidence + "/net-capture.json", "{\"runtimeMsaa\":" + QualitySettings.antiAliasing + "}");
    }
    [UnityTest] public IEnumerator NetTapeMeshAndPostsFollowMeasuredSagProfile()
    {
        yield return Boot(); Call("EnterTraining"); ((Behaviour)court).enabled = false;
        Capture("court-net");
        Camera.main.transform.position = new Vector3(0, 2.2f, -5); Camera.main.transform.LookAt(new Vector3(0, 1.2f, 0)); Capture("net-near");
        Camera.main.transform.position = new Vector3(4, 2, -4); Camera.main.transform.LookAt(new Vector3(0, 1.2f, 0)); Capture("net-side");
        var tape = GameObject.Find("NetTape");
        Assert.That(tape.GetComponent<LineRenderer>(), Is.Null, "Tape must stay in net plane, not face camera");
        var mesh = tape.GetComponent<MeshFilter>().sharedMesh;
        var vertices = mesh.vertices.Select(tape.transform.TransformPoint).ToArray();
        Assert.That(vertices.Min(v => v.x), Is.EqualTo(-3.05f).Within(.00001f));
        Assert.That(vertices.Max(v => v.x), Is.EqualTo(3.05f).Within(.00001f));
        foreach (var v in vertices)
        {
            float top = 1.524f + .026f * (v.x / 3.05f) * (v.x / 3.05f);
            Assert.That(top - v.y, Is.EqualTo(0).Within(.00001f).Or.EqualTo(.075f).Within(.00001f));
            Assert.That(v.z, Is.EqualTo(0).Within(.001f));
        }
        Assert.That(vertices.Any(v => Mathf.Abs(v.x) < .001f && Mathf.Abs(v.y - 1.524f) < .00001f), Is.True);
        foreach (var post in GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None).Where(t => t.name == "NetPost"))
            Assert.That(post.GetComponent<Renderer>().bounds.max.y, Is.EqualTo(1.55f).Within(.00001f));
        var net = GameObject.Find("NetMesh"); Assert.That(net.GetComponent<MeshFilter>(), Is.Not.Null);
        foreach (var v in net.GetComponent<MeshFilter>().sharedMesh.vertices.Select(net.transform.TransformPoint))
        {
            float top = 1.524f + .026f * (v.x / 3.05f) * (v.x / 3.05f);
            Assert.That(v.y, Is.InRange(top - .761f, top - .074f));
        }
    }
    [UnityTest] public IEnumerator PaintedCourtLinesMatchLegalEdgesAndGroundPlane()
    {
        yield return Boot();
        var floor = GameObject.Find("Court").GetComponent<Renderer>().bounds;
        Assert.That(floor.max.y, Is.EqualTo(0).Within(.001f), "Visible floor must meet simulation ground");
        var lines = GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None).Where(t => t.name == "Sideline" || t.name == "CourtLine" || t.name == "ServiceCentre").ToArray();
        Assert.That(lines.Length, Is.EqualTo(12));
        foreach (var line in lines)
        {
            Assert.That(line.GetComponent<LineRenderer>(), Is.Null, "Court paint must not face camera");
            var bounds = line.GetComponent<MeshRenderer>().bounds;
            Assert.That(bounds.min.y, Is.InRange(0f, .002f)); Assert.That(bounds.max.y, Is.LessThan(.003f));
            if (line.name == "Sideline")
            {
                Assert.That(bounds.size.x, Is.EqualTo(.04f).Within(.00001f));
                float edge = Mathf.Max(Mathf.Abs(bounds.min.x), Mathf.Abs(bounds.max.x));
                Assert.That(edge, Is.EqualTo(2.59f).Within(.00001f).Or.EqualTo(3.05f).Within(.00001f));
                Assert.That(bounds.min.z, Is.EqualTo(-6.7f).Within(.00001f)); Assert.That(bounds.max.z, Is.EqualTo(6.7f).Within(.00001f));
            }
            else if (line.name == "CourtLine")
            {
                Assert.That(bounds.size.z, Is.EqualTo(.04f).Within(.00001f));
                float near = Mathf.Min(Mathf.Abs(bounds.min.z), Mathf.Abs(bounds.max.z));
                float far = Mathf.Max(Mathf.Abs(bounds.min.z), Mathf.Abs(bounds.max.z));
                if (far < 2.1f) Assert.That(near, Is.EqualTo(1.98f).Within(.00001f));
                else Assert.That(far, Is.EqualTo(5.94f).Within(.00001f).Or.EqualTo(6.7f).Within(.00001f));
                Assert.That(bounds.min.x, Is.EqualTo(-3.05f).Within(.00001f)); Assert.That(bounds.max.x, Is.EqualTo(3.05f).Within(.00001f));
            }
            else
            {
                Assert.That(bounds.size.x, Is.EqualTo(.04f).Within(.00001f)); Assert.That(bounds.center.x, Is.Zero);
                Assert.That(Mathf.Min(Mathf.Abs(bounds.min.z), Mathf.Abs(bounds.max.z)), Is.EqualTo(1.98f).Within(.00001f));
                Assert.That(Mathf.Max(Mathf.Abs(bounds.min.z), Mathf.Abs(bounds.max.z)), Is.EqualTo(6.7f).Within(.00001f));
            }
        }
        foreach (bool doubles in new[] { false, true }) foreach (float xsign in new[] { -1f, 1f }) foreach (float zsign in new[] { -1f, 1f })
        {
            float width = doubles ? 3.05f : 2.59f;
            var corner = new Vector3(width * xsign, 0, 6.7f * zsign);
            Assert.That(type.GetMethod("InCourt").Invoke(null, new object[] { corner, doubles }), Is.EqualTo(true));
            Assert.That(type.GetMethod("InCourt").Invoke(null, new object[] { corner + Vector3.right * .001f * xsign, doubles }), Is.EqualTo(false));
            Assert.That(type.GetMethod("InCourt").Invoke(null, new object[] { corner + Vector3.forward * .001f * zsign, doubles }), Is.EqualTo(false));
        }
    }
    [UnityTest] public IEnumerator ServiceBoundaryPaintIsLegalForEitherCentreHalfAndBothEnds()
    {
        yield return Boot();
        foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true }) foreach (int server in new[] { 0, 1 }) foreach (int parity in new[] { 0, 1 })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("BeginExhibition");
            var match = type.GetProperty("Match").GetValue(court); var mt = match.GetType();
            mt.GetField("Server").SetValue(match, server); mt.GetField("EndsSwapped").SetValue(match, swapped);
            ((int[])mt.GetField("Points").GetValue(match))[server] = parity;
            float end = swapped ? -1 : 1, receiving = (server == 0 ? 1 : -1) * end;
            float wrongHalf = receiving * (parity == 0 ? 1 : -1);
            bool Legal(float x, float z) => (bool)type.GetMethod("LegalServiceLanding", Hidden).Invoke(court, new object[] { new Vector3(x, .04f, z * receiving) });
            Assert.That(Legal(.019f * wrongHalf, 3), Is.True, "Centre-line paint belongs to both service halves");
            Assert.That(Legal(.021f * wrongHalf, 3), Is.False);
            Assert.That(Legal(-wrongHalf, 1.98f), Is.True); Assert.That(Legal(-wrongHalf, 1.979f), Is.False);
            float back = doubles ? 5.94f : 6.7f;
            Assert.That(Legal(-wrongHalf, back), Is.True); Assert.That(Legal(-wrongHalf, back + .001f), Is.False);
        }
    }
    [UnityTest] public IEnumerator ArenaHasOfficialsAndLiveScoreboardWithoutSpectators()
    {
        yield return Boot();
        foreach (string name in new[] { "UmpireChair", "Umpire", "ServiceJudge", "ArenaScoreboard" })
        {
            var item = GameObject.Find(name);
            Assert.That(item, Is.Not.Null, name + " missing from actual Boot venue");
            Assert.That(item.GetComponentsInChildren<Renderer>().Any(r => r.enabled), Is.True, name + " has no visible geometry");
        }
        var audience = GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None).Where(t => t.name.StartsWith("Spectator-", StringComparison.Ordinal)).ToArray();
        Assert.That(audience, Is.Empty, "D-062 requires zero spectators");
        foreach (string name in new[] { "SpectatorStands", "Terrace", "Seat", "OfficialBakeSource" })
            Assert.That(GameObject.Find(name), Is.Null, name + " must not remain");
        Call("BeginExhibition"); yield return null;
        var score = GameObject.Find("ArenaScoreboard").GetComponentInChildren<TextMesh>();
        Assert.That(score.text, Does.Contain("0").And.Contain("JUST SMASH"));
        var match = type.GetProperty("Match").GetValue(court);
        match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
        yield return null;
        Assert.That(score.text, Does.Contain("1"), "Arena scoreboard must follow actual match score");
    }
    [UnityTest] public IEnumerator EmptyArenaReleasesOwnedMeshesAndMaterialsOnReload()
    {
        yield return Boot();
        var meshes = ((System.Collections.Generic.List<Mesh>)type.GetField("arenaMeshes", Hidden).GetValue(court)).ToArray();
        var materials = ((System.Collections.Generic.List<Material>)type.GetField("arenaMaterials", Hidden).GetValue(court)).ToArray();
        Assert.That(meshes.Length, Is.GreaterThan(0), "Seated official still needs baked geometry");
        Assert.That(GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None).Any(t => t.name.StartsWith("Spectator-", StringComparison.Ordinal)), Is.False);
        yield return Boot(); yield return null;
        Assert.That(meshes.All(m => m == null), Is.True, "Leaked arena meshes on scene reload");
        Assert.That(materials.All(m => m == null), Is.True, "Leaked arena materials on scene reload");
        Assert.That(GameObject.Find("ServiceJudge"), Is.Not.Null);
        Assert.That(GameObject.Find("SpectatorStands"), Is.Null);
    }
    [UnityTest] public IEnumerator UmpireSignalsWithConnectedVisibleArmNotFaceBlock()
    {
        yield return Boot();
        var umpire = GameObject.Find("Umpire").transform;
        var upper = umpire.Find("Root/Hips/Spine/Spine1/RightShoulder/RightArm");
        Assert.That(upper, Is.Not.Null, "Umpire is baked; separate signal block crosses face instead of moving arm");
        Assert.That(GameObject.Find("UmpireSignal"), Is.Null, "Detached signal geometry must not remain");
        var elbow = upper.Find("RightForeArm"); var hand = elbow.Find("RightHand");
        var skins = umpire.GetComponentsInChildren<SkinnedMeshRenderer>();
        Assert.That(skins.Any(s => s.bones.Contains(upper) && s.bones.Contains(hand)), Is.True);
        float a = Vector3.Distance(upper.position, elbow.position), b = Vector3.Distance(elbow.position, hand.position);
        var position = umpire.position; var idle = hand.position;
        type.GetField("officialSignalUntil", Hidden).SetValue(court, Time.time + 1.5f);
        type.GetMethod("UpdateArena", Hidden).Invoke(court, null);
        Assert.That(Vector3.Distance(idle, hand.position), Is.GreaterThan(.2f));
        Assert.That(Vector3.Distance(upper.position, elbow.position), Is.EqualTo(a).Within(.001f));
        Assert.That(Vector3.Distance(elbow.position, hand.position), Is.EqualTo(b).Within(.001f));
        Assert.That(umpire.position, Is.EqualTo(position));
        var head = umpire.Find("Root/Hips/Spine/Spine1/Neck/Head");
        Assert.That(Vector3.Distance(hand.position, head.position), Is.GreaterThan(.25f));
        Time.timeScale = 0; var frozen = hand.position; yield return new WaitForSecondsRealtime(.08f);
        Assert.That(hand.position, Is.EqualTo(frozen)); Time.timeScale = 1;
        yield return null; Camera.main.transform.position = position + new Vector3(2.2f, 1.4f, -2.2f);
        Camera.main.transform.LookAt(position + Vector3.up * .9f); Capture("umpire-connected-signal");
        type.GetField("officialSignalUntil", Hidden).SetValue(court, Time.time - 1);
        type.GetMethod("UpdateArena", Hidden).Invoke(court, null);
        Assert.That(Vector3.Distance(idle, hand.position), Is.LessThan(.001f));
    }
    [UnityTest] public IEnumerator GameWinningPointTriggersArenaReactionDespiteScoreReset()
    {
        yield return Boot();
        foreach (bool doubles in new[] { false, true })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("BeginExhibition"); Call("AdvanceFlow"); Call("AdvanceFlow");
            type.GetProperty("ReplayMode").SetValue(court, 2);
            var match = type.GetProperty("Match").GetValue(court); var mt = match.GetType();
            ((int[])mt.GetField("Points").GetValue(match))[0] = 14;
            yield return null;
            type.GetField("officialSignalUntil", Hidden).SetValue(court, Time.time - 1); // Ignore fixture setup; measure actual winning point only.
            Call("Serve", false);
            type.GetField("serviceFlight", Hidden).SetValue(court, false);
            type.GetField("velocity", Hidden).SetValue(court, Vector3.down);
            GameObject.Find("Shuttle").transform.position = new Vector3(-1, .045f, 3);
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[collision\] ground.*"));
            LogAssert.Expect(LogType.Log, "[scoring] point winner=0");
            type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
            yield return null;
            Assert.That((int)mt.GetField("GameNumber").GetValue(match), Is.EqualTo(2));
            Assert.That((int[])mt.GetField("Points").GetValue(match), Is.EqualTo(new[] { 0, 0 }));
            float remaining = (float)type.GetField("officialSignalUntil", Hidden).GetValue(court) - Time.time;
            Assert.That(remaining, Is.GreaterThan(1), "Game-winning point lost umpire signal after score reset to 0-0");
            var official = GameObject.Find("Umpire").transform;
            var signal = official.Find("Root/Hips/Spine/Spine1/RightShoulder/RightArm/RightForeArm/RightHand");
            Assert.That(official.InverseTransformPoint(signal.position).z, Is.GreaterThan(.45f), "Connected umpire arm must point into court");
            Assert.That(((Component)court).GetComponents<AudioSource>().Any(s => s.clip != null && s.clip.name.StartsWith("crowd-", StringComparison.Ordinal)), Is.False, "No crowd on game-winning point");
            Time.timeScale = 0; var frozen = signal.localRotation; yield return new WaitForSecondsRealtime(.12f);
            Assert.That(Quaternion.Angle(signal.localRotation, frozen), Is.LessThan(.01f));
            Time.timeScale = 1;
            if (!Application.isBatchMode)
            {
                yield return new WaitForEndOfFrame();
                var image = ScreenCapture.CaptureScreenshotAsTexture();
                File.WriteAllBytes(Evidence + "/game-reaction-after1-" + (doubles ? "doubles" : "singles") + ".png", image.EncodeToPNG());
                UnityEngine.Object.Destroy(image);
            }
            Call("BeginExhibition"); yield return null;
            Assert.That((float)type.GetField("officialSignalUntil", Hidden).GetValue(court), Is.LessThanOrEqualTo(Time.time), "New match inherits previous umpire signal");
        }
    }
    [UnityTest] public IEnumerator PointReactionsFollowTeamAndSurvivePauseSaveUntilNextServe()
    {
        yield return Boot();
        const string bone = "Root/Hips/Spine/Spine1/LeftShoulder/LeftArm/LeftForeArm/LeftHand";
        foreach (bool doubles in new[] { false, true }) foreach (int winner in new[] { 0, 1 })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("BeginExhibition");
            var match = type.GetProperty("Match").GetValue(court); var mt = match.GetType();
            mt.GetField("EndsSwapped").SetValue(match, winner == 1); Call("NextPoint");
            while (new[] { "Warmup", "Ready", "ChangeEnds" }.Contains((string)type.GetProperty("FlowStage").GetValue(court))) Call("AdvanceFlow");
            type.GetProperty("ReplayMode").SetValue(court, 2); Call("Serve", false);
            float end = winner == 1 ? -1 : 1;
            type.GetField("serviceFlight", Hidden).SetValue(court, false);
            type.GetField("lastHitter", Hidden).SetValue(court, winner);
            if (doubles) type.GetField("lastPlayer", Hidden).SetValue(court, winner);
            type.GetField("velocity", Hidden).SetValue(court, Vector3.down);
            GameObject.Find("Shuttle").transform.position = new Vector3(-1, .045f, (winner == 0 ? 3 : -3) * end);
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[collision\] ground.*"));
            LogAssert.Expect(LogType.Log, "[scoring] point winner=" + winner);
            type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
            var roots = (doubles ? new[] { "Player", "Opponent", "Partner", "OpponentPartner" } : new[] { "Player", "Opponent" }).Select(n => GameObject.Find(n).transform).ToArray();
            var positions = roots.Select(r => r.position).ToArray();
            yield return new WaitForSeconds(.4f);
            for (int id = 0; id < roots.Length; id++)
            {
                var root = roots[id]; var hand = root.Find("AthleteVisual/" + bone);
                float height = root.InverseTransformPoint(hand.position).y;
                Assert.That(height, id % 2 == winner ? Is.GreaterThan(1.65f) : Is.LessThan(1.35f), "Point reaction missing/wrong team: " + root.name);
                Assert.That(root.position, Is.EqualTo(positions[id]), "Reaction must not move simulation root: " + root.name);
                var palm = root.Find("AthleteVisual/Root/Hips/Spine/Spine1/RightShoulder/RightArm/RightForeArm/RightHand");
                Assert.That(Vector3.Distance(palm.position, palm.Find("RacketSocket").position), Is.LessThan(.09f));
            }
            var winningHand = roots[winner].Find("AthleteVisual/" + bone);
            Time.timeScale = 0; var frozen = winningHand.position;
            var model = roots[winner].Find("AthleteVisual"); var chest = model.Find("Root/Hips/Spine/Spine1");
            var pausedChest = chest.localRotation;
            yield return new WaitForSecondsRealtime(.12f);
            Assert.That(chest.localRotation, Is.EqualTo(pausedChest));
            Assert.That(model.GetComponent<Animation>().Cast<AnimationState>().Any(s => s.enabled), Is.False, "Automatic animation must not overwrite manual pose");
            Assert.That(winningHand.position, Is.EqualTo(frozen)); Time.timeScale = 1;
            string saved = (string)Call("CaptureMatch"); Call("RestoreMatch", saved); yield return null;
            Assert.That(roots[winner].InverseTransformPoint(winningHand.position).y, Is.GreaterThan(1.65f), "Save loses point reaction");
            if (!Application.isBatchMode)
            {
                yield return new WaitForEndOfFrame(); var image = ScreenCapture.CaptureScreenshotAsTexture();
                File.WriteAllBytes(Evidence + "/athlete-point-after1-" + (doubles ? "doubles" : "singles") + "-" + winner + ".png", image.EncodeToPNG());
                UnityEngine.Object.Destroy(image);
            }
            Call("NextPoint"); yield return null;
            foreach (var root in roots) Assert.That(root.InverseTransformPoint(root.Find("AthleteVisual/" + bone).position).y, Is.LessThan(1.5f), "Reaction leaks into next serve");
        }
    }
    [UnityTest] public IEnumerator ReactionsTrackOverturnedCallsGameIntervalsAndMatchResults()
    {
        yield return Boot();
        const string bone = "AthleteVisual/Root/Hips/Spine/Spine1/LeftShoulder/LeftArm/LeftForeArm/LeftHand";
        foreach (bool doubles in new[] { false, true }) foreach (int winner in new[] { 0, 1 })
        foreach (string scenario in new[] { "Review", "Game", "Match" })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("BeginExhibition");
            var match = type.GetProperty("Match").GetValue(court); var mt = match.GetType();
            if (scenario == "Match")
            {
                for (int i = 0; i < 15; i++) mt.GetMethod("AwardPoint").Invoke(match, new object[] { winner });
                type.GetField("flowGame", Hidden).SetValue(court, 2);
            }
            mt.GetField("EndsSwapped").SetValue(match, winner == 1);
            if (scenario != "Review") for (int i = 0; i < 14; i++) mt.GetMethod("AwardPoint").Invoke(match, new object[] { winner });
            Call("NextPoint");
            while (new[] { "Warmup", "Ready", "GameEnd", "Interval", "ChangeEnds" }.Contains((string)type.GetProperty("FlowStage").GetValue(court))) Call("AdvanceFlow");
            type.GetProperty("ReplayMode").SetValue(court, 2);
            if ((bool)type.GetProperty("HumanServe", Hidden).GetValue(court)) Call("Serve", false);
            else
            {
                type.GetField("serveReadyAt", Hidden).SetValue(court, Time.time);
                type.GetMethod("UpdateOpponent", Hidden).Invoke(court, null);
            }
            Assert.That(type.GetProperty("Flying").GetValue(court), Is.EqualTo(true), scenario + " must launch through legal server");
            yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
            type.GetField("serviceFlight", Hidden).SetValue(court, false);
            type.GetField("lastHitter", Hidden).SetValue(court, winner);
            if (doubles) type.GetField("lastPlayer", Hidden).SetValue(court, winner);
            type.GetField("velocity", Hidden).SetValue(court, Vector3.down);
            GameObject.Find("Shuttle").transform.position = new Vector3(-1, .045f, 3);
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[collision\] ground.*"));
            LogAssert.Expect(LogType.Log, "[scoring] point winner=" + winner);
            type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
            var roots = (doubles ? new[] { "Player", "Opponent", "Partner", "OpponentPartner" } : new[] { "Player", "Opponent" }).Select(n => GameObject.Find(n).transform).ToArray();
            void Check(int side)
            {
                for (int id = 0; id < roots.Length; id++) Assert.That(roots[id].InverseTransformPoint(roots[id].Find(bone).position).y,
                    id % 2 == side ? Is.GreaterThan(1.65f) : Is.LessThan(1.35f), scenario + " reaction " + roots[id].name);
            }
            yield return new WaitForSeconds(.4f); Check(winner);
            if (scenario == "Review")
            {
                // Controlled wrong announcement, preserving actual landing and valid pre-point history.
                JsonUtility.FromJsonOverwrite((string)type.GetField("beforePoint", Hidden).GetValue(court), match);
                mt.GetMethod("AwardPoint").Invoke(match, new object[] { 1 - winner });
                type.GetField("announcedIn", Hidden).SetValue(court, false);
                yield return null; Check(1 - winner);
                Assert.That(Call("ChallengeCall"), Is.EqualTo(true)); yield return null; Check(winner);
                Assert.That(Call("ChallengeCall"), Is.EqualTo(false));
            }
            else
            {
                string saved = (string)Call("CaptureMatch"); Call("RestoreMatch", saved); yield return null; Check(winner);
                type.GetProperty("ReplayMode").SetValue(court, 1);
                Assert.That(Call("BeginReplay"), Is.EqualTo(true));
                float frozen = (float)type.GetField("pointWait", Hidden).GetValue(court);
                type.GetMethod("ContinuePoint", Hidden).Invoke(court, new object[] { 5f });
                Assert.That((float)type.GetField("pointWait", Hidden).GetValue(court), Is.EqualTo(scenario == "Match" ? 0 : frozen));
                yield return null; Call("SkipReplay"); yield return null; Check(winner);
                type.GetMethod("ContinuePoint", Hidden).Invoke(court, new object[] { 3.01f }); yield return null;
                Assert.That((string)type.GetProperty("FlowStage").GetValue(court), Is.EqualTo(scenario == "Match" ? "MatchEnd" : "GameEnd"));
                if (scenario == "Match") Check(winner);
                else foreach (var root in roots) Assert.That(root.InverseTransformPoint(root.Find(bone).position).y, Is.LessThan(1.5f), "Celebration leaks into game preparation");
            }
        }
    }
    [UnityTest] public IEnumerator ArenaFontRendersLightGlyphsAndHonorsDepth()
    {
        yield return Boot();
        var source = GameObject.Find("ArenaScoreText").GetComponent<TextMesh>();
        var item = new GameObject("FontRenderProbe"); item.layer = 30;
        var label = item.AddComponent<TextMesh>(); label.font = source.font; label.text = "JUST SMASH";
        label.fontSize = 64; label.characterSize = .12f; label.anchor = TextAnchor.MiddleCenter; label.color = Color.white;
        item.GetComponent<Renderer>().sharedMaterial = source.GetComponent<Renderer>().sharedMaterial;
        var cameraObject = new GameObject("FontProbeCamera"); var camera = cameraObject.AddComponent<Camera>();
        camera.transform.position = new Vector3(0,0,-10); camera.orthographic = true; camera.orthographicSize = 2;
        camera.cullingMask = 1 << 30; camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = Color.black;
        var target = RenderTexture.GetTemporary(256,128,24); camera.targetTexture = target;
        var image = new Texture2D(256,128,TextureFormat.RGB24,false); var previous = RenderTexture.active;
        GameObject blocker = null;
        int LightPixels()
        {
            camera.Render(); RenderTexture.active = target; image.ReadPixels(new Rect(0,0,256,128),0,0); image.Apply();
            return image.GetPixels().Count(c => c.r > .7f && c.g > .7f && c.b > .7f);
        }
        try
        {
            yield return null;
            Assert.That(LightPixels(), Is.GreaterThan(200), "Font alpha atlas must render white glyphs, not black RGB texels");
            blocker = GameObject.CreatePrimitive(PrimitiveType.Cube); blocker.layer = 30;
            blocker.transform.position = new Vector3(0,0,-1); blocker.transform.localScale = new Vector3(20,20,.1f);
            blocker.GetComponent<Renderer>().material.color = Color.black;
            Assert.That(LightPixels(), Is.EqualTo(0), "World lettering must not draw through athletes/geometry");
        }
        finally
        {
            camera.targetTexture = null; RenderTexture.active = previous; RenderTexture.ReleaseTemporary(target);
            UnityEngine.Object.Destroy(item); UnityEngine.Object.Destroy(cameraObject); UnityEngine.Object.Destroy(image);
            if (blocker != null) UnityEngine.Object.Destroy(blocker);
        }
    }
    [UnityTest] public IEnumerator PhysicalScoreboardUsesReadableAreaWithoutOverflow()
    {
        yield return Boot(); yield return null;
        var board = GameObject.Find("ArenaScoreboard").GetComponent<Renderer>().bounds;
        var text = GameObject.Find("ArenaScoreText").GetComponent<Renderer>().bounds;
        Assert.That(text.size.x, Is.InRange(board.size.x * .60f, board.size.x * .95f), "Score must fill board, not remain a tiny central patch");
        Assert.That(text.size.y, Is.LessThan(board.size.y * .95f));
    }
    [UnityTest] public IEnumerator PhysicalScoreboardStaysReadableFromActualMatchCamera()
    {
        yield return Boot(); Call("BeginExhibition"); Call("AdvanceFlow"); Call("AdvanceFlow");
        var match = type.GetProperty("Match").GetValue(court);
        ((int[])match.GetType().GetField("Points").GetValue(match))[0] = 14;
        ((int[])match.GetType().GetField("Points").GetValue(match))[1] = 13;
        yield return null;
        var text = GameObject.Find("ArenaScoreText").GetComponent<Renderer>().bounds;
        var points = new Vector3[8];
        for (int i = 0; i < 8; i++) points[i] = Camera.main.WorldToViewportPoint(new Vector3(
            (i & 1) == 0 ? text.min.x : text.max.x, (i & 2) == 0 ? text.min.y : text.max.y, (i & 4) == 0 ? text.min.z : text.max.z));
        float height = (points.Max(p => p.y) - points.Min(p => p.y)) * 471;
        Assert.That(height, Is.GreaterThanOrEqualTo(48), "Three scoreboard rows too small at recorded 471px viewport; height=" + height);
        Assert.That(points.All(p => p.x > 0 && p.x < 1 && p.y > 0 && p.y < 1 && p.z > 0), Is.True);
        var board = GameObject.Find("ArenaScoreboard").GetComponent<Renderer>().bounds;
        Assert.That(text.size.x, Is.InRange(board.size.x * .6f, board.size.x * .95f));
        Assert.That(text.size.y, Is.LessThan(board.size.y * .95f));
        if (!Application.isBatchMode)
        {
            yield return new WaitForEndOfFrame();
            var image = ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(Evidence + "/scoreboard-readable-after1.png", image.EncodeToPNG());
            UnityEngine.Object.Destroy(image);
        }
    }
    [UnityTest] public IEnumerator SeatedOfficialHasKneesForwardFeetDownAndHandsOnLap()
    {
        yield return Boot();
        var pose = type.GetMethod("PoseSeatedOfficial", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(pose, Is.Not.Null, "Seated pose must be defined in world directions, not arbitrary bind Euler axes");
        var model = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("AthleteNPC"));
        try
        {
            pose.Invoke(null, new object[] { model.transform });
            var hips = model.transform.Find("Root/Hips");
            foreach (string side in new[] { "Left", "Right" })
            {
                var thigh = hips.Find(side + "UpLeg"); var knee = thigh.Find(side + "Leg"); var foot = knee.Find(side + "Foot");
                var hand = hips.Find("Spine/Spine1/" + side + "Shoulder/" + side + "Arm/" + side + "ForeArm/" + side + "Hand");
                Assert.That(knee.position.z - thigh.position.z, Is.GreaterThan(.25f), "Thigh must point toward knees, not into bench");
                Assert.That(foot.position.y, Is.LessThan(knee.position.y - .25f));
                Assert.That(foot.position.y, Is.InRange(-.04f, .20f), "Feet rest on terrace floor");
                Assert.That(hand.position.y, Is.InRange(hips.position.y - .12f, hips.position.y + .20f), "Hands rest on lap");
            }
        }
        finally { UnityEngine.Object.Destroy(model); }
    }
    [UnityTest] public IEnumerator VenueAndTrailUseReadableLowCostGeometry()
    {
        yield return Boot();
        Assert.That(GameObject.Find("HallFloor"), Is.Not.Null, "Outside court still empty");
        Assert.That(GameObject.Find("HallBackWall"), Is.Not.Null);
        var light = GameObject.Find("CourtLight").GetComponent<Light>();
        Assert.That(light.type, Is.EqualTo(LightType.Directional)); Assert.That(light.shadows, Is.EqualTo(LightShadows.Soft));
        Assert.That(GameObject.Find("Shuttle").GetComponent<TrailRenderer>().time, Is.LessThanOrEqualTo(.15f));
        foreach (string name in new[] { "Player", "Opponent" })
        {
            var root = GameObject.Find(name).transform;
            var shadow = root.Find("ContactShadow"); Assert.That(shadow, Is.Not.Null);
            Assert.That(shadow.GetComponent<Renderer>().bounds.center.y, Is.LessThan(.04f));
        }
    }
}
