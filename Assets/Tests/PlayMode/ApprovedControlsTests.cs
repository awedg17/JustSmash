using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class ApprovedControlsTests : PreferenceFixture
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
    [UnityTest] public IEnumerator LowEnergySlowsMovementWithoutDrainingWalkingOrForcingMiss()
    {
        yield return Boot(); type.GetField("assisted", Hidden).SetValue(court, false);
        var energy = type.GetProperty("Energy");
        var player = GameObject.Find("Player").transform;
        Vector3 origin = player.position; float fresh = 0;
        foreach (float amount in new[]{100f,30f})
        {
            type.GetMethod("StartMatch").Invoke(court, null); type.GetMethod("EnterTraining").Invoke(court, null);
            energy.SetValue(court,amount); player.position = origin;
            type.GetMethod("SetAnalog").Invoke(court,new object[]{Vector2.right});
            type.GetMethod("FixedUpdate",Hidden).Invoke(court,null);
            float moved = Vector3.Distance(origin,player.position);
            Assert.That((float)energy.GetValue(court), Is.EqualTo(amount), "Walking must not drain energy");
            if (amount == 100) fresh = moved;
            else { Assert.That(moved,Is.LessThan(fresh*.95f)); Assert.That(moved,Is.GreaterThan(fresh*.6f)); }
        }
        type.GetMethod("SetAnalog").Invoke(court,new object[]{Vector2.zero});
        type.GetMethod("Feed").Invoke(court,new object[]{"Lob"});
        GameObject.Find("Shuttle").transform.position = player.position + new Vector3(.3f,2.1f,.35f);
        type.GetMethod("BeginShot").Invoke(court,new object[]{"Clear"});
        Assert.That(type.GetMethod("ReleaseShot").Invoke(court,null),Is.EqualTo(true));
    }
    [UnityTest] public IEnumerator GradeMeasuresIncomingTimingAndContactWithoutChangingFlight()
    {
        yield return Boot();
        var timing = type.GetProperty("LastTiming"); var contact = type.GetProperty("LastContact");
        var score = type.GetProperty("LastGradeScore");
        Assert.That(timing, Is.Not.Null, "Incoming-flight timing grade missing");
        Assert.That(contact, Is.Not.Null); Assert.That(score, Is.Not.Null);
        var player = GameObject.Find("Player").transform;
        var shuttle = GameObject.Find("Shuttle").transform;
        float first = 0; Vector3 launched = Vector3.zero;
        foreach (bool early in new[]{false,true})
        {
            type.GetMethod("Feed").Invoke(court,new object[]{"Lob"});
            shuttle.position = player.position + new Vector3(.35f,2.1f,.45f);
            type.GetField("velocity",Hidden).SetValue(court,early ? new Vector3(0,0,-5) : new Vector3(5,0,0));
            type.GetMethod("TapShot").Invoke(court,new object[]{"Clear"});
            float t = (float)timing.GetValue(court);
            Assert.That((float)contact.GetValue(court), Is.InRange(0f,1f));
            Assert.That((float)score.GetValue(court), Is.InRange(0f,100f));
            if (!early) { first = t; launched = (Vector3)type.GetProperty("ShuttleVelocity").GetValue(court); }
            else
            {
                Assert.That(t, Is.LessThan(first), "Same height/reach must distinguish incoming contact timing");
                Assert.That((Vector3)type.GetProperty("ShuttleVelocity").GetValue(court), Is.EqualTo(launched), "Grade must not rescue or penalize outgoing physics");
            }
        }
    }
    [UnityTest] public IEnumerator LoadingJoystickIsReadOnlyEvenWhenBackupIsOpen()
    {
        string prefs = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, "..", ".hermes", "local-game-data", "prefs"));
        string path = System.IO.Path.Combine(prefs, "joystickMode.txt");
        foreach (string saved in new[] { "Fixed", "Floating", "invalid", null })
        {
            if (saved == null) System.IO.File.Delete(path); else System.IO.File.WriteAllText(path, saved);
            System.IO.File.WriteAllText(path + ".bak", "fixture backup");
            var before = System.IO.Directory.GetFiles(prefs).ToDictionary(p => p, System.IO.File.ReadAllBytes);
            using (var held = new System.IO.FileStream(path + ".bak", System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
            {
                yield return Boot();
                Assert.That(type.GetProperty("JoystickMode").GetValue(court), Is.EqualTo(saved == "Floating" ? "Floating" : "Fixed"));
                Assert.That(type.GetProperty("Analog").GetValue(court), Is.EqualTo(Vector2.zero));
                Assert.That(type.GetField("moveFinger", Hidden).GetValue(court), Is.EqualTo(-1));
                Assert.That(type.GetField("stickCenter", Hidden).GetValue(court), Is.EqualTo(type.GetProperty("DefaultStickCenter", Hidden).GetValue(court)));
                LogAssert.NoUnexpectedReceived();
            }
            Assert.That(System.IO.Directory.GetFiles(prefs), Is.EquivalentTo(before.Keys), "Boot must not create/remove preference files");
            foreach (var pair in before) Assert.That(System.IO.File.ReadAllBytes(pair.Key), Is.EqualTo(pair.Value), pair.Key);
        }
    }
    [UnityTest] public IEnumerator FixedAndFloatingJoystickUseDistinctOriginsAndPersistChoice()
    {
        yield return Boot();
        var configure = type.GetMethod("ConfigureJoystick");
        Assert.That(configure, Is.Not.Null, "Joystick Settings toggle missing");
        string previous = (string)AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.LocalGameData")).First(t => t != null).GetMethod("GetString").Invoke(null, new object[] { "joystickMode", "Fixed" });
        try
        {
            foreach (string mode in new[] { "Fixed", "Floating" })
            {
                configure.Invoke(court, new object[] { mode });
                Assert.That((string)AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.LocalGameData")).First(t => t != null).GetMethod("GetString").Invoke(null, new object[] { "joystickMode", "Fixed" }), Is.EqualTo(mode));
                Vector2 point = new Vector2(300, 570);
                type.GetMethod("PointerDown", Hidden).Invoke(court, new object[] { 7, point });
                Vector2 center = (Vector2)type.GetField("stickCenter", Hidden).GetValue(court);
                Assert.That(center, Is.EqualTo(mode == "Fixed" ? (Vector2)type.GetProperty("DefaultStickCenter", Hidden).GetValue(court) : point));
                type.GetMethod("PointerUp", Hidden).Invoke(court, new object[] { 7 });
                Assert.That((Vector2)type.GetProperty("Analog").GetValue(court), Is.EqualTo(Vector2.zero));
            }
            yield return Boot();
            Assert.That(type.GetProperty("JoystickMode").GetValue(court), Is.EqualTo("Floating"));
            Assert.Throws<TargetInvocationException>(() => configure.Invoke(court, new object[] { "Bad" }));
        }
        finally { type.GetMethod("ConfigureJoystick")?.Invoke(court, new object[] { previous }); }
    }
    [UnityTest] public IEnumerator ServeFeedbackUsesSameOneSecondPowerAsReleaseAndCancelDoesNotServe()
    {
        yield return Boot();
        var power = type.GetProperty("ServePower");
        Assert.That(power, Is.Not.Null, "Single source of truth for serve feedback missing");
        foreach (float hold in new[] { 0f, .4f, .8f, 1f, 1.5f })
        {
            type.GetMethod("BeginServe").Invoke(court, null);
            type.GetField("serveChargedAt", Hidden).SetValue(court, Time.time - hold);
            Assert.That((float)power.GetValue(court), Is.EqualTo(Mathf.Clamp(hold, .2f, 1f)).Within(.001f));
            type.GetMethod("ReleaseServe").Invoke(court, null);
            var velocity = (Vector3)type.GetProperty("ShuttleVelocity").GetValue(court);
            Assert.That(velocity.sqrMagnitude, Is.GreaterThan(0));
            type.GetField("flying", Hidden).SetValue(court, false);
        }
        type.GetMethod("BeginServe").Invoke(court, null);
        Assert.That(type.GetMethod("CancelServe"), Is.Not.Null);
        type.GetMethod("CancelServe").Invoke(court, null);
        type.GetMethod("ReleaseServe").Invoke(court, null);
        Assert.That((bool)type.GetProperty("Flying").GetValue(court), Is.False);
        Assert.That((float)power.GetValue(court), Is.Zero);
    }
}
