using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class HumanContactWaveTests : PreferenceFixture
{
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    Type type; Component court; Transform player, shuttle; Component visual;
    object Call(string name, params object[] args) => type.GetMethod(name).Invoke(court, args);
    object Get(string name) => type.GetProperty(name).GetValue(court);
    object Field(string name) => type.GetField(name, Hidden).GetValue(court);
    void Set(string name, object value) => type.GetField(name, Hidden).SetValue(court, value);
    IEnumerator Boot()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = (Component)UnityEngine.Object.FindFirstObjectByType(type);
        ((Behaviour)court).enabled = false;
        player = GameObject.Find("Player").transform; shuttle = GameObject.Find("Shuttle").transform;
        visual = player.Find("AthleteVisual").GetComponents<MonoBehaviour>().Single(t => t.GetType().Name == "AthleteVisual");
    }
    void Prepare(bool doubles, bool swapped)
    {
        Time.timeScale = 1; Set("replaying", false);
        Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("StartMatch");
        var match = Get("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped);
        Call("NextPoint");
        for (int i = 0; i < 3 && (string)Get("FlowStage") != "Serving"; i++) Call("AdvanceFlow");
        Assert.That(Get("MatchState"), Is.EqualTo("Serving"));
        visual.GetType().GetMethod("Present").Invoke(visual, new object[] { "Clear", 10f, Vector3.zero, Vector3.zero, 0f, 0f });
    }
    void Serve(bool release, bool flick)
    {
        if (!release) { Call("Serve", flick); return; }
        Call("BeginServe"); Set("serveChargedAt", Time.time - (flick ? 1f : .2f)); Call("ReleaseServe");
    }
    [UnityTest] public IEnumerator LegalServePresentsStringBedBeforeReturningAcrossEndsFormatsAndInputs()
    {
        yield return Boot(); int cases = 0;
        var contactErrors = new List<string>();
        string evidence = BeginContactEvidence();
        foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
        foreach (bool release in new[] { false, true }) foreach (bool flick in new[] { false, true })
        {
            Prepare(doubles, swapped);
            var bed = (Transform)visual.GetType().GetProperty("StringBed").GetValue(visual);
            // Match AthleteVisual.Bind paths, not duplicate mesh object names.
            var hips = visual.transform.Find("Root/Hips");
            Assert.That(hips, Is.Not.Null);
            var bones = new[] {
                "Spine/Spine1/RightShoulder/RightArm/RightForeArm",
                "Spine/Spine1/RightShoulder/RightArm/RightForeArm/RightHand",
                "Spine/Spine1/LeftShoulder/LeftArm/LeftForeArm",
                "Spine/Spine1/LeftShoulder/LeftArm/LeftForeArm/LeftHand",
                "RightUpLeg/RightLeg", "LeftUpLeg/LeftLeg",
                "RightUpLeg/RightLeg/RightFoot", "LeftUpLeg/LeftLeg/LeftFoot"
            }.Select(path => hips.Find(path)).ToArray();
            Assert.That(bones.All(t => t != null), Is.True, "Skeleton path missing");
            Assert.That(bones.Distinct().Count(), Is.EqualTo(8));
            Assert.That(bones.Length, Is.EqualTo(8));
            var lengths = bones.Select(t => Vector3.Distance(t.position, t.parent.position)).ToArray();
            Vector3 root = player.position, scale = player.localScale; Quaternion rotation = player.rotation;
            float energy = (float)Get("Energy"), instant = Time.time;
            int sounds = (int)Get("SoundEventCount");
            Serve(release, flick); // No yield, Update, AnimateAthlete or Present after entrypoint.
            string label = "doubles=" + doubles + " swapped=" + swapped + " release=" + release + " flick=" + flick;
            Assert.That(Get("Flying"), Is.True, label); Assert.That(Get("MatchState"), Is.EqualTo("Rally"), label);
            Assert.That(Time.time, Is.EqualTo(instant), label);
            Assert.That(Field("contact"), Is.EqualTo(shuttle.position), label);
            float error = Vector3.Distance(bed.position, shuttle.position);
            // Measure before rendering; never yield or update the pose after Serve.
            string caseName = "doubles=" + doubles + "_end=" + swapped + "_release=" + release + "_flick=" + flick;
            string row = caseName + "," + Time.frameCount + "," + Time.time.ToString("R", CultureInfo.InvariantCulture)
                + "," + error.ToString("R", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.ToString("O");
            TestContext.WriteLine(row);
            if (evidence != null) File.AppendAllText(Path.Combine(evidence, "contacts.csv"), row + Environment.NewLine);
            if (!(error < .05f)) contactErrors.Add("Serve returned before visible racket reached contact: " + label + " error=" + error.ToString("R", CultureInfo.InvariantCulture));
            // Baseline may also retain Ready; defer this presentation gate, not weaken it.
            string clip = (string)visual.GetType().GetProperty("CurrentClip").GetValue(visual);
            if (clip != "Serve") contactErrors.Add(label + " expected clip Serve, actual=" + clip);
            if (evidence != null) CaptureContact(Path.Combine(evidence, caseName + ".png"));
            Assert.That(player.position, Is.EqualTo(root)); Assert.That(player.rotation, Is.EqualTo(rotation)); Assert.That(player.localScale, Is.EqualTo(scale));
            Assert.That(Get("Energy"), Is.EqualTo(energy)); Assert.That(Get("SoundEventCount"), Is.EqualTo(sounds + 1));
            for (int i = 0; i < bones.Length; i++) Assert.That(Vector3.Distance(bones[i].position, bones[i].parent.position), Is.EqualTo(lengths[i]).Within(.001f), bones[i].name);
            cases++;
        }
        Assert.That(cases, Is.EqualTo(16));
        Assert.That(contactErrors, Is.Empty, string.Join(Environment.NewLine, contactErrors));
    }
    [UnityTest] public IEnumerator LegalInputInterruptsRecoveryAtImmediateContact()
    {
        yield return Boot(); int cases = 0;
        var rows = new List<string> { "doubles,swapped,stroke,input,recoveryAge,stringBedErrorMetres" };
        var errors = new List<string>();
        var present = visual.GetType().GetMethod("Present");
        var bed = (Transform)visual.GetType().GetProperty("StringBed").GetValue(visual);
        object HiddenCall(string name, params object[] args) => type.GetMethod(name, Hidden).Invoke(court, args);
        try
        {
            foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
            foreach (string shot in new[] { "Smash", "Clear", "Drop", "Net", "Lift" })
            foreach (string input in new[] { "tap", "release", "buffer", "swipe" })
            foreach (float age in new[] { .31f, .64f })
            {
                Prepare(doubles, swapped); Call("Serve", true);
                Set("lastHitter", 1); Set("lastPlayer", 1); Set("serviceFlight", false);
                Set("lastStroke", "Clear"); Set("swingAt", Time.time - age);
                Vector3 prior = player.TransformPoint(new Vector3(-.3f, 2.1f, .35f));
                Set("contact", prior); present.Invoke(visual, new object[] { "Clear", age, prior, Vector3.zero, 0f, 0f });
                var target = player.TransformPoint(new Vector3(.3f, shot == "Net" || shot == "Lift" ? 1f : 2.1f, .35f));
                shuttle.position = target; Set("velocity", Vector3.down);
                string label = doubles + "," + swapped + "," + shot + "," + input + "," + age.ToString("R", CultureInfo.InvariantCulture);
                Assert.That((bool)visual.GetType().GetMethod("CanReach").Invoke(visual, new object[] { target, shot }), Is.True, label);
                if (input == "buffer")
                {
                    shuttle.position = target + player.right * 3;
                    int sounds = (int)Get("SoundEventCount"); float energy = (float)Get("Energy");
                    Call("TapShot", shot);
                    Assert.That(Get("SoundEventCount"), Is.EqualTo(sounds), label + " premature buffer audio");
                    Assert.That(Get("Energy"), Is.EqualTo(energy), label + " premature buffer cost");
                    shuttle.position = target;
                }
                if (input == "swipe")
                {
                    var jump = (Rect)type.GetProperty("JumpRect", Hidden).GetValue(court);
                    var rect = (Rect)HiddenCall("ShotRect", Array.IndexOf(new[] { "Smash", "Clear", "Drop", "Net", "Lift" }, shot));
                    HiddenCall("PointerDown", 96, jump.center); HiddenCall("PointerMove", 96, rect.center);
                    Assert.That(Field("charging"), Is.EqualTo(true), label + " swipe did not select stroke");
                }
                Vector3 root = player.position; float time = Time.time, beforeEnergy = (float)Get("Energy");
                int beforeSounds = (int)Get("SoundEventCount");
                LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\] (Excellent|Good|Fair|Poor) \| " + shot + ".*"));
                if (input == "tap") Call("TapShot", shot);
                else if (input == "release") { Call("BeginShot", shot); Assert.That(Call("ReleaseShot"), Is.EqualTo(true), label); }
                else if (input == "buffer") HiddenCall("TryBufferedShot");
                else HiddenCall("PointerUp", 96);
                float gap = Vector3.Distance(bed.position, target);
                rows.Add(label + "," + gap.ToString("R", CultureInfo.InvariantCulture));
                if (!(gap < .035f)) errors.Add(label + " stale pose gap=" + gap.ToString("R", CultureInfo.InvariantCulture));
                Assert.That(Field("lastHitter"), Is.EqualTo(0), label);
                Assert.That(Field("lastStroke"), Is.EqualTo(shot), label);
                Assert.That(Field("swingAt"), Is.EqualTo(time), label + " delayed contact");
                Assert.That(Time.time, Is.EqualTo(time)); Assert.That(player.position, Is.EqualTo(root));
                Assert.That(Field("contact"), Is.EqualTo(target));
                Assert.That(Get("SoundEventCount"), Is.EqualTo(beforeSounds + 1), label);
                Assert.That((float)Get("Energy"), Is.EqualTo(beforeEnergy - (shot == "Smash" ? 3f : 1f)).Within(.001f), label);
                var velocity = Get("ShuttleVelocity"); Call("TapShot", shot); Call("ReleaseShot"); HiddenCall("TryBufferedShot");
                Assert.That(Get("SoundEventCount"), Is.EqualTo(beforeSounds + 1), label + " repeated sound");
                Assert.That(Get("ShuttleVelocity"), Is.EqualTo(velocity), label + " repeated hit");
                cases++;
            }
            Assert.That(cases, Is.EqualTo(160)); Assert.That(errors, Is.Empty, string.Join("; ", errors));
        }
        finally { File.WriteAllLines(EvidenceDirectory + "/recovery-input-contacts.csv", rows); }
    }
    static string BeginContactEvidence()
    {
        string run = Environment.GetEnvironmentVariable("JUSTSMASH_QA_RUN");
        TestContext.WriteLine("case,frame,time,immediateStringBedErrorMetres,utc");
        if (string.IsNullOrEmpty(run)) return null;
        string folder = Path.Combine(run, "HumanServeContacts");
        Directory.CreateDirectory(folder);
        // Parent uses separate QA run roots for baseline/candidate; bind each to actual sources.
        var sources = Directory.GetFiles(Path.Combine(Application.dataPath, "Scripts"), "*.cs")
            .Concat(new[] { Path.Combine(Application.dataPath, "Tests/PlayMode/HumanContactWaveTests.cs"),
                Path.Combine(Application.dataPath, "Resources/AthleteNPC.prefab"), Path.Combine(Application.dataPath, "Scenes/Boot.unity") }).OrderBy(path => path);
        using (var hash = SHA256.Create())
            File.WriteAllLines(Path.Combine(folder, "sources.sha256"), sources.Select(path =>
                BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant() + "  " + path));
        File.WriteAllText(Path.Combine(folder, "contacts.csv"), "case,frame,time,immediateStringBedErrorMetres,utc" + Environment.NewLine);
        return folder;
    }
    static void CaptureContact(string path)
    {
        var camera = Camera.main;
        Assert.That(camera, Is.Not.Null, "Contact capture needs main camera");
        var previousTarget = camera.targetTexture;
        var previousActive = RenderTexture.active;
        var target = RenderTexture.GetTemporary(1280, 720, 24, RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.Default, QualitySettings.antiAliasing > 0 ? QualitySettings.antiAliasing : 2);
        Texture2D image = null;
        try
        {
            camera.targetTexture = target;
            camera.Render(); // Synchronous, after exact-frame distance measurement.
            RenderTexture.active = target;
            image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); image.Apply();
            File.WriteAllBytes(path, image.EncodeToPNG());
            TestContext.WriteLine(path + " cameraPosition=" + camera.transform.position.ToString("R")
                + " cameraRotation=" + camera.transform.rotation.ToString("R")
                + " fov=" + camera.fieldOfView.ToString("R", CultureInfo.InvariantCulture)
                + " aspect=" + camera.aspect.ToString("R", CultureInfo.InvariantCulture)
                + " msaa=" + target.antiAliasing);
        }
        finally
        {
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            RenderTexture.ReleaseTemporary(target);
            if (image != null) UnityEngine.Object.Destroy(image);
        }
    }
    [UnityTest] public IEnumerator RejectedServePreservesPoseEnergyFlightAndSilence()
    {
        yield return Boot(); int cases = 0;
        try
        {
            foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
            foreach (bool release in new[] { false, true })
            foreach (string state in new[] { "pause", "replay", "repeat", "Warmup", "GameEnd", "Interval", "ChangeEnds", "opponent" })
            {
                Prepare(doubles, swapped);
                if (state == "repeat") Serve(release, true);
                if (release) { Call("BeginServe"); Set("serveChargedAt", Time.time - 1f); }
                if (state == "pause") Time.timeScale = 0;
                else if (state == "replay") Set("replaying", true);
                else if (state == "opponent")
                {
                    var match = Get("Match"); match.GetType().GetField("Server").SetValue(match, 1);
                }
                else if (state != "repeat") Set("flowStage", state);
                AssertSilentUnchanged(() => { if (release) Call("ReleaseServe"); else Call("Serve", true); }, state);
                Time.timeScale = 1; Set("replaying", false); cases++;
            }
            Assert.That(cases, Is.EqualTo(64));
            Prepare(false, false);
            AssertSilentUnchanged(() => Call("ReleaseServe"), "uncharged");
            Call("BeginServe"); Call("CancelServe");
            AssertSilentUnchanged(() => Call("ReleaseServe"), "cancelled");
            foreach (float invalid in new[] { float.NaN, float.PositiveInfinity, float.NegativeInfinity, -.01f, 1.01f })
                AssertSilentUnchanged(() => {
                    var error = Assert.Throws<TargetInvocationException>(() => Call("ServeWithPower", invalid));
                    Assert.That(error.InnerException, Is.TypeOf<ArgumentOutOfRangeException>());
                }, "invalid power");
        }
        finally { Time.timeScale = 1; Set("replaying", false); }
    }
    void AssertSilentUnchanged(Action action, string label)
    {
        var audio = (AudioSource)Field("soundSource"); if (audio != null) audio.Stop();
        var transforms = player.GetComponentsInChildren<Transform>(true);
        var positions = transforms.Select(t => t.localPosition).ToArray();
        var rotations = transforms.Select(t => t.localRotation).ToArray();
        var scales = transforms.Select(t => t.localScale).ToArray();
        object energy = Get("Energy"), sounds = Get("SoundEventCount"), flying = Get("Flying"), velocity = Get("ShuttleVelocity");
        object contact = Field("contact"), swing = Field("swingAt"), stroke = Field("lastStroke");
        Vector3 shuttlePosition = shuttle.position;
        action();
        Assert.That(Get("Energy"), Is.EqualTo(energy), label); Assert.That(Get("SoundEventCount"), Is.EqualTo(sounds), label);
        Assert.That(Get("Flying"), Is.EqualTo(flying), label); Assert.That(Get("ShuttleVelocity"), Is.EqualTo(velocity), label);
        Assert.That(shuttle.position, Is.EqualTo(shuttlePosition), label);
        Assert.That(Field("contact"), Is.EqualTo(contact), label); Assert.That(Field("swingAt"), Is.EqualTo(swing), label); Assert.That(Field("lastStroke"), Is.EqualTo(stroke), label);
        if (audio != null) Assert.That(audio.isPlaying, Is.False, label);
        for (int i = 0; i < transforms.Length; i++)
        {
            Assert.That(transforms[i].localPosition, Is.EqualTo(positions[i]), label + " " + transforms[i].name);
            Assert.That(transforms[i].localRotation, Is.EqualTo(rotations[i]), label + " " + transforms[i].name);
            Assert.That(transforms[i].localScale, Is.EqualTo(scales[i]), label + " " + transforms[i].name);
        }
    }
}
