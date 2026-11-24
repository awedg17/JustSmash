using System;
using UnityEngine;

namespace JustSmash
{
    public sealed partial class CourtPrototype
    {
        enum GameScreen { Home, Setup, Playing, Training, Pause, Settings, Credits, Confirm }
        GameScreen screen, settingsReturn, confirmReturn;
        string confirmAction, menuAction, menuNotice;
        string MenuNotice => screen == GameScreen.Pause || screen == GameScreen.Settings ? menuNotice ?? "" : "";
        int menuFinger = -1;
        Vector2 menuPoint, settingsScroll;
        GUIStyle bodyStyle, titleStyle, buttonStyle, smallStyle, scoreStyle, sliderStyle, sliderThumbStyle;
        Texture2D slantTexture, sweepTexture, circleTexture;
        readonly Color panelColor = new Color(.094f, .345f, .925f), textColor = new Color(.957f, .973f, 1), accentColor = new Color(1, .855f, .271f);
        readonly Color inkColor = new Color(.027f, .098f, .243f), skyColor = new Color(.46f, .745f, 1);
        public string ScreenName => screen.ToString();
        public string Difficulty { get; private set; } = "Normal";
        public string EventName => "JUST SMASH OPEN";
        public string MatchTitle => DoublesMode ? "GANDA PUTRA" : "TUNGGAL PUTRA";
        public string OpponentName => Difficulty == "Hard" ? "Raka Pradana" : Difficulty == "Easy" ? "Bima Santosa" : "Arga Wijaya";
        string TeamName(int side) => side == 0 ? (DoublesMode ? "Kamu + Dimas" : "Kamu") : DoublesMode ? OpponentName.Split(' ')[0] + " + Fajar" : OpponentName;
        Rect SetupTeamRect(int side) => new Rect(52, 330 + side * 48, 350, 36);
        float UiHeight => Screen.safeArea.height / Mathf.Max(.01f, uiScale);
        bool GameplayInputEnabled => (screen == GameScreen.Playing || screen == GameScreen.Training) && Time.timeScale > 0 && !testerOpen;
        string[] UiActions => screen switch
        {
            GameScreen.Home => new[] { "Main", "Latihan", "Pengaturan", "Kredit" },
            GameScreen.Setup => new[] { "Easy", "Normal", "Hard", "Mulai", "Kembali", "Tunggal", "Ganda" },
            GameScreen.Pause => new[] { "Lanjut", "Pengaturan", "Simpan", "Muat", "Ulangi", "Menu" },
            GameScreen.Settings => new[] { "Bantuan", "Joystick", "ReplayMode", "TrailColor", "ShadowColor", "FrameRate", "Kembali" },
            GameScreen.Credits => new[] { "Kembali" },
            GameScreen.Confirm => new[] { "Ya", "Tidak" },
            _ => Match != null && Match.Winner >= 0 ? new[] { "Main lagi", "Menu" }
                : replaying ? new[] { "Lewati" } : new[] { "Pause", "Teruskan", "Challenge", "Umpan lob", "Umpan net" }
        };
        static Rect ControlRect(int index, Vector2 size)
        {
            if (index == 0) return new Rect(33, size.y - 227, 190, 190);
            if (index == 6) return new Rect(size.x - 238, size.y - 116, 128, 80);
            if (index == 7) return new Rect(size.x - 324, size.y - 220, 292, 80);
            int i = index - 1;
            return new Rect(size.x - 324 + i % 3 * 100, size.y - 310 + i / 3 * 90, 92, 80);
        }
        public static Rect[] GameplayRects(Vector2 size)
        {
            var result = new Rect[8];
            for (int i = 0; i < result.Length; i++) result[i] = ControlRect(i, size);
            return result;
        }
        LineRenderer activePlayerMarker;
        void LateUpdate()
        {
            bool visible = GameplayInputEnabled && !replaying && (Match == null || Match.Winner < 0);
            if (visible && activePlayerMarker == null)
            {
                var go = new GameObject("ControlledPlayerMarker"); go.transform.SetParent(transform, false);
                go.transform.rotation = Quaternion.Euler(90, 0, 0);
                activePlayerMarker = go.AddComponent<LineRenderer>();
                activePlayerMarker.useWorldSpace = false; activePlayerMarker.loop = true;
                activePlayerMarker.alignment = LineAlignment.TransformZ; activePlayerMarker.positionCount = 32;
                activePlayerMarker.startWidth = activePlayerMarker.endWidth = .04f;
                activePlayerMarker.startColor = activePlayerMarker.endColor = accentColor;
                activePlayerMarker.sharedMaterial = new Material(SpriteShader);
                activePlayerMarker.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                activePlayerMarker.receiveShadows = false;
                for (int i = 0; i < activePlayerMarker.positionCount; i++)
                {
                    float angle = i * Mathf.PI * 2 / activePlayerMarker.positionCount;
                    activePlayerMarker.SetPosition(i, new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * .36f);
                }
            }
            if (activePlayerMarker != null)
            {
                activePlayerMarker.gameObject.SetActive(visible);
                if (visible) activePlayerMarker.transform.position = new Vector3(player.position.x, .018f, player.position.z);
            }
            if (suggestedPosition != null) suggestedPosition.gameObject.SetActive(visible && DoublesMode);
        }
        void InitializeUi()
        {
            uiScale = Mathf.Max(.01f, CanvasScale(Screen.safeArea.size));
            screen = GameScreen.Home; stickCenter = DefaultStickCenter;
            string saved = LocalGameData.GetString("difficulty", "Normal");
            Difficulty = saved == "Easy" || saved == "Hard" ? saved : "Normal";
            assisted = LocalGameData.GetString("assist", "Manual") == "Assisted";
            ReplayMode = Mathf.Clamp((int)LocalGameData.GetFloat("replayMode", 1), 0, 2);
            Volume = Mathf.Clamp01(LocalGameData.GetFloat("volume", .7f));
            Application.targetFrameRate = LocalGameData.GetFloat("targetFps", 30) == 60 ? 60 : 30;
        }
        void ClearGameplayInput()
        {
            shotIntent.Clear();
            analog = Vector2.zero; charging = servingCharge = jumpHeld = false;
            moveFinger = shotFinger = jumpFinger = serveFinger = pressedShot = -1;
            stickCenter = DefaultStickCenter;
        }
        void EnterGameScreen(bool training)
        {
            screen = training ? GameScreen.Training : GameScreen.Playing; menuNotice = null;
            Time.timeScale = 1; ClearGameplayInput(); view.fieldOfView = 48; SetShuttleVisible(true);
        }
        public void EnterTraining()
        {
            if (screen == GameScreen.Training && Match == null) return;
            SkipReplay(); replay.Clear(); Match = null; doublesSelected = false; SetExtraAthletes(false); flying = false; flowStage = null;
            MatchState = "Training"; reviewAvailable = serviceFlight = false; beforePoint = null; pointWait = 0; pointCause = PointCause.Unknown; pointReview = PointReview.None;
            ResetAthleteMovement(); EnterGameScreen(true);
            player.SetPositionAndRotation(new Vector3(0, 0, -4), Quaternion.identity);
            opponent.SetPositionAndRotation(new Vector3(0, 0, 4), Quaternion.Euler(0, 180, 0));
            previousPlayer = player.position; previousOpponent = opponent.position;
            swingAt = aiContactAt = Time.time - 10;
            view.transform.position = new Vector3(.2f, 4f, -14.2f); view.transform.LookAt(new Vector3(.2f, 0, -1));
        }
        void OpenUi(GameScreen next)
        {
            StopCourtAudio();
            if (next == GameScreen.Settings || next == GameScreen.Credits) settingsReturn = screen;
            ClearGameplayInput(); screen = next; menuNotice = null;
            Time.timeScale = next == GameScreen.Home || next == GameScreen.Setup ? 1 : 0;
            if (next == GameScreen.Home || next == GameScreen.Setup) StageMenu();
        }
        public Rect UiActionRect(string action)
        {
            float w = UiWidth, h = UiHeight;
            int index = Array.IndexOf(UiActions, action);
            if (index < 0) return Rect.zero;
            if (screen == GameScreen.Home) return action == "Main" ? new Rect(46, 428, 354, 100)
                : action == "Latihan" ? new Rect(50, 548, 324, 70)
                : action == "Pengaturan" ? new Rect(48, 632, 212, 64) : new Rect(280, 632, 128, 64);
            if (screen == GameScreen.Setup)
            {
                if (action == "Tunggal" || action == "Ganda") return new Rect(action == "Tunggal" ? 45 : 228, 246, 169, 64);
                if (index < 3) return new Rect(46 + index * 115, 474, 103, 64);
                return index == 3 ? new Rect(46, 568, 340, 78) : new Rect(w - 212, 24, 172, 64);
            }
            if (screen == GameScreen.Settings)
                return action == "Kembali" ? new Rect(w - 212, 28, 172, 64) : new Rect(44, 130 + index * 78, 416, 64);
            if (screen == GameScreen.Credits) return new Rect(w - 212, 28, 172, 64);
            if (screen == GameScreen.Pause) return new Rect(w / 2 - 180, 132 + index * 76, 360, 64);
            if (screen == GameScreen.Confirm) return new Rect(w / 2 - 180 + index * 192, h / 2 + 34, 168, 64);
            if (Match != null && Match.Winner >= 0) return index == 0 ? new Rect(44, 550, 354, 76) : new Rect(48, 638, 200, 64);
            if (action == "Pause") return new Rect(w - 120, 24, 96, 64);
            if (action == "Lewati") return new Rect(w - 224, 24, 200, 64);
            if (action == "Teruskan") return flowStage != null ? new Rect(40, 372, 318, 64) : Rect.zero;
            if (action == "Challenge") return reviewAvailable ? new Rect(w - 324, 112, 300, 64) : Rect.zero;
            if (Match == null) return new Rect(24 + (action == "Umpan net" ? 180 : 0), 154, 168, 64);
            return Rect.zero;
        }
        bool UiPointerDown(int id, Vector2 point)
        {
            menuPoint = point;
            foreach (string action in UiActions)
            {
                Rect rect = UiActionRect(action);
                if (rect.width > 0 && rect.Contains(point))
                {
                    if (menuFinger == -1) { menuFinger = id; menuAction = action; }
                    return true;
                }
            }
            return !GameplayInputEnabled;
        }
        bool UiPointerUp(int id)
        {
            if (id != menuFinger) return false;
            string action = menuAction;
            bool commit = UiActionRect(action).Contains(menuPoint);
            menuFinger = -1; menuAction = null;
            if (commit) ActOnUi(action);
            return true;
        }
        void ActOnUi(string action)
        {
            switch (action)
            {
                case "Main": OpenUi(GameScreen.Setup); break;
                case "Tunggal": ConfigureDiscipline(false); break;
                case "Ganda": ConfigureDiscipline(true); break;
                case "Easy": case "Normal": case "Hard": Difficulty = action; break;
                case "Mulai": LocalGameData.SetString("difficulty", Difficulty); BeginExhibition(); break;
                case "Latihan": EnterTraining(); feedback = "Pilih umpan untuk mulai"; break;
                case "Pause": OpenUi(GameScreen.Pause); break;
                case "Lanjut": EnterGameScreen(Match == null); break;
                case "Teruskan": AdvanceFlow(); break;
                case "Pengaturan": OpenUi(GameScreen.Settings); break;
                case "Kredit": OpenUi(GameScreen.Credits); break;
                case "Kembali":
                    screen = screen == GameScreen.Setup ? GameScreen.Home : settingsReturn;
                    Time.timeScale = screen == GameScreen.Pause ? 0 : 1;
                    if (screen == GameScreen.Home || screen == GameScreen.Setup) StageMenu();
                    break;
                case "Simpan": RunMenuAction("Save"); break;
                case "Muat": RunMenuAction("Load"); break;
                case "Bantuan": assisted = !assisted; LocalGameData.SetString("assist", assisted ? "Assisted" : "Manual"); break;
                case "Joystick": ConfigureJoystick(JoystickMode == "Fixed" ? "Floating" : "Fixed"); break;
                case "ReplayMode": ReplayMode = (ReplayMode + 1) % 3; LocalGameData.SetFloat("replayMode", ReplayMode); break;
                case "TrailColor": ConfigureVisibility(trailBrightness, shadowBrightness, trailTint == Color.yellow ? Color.white : Color.yellow, shadowTint); break;
                case "ShadowColor": ConfigureVisibility(trailBrightness, shadowBrightness, trailTint, shadowTint == Color.black ? new Color(.12f, .18f, .18f) : Color.black); break;
                case "FrameRate": Application.targetFrameRate = Application.targetFrameRate == 60 ? 30 : 60; LocalGameData.SetFloat("targetFps", Application.targetFrameRate); break;
                case "Challenge": ChallengeCall(); break;
                case "Lewati": SkipReplay(); break;
                case "Umpan lob": Feed("Lob"); break;
                case "Umpan net": Feed("Net"); break;
                case "Ulangi": case "Menu":
                    if (Match != null && Match.Winner < 0) { confirmReturn = screen; confirmAction = action; OpenUi(GameScreen.Confirm); }
                    else FinishUiAction(action);
                    break;
                case "Ya": FinishUiAction(confirmAction); break;
                case "Tidak": screen = confirmReturn; Time.timeScale = screen == GameScreen.Pause ? 0 : 1; break;
                case "Main lagi": BeginExhibition(); break;
            }
        }
        void HandleBack()
        {
            switch (screen)
            {
                case GameScreen.Playing: case GameScreen.Training: OpenUi(GameScreen.Pause); break;
                case GameScreen.Pause: EnterGameScreen(Match == null); break;
                case GameScreen.Settings: case GameScreen.Credits: case GameScreen.Setup: ActOnUi("Kembali"); break;
                case GameScreen.Confirm: ActOnUi("Tidak"); break;
                case GameScreen.Home: confirmReturn = screen; confirmAction = "Exit"; OpenUi(GameScreen.Confirm); break;
            }
        }
        void FinishUiAction(string action)
        {
            if (action == "Exit") { Application.Quit(); return; }
            if (action == "Ulangi") { BeginExhibition(); return; }
            StopCourtAudio();
            SkipReplay(); ClearGameplayInput(); Match = null; flying = false; velocity = Vector3.zero; replay.Clear();
            OpenUi(GameScreen.Home); SetExtraAthletes(doublesSelected);
            jumpY = landingWeight = 0; jumpPhase = 1;
            jumpAt = swingAt = aiContactAt = partnerAt = secondAt = Time.time - 10;
            for (int id = 0; id < (doublesSelected ? 4 : 2); id++)
            {
                bool near = id % 2 == 0; var root = Actor(id);
                root.SetPositionAndRotation(new Vector3(doublesSelected ? (id < 2 ? 1.3f : -1.3f) * (near ? 1 : -1) : 0, 0, near ? -4 : 4), Quaternion.Euler(0, near ? 0 : 180, 0));
                root.Find("ContactShadow").localPosition = new Vector3(0, .004f, 0);
                Visual(id).SetTravel(0); Visual(id).Present("Ready", 1, Vector3.zero, Vector3.zero, 0, 0);
            }
            previousPlayer = player.position; previousOpponent = opponent.position;
            if (partner != null) { previousPartner = partner.position; previousOpponentPartner = opponentPartner.position; }
            shuttle.position = new Vector3(0, .1f, -3); shadow.position = new Vector3(0, .02f, -3);
            landingShadow.gameObject.SetActive(false); shuttle.GetComponent<TrailRenderer>().Clear();
            StageMenu();
        }
        void SetShuttleVisible(bool visible)
        {
            foreach (var renderer in shuttle.GetComponentsInChildren<Renderer>()) renderer.enabled = visible;
            shadow.GetComponent<Renderer>().enabled = visible;
            if (!visible) landingShadow.gameObject.SetActive(false);
        }
        void StageMenu()
        {
            if (screen != GameScreen.Home && screen != GameScreen.Setup) return;
            ResetAthleteMovement();
            void Pose(int id, Vector3 position, float yaw)
            {
                var root = Actor(id); if (root == null || !root.gameObject.activeInHierarchy) return;
                root.SetPositionAndRotation(position, Quaternion.Euler(0, yaw, 0));
                root.Find("ContactShadow").localPosition = new Vector3(0, .004f, 0);
                Visual(id).SetTravel(0); Visual(id).Present("Menu", 1, Vector3.zero, Vector3.zero, 0, 0);
            }
            SetShuttleVisible(false); view.fieldOfView = 42;
            if (screen == GameScreen.Home)
            {
                Pose(0, new Vector3(1, 0, -4.5f), 145); Pose(1, new Vector3(0, 0, 4), 180);
                if (doublesSelected) { Pose(2, new Vector3(-1.3f, 0, -1), 160); Pose(3, new Vector3(2.9f, 0, 3), 195); }
                view.transform.position = new Vector3(-.4f, 1.35f, -8.4f); view.transform.LookAt(new Vector3(-.4f, 1, -4.5f));
            }
            else if (doublesSelected)
            {
                Pose(0, new Vector3(-.1f, 0, -4.6f), 160); Pose(2, new Vector3(1.9f, 0, -4.4f), 175);
                Pose(1, new Vector3(-.5f, 0, -1), 180); Pose(3, new Vector3(2.9f, 0, .3f), 195);
                view.transform.position = new Vector3(-.9f, 1.7f, -10.2f); view.transform.LookAt(new Vector3(-.9f, .9f, -3.8f));
            }
            else
            {
                Pose(0, new Vector3(.3f, 0, -4.3f), 160); Pose(1, new Vector3(1.65f, 0, -3.5f), 195);
                view.transform.position = new Vector3(-1.1f, 1.5f, -9.6f); view.transform.LookAt(new Vector3(-1.1f, .9f, -4));
            }
            previousPlayer = player.position; previousOpponent = opponent.position;
            if (partner != null) { previousPartner = partner.position; previousOpponentPartner = opponentPartner.position; }
        }
        void EnsureUiStyles()
        {
            if (bodyStyle != null) return;
            bodyStyle = new GUIStyle(GUI.skin.label) { font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"), fontSize = 26, wordWrap = true, padding = new RectOffset(0, 0, 0, 0) };
            var display = Resources.Load<Font>("Fonts/Anton-Regular");
            if (display == null) throw new InvalidOperationException("Approved Anton font missing");
            slantTexture = UiMask(256, 128, (x, y) => x >= 12 * (1 - y / 127f) && x <= 255 - 12 * y / 127f);
            sweepTexture = UiMask(560, 720, (x, y) => x < Mathf.Lerp(548, 426, y / 719f));
            circleTexture = UiMask(192, 192, (x, y) => (new Vector2(x, y) - new Vector2(95.5f, 95.5f)).sqrMagnitude <= 95 * 95);
            bodyStyle.normal.textColor = textColor;
            titleStyle = new GUIStyle(bodyStyle) { font = display, fontSize = 48, wordWrap = false };
            scoreStyle = new GUIStyle(titleStyle) { fontSize = 36, alignment = TextAnchor.MiddleCenter };
            smallStyle = new GUIStyle(bodyStyle) { fontSize = 22 };
            smallStyle.normal.textColor = textColor;
            buttonStyle = new GUIStyle(titleStyle) { fontSize = 28, alignment = TextAnchor.MiddleCenter, wordWrap = true };
            buttonStyle.normal.textColor = buttonStyle.hover.textColor = buttonStyle.active.textColor = buttonStyle.focused.textColor = inkColor;
            sliderStyle = new GUIStyle(GUI.skin.horizontalSlider) { fixedHeight = 48 };
            sliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb) { fixedWidth = 48, fixedHeight = 48 };
            // ponytail: fixed Indonesian/Latin UI; extend glyphs when localization adds a script.
            var glyphs = new GUIContent("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 .,:;!?+-/%()|•");
            bodyStyle.CalcSize(glyphs); titleStyle.CalcSize(glyphs);
            new GUIStyle(bodyStyle) { fontStyle = FontStyle.Bold }.CalcSize(glyphs);
        }
        void FillUi(Rect rect, Color color) { GUI.color = color; GUI.DrawTexture(rect, Texture2D.whiteTexture); GUI.color = Color.white; }
        void PanelUi(Rect rect) { FillUi(new Rect(rect.x - 3, rect.y - 3, rect.width + 6, rect.height + 10), inkColor); FillUi(rect, panelColor); }
        static Texture2D UiMask(int width, int height, Func<float, float, bool> inside)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
            var pixels = new Color32[width * height];
            for (int y = 0; y < height; y++) for (int x = 0; x < width; x++) pixels[y * width + x] = inside(x, height - 1 - y) ? new Color32(255,255,255,255) : new Color32(0,0,0,0);
            texture.SetPixels32(pixels); texture.Apply(false, true); return texture;
        }
        void MaskUi(Rect rect, Texture2D mask, Color color) { GUI.color = color; GUI.DrawTexture(rect, mask); GUI.color = Color.white; }
        void DrawControl(Rect rect, string label, bool primary = false, bool pressed = false, int size = 28)
        {
            MaskUi(new Rect(rect.x + 4, rect.y + 6, rect.width, rect.height), slantTexture, inkColor);
            if (pressed) rect.y += 3;
            MaskUi(rect, slantTexture, inkColor);
            var face = new Rect(rect.x + 3, rect.y + 3, rect.width - 6, rect.height - 6);
            MaskUi(face, slantTexture, primary ? accentColor : textColor);
            buttonStyle.fontSize = size; buttonStyle.normal.textColor = inkColor;
            GUI.Label(rect, label, buttonStyle);
        }
        void MenuSweep(float h)
        {
            MaskUi(new Rect(0, 0, 565, h), sweepTexture, skyColor);
            MaskUi(new Rect(0, 0, 560, h), sweepTexture, panelColor);
        }
        void DisplayWord(Rect rect, string word, int size, Color color, float angle = -4)
        {
            var matrix = GUI.matrix; GUIUtility.RotateAroundPivot(angle, rect.position);
            var style = new GUIStyle(titleStyle) { fontSize = size };
            style.normal.textColor = inkColor;
            GUI.Label(new Rect(rect.x + 5, rect.y + 7, rect.width, rect.height), word, style);
            foreach (var offset in new[] { new Vector2(-2, 0), new Vector2(2, 0), new Vector2(0, -2), new Vector2(0, 2) })
                GUI.Label(new Rect(rect.position + offset, rect.size), word, style);
            style.normal.textColor = color; GUI.Label(rect, word, style); GUI.matrix = matrix;
        }
        void ReleaseUi()
        {
            if (slantTexture != null) Destroy(slantTexture); if (sweepTexture != null) Destroy(sweepTexture); if (circleTexture != null) Destroy(circleTexture);
            if (activePlayerMarker != null) { Destroy(activePlayerMarker.sharedMaterial); Destroy(activePlayerMarker.gameObject); }
            if (suggestedPosition != null) { Destroy(suggestedPosition.GetComponent<Renderer>().sharedMaterial); Destroy(suggestedPosition.gameObject); }
        }
        string ActionLabel(string action) => action switch
        {
            "Main" => "MAIN", "Mulai" => "MULAI", "Main lagi" => "TANDING ULANG",
            "Easy" => "Mudah", "Hard" => "Sulit", "Pause" => "Jeda", "Teruskan" => flowStage == "Warmup" ? "Bersiap" : flowStage == "Ready" ? "Mulai bermain" : "Lanjut",
            "Bantuan" => "Gerak: " + (assisted ? "Assisted" : "Manual"),
            "Joystick" => "Joystick: " + JoystickMode,
            "ReplayMode" => "Replay: " + (ReplayMode == 2 ? "OFF" : ReplayMode == 1 ? "Gameplay" : "Sudut + gameplay"),
            "TrailColor" => "Warna jejak: " + (trailTint == Color.yellow ? "Kuning" : "Putih"),
            "ShadowColor" => "Warna bayangan: #" + ColorUtility.ToHtmlStringRGB(shadowTint), "FrameRate" => Application.targetFrameRate + " FPS",
            "Challenge" => "Tinjau garis (" + Match.Challenges[0] + ")", "Ulangi" => "Ulangi pertandingan", _ => action
        };
        void DrawUiButton(string action)
        {
            Rect rect = UiActionRect(action); if (rect.width <= 0) return;
            bool selected = screen == GameScreen.Setup && (action == Difficulty || action == "Ganda" && doublesSelected || action == "Tunggal" && !doublesSelected);
            bool pressed = menuFinger != -1 && menuAction == action;
            bool primary = action == "Main" || action == "Mulai" || action == "Teruskan" || action == "Main lagi" || action == "Lanjut";
            bool link = screen == GameScreen.Home && (action == "Pengaturan" || action == "Kredit") || Match != null && Match.Winner >= 0 && action == "Menu";
            if (link)
            {
                var style = new GUIStyle(smallStyle) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
                style.normal.textColor = pressed ? accentColor : textColor; GUI.Label(rect, ActionLabel(action).ToUpperInvariant(), style); return;
            }
            DrawControl(rect, ActionLabel(action).ToUpperInvariant(), selected || primary, pressed, action == "Main" ? 50 : action == "Mulai" ? 40 : screen == GameScreen.Settings ? 24 : 28);
            if (selected) FillUi(new Rect(rect.x + 24, rect.yMax - 9, rect.width - 48, 3), inkColor);
        }
        void OnGUI()
        {
            uiScale = Mathf.Max(.01f, CanvasScale(Screen.safeArea.size));
            GUI.matrix = Matrix4x4.TRS(UiOrigin, Quaternion.identity, Vector3.one * uiScale);
            EnsureUiStyles(); float w = UiWidth, h = UiHeight;
            if (testerOpen) { DrawTester(); return; }
            bool playing = screen == GameScreen.Playing || screen == GameScreen.Training;
            if (!playing)
            {
                if (screen != GameScreen.Home && screen != GameScreen.Setup) FillUi(new Rect(0, 0, w, h), new Color(.027f, .098f, .243f, .62f));
                if (screen == GameScreen.Home || screen == GameScreen.Setup)
                {
                    MenuSweep(h);
                    GUI.Label(new Rect(48, 27, 424, 34), screen == GameScreen.Home ? EventName : "PERTANDINGAN", smallStyle);
                    if (screen == GameScreen.Home)
                    {
                        DisplayWord(new Rect(48, 104, 400, 156), "JUST", 127, textColor);
                        DisplayWord(new Rect(38, 230, 440, 166), "SMASH", 127, textColor);
                    }
                    else
                    {
                        DisplayWord(new Rect(44, 104, 450, 96), doublesSelected ? "GANDA" : "TUNGGAL", 70, textColor, -2);
                        GUI.Label(new Rect(52, 200, 410, 36), "PUTRA / 15 POIN / TERBAIK DARI 3", new GUIStyle(smallStyle) { fontSize = 18 });
                        GUI.Label(SetupTeamRect(0), TeamName(0).ToUpperInvariant(), bodyStyle);
                        GUI.Label(SetupTeamRect(1), "VS  " + TeamName(1).ToUpperInvariant(), bodyStyle);
                        GUI.Label(new Rect(52, 438, 350, 32), "KESULITAN LAWAN", new GUIStyle(smallStyle) { fontSize = 19 });
                    }
                    var venue = new GUIStyle(smallStyle) { alignment = TextAnchor.UpperRight };
                    if (screen == GameScreen.Home) GUI.Label(new Rect(w - 620, 30, 580, 36), "GELANGGANG JUST SMASH", venue);
                }
                else if (screen == GameScreen.Pause) { PanelUi(new Rect(w / 2 - 212, 40, 424, 604)); GUI.Label(new Rect(w / 2 - 180, 54, 360, 64), "JEDA", titleStyle); }
                else if (screen == GameScreen.Settings) { PanelUi(new Rect(24, 24, w - 48, h - 48)); GUI.Label(new Rect(44, 44, 450, 64), "Pengaturan", titleStyle); DrawSettings(); }
                else if (screen == GameScreen.Credits)
                {
                    PanelUi(new Rect(24, 24, w - 48, h - 48)); GUI.Label(new Rect(44, 44, 500, 64), "Kredit", titleStyle);
                    settingsScroll = GUI.BeginScrollView(new Rect(44, 130, w - 88, h - 170), settingsScroll, new Rect(0, 0, w - 120, Mathf.Max(680, bodyStyle.CalcHeight(new GUIContent(CreditsText), w - 140))));
                    GUI.Label(new Rect(0, 0, w - 140, Mathf.Max(680, bodyStyle.CalcHeight(new GUIContent(CreditsText), w - 140))), CreditsText, bodyStyle); GUI.EndScrollView();
                }
                else if (screen == GameScreen.Confirm) { PanelUi(new Rect(w / 2 - 212, h / 2 - 92, 424, 212)); GUI.Label(new Rect(w / 2 - 180, h / 2 - 64, 360, 82), confirmAction == "Exit" ? "Keluar dari permainan?" : confirmAction == "Menu" ? "Tinggalkan pertandingan?" : "Mulai ulang pertandingan?", bodyStyle); }
            }
            else DrawGameHud(w, h);
            foreach (string action in UiActions) DrawUiButton(action);
            if (MenuNotice.Length > 0) GUI.Label(new Rect(24, h - 56, w - 48, 48), MenuNotice, smallStyle);
        }
        public static Rect[] SettingsSliderRects(Vector2 size)
        {
            float x = Mathf.Max(500, size.x * .5f);
            return new[] { new Rect(x, 176, size.x - x - 48, 48), new Rect(x, 288, size.x - x - 48, 48), new Rect(x, 400, size.x - x - 48, 48) };
        }
        float SettingsSlider(Rect rect, float value)
        {
            value = GUI.HorizontalSlider(rect, value, 0, 1, sliderStyle, sliderThumbStyle);
            FillUi(new Rect(rect.x + 24, rect.center.y - 3, rect.width - 48, 6), textColor * .5f);
            FillUi(new Rect(rect.x + 24, rect.center.y - 3, (rect.width - 48) * value, 6), accentColor);
            FillUi(new Rect(rect.x + (rect.width - 48) * value, rect.y, 48, 48), accentColor);
            return value;
        }
        void DrawSettings()
        {
            var rects = SettingsSliderRects(new Vector2(UiWidth, UiHeight));
            float x = rects[0].x, width = rects[0].width;
            GUI.Label(new Rect(x, 130, width, 40), "Volume  " + Volume.ToString("P0"), bodyStyle);
            float volume = SettingsSlider(rects[0], Volume);
            if (Mathf.Abs(volume - Volume) > .001f) { Volume = volume; LocalGameData.SetFloat("volume", volume); }
            GUI.Label(new Rect(x, 242, width, 40), "Kecerahan jejak  " + trailBrightness.ToString("P0"), bodyStyle);
            float trail = SettingsSlider(rects[1], trailBrightness);
            GUI.Label(new Rect(x, 354, width, 40), "Kekuatan bayangan  " + shadowBrightness.ToString("P0"), bodyStyle);
            float shade = SettingsSlider(rects[2], shadowBrightness);
            if (Mathf.Abs(trail - trailBrightness) > .001f || Mathf.Abs(shade - shadowBrightness) > .001f) ConfigureVisibility(trail, shade, trailTint, shadowTint);
            GUI.Label(new Rect(x, 480, width, 150), "Manual: joystick menggerakkan kaki.\nAssisted: kaki dibantu; joystick memilih arah.\nKeyboard: WASD, 1–5 pukulan, Space lompat, L servis.", smallStyle);
        }
        string PointNotice
        {
            get
            {
                if (Match == null || (MatchState != "Point" && MatchState != "MatchEnd")) return "";
                string cause = pointCause switch
                {
                    PointCause.In => "Kok masuk", PointCause.Out => "Kok keluar",
                    PointCause.ServiceArea => "Servis di luar area", PointCause.BodyTouch => "Kok menyentuh badan",
                    PointCause.NetTouch => "Menyentuh net", PointCause.BelowNet => "Kok di bawah net",
                    _ => "Sebab poin tidak tersimpan"
                };
                return (Match.Server == 0 ? "Poin kamu" : "Poin lawan")
                    + (pointReview == PointReview.Overturned ? " (dikoreksi)" : pointReview == PointReview.Confirmed ? " (tetap)" : "") + "\n" + cause;
            }
        }
        string HudScoreHeader => Match == null ? "LATIHAN" : "GAME " + Match.GameNumber + " / " + ActionLabel(Difficulty);
        Rect HudNoticeRect => new Rect(24, 230, 350, 64);
        Rect FlowPanelRect => new Rect(24, 230, 350, 222);
        string ResultWinnerLabel => "Pemenang: " + TeamName(Match.Winner);
        string ResultGameLine(int game)
        {
            var points = Match.GamePoints;
            return "Game " + (game + 1) + "    " + (points == null || points.Length < (game + 1) * 2 || points[game * 2] < 0
                ? "Data lama tidak tersedia" : points[game * 2] + " : " + points[game * 2 + 1]);
        }
        public bool ShotPressed(int index) => pressedShot == index && shotFinger != -1;
        void DrawGameHud(float w, float h)
        {
            if (Match != null && Match.Winner >= 0)
            {
                MenuSweep(h);
                GUI.Label(new Rect(48, 27, 400, 34), EventName, smallStyle);
                DisplayWord(new Rect(44, 88, 430, 100), Match.Winner == 0 ? "KAMU" : "LAGA", 76, textColor);
                DisplayWord(new Rect(34, 178, 440, 100), Match.Winner == 0 ? "MENANG!" : "SELESAI", 76, accentColor);
                GUI.Label(new Rect(52, 280, 390, 32), ResultWinnerLabel, smallStyle);
                GUI.Label(new Rect(52, 316, 390, 30), MatchTitle, new GUIStyle(smallStyle) { fontSize = 20 });
                DisplayWord(new Rect(48, 342, 210, 68), Match.Games[0] + " : " + Match.Games[1], 52, textColor, -2);
                GUI.Label(new Rect(220, 370, 120, 32), "GAME", smallStyle);
                GUI.Label(new Rect(52, 411, 390, 27), TeamName(0) + " vs " + TeamName(1), new GUIStyle(smallStyle) { fontSize = 18 });
                var rowStyle = new GUIStyle(smallStyle) { fontSize = 20, wordWrap = false };
                for (int game = 0; game < Match.Games[0] + Match.Games[1]; game++)
                    GUI.Label(new Rect(52, 444 + game * 30, 390, 28), ResultGameLine(game), rowStyle);
                return;
            }
            Rect score = new Rect(24, 24, 350, Match == null ? 70 : 132); PanelUi(score);
            FillUi(new Rect(24, 24, 350, 5), accentColor);
            GUI.Label(new Rect(40, 32, 250, 26), HudScoreHeader, new GUIStyle(smallStyle) { fontSize = 18 });
            if (Match != null)
            {
                GUI.Label(new Rect(296, 32, 64, 24), "POIN", new GUIStyle(smallStyle) { fontSize = 16, alignment = TextAnchor.MiddleCenter });
                GUI.Label(new Rect(40, 58, 250, 32), (Match.Server == 0 ? "•  " : "   ") + TeamName(0), bodyStyle);
                GUI.Label(new Rect(40, 92, 250, 32), (Match.Server == 1 ? "•  " : "   ") + TeamName(1), bodyStyle);
                GUI.Label(new Rect(296, 54, 64, 40), Match.Points[0].ToString(), scoreStyle);
                GUI.Label(new Rect(296, 88, 64, 40), Match.Points[1].ToString(), scoreStyle);
                GUI.Label(new Rect(40, 128, 280, 24), Match.Games[0] + " : " + Match.Games[1] + " GAME", new GUIStyle(smallStyle) { fontSize = 18 });
            }
            FillUi(new Rect(24, score.yMax + 12, 180, 7), panelColor);
            FillUi(new Rect(24, score.yMax + 12, 180 * Energy / EnergyCapacity, 7), accentColor);
            GUI.Label(new Rect(216, score.yMax + 2, 160, 32), "ENERGI " + (Energy / EnergyCapacity).ToString("P0"), new GUIStyle(smallStyle) { fontSize = 18 });
            if (flowStage != null)
            {
                PanelUi(FlowPanelRect);
                GUI.Label(new Rect(40, 244, 318, 32), flowStage == "Warmup" ? MatchTitle : EventName, smallStyle);
                GUI.Label(new Rect(40, 284, 318, 76), flowStage == "Warmup" ? TeamName(0) + "\nvs " + TeamName(1) : flowStage == "Ready" ? "Siap servis" : flowStage == "GameEnd" ? "Game selesai" : flowStage == "Interval" ? "Istirahat" : "Ganti sisi lapangan", bodyStyle);
            }
            else if (MatchState == "Point" || MatchState == "Serving") GUI.Label(HudNoticeRect, MatchState == "Serving" ? (HumanServe ? "Giliran servis kamu" : Match.Server == 0 ? "Servis Dimas" : "Servis lawan") : PointNotice, smallStyle);
            if (replaying) return;
            if (feedback.StartsWith("Terlalu", StringComparison.Ordinal)) GUI.Label(HudNoticeRect, feedback, smallStyle);
            bool serve = Match != null && MatchState == "Serving" && HumanServe && flowStage == null;
            if (serve) DrawControl(ServeRect, servingCharge ? "SERVIS " + ServePower.ToString("P0") : "SERVIS PENDEK", true, servingCharge);
            else if (flowStage == null && (Match == null || MatchState == "Rally"))
            {
                MaskUi(StickRect, circleTexture, textColor);
                MaskUi(new Rect(StickRect.x + 4, StickRect.y + 4, StickRect.width - 8, StickRect.height - 8), circleTexture, inkColor);
                Vector2 dot = StickRect.center + analog * new Vector2(65, -65);
                MaskUi(new Rect(dot.x - 27, dot.y - 27, 54, 54), circleTexture, panelColor);
                if (jumpHeld) for (int i = 0; i < shots.Length; i++) DrawGuide(JumpRect.center, ShotRect(i).center);
                for (int i = 0; i < shots.Length; i++) DrawControl(ShotRect(i), (i == 1 ? "LOB" : shots[i].ToUpperInvariant()), ShotPressed(i), ShotPressed(i), 25);
                DrawControl(JumpRect, "LOMPAT", true, jumpHeld, 26);
            }
            if (DebugVisible) GUI.Label(new Rect(24, h - 40, w - 48, 36), ProfileSummary, smallStyle);
            if (Application.isEditor || Debug.isDebugBuild) DrawTester();
        }
    }
}
