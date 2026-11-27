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
    [UnityTest] public IEnumerator FixedJoystickIgnoresEmptyLeftSpaceFollowsDragOutsideAndSupportsMultiTouch()
    {
        yield return Boot();
        type.GetField("assisted", Hidden).SetValue(court, false);
        var down = type.GetMethod("PointerDown", Hidden);
        var move = type.GetMethod("PointerMove", Hidden);
        var up = type.GetMethod("PointerUp", Hidden);
        var stick = (Rect)type.GetProperty("StickRect", Hidden).GetValue(court);
        var player = GameObject.Find("Player").transform;
        Vector3 startPos = player.position;
        Vector2 emptyLeft = stick.center + new Vector2(180f, -60f);
        Assert.That(Vector2.Distance(emptyLeft, stick.center), Is.GreaterThan(95f));
        down.Invoke(court, new object[] { 11, emptyLeft });
        Assert.That(type.GetField("moveFinger", Hidden).GetValue(court), Is.EqualTo(-1), "Fixed joystick must ignore empty left space");
        Assert.That((Vector2)type.GetProperty("Analog").GetValue(court), Is.EqualTo(Vector2.zero));
        type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        Assert.That(player.position, Is.EqualTo(startPos));
        up.Invoke(court, new object[] { 11 });

        // Dead zone suppresses tiny wobble inside circle; curve + sensitivity shape magnitude.
        down.Invoke(court, new object[] { 12, stick.center });
        move.Invoke(court, new object[] { 12, stick.center + Vector2.right * 6f });
        Assert.That((Vector2)type.GetProperty("Analog").GetValue(court), Is.EqualTo(Vector2.zero), "Dead zone must zero micro-drift");
        move.Invoke(court, new object[] { 12, stick.center + Vector2.right * 35f });
        float midDefault = ((Vector2)type.GetProperty("Analog").GetValue(court)).x;
        Assert.That(midDefault, Is.GreaterThan(0f).And.LessThan(.5f));
        type.GetMethod("ConfigureJoystickSensitivity").Invoke(court, new object[] { 1f });
        move.Invoke(court, new object[] { 12, stick.center + Vector2.right * 35f });
        Assert.That(((Vector2)type.GetProperty("Analog").GetValue(court)).x, Is.GreaterThan(midDefault), "Higher sensitivity must increase response");
        type.GetMethod("ConfigureJoystickSensitivity").Invoke(court, new object[] { .5f });

        // Dragging outside circle stays captured; multi-touch shot with right thumb works concurrently.
        move.Invoke(court, new object[] { 12, stick.center + Vector2.right * 140f });
        Assert.That(((Vector2)type.GetProperty("Analog").GetValue(court)).x, Is.EqualTo(1f).Within(.001f));
        for (int i = 0; i < 4; i++) type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        Assert.That(player.position.x, Is.GreaterThan(startPos.x + .05f));

        type.GetMethod("Feed").Invoke(court, new object[] { "Lob" });
        GameObject.Find("Shuttle").transform.position = player.TransformPoint(new Vector3(.3f, 2.1f, .35f));
        type.GetField("velocity", Hidden).SetValue(court, Vector3.zero);
        Rect lobRect = (Rect)type.GetMethod("ShotRect", Hidden).Invoke(court, new object[] { 1 });
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\].*"));
        down.Invoke(court, new object[] { 13, lobRect.center });
        up.Invoke(court, new object[] { 13 });
        Assert.That(type.GetField("lastHitter", Hidden).GetValue(court), Is.EqualTo(0), "Right thumb shot must succeed while left thumb holds joystick");
        Assert.That(type.GetField("moveFinger", Hidden).GetValue(court), Is.EqualTo(12));
        Assert.That(((Vector2)type.GetProperty("Analog").GetValue(court)).x, Is.EqualTo(1f).Within(.001f));
        up.Invoke(court, new object[] { 12 });
        Assert.That((Vector2)type.GetProperty("Analog").GetValue(court), Is.EqualTo(Vector2.zero));
    }
    [UnityTest] public IEnumerator AimUsesActiveInputWithHorizontalNeutralAndPreservesBufferedAimOnBothEnds()
    {
        yield return Boot();
        var down = type.GetMethod("PointerDown", Hidden);
        var move = type.GetMethod("PointerMove", Hidden);
        var up = type.GetMethod("PointerUp", Hidden);
        var player = GameObject.Find("Player").transform;
        var shuttle = GameObject.Find("Shuttle").transform;
        foreach (bool swapped in new[] { false, true })
        {
            float end = swapped ? -1f : 1f;
            type.GetMethod("StartMatch").Invoke(court, null);
            var match = type.GetProperty("Match").GetValue(court);
            match.GetType().GetField("EndsSwapped").SetValue(match, swapped);
            type.GetMethod("NextPoint").Invoke(court, null);
            for (int i = 0; i < 3 && (string)type.GetProperty("FlowStage").GetValue(court) != "Serving"; i++)
                type.GetMethod("AdvanceFlow").Invoke(court, null);
            type.GetMethod("Serve").Invoke(court, new object[] { true });
            type.GetField("serviceFlight", Hidden).SetValue(court, false);
            var stick = (Rect)type.GetProperty("StickRect", Hidden).GetValue(court);
            var lobRect = (Rect)type.GetMethod("ShotRect", Hidden).Invoke(court, new object[] { 1 });

            void PlaceReachable()
            {
                type.GetField("lastHitter", Hidden).SetValue(court, 1);
                type.GetField("lastPlayer", Hidden).SetValue(court, 1);
                type.GetField("flying", Hidden).SetValue(court, true);
                type.GetField("velocity", Hidden).SetValue(court, Vector3.zero);
                shuttle.position = player.TransformPoint(new Vector3(.3f, 2.1f, .35f));
            }

            // 1. Run left -> lift finger -> Lob = center (no carried lastAim).
            down.Invoke(court, new object[] { 20, stick.center });
            move.Invoke(court, new object[] { 20, stick.center + Vector2.left * 70f });
            up.Invoke(court, new object[] { 20 });
            PlaceReachable();
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\].*"));
            down.Invoke(court, new object[] { 21, lobRect.center });
            up.Invoke(court, new object[] { 21 });
            Vector3 landCenter = ((Transform)type.GetField("landingShadow", Hidden).GetValue(court)).position;
            Assert.That(Mathf.Abs(landCenter.x), Is.LessThan(.15f), "Releasing stick before Lob must land center, swapped=" + swapped);

            // 2. Intentional hold left -> Lob = left.
            down.Invoke(court, new object[] { 22, stick.center });
            move.Invoke(court, new object[] { 22, stick.center + Vector2.left * 70f });
            PlaceReachable();
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\].*"));
            down.Invoke(court, new object[] { 23, lobRect.center });
            up.Invoke(court, new object[] { 23 });
            Vector3 landLeft = ((Transform)type.GetField("landingShadow", Hidden).GetValue(court)).position;
            Assert.That(landLeft.x * end, Is.LessThan(-1.5f), "Holding left must land left, swapped=" + swapped);
            up.Invoke(court, new object[] { 22 });

            // 3. Small horizontal drift -> center, independent of sensitivity slider.
            type.GetMethod("ConfigureJoystickSensitivity").Invoke(court, new object[] { 1f });
            type.GetMethod("SetAnalog").Invoke(court, new object[] { new Vector2(.14f, .8f) });
            PlaceReachable();
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\].*"));
            type.GetMethod("TapShot").Invoke(court, new object[] { "Clear" });
            Vector3 landDrift = ((Transform)type.GetField("landingShadow", Hidden).GetValue(court)).position;
            Assert.That(Mathf.Abs(landDrift.x), Is.LessThan(.15f), "Small horizontal drift must land center, swapped=" + swapped);
            type.GetMethod("ConfigureJoystickSensitivity").Invoke(court, new object[] { .5f });
            type.GetMethod("SetAnalog").Invoke(court, new object[] { Vector2.zero });

            // 4. Queue shot aiming right -> move stick left before contact -> buffered shot still aims right.
            type.GetField("lastHitter", Hidden).SetValue(court, 1);
            type.GetField("lastPlayer", Hidden).SetValue(court, 1);
            type.GetField("flying", Hidden).SetValue(court, true);
            type.GetField("velocity", Hidden).SetValue(court, Vector3.zero);
            shuttle.position = player.TransformPoint(new Vector3(3f, 2.1f, .35f));
            type.GetMethod("SetAnalog").Invoke(court, new object[] { Vector2.right });
            type.GetMethod("TapShot").Invoke(court, new object[] { "Clear" });
            type.GetMethod("SetAnalog").Invoke(court, new object[] { Vector2.left });
            PlaceReachable();
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\].*"));
            type.GetMethod("TryBufferedShot", Hidden).Invoke(court, null);
            Vector3 landBuffered = ((Transform)type.GetField("landingShadow", Hidden).GetValue(court)).position;
            Assert.That(landBuffered.x * end, Is.GreaterThan(1.5f), "Buffered shot must land right, swapped=" + swapped);
            type.GetMethod("SetAnalog").Invoke(court, new object[] { Vector2.zero });
        }
    }
}
