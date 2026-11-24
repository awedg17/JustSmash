using UnityEngine;

namespace JustSmash
{
    public sealed partial class CourtPrototype
    {
        public bool DebugVisible { get; private set; }
        float profileFrameTime;
        public string ProfileSummary => $"{(profileFrameTime > 0 ? 1 / profileFrameTime : 0):F1} FPS | {profileFrameTime * 1000:F1} ms | Unity allocated {UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1048576f:F1} MiB | colliders {FindObjectsByType<Collider>(FindObjectsSortMode.None).Length} | step {Time.fixedDeltaTime:F3}s";
        LineRenderer debugFlight;
        float trailBrightness = .85f, shadowBrightness = .9f;
        Color trailTint = Color.white, shadowTint = new Color(.1f, .14f, .14f);
        public void ConfigureVisibility(float trail, float shade, Color tailColor, Color groundColor)
        {
            ApplyVisibility(trail, shade, tailColor, groundColor);
            LocalGameData.SetFloat("trailBrightness", trail); LocalGameData.SetFloat("shadowBrightness", shade);
            LocalGameData.SetString("trailTint", ColorUtility.ToHtmlStringRGB(tailColor));
            LocalGameData.SetString("shadowTint", ColorUtility.ToHtmlStringRGB(groundColor)); LocalGameData.Save();
        }
        void ApplyVisibility(float trail, float shade, Color tailColor, Color groundColor)
        {
            foreach (float n in new[] { trail, shade, tailColor.r, tailColor.g, tailColor.b, groundColor.r, groundColor.g, groundColor.b })
                if (!float.IsFinite(n) || n < 0 || n > 1) throw new System.ArgumentOutOfRangeException("visibility");
            trailBrightness = trail; shadowBrightness = shade; trailTint = tailColor; shadowTint = groundColor;
            var renderer = shuttle.GetComponent<TrailRenderer>();
            renderer.startColor = new Color(tailColor.r, tailColor.g, tailColor.b, trail);
            shadow.GetComponent<Renderer>().sharedMaterial.color = Color.Lerp(Color.white, groundColor, shade);
        }
        void LoadVisibility()
        {
            ColorUtility.TryParseHtmlString("#" + LocalGameData.GetString("trailTint", "FFFFFF"), out trailTint);
            ColorUtility.TryParseHtmlString("#" + LocalGameData.GetString("shadowTint", "1A2424"), out shadowTint);
            float trail = LocalGameData.GetFloat("trailBrightness", .85f);
            if (trail < .7f) trail = .85f;
            ApplyVisibility(Mathf.Clamp01(trail), Mathf.Clamp01(LocalGameData.GetFloat("shadowBrightness", .9f)), trailTint, shadowTint);
        }
        [System.Serializable] sealed class TesterReport
        {
            public string utc, missReason, notes, hitResult, stroke;
            public bool inputMisread;
            public int leftComfort, rightComfort, cheatedFrequency;
            public Vector2 analog;
        }
        public string SubmitTesterReport(string reason, bool misread, int left, int right, int cheated, string notes)
        {
            if (System.Array.IndexOf(missReasons, reason) < 0 || left < 1 || left > 5 || right < 1 || right > 5 || cheated < 1 || cheated > 5 || notes == null || notes.Length > 500)
                throw new System.ArgumentException("Invalid tester report");
            System.IO.Directory.CreateDirectory(LocalGameData.Root);
            string path = System.IO.Path.Combine(LocalGameData.Root, "tester-reports.jsonl");
            var report = new TesterReport { utc = System.DateTime.UtcNow.ToString("o"), missReason = reason, inputMisread = misread, leftComfort = left, rightComfort = right, cheatedFrequency = cheated, notes = notes, hitResult = feedback, stroke = stroke, analog = analog };
            System.IO.File.AppendAllText(path, JsonUtility.ToJson(report) + System.Environment.NewLine);
            return path;
        }
        Vector2 testerScroll;
        bool testerOpen, inputMisread;
        bool panelPaused;
        int missReason, leftComfort = 3, rightComfort = 3, cheatedFrequency = 1;
        string testerNotes = "", testerStatus = "";
        readonly string[] missReasons = { "Timing", "Position", "Input", "Unclear" };
        readonly string[] ratings = { "1", "2", "3", "4", "5" };
        Rect TesterRect => new Rect(24, 24, Mathf.Min(700, UiWidth - 48), 650);
        void ToggleTester()
        {
            testerOpen = !testerOpen;
            if (testerOpen) { panelPaused = Time.timeScale == 0; Time.timeScale = 0; analog = Vector2.zero; charging = false; moveFinger = shotFinger = jumpFinger = -1; jumpHeld = false; }
            else Time.timeScale = panelPaused ? 0 : 1;
        }
        void DrawTester()
        {
            if (!testerOpen && (Application.isEditor || Debug.isDebugBuild) && Input.GetKey(KeyCode.F2)) ToggleTester();
            if (!testerOpen) return;
            GUI.Box(TesterRect, "LOCAL TESTER");
            GUILayout.BeginArea(new Rect(40, 60, TesterRect.width - 32, 600));
            testerScroll = GUILayout.BeginScrollView(testerScroll);
            GUILayout.Label("Miss reason"); missReason = GUILayout.SelectionGrid(missReason, missReasons, 4, GUILayout.Height(40));
            inputMisread = GUILayout.Toggle(inputMisread, "Input misread", GUILayout.Height(32));
            GUILayout.Label("Left thumb comfort: 1 poor / 5 good"); leftComfort = GUILayout.SelectionGrid(leftComfort - 1, ratings, 5, GUILayout.Height(36)) + 1;
            GUILayout.Label("Right thumb comfort: 1 poor / 5 good"); rightComfort = GUILayout.SelectionGrid(rightComfort - 1, ratings, 5, GUILayout.Height(36)) + 1;
            GUILayout.Label("Felt cheated: 1 never / 5 often"); cheatedFrequency = GUILayout.SelectionGrid(cheatedFrequency - 1, ratings, 5, GUILayout.Height(36)) + 1;
            GUILayout.Label("Notes (optional, max 500 characters)");
            testerNotes = GUILayout.TextField(testerNotes, 500, GUILayout.Height(40));
            if (GUILayout.Button("Save local report", GUILayout.Height(40)))
            {
                try { SubmitTesterReport(missReasons[missReason], inputMisread, leftComfort, rightComfort, cheatedFrequency, testerNotes); testerStatus = "Saved locally: tester-reports.jsonl"; }
                catch (System.IO.IOException e) { testerStatus = "Save failed: " + e.Message; }
                catch (System.UnauthorizedAccessException e) { testerStatus = "Save denied: " + e.Message; }
            }
            GUILayout.Label(testerStatus);
            GUILayout.Label("Trail brightness " + trailBrightness.ToString("F2"));
            float tail = GUILayout.HorizontalSlider(trailBrightness, 0, 1, GUILayout.Height(24));
            GUILayout.Label("Shadow strength " + shadowBrightness.ToString("F2"));
            float shade = GUILayout.HorizontalSlider(shadowBrightness, 0, 1, GUILayout.Height(24));
            if (tail != trailBrightness || shade != shadowBrightness) ConfigureVisibility(tail, shade, trailTint, shadowTint);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Trail white", GUILayout.Height(36))) ConfigureVisibility(trailBrightness, shadowBrightness, Color.white, shadowTint);
            if (GUILayout.Button("Trail yellow", GUILayout.Height(36))) ConfigureVisibility(trailBrightness, shadowBrightness, Color.yellow, shadowTint);
            if (GUILayout.Button("Shadow black", GUILayout.Height(36))) ConfigureVisibility(trailBrightness, shadowBrightness, trailTint, Color.black);
            GUILayout.EndHorizontal();
            if (Application.isEditor || Debug.isDebugBuild)
                if (GUILayout.Button(DebugVisible ? "Debug ON (F3)" : "Debug OFF (F3)", GUILayout.Height(36))) ToggleDebug();
            if (GUILayout.Button("Close", GUILayout.Height(40))) ToggleTester();
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
        public void SetDebugVisible(bool visible)
        {
            DebugVisible = visible && (Application.isEditor || Debug.isDebugBuild);
            if (debugFlight == null)
            {
                debugFlight = new GameObject("DebugFlight").AddComponent<LineRenderer>();
                debugFlight.sharedMaterial = new Material(SpriteShader);
                debugFlight.startWidth = debugFlight.endWidth = .025f;
                debugFlight.startColor = debugFlight.endColor = Color.yellow;
            }
            debugFlight.enabled = DebugVisible;
        }
        [ContextMenu("Toggle flight/contact debug")]
        void ToggleDebug() { SetDebugVisible(!DebugVisible); }
        void UpdateDebug()
        {
            profileFrameTime = Mathf.Lerp(profileFrameTime, Time.unscaledDeltaTime, .1f);
            if (Input.GetKeyDown(KeyCode.F3)) ToggleDebug();
            if (!DebugVisible) return;
            debugFlight.positionCount = replay.Count;
            for (int i = 0; i < replay.Count; i++) debugFlight.SetPosition(i, replay[i].shuttle);
        }
        void OnDrawGizmos()
        {
            if (!DebugVisible || player == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position + Vector3.up * (stroke == "Net" || stroke == "Lift" ? 1 : 2.1f), 1.15f);
            Gizmos.color = GradeColor(feedback); Gizmos.DrawSphere(contact, .08f);
            if (shuttle != null)
            {
                var p = shuttle.position; var v = velocity;
                for (int i = 0; i < 150 && p.y > .04f; i++) { v = StepVelocity(v, .02f); p += v * .02f; }
                Gizmos.color = Color.red; Gizmos.DrawWireSphere(new Vector3(p.x, .04f, p.z), .15f);
            }
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(CourtPoint(analog, 0) + Vector3.up * .03f, new Vector3(2.3f, .02f, 1.4f));
        }
    }
}
