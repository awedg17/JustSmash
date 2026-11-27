using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MobileMatchTests : PreferenceFixture
{
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    string Evidence => EvidenceDirectory;
    string CaptureEvidence => EvidenceDirectory;
    StreamWriter timeline; float captureStarted, captureGameStarted;
    Type type; UnityEngine.Object court;
    object Call(string name, params object[] args) => type.GetMethod(name).Invoke(court, args);
    T Get<T>(string name) => (T)type.GetProperty(name).GetValue(court);
    void Click(string action)
    {
        Rect rect = (Rect)Call("UiActionRect", action); Assert.That(rect.width, Is.GreaterThan(0), action);
        type.GetMethod("PointerDown", Hidden).Invoke(court, new object[] { 87, rect.center });
        type.GetMethod("PointerUp", Hidden).Invoke(court, new object[] { 87 });
    }
    [UnityTest, Category("Capture")] public IEnumerator NativeMouseAndKeyboardUseGameView()
    {
        Assert.That(Application.isBatchMode, Is.False);
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type);
        float deadline = Time.realtimeSinceStartup + 180;
        while (Get<string>("ScreenName") != "Settings" && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(Get<string>("ScreenName"), Is.EqualTo("Settings"), "Native mouse must open Settings");
        while (Get<float>("Volume") > .1f && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(Get<float>("Volume"), Is.LessThanOrEqualTo(.1f), "Native mouse must change slider");
        while (Get<string>("ScreenName") != "Training" && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(Get<string>("ScreenName"), Is.EqualTo("Training"), "Native mouse must enter Training");
        Vector3 before = Get<Vector3>("PlayerPosition");
        while (Vector3.Distance(before, Get<Vector3>("PlayerPosition")) < .5f && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(Vector3.Distance(before, Get<Vector3>("PlayerPosition")), Is.GreaterThanOrEqualTo(.5f), "Native WASD must move athlete");
        while (GameObject.Find("Player").transform.position.y < .1f && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(GameObject.Find("Player").transform.position.y, Is.GreaterThanOrEqualTo(.1f), "Native Space must jump");
        yield return new WaitForEndOfFrame();
        var image = ScreenCapture.CaptureScreenshotAsTexture(); File.WriteAllBytes(Evidence + "/native-input.png", image.EncodeToPNG()); UnityEngine.Object.Destroy(image);
    }
    [Test] public void AudioCaptureClockPreservesSamplesAndStopIsIdempotent()
    {
        string path = Evidence + "/audio-clock-check.f32";
        var host = new GameObject("AudioClockCheck");
        var audio = host.AddComponent<MobileGameAudio>();
        try
        {
            float[] data = { .25f, -.5f, 0f, 1f }; byte[] expected = new byte[data.Length * sizeof(float)];
            Buffer.BlockCopy(data, 0, expected, 0, expected.Length);
            audio.StartCapture(path);
            typeof(MobileGameAudio).GetMethod("OnAudioFilterRead", Hidden).Invoke(audio, new object[] { data, 2 });
            audio.StopCapture();
            Assert.That(File.ReadAllBytes(path), Is.EqualTo(expected), "Diagnostic must not alter PCM");
            Assert.That(File.Exists(path + ".clock.json"), Is.True, "Missing capture clock diagnostics");
            string clock = File.ReadAllText(path + ".clock.json");
            Assert.That(clock, Does.Contain("\"sampleFloats\":4"));
            Assert.That(clock, Does.Contain("\"callbacks\":1"));
            Assert.That(clock, Does.Contain("\"channels\":2"));
            audio.StopCapture(); Assert.That(File.ReadAllText(path + ".clock.json"), Is.EqualTo(clock));
        }
        finally { audio.StopCapture(); UnityEngine.Object.DestroyImmediate(host); }
    }
    [UnityTest, Category("Capture")] public IEnumerator FullUiAndGameAudioCaptureSmoke()
    {
        Assert.That(Application.isBatchMode, Is.False);
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type);
        Directory.CreateDirectory(CaptureEvidence);
        var audio = Camera.main.gameObject.AddComponent<MobileGameAudio>(); audio.StartCapture(CaptureEvidence + "/audio-smoke.f32");
        foreach (string action in new[] { "Home", "Pengaturan", "Kembali", "Kredit", "Kembali", "Main", "Mulai", "Pause", "Lanjut" })
        {
            if (action != "Home") Click(action);
            yield return new WaitForEndOfFrame();
            var image = ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(CaptureEvidence + "/ui-" + Get<string>("ScreenName") + ".png", image.EncodeToPNG());
            Debug.Log("[capture] " + image.width + "x" + image.height + " format=" + image.format);
            UnityEngine.Object.Destroy(image);
        }
        Click("Teruskan"); Click("Teruskan"); Call("Serve", true);
        yield return new WaitForSecondsRealtime(2); audio.StopCapture();
        type.GetField("flying", Hidden).SetValue(court, false);
        var match = Get<object>("Match");
        while ((int)match.GetType().GetField("Winner").GetValue(match) < 0)
            match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
        yield return new WaitForEndOfFrame();
        var result = ScreenCapture.CaptureScreenshotAsTexture();
        File.WriteAllBytes(CaptureEvidence + "/ui-Result-controlled.png", result.EncodeToPNG()); UnityEngine.Object.Destroy(result);
        Click("Menu"); Assert.That(Get<string>("ScreenName"), Is.EqualTo("Home"));
        Assert.That(audio.Channels, Is.GreaterThan(0));
        Assert.That(new FileInfo(CaptureEvidence + "/audio-smoke.f32").Length, Is.GreaterThan(100000));
    }
    [UnityTest, Category("Capture")] public IEnumerator DoublesUiFlowCaptureUsesActualRuntimeScreens()
    {
        Assert.That(Application.isBatchMode, Is.False);
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type);
        string directory = CaptureEvidence + "/doubles-ui-after-reaction";
        Directory.CreateDirectory(directory);
        IEnumerator Capture(string name)
        {
            yield return null; yield return new WaitForEndOfFrame();
            var image = ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(directory + "/" + name + ".png", image.EncodeToPNG());
            UnityEngine.Object.Destroy(image);
        }
        yield return Capture("01-home"); Click("Main"); Click("Ganda"); Click("Hard");
        yield return Capture("02-setup"); Click("Mulai");
        yield return Capture("03-warmup"); Click("Teruskan");
        yield return Capture("04-ready"); Click("Teruskan");
        yield return Capture("05-serving"); Call("Serve", true);
        yield return new WaitForSeconds(.6f); yield return Capture("06-rally");
        Click("Pause"); yield return Capture("07-pause"); Click("Pengaturan");
        yield return Capture("08-settings"); Click("Kembali"); Click("Lanjut");
        type.GetField("flying", Hidden).SetValue(court, false);
        var match = Get<object>("Match");
        while ((int)match.GetType().GetField("Winner").GetValue(match) < 0)
            match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
        yield return Capture("09-result-controlled");
        Assert.That(Get<bool>("DoublesMode"), Is.True);
        Click("Menu"); Assert.That(Get<string>("ScreenName"), Is.EqualTo("Home"));
        yield return Capture("10-home-return");
    }
    [UnityTest, Category("Capture")] public IEnumerator CaptureIdleBreathingAt1x()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720();
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type); Call("EnterTraining");
        // Controlled native pose loop: disable gameplay camera/HUD, keep actual AnimateAthlete path.
        ((Behaviour)court).enabled = false;
        var player = GameObject.Find("Player").transform;
        player.SetPositionAndRotation(new Vector3(0, 0, -4), Quaternion.identity);
        type.GetField("previousPlayer", Hidden).SetValue(court, player.position);
        type.GetField("lastStroke", Hidden).SetValue(court, "Clear");
        type.GetField("swingAt", Hidden).SetValue(court, Time.time - .65f);
        var camera = Camera.main; camera.transform.position = new Vector3(2.5f, 1.5f, -2.2f); camera.transform.LookAt(player.position + Vector3.up * 1.1f); camera.fieldOfView = 48;
        File.WriteAllText(CaptureEvidence + "/camera.json", JsonUtility.ToJson(new CameraFixture { position = camera.transform.position, rotation = camera.transform.rotation, fov = camera.fieldOfView }));
        string stem = CaptureEvidence + "/idle-breath";
        var recorder = new System.Diagnostics.Process();
        recorder.StartInfo = new System.Diagnostics.ProcessStartInfo("ffmpeg", "-hide_banner -loglevel error -n -use_wallclock_as_timestamps 1 -f rawvideo -pixel_format rgba -video_size 1280x720 -framerate 1000 -i pipe:0 -fps_mode passthrough -enc_time_base 1:1000 -vf vflip -an -c:v libx264 -preset ultrafast -crf 18 -pix_fmt yuv420p \"" + stem + "-video.mp4\"") { UseShellExecute = false, RedirectStandardInput = true, CreateNoWindow = true };
        recorder.Start(); captureStarted = Time.realtimeSinceStartup; captureGameStarted = Time.time; recordedFrames = 0;
        timeline = new StreamWriter(stem + "-timeline.csv");
        timeline.WriteLine("realSeconds,gameSeconds,state,hitter,stroke,strokeElapsed,playerX,playerY,playerZ,shuttleX,shuttleY,shuttleZ,screen,flow");
        try
        {
            while (Time.time - captureGameStarted < 4.25f)
            {
                type.GetMethod("AnimateAthlete", Hidden).Invoke(court, null);
                yield return new WaitForEndOfFrame(); WriteFrame(recorder); recordedFrames++; yield return null;
            }
        }
        finally
        {
            timeline.Dispose(); timeline = null; recorder.StandardInput.Close();
            Assert.That(recorder.WaitForExit(15000), Is.True); Assert.That(recorder.ExitCode, Is.Zero); recorder.Dispose();
        }
        Assert.That(recordedFrames, Is.GreaterThan(80)); Assert.That(player.position, Is.EqualTo(new Vector3(0, 0, -4)));
    }
    [Serializable] class CameraFixture { public Vector3 position; public Quaternion rotation; public float fov; public bool controlled = true; public bool hud = false; }
    [UnityTest, Timeout(120000), Category("Capture")] public IEnumerator CaptureWaveOneNaturalRalliesAt1x()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720();
        int priorFps = Application.targetFrameRate;
        Time.timeScale = 1; Application.targetFrameRate = 60;
        try
        {
            foreach (bool doubles in new[] { false, true })
            {
                LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
                yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
                type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
                court = UnityEngine.Object.FindFirstObjectByType(type);
                string stem = CaptureEvidence + (doubles ? "/doubles" : "/singles");
                // Controlled stills only: same camera and geometry positions on both source revisions.
                if (!doubles)
                {
                    Call("EnterTraining"); ((Behaviour)court).enabled = false;
                    var specimen = GameObject.Find("Shuttle").transform;
                    Vector3[] poses = { new Vector3(0, 1.7f, 0), new Vector3(0, 1.4f, -3.2f), new Vector3(0, 1.4f, 3.6f), new Vector3(0, 5, 2) };
                    for (int i = 0; i < poses.Length; i++)
                    {
                        specimen.position = poses[i]; specimen.rotation = Quaternion.identity;
                        specimen.GetComponent<TrailRenderer>().Clear();
                        yield return new WaitForEndOfFrame();
                        var image = ScreenCapture.CaptureScreenshotAsTexture();
                        File.WriteAllBytes(CaptureEvidence + "/matched-pose-" + i + ".png", image.EncodeToPNG()); UnityEngine.Object.Destroy(image);
                    }
                    ((Behaviour)court).enabled = true;
                    Click("Pause"); Click("Menu");
                }
                type.GetProperty("Volume").SetValue(court, .7f);
                var audio = Camera.main.gameObject.AddComponent<MobileGameAudio>();
                var recorder = new System.Diagnostics.Process();
                recorder.StartInfo = new System.Diagnostics.ProcessStartInfo("ffmpeg", "-hide_banner -loglevel error -n -use_wallclock_as_timestamps 1 -f rawvideo -pixel_format rgba -video_size " + Screen.width + "x" + Screen.height + " -framerate 1000 -i pipe:0 -fps_mode passthrough -enc_time_base 1:1000 -vf vflip,pad=ceil(iw/2)*2:ceil(ih/2)*2 -an -c:v libx264 -preset ultrafast -crf 20 -pix_fmt yuv420p -movflags +faststart \"" + stem + "-video.mp4\"") { UseShellExecute = false, RedirectStandardInput = true, CreateNoWindow = true };
                recorder.Start(); captureStarted = Time.realtimeSinceStartup; captureGameStarted = Time.time;
                recordedFrames = 0; firstRecordedFrame = 0;
                timeline = new StreamWriter(stem + "-timeline.csv");
                timeline.WriteLine("realSeconds,gameSeconds,state,hitter,stroke,strokeElapsed,playerX,playerY,playerZ,shuttleX,shuttleY,shuttleZ,screen,flow");
                audio.StartCapture(stem + ".f32");
                var recording = ((MonoBehaviour)court).StartCoroutine(RecordMatchFrames(recorder));
                int returns = 0; float gameBegan = Time.time;
                try
                {
                    yield return new WaitForSecondsRealtime(.5f);
                    Click("Main"); Click(doubles ? "Ganda" : "Tunggal"); Click("Normal");
                    yield return new WaitForSecondsRealtime(.5f);
                    Click("Mulai"); Call("ConfigureOpponent", "Normal", 71); type.GetProperty("ReplayMode").SetValue(court, 2);
                    var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
                    var driver = player.Find("AthleteVisual").GetComponent(AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.AthleteVisual")).First(t => t != null));
                    var camera = Camera.main;
                    File.WriteAllText(stem + "-camera.json", "{\"position\":" + JsonUtility.ToJson(camera.transform.position) + ",\"rotation\":" + JsonUtility.ToJson(camera.transform.rotation) + ",\"fov\":" + camera.fieldOfView + ",\"width\":1280,\"height\":720,\"seed\":71}");
                    gameBegan = Time.time;
                    while (Time.time - gameBegan < 24 && Get<string>("MatchState") != "MatchEnd")
                    {
                        string flow = Get<string>("FlowStage");
                        if (new[] { "Warmup", "Ready", "GameEnd", "Interval", "ChangeEnds" }.Contains(flow)) Click("Teruskan");
                        string state = Get<string>("MatchState");
                        int hitter = (int)type.GetField(doubles ? "lastPlayer" : "lastHitter", Hidden).GetValue(court);
                        if (state == "Rally")
                        {
                            bool incoming = hitter % 2 == 1;
                            bool owns = !doubles || (int)type.GetField("nearOwner", Hidden).GetValue(court) == 0;
                            float end = player.forward.z > 0 ? 1 : -1;
                            Vector3 desired = incoming && owns ? shuttle.position - player.right * .3f - player.forward * .35f : doubles ? Get<Vector3>("SuggestedPosition") : new Vector3(0, 0, -3.8f * end);
                            desired.z = Mathf.Min(desired.z * end, -1) * end;
                            Call("SetAnalog", Vector2.ClampMagnitude(new Vector2(desired.x - player.position.x, desired.z - player.position.z) * 4 * end, 1));
                            string shot = shuttle.position.y - player.position.y >= 1.45f ? (returns % 4 == 3 ? "Smash" : "Clear") : "Lift";
                            if (incoming && owns && (bool)driver.GetType().GetMethod("CanReach").Invoke(driver, new object[] { shuttle.position, shot }))
                            {
                                Call("SetAnalog", new Vector2(returns % 2 == 0 ? -1 : 1, 1)); Call("TapShot", shot);
                                if ((int)type.GetField(doubles ? "lastPlayer" : "lastHitter", Hidden).GetValue(court) == 0) returns++;
                            }
                        }
                        else { Call("SetAnalog", Vector2.zero); if (state == "Serving" && flow == "Serving") Call("Serve", true); }
                        yield return new WaitForFixedUpdate();
                    }
                    Call("SetAnalog", Vector2.zero);
                    if (Get<string>("MatchState") != "MatchEnd") Click("Pause");
                    Click("Menu"); if (Get<string>("ScreenName") == "Confirm") Click("Ya");
                    Assert.That(Get<string>("ScreenName"), Is.EqualTo("Home"));
                    yield return new WaitForSecondsRealtime(.5f);
                }
                finally
                {
                    ((MonoBehaviour)court).StopCoroutine(recording); audio.StopCapture(); timeline.Dispose(); timeline = null;
                    recorder.StandardInput.Close(); Assert.That(recorder.WaitForExit(15000), Is.True); Assert.That(recorder.ExitCode, Is.Zero); recorder.Dispose();
                    File.WriteAllText(stem + "-capture.json", "{\"frames\":" + recordedFrames + ",\"sampleRate\":" + AudioSettings.outputSampleRate + ",\"channels\":" + audio.Channels + ",\"timeScale\":1,\"controlled\":false,\"excerpt\":true,\"playerReturns\":" + returns + "}");
                }
                Assert.That(recordedFrames, Is.GreaterThan(100)); Assert.That(returns, Is.GreaterThan(0), "Natural excerpt must exercise human returns");
            }
        }
        finally { Application.targetFrameRate = priorFps; Time.timeScale = 1; }
    }
    [UnityTest, Timeout(1500000), Category("Capture")] public IEnumerator CaptureOneTimespeedMatchesWithGameAudio() => CaptureSinglesMatches(new[] { "Easy", "Normal", "Hard" });
    [UnityTest, Timeout(600000), Category("Capture")] public IEnumerator CaptureNormalSinglesWithGameAudio() => CaptureSinglesMatches(new[] { "Normal" });
    IEnumerator CaptureSinglesMatches(string[] difficulties)
    {
        Assert.That(Application.isBatchMode, Is.False, "Capture gate requires visible Game View; exclude Capture category from batch regression");
        yield return CaptureAt1280x720();
        Time.timeScale = 1;
        foreach (string difficulty in difficulties)
        {
            LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
            yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
            type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
            court = UnityEngine.Object.FindFirstObjectByType(type);
            type.GetField("assisted", Hidden).SetValue(court, false);
            type.GetProperty("Volume").SetValue(court, .7f);
            var audio = Camera.main.gameObject.AddComponent<MobileGameAudio>();
            Directory.CreateDirectory(CaptureEvidence);
            string stem = CaptureEvidence + "/match-1x-" + difficulty;
            captureStarted = Time.realtimeSinceStartup; captureGameStarted = Time.time;
            timeline = new StreamWriter(stem + "-timeline.csv");
            timeline.WriteLine("realSeconds,gameSeconds,state,hitter,stroke,strokeElapsed,playerX,playerY,playerZ,shuttleX,shuttleY,shuttleZ,screen,flow");
            var recorder = new System.Diagnostics.Process();
            recorder.StartInfo = new System.Diagnostics.ProcessStartInfo("ffmpeg", "-hide_banner -loglevel error -n -use_wallclock_as_timestamps 1 -f rawvideo -pixel_format rgba -video_size " + Screen.width + "x" + Screen.height + " -framerate 1000 -i pipe:0 -fps_mode passthrough -enc_time_base 1:1000 -vf vflip,pad=ceil(iw/2)*2:ceil(ih/2)*2 -an -c:v libx264 -preset ultrafast -crf 23 -pix_fmt yuv420p -movflags +faststart \"" + stem + "-video.mp4\"") { UseShellExecute = false, RedirectStandardInput = true, CreateNoWindow = true };
            recorder.Start();
            var result = new Result { difficulty = difficulty, seed = 71 };
            Application.targetFrameRate = 60; recordedFrames = 0; firstRecordedFrame = 0;
            Coroutine recording = null;
            float began = Time.unscaledTime;
            try
            {
                yield return new WaitForEndOfFrame();
                audio.StartCapture(stem + ".f32");
                firstRecordedFrame = Time.realtimeSinceStartup - captureStarted;
                WriteFrame(recorder); recordedFrames++;
                recording = ((MonoBehaviour)court).StartCoroutine(RecordMatchFrames(recorder));
                yield return new WaitForSecondsRealtime(1);
                Click("Main"); Click("Tunggal"); Click(difficulty); yield return new WaitForSecondsRealtime(1);
                Click("Mulai"); Call("ConfigureOpponent", difficulty, 71); type.GetProperty("ReplayMode").SetValue(court, 2);
                var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
                var driver = player.Find("AthleteVisual").GetComponent(AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.AthleteVisual")).First(t => t != null));
                var reachable = driver.GetType().GetMethod("CanReach");
                Time.timeScale = 1;
                while (Get<string>("MatchState") != "MatchEnd" && Time.unscaledTime - began < 500)
                {
                    string flow = Get<string>("FlowStage");
                    if (new[] { "Warmup", "Ready", "GameEnd", "Interval", "ChangeEnds" }.Contains(flow)) { yield return new WaitForSecondsRealtime(1); Click("Teruskan"); }
                    string state = Get<string>("MatchState"); int hitter = (int)type.GetField("lastHitter", Hidden).GetValue(court);
                    float end = player.forward.z > 0 ? 1 : -1;
                    if (state == "Rally")
                    {
                        bool incoming = hitter == 1;
                        var desired = incoming ? shuttle.position - player.right * .3f - player.forward * .35f : new Vector3(0, 0, -3.8f * end);
                        desired.z = Mathf.Min(desired.z * end, -1.0f) * end;
                        Call("SetAnalog", Vector2.ClampMagnitude(new Vector2(desired.x - player.position.x, desired.z - player.position.z) * 4 * end, 1));
                        string shot = shuttle.position.y - player.position.y >= 1.45f ? (result.playerReturns % 4 == 3 ? "Smash" : "Clear") : "Lift";
                        if (incoming && (bool)reachable.Invoke(driver, new object[] { shuttle.position, shot }))
                        {
                            Call("SetAnalog", new Vector2(result.playerReturns % 2 == 0 ? -1 : 1, 1)); Call("TapShot", shot);
                            if ((int)type.GetField("lastHitter", Hidden).GetValue(court) == 0) result.playerReturns++;
                        }
                    }
                    else { Call("SetAnalog", Vector2.zero); if (state == "Serving" && flow == "Serving") Call("Serve", true); }
                    yield return new WaitForFixedUpdate();
                }
                Assert.That(Get<string>("MatchState"), Is.EqualTo("MatchEnd"));
                yield return new WaitForSecondsRealtime(2); Click("Menu");
                Assert.That(Get<string>("ScreenName"), Is.EqualTo("Home")); yield return new WaitForSecondsRealtime(1);
            }
            finally
            {
                ((MonoBehaviour)court).StopCoroutine(recording);
                audio.StopCapture(); timeline.Dispose(); timeline = null; recorder.StandardInput.Close();
                Assert.That(recorder.WaitForExit(15000), Is.True, "Video encoder did not finish");
                Assert.That(recorder.ExitCode, Is.Zero); recorder.Dispose();
                File.WriteAllText(stem + "-capture.json", "{\"frames\":" + recordedFrames + ",\"firstFrameSeconds\":" + firstRecordedFrame.ToString(System.Globalization.CultureInfo.InvariantCulture) + ",\"seconds\":" + (Time.unscaledTime - began).ToString(System.Globalization.CultureInfo.InvariantCulture) + ",\"sampleRate\":" + AudioSettings.outputSampleRate + ",\"channels\":" + audio.Channels + ",\"timeScale\":1,\"controlled\":false}");
                Assert.That(new FileInfo(stem + ".f32").Length, Is.GreaterThan(100000), "Game-only audio capture missing");
            }
        }
    }
    [UnityTest, Timeout(180000), Category("Capture")] public IEnumerator CaptureReplayChallengeModesWithGameAudio()
    {
        Assert.That(Application.isBatchMode, Is.False);
        yield return CaptureAt1280x720();
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type);
        Application.targetFrameRate = 60; Time.timeScale = 1;
        string stem = CaptureEvidence + "/replay-challenge";
        var audio = Camera.main.gameObject.AddComponent<MobileGameAudio>();
        captureStarted = Time.realtimeSinceStartup; captureGameStarted = Time.time; recordedFrames = 0;
        timeline = new StreamWriter(stem + "-timeline.csv");
        timeline.WriteLine("realSeconds,gameSeconds,state,hitter,stroke,strokeElapsed,playerX,playerY,playerZ,shuttleX,shuttleY,shuttleZ,screen,flow");
        var events = new List<string> { "realSeconds,scenario,event,replaying,pointWait,points0,points1,quota0" };
        string scenario = "home"; int completed = 0;
        void Mark(string name)
        {
            var match = Get<object>("Match"); var mt = match.GetType();
            var points = (int[])mt.GetField("Points").GetValue(match);
            var quotas = (int[])mt.GetField("Challenges").GetValue(match);
            events.Add(string.Join(",", new object[] { Time.realtimeSinceStartup - captureStarted, scenario, name,
                type.GetField("replaying", Hidden).GetValue(court), type.GetField("pointWait", Hidden).GetValue(court), points[0], points[1], quotas[0]
            }.Select(v => Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture))));
        }
        var recorder = new System.Diagnostics.Process();
        recorder.StartInfo = new System.Diagnostics.ProcessStartInfo("ffmpeg", "-hide_banner -loglevel error -n -use_wallclock_as_timestamps 1 -f rawvideo -pixel_format rgba -video_size " + Screen.width + "x" + Screen.height + " -framerate 1000 -i pipe:0 -fps_mode passthrough -enc_time_base 1:1000 -vf vflip,pad=ceil(iw/2)*2:ceil(ih/2)*2 -an -c:v libx264 -preset ultrafast -crf 23 -pix_fmt yuv420p -movflags +faststart \"" + stem + "-video.mp4\"") { UseShellExecute = false, RedirectStandardInput = true, CreateNoWindow = true };
        recorder.Start();
        Coroutine recording = null;
        double firstVideoWall = 0, lastVideoWall = 0;
        try
        {
            yield return new WaitForEndOfFrame();
            // Actual first/last game frames bound this test capture; no PCM trim, padding or retiming.
            audio.StartCapture(stem + ".f32");
            firstRecordedFrame = Time.realtimeSinceStartup - captureStarted;
            firstVideoWall = (double)System.Diagnostics.Stopwatch.GetTimestamp() / System.Diagnostics.Stopwatch.Frequency;
            WriteFrame(recorder); recordedFrames++;
            recording = ((MonoBehaviour)court).StartCoroutine(RecordMatchFrames(recorder));
            yield return new WaitForSecondsRealtime(.5f);
            foreach (bool doubles in new[] { false, true }) foreach (int mode in new[] { 0, 1, 2 }) foreach (bool corrected in new[] { false, true })
            {
                scenario = (doubles ? "doubles" : "singles") + "-mode" + mode + (corrected ? "-overturned" : "-confirmed");
                Click("Main"); Click(doubles ? "Ganda" : "Tunggal"); Click("Mulai"); Click("Teruskan"); Click("Teruskan");
                type.GetProperty("ReplayMode").SetValue(court, mode);
                Call("Serve", false); yield return new WaitForSeconds(.6f);
                Assert.That(Get<string>("MatchState"), Is.EqualTo("Rally"));
                // Controlled line-call fixture; recording is not natural match footage.
                type.GetField("serviceFlight", Hidden).SetValue(court, false);
                type.GetField("lastHitter", Hidden).SetValue(court, 0);
                if (doubles) type.GetField("lastPlayer", Hidden).SetValue(court, 0);
                type.GetField("velocity", Hidden).SetValue(court, Vector3.down);
                GameObject.Find("Shuttle").transform.position = new Vector3(-1, .045f, 3);
                LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[collision\] ground.*"));
                LogAssert.Expect(LogType.Log, "[scoring] point winner=0");
                type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
                var match = Get<object>("Match"); var mt = match.GetType();
                if (corrected)
                {
                    JsonUtility.FromJsonOverwrite((string)type.GetField("beforePoint", Hidden).GetValue(court), match);
                    mt.GetMethod("AwardPoint").Invoke(match, new object[] { 1 });
                    type.GetField("announcedIn", Hidden).SetValue(court, false);
                }
                Mark("point-before-review"); yield return new WaitForSeconds(.3f);
                var roots = (doubles ? new[] { "Player", "Opponent", "Partner", "OpponentPartner" } : new[] { "Player", "Opponent" }).Select(n => GameObject.Find(n).transform).ToArray();
                var positions = roots.Select(t => t.position).ToArray();
                var cameraPosition = Camera.main.transform.position; var cameraRotation = Camera.main.transform.rotation;
                float frozen = (float)type.GetField("pointWait", Hidden).GetValue(court);
                uint random = (uint)type.GetField("opponentRandom", Hidden).GetValue(court);
                Click("Challenge");
                Assert.That((bool)type.GetField("replaying", Hidden).GetValue(court), Is.EqualTo(mode != 2));
                Assert.That(Call("ChallengeCall"), Is.EqualTo(false));
                match = Get<object>("Match");
                Assert.That((int[])mt.GetField("Points").GetValue(match), Is.EqualTo(new[] { 1, 0 }));
                Assert.That((int[])mt.GetField("Challenges").GetValue(match), Is.EqualTo(new[] { corrected ? 2 : 1, 2 }));
                Mark(mode == 2 ? "review-without-replay" : "replay-start");
                yield return new WaitForSecondsRealtime(.25f);
                if (mode != 2)
                {
                    Assert.That((bool)type.GetField("replaying", Hidden).GetValue(court), Is.True);
                    Assert.That((float)type.GetField("pointWait", Hidden).GetValue(court), Is.EqualTo(frozen));
                    Mark("replay-active");
                    if (mode == 0)
                    {
                        Assert.That(Vector3.Distance(Camera.main.transform.position, new Vector3(8,4,0)), Is.LessThan(.001f));
                        var frames = (System.Collections.IList)type.GetField("replay", Hidden).GetValue(court);
                        yield return new WaitForSecondsRealtime(frames.Count * Time.fixedDeltaTime * 2);
                        Assert.That((bool)type.GetField("replaying", Hidden).GetValue(court), Is.True);
                        Assert.That(Vector3.Distance(Camera.main.transform.position, cameraPosition), Is.LessThan(.001f));
                        Mark("replay-second-angle");
                    }
                    Click("Lewati");
                    for (int i = 0; i < roots.Length; i++) Assert.That(Vector3.Distance(roots[i].position, positions[i]), Is.LessThan(.001f));
                    Assert.That(Vector3.Distance(Camera.main.transform.position, cameraPosition), Is.LessThan(.001f));
                    Assert.That(Quaternion.Angle(Camera.main.transform.rotation, cameraRotation), Is.LessThan(.01f));
                    Assert.That((float)type.GetField("pointWait", Hidden).GetValue(court), Is.EqualTo(frozen));
                    Assert.That((uint)type.GetField("opponentRandom", Hidden).GetValue(court), Is.EqualTo(random));
                    Mark("replay-skipped-live-restored");
                }
                float until = Time.realtimeSinceStartup + 4;
                while (Get<string>("MatchState") == "Point" && Time.realtimeSinceStartup < until) yield return null;
                Assert.That(Get<string>("MatchState"), Is.EqualTo("Serving"));
                Assert.That(Get<bool>("Flying"), Is.False, "Point preparation must not auto-serve human");
                Assert.That((int[])mt.GetField("Points").GetValue(Get<object>("Match")), Is.EqualTo(new[] { 1, 0 }));
                Assert.That((int[])mt.GetField("Challenges").GetValue(Get<object>("Match")), Is.EqualTo(new[] { corrected ? 2 : 1, 2 }));
                Assert.That(Call("ChallengeCall"), Is.EqualTo(false));
                Mark("serving-once"); completed++;
                yield return new WaitForSecondsRealtime(.2f);
                Click("Pause"); Click("Menu"); Click("Ya");
                Assert.That(Get<string>("ScreenName"), Is.EqualTo("Home"));
                yield return new WaitForSecondsRealtime(.2f);
            }
            Assert.That(completed, Is.EqualTo(12));
            yield return new WaitForEndOfFrame();
            ((MonoBehaviour)court).StopCoroutine(recording); recording = null;
            lastVideoWall = (double)System.Diagnostics.Stopwatch.GetTimestamp() / System.Diagnostics.Stopwatch.Frequency;
            WriteFrame(recorder); recordedFrames++; audio.StopCapture();
        }
        finally
        {
            if (recording != null) ((MonoBehaviour)court).StopCoroutine(recording);
            audio.StopCapture(); timeline.Dispose(); timeline = null;
            File.WriteAllText(stem + "-frame-boundaries.csv", "firstVideoWall,lastVideoWall\n" + firstVideoWall.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + "," + lastVideoWall.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
            recorder.StandardInput.Close(); Assert.That(recorder.WaitForExit(15000), Is.True); Assert.That(recorder.ExitCode, Is.Zero); recorder.Dispose();
            File.WriteAllLines(stem + "-events.csv", events);
            File.WriteAllText(stem + "-capture.json", "{\"frames\":" + recordedFrames + ",\"firstFrameSeconds\":" + firstRecordedFrame.ToString(System.Globalization.CultureInfo.InvariantCulture) + ",\"seconds\":" + (Time.realtimeSinceStartup - captureStarted).ToString(System.Globalization.CultureInfo.InvariantCulture) + ",\"sampleRate\":" + AudioSettings.outputSampleRate + ",\"channels\":" + audio.Channels + ",\"timeScale\":1,\"controlled\":true,\"completedScenarios\":" + completed + "}");
        }
    }
    [UnityTest, Timeout(120000), Category("Capture")] public IEnumerator CaptureControlledPlayerMarkerAndPointFlowAt1x()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720();
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type);
        int oldRate = Application.targetFrameRate; float oldScale = Time.timeScale;
        Application.targetFrameRate = 60; Time.timeScale = 1;
        string stem = CaptureEvidence + "/marker-flow", scenario = "home", phase = "home", lastPhase = "";
        bool doubles = false, expected = false, success = false; int frames = 0, completed = 0, airborne = 0;
        float started = Time.realtimeSinceStartup, first = -1, last = -1;
        var clock = System.Diagnostics.Stopwatch.StartNew();
        object Field(string name) => type.GetField(name, Hidden).GetValue(court);
        int[] Points() { var m = Get<object>("Match"); return m == null ? new[] { 0, 0 } : (int[])m.GetType().GetField("Points").GetValue(m); }
        string Csv(params object[] values) => string.Join(",", values.Select(v => Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture).Replace(",", ";").Replace("\r", "").Replace("\n", " | ")));
        using var samples = new StreamWriter(stem + "-samples.csv");
        using var events = new StreamWriter(stem + "-events.csv");
        samples.WriteLine("frame,scenario,phase,wall,game,timeScale,screen,state,flow,replaying,pointWait,points0,points1,energy,rng,playerX,playerY,playerZ,expected,markerActive,markerEnabled,markerX,markerY,markerZ,hintActive,pointNotice");
        events.WriteLine("frame,scenario,phase,wall");
        using var recorder = new System.Diagnostics.Process();
        recorder.StartInfo = new System.Diagnostics.ProcessStartInfo("ffmpeg", "-hide_banner -loglevel error -n -use_wallclock_as_timestamps 1 -f rawvideo -pixel_format rgba -video_size 1280x720 -framerate 1000 -i pipe:0 -fps_mode passthrough -enc_time_base 1:1000 -vf vflip -an -c:v libx264 -preset ultrafast -crf 23 -pix_fmt yuv420p -movflags +faststart \"" + stem + "-video.mp4\"") { UseShellExecute = false, RedirectStandardInput = true, CreateNoWindow = true };
        recorder.Start();
        IEnumerator Sample()
        {
            yield return new WaitForEndOfFrame();
            Assert.That(clock.Elapsed.TotalSeconds, Is.LessThan(80), "Capture watchdog");
            var player = GameObject.Find("Player").transform;
            var ring = Field("activePlayerMarker") as LineRenderer; var hint = Field("suggestedPosition") as Transform;
            bool active = ring != null && ring.gameObject.activeInHierarchy, cyan = hint != null && hint.gameObject.activeInHierarchy;
            Assert.That(active, Is.EqualTo(expected), scenario + "/" + phase + " ring");
            Assert.That(cyan, Is.EqualTo(expected && doubles), scenario + "/" + phase + " hint");
            if (active)
            {
                Assert.That(ring.enabled && ring.sharedMaterial != null && ring.sharedMaterial.shader != null && ring.sharedMaterial.color.a > 0 && ring.startColor.a > 0 && ring.startWidth > 0, Is.True);
                Assert.That(Vector2.Distance(new Vector2(player.position.x, player.position.z), new Vector2(ring.transform.position.x, ring.transform.position.z)), Is.LessThan(.0001f));
                Assert.That(ring.transform.position.y, Is.EqualTo(.018f).Within(.0001f));
            }
            if (phase == "moving-jump" && player.position.y > .02f) airborne++;
            float wall = Time.realtimeSinceStartup - started; if (first < 0) first = wall; last = wall;
            if (lastPhase != scenario + "/" + phase) { events.WriteLine(Csv(frames, scenario, phase, wall)); lastPhase = scenario + "/" + phase; }
            var p = player.position; var r = ring == null ? Vector3.zero : ring.transform.position; var score = Points();
            samples.WriteLine(Csv(frames, scenario, phase, wall, Time.time, Time.timeScale, Get<string>("ScreenName"), Get<string>("MatchState"), Get<string>("FlowStage"), Field("replaying"), Field("pointWait"), score[0], score[1], Get<float>("Energy"), Field("opponentRandom"), p.x, p.y, p.z, expected, active, ring != null && ring.enabled, r.x, r.y, r.z, cyan, type.GetProperty("PointNotice", Hidden).GetValue(court)));
            WriteFrame(recorder); frames++;
        }
        IEnumerator Hold(string label, float seconds, bool visible)
        {
            phase = label; expected = visible; float until = Time.realtimeSinceStartup + seconds; int before = frames;
            do { yield return Sample(); } while (Time.realtimeSinceStartup < until || frames - before < 2);
        }
        try
        {
            foreach (bool pair in new[] { false, true }) foreach (bool swapped in new[] { false, true })
            {
                doubles = pair; scenario = (pair ? "doubles" : "singles") + (swapped ? "-far" : "-near");
                yield return Hold("home", .2f, false);
                Click("Main"); Click(pair ? "Ganda" : "Tunggal"); Click("Mulai");
                var match = Get<object>("Match"); var mt = match.GetType(); mt.GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
                for (int i = 0; i < 3 && Get<string>("FlowStage") != "Serving"; i++) Call("AdvanceFlow");
                Assert.That(Get<string>("FlowStage"), Is.EqualTo("Serving"));
                type.GetProperty("ReplayMode").SetValue(court, 1);
                string saved = (string)Call("CaptureMatch");
                yield return Hold("controlled-serving", .25f, true);
                var root = GameObject.Find("Player").transform; var beforeMove = root.position; airborne = 0;
                Call("Serve", true); Assert.That(Get<string>("MatchState"), Is.EqualTo("Rally"));
                Call("SetAnalog", new Vector2(.6f, -.2f)); Call("Jump");
                yield return Hold("moving-jump", .35f, true); Call("SetAnalog", Vector2.zero);
                Assert.That(airborne, Is.GreaterThanOrEqualTo(2));
                Assert.That(Vector2.Distance(new Vector2(root.position.x, root.position.z), new Vector2(beforeMove.x, beforeMove.z)), Is.GreaterThan(.1f));
                yield return Hold("landing", .45f, true); Assert.That(root.position.y, Is.LessThan(.001f));
                Assert.That(Get<string>("MatchState"), Is.EqualTo("Rally"));
                // Controlled legal ground collision, not a natural rally outcome or forced score.
                type.GetField("serviceFlight", Hidden).SetValue(court, false); type.GetField("lastHitter", Hidden).SetValue(court, 0);
                if (pair) type.GetField("lastPlayer", Hidden).SetValue(court, 0);
                type.GetField("velocity", Hidden).SetValue(court, Vector3.down);
                GameObject.Find("Shuttle").transform.position = new Vector3(-1, .045f, swapped ? -3 : 3);
                LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[collision\] ground.*"));
                LogAssert.Expect(LogType.Log, "[scoring] point winner=0");
                yield return Hold("controlled-ground-point", .15f, true);
                Assert.That(Get<string>("MatchState"), Is.EqualTo("Point")); Assert.That(Points(), Is.EqualTo(new[] { 1, 0 }));
                Assert.That(type.GetProperty("PointNotice", Hidden).GetValue(court), Is.EqualTo("Poin kamu\nKok masuk"));
                Click("Pause"); float frozen = (float)Field("pointWait"), energy = Get<float>("Energy"); var rng = Field("opponentRandom"); var pausedRoot = root.position;
                yield return Hold("pause", .35f, false);
                Assert.That(Get<string>("ScreenName"), Is.EqualTo("Pause")); Assert.That(Time.timeScale, Is.Zero);
                Assert.That(Field("pointWait"), Is.EqualTo(frozen)); Assert.That(Get<float>("Energy"), Is.EqualTo(energy)); Assert.That(Field("opponentRandom"), Is.EqualTo(rng)); Assert.That(root.position, Is.EqualTo(pausedRoot));
                Click("Lanjut"); yield return Hold("resumed", .15f, true);
                var roots = (pair ? new[] { "Player", "Opponent", "Partner", "OpponentPartner" } : new[] { "Player", "Opponent" }).Select(n => GameObject.Find(n).transform).ToArray();
                var positions = roots.Select(t => t.position).ToArray(); var cameraPosition = Camera.main.transform.position; var cameraRotation = Camera.main.transform.rotation;
                frozen = (float)Field("pointWait"); rng = Field("opponentRandom");
                Assert.That(Call("BeginReplay"), Is.EqualTo(true)); yield return Hold("replay", .25f, false);
                Assert.That(Field("replaying"), Is.EqualTo(true)); Assert.That(Field("pointWait"), Is.EqualTo(frozen));
                Assert.That(Get<float>("Energy"), Is.EqualTo(energy)); Assert.That(Field("opponentRandom"), Is.EqualTo(rng)); Assert.That(Points(), Is.EqualTo(new[] { 1, 0 }));
                Click("Lewati");
                for (int i = 0; i < roots.Length; i++) Assert.That(Vector3.Distance(roots[i].position, positions[i]), Is.LessThan(.001f));
                Assert.That(Vector3.Distance(Camera.main.transform.position, cameraPosition), Is.LessThan(.001f)); Assert.That(Quaternion.Angle(Camera.main.transform.rotation, cameraRotation), Is.LessThan(.01f));
                Assert.That(Field("pointWait"), Is.EqualTo(frozen)); Assert.That(Field("opponentRandom"), Is.EqualTo(rng));
                yield return Hold("replay-skipped", .15f, true);
                phase = "point-wait"; expected = true; float deadline = Time.realtimeSinceStartup + 4;
                while (Get<string>("MatchState") == "Point" && Time.realtimeSinceStartup < deadline) yield return Sample();
                Assert.That(Get<string>("MatchState"), Is.EqualTo("Serving")); Assert.That(Field("pointWait"), Is.EqualTo(0f));
                yield return Hold("serving-once", .35f, true);
                Assert.That(Get<bool>("Flying"), Is.False); Assert.That(Points(), Is.EqualTo(new[] { 1, 0 })); Assert.That(Get<float>("Energy"), Is.LessThanOrEqualTo(energy));
                Call("RestoreMatch", saved); Assert.That(Points(), Is.EqualTo(new[] { 0, 0 })); Assert.That(Get<string>("MatchState"), Is.EqualTo("Serving"));
                Assert.That(mt.GetField("EndsSwapped").GetValue(Get<object>("Match")), Is.EqualTo(swapped));
                yield return Hold("controlled-restored", .25f, true);
                match = Get<object>("Match");
                for (int i = 0; i < 100 && (int)mt.GetField("Winner").GetValue(match) < 0; i++) mt.GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
                Assert.That((int)mt.GetField("Winner").GetValue(match), Is.EqualTo(0)); Call("NextPoint");
                yield return Hold("controlled-result", .35f, false);
                Click("Menu"); if (Get<string>("ScreenName") == "Confirm") Click("Ya");
                Assert.That(Get<string>("ScreenName"), Is.EqualTo("Home"));
                yield return Hold("home-returned", .2f, false); completed++;
            }
            Assert.That(completed, Is.EqualTo(4)); success = true;
        }
        finally
        {
            Call("SetAnalog", Vector2.zero); Time.timeScale = oldScale; Application.targetFrameRate = oldRate;
            samples.Flush(); events.Flush(); recorder.StandardInput.Close(); bool ended = recorder.WaitForExit(15000);
            if (!ended) recorder.Kill();
            int exit = ended ? recorder.ExitCode : -1;
            File.WriteAllText(stem + "-capture.json", "{\"success\":" + (success && ended && exit == 0 ? "true" : "false") + ",\"controlled\":true,\"audio\":false,\"completedScenarios\":" + completed + ",\"frames\":" + frames + ",\"firstFrameSeconds\":" + first.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + ",\"lastFrameSeconds\":" + last.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + ",\"encoderExit\":" + exit + "}");
            Assert.That(ended && exit == 0, Is.True, "Encoder did not finalize successfully");
        }
    }
    [UnityTest, Timeout(120000), Category("Capture")] public IEnumerator CaptureDoublesSeparationAtOneTimespeed()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720();
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type); Time.timeScale = 1; Application.targetFrameRate = 60;
        Call("ConfigureDiscipline", true);
        string stem = CaptureEvidence + "/doubles-separation";
        var recorder = new System.Diagnostics.Process();
        recorder.StartInfo = new System.Diagnostics.ProcessStartInfo("ffmpeg", "-hide_banner -loglevel error -n -use_wallclock_as_timestamps 1 -f rawvideo -pixel_format rgba -video_size " + Screen.width + "x" + Screen.height + " -framerate 1000 -i pipe:0 -fps_mode passthrough -enc_time_base 1:1000 -vf vflip,pad=ceil(iw/2)*2:ceil(ih/2)*2 -an -c:v libx264 -preset ultrafast -crf 20 -pix_fmt yuv420p \"" + stem + "-video.mp4\"") { UseShellExecute = false, RedirectStandardInput = true, CreateNoWindow = true };
        recorder.Start(); captureStarted = Time.realtimeSinceStartup; captureGameStarted = Time.time;
        timeline = new StreamWriter(stem + "-timeline.csv");
        timeline.WriteLine("realSeconds,gameSeconds,state,hitter,stroke,strokeElapsed,playerX,playerY,playerZ,shuttleX,shuttleY,shuttleZ,screen,flow");
        var samples = new List<string> { "realSeconds,scenario,distance,playerX,playerZ,partnerX,partnerZ" };
        var events = new List<string> { "realSeconds,scenario,event" }; int frames = 0, completed = 0;
        void Set(string field, object value) => type.GetField(field, Hidden).SetValue(court, value);
        Transform Actor(int id) => (Transform)type.GetMethod("Actor", Hidden).Invoke(court, new object[] { id });
        try
        {
            foreach (bool swapped in new[] { false, true }) foreach (int owner in new[] { 0, 2 })
            {
                Call("ConfigureOpponent", "Normal", 193); Call("StartMatch"); var match = Get<object>("Match");
                match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
                Set("flowStage", null); Call("Serve", true); Set("assisted", false); Call("SetAnalog", Vector2.zero);
                Set("serviceFlight", false); Set("lastHitter", 1); Set("lastPlayer", 1); Set("nearOwner", owner);
                float end = swapped ? -1 : 1;
                Actor(0).position = Actor(2).position = new Vector3(0, 0, -3.6f * end);
                GameObject.Find("Shuttle").transform.position = new Vector3(0, 20, -3 * end); Set("velocity", Vector3.up);
                Set("teamObserved", type.GetField("aiContactAt", Hidden).GetValue(court));
                var targets = (Vector3[])type.GetField("teamTargets", Hidden).GetValue(court);
                var thinks = (double[])type.GetField("teamThink", Hidden).GetValue(court); var refresh = (double[])type.GetField("teamRefresh", Hidden).GetValue(court);
                for (int id = 0; id < 4; id++) { targets[id] = Actor(id).position; thinks[id] = 0; refresh[id] = double.MaxValue; }
                string scenario = (swapped ? "swapped" : "home") + "-owner" + owner;
                void Mark(string ev) => events.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2}", Time.realtimeSinceStartup - captureStarted, scenario, ev));
                Mark("start"); float began = Time.time, next = Time.realtimeSinceStartup; bool captured = false;
                do
                {
                    yield return new WaitForEndOfFrame();
                    if (Time.realtimeSinceStartup < next && Time.time - began < 1) continue; next += 1f / 30;
                    WriteFrame(recorder); frames++;
                    samples.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6}", Time.realtimeSinceStartup - captureStarted, scenario, Vector3.Distance(Actor(0).position, Actor(2).position), Actor(0).position.x, Actor(0).position.z, Actor(2).position.x, Actor(2).position.z));
                    if (!captured && Time.time - began >= .75f)
                    {
                        var image = ScreenCapture.CaptureScreenshotAsTexture(); File.WriteAllBytes(CaptureEvidence + "/" + scenario + ".png", image.EncodeToPNG()); UnityEngine.Object.Destroy(image);
                        Mark("still"); captured = true;
                    }
                } while (Time.time - began < 1);
                Assert.That(captured, Is.True); Mark("complete"); completed++;
            }
        }
        finally
        {
            timeline.Dispose(); timeline = null; recorder.StandardInput.Close();
            Assert.That(recorder.WaitForExit(15000), Is.True); Assert.That(recorder.ExitCode, Is.Zero); recorder.Dispose();
            File.WriteAllLines(stem + "-events.csv", events); File.WriteAllLines(stem + "-distance.csv", samples);
            File.WriteAllText(stem + "-capture.json", "{\"frames\":" + frames + ",\"timeScale\":1,\"controlled\":true,\"audio\":false,\"completedScenarios\":" + completed + "}");
        }
        Assert.That(completed, Is.EqualTo(4));
    }
    [UnityTest, Timeout(120000), Category("Capture")] public IEnumerator CaptureAiForecourtDecisionAtOneTimespeed()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720();
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type); Time.timeScale = 1; Application.targetFrameRate = 60;
        var player = GameObject.Find("Player").transform; var opponent = GameObject.Find("Opponent").transform; var shuttle = GameObject.Find("Shuttle").transform;
        string stem = CaptureEvidence + "/ai-forecourt";
        var recorder = new System.Diagnostics.Process();
        recorder.StartInfo = new System.Diagnostics.ProcessStartInfo("ffmpeg", "-hide_banner -loglevel error -n -use_wallclock_as_timestamps 1 -f rawvideo -pixel_format rgba -video_size " + Screen.width + "x" + Screen.height + " -framerate 1000 -i pipe:0 -fps_mode passthrough -enc_time_base 1:1000 -vf vflip,pad=ceil(iw/2)*2:ceil(ih/2)*2 -an -c:v libx264 -preset ultrafast -crf 20 -pix_fmt yuv420p \"" + stem + "-video.mp4\"") { UseShellExecute = false, RedirectStandardInput = true, CreateNoWindow = true };
        recorder.Start(); captureStarted = Time.realtimeSinceStartup; captureGameStarted = Time.time;
        timeline = new StreamWriter(stem + "-timeline.csv");
        timeline.WriteLine("realSeconds,gameSeconds,state,hitter,stroke,strokeElapsed,playerX,playerY,playerZ,shuttleX,shuttleY,shuttleZ,screen,flow");
        var events = new List<string> { "realSeconds,scenario,event,shot" }; int frames = 0, completed = 0;
        var clocks = new List<string> { "scenario,frame,gameFrame,unscaledFrame,wallBefore,wallAfter,flying,recorded" };
        void Set(string field, object value) => type.GetField(field, Hidden).SetValue(court, value);
        try
        {
            foreach (bool swapped in new[] { false, true }) foreach (string difficulty in new[] { "Normal", "Hard" })
            {
                Call("ConfigureOpponent", difficulty, 193); Call("StartMatch"); var match = Get<object>("Match");
                match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
                Set("flowStage", null); Call("Serve", true); Set("assisted", false); Set("serviceFlight", false); Set("lastHitter", 0);
                float end = swapped ? -1 : 1; opponent.position = new Vector3(.5f * end, 0, 1.2f * end); player.position = new Vector3(-1.5f * end, 0, -5.5f * end);
                shuttle.position = opponent.TransformPoint(new Vector3(.3f, 1.1f, .35f)); Set("velocity", Vector3.down);
                Set("observedSwing", type.GetField("swingAt", Hidden).GetValue(court)); Set("opponentThinkAt", Time.time - 1);
                Set("opponentRefreshAt", float.MaxValue); Set("opponentTarget", opponent.position); Set("aiContactAt", Time.time - 1);
                var before = opponent.position; type.GetMethod("UpdateOpponent", Hidden).Invoke(court, null);
                Assert.That(opponent.position, Is.EqualTo(before)); Assert.That((int)type.GetField("lastHitter", Hidden).GetValue(court), Is.EqualTo(1));
                string shot = (string)type.GetField("opponentStroke", Hidden).GetValue(court), scenario = (swapped ? "swapped-" : "home-") + difficulty;
                void Mark(string ev) => events.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3}", Time.realtimeSinceStartup - captureStarted, scenario, ev, shot));
                Mark("contact"); float began = Time.time, next = Time.realtimeSinceStartup; bool captured = false;
                do
                {
                    yield return new WaitForEndOfFrame();
                    double game = Time.timeAsDouble, unscaled = Time.unscaledTimeAsDouble, wall = Time.realtimeSinceStartupAsDouble;
                    bool flying = Get<bool>("Flying"), record = Time.realtimeSinceStartup >= next || !flying;
                    if (!record)
                    {
                        clocks.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2:R},{3:R},{4:R},{4:R},{5},False", scenario, Time.frameCount, game, unscaled, wall, flying));
                        continue;
                    }
                    next += 1f / 30; WriteFrame(recorder); frames++;
                    clocks.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2:R},{3:R},{4:R},{5:R},{6},True", scenario, Time.frameCount, game, unscaled, wall, Time.realtimeSinceStartupAsDouble, flying));
                    if (!captured && Time.time - began >= .12f)
                    {
                        var image = ScreenCapture.CaptureScreenshotAsTexture(); File.WriteAllBytes(CaptureEvidence + "/" + scenario + ".png", image.EncodeToPNG()); UnityEngine.Object.Destroy(image);
                        Mark("still"); captured = true;
                    }
                } while (Get<bool>("Flying") && Time.time - began < 4);
                Assert.That(Get<bool>("Flying"), Is.False); Assert.That(captured, Is.True); Mark("landed"); completed++;
            }
        }
        finally
        {
            timeline.Dispose(); timeline = null; recorder.StandardInput.Close();
            Assert.That(recorder.WaitForExit(15000), Is.True); Assert.That(recorder.ExitCode, Is.Zero); recorder.Dispose();
            File.WriteAllLines(stem + "-events.csv", events); File.WriteAllLines(stem + "-clocks.csv", clocks);
            File.WriteAllText(stem + "-capture.json", "{\"frames\":" + frames + ",\"timeScale\":1,\"controlled\":true,\"audio\":false,\"completedScenarios\":" + completed + "}");
        }
        Assert.That(completed, Is.EqualTo(4));
    }
    [UnityTest, Timeout(120000), Category("Capture")] public IEnumerator CaptureMovementMomentumAtOneTimespeed()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720();
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type); Time.timeScale = 1; Application.targetFrameRate = 60;
        Call("ConfigureOpponent", "Normal", 193);
        var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
        string stem = CaptureEvidence + "/movement";
        var recorder = new System.Diagnostics.Process();
        recorder.StartInfo = new System.Diagnostics.ProcessStartInfo("ffmpeg", "-hide_banner -loglevel error -n -use_wallclock_as_timestamps 1 -f rawvideo -pixel_format rgba -video_size " + Screen.width + "x" + Screen.height + " -framerate 1000 -i pipe:0 -fps_mode passthrough -enc_time_base 1:1000 -vf vflip,pad=ceil(iw/2)*2:ceil(ih/2)*2 -an -c:v libx264 -preset ultrafast -crf 20 -pix_fmt yuv420p \"" + stem + "-video.mp4\"") { UseShellExecute = false, RedirectStandardInput = true, CreateNoWindow = true };
        recorder.Start(); captureStarted = Time.realtimeSinceStartup; captureGameStarted = Time.time;
        timeline = new StreamWriter(stem + "-timeline.csv");
        timeline.WriteLine("realSeconds,gameSeconds,state,hitter,stroke,strokeElapsed,playerX,playerY,playerZ,shuttleX,shuttleY,shuttleZ,screen,flow");
        var events = new List<string> { "realSeconds,scenario,event" }; int frames = 0, completed = 0;
        void Set(string field, object value) => type.GetField(field, Hidden).SetValue(court, value);
        try
        {
            foreach (bool swapped in new[] { false, true }) foreach (float energy in new[] { 100f, 30f })
            {
                Call("StartMatch"); var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
                Set("flowStage", null); Call("Serve", true); Set("assisted", false); Set("serviceFlight", false);
                float end = swapped ? -1 : 1; player.position = new Vector3(0, 0, -4 * end);
                type.GetProperty("Energy").SetValue(court, energy); shuttle.position = new Vector3(0, 30, -3 * end); Set("velocity", Vector3.up);
                string scenario = (swapped ? "swapped" : "home") + "-energy" + energy;
                float began = Time.time, next = Time.realtimeSinceStartup; int lastPhase = -1; bool captured = false;
                while (Time.time - began < 1.9f)
                {
                    float elapsed = Time.time - began;
                    int phase = elapsed < .35f ? 0 : elapsed < .55f ? 1 : elapsed < .9f ? 2 : elapsed < 1.6f ? 3 : 4;
                    if (phase != lastPhase)
                    {
                        Call("SetAnalog", phase == 0 || phase == 2 ? Vector2.right : phase == 3 ? Vector2.left : Vector2.zero);
                        if (phase == 3) Call("Jump");
                        events.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2}", Time.realtimeSinceStartup - captureStarted, scenario, phase)); lastPhase = phase;
                    }
                    yield return new WaitForEndOfFrame();
                    if (Time.realtimeSinceStartup < next) continue; next += 1f / 30;
                    WriteFrame(recorder); frames++;
                    if (!captured && elapsed >= 1.04f)
                    {
                        var image = ScreenCapture.CaptureScreenshotAsTexture(); File.WriteAllBytes(CaptureEvidence + "/" + scenario + ".png", image.EncodeToPNG()); UnityEngine.Object.Destroy(image); captured = true;
                    }
                }
                Assert.That(captured, Is.True); Assert.That(lastPhase, Is.EqualTo(4)); completed++;
            }
        }
        finally
        {
            Call("SetAnalog", Vector2.zero); timeline.Dispose(); timeline = null; recorder.StandardInput.Close();
            Assert.That(recorder.WaitForExit(15000), Is.True); Assert.That(recorder.ExitCode, Is.Zero); recorder.Dispose();
            File.WriteAllLines(stem + "-events.csv", events);
            File.WriteAllText(stem + "-capture.json", "{\"frames\":" + frames + ",\"timeScale\":1,\"controlled\":true,\"audio\":false,\"completedScenarios\":" + completed + "}");
        }
        Assert.That(completed, Is.EqualTo(4));
    }
    [UnityTest, Timeout(120000), Category("Capture")] public IEnumerator CaptureFatigueRecoveryAtOneTimespeed()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720();
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type); Time.timeScale = 1; Application.targetFrameRate = 60;
        Call("ConfigureOpponent", "Normal", 193);
        var opponent = GameObject.Find("Opponent").transform; var shuttle = GameObject.Find("Shuttle").transform;
        string stem = CaptureEvidence + "/fatigue";
        var recorder = new System.Diagnostics.Process();
        recorder.StartInfo = new System.Diagnostics.ProcessStartInfo("ffmpeg", "-hide_banner -loglevel error -n -use_wallclock_as_timestamps 1 -f rawvideo -pixel_format rgba -video_size " + Screen.width + "x" + Screen.height + " -framerate 1000 -i pipe:0 -fps_mode passthrough -enc_time_base 1:1000 -vf vflip,pad=ceil(iw/2)*2:ceil(ih/2)*2 -an -c:v libx264 -preset ultrafast -crf 20 -pix_fmt yuv420p \"" + stem + "-video.mp4\"") { UseShellExecute = false, RedirectStandardInput = true, CreateNoWindow = true };
        recorder.Start(); captureStarted = Time.realtimeSinceStartup; captureGameStarted = Time.time;
        timeline = new StreamWriter(stem + "-timeline.csv");
        timeline.WriteLine("realSeconds,gameSeconds,state,hitter,stroke,strokeElapsed,playerX,playerY,playerZ,shuttleX,shuttleY,shuttleZ,screen,flow");
        var samples = new List<string> { "realSeconds,gameSeconds,scenario,opponentX,opponentZ,opponentEnergy,playerEnergy" };
        var events = new List<string> { "realSeconds,scenario" }; int frames = 0, completed = 0;
        void Set(string field, object value) => type.GetField(field, Hidden).SetValue(court, value);
        void Mark(string value) => events.Add((Time.realtimeSinceStartup - captureStarted).ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + value);
        try
        {
            foreach (bool swapped in new[] { false, true }) foreach (float energy in new[] { 100f, 30f })
            {
                Call("StartMatch"); var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
                Set("flowStage", null); Call("Serve", true);
                Assert.That(Get<string>("MatchState"), Is.EqualTo("Rally"));
                float end = swapped ? -1 : 1; opponent.position = new Vector3(1, 0, 5.8f * end);
                type.GetProperty("Energy").SetValue(court, energy); type.GetProperty("OpponentEnergy").SetValue(court, energy);
                Set("serviceFlight", false); Set("lastHitter", 1); Set("lastPlayer", 1);
                shuttle.position = new Vector3(0, 8, -3 * end); Set("velocity", Vector3.up);
                string scenario = (swapped ? "swapped" : "home") + "-energy" + energy;
                Mark(scenario + "-start"); float began = Time.time, next = Time.realtimeSinceStartup; bool captured = false;
                while (Time.time - began < 1.2f)
                {
                    yield return new WaitForEndOfFrame();
                    if (Time.realtimeSinceStartup < next) continue; next += 1f / 30;
                    WriteFrame(recorder); frames++;
                    samples.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6}", Time.realtimeSinceStartup - captureStarted, Time.time - captureGameStarted, scenario, opponent.position.x, opponent.position.z, Get<float>("OpponentEnergy"), Get<float>("Energy")));
                    if (!captured && Time.time - began >= .22f)
                    {
                        var image = ScreenCapture.CaptureScreenshotAsTexture(); File.WriteAllBytes(CaptureEvidence + "/" + scenario + ".png", image.EncodeToPNG()); UnityEngine.Object.Destroy(image);
                        Mark(scenario + "-still"); captured = true;
                    }
                }
                Assert.That(captured, Is.True); Assert.That(Vector3.Distance(opponent.position, new Vector3(0, 0, 3.8f * end)), Is.LessThan(.01f));
                Mark(scenario + "-complete"); completed++;
            }
        }
        finally
        {
            timeline.Dispose(); timeline = null; recorder.StandardInput.Close();
            Assert.That(recorder.WaitForExit(15000), Is.True); Assert.That(recorder.ExitCode, Is.Zero); recorder.Dispose();
            File.WriteAllLines(stem + "-events.csv", events); File.WriteAllLines(stem + "-energy.csv", samples);
            File.WriteAllText(stem + "-capture.json", "{\"frames\":" + frames + ",\"timeScale\":1,\"controlled\":true,\"audio\":false,\"completedScenarios\":" + completed + "}");
        }
        Assert.That(completed, Is.EqualTo(4));
    }
    [UnityTest, Timeout(120000), Category("Capture")] public IEnumerator CaptureFiveShotFlightAtOneTimespeed()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720();
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type); Call("EnterTraining");
        Time.timeScale = 1; Application.targetFrameRate = 60; type.GetField("assisted", Hidden).SetValue(court, false);
        var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
        string stem = CaptureEvidence + "/five-shot";
        var recorder = new System.Diagnostics.Process();
        recorder.StartInfo = new System.Diagnostics.ProcessStartInfo("ffmpeg", "-hide_banner -loglevel error -n -use_wallclock_as_timestamps 1 -f rawvideo -pixel_format rgba -video_size " + Screen.width + "x" + Screen.height + " -framerate 1000 -i pipe:0 -fps_mode passthrough -enc_time_base 1:1000 -vf vflip,pad=ceil(iw/2)*2:ceil(ih/2)*2 -an -c:v libx264 -preset ultrafast -crf 20 -pix_fmt yuv420p \"" + stem + "-video.mp4\"") { UseShellExecute=false, RedirectStandardInput=true, CreateNoWindow=true };
        recorder.Start(); captureStarted = Time.realtimeSinceStartup; captureGameStarted = Time.time;
        timeline = new StreamWriter(stem + "-timeline.csv");
        timeline.WriteLine("realSeconds,gameSeconds,state,hitter,stroke,strokeElapsed,playerX,playerY,playerZ,shuttleX,shuttleY,shuttleZ,screen,flow");
        var events = new List<string> { "realSeconds,scenario" }; int frames = 0, completed = 0;
        void Mark(string value) => events.Add((Time.realtimeSinceStartup-captureStarted).ToString(System.Globalization.CultureInfo.InvariantCulture)+","+value);
        try
        {
            foreach (string shot in new[] { "Net", "Smash", "Drop", "Clear", "Lift" })
            {
                Call("Feed", "Net"); player.position = new Vector3(0, 0, shot == "Net" || shot == "Lift" ? -.9f : -3.5f);
                shuttle.position = player.TransformPoint(new Vector3(.3f, shot == "Net" || shot == "Lift" ? 1 : 2.1f, .35f));
                type.GetField("velocity", Hidden).SetValue(court, Vector3.zero);
                Call("SetAnalog", Vector2.zero); Mark(shot + "-contact"); Call("TapShot", shot);
                Assert.That(type.GetField("lastStroke", Hidden).GetValue(court), Is.EqualTo(shot));
                float until = Time.time + 4, next = Time.realtimeSinceStartup;
                do
                {
                    yield return new WaitForEndOfFrame();
                    if (Time.realtimeSinceStartup < next) continue; next += 1f/30;
                    WriteFrame(recorder); frames++;
                } while (Get<bool>("Flying") && Time.time < until);
                Assert.That(Get<bool>("Flying"), Is.False); Mark(shot + "-landed"); completed++;
            }
        }
        finally
        {
            timeline.Dispose(); timeline = null; recorder.StandardInput.Close();
            Assert.That(recorder.WaitForExit(15000), Is.True); Assert.That(recorder.ExitCode, Is.Zero); recorder.Dispose();
            File.WriteAllLines(stem + "-events.csv", events);
            File.WriteAllText(stem + "-capture.json", "{\"frames\":" + frames + ",\"seconds\":" + (Time.realtimeSinceStartup-captureStarted).ToString(System.Globalization.CultureInfo.InvariantCulture) + ",\"controlled\":true,\"timeScale\":1,\"completedScenarios\":" + completed + "}");
        }
        Assert.That(completed, Is.EqualTo(5));
    }
    [UnityTest, Timeout(120000), Category("Capture")] public IEnumerator CaptureApproachJumpSmashPilot()
    {
        Assert.That(Application.isBatchMode, Is.False);
        yield return CaptureAt1280x720();
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type); Call("EnterTraining");
        Application.targetFrameRate = 60; Time.timeScale = 1;
        var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
        var driver = player.Find("AthleteVisual").GetComponent(AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.AthleteVisual")).First(t => t != null));
        var bed = (Transform)driver.GetType().GetProperty("StringBed").GetValue(driver);
        GameObject.Find("Shuttle").GetComponent<TrailRenderer>().enabled = false;
        var auditCamera = Camera.main.gameObject.AddComponent<PilotAuditCamera>();
        auditCamera.subject = player;
        string stem = CaptureEvidence + "/pilot";
        var recorder = new System.Diagnostics.Process();
        recorder.StartInfo = new System.Diagnostics.ProcessStartInfo("ffmpeg", "-hide_banner -loglevel error -n -use_wallclock_as_timestamps 1 -f rawvideo -pixel_format rgba -video_size " + Screen.width + "x" + Screen.height + " -framerate 1000 -i pipe:0 -fps_mode passthrough -enc_time_base 1:1000 -vf vflip,pad=ceil(iw/2)*2:ceil(ih/2)*2 -an -c:v libx264 -preset ultrafast -crf 20 -pix_fmt yuv420p \"" + stem + "-video.mp4\"") { UseShellExecute=false, RedirectStandardInput=true, CreateNoWindow=true };
        recorder.Start(); captureStarted=Time.realtimeSinceStartup; captureGameStarted=Time.time;
        timeline = new StreamWriter(stem + "-timeline.csv");
        timeline.WriteLine("realSeconds,gameSeconds,state,hitter,stroke,strokeElapsed,playerX,playerY,playerZ,shuttleX,shuttleY,shuttleZ,screen,flow");
        var events = new List<string> { "realSeconds,scenario" }; int frames=0;
        IEnumerator Record(float seconds, bool sideCamera)
        {
            auditCamera.enabled = sideCamera;
            float until=Time.time+seconds, next=Time.realtimeSinceStartup;
            while(Time.time<until)
            {
                yield return new WaitForEndOfFrame();
                if(Time.realtimeSinceStartup<next) continue;
                if (sideCamera) Assert.That(Vector3.Distance(Camera.main.transform.position, player.position + new Vector3(3, 1.6f, 1)), Is.LessThan(.01f), "Audit camera overwritten before capture");
                WriteFrame(recorder); frames++; next+=1f/30;
            }
        }
        void Mark(string value) => events.Add((Time.realtimeSinceStartup-captureStarted).ToString(System.Globalization.CultureInfo.InvariantCulture)+","+value);
        try
        {
            foreach(bool audit in new[] { false,true }) foreach(float side in new[] { .4f,-.4f })
            {
                Call("SetAnalog",Vector2.zero); player.position=new Vector3(0,0,-4.5f);
                Mark((audit?"side-":"gameplay-")+(side<0?"backhand":"forehand"));
                yield return Record(.4f,audit);
                Call("SetAnalog",new Vector2(.15f,1)); yield return Record(.4f,audit); Call("SetAnalog",Vector2.zero);
                Call("Jump"); yield return Record(.30f,audit);
                Call("Feed","High"); shuttle.position=player.TransformPoint(new Vector3(side,2.1f,.35f));
                type.GetField("velocity",Hidden).SetValue(court,Vector3.zero);
                float energy=Get<float>("Energy"); Mark("contact"); Call("TapShot","Smash");
                Assert.That(Get<float>("Energy"),Is.LessThan(energy));
                Assert.That(Vector3.Distance(bed.position,shuttle.position),Is.LessThan(.05f));
                yield return Record(.8f,audit);
                Assert.That(player.position.y,Is.EqualTo(0).Within(.001f));
                Call("SetAnalog",Vector2.down); yield return Record(.3f,audit); Call("SetAnalog",Vector2.zero); yield return Record(.3f,audit);
                type.GetField("flying",Hidden).SetValue(court,false);
            }
        }
        finally
        {
            UnityEngine.Object.Destroy(auditCamera);
            timeline.Dispose(); timeline=null; recorder.StandardInput.Close();
            Assert.That(recorder.WaitForExit(15000),Is.True); Assert.That(recorder.ExitCode,Is.Zero); recorder.Dispose();
            File.WriteAllLines(stem+"-events.csv",events);
            File.WriteAllText(stem+"-capture.json","{\"frames\":"+frames+",\"seconds\":"+(Time.realtimeSinceStartup-captureStarted).ToString(System.Globalization.CultureInfo.InvariantCulture)+",\"controlled\":true,\"requestedFps\":30,\"timeScale\":1}");
        }
    }
    [UnityTest, Timeout(120000), Category("Capture")] public IEnumerator CaptureControlledFootworkStrokesAndJumpAtOneTimespeed() => CaptureControlledMotion(false);
    [UnityTest, Timeout(120000), Category("Capture")] public IEnumerator CaptureLocomotionCloseupWithGameAudio() => CaptureControlledMotion(true);
    IEnumerator CaptureControlledMotion(bool closeup)
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720();
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type);
        Call("EnterTraining"); type.GetField("assisted", Hidden).SetValue(court, false);
        type.GetProperty("ReplayMode").SetValue(court, 2); type.GetProperty("Volume").SetValue(court, .7f);
        Time.timeScale = 1; Application.targetFrameRate = 60;
        Directory.CreateDirectory(CaptureEvidence);
        string stem = CaptureEvidence + (closeup ? "/locomotion-closeup-1x" : "/controlled-motion-1x");
        var audio = Camera.main.gameObject.AddComponent<MobileGameAudio>(); audio.StartCapture(stem + ".f32");
        var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
        var auditCamera = closeup ? Camera.main.gameObject.AddComponent<PilotAuditCamera>() : null;
        if (auditCamera != null) auditCamera.subject = player;
        var recorder = new System.Diagnostics.Process();
        recorder.StartInfo = new System.Diagnostics.ProcessStartInfo("ffmpeg", "-hide_banner -loglevel error -n -use_wallclock_as_timestamps 1 -f rawvideo -pixel_format rgba -video_size " + Screen.width + "x" + Screen.height + " -framerate 1000 -i pipe:0 -fps_mode passthrough -enc_time_base 1:1000 -vf vflip,pad=ceil(iw/2)*2:ceil(ih/2)*2 -an -c:v libx264 -preset ultrafast -crf 23 -pix_fmt yuv420p -movflags +faststart \"" + stem + "-video.mp4\"") { UseShellExecute = false, RedirectStandardInput = true, CreateNoWindow = true };
        recorder.Start();
        captureStarted = Time.realtimeSinceStartup; captureGameStarted = Time.time; firstRecordedFrame = 0;
        timeline = new StreamWriter(stem + "-timeline.csv");
        timeline.WriteLine("realSeconds,gameSeconds,state,hitter,stroke,strokeElapsed,playerX,playerY,playerZ,shuttleX,shuttleY,shuttleZ,screen,flow");
        var events = new List<string> { "realSeconds,scenario" }; int frames = 0; float apex = 0;
        IEnumerator Record(float seconds)
        {
            float until = Time.time + seconds, next = Time.realtimeSinceStartup;
            while (Time.time < until)
            {
                yield return new WaitForEndOfFrame();
                if (Time.realtimeSinceStartup < next) continue; next += 1f / 30;
                if (frames == 0) firstRecordedFrame = Time.realtimeSinceStartup - captureStarted;
                WriteFrame(recorder); frames++;
                apex = Mathf.Max(apex, player.position.y);

            }
        }
        void Mark(string name) => events.Add((Time.realtimeSinceStartup - captureStarted).ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + name);
        try
        {
            Mark("idle"); yield return Record(.5f);
            foreach (float pace in closeup ? new[] { .35f, 1f } : new[] { 1f })
            foreach (var direction in new[] { Vector2.left, Vector2.right, Vector2.up, Vector2.down })
            {
                Mark("footwork-" + pace + "-" + direction.ToString().Replace(",", ";")); Vector3 before = player.position;
                Call("SetAnalog", direction * pace); yield return Record(.6f); Call("SetAnalog", Vector2.zero);
                Assert.That(Vector3.Distance(before, player.position), Is.GreaterThan(.5f));
                yield return Record(.2f);
            }
            foreach (string shot in new[] { "Smash", "Clear", "Drop", "Net", "Lift" })
            foreach (float side in new[] { -.4f, .4f })
            {
                Call("Feed", "Net"); player.position = new Vector3(0, 0, shot == "Net" ? -.9f : -3.5f);
                yield return Record(.25f);
                shuttle.position = player.TransformPoint(new Vector3(side, shot == "Net" || shot == "Lift" ? 1 : 2.1f, .35f));
                type.GetField("velocity", Hidden).SetValue(court, Vector3.zero);
                float energy = Get<float>("Energy"); Mark(shot + (side < 0 ? "-backhand" : "-forehand"));
                Call("TapShot", shot);
                Assert.That((string)type.GetField("lastStroke", Hidden).GetValue(court), Is.EqualTo(shot));
                Assert.That(Get<float>("Energy"), Is.LessThan(energy), "Controlled shot must reach actual hit entrypoint");
                yield return Record(.9f);
            }
            Call("EnterTraining"); Call("SetAnalog", Vector2.zero);
            Mark("jump-landing"); Call("Jump"); yield return Record(1.25f);
            Assert.That(apex, Is.GreaterThan(.5f)); Assert.That(player.position.y, Is.EqualTo(0).Within(.001f));
            foreach (float side in new[] { -.4f, .4f })
            {
                Mark(side < 0 ? "jump-smash-backhand" : "jump-smash-forehand");
                Call("Jump"); yield return Record(.3f);
                Call("Feed", "High");
                shuttle.position = player.TransformPoint(new Vector3(side, 2.1f, .35f));
                type.GetField("velocity", Hidden).SetValue(court, Vector3.zero);
                float energy = Get<float>("Energy"); Call("TapShot", "Smash");
                Assert.That(Get<float>("Energy"), Is.LessThan(energy));
                yield return Record(1.0f);
                Assert.That(player.position.y, Is.EqualTo(0).Within(.001f));
            }
            Assert.That(frames, Is.GreaterThan(150));
        }
        finally
        {
            if (auditCamera != null) UnityEngine.Object.Destroy(auditCamera);
            audio.StopCapture(); timeline.Dispose(); timeline = null; recorder.StandardInput.Close();
            Assert.That(recorder.WaitForExit(15000), Is.True); Assert.That(recorder.ExitCode, Is.Zero); recorder.Dispose();
            File.WriteAllLines(stem + "-events.csv", events);
            File.WriteAllText(stem + "-capture.json", "{\"frames\":" + frames + ",\"seconds\":" + (Time.realtimeSinceStartup - captureStarted).ToString(System.Globalization.CultureInfo.InvariantCulture) + ",\"firstFrameSeconds\":" + firstRecordedFrame.ToString(System.Globalization.CultureInfo.InvariantCulture) + ",\"sampleRate\":" + AudioSettings.outputSampleRate + ",\"channels\":" + audio.Channels + ",\"timeScale\":1,\"controlled\":true}"); Assert.That(firstRecordedFrame, Is.GreaterThanOrEqualTo(0));
        }
    }
    void WriteFrame(System.Diagnostics.Process recorder)
    {
        if (timeline != null)
        {
            var p = GameObject.Find("Player").transform.position; var s = GameObject.Find("Shuttle").transform.position;
            timeline.WriteLine(string.Join(",", new object[] { Time.realtimeSinceStartup - captureStarted, Time.time - captureGameStarted, Get<string>("MatchState"), type.GetField("lastHitter", Hidden).GetValue(court), type.GetField("lastStroke", Hidden).GetValue(court), Time.time - (float)type.GetField("swingAt", Hidden).GetValue(court), p.x, p.y, p.z, s.x, s.y, s.z, Get<string>("ScreenName"), Get<string>("FlowStage") }.Select(v => Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture))));
        }
        var image = ScreenCapture.CaptureScreenshotAsTexture();
        try { var data = image.GetRawTextureData<byte>(); recorder.StandardInput.BaseStream.Write(data.ToArray(), 0, data.Length); }
        finally { UnityEngine.Object.Destroy(image); }
    }
    [Serializable] class Result
    {
        public string difficulty, state;
        public int seed, playerReturns, opponentReturns, rallies, points, serviceFaults, netFaults, longestRally, winner;
        public float seconds, movedMetres, playerEnergy, opponentEnergy;
    }
    [Serializable] class DoublesResult
    {
        public string difficulty, state;
        public int[] returns = new int[4];
        public int points, serviceFaults, netFaults, humanOwnedTicks, humanReachableTicks;
        public float seconds;
        public float startTime, fixedStep;
        public int fixedTicks, renderFrames;
    }
    int recordedFrames;
    float firstRecordedFrame;
    IEnumerator RecordMatchFrames(System.Diagnostics.Process recorder)
    {
        float next = Time.realtimeSinceStartup;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (Time.realtimeSinceStartup < next) continue;
            if (recordedFrames == 0) firstRecordedFrame = Time.realtimeSinceStartup - captureStarted;
            WriteFrame(recorder); recordedFrames++; next += 1f / 30;
        }
    }
    [UnityTest, Timeout(1200000), Category("Capture")] public IEnumerator CaptureNaturalDoublesMatchWithGameAudio() => NaturalDoublesMatches(true);
    [UnityTest] public IEnumerator NaturalDoublesHarnessClockSurvivesStartUi()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type);
        Click("Main"); Click("Ganda"); Click("Normal"); Click("Mulai");
        Time.timeScale = 15;
        yield return new WaitForFixedUpdate();
        Assert.That(Time.timeScale, Is.EqualTo(15), "Start UI reset diagnostic clock; set harness timeScale after Mulai");
    }
    [UnityTest, Timeout(600000)] public IEnumerator ThreeNaturalDoublesMatchesFinishWithAllFourActiveAthletes() => NaturalDoublesMatches(false);
    IEnumerator NaturalDoublesMatches(bool capture)
    {
        if (capture) { Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720(); }
        var results = new List<string>(); var measured = new List<DoublesResult>();
        foreach (string difficulty in capture ? new[] { "Normal" } : new[] { "Easy", "Normal", "Hard" })
        {
            LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
            yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
            type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
            court = UnityEngine.Object.FindFirstObjectByType(type);
            System.Diagnostics.Process recorder = null; MobileGameAudio audio = null; Coroutine recording = null;
            string stem = CaptureEvidence + "/natural-doubles-normal2";
            Time.timeScale = 1;
            if (capture)
            {
                Directory.CreateDirectory(CaptureEvidence); Assert.That(File.Exists(stem + "-video.mp4"), Is.False);
                Application.targetFrameRate = 60; captureStarted = Time.realtimeSinceStartup; captureGameStarted = Time.time; recordedFrames = 0;
                type.GetProperty("Volume").SetValue(court, .7f);
                timeline = new StreamWriter(stem + "-timeline.csv");
                timeline.WriteLine("realSeconds,gameSeconds,state,hitter,stroke,strokeElapsed,playerX,playerY,playerZ,shuttleX,shuttleY,shuttleZ,screen,flow");
                audio = Camera.main.gameObject.AddComponent<MobileGameAudio>(); audio.StartCapture(stem + ".f32");
                recorder = new System.Diagnostics.Process();
                recorder.StartInfo = new System.Diagnostics.ProcessStartInfo("ffmpeg", "-hide_banner -loglevel error -n -use_wallclock_as_timestamps 1 -f rawvideo -pixel_format rgba -video_size " + Screen.width + "x" + Screen.height + " -framerate 1000 -i pipe:0 -fps_mode passthrough -enc_time_base 1:1000 -vf vflip,pad=ceil(iw/2)*2:ceil(ih/2)*2 -an -c:v libx264 -preset ultrafast -crf 23 -pix_fmt yuv420p -movflags +faststart \"" + stem + "-video.mp4\"") { UseShellExecute = false, RedirectStandardInput = true, CreateNoWindow = true };
                recorder.Start(); recording = ((MonoBehaviour)court).StartCoroutine(RecordMatchFrames(recorder));
            }
            try
            {
                if (capture) yield return new WaitForSecondsRealtime(1);
                Click("Main"); Click("Ganda"); Click(difficulty);
                if (capture) yield return new WaitForSecondsRealtime(1);
                Click("Mulai"); Call("ConfigureOpponent", difficulty, 71); type.GetProperty("ReplayMode").SetValue(court, 2);
                Time.timeScale = capture ? 1 : 15;
                var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
                var driver = player.Find("AthleteVisual").GetComponent(AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.AthleteVisual")).First(t => t != null));
                var result = new DoublesResult { difficulty = difficulty, startTime = Time.time, fixedStep = Time.fixedDeltaTime };
                float began = Time.time; bool wasRally = false; int priorPlayer = -1, firstFrame = Time.frameCount;
                using var trace = new StreamWriter(CaptureEvidence + "/doubles-trace-" + difficulty + ".csv");
                trace.WriteLine("tick,renderFrame,fixedSeconds,frameSeconds,state,flow,hitter,nearOwner,farOwner,random,playerX,playerZ,shuttleX,shuttleY,shuttleZ,points0,points1,games0,games1,fixedDouble,frameDouble,pointWait,serveReadyAt,teamObserved,swingAt,aiContactAt,partnerAt,secondAt,think1,think2,think3,refresh1,refresh2,refresh3,target1X,target1Z,target2X,target2Z,target3X,target3Z,opponentX,opponentZ,partnerX,partnerZ,secondX,secondZ");
                while (Get<string>("MatchState") != "MatchEnd" && Time.time - began < (capture ? 1000 : 3600))
                {
                    result.fixedTicks++; result.renderFrames = Time.frameCount - firstFrame;
                    string flow = Get<string>("FlowStage");
                    if (new[] { "Warmup", "Ready", "GameEnd", "Interval", "ChangeEnds" }.Contains(flow))
                    {
                        if (capture) yield return new WaitForSecondsRealtime(1);
                        Click("Teruskan");
                    }
                    string state = Get<string>("MatchState");
                    int hitter = (int)type.GetField("lastPlayer", Hidden).GetValue(court);
                    var match = Get<object>("Match");
                    var points = (int[])match.GetType().GetField("Points").GetValue(match);
                    var games = (int[])match.GetType().GetField("Games").GetValue(match);
                    // Diagnostic only: round-trip clocks expose deadline quantization without changing scheduling.
                    var think = (double[])type.GetField("teamThink", Hidden).GetValue(court);
                    var refresh = (double[])type.GetField("teamRefresh", Hidden).GetValue(court);
                    var targets = (Vector3[])type.GetField("teamTargets", Hidden).GetValue(court);
                    var opponent = ((Transform)type.GetField("opponent", Hidden).GetValue(court)).position;
                    var partner = ((Transform)type.GetField("partner", Hidden).GetValue(court)).position;
                    var second = ((Transform)type.GetField("opponentPartner", Hidden).GetValue(court)).position;
                    trace.WriteLine(string.Join(",", new object[] { result.fixedTicks, result.renderFrames, Time.fixedTime, Time.time, state, flow, hitter,
                        type.GetField("nearOwner", Hidden).GetValue(court), type.GetField("farOwner", Hidden).GetValue(court), type.GetField("opponentRandom", Hidden).GetValue(court),
                        player.position.x, player.position.z, shuttle.position.x, shuttle.position.y, shuttle.position.z, points[0], points[1], games[0], games[1],
                        Time.fixedTimeAsDouble, Time.timeAsDouble, type.GetField("pointWait", Hidden).GetValue(court), type.GetField("serveReadyAt", Hidden).GetValue(court),
                        type.GetField("teamObserved", Hidden).GetValue(court), type.GetField("swingAt", Hidden).GetValue(court), type.GetField("aiContactAt", Hidden).GetValue(court),
                        type.GetField("partnerAt", Hidden).GetValue(court), type.GetField("secondAt", Hidden).GetValue(court),
                        think[1], think[2], think[3], refresh[1], refresh[2], refresh[3], targets[1].x, targets[1].z, targets[2].x, targets[2].z, targets[3].x, targets[3].z,
                        opponent.x, opponent.z, partner.x, partner.z, second.x, second.z
                    }.Select(v => v is float f ? f.ToString("R", System.Globalization.CultureInfo.InvariantCulture)
                        : v is double d ? d.ToString("R", System.Globalization.CultureInfo.InvariantCulture)
                        : Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture))));
                    if (state == "Rally")
                    {
                        if (wasRally && hitter != priorPlayer) result.returns[hitter]++;
                        float end = player.forward.z > 0 ? 1 : -1;
                        bool incoming = hitter % 2 == 1;
                        bool owns = (int)type.GetField("nearOwner", Hidden).GetValue(court) == 0;
                        if (incoming && owns) result.humanOwnedTicks++;
                        Vector3 desired = incoming && owns ? shuttle.position - player.right * .3f - player.forward * .35f : Get<Vector3>("SuggestedPosition");
                        desired.z = Mathf.Min(desired.z * end, -1) * end;
                        Call("SetAnalog", Vector2.ClampMagnitude(new Vector2(desired.x - player.position.x, desired.z - player.position.z) * 4 * end, 1));
                        string shot = shuttle.position.y - player.position.y >= 1.45f ? (result.returns[0] % 4 == 3 ? "Smash" : "Clear") : "Lift";
                        if (incoming && (bool)driver.GetType().GetMethod("CanReach").Invoke(driver, new object[] { shuttle.position, shot }))
                        {
                            result.humanReachableTicks++;
                            Call("SetAnalog", new Vector2(result.returns[0] % 2 == 0 ? -1 : 1, 1)); Call("TapShot", shot);
                            if ((int)type.GetField("lastPlayer", Hidden).GetValue(court) == 0) result.returns[0]++;
                        }
                    }
                    else
                    {
                        Call("SetAnalog", Vector2.zero);
                        if (wasRally)
                        {
                            result.points++; string feedback = (string)type.GetField("feedback", Hidden).GetValue(court);
                            if (feedback.Contains("service area")) result.serviceFaults++;
                            if (feedback.Contains("net touch")) result.netFaults++;
                        }
                        if (state == "Serving" && flow == "Serving") Call("Serve", true);
                    }
                    priorPlayer = (int)type.GetField("lastPlayer", Hidden).GetValue(court); wasRally = state == "Rally";
                    yield return new WaitForFixedUpdate();
                }
                result.state = Get<string>("MatchState"); result.seconds = Time.time - began;
                results.Add(JsonUtility.ToJson(result)); Directory.CreateDirectory(CaptureEvidence);
                File.WriteAllLines(CaptureEvidence + (capture ? "/natural-doubles-normal2.jsonl" : "/natural-doubles8.jsonl"), results); measured.Add(result);
                if (capture && result.state == "MatchEnd")
                {
                    yield return new WaitForSecondsRealtime(2); Click("Menu");
                    Assert.That(Get<string>("ScreenName"), Is.EqualTo("Home")); yield return new WaitForSecondsRealtime(1);
                }
            }
            finally
            {
                Time.timeScale = 1;
                if (capture)
                {
                    if (recording != null) ((MonoBehaviour)court).StopCoroutine(recording);
                    audio.StopCapture(); timeline.Dispose(); timeline = null; recorder.StandardInput.Close();
                    Assert.That(recorder.WaitForExit(15000), Is.True, "Doubles encoder did not finish");
                    Assert.That(recorder.ExitCode, Is.Zero); recorder.Dispose();
                    File.WriteAllText(stem + "-capture.json", "{\"frames\":" + recordedFrames + ",\"seconds\":" + (Time.realtimeSinceStartup - captureStarted).ToString(System.Globalization.CultureInfo.InvariantCulture) + ",\"firstFrameSeconds\":" + firstRecordedFrame.ToString(System.Globalization.CultureInfo.InvariantCulture) + ",\"sampleRate\":" + AudioSettings.outputSampleRate + ",\"channels\":" + audio.Channels + ",\"timeScale\":1,\"controlled\":false}");
                    Assert.That(recordedFrames, Is.GreaterThan(100));
                    Assert.That(new FileInfo(stem + ".f32").Length, Is.GreaterThan(100000));
                }
            }
        }
        foreach (var result in measured)
        {
            Assert.That(result.state, Is.EqualTo("MatchEnd"), result.difficulty);
            Assert.That(result.seconds, Is.LessThan(1000), result.difficulty + " original 1000-second gate; extra time is diagnostic only");
            Assert.That(result.serviceFaults, Is.Zero, result.difficulty + " service faults");
            for (int id = 0; id < 4; id++) Assert.That(result.returns[id], Is.GreaterThan(1), result.difficulty + " athlete " + id + " never returns");
        }
    }
    [UnityTest, Timeout(600000)] public IEnumerator NineSeededMatchesUseOnlyLegalInputsAndAutomaticPointTimer()
    {
        var results = new List<string>();
        foreach (string difficulty in new[] { "Easy", "Normal", "Hard" })
        foreach (int seed in new[] { 37, 71, 109 })
        {
            LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
            yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
            type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
            court = UnityEngine.Object.FindFirstObjectByType(type);
            type.GetField("assisted", Hidden).SetValue(court, false);
            Click("Main"); Click(difficulty); Click("Mulai"); Call("ConfigureOpponent", difficulty, seed);
            type.GetProperty("ReplayMode").SetValue(court, 2);
            var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
            var driver = player.Find("AthleteVisual").GetComponent(AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.AthleteVisual")).First(t => t != null));
            var reachable = driver.GetType().GetMethod("CanReach");
            var result = new Result { difficulty = difficulty, seed = seed };
            Time.timeScale = 15;
            float began = Time.time; int previousHitter = -1, rallyHits = 0; bool previousRally = false;
            Vector3 previous = player.position;
            while (Get<string>("MatchState") != "MatchEnd" && Time.time - began < 900)
            {
                string flow = Get<string>("FlowStage");
                if (new[] { "Warmup", "Ready", "GameEnd", "Interval", "ChangeEnds" }.Contains(flow)) Click("Teruskan");
                string state = Get<string>("MatchState");
                bool rally = state == "Rally";
                int hitter = (int)type.GetField("lastHitter", Hidden).GetValue(court);
                if (rally)
                {
                    if (!previousRally) { result.rallies++; rallyHits = 0; previousHitter = hitter; }
                    else
                    {
                        result.movedMetres += Vector2.Distance(new Vector2(previous.x, previous.z), new Vector2(player.position.x, player.position.z));
                        if (hitter != previousHitter) { if (hitter == 1) result.opponentReturns++; rallyHits++; previousHitter = hitter; }
                    }
                    bool incoming = hitter == 1;
                    float end = player.forward.z > 0 ? 1 : -1;
                    var desired = incoming ? shuttle.position - player.right * .3f - player.forward * .35f : new Vector3(0, 0, -3.8f * end);
                    desired.z = Mathf.Min(desired.z * end, -1.0f) * end;
                    var offset = new Vector2(desired.x - player.position.x, desired.z - player.position.z);
                    Call("SetAnalog", Vector2.ClampMagnitude(offset * 4 * end, 1));
                    string shot = shuttle.position.y - player.position.y >= 1.45f ? (result.playerReturns % 4 == 3 ? "Smash" : "Clear") : "Lift";
                    if (incoming && (bool)reachable.Invoke(driver, new object[] { shuttle.position, shot }))
                    {
                        Call("SetAnalog", new Vector2(result.playerReturns % 2 == 0 ? -1 : 1, 1));
                        Call("TapShot", shot);
                        if ((int)type.GetField("lastHitter", Hidden).GetValue(court) == 0) result.playerReturns++;
                    }
                }
                else
                {
                    Call("SetAnalog", Vector2.zero);
                    if (previousRally)
                    {
                        result.points++; result.longestRally = Mathf.Max(result.longestRally, rallyHits);
                        string feedback = (string)type.GetField("feedback", Hidden).GetValue(court);
                        if (feedback.Contains("service area")) result.serviceFaults++;
                        if (feedback.Contains("net touch")) result.netFaults++;
                    }
                    if (state == "Serving" && flow == "Serving") Call("Serve", true);
                }
                previousRally = rally; previous = player.position;
                yield return new WaitForFixedUpdate();
            }
            Time.timeScale = 1;
            result.seconds = Time.time - began; result.state = Get<string>("MatchState");
            result.winner = (int)Get<object>("Match").GetType().GetField("Winner").GetValue(Get<object>("Match"));
            result.playerEnergy = Get<float>("Energy"); result.opponentEnergy = Get<float>("OpponentEnergy");
            string json = JsonUtility.ToJson(result); results.Add(json);
            File.WriteAllText(Evidence + "/match-" + difficulty + "-" + seed + ".json", json);
            Assert.That(result.state, Is.EqualTo("MatchEnd"), json);
            Assert.That(result.playerReturns, Is.GreaterThan(0), json);
            Assert.That(result.opponentReturns, Is.GreaterThan(0), json);
            Assert.That(result.movedMetres, Is.GreaterThan(15), json);
            Assert.That(result.serviceFaults, Is.LessThan(result.points), json);
            Assert.That(result.netFaults, Is.LessThan(result.points / 2), "Bot must not run into net every rally: " + json);
        }
        File.WriteAllLines(Evidence + "/nine-matches.jsonl", results);
    }
    [UnityTest, Timeout(120000), Category("Capture")] public IEnumerator CaptureTouchControlAndAimVerificationAt1x()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720();
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type);
        Call("ConfigureJoystick", "Fixed");
        Call("ConfigureJoystickSensitivity", .5f);
        type.GetField("assisted", Hidden).SetValue(court, false);
        type.GetProperty("ReplayMode").SetValue(court, 2);
        type.GetProperty("ShowTouchOverlay").SetValue(court, true);
        Time.timeScale = 1; Application.targetFrameRate = 60;
        Call("StartMatch");
        for (int i = 0; i < 3 && Get<string>("FlowStage") != "Serving"; i++) Call("AdvanceFlow");
        Call("Serve", false);
        type.GetField("serviceFlight", Hidden).SetValue(court, false);
        Directory.CreateDirectory(CaptureEvidence);
        string stem = CaptureEvidence + "/touch-control-aim";
        var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
        var down = type.GetMethod("PointerDown", Hidden);
        var move = type.GetMethod("PointerMove", Hidden);
        var up = type.GetMethod("PointerUp", Hidden);
        var stick = (Rect)type.GetProperty("StickRect", Hidden).GetValue(court);
        var lobRect = (Rect)type.GetMethod("ShotRect", Hidden).Invoke(court, new object[] { 1 });

        var recorder = new System.Diagnostics.Process();
        recorder.StartInfo = new System.Diagnostics.ProcessStartInfo("ffmpeg", "-hide_banner -loglevel error -n -use_wallclock_as_timestamps 1 -f rawvideo -pixel_format rgba -video_size " + Screen.width + "x" + Screen.height + " -framerate 1000 -i pipe:0 -fps_mode passthrough -enc_time_base 1:1000 -vf vflip,pad=ceil(iw/2)*2:ceil(ih/2)*2 -an -c:v libx264 -preset ultrafast -crf 20 -pix_fmt yuv420p -movflags +faststart \"" + stem + "-video.mp4\"") { UseShellExecute = false, RedirectStandardInput = true, CreateNoWindow = true };
        recorder.Start();
        captureStarted = Time.realtimeSinceStartup; captureGameStarted = Time.time; firstRecordedFrame = 0;
        timeline = new StreamWriter(stem + "-timeline.csv");
        timeline.WriteLine("realSeconds,gameSeconds,state,hitter,stroke,strokeElapsed,playerX,playerY,playerZ,shuttleX,shuttleY,shuttleZ,screen,flow");
        var events = new List<string> { "realSeconds,scenario" }; int frames = 0;
        void Mark(string label) => events.Add((Time.realtimeSinceStartup - captureStarted).ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + label);
        void ParkShuttle()
        {
            type.GetField("lastHitter", Hidden).SetValue(court, 1);
            type.GetField("lastPlayer", Hidden).SetValue(court, 1);
            type.GetField("flying", Hidden).SetValue(court, true);
            type.GetField("velocity", Hidden).SetValue(court, Vector3.zero);
            shuttle.position = new Vector3(0, 5.5f, 3f);
        }
        IEnumerator HoldRecord(float seconds, bool keepParked)
        {
            float until = Time.realtimeSinceStartup + seconds, next = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < until)
            {
                if (keepParked) ParkShuttle();
                yield return new WaitForEndOfFrame();
                if (Time.realtimeSinceStartup < next) continue;
                WriteFrame(recorder); frames++; next += 1f / 30;
            }
        }
        try
        {
            player.position = new Vector3(0, 0, -4f); ParkShuttle();
            yield return HoldRecord(.35f, true);

            // 1. Touch empty left space -> athlete remains still.
            Vector3 startPos = player.position;
            Vector2 emptyLeft = stick.center + new Vector2(210f, -120f);
            Mark("touch-empty-left-start");
            down.Invoke(court, new object[] { 10, emptyLeft });
            yield return HoldRecord(.65f, true);
            up.Invoke(court, new object[] { 10 });
            Assert.That(Vector3.Distance(startPos, player.position), Is.LessThan(.001f), "Empty space touch moved athlete");
            Mark("touch-empty-left-end");
            yield return HoldRecord(.25f, true);

            // 2. Run left -> release stick -> tap Lob -> shuttle lands center.
            Mark("run-left-start");
            down.Invoke(court, new object[] { 11, stick.center });
            move.Invoke(court, new object[] { 11, stick.center + Vector2.left * 70f });
            yield return HoldRecord(.50f, true);
            up.Invoke(court, new object[] { 11 });
            Mark("release-stick-tap-lob-center");
            yield return HoldRecord(.18f, true);
            ParkShuttle();
            shuttle.position = player.TransformPoint(new Vector3(.3f, 2.1f, .35f));
            down.Invoke(court, new object[] { 12, lobRect.center });
            yield return HoldRecord(.15f, false);
            up.Invoke(court, new object[] { 12 });
            Vector3 landCenter = ((Transform)type.GetField("landingShadow", Hidden).GetValue(court)).position;
            Assert.That(Mathf.Abs(landCenter.x), Is.LessThan(.15f), "Released stick Lob did not target center");
            yield return HoldRecord(.95f, false);

            // 3. Hold left -> tap Lob with second finger -> shuttle lands left.
            player.position = new Vector3(.6f, 0, -4f); ParkShuttle();
            yield return HoldRecord(.25f, true);
            Mark("hold-left-and-lob-left-start");
            down.Invoke(court, new object[] { 13, stick.center });
            move.Invoke(court, new object[] { 13, stick.center + Vector2.left * 70f });
            yield return HoldRecord(.35f, true);
            ParkShuttle();
            shuttle.position = player.TransformPoint(new Vector3(.3f, 2.1f, .35f));
            down.Invoke(court, new object[] { 14, lobRect.center });
            yield return HoldRecord(.15f, false);
            up.Invoke(court, new object[] { 14 });
            Vector3 landLeft = ((Transform)type.GetField("landingShadow", Hidden).GetValue(court)).position;
            Assert.That(landLeft.x, Is.LessThan(-1.5f), "Held left Lob did not target left");
            yield return HoldRecord(.85f, false);
            up.Invoke(court, new object[] { 13 });
            Mark("sequence-end");
            yield return HoldRecord(.25f, false);
        }
        finally
        {
            type.GetProperty("ShowTouchOverlay").SetValue(court, false);
            timeline.Dispose(); timeline = null; recorder.StandardInput.Close();
            Assert.That(recorder.WaitForExit(15000), Is.True); Assert.That(recorder.ExitCode, Is.Zero); recorder.Dispose();
            File.WriteAllLines(stem + "-events.csv", events);
            File.WriteAllText(stem + "-capture.json", "{\"frames\":" + frames + ",\"controlled\":true,\"timeScale\":1,\"touchOverlay\":true}");
        }
    }
    [UnityTest, Timeout(120000)] public IEnumerator SideAndBackShuttlePlaytestDiagnostics()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type);
        Call("EnterTraining");
        Call("ConfigureJoystick", "Fixed");
        Call("ConfigureJoystickSensitivity", .5f);
        type.GetField("assisted", Hidden).SetValue(court, false);
        type.GetProperty("ReplayMode").SetValue(court, 2);

        var player = GameObject.Find("Player").transform;
        var shuttle = GameObject.Find("Shuttle").transform;
        var driver = player.Find("AthleteVisual").GetComponent(AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.AthleteVisual")).First(t => t != null));
        float armLength = (float)driver.GetType().GetField("armLength", Hidden).GetValue(driver);
        float reachLimit = armLength + .38f - .012f;
        var reachFailure = driver.GetType().GetMethod("ReachFailure");
        var down = type.GetMethod("PointerDown", Hidden);
        var move = type.GetMethod("PointerMove", Hidden);
        var up = type.GetMethod("PointerUp", Hidden);
        var stick = (Rect)type.GetProperty("StickRect", Hidden).GetValue(court);
        var tick = type.GetMethod("FixedUpdate", Hidden);

        var rows = new List<string> { "category,scenario,stroke,playerX,playerZ,shuttleX,shuttleY,shuttleZ,localX,localY,localZ,horizDist,shoulderDist,reachLimit,hit,reason,classification" };
        int totalProbes = 0, probeHits = 0, probeMiss = 0, liveRuns = 0, liveHits = 0, posLate = 0, boundaryBlock = 0;

        void RecordRow(string category, string scenario, string strokeName, bool hit, string reason, string classification)
        {
            Vector3 local = player.InverseTransformPoint(shuttle.position);
            bool high = strokeName == "Smash" || strokeName == "Clear" || strokeName == "Drop";
            Vector3 shoulder = player.TransformPoint(new Vector3(.183f, high ? 1.46f : 1.34f, -.018f));
            float shoulderDist = Vector3.Distance(shuttle.position, shoulder);
            float horiz = new Vector2(local.x, local.z).magnitude;
            rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "{0},{1},{2},{3:F3},{4:F3},{5:F3},{6:F3},{7:F3},{8:F3},{9:F3},{10:F3},{11:F3},{12:F3},{13:F3},{14},{15},{16}",
                category, scenario, strokeName, player.position.x, player.position.z,
                shuttle.position.x, shuttle.position.y, shuttle.position.z,
                local.x, local.y, local.z, horiz, shoulderDist, reachLimit, hit,
                reason.Replace(",", ";"), classification));
        }

        // A. Side, backhand, slightly-behind, far-behind, and floor contact boundary probes at BOTH ends.
        var probes = new (string name, string stroke, Vector3 localOffset, bool expectHit, string expectReason)[] {
            ("side-forehand-near", "Lift", new Vector3(.65f, 1.25f, .20f), true, "OK"),
            ("side-backhand-near", "Lift", new Vector3(-.65f, 1.25f, .20f), true, "OK"),
            ("side-forehand-wide", "Lift", new Vector3(.85f, 1.25f, .20f), true, "OK"),
            ("side-backhand-wide", "Lift", new Vector3(-.85f, 1.25f, .20f), true, "OK"),
            ("side-forehand-high", "Clear", new Vector3(.75f, 2.05f, .20f), true, "OK"),
            ("side-backhand-high", "Clear", new Vector3(-.75f, 2.05f, .20f), true, "OK"),
            ("back-center-high", "Clear", new Vector3(.20f, 2.05f, -.50f), true, "OK"),
            ("back-left-high", "Clear", new Vector3(-.55f, 2.05f, -.50f), true, "OK"),
            ("back-center-low", "Lift", new Vector3(.20f, 1.20f, -.55f), true, "OK"),
            ("back-low-crouch-048", "Lift", new Vector3(-.20f, .48f, -.20f), true, "OK"),
            ("back-far-out", "Clear", new Vector3(0f, 2.05f, -1.35f), false, "Di luar jangkauan"),
            ("floor-too-low", "Lift", new Vector3(.20f, .04f, .20f), false, "Kok terlalu rendah"),
            ("height-gap-clear-140", "Clear", new Vector3(.30f, 1.40f, .35f), false, "Kok terlalu rendah"),
            ("height-gap-lift-195", "Lift", new Vector3(.30f, 1.95f, .35f), false, "Kok terlalu tinggi")
        };
        var bed = (Transform)driver.GetType().GetProperty("StringBed").GetValue(driver);
        foreach (bool swapped in new[] { false, true })
        {
            float endSign = swapped ? -1f : 1f;
            foreach (var p in probes)
            {
                Call("Feed", "Lob");
                player.SetPositionAndRotation(new Vector3(0, 0, -3.5f * endSign), Quaternion.Euler(0, swapped ? 180f : 0f, 0));
                type.GetField("velocity", Hidden).SetValue(court, Vector3.zero);
                shuttle.position = player.TransformPoint(p.localOffset);
                string fail = (string)reachFailure.Invoke(driver, new object[] { shuttle.position, p.stroke });
                Call("BeginShot", p.stroke);
                bool hit = (bool)Call("ReleaseShot");
                float bedGap = Vector3.Distance(bed.position, shuttle.position);
                Assert.That(hit, Is.EqualTo(p.expectHit), (swapped ? "Swapped/" : "Near/") + p.name + ": " + fail);
                if (p.expectHit) Assert.That(bedGap, Is.LessThan(.035f), p.name + " StringBed gap");
                else Assert.That(fail, Does.Contain(p.expectReason), p.name);
                if (!swapped)
                {
                    string cls = hit ? "HIT_OK" : (fail.Contains("tinggi") || fail.Contains("rendah") ? "BOUNDARY_HEIGHT_WINDOW" : "BOUNDARY_REACH_RADIUS");
                    RecordRow("boundary-probe", p.name, p.stroke, hit, hit ? $"OK|gap={bedGap:F3}" : fail, cls);
                    totalProbes++; if (hit) probeHits++; else probeMiss++;
                }
            }
        }

        // B. Live approach playtest with Fixed joystick + TapShot across side & back trajectories.
        var approaches = new (string name, string stroke, Vector3 targetLanding, float arcDuration)[] {
            ("live-side-right-mid", "Lift", new Vector3(2.2f, .04f, -3.5f), 1.45f),
            ("live-side-left-mid", "Lift", new Vector3(-2.2f, .04f, -3.5f), 1.45f),
            ("live-side-left-fast", "Lift", new Vector3(-2.3f, .04f, -3.5f), 1.15f),
            ("live-back-center-deep", "Clear", new Vector3(0f, .04f, -6.1f), 1.55f),
            ("live-back-left-deep", "Clear", new Vector3(-1.9f, .04f, -6.0f), 1.55f),
            ("live-back-right-deep", "Clear", new Vector3(1.9f, .04f, -6.0f), 1.55f),
            ("live-back-left-fast", "Clear", new Vector3(-2.1f, .04f, -6.2f), 1.25f),
            ("live-front-side-left", "Net", new Vector3(-1.9f, .04f, -1.5f), 1.35f)
        };
        var launchVel = type.GetMethod("LaunchVelocity");
        foreach (var a in approaches)
        {
            Call("StartMatch");
            for (int i = 0; i < 3 && Get<string>("FlowStage") != "Serving"; i++) Call("AdvanceFlow");
            Call("Serve", true);
            type.GetMethod("ResetAthleteMovement", Hidden).Invoke(court, null);
            player.position = new Vector3(0, 0, -3.5f);
            type.GetField("serviceFlight", Hidden).SetValue(court, false);
            type.GetField("lastHitter", Hidden).SetValue(court, 1);
            type.GetField("lastPlayer", Hidden).SetValue(court, 1);
            shuttle.position = new Vector3(0, 2.2f, 4.0f);
            Vector3 v = (Vector3)launchVel.Invoke(null, new object[] { shuttle.position, a.targetLanding, a.arcDuration });
            type.GetField("velocity", Hidden).SetValue(court, v);

            down.Invoke(court, new object[] { 40, stick.center });
            bool hit = false;
            string bestCloseReason = "Belum dalam jangkauan";
            float minHoriz = float.MaxValue;
            Vector3 closestShuttle = shuttle.position, closestPlayer = player.position;

            for (int step = 0; step < 120 && Get<bool>("Flying") && (int)type.GetField("lastHitter", Hidden).GetValue(court) == 1; step++)
            {
                Vector3 toTarget = a.targetLanding - player.position;
                Vector2 dir = new Vector2(toTarget.x, toTarget.z);
                if (dir.magnitude > .22f)
                {
                    Vector2 stickDir = dir.normalized * new Vector2(1f, -1f);
                    move.Invoke(court, new object[] { 40, stick.center + stickDir * 70f });
                }
                else
                    move.Invoke(court, new object[] { 40, stick.center });

                tick.Invoke(court, null);
                float hDist = Vector2.Distance(new Vector2(player.position.x, player.position.z), new Vector2(shuttle.position.x, shuttle.position.z));
                string chosen = shuttle.position.y >= 1.45f ? a.stroke : (a.stroke == "Clear" ? "Lift" : a.stroke);
                string reasonNow = (string)reachFailure.Invoke(driver, new object[] { shuttle.position, chosen });
                if (hDist < minHoriz) { minHoriz = hDist; closestShuttle = shuttle.position; closestPlayer = player.position; bestCloseReason = reasonNow.Length == 0 ? "OK" : reasonNow; }

                if (reasonNow.Length == 0 && (int)type.GetField("lastHitter", Hidden).GetValue(court) == 1)
                {
                    Call("TapShot", chosen);
                    if ((int)type.GetField("lastHitter", Hidden).GetValue(court) == 0) { hit = true; bestCloseReason = (string)type.GetField("feedback", Hidden).GetValue(court); break; }
                }
            }
            up.Invoke(court, new object[] { 40 });
            if (!hit) { player.position = closestPlayer; shuttle.position = closestShuttle; }
            string cls = hit ? "HIT_OK" : (minHoriz > 0.85f ? "POSITION_TOO_SLOW_OR_FAR" : "CONTACT_BOUNDARY_AT_INTERCEPT");
            RecordRow("live-approach", a.name, a.stroke, hit, bestCloseReason, cls);
            liveRuns++; if (hit) liveHits++; else if (cls == "POSITION_TOO_SLOW_OR_FAR") posLate++; else boundaryBlock++;
        }

        File.WriteAllLines(Path.Combine(EvidenceDirectory, "side-back-playtest.csv"), rows);
        string summary = string.Format(System.Globalization.CultureInfo.InvariantCulture,
            "{{\"armLength\":{0:F4},\"reachLimit\":{1:F4},\"probes\":{2},\"probeHits\":{3},\"probeMisses\":{4},\"liveRuns\":{5},\"liveHits\":{6},\"positionFailures\":{7},\"boundaryFailures\":{8}}}",
            armLength, reachLimit, totalProbes, probeHits, probeMiss, liveRuns, liveHits, posLate, boundaryBlock);
        File.WriteAllText(Path.Combine(EvidenceDirectory, "side-back-summary.json"), summary);
        TestContext.WriteLine("SIDE_BACK_SUMMARY: " + summary);
        Assert.That(totalProbes, Is.EqualTo(14));
        Assert.That(liveRuns, Is.EqualTo(8));
        Assert.That(liveHits, Is.EqualTo(8), "All 8 reachable live side & back trajectories should save cleanly");
    }
    [UnityTest, Timeout(120000), Category("Capture")] public IEnumerator CaptureAnimationAndGripSequenceAt1xAndSideSlowMo()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720();
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type);
        Call("EnterTraining");
        ((Behaviour)court).enabled = false;
        Directory.CreateDirectory(CaptureEvidence);
        string stem = CaptureEvidence + "/anim-grip-sequence";
        var player = GameObject.Find("Player").transform;
        var model = player.Find("AthleteVisual");
        var driver = model.GetComponents<MonoBehaviour>().Single(t => t.GetType().Name == "AthleteVisual");
        var present = driver.GetType().GetMethod("Present");
        var hips = model.Find("Root/Hips");
        var lThigh = hips.Find("LeftUpLeg"); var lKnee = lThigh.Find("LeftLeg"); var lFoot = lKnee.Find("LeftFoot");
        var rThigh = hips.Find("RightUpLeg"); var rKnee = rThigh.Find("RightLeg"); var rFoot = rKnee.Find("RightFoot");
        var rHand = model.Find("Root/Hips/Spine/Spine1/RightShoulder/RightArm/RightForeArm/RightHand");
        var socket = rHand.Find("RightHandProp");
        var grip = (Transform)driver.GetType().GetProperty("Grip").GetValue(driver);
        var lHand = model.Find("Root/Hips/Spine/Spine1/LeftShoulder/LeftArm/LeftForeArm/LeftHand");

        // Verify biomechanical posture invariants before filming.
        present.Invoke(driver, new object[] { "Menu", 1f, Vector3.zero, Vector3.zero, 0f, 0f });
        float menuHipY = hips.position.y;
        present.Invoke(driver, new object[] { "Ready", 1f, Vector3.zero, Vector3.zero, 0f, 0f });
        float readyHipY = hips.position.y;
        Assert.That(readyHipY, Is.LessThan(menuHipY - .05f), "Ready pose must lower center of mass");
        float kneeAngle = Vector3.Angle(rThigh.position - rKnee.position, rFoot.position - rKnee.position);
        Assert.That(kneeAngle, Is.LessThan(165f), "Ready pose knees must bend under load");
        Assert.That(rFoot.position.y, Is.EqualTo(.13f).Within(.015f));
        Assert.That(Vector3.Distance(grip.position, socket.position), Is.LessThan(.005f), "Grip must stay in palm");
        Vector3 readyLeftHand = lHand.position;
        present.Invoke(driver, new object[] { "Net", .18f, player.TransformPoint(new Vector3(.45f, 1.0f, .35f)), Vector3.zero, 0f, 0f });
        Assert.That(Vector3.Distance(lHand.position, readyLeftHand), Is.GreaterThan(.15f), "Free arm must counterbalance lunge");

        var recorder = new System.Diagnostics.Process();
        recorder.StartInfo = new System.Diagnostics.ProcessStartInfo("ffmpeg", "-hide_banner -loglevel error -n -use_wallclock_as_timestamps 1 -f rawvideo -pixel_format rgba -video_size " + Screen.width + "x" + Screen.height + " -framerate 1000 -i pipe:0 -fps_mode passthrough -enc_time_base 1:1000 -vf vflip,pad=ceil(iw/2)*2:ceil(ih/2)*2 -an -c:v libx264 -preset ultrafast -crf 20 -pix_fmt yuv420p -movflags +faststart \"" + stem + "-video.mp4\"") { UseShellExecute = false, RedirectStandardInput = true, CreateNoWindow = true };
        recorder.Start();
        captureStarted = Time.realtimeSinceStartup; captureGameStarted = Time.time; firstRecordedFrame = 0;
        timeline = new StreamWriter(stem + "-timeline.csv");
        timeline.WriteLine("realSeconds,gameSeconds,state,hitter,stroke,strokeElapsed,playerX,playerY,playerZ,shuttleX,shuttleY,shuttleZ,screen,flow");
        int frames = 0;

        IEnumerator RunSequence(float timeRate, bool sideCam)
        {
            player.SetPositionAndRotation(new Vector3(0, 0, -3.5f), Quaternion.identity);
            driver.GetType().GetMethod("SetTravel").Invoke(driver, new object[] { 0f });
            for (int i = 0; i < 6; i++) present.Invoke(driver, new object[] { "Ready", 1f, Vector3.zero, Vector3.zero, 0f, 0f });
            void SetCam()
            {
                if (sideCam)
                {
                    Camera.main.fieldOfView = 44;
                    Camera.main.transform.position = player.position + new Vector3(3.5f, 1.18f, .1f);
                    Camera.main.transform.LookAt(player.position + Vector3.up * 1.15f);
                }
                else
                {
                    Camera.main.fieldOfView = 48;
                    Camera.main.transform.position = new Vector3(.2f, 4f, -14.2f);
                    Camera.main.transform.LookAt(new Vector3(.2f, 0, -1f));
                }
            }
            // 1. Ready (0.4s)
            for (float t = 0; t < .4f; t += 1f / 30f * timeRate)
            {
                present.Invoke(driver, new object[] { "Ready", 1f + t, Vector3.zero, Vector3.zero, 0f, 0f });
                SetCam(); yield return new WaitForEndOfFrame(); WriteFrame(recorder); frames++;
            }
            // 2. Shuffle / move right + forward (0.6s)
            for (float t = 0; t < .6f; t += 1f / 30f * timeRate)
            {
                Vector3 step = new Vector3(.65f, 0, .8f) * (1f / 30f * timeRate / .6f);
                player.position += step;
                present.Invoke(driver, new object[] { "Ready", 1f, Vector3.zero, step, 0f, 0f });
                SetCam(); yield return new WaitForEndOfFrame(); WriteFrame(recorder); frames++;
            }
            // 3. Low Lunge + Recovery (0.70s)
            Vector3 lowContact = player.TransformPoint(new Vector3(.48f, .95f, .38f));
            for (float t = 0; t < .70f; t += 1f / 30f * timeRate)
            {
                present.Invoke(driver, new object[] { "Net", t, lowContact, Vector3.zero, 0f, 0f });
                SetCam(); yield return new WaitForEndOfFrame(); WriteFrame(recorder); frames++;
            }
            // 4. Overhead Clear/Smash + Recovery (0.75s)
            Vector3 highContact = player.TransformPoint(new Vector3(.30f, 2.25f, .25f));
            for (float t = 0; t < .75f; t += 1f / 30f * timeRate)
            {
                present.Invoke(driver, new object[] { "Smash", t, highContact, Vector3.zero, 0f, 0f });
                SetCam(); yield return new WaitForEndOfFrame(); WriteFrame(recorder); frames++;
            }
        }
        try
        {
            yield return RunSequence(1f, false); // Part 1: 1x main camera
            yield return RunSequence(.35f, true); // Part 2: 0.35x slow-mo side camera
        }
        finally
        {
            timeline.Dispose(); timeline = null; recorder.StandardInput.Close();
            Assert.That(recorder.WaitForExit(15000), Is.True); Assert.That(recorder.ExitCode, Is.Zero); recorder.Dispose();
            File.WriteAllText(stem + "-capture.json", "{\"frames\":" + frames + ",\"main1x\":true,\"sideSlowMo035x\":true}");
        }
    }
}

[DefaultExecutionOrder(32000)]
public sealed class PilotAuditCamera : MonoBehaviour
{
    public Transform subject;
    void LateUpdate()
    {
        var position = subject.position + new Vector3(3, 1.6f, 1);
        position.z = Mathf.Sign(subject.position.z) * Mathf.Max(.75f, Mathf.Abs(position.z));
        transform.position = position;
        transform.LookAt(subject.position + Vector3.up);
    }
}
