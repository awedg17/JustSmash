using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class CourtSmokeTests : PreferenceFixture
{
    [UnityTest] public IEnumerator BootProvidesPlayableCourtWithoutLandingPreview()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot");
        yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).FirstOrDefault(t => t != null);
        Assert.That(type, Is.Not.Null, "M1 playable court missing");
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
        Assert.That(court, Is.Not.Null, "Boot must start playable court");
        Assert.That(GameObject.Find("Court"), Is.Not.Null);
        Assert.That(GameObject.Find("Player"), Is.Not.Null);
        Assert.That(GameObject.Find("Opponent"), Is.Not.Null);
        Assert.That(GameObject.Find("Shuttle"), Is.Not.Null);
        Assert.That(GameObject.Find("LandingPreview"), Is.Null);
        yield return null;
        var camera = Camera.main;
        var target = new RenderTexture(1280, 720, 24);
        camera.targetTexture = target; camera.Render(); RenderTexture.active = target;
        var image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); image.Apply();
        System.IO.File.WriteAllBytes(EvidenceDirectory + "/court-runtime.png", image.EncodeToPNG());
        camera.targetTexture = null; RenderTexture.active = null;
        UnityEngine.Object.Destroy(target); UnityEngine.Object.Destroy(image);
        yield return null;
        LogAssert.NoUnexpectedReceived();
    }
    [UnityTest] public IEnumerator AssistedAnalogAimsWithoutMovingIdleAthlete()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
        type.GetField("assisted", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(court, true);
        var before = (Vector3)type.GetProperty("PlayerPosition").GetValue(court);
        type.GetMethod("SetAnalog").Invoke(court, new object[] { Vector2.right });
        yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
        Assert.That((Vector3)type.GetProperty("PlayerPosition").GetValue(court), Is.EqualTo(before), "Assisted analog is aim only, including between feeds");
        Assert.That((Vector2)type.GetProperty("Analog").GetValue(court), Is.EqualTo(Vector2.right));
    }

    [UnityTest] public IEnumerator ManualAndAssistedCoverThreeTrainingApproaches()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        foreach (bool assist in new[] { false, true })
        foreach (string scenario in new[] { "High lob", "Smash approach", "Near net" })
        {
            var player = GameObject.Find("Player").transform;
            player.position = new Vector3(0, 0, -3.5f);
            type.GetField("assisted", flags).SetValue(court, assist);
            type.GetMethod("Feed").Invoke(court, new object[] { scenario == "Near net" ? "Net" : "Lob" });
            var target = new Vector3(.7f, scenario == "Near net" ? 1 : 2.1f, scenario == "Near net" ? -.8f : -2.8f);
            GameObject.Find("Shuttle").transform.position = target;
            type.GetField("velocity", flags).SetValue(court, Vector3.zero);
            type.GetMethod("SetAnalog").Invoke(court, new object[] { Vector2.one.normalized });
            float distance = Vector2.Distance(new Vector2(player.position.x, player.position.z), new Vector2(target.x, target.z));
            yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
            Assert.That(Vector2.Distance(new Vector2(player.position.x, player.position.z), new Vector2(target.x, target.z)), Is.LessThan(distance), scenario + " assisted=" + assist);
            type.GetMethod("SetAnalog").Invoke(court, new object[] { Vector2.zero });
            GameObject.Find("Shuttle").transform.position = player.position + new Vector3(.3f, scenario == "Near net" ? 1 : 2.1f, .35f);
            string shot = scenario == "Near net" ? "Net" : scenario == "Smash approach" ? "Smash" : "Clear";
            type.GetMethod("BeginShot").Invoke(court, new object[] { shot });
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\] Excellent.*"));
            var root = player.position;
            Assert.That(type.GetMethod("ReleaseShot").Invoke(court, null), Is.EqualTo(true), scenario);
            Assert.That(player.position, Is.EqualTo(root), "Contact must not teleport root");
        }
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest] public IEnumerator PointerSupportsMoveChargeAndJumpSwipeTogether()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        var down = type.GetMethod("PointerDown", flags); var up = type.GetMethod("PointerUp", flags);
        var stick = (Rect)type.GetProperty("StickRect", flags).GetValue(court);
        var shot = (Rect)type.GetMethod("ShotRect", flags).Invoke(court, new object[] { 0 });
        down.Invoke(court, new object[] { 10, stick.center + Vector2.right * 60 });
        down.Invoke(court, new object[] { 11, shot.center });
        Assert.That((bool)type.GetField("charging", flags).GetValue(court), Is.False, "D-035 tap strikes immediately");
        Assert.That(((Vector2)type.GetProperty("Analog").GetValue(court)).x, Is.GreaterThan(.5f));
        up.Invoke(court, new object[] { 10 });
        up.Invoke(court, new object[] { 11 });
        var jump = (Rect)type.GetProperty("JumpRect", flags).GetValue(court);
        down.Invoke(court, new object[] { 12, jump.center });
        Assert.That((bool)type.GetField("jumpHeld", flags).GetValue(court), Is.True, "Jump hold drives visible guide");
        type.GetMethod("PointerMove", flags).Invoke(court, new object[] { 12, shot.center });
        Assert.That((string)type.GetField("stroke", flags).GetValue(court), Is.EqualTo("Smash"));
        Assert.That((bool)type.GetField("charging", flags).GetValue(court), Is.True);
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\] Poor \| miss: Kok tidak aktif.*"));
        up.Invoke(court, new object[] { 12 });
        Assert.That((bool)type.GetField("jumpHeld", flags).GetValue(court), Is.False);
        yield return new WaitForSeconds(.1f);
        Assert.That(((Vector3)type.GetProperty("PlayerPosition").GetValue(court)).y, Is.GreaterThan(0));
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest] public IEnumerator NetCrossingCannotTunnelAtHighSpeed()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
        type.GetMethod("Feed").Invoke(court, new object[] { "Net" });
        var shuttle = GameObject.Find("Shuttle").transform;
        shuttle.position = new Vector3(0, 1, -.2f);
        type.GetField("velocity", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(court, new Vector3(0, 0, 80));
        yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
        Assert.That(((Vector3)type.GetProperty("ShuttleVelocity").GetValue(court)).z, Is.LessThan(0), "Net must reverse/damp shuttle");
        Assert.That(shuttle.position.z, Is.LessThan(.1f));
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest] public IEnumerator FeedMovesUnderGravityAndRejectsUnreachableHit()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot");
        yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
        type.GetMethod("Feed").Invoke(court, new object[] { "Lob" });
        var shuttle = GameObject.Find("Shuttle").transform;
        float before = shuttle.position.z;
        yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
        Assert.That(shuttle.position.z, Is.LessThan(before));
        type.GetMethod("BeginShot").Invoke(court, new object[] { "Smash" });
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex("\\[hit\\] Poor \\| miss: Kok terlalu tinggi untuk smash.*"));
        Assert.That(type.GetMethod("ReleaseShot").Invoke(court, null), Is.EqualTo(false));
        var start = (Vector3)type.GetProperty("PlayerPosition").GetValue(court);
        type.GetMethod("SetAnalog").Invoke(court, new object[] { Vector2.right });
        yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
        Assert.That(((Vector3)type.GetProperty("PlayerPosition").GetValue(court)).x, Is.GreaterThan(start.x));
        // Required serve entrypoint: must launch from athlete side, not feeder's far court.
        Assert.That(type.GetMethod("Serve"), Is.Not.Null, "M1 short/flick serve missing");
        shuttle.position = ((Vector3)type.GetProperty("PlayerPosition").GetValue(court)) + new Vector3(.3f, 2.1f, .35f);
        type.GetMethod("BeginShot").Invoke(court, new object[] { "Clear" });
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex("\\[hit\\] Excellent.*"));
        Assert.That(type.GetMethod("ReleaseShot").Invoke(court, null), Is.EqualTo(true));
        Assert.That(((Vector3)type.GetProperty("ShuttleVelocity").GetValue(court)).z, Is.GreaterThan(0));
        type.GetMethod("Jump").Invoke(court, null);
        yield return new WaitForSeconds(.1f);
        Assert.That(((Vector3)type.GetProperty("PlayerPosition").GetValue(court)).y, Is.GreaterThan(0));
        type.GetMethod("SetAnalog").Invoke(court, new object[] { Vector2.zero });
        yield return new WaitForSeconds(4);
        Assert.That(type.GetProperty("Flying").GetValue(court), Is.EqualTo(false));
        type.GetMethod("Serve").Invoke(court, new object[] { false });
        Assert.That(type.GetProperty("Flying").GetValue(court), Is.EqualTo(true));
        Assert.That(shuttle.position.z, Is.LessThan(0));
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex("\\[collision\\] ground.*"));
        LogAssert.NoUnexpectedReceived();
    }
}
