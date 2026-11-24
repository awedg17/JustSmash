using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace JustSmash
{
    public sealed class BootDiagnostics : MonoBehaviour
    {
        [Serializable] public sealed class Config
        {
            public int version = 1, targetFps = 30;
            public float fixedStep = .02f, drag = .065f, rallyDrain = .6f, moveDrain = 0, gameRecovery = 8, smashCost = 3, shotCost = 1, jumpCost = 4, aiSpeed = 4.5f;
            public float singlesHalfWidth = 2.59f, doublesHalfWidth = 3.05f, halfLength = 6.7f, netHeight = 1.55f;
            public string scoreRule = "Official15", difficulty = "Normal";
            public bool logging = true, inputLogging;
        }
        public static Config Current { get; private set; } = new Config();
        public static void Log(string category, string message)
        {
            if (Current.logging && (Application.isEditor || Debug.isDebugBuild)) Debug.Log("[" + category + "] " + message);
        }
        public Config Settings { get; private set; }
        public bool Paused { get; private set; }
        float frameTime;
        public void SetPaused(bool value) { Paused = value; Time.timeScale = value ? 0 : 1; }
        void Awake()
        {
            var source = Resources.Load<TextAsset>("prototype-config");
            if (source == null) throw new InvalidOperationException("prototype-config missing");
            Settings = JsonUtility.FromJson<Config>(source.text);
            if (Settings.version != 1 || Settings.targetFps < 15 || Settings.targetFps > 120 || Settings.fixedStep <= 0 || Settings.fixedStep > .05f)
                throw new InvalidOperationException("Invalid prototype config");
            foreach (float n in new[] { Settings.fixedStep, Settings.drag, Settings.rallyDrain, Settings.moveDrain, Settings.gameRecovery, Settings.smashCost, Settings.shotCost, Settings.jumpCost, Settings.aiSpeed, Settings.singlesHalfWidth, Settings.doublesHalfWidth, Settings.halfLength, Settings.netHeight })
                if (!float.IsFinite(n) || n < 0 || n > 100) throw new InvalidOperationException("Invalid numeric prototype config");
            if (Settings.fixedStep <= 0 || Settings.drag <= 0 || Settings.aiSpeed <= 0 || Settings.singlesHalfWidth <= 0 || Settings.doublesHalfWidth <= 0 || Settings.halfLength <= 0 || Settings.netHeight <= 0) throw new InvalidOperationException("Nonpositive prototype dimension");
            if (Settings.scoreRule != "Official15") throw new InvalidOperationException("Unsupported prototype rules/difficulty");
            OpponentProfile.For(Settings.difficulty);
            Current = Settings;
            Application.targetFrameRate = Settings.targetFps;
            Time.fixedDeltaTime = Settings.fixedStep;
            Debug.Log("[boot] config v" + Settings.version + "; scene=" + gameObject.scene.name);
        }
        void Update() { frameTime = Mathf.Lerp(frameTime, Time.unscaledDeltaTime, .1f); }
        void OnDestroy() { Time.timeScale = 1; }
        void OnGUI()
        {
            var safe = Screen.safeArea;
            float scale = Mathf.Max(1, Screen.height / 720f);
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * scale);
            GUILayout.BeginArea(new Rect(safe.x / scale + 20, (Screen.height - safe.yMax) / scale + 20, Mathf.Min(600, safe.width / scale - 40), safe.height / scale - 40), GUI.skin.box);
            GUILayout.Label("JUST SMASH | M0 DIAGNOSTICS");
            GUILayout.Label($"{(frameTime > 0 ? 1 / frameTime : 0):F1} FPS | {frameTime * 1000:F1} ms | Unity allocated memory {Profiler.GetTotalAllocatedMemoryLong() / 1048576f:F1} MiB");
            GUILayout.Label($"Physics step {Time.fixedDeltaTime:F3}s | colliders {FindObjectsByType<Collider>(FindObjectsSortMode.None).Length}");
            GUILayout.Label("Boot smoke scene. No gameplay yet.");
            if (GUILayout.Button(Paused ? "Resume" : "Pause", GUILayout.Height(48))) SetPaused(!Paused);
            if (GUILayout.Button("Exit", GUILayout.Height(48))) Application.Quit();
            GUILayout.EndArea();
        }
    }
}
