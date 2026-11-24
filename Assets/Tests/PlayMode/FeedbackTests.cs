using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
public class FeedbackTests : PreferenceFixture
{
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    Type type; UnityEngine.Object court;
    object Call(string name, params object[] args) => type.GetMethod(name).Invoke(court, args);
    object CallHidden(string name, params object[] args) => type.GetMethod(name, Hidden).Invoke(court, args);
    IEnumerator Boot()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
    }
    [UnityTest] public IEnumerator FootfallCadenceIsBoundedAndMatchesVisibleSolePlant()
    {
        yield return Boot(); Call("ConfigureDiscipline", true); ((Behaviour)court).enabled = false;
        var driverType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.AthleteVisual")).First(t => t != null);
        foreach (string name in new[] { "Player", "Opponent", "Partner", "OpponentPartner" })
        foreach (float speed in new[] { 1.5f, 4.5f })
        {
            var root = GameObject.Find(name).transform; var model = root.Find("AthleteVisual"); var driver = model.GetComponent(driverType);
            var present = driverType.GetMethod("Present"); var take = driverType.GetMethod("TakeFootContacts");
            driverType.GetMethod("SetTravel").Invoke(driver, new object[] { 0f });
            present.Invoke(driver, new object[] { "Ready", 10f, Vector3.zero, Vector3.zero, 0f, 0f }); take.Invoke(driver, null);
            int contacts = 0, lastFrame = -1000, shortestGap = 1000;
            for (int frame = 0; frame < 240; frame++)
            {
                var delta = root.forward * speed / 60; root.position += delta;
                present.Invoke(driver, new object[] { "Ready", 10f, Vector3.zero, delta, 0f, 0f });
                int mask = (int)take.Invoke(driver, null);
                if (mask == 0) continue;
                Assert.That(mask, Is.EqualTo(1).Or.EqualTo(2), "Grounded run must alternate single soles");
                contacts++; shortestGap = Mathf.Min(shortestGap, frame - lastFrame); lastFrame = frame;
                string side = mask == 1 ? "Left" : "Right";
                var foot = model.Find("Root/Hips/" + side + "UpLeg/" + side + "Leg/" + side + "Foot");
                Assert.That(foot.position.y, Is.EqualTo(.13f).Within(.015f), "Audible footfall must be on ground");
            }
            TestContext.WriteLine(name + " speed=" + speed + " contacts/4s=" + contacts + " minGap=" + shortestGap / 60f);
            Assert.That(contacts, Is.InRange(speed < 2 ? 6 : 12, speed < 2 ? 10 : 18), "Gait is chattering, not walking/running: " + name);
            Assert.That(shortestGap / 60f, Is.GreaterThanOrEqualTo(.2f), "Step audio retriggers too fast");
        }
    }
    [UnityTest] public IEnumerator FourAthletesSoundOnlyOnGroundedFootContacts()
    {
        yield return Boot(); Call("ConfigureDiscipline", true); ((Behaviour)court).enabled = false;
        var driverType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.AthleteVisual")).First(t => t != null);
        var contact = driverType.GetProperty("FootContacts");
        Assert.That(contact, Is.Not.Null, "Footsteps still use player-only timer, not foot phase");
        var play = type.GetMethod("PlayFootstep", Hidden); Assert.That(play, Is.Not.Null);
        for (int id = 0; id < 4; id++)
        {
            var root = GameObject.Find(new[] { "Player", "Opponent", "Partner", "OpponentPartner" }[id]).transform;
            var driver = root.Find("AthleteVisual").GetComponent(driverType); var present = driverType.GetMethod("Present");
            void Pose(Vector3 delta, float height) => present.Invoke(driver, new object[] { "Ready", 10f, Vector3.zero, delta, height, 0f });
            Pose(Vector3.zero, 0); Assert.That((int)contact.GetValue(driver), Is.Zero);
            int steps = 0;
            for (int i = 0; i < 60; i++)
            {
                var delta = root.forward * .04f; root.position += delta; Pose(delta,0);
                if ((int)contact.GetValue(driver) == 0) continue;
                steps++; play.Invoke(court, new object[] { id, driver });
                Assert.That(((AudioSource)type.GetField("soundSource",Hidden).GetValue(court)).isPlaying, Is.True, root.name);
                Assert.That((int)contact.GetValue(driver), Is.Zero, "Foot contact must be consumed once, not replayed by next audio call");
                var played = (AudioSource)type.GetField("soundSource",Hidden).GetValue(court); played.Stop();
                play.Invoke(court, new object[] { id, driver }); Assert.That(played.isPlaying, Is.False, "Duplicate footstep");
            }
            Assert.That(steps, Is.InRange(2,3), root.name + " 2.4m travel must produce 2-3 alternating footfalls, not chattering");
            Pose(Vector3.zero,0); Assert.That((int)contact.GetValue(driver), Is.Zero);
            root.position += Vector3.up * .5f; Pose(Vector3.forward * .05f,.5f); Assert.That((int)contact.GetValue(driver), Is.Zero);
            root.position -= Vector3.up * .5f; Pose(Vector3.zero,0); Assert.That((int)contact.GetValue(driver), Is.EqualTo(3), "Landing contacts both feet");
            var source = (AudioSource)type.GetField("soundSource",Hidden).GetValue(court); source.Stop();
            Time.timeScale=0; play.Invoke(court,new object[]{id,driver}); Assert.That(source.isPlaying,Is.False); Time.timeScale=1;
            type.GetField("replaying",Hidden).SetValue(court,true); play.Invoke(court,new object[]{id,driver}); Assert.That(source.isPlaying,Is.False); type.GetField("replaying",Hidden).SetValue(court,false);
            type.GetProperty("Volume").SetValue(court,0f); play.Invoke(court,new object[]{id,driver}); Assert.That(source.isPlaying,Is.False); type.GetProperty("Volume").SetValue(court,.7f);
            Pose(Vector3.zero,0); Assert.That((int)contact.GetValue(driver),Is.Zero,"Landing must not repeat");
        }
    }
    [UnityTest] public IEnumerator FootstepUsesFourRecordedShoeVariantsInsteadOfNoiseTone()
    {
        yield return Boot();
        var clip = (AudioClip)Call("CreateSound", "step");
        Assert.That(clip.name, Does.StartWith("step-"), "Footstep is still synthesized noise + 120Hz");
        UnityEngine.Object.Destroy(clip);
        var fingerprints = new System.Collections.Generic.HashSet<string>();
        for (int i = 0; i < 4; i++)
        {
            var recording = Resources.Load<AudioClip>("Audio/step-" + i);
            Assert.That(recording, Is.Not.Null, "Missing recorded sneaker variant " + i);
            Assert.That(recording.channels, Is.EqualTo(1));
            Assert.That(recording.length, Is.InRange(.15f, .35f));
            var samples = new float[recording.samples]; Assert.That(recording.GetData(samples, 0), Is.True);
            Assert.That(samples.All(float.IsFinite), Is.True);
            Assert.That(samples.Max(Mathf.Abs), Is.InRange(.05f, .4f));
            var bytes = new byte[samples.Length * sizeof(float)]; Buffer.BlockCopy(samples, 0, bytes, 0, bytes.Length);
            using (var hash = System.Security.Cryptography.SHA256.Create()) fingerprints.Add(Convert.ToBase64String(hash.ComputeHash(bytes)));
        }
        Assert.That(fingerprints.Count, Is.EqualTo(4), "Do not repeat one recording as four variants");
        Assert.That(Resources.Load<TextAsset>("Audio/step-license"), Is.Not.Null, "Bundle source and CC0 provenance");
    }
    [UnityTest] public IEnumerator RacketPlaybackRequiresFreshLegalContactAcrossInputs()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        var sourceField = type.GetField("soundSource", Hidden);
        AudioSource Source() => (AudioSource)sourceField.GetValue(court);
        void Set(string name, object value) => type.GetField(name, Hidden).SetValue(court, value);
        var rows = new System.Collections.Generic.List<string> { "doubles,swapped,stroke,input,phase,expectedPlaying,actualPlaying" };
        int cases = 0;
        try
        {
            foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
            {
                Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("StartMatch");
                var match = type.GetProperty("Match").GetValue(court); match.GetType().GetField("EndsSwapped").SetValue(match, swapped);
                foreach (string stroke in new[] { "Smash", "Clear", "Drop", "Net", "Lift" }) foreach (bool tap in new[] { false, true })
                {
                    Call("NextPoint");
                    for (int i = 0; i < 3 && (string)type.GetProperty("FlowStage").GetValue(court) != "Serving"; i++) Call("AdvanceFlow");
                    Call("Serve", true); Assert.That(Source().isPlaying, Is.True, "Serve must start playback"); Source().Stop();
                    Set("serviceFlight", false); Set("lastHitter", 1); Set("lastPlayer", 1); Set("velocity", Vector3.zero);
                    var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
                    Vector3 root = player.position, legal = player.TransformPoint(new Vector3(.3f, stroke == "Net" || stroke == "Lift" ? 1 : 2.1f, .35f));
                    float energy = (float)type.GetProperty("Energy").GetValue(court);
                    void Check(string phase, bool expected)
                    {
                        bool playing = Source().isPlaying;
                        rows.Add(doubles + "," + swapped + "," + stroke + "," + (tap ? "TapShot" : "ReleaseShot") + "," + phase + "," + expected + "," + playing);
                        Assert.That(playing, Is.EqualTo(expected), rows[rows.Count - 1]);
                    }
                    void Hit() { if (tap) Call("TapShot", stroke); else { Call("BeginShot", stroke); Call("ReleaseShot"); } }
                    shuttle.position = legal + player.right * 3;
                    if (!tap) LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\] Poor \| miss: .*"));
                    Hit(); Check("outside-reach", false);
                    Assert.That((float)type.GetProperty("Energy").GetValue(court), Is.EqualTo(energy));
                    shuttle.position = legal;
                    LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\] (Excellent|Good|Fair|Poor) \| " + stroke + ".*"));
                    Hit(); Check("legal-contact", true);
                    Assert.That(type.GetField("lastHitter", Hidden).GetValue(court), Is.EqualTo(0));
                    Assert.That((float)type.GetProperty("Energy").GetValue(court), Is.LessThan(energy));
                    Assert.That(player.position, Is.EqualTo(root), "Audio must not require root teleport");
                    Source().Stop(); Hit(); Check("same-side-repeat", false);
                    Call("ReleaseShot"); CallHidden("TryBufferedShot"); Check("stale-release", false);
                    Set("lastHitter", 1); Set("lastPlayer", 1); Set("flying", false);
                    if (!tap) LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\] Poor \| miss: shuttle inactive.*"));
                    Hit(); Check("inactive-shuttle", false); Set("flying", true);
                    Time.timeScale = 0; Hit(); Check("paused-input", false); Time.timeScale = 1;
                    Set("replaying", true); Hit(); Check("replay-input", false); Set("replaying", false);
                    cases++;
                }
            }
            Assert.That(cases, Is.EqualTo(40)); Assert.That(rows.Count - 1, Is.EqualTo(280));
            LogAssert.NoUnexpectedReceived();
        }
        finally
        {
            Time.timeScale = 1; Set("replaying", false); if (Source() != null) Source().Stop();
            System.IO.File.WriteAllLines(EvidenceDirectory + "/racket-contact-playback.csv", rows);
        }
    }
    [UnityTest] public IEnumerator LeavingOrRestartingMatchStopsOldMatchAudio()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        var rows = new System.Collections.Generic.List<string> { "doubles,swapped,action,beforePlaying,afterPlaying,screen,flow" };
        var failures = new System.Collections.Generic.List<string>();
        void Click(string action)
        {
            var rect = (Rect)Call("UiActionRect", action); Assert.That(rect.width, Is.GreaterThan(0), action);
            CallHidden("PointerDown", 94, rect.center); CallHidden("PointerUp", 94);
        }
        try
        {
            foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
            foreach (string action in new[] { "Menu", "Ulangi", "Main lagi" })
            {
                Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("StartMatch");
                var match = type.GetProperty("Match").GetValue(court); var mt = match.GetType();
                mt.GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
                for (int i = 0; i < 3 && (string)type.GetProperty("FlowStage").GetValue(court) != "Serving"; i++) Call("AdvanceFlow");
                Call("Serve", true);
                var source = (AudioSource)type.GetField("soundSource", Hidden).GetValue(court);
                Assert.That(source.isPlaying, Is.True, "Actual serve must start baseline match audio");
                if (action == "Main lagi")
                {
                    for (int i = 0; i < 100 && (int)mt.GetField("Winner").GetValue(match) < 0; i++) mt.GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
                    Assert.That(mt.GetField("Winner").GetValue(match), Is.EqualTo(0)); Call("NextPoint"); Click(action);
                }
                else { Click("Pause"); Click(action); Assert.That(type.GetProperty("ScreenName").GetValue(court), Is.EqualTo("Confirm")); Click("Ya"); }
                bool playing = source.isPlaying;
                rows.Add(doubles + "," + swapped + "," + action + ",True," + playing + "," + type.GetProperty("ScreenName").GetValue(court) + "," + type.GetProperty("FlowStage").GetValue(court));
                if (playing) failures.Add(doubles + "/" + swapped + "/" + action);
                Assert.That(type.GetProperty("ScreenName").GetValue(court), Is.EqualTo(action == "Menu" ? "Home" : "Playing"));
                Assert.That(type.GetProperty("Flying").GetValue(court), Is.EqualTo(false));
                if (action != "Menu") Assert.That(type.GetProperty("FlowStage").GetValue(court), Is.EqualTo("Warmup"));
                source.Stop();
            }
            Assert.That(rows.Count - 1, Is.EqualTo(12));
            Assert.That(failures, Is.Empty, "Old match audio survives boundary: " + string.Join("; ", failures));
        }
        finally { Time.timeScale = 1; System.IO.File.WriteAllLines(EvidenceDirectory + "/match-audio-boundary.csv", rows); }
    }
    [UnityTest] public IEnumerator RacketUsesRecordedImpactInsteadOfSyntheticTone()
    {
        yield return Boot();
        var clip = (AudioClip)Call("CreateSound", "racket");
        StringAssert.StartsWith("racket-", clip.name);
        Assert.That(clip.length, Is.InRange(.1f, .7f));
    }
    [UnityTest] public IEnumerator EmptyArenaNeverPlaysCrowdBedOrApplause()
    {
        yield return Boot();
        void NoCrowd() => Assert.That(((Component)court).GetComponents<AudioSource>().Any(s => s.clip != null &&
            (s.clip.name.StartsWith("crowd-", StringComparison.Ordinal) || s.clip.name == "arena-room")), Is.False, "Crowd must not play in empty venue");
        NoCrowd();
        var error = Assert.Throws<TargetInvocationException>(() => Call("CreateSound", "crowd"));
        Assert.That(error.InnerException, Is.TypeOf<ArgumentException>());
        Call("StartMatch"); Call("Serve", true); yield return null; NoCrowd();
        var player = GameObject.Find("Player").transform;
        GameObject.Find("Shuttle").transform.position = player.position + new Vector3(0, 1, -.6f);
        type.GetField("velocity", Hidden).SetValue(court, new Vector3(0, 0, 40));
        type.GetField("lastHitter", Hidden).SetValue(court, 1);
        LogAssert.Expect(LogType.Log, "[scoring] fault body touch player=0");
        CallHidden("FixedUpdate"); yield return null;
        NoCrowd();
        Time.timeScale = 0; yield return null; NoCrowd();
        Time.timeScale = 1; yield return null; NoCrowd();
        Call("EnterTraining"); yield return null; NoCrowd();
    }
    [UnityTest] public IEnumerator PointContinuesAutomaticallyButPausePreservesWindow()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", true);
        var root = GameObject.Find("Player").transform;
        GameObject.Find("Shuttle").transform.position = root.position + new Vector3(0, 1, -.6f);
        type.GetField("velocity", Hidden).SetValue(court, new Vector3(0, 0, 40));
        type.GetField("lastHitter", Hidden).SetValue(court, 1);
        LogAssert.Expect(LogType.Log, "[scoring] fault body touch player=0");
        yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(.2f);
        Assert.That(type.GetProperty("MatchState").GetValue(court), Is.EqualTo("Point"));
        Time.timeScale = 1;
        yield return new WaitForSeconds(3.3f);
        Assert.That(type.GetProperty("MatchState").GetValue(court), Is.Not.EqualTo("Point"));
    }
    [UnityTest] public IEnumerator PointWindowSurvivesReplayPauseSaveAndAdvancesExactlyOnce()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", true);
        yield return new WaitForSeconds(.1f);
        var player = GameObject.Find("Player").transform;
        GameObject.Find("Shuttle").transform.position = player.position + new Vector3(0, 1, -.6f);
        type.GetField("velocity", Hidden).SetValue(court, new Vector3(0, 0, 40));
        type.GetField("lastHitter", Hidden).SetValue(court, 1);
        LogAssert.Expect(LogType.Log, "[scoring] fault body touch player=0");
        CallHidden("FixedUpdate");
        Assert.That(type.GetProperty("MatchState").GetValue(court), Is.EqualTo("Point"));
        CallHidden("ContinuePoint", 1.25f);
        string saved = (string)Call("CaptureMatch");
        Call("RestoreMatch", saved);
        Assert.That((float)type.GetField("pointWait", Hidden).GetValue(court), Is.EqualTo(1.25f).Within(.001f));
        Assert.That(Call("BeginReplay"), Is.EqualTo(true), "Actual rally must retain replay frames");
        CallHidden("ContinuePoint", 5f);
        Assert.That((float)type.GetField("pointWait", Hidden).GetValue(court), Is.EqualTo(1.25f).Within(.001f));
        Call("SkipReplay");
        try
        {
            Time.timeScale = 0; CallHidden("ContinuePoint", 5f);
            Assert.That(type.GetProperty("MatchState").GetValue(court), Is.EqualTo("Point"));
        }
        finally { Time.timeScale = 1; }
        CallHidden("ContinuePoint", 1.74f);
        Assert.That(type.GetProperty("MatchState").GetValue(court), Is.EqualTo("Point"));
        CallHidden("ContinuePoint", .02f);
        Assert.That(type.GetProperty("MatchState").GetValue(court), Is.EqualTo("Serving"));
        string next = (string)Call("CaptureMatch");
        CallHidden("ContinuePoint", 10f);
        Assert.That(Call("CaptureMatch"), Is.EqualTo(next), "No duplicate transition, score or recovery");
    }
    [UnityTest] public IEnumerator AutomaticPointWindowPreservesGameIntervalAndMatchEnd()
    {
        yield return Boot();
        foreach (bool final in new[] { false, true })
        {
            Call("StartMatch");
            var match = type.GetProperty("Match").GetValue(court);
            var mt = match.GetType();
            if (final)
            {
                mt.GetField("Games").SetValue(match, new[] { 0, 1 });
                mt.GetField("GameNumber").SetValue(match, 2);
                type.GetField("flowGame", Hidden).SetValue(court, 2);
            }
            mt.GetField("Points").SetValue(match, new[] { 0, 14 });
            Call("Serve", true);
            var player = GameObject.Find("Player").transform;
            GameObject.Find("Shuttle").transform.position = player.position + new Vector3(0, 1, -.6f);
            type.GetField("velocity", Hidden).SetValue(court, new Vector3(0, 0, 40));
            type.GetField("lastHitter", Hidden).SetValue(court, 1);
            LogAssert.Expect(LogType.Log, "[scoring] fault body touch player=0");
            CallHidden("FixedUpdate");
            string saved = (string)Call("CaptureMatch");
            Call("RestoreMatch", saved);
            float energy = (float)type.GetProperty("Energy").GetValue(court);
            CallHidden("ContinuePoint", 3f);
            Assert.That(type.GetProperty("FlowStage").GetValue(court), Is.EqualTo(final ? "MatchEnd" : "GameEnd"));
            float recovered = (float)type.GetProperty("Energy").GetValue(court);
            CallHidden("ContinuePoint", 10f);
            Assert.That((float)type.GetProperty("Energy").GetValue(court), Is.EqualTo(recovered), "Recovery must not repeat");
            if (final)
            {
                Assert.That(recovered, Is.EqualTo(energy), "Finished match must not recover for another game");
                Assert.That(type.GetProperty("MatchState").GetValue(court), Is.EqualTo("MatchEnd"));
            }
            else
            {
                foreach (string stage in new[] { "Interval", "ChangeEnds", "Ready" })
                {
                    Call("AdvanceFlow");
                    Assert.That(type.GetProperty("FlowStage").GetValue(court), Is.EqualTo(stage));
                    CallHidden("ContinuePoint", 10f);
                    Assert.That(type.GetProperty("FlowStage").GetValue(court), Is.EqualTo(stage), "Timer must not skip transition gates");
                }
            }
        }
    }
    [UnityTest] public IEnumerator LandingMarkerStaysFixedUntilFlightEnds()
    {
        yield return Boot(); Call("Feed", "Lob");
        var marker = GameObject.Find("ShuttleLandingShadow").transform;
        yield return new WaitForFixedUpdate();
        var expected = marker.position;
        for (int i=0;i<12;i++) yield return new WaitForFixedUpdate();
        Assert.That(Vector3.Distance(marker.position, expected), Is.LessThan(.001f), "Marker must not chase shuttle");
        for (int i=0;i<300 && (bool)type.GetProperty("Flying").GetValue(court);i++) yield return new WaitForFixedUpdate();
        var actual = GameObject.Find("Shuttle").transform.position; actual.y = expected.y;
        Assert.That(Vector3.Distance(actual, expected), Is.LessThan(.06f));
    }
    [UnityTest] public IEnumerator WalkingDoesNotDrainEnergyOnlyJumpAndHitsDo()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", true);
        type.GetField("flying", Hidden).SetValue(court, false);
        var root = GameObject.Find("Player").transform;
        root.position = new Vector3(0, 0, -4);
        Call("SetAnalog", Vector2.left);
        float before = (float)type.GetProperty("Energy").GetValue(court);
        for (int i=0;i<15;i++) type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        Assert.That((float)type.GetProperty("Energy").GetValue(court), Is.EqualTo(before).Within(.001), "Walking must not drain energy");
        Call("Jump");
        Assert.That((float)type.GetProperty("Energy").GetValue(court), Is.LessThan(before), "Jump must drain energy");
    }
    [UnityTest] public IEnumerator FloatingJoystickAdaptsToTouchOrigin()
    {
        yield return Boot();
        string previous = (string)type.GetProperty("JoystickMode").GetValue(court);
        try
        {
            Call("ConfigureJoystick", "Floating");
            CallHidden("PointerDown", 1, new Vector2(100, 450));
            CallHidden("PointerMove", 1, new Vector2(170, 450));
            var analog = (Vector2)type.GetField("analog", Hidden).GetValue(court);
            Assert.That(analog.x, Is.GreaterThan(0.85f), "Analog should point right relative to touch origin");
            Assert.That(Mathf.Abs(analog.y), Is.LessThan(0.15f));
            CallHidden("PointerUp", 1);
            analog = (Vector2)type.GetField("analog", Hidden).GetValue(court);
            Assert.That(analog, Is.EqualTo(Vector2.zero));
        }
        finally { Call("ConfigureJoystick", previous); }
    }
    [UnityTest] public IEnumerator LowDistantShuttleCannotTeleportOrConsumeEnergy()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", true);
        type.GetField("lastHitter", Hidden).SetValue(court, 1);
        var player = GameObject.Find("Player").transform;
        var before = player.position; float energy = (float)type.GetProperty("Energy").GetValue(court);
        GameObject.Find("Shuttle").transform.position = before + new Vector3(.5f, .35f, 1.3f);
        Call("BeginShot", "Lift");
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\] Poor \| miss: .*"));
        Assert.That(Call("ReleaseShot"), Is.EqualTo(false));
        Assert.That(player.position, Is.EqualTo(before));
        Assert.That((float)type.GetProperty("Energy").GetValue(court), Is.EqualTo(energy));
    }
    [UnityTest] public IEnumerator BackhandReachPositionsRacketOnLeftSideOfTorso()
    {
        yield return Boot();
        var player = GameObject.Find("Player").transform;
        player.position = new Vector3(0, 0, -4);
        type.GetField("contact", Hidden).SetValue(court, new Vector3(-0.6f, 1.2f, -4f));
        type.GetField("swingAt", Hidden).SetValue(court, Time.time);
        type.GetField("lastStroke", Hidden).SetValue(court, "Net");
        type.GetMethod("Update", Hidden).Invoke(court, null);
        var racket = GameObject.Find("Player/Hips/Spine/Chest/ShoulderRight/ElbowRight/Racket").transform;
        Assert.That(racket.position.x, Is.LessThan(player.position.x), "Backhand reach must move racket to left side of torso");
    }
    [UnityTest] public IEnumerator RacketMeetsContactWithoutStretchingBonesForBothAthletes()
    {
        yield return Boot();
        foreach (string name in new[] { "Player", "Opponent" })
        foreach (float side in new[] { -.4f, .4f })
        {
            var root = GameObject.Find(name).transform;
            var target = root.TransformPoint(new Vector3(side, 1.4f, .45f));
            type.GetField(name == "Player" ? "contact" : "opponentContact", Hidden).SetValue(court, target);
            type.GetField(name == "Player" ? "swingAt" : "aiContactAt", Hidden).SetValue(court, Time.time);
            CallHidden(name == "Player" ? "AnimateAthlete" : "AnimateOpponent");
            var shoulder = root.Find("Hips/Spine/Chest/ShoulderRight");
            var elbow = shoulder.Find("ElbowRight");
            var racket = elbow.Find("Racket");
            Assert.That(Vector3.Distance(racket.position, target), Is.LessThan(.025f), name + " contact alignment");
            Assert.That(Vector3.Distance(shoulder.position, elbow.position), Is.EqualTo(.32f).Within(.001f));
            Assert.That(elbow.localPosition, Is.EqualTo(new Vector3(0, -.32f, 0)));
        }
    }
    [UnityTest] public IEnumerator OpponentBodyNetTouchAwardsPlayer()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", true);
        var opponent = GameObject.Find("Opponent").transform;
        opponent.position = new Vector3(0, 0, .05f);
        LogAssert.Expect(LogType.Log, "[scoring] fault net touch player=1");
        CallHidden("FixedUpdate");
        Assert.That((string)type.GetProperty("MatchState").GetValue(court), Is.EqualTo("Point"));
    }
    [UnityTest] public IEnumerator LimbMeshLengthMatchesJointSpacing()
    {
        yield return Boot();
        var hip = GameObject.Find("Player").transform.Find("Hips/LegLeft");
        var mesh = hip.Find("LegLeftMesh");
        Assert.That(mesh.localScale.y * 2, Is.EqualTo(.45f).Within(.001f), "Unity capsule height is two units");
    }
    [UnityTest] public IEnumerator TorsoSegmentsBridgeUpwardAndShoesStayAboveFloor()
    {
        yield return Boot();
        var root = GameObject.Find("Player").transform;
        var spine = root.Find("Hips/Spine");
        Assert.That(spine.Find("SpineMesh").localPosition.y, Is.GreaterThan(0), "Torso mesh must reach next joint, not extend down");
        foreach (string stroke in new[] { "Clear", "Net" })
        {
            type.GetField("lastStroke", Hidden).SetValue(court, stroke);
            type.GetField("swingAt", Hidden).SetValue(court, Time.time - (stroke == "Net" ? .1f : 1f));
            CallHidden("AnimateAthlete");
            foreach (string side in new[] { "Left", "Right" })
            {
                var foot = root.Find("Hips/Leg" + side + "/Knee" + side + "/Foot" + side);
                var shoe = foot.GetComponentInChildren<Renderer>();
                Assert.That(shoe.bounds.min.y, Is.InRange(0f, .035f), side + " planted sole");
                Assert.That(Vector3.Dot(foot.up, Vector3.up), Is.GreaterThan(.99f), "Shoe must stay level");
            }
        }
    }
    [UnityTest] public IEnumerator CaptureArticulatedStrokeSequence()
    {
        yield return Boot();
        var root = GameObject.Find("Player").transform;
        var camera = Camera.main;
        camera.transform.position = root.position + new Vector3(3, 2.2f, -3);
        camera.transform.LookAt(root.position + Vector3.up);
        var output = EvidenceDirectory;
        System.IO.Directory.CreateDirectory(output);
        foreach (string stroke in new[] { "Smash", "Net" })
        for (int i = 0; i < 8; i++)
        {
            type.GetField("lastStroke", Hidden).SetValue(court, stroke);
            type.GetField("contact", Hidden).SetValue(court, root.TransformPoint(new Vector3(.4f, stroke == "Smash" ? 2f : 1.2f, .4f)));
            type.GetField("swingAt", Hidden).SetValue(court, Time.time - i * .09f);
            CallHidden("AnimateAthlete");
            var rt = RenderTexture.GetTemporary(640, 640, 24);
            camera.targetTexture = rt; camera.Render(); RenderTexture.active = rt;
            var image = new Texture2D(640, 640, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 640, 640), 0, 0); image.Apply();
            System.IO.File.WriteAllBytes(output + "/" + stroke + "-" + i + ".png", image.EncodeToPNG());
            RenderTexture.active = null; camera.targetTexture = null;
            RenderTexture.ReleaseTemporary(rt); UnityEngine.Object.Destroy(image);
        }
    }
    [UnityTest] public IEnumerator CourtLightingComesFromAboveAndFront()
    {
        yield return Boot();
        var light = GameObject.Find("CourtLight").GetComponent<Light>();
        Assert.That(light.transform.eulerAngles.x, Is.InRange(45f, 80f), "Hall key light comes from above/front, not a horizontal sunset");
    }
    [UnityTest] public IEnumerator ShuttleTrailStaysShortAndReadable()
    {
        yield return Boot();
        var trail = GameObject.Find("Shuttle").GetComponent<TrailRenderer>();
        Assert.That(trail.time, Is.InRange(.08f, .15f), "Short trail must show direction without a long ribbon obscuring contact");
        Assert.That(trail.startWidth, Is.GreaterThanOrEqualTo(0.1f), "Trail width must be at least 0.1m");
        Assert.That(trail.startColor.a, Is.GreaterThanOrEqualTo(0.7f), "Trail start opacity must be at least 0.7");
    }
}
