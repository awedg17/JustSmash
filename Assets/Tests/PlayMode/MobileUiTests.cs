using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MobileUiTests : PreferenceFixture
{
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    Type type; UnityEngine.Object court;
    object Call(string method, params object[] args) => type.GetMethod(method).Invoke(court, args);
    T Get<T>(string name) => (T)type.GetProperty(name).GetValue(court);
    void Click(string action)
    {
        var rect = (Rect)type.GetMethod("UiActionRect").Invoke(court, new object[] { action });
        Assert.That(rect.width, Is.GreaterThanOrEqualTo(48), action + " on " + Get<string>("ScreenName"));
        type.GetMethod("PointerDown", Hidden).Invoke(court, new object[] { 81, rect.center });
        type.GetMethod("PointerUp", Hidden).Invoke(court, new object[] { 81 });
    }
    IEnumerator Boot()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type);
    }
    [UnityTest, Category("Capture")] public IEnumerator FirstMatchHudAvoidsColdGlyphFrameHitch()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720(); yield return Boot();
        yield return new WaitForSecondsRealtime(.5f);
        Click("Main"); Click("Tunggal"); Click("Normal");
        yield return new WaitForSecondsRealtime(.5f);
        Click("Mulai");
        var rows = new System.Collections.Generic.List<string> { "frame,wall,frameStart,elapsedSeconds,state,screen" };
        double peak = 0;
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForEndOfFrame();
            double wall = Time.realtimeSinceStartupAsDouble, start = Time.unscaledTimeAsDouble;
            peak = Math.Max(peak, wall - start);
            rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1:R},{2:R},{3:R},{4},{5}", Time.frameCount, wall, start, wall - start, Get<string>("MatchState"), Get<string>("ScreenName")));
            Assert.That(Get<string>("ScreenName"), Is.EqualTo("Playing"));
            if (i < 2) yield return null;
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/first-hud-clocks.csv", rows);
        // Measurement ended; freeze simulation, keep OnGUI enabled for controlled HUD evidence.
        Time.timeScale = 0;
        Camera.main.transform.SetPositionAndRotation(new Vector3(-1.1f, 1.5f, -9.6f), new Quaternion(.0533425063f, 0, 0, .9985762835f));
        yield return new WaitForEndOfFrame();
        var image = ScreenCapture.CaptureScreenshotAsTexture();
        try { System.IO.File.WriteAllBytes(EvidenceDirectory + "/first-hud.png", image.EncodeToPNG()); }
        finally { UnityEngine.Object.Destroy(image); }
        System.IO.File.WriteAllText(EvidenceDirectory + "/camera.json", "{\"position\":" + JsonUtility.ToJson(Camera.main.transform.position) + ",\"rotation\":" + JsonUtility.ToJson(Camera.main.transform.rotation) + ",\"fieldOfView\":" + Camera.main.fieldOfView + "}");
        Assert.That(peak, Is.LessThanOrEqualTo(.05), "First match HUD must not stall a frame beyond strict 50ms gate; no recorder present");
    }
    [UnityTest, Category("Capture")] public IEnumerator CaptureRallyHudAcrossFormatsAndEnds()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720(); yield return Boot();
        var rows = new System.Collections.Generic.List<string> { "format,end,state,file,width,height,actors,fieldOfView" };
        var guides = new System.Collections.Generic.List<string> { "file,control,cyanInterior,cyanVisible" };
        int obstructed = 0;
        foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("BeginExhibition");
            var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped);
            Call("NextPoint");
            for (int i = 0; i < 3 && Get<string>("FlowStage") != "Serving"; i++) Call("AdvanceFlow");
            Assert.That(Get<string>("FlowStage"), Is.EqualTo("Serving"));
            type.GetProperty("ReplayMode").SetValue(court, 2); Call("Serve", true);
            Assert.That(Get<string>("MatchState"), Is.EqualTo("Rally"));
            yield return new WaitForSeconds(.12f);
            foreach (bool held in new[] { false, true })
            {
                Rect jump = (Rect)type.GetProperty("JumpRect", Hidden).GetValue(court);
                if (held) type.GetMethod("PointerDown", Hidden).Invoke(court, new object[] { 93, jump.center });
                yield return new WaitForEndOfFrame();
                Assert.That(Get<string>("ScreenName"), Is.EqualTo("Playing"));
                Assert.That(Get<string>("MatchState"), Is.EqualTo("Rally"));
                Assert.That(type.GetField("jumpHeld", Hidden).GetValue(court), Is.EqualTo(held));
                string file = (doubles ? "doubles" : "singles") + (swapped ? "-far" : "-near") + (held ? "-jump-held" : "-rally") + ".png";
                var image = ScreenCapture.CaptureScreenshotAsTexture();
                try
                {
                    System.IO.File.WriteAllBytes(EvidenceDirectory + "/" + file, image.EncodeToPNG());
                    if (held)
                    {
                        var pixels = image.GetPixels32();
                        float scale = (float)type.GetField("uiScale", Hidden).GetValue(court);
                        Vector2 origin = (Vector2)type.GetProperty("UiOrigin", Hidden).GetValue(court);
                        int Cyan(Rect rect)
                        {
                            int count = 0;
                            for (int y = Mathf.CeilToInt(origin.y + rect.yMin * scale); y < origin.y + rect.yMax * scale; y++)
                            for (int x = Mathf.CeilToInt(origin.x + rect.xMin * scale); x < origin.x + rect.xMax * scale; x++)
                            {
                                Color32 color = pixels[(image.height - 1 - y) * image.width + x];
                                if (color.r < 80 && color.g > 160 && color.b > 160) count++;
                            }
                            return count;
                        }
                        float width = (float)type.GetProperty("UiWidth", Hidden).GetValue(court), height = (float)type.GetProperty("UiHeight", Hidden).GetValue(court);
                        int visible = Cyan(new Rect(width - 324, height - 310, 292, 274));
                        Assert.That(visible, Is.GreaterThan(25), "Held guide disappeared instead of staying behind labels");
                        for (int i = 0; i < 6; i++)
                        {
                            Rect rect = i == 5 ? jump : (Rect)type.GetMethod("ShotRect", Hidden).Invoke(court, new object[] { i });
                            int covered = Cyan(new Rect(rect.x + 15, rect.y + 15, rect.width - 30, rect.height - 30));
                            guides.Add(string.Join(",", file, i, covered, visible)); obstructed += covered;
                        }
                    }
                }
                finally { UnityEngine.Object.Destroy(image); }
                rows.Add(string.Join(",", doubles ? "doubles" : "singles", swapped ? "far" : "near", held ? "jump-held" : "rally", file, Screen.width, Screen.height, doubles ? 4 : 2, Camera.main.fieldOfView));
                if (held) type.GetMethod("PointerCancel", Hidden).Invoke(court, new object[] { 93 });
            }
        }
        Assert.That(rows.Count - 1, Is.EqualTo(8)); System.IO.File.WriteAllLines(EvidenceDirectory + "/hud-preview.csv", rows);
        System.IO.File.WriteAllLines(EvidenceDirectory + "/guide-pixels.csv", guides);
        Assert.That(guides.Count - 1, Is.EqualTo(24)); Assert.That(obstructed, Is.Zero, "Held guide crosses control labels");
    }
    [UnityTest, Category("Capture")] public IEnumerator CaptureCompactScoreboardAcrossFormatsEndsAndScores()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720(); yield return Boot();
        var rows = new System.Collections.Generic.List<string> { "width,height,format,end,state,game,points0,points1,games0,games1,panelHeight,contrast,file" };
        var failures = new System.Collections.Generic.List<string>();
        foreach (Vector2Int size in new[] { new Vector2Int(1280,720), new Vector2Int(1280,800), new Vector2Int(1920,1080), new Vector2Int(2400,1080) })
        {
            yield return CaptureAtResolution(size.x, size.y);
            foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
            foreach (string state in new[] { "serving", "deuce", "game3", "rally" })
            {
                Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("BeginExhibition");
                var match = Get<object>("Match"); var mt = match.GetType();
                void Award(int side) => mt.GetMethod("AwardPoint").Invoke(match, new object[] { side });
                if (state == "game3") { for (int i = 0; i < 15; i++) Award(0); for (int i = 0; i < 15; i++) Award(1); }
                if (state == "deuce") for (int i = 0; i < 20; i++) { Award(0); Award(1); }
                if (state == "rally") { Award(1); Award(0); Award(0); }
                mt.GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
                for (int i = 0; i < 5 && Get<string>("FlowStage") != "Serving"; i++) Call("AdvanceFlow");
                Assert.That(Get<string>("FlowStage"), Is.EqualTo("Serving"));
                type.GetProperty("ReplayMode").SetValue(court, 2);
                if (state == "rally") { Call("Serve", true); type.GetField("flying", Hidden).SetValue(court, false); }
                type.GetField("serveReadyAt", Hidden).SetValue(court, Time.time + 100);
                yield return new WaitForSeconds(.12f); yield return new WaitForEndOfFrame();
                int game = (int)mt.GetField("GameNumber").GetValue(match);
                var points = (int[])mt.GetField("Points").GetValue(match); var games = (int[])mt.GetField("Games").GetValue(match);
                Assert.That(points, Is.EqualTo(state == "deuce" ? new[] { 20,20 } : state == "rally" ? new[] { 2,1 } : new[] { 0,0 }));
                Assert.That(games, Is.EqualTo(state == "game3" ? new[] { 1,1 } : new[] { 0,0 }));
                string key = size.x + "x" + size.y + "-" + (doubles ? "doubles" : "singles") + (swapped ? "-far-" : "-near-") + state;
                var image = ScreenCapture.CaptureScreenshotAsTexture();
                try
                {
                    System.IO.File.WriteAllBytes(EvidenceDirectory + "/" + key + ".png", image.EncodeToPNG());
                    float scale = (float)type.GetField("uiScale", Hidden).GetValue(court);
                    Vector2 origin = (Vector2)type.GetProperty("UiOrigin", Hidden).GetValue(court);
                    var pixels = image.GetPixels32();
                    Color32 Pixel(float x, float y) => pixels[(image.height - 1 - Mathf.RoundToInt(origin.y + y * scale)) * image.width + Mathf.RoundToInt(origin.x + x * scale)];
                    Color32 background = Pixel(30, 40), foreground = (Color32)((GUIStyle)type.GetField("smallStyle", Hidden).GetValue(court)).normal.textColor;
                    bool Same(Color32 a, Color32 b) => Math.Abs(a.r-b.r) <= 1 && Math.Abs(a.g-b.g) <= 1 && Math.Abs(a.b-b.b) <= 1;
                    int y = Mathf.RoundToInt(origin.y + 40 * scale), x = Mathf.RoundToInt(origin.x + 30 * scale);
                    while (y < origin.y + 220 * scale && Same(pixels[(image.height - 1 - y) * image.width + x], background)) y++;
                    float panelHeight = (y - origin.y) / scale - 24;
                    float Linear(float c) => c <= .04045f ? c / 12.92f : Mathf.Pow((c + .055f) / 1.055f, 2.4f);
                    float Luminance(Color32 c) => .2126f * Linear(c.r / 255f) + .7152f * Linear(c.g / 255f) + .0722f * Linear(c.b / 255f);
                    float a = Luminance(background), b = Luminance(foreground), contrast = (Mathf.Max(a,b) + .05f) / (Mathf.Min(a,b) + .05f);
                    rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10:R},{11:R},{12}.png", size.x,size.y,doubles ? "doubles" : "singles",swapped ? "far" : "near",state,game,points[0],points[1],games[0],games[1],panelHeight,contrast,key));
                    if (panelHeight > 133) failures.Add(key + "/panel height " + panelHeight + " exceeds compact 132+1 raster tolerance");
                    if (contrast < 4.5f) failures.Add(key + "/text contrast " + contrast + " below 4.5");
                    var header = type.GetProperty("HudScoreHeader", Hidden);
                    if (header == null || (string)header.GetValue(court) != "GAME " + game + " / " + Get<string>("Difficulty")) failures.Add(key + "/explicit game header missing");
                }
                finally { UnityEngine.Object.Destroy(image); }
            }
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/scoreboard.csv", rows);
        System.IO.File.WriteAllLines(EvidenceDirectory + "/scoreboard-failures.txt", failures);
        Assert.That(rows.Count - 1, Is.EqualTo(64)); Assert.That(failures, Is.Empty, string.Join("; ", failures.Take(5)));
    }
    [UnityTest, Category("Capture")] public IEnumerator CaptureHudEnergyAndNoticeContrast()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720(); yield return Boot();
        var rows = new System.Collections.Generic.List<string> { "width,height,format,end,state,energy,region,glyphPixels,minContrast,file" };
        var failures = new System.Collections.Generic.List<string>();
        foreach (Vector2Int size in new[] { new Vector2Int(1280,720), new Vector2Int(1280,800), new Vector2Int(1920,1080), new Vector2Int(2400,1080) })
        {
            yield return CaptureAtResolution(size.x, size.y);
            foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
            foreach (string state in new[] { "serve", "rally", "point", "fault" })
            {
                Time.timeScale = 1;
                if (state == "point") LandPoint(doubles, swapped, !swapped, swapped ? 1 : 0);
                else
                {
                    Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("BeginExhibition");
                    var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
                    for (int i = 0; i < 5 && Get<string>("FlowStage") != "Serving"; i++) Call("AdvanceFlow");
                    if (state != "serve") Call("Serve", false);
                    if (state == "fault") type.GetMethod("ContactFault", Hidden).Invoke(court, new[] { (object)0, Enum.Parse(type.GetField("pointCause", Hidden).FieldType, "ServiceArea") });
                }
                type.GetField("flying", Hidden).SetValue(court, false);
                type.GetField("serveReadyAt", Hidden).SetValue(court, Time.time + 100);
                type.GetField("feedback", Hidden).SetValue(court, "Excellent | Smash | IN");
                float energy = state == "serve" ? 100 : state == "rally" ? 75 : state == "point" ? 30 : 0;
                type.GetProperty("Energy").SetValue(court, energy);
                yield return null;
                // Freeze the same rendered HUD while suppressing only small text to measure its true background.
                Time.timeScale = 0; yield return null; yield return new WaitForEndOfFrame();
                string snapshot = (string)Call("CaptureMatch");
                string key = size.x + "x" + size.y + (doubles ? "-doubles" : "-singles") + (swapped ? "-far-" : "-near-") + state;
                var style = (GUIStyle)type.GetField("smallStyle", Hidden).GetValue(court); Color foreground = style.normal.textColor;
                var image = ScreenCapture.CaptureScreenshotAsTexture(); Texture2D background = null, blackMask = null, whiteMask = null;
                try
                {
                    System.IO.File.WriteAllBytes(EvidenceDirectory + "/" + key + ".png", image.EncodeToPNG());
                    style.normal.textColor = Color.clear; yield return null; yield return new WaitForEndOfFrame();
                    background = ScreenCapture.CaptureScreenshotAsTexture();
                    System.IO.File.WriteAllBytes(EvidenceDirectory + "/" + key + "-background.png", background.EncodeToPNG());
                    // Mask comes from opposite text colors, never from foreground/background contrast.
                    style.normal.textColor = Color.black; yield return null; yield return new WaitForEndOfFrame();
                    blackMask = ScreenCapture.CaptureScreenshotAsTexture();
                    style.normal.textColor = Color.white; yield return null; yield return new WaitForEndOfFrame();
                    whiteMask = ScreenCapture.CaptureScreenshotAsTexture();
                    System.IO.File.WriteAllBytes(EvidenceDirectory + "/" + key + "-mask-black.png", blackMask.EncodeToPNG());
                    System.IO.File.WriteAllBytes(EvidenceDirectory + "/" + key + "-mask-white.png", whiteMask.EncodeToPNG());
                    var black = blackMask.GetPixels32(); var white = whiteMask.GetPixels32(); var hidden = background.GetPixels32();
                    float scale = (float)type.GetField("uiScale", Hidden).GetValue(court); Vector2 origin = (Vector2)type.GetProperty("UiOrigin", Hidden).GetValue(court);
                    float Linear(float c) => c <= .04045f ? c / 12.92f : Mathf.Pow((c + .055f) / 1.055f, 2.4f);
                    float Luminance(Color c) => .2126f * Linear(c.r) + .7152f * Linear(c.g) + .0722f * Linear(c.b);
                    foreach (string region in new[] { "energy", "notice" })
                    {
                        if (region == "notice" && state == "rally") continue;
                        Rect rect = region == "energy" ? new Rect(216,158,160,32) : (Rect)type.GetProperty("HudNoticeRect", Hidden).GetValue(court);
                        int count = 0; float minimum = 100, fg = Luminance(foreground);
                        for (int y = Mathf.CeilToInt(origin.y + rect.yMin * scale); y < origin.y + rect.yMax * scale; y++)
                        for (int x = Mathf.CeilToInt(origin.x + rect.xMin * scale); x < origin.x + rect.xMax * scale; x++)
                        {
                            int at = (image.height - 1 - y) * image.width + x; Color32 a = black[at], b = white[at];
                            if (Math.Abs(a.r-b.r) + Math.Abs(a.g-b.g) + Math.Abs(a.b-b.b) < 40) continue;
                            float bg = Luminance(hidden[at]); minimum = Mathf.Min(minimum, (Mathf.Max(fg,bg)+.05f)/(Mathf.Min(fg,bg)+.05f)); count++;
                        }
                        rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6},{7},{8:R},{9}.png", size.x,size.y,doubles ? "doubles" : "singles",swapped ? "far" : "near",state,energy,region,count,minimum,key));
                        if (count < 30 || minimum < 4.5f) failures.Add(key + "/" + region + " glyphs=" + count + " contrast=" + minimum);
                    }
                    Assert.That(Call("CaptureMatch"), Is.EqualTo(snapshot), "HUD rendering must not mutate simulation");
                }
                finally { style.normal.textColor = foreground; UnityEngine.Object.Destroy(image); if (background != null) UnityEngine.Object.Destroy(background); if (blackMask != null) UnityEngine.Object.Destroy(blackMask); if (whiteMask != null) UnityEngine.Object.Destroy(whiteMask); Time.timeScale = 1; }
            }
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/hud-contrast.csv", rows);
        System.IO.File.WriteAllLines(EvidenceDirectory + "/hud-contrast-failures.txt", failures);
        Assert.That(rows.Count - 1, Is.EqualTo(112)); Assert.That(failures, Is.Empty, string.Join("; ", failures.Take(6)));
    }
    [UnityTest] public IEnumerator PlayerMarkersReleaseOwnedResourcesOnDestroy()
    {
        if (!Application.isBatchMode) yield return CaptureAt1280x720();
        var failures = new System.Collections.Generic.List<string>();
        var rows = new System.Collections.Generic.List<string> { "cycle,ringAlive,ringMaterialAlive,hintAlive,hintMaterialAlive" };
        for (int cycle = 0; cycle < 2; cycle++)
        {
            yield return Boot(); Call("EnterTraining"); Call("ConfigureDiscipline", true); Call("BeginExhibition");
            for (int i = 0; i < 3 && Get<string>("FlowStage") != "Serving"; i++) Call("AdvanceFlow");
            yield return null; yield return null;
            var ring = (LineRenderer)type.GetField("activePlayerMarker", Hidden).GetValue(court);
            var hint = (Transform)type.GetField("suggestedPosition", Hidden).GetValue(court);
            Assert.That(ring != null && hint != null, Is.True, "Both markers must exist before destruction");
            var ringMaterial = ring.sharedMaterial; var hintMaterial = hint.GetComponent<Renderer>().sharedMaterial;
            Assert.That(ringMaterial != null && hintMaterial != null, Is.True);
            try
            {
                if (!Application.isBatchMode)
                {
                    yield return new WaitForEndOfFrame(); var image = ScreenCapture.CaptureScreenshotAsTexture();
                    try { System.IO.File.WriteAllBytes(EvidenceDirectory + "/cycle-" + cycle + "-before.png", image.EncodeToPNG()); }
                    finally { UnityEngine.Object.Destroy(image); }
                }
                UnityEngine.Object.Destroy(((Component)court).gameObject);
                yield return null; yield return null;
                rows.Add(cycle + "," + (ring != null) + "," + (ringMaterial != null) + "," + (hint != null) + "," + (hintMaterial != null));
                if (ring != null || ringMaterial != null) failures.Add("cycle " + cycle + ": active marker resources survived owner destruction");
                if (hint != null || hintMaterial != null) failures.Add("cycle " + cycle + ": positioning hint resources survived owner destruction");
                if (!Application.isBatchMode)
                {
                    yield return new WaitForEndOfFrame(); var image = ScreenCapture.CaptureScreenshotAsTexture();
                    try { System.IO.File.WriteAllBytes(EvidenceDirectory + "/cycle-" + cycle + "-after.png", image.EncodeToPNG()); }
                    finally { UnityEngine.Object.Destroy(image); }
                }
            }
            finally
            {
                if (hint != null) UnityEngine.Object.Destroy(hint.gameObject);
                if (hintMaterial != null) UnityEngine.Object.Destroy(hintMaterial);
                if (ring != null) UnityEngine.Object.Destroy(ring.gameObject);
                if (ringMaterial != null) UnityEngine.Object.Destroy(ringMaterial);
            }
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/marker-resources.csv", rows);
        System.IO.File.WriteAllLines(EvidenceDirectory + "/marker-resource-failures.txt", failures);
        Assert.That(failures, Is.Empty, string.Join("; ", failures));
    }
    [UnityTest] public IEnumerator ControlledPlayerMarkerTracksRootAndHidesOutsidePlay()
    {
        if (!Application.isBatchMode) yield return CaptureAt1280x720(); yield return Boot();
        var rows = new System.Collections.Generic.List<string> { "width,height,format,end,state,expectedActive,active,hint,rootY,markerY,rootError,file" };
        var failures = new System.Collections.Generic.List<string>();
        string key = ""; bool doubles = false, swapped = false; LineRenderer firstMarker = null;
        IEnumerator Inspect(string state, bool visible, bool hintVisible)
        {
            yield return null;
            if (!Application.isBatchMode) yield return new WaitForEndOfFrame();
            var root = GameObject.Find("Player").transform;
            string before = Get<object>("Match") == null ? null : (string)Call("CaptureMatch");
            Vector3 position = root.position; float energy = Get<float>("Energy");
            uint rng = (uint)type.GetField("opponentRandom", Hidden).GetValue(court);
            // Visible runs inspect natural LateUpdate at EndOfFrame; batch has no EndOfFrame capture.
            if (Application.isBatchMode) type.GetMethod("LateUpdate", Hidden)?.Invoke(court, null);
            Assert.That(root.position, Is.EqualTo(position)); Assert.That(Get<float>("Energy"), Is.EqualTo(energy));
            Assert.That(type.GetField("opponentRandom", Hidden).GetValue(court), Is.EqualTo(rng));
            if (before != null) Assert.That(Call("CaptureMatch"), Is.EqualTo(before), "Marker must not change simulation/snapshot");
            var marker = type.GetField("activePlayerMarker", Hidden)?.GetValue(court) as LineRenderer;
            var hint = type.GetField("suggestedPosition", Hidden).GetValue(court) as Transform;
            bool active = marker != null && marker.gameObject.activeInHierarchy;
            bool activeHint = hint != null && hint.gameObject.activeInHierarchy;
            float error = marker == null ? -1 : Vector2.Distance(new Vector2(root.position.x, root.position.z), new Vector2(marker.transform.position.x, marker.transform.position.z));
            string name = key + "-" + state;
            if (active != visible) failures.Add(name + "/active marker visibility " + active);
            if (activeHint != hintVisible) failures.Add(name + "/position hint visibility " + activeHint);
            if (visible && marker != null)
            {
                if (firstMarker == null) firstMarker = marker;
                if (marker != firstMarker) failures.Add(name + "/marker recreated during UI transitions");
                if (!marker.enabled || marker.sharedMaterial == null || marker.sharedMaterial.shader == null || marker.sharedMaterial.color.a <= 0 || marker.startColor.a <= 0 || marker.endColor.a <= 0 || marker.startWidth <= 0 || marker.endWidth <= 0) failures.Add(name + "/marker renderer invisible");
                if (error > .0001f || Mathf.Abs(marker.transform.position.y - .018f) > .0001f) failures.Add(name + "/marker not at controlled-player ground position");
                if (!marker.loop || marker.positionCount < 16 || marker.useWorldSpace || marker.alignment != LineAlignment.TransformZ) failures.Add(name + "/marker must be hollow ground ring");
                if (marker.startColor.r < .9f || marker.startColor.g < .7f || marker.startColor.b > .4f) failures.Add(name + "/active marker must differ from cyan positioning hint");
                if (marker.GetComponent<Collider>() != null) failures.Add(name + "/marker adds collision");
            }
            if (!Application.isBatchMode)
            {
                var image = ScreenCapture.CaptureScreenshotAsTexture();
                try { System.IO.File.WriteAllBytes(EvidenceDirectory + "/" + name + ".png", image.EncodeToPNG()); }
                finally { UnityEngine.Object.Destroy(image); }
            }
            rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6},{7},{8:R},{9:R},{10:R},{11}.png", Screen.width, Screen.height, doubles ? "doubles" : "singles", swapped ? "far" : "near", state, visible, active, activeHint, root.position.y, marker == null ? -1 : marker.transform.position.y, error, name));
        }
        foreach (Vector2Int size in new[] { new Vector2Int(1280,720), new Vector2Int(1280,800) })
        {
            if (!Application.isBatchMode) yield return CaptureAtResolution(size.x, size.y);
            foreach (bool pair in new[] { false, true }) foreach (bool ends in new[] { false, true })
            {
                doubles = pair; swapped = ends; key = size.x + "x" + size.y + (pair ? "-doubles" : "-singles") + (ends ? "-far" : "-near");
                Call("EnterTraining"); Call("ConfigureDiscipline", pair); Call("BeginExhibition");
                var match = Get<object>("Match"); var mt = match.GetType(); mt.GetField("EndsSwapped").SetValue(match, ends); Call("NextPoint");
                for (int i = 0; i < 3 && Get<string>("FlowStage") != "Serving"; i++) Call("AdvanceFlow");
                type.GetProperty("ReplayMode").SetValue(court, 1);
                string saved = (string)Call("CaptureMatch");
                yield return Inspect("serving", true, pair);
                Call("Serve", true); yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
                Call("Jump"); Call("SetAnalog", new Vector2(.6f, -.2f));
                for (int i = 0; i < 4; i++) yield return new WaitForFixedUpdate();
                Call("SetAnalog", Vector2.zero);
                Assert.That(GameObject.Find("Player").transform.position.y, Is.GreaterThan(.02f));
                yield return Inspect("moving-jump", true, pair);
                Click("Pause"); yield return Inspect("pause", false, false);
                Click("Lanjut"); yield return Inspect("resumed", true, pair);
                Assert.That(Call("BeginReplay"), Is.True); yield return Inspect("replay", false, false);
                Call("SkipReplay"); yield return Inspect("replay-skipped", true, pair);
                Call("RestoreMatch", saved); yield return Inspect("restored", true, pair);
                match = Get<object>("Match");
                while ((int)mt.GetField("Winner").GetValue(match) < 0) mt.GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
                Call("NextPoint"); yield return Inspect("result", false, false);
                Call("EnterTraining"); yield return Inspect("training", true, false);
                Click("Pause"); Click("Menu"); yield return Inspect("home", false, false);
            }
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/player-markers.csv", rows);
        System.IO.File.WriteAllLines(EvidenceDirectory + "/player-marker-failures.txt", failures);
        Assert.That(rows.Count - 1, Is.EqualTo(80)); Assert.That(failures, Is.Empty, string.Join("; ", failures.Take(6)));
    }
    [UnityTest, Category("Capture")] public IEnumerator CaptureTouchControlsAcrossResizeAndPause()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720(); yield return Boot();
        var down = type.GetMethod("PointerDown", Hidden); var move = type.GetMethod("PointerMove", Hidden);
        var cancel = type.GetMethod("PointerCancel", Hidden); var up = type.GetMethod("PointerUp", Hidden);
        var rows = new System.Collections.Generic.List<string> { "mode,width,height,stickError,jumpOwner,jumpHeld,file" };
        float maximum = 0;
        foreach (string mode in new[] { "Fixed", "Floating" })
        {
            yield return CaptureAt1280x720(); Call("EnterTraining");
            type.GetProperty("JoystickMode").SetValue(court, mode);
            type.GetMethod("ClearGameplayInput", Hidden).Invoke(court, null);
            foreach (Vector2Int size in new[] { new Vector2Int(1280, 800), new Vector2Int(1920, 1080), new Vector2Int(2400, 1080), new Vector2Int(1280, 720) })
            {
                Rect jump = (Rect)type.GetProperty("JumpRect", Hidden).GetValue(court);
                down.Invoke(court, new object[] { 91, jump.center });
                yield return CaptureAtResolution(size.x, size.y); yield return null;
                Assert.That(type.GetField("jumpFinger", Hidden).GetValue(court), Is.EqualTo(91), "Resize must preserve held gesture owner");
                Assert.That(type.GetField("jumpHeld", Hidden).GetValue(court), Is.EqualTo(true));
                Rect stick = (Rect)type.GetProperty("StickRect", Hidden).GetValue(court);
                Vector2 expected = (Vector2)type.GetProperty("DefaultStickCenter", Hidden).GetValue(court);
                float error = Vector2.Distance(stick.center, expected); maximum = Mathf.Max(maximum, error);
                float w = (float)type.GetProperty("UiWidth", Hidden).GetValue(court), h = (float)type.GetProperty("UiHeight", Hidden).GetValue(court);
                var rects = (Rect[])type.GetMethod("GameplayRects").Invoke(null, new object[] { new Vector2(w, h) });
                for (int i = 1; i <= 6; i++)
                {
                    Rect actual = i == 6 ? (Rect)type.GetProperty("JumpRect", Hidden).GetValue(court) : (Rect)type.GetMethod("ShotRect", Hidden).Invoke(court, new object[] { i - 1 });
                    Assert.That(actual, Is.EqualTo(rects[i]), "Drawn controls and input rectangles differ");
                    Assert.That(actual.xMin >= 0 && actual.yMin >= 0 && actual.xMax <= w && actual.yMax <= h, Is.True);
                }
                yield return new WaitForEndOfFrame();
                string file = mode + "-" + size.x + "x" + size.y + "-held.png";
                var image = ScreenCapture.CaptureScreenshotAsTexture();
                try { System.IO.File.WriteAllBytes(EvidenceDirectory + "/" + file, image.EncodeToPNG()); }
                finally { UnityEngine.Object.Destroy(image); }
                rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},91,true,{4}", mode, size.x, size.y, error, file));
                Rect shot = (Rect)type.GetMethod("ShotRect", Hidden).Invoke(court, new object[] { 0 });
                move.Invoke(court, new object[] { 91, shot.center });
                Assert.That(type.GetField("charging", Hidden).GetValue(court), Is.EqualTo(true));
                Vector3 velocity = Get<Vector3>("ShuttleVelocity"); float energy = Get<float>("Energy");
                if (size.x == 1920) { Click("Pause"); Click("Lanjut"); } else cancel.Invoke(court, new object[] { 91 });
                up.Invoke(court, new object[] { 91 });
                Assert.That(type.GetField("jumpHeld", Hidden).GetValue(court), Is.EqualTo(false));
                Assert.That(type.GetField("charging", Hidden).GetValue(court), Is.EqualTo(false));
                Assert.That(Get<Vector3>("ShuttleVelocity"), Is.EqualTo(velocity)); Assert.That(Get<float>("Energy"), Is.EqualTo(energy));
                down.Invoke(court, new object[] { 92, expected });
                move.Invoke(court, new object[] { 92, expected + Vector2.right * 35 });
                Assert.That(Get<Vector2>("Analog").x, Is.EqualTo(.5f).Within(.001f));
                cancel.Invoke(court, new object[] { 92 }); Assert.That(Get<Vector2>("Analog"), Is.EqualTo(Vector2.zero));
            }
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/resize-input.csv", rows);
        Assert.That(rows.Count - 1, Is.EqualTo(8)); Assert.That(maximum, Is.LessThan(.001f), "Resize leaves idle joystick at old canvas origin");
    }
    [UnityTest, Category("Capture")] public IEnumerator CaptureGameplayCameraAcrossFormatsEndsAndResolutions()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720(); yield return Boot();
        var rows = new System.Collections.Generic.List<string> { "width,height,format,end,state,subject,minX,minY,maxX,maxY,controlOverlap,file" };
        var failures = new System.Collections.Generic.List<string>();
        int scenarios = 0;
        foreach (Vector2Int size in new[] { new Vector2Int(1280,720), new Vector2Int(1280,800), new Vector2Int(1920,1080), new Vector2Int(2400,1080) })
        {
            yield return CaptureAtResolution(size.x, size.y);
            foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
            {
                Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("BeginExhibition");
                var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
                for (int i = 0; i < 3 && Get<string>("FlowStage") != "Serving"; i++) Call("AdvanceFlow");
                Assert.That(Get<string>("FlowStage"), Is.EqualTo("Serving"));
                type.GetProperty("MatchState").SetValue(court, "Rally"); type.GetProperty("ReplayMode").SetValue(court, 2);
                type.GetField("flying", Hidden).SetValue(court, false);
                float end = swapped ? -1 : 1;
                Vector3 World(Vector3 p) => new Vector3(p.x * end, p.y, p.z * end);
                var actors = (doubles ? new[] { "Player", "Opponent", "Partner", "OpponentPartner" } : new[] { "Player", "Opponent" }).Select(n => GameObject.Find(n).transform).ToArray();
                var camera = Camera.main;
                foreach (string state in new[] { "base", "near-net", "wide-defense", "jump" })
                {
                    type.GetMethod("ResetAthleteMovement", Hidden).Invoke(court, null); Call("SetAnalog", Vector2.zero);
                    for (int id = 0; id < actors.Length; id++)
                    {
                        bool near = id % 2 == 0;
                        float x = state == "wide-defense" ? (id < 2 ? 2.8f : -2.8f) : doubles ? (id < 2 ? 1.3f : -1.3f) : 0;
                        float depth = state == "near-net" && id < 2 ? 1.1f : state == "wide-defense" ? 6.2f : 4;
                        actors[id].position = World(new Vector3(x, 0, near ? -depth : depth));
                    }
                    var shuttle = GameObject.Find("Shuttle").transform;
                    shuttle.position = World(new Vector3(0, state == "near-net" ? 1.7f : 2.1f, state == "near-net" ? .2f : -2));
                    if (state == "jump") { type.GetField("jumpAt", Hidden).SetValue(court, Time.time - 2); Call("Jump"); yield return new WaitForSeconds(.32f); }
                    else yield return new WaitForSeconds(.25f);
                    yield return new WaitForEndOfFrame();
                    string key = size.x + "x" + size.y + "-" + (doubles ? "doubles" : "singles") + (swapped ? "-far-" : "-near-") + state;
                    var image = ScreenCapture.CaptureScreenshotAsTexture();
                    try { System.IO.File.WriteAllBytes(EvidenceDirectory + "/" + key + ".png", image.EncodeToPNG()); }
                    finally { UnityEngine.Object.Destroy(image); }
                    float width = (float)type.GetProperty("UiWidth", Hidden).GetValue(court), height = (float)type.GetProperty("UiHeight", Hidden).GetValue(court);
                    var controls = (Rect[])type.GetMethod("GameplayRects").Invoke(null, new object[] { new Vector2(width, height) });
                    void Record(string subject, Vector3[] points, bool body)
                    {
                        var projected = points.Select(camera.WorldToViewportPoint).ToArray();
                        float minX = projected.Min(p => p.x), maxX = projected.Max(p => p.x), minY = projected.Min(p => p.y), maxY = projected.Max(p => p.y);
                        var rect = Rect.MinMaxRect(minX * width, (1 - maxY) * height, maxX * width, (1 - minY) * height);
                        int overlap = body ? controls.Take(7).Count(r => r.Overlaps(rect)) : 0;
                        rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6:R},{7:R},{8:R},{9:R},{10},{11}.png", size.x, size.y, doubles ? "doubles" : "singles", swapped ? "far" : "near", state, subject, minX, minY, maxX, maxY, overlap, key));
                        if (projected.Any(p => p.z <= 0) || minX < .02f || maxX > .98f || minY < .02f || maxY > .98f) failures.Add(key + "/" + subject + " outside 2% viewport margin");
                        if (overlap > 0) failures.Add(key + "/" + subject + " intersects control bounds");
                    }
                    foreach (var actor in actors)
                    foreach (var skin in actor.GetComponentsInChildren<SkinnedMeshRenderer>().Where(r => r.enabled))
                    {
                        var mesh = new Mesh();
                        try { skin.BakeMesh(mesh); Record(actor.name, mesh.vertices.Select(v => skin.transform.TransformPoint(v)).ToArray(), true); }
                        finally { UnityEngine.Object.Destroy(mesh); }
                    }
                    Record("court", new[] { World(new Vector3(-3.05f, 0, -6.7f)), World(new Vector3(3.05f, 0, -6.7f)), World(new Vector3(-3.05f, 0, 6.7f)), World(new Vector3(3.05f, 0, 6.7f)) }, false);
                    foreach (var line in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None).Where(r => r.name == "Sideline" || r.name == "CourtLine" || r.name == "ServiceCentre"))
                    {
                        var corners = new[] { new Vector3(-.5f, .5f, -.5f), new Vector3(.5f, .5f, -.5f), new Vector3(.5f, .5f, .5f), new Vector3(-.5f, .5f, .5f) }
                            .Select(p => camera.WorldToViewportPoint(line.transform.TransformPoint(p))).Select(p => new Vector2(p.x * width, (1 - p.y) * height)).ToArray();
                        bool Intersects(Rect control)
                        {
                            control = Rect.MinMaxRect(control.xMin - 3, control.yMin - 3, control.xMax + 3, control.yMax + 3);
                            var box = new[] { control.min, new Vector2(control.xMax, control.yMin), control.max, new Vector2(control.xMin, control.yMax) };
                            var axes = new System.Collections.Generic.List<Vector2> { Vector2.right, Vector2.up };
                            for (int i = 0; i < 4; i++) { var edge = corners[(i + 1) % 4] - corners[i]; axes.Add(new Vector2(-edge.y, edge.x)); }
                            foreach (var axis in axes)
                            {
                                var a = corners.Select(p => Vector2.Dot(p, axis)).ToArray(); var b = box.Select(p => Vector2.Dot(p, axis)).ToArray();
                                if (a.Max() < b.Min() || b.Max() < a.Min()) return false;
                            }
                            return true;
                        }
                        int overlap = controls.Take(7).Count(Intersects);
                        rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},line-{5},{6:R},{7:R},{8:R},{9:R},{10},{11}.png", size.x, size.y, doubles ? "doubles" : "singles", swapped ? "far" : "near", state, line.name, corners.Min(p => p.x) / width, 1 - corners.Max(p => p.y) / height, corners.Max(p => p.x) / width, 1 - corners.Min(p => p.y) / height, overlap, key));
                        if (overlap > 0) failures.Add(key + "/" + line.name + " intersects control bounds with 3-unit margin");
                    }
                    Record("net", new[] { new Vector3(-3.05f, 1.55f, 0), new Vector3(3.05f, 1.55f, 0) }, false);
                    Record("shuttle", new[] { shuttle.position }, false);
                    if (state == "base")
                    {
                        float athleteHeight = camera.WorldToViewportPoint(World(new Vector3(0, 1.8f, -4))).y - camera.WorldToViewportPoint(World(new Vector3(0, 0, -4))).y;
                        float headroom = 1 - camera.WorldToViewportPoint(World(new Vector3(0, 0, 6.7f))).y;
                        rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},base,reference,0,0,{4:R},{5:R},0,{6}.png", size.x, size.y, doubles ? "doubles" : "singles", swapped ? "far" : "near", athleteHeight, headroom, key));
                        if (athleteHeight < .18f) failures.Add(key + "/reference athlete height " + athleteHeight + " < .18 viewport");
                        if (headroom > .40f) failures.Add(key + "/court headroom " + headroom + " > .40 viewport");
                    }
                    scenarios++;
                }
            }
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/camera-framing.csv", rows);
        System.IO.File.WriteAllLines(EvidenceDirectory + "/camera-failures.txt", failures);
        Assert.That(scenarios, Is.EqualTo(64)); Assert.That(failures, Is.Empty, "Camera framing matrix: " + string.Join("; ", failures.Take(6)));
    }
    [UnityTest, Category("Capture")] public IEnumerator CaptureHighFlightsInsideGameplayView()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720(); yield return Boot();
        var rows = new System.Collections.Generic.List<string> { "width,height,format,end,shot,frame,gameSeconds,wallSeconds,shuttleY,viewportX,viewportY" };
        int outside = 0, completed = 0;
        foreach (Vector2Int size in new[] { new Vector2Int(1280,720), new Vector2Int(1280,800), new Vector2Int(1920,1080), new Vector2Int(2400,1080) })
        {
            yield return CaptureAtResolution(size.x, size.y);
            foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true }) foreach (string shot in new[] { "Clear", "Lift" })
            {
                Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("BeginExhibition");
                var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
                for (int i = 0; i < 3 && Get<string>("FlowStage") != "Serving"; i++) Call("AdvanceFlow");
                type.GetProperty("ReplayMode").SetValue(court, 2); Call("Serve", true);
                var player = GameObject.Find("Player").transform; var shuttle = GameObject.Find("Shuttle").transform;
                float end = swapped ? -1 : 1;
                player.position = new Vector3(0, 0, (shot == "Lift" ? -.9f : -3.5f) * end);
                shuttle.position = player.TransformPoint(new Vector3(.3f, shot == "Lift" ? 1 : 2.1f, .35f));
                type.GetField("velocity", Hidden).SetValue(court, Vector3.zero); type.GetField("lastHitter", Hidden).SetValue(court, 1);
                type.GetField("serviceFlight", Hidden).SetValue(court, false); Call("TapShot", shot);
                Assert.That(type.GetField("lastStroke", Hidden).GetValue(court), Is.EqualTo(shot));
                float start = Time.time, wall = Time.realtimeSinceStartup, apex = 0; int frames = 0; bool captured = false;
                while (Get<bool>("Flying") && (int)type.GetField("lastHitter", Hidden).GetValue(court) == 0 && Time.time - start < 4)
                {
                    yield return new WaitForEndOfFrame();
                    var point = Camera.main.WorldToViewportPoint(shuttle.position); apex = Mathf.Max(apex, shuttle.position.y);
                    rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6:R},{7:R},{8:R},{9:R},{10:R}", size.x, size.y, doubles ? "doubles" : "singles", swapped ? "far" : "near", shot, frames++, Time.time - start, Time.realtimeSinceStartup - wall, shuttle.position.y, point.x, point.y));
                    if (point.z <= 0 || point.x < .02f || point.x > .98f || point.y < .02f || point.y > .98f) outside++;
                    if (!captured && Get<Vector3>("ShuttleVelocity").y <= 0)
                    {
                        var image = ScreenCapture.CaptureScreenshotAsTexture();
                        try { System.IO.File.WriteAllBytes(EvidenceDirectory + "/" + size.x + "x" + size.y + "-" + (doubles ? "doubles" : "singles") + (swapped ? "-far-" : "-near-") + shot + "-apex.png", image.EncodeToPNG()); }
                        finally { UnityEngine.Object.Destroy(image); }
                        captured = true;
                    }
                }
                Assert.That(apex, Is.GreaterThan(3.5f)); Assert.That(captured, Is.True); Assert.That(frames, Is.GreaterThan(10)); completed++;
            }
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/camera-flight.csv", rows);
        Assert.That(completed, Is.EqualTo(32)); Assert.That(outside, Is.Zero, "High flight leaves safe viewport");
    }
    [UnityTest, Category("Capture")] public IEnumerator ShadowColorLabelShowsCurrentRgbWithoutChangingToggle()
    {
        Assert.That(Application.isBatchMode, Is.False);
        yield return CaptureAt1280x720(); yield return Boot(); Click("Pengaturan");
        var labelMethod = type.GetMethod("ActionLabel", Hidden);
        var tintField = type.GetField("shadowTint", Hidden);
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForEndOfFrame();
            Color tint = (Color)tintField.GetValue(court);
            string label = (string)labelMethod.Invoke(court, new object[] { "ShadowColor" });
            Assert.That(label, Does.EndWith("#" + ColorUtility.ToHtmlStringRGB(tint)), "Active shadow color missing from label");
            var style = (GUIStyle)type.GetField("buttonStyle", Hidden).GetValue(court);
            Rect rect = (Rect)Call("UiActionRect", "ShadowColor");
            Assert.That(style.CalcSize(new GUIContent(label.ToUpperInvariant())).x, Is.LessThanOrEqualTo(rect.width - 32), "Shadow label must retain horizontal padding");
            var image = ScreenCapture.CaptureScreenshotAsTexture();
            try { System.IO.File.WriteAllBytes(EvidenceDirectory + "/shadow-color-" + i + ".png", image.EncodeToPNG()); }
            finally { UnityEngine.Object.Destroy(image); }
            Click("ShadowColor");
            Assert.That((Color)tintField.GetValue(court), Is.EqualTo(tint == Color.black ? new Color(.12f, .18f, .18f) : Color.black), "Existing toggle colors must not change");
        }
    }
    [UnityTest, Category("Capture")] public IEnumerator ControlLabelsKeepInkContrastWhenHoveredOrFocused()
    {
        Assert.That(Application.isBatchMode, Is.False, "GUI skin requires rendered OnGUI, not batch simulation");
        yield return CaptureAt1280x720();
        yield return Boot(); Click("Pengaturan"); yield return new WaitForEndOfFrame();
        var style = (GUIStyle)type.GetField("buttonStyle", Hidden).GetValue(court);
        Assert.That(style, Is.Not.Null);
        Color ink = (Color)type.GetField("inkColor", Hidden).GetValue(court);
        foreach (var state in new[] { style.normal, style.hover, style.active, style.focused })
            Assert.That(state.textColor, Is.EqualTo(ink), "Control label inherits pale skin state");
        if (!Application.isBatchMode)
        {
            yield return new WaitForEndOfFrame();
            var image = ScreenCapture.CaptureScreenshotAsTexture();
            System.IO.File.WriteAllBytes(EvidenceDirectory + "/settings-labels.png", image.EncodeToPNG());
            UnityEngine.Object.Destroy(image);
        }
    }
    [UnityTest] public IEnumerator MatchBreakPanelStaysInScoreLaneAndContinueStillWorks()
    {
        yield return Boot(); Click("Main"); Click("Ganda"); Click("Mulai");
        foreach (string stage in new[] { "Warmup", "Ready", "GameEnd", "Interval", "ChangeEnds" })
        {
            type.GetField("flowStage", Hidden).SetValue(court, stage);
            var panel = type.GetProperty("FlowPanelRect", Hidden);
            Assert.That(panel, Is.Not.Null, "Break dialog still covers far athletes in court centre");
            Rect rect = (Rect)panel.GetValue(court), button = (Rect)Call("UiActionRect", "Teruskan");
            Assert.That(rect.xMax, Is.LessThanOrEqualTo(374));
            Assert.That(rect.yMin, Is.GreaterThanOrEqualTo(230));
            Assert.That(rect.Contains(button.min) && rect.Contains(button.max), Is.True);
            Assert.That(button.height, Is.GreaterThanOrEqualTo(64));
        }
        type.GetField("flowStage", Hidden).SetValue(court, "Warmup");
        Click("Teruskan"); Assert.That(Get<string>("FlowStage"), Is.EqualTo("Ready"));
        Click("Teruskan"); Assert.That(Get<string>("FlowStage"), Is.EqualTo("Serving"));
        Assert.That(Get<bool>("Flying"), Is.False);
    }
    [UnityTest] public IEnumerator ReturningToTrainingClearsServeAndReviewStateWithoutRecoveringEnergy()
    {
        yield return Boot();
        foreach (bool doubles in new[] { false, true }) foreach (bool afterPoint in new[] { false, true })
        {
            Call("ConfigureDiscipline", doubles); Call("BeginExhibition"); Click("Teruskan"); Click("Teruskan");
            type.GetProperty("ReplayMode").SetValue(court, 2);
            if (afterPoint)
            {
                Call("Serve", false); type.GetField("serviceFlight", Hidden).SetValue(court, false);
                GameObject.Find("Shuttle").transform.position = new Vector3(0, .045f, 3);
                type.GetField("velocity", Hidden).SetValue(court, Vector3.down);
                LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[collision\] ground.*"));
                LogAssert.Expect(LogType.Log, "[scoring] point winner=0");
                type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
                Assert.That(Get<string>("MatchState"), Is.EqualTo("Point"));
                Assert.That(type.GetField("reviewAvailable", Hidden).GetValue(court), Is.EqualTo(true));
            }
            type.GetProperty("Energy").SetValue(court, 60f);
            Click("Pause"); Click("Menu"); Click("Ya"); Click("Latihan");
            Assert.That(Get<object>("Match"), Is.Null);
            Assert.That(Get<string>("ScreenName"), Is.EqualTo("Training"));
            Assert.That(Get<string>("MatchState"), Is.EqualTo("Training"), "Training inherits match serve/point state");
            Assert.That(Get<string>("FlowStage"), Is.EqualTo("Training"));
            Assert.That(type.GetField("reviewAvailable", Hidden).GetValue(court), Is.EqualTo(false));
            Assert.That((string)type.GetField("beforePoint", Hidden).GetValue(court), Is.Null.Or.Empty);
            Assert.That(((Rect)Call("UiActionRect", "Challenge")).width, Is.Zero);
            Assert.That(Get<bool>("Flying"), Is.False);
            Assert.That(Get<float>("Energy"), Is.EqualTo(60), "Training must not grant idle recovery");
            foreach (string action in new[] { "Umpan lob", "Umpan net", "Pause" }) Assert.That(((Rect)Call("UiActionRect", action)).width, Is.GreaterThan(0));
            Click("Umpan lob"); Assert.That(Get<bool>("Flying"), Is.True);
        }
    }
    [UnityTest, Category("Capture")] public IEnumerator CaptureMenuSaveFeedbackStates()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720(); yield return Boot();
        Click("Main"); Click("Mulai"); Click("Pause");
        string path = (string)type.GetProperty("SavePath", Hidden).GetValue(court);
        IEnumerator Capture(string name)
        {
            yield return new WaitForEndOfFrame(); var image = ScreenCapture.CaptureScreenshotAsTexture();
            try { System.IO.File.WriteAllBytes(EvidenceDirectory + "/" + name + ".png", image.EncodeToPNG()); }
            finally { UnityEngine.Object.Destroy(image); }
        }
        Click("Simpan"); yield return Capture("save-success");
        Click("Muat"); yield return Capture("load-success");
        System.IO.File.WriteAllText(path, "{broken"); Click("Muat"); yield return Capture("invalid-save");
        System.IO.File.Delete(path); Click("Muat"); yield return Capture("missing-save");
        Click("Menu"); Click("Ya"); Click("Latihan"); Click("Pause"); Click("Simpan"); yield return Capture("no-active-match");
    }
    [UnityTest] public IEnumerator PauseSaveFeedbackShowsSuccessAndFailureWithoutChangingMatch()
    {
        yield return Boot(); Click("Main"); Click("Mulai"); Click("Pause");
        string path = (string)type.GetProperty("SavePath", Hidden).GetValue(court);
        var notice = type.GetProperty("MenuNotice", Hidden);
        string Notice() => (string)notice?.GetValue(court) ?? "";
        Click("Simpan"); string saved = System.IO.File.ReadAllText(path);
        Assert.That(Notice(), Is.EqualTo("Pertandingan disimpan."), "Pause hides successful save feedback");
        Click("Muat"); Assert.That(Notice(), Is.EqualTo("Pertandingan dimuat."));
        string live = (string)Call("CaptureMatch");
        System.IO.File.WriteAllText(path, "{broken"); Click("Muat");
        Assert.That(Notice(), Is.EqualTo("Data simpan tidak valid. Pertandingan tidak diubah."));
        Assert.That(Call("CaptureMatch"), Is.EqualTo(live));
        System.IO.File.WriteAllText(path, saved.Replace("\"version\":3", "\"version\":99")); Click("Muat");
        Assert.That(Notice(), Is.EqualTo("Data simpan tidak valid. Pertandingan tidak diubah."));
        Assert.That(Call("CaptureMatch"), Is.EqualTo(live), "Invalid schema must not change live state");
        System.IO.File.Delete(path); Click("Muat");
        Assert.That(Notice(), Is.EqualTo("Berkas simpan tidak ditemukan."));
        Assert.That(Call("CaptureMatch"), Is.EqualTo(live));
        System.IO.File.WriteAllText(path, saved);
        using (var locked = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None))
        {
            Click("Muat"); Assert.That(Notice(), Is.EqualTo("Berkas tidak dapat diakses. Coba lagi."));
            Assert.That(Call("CaptureMatch"), Is.EqualTo(live));
        }
        Click("Lanjut"); Assert.That(Notice(), Is.Empty);
        Click("Pause"); Assert.That(Notice(), Is.Empty, "Old failure must not leak into new pause");
        Click("Menu"); Click("Ya"); Click("Latihan"); Click("Pause"); Click("Simpan");
        Assert.That(Notice(), Is.EqualTo("Belum ada pertandingan untuk disimpan."));
    }
    void LandPoint(bool doubles, bool swapped, bool inside, int winner)
    {
        Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("StartMatch");
        var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped);
        Call("NextPoint"); type.GetField("flowStage", Hidden).SetValue(court, null); Call("Serve", false);
        Assert.That(Get<bool>("Flying"), Is.True);
        type.GetField("serviceFlight", Hidden).SetValue(court, false);
        int hitter = inside ? winner : 1 - winner;
        type.GetField("lastHitter", Hidden).SetValue(court, hitter); type.GetField("lastPlayer", Hidden).SetValue(court, hitter);
        GameObject.Find("Shuttle").transform.position = new Vector3(inside ? 2.4f : 4, .045f, (winner == 0 ? 3 : -3) * (swapped ? -1 : 1));
        type.GetField("velocity", Hidden).SetValue(court, Vector3.down);
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[collision\] ground.*"));
        type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
        Assert.That(Get<string>("MatchState"), Is.EqualTo("Point"));
        Assert.That((int)match.GetType().GetField("Server").GetValue(match), Is.EqualTo(winner));
    }
    [UnityTest] public IEnumerator PointNoticeUsesAdjudicationAndSurvivesRestore()
    {
        yield return Boot();
        var notice = type.GetProperty("PointNotice", Hidden);
        var rows = new System.Collections.Generic.List<string> { "doubles,swapped,inside,winner,notice" };
        foreach (bool doubles in new[] { false, true })
        foreach (bool swapped in new[] { false, true })
        foreach (bool inside in new[] { false, true })
        foreach (int winner in new[] { 0, 1 })
        {
            LandPoint(doubles, swapped, inside, winner);
            string expected = (winner == 0 ? "Poin kamu" : "Poin lawan") + (inside ? "\nKok masuk" : "\nKok keluar");
            Assert.That(notice.GetValue(court), Is.EqualTo(expected), "Point cause must come from adjudication, not shot debug text");
            float gradeBefore = Get<float>("LastGradeScore"), energy = Get<float>("Energy");
            foreach (string raw in new[] { "Excellent | Smash | IN", "Good | Clear | OUT", "Fair | miss: outside reach", "Poor | Net", "Fault | body touch", "Review: IN | call confirmed | left 1", "Terlalu jauh", "Match restored", "Goodbye | IN", "" })
            {
                type.GetField("feedback", Hidden).SetValue(court, raw);
                Assert.That(notice.GetValue(court), Is.EqualTo(expected));
                Assert.That(type.GetField("feedback", Hidden).GetValue(court), Is.EqualTo(raw));
                Assert.That(Get<float>("LastGradeScore"), Is.EqualTo(gradeBefore));
            }
            Call("SaveMatch"); Call("StartMatch"); Call("LoadMatch");
            Assert.That(notice.GetValue(court), Is.EqualTo(expected), "Loading must retain actual point cause");
            Assert.That(Get<float>("Energy"), Is.EqualTo(energy));
            rows.Add(doubles + "," + swapped + "," + inside + "," + winner + "," + expected.Replace("\n", " | "));
            Call("NextPoint"); Assert.That(notice.GetValue(court), Is.EqualTo(""), "Prior cause must not leak into serve");
            Call("EnterTraining"); Assert.That(notice.GetValue(court), Is.EqualTo(""), "Prior cause must not leak into Training");
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/point-outcomes.csv", rows);
    }
    [UnityTest] public IEnumerator PointFaultCauseSurvivesPauseSaveAndNextServe()
    {
        yield return Boot();
        var notice = type.GetProperty("PointNotice", Hidden);
        foreach (string cause in new[] { "body", "net", "below", "service" })
        foreach (bool doubles in new[] { false, true })
        foreach (bool swapped in new[] { false, true })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("StartMatch");
            var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped);
            Call("NextPoint"); type.GetField("flowStage", Hidden).SetValue(court, null); Call("Serve", false);
            float sign = swapped ? -1 : 1;
            var shuttle = GameObject.Find("Shuttle").transform;
            var player = GameObject.Find("Player").transform;
            string expected = cause == "body" ? "Kok menyentuh badan" : cause == "net" ? "Menyentuh net" : cause == "below" ? "Kok di bawah net" : "Servis di luar area";
            if (cause == "body")
            {
                shuttle.position = player.position + new Vector3(0, 1, -.6f * sign);
                type.GetField("velocity", Hidden).SetValue(court, new Vector3(0, 0, 40 * sign));
            }
            else if (cause == "net")
            {
                shuttle.position = new Vector3(2, 3, 2 * sign);
                player.position = new Vector3(0, 0, -.25f * sign);
                ((Transform)type.GetField("racket", Hidden).GetValue(court)).position = new Vector3(0, 1.3f, 0);
            }
            else
            {
                shuttle.position = cause == "below" ? new Vector3(2, .4f, -.2f * sign) : new Vector3(-1 * sign, .045f, 1 * sign);
                type.GetField("velocity", Hidden).SetValue(court, cause == "below" ? new Vector3(0, 0, 40 * sign) : Vector3.down);
            }
            if (cause == "service") LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[collision\] ground.*"));
            else LogAssert.Expect(LogType.Log, "[scoring] fault " + (cause == "below" ? "shuttle below net" : cause + " touch") + " player=0");
            type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
            Assert.That(Get<string>("MatchState"), Is.EqualTo("Point"));
            Assert.That(notice.GetValue(court), Is.EqualTo("Poin lawan\n" + expected));
            Assert.That(Call("ChallengeCall"), Is.False);
            Click("Pause"); Click("Simpan"); Click("Muat");
            Assert.That(notice.GetValue(court), Is.EqualTo("Poin lawan\n" + expected));
            Click("Lanjut");
            type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
            Assert.That((int[])Get<object>("Match").GetType().GetField("Points").GetValue(Get<object>("Match")), Is.EqualTo(new[] { 0, 1 }));
            Call("NextPoint"); Assert.That(notice.GetValue(court), Is.EqualTo(""));
        }
    }
    [UnityTest] public IEnumerator PointReviewNoticeUsesActualCallAndSurvivesReplayRestore()
    {
        if (!Application.isBatchMode) yield return CaptureAt1280x720();
        yield return Boot();
        var notice = type.GetProperty("PointNotice", Hidden);
        foreach (bool doubles in new[] { false, true })
        foreach (bool swapped in new[] { false, true })
        foreach (bool overturned in new[] { false, true })
        foreach (int replayMode in new[] { 0, 1, 2 })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("StartMatch");
            var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped);
            Call("NextPoint"); type.GetField("flowStage", Hidden).SetValue(court, null);
            type.GetProperty("ReplayMode").SetValue(court, replayMode);
            type.GetProperty("SimulateLineErrors").SetValue(court, overturned);
            type.GetField("judgeState", Hidden).SetValue(court, (uint)1);
            Call("Serve", false);
            var tick = type.GetMethod("FixedUpdate", Hidden); tick.Invoke(court, null); tick.Invoke(court, null);
            type.GetField("serviceFlight", Hidden).SetValue(court, false);
            GameObject.Find("Shuttle").transform.position = new Vector3(doubles ? 3.04f : 2.58f, .045f, 3 * (swapped ? -1 : 1));
            type.GetField("velocity", Hidden).SetValue(court, Vector3.down);
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[collision\] ground.*"));
            tick.Invoke(court, null);
            Assert.That(notice.GetValue(court), Is.EqualTo(overturned ? "Poin lawan\nKok keluar" : "Poin kamu\nKok masuk"));
            Assert.That(Call("ChallengeCall"), Is.True);
            string expected = "Poin kamu" + (overturned ? " (dikoreksi)" : " (tetap)") + "\nKok masuk";
            Assert.That(notice.GetValue(court), Is.EqualTo(expected), "Review must replace original decision without parsing debug");
            Assert.That(type.GetField("replaying", Hidden).GetValue(court), Is.EqualTo(replayMode != 2));
            Assert.That(Call("ChallengeCall"), Is.False);
            string saved = (string)Call("CaptureMatch"); Call("StartMatch"); Call("RestoreMatch", saved);
            Assert.That(notice.GetValue(court), Is.EqualTo(expected));
            Assert.That(Call("ChallengeCall"), Is.False);
            var restored = Get<object>("Match");
            Assert.That((int[])restored.GetType().GetField("Points").GetValue(restored), Is.EqualTo(new[] { 1, 0 }));
            Assert.That(((int[])restored.GetType().GetField("Challenges").GetValue(restored))[0], Is.EqualTo(overturned ? 2 : 1));
            if (!Application.isBatchMode && replayMode == 2)
            {
                yield return new WaitForEndOfFrame();
                var image = ScreenCapture.CaptureScreenshotAsTexture();
                try { System.IO.File.WriteAllBytes(EvidenceDirectory + "/" + (doubles ? "doubles" : "singles") + "-" + (swapped ? "swapped" : "home") + "-" + (overturned ? "overturned" : "confirmed") + ".png", image.EncodeToPNG()); }
                finally { UnityEngine.Object.Destroy(image); }
            }
            Call("NextPoint"); Assert.That(notice.GetValue(court), Is.EqualTo(""));
        }
    }
    [UnityTest] public IEnumerator PointOutcomeSnapshotRejectsInvalidEnumsAndKeepsLegacyUnknown()
    {
        yield return Boot(); LandPoint(false, false, true, 0);
        var notice = type.GetProperty("PointNotice", Hidden);
        string saved = (string)Call("CaptureMatch");
        Assert.That(saved, Does.Contain("\"pointCause\":1")); Assert.That(saved, Does.Contain("\"pointReview\":0"));
        foreach (string invalid in new[] {
            saved.Replace("\"pointCause\":1", "\"pointCause\":-1"), saved.Replace("\"pointCause\":1", "\"pointCause\":7"),
            saved.Replace("\"pointReview\":0", "\"pointReview\":-1"), saved.Replace("\"pointReview\":0", "\"pointReview\":3"),
            saved.Replace("\"pointReview\":0", "\"pointReview\":1"),
            saved.Replace("\"pointCause\":1", "\"pointCause\":4").Replace("\"pointReview\":0", "\"pointReview\":1").Replace("\"reviewAvailable\":true", "\"reviewAvailable\":false") })
        {
            var error = Assert.Throws<TargetInvocationException>(() => Call("RestoreMatch", invalid));
            Assert.That(error.InnerException, Is.TypeOf<ArgumentException>());
            Assert.That(Call("CaptureMatch"), Is.EqualTo(saved), "Invalid outcome must not mutate live state");
            Assert.That(notice.GetValue(court), Is.EqualTo("Poin kamu\nKok masuk"));
        }
        foreach (int version in new[] { 1, 2, 3 })
        {
            string legacy = saved.Replace("\"version\":3", "\"version\":" + version).Replace("\"pointCause\":1,", "").Replace("\"pointReview\":0,", "");
            Assert.That(legacy, Does.Not.Contain("pointCause"));
            Call("RestoreMatch", legacy);
            Assert.That(notice.GetValue(court), Is.EqualTo("Poin kamu\nSebab poin tidak tersimpan"));
            Call("SaveMatch"); Call("StartMatch"); Call("LoadMatch");
            Assert.That(notice.GetValue(court), Is.EqualTo("Poin kamu\nSebab poin tidak tersimpan"));
            Assert.That(Call("ChallengeCall"), Is.True);
            Assert.That(notice.GetValue(court), Is.EqualTo("Poin kamu (tetap)\nKok masuk"));
        }
        Call("NextPoint"); string serve = (string)Call("CaptureMatch");
        var servingError = Assert.Throws<TargetInvocationException>(() => Call("RestoreMatch", serve.Replace("\"pointCause\":0", "\"pointCause\":1")));
        Assert.That(servingError.InnerException, Is.TypeOf<ArgumentException>());
        Assert.That(Call("CaptureMatch"), Is.EqualTo(serve));
    }
    [UnityTest, Category("Capture")] public IEnumerator CaptureAdjudicationPointFeedback()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720(); yield return Boot();
        foreach (bool doubles in new[] { false, true })
        foreach (bool swapped in new[] { false, true })
        {
            LandPoint(doubles, swapped, !swapped, swapped ? 1 : 0);
            Call("SaveMatch");
            foreach (bool restored in new[] { false, true })
            {
                if (restored) { Call("StartMatch"); Call("LoadMatch"); }
                yield return new WaitForEndOfFrame();
                var image = ScreenCapture.CaptureScreenshotAsTexture();
                try { System.IO.File.WriteAllBytes(EvidenceDirectory + "/" + (doubles ? "doubles" : "singles") + "-" + (swapped ? "swapped-out" : "home-in") + (restored ? "-restored" : "-live") + ".png", image.EncodeToPNG()); }
                finally { UnityEngine.Object.Destroy(image); }
            }
        }
    }
    [UnityTest] public IEnumerator PointAndMissNoticeHasOwnLaneOutsideScoreAndControls()
    {
        yield return Boot();
        var property = type.GetProperty("HudNoticeRect", Hidden);
        Assert.That(property, Is.Not.Null, "Point feedback still straddles scoreboard edge");
        foreach (bool training in new[] { true, false })
        {
            Call(training ? "EnterTraining" : "BeginExhibition");
            var rect = (Rect)property.GetValue(court);
            Assert.That(rect.yMin, Is.GreaterThanOrEqualTo(222));
            Assert.That(rect.width, Is.GreaterThanOrEqualTo(350));
            Assert.That(rect.Overlaps(new Rect(24,24,350,188)), Is.False);
            foreach (string action in (string[])type.GetProperty("UiActions", Hidden).GetValue(court))
                Assert.That(rect.Overlaps((Rect)Call("UiActionRect", action)), Is.False, action);
        }
    }
    [UnityTest, Category("Capture")] public IEnumerator AllRuntimeScreensAtFourApprovedResolutions()
    {
#if UNITY_EDITOR
        Assert.That(Application.isBatchMode, Is.False);
        var editor = typeof(UnityEditor.Editor).Assembly;
        var sizesType = editor.GetType("UnityEditor.GameViewSizes");
        var sizes = sizesType.BaseType.GetProperty("instance").GetValue(null);
        var group = sizesType.GetProperty("currentGroup").GetValue(sizes);
        var gt = group.GetType(); var sizeType = editor.GetType("UnityEditor.GameViewSize");
        var fixedSize = Enum.Parse(editor.GetType("UnityEditor.GameViewSizeType"), "FixedResolution");
        var viewType = editor.GetType("UnityEditor.GameView");
        var view = UnityEditor.EditorWindow.GetWindow(viewType);
        int original = (int)viewType.GetProperty("selectedSizeIndex").GetValue(view), added = -1;
        var select = viewType.GetMethod("SizeSelectionCallback");
        var ledger = new System.Collections.Generic.List<string> { "width,height,state,safeX,safeY,safeWidth,safeHeight,actions" };
        IEnumerator Capture(string state)
        {
            yield return null; yield return new WaitForEndOfFrame();
            float w = (float)type.GetProperty("UiWidth", Hidden).GetValue(court), h = (float)type.GetProperty("UiHeight", Hidden).GetValue(court);
            var actions = (string[])type.GetProperty("UiActions", Hidden).GetValue(court);
            var rects = actions.Select(a => (Rect)Call("UiActionRect", a)).ToArray();
            for (int i = 0; i < rects.Length; i++)
            {
                var r = rects[i]; if (r.width == 0) continue;
                Assert.That(r.xMin, Is.GreaterThanOrEqualTo(0), state + actions[i]);
                Assert.That(r.yMin, Is.GreaterThanOrEqualTo(0), state + actions[i]);
                Assert.That(r.xMax, Is.LessThanOrEqualTo(w), state + actions[i]);
                Assert.That(r.yMax, Is.LessThanOrEqualTo(h), state + actions[i]);
                Assert.That(r.height, Is.GreaterThanOrEqualTo(64), state + actions[i]);
                for (int j = i + 1; j < rects.Length; j++) Assert.That(r.Overlaps(rects[j]), Is.False, state + ": " + actions[i] + "/" + actions[j]);
            }
            var image = ScreenCapture.CaptureScreenshotAsTexture();
            try
            {
                string path = EvidenceDirectory + "/" + Screen.width + "x" + Screen.height + "-" + state + ".png";
                Assert.That(System.IO.File.Exists(path), Is.False);
                System.IO.File.WriteAllBytes(path, image.EncodeToPNG());
                var safe = Screen.safeArea;
                ledger.Add(string.Join(",", Screen.width, Screen.height, state, safe.x, safe.y, safe.width, safe.height, string.Join("|", actions)));
            }
            finally { UnityEngine.Object.Destroy(image); }
        }
        try
        {
            foreach (Vector2Int resolution in new[] { new Vector2Int(1280,720), new Vector2Int(1920,1080), new Vector2Int(2400,1080), new Vector2Int(1280,800) })
            {
                added = (int)gt.GetMethod("GetTotalCount").Invoke(group, null);
                gt.GetMethod("AddCustomSize").Invoke(group, new[] { Activator.CreateInstance(sizeType, fixedSize, resolution.x, resolution.y, "JustSmash QA") });
                select.Invoke(view, new object[] { added, null }); view.Repaint();
                for (int i = 0; i < 10 && (Screen.width != resolution.x || Screen.height != resolution.y); i++) yield return null;
                Assert.That(new Vector2Int(Screen.width, Screen.height), Is.EqualTo(resolution));
                yield return Boot(); yield return Capture("01-home");
                Click("Pengaturan"); yield return Capture("02-settings"); Click("Kembali");
                Click("Kredit"); yield return Capture("03-credits"); Click("Kembali");
                Click("Main"); Click("Tunggal"); yield return Capture("04-setup-singles");
                Click("Ganda"); yield return Capture("05-setup-doubles"); Click("Mulai");
                yield return Capture("06-warmup"); Click("Teruskan"); yield return Capture("07-ready"); Click("Teruskan");
                yield return Capture("08-serving"); type.GetProperty("ReplayMode").SetValue(court, 2);
                Call("Serve", false); yield return new WaitForSeconds(.12f); yield return Capture("09-rally");
                Click("Pause"); yield return Capture("10-pause"); Click("Menu"); yield return Capture("11-confirm"); Click("Tidak"); Click("Lanjut");
                type.GetField("serviceFlight", Hidden).SetValue(court, false);
                GameObject.Find("Shuttle").transform.position = new Vector3(0, .045f, 3);
                type.GetField("velocity", Hidden).SetValue(court, Vector3.down);
                LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[collision\] ground.*"));
                LogAssert.Expect(LogType.Log, "[scoring] point winner=0");
                type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
                Assert.That(Get<string>("MatchState"), Is.EqualTo("Point")); yield return Capture("12-point-challenge");
                Click("Challenge"); yield return Capture("13-reviewed");
                type.GetProperty("ReplayMode").SetValue(court, 1); Assert.That(Call("BeginReplay"), Is.EqualTo(true));
                yield return Capture("14-replay"); Click("Lewati"); Call("NextPoint");
                var match = Get<object>("Match"); var mt = match.GetType();
                while ((int)mt.GetField("GameNumber").GetValue(match) == 1) mt.GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
                Call("NextPoint"); yield return Capture("15-game-end"); Click("Teruskan");
                yield return Capture("16-interval"); Click("Teruskan"); yield return Capture("17-change-ends");
                Click("Teruskan"); yield return Capture("18-ready-swapped"); Click("Teruskan");
                while ((int)mt.GetField("Winner").GetValue(match) < 0) mt.GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
                type.GetProperty("MatchState").SetValue(court, "MatchEnd"); yield return Capture("19-result");
                Click("Main lagi"); yield return Capture("20-rematch");
                Click("Pause"); Click("Menu"); Click("Ya"); yield return Capture("21-home-return");
                Click("Latihan"); yield return Capture("22-training"); Click("Pause"); Click("Menu");
                select.Invoke(view, new object[] { original, null }); gt.GetMethod("RemoveCustomSize").Invoke(group, new object[] { added }); added = -1;
            }
            Assert.That(ledger.Count - 1, Is.EqualTo(88));
        }
        finally
        {
            select.Invoke(view, new object[] { original, null });
            if (added >= 0) gt.GetMethod("RemoveCustomSize").Invoke(group, new object[] { added });
            System.IO.File.WriteAllLines(EvidenceDirectory + "/screens.csv", ledger);
        }
#endif
    }
    [UnityTest, Category("Capture")] public IEnumerator CaptureResultScoresAcrossWinnersAndLegacy()
    {
        Assert.That(Application.isBatchMode, Is.False); yield return CaptureAt1280x720(); yield return Boot();
        var ledger = new System.Collections.Generic.List<string> { "format,winner,history,scores,file" };
        foreach (bool doubles in new[] { false, true }) foreach (int winner in new[] { 0, 1 }) foreach (bool legacy in new[] { false, true })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("StartMatch");
            var match = Get<object>("Match"); var mt = match.GetType();
            Action<int> point = side => mt.GetMethod("AwardPoint").Invoke(match, new object[] { side });
            foreach (var score in new[] { new[] { 16, 14 }, new[] { 20, 21 }, winner == 0 ? new[] { 15, 7 } : new[] { 7, 15 } })
            {
                int tied = Math.Min(score[0], score[1]);
                for (int i = 0; i < tied; i++) { point(0); point(1); }
                int side = score[0] > score[1] ? 0 : 1; for (int i = tied; i < score[side]; i++) point(side);
            }
            Call("NextPoint");
            if (legacy)
            {
                string old = System.Text.RegularExpressions.Regex.Replace(((string)Call("CaptureMatch")).Replace("\"version\":3", "\"version\":2"), "\"GamePoints\":\\[[^\\]]*\\],", "");
                Call("RestoreMatch", old); match = Get<object>("Match");
            }
            yield return null; yield return new WaitForEndOfFrame();
            string file = (doubles ? "doubles" : "singles") + "-winner" + winner + (legacy ? "-legacy" : "-complete") + ".png";
            var image = ScreenCapture.CaptureScreenshotAsTexture();
            try { System.IO.File.WriteAllBytes(EvidenceDirectory + "/" + file, image.EncodeToPNG()); }
            finally { UnityEngine.Object.Destroy(image); }
            ledger.Add(string.Join(",", doubles ? "doubles" : "singles", winner, legacy ? "legacy" : "complete", string.Join("|", (int[])mt.GetField("GamePoints").GetValue(match)), file));
        }
        Assert.That(ledger.Count - 1, Is.EqualTo(8)); System.IO.File.WriteAllLines(EvidenceDirectory + "/results.csv", ledger);
    }
    [UnityTest] public IEnumerator ResultNamesWinnerAndDisplaysExactOrUnavailableGameRows()
    {
        yield return Boot();
        var line = type.GetMethod("ResultGameLine", Hidden); var label = type.GetProperty("ResultWinnerLabel", Hidden);
        Assert.That(line, Is.Not.Null, "Result does not render completed game points"); Assert.That(label, Is.Not.Null);
        foreach (bool doubles in new[] { false, true }) foreach (int winner in new[] { 0, 1 })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("StartMatch");
            var match = Get<object>("Match"); var mt = match.GetType();
            for (int i = 0; i < 30; i++) mt.GetMethod("AwardPoint").Invoke(match, new object[] { winner });
            Call("NextPoint");
            string team = (string)type.GetMethod("TeamName", Hidden).Invoke(court, new object[] { winner });
            Assert.That(label.GetValue(court), Is.EqualTo("Pemenang: " + team));
            for (int i = 0; i < 2; i++) Assert.That(line.Invoke(court, new object[] { i }), Is.EqualTo("Game " + (i + 1) + (winner == 0 ? "    15 : 0" : "    0 : 15")));
            string old = System.Text.RegularExpressions.Regex.Replace(((string)Call("CaptureMatch")).Replace("\"version\":3", "\"version\":2"), "\"GamePoints\":\\[[^\\]]*\\],", "");
            Call("RestoreMatch", old);
            Assert.That(line.Invoke(court, new object[] { 0 }), Is.EqualTo("Game 1    Data lama tidak tersedia"));
            Assert.That(line.Invoke(court, new object[] { 1 }), Is.EqualTo(winner == 0 ? "Game 2    15 : 0" : "Game 2    0 : 15"));
        }
    }
    [UnityTest] public IEnumerator ResultFramesWinningTeamOutsidePanelWithoutMovingSimulationRoots()
    {
        yield return Boot();
        foreach (bool doubles in new[] { false, true }) foreach (int winner in new[] { 0, 1 })
        foreach (bool swapped in new[] { false, true })
        {
            Call("EnterTraining"); Call("ConfigureDiscipline", doubles); Call("BeginExhibition");
            var match = Get<object>("Match"); var mt = match.GetType();
            mt.GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
            while ((int)mt.GetField("Winner").GetValue(match) < 0) mt.GetMethod("AwardPoint").Invoke(match, new object[] { winner });
            type.GetProperty("MatchState").SetValue(court, "MatchEnd");
            var names = doubles ? new[] { "Player", "Opponent", "Partner", "OpponentPartner" } : new[] { "Player", "Opponent" };
            var roots = names.Select(n => GameObject.Find(n).transform).ToArray();
            float sign = (bool)mt.GetField("EndsSwapped").GetValue(match) ? -1 : 1;
            for (int i = 0; i < roots.Length; i++) roots[i].position = new Vector3(i < 2 ? 1.3f : -1.3f, 0, (i % 2 == 0 ? -2.2f : 2.2f) * sign);
            var positions = roots.Select(t => t.position).ToArray();
            foreach (float aspect in new[] { 1280f/720, 1920f/1080, 2400f/1080, 1280f/800 })
            {
                Camera.main.aspect = aspect; yield return null;
                for (int i = 0; i < roots.Length; i++)
                {
                    Assert.That(roots[i].position, Is.EqualTo(positions[i]), "Result camera must not stage simulation roots");
                    if (i % 2 != winner) continue;
                    foreach (Vector3 point in new[] { roots[i].position, roots[i].position + Vector3.up * 2.05f,
                        roots[i].TransformPoint(new Vector3(-.45f, 1, 0)), roots[i].TransformPoint(new Vector3(.55f, 1, 0)) })
                    {
                        Vector3 p = Camera.main.WorldToViewportPoint(point);
                        Assert.That(p.x, Is.InRange(.43f, .98f), names[i] + " clipped by result panel/edge: " + p);
                        Assert.That(p.y, Is.InRange(.03f, .97f), names[i] + " head/feet clipped: " + p);
                        Assert.That(p.z, Is.GreaterThan(0));
                    }
                    Assert.That(Camera.main.transform.position.z * roots[i].position.z, Is.GreaterThan(0), "Net lies between camera and winner");
                }
            }
            Camera.main.ResetAspect();
        }
    }
    [UnityTest] public IEnumerator MenuRacketsClearBakedHeadProjectionWithoutMovingRoots()
    {
        if (!Application.isBatchMode) yield return CaptureAt1280x720();
        yield return Boot();
        var camera = Camera.main; float aspectBefore = camera.aspect;
        int checkedActors = 0;
        float Cross(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;
        float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b) => Vector2.Distance(p,
            a + (b - a) * Mathf.Clamp01(Vector2.Dot(p - a, b - a) / Mathf.Max((b - a).sqrMagnitude, .00000001f)));
        bool Inside(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            float x = Cross(b - a, p - a), y = Cross(c - b, p - b), z = Cross(a - c, p - c);
            return !((x < 0 || y < 0 || z < 0) && (x > 0 || y > 0 || z > 0));
        }
        bool Near(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float pad) =>
            (Cross(b - a, c - a) * Cross(b - a, d - a) < 0 && Cross(d - c, a - c) * Cross(d - c, b - c) < 0)
            || Mathf.Min(Mathf.Min(DistanceToSegment(a, c, d), DistanceToSegment(b, c, d)),
                Mathf.Min(DistanceToSegment(c, a, b), DistanceToSegment(d, a, b))) <= pad;
        Assert.That(Near(new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(1, 0), 0), Is.True);
        Assert.That(Near(Vector2.zero, Vector2.right, Vector2.right * 2, Vector2.right * 3, .01f), Is.False);
        try
        {
            foreach (bool doubles in new[] { false, true })
            {
                Call("ConfigureDiscipline", doubles);
                foreach (string screenName in new[] { "Home", "Setup" })
                {
                    if (screenName == "Setup") Click("Main");
                    Assert.That(Get<string>("ScreenName"), Is.EqualTo(screenName));
                    var roots = (doubles ? new[] { "Player", "Partner", "Opponent", "OpponentPartner" } : new[] { "Player", "Opponent" })
                        .Select(n => GameObject.Find(n).transform).ToArray();
                    var positions = roots.Select(t => t.position).ToArray(); var rotations = roots.Select(t => t.rotation).ToArray();
                    if (!Application.isBatchMode)
                    {
                        camera.aspect = 1280f / 720; yield return new WaitForEndOfFrame();
                        var image = ScreenCapture.CaptureScreenshotAsTexture();
                        try { System.IO.File.WriteAllBytes(EvidenceDirectory + "/" + screenName + (doubles ? "-doubles" : "-singles") + ".png", image.EncodeToPNG()); }
                        finally { UnityEngine.Object.Destroy(image); }
                    }
                    foreach (float aspect in new[] { 1280f / 720, 1920f / 1080, 2400f / 1080, 1280f / 800 })
                    {
                        camera.aspect = aspect;
                        for (int i = 0; i < roots.Length; i++)
                        {
                            var model = roots[i].Find("AthleteVisual");
                            var head = model.GetComponentsInChildren<SkinnedMeshRenderer>().Single(s => s.name.Contains("head2"));
                            var mesh = new Mesh();
                            try
                            {
                                head.BakeMesh(mesh); Assert.That(mesh.vertexCount, Is.GreaterThan(0));
                                var face = mesh.vertices.Select(v => camera.WorldToViewportPoint(head.transform.TransformPoint(v))).ToArray();
                                Assert.That(face.All(p => p.z > 0), Is.True);
                                var faceRect = Rect.MinMaxRect(face.Min(p => p.x), face.Min(p => p.y), face.Max(p => p.x), face.Max(p => p.y));
                                var grip = model.GetComponentsInChildren<Transform>().Single(t => t.name == "RacketSocket");
                                foreach (var line in grip.GetComponentsInChildren<LineRenderer>())
                                {
                                    var points = Enumerable.Range(0, line.positionCount).Select(n => camera.WorldToViewportPoint(line.useWorldSpace ? line.GetPosition(n) : line.transform.TransformPoint(line.GetPosition(n)))).ToArray();
                                    Assert.That(points.All(p => p.z > 0), Is.True);
                                    float pad = Mathf.Max(line.startWidth, line.endWidth) * .5f / (2 * points.Min(p => p.z) * Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad * .5f)) + 2f / 720;
                                    var racketRect = Rect.MinMaxRect(points.Min(p => p.x) - pad / aspect, points.Min(p => p.y) - pad,
                                        points.Max(p => p.x) + pad / aspect, points.Max(p => p.y) + pad);
                                    if (!faceRect.Overlaps(racketRect)) continue;
                                    var triangles = mesh.triangles;
                                    bool overlaps = false;
                                    for (int edge = 0; edge < (line.loop ? points.Length : points.Length - 1) && !overlaps; edge++)
                                    {
                                        Vector2 a = Vector2.Scale(points[edge], new Vector2(aspect, 1));
                                        Vector2 b = Vector2.Scale(points[(edge + 1) % points.Length], new Vector2(aspect, 1));
                                        for (int t = 0; t < triangles.Length; t += 3)
                                        {
                                            Vector2 x = Vector2.Scale(face[triangles[t]], new Vector2(aspect, 1));
                                            Vector2 y = Vector2.Scale(face[triangles[t + 1]], new Vector2(aspect, 1));
                                            Vector2 z = Vector2.Scale(face[triangles[t + 2]], new Vector2(aspect, 1));
                                            if (Mathf.Abs(Cross(y - x, z - x)) < .00000001f) continue;
                                            if (Inside(a, x, y, z) || Inside(b, x, y, z) || Near(a, b, x, y, pad) || Near(a, b, y, z, pad) || Near(a, b, z, x, pad)) { overlaps = true; break; }
                                        }
                                    }
                                    Assert.That(overlaps, Is.False, screenName + " " + roots[i].name + " " + line.name + " overlaps projected head triangles at " + aspect);
                                }
                            }
                            finally { UnityEngine.Object.Destroy(mesh); }
                            Assert.That(roots[i].position, Is.EqualTo(positions[i]));
                            Assert.That(Quaternion.Angle(roots[i].rotation, rotations[i]), Is.LessThan(.001f));
                            checkedActors++;
                        }
                    }
                    if (screenName == "Setup") Click("Kembali");
                }
            }
            Assert.That(checkedActors, Is.EqualTo(48));
        }
        finally { camera.aspect = aspectBefore; }
    }
    [UnityTest] public IEnumerator DoublesSetupKeepsWholeAthletesInsideEveryApprovedViewport()
    {
        yield return Boot(); Click("Main"); Click("Ganda");
        var camera = Camera.main; float before = camera.aspect;
        try
        {
            foreach (float aspect in new[] { 1280f / 800, 1280f / 720, 1920f / 1080, 2400f / 1080 })
            {
                camera.aspect = aspect;
                foreach (string name in new[] { "Player", "Partner", "Opponent", "OpponentPartner" })
                foreach (var skin in GameObject.Find(name).GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    var mesh = new Mesh();
                    try
                    {
                        skin.BakeMesh(mesh);
                        var points = mesh.vertices.Select(v => camera.WorldToViewportPoint(skin.transform.TransformPoint(v))).ToArray();
                        Assert.That(points.All(p => p.z > 0), Is.True);
                        float minX = points.Min(p => p.x), maxX = points.Max(p => p.x);
                        TestContext.WriteLine(name + " " + skin.name + " aspect=" + aspect + " x=" + minX + ".." + maxX);
                        Assert.That(minX, Is.GreaterThan(.40f), name + " hidden by setup menu");
                        Assert.That(maxX, Is.LessThan(.99f), name + " mesh cropped at right edge: " + skin.name);
                        Assert.That(points.Min(p => p.y), Is.GreaterThan(.02f));
                        Assert.That(points.Max(p => p.y), Is.LessThan(.98f));
                    }
                    finally { UnityEngine.Object.Destroy(mesh); }
                }
            }
        }
        finally { camera.aspect = before; }
    }
    [UnityTest] public IEnumerator DoublesSetupShowsFourSeparateTorsoSilhouettes()
    {
        yield return Boot(); Click("Main"); Click("Ganda"); yield return null;
        var camera = Camera.main;
        var bodies = new[] { "Player", "Opponent", "Partner", "OpponentPartner" }.Select(n => GameObject.Find(n).transform).ToArray();
        foreach (float aspect in new[] { 16f/9, 20f/9, 16f/10 })
        {
            camera.aspect = aspect;
            for (int i = 0; i < bodies.Length; i++) for (int j = i+1; j < bodies.Length; j++)
            {
                float a = camera.WorldToViewportPoint(bodies[i].position + Vector3.up * 1.2f).x;
                float b = camera.WorldToViewportPoint(bodies[j].position + Vector3.up * 1.2f).x;
                Assert.That(Mathf.Abs(a-b) * aspect, Is.GreaterThan(.12f), bodies[i].name + " hides " + bodies[j].name);
            }
        }
        camera.ResetAspect();
    }
    [UnityTest] public IEnumerator ApprovedSceneUsesGreenCourtToonAthletesAndLargeHomeSubject()
    {
        yield return Boot();
        var floor = GameObject.Find("Court").GetComponent<Renderer>().sharedMaterial;
        Assert.That(floor.color.g, Is.GreaterThan(floor.color.b * 1.5f), "Owner requested green playing surface, not teal/blue");
        Assert.That(floor.shader.name, Is.EqualTo("JustSmash/CelCourt"));
        var player = GameObject.Find("Player").transform;
        var camera = Camera.main;
        float height = camera.WorldToViewportPoint(player.position + Vector3.up * 1.85f).y - camera.WorldToViewportPoint(player.position).y;
        Assert.That(height, Is.InRange(.55f, .70f), "Approved foreground athlete must dominate Home, not tiny court preview");
        Assert.That(camera.WorldToViewportPoint(player.position).x, Is.InRange(.58f, .88f));
        Click("Main"); Click("Ganda"); yield return null;
        float hostAspect = camera.aspect;
        try
        {
            foreach (float aspect in new[] { 1280f/720, 1920f/1080, 2400f/1080, 1280f/800 })
            {
                camera.aspect = aspect;
                foreach (string name in new[] { "Player", "Partner", "Opponent", "OpponentPartner" })
                {
                    var root = GameObject.Find(name).transform;
                    foreach (var skin in root.GetComponentsInChildren<SkinnedMeshRenderer>())
                        foreach (var material in skin.sharedMaterials) Assert.That(material.shader.name, Is.EqualTo("JustSmash/CelCourt"));
                    Vector3 head = camera.WorldToViewportPoint(root.position + Vector3.up * 1.7f);
                    Assert.That(head.x, Is.InRange(.40f, .99f), name + " hidden behind menu or screen edge at " + aspect);
                    Assert.That(head.y, Is.InRange(.1f, .9f));
                }
            }
        }
        finally { camera.aspect = hostAspect; }
        Click("Mulai"); Click("Teruskan"); Click("Teruskan"); yield return null;
        Assert.That(camera.fieldOfView, Is.EqualTo(48));
        Assert.That(player.forward.z, Is.GreaterThan(.99f));
        Assert.That(GameObject.Find("Shuttle").GetComponentsInChildren<Renderer>().Any(r => r.enabled), Is.True);
        Assert.That(Get<bool>("Flying"), Is.False);
        Click("Pause"); Click("Menu"); Click("Ya"); Click("Latihan"); yield return null;
        Assert.That(player.forward.z, Is.GreaterThan(.99f), "Training inherits menu pose");
        Assert.That(player.position.z, Is.EqualTo(-4).Within(.01f));
    }
    [UnityTest] public IEnumerator ApprovedBlueMenuUsesDisplayTypeAndPreviewActionHierarchy()
    {
        yield return Boot();
        var main = (Rect)Call("UiActionRect", "Main");
        Assert.That(main, Is.EqualTo(new Rect(46, 428, 354, 100)), "Approved main action must occupy lower-left comic composition");
        var font = Resources.Load<Font>("Fonts/Anton-Regular");
        Assert.That(font, Is.Not.Null, "Approved embeddable display font absent");
        var blue = (Color)type.GetField("panelColor", Hidden).GetValue(court);
        Assert.That(blue.b, Is.GreaterThan(.8f)); Assert.That(blue.r, Is.LessThan(.15f));
        Assert.That((string)type.GetMethod("ActionLabel", Hidden).Invoke(court, new object[] { "Main" }), Is.EqualTo("MAIN"));
        foreach (string action in new[] { "Main", "Latihan", "Pengaturan", "Kredit" })
        {
            var rect = (Rect)Call("UiActionRect", action);
            Assert.That(rect.height, Is.GreaterThanOrEqualTo(64));
            Assert.That(rect.xMax, Is.LessThanOrEqualTo(432));
            Assert.That(rect.yMax, Is.LessThanOrEqualTo((float)type.GetProperty("UiHeight", Hidden).GetValue(court) - 24));
        }
        Click("Main"); Click("Ganda");
        Assert.That((Rect)Call("UiActionRect", "Mulai"), Is.EqualTo(new Rect(46, 568, 340, 78)));
        Click("Mulai"); Click("Teruskan"); Click("Teruskan");
        Assert.That(Get<string>("FlowStage"), Is.EqualTo("Serving"));
        Assert.That(Get<bool>("Flying"), Is.False);
    }
    [UnityTest, Category("Career")] public IEnumerator CareerFinalSurvivesReopenAndBackupLockWithoutDoubleRewards()
    {
        string root = DataDirectory;
        string path = System.IO.Path.Combine(root, "careers", "slot-1-v1.json");
        string exhibition = System.IO.Path.Combine(root, "singles-v1.json");
        byte[] originalExhibition = System.IO.File.Exists(exhibition) ? System.IO.File.ReadAllBytes(exhibition) : null;
        yield return Boot();
        Click("Karier"); Click("Slot 1"); Click("Buat Atlet"); Click("Open Kota"); Click("Main laga");
        Assert.That(Get<bool>("DoublesMode"), Is.False);
        Assert.That(Get<string>("MatchSummary"), Does.StartWith("Official15 | Normal"));
        type.GetProperty("ReplayMode").SetValue(court, 2);
        void CloseFixtureMatch(int winner)
        {
            var m = Get<object>("Match");
            var mt = m.GetType();
            mt.GetField("Games").SetValue(m, winner == 0 ? new[] { 1, 0 } : new[] { 0, 1 });
            mt.GetField("GamePoints").SetValue(m, winner == 0 ? new[] { 15, 0 } : new[] { 0, 15 });
            mt.GetField("Points").SetValue(m, winner == 0 ? new[] { 14, 0 } : new[] { 0, 14 });
            mt.GetField("GameNumber").SetValue(m, 2);
            type.GetMethod("ScorePoint", Hidden).Invoke(court, new object[] { winner });
            Assert.That(((Rect)Call("UiActionRect", "Ke kalender")).width, Is.Zero, "Do not commit before the review window closes");
            type.GetMethod("ContinuePoint", Hidden).Invoke(court, new object[] { 3f });
            type.GetMethod("Update", Hidden).Invoke(court, null);
        }
        int Stat(string property) { var c = Get<object>("ActiveCareer"); return (int)c.GetType().GetProperty(property).GetValue(c); }
        CloseFixtureMatch(0);
        Assert.That(Stat("Points"), Is.Zero);
        byte[] semifinal = System.IO.File.ReadAllBytes(path);
        if (Environment.GetEnvironmentVariable("JUSTSMASH_QA_INTERRUPT") == "1")
        {
            System.IO.File.WriteAllText(System.IO.Path.Combine(EvidenceDirectory, "ready.txt"), path);
            yield return new WaitForSecondsRealtime(30);
            Assert.Fail("Expected the QA runner to terminate before teardown");
        }
        yield return Boot(); // Fresh runtime; only the slot carries the bracket.
        Click("Karier"); Click("Slot 1"); Click("Main laga");
        type.GetProperty("ReplayMode").SetValue(court, 2);
        using (var held = new System.IO.FileStream(path + ".bak", System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
        {
            CloseFixtureMatch(0);
            Assert.That(((Rect)Call("UiActionRect", "Ulang simpan")).width, Is.GreaterThan(0));
            Assert.That(System.IO.File.ReadAllBytes(path), Is.EqualTo(semifinal));
            Assert.That(Stat("Titles"), Is.Zero);
        }
        yield return Boot();
        Click("Karier"); Click("Slot 1"); Click("Lanjut laga");
        yield return null;
        Assert.That(Stat("Titles"), Is.EqualTo(1), "Recover the exact final score without replaying the match");
        Assert.That(Stat("Points"), Is.EqualTo(160));
        Assert.That(Stat("Prize"), Is.EqualTo(800));
        byte[] final = System.IO.File.ReadAllBytes(path);
        type.GetMethod("RecordCareerResult", Hidden).Invoke(court, null);
        Assert.That(System.IO.File.ReadAllBytes(path), Is.EqualTo(final));
        Click("Ke kalender");
        Assert.That(((Rect)Call("UiActionRect", "Sirkuit Nasional")).width, Is.GreaterThan(0));
        Click("Arsip"); Assert.That(Get<string>("ScreenName"), Is.EqualTo("CareerArchive")); Click("Kembali");
        yield return Boot();
        Click("Karier"); Click("Slot 1");
        Assert.That(Stat("Titles"), Is.EqualTo(1)); Assert.That(Stat("Prize"), Is.EqualTo(800));
        Click("Open Kota"); Click("Main laga"); type.GetProperty("ReplayMode").SetValue(court, 2);
        CloseFixtureMatch(1); Click("Ke kalender");
        Assert.That(Stat("Titles"), Is.EqualTo(1)); Assert.That(Stat("Points"), Is.EqualTo(200));
        Assert.That(Stat("Rank"), Is.EqualTo(56));
        Assert.That(((Rect)Call("UiActionRect", "World Circuit")).width, Is.Zero);
        Click("Latihan atlet"); Click("Tambah stamina");
        Assert.That((string)type.GetProperty("CareerRankingLabel", Hidden)?.GetValue(court), Does.Contain("No. 1 dunia"));
        Click("Sirkuit Nasional"); Click("Main laga");
        Assert.That(Get<int>("StaminaStat"), Is.EqualTo(71));
        Assert.That(Get<float>("Energy"), Is.EqualTo(101).Within(.001f));
        Assert.That(System.IO.File.Exists(exhibition) ? System.IO.File.ReadAllBytes(exhibition) : null, Is.EqualTo(originalExhibition));
    }
    [UnityTest, Category("Career")] public IEnumerator CareerReplayRoutesPointersAndLocksGameplay()
    {
        yield return Boot();
        Click("Karier"); Click("Slot 1"); Click("Buat Atlet"); Click("Open Kota"); Click("Main laga");
        foreach (int pointer in new[] { -2, 23 })
        {
            var replay = (IList)type.GetField("replay", Hidden).GetValue(court);
            var frame = type.GetMethod("CaptureFrame", Hidden).Invoke(court, null);
            for (int i = 0; i < 30; i++) replay.Add(frame);
            Assert.That(Call("BeginReplay"), Is.True);
            Assert.That(type.GetProperty("GameplayInputEnabled", Hidden).GetValue(court), Is.False, "Replay must block gameplay while routing UI pointers");
            var skip = (Rect)Call("UiActionRect", "Lewati");
            Assert.That(skip.width, Is.GreaterThanOrEqualTo(48));
            type.GetMethod("PointerDown", Hidden).Invoke(court, new object[] { pointer, skip.center });
            Assert.That(type.GetField("menuAction", Hidden).GetValue(court), Is.EqualTo("Lewati"));
            type.GetMethod("PointerUp", Hidden).Invoke(court, new object[] { pointer });
            Assert.That(((Rect)Call("UiActionRect", "Lewati")).width, Is.Zero);
        }
    }
    [UnityTest, Category("Career")] public IEnumerator CareerFeedCannotDiscardActiveMatch()
    {
        yield return Boot();
        Click("Karier"); Click("Slot 1"); Click("Buat Atlet"); Click("Open Kota"); Click("Main laga");
        var match = Get<object>("Match");
        Call("Feed", "Lob"); // Same command as the F hotkey.
        Assert.That(Get<object>("Match"), Is.SameAs(match));
        Assert.That(Get<string>("ScreenName"), Is.EqualTo("Playing"));
    }
    [UnityTest, Category("Career")] public IEnumerator CareerReviewResolvesBeforeReturningToCalendar()
    {
        yield return Boot();
        Click("Karier"); Click("Slot 1"); Click("Buat Atlet"); Click("Open Kota"); Click("Main laga");
        type.GetProperty("ReplayMode").SetValue(court, 2);
        var match = Get<object>("Match");
        var rules = match.GetType();
        rules.GetField("Games").SetValue(match, new[] { 1, 0 });
        rules.GetField("GameNumber").SetValue(match, 2);
        rules.GetField("GamePoints").SetValue(match, new[] { 15, 0 });
        rules.GetField("Points").SetValue(match, new[] { 14, 0 });
        type.GetField("beforePoint", Hidden).SetValue(court, JsonUtility.ToJson(match));
        type.GetField("lastLanding", Hidden).SetValue(court, new Vector3(0, 0, 3));
        type.GetField("announcedIn", Hidden).SetValue(court, true);
        type.GetField("reviewAvailable", Hidden).SetValue(court, true);
        type.GetMethod("ScorePoint", Hidden).Invoke(court, new object[] { 0 });
        Click("Pause");
        Assert.That(((Rect)Call("UiActionRect", "Ke kalender")).width, Is.Zero);
        Click("Selesaikan challenge");
        Click("Challenge");
        yield return null;
        byte[] saved = System.IO.File.ReadAllBytes(CareerSlotPath);
        var career = LoadCareer();
        Assert.That((IList)career.GetType().GetField("history").GetValue(career), Has.Count.EqualTo(1));
        Click("Ke kalender");
        Assert.That(Get<string>("ScreenName"), Is.EqualTo("CareerCalendar"));
        Assert.That(System.IO.File.ReadAllBytes(CareerSlotPath), Is.EqualTo(saved));
    }
    Type CareerStorage => type.Assembly.GetType("JustSmash.CareerStore");
    object LoadCareer() => CareerStorage.GetMethod("Load").Invoke(null, new object[] { 1 });
    void SaveCareer(object slot) => CareerStorage.GetMethod("Save").Invoke(null, new[] { slot });
    string CareerSlotPath => System.IO.Path.Combine(DataDirectory, "careers", "slot-1-v1.json");

    [UnityTest, Category("Career")] public IEnumerator CareerCorruptSnapshotFallsBackAndPreservesBackup()
    {
        yield return Boot();
        Click("Karier"); Click("Slot 1"); Click("Buat Atlet"); Click("Open Kota"); Click("Main laga");
        Click("Pause"); Click("Simpan"); Click("Ke kalender");
        byte[] backup = System.IO.File.ReadAllBytes(CareerSlotPath + ".bak");
        var broken = LoadCareer();
        broken.GetType().GetField("matchSnapshot").SetValue(broken, "{}");
        System.IO.File.WriteAllText(CareerSlotPath, JsonUtility.ToJson(broken));
        yield return Boot();
        Click("Karier"); Click("Slot 1"); Click("Lanjut laga");
        Assert.That(Get<string>("ScreenName"), Is.EqualTo("Playing"));
        Click("Pause"); Click("Simpan");
        Assert.That(System.IO.File.ReadAllBytes(CareerSlotPath + ".bak"), Is.EqualTo(backup));
    }
    [UnityTest, Category("Career")] public IEnumerator CareerStaleWriterCannotRollBackNewProgress()
    {
        yield return Boot();
        Click("Karier"); Click("Slot 1"); Click("Buat Atlet");
        var stale = LoadCareer();
        Click("Latihan atlet");
        byte[] current = System.IO.File.ReadAllBytes(CareerSlotPath);
        var error = Assert.Throws<TargetInvocationException>(() => SaveCareer(stale));
        Assert.That(error.InnerException, Is.TypeOf<InvalidOperationException>());
        Assert.That(System.IO.File.ReadAllBytes(CareerSlotPath), Is.EqualTo(current));
        Assert.That(LoadCareer().GetType().GetField("day").GetValue(LoadCareer()), Is.EqualTo(1));
    }
    [UnityTest, Category("Career")] public IEnumerator CareerFallbackDoesNotReuseCorruptPrimaryRevision()
    {
        yield return Boot();
        Click("Karier"); Click("Slot 1"); Click("Buat Atlet");
        Click("Latihan atlet");
        Click("Latihan atlet");
        var staleFromPrimary = LoadCareer();
        staleFromPrimary.GetType().GetField("matchSnapshot").SetValue(staleFromPrimary, "{}");
        System.IO.File.WriteAllText(CareerSlotPath, JsonUtility.ToJson(staleFromPrimary));
        staleFromPrimary.GetType().GetField("matchSnapshot").SetValue(staleFromPrimary, null);
        yield return Boot();
        Click("Karier"); Click("Slot 1"); Click("Open Kota");
        byte[] recovered = System.IO.File.ReadAllBytes(CareerSlotPath);
        var error = Assert.Throws<TargetInvocationException>(() => SaveCareer(staleFromPrimary));
        Assert.That(error.InnerException, Is.TypeOf<InvalidOperationException>());
        Assert.That(System.IO.File.ReadAllBytes(CareerSlotPath), Is.EqualTo(recovered));
        Assert.That(LoadCareer().GetType().GetField("tier").GetValue(LoadCareer()), Is.EqualTo(0));
    }
    [UnityTest, Category("Career")] public IEnumerator CareerCorruptPendingDoesNotBlockSlotLoadOrSave()
    {
        yield return Boot();
        Click("Karier"); Click("Slot 1"); Click("Buat Atlet"); Click("Open Kota"); Click("Main laga");
        Click("Pause"); Click("Simpan"); Click("Ke kalender");
        System.IO.File.WriteAllText(CareerSlotPath + ".pending", "{corrupt");
        yield return Boot();
        Click("Karier");
        Assert.That((string)type.GetMethod("CareerLabel", Hidden).Invoke(court, new object[] { "Slot 1" }), Does.Not.Contain("Gagal dibaca"));
        Click("Slot 1"); Click("Lanjut laga");
        Assert.That(Get<string>("ScreenName"), Is.EqualTo("Playing"));
        type.GetProperty("ReplayMode").SetValue(court, 2);
        var m = Get<object>("Match");
        var mt = m.GetType();
        mt.GetField("Games").SetValue(m, new[] { 1, 0 });
        mt.GetField("GamePoints").SetValue(m, new[] { 15, 0 });
        mt.GetField("Points").SetValue(m, new[] { 14, 0 });
        mt.GetField("GameNumber").SetValue(m, 2);
        type.GetMethod("ScorePoint", Hidden).Invoke(court, new object[] { 0 });
        type.GetMethod("ContinuePoint", Hidden).Invoke(court, new object[] { 3f });
        type.GetMethod("Update", Hidden).Invoke(court, null);
        Click("Ke kalender");
        Assert.That(Get<string>("ScreenName"), Is.EqualTo("CareerCalendar"));
        Assert.That((IList)LoadCareer().GetType().GetField("history").GetValue(LoadCareer()), Has.Count.EqualTo(1));
    }
    [UnityTest, Category("Career")] public IEnumerator CareerOutfitsSurviveTwoSlotsWithoutTintingOpponentsOrExhibition()
    {
        Color Jersey(string actor, bool patterned)
        {
            var skin = GameObject.Find(actor).GetComponentsInChildren<SkinnedMeshRenderer>().Single(r => r.name.Contains("tshirt"));
            var block = new MaterialPropertyBlock(); skin.GetPropertyBlock(block);
            if (patterned)
            {
                Assert.That(block.GetFloat("_Flat"), Is.Zero);
                Assert.That(block.GetTexture("_MainTex"), Is.Not.Null);
            }
            return block.GetColor("_Color");
        }
        yield return Boot();
        var blue = Jersey("Player", false); var orange = Jersey("Opponent", false);
        Click("Karier"); Click("Slot 1"); Click("Ubah Penampilan");
        Click("Model kaus"); Click("Warna kaus");
        var first = Jersey("Player", true);
        var player = GameObject.Find("Player").transform;
        var front = player.rotation;
        Click("Putar"); Assert.That(Quaternion.Angle(front, player.rotation), Is.EqualTo(180).Within(.1f));
        Click("Simpan penampilan"); Click("Buat Atlet"); Click("Kembali");
        Click("Slot 2"); Click("Ubah Penampilan"); Click("Warna kaus"); Click("Warna kaus");
        var second = Jersey("Player", false);
        Assert.That(second, Is.Not.EqualTo(first));
        Click("Simpan penampilan"); Click("Buat Atlet");
        yield return Boot();
        Click("Karier"); Click("Slot 2"); Click("Ubah Penampilan");
        Assert.That(Jersey("Player", false), Is.EqualTo(second));
        Click("Kembali"); Click("Kembali"); Click("Slot 1"); Click("Ubah Penampilan");
        Assert.That(Jersey("Player", true), Is.EqualTo(first));
        Click("Kembali"); Click("Open Kota"); Click("Main laga");
        Assert.That(Jersey("Player", true), Is.EqualTo(first));
        Assert.That(Jersey("Opponent", false), Is.EqualTo(orange));
        Click("Teruskan"); Click("Teruskan"); Call("Serve", true);
        yield return new WaitForSeconds(.2f);
        var match = Get<object>("Match"); var mt = match.GetType();
        mt.GetField("Games").SetValue(match, new[] { 0, 1 });
        mt.GetField("GamePoints").SetValue(match, new[] { 0, 15 });
        mt.GetField("Points").SetValue(match, new[] { 0, 14 });
        mt.GetField("GameNumber").SetValue(match, 2);
        type.GetField("flying", Hidden).SetValue(court, false);
        type.GetMethod("ScorePoint", Hidden).Invoke(court, new object[] { 1 });
        type.GetMethod("ContinuePoint", Hidden).Invoke(court, new object[] { 3f });
        Assert.That(((Rect)Call("UiActionRect", "Lewati")).width, Is.GreaterThan(0));
        Assert.That(Jersey("Player", true), Is.EqualTo(first));
        Click("Lewati"); yield return null;
        Assert.That(Jersey("Player", true), Is.EqualTo(first));
        Assert.That(Camera.main.WorldToViewportPoint(GameObject.Find("Player").transform.position + Vector3.up).x, Is.InRange(.5f, .95f), "Career result must frame the custom athlete even after a loss");
        Click("Ke kalender"); Click("Kembali"); Click("Kembali");
        Click("Main"); Click("Tunggal"); Click("Mulai");
        Assert.That(Jersey("Player", false), Is.EqualTo(blue));
        Assert.That(Jersey("Opponent", false), Is.EqualTo(orange));
    }
    [UnityTest, Category("Career")] public IEnumerator CareerAndBothExhibitionFormatsOpenFromHome()
    {
        foreach (bool doubles in new[] { false, true })
        {
            yield return Boot();
            Click("Karier");
            Assert.That(Get<string>("ScreenName"), Is.EqualTo("CareerSlots"));
            Click("Kembali");
            Click("Main"); Click(doubles ? "Ganda" : "Tunggal"); Click("Mulai");
            Assert.That(Get<bool>("DoublesMode"), Is.EqualTo(doubles));
            Assert.That(Get<string>("EventName"), Is.EqualTo("JUST SMASH OPEN"));
            Assert.That(Get<string>("FlowStage"), Is.EqualTo("Warmup"));
            Click("Teruskan"); Click("Teruskan");
            Assert.That(Get<string>("FlowStage"), Is.EqualTo("Serving"));
            Assert.That(Get<bool>("Flying"), Is.False);
            Assert.That(Get<string>("ScreenName"), Is.EqualTo("Playing"));
            string snapshot = (string)Call("CaptureMatch"); Call("RestoreMatch", snapshot);
            Assert.That(Get<bool>("DoublesMode"), Is.EqualTo(doubles));
            Click("Pause"); Click("Pengaturan"); Click("Kembali"); Click("Lanjut");
            var match = Get<object>("Match");
            while ((int)match.GetType().GetField("Winner").GetValue(match) < 0)
                match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
            Click("Main lagi"); Assert.That(Get<bool>("DoublesMode"), Is.EqualTo(doubles));
            Click("Pause"); Click("Menu"); Click("Ya");
            Assert.That(Get<string>("ScreenName"), Is.EqualTo("Home"));
            Assert.That(((Rect)Call("UiActionRect", "Karier")).width, Is.GreaterThan(0));
            if (!Application.isBatchMode)
            {
                yield return null; yield return new WaitForEndOfFrame();
                var image = ScreenCapture.CaptureScreenshotAsTexture();
                System.IO.File.WriteAllBytes(EvidenceDirectory + "/home-no-career-" + (doubles ? "doubles" : "singles") + ".png", image.EncodeToPNG());
                UnityEngine.Object.Destroy(image);
            }
        }
    }
    [UnityTest] public IEnumerator GameplayControlsUseSafeCanvasAndNeverOverlap()
    {
        yield return Boot();
        var layout = type.GetMethod("GameplayRects", BindingFlags.Public | BindingFlags.Static);
        Assert.That(layout, Is.Not.Null, "Control layout still tied to 720px height");
        foreach (Vector2 size in new[] { new Vector2(1280, 720), new Vector2(2340 - 160, 1080 - 24), new Vector2(2400 - 160, 1080 - 24), new Vector2(960, 720) })
        {
            float scale = (float)type.GetMethod("CanvasScale").Invoke(null, new object[] { size });
            Vector2 canvas = size / scale;
            var rects = (Rect[])layout.Invoke(null, new object[] { canvas });
            Assert.That(rects.Length, Is.EqualTo(8), "Joystick, five shots, jump, serve");
            foreach (Rect rect in rects)
            {
                Assert.That(rect.width, Is.GreaterThanOrEqualTo(48)); Assert.That(rect.height, Is.GreaterThanOrEqualTo(48));
                Assert.That(rect.xMin, Is.GreaterThanOrEqualTo(0)); Assert.That(rect.yMin, Is.GreaterThanOrEqualTo(0));
                Assert.That(rect.xMax, Is.LessThanOrEqualTo(canvas.x)); Assert.That(rect.yMax, Is.LessThanOrEqualTo(canvas.y));
            }
            for (int i = 0; i < 7; i++) for (int j = i + 1; j < 7; j++) Assert.That(rects[i].Overlaps(rects[j]), Is.False, i + "/" + j);
            Assert.That(rects[0].center.y, Is.EqualTo(canvas.y - 132));
            Assert.That(rects[6].y, Is.EqualTo(canvas.y - 116));
        }
    }
    [UnityTest] public IEnumerator LineReviewStaysInRightHudLaneAndUsesRealPointerAction()
    {
        yield return Boot(); Click("Main"); Click("Ganda"); Click("Mulai"); Click("Teruskan"); Click("Teruskan");
        type.GetProperty("ReplayMode").SetValue(court, 2);
        foreach (bool swapped in new[] { false, true })
        {
            var match = Get<object>("Match"); var mt = match.GetType();
            mt.GetField("EndsSwapped").SetValue(match, swapped); Call("NextPoint");
            while (Get<string>("FlowStage") == "Ready" || Get<string>("FlowStage") == "ChangeEnds") Call("AdvanceFlow");
            Call("Serve", false);
            type.GetField("serviceFlight", Hidden).SetValue(court, false);
            GameObject.Find("Shuttle").transform.position = new Vector3(-1, .045f, swapped ? -3 : 3);
            type.GetField("velocity", Hidden).SetValue(court, Vector3.down);
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[collision\] ground.*"));
            type.GetMethod("FixedUpdate", Hidden).Invoke(court, null);
            Assert.That(Get<string>("MatchState"), Is.EqualTo("Point"));
            var review = (Rect)Call("UiActionRect", "Challenge"); var pause = (Rect)Call("UiActionRect", "Pause");
            float width = (float)type.GetProperty("UiWidth", Hidden).GetValue(court);
            float height = (float)type.GetProperty("UiHeight", Hidden).GetValue(court);
            Assert.That(review.xMin, Is.GreaterThanOrEqualTo(width - 324), "Line-review button covers far athletes instead of right HUD lane");
            Assert.That(review.xMax, Is.LessThanOrEqualTo(width - 24));
            Assert.That(review.yMin, Is.GreaterThanOrEqualTo(pause.yMax + 16));
            Assert.That(review.yMax, Is.LessThanOrEqualTo(height - 326));
            var font = new GUIStyle { font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"), fontSize = 26, fontStyle = FontStyle.Bold };
            Assert.That(font.CalcSize(new GUIContent("Tinjau garis (2)")).x + 32, Is.LessThanOrEqualTo(review.width));
            if (!Application.isBatchMode)
            {
                yield return null; yield return new WaitForEndOfFrame();
                var image = ScreenCapture.CaptureScreenshotAsTexture();
                System.IO.File.WriteAllBytes(EvidenceDirectory + "/challenge-hud-after1-" + (swapped ? "swapped" : "normal") + ".png", image.EncodeToPNG());
                UnityEngine.Object.Destroy(image);
            }
            int before = ((int[])mt.GetField("Challenges").GetValue(match))[0];
            Click("Challenge");
            Assert.That(((int[])mt.GetField("Challenges").GetValue(match))[0], Is.EqualTo(before - 1));
            Assert.That((Rect)Call("UiActionRect", "Challenge"), Is.EqualTo(Rect.zero));
        }
    }
    [UnityTest] public IEnumerator JumpLabelFitsSingleLineWithPadding()
    {
        yield return Boot();
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        var style = new GUIStyle { font = font, fontSize = 26, fontStyle = FontStyle.Bold };
        float width = style.CalcSize(new GUIContent("Lompat")).x;
        var rects = (Rect[])type.GetMethod("GameplayRects").Invoke(null, new object[] { new Vector2(1280, 720) });
        Assert.That(rects[6].width, Is.GreaterThanOrEqualTo(width + 16), "Lompat must fit one line with 8-unit side padding");
    }
    [UnityTest] public IEnumerator MatchIdentityFollowsSetupArenaHudAndResult()
    {
        yield return Boot();
        Assert.That(type.GetProperty("EventName"), Is.Not.Null, "Home and match still lack shared event identity");
        Assert.That(Get<string>("EventName"), Is.EqualTo("JUST SMASH OPEN"));
        Click("Main"); Click("Hard"); Click("Mulai");
        Assert.That(Get<string>("OpponentName"), Is.EqualTo("Raka Pradana"));
        Assert.That(Get<string>("MatchTitle"), Is.EqualTo("TUNGGAL PUTRA"));
        Click("Teruskan"); Click("Teruskan");
        Assert.That(Get<string>("FlowStage"), Is.EqualTo("Serving"));
        Assert.That(Get<bool>("Flying"), Is.False);
        var match = Get<object>("Match");
        for (int i = 0; i < 30; i++) match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
        yield return null;
        Assert.That(GameObject.Find("ArenaScoreboard").GetComponentInChildren<TextMesh>().text, Does.Contain("2 : 0"));
        Assert.That((Rect)type.GetMethod("UiActionRect").Invoke(court, new object[] { "Main lagi" }), Is.Not.EqualTo(Rect.zero));
        Click("Menu"); Assert.That(Get<string>("ScreenName"), Is.EqualTo("Home"));
    }
    [UnityTest] public IEnumerator DisciplineSelectionUpdatesArenaAndFourAthletePreview()
    {
        yield return Boot(); Click("Main"); Click("Ganda"); yield return null;
        var board = GameObject.Find("ArenaScoreboard").GetComponentInChildren<TextMesh>();
        Assert.That(board.text, Does.Contain("GANDA PUTRA"), "Setup arena still advertises singles");
        foreach (string name in new[] { "Player", "Partner", "Opponent", "OpponentPartner" })
            Assert.That(GameObject.Find(name), Is.Not.Null, "Preview missing " + name);
        Click("Tunggal"); yield return null;
        Assert.That(board.text, Does.Contain("TUNGGAL PUTRA"));
        Assert.That(GameObject.Find("Partner"), Is.Null); Assert.That(GameObject.Find("OpponentPartner"), Is.Null);
    }
    [UnityTest] public IEnumerator DoublesTeamLabelsFitSetupAndRemainPairedThroughResult()
    {
        yield return Boot(); Click("Main"); Click("Ganda");
        var label = type.GetMethod("TeamName", Hidden); var layout = type.GetMethod("SetupTeamRect", Hidden);
        Assert.That(label, Is.Not.Null, "HUD and result still name singles in doubles");
        Assert.That(layout, Is.Not.Null, "Setup still squeezes four names into one row");
        var style = new GUIStyle { font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"), fontSize = 26 };
        Func<int, string> team = side => (string)label.Invoke(court, new object[] { side });
        Func<int, Rect> row = side => (Rect)layout.Invoke(court, new object[] { side });
        foreach (string difficulty in new[] { "Easy", "Normal", "Hard" })
        {
            Click(difficulty); Assert.That(team(0), Is.EqualTo("Kamu + Dimas"));
            Assert.That(team(1), Is.EqualTo((difficulty == "Hard" ? "Raka" : difficulty == "Easy" ? "Bima" : "Arga") + " + Fajar"));
            for (int side = 0; side < 2; side++) Assert.That(style.CalcSize(new GUIContent(team(side).ToUpperInvariant())).x, Is.LessThan(row(side).width));
        }
        Assert.That(row(0).Overlaps(row(1)), Is.False);
        Assert.That(row(1).yMax + 48, Is.LessThanOrEqualTo(((Rect)Call("UiActionRect", "Easy")).yMin));
        Click("Mulai"); Assert.That(team(1), Is.EqualTo("Raka + Fajar"));
        var match = Get<object>("Match");
        while ((int)match.GetType().GetField("Winner").GetValue(match) < 0)
            match.GetType().GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
        Assert.That(team(0), Is.EqualTo("Kamu + Dimas")); Assert.That(team(1), Is.EqualTo("Raka + Fajar"));
        Click("Menu"); Click("Main"); Click("Tunggal");
        Assert.That(team(0), Is.EqualTo("Kamu")); Assert.That(team(1), Is.EqualTo("Raka Pradana"));
    }
    [UnityTest] public IEnumerator ChangeEndsCameraClearsVenueAndMenuRestoresHomeView()
    {
        yield return Boot();
        Vector3 homePosition = Camera.main.transform.position; Quaternion homeRotation = Camera.main.transform.rotation;
        Click("Main"); Click("Ganda"); Click("Mulai");
        var match = Get<object>("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, true);
        Call("NextPoint"); yield return null;
        var camera = Camera.main.transform;
        foreach (string name in new[] { "HallBackWall", "VenueName", "VenueStripe", "ArenaScoreboard" })
        {
            var bounds = GameObject.Find(name).GetComponent<Renderer>().bounds;
            Assert.That(bounds.min.z, Is.GreaterThan(camera.position.z + .3f), name + " blocks court after change ends");
        }
        Click("Pause"); Click("Menu"); Click("Ya");
        Assert.That(Get<string>("ScreenName"), Is.EqualTo("Home"));
        Assert.That(Vector3.Distance(Camera.main.transform.position, homePosition), Is.LessThan(.01f), "Home keeps swapped-end camera");
        Assert.That(Quaternion.Angle(Camera.main.transform.rotation, homeRotation), Is.LessThan(.1f));
    }
    [UnityTest] public IEnumerator MenuReturnResetsSwappedAthletesToNeutralPreview()
    {
        foreach (bool doubles in new[] { false, true })
        foreach (bool completed in new[] { false, true })
        {
            yield return Boot(); Click("Main"); if (doubles) Click("Ganda"); Click("Mulai");
            var match = Get<object>("Match"); var mt = match.GetType();
            mt.GetField("EndsSwapped").SetValue(match, true); Call("NextPoint");
            if (completed)
            {
                while ((int)mt.GetField("Winner").GetValue(match) < 0) mt.GetMethod("AwardPoint").Invoke(match, new object[] { 0 });
                type.GetProperty("MatchState").SetValue(court, "MatchEnd");
            }
            else { Click("Teruskan"); Click("Teruskan"); }
            yield return null;
            var names = doubles ? new[] { "Player", "Opponent", "Partner", "OpponentPartner" } : new[] { "Player", "Opponent" };
            if (!completed) { Click("Pause"); Click("Menu"); Click("Ya"); } else Click("Menu");
            yield return null;
            Assert.That(Get<string>("ScreenName"), Is.EqualTo("Home")); Assert.That(Get<object>("Match"), Is.Null);
            foreach (string name in names)
            {
                var root = GameObject.Find(name).transform; bool near = name == "Player" || name == "Partner";
                Assert.That(root.position.z * (near ? -1 : 1), Is.GreaterThanOrEqualTo(1), name + " remains on swapped side in Home");
                float yaw = name == "Player" ? 145 : name == "Partner" ? 160 : name == "OpponentPartner" ? 195 : 180;
                Assert.That(Quaternion.Angle(root.rotation, Quaternion.Euler(0, yaw, 0)), Is.LessThan(.1f), name + " fails approved Home staging");
                Assert.That(root.position.y, Is.EqualTo(0));
                var driver = root.Find("AthleteVisual").GetComponents<MonoBehaviour>().Single(x => x.GetType().Name == "AthleteVisual");
                Assert.That((string)driver.GetType().GetProperty("CurrentClip").GetValue(driver), Is.EqualTo("Ready"), name + " keeps match pose");
                var hand = driver.transform.Find("Root/Hips/Spine/Spine1/LeftShoulder/LeftArm/LeftForeArm/LeftHand");
                Assert.That(root.InverseTransformPoint(hand.position).y, Is.LessThan(1.4f), name + " keeps victory gesture in Home");
            }
            if (doubles) Assert.That(GameObject.Find("DoublesSuggestedPosition"), Is.Null);
            if (!Application.isBatchMode)
            {
                yield return new WaitForEndOfFrame(); var image = ScreenCapture.CaptureScreenshotAsTexture();
                System.IO.File.WriteAllBytes(EvidenceDirectory + "/home-preview-" + (doubles ? "doubles" : "singles") + "-" + (completed ? "result" : "abandon") + ".png", image.EncodeToPNG()); UnityEngine.Object.Destroy(image);
            }
            Click("Main"); Assert.That(Get<bool>("DoublesMode"), Is.EqualTo(doubles));
        }
    }
    [UnityTest] public IEnumerator DoublesSetupBuildsFourAthletesAndPartnerReturnsIncomingShot()
    {
        yield return Boot(); Click("Main");
        Assert.That((Rect)type.GetMethod("UiActionRect").Invoke(court, new object[] { "Ganda" }), Is.Not.EqualTo(Rect.zero), "Men's doubles absent from setup");
        Click("Ganda"); Click("Mulai"); Click("Teruskan"); Click("Teruskan");
        Assert.That(Get<string>("MatchTitle"), Is.EqualTo("GANDA PUTRA"));
        var match = Get<object>("Match"); var mt = match.GetType();
        Assert.That((bool)mt.GetField("Doubles").GetValue(match), Is.True);
        var partner = GameObject.Find("Partner"); var second = GameObject.Find("OpponentPartner");
        Assert.That(partner, Is.Not.Null); Assert.That(second, Is.Not.Null);
        Assert.That(partner.GetComponentsInChildren<SkinnedMeshRenderer>().Any(r => r.enabled), Is.True);
        Assert.That(second.GetComponentsInChildren<SkinnedMeshRenderer>().Any(r => r.enabled), Is.True);
        Call("Serve", false);
        var player = GameObject.Find("Player").transform; player.position = new Vector3(-2.8f, 0, -6);
        partner.transform.position = new Vector3(1.2f, 0, -3);
        GameObject.Find("Shuttle").transform.position = partner.transform.position + new Vector3(.35f, 1.15f, .35f);
        type.GetField("lastHitter", Hidden).SetValue(court, 1); type.GetField("lastPlayer", Hidden).SetValue(court, 1);
        type.GetField("serviceFlight", Hidden).SetValue(court, false);
        type.GetField("velocity", Hidden).SetValue(court, Vector3.down * 2);
        for (int tick = 0; tick < 35 && (int)type.GetField("lastHitter", Hidden).GetValue(court) == 1; tick++)
            yield return new WaitForFixedUpdate();
        Assert.That((int)type.GetField("lastHitter", Hidden).GetValue(court), Is.Zero, "Partner must hit, not decorative clone");
        Assert.That(Get<Vector3>("ShuttleVelocity").z, Is.GreaterThan(0));
        string saved = (string)Call("CaptureMatch"); Vector3 position = partner.transform.position;
        Call("StartMatch"); Call("RestoreMatch", saved);
        Assert.That(GameObject.Find("Partner").transform.position, Is.EqualTo(position));
        Assert.That((bool)Get<object>("Match").GetType().GetField("Doubles").GetValue(Get<object>("Match")), Is.True);
        Click("Pause"); Click("Menu"); Click("Ya"); Click("Main"); Click("Tunggal"); Click("Mulai");
        Assert.That(GameObject.Find("Partner"), Is.Null, "Singles must disable extra players");
    }
    [UnityTest] public IEnumerator DoublesFormationFollowsAttackDefenseAndNetTransitionWithHints()
    {
        yield return Boot(); Click("Main"); Click("Ganda"); Click("Mulai"); Click("Teruskan"); Click("Teruskan");
        var target = type.GetMethod("FormationTarget", Hidden);
        Func<int, Vector3> position = id => (Vector3)target.Invoke(court, new object[] { id });
        type.GetField("lastPlayer", Hidden).SetValue(court, 0); type.GetField("lastHitter", Hidden).SetValue(court, 0);
        type.GetField("lastStroke", Hidden).SetValue(court, "Smash");
        Assert.That(position(0).z, Is.LessThan(position(2).z)); Assert.That(Get<string>("Formation"), Is.EqualTo("Front-Back"));
        type.GetField("lastStroke", Hidden).SetValue(court, "Clear");
        Assert.That(position(0).z, Is.EqualTo(position(2).z)); Assert.That(Get<string>("Formation"), Is.EqualTo("Side-by-Side"));
        type.GetField("lastStroke", Hidden).SetValue(court, "Net");
        Assert.That(position(0).z, Is.GreaterThan(position(2).z), "Blocker follows net, partner covers rear");
        Assert.That(Get<string>("Formation"), Is.EqualTo("Free Rotation"));
        Assert.That(type.GetProperty("SuggestedPosition"), Is.Not.Null, "No positioning hint for human player");
        yield return null;
        Assert.That(GameObject.Find("DoublesSuggestedPosition"), Is.Not.Null);
    }
    [UnityTest] public IEnumerator DoublesServingSidesAndServiceLimitsSurviveSnapshotAndReplay()
    {
        yield return Boot(); Click("Main"); Click("Ganda"); Click("Mulai"); Click("Teruskan"); Click("Teruskan");
        var match = Get<object>("Match"); var mt = match.GetType();
        var service = type.GetMethod("LegalServiceLanding", Hidden);
        Assert.That(service.Invoke(court, new object[] { new Vector3(-3, .04f, 5.8f) }), Is.EqualTo(true));
        Assert.That(service.Invoke(court, new object[] { new Vector3(-3, .04f, 6.2f) }), Is.EqualTo(false), "Doubles long service line");
        Assert.That(type.GetMethod("LandingInside", Hidden).Invoke(court, new object[] { new Vector3(3, .04f, 6.2f) }), Is.EqualTo(true), "Full doubles boundary during rally");
        foreach (int side in new[] { 1, 0, 1, 0 })
        {
            mt.GetMethod("AwardPoint").Invoke(match, new object[] { side }); Call("NextPoint");
            string saved = (string)Call("CaptureMatch"); Call("RestoreMatch", saved);
            match = Get<object>("Match");
            Assert.That(mt.GetProperty("ServerPlayer").GetValue(match), Is.InRange(0, 3));
            Assert.That((string)Call("CaptureMatch"), Does.Contain("\"Doubles\":true"));
        }
        Call("AdvanceFlow");
        type.GetMethod("BotServe", Hidden).Invoke(court, new object[] { (int)mt.GetProperty("ServerPlayer").GetValue(match) });
        yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
        string live = (string)Call("CaptureMatch");
        Call("BeginReplay"); yield return null; Call("SkipReplay");
        Assert.That(Vector3.Distance(GameObject.Find("Partner").transform.position, GetPartnerPosition(live)), Is.LessThan(.15f));
        Call("RestoreMatch", live);
        string beforeInvalid = (string)Call("CaptureMatch");
        string invalid = live.Replace("\"lastPlayer\":", "\"lastPlayer\":99,\"ignoredPlayer\":");
        Assert.Throws<TargetInvocationException>(() => Call("RestoreMatch", invalid));
        Assert.That((string)Call("CaptureMatch"), Is.EqualTo(beforeInvalid), "Invalid doubles save cannot mutate live state");
    }
    [Serializable] class ExtraLocation { public bool present; public Vector3 position; }
    [Serializable] class PoseLocation { public ExtraLocation partner; }
    [Serializable] class MatchLocation { public PoseLocation livePose; }
    static Vector3 GetPartnerPosition(string json) => JsonUtility.FromJson<MatchLocation>(json).livePose.partner.position;
    [UnityTest] public IEnumerator DoublesAthletesShareFatigueCostsAndSaveAllFourEnergyValues()
    {
        yield return Boot(); Click("Main"); Click("Ganda"); Click("Mulai"); Click("Teruskan"); Click("Teruskan");
        var energy = type.GetMethod("AthleteEnergy", Hidden);
        Assert.That(energy, Is.Not.Null, "Doubles bots have no per-athlete stamina");
        Call("Serve", true);
        float[] before = Enumerable.Range(0, 4).Select(id => (float)energy.Invoke(court, new object[] { id })).ToArray();
        yield return new WaitForFixedUpdate();
        for (int id = 0; id < 4; id++)
            Assert.That((float)energy.Invoke(court, new object[] { id }), Is.LessThan(before[id]), "Rally drain athlete " + id);
        var fatigue = type.GetMethod("AthleteFatigueSpeed", Hidden);
        Assert.That(fatigue, Is.Not.Null);
        for (int id = 0; id < 4; id++)
            Assert.That((float)fatigue.Invoke(court, new object[] { id }), Is.EqualTo((float)fatigue.Invoke(court, new object[] { 0 })).Within(.001f));
        string saved = (string)Call("CaptureMatch");
        var original = Enumerable.Range(0, 4).Select(id => (float)energy.Invoke(court, new object[] { id })).ToArray();
        Call("StartMatch"); Call("RestoreMatch", saved);
        for (int id = 0; id < 4; id++) Assert.That((float)energy.Invoke(court, new object[] { id }), Is.EqualTo(original[id]));
    }
    [UnityTest] public IEnumerator DoublesAttackProducesDescendingSmashWithoutChangingContactReach()
    {
        yield return Boot(); Click("Main"); Click("Ganda"); Click("Hard"); Click("Mulai"); Click("Teruskan"); Click("Teruskan");
        ((Behaviour)court).enabled = false; Call("Serve", false);
        var root = GameObject.Find("Opponent").transform;
        root.position = new Vector3(1.2f, 0, 2.7f);
        GameObject.Find("OpponentPartner").transform.position = new Vector3(-2.7f, 0, 6);
        var shuttle = GameObject.Find("Shuttle").transform;
        shuttle.position = root.TransformPoint(new Vector3(.3f, 2.1f, .25f));
        var driver = root.Find("AthleteVisual").GetComponent(AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.AthleteVisual")).First(t => t != null));
        Assert.That(driver.GetType().GetMethod("CanReach").Invoke(driver, new object[] { shuttle.position, "Smash" }), Is.EqualTo(true), "Fixture must stay inside existing arm/racket reach");
        type.GetField("velocity", Hidden).SetValue(court, Vector3.down);
        type.GetField("lastHitter", Hidden).SetValue(court, 0); type.GetField("lastPlayer", Hidden).SetValue(court, 0);
        type.GetField("lastStroke", Hidden).SetValue(court, "Clear");
        type.GetField("serviceFlight", Hidden).SetValue(court, false);
        for (int tick = 0; tick < 12 && (int)type.GetField("lastHitter", Hidden).GetValue(court) == 0; tick++)
        {
            type.GetMethod("UpdateDoubles", Hidden).Invoke(court, null); yield return new WaitForFixedUpdate();
        }
        Assert.That((int)type.GetField("lastHitter", Hidden).GetValue(court), Is.EqualTo(1));
        string shot = (string)type.GetField("opponentStroke", Hidden).GetValue(court);
        Assert.That(shot, Is.EqualTo("Smash"), "High reachable short reply must allow attack");
        Assert.That(Get<Vector3>("ShuttleVelocity").y, Is.LessThan(0), "Smash currently lofts upward, gives every receiver time to recover");
    }
    [UnityTest] public IEnumerator SettingsSlidersHaveTouchTargetsAndPersistValues()
    {
        yield return Boot(); Click("Pengaturan");
        var layout = type.GetMethod("SettingsSliderRects", BindingFlags.Public | BindingFlags.Static);
        Assert.That(layout, Is.Not.Null, "Settings sliders still use tiny default thumb");
        foreach (Vector2 size in new[] { new Vector2(1280, 720), new Vector2(960, 720), new Vector2(2180, 1056) })
        {
            var rects = (Rect[])layout.Invoke(null, new object[] { size });
            Assert.That(rects.Length, Is.EqualTo(3));
            foreach (var rect in rects) { Assert.That(rect.height, Is.GreaterThanOrEqualTo(48)); Assert.That(rect.xMax, Is.LessThanOrEqualTo(size.x - 24)); Assert.That(rect.yMax, Is.LessThanOrEqualTo(size.y - 24)); }
        }
        foreach (string action in new[] { "Bantuan", "Joystick", "ReplayMode", "TrailColor", "ShadowColor", "FrameRate" }) Click(action);
        type.GetProperty("Volume").SetValue(court, .25f);
        Assert.That(Get<float>("Volume"), Is.EqualTo(.25f));
        Click("Kembali"); Assert.That(Get<string>("ScreenName"), Is.EqualTo("Home"));
        yield return Boot();
        Assert.That((bool)type.GetField("assisted", Hidden).GetValue(court), Is.True);
        Assert.That(Get<string>("JoystickMode"), Is.EqualTo("Floating"));
        Assert.That(Get<int>("ReplayMode"), Is.EqualTo(2));
        Assert.That(Application.targetFrameRate, Is.EqualTo(60));
    }
    [UnityTest] public IEnumerator BackKeyAndBackgroundPauseWithoutLeakingInput()
    {
        yield return Boot(); Click("Main"); Click("Mulai");
        var back = type.GetMethod("HandleBack", Hidden);
        Assert.That(back, Is.Not.Null, "Android Back navigation missing");
        back.Invoke(court, null); Assert.That(Get<string>("ScreenName"), Is.EqualTo("Pause"));
        Click("Pengaturan"); back.Invoke(court, null);
        Assert.That(Get<string>("ScreenName"), Is.EqualTo("Pause"));
        back.Invoke(court, null); Assert.That(Get<string>("ScreenName"), Is.EqualTo("Playing"));
        type.GetMethod("OnApplicationPause", Hidden).Invoke(court, new object[] { true });
        Assert.That(Get<string>("ScreenName"), Is.EqualTo("Pause")); Assert.That(Time.timeScale, Is.Zero);
        type.GetMethod("OnApplicationPause", Hidden).Invoke(court, new object[] { false });
        Assert.That(Get<string>("ScreenName"), Is.EqualTo("Pause"), "Resume must await player");
        Click("Menu"); Assert.That(Get<string>("ScreenName"), Is.EqualTo("Confirm"));
        Click("Tidak"); Assert.That(Get<string>("ScreenName"), Is.EqualTo("Pause"));
        Click("Menu"); Click("Ya"); Assert.That(Get<string>("ScreenName"), Is.EqualTo("Home"));
        Assert.That(Get<object>("Match"), Is.Null);
    }
    [UnityTest] public IEnumerator HomePlayStartsSelectedDifficultyAndModalBlocksCourt()
    {
        yield return Boot();
        Assert.That(type.GetProperty("ScreenName"), Is.Not.Null, "Home/setup UI missing");
        Assert.That(Get<string>("ScreenName"), Is.EqualTo("Home"));
        Assert.That(Get<bool>("Flying"), Is.False);
        var before = Get<Vector3>("PlayerPosition");
        type.GetMethod("PointerDown", Hidden).Invoke(court, new object[] { 3, new Vector2(650, 570) });
        yield return new WaitForFixedUpdate();
        Assert.That(Get<Vector3>("PlayerPosition"), Is.EqualTo(before));
        Click("Main"); Assert.That(Get<string>("ScreenName"), Is.EqualTo("Setup"));
        Click("Hard"); Click("Mulai");
        Assert.That(Get<string>("ScreenName"), Is.EqualTo("Playing"));
        Assert.That(Get<string>("Difficulty"), Is.EqualTo("Hard"));
        Assert.That(Get<string>("FlowStage"), Is.EqualTo("Warmup"));
        Click("Pause");
        Assert.That(Get<string>("ScreenName"), Is.EqualTo("Pause"));
        Assert.That(Get<Vector2>("Analog"), Is.EqualTo(Vector2.zero));
        Click("Pengaturan"); Assert.That(Get<string>("ScreenName"), Is.EqualTo("Settings"));
        Click("Kembali"); Click("Lanjut");
        Assert.That(Get<string>("ScreenName"), Is.EqualTo("Playing"));
        Assert.That(Get<bool>("Flying"), Is.False, "Resume must not serve or release a stale shot");
        LogAssert.NoUnexpectedReceived();
    }
}
