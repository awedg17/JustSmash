using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MobileInputTests : PreferenceFixture
{
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    Type type; UnityEngine.Object court;
    object Call(string method, params object[] args) => type.GetMethod(method).Invoke(court, args);
    void Set(string field, object value) => type.GetField(field, Hidden).SetValue(court, value);
    T Get<T>(string name) => (T)type.GetProperty(name).GetValue(court);
    IEnumerator Boot()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type);
    }
    [UnityTest] public IEnumerator JumpAndServeKeepFirstPointerUntilReleaseOrCancel()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        var down = type.GetMethod("PointerDown", Hidden); var move = type.GetMethod("PointerMove", Hidden);
        var up = type.GetMethod("PointerUp", Hidden); var cancel = type.GetMethod("PointerCancel", Hidden);
        var rows = new System.Collections.Generic.List<string> { "format,end,mode,control,owner,foreignOwner,powerBefore,powerAfter,energyBefore,energyAfter" };
        var failures = new System.Collections.Generic.List<string>();
        foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
        foreach (string mode in new[] { "Fixed", "Floating" }) foreach (bool jumping in new[] { false, true })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("BeginExhibition");
            var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
            for (int i = 0; i < 3 && Get<string>("FlowStage") != "Serving"; i++) Call("AdvanceFlow");
            Assert.That(Get<string>("FlowStage"), Is.EqualTo("Serving"));
            type.GetProperty("JoystickMode").SetValue(court, mode);
            if (jumping) Call("Serve", true);
            var stick = (Rect)type.GetProperty("StickRect", Hidden).GetValue(court);
            down.Invoke(court, new object[] { 70, stick.center }); move.Invoke(court, new object[] { 70, stick.center + Vector2.right * 50 });
            Vector2 movement = Get<Vector2>("Analog");
            string owner = jumping ? "jumpFinger" : "serveFinger";
            var target = (Rect)type.GetProperty(jumping ? "JumpRect" : "ServeRect", Hidden).GetValue(court);
            down.Invoke(court, new object[] { 81, target.center });
            if (!jumping) Set("serveChargedAt", Time.time - .4f);
            float power = Get<float>("ServePower"), energy = Get<float>("Energy");
            down.Invoke(court, new object[] { 82, target.center });
            int actual = (int)type.GetField(owner, Hidden).GetValue(court);
            string context = doubles + "/" + swapped + "/" + mode + "/" + owner;
            rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", doubles, swapped, mode, owner, 81, actual, power, Get<float>("ServePower"), energy, Get<float>("Energy")));
            if (actual != 81 || Mathf.Abs(Get<float>("ServePower") - power) > .0001f) failures.Add(context + " stolen/reset");
            up.Invoke(court, new object[] { 82 }); cancel.Invoke(court, new object[] { 83 });
            if ((int)type.GetField(owner, Hidden).GetValue(court) != 81) failures.Add(context + " foreign release");
            Assert.That(Get<Vector2>("Analog"), Is.EqualTo(movement));
            if (jumping)
            {
                var shot = (Rect)type.GetMethod("ShotRect", Hidden).Invoke(court, new object[] { 0 });
                move.Invoke(court, new object[] { 81, shot.center });
                if (!(bool)type.GetField("charging", Hidden).GetValue(court)) failures.Add(context + " lost swipe");
            }
            Vector3 velocity = Get<Vector3>("ShuttleVelocity");
            cancel.Invoke(court, new object[] { 81 }); up.Invoke(court, new object[] { 81 });
            Assert.That(type.GetField(owner, Hidden).GetValue(court), Is.EqualTo(-1));
            Assert.That(type.GetField("charging", Hidden).GetValue(court), Is.EqualTo(false));
            Assert.That(type.GetField("jumpHeld", Hidden).GetValue(court), Is.EqualTo(false));
            Assert.That(Get<float>("ServePower"), Is.Zero);
            Assert.That(Get<Vector3>("ShuttleVelocity"), Is.EqualTo(velocity), "Cancel then stale release must not strike");
            Assert.That(Get<float>("Energy"), Is.EqualTo(energy));
            cancel.Invoke(court, new object[] { 70 }); Assert.That(Get<Vector2>("Analog"), Is.EqualTo(Vector2.zero));
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/pointer-ownership.csv", rows);
        Assert.That(rows.Count - 1, Is.EqualTo(16)); Assert.That(failures, Is.Empty, string.Join("; ", failures));
    }
    [UnityTest] public IEnumerator PauseCancelsHeldControlsAndResetsFloatingStick()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        var down = type.GetMethod("PointerDown", Hidden); var move = type.GetMethod("PointerMove", Hidden);
        var up = type.GetMethod("PointerUp", Hidden);
        var rows = new System.Collections.Generic.List<string> { "doubles,end,mode,control,centerError" };
        float maximum = 0;
        foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
        foreach (string mode in new[] { "Fixed", "Floating" }) foreach (bool jumping in new[] { false, true })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("BeginExhibition");
            var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
            for (int i = 0; i < 3 && Get<string>("FlowStage") != "Serving"; i++) Call("AdvanceFlow");
            type.GetProperty("JoystickMode").SetValue(court, mode);
            if (jumping) Call("Serve", true);
            down.Invoke(court, new object[] { 70, new Vector2(300, 570) });
            move.Invoke(court, new object[] { 70, new Vector2(350, 570) });
            var target = (Rect)type.GetProperty(jumping ? "JumpRect" : "ServeRect", Hidden).GetValue(court);
            down.Invoke(court, new object[] { 81, target.center });
            if (jumping) move.Invoke(court, new object[] { 81, ((Rect)type.GetMethod("ShotRect", Hidden).Invoke(court, new object[] { 0 })).center });
            else Set("serveChargedAt", Time.time - .4f);
            float energy = Get<float>("Energy"); Vector3 velocity = Get<Vector3>("ShuttleVelocity");
            var pause = (Rect)Call("UiActionRect", "Pause");
            down.Invoke(court, new object[] { 82, pause.center }); up.Invoke(court, new object[] { 82 });
            Assert.That(Get<string>("ScreenName"), Is.EqualTo("Pause")); Assert.That(Time.timeScale, Is.Zero);
            foreach (string flag in new[] { "charging", "servingCharge", "jumpHeld" }) Assert.That(type.GetField(flag, Hidden).GetValue(court), Is.EqualTo(false));
            foreach (string finger in new[] { "moveFinger", "shotFinger", "jumpFinger", "serveFinger" }) Assert.That(type.GetField(finger, Hidden).GetValue(court), Is.EqualTo(-1));
            Assert.That(Get<Vector2>("Analog"), Is.EqualTo(Vector2.zero));
            float error = Vector2.Distance((Vector2)type.GetField("stickCenter", Hidden).GetValue(court), (Vector2)type.GetProperty("DefaultStickCenter", Hidden).GetValue(court));
            maximum = Mathf.Max(maximum, error); rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4}", doubles, swapped, mode, jumping ? "jump" : "serve", error));
            var resume = (Rect)Call("UiActionRect", "Lanjut");
            down.Invoke(court, new object[] { 82, resume.center }); up.Invoke(court, new object[] { 82 });
            up.Invoke(court, new object[] { 81 }); up.Invoke(court, new object[] { 70 });
            Assert.That(Get<float>("Energy"), Is.EqualTo(energy)); Assert.That(Get<Vector3>("ShuttleVelocity"), Is.EqualTo(velocity));
            Assert.That(Get<string>("ScreenName"), Is.EqualTo("Playing")); Assert.That(Get<Vector2>("Analog"), Is.EqualTo(Vector2.zero));
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/pause-pointers.csv", rows);
        Assert.That(rows.Count - 1, Is.EqualTo(16)); Assert.That(maximum, Is.LessThan(.001f), "Pause leaves Floating joystick at old touch location");
    }
    [UnityTest] public IEnumerator MovementAcceleratesBrakesAndReversesWithoutInputDelay()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        var player = GameObject.Find("Player").transform;
        var tick = type.GetMethod("FixedUpdate", Hidden);
        var rows = new System.Collections.Generic.List<string> { "end,energy,phase,tick,speed,x,z" };
        foreach (bool swapped in new[] { false, true }) foreach (float energy in new[] { 100f, 30f })
        {
            Call("StartMatch"); var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
            float end = swapped ? -1 : 1, cap = 4.5f * Mathf.Lerp(.7f, 1, Mathf.Clamp01(energy / 75));
            player.position = new Vector3(0, 0, -4 * end); type.GetProperty("Energy").SetValue(court, energy);
            float Step(string phase, int index)
            {
                Vector3 before = player.position; tick.Invoke(court, null);
                float speed = (player.position.x - before.x) * end / Time.fixedDeltaTime;
                rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6}", end, energy, phase, index, speed, player.position.x, player.position.z));
                System.IO.File.WriteAllLines(EvidenceDirectory + "/movement.csv", rows); return speed;
            }
            Call("SetAnalog", Vector2.right);
            float first = Step("start", 0);
            Assert.That(first, Is.GreaterThan(0), "Input must move on first fixed tick");
            Assert.That(first, Is.LessThan(cap * .5f), "Root jumps directly to full ground speed");
            float speed = first;
            for (int i = 1; i < 10; i++) { speed = Step("accelerate", i); Assert.That(speed, Is.InRange(0, cap + .0001f)); }
            Assert.That(speed, Is.EqualTo(cap).Within(.0001f));
            Call("SetAnalog", Vector2.zero); Vector3 release = player.position;
            float brake = Step("brake", 0); Assert.That(brake, Is.InRange(.001f, cap - .001f), "Release must brake instead of snap or coast at full speed");
            for (int i = 1; i < 10; i++) speed = Step("brake", i);
            Assert.That(speed, Is.EqualTo(0).Within(.0001f));
            Assert.That(Vector3.Distance(release, player.position), Is.LessThan(.2f), "Braking must not create long skating");
            Call("SetAnalog", Vector2.right); for (int i = 0; i < 10; i++) Step("restart", i);
            Call("SetAnalog", Vector2.left); float reverse = Step("reverse", 0);
            Assert.That(reverse, Is.InRange(.001f, cap - .001f), "Direction reversal must first brake existing momentum");
            Assert.That(Get<float>("Energy"), Is.EqualTo(energy), "Movement cost remains zero under D-050");
        }
    }
    [UnityTest] public IEnumerator AirborneMovementKeepsTakeoffMomentumWithBoundedSteering()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        var player = GameObject.Find("Player").transform; var tick = type.GetMethod("FixedUpdate", Hidden);
        var rows = new System.Collections.Generic.List<string> { "end,energy,tick,speed,height" };
        foreach (bool swapped in new[] { false, true }) foreach (float energy in new[] { 100f, 30f })
        {
            Call("StartMatch"); var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
            Set("flowStage", null); Call("Serve", true); Set("serviceFlight", false);
            float end = swapped ? -1 : 1; player.position = new Vector3(-1 * end, 0, -4 * end);
            type.GetProperty("Energy").SetValue(court, energy); Set("jumpAt", Time.time - 2); Set("jumpPhase", 1f); Set("jumpY", 0f);
            GameObject.Find("Shuttle").transform.position = new Vector3(0, 20, -3 * end); Set("velocity", Vector3.up);
            Call("SetAnalog", Vector2.right); for (int i = 0; i < 6; i++) tick.Invoke(court, null);
            Call("Jump"); Call("SetAnalog", Vector2.left);
            float previous = 4.5f * Mathf.Lerp(.7f, 1, Mathf.Clamp01(Get<float>("Energy") / 75));
            for (int i = 0; i < 10; i++)
            {
                Vector3 before = player.position; tick.Invoke(court, null);
                float speed = (player.position.x - before.x) * end / Time.fixedDeltaTime;
                rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4}", end, energy, i, speed, player.position.y));
                System.IO.File.WriteAllLines(EvidenceDirectory + "/air-steering.csv", rows);
                Assert.That(player.position.y, Is.GreaterThan(0));
                Assert.That(speed, Is.GreaterThan(0), "Opposite airborne input reverses takeoff momentum");
                Assert.That(Mathf.Abs(speed - previous), Is.LessThan(.15f), "Air steering still uses ground acceleration");
                previous = speed;
            }
            float paid = Get<float>("Energy"); Call("Jump"); Assert.That(Get<float>("Energy"), Is.EqualTo(paid), "No second airborne jump charge");
        }
    }
    [UnityTest] public IEnumerator MomentumSnapshotRestoresExactNextTickAcrossFormatsEndsAndJump()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
        var tick = type.GetMethod("FixedUpdate", Hidden);
        var rows = new System.Collections.Generic.List<string> { "doubles,end,airborne,energy,positionError" };
        foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
        foreach (bool airborne in new[] { false, true }) foreach (float energy in new[] { 100f, 30f })
        {
            Set("doublesSelected", doubles); Call("StartMatch"); var match = Get<object>("Match");
            match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint"); Set("flowStage", null); Call("Serve", true);
            Set("serviceFlight", false); Set("assisted", false); Set("jumpPhase", 1f); Set("jumpY", 0f); Set("jumpAt", Time.time - 2);
            float end = swapped ? -1 : 1; player.position = new Vector3(0, 0, -4 * end);
            type.GetProperty("Energy").SetValue(court, energy); shuttle.position = new Vector3(0, 20, -3 * end); Set("velocity", Vector3.up);
            Call("SetAnalog", new Vector2(.6f, .8f)); for (int i = 0; i < 6; i++) tick.Invoke(court, null);
            if (airborne) { Call("Jump"); for (int i = 0; i < 3; i++) tick.Invoke(court, null); }
            string saved = (string)Call("CaptureMatch");
            tick.Invoke(court, null); Vector3 expected = player.position; float expectedEnergy = Get<float>("Energy");
            Call("StartMatch"); Call("RestoreMatch", saved); tick.Invoke(court, null);
            float error = Vector3.Distance(player.position, expected);
            rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4}", doubles, end, airborne, energy, error));
            System.IO.File.WriteAllLines(EvidenceDirectory + "/movement-snapshot.csv", rows);
            Assert.That(error, Is.LessThan(.00001f), "Loading loses horizontal momentum on next real fixed tick");
            Assert.That(Get<float>("Energy"), Is.EqualTo(expectedEnergy).Within(.00001f));
        }
        Assert.That(rows.Count - 1, Is.EqualTo(16));
    }
    [UnityTest] public IEnumerator MomentumSnapshotRejectsInvalidDataAndDefaultsLegacyToRest()
    {
        yield return Boot(); ((Behaviour)court).enabled = false; Call("StartMatch");
        var tick = type.GetMethod("FixedUpdate", Hidden); Call("SetAnalog", Vector2.right); tick.Invoke(court, null);
        string saved = (string)Call("CaptureMatch");
        var schema = type.GetNestedType("Snapshot", BindingFlags.NonPublic); var field = schema.GetField("athleteVelocity");
        Assert.That(field, Is.Not.Null, "Snapshot omits movement momentum");
        var bad = new[] { new Vector3[1], new[] { new Vector3(4.51f, 0, 0), Vector3.zero, Vector3.zero, Vector3.zero },
            new[] { Vector3.zero, new Vector3(0, .01f, 0), Vector3.zero, Vector3.zero },
            new[] { Vector3.zero, Vector3.zero, new Vector3(float.NaN, 0, 0), Vector3.zero },
            new[] { Vector3.zero, Vector3.zero, Vector3.zero, new Vector3(0, 0, float.PositiveInfinity) } };
        foreach (var values in bad)
        {
            var data = JsonUtility.FromJson(saved, schema); field.SetValue(data, values);
            var error = Assert.Throws<TargetInvocationException>(() => Call("RestoreMatch", JsonUtility.ToJson(data)));
            Assert.That(error.InnerException.Message, Is.EqualTo("Invalid athlete velocity"));
            Assert.That(Call("CaptureMatch"), Is.EqualTo(saved), "Invalid momentum mutated live state");
        }
        foreach (int version in new[] { 1, 2, 3 })
        {
            Call("RestoreMatch", saved);
            string legacy = System.Text.RegularExpressions.Regex.Replace(saved, @",?""athleteVelocity"":\[[^\]]*\]", "").Replace("\"version\":3", "\"version\":" + version);
            Assert.That(legacy, Is.Not.EqualTo(saved)); Call("RestoreMatch", legacy);
            var momentum = (Vector3[])type.GetField("athleteVelocity", Hidden).GetValue(court);
            Assert.That(momentum, Is.EqualTo(new Vector3[4]), "Missing legacy momentum must not inherit current match velocity");
        }
    }
    [UnityTest] public IEnumerator MomentumFreezesDuringPauseReplayAndClearsOnAdministrativeReset()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        var tick = type.GetMethod("FixedUpdate", Hidden); var ui = type.GetMethod("ActOnUi", Hidden);
        var player = GameObject.Find("Player").transform;
        var rows = new System.Collections.Generic.List<string> { "doubles,end,action" };
        var errors = new System.Collections.Generic.List<string>();
        foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
        foreach (string action in new[] { "NextPoint", "ChangeEnds", "StartMatch", "Training", "Menu" })
        {
            Set("doublesSelected", doubles); Call("StartMatch"); var match = Get<object>("Match");
            match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint"); Set("flowStage", null); Call("Serve", true);
            Set("serviceFlight", false); Set("assisted", false); Set("jumpPhase", 1f); Set("jumpY", 0f); Set("jumpAt", Time.time - 2);
            float end = swapped ? -1 : 1; player.position = new Vector3(0, 0, -4 * end);
            GameObject.Find("Shuttle").transform.position = new Vector3(0, 20, -3 * end); Set("velocity", Vector3.up);
            Call("SetAnalog", Vector2.right); for (int i = 0; i < 6; i++) tick.Invoke(court, null);
            Call("Jump"); tick.Invoke(court, null);
            var values = (Vector3[])type.GetField("athleteVelocity", Hidden).GetValue(court); var saved = (Vector3[])values.Clone();
            Vector3 position = player.position; float energy = Get<float>("Energy");
            ui.Invoke(court, new object[] { "Pause" }); for (int i = 0; i < 5; i++) tick.Invoke(court, null);
            Assert.That(values, Is.EqualTo(saved)); Assert.That(player.position, Is.EqualTo(position)); Assert.That(Get<float>("Energy"), Is.EqualTo(energy));
            ui.Invoke(court, new object[] { "Lanjut" });
            foreach (int mode in new[] { 0, 1 })
            {
                type.GetProperty("ReplayMode").SetValue(court, mode); Assert.That(Call("BeginReplay"), Is.EqualTo(true));
                type.GetMethod("ReplayTick", Hidden).Invoke(court, null); tick.Invoke(court, null);
                Assert.That(values, Is.EqualTo(saved));
                var schema = type.GetNestedType("Snapshot", BindingFlags.NonPublic);
                var data = JsonUtility.FromJson((string)Call("CaptureMatch"), schema);
                Assert.That((Vector3[])schema.GetField("athleteVelocity").GetValue(data), Is.EqualTo(saved));
                Call("SkipReplay"); Assert.That(player.position, Is.EqualTo(position)); Assert.That(Get<float>("Energy"), Is.EqualTo(energy));
            }
            if (action == "NextPoint") Call("NextPoint");
            else if (action == "ChangeEnds") { match.GetType().GetField("EndsSwapped").SetValue(match, !swapped); Call("NextPoint"); }
            else if (action == "StartMatch") Call("StartMatch");
            else if (action == "Training") Call("EnterTraining");
            else { ui.Invoke(court, new object[] { "Menu" }); ui.Invoke(court, new object[] { "Ya" }); }
            rows.Add(doubles + "," + end + "," + action); System.IO.File.WriteAllLines(EvidenceDirectory + "/movement-resets.csv", rows);
            string label = doubles + "/" + end + "/" + action;
            if (values.Any(v => v != Vector3.zero)) errors.Add(label + " retained stale momentum");
            if ((float)type.GetField("jumpY", Hidden).GetValue(court) != 0 || (float)type.GetField("jumpPhase", Hidden).GetValue(court) < .7f) errors.Add(label + " retained stale jump");
            Call("SetAnalog", Vector2.zero); Vector3 reset = player.position; tick.Invoke(court, null);
            if (player.position != reset) errors.Add(label + " moved after administrative teleport");
        }
        Assert.That(rows.Count - 1, Is.EqualTo(20));
        Assert.That(errors, Is.Empty, string.Join("; ", errors));
    }
    [UnityTest] public IEnumerator MovementCapsDiagonalsClearsBoundaryVelocityAndKeepsManualAuthority()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        var tick = type.GetMethod("FixedUpdate", Hidden); var player = GameObject.Find("Player").transform;
        var rows = new System.Collections.Generic.List<string> { "end,energy,charging,direction,ticks,finalSpeed" };
        foreach (bool swapped in new[] { false, true }) foreach (float energy in new[] { 100f, 30f }) foreach (bool charge in new[] { false, true })
        foreach (Vector2 input in new[] { Vector2.right, Vector2.left, Vector2.up, Vector2.down, new Vector2(1,1), new Vector2(-1,1), new Vector2(1,-1), new Vector2(-1,-1) })
        {
            Call("StartMatch"); var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
            Set("assisted", false); Set("charging", charge); type.GetProperty("Energy").SetValue(court, energy);
            float end = swapped ? -1 : 1, cap = (charge ? 2.8f : 4.5f) * Mathf.Lerp(.7f, 1, Mathf.Clamp01(energy / 75));
            player.position = new Vector3(0, 0, -4 * end); Call("SetAnalog", input); float speed = 0;
            for (int i = 0; i < 10; i++)
            {
                Vector3 before = player.position; tick.Invoke(court, null); Vector3 step = player.position - before;
                speed = step.magnitude / Time.fixedDeltaTime;
                Assert.That(speed, Is.InRange(.001f, cap + .0001f));
                Assert.That(Vector3.Dot(step.normalized, new Vector3(input.x, 0, input.y).normalized * end), Is.GreaterThan(.9999f));
            }
            Assert.That(speed, Is.EqualTo(cap).Within(.0001f));
            rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3}:{4},10,{5}", end,energy,charge,input.x,input.y,speed));
            Call("SetAnalog", Vector2.zero); for (int i = 0; i < 12; i++) tick.Invoke(court, null);
            Vector3 rest = player.position; for (int i = 0; i < 20; i++) tick.Invoke(court, null);
            Assert.That(player.position, Is.EqualTo(rest), "Manual recovery must not pull root toward base");
            Call("NextPoint"); Set("charging", charge); type.GetProperty("Energy").SetValue(court, energy);
            player.position = new Vector3(input.x == 0 ? 0 : Mathf.Sign(input.x) * 3.4f * end, 0, (input.y >= 0 ? -.25f : -7.1f) * end);
            Call("SetAnalog", input); Vector3 boundary = player.position; for (int i = 0; i < 10; i++) tick.Invoke(court, null);
            Assert.That(player.position, Is.EqualTo(boundary));
            Assert.That(((Vector3[])type.GetField("athleteVelocity", Hidden).GetValue(court))[0], Is.EqualTo(Vector3.zero));
            Call("SetAnalog", -input); tick.Invoke(court, null);
            Assert.That(Vector3.Distance(player.position, boundary), Is.GreaterThan(0), "Boundary push must not delay reversal");
            Assert.That(Get<float>("Energy"), Is.EqualTo(energy));
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/movement-directions.csv", rows); Assert.That(rows.Count - 1, Is.EqualTo(64));
    }
    [UnityTest] public IEnumerator InvalidInputAndIllegalJumpLeaveLiveStateUnchanged()
    {
        yield return Boot(); Call("StartMatch");
        string before = (string)Call("CaptureMatch");
        foreach (float power in new[] { float.NaN, float.PositiveInfinity, -.1f, 1.1f })
        {
            var error = Assert.Throws<TargetInvocationException>(() => Call("ServeWithPower", power));
            Assert.That(error.InnerException, Is.TypeOf<ArgumentOutOfRangeException>());
        }
        var bad = Assert.Throws<TargetInvocationException>(() => Call("SetAnalog", new Vector2(float.NaN, 0)));
        Assert.That(bad.InnerException, Is.TypeOf<ArgumentOutOfRangeException>());
        Call("Jump");
        Assert.That(Call("CaptureMatch"), Is.EqualTo(before));
    }
    [UnityTest] public IEnumerator ExpiredShotExplainsMissAndTapServeUsesShortPower()
    {
        yield return Boot(); Call("EnterTraining"); Call("Feed", "Net");
        var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
        shuttle.position = player.position + new Vector3(3, 1, .4f); Set("velocity", Vector3.zero);
        var rect = (Rect)type.GetMethod("ShotRect", Hidden).Invoke(court, new object[] { 4 });
        type.GetMethod("PointerDown", Hidden).Invoke(court, new object[] { 92, rect.center });
        Assert.That(type.GetMethod("ShotPressed"), Is.Not.Null, "Shot tap needs immediate visual feedback");
        Assert.That(Call("ShotPressed", 4), Is.EqualTo(true));
        float energy = Get<float>("Energy");
        yield return new WaitForSeconds(.18f);
        type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        Assert.That((string)type.GetField("feedback", Hidden).GetValue(court), Is.EqualTo("Terlalu jauh"));
        Assert.That(Get<float>("Energy"), Is.EqualTo(energy));
        type.GetMethod("PointerCancel", Hidden).Invoke(court, new object[] { 92 });
        Assert.That(Call("ShotPressed", 4), Is.EqualTo(false));
        Call("StartMatch"); Set("flowStage", null);
        var serve = (Rect)type.GetProperty("ServeRect", Hidden).GetValue(court);
        type.GetMethod("PointerDown", Hidden).Invoke(court, new object[] { 93, serve.center });
        type.GetMethod("PointerUp", Hidden).Invoke(court, new object[] { 93 });
        StringAssert.Contains("Short serve", (string)type.GetField("feedback", Hidden).GetValue(court));
        float targetDepth = GameObject.Find("ShuttleLandingShadow").transform.position.z;
        Assert.That(targetDepth, Is.InRange(2.6f, 3.2f), "Tap should use power .2, not near-zero power");
    }
    [UnityTest] public IEnumerator BufferedPointerHitsOnceAtReachAndCancelNeverFires()
    {
        yield return Boot(); Call("EnterTraining"); Call("Feed", "Net");
        var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
        shuttle.position = player.position + new Vector3(.4f, 1f, 1.25f); Set("velocity", Vector3.zero);
        Call("SetAnalog", Vector2.left); Call("SetAnalog", Vector2.zero);
        var rect = (Rect)type.GetMethod("ShotRect", Hidden).Invoke(court, new object[] { 4 });
        type.GetMethod("PointerDown", Hidden).Invoke(court, new object[] { 71, rect.center });
        float energy = Get<float>("Energy");
        shuttle.position = player.position + new Vector3(.35f, 1f, .4f);
        type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        Assert.That(Get<float>("Energy"), Is.EqualTo(energy - 1).Within(.001f), "Early input was lost instead of buffered");
        Assert.That(Get<Vector2>("Analog"), Is.EqualTo(Vector2.zero), "Shot aim must not modify movement");
        type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        Assert.That(Get<float>("Energy"), Is.EqualTo(energy - 1).Within(.001f));
        Assert.That(Get<Vector3>("ShuttleVelocity").x, Is.LessThan(0), "Released joystick must preserve left aim");
        type.GetMethod("PointerUp", Hidden).Invoke(court, new object[] { 71 });
        Call("Feed", "Net"); shuttle.position = player.position + new Vector3(.4f, 1f, 1.25f); Set("velocity", Vector3.zero);
        type.GetMethod("PointerDown", Hidden).Invoke(court, new object[] { 72, rect.center });
        Assert.That(type.GetMethod("PointerCancel", Hidden), Is.Not.Null);
        type.GetMethod("PointerCancel", Hidden).Invoke(court, new object[] { 72 });
        shuttle.position = player.position + new Vector3(.35f, 1f, .4f);
        type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        Assert.That(Get<float>("Energy"), Is.EqualTo(energy - 1).Within(.001f));
    }
    [UnityTest] public IEnumerator SnapshotRetainsOpponentIntentAndVisibleReplayWithoutMutationOnBadData()
    {
        yield return Boot(); type.GetProperty("ReplayMode").SetValue(court, 1); Call("ConfigureOpponent", "Hard", 71); Call("StartMatch"); Call("Serve", true);
        Set("lastHitter", 1); Set("serviceFlight", false); Call("SetAnalog", Vector2.left); Call("SetAnalog", Vector2.zero);
        var shuttle = GameObject.Find("Shuttle").transform;
        var player = GameObject.Find("Player").transform;
        shuttle.position = player.position + new Vector3(.4f, 1f, 1.25f); Set("velocity", Vector3.zero);
        Call("TapShot", "Lift");
        string snapshot = (string)Call("CaptureMatch");
        StringAssert.Contains("\"version\":3", snapshot);
        StringAssert.Contains("\"difficulty\":\"Hard\"", snapshot);
        StringAssert.Contains("\"intentPending\":true", snapshot);
        Call("StartMatch"); Call("RestoreMatch", snapshot);
        Assert.That(Get<string>("Difficulty"), Is.EqualTo("Hard"));
        Assert.That((string)Call("CaptureMatch"), Is.EqualTo(snapshot));
        string bad = snapshot.Replace("\"difficulty\":\"Hard\"", "\"difficulty\":\"Unknown\"");
        Assert.Throws<TargetInvocationException>(() => Call("RestoreMatch", bad));
        Assert.That((string)Call("CaptureMatch"), Is.EqualTo(snapshot));
        shuttle.position = player.position + new Vector3(.35f, 1f, .4f);
        type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        Assert.That(Get<float>("Energy"), Is.LessThan(100));
        type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        string withPose = (string)Call("CaptureMatch");
        StringAssert.Contains("\"pContact\"", withPose);
        float energy = Get<float>("Energy");
        Assert.That(Call("BeginReplay"), Is.EqualTo(true));
        type.GetMethod("ReplayTick", Hidden).Invoke(court, null);
        Call("SkipReplay");
        Assert.That(Get<float>("Energy"), Is.EqualTo(energy));
    }
    [UnityTest] public IEnumerator FiveShotsKeepDistinctDepthAndFlightAcrossPowerAndEnds()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
        var tick = type.GetMethod("FixedUpdate", Hidden);
        var rows = new System.Collections.Generic.List<string> { "shot,end,power,aim,apex,time,x,z" };
        foreach (float end in new[] { 1f, -1f })
        foreach (float power in new[] { 0f, .5f, 1f })
        foreach (float aim in new[] { -1f, 0f, 1f })
        {
            float netDepth = 0, netApex = 0, clearDepth = 0, clearApex = 0, smashTime = 0, clearTime = 0;
            foreach (string shot in new[] { "Net", "Smash", "Drop", "Clear", "Lift" })
            {
                Call("Feed", "Net");
                player.position = new Vector3(0, 0, (shot == "Net" ? -.9f : -3.5f) * end);
                player.rotation = Quaternion.Euler(0, end > 0 ? 0 : 180, 0);
                // Training has no Match.EndsSwapped; use a real match only as coordinate basis, not as opponent simulation.
                var endField = type.GetField("<Match>k__BackingField", Hidden);
                var matchType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.SinglesMatch")).First(t => t != null);
                var basis = Activator.CreateInstance(matchType); matchType.GetField("EndsSwapped").SetValue(basis, end < 0);
                endField.SetValue(court, basis); type.GetProperty("MatchState").SetValue(court, "Rally"); Set("lastHitter", 1);
                Set("flowStage", "Ready"); // Freeze AI, keep legal manual contact/flight.
                shuttle.position = player.TransformPoint(new Vector3(.3f, shot == "Net" || shot == "Lift" ? 1 : 2.1f, .35f));
                Call("SetAnalog", new Vector2(aim, 0)); Call("BeginShot", shot); Set("chargedAt", Time.time - power * .8f);
                Assert.That(Call("ReleaseShot"), Is.EqualTo(true), shot);
                type.GetProperty("MatchState").SetValue(court, "Training"); // Isolate flight; net/body fault behavior has separate regressions.
                Call("SetAnalog", Vector2.zero);
                float apex = shuttle.position.y; int ticks = 0;
                while (Get<bool>("Flying") && ticks++ < 300) { tick.Invoke(court, null); apex = Mathf.Max(apex, shuttle.position.y); }
                float depth = shuttle.position.z * end;
                Assert.That(Get<bool>("Flying"), Is.False);
                Assert.That(depth, Is.GreaterThan(0), shot + " failed to cross net");
                Assert.That(Mathf.Abs(shuttle.position.x), Is.LessThan(2.59f));
                rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6},{7}", shot, end, power, aim, apex, ticks * .02f, shuttle.position.x, depth));
                if (shot == "Net") { netDepth = depth; netApex = apex; }
                if (shot == "Smash") smashTime = ticks * .02f;
                if (shot == "Clear") { clearDepth = depth; clearApex = apex; clearTime = ticks * .02f; }
                if (shot == "Drop") Assert.That(depth, Is.InRange(1.5f, 3.1f), "Drop must land in front court");
                if (shot == "Lift") Assert.That(depth, Is.GreaterThan(4.8f), "Lift must escape deep");
                endField.SetValue(court, null);
            }
            Assert.That(netDepth, Is.LessThan(2.2f)); Assert.That(clearDepth, Is.GreaterThan(4.8f));
            Assert.That(clearApex, Is.GreaterThan(netApex + .3f)); Assert.That(smashTime, Is.LessThan(clearTime));
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/five-shot-flight.csv", rows);
    }
    [UnityTest] public IEnumerator NetShotStaysLowAcrossPowerAimHeightAndEnds()
    {
        yield return Boot(); ((Behaviour)court).enabled = false; Call("ConfigureOpponent", "Normal", 71); Call("Feed", "Net");
        var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
        var marker = GameObject.Find("ShuttleLandingShadow").transform; var tick = type.GetMethod("FixedUpdate", Hidden);
        var matchType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.SinglesMatch")).First(t => t != null);
        var rows = new System.Collections.Generic.List<string> { "end,height,power,aimX,aimY,apex,clearance,seconds,landingX,landingZ,predictionError" };
        float maxApex = 0, minClearance = float.MaxValue, maxClearance = 0, maxPredictionError = 0; int crossed = 0;
        foreach (float end in new[] { 1f, -1f }) foreach (float height in new[] { 1f, 1.5f })
        foreach (float power in new[] { 0f, .5f, 1f }) foreach (float x in new[] { -1f, 0f, 1f }) foreach (float y in new[] { -1f, 0f, 1f })
        {
            Call("Feed", "Net"); player.position = new Vector3(0, 0, -.9f * end); player.rotation = Quaternion.Euler(0, end > 0 ? 0 : 180, 0);
            var basis = Activator.CreateInstance(matchType); matchType.GetField("EndsSwapped").SetValue(basis, end < 0);
            type.GetField("<Match>k__BackingField", Hidden).SetValue(court, basis); type.GetProperty("MatchState").SetValue(court, "Rally");
            Set("lastHitter", 1); Set("flowStage", "Ready");
            shuttle.position = player.TransformPoint(new Vector3(.3f, height, .35f)); Set("velocity", Vector3.zero);
            var root = player.position; Call("SetAnalog", new Vector2(x, y)); Call("BeginShot", "Net"); Set("chargedAt", Time.time - power * .8f);
            Assert.That(Call("ReleaseShot"), Is.EqualTo(true)); Assert.That(player.position, Is.EqualTo(root));
            var predicted = marker.position; type.GetProperty("MatchState").SetValue(court, "Training"); Call("SetAnalog", Vector2.zero);
            float apex = height, clearance = float.NaN; int ticks = 0;
            while (Get<bool>("Flying") && ticks++ < 300)
            {
                var before = shuttle.position; tick.Invoke(court, null); var after = shuttle.position;
                apex = Mathf.Max(apex, after.y);
                if (before.z * end < 0 && after.z * end >= 0)
                {
                    var crossing = Vector3.Lerp(before, after, -before.z / (after.z - before.z));
                    clearance = crossing.y - (1.524f + .026f * Mathf.Pow(crossing.x / 3.05f, 2));
                }
                var velocity = Get<Vector3>("ShuttleVelocity");
                if (velocity.sqrMagnitude > .01f) Assert.That(Vector3.Dot(-shuttle.up, velocity.normalized), Is.GreaterThan(.999f), "Cork must lead feathers");
                Assert.That(marker.position, Is.EqualTo(predicted), "Prediction must not follow shuttle ground projection");
            }
            Assert.That(Get<bool>("Flying"), Is.False); float depth = shuttle.position.z * end;
            if (depth > 0 && float.IsFinite(clearance)) crossed++;
            float error = Vector2.Distance(new Vector2(predicted.x, predicted.z), new Vector2(shuttle.position.x, shuttle.position.z));
            maxPredictionError = Mathf.Max(maxPredictionError, error); maxApex = Mathf.Max(maxApex, apex);
            minClearance = Mathf.Min(minClearance, clearance); maxClearance = Mathf.Max(maxClearance, clearance);
            rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", end,height,power,x,y,apex,clearance,ticks*.02f,shuttle.position.x,depth,error));
            type.GetField("<Match>k__BackingField", Hidden).SetValue(court, null);
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/net-envelope.csv", rows);
        Assert.That(crossed, Is.EqualTo(108), "Representative reachable net shots must cross");
        Assert.That(maxPredictionError, Is.LessThan(.0001f));
        // D-062 calibration guards, not official badminton dimensions or owner-selected numbers.
        Assert.That(maxApex, Is.LessThanOrEqualTo(2.3f), "Net shot must stay low rather than becoming a lift");
        Assert.That(minClearance, Is.GreaterThan(.02f)); Assert.That(maxClearance, Is.LessThan(.7f));
    }
    [UnityTest] public IEnumerator DistantEmergencyLiftCannotTeleportOrInventDoubleHit()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", true);
        var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
        Set("lastHitter", 1); shuttle.position = player.position + new Vector3(.5f, .35f, 1.3f);
        var before = player.position; Call("TapShot", "Lift");
        Assert.That(player.position, Is.EqualTo(before), "Emergency shot teleported root");
        Set("lastHitter", 0); shuttle.position = player.position + new Vector3(.3f, 2.1f, .35f);
        Call("TapShot", "Clear");
        Assert.That(Get<string>("MatchState"), Is.EqualTo("Rally"), "Extra tap must not fabricate a physical double hit");
    }
}
