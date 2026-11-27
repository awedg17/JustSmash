using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

// Controlled fixtures, real game camera/physics/HUD/audio at 1x. Not manual match footage.
public class ReleasePolishCaptureTests : PreferenceFixture
{
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    Type type; Component court; Transform player, opponent, shuttle; MobileGameAudio audio;
    System.Diagnostics.Process encoder; StreamWriter timeline, events; string stem, scenario;
    double began, first = -1, last; float next; int frames;
    object Call(string name, params object[] args) => type.GetMethod(name).Invoke(court, args);
    object Get(string name) => type.GetField(name, Hidden).GetValue(court);
    void Set(string name, object value) => type.GetField(name, Hidden).SetValue(court, value);
    object Prop(string name) => type.GetProperty(name).GetValue(court);
    void Mark(string name) => events.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:R},{1},{2},{3},{4}", Time.realtimeSinceStartupAsDouble - began, scenario, name, Get("feedback"), Prop("MatchState")));
    IEnumerator Boot()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720();
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = (Component)UnityEngine.Object.FindFirstObjectByType(type);
        player = GameObject.Find("Player").transform; opponent = GameObject.Find("Opponent").transform; shuttle = GameObject.Find("Shuttle").transform;
        audio = Camera.main.gameObject.AddComponent<MobileGameAudio>(); Time.timeScale = 1; Application.targetFrameRate = 60;
    }
    void Begin(string name)
    {
        stem = EvidenceDirectory + "/" + name;
        encoder = new System.Diagnostics.Process { StartInfo = new System.Diagnostics.ProcessStartInfo("ffmpeg",
            "-hide_banner -loglevel error -n -use_wallclock_as_timestamps 1 -f rawvideo -pixel_format rgba -video_size 1280x720 -framerate 1000 -i pipe:0 -fps_mode passthrough -enc_time_base 1:1000 -vf vflip -an -c:v libx264 -preset ultrafast -crf 20 -pix_fmt yuv420p \"" + stem + "-video.mp4\"")
            { UseShellExecute = false, RedirectStandardInput = true, CreateNoWindow = true } };
        encoder.Start(); began = Time.realtimeSinceStartupAsDouble; frames = 0; first = -1; next = 0;
        timeline = new StreamWriter(stem + "-timeline.csv"); events = new StreamWriter(stem + "-events.csv");
        timeline.WriteLine("wall,game,scenario,state,hitter,shuttleX,shuttleY,shuttleZ"); events.WriteLine("wall,scenario,event,feedback,state");
    }
    IEnumerator Record(float seconds)
    {
        float until = Time.time + seconds;
        do
        {
            yield return new WaitForEndOfFrame();
            if (Time.realtimeSinceStartup < next) continue;
            next = Time.realtimeSinceStartup + 1f / 30;
            double wall = Time.realtimeSinceStartupAsDouble;
            if (first < 0) { first = wall; audio.StartCapture(stem + ".f32"); }
            var texture = ScreenCapture.CaptureScreenshotAsTexture(); var raw = texture.GetRawTextureData<byte>().ToArray(); UnityEngine.Object.Destroy(texture);
            encoder.StandardInput.BaseStream.Write(raw, 0, raw.Length); encoder.StandardInput.BaseStream.Flush();
            last = Time.realtimeSinceStartupAsDouble; frames++;
            timeline.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:R},{1:R},{2},{3},{4},{5},{6},{7}", wall - began, Time.timeAsDouble, scenario, Prop("MatchState"), Get("lastHitter"), shuttle.position.x, shuttle.position.y, shuttle.position.z));
        } while (Time.time < until);
    }
    void Still(string name)
    {
        var image = ScreenCapture.CaptureScreenshotAsTexture(); File.WriteAllBytes(EvidenceDirectory + "/" + name + ".png", image.EncodeToPNG()); UnityEngine.Object.Destroy(image);
    }
    void End()
    {
        audio.StopCapture(); timeline.Dispose(); events.Dispose(); encoder.StandardInput.Close();
        Assert.That(encoder.WaitForExit(15000), Is.True); Assert.That(encoder.ExitCode, Is.Zero); encoder.Dispose();
        File.WriteAllText(stem + "-capture.json", string.Format(CultureInfo.InvariantCulture,
            "{{\"frames\":{0},\"sampleRate\":{1},\"channels\":{2},\"timeScale\":1,\"controlled\":true,\"firstWall\":{3:R},\"lastWall\":{4:R}", frames, AudioSettings.outputSampleRate, audio.Channels, first - began, last - began) + "}");
    }
    void Rally(bool swapped)
    {
        Call("EnterTraining"); Call("ConfigureDiscipline", false); Call("StartMatch");
        var match = Prop("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
        Set("flowStage", null); Call("Serve", true); Set("assisted", false); Set("serviceFlight", false); Set("lastHitter", 1); Set("lastPlayer", 1);
        Set("lastAim", Vector2.zero); Set("velocity", Vector3.zero); Set("swingAt", Time.time - 10); Set("pointWait", -10f);
        type.GetProperty("ReplayMode").SetValue(court, 2);
    }
    void FreezeIncoming()
    {
        // Only park incoming fixture, leave component enabled for live HUD and pose.
        Set("flying", false); Set("velocity", Vector3.zero);
    }
    void LockOpponent()
    {
        Set("observedSwing", Get("swingAt")); Set("opponentThinkAt", float.MaxValue); Set("opponentRefreshAt", float.MaxValue);
    }
    [UnityTest, Timeout(180000), Category("Capture")] public IEnumerator CaptureReleasePolishMedia()
    {
        yield return Boot();
        Begin("contact");
        try
        {
            foreach (bool swapped in new[] { false, true })
            {
                float sign = swapped ? -1 : 1;
                foreach (string shot in new[] { "Lift", "Smash", "Clear", "buffer-Lift" })
                {
                    Rally(swapped); scenario = (swapped ? "swapped-" : "home-") + shot;
                    player.position = new Vector3(-.3f * sign, 0, -3.5f * sign);
                    Vector3 legal = player.TransformPoint(new Vector3(.3f, 1f, .35f));
                    shuttle.position = shot == "Clear" ? player.TransformPoint(new Vector3(1.3f, 2.1f, .35f)) : legal;
                    FreezeIncoming(); yield return Record(.35f);
                    Set("flying", true);
                    if (shot == "buffer-Lift")
                    {
                        shuttle.position = legal + player.right * 2; Call("TapShot", "Lift");
                        shuttle.position = legal; Set("velocity", Vector3.down * .5f); Mark("queued-then-reachable");
                    }
                    else if (shot == "Lift")
                    {
                        Call("TapShot", "Lift"); LockOpponent(); Mark("tap-success");
                    }
                    else
                    {
                        // Real buffered input expires against same parked contact geometry.
                        Call("TapShot", shot); Set("velocity", -Physics.gravity * .02f); Mark("buffered-miss");
                    }
                    if (shot == "Smash" || shot == "Clear")
                    {
                        float until = Time.time + .23f; Vector3 parked = shuttle.position;
                        while (Time.time < until) { shuttle.position = parked; Set("velocity", -Physics.gravity * .02f); yield return Record(.02f); }
                        FreezeIncoming(); Mark("expired");
                    }
                    yield return Record(.65f); Still(scenario); Mark("shown");
                    yield return Record(.35f);
                }
            }
        }
        finally { End(); }
        Begin("smash");
        try
        {
            foreach (bool swapped in new[] { false, true }) foreach (int example in new[] { 0, 1, 2 })
            {
                Rally(swapped); float sign = swapped ? -1 : 1;
                scenario = (swapped ? "swapped-" : "home-") + new[] { "front210", "rear210", "rear240" }[example];
                float height = example == 2 ? 2.4f : 2.1f, z = example == 0 ? -1.5f : -5.5f;
                Vector3 contact = new Vector3(0, height, z * sign);
                player.position = new Vector3(-.3f * sign, 0, (z - .35f) * sign); shuttle.position = contact; FreezeIncoming();
                yield return Record(.55f);
                if (example == 2)
                {
                    // Real Jump entrypoint: hit at first rising sample >= .30 m.
                    Call("Jump"); do { yield return Record(.02f); } while (player.position.y < .30f);
                    shuttle.position = contact;
                }
                Set("flying", true); Call("BeginShot", "Smash"); Set("chargedAt", Time.time - .4f);
                LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\].*Smash.*"));
                Assert.That(Call("ReleaseShot"), Is.True); LockOpponent(); Mark("contact"); Still(scenario + "-contact");
                yield return Record(.16f); Still(scenario + "-overhead");
                yield return Record(1.2f); Mark("flight-ended"); Still(scenario + "-result");
                yield return Record(.45f);
            }
        }
        finally { End(); }
        Begin("visual");
        try
        {
            var home = type.GetNestedType("GameScreen", BindingFlags.NonPublic);
            type.GetMethod("OpenUi", Hidden).Invoke(court, new[] { Enum.Parse(home, "Home") });
            scenario = "Home"; yield return Record(.8f); Still("home-after");
            foreach (bool swapped in new[] { false, true })
            {
                Rally(swapped); scenario = swapped ? "swapped-ready-overhead-recovery" : "home-ready-overhead-recovery";
                float sign = swapped ? -1 : 1; player.position = new Vector3(-.3f * sign, 0, -3.5f * sign);
                shuttle.position = player.TransformPoint(new Vector3(.3f, 2.1f, .35f)); FreezeIncoming();
                yield return Record(.8f); Still(scenario + "-ready");
                Set("flying", true); Call("BeginShot", "Smash"); Set("chargedAt", Time.time - .4f);
                LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\].*Smash.*")); Assert.That(Call("ReleaseShot"), Is.True); LockOpponent(); Mark("contact");
                yield return Record(.12f); Still(scenario + "-overhead"); yield return Record(.65f); Still(scenario + "-recovery"); yield return Record(.45f);
            }
        }
        finally { End(); }
    }
}
