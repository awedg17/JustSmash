using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class M1GapTests : PreferenceFixture
{
    Type type;
    UnityEngine.Object court;
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    IEnumerator Boot()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
    }
    object Call(string method, params object[] args) => type.GetMethod(method).Invoke(court, args);
    void Set(string field, object value) => type.GetField(field, Hidden).SetValue(court, value);
    T Get<T>(string name) => (T)type.GetProperty(name).GetValue(court);
    void Capture(string name)
    {
        var camera = Camera.main; var rt = new RenderTexture(1280, 720, 24);
        var old = RenderTexture.active;
        camera.targetTexture = rt; camera.Render(); RenderTexture.active = rt;
        var image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); image.Apply();
        System.IO.Directory.CreateDirectory(EvidenceDirectory);
        System.IO.File.WriteAllBytes(EvidenceDirectory + "/" + name + ".png", image.EncodeToPNG());
        camera.targetTexture = null; RenderTexture.active = old;
        UnityEngine.Object.Destroy(rt); UnityEngine.Object.Destroy(image);
    }
    [UnityTest] public IEnumerator CameraWidensForJumpAndRecoversWithBothAthletesVisible()
    {
        yield return Boot();
        var camera = Camera.main; var start = camera.transform.position;
        Capture("camera-standard"); Call("Jump");
        yield return new WaitForSeconds(.3f);
        Assert.That(camera.transform.position.y, Is.GreaterThan(start.y + .1f));
        Assert.That(camera.transform.position.z, Is.LessThan(start.z - .1f));
        foreach (string name in new[] { "Player", "Opponent" })
        {
            var p = camera.WorldToViewportPoint(GameObject.Find(name).transform.position + Vector3.up);
            Assert.That(p.z, Is.GreaterThan(0)); Assert.That(p.x, Is.InRange(.05f, .95f)); Assert.That(p.y, Is.InRange(.05f, .95f));
        }
        Capture("camera-jump"); yield return new WaitForSeconds(1.5f);
        Assert.That(Vector3.Distance(camera.transform.position, start), Is.LessThan(.04f));
        Capture("camera-recovered");
    }
    [Test] public void LockedBackupReproducesReplaceErrorWithoutLosingOldOrNewData()
    {
        string path = System.IO.Path.Combine(EvidenceDirectory, "replace-probe.txt");
        System.IO.File.WriteAllText(path, "old");
        System.IO.File.WriteAllText(path + ".tmp", "new");
        System.IO.File.WriteAllText(path + ".bak", "older");
        System.IO.IOException error;
        using (var held = new System.IO.FileStream(path + ".bak", System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
        {
            error = Assert.Throws<System.IO.IOException>(() => System.IO.File.Replace(path + ".tmp", path, path + ".bak"));
            Assert.That(System.IO.File.ReadAllText(path), Is.EqualTo("old"));
            Assert.That(System.IO.File.ReadAllText(path + ".tmp"), Is.EqualTo("new"));
            Assert.That(System.IO.File.ReadAllText(path + ".bak"), Is.EqualTo("older"));
        }
        Assert.That(error.Message, Is.EqualTo("Unable to remove the file to be replaced"));
        Assert.That(error.HResult, Is.EqualTo(unchecked((int)0x80070497)));
        TestContext.WriteLine("Synthetic backup lock: HResult=0x" + error.HResult.ToString("X8") + " coreMvid=" + typeof(System.IO.File).Module.ModuleVersionId);
        System.IO.File.Replace(path + ".tmp", path, path + ".bak");
        Assert.That(System.IO.File.ReadAllText(path), Is.EqualTo("new"));
        Assert.That(System.IO.File.ReadAllText(path + ".bak"), Is.EqualTo("old"));
        Assert.That(System.IO.File.Exists(path + ".tmp"), Is.False);
    }
    [UnityTest] public IEnumerator LoadingVisibilityIsReadOnlyEvenWhenBackupsAreOpen()
    {
        yield return Boot();
        var dataType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.LocalGameData")).First(t => t != null);
        string prefs = System.IO.Path.Combine((string)dataType.GetProperty("Root").GetValue(null), "prefs");
        System.IO.File.WriteAllText(System.IO.Path.Combine(prefs, "trailBrightness.txt"), ".35");
        System.IO.File.WriteAllText(System.IO.Path.Combine(prefs, "shadowBrightness.txt"), ".25");
        System.IO.File.WriteAllText(System.IO.Path.Combine(prefs, "trailTint.txt"), "FFFF00");
        System.IO.File.WriteAllText(System.IO.Path.Combine(prefs, "shadowTint.txt"), "000000");
        var before = System.IO.Directory.GetFiles(prefs).ToDictionary(p => p, System.IO.File.ReadAllBytes);
        var held = new System.Collections.Generic.List<System.IO.FileStream>();
        try
        {
            foreach (string key in new[] { "trailBrightness", "shadowBrightness", "trailTint", "shadowTint" })
                held.Add(new System.IO.FileStream(System.IO.Path.Combine(prefs, key + ".txt.bak"), System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite));
            Assert.DoesNotThrow(() => type.GetMethod("LoadVisibility", Hidden).Invoke(court, null), "Loading settings must not write preferences");
            Assert.That(GameObject.Find("Shuttle").GetComponent<TrailRenderer>().startColor.a, Is.EqualTo(.85f).Within(.001f), "Existing low-brightness migration stays in memory");
            Assert.That(GameObject.Find("ShuttleShadow").GetComponent<Renderer>().sharedMaterial.color.r, Is.EqualTo(.75f).Within(.001f));
        }
        finally { foreach (var stream in held) stream.Dispose(); }
        Assert.That(System.IO.Directory.GetFiles(prefs), Is.EquivalentTo(before.Keys));
        foreach (var pair in before) Assert.That(System.IO.File.ReadAllBytes(pair.Key), Is.EqualTo(pair.Value), pair.Key);
        Call("ConfigureVisibility", .8f, .4f, Color.white, Color.black);
        Assert.That((float)dataType.GetMethod("GetFloat").Invoke(null, new object[] { "trailBrightness", 0f }), Is.EqualTo(.8f));
        Assert.That((float)dataType.GetMethod("GetFloat").Invoke(null, new object[] { "shadowBrightness", 0f }), Is.EqualTo(.4f));
    }
    [UnityTest] public IEnumerator VisibilityControlsChangeOnlyVfxAndPersist()
    {
        yield return Boot();
        Assert.That(type.GetMethod("ConfigureVisibility"), Is.Not.Null, "Trail/shadow controls missing");
        var before = Get<Vector3>("ShuttleVelocity");
        Call("ConfigureVisibility", .8f, .25f, Color.yellow, Color.black);
        Assert.That(GameObject.Find("Shuttle").GetComponent<TrailRenderer>().startColor.a, Is.EqualTo(.8f).Within(.001f));
        Assert.That(GameObject.Find("ShuttleShadow").GetComponent<Renderer>().sharedMaterial.color.r, Is.EqualTo(.75f).Within(.01f));
        Assert.That(Get<Vector3>("ShuttleVelocity"), Is.EqualTo(before));
        Assert.Throws<TargetInvocationException>(() => Call("ConfigureVisibility", float.NaN, .2f, Color.white, Color.black));
        try { Call("ConfigureVisibility", .35f, .9f, Color.white, new Color(.1f, .14f, .14f)); }
        catch (TargetInvocationException error) when (error.InnerException is System.IO.IOException)
        {
            // Diagnostic only: retain failure and file metadata, never preference contents.
            var dataType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.LocalGameData")).First(t => t != null);
            string prefs = System.IO.Path.Combine((string)dataType.GetProperty("Root").GetValue(null), "prefs");
            var lines = new System.Collections.Generic.List<string> { "HResult=" + error.InnerException.HResult, error.InnerException.ToString() };
            foreach (string file in System.IO.Directory.GetFiles(prefs))
            {
                var info = new System.IO.FileInfo(file);
                lines.Add(info.Name + " | bytes=" + info.Length + " | attributes=" + info.Attributes);
            }
            System.IO.File.WriteAllLines(EvidenceDirectory + "/preference-replace-failure.txt", lines);
            throw;
        }
    }
    [UnityTest] public IEnumerator TesterFormWritesStructuredLocalRecordAndRejectsInvalidScores()
    {
        yield return Boot();
        Assert.That(type.GetMethod("SubmitTesterReport"), Is.Not.Null, "Tester form missing");
        type.GetMethod("ToggleTester", Hidden).Invoke(court, null);
        Assert.That(Time.timeScale, Is.EqualTo(0));
        var point = ((Rect)type.GetProperty("StickRect", Hidden).GetValue(court)).center + Vector2.right * 60;
        type.GetMethod("PointerDown", Hidden).Invoke(court, new object[] { 7, point });
        Assert.That(Get<Vector2>("Analog"), Is.EqualTo(Vector2.zero));
        yield return null;
        if (!Application.isBatchMode)
        {
            yield return new WaitForEndOfFrame();
            var screenshot = ScreenCapture.CaptureScreenshotAsTexture();
            System.IO.File.WriteAllBytes(EvidenceDirectory + "/tester-ui.png", screenshot.EncodeToPNG());
            UnityEngine.Object.Destroy(screenshot);
        }
        type.GetMethod("ToggleTester", Hidden).Invoke(court, null);
        Assert.That(Time.timeScale, Is.EqualTo(1));
        string path = (string)Call("SubmitTesterReport", "Timing", true, 2, 4, 3, "test newline\nquote\"");
        var text = System.IO.File.ReadAllLines(path).Last();
        Assert.That(text, Does.Contain("\"missReason\":\"Timing\""));
        Assert.That(text, Does.Contain("\"leftComfort\":2"));
        Assert.That(text, Does.Contain("test newline\\nquote\\\""));
        int count = System.IO.File.ReadAllLines(path).Length;
        Assert.Throws<TargetInvocationException>(() => Call("SubmitTesterReport", "Timing", false, 0, 4, 3, ""));
        Assert.That(System.IO.File.ReadAllLines(path).Length, Is.EqualTo(count));
    }
    [UnityTest] public IEnumerator AnimationRespondsToMotionBrakeLungeAndStrokeWithoutRootTeleport()
    {
        yield return Boot();
        Assert.That(type.GetProperty("AnimationState"), Is.Not.Null, "Motion states missing");
        Assert.That(Get<string>("AnimationState"), Is.EqualTo("Idle"));
        var torso = GameObject.Find("Player").transform.Find("Hips/Spine");
        Call("SetAnalog", Vector2.up); yield return new WaitForSeconds(.15f);
        Assert.That(Get<string>("AnimationState"), Is.EqualTo("Forward"));
        Assert.That(Quaternion.Angle(torso.localRotation, Quaternion.identity), Is.GreaterThan(1));
        Call("SetAnalog", Vector2.zero); yield return new WaitForSeconds(.05f);
        Assert.That(Get<string>("AnimationState"), Is.EqualTo("Braking"));
        yield return new WaitForSeconds(.4f);
        foreach (string shot in new[] { "Smash", "Net" })
        {
            Call("Feed", "Net");
            var player = GameObject.Find("Player").transform;
            GameObject.Find("Shuttle").transform.position = player.position + new Vector3(.4f, shot == "Smash" ? 2.1f : 1, .35f);
            Set("velocity", Vector3.zero); Call("BeginShot", shot);
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\].*"));
            var before = player.position; Assert.That(Call("ReleaseShot"), Is.EqualTo(true));
            Assert.That(player.position, Is.EqualTo(before)); yield return null;
            Assert.That(Get<string>("AnimationState"), Is.EqualTo(shot == "Smash" ? "Overhead" : "Lunge"));
            yield return new WaitForSeconds(.1f);
            if (shot == "Net")
                Assert.That(Mathf.Abs(player.Find("Hips/LegLeft").localEulerAngles.x - player.Find("Hips/LegRight").localEulerAngles.x), Is.GreaterThan(20f), "Lunge needs visible front/rear leg split");
            Capture("animation-" + shot.ToLowerInvariant());
            Set("flying", false); yield return new WaitForSeconds(.35f);
            Assert.That(Get<string>("AnimationState"), Is.EqualTo("Recovery"));
            yield return new WaitForSeconds(.4f);
        }
    }
    [UnityTest] public IEnumerator AllStrokesAcrossSixZonesAndChargeLevelsCompleteFlight()
    {
        yield return Boot();
        var player = GameObject.Find("Player").transform;
        var shuttle = GameObject.Find("Shuttle").transform;
        int cases = 0, net = 0;
        var watch = System.Diagnostics.Stopwatch.StartNew();
        foreach (string shot in new[] { "Smash", "Clear", "Drop", "Net", "Lift" })
        foreach (float x in new[] { -1f, 1f }) foreach (float y in new[] { -1f, 0f, 1f })
        foreach (float charge in new[] { 0f, .4f, .8f })
        {
            Call("Feed", "Net"); player.position = new Vector3(0, 0, shot == "Net" ? -.9f : -3.5f);
            shuttle.position = player.position + new Vector3(.3f, shot == "Net" || shot == "Lift" ? 1 : 2.1f, .35f);
            Set("velocity", Vector3.zero); Call("SetAnalog", new Vector2(x, y));
            Call("BeginShot", shot); Set("chargedAt", Time.time - charge);
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\].*"));
            Assert.That(Call("ReleaseShot"), Is.EqualTo(true), shot);
            Call("SetAnalog", Vector2.zero);
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[collision\] ground.*"));
            int ticks = 0;
            while (Get<bool>("Flying") && ticks++ < 300)
                type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
            Assert.That(Get<bool>("Flying"), Is.False, shot + " flight stuck");
            Assert.That(float.IsFinite(shuttle.position.x + shuttle.position.y + shuttle.position.z), Is.True);
            Assert.That(shuttle.position.y, Is.EqualTo(.04f).Within(.001f));
            if (((string)type.GetField("feedback", Hidden).GetValue(court)).StartsWith("Net |")) net++;
            cases++;
        }
        watch.Stop();
        System.IO.File.WriteAllText(EvidenceDirectory + "/stroke-stress.json", "{\"controlledFlights\":" + cases + ",\"netContacts\":" + net + ",\"hostMilliseconds\":" + watch.Elapsed.TotalMilliseconds.ToString(System.Globalization.CultureInfo.InvariantCulture) + "}");
        Assert.That(cases, Is.EqualTo(90));
    }
    [UnityTest] public IEnumerator ShuttleBelowNetFaultsHitterOnceInEitherDirection()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        foreach (int hitter in new[] { 0, 1 })
        {
            Call("StartMatch"); Call("Serve", true); Set("lastHitter", hitter);
            var shuttle = GameObject.Find("Shuttle").transform;
            float sign = hitter == 0 ? 1 : -1;
            shuttle.position = new Vector3(2, .4f, -.2f * sign); Set("velocity", new Vector3(0, 0, 40 * sign));
            LogAssert.Expect(LogType.Log, "[scoring] fault shuttle below net player=" + hitter);
            type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
            Assert.That(Get<string>("MatchState"), Is.EqualTo("Point")); Assert.That(Get<bool>("Flying"), Is.False);
            type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
            var match = Get<object>("Match"); var points = (int[])match.GetType().GetField("Points").GetValue(match);
            Assert.That(points[hitter], Is.Zero); Assert.That(points[1 - hitter], Is.EqualTo(1));
            Assert.That(Call("ChallengeCall"), Is.False);
        }
    }
    [UnityTest] public IEnumerator NetFaultRespectsLocalTopAndBottom()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        var athlete = GameObject.Find("Player").transform; athlete.position = new Vector3(0, 0, -3);
        var touch = type.GetMethod("TouchesNet", Hidden);
        var contact = GameObject.CreatePrimitive(PrimitiveType.Cube); contact.transform.localScale = Vector3.one * .002f;
        try
        {
            foreach (float x in new[] { -3f, 0f, 3f })
            {
                float top = 1.524f + .026f * (x / 3.05f) * (x / 3.05f);
                foreach (float y in new[] { top - .003f, top + .003f, top - .757f, top - .763f })
                {
                    contact.transform.position = new Vector3(x, y, 0);
                    Assert.That((bool)touch.Invoke(court, new object[] { athlete, contact.transform }), Is.EqualTo(y < top && y > top - .76f), "Racket bounds x=" + x + " y=" + y);
                }
            }
        }
        finally { UnityEngine.Object.Destroy(contact); }
    }
    [UnityTest] public IEnumerator SaggedNetCrossingAndLandingPredictorAgreeAtCentreAndPosts()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        var shuttle = GameObject.Find("Shuttle").transform;
        var tick = type.GetMethod("FixedUpdate", Hidden);
        foreach (float x in new[] { -3f, 0f, 3f }) foreach (float sign in new[] { -1f, 1f })
        foreach (float speed in new[] { 40f, 300f }) foreach (float clearance in new[] { -.003f, .003f })
        {
            Call("Feed", "Net");
            float top = 1.524f + .026f * (x / 3.05f) * (x / 3.05f);
            var velocity = new Vector3(0, .2f, speed * sign);
            var next = (Vector3)type.GetMethod("StepVelocity").Invoke(null, new object[] { velocity, Time.fixedDeltaTime });
            float crossingTime = .2f / Mathf.Abs(next.z);
            shuttle.position = new Vector3(x, top + clearance - next.y * crossingTime, -.2f * sign);
            Set("velocity", velocity); type.GetMethod("MarkLanding", Hidden).Invoke(court, null);
            var predicted = GameObject.Find("ShuttleLandingShadow").transform.position;
            tick.Invoke(court, null);
            Assert.That(Mathf.Sign(Get<Vector3>("ShuttleVelocity").z) == sign, Is.EqualTo(clearance > 0), "Wrong net crossing x=" + x + " clearance=" + clearance);
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[collision\] ground.*"));
            for (int i = 0; Get<bool>("Flying") && i < 1000; i++) tick.Invoke(court, null);
            Assert.That(Get<bool>("Flying"), Is.False);
            Assert.That(Vector2.Distance(new Vector2(predicted.x, predicted.z), new Vector2(shuttle.position.x, shuttle.position.z)), Is.LessThan(.0001f));
        }
    }
    [UnityTest] public IEnumerator HighVelocitySweptNetGroundAndFixedTickAreStable()
    {
        yield return Boot();
        var shuttle = GameObject.Find("Shuttle").transform;
        var tick = type.GetMethod("FixedUpdate", Hidden);
        var watch = System.Diagnostics.Stopwatch.StartNew();
        int count = 0;
        foreach (float speed in new[] { 40f, 80f, 150f, 300f })
        foreach (float sign in new[] { -1f, 1f })
        for (int repeat = 0; repeat < 100; repeat++)
        {
            Call("Feed", "Net"); shuttle.position = new Vector3(0, 1, -.2f * sign);
            Set("velocity", new Vector3(0, 0, speed * sign)); tick.Invoke(court, null);
            Assert.That(Get<Vector3>("ShuttleVelocity").z * sign, Is.LessThan(0));
            Assert.That(shuttle.position.z * sign, Is.LessThan(0));
            count++;
        }
        Call("Feed", "Net"); shuttle.position = new Vector3(0, .1f, -2);
        Set("velocity", Vector3.down * 150);
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[collision\] ground.*"));
        tick.Invoke(court, null);
        Assert.That(shuttle.position.y, Is.EqualTo(.04f)); Assert.That(Get<bool>("Flying"), Is.False);
        watch.Stop();
        System.IO.File.WriteAllText(EvidenceDirectory + "/velocity-stress.json", "{\"netCrossings\":" + count + ",\"maxMetersPerSecond\":300,\"hostMilliseconds\":" + watch.Elapsed.TotalMilliseconds.ToString(System.Globalization.CultureInfo.InvariantCulture) + "}");
    }
    [UnityTest] public IEnumerator DebugToggleDoesNotExposeAimPreviewByDefault()
    {
        yield return Boot();
        Assert.That(type.GetProperty("DebugVisible"), Is.Not.Null, "Dedicated debug toggle missing");
        Assert.That(Get<bool>("DebugVisible"), Is.False);
        Call("SetDebugVisible", true); Call("Feed", "Lob");
        yield return new WaitForFixedUpdate(); yield return null;
        Assert.That(GameObject.Find("DebugFlight").GetComponent<LineRenderer>().enabled, Is.True);
        Capture("debug-on");
        Call("SetDebugVisible", false);
        Assert.That(GameObject.Find("DebugFlight").GetComponent<LineRenderer>().enabled, Is.False);
        Assert.That(GameObject.Find("LandingPreview"), Is.Null);
    }
}
