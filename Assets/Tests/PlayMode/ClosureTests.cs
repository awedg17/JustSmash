using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class ClosureTests : PreferenceFixture
{
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    Type type; UnityEngine.Object court;
    object Call(string name, params object[] args) => type.GetMethod(name).Invoke(court, args);
    IEnumerator Boot()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
    }
    [UnityTest] public IEnumerator LowBroadcastCameraAndHoldServeWork()
    {
        yield return Boot(); Call("StartMatch");
        var camera = Camera.main;
        Assert.That(camera.transform.position.y, Is.LessThan(6.5f), "Camera elevation must be low broadcast");
        Assert.That(camera.fieldOfView, Is.LessThanOrEqualTo(50f), "FOV must be tight broadcast");
        foreach (string name in new[] { "Player", "Opponent" })
        {
            var p = camera.WorldToViewportPoint(GameObject.Find(name).transform.position + Vector3.up);
            Assert.That(p.z, Is.GreaterThan(0));
            Assert.That(p.x, Is.InRange(.05f, .95f), name + " X in viewport");
            Assert.That(p.y, Is.InRange(.05f, .95f), name + " Y in viewport");
        }
        var begin = type.GetMethod("BeginServe");
        var release = type.GetMethod("ReleaseServe");
        Assert.That(begin, Is.Not.Null, "BeginServe missing");
        Assert.That(release, Is.Not.Null, "ReleaseServe missing");
        begin.Invoke(court, null);
        Assert.That((bool)type.GetProperty("Flying").GetValue(court), Is.False, "Serving must charge on hold");
        yield return new WaitForSeconds(.4f);
        release.Invoke(court, null);
        Assert.That((bool)type.GetProperty("Flying").GetValue(court), Is.True, "ReleaseServe must launch shuttle");
    }
    [UnityTest] public IEnumerator RacketNetContactFaultsOnlyDuringLiveRally()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", true);
        GameObject.Find("Shuttle").transform.position = new Vector3(2, 3, 2);
        type.GetField("velocity", Hidden).SetValue(court, Vector3.up);
        var player = GameObject.Find("Player").transform;
        player.position = new Vector3(0, 0, -.25f);
        var racket = (Transform)type.GetField("racket", Hidden).GetValue(court);
        racket.position = new Vector3(0, 1.3f, 0);
        LogAssert.Expect(LogType.Log, "[scoring] fault net touch player=0");
        type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        Assert.That(type.GetProperty("MatchState").GetValue(court), Is.EqualTo("Point"));
        Assert.That(Call("ChallengeCall"), Is.EqualTo(false));
        type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        var match = type.GetProperty("Match").GetValue(court);
        Assert.That(((int[])match.GetType().GetField("Points").GetValue(match))[1], Is.EqualTo(1), "No duplicate fault outside rally");
    }
    [UnityTest] public IEnumerator ShuttleBodyContactAwardsOpponentAndCannotBeChallenged()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", true);
        var root = GameObject.Find("Player").transform;
        GameObject.Find("Shuttle").transform.position = root.position + new Vector3(0, 1, -.6f);
        type.GetField("velocity", Hidden).SetValue(court, new Vector3(0, 0, 40));
        type.GetField("lastHitter", Hidden).SetValue(court, 1);
        LogAssert.Expect(LogType.Log, "[scoring] fault body touch player=0");
        yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
        Assert.That(type.GetProperty("MatchState").GetValue(court), Is.EqualTo("Point"));
        var match = type.GetProperty("Match").GetValue(court);
        Assert.That(((int[])match.GetType().GetField("Points").GetValue(match))[1], Is.EqualTo(1));
        Assert.That(Call("ChallengeCall"), Is.EqualTo(false));
        Assert.That((bool)type.GetProperty("Flying").GetValue(court), Is.False);
    }
    [UnityTest] public IEnumerator StaminaStatScalesCapacityRecoveryAndSurvivesRestore()
    {
        yield return Boot();
        var configure = type.GetMethod("SetStaminaStat");
        Assert.That(configure, Is.Not.Null, "D-035 stamina stat not wired");
        configure.Invoke(court, new object[] { 80 }); Call("StartMatch");
        Assert.That((float)type.GetProperty("Energy").GetValue(court), Is.EqualTo(110).Within(.001));
        type.GetField("<Energy>k__BackingField", Hidden).SetValue(court, 80f);
        float spent = (float)type.GetProperty("Energy").GetValue(court); Assert.That(spent, Is.EqualTo(80));
        var match = type.GetProperty("Match").GetValue(court);
        for (int i = 0; i < 15; i++) match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
        Call("NextPoint");
        Assert.That((float)type.GetProperty("Energy").GetValue(court), Is.EqualTo(Mathf.Min(110, spent + 8.8f)).Within(.001));
        string saved = (string)Call("CaptureMatch");
        for (int i = 0; i < 15; i++) match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
        configure.Invoke(court, new object[] { 70 }); Call("StartMatch"); Call("RestoreMatch", saved);
        Assert.That((float)type.GetProperty("EnergyCapacity").GetValue(court), Is.EqualTo(110).Within(.001));
        Assert.That((float)type.GetProperty("Energy").GetValue(court), Is.EqualTo(88.8f).Within(.001));
        Call("NextPoint"); Assert.That((float)type.GetProperty("Energy").GetValue(court), Is.EqualTo(88.8f).Within(.001));
        Assert.Throws<TargetInvocationException>(() => configure.Invoke(court, new object[] { 101 }));
    }
    [UnityTest] public IEnumerator TechniqueTapStrikesOnDownWithoutSecondHitOnUp()
    {
        yield return Boot(); Call("Feed", "Net");
        GameObject.Find("Shuttle").transform.position = GameObject.Find("Player").transform.position + new Vector3(.4f, 2.1f, .3f);
        type.GetField("velocity", Hidden).SetValue(court, Vector3.zero);
        var rect = (Rect)type.GetMethod("ShotRect", Hidden).Invoke(court, new object[] { 1 });
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\].*"));
        type.GetMethod("PointerDown", Hidden).Invoke(court, new object[] { 91, rect.center });
        Assert.That((bool)type.GetField("charging", Hidden).GetValue(court), Is.False, "Tap must strike on down, not wait for release");
        Assert.That(((Vector3)type.GetField("velocity", Hidden).GetValue(court)).sqrMagnitude, Is.GreaterThan(1));
        float energy = (float)type.GetProperty("Energy").GetValue(court);
        type.GetMethod("PointerUp", Hidden).Invoke(court, new object[] { 91 });
        Assert.That((float)type.GetProperty("Energy").GetValue(court), Is.EqualTo(energy));
    }
    [UnityTest] public IEnumerator ChangeEndsImmediatelyFramesBothAthletes()
    {
        yield return Boot(); Call("StartMatch");
        var match = type.GetProperty("Match").GetValue(court);
        for (int i=0;i<15;i++) match.GetType().GetMethod("AwardPoint").Invoke(match,new object[]{0});
        Call("NextPoint");
        yield return new WaitForSeconds(.12f);
        foreach (string name in new[]{"Player","Opponent"})
        {
            var point=Camera.main.WorldToViewportPoint(GameObject.Find(name).transform.position+Vector3.up);
            Assert.That(point.z,Is.GreaterThan(0)); Assert.That(point.y,Is.InRange(.05f,.95f),name);
        }
    }
    [UnityTest] public IEnumerator ActualRenderSchedulesKeepTrainingFlightStable()
    {
        yield return Boot();
        int prior = Application.targetFrameRate;
        try
        {
            Vector3 expected = Vector3.zero;
            foreach (int fps in new[] { 30, 60, 120 })
            {
                Application.targetFrameRate = fps;
                Call("Feed", "Lob");
                for (int tick = 0; tick < 20; tick++) yield return new WaitForFixedUpdate();
                var position = GameObject.Find("Shuttle").transform.position;
                if (fps == 30) expected = position;
                Assert.That(Vector3.Distance(position, expected), Is.LessThan(.35f), "Render schedule " + fps);
            }
        }
        finally { Application.targetFrameRate = prior; }
    }
    IEnumerator CaptureUi(string name)
    {
        yield return null; yield return new WaitForEndOfFrame();
        var image = ScreenCapture.CaptureScreenshotAsTexture();
        System.IO.File.WriteAllBytes(EvidenceDirectory + "/closure-" + name + ".png", image.EncodeToPNG());
        UnityEngine.Object.Destroy(image);
    }
    [UnityTest] public IEnumerator CaptureRemainingLocalPanelsAndPoses()
    {
        yield return Boot();
        if (Application.isBatchMode) yield break;
        Call("SetDebugVisible", true); Call("Feed", "Lob");
        yield return new WaitForSeconds(.2f); yield return CaptureUi("debug");
        Call("SetDebugVisible", false);
        type.GetMethod("ToggleTester", Hidden).Invoke(court, null);
        yield return CaptureUi("tester");
        type.GetMethod("ToggleTester", Hidden).Invoke(court, null);
        foreach (string shot in new[] { "Lift", "Clear", "Net", "Smash" })
        {
            Call("Feed", "Net");
            var root = GameObject.Find("Player").transform;
            GameObject.Find("Shuttle").transform.position = root.position + new Vector3(shot == "Clear" ? -.6f : .6f, shot == "Lift" || shot == "Net" ? 1 : 2.1f, .3f);
            type.GetField("velocity", Hidden).SetValue(court, Vector3.zero);
            Call("BeginShot", shot); LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\].*")); Call("ReleaseShot");
            yield return new WaitForSeconds(.08f); yield return CaptureUi("pose-" + shot);
        }
        Call("StartMatch"); Call("Serve", true);
        yield return new WaitForSeconds(.2f); Call("BeginReplay");
        yield return CaptureUi("replay"); Call("SkipReplay");
        var match = type.GetProperty("Match").GetValue(court);
        for (int i = 0; i < 15; i++) match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
        Call("NextPoint"); yield return CaptureUi("gameend");
        Call("AdvanceFlow"); yield return CaptureUi("interval");
        Call("AdvanceFlow"); yield return CaptureUi("changeends");
        for (int i = 0; i < 15; i++) match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
        Call("NextPoint"); yield return CaptureUi("result");
    }
    [UnityTest] public IEnumerator CorruptSaveFileLeavesLiveStateAndBackupUntouched()
    {
        yield return Boot(); Call("StartMatch");
        string path = System.IO.Path.Combine((string)AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.LocalGameData")).First(t => t != null).GetProperty("Root").GetValue(null), "singles-v1.json");
        byte[] old = System.IO.File.Exists(path) ? System.IO.File.ReadAllBytes(path) : null;
        byte[] bak = System.IO.File.Exists(path + ".bak") ? System.IO.File.ReadAllBytes(path + ".bak") : null;
        try
        {
            Call("SaveMatch"); Call("SaveMatch");
            byte[] backup = System.IO.File.ReadAllBytes(path + ".bak");
            string live = (string)Call("CaptureMatch");
            System.IO.File.WriteAllText(path, "{broken");
            Call("RunMenuAction", "Load");
            Assert.That((string)Call("CaptureMatch"), Is.EqualTo(live));
            CollectionAssert.AreEqual(backup, System.IO.File.ReadAllBytes(path + ".bak"));
        }
        finally
        {
            if (old == null) System.IO.File.Delete(path); else System.IO.File.WriteAllBytes(path, old);
            if (bak == null) System.IO.File.Delete(path + ".bak"); else System.IO.File.WriteAllBytes(path + ".bak", bak);
        }
    }
    [UnityTest] public IEnumerator InputLoggingHonorsCentralOffSwitch()
    {
        yield return Boot();
        var boot = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.BootDiagnostics")).First(t => t != null);
        var config = boot.GetProperty("Current").GetValue(null);
        var enabled = config.GetType().GetField("logging");
        bool prior = (bool)enabled.GetValue(config);
        var input = config.GetType().GetField("inputLogging");
        bool priorInput = (bool)input.GetValue(config);
        input.SetValue(config, true);
        try
        {
            enabled.SetValue(config, true);
            LogAssert.Expect(LogType.Log, "[input] charge Clear");
            Call("BeginShot", "Clear");
            enabled.SetValue(config, false);
            Call("BeginShot", "Lift");
            boot.GetMethod("Log").Invoke(null, new object[] { "save", "suppressed" });
            LogAssert.NoUnexpectedReceived();
        }
        finally { enabled.SetValue(config, prior); input.SetValue(config, priorInput); }
    }
    [UnityTest] public IEnumerator CourtDimensionsReadCentralConfig()
    {
        yield return Boot();
        var boot = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.BootDiagnostics")).First(t => t != null);
        var config = boot.GetProperty("Current").GetValue(null);
        var width = config.GetType().GetField("singlesHalfWidth");
        float prior = (float)width.GetValue(config);
        try
        {
            width.SetValue(config, 2f);
            Assert.That(type.GetMethod("InCourt").Invoke(null, new object[] { new Vector3(2.1f, 0, 1), false }), Is.EqualTo(false));
            var bounds = type.GetProperty("CourtDimensions");
            Assert.That(bounds, Is.Not.Null, "Geometry still duplicates config dimensions");
            Assert.That(((Vector3)bounds.GetValue(null)).x, Is.EqualTo(2f));
        }
        finally { width.SetValue(config, prior); }
    }
    [UnityTest] public IEnumerator ReplayToolbarStaysOutsideCentralCourt()
    {
        yield return Boot();
        var rect = type.GetProperty("ReplayToolbarRect");
        Assert.That(rect, Is.Not.Null);
        Assert.That(((Rect)rect.GetValue(court)).xMax, Is.LessThanOrEqualTo(454));
    }
    [UnityTest] public IEnumerator RuntimeCaptureIncludesGameplayAndUi()
    {
        yield return Boot();
        if (Application.isBatchMode) yield break; // Captures run separately in non-batch Editor.
        Call("BeginExhibition");
        yield return null; yield return new WaitForEndOfFrame();
        var image = ScreenCapture.CaptureScreenshotAsTexture();
        System.IO.File.WriteAllBytes(EvidenceDirectory + "/closure-ui.png", image.EncodeToPNG());
        UnityEngine.Object.Destroy(image);
        Call("AdvanceFlow"); Call("AdvanceFlow"); Call("Serve", true);
        yield return new WaitForSeconds(.3f); yield return new WaitForEndOfFrame();
        image = ScreenCapture.CaptureScreenshotAsTexture();
        System.IO.File.WriteAllBytes(EvidenceDirectory + "/closure-rally-ui.png", image.EncodeToPNG());
        UnityEngine.Object.Destroy(image);
    }
    [UnityTest] public IEnumerator CentralConfigExposesExistingPhysicsStaminaAndDiagnosticsDefaults()
    {
        yield return Boot();
        var configType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.BootDiagnostics+Config")).First(t => t != null);
        var config = JsonUtility.FromJson(Resources.Load<TextAsset>("prototype-config").text, configType);
        foreach (string key in new[] { "drag", "rallyDrain", "moveDrain", "gameRecovery", "smashCost", "shotCost", "jumpCost", "aiSpeed", "singlesHalfWidth", "doublesHalfWidth", "halfLength", "netHeight" })
            Assert.That(configType.GetField(key), Is.Not.Null, key + " central config missing");
        Assert.That(type.GetProperty("ProfileSummary"), Is.Not.Null, "Court profiling HUD missing");
        StringAssert.Contains("ms", (string)type.GetProperty("ProfileSummary").GetValue(court));
    }
    [UnityTest] public IEnumerator SafeAreaCanvasContainsControlsAcrossLandscapeRatios()
    {
        yield return Boot();
        var method = type.GetMethod("CanvasScale");
        Assert.That(method, Is.Not.Null, "Safe-area scaling missing");
        foreach (var size in new[] { new Vector2(960, 720), new Vector2(1280, 720), new Vector2(2400, 1080), new Vector2(600, 360) })
        {
            float scale = (float)method.Invoke(null, new object[] { size });
            Assert.That(1280 * scale, Is.LessThanOrEqualTo(size.x + .01f));
            Assert.That(720 * scale, Is.LessThanOrEqualTo(size.y + .01f));
        }
    }
    [UnityTest] public IEnumerator OpponentHasBoundedVisibleRacketAndLocomotion()
    {
        yield return Boot();
        var root = GameObject.Find("Opponent").transform;
        var oRacket = root.Find("Hips/Spine/Chest/ShoulderRight/ElbowRight/Racket");
        Assert.That(oRacket, Is.Not.Null, "Opponent contact visual missing");
        Call("StartMatch");
        var prior = root.position;
        root.position += Vector3.right * .3f;
        yield return null;
        Assert.That(Quaternion.Angle(root.Find("Hips/LegLeft").localRotation, Quaternion.identity), Is.GreaterThan(1));
        Assert.That(Vector3.Distance(oRacket.position, root.position + Vector3.up * 1.35f), Is.LessThan(1.2f));
        Assert.That(root.position, Is.EqualTo(prior + Vector3.right * .3f), "Visuals must not move root");
    }
    [UnityTest] public IEnumerator ApplicationPauseAutosavesRallyWithoutStuckTouchCharge()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", true); Call("BeginShot", "Clear");
        string path = System.IO.Path.Combine((string)AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.LocalGameData")).First(t => t != null).GetProperty("Root").GetValue(null), "singles-v1.json");
        byte[] prior = System.IO.File.Exists(path) ? System.IO.File.ReadAllBytes(path) : null;
        string backup = path + ".bak";
        byte[] priorBackup = System.IO.File.Exists(backup) ? System.IO.File.ReadAllBytes(backup) : null;
        try
        {
            type.GetField("moveFinger", Hidden).SetValue(court, 7);
            type.GetField("shotFinger", Hidden).SetValue(court, 8);
            var pause = type.GetMethod("OnApplicationPause", Hidden);
            Assert.That(pause, Is.Not.Null, "Lifecycle autosave missing");
            pause.Invoke(court, new object[] { true });
            Assert.That(Time.timeScale, Is.EqualTo(0));
            Assert.That((bool)type.GetField("charging", Hidden).GetValue(court), Is.False);
            Assert.That((int)type.GetField("moveFinger", Hidden).GetValue(court), Is.EqualTo(-1));
            Assert.That(System.IO.File.Exists(path), Is.True);
            string json = System.IO.File.ReadAllText(path);
            StringAssert.Contains("\"state\":\"Rally\"", json);
            Time.timeScale = 1; Call("StartMatch"); Call("RestoreMatch", json);
            Assert.That(type.GetProperty("MatchState").GetValue(court), Is.EqualTo("Rally"));
        }
        finally
        {
            Time.timeScale = 1;
            if (prior != null) System.IO.File.WriteAllBytes(path, prior); else if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            if (priorBackup != null) System.IO.File.WriteAllBytes(backup, priorBackup); else if (System.IO.File.Exists(backup)) System.IO.File.Delete(backup);
        }
    }
}
