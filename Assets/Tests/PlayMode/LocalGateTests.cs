using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class LocalGateTests : PreferenceFixture
{
    Type type; UnityEngine.Object court;
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    object Call(string name, params object[] args) => type.GetMethod(name).Invoke(court, args);
    void Set(string name, object value) => type.GetField(name, Hidden).SetValue(court, value);
    T Get<T>(string name) => (T)type.GetProperty(name).GetValue(court);
    IEnumerator Boot()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("EnterTraining").Invoke(court, null);
    }
    [Serializable] class PoseCamera { public Vector3 position; public Quaternion rotation; public float fov; public bool controlled = true, hud = false; }
    [UnityTest] public IEnumerator RecordedPoseSurvivesUnrelatedFootHistory()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        var errors = new System.Collections.Generic.List<string>();
        var rows = new System.Collections.Generic.List<string> { "doubles,swapped,shot,actor,route,footDeltaMetres,racketDeltaMetres,chestDeltaDegrees" };
        foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
        foreach (string shot in new[] { "Net", "Lift", "Clear" })
        foreach (string movement in new[] { "still", "stride", "air" })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("StartMatch");
            var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
            string[] names = doubles ? new[] { "Player", "Opponent", "Partner", "OpponentPartner" } : new[] { "Player", "Opponent" };
            var roots = names.Select(n => GameObject.Find(n).transform).ToArray();
            var models = roots.Select(t => t.Find("AthleteVisual")).ToArray();
            var drivers = models.Select(t => t.GetComponents<MonoBehaviour>().Single(b => b.GetType().Name == "AthleteVisual")).ToArray();
            var feet = models.Select(t => new[] { t.Find("Root/Hips/LeftUpLeg/LeftLeg/LeftFoot"), t.Find("Root/Hips/RightUpLeg/RightLeg/RightFoot") }).ToArray();
            var chests = models.Select(t => t.Find("Root/Hips/Spine/Spine1")).ToArray();
            var beds = drivers.Select(d => (Transform)d.GetType().GetProperty("StringBed").GetValue(d)).ToArray();
            float elapsed = shot == "Clear" ? 1.45f : .18f;
            for (int id = 0; id < roots.Length; id++)
            {
                var driver = drivers[id]; var present = driver.GetType().GetMethod("Present");
                for (int n = 0; n < 24; n++) present.Invoke(driver, new object[] { "Clear", 1.45f, Vector3.zero, Vector3.zero, 0f, 0f });
                Vector3 contact = roots[id].TransformPoint(new Vector3(.4f, 1, .35f));
                Set(new[] { "lastStroke", "opponentStroke", "partnerStroke", "secondStroke" }[id], shot);
                Set(new[] { "swingAt", "aiContactAt", "partnerAt", "secondAt" }[id], Time.time - elapsed);
                Set(new[] { "contact", "opponentContact", "partnerContact", "secondContact" }[id], contact);
                Set(new[] { "previousPlayer", "previousOpponent", "previousPartner", "previousOpponentPartner" }[id], roots[id].position);
                Vector3 step = movement == "stride" ? roots[id].forward * .06f : Vector3.zero;
                float jump = movement == "air" && id == 0 ? .18f : 0;
                if (movement == "stride")
                    for (int n = 0; n < 7; n++) { roots[id].position += step; present.Invoke(driver, new object[] { "Clear", 1.45f, contact, step, 0f, 0f }); }
                if (jump > 0) { roots[id].position += Vector3.up * jump; Set("jumpY", jump); Set("jumpPhase", .06f); }
                contact = roots[id].TransformPoint(new Vector3(.4f, 1, .35f));
                Set(new[] { "contact", "opponentContact", "partnerContact", "secondContact" }[id], contact);
                present.Invoke(driver, new object[] { shot, elapsed, contact, step, jump, 0f });
                Set(new[] { "previousPlayer", "previousOpponent", "previousPartner", "previousOpponentPartner" }[id], roots[id].position - step);
            }
            var expectedFeet = feet.Select(a => a.Select(t => t.position).ToArray()).ToArray();
            var expectedBeds = beds.Select(t => t.position).ToArray(); var expectedChest = chests.Select(t => t.localRotation).ToArray();
            var expectedRoots = roots.Select(t => t.position).ToArray();
            object frame = type.GetMethod("CaptureFrame", Hidden).Invoke(court, null);
            string frameJson = JsonUtility.ToJson(frame), saved = (string)Call("CaptureMatch");
            int sounds = Get<int>("SoundEventCount"); float energy = Get<float>("Energy");
            bool capture = !Application.isBatchMode && !doubles && !swapped && shot == "Net" && movement == "still";
            IEnumerator CapturePose(string label)
            {
                var camera = Camera.main;
                camera.transform.position = roots[0].position + new Vector3(2.5f, 1.2f, 1.5f); camera.transform.LookAt(roots[0].position + Vector3.up * .9f);
                yield return null; yield return new WaitForEndOfFrame();
                var target = new RenderTexture(1280, 720, 24) { antiAliasing = Mathf.Max(1, QualitySettings.antiAliasing) };
                var oldTarget = camera.targetTexture; var oldActive = RenderTexture.active;
                var image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
                try { camera.targetTexture = target; camera.Render(); RenderTexture.active = target; image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); image.Apply(); System.IO.File.WriteAllBytes(EvidenceDirectory + "/pose-" + label + ".png", image.EncodeToPNG()); }
                finally { camera.targetTexture = oldTarget; RenderTexture.active = oldActive; UnityEngine.Object.Destroy(image); target.Release(); UnityEngine.Object.Destroy(target); }
            }
            if (capture) { yield return CapturePose("expected"); System.IO.File.WriteAllText(EvidenceDirectory + "/camera.json", JsonUtility.ToJson(new PoseCamera { position = Camera.main.transform.position, rotation = Camera.main.transform.rotation, fov = Camera.main.fieldOfView })); }
            foreach (string route in new[] { "ApplyFrame", "RestoreMatch" })
            {
                for (int id = 0; id < roots.Length; id++)
                    for (int n = 0; n < 12; n++) drivers[id].GetType().GetMethod("Present").Invoke(drivers[id], new object[] { "Net", .18f, roots[id].TransformPoint(new Vector3(-.4f, 1, .35f)), Vector3.zero, 0f, 0f });
                if (route == "ApplyFrame") type.GetMethod("ApplyFrame", Hidden).Invoke(court, new[] { JsonUtility.FromJson(frameJson, frame.GetType()) });
                else Call("RestoreMatch", saved);
                if (capture) yield return CapturePose(route);
                for (int id = 0; id < roots.Length; id++)
                {
                    float footDelta = feet[id].Select((t, i) => Vector3.Distance(t.position, expectedFeet[id][i])).Max();
                    float bedDelta = Vector3.Distance(beds[id].position, expectedBeds[id]); float chestDelta = Quaternion.Angle(chests[id].localRotation, expectedChest[id]);
                    rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5:R},{6:R},{7:R}", doubles, swapped, shot + "/" + movement, names[id], route, footDelta, bedDelta, chestDelta));
                    if (footDelta > .001f || bedDelta > .001f || chestDelta > .05f) errors.Add(doubles + "/" + swapped + "/" + shot + "/" + names[id] + "/" + route + " feet=" + footDelta + " bed=" + bedDelta + " chest=" + chestDelta);
                    Assert.That(roots[id].position, Is.EqualTo(expectedRoots[id]));
                }
                Assert.That(Get<int>("SoundEventCount"), Is.EqualTo(sounds)); Assert.That(Get<float>("Energy"), Is.EqualTo(energy));
            }
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/pose-roundtrip.csv", rows);
        Assert.That(rows.Count - 1, Is.EqualTo(216));
        Assert.That(errors, Is.Empty, "Recorded pose depends on unrelated foot history: " + string.Join("; ", errors));
    }
    [UnityTest] public IEnumerator FullReplayBufferSnapshotFitsRestoreBoundary()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        var rows = new System.Collections.Generic.List<string> { "doubles,frames,jsonCharacters" };
        var errors = new System.Collections.Generic.List<string>();
        foreach (bool doubles in new[] { false, true })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("StartMatch");
            var frames = (IList)type.GetField("replay", Hidden).GetValue(court); frames.Clear();
            object frame = type.GetMethod("CaptureFrame", Hidden).Invoke(court, null);
            for (int i = 0; i < 1500; i++) frames.Add(frame);
            string saved = (string)Call("CaptureMatch"); rows.Add(doubles + "," + frames.Count + "," + saved.Length);
            try { Call("RestoreMatch", saved); }
            catch (TargetInvocationException error) { errors.Add(doubles + ":" + saved.Length + ":" + error.InnerException.Message); }
            string unchanged = (string)Call("CaptureMatch");
            var oversize = Assert.Throws<TargetInvocationException>(() => Call("RestoreMatch", new string(' ', 8000001)));
            Assert.That(oversize.InnerException.Message, Is.EqualTo("Invalid snapshot size")); Assert.That(Call("CaptureMatch"), Is.EqualTo(unchanged));
            frames.Add(frame); string tooMany = (string)Call("CaptureMatch"); frames.RemoveAt(frames.Count - 1);
            var excessFrames = Assert.Throws<TargetInvocationException>(() => Call("RestoreMatch", tooMany));
            Assert.That(excessFrames.InnerException.Message, Is.EqualTo("Too many replay frames")); Assert.That(Call("CaptureMatch"), Is.EqualTo(unchanged));
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/full-replay-size.csv", rows);
        Assert.That(errors, Is.Empty, string.Join("; ", errors));
    }
    [UnityTest] public IEnumerator ReplayTicksSkipAndLoadPreserveFeetWithoutFootfall()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        int cases = 0;
        foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("StartMatch");
            Get<object>("Match").GetType().GetField("EndsSwapped").SetValue(Get<object>("Match"), swapped); Call("NextPoint");
            var roots = (doubles ? new[] { "Player", "Opponent", "Partner", "OpponentPartner" } : new[] { "Player", "Opponent" }).Select(n => GameObject.Find(n).transform).ToArray();
            var drivers = roots.Select(t => t.Find("AthleteVisual").GetComponents<MonoBehaviour>().Single(b => b.GetType().Name == "AthleteVisual")).ToArray();
            var feet = roots.SelectMany(t => new[] { t.Find("AthleteVisual/Root/Hips/LeftUpLeg/LeftLeg/LeftFoot"), t.Find("AthleteVisual/Root/Hips/RightUpLeg/RightLeg/RightFoot") }).ToArray();
            object Frame() => type.GetMethod("CaptureFrame", Hidden).Invoke(court, null);
            var frames = (IList)type.GetField("replay", Hidden).GetValue(court); frames.Clear();
            for (int id = 0; id < roots.Length; id++)
            {
                var contact = roots[id].TransformPoint(new Vector3(.4f, 1, .35f));
                Set(new[] { "lastStroke", "opponentStroke", "partnerStroke", "secondStroke" }[id], "Net");
                Set(new[] { "swingAt", "aiContactAt", "partnerAt", "secondAt" }[id], Time.time - .18f);
                Set(new[] { "contact", "opponentContact", "partnerContact", "secondContact" }[id], contact);
                Set(new[] { "previousPlayer", "previousOpponent", "previousPartner", "previousOpponentPartner" }[id], roots[id].position);
                drivers[id].GetType().GetMethod("Present").Invoke(drivers[id], new object[] { "Net", .18f, contact, Vector3.zero, 0f, 0f });
            }
            var expected = feet.Select(t => t.position).ToArray(); var rootPositions = roots.Select(t => t.position).ToArray();
            frames.Add(Frame()); frames.Add(Frame()); int sounds = Get<int>("SoundEventCount"); float energy = Get<float>("Energy");
            Assert.That(Call("BeginReplay"), Is.True); string saved = (string)Call("CaptureMatch");
            void Check()
            {
                for (int i = 0; i < feet.Length; i++) Assert.That(Vector3.Distance(feet[i].position, expected[i]), Is.LessThan(.001f));
                for (int i = 0; i < roots.Length; i++) Assert.That(roots[i].position, Is.EqualTo(rootPositions[i]));
                foreach (var driver in drivers) Assert.That(driver.GetType().GetMethod("TakeFootContacts").Invoke(driver, null), Is.EqualTo(0));
                Assert.That(Get<int>("SoundEventCount"), Is.EqualTo(sounds)); Assert.That(Get<float>("Energy"), Is.EqualTo(energy)); cases++;
            }
            for (int i = 0; i < 12; i++) { type.GetMethod("ReplayTick", Hidden).Invoke(court, null); Check(); }
            Call("SkipReplay"); Check();
            Call("StartMatch"); Call("RestoreMatch", saved); Check();
            Assert.That((bool)type.GetField("replaying", Hidden).GetValue(court), Is.False);
        }
        Assert.That(cases, Is.EqualTo(56));
    }
    [UnityTest] public IEnumerator FootPlantSnapshotRejectsCorruptionBeforeMutationAndLoadsLegacy()
    {
        yield return Boot(); Call("ConfigureDiscipline", true); Call("StartMatch"); ((Behaviour)court).enabled = false;
        string saved = (string)Call("CaptureMatch");
        var schema = type.GetNestedType("Snapshot", BindingFlags.NonPublic);
        string[] actors = { "pFeet", "oFeet", "partner", "second" };
        int rejected = 0;
        foreach (bool buffer in new[] { false, true }) foreach (string actor in actors)
        foreach (string side in new[] { "left", "right" }) foreach (float invalid in new[] { float.NaN, float.PositiveInfinity, 1001f })
        {
            object data = JsonUtility.FromJson(saved, schema), frame = schema.GetField("livePose").GetValue(data);
            object holder = actor == "partner" || actor == "second" ? frame.GetType().GetField(actor).GetValue(frame) : frame;
            var field = holder.GetType().GetField(holder == frame ? actor : "feet"); object foot = field.GetValue(holder);
            foot.GetType().GetField(side).SetValue(foot, new Vector3(invalid, .13f, 0)); field.SetValue(holder, foot);
            if (holder != frame) frame.GetType().GetField(actor).SetValue(frame, holder);
            if (buffer) { var frames = Array.CreateInstance(frame.GetType(), 1); frames.SetValue(frame, 0); schema.GetField("frames").SetValue(data, frames); }
            else schema.GetField("livePose").SetValue(data, frame);
            var error = Assert.Throws<TargetInvocationException>(() => Call("RestoreMatch", JsonUtility.ToJson(data)));
            Assert.That(error.InnerException.Message, Is.EqualTo("Invalid replay foot plant"));
            Assert.That(Call("CaptureMatch"), Is.EqualTo(saved), "Rejected foot pose mutated match"); rejected++;
        }
        Assert.That(rejected, Is.EqualTo(48));
        foreach (int version in new[] { 2, 3 })
        {
            string legacy = Regex.Replace(saved, @",?""(?:pFeet|oFeet|feet)"":\{""recorded"":(?:true|false),""planted"":(?:true|false),""airborne"":(?:true|false),""left"":\{[^}]*\},""right"":\{[^}]*\}\}", "").Replace("\"version\":3", "\"version\":" + version);
            Assert.That(legacy, Does.Not.Contain("\"recorded\"")); Call("RestoreMatch", legacy);
            Assert.That(Get<bool>("DoublesMode"), Is.True);
        }
    }
    [UnityTest] public IEnumerator SpectacleSaveRestoresMarkerAndReplayKeepsActualLanding()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", true);
        yield return new WaitForSeconds(.1f);
        type.GetProperty("SpectacularMoment").SetValue(court, true);
        string saved = (string)Call("CaptureMatch"); Call("StartMatch"); Call("RestoreMatch", saved);
        Assert.That(Get<bool>("SpectacularMoment"), Is.True, "Saved spectacle marker lost");
        Set("serviceFlight", false);
        GameObject.Find("Shuttle").transform.position = new Vector3(0, .045f, 3);
        Set("velocity", Vector3.down);
        LogAssert.Expect(LogType.Log, new Regex(@"\[collision\] ground.*"));
        type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        Assert.That((bool)type.GetField("replaying", Hidden).GetValue(court), Is.True);
        Call("SkipReplay");
        Assert.That(GameObject.Find("Shuttle").transform.position.y, Is.EqualTo(.04f).Within(.0001f));
    }
    [UnityTest] public IEnumerator OpponentServeEmitsRacketSound()
    {
        yield return Boot(); Call("StartMatch");
        var match = Get<object>("Match"); match.GetType().GetField("Server").SetValue(match, 1);
        Call("NextPoint"); Call("AdvanceFlow"); Set("serveReadyAt", Time.time - 1);
        int before = Get<int>("SoundEventCount");
        type.GetMethod("UpdateOpponent", Hidden).Invoke(court, null);
        Assert.That(Get<bool>("Flying"), Is.True);
        Assert.That(Get<int>("SoundEventCount"), Is.EqualTo(before + 1), "Opponent serve must emit exactly one event");
        Assert.That(Get<AudioClip>("LastSoundClip"), Is.SameAs(Resources.Load<AudioClip>("Audio/racket-0")));
        Assert.That(Get<float>("LastSoundGain"), Is.EqualTo(.55f));
    }
    [UnityTest] public IEnumerator PausedReleaseCannotConsumeChargeOrHit()
    {
        yield return Boot(); Call("Feed", "Lob");
        var p = GameObject.Find("Player").transform;
        GameObject.Find("Shuttle").transform.position = p.position + new Vector3(.3f, 2.1f, .35f);
        Call("BeginShot", "Clear"); float energy = Get<float>("Energy");
        Time.timeScale = 0;
        Assert.That(Call("ReleaseShot"), Is.EqualTo(false));
        Assert.That((bool)type.GetField("charging", Hidden).GetValue(court), Is.True);
        Assert.That(Get<float>("Energy"), Is.EqualTo(energy));
        Time.timeScale = 1;
    }
    [UnityTest] public IEnumerator MatchSummaryShowsRuleGameAndWinner()
    {
        yield return Boot(); Call("StartMatch");
        Assert.That(type.GetProperty("MatchSummary"), Is.Not.Null);
        StringAssert.Contains("Official15", Get<string>("MatchSummary"));
        var m = Get<object>("Match");
        for (int i=0;i<30;i++) m.GetType().GetMethod("AwardPoint").Invoke(m,new object[]{0});
        Call("NextPoint");
        StringAssert.Contains("You win",Get<string>("MatchSummary"));
        StringAssert.Contains("2 - 0",Get<string>("MatchSummary"));
    }
    [UnityTest] public IEnumerator ExhibitionWarmupContinuesIntoReady()
    {
        yield return Boot();
        Assert.That(type.GetMethod("BeginExhibition"), Is.Not.Null, "Warmup entrypoint missing");
        Call("BeginExhibition"); Assert.That(Get<string>("FlowStage"), Is.EqualTo("Warmup"));
        Call("Serve", false); Assert.That(Get<bool>("Flying"), Is.False);
        Call("AdvanceFlow"); Assert.That(Get<string>("FlowStage"), Is.EqualTo("Ready"));
        string saved = (string)Call("CaptureMatch"); Call("StartMatch"); Call("RestoreMatch",saved);
        Assert.That(Get<string>("FlowStage"), Is.EqualTo("Ready"));
    }
    [UnityTest] public IEnumerator DecidingEightChangesEndsAndRestoreFacesNet()
    {
        yield return Boot(); Call("StartMatch");
        var m = Get<object>("Match"); var mt = m.GetType();
        for (int side = 0; side < 2; side++) for (int i = 0; i < 15; i++) mt.GetMethod("AwardPoint").Invoke(m, new object[]{side});
        Set("flowGame", 3); Set("flowStage", null);
        mt.GetField("Points").SetValue(m,new[]{7,0}); mt.GetMethod("AwardPoint").Invoke(m,new object[]{0});
        type.GetField("flowEnds", Hidden)?.SetValue(court, false);
        Call("NextPoint");
        Assert.That(Get<string>("FlowStage"), Is.EqualTo("ChangeEnds"));
        string saved = (string)Call("CaptureMatch"); Call("StartMatch"); Call("RestoreMatch",saved);
        Assert.That(GameObject.Find("Player").transform.forward.z, Is.LessThan(0));
        Assert.That(GameObject.Find("Opponent").transform.forward.z, Is.GreaterThan(0));
    }
    [UnityTest] public IEnumerator SwappedEndsPreserveServeMovementAndCameraBehindPlayer()
    {
        yield return Boot(); Call("StartMatch");
        var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, true);
        Call("NextPoint"); Call("AdvanceFlow");
        Assert.That(GameObject.Find("Player").transform.position.z, Is.GreaterThan(0));
        Assert.That(GameObject.Find("Opponent").transform.position.z, Is.LessThan(0));
        var before = Get<Vector3>("PlayerPosition"); Call("SetAnalog", Vector2.up);
        type.GetMethod("FixedUpdate", Hidden).Invoke(court, null); Call("SetAnalog", Vector2.zero);
        Assert.That(Get<Vector3>("PlayerPosition").z, Is.LessThan(before.z), "Forward still means toward net");
        Call("Serve", true); Assert.That(Get<Vector3>("ShuttleVelocity").z, Is.LessThan(0));
        yield return new WaitForSeconds(1);
        Assert.That(Camera.main.transform.position.z, Is.GreaterThan(Get<Vector3>("PlayerPosition").z));
    }
    [UnityTest] public IEnumerator LineJudgeLimitsErrorsToOneNearBoundaryAndPersistsState()
    {
        yield return Boot(); Call("StartMatch");
        var announce = type.GetMethod("AnnounceLanding", Hidden);
        Assert.That(announce, Is.Not.Null, "Line judge missing");
        type.GetProperty("SimulateLineErrors").SetValue(court, true);
        foreach (var p in new[] { new Vector3(0, .04f, 3), new Vector3(3, .04f, 8), new Vector3(2.59f, .04f, 8) })
            for (int i = 0; i < 100; i++) Assert.That((bool)announce.Invoke(court, new object[]{p}), Is.EqualTo(Mathf.Abs(p.x) <= 2.59f && Mathf.Abs(p.z) <= 6.7f));
        int errors = 0;
        for (int i = 0; i < 1000; i++) if (!(bool)announce.Invoke(court, new object[]{new Vector3(2.58f,.04f,3)})) errors++;
        Assert.That(errors, Is.EqualTo(1));
        string saved = (string)Call("CaptureMatch"); Call("StartMatch"); Call("RestoreMatch", saved);
        for (int i = 0; i < 1000; i++) Assert.That((bool)announce.Invoke(court,new object[]{new Vector3(2.58f,.04f,3)}), Is.True);
    }
    [UnityTest] public IEnumerator GameScoreHistoryRejectsCorruptionBeforeMutatingLiveMatch()
    {
        yield return Boot(); ((Behaviour)court).enabled = false; Call("StartMatch");
        var match = Get<object>("Match"); var mt = match.GetType();
        for (int i = 0; i < 7; i++) { mt.GetMethod("AwardPoint").Invoke(match, new object[] { 0 }); mt.GetMethod("AwardPoint").Invoke(match, new object[] { 1 }); }
        for (int i = 7; i < 15; i++) mt.GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
        Call("NextPoint"); string valid = (string)Call("CaptureMatch");
        foreach (string points in new[] { "null", "[]", "[15]", "[15,7,15,0]", "[-1,7]", "[15,14]", "[16,7]", "[22,20]", "[7,15]" })
        {
            string bad = Regex.Replace(valid, "\"GamePoints\":\\[[^\\]]*\\]", "\"GamePoints\":" + points);
            Assert.That(bad, Is.Not.EqualTo(valid));
            var error = Assert.Throws<TargetInvocationException>(() => Call("RestoreMatch", bad), points);
            Assert.That(error.InnerException, Is.TypeOf<ArgumentException>());
            Assert.That(Call("CaptureMatch"), Is.EqualTo(valid), points + " mutated live state");
        }
        string prior = JsonUtility.ToJson(match); Set("beforePoint", prior.Replace("\"GamePoints\":[15,7]", "\"GamePoints\":[7,15]"));
        string invalidReview = (string)Call("CaptureMatch");
        Assert.Throws<TargetInvocationException>(() => Call("RestoreMatch", invalidReview));
        Assert.That(Call("CaptureMatch"), Is.EqualTo(invalidReview), "Invalid review history mutated live state");
    }
    [UnityTest] public IEnumerator LegacyGameScoresStayUnknownWhileNewGamesSurviveSaveAndResult()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        foreach (int version in new[] { 1, 2 }) foreach (bool doubles in new[] { false, true }) foreach (int winner in new[] { 0, 1 })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("StartMatch");
            var match = Get<object>("Match"); var mt = match.GetType();
            for (int i = 0; i < 15; i++) mt.GetMethod("AwardPoint").Invoke(match, new object[] { winner });
            Call("NextPoint"); string saved = (string)Call("CaptureMatch");
            saved = saved.Replace("\"version\":3", "\"version\":" + version);
            saved = Regex.Replace(saved, "\"GamePoints\":\\[[^\\]]*\\],", "");
            Call("StartMatch"); Call("RestoreMatch", saved); match = Get<object>("Match");
            Assert.That((int[])mt.GetField("GamePoints").GetValue(match), Is.EqualTo(new[] { -1, -1 }), "Old scores must not be invented");
            for (int i = 0; i < 15; i++) mt.GetMethod("AwardPoint").Invoke(match, new object[] { winner });
            Call("NextPoint"); string final = (string)Call("CaptureMatch");
            Call("StartMatch"); Call("RestoreMatch", final); match = Get<object>("Match");
            var expected = winner == 0 ? new[] { -1, -1, 15, 0 } : new[] { -1, -1, 0, 15 };
            Assert.That((int[])mt.GetField("GamePoints").GetValue(match), Is.EqualTo(expected));
            string result = (string)Call("CaptureResult");
            StringAssert.Contains("\"gamePoints\":[" + string.Join(",", expected) + "]", result);
            StringAssert.Contains("\"winner\":" + winner, result);
            string legacyFinal = Regex.Replace(final.Replace("\"version\":3", "\"version\":" + version), "\"GamePoints\":\\[[^\\]]*\\],", "");
            Call("StartMatch"); Call("RestoreMatch", legacyFinal);
            Assert.That((int[])mt.GetField("GamePoints").GetValue(Get<object>("Match")), Is.EqualTo(expected), "Legacy final score is known; earlier game is not");
        }
    }
    [UnityTest] public IEnumerator DecidingGameReviewReplacesFinalScoreOnceAcrossFormatsEndsAndReplay()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        foreach (bool doubles in new[] { false, true }) foreach (int winner in new[] { 0, 1 })
        foreach (bool swapped in new[] { false, true }) foreach (int replayMode in new[] { 1, 2 })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("StartMatch"); type.GetProperty("ReplayMode").SetValue(court, replayMode);
            var match = Get<object>("Match"); var mt = match.GetType();
            Action<int> point = side => mt.GetMethod("AwardPoint").Invoke(match, new object[] { side });
            for (int side = 0; side < 2; side++) for (int i = 0; i < 15; i++) point(side);
            for (int i = 0; i < 20; i++) { point(0); point(1); }
            mt.GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
            string prior = JsonUtility.ToJson(match); point(1 - winner);
            Set("beforePoint", prior); Set("reviewAvailable", true); Set("announcedIn", false); Set("lastHitter", winner); Set("lastPlayer", winner);
            Set("lastLanding", new Vector3(0, .04f, (winner == 0 ? 3 : -3) * (swapped ? -1 : 1)));
            Set("serviceFlight", false); Set("flying", false); type.GetProperty("MatchState").SetValue(court, "MatchEnd");
            string saved = (string)Call("CaptureMatch"); Call("RestoreMatch", saved);
            Assert.That(Call("ChallengeCall"), Is.EqualTo(true)); match = Get<object>("Match");
            var expected = winner == 0 ? new[] { 15, 0, 0, 15, 21, 20 } : new[] { 15, 0, 0, 15, 20, 21 };
            Assert.That((int[])mt.GetField("GamePoints").GetValue(match), Is.EqualTo(expected));
            Assert.That((int)mt.GetField("Winner").GetValue(match), Is.EqualTo(winner));
            Assert.That((int[])mt.GetField("Challenges").GetValue(match), Is.EqualTo(new[] { 2, 2 }));
            string result = (string)Call("CaptureResult"); Call("SkipReplay");
            Assert.That(Call("ChallengeCall"), Is.False);
            Call("RestoreMatch", (string)Call("CaptureMatch"));
            Assert.That(Call("CaptureResult"), Is.EqualTo(result));
            Call("NextPoint"); Assert.That(Call("CaptureResult"), Is.EqualTo(result));
        }
    }
    [UnityTest] public IEnumerator ReviewCannotReplaceEarlierGameWithDifferentValidScore()
    {
        yield return Boot(); ((Behaviour)court).enabled = false; Call("StartMatch");
        var match = Get<object>("Match"); var mt = match.GetType();
        for (int i = 0; i < 15; i++) mt.GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
        Call("NextPoint"); string prior = JsonUtility.ToJson(match);
        mt.GetMethod("AwardPoint").Invoke(match, new object[] { 1 });
        Set("beforePoint", prior.Replace("\"GamePoints\":[15,0]", "\"GamePoints\":[15,7]"));
        Set("reviewAvailable", true); Set("announcedIn", false); Set("lastHitter", 0); Set("lastLanding", new Vector3(0, .04f, 3));
        type.GetProperty("MatchState").SetValue(court, "Point");
        foreach (string forged in new[] {
            prior.Replace("\"GamePoints\":[15,0]", "\"GamePoints\":[15,7]"),
            prior.Replace("\"GamePoints\":[15,0]", "\"GamePoints\":[]").Replace("\"Games\":[1,0]", "\"Games\":[0,0]").Replace("\"GameNumber\":2", "\"GameNumber\":1"),
            prior.Replace("\"Points\":[0,0]", "\"Points\":[7,3]") })
        {
            Set("beforePoint", forged); string bad = (string)Call("CaptureMatch");
            var error = Assert.Throws<TargetInvocationException>(() => Call("RestoreMatch", bad), forged);
            Assert.That(error.InnerException, Is.TypeOf<ArgumentException>());
            Assert.That(Call("CaptureMatch"), Is.EqualTo(bad), "Forged prior score changed live state");
        }
    }
    [UnityTest] public IEnumerator LegacyGameEndingReviewRetainsObservedPointsWithoutInventingEarlierScores()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        foreach (int version in new[] { 1, 2 })
        {
            Call("StartMatch"); var match = Get<object>("Match"); var mt = match.GetType();
            for (int i = 0; i < 14; i++) mt.GetMethod("AwardPoint").Invoke(match, new object[] { 1 });
            string prior = JsonUtility.ToJson(match); mt.GetMethod("AwardPoint").Invoke(match, new object[] { 1 });
            Set("beforePoint", prior); Set("reviewAvailable", true); Set("announcedIn", false); Set("lastHitter", 0);
            Set("lastLanding", new Vector3(0, .04f, 3)); Set("serviceFlight", false); Set("flying", false);
            type.GetProperty("MatchState").SetValue(court, "Point");
            var snapshotType = type.GetNestedType("Snapshot", BindingFlags.NonPublic);
            object legacy = JsonUtility.FromJson((string)Call("CaptureMatch"), snapshotType);
            snapshotType.GetField("version").SetValue(legacy, version);
            mt.GetField("GamePoints").SetValue(snapshotType.GetField("match").GetValue(legacy), null);
            snapshotType.GetField("beforePoint").SetValue(legacy, Regex.Replace(prior, "\"GamePoints\":\\[[^\\]]*\\],", ""));
            Call("RestoreMatch", JsonUtility.ToJson(legacy));
            Assert.That((int[])mt.GetField("GamePoints").GetValue(Get<object>("Match")), Is.EqualTo(new[] { 0, 15 }));
            Assert.That(Call("ChallengeCall"), Is.True); Call("SkipReplay");
            Assert.That((int[])mt.GetField("GamePoints").GetValue(Get<object>("Match")), Is.Empty);
            Assert.That((int[])mt.GetField("Points").GetValue(Get<object>("Match")), Is.EqualTo(new[] { 1, 14 }));
            Call("SaveMatch"); Call("StartMatch"); Call("LoadMatch");
            Assert.That((int[])mt.GetField("Points").GetValue(Get<object>("Match")), Is.EqualTo(new[] { 1, 14 }));
            Assert.That(Call("ChallengeCall"), Is.False);
        }
    }
    [UnityTest] public IEnumerator SnapshotRejectsInvalidFieldsBeforeAnyMutation()
    {
        yield return Boot(); Call("StartMatch");
        string valid = (string)Call("CaptureMatch");
        foreach (var pair in new[] { new[]{"jumpElapsed", "-1"}, new[]{"jumpHeight", "10"}, new[]{"chargeElapsed", "-1"}, new[]{"aiElapsed", "-1"}, new[]{"energyGame", "0"}, new[]{"stroke", "\"Unknown\""}, new[]{"Challenges", "[-1,2]"} })
        {
            string bad = Regex.Replace(valid, "\"" + pair[0] + "\":(?:\\[[^\\]]*\\]|\"[^\"]*\"|[^,}]+)", "\"" + pair[0] + "\":" + pair[1]);
            Assert.That(bad, Is.Not.EqualTo(valid), pair[0] + " field missing");
            var error = Assert.Throws<TargetInvocationException>(() => Call("RestoreMatch", bad), pair[0]);
            Assert.That(error.InnerException, Is.TypeOf<ArgumentException>(), pair[0]);
            Assert.That((string)Call("CaptureMatch"), Is.EqualTo(valid), pair[0] + " mutated live state");
        }
    }
    [UnityTest] public IEnumerator SnapshotDuringReplayRestoresLiveStateAndReviewQuota()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", false);
        yield return new WaitForSeconds(.2f);
        var live = GameObject.Find("Shuttle").transform.position;
        Assert.That(Call("BeginReplay"), Is.EqualTo(true));
        yield return new WaitForSeconds(.05f);
        string json = (string)Call("CaptureMatch");
        Call("RestoreMatch", json);
        Assert.That(GameObject.Find("Shuttle").transform.position, Is.EqualTo(live), "Save must never capture replay display as live physics");
        Assert.That((bool)type.GetField("replaying", Hidden).GetValue(court), Is.False);
    }
    [UnityTest] public IEnumerator ServeLandingBeforeShortLineAwardsReceiver()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", false);
        GameObject.Find("Shuttle").transform.position = new Vector3(-1, .045f, 1);
        Set("velocity", Vector3.down);
        LogAssert.Expect(LogType.Log, new Regex(@"\[collision\] ground.*"));
        yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
        var match = Get<object>("Match");
        Assert.That(((int[])match.GetType().GetField("Points").GetValue(match))[1], Is.EqualTo(1), "Serve before1.98m short line is fault even inside singles court");
    }
    [UnityTest] public IEnumerator ServiceAreasFollowBothServersParityAndRestore()
    {
        yield return Boot();
        for (int server = 0; server < 2; server++)
        for (int parity = 0; parity < 2; parity++)
        foreach (bool swapped in new[] { false, true })
        foreach (bool legal in new[] { true, false })
        {
            Call("StartMatch"); var match = Get<object>("Match"); var mt = match.GetType();
            var points = (int[])mt.GetField("Points").GetValue(match); points[server] = parity;
            mt.GetField("Server").SetValue(match, server); mt.GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
            while (new[]{"Ready","ChangeEnds"}.Contains(Get<string>("FlowStage"))) Call("AdvanceFlow");
            if (server == 0) Call("Serve", false);
            else { Set("serveReadyAt", Time.time - 1); type.GetMethod("UpdateOpponent", Hidden).Invoke(court, null); }
            Assert.That((bool)type.GetField("serviceFlight", Hidden).GetValue(court), Is.True);
            string snapshot = (string)Call("CaptureMatch"); Call("StartMatch"); Call("RestoreMatch", snapshot);
            match = Get<object>("Match"); points = (int[])mt.GetField("Points").GetValue(match);
            float side = parity == 0 ? 1 : -1;
            float x = (server == 0 ? -side : side) * (legal ? 1 : -1);
            GameObject.Find("Shuttle").transform.position = new Vector3(x * (swapped ? -1 : 1), .045f, (server == 0 ? 3 : -3) * (swapped ? -1 : 1));
            Set("velocity", Vector3.down);
            LogAssert.Expect(LogType.Log, new Regex(@"\[collision\] ground.*"));
            type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
            int winner = legal ? server : 1 - server;
            Assert.That(points[winner], Is.EqualTo((winner == server ? parity : 0) + 1), "server=" + server + " parity=" + parity + " legal=" + legal);
        }
    }
    [UnityTest] public IEnumerator LetReplaysRallyWithoutScoreEnergyOrServerChange()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", true);
        float energy = Get<float>("Energy");
        Assert.That(type.GetMethod("Let"), Is.Not.Null, "Let entrypoint missing");
        Call("Let");
        Assert.That(Get<string>("MatchState"), Is.EqualTo("Serving"));
        Assert.That(Get<bool>("Flying"), Is.False);
        Assert.That(Get<float>("Energy"), Is.EqualTo(energy));
        var match = Get<object>("Match");
        Assert.That((int[])match.GetType().GetField("Points").GetValue(match), Is.EqualTo(new[] { 0, 0 }));
        Assert.That((int)match.GetType().GetField("Server").GetValue(match), Is.EqualTo(0));
    }
    [UnityTest] public IEnumerator SuccessfulReviewReversesGameWinningWrongCall()
    {
        yield return Boot(); Call("StartMatch");
        var match = Get<object>("Match"); var mt = match.GetType();
        ((int[])mt.GetField("Points").GetValue(match))[1] = 14;
        Call("Serve", false);
        GameObject.Find("Shuttle").transform.position = new Vector3(-1, .045f, 3);
        Set("velocity", Vector3.down);
        LogAssert.Expect(LogType.Log, new Regex(@"\[collision\] ground.*"));
        type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        // Inject mistaken line-judge announcement, not different true landing.
        string before = "{\"Points\":[0,14],\"Games\":[0,0],\"Challenges\":[2,2],\"Server\":0,\"Winner\":-1,\"GameNumber\":1}";
        JsonUtility.FromJsonOverwrite(before, match); mt.GetMethod("AwardPoint").Invoke(match, new object[] { 1 });
        Set("announcedIn", false);
        string saved = (string)Call("CaptureMatch"); Call("StartMatch"); Call("RestoreMatch", saved);
        Assert.That(Call("ChallengeCall"), Is.EqualTo(true));
        match = Get<object>("Match");
        Assert.That((int[])mt.GetField("Points").GetValue(match), Is.EqualTo(new[] { 1, 14 }));
        Assert.That((int[])mt.GetField("Games").GetValue(match), Is.EqualTo(new[] { 0, 0 }));
        Assert.That((int[])mt.GetField("Challenges").GetValue(match), Is.EqualTo(new[] { 2, 2 }));
        Assert.That(Get<string>("MatchState"), Is.EqualTo("Point"));
    }
    [UnityTest] public IEnumerator ConfirmedChallengeWithoutReplayConsumesOneQuotaAndAdvancesOnce()
    {
        yield return Boot(); Call("StartMatch");
        type.GetProperty("ReplayMode").SetValue(court, 2);
        Call("Serve", false); yield return new WaitForSeconds(.1f);
        GameObject.Find("Shuttle").transform.position = new Vector3(-1, .045f, 3);
        Set("velocity", Vector3.down);
        LogAssert.Expect(LogType.Log, new Regex(@"\[collision\] ground.*"));
        type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        Set("pointWait", 2.9f);
        string saved = (string)Call("CaptureMatch"); Call("RestoreMatch", saved);
        Assert.That(Call("ChallengeCall"), Is.EqualTo(true));
        Assert.That((bool)type.GetField("replaying", Hidden).GetValue(court), Is.False);
        var match = Get<object>("Match"); var mt = match.GetType();
        Assert.That((int[])mt.GetField("Points").GetValue(match), Is.EqualTo(new[]{1,0}));
        Assert.That((int[])mt.GetField("Challenges").GetValue(match), Is.EqualTo(new[]{1,2}));
        Assert.That(Call("ChallengeCall"), Is.EqualTo(false));
        type.GetMethod("ContinuePoint", Hidden).Invoke(court, new object[]{.11f});
        Assert.That(Get<string>("MatchState"), Is.EqualTo("Serving"));
        type.GetMethod("ContinuePoint", Hidden).Invoke(court, new object[]{5f});
        Assert.That(Get<bool>("Flying"), Is.False, "Preparation must not auto-serve");
        Assert.That((int[])mt.GetField("Points").GetValue(match), Is.EqualTo(new[]{1,0}));
        Assert.That((int[])mt.GetField("Challenges").GetValue(match), Is.EqualTo(new[]{1,2}));
    }
    [UnityTest] public IEnumerator ChallengeNearDeadlineFreezesWindowAndContinuesCorrectedScoreOnce()
    {
        yield return Boot(); Call("StartMatch");
        var match = Get<object>("Match"); var mt = match.GetType();
        ((int[])mt.GetField("Points").GetValue(match))[1] = 14;
        Call("Serve", false);
        yield return new WaitForSeconds(.1f);
        GameObject.Find("Shuttle").transform.position = new Vector3(-1, .045f, 3);
        Set("velocity", Vector3.down);
        LogAssert.Expect(LogType.Log, new Regex(@"\[collision\] ground.*"));
        type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        string before = "{\"Points\":[0,14],\"Games\":[0,0],\"Challenges\":[2,2],\"Server\":0,\"Winner\":-1,\"GameNumber\":1}";
        JsonUtility.FromJsonOverwrite(before, match);
        mt.GetMethod("AwardPoint").Invoke(match, new object[] { 1 });
        Set("announcedIn", false); Set("pointWait", 2.9f);
        string saved = (string)Call("CaptureMatch"); Call("RestoreMatch", saved);
        Assert.That(Call("ChallengeCall"), Is.EqualTo(true));
        Assert.That((bool)type.GetField("replaying", Hidden).GetValue(court), Is.True);
        type.GetMethod("ContinuePoint", Hidden).Invoke(court, new object[] { 5f });
        Assert.That(Get<string>("MatchState"), Is.EqualTo("Point"));
        Assert.That((float)type.GetField("pointWait", Hidden).GetValue(court), Is.EqualTo(2.9f).Within(.001f));
        Call("SkipReplay");
        saved = (string)Call("CaptureMatch"); Call("RestoreMatch", saved);
        type.GetMethod("ContinuePoint", Hidden).Invoke(court, new object[] { .11f });
        Assert.That(Get<string>("MatchState"), Is.EqualTo("Serving"));
        Assert.That(Get<string>("FlowStage"), Is.Not.EqualTo("GameEnd"));
        match = Get<object>("Match");
        Assert.That((int[])mt.GetField("Points").GetValue(match), Is.EqualTo(new[] { 1, 14 }));
        Assert.That((int[])mt.GetField("Games").GetValue(match), Is.EqualTo(new[] { 0, 0 }));
        Assert.That((int[])mt.GetField("Challenges").GetValue(match), Is.EqualTo(new[] { 2, 2 }));
        Assert.That(Call("ChallengeCall"), Is.EqualTo(false), "Old review must not leak into next point");
        saved = (string)Call("CaptureMatch");
        type.GetMethod("ContinuePoint", Hidden).Invoke(court, new object[] { 10f });
        Assert.That(Call("CaptureMatch"), Is.EqualTo(saved), "No duplicate point transition");
    }
    [UnityTest] public IEnumerator CorruptReviewHistoryRejectedBeforeMutation()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", false);
        GameObject.Find("Shuttle").transform.position = new Vector3(-1, .045f, 3); Set("velocity", Vector3.down);
        LogAssert.Expect(LogType.Log, new Regex(@"\[collision\] ground.*")); type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        Set("beforePoint", "{}"); string json = (string)Call("CaptureMatch");
        var live = Get<object>("Match");
        Assert.Throws<TargetInvocationException>(() => Call("RestoreMatch", json));
        Assert.That(Get<object>("Match"), Is.SameAs(live));
    }
    [UnityTest] public IEnumerator SavedPointRetainsOriginalReplayFrames()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", true);
        yield return new WaitForSeconds(.2f);
        string json = (string)Call("CaptureMatch");
        Assert.That(Call("BeginReplay"), Is.EqualTo(true)); Call("SkipReplay");
        Call("StartMatch"); Call("RestoreMatch", json);
        Assert.That(Call("BeginReplay"), Is.EqualTo(true), "Save/load must retain original trajectory, not regenerate it");
        Call("SkipReplay");
    }
    [UnityTest] public IEnumerator InvalidReplayFrameCannotClearLiveReplay()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", true); yield return new WaitForSeconds(.2f);
        string json = (string)Call("CaptureMatch");
        json = Regex.Replace(json, "(\"frames\":\\[\\{\"player\":\\{\"x\":)[^,]+", "${1}9999");
        Assert.Throws<TargetInvocationException>(() => Call("RestoreMatch", json));
        Assert.That(Call("BeginReplay"), Is.EqualTo(true)); Call("SkipReplay");
    }
    [UnityTest] public IEnumerator ThirtySecondFixedDrainRecordsPartialRecovery()
    {
        yield return Boot(); Call("StartMatch");
        // Fixed 1500 ticks at .02s; stop flight between ticks, not a natural rally proof.
        Call("Serve", true); Set("flying", false);
        for (int i = 0; i < 1500; i++) type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        float remaining = Get<float>("Energy");
        System.IO.File.WriteAllText(EvidenceDirectory + "/energy-baseline.json", "{\"seconds\":30,\"idleRallyRemaining\":" + remaining.ToString(System.Globalization.CultureInfo.InvariantCulture) + ",\"scope\":\"fixed drain only; lob contact cost separate\"}");
        Assert.That(remaining, Is.InRange(0f, 100f));
        var match = Get<object>("Match");
        for (int i = 0; i < 15; i++) match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
        Call("NextPoint"); Assert.That(Get<float>("Energy") - remaining, Is.EqualTo(Mathf.Min(8, 100 - remaining)).Within(.001f));
        Assert.That(remaining, Is.EqualTo(100).Within(.001f), "Idle Rally no longer spends movement energy");
    }
    [UnityTest] public IEnumerator OriginalAudioEventsProduceBoundedSamples()
    {
        yield return Boot();
        Assert.That(type.GetMethod("CreateSound"), Is.Not.Null, "Original procedural SFX missing");
        foreach (string sound in new[] { "racket", "net", "step", "whistle", "score" })
        {
            var clip = (AudioClip)Call("CreateSound", sound);
            Assert.That(clip.length, Is.InRange(.02f, 2f));
            var samples = new float[clip.samples]; Assert.That(clip.GetData(samples, 0), Is.True);
            float peak = 0; foreach (float sample in samples) { Assert.That(float.IsFinite(sample), Is.True); peak = Mathf.Max(peak, Mathf.Abs(sample)); }
            Assert.That(peak, Is.InRange(.01f, .8f));
            UnityEngine.Object.Destroy(clip);
        }
    }
    [UnityTest] public IEnumerator MatchFlowOffersReadyIntervalChangeEndsAndResult()
    {
        yield return Boot();
        Assert.That(type.GetProperty("FlowStage"), Is.Not.Null, "Match transition presentation missing");
        Call("StartMatch"); Assert.That(Get<string>("FlowStage"), Is.EqualTo("Ready"));
        Call("AdvanceFlow"); Assert.That(Get<string>("FlowStage"), Is.EqualTo("Serving"));
        var match = Get<object>("Match");
        for (int i = 0; i < 15; i++) match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
        Call("NextPoint"); Assert.That(Get<string>("FlowStage"), Is.EqualTo("GameEnd"));
        Call("AdvanceFlow"); Assert.That(Get<string>("FlowStage"), Is.EqualTo("Interval"));
        Call("AdvanceFlow"); Assert.That(Get<string>("FlowStage"), Is.EqualTo("ChangeEnds"));
        Call("AdvanceFlow"); Assert.That(Get<string>("FlowStage"), Is.EqualTo("Ready"));
        for (int i = 0; i < 15; i++) match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
        Call("NextPoint"); Assert.That(Get<string>("FlowStage"), Is.EqualTo("MatchEnd"));
    }
    [UnityTest] public IEnumerator SpectacularJumpSmashOffersReplayOnlyWhenEnabled()
    {
        yield return Boot();
        Assert.That(type.GetProperty("SpectacularMoment"), Is.Not.Null);
        Call("Feed", "Lob");
        var player = GameObject.Find("Player").transform;
        GameObject.Find("Shuttle").transform.position = player.position + new Vector3(.3f, 2.1f, .35f);
        Set("jumpY", .3f); Call("BeginShot", "Smash");
        LogAssert.Expect(LogType.Log, new Regex(@"\[hit\].*")); Call("ReleaseShot");
        Assert.That(Get<bool>("SpectacularMoment"), Is.True);
        Call("Feed", "Net"); Assert.That(Get<bool>("SpectacularMoment"), Is.False);
    }
    [UnityTest] public IEnumerator ReplayElapsedTimeCannotAdvanceLiveCharge()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", true); yield return new WaitForSeconds(.2f);
        Set("lastHitter", 1); Call("BeginShot", "Clear");
        float charged = Time.time - (float)type.GetField("chargedAt", Hidden).GetValue(court);
        Assert.That(Call("BeginReplay"), Is.EqualTo(true)); yield return new WaitForSeconds(.15f);
        Call("SkipReplay");
        float after = Time.time - (float)type.GetField("chargedAt", Hidden).GetValue(court);
        Assert.That(after, Is.EqualTo(charged).Within(.04f), "Replay display must not charge live shot");
    }
    [UnityTest] public IEnumerator StaminaDrainDependsOnLiveRallyNotMovementOrBoundaryInput()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        var rows = new System.Collections.Generic.List<string> { "doubles,end,rally,mode,metres,player,opponent,partner,second" };
        var measured = new System.Collections.Generic.Dictionary<string, float>();
        var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
        float[] Read() => new[] { Get<float>("Energy"), Get<float>("OpponentEnergy"), (float)type.GetField("partnerEnergy", Hidden).GetValue(court), (float)type.GetField("secondEnergy", Hidden).GetValue(court) };
        foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
        foreach (bool rally in new[] { false, true }) foreach (string mode in new[] { "idle", "manual", "assisted", "boundary" })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("StartMatch");
            var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
            float end = swapped ? -1 : 1;
            type.GetProperty("Energy").SetValue(court, 63f); type.GetProperty("OpponentEnergy").SetValue(court, 58f);
            Set("partnerEnergy", 47f); Set("secondEnergy", 29f);
            if (rally) { Call("AdvanceFlow"); Call("Serve", true); Set("serviceFlight", false); }
            // Controlled fixed-step probe: isolate energy from AI hits/point transitions, not a natural rally.
            Set("flowStage", "Ready"); Set("flying", rally || mode == "assisted");
            Set("lastHitter", 1); Set("lastPlayer", 1); Set("assisted", mode == "assisted");
            player.position = new Vector3(mode == "boundary" ? 3.4f : 0, 0, -4 * end);
            Call("SetAnalog", mode == "manual" || mode == "boundary" ? Vector2.right * end : Vector2.zero);
            float moved = 0; float[] initial = Read();
            for (int tick = 0; tick < 100; tick++)
            {
                shuttle.position = player.position + new Vector3(2, 5, 0); Set("velocity", Vector3.zero);
                Vector3 before = player.position; type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
                moved += Vector3.Distance(before, player.position);
            }
            float[] after = Read(); string key = doubles + "/" + swapped + "/" + rally;
            rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6},{7},{8}", doubles, end, rally, mode, moved, after[0], after[1], after[2], after[3]));
            System.IO.File.WriteAllLines(EvidenceDirectory + "/stamina-drain.csv", rows);
            for (int id = 0; id < 4; id++)
                Assert.That(after[id], Is.EqualTo(initial[id] - (rally && (id < 2 || doubles) ? .6f * 100 * Time.fixedDeltaTime : 0)).Within(.001f), key + "/" + mode + "/athlete" + id);
            if (mode == "manual") { Assert.That(moved, Is.GreaterThan(1)); measured[key] = moved; }
            else if (mode == "assisted") Assert.That(moved, Is.EqualTo(measured[key]).Within(.001f), "Same travel must have same zero movement cost");
            else Assert.That(moved, Is.EqualTo(0).Within(.00001f), mode);
        }
        Assert.That(rows.Count - 1, Is.EqualTo(32));
    }
    [UnityTest] public IEnumerator StaminaPauseReplayAndGameRecoveryPreserveAllAthletes()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        var rows = new System.Collections.Generic.List<string> { "doubles,end,stage,player,opponent,partner,second" };
        float[] Read() => new[] { Get<float>("Energy"), Get<float>("OpponentEnergy"), (float)type.GetField("partnerEnergy", Hidden).GetValue(court), (float)type.GetField("secondEnergy", Hidden).GetValue(court) };
        void Tick() => type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        void Ui(string action) => type.GetMethod("ActOnUi", Hidden).Invoke(court, new object[] { action });
        foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("StartMatch");
            var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
            for (int gate = 0; gate < 3 && Get<string>("FlowStage") != "Serving"; gate++) Call("AdvanceFlow");
            Assert.That(Get<string>("FlowStage"), Is.EqualTo("Serving"));
            Call("Serve", true); Assert.That(Get<string>("MatchState"), Is.EqualTo("Rally"));
            type.GetProperty("Energy").SetValue(court, 63f); type.GetProperty("OpponentEnergy").SetValue(court, 58f);
            Set("partnerEnergy", 47f); Set("secondEnergy", 99f);
            Tick(); Tick(); float[] energy = Read();
            void Check(string stage, float[] expected)
            {
                var actual = Read(); rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6}", doubles, swapped ? -1 : 1, stage, actual[0], actual[1], actual[2], actual[3]));
                System.IO.File.WriteAllLines(EvidenceDirectory + "/stamina-freeze-recovery.csv", rows);
                for (int id = 0; id < (doubles ? 4 : 2); id++) Assert.That(actual[id], Is.EqualTo(expected[id]).Within(.001f), stage + "/" + id);
            }
            Assert.That(Call("BeginReplay"), Is.EqualTo(true));
            for (int tick = 0; tick < 100; tick++) Tick(); Check("replay-frozen", energy);
            string saved = (string)Call("CaptureMatch"); Call("SkipReplay"); Call("StartMatch"); Call("RestoreMatch", saved); Check("replay-snapshot", energy);
            Ui("Pause"); for (int tick = 0; tick < 100; tick++) Tick(); Call("Jump"); Check("pause-frozen", energy); Ui("Lanjut");
            match = Get<object>("Match"); match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 }); Call("NextPoint"); Check("normal-point", energy);
            for (int point = 0; point < 14; point++) match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
            Call("NextPoint"); var recovered = energy.Select(e => Mathf.Min(100, e + 8)).ToArray(); Check("intergame-once", recovered);
            saved = (string)Call("CaptureMatch"); Call("StartMatch"); Call("RestoreMatch", saved); Call("NextPoint"); Check("intergame-restored", recovered);
            foreach (string stage in new[] { "Interval", "ChangeEnds", "Ready" })
            {
                Call("AdvanceFlow"); Assert.That(Get<string>("FlowStage"), Is.EqualTo(stage));
                for (int tick = 0; tick < 100; tick++) Tick(); Check(stage, recovered);
            }
            match = Get<object>("Match"); for (int point = 0; point < 15; point++) match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
            Call("NextPoint"); Check("match-end-no-recovery", recovered); Call("NextPoint"); Check("match-end-repeat", recovered);
            Ui("Menu"); for (int tick = 0; tick < 100; tick++) Tick(); Check("home-idle", recovered);
            Call("EnterTraining"); for (int tick = 0; tick < 100; tick++) Tick(); Check("training-idle", recovered);
        }
        Assert.That(rows.Count - 1, Is.EqualTo(52));
    }
    [UnityTest] public IEnumerator StaminaChargesExactHitAndJumpCostsWithoutMissOrRepeatCharge()
    {
        yield return Boot(); ((Behaviour)court).enabled = false;
        var rows = new System.Collections.Generic.List<string> { "doubles,end,action,before,after" };
        foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("StartMatch");
            var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
            for (int gate = 0; gate < 3 && Get<string>("FlowStage") != "Serving"; gate++) Call("AdvanceFlow");
            Call("Serve", true); Assert.That(Get<string>("MatchState"), Is.EqualTo("Rally")); Set("serviceFlight", false);
            var root = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
            foreach (string shot in new[] { "Smash", "Clear", "Drop", "Net", "Lift" }) foreach (bool reachable in new[] { true, false })
            {
                type.GetProperty("Energy").SetValue(court, 63f); Set("lastHitter", 1); Set("lastPlayer", 1);
                shuttle.position = root.TransformPoint(new Vector3(reachable ? .3f : 3f, shot == "Net" || shot == "Lift" ? 1 : 2.1f, .35f));
                Set("velocity", Vector3.zero); Call("BeginShot", shot);
                Assert.That(Call("ReleaseShot"), Is.EqualTo(reachable));
                float expected = reachable ? 63 - (shot == "Smash" ? 3 : 1) : 63;
                Assert.That(Get<float>("Energy"), Is.EqualTo(expected));
                Assert.That(Call("ReleaseShot"), Is.EqualTo(false)); Assert.That(Get<float>("Energy"), Is.EqualTo(expected));
                rows.Add(doubles + "," + (swapped ? -1 : 1) + "," + shot + (reachable ? "-hit" : "-miss") + ",63," + expected);
            }
            foreach (float energy in new[] { 63f, 3f })
            {
                type.GetProperty("Energy").SetValue(court, energy); Set("jumpAt", -10f); Set("jumpY", 0f); Set("jumpPhase", 1f);
                Call("Jump"); float expected = energy >= 4 ? energy - 4 : energy;
                Assert.That(Get<float>("Energy"), Is.EqualTo(expected)); Call("Jump"); Assert.That(Get<float>("Energy"), Is.EqualTo(expected));
                rows.Add(doubles + "," + (swapped ? -1 : 1) + ",jump," + energy + "," + expected);
            }
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/stamina-costs.csv", rows);
        Assert.That(rows.Count - 1, Is.EqualTo(48));
    }
    [UnityTest] public IEnumerator NaturalLobRallyMeasuresThirtySecondEnergyBaseline()
    {
        yield return Boot(); Call("ConfigureOpponent", "Easy", 37); Call("StartMatch"); Call("AdvanceFlow"); Set("assisted", true); Call("Serve", true);
        Time.timeScale = 20;
        float began = Time.time; int returns = 0; float moved = 0; var previous = Get<Vector3>("PlayerPosition");
        var samples = new System.Collections.Generic.List<string>();
        while (Get<string>("MatchState") == "Rally" && Time.time - began < 30)
        {
            var local = GameObject.Find("Shuttle").transform.position - GameObject.Find("Player").transform.position;
            if ((int)type.GetField("lastHitter", Hidden).GetValue(court) == 1 && local.y > .4f && local.y < 2.7f && new Vector2(local.x, local.z).magnitude < 1)
            {
                Call("SetAnalog", new Vector2(1, 1)); Call("BeginShot", local.y >= 1.45f ? "Clear" : "Lift");
                LogAssert.Expect(LogType.Log, new Regex(@"\[hit\].*"));
                if ((bool)Call("ReleaseShot")) returns++;
                Call("SetAnalog", Vector2.zero);
            }
            var position = Get<Vector3>("PlayerPosition"); moved += Vector3.Distance(position, previous); previous = position;
            samples.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1}", (Time.time - began), Get<float>("Energy")));
            yield return new WaitForFixedUpdate();
        }
        float duration = Time.time - began, energy = Get<float>("Energy"); Time.timeScale = 1;
        // D-043: rallyDrain config via reflection (test asmdef no cross-reference)
        var bootType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.BootDiagnostics")).First(t => t != null);
        float drain = (float)bootType.GetProperty("Current", BindingFlags.Static | BindingFlags.Public).GetValue(null).GetType().GetField("rallyDrain").GetValue(bootType.GetProperty("Current", BindingFlags.Static | BindingFlags.Public).GetValue(null));
        System.IO.File.WriteAllText(EvidenceDirectory + "/natural-lob-energy.json", string.Format(System.Globalization.CultureInfo.InvariantCulture, "{{\"seconds\":{0},\"returns\":{1},\"energyRemaining\":{2},\"energyCapacity\":{3},\"rallyDrainPerSecond\":{4},\"samples\":[{5}]}}", duration, returns, energy, Get<float>("EnergyCapacity"), drain, string.Join(",", samples)));
        Assert.That(duration, Is.GreaterThanOrEqualTo(30), "Natural baseline cannot end before30 seconds");
        Assert.That(returns, Is.GreaterThan(0));
        System.IO.File.WriteAllText(EvidenceDirectory + "/moving-rally.json", string.Format(System.Globalization.CultureInfo.InvariantCulture, "{{\"seconds\":{0},\"returns\":{1},\"movedMetres\":{2},\"energyRemaining\":{3}}}", duration, returns, moved, energy));
        Assert.That(moved, Is.GreaterThan(1f), "30s rally evidence requires actual movement, not stationary swings");
        // D-043: 30s rally must cut stamina noticeably, engage fatigue (<75% capacity) but never empty it.
        Assert.That(energy, Is.InRange(50f, 75f), "30s natural rally must leave stamina reduced but not empty");
        Assert.That(energy, Is.LessThan(Get<float>("EnergyCapacity") * .75f), "Fatigue must be engaged by30s rally");
    }
    [UnityTest] public IEnumerator SnapshotRejectsContradictoryMatchState()
    {
        yield return Boot(); Call("StartMatch");
        string json = (string)Call("CaptureMatch"); var live = Get<object>("Match");
        foreach (string invalid in new[] {
            json.Replace("\"state\":\"Serving\"", "\"state\":\"MatchEnd\""),
            json.Replace("\"flying\":false", "\"flying\":true"),
            json.Replace("\"GameNumber\":1", "\"GameNumber\":3") })
        {
            Assert.Throws<TargetInvocationException>(() => Call("RestoreMatch", invalid));
            Assert.That(Get<object>("Match"), Is.SameAs(live));
        }
    }
    [UnityTest] public IEnumerator FinishedMatchExportsReusableResult()
    {
        yield return Boot(); Call("StartMatch");
        Assert.That(type.GetMethod("CaptureResult"), Is.Not.Null);
        Assert.Throws<TargetInvocationException>(() => Call("CaptureResult"));
        var match = Get<object>("Match");
        for (int i = 0; i < 30; i++) match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
        string result = (string)Call("CaptureResult");
        StringAssert.Contains("\"winner\":0", result); StringAssert.Contains("\"rules\":\"Official15\"", result);
        StringAssert.Contains("\"games\":[2,0]", result);
    }
    [UnityTest] public IEnumerator NaturalEasyMatchFinishesWithoutPositionedContacts()
    {
        yield return Boot(); Call("StartMatch"); Set("assisted", true);
        Time.timeScale = 20;
        int contacts = 0, points = 0; float began = Time.time;
        while (Get<string>("MatchState") != "MatchEnd" && Time.time - began < 600)
        {
            if (Get<string>("FlowStage") == "Ready" || Get<string>("FlowStage") == "GameEnd" || Get<string>("FlowStage") == "Interval" || Get<string>("FlowStage") == "ChangeEnds") { Call("AdvanceFlow"); yield return new WaitForFixedUpdate(); continue; }
            if (Get<string>("MatchState") == "Point") { points++; Call("NextPoint"); }
            if (Get<string>("MatchState") == "Serving") Call("Serve", true);
            var local = GameObject.Find("Shuttle").transform.position - GameObject.Find("Player").transform.position;
            if (Get<string>("MatchState") == "Rally" && Get<bool>("Flying") && (int)type.GetField("lastHitter", Hidden).GetValue(court) == 1
                && local.y > .4f && local.y < 2.7f && new Vector2(local.x, local.z).magnitude < 1)
            {
                Call("SetAnalog", new Vector2(contacts % 2 == 0 ? -1 : 1, 1));
                Call("BeginShot", local.y >= 1.45f ? (contacts % 3 == 0 ? "Smash" : "Clear") : "Lift");
                LogAssert.Expect(LogType.Log, new Regex(@"\[hit\].*"));
                if ((bool)Call("ReleaseShot")) contacts++;
                Call("SetAnalog", Vector2.zero);
            }
            // Natural run retains all collision logs; they are expected gameplay output.
            yield return new WaitForFixedUpdate();
        }
        Time.timeScale = 1;
        System.IO.File.WriteAllText(EvidenceDirectory + "/trajectory-natural-match.json", "{\"seconds\":" + (Time.time-began).ToString(System.Globalization.CultureInfo.InvariantCulture) + ",\"playerReturns\":" + contacts + ",\"pointsBeforeFinal\":" + points + ",\"state\":\"" + Get<string>("MatchState") + "\"}");
        Assert.That(Get<string>("MatchState"), Is.EqualTo("MatchEnd"));
        Assert.That(contacts, Is.GreaterThan(0), "Natural AI return/interception required");
        System.IO.File.WriteAllText(EvidenceDirectory + "/natural-result.json", (string)Call("CaptureResult"));
    }
    [UnityTest] public IEnumerator RepresentativeSmashAndNetCrossAndLandWithoutRootTeleport()
    {
        yield return Boot();
        var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
        foreach (string shot in new[] { "Smash", "Net" })
        {
            Call("Feed", "Net"); player.position = new Vector3(0, 0, shot == "Net" ? -.9f : -3.5f);
            shuttle.position = player.position + new Vector3(.3f, shot == "Net" ? 1 : 2.1f, .35f);
            Call("SetAnalog", new Vector2(1, shot == "Net" ? -1 : 1));
            Call("BeginShot", shot); Set("chargedAt", Time.time - .4f);
            LogAssert.Expect(LogType.Log, new Regex(@"\[hit\].*"));
            var root = player.position; Assert.That(Call("ReleaseShot"), Is.EqualTo(true));
            Assert.That(player.position, Is.EqualTo(root)); Call("SetAnalog", Vector2.zero);
            LogAssert.Expect(LogType.Log, new Regex(@"\[collision\] ground.*"));
            int ticks = 0; while (Get<bool>("Flying") && ticks++ < 300) type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
            Assert.That(Get<bool>("Flying"), Is.False);
            Assert.That(shuttle.position.z, Is.GreaterThan(0), shot + " valid representative shot must reach opponent side");
            Assert.That(Mathf.Abs(shuttle.position.x), Is.LessThan(2.59f));
        }
    }
    [UnityTest] public IEnumerator PowerChangesFlightAndNetFaultRemainsPossible()
    {
        yield return Boot();
        var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
        Vector3 first = Vector3.zero;
        foreach (float charge in new[] { 0f, .8f })
        {
            Call("Feed", "Net"); player.position = new Vector3(0, 0, -3.5f);
            shuttle.position = player.position + new Vector3(.3f, 2.1f, .35f);
            Call("SetAnalog", new Vector2(1, -1)); Call("BeginShot", "Smash"); Set("chargedAt", Time.time - charge);
            LogAssert.Expect(LogType.Log, new Regex(@"\[hit\].*")); Assert.That(Call("ReleaseShot"), Is.EqualTo(true));
            if (charge == 0) first = Get<Vector3>("ShuttleVelocity");
            else Assert.That(Vector3.Distance(first, Get<Vector3>("ShuttleVelocity")), Is.GreaterThan(.1f));
            Call("SetAnalog", Vector2.zero);
            LogAssert.Expect(LogType.Log, new Regex(@"\[collision\] ground.*"));
            int ticks = 0; while (Get<bool>("Flying") && ticks++ < 300) type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
            Assert.That(Get<bool>("Flying"), Is.False);
            Assert.That(shuttle.position.z, Is.GreaterThan(0), "Valid smash envelope must cross without timing punishment");
        }
        Call("Feed", "Net"); Call("SetAnalog", Vector2.zero);
        shuttle.position = new Vector3(0, 1, -.08f); Set("velocity", new Vector3(0, 0, 8));
        type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        Assert.That(Get<Vector3>("ShuttleVelocity").z, Is.LessThan(0), "Below-tape shuttle must rebound, not receive clearance rescue");
        Call("Feed", "Net"); shuttle.position = player.position + new Vector3(3, 2.1f, 0);
        Call("BeginShot", "Smash"); LogAssert.Expect(LogType.Log, new Regex(@"\[hit\] Poor.*outside reach.*"));
        Assert.That(Call("ReleaseShot"), Is.EqualTo(false), "No aim/reach rescue");
    }
    [UnityTest] public IEnumerator StrokeCalibrationRecordsNetClearanceAndLandingPerCase()
    {
        yield return Boot();
        var rows = new System.Collections.Generic.List<string> { "stroke,x,y,charge,net,landingX,landingZ" };
        var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
        var tick = type.GetMethod("FixedUpdate", Hidden);
        foreach (string shot in new[] { "Smash", "Clear", "Drop", "Net", "Lift" })
        foreach (float x in new[] { -1f, 1f }) foreach (float y in new[] { -1f, 0f, 1f })
        foreach (float charge in new[] { 0f, .4f, .8f })
        {
            Call("Feed", "Net"); player.position = new Vector3(0, 0, shot == "Net" ? -.9f : -3.5f);
            shuttle.position = player.position + new Vector3(.3f, shot == "Net" || shot == "Lift" ? 1 : 2.1f, .35f);
            Set("velocity", Vector3.zero); Call("SetAnalog", new Vector2(x, y));
            Call("BeginShot", shot); Set("chargedAt", Time.time - charge);
            LogAssert.Expect(LogType.Log, new Regex(@"\[hit\].*")); Assert.That(Call("ReleaseShot"), Is.EqualTo(true));
            Call("SetAnalog", Vector2.zero);
            LogAssert.Expect(LogType.Log, new Regex(@"\[collision\] ground.*"));
            int ticks = 0; while (Get<bool>("Flying") && ticks++ < 300) tick.Invoke(court, null);
            Assert.That(Get<bool>("Flying"), Is.False);
            bool net = ((string)type.GetField("feedback", Hidden).GetValue(court)).StartsWith("Net |");
            rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6}", shot, x, y, charge, net, shuttle.position.x, shuttle.position.z));
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/trajectory-calibration.csv", rows);
    }
    [UnityTest] public IEnumerator ReplayAdditionalAngleRestoresCameraOnSkip()
    {
        yield return Boot(); Call("Feed", "Lob"); yield return new WaitForSeconds(.1f);
        var camera = Camera.main; var position = camera.transform.position; var rotation = camera.transform.rotation;
        type.GetProperty("ReplayMode").SetValue(court, 0);
        Assert.That(Call("BeginReplay"), Is.EqualTo(true)); yield return null;
        Assert.That(Mathf.Abs(camera.transform.position.x), Is.GreaterThan(5), "Additional angle must be umpire-side, not gameplay camera");
        Call("SkipReplay");
        Assert.That(camera.transform.position, Is.EqualTo(position));
        Assert.That(Quaternion.Angle(camera.transform.rotation, rotation), Is.LessThan(.01f));
    }
    [UnityTest] public IEnumerator PausedAndNonRallyInputsCannotMutateMatch()
    {
        yield return Boot(); Call("StartMatch");
        float energy = Get<float>("Energy");
        Time.timeScale = 0;
        Call("Jump"); Call("Serve", true); Call("BeginShot", "Clear");
        Assert.That(Get<float>("Energy"), Is.EqualTo(energy));
        Assert.That(Get<bool>("Flying"), Is.False);
        Assert.That((bool)type.GetField("charging", Hidden).GetValue(court), Is.False);
        Time.timeScale = 1;
        Call("BeginShot", "Clear");
        Assert.That((bool)type.GetField("charging", Hidden).GetValue(court), Is.False, "Cannot charge while Serving");
    }
    [UnityTest] public IEnumerator SnapshotRetainsPointReviewAndChallengeCannotLeakAcrossRestore()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", false);
        GameObject.Find("Shuttle").transform.position = new Vector3(0, .045f, 3);
        Set("velocity", Vector3.down);
        LogAssert.Expect(LogType.Log, new Regex(@"\[collision\] ground.*"));
        yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
        string point = (string)Call("CaptureMatch");
        Call("StartMatch"); Call("RestoreMatch", point);
        Assert.That(Call("ChallengeCall"), Is.EqualTo(true), "Saved point must retain review opportunity");
        string reviewed = (string)Call("CaptureMatch");
        Call("RestoreMatch", point); Call("RestoreMatch", reviewed);
        Assert.That(Call("ChallengeCall"), Is.EqualTo(false), "Reviewed point cannot be reviewed again after load");
    }
    [UnityTest] public IEnumerator FixedPhysicsIndependentOfRenderGroupingAndSnapshotContinuation()
    {
        yield return Boot(); Call("StartMatch"); Call("Serve", true);
        string json = (string)Call("CaptureMatch");
        var tick = type.GetMethod("FixedUpdate", Hidden);
        Vector3 expected = Vector3.zero, speed = Vector3.zero;
        foreach (int group in new[] { 1, 2, 4 })
        {
            Call("RestoreMatch", json);
            for (int frame = 0; frame < 24 / group; frame++)
                for (int i = 0; i < group; i++) tick.Invoke(court, null);
            var actual = GameObject.Find("Shuttle").transform.position;
            if (group == 1) { expected = actual; speed = Get<Vector3>("ShuttleVelocity"); }
            Assert.That(actual, Is.EqualTo(expected));
            Assert.That(Get<Vector3>("ShuttleVelocity"), Is.EqualTo(speed));
        }
    }
    [UnityTest] public IEnumerator FailedChallengeNearLineConsumesNothingAndKeepsWindowOpen()
    {
        yield return Boot(); Call("StartMatch");
        var match = Get<object>("Match"); var mt = match.GetType();
        ((int[])mt.GetField("Points").GetValue(match))[1] = 14;
        Call("Serve", false);
        yield return new WaitForSeconds(.1f);
        GameObject.Find("Shuttle").transform.position = new Vector3(-1, .045f, 3);
        Set("velocity", Vector3.down);
        LogAssert.Expect(LogType.Log, new Regex(@"\[collision\] ground.*"));
        type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        Set("pointWait", 2.9f);
        string saved = (string)Call("CaptureMatch"); Call("RestoreMatch", saved);
        match = Get<object>("Match"); mt = match.GetType();
        Assert.That((int[])mt.GetField("Points").GetValue(match), Is.EqualTo(new[] { 1, 14 }), "Real landing already awarded the point");
        ((int[])mt.GetField("Challenges").GetValue(match))[0] = 0;
        Assert.That(Call("ChallengeCall"), Is.EqualTo(false), "Exhausted quota must reject challenge");
        Assert.That((bool)type.GetField("reviewAvailable", Hidden).GetValue(court), Is.True, "Rejected challenge keeps window open");
        Assert.That(Get<string>("MatchState"), Is.EqualTo("Point"), "Failed challenge must not mutate state");
        Assert.That((float)type.GetField("pointWait", Hidden).GetValue(court), Is.EqualTo(2.9f).Within(.001f), "Failed challenge must not consume window");
        Assert.That((bool)type.GetField("replaying", Hidden).GetValue(court), Is.False, "Failed challenge must not start replay");
        match = Get<object>("Match"); mt = match.GetType();
        Assert.That((int[])mt.GetField("Challenges").GetValue(match), Is.EqualTo(new[] { 0, 2 }), "Rejected challenge consumes no quota");
        Assert.That((int[])mt.GetField("Points").GetValue(match), Is.EqualTo(new[] { 1, 14 }), "Rejected challenge must not alter live score");
        Assert.That((int[])mt.GetField("Games").GetValue(match), Is.EqualTo(new[] { 0, 0 }), "Rejected challenge must not alter games");
        ((int[])mt.GetField("Challenges").GetValue(match))[0] = 1;
        Assert.That(Call("ChallengeCall"), Is.EqualTo(true), "Window still usable after quota restored");
        Assert.That((bool)type.GetField("replaying", Hidden).GetValue(court), Is.True, "Challenge must start replay");
        match = Get<object>("Match");
        Assert.That((int[])match.GetType().GetField("Challenges").GetValue(match), Is.EqualTo(new[] { 0, 2 }), "Confirmed (failed) challenge consumes exactly one quota per P-008");
    }
    [UnityTest] public IEnumerator ChallengeWithReplayModeOffStillCorrectsAndAdvancesExactlyOnce()
    {
        yield return Boot(); Call("StartMatch");
        var match = Get<object>("Match"); var mt = match.GetType();
        type.GetProperty("ReplayMode").SetValue(court, 2);
        ((int[])mt.GetField("Points").GetValue(match))[1] = 14;
        Call("Serve", false);
        yield return new WaitForSeconds(.1f);
        GameObject.Find("Shuttle").transform.position = new Vector3(-1, .045f, 3);
        Set("velocity", Vector3.down);
        LogAssert.Expect(LogType.Log, new Regex(@"\[collision\] ground.*"));
        type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        Set("announcedIn", false); Set("pointWait", 2.9f);
        string saved = (string)Call("CaptureMatch"); Call("RestoreMatch", saved);
        match = Get<object>("Match"); mt = match.GetType();
        Assert.That(Call("ChallengeCall"), Is.EqualTo(true), "Replay-off mode must still allow challenge");
        Assert.That((bool)type.GetField("replaying", Hidden).GetValue(court), Is.False, "ReplayMode=2 must block replay");
        StringAssert.Contains("overturned", (string)type.GetField("feedback", Hidden).GetValue(court));
        Assert.That((int[])mt.GetField("Points").GetValue(match), Is.EqualTo(new[] { 1, 14 }), "Overturned call restores true landing winner");
        Assert.That((int[])mt.GetField("Challenges").GetValue(match), Is.EqualTo(new[] { 2, 2 }), "Successful overturn must not consume quota per P-008");
        type.GetMethod("ContinuePoint", Hidden).Invoke(court, new object[] { 5f });
        Assert.That(Get<string>("MatchState"), Is.EqualTo("Serving"), "Deadline advances exactly once after challenge without replay");
        Assert.That(Get<string>("FlowStage"), Is.Not.EqualTo("GameEnd"));
        Assert.That((int[])mt.GetField("Points").GetValue(match), Is.EqualTo(new[] { 1, 14 }), "No double award after window advance");
        saved = (string)Call("CaptureMatch");
        type.GetMethod("ContinuePoint", Hidden).Invoke(court, new object[] { 10f });
        Assert.That(Call("CaptureMatch"), Is.EqualTo(saved), "No duplicate transition");
    }
    [UnityTest] public IEnumerator FullMatchProducesAudioSamplesThroughMatchEnd()
    {
        yield return Boot(); Call("StartMatch"); Set("assisted", true);
        var audio = type.GetField("soundSource", Hidden);
        Assert.That(audio, Is.Not.Null, "CourtAudio soundSource field must exist");
        var kinds = new System.Collections.Generic.HashSet<string>();
        int previousEvent = Get<int>("SoundEventCount");
        Time.timeScale = 8;
        int contacts = 0, audibleFrames = 0, loudFrames = 0; float began = Time.time, maxRms = 0f;
        var buffer = new float[512];
        while (Get<string>("MatchState") != "MatchEnd" && Time.time - began < 300)
        {
            if (Get<string>("FlowStage") == "Ready" || Get<string>("FlowStage") == "GameEnd" || Get<string>("FlowStage") == "Interval" || Get<string>("FlowStage") == "ChangeEnds") { Call("AdvanceFlow"); yield return new WaitForFixedUpdate(); continue; }
            if (Get<string>("MatchState") == "Point") { Call("NextPoint"); }
            if (Get<string>("MatchState") == "Serving") Call("Serve", true);
            var local = GameObject.Find("Shuttle").transform.position - GameObject.Find("Player").transform.position;
            if (Get<string>("MatchState") == "Rally" && Get<bool>("Flying") && (int)type.GetField("lastHitter", Hidden).GetValue(court) == 1
                && local.y > .4f && local.y < 2.7f && new Vector2(local.x, local.z).magnitude < 1)
            {
                Call("SetAnalog", new Vector2(contacts % 2 == 0 ? -1 : 1, 1));
                Call("BeginShot", local.y >= 1.45f ? (contacts % 3 == 0 ? "Smash" : "Clear") : "Lift");
                LogAssert.Expect(LogType.Log, new Regex(@"\[hit\].*"));
                if ((bool)Call("ReleaseShot")) contacts++;
                Call("SetAnalog", Vector2.zero);
            }
            int events = Get<int>("SoundEventCount");
            if (events != previousEvent)
            {
                var clip = Get<AudioClip>("LastSoundClip");
                Assert.That(clip, Is.Not.Null, "Committed sound event must select a clip");
                if (clip.name.StartsWith("racket-", StringComparison.Ordinal)) kinds.Add("racket");
                else if (clip.name == "JustSmash-original-score") kinds.Add("score");
                previousEvent = events;
            }
            var src = (AudioSource)audio.GetValue(court);
            if (src != null && src.isPlaying)
            {
                src.GetOutputData(buffer, 0);
                float sum = 0; foreach (float v in buffer) sum += v * v;
                float rms = Mathf.Sqrt(sum / buffer.Length);
                if (rms > 1e-5f) audibleFrames++;
                if (rms > 1e-3f) loudFrames++;
                if (rms > maxRms) maxRms = rms;
            }
            yield return new WaitForFixedUpdate();
        }
        Assert.That(Get<string>("MatchState"), Is.EqualTo("MatchEnd"), "Audio audit requires a completed match");
        Time.timeScale = 1;
                System.IO.Directory.CreateDirectory(EvidenceDirectory);
        System.IO.File.WriteAllText(EvidenceDirectory + "/audio-audit.json", string.Format(System.Globalization.CultureInfo.InvariantCulture,
            "{{\"seconds\":{0},\"contacts\":{1},\"audibleFrames\":{2},\"loudFrames\":{3},\"maxRms\":{4},\"kindsFired\":[\"{5}\"]}}",
            Time.time - began, contacts, audibleFrames, loudFrames, maxRms, string.Join("\",\"", kinds)));
        Assert.That(kinds, Does.Contain("racket"), "Racket event must fire during play");
        Assert.That(kinds, Does.Contain("score"), "Score event must fire on points");
        Assert.That(((Component)court).GetComponents<AudioSource>().Any(s => s.clip != null && (s.clip.name.StartsWith("crowd-", StringComparison.Ordinal) || s.clip.name == "arena-room")), Is.False, "Empty venue must stay crowd-free through MatchEnd");
        Assert.That(audibleFrames, Is.GreaterThan(20), "Audio output must be nonzero during real play, not only events logged; not a listening review");
        Assert.That(maxRms, Is.GreaterThan(1e-3f), "Recorded/generated SFX must produce measured output amplitude");
    }
    [UnityTest] public IEnumerator NaturalMatchThreeSeedsReachMatchEndWithWideMotion()
    {
        System.IO.Directory.CreateDirectory(EvidenceDirectory);
        for (int seed = 0; seed < 3; seed++)
        {
            yield return Boot(); Call("StartMatch"); Set("assisted", true);
            Time.timeScale = 15;
            int contacts = 0, points = 0; float began = Time.time, moved = 0f;
            Vector3 previous = Get<Vector3>("PlayerPosition");
            while (Get<string>("MatchState") != "MatchEnd" && Time.time - began < 600)
            {
                if (Get<string>("FlowStage") == "Ready" || Get<string>("FlowStage") == "GameEnd" || Get<string>("FlowStage") == "Interval" || Get<string>("FlowStage") == "ChangeEnds") { Call("AdvanceFlow"); yield return new WaitForFixedUpdate(); continue; }
                if (Get<string>("MatchState") == "Point") { points++; Call("NextPoint"); }
                if (Get<string>("MatchState") == "Serving") Call("Serve", true);
                var local = GameObject.Find("Shuttle").transform.position - GameObject.Find("Player").transform.position;
                if (Get<string>("MatchState") == "Rally" && Get<bool>("Flying") && (int)type.GetField("lastHitter", Hidden).GetValue(court) == 1
                    && local.y > .4f && local.y < 2.7f && new Vector2(local.x, local.z).magnitude < 1)
                {
                    Call("SetAnalog", new Vector2((contacts + seed) % 2 == 0 ? -1 : 1, 1));
                    Call("BeginShot", local.y >= 1.45f ? ((contacts + seed) % 3 == 0 ? "Smash" : "Clear") : "Lift");
                    LogAssert.Expect(LogType.Log, new Regex(@"\[hit\].*"));
                    if ((bool)Call("ReleaseShot")) contacts++;
                    Call("SetAnalog", Vector2.zero);
                }
                float step = Vector3.Distance(Get<Vector3>("PlayerPosition"), previous); previous = Get<Vector3>("PlayerPosition");
                moved += step;
                yield return new WaitForFixedUpdate();
            }
            Time.timeScale = 1;
            var match = Get<object>("Match"); var mt = match.GetType();
            System.IO.File.WriteAllText(EvidenceDirectory + "/seed" + seed + "-match.json", string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "{{\"seconds\":{0},\"playerReturns\":{1},\"points\":{2},\"movedMetres\":{3},\"games\":[{4},{5}],\"winner\":{6},\"state\":\"{7}\",\"seed\":{8}}}",
                Time.time - began, contacts, points, moved,
                ((int[])mt.GetField("Games").GetValue(match))[0], ((int[])mt.GetField("Games").GetValue(match))[1],
                (int)mt.GetField("Winner").GetValue(match), Get<string>("MatchState"), seed));
            Assert.That(Get<string>("MatchState"), Is.EqualTo("MatchEnd"), "Seed " + seed + " must finish the match");
            Assert.That(contacts, Is.GreaterThan(0), "Seed " + seed + " needs natural AI returns");
            Assert.That(points, Is.GreaterThan(10), "Seed " + seed + " must play a real match length");
            Assert.That(moved, Is.GreaterThan(15f), "Seed " + seed + " player must actually cover the court, not idle mid-court");
        }
    }
}