using UnityEngine;
using UnityEngine.SceneManagement;

namespace JustSmash
{
    // ponytail: M1 training court, not match AI/scoring. Replace feeder in M2.
    public sealed partial class CourtPrototype : MonoBehaviour
    {
        static Shader _litShader, _spriteShader;
        static Shader LitShader => _litShader ??= Resources.Load<Shader>("CelCourt");
        static Shader SpriteShader => _spriteShader ??= Shader.Find("Sprites/Default") ?? Shader.Find("UI/Default");
        Transform player, opponent, shuttle, arm, racket, shadow, landingShadow;
        AthleteVisual playerVisual, opponentVisual;
        Camera view;
        LineRenderer armBones;
        Vector3 velocity;
        readonly Vector3[] athleteVelocity = new Vector3[4];
        Vector2 analog, lastAim, shotAim;
        readonly ShotIntent shotIntent = new ShotIntent();
        bool flying, assisted, charging, jumpHeld, servingCharge;
        int moveFinger = -1, shotFinger = -1, jumpFinger = -1, serveFinger = -1, pressedShot = -1;
        float chargedAt, serveChargedAt, jumpAt = -10, jumpPhase = 1, jumpY, lastFeed, swingAt = -10, diveRecoveryUntil = -10;
        const float StickRadius = 95f, StickDeadZone = .12f, StickCurvePower = 1.6f, AimNeutralX = .18f;
        Vector2 stickCenter;
        Vector2 DefaultStickCenter => new Vector2(128, UiHeight - 132);
        public string JoystickMode { get; private set; } = "Fixed";
        public float JoystickSensitivity { get; private set; } = .5f;
        public string CameraPreset { get; private set; } = "Standar";
        public float CameraZoom { get; private set; } = .5f;
        public float CameraHeight { get; private set; } = .5f;
        public float CameraAngle { get; private set; } = .5f;
        public void ConfigureCameraPreset(string preset)
        {
            if (preset == "Standar") ConfigureCamera(.5f, .5f, .5f, "Standar");
            else if (preset == "Tinggi") ConfigureCamera(.35f, .90f, .65f, "Tinggi");
            else if (preset == "Dekat") ConfigureCamera(.85f, .30f, .35f, "Dekat");
            else if (preset == "Kustom") ConfigureCamera(CameraZoom, CameraHeight, CameraAngle, "Kustom");
            else throw new System.ArgumentException("Unknown camera preset", nameof(preset));
        }
        public void ConfigureCamera(float zoom, float height, float angle, string preset = "Kustom")
        {
            if (!float.IsFinite(zoom) || !float.IsFinite(height) || !float.IsFinite(angle))
                throw new System.ArgumentOutOfRangeException(nameof(zoom));
            CameraZoom = Mathf.Clamp01(zoom);
            CameraHeight = Mathf.Clamp01(height);
            CameraAngle = Mathf.Clamp01(angle);
            CameraPreset = preset;
            LocalGameData.SetString("cameraPreset", CameraPreset);
            LocalGameData.SetFloat("cameraZoom", CameraZoom);
            LocalGameData.SetFloat("cameraHeight", CameraHeight);
            LocalGameData.SetFloat("cameraAngle", CameraAngle);
            LocalGameData.Save();
        }
        public void ResetCamera() => ConfigureCameraPreset("Standar");
        void ApplyGameplayCamera(bool immediate)
        {
            float dist = Mathf.Lerp(16.4f, 12.0f, CameraZoom);
            float fov = Mathf.Lerp(51f, 45f, CameraZoom);
            float baseY = Mathf.Lerp(2.8f, 5.2f, CameraHeight);
            float lookZ = Mathf.Lerp(-2.2f, .2f, CameraAngle);
            view.fieldOfView = fov;
            var cam = EndPoint(new Vector3(.2f, baseY + jumpY * .5f, -dist - jumpY * .4f));
            // Change ends must not interpolate through the court and clip both athletes.
            view.transform.position = immediate || view.transform.position.z * cam.z < 0 ? cam : Vector3.Lerp(view.transform.position, cam, Time.deltaTime * 6);
            view.transform.LookAt(EndPoint(new Vector3(.2f, jumpY * .15f, lookZ)));
        }
        public void ConfigureJoystick(string mode)
        {
            if (mode != "Fixed" && mode != "Floating") throw new System.ArgumentException("Unknown joystick mode", nameof(mode));
            JoystickMode = mode; moveFinger = -1; SetAnalog(Vector2.zero); stickCenter = DefaultStickCenter;
            LocalGameData.SetString("joystickMode", mode); LocalGameData.Save();
        }
        public void ConfigureJoystickSensitivity(float value)
        {
            if (!float.IsFinite(value) || value < 0 || value > 1) throw new System.ArgumentOutOfRangeException(nameof(value));
            JoystickSensitivity = value;
            LocalGameData.SetFloat("joystickSensitivity", value); LocalGameData.Save();
        }
        public static Vector2 ShapeJoystickDelta(Vector2 delta, float sensitivity)
        {
            float radius = Mathf.Lerp(105f, 35f, Mathf.Clamp01(sensitivity));
            Vector2 raw = Vector2.ClampMagnitude(delta / radius, 1f);
            float mag = raw.magnitude;
            if (mag <= StickDeadZone) return Vector2.zero;
            float scaled = Mathf.Pow((mag - StickDeadZone) / (1f - StickDeadZone), StickCurvePower);
            return (raw / mag) * scaled * new Vector2(1f, -1f);
        }
        string stroke = "Clear", feedback = "Feed lob to begin", lastStroke = "Clear";
        Vector2 jumpStart, jumpDrag;
        Vector3 contact;
        float uiScale;
        public static float CanvasScale(Vector2 size) => Mathf.Min(size.x / 1280f, size.y / 720f);
        Vector2 UiOrigin => new Vector2(Screen.safeArea.x, Screen.height - Screen.safeArea.yMax);
        float UiWidth => Screen.safeArea.width / uiScale;
        float EndSign => (Match != null && (HoldingSetScore ? flowEnds : Match.EndsSwapped)) ? -1 : 1;
        Vector3 EndPoint(Vector3 p) => new Vector3(p.x * EndSign, p.y, p.z * EndSign);
        public SinglesMatch Match { get; private set; }
        public string MatchState { get; private set; } = "Training";
        int lastHitter, energyGame = 1;
        Vector3 lastLanding;
        string beforePoint;
        enum PointCause { Unknown, In, Out, ServiceArea, BodyTouch, NetTouch, BelowNet }
        PointCause pointCause;
        enum PointReview { None, Confirmed, Overturned }
        PointReview pointReview;
        bool reviewAvailable, announcedIn, serviceFlight, judgeErrorUsed;
        uint judgeState = 37;
        bool AnnounceLanding(Vector3 p)
        {
            bool inside = LandingInside(p);
            float dx = Mathf.Max(Mathf.Abs(p.x) - (DoublesMode ? BootDiagnostics.Current.doublesHalfWidth : BootDiagnostics.Current.singlesHalfWidth), 0), dz = Mathf.Max(Mathf.Abs(p.z) - BootDiagnostics.Current.halfLength, 0);
            float distance = inside ? Mathf.Min((DoublesMode ? BootDiagnostics.Current.doublesHalfWidth : BootDiagnostics.Current.singlesHalfWidth) - Mathf.Abs(p.x), BootDiagnostics.Current.halfLength - Mathf.Abs(p.z)) : Mathf.Sqrt(dx * dx + dz * dz);
            if (!SimulateLineErrors || judgeErrorUsed || distance > .05f) return inside;
            judgeState ^= judgeState << 13; judgeState ^= judgeState >> 17; judgeState ^= judgeState << 5;
            if (judgeState / 4294967296.0 >= .03) return inside;
            judgeErrorUsed = true; return !inside;
        }
        public bool ChallengeCall()
        {
            if (Match == null || !reviewAvailable || (MatchState != "Point" && MatchState != "MatchEnd")) return false;
            bool actualIn = LandingInside(lastLanding), corrected = actualIn != announcedIn;
            if (!Match.Review(0, corrected)) return false;
            if (corrected)
            {
                if (string.IsNullOrEmpty(beforePoint)) throw new System.InvalidOperationException("Missing pre-point state");
                Match = JsonUtility.FromJson<SinglesMatch>(beforePoint);
                ScorePoint(actualIn ? (lastLanding.z * EndSign > 0 ? 0 : 1) : 1 - lastHitter);
            }
            reviewAvailable = false;
            pointCause = actualIn ? PointCause.In : PointCause.Out;
            pointReview = corrected ? PointReview.Overturned : PointReview.Confirmed;
            feedback = "Review: " + (actualIn ? "IN" : "OUT") + (corrected ? " | call overturned | left " : " | call confirmed | left ") + Match.Challenges[0];
            if (SetPointPending) FinishPoint();
            else
            {
                pointReplayed = false;
                BeginReplay(); // Line review is not an automatic end-of-set replay.
            }
            return true;
        }
        public float Energy { get; private set; } = 100;
        public int StaminaStat { get; private set; } = 70;
        public float EnergyCapacity => 100 * (1 + (StaminaStat - 70) * .01f);
        public void SetStaminaStat(int value)
        {
            if (value < 1 || value > 100) throw new System.ArgumentOutOfRangeException(nameof(value));
            if (Match != null && Match.Winner < 0) throw new System.InvalidOperationException("Cannot change stamina during match");
            StaminaStat = value; Energy = EnergyCapacity;
        }
        public void StartMatch()
        {
            StopCourtAudio();
            SkipReplay(); replay.Clear();
            EnterGameScreen(false);
            opponentRandom = opponentSeed; OpponentEnergy = partnerEnergy = secondEnergy = EnergyCapacity;
            Match = new SinglesMatch { Doubles = doublesSelected }; SetExtraAthletes(doublesSelected); judgeErrorUsed = false; judgeState = 37; lastHitter = 0; Energy = EnergyCapacity; energyGame = 1; beforePoint = null; ResetFlow();
            flowStage = null;
            NextPoint(); ResetFlow();
        }
        public void NextPoint()
        {
            if (Match == null) return;
            pointWait = 0;
            finalPointPending = false;
            reviewAvailable = false;
            ResetAthleteMovement();
            SpectacularMoment = false;
            landingShadow.gameObject.SetActive(false);
            flying = charging = serviceFlight = false; velocity = Vector3.zero; analog = lastAim = Vector2.zero; shotIntent.Clear();
            if (Match.Winner >= 0) { replay.Clear(); MatchState = "MatchEnd"; feedback = Match.Winner == 0 ? "You win" : "Opponent wins"; return; }
            pointCause = PointCause.Unknown; pointReview = PointReview.None;
            PrepareFlow();
            if (Match.GameNumber > energyGame) { Energy = Mathf.Min(EnergyCapacity, Energy + BootDiagnostics.Current.gameRecovery * (EnergyCapacity / 100)); OpponentEnergy = Mathf.Min(EnergyCapacity, OpponentEnergy + BootDiagnostics.Current.gameRecovery * (EnergyCapacity / 100));
                partnerEnergy = Mathf.Min(EnergyCapacity, partnerEnergy + BootDiagnostics.Current.gameRecovery * (EnergyCapacity / 100));
                secondEnergy = Mathf.Min(EnergyCapacity, secondEnergy + BootDiagnostics.Current.gameRecovery * (EnergyCapacity / 100)); energyGame = Match.GameNumber; }
            SkipReplay(); replay.Clear(); reviewAvailable = false;
            MatchState = "Serving"; view.fieldOfView = 48; SetShuttleVisible(true);
            serveReadyAt = Time.time + 1;
            float side = Match.Points[Match.Server] % 2 == 0 ? 1 : -1;
            player.position = EndPoint(new Vector3(side * 1.3f, 0, -4));
            player.rotation = Quaternion.Euler(0, EndSign < 0 ? 180 : 0, 0);
            opponent.rotation = Quaternion.Euler(0, EndSign < 0 ? 0 : 180, 0);
            opponent.position = EndPoint(new Vector3(-side * 1.3f, 0, 4));
            shuttle.position = (Match.Server == 0 ? player.position : opponent.position) + EndPoint(new Vector3(.35f, 1, Match.Server == 0 ? .45f : -.45f));
            PrepareDoubles(); ResetOpponent();
            previousPlayer = player.position; previousOpponent = opponent.position;
            feedback = Match.Server == 0 ? "Your serve" : "Opponent serve";
        }
        public void Let()
        {
            if (Match == null || MatchState != "Rally" || replaying || Time.timeScale == 0) return;
            NextPoint(); feedback = "Let | serve again"; PlaySound("whistle");
        }
        [System.Serializable] sealed class Snapshot
        {
            public int version = 1;
            public SinglesMatch match;
            public DoublesState doubles;
            public string state, beforePoint, flowStage;
            public PointCause pointCause;
            public PointReview pointReview;
            public string difficulty, pStroke, oStroke;
            public uint opponentSeed, opponentRandom;
            public bool intentPending, simulateLineErrors;
            public int intentKind;
            public Vector2 lastAim, shotAim, intentAim;
            public float intentPower, intentRemaining, opponentEnergy, observationAge, reactionRemaining, refreshRemaining, swingElapsed;
            public Vector3 opponentTarget;
            public ReplayFrame livePose;
            public int flowGame;
            public ReplayFrame[] frames;
            public Vector3 player, opponent, shuttle, velocity;
            public Vector3[] athleteVelocity;
            public bool flying, assisted, reviewAvailable, announcedIn, serviceFlight, judgeErrorUsed, spectacular, finalPointPending;
            public uint judgeState = 37;
            public Vector3 landing;
            public int hitter, energyGame, staminaStat = 70;
            public float energy, jumpElapsed, jumpHeight, chargeElapsed, aiElapsed, serveRemaining, pointWait;
            public Vector2 analog;
            public bool charging;
            public string stroke;
        }
        public string CaptureMatch()
        {
            if (Match == null) throw new System.InvalidOperationException("No active match");
            float now = replaying ? replayGameAt : Time.time;
            var frame = replaying ? liveFrame : new ReplayFrame { player = player.position, opponent = opponent.position, shuttle = shuttle.position };
            return JsonUtility.ToJson(new Snapshot { version = 3, athleteVelocity = (Vector3[])athleteVelocity.Clone(), doubles = CaptureDoubles(), difficulty = Difficulty, opponentSeed = opponentSeed, opponentRandom = opponentRandom,
                opponentEnergy = OpponentEnergy, observationAge = now - observedSwing, reactionRemaining = Mathf.Clamp(opponentThinkAt - now, 0, 1), refreshRemaining = Mathf.Clamp(opponentRefreshAt - now, 0, 1), opponentTarget = opponentTarget,
                lastAim = lastAim, shotAim = shotAim, intentPending = shotIntent.Pending, intentKind = shotIntent.Kind, intentAim = shotIntent.Aim, intentPower = shotIntent.Power, intentRemaining = Mathf.Clamp(shotIntent.ExpiresAt - now, 0, ShotIntent.Window),
                simulateLineErrors = SimulateLineErrors, livePose = replaying ? liveFrame : CaptureFrame(), swingElapsed = now - swingAt, pStroke = lastStroke, oStroke = opponentStroke, pointWait = pointWait, finalPointPending = finalPointPending, match = Match, spectacular = SpectacularMoment, judgeErrorUsed = judgeErrorUsed, judgeState = judgeState, state = MatchState, flowStage = flowStage, flowGame = flowGame, beforePoint = beforePoint, pointCause = pointCause, pointReview = pointReview, frames = replay.ToArray(), reviewAvailable = reviewAvailable, announcedIn = announcedIn, serviceFlight = serviceFlight, landing = lastLanding, player = frame.player, opponent = frame.opponent, shuttle = frame.shuttle, velocity = velocity, flying = flying, assisted = assisted, hitter = lastHitter, energyGame = energyGame, staminaStat = StaminaStat, energy = Energy, jumpElapsed = jumpPhase, jumpHeight = jumpY, chargeElapsed = now - chargedAt, aiElapsed = now - aiContactAt, serveRemaining = Mathf.Clamp(serveReadyAt - now, 0, 1), analog = analog, charging = charging, stroke = stroke });
        }
        public void RestoreMatch(string json)
        {
            var s = ReadSnapshot(json);
            ApplySnapshot(s);
        }
        internal static string CareerSnapshotResult(string json)
        {
            var s = ReadSnapshot(json);
            if (s.version != 3 || s.match.Doubles || s.difficulty != "Normal")
                throw new System.ArgumentException("Snapshot karier bukan laga Tunggal Normal");
            return s.match.Winner >= 0 && !s.finalPointPending && !s.reviewAvailable ? EncodeResult(s.match) : null;
        }
        static Snapshot ReadSnapshot(string json)
        {
            // ponytail: bounded JSON for the existing 1500-frame/four-athlete replay; larger histories need a compact format.
            if (string.IsNullOrEmpty(json) || json.Length > 8000000) throw new System.ArgumentException("Invalid snapshot size");
            var s = JsonUtility.FromJson<Snapshot>(json);
            if (s == null || (s.version < 1 || s.version > 3) || s.match == null || s.match.Points == null || s.match.Games == null || s.match.Points.Length != 2 || s.match.Games.Length != 2 || s.match.Server < 0 || s.match.Server > 1 || s.match.Winner < -1 || s.match.Winner > 1 || s.match.GameNumber < 1 || s.match.GameNumber > 3 || s.hitter < 0 || s.hitter > 1)
                throw new System.ArgumentException("Invalid snapshot schema");
            if (s.state != "Serving" && s.state != "Rally" && s.state != "Point" && s.state != "MatchEnd") throw new System.ArgumentException("Invalid match state");
            foreach (int n in s.match.Points) if (n < 0 || n > 21) throw new System.ArgumentException("Invalid points");
            foreach (int n in s.match.Games) if (n < 0 || n > 2) throw new System.ArgumentException("Invalid games");
            bool ended = s.match.Winner >= 0;
            if ((s.state == "MatchEnd") != ended || (s.state == "Rally") != s.flying
                || (s.serviceFlight && (!s.flying || s.hitter != s.match.Server))
                || (s.charging && s.state != "Rally")
                || s.match.GameNumber != s.match.Games[0] + s.match.Games[1] + (ended ? 0 : 1)
                || (ended && s.match.Games[s.match.Winner] != 2)
                || (!ended && (s.match.Games[0] == 2 || s.match.Games[1] == 2)))
                throw new System.ArgumentException("Contradictory match state");
            if (s.pointCause < PointCause.Unknown || s.pointCause > PointCause.BelowNet
                || (s.pointCause != PointCause.Unknown && s.state != "Point" && s.state != "MatchEnd"))
                throw new System.ArgumentException("Invalid point cause");
            if (s.pointReview < PointReview.None || s.pointReview > PointReview.Overturned
                || (s.pointReview != PointReview.None && (s.reviewAvailable || (s.pointCause != PointCause.In && s.pointCause != PointCause.Out))))
                throw new System.ArgumentException("Invalid point review");
            s.match.ValidateGamePoints(s.version < 3);
            foreach (var v in new[] { s.player, s.opponent, s.shuttle, s.velocity, s.landing })
                if (!float.IsFinite(v.x) || !float.IsFinite(v.y) || !float.IsFinite(v.z) || v.sqrMagnitude > 1000000) throw new System.ArgumentException("Invalid snapshot vector");
            if (s.staminaStat < 1 || s.staminaStat > 100 || !float.IsFinite(s.energy) || s.energy < 0 || s.energy > 100 * (1 + (s.staminaStat - 70) * .01f) + .001f) throw new System.ArgumentException("Invalid energy");
            if (!float.IsFinite(s.serveRemaining) || s.serveRemaining < 0 || s.serveRemaining > 1)
                throw new System.ArgumentException("Invalid serve timer");
            if (s.match.Challenges == null || s.match.Challenges.Length != 2 || System.Array.Exists(s.match.Challenges, n => n < 0 || n > 2))
                throw new System.ArgumentException("Invalid challenges");
            if (s.energyGame < 1 || s.energyGame > s.match.GameNumber || !float.IsFinite(s.jumpHeight) || s.jumpHeight < 0 || s.jumpHeight > .7f || !float.IsFinite(s.analog.x) || !float.IsFinite(s.analog.y) || s.analog.sqrMagnitude > 1.0001f)
                throw new System.ArgumentException("Invalid movement state");
            foreach (float elapsed in new[] { s.jumpElapsed, s.chargeElapsed, s.aiElapsed })
                if (!float.IsFinite(elapsed) || elapsed < 0) throw new System.ArgumentException("Invalid elapsed timer");
            if (System.Array.IndexOf(shots, s.stroke) < 0) throw new System.ArgumentException("Invalid stroke");
            if (!string.IsNullOrEmpty(s.flowStage) && s.flowStage != "Warmup" && s.flowStage != "Ready" && s.flowStage != "GameEnd" && s.flowStage != "Interval" && s.flowStage != "ChangeEnds") throw new System.ArgumentException("Invalid flow stage");
            if (s.flowGame < 0 || s.flowGame > s.match.GameNumber) throw new System.ArgumentException("Invalid flow game");
            if (!float.IsFinite(s.pointWait) || s.pointWait < 0 || s.pointWait > 3) throw new System.ArgumentException("Invalid point timer");
            if (s.finalPointPending && !ended) throw new System.ArgumentException("Invalid pending match result");
            if (s.reviewAvailable && string.IsNullOrEmpty(s.beforePoint)) throw new System.ArgumentException("Missing review history");
            if (!string.IsNullOrEmpty(s.beforePoint))
            {
                var prior = JsonUtility.FromJson<SinglesMatch>(s.beforePoint);
                if (!s.beforePoint.Contains("\"Points\"") || !s.beforePoint.Contains("\"Games\"") || prior == null
                    || prior.Points == null || prior.Points.Length != 2 || System.Array.Exists(prior.Points, n => n < 0 || n > 20)
                    || prior.Games == null || prior.Games.Length != 2 || System.Array.Exists(prior.Games, n => n < 0 || n > 1)
                    || prior.Challenges == null || prior.Challenges.Length != 2 || System.Array.Exists(prior.Challenges, n => n < 0 || n > 2)
                    || prior.Server < 0 || prior.Server > 1 || prior.Winner != -1 || prior.GameNumber < 1 || prior.GameNumber > 3)
                    throw new System.ArgumentException("Invalid review history");
                prior.ValidateGamePoints(s.version < 3);
                if (prior.Doubles != s.match.Doubles || prior.GamePoints.Length > s.match.GamePoints.Length)
                    throw new System.ArgumentException("Contradictory review history");
                for (int i = 0; i < prior.GamePoints.Length; i++)
                    if (prior.GamePoints[i] != s.match.GamePoints[i]) throw new System.ArgumentException("Contradictory review game points");
                s.beforePoint = JsonUtility.ToJson(prior);
                if (s.reviewAvailable)
                {
                    if (prior.Doubles && (prior.RightPlayers == null || prior.RightPlayers.Length != 2
                        || (prior.RightPlayers[0] != 0 && prior.RightPlayers[0] != 2) || (prior.RightPlayers[1] != 1 && prior.RightPlayers[1] != 3)))
                        throw new System.ArgumentException("Invalid review service order");
                    prior.AwardPoint(s.match.Server);
                    if (prior.Winner != s.match.Winner || prior.GameNumber != s.match.GameNumber || prior.GamePoints.Length != s.match.GamePoints.Length)
                        throw new System.ArgumentException("Contradictory review transition");
                    for (int i = 0; i < 2; i++)
                        if (prior.Points[i] != s.match.Points[i] || prior.Games[i] != s.match.Games[i]) throw new System.ArgumentException("Contradictory review score");
                    for (int i = 0; i < prior.GamePoints.Length; i++)
                    {
                        if (s.version < 3 && s.match.GamePoints[i] < 0) s.match.GamePoints[i] = prior.GamePoints[i];
                        if (prior.GamePoints[i] != s.match.GamePoints[i]) throw new System.ArgumentException("Contradictory review game points");
                    }
                }
            }
            if (s.frames != null)
            {

                if (s.frames.Length > 1500) throw new System.ArgumentException("Too many replay frames");
                foreach (var f in s.frames) foreach (var v in new[] { f.player, f.opponent, f.shuttle })
                    if (!float.IsFinite(v.x) || !float.IsFinite(v.y) || !float.IsFinite(v.z) || v.sqrMagnitude > 1000000)
                        throw new System.ArgumentException("Invalid replay vector");
            }
            if (s.athleteVelocity != null && (s.athleteVelocity.Length != 4 || System.Array.Exists(s.athleteVelocity,
                v => !Bounded(v) || v.y != 0 || v.sqrMagnitude > 20.2501f)))
                throw new System.ArgumentException("Invalid athlete velocity");
            ValidateMobileSnapshot(s);
            ValidateDoubles(s);
            return s;
        }
        void ApplySnapshot(Snapshot s)
        {
            bool ended = s.match.Winner >= 0;
            SkipReplay(); replay.Clear();
            if (s.frames != null)
            {
                replay.AddRange(s.frames);
            }
            System.Array.Clear(athleteVelocity, 0, athleteVelocity.Length);
            if (s.athleteVelocity != null) System.Array.Copy(s.athleteVelocity, athleteVelocity, 4);
            pointWait = s.pointWait;
            finalPointPending = ended && (s.finalPointPending || s.reviewAvailable);
            SpectacularMoment = s.spectacular;
            Match = s.match;
            flowGame = s.flowGame == 0 ? s.match.GameNumber : s.flowGame;
            flowEnds = s.state == "Point" && s.match.Winner < 0 && flowGame < s.match.GameNumber ? !Match.EndsSwapped : Match.EndsSwapped;
            player.rotation = Quaternion.Euler(0, EndSign < 0 ? 180 : 0, 0);
            opponent.rotation = Quaternion.Euler(0, EndSign < 0 ? 0 : 180, 0);
            judgeErrorUsed = s.judgeErrorUsed; judgeState = s.judgeState; MatchState = s.state; player.position = s.player; opponent.position = s.opponent; shuttle.position = s.shuttle;
            velocity = s.velocity; flying = s.flying; assisted = s.assisted; lastHitter = s.hitter;
            energyGame = s.energyGame;
            flowStage = string.IsNullOrEmpty(s.flowStage) ? null : s.flowStage;
            beforePoint = s.beforePoint; pointCause = s.pointCause; pointReview = s.pointReview;
            reviewAvailable = s.reviewAvailable; announcedIn = s.announcedIn; lastLanding = s.landing; serviceFlight = s.serviceFlight;
            StaminaStat = s.staminaStat; Energy = Mathf.Min(s.energy, EnergyCapacity); jumpAt = Time.time - s.jumpElapsed; jumpPhase = s.jumpElapsed; jumpY = s.jumpHeight;
            charging = s.charging; chargedAt = Time.time - s.chargeElapsed; stroke = s.stroke; analog = s.analog;
            serveReadyAt = Time.time + s.serveRemaining; aiContactAt = Time.time - s.aiElapsed;
            RestoreDoubles(s);
            RestoreMobileSnapshot(s);
            if (flying) MarkLanding(); else landingShadow.gameObject.SetActive(false);
            feedback = "Match restored";
        }
        [System.Serializable] sealed class MatchResult
        {
            public int version = 2, winner;
            public string rules = "Official15";
            public int[] games, finalGamePoints, gamePoints;
        }
        public string CaptureResult()
        {
            if (Match == null || Match.Winner < 0) throw new System.InvalidOperationException("Match not finished");
            return EncodeResult(Match);
        }
        static string EncodeResult(SinglesMatch match) => JsonUtility.ToJson(new MatchResult { winner = match.Winner, games = match.Games, finalGamePoints = match.Points, gamePoints = match.GamePoints });
        string SavePath => System.IO.Path.Combine(LocalGameData.Root, "singles-v1.json");
        public void SaveMatch()
        {
            if (careerMatchId != null)
            {
                if (ShowMatchResult) RecordCareerResult();
                else ChangeCareer(next => next.matchSnapshot = CaptureMatch());
                return;
            }
            System.IO.Directory.CreateDirectory(LocalGameData.Root);
            string json = CaptureMatch(), temp = SavePath + ".tmp";
            System.IO.File.WriteAllText(temp, json);
            if (System.IO.File.Exists(SavePath)) System.IO.File.Replace(temp, SavePath, SavePath + ".bak");
            else System.IO.File.Move(temp, SavePath);
            feedback = "Match saved"; BootDiagnostics.Log("save", "snapshot saved");
        }
        public void LoadMatch()
        {
            if (careerMatchId != null) throw new System.InvalidOperationException("Muat karier dari slot");
            RestoreMatch(System.IO.File.ReadAllText(SavePath));
        }
        void OnApplicationPause(bool paused)
        {
            if (!paused) return; // Resume remains explicit; never advance behind an OS overlay.
            ClearGameplayInput();
            if (Match != null && (Match.Winner < 0 || (careerMatchId != null && !careerResultSaved))) RunMenuAction("Save");
            string pendingNotice = careerMatchId != null && !careerResultSaved ? menuNotice : null;
            if (screen == GameScreen.Playing || screen == GameScreen.Training) OpenUi(GameScreen.Pause);
            if (!string.IsNullOrEmpty(pendingNotice)) menuNotice = pendingNotice;
            Time.timeScale = 0;
        }
        public bool Flying => flying;
        public Vector3 ShuttleVelocity => velocity;
        public Vector3 PlayerPosition => player.position;
        public static Vector3 CourtDimensions => new Vector3(BootDiagnostics.Current.singlesHalfWidth, BootDiagnostics.Current.netHeight, BootDiagnostics.Current.halfLength);
        public Vector2 Analog => analog;
        public static Vector3 CourtPoint(Vector2 direction, float power)
        {
            direction = Vector2.ClampMagnitude(direction, 1);
            float x = Mathf.Abs(direction.x) < AimNeutralX ? 0 : direction.x < 0 ? -1.8f : 1.8f;
            return new Vector3(x, 0, direction.y < -.33f ? 1.1f : direction.y > .33f ? 5.8f : 3.5f);
        }
        public static Vector3 StepVelocity(Vector3 v, float dt) => v + (Physics.gravity - BootDiagnostics.Current.drag * v.magnitude * v) * dt;
        public static Vector3 ElbowPosition(Vector3 shoulder, Vector3 hand, float upper, float lower)
        {
            var delta = hand - shoulder;
            float distance = Mathf.Clamp(delta.magnitude, .001f, upper + lower - .001f);
            var axis = delta.sqrMagnitude < .000001f ? Vector3.right : delta.normalized;
            float along = (upper * upper - lower * lower + distance * distance) / (2 * distance);
            var bend = Vector3.Cross(axis, Vector3.forward).normalized;
            if (bend.sqrMagnitude < .01f) bend = Vector3.up;
            return shoulder + axis * along + bend * Mathf.Sqrt(Mathf.Max(0, upper * upper - along * along));
        }
        public static Vector3 LaunchVelocity(Vector3 start, Vector3 target, float duration)
        {
            if (duration < .2f || duration > 3 || !float.IsFinite(duration)) throw new System.ArgumentOutOfRangeException(nameof(duration));
            const float dt = .02f;
            int steps = Mathf.RoundToInt(duration / dt);
            duration = steps * dt;
            var guess = (target - start) / duration - Physics.gravity * duration * .5f;
            for (int iteration = 0; iteration < 18; iteration++)
            {
                var p = start; var v = guess;
                for (int i = 0; i < steps; i++) { v = StepVelocity(v, dt); p += v * dt; }
                var error = target - p;
                if (error.sqrMagnitude < .0004f) break;
                guess += error / duration * 1.5f;
            }
            return guess;
        }
        const float CourtLineWidth = .04f, ShortServiceLine = 1.98f;
        // Avoid subtraction ULP excluding the exact centimetre boundary (6.70 - 0.76).
        static float DoublesServiceLine => (float)System.Math.Round(BootDiagnostics.Current.halfLength - .76, 4);
        public static bool InCourt(Vector3 p, bool doubles = false) => Mathf.Abs(p.x) <= (doubles ? BootDiagnostics.Current.doublesHalfWidth : BootDiagnostics.Current.singlesHalfWidth) && Mathf.Abs(p.z) <= BootDiagnostics.Current.halfLength;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void StartCourt()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
        static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Boot" && FindFirstObjectByType<CourtPrototype>() == null)
                new GameObject("CourtPrototype").AddComponent<CourtPrototype>();
        }
        void Awake()
        {
            var old = FindFirstObjectByType<BootDiagnostics>();
            if (old != null) old.enabled = false;
            BuildCourt();
            BuildArena();
            LoadVisibility();
            string joystick = LocalGameData.GetString("joystickMode", "Fixed");
            JoystickMode = joystick == "Floating" ? "Floating" : "Fixed";
            previousPlayer = player.position;
            InitializeUi(); StageMenu();
        }
        Material Mat(Color color)
        {
            var m = new Material(LitShader); m.color = color; return m;
        }
        Transform Shape(string name, PrimitiveType type, Vector3 position, Vector3 size, Color color, Transform parent = null)
        {
            var go = GameObject.CreatePrimitive(type); go.name = name;
            go.transform.SetParent(parent, false); go.transform.localPosition = position; go.transform.localScale = size;
            go.GetComponent<Renderer>().sharedMaterial = Mat(color);
            var collider = go.GetComponent<Collider>(); if (collider != null) Destroy(collider);
            return go.transform;
        }
        void Line(string name, Vector3 from, Vector3 to, float width, Color color, Transform parent = null)
        {
            var go = new GameObject(name); go.transform.SetParent(parent, false);
            var line = go.AddComponent<LineRenderer>(); line.positionCount = 2; line.useWorldSpace = false;
            line.SetPosition(0, from); line.SetPosition(1, to); line.startWidth = line.endWidth = width;
            line.sharedMaterial = new Material(SpriteShader); line.startColor = line.endColor = color;
        }
        // ponytail: primitive mannequin with real joints (hip->knee->ankle, shoulder->elbow->wrist). Upgrade to licensed FBX rig when owner accepts feel.
        Transform Joint(string name, Vector3 pos, Vector3 size, Color color, Transform parent)
        {
            var pivot = new GameObject(name).transform; pivot.SetParent(parent, false); pivot.localPosition = pos;
            Shape(name + "Mesh", PrimitiveType.Capsule, new Vector3(0, -size.y * .5f, 0), new Vector3(size.x, size.y * .5f, size.z), color, pivot);
            bool torso = name == "Spine" || name == "Chest" || name == "Neck";
            if (torso) pivot.Find(name + "Mesh").localPosition = Vector3.up * size.y * .5f;
            Shape(name + "Socket", PrimitiveType.Sphere, Vector3.zero, new Vector3(size.x, size.x, size.z), color, pivot);
            return pivot;
        }
        Transform Athlete(string name, Vector3 position, Color color)
        {
            var root = new GameObject(name).transform; root.position = position;
            // pelvis at hip height ~0.92m; torso rises from there
            var hips = Joint("Hips", new Vector3(0, .92f, 0), new Vector3(.34f, .18f, .22f), color, root);
            var spine = Joint("Spine", new Vector3(0, .12f, 0), new Vector3(.36f, .35f, .24f), color, hips);
            var chest = Joint("Chest", new Vector3(0, .35f, 0), new Vector3(.42f, .28f, .26f), color, spine);
            var neck = Joint("Neck", new Vector3(0, .28f, 0), new Vector3(.14f, .1f, .14f), new Color(.65f, .44f, .3f), chest);
            Shape("Head", PrimitiveType.Sphere, new Vector3(0, .18f, 0), Vector3.one * .28f, new Color(.65f, .44f, .3f), neck);
            skin = new Color(.65f, .44f, .3f); cloth = color;
            // legs: thigh 0.45m, shin 0.45m -> foot lands at y~0.02
            var lHip = Joint("LegLeft", new Vector3(-.13f, -.09f, 0), new Vector3(.16f, .45f, .17f), cloth, hips);
            var lKnee = Joint("KneeLeft", new Vector3(0, -.45f, 0), new Vector3(.14f, .45f, .15f), skin, lHip);
            var footLeft = new GameObject("FootLeft").transform;
            footLeft.SetParent(lKnee, false); footLeft.localPosition = new Vector3(0, -.45f, 0);
            Shape("ShoeLeft", PrimitiveType.Cube, new Vector3(0, -.025f, .055f), new Vector3(.16f, .10f, .28f), new Color(.88f,.9f,.92f), footLeft);
            var rHip = Joint("LegRight", new Vector3(.13f, -.09f, 0), new Vector3(.16f, .45f, .17f), cloth, hips);
            var rKnee = Joint("KneeRight", new Vector3(0, -.45f, 0), new Vector3(.14f, .45f, .15f), skin, rHip);
            var footRight = new GameObject("FootRight").transform;
            footRight.SetParent(rKnee, false); footRight.localPosition = new Vector3(0, -.45f, 0);
            Shape("ShoeRight", PrimitiveType.Cube, new Vector3(0, -.025f, .055f), new Vector3(.16f, .10f, .28f), new Color(.88f,.9f,.92f), footRight);
            Shape("ShoulderBridge", PrimitiveType.Capsule, new Vector3(0, .1f, 0), new Vector3(.23f, .25f, .24f), color, chest).localRotation = Quaternion.Euler(0, 0, 90);
            // arms attach to chest; non-racket arm mirrored
            var lShoulder = Joint("ShoulderLeft", new Vector3(-.24f, .1f, 0), new Vector3(.13f, .32f, .13f), skin, chest);
            var lElbow = Joint("ElbowLeft", new Vector3(0, -.32f, 0), new Vector3(.11f, .3f, .11f), skin, lShoulder);
            Shape("HandLeft", PrimitiveType.Sphere, new Vector3(0, -.3f, 0), Vector3.one * .1f, skin, lElbow);
            var rShoulder = Joint("ShoulderRight", new Vector3(.24f, .1f, 0), new Vector3(.13f, .32f, .13f), skin, chest);
            var rElbow = Joint("ElbowRight", new Vector3(0, -.32f, 0), new Vector3(.11f, .3f, .11f), skin, rShoulder);
            Shape("HandRight", PrimitiveType.Sphere, new Vector3(0, -.3f, 0), Vector3.one * .1f, skin, rElbow);
            Shape("RacketHandle", PrimitiveType.Cylinder, new Vector3(0, -.43f, 0), new Vector3(.025f, .13f, .025f), Color.gray, rElbow);
            // expose racket-hand anchor for existing code paths
            arm = rElbow; armAnchor = rShoulder; handAnchor = rElbow.Find("HandRight");
            return root;
        }
        Color skin, cloth;
        Transform armAnchor, handAnchor;
        void PaintLine(string name, Vector3 from, Vector3 to, Material material)
        {
            var span = to - from;
            ArenaPart(name, (from + to) * .5f, new Vector3(Mathf.Abs(span.x) > 0 ? Mathf.Abs(span.x) : CourtLineWidth, .001f,
                Mathf.Abs(span.z) > 0 ? Mathf.Abs(span.z) : CourtLineWidth), material);
        }
        void BuildCourt()
        {
            var hall = ArenaMaterial(new Color(.075f, .12f, .20f));
            var wall = ArenaMaterial(new Color(.065f, .09f, .13f));
            var beam = ArenaMaterial(new Color(.09f, .12f, .16f));
            ArenaPart("HallFloor", new Vector3(0, -.055f, 0), new Vector3(24, .1f, 38), hall);
            ArenaPart("HallBackWall", new Vector3(0, 4, 16), new Vector3(24, 8, .2f), wall);
            foreach (float side in new[] { -1f, 1f })
            {
                ArenaPart("HallSideWall", new Vector3(side * 10, 4, 0), new Vector3(.2f, 8, 28), wall);
                for (int i = -1; i <= 2; i++) ArenaPart("HallBeam", new Vector3(side * 9.8f, 4, i * 6), new Vector3(.22f, 8, .22f), beam);
            }
            ArenaPart("Court", new Vector3(0, -.05f, 0), new Vector3(8, .1f, 16), CourtSurface());
            var white = new Color(.93f, .93f, .89f);
            var paint = ArenaMaterial(white);
            float width = BootDiagnostics.Current.doublesHalfWidth, length = BootDiagnostics.Current.halfLength;
            foreach (float edge in new[] { -width, -BootDiagnostics.Current.singlesHalfWidth, BootDiagnostics.Current.singlesHalfWidth, width })
            {
                float x = edge - Mathf.Sign(edge) * CourtLineWidth * .5f;
                PaintLine("Sideline", new Vector3(x, .001f, -length), new Vector3(x, .001f, length), paint);
            }
            foreach (float edge in new[] { -length, -DoublesServiceLine, -ShortServiceLine, ShortServiceLine, DoublesServiceLine, length })
            {
                float z = edge + Mathf.Sign(edge) * (Mathf.Abs(edge) == ShortServiceLine ? 1 : -1) * CourtLineWidth * .5f;
                PaintLine("CourtLine", new Vector3(-width, .001f, z), new Vector3(width, .001f, z), paint);
            }
            PaintLine("ServiceCentre", new Vector3(0, .001f, -length), new Vector3(0, .001f, -ShortServiceLine), paint);
            PaintLine("ServiceCentre", new Vector3(0, .001f, ShortServiceLine), new Vector3(0, .001f, length), paint);
            BuildNet(white);
            player = Athlete("Player", new Vector3(0, 0, -3.5f), new Color(.05f, .8f, .82f));
            opponent = Athlete("Opponent", new Vector3(0, 0, 4), new Color(.82f, .48f, .25f)); opponent.rotation = Quaternion.Euler(0, 180, 0);
            // racket rides in the jointed right hand; ArmIK line renders upper+forearm as visible bones
            racket = Shape("Racket", PrimitiveType.Sphere, new Vector3(0, -.62f, 0), new Vector3(.24f, .30f, .018f), white, player.Find("Hips/Spine/Chest/ShoulderRight/ElbowRight"));
            var oHand = opponent.Find("Hips/Spine/Chest/ShoulderRight/ElbowRight");
            Shape("Racket", PrimitiveType.Sphere, new Vector3(0, -.62f, 0), new Vector3(.24f, .30f, .018f), white, oHand);
            var bones = new GameObject("ArmIK"); armBones = bones.AddComponent<LineRenderer>();
            armBones.positionCount = 3; armBones.startWidth = armBones.endWidth = .09f;
            armBones.sharedMaterial = new Material(SpriteShader);
            playerVisual = AthleteVisual.Attach(player); opponentVisual = AthleteVisual.Attach(opponent);
            racket = playerVisual.StringBed; armBones.enabled = false;
            foreach (var athlete in new[] { player, opponent }) AddContactShadow(athlete);
            previousOpponent = opponent.position;
            shuttle = new GameObject("Shuttle").transform;
            var cork = Shape("Cork", PrimitiveType.Sphere, Vector3.zero, Vector3.one * .075f, white, shuttle);
            var shuttleMaterial = cork.GetComponent<Renderer>().sharedMaterial;
            shuttleMaterial.SetFloat("_Outline", .012f); shuttleMaterial.SetFloat("_Unlit", 1); arenaMaterials.Add(shuttleMaterial);
            // ponytail: solid eight-panel skirt for match-camera readability; detailed feathers only if close-ups need them.
            // Skirt matches old spoke-tip extent; root, cork and contact coordinates stay unchanged.
            var vertices = new Vector3[18]; var triangles = new int[96];
            for (int i = 0; i < 8; i++)
            {
                float a = i * Mathf.PI / 4; var radial = new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a));
                vertices[i] = radial * .025f + Vector3.up * .025f;
                vertices[i + 8] = radial * .08f + Vector3.up * .17f;
                int n = (i + 1) % 8, t = i * 12;
                triangles[t] = i; triangles[t + 1] = i + 8; triangles[t + 2] = n + 8;
                triangles[t + 3] = i; triangles[t + 4] = n + 8; triangles[t + 5] = n;
                triangles[t + 6] = 16; triangles[t + 7] = i; triangles[t + 8] = n;
                triangles[t + 9] = 17; triangles[t + 10] = n + 8; triangles[t + 11] = i + 8;
            }
            vertices[16] = Vector3.up * .025f; vertices[17] = Vector3.up * .17f;
            var skirtMesh = new Mesh { name = "Shuttle feather skirt", vertices = vertices, triangles = triangles };
            skirtMesh.RecalculateNormals(); skirtMesh.RecalculateBounds(); arenaMeshes.Add(skirtMesh);
            var skirt = new GameObject("FeatherSkirt"); skirt.transform.SetParent(shuttle, false);
            skirt.AddComponent<MeshFilter>().sharedMesh = skirtMesh;
            skirt.AddComponent<MeshRenderer>().sharedMaterial = shuttleMaterial;
            shuttle.position = new Vector3(0, .1f, -3);
            var trail = shuttle.gameObject.AddComponent<TrailRenderer>(); trail.time = .12f; trail.minVertexDistance = .015f; trail.startWidth = .12f; trail.endWidth = .01f;
            trail.sharedMaterial = new Material(SpriteShader); trail.startColor = new Color(1f, .95f, .6f, .85f); trail.endColor = Color.clear;
            shadow = Shape("ShuttleShadow", PrimitiveType.Sphere, new Vector3(0, .02f, -3), new Vector3(.12f, .005f, .12f), new Color(.1f, .14f, .14f));
            landingShadow = Shape("ShuttleLandingShadow", PrimitiveType.Sphere, new Vector3(0, .015f, -3), new Vector3(.32f, .006f, .32f), new Color(.95f, .8f, .25f));
            view = Camera.main;
            if (view == null) view = new GameObject("Main Camera", typeof(Camera)).GetComponent<Camera>();
            view.transform.position = new Vector3(.2f, 4f, -14.2f); view.transform.LookAt(new Vector3(.2f, 0, -1)); view.fieldOfView = 48;
            view.backgroundColor = new Color(.14f, .18f, .19f);
            var existingLight = FindFirstObjectByType<Light>();
            Light l = existingLight != null ? existingLight : new GameObject("CourtLight").AddComponent<Light>();
            l.name = "CourtLight"; l.type = LightType.Directional; l.transform.rotation = Quaternion.Euler(52, -25, 0); l.shadows = LightShadows.Soft; l.shadowStrength = .55f; l.intensity = 1.25f; l.color = new Color(.98f, .98f, 1f);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(.58f, .63f, .66f);
            RenderSettings.skybox = null; view.clearFlags = CameraClearFlags.SolidColor;
        }
        void MarkLanding()
        {
            var p = shuttle.position; var v = velocity;
            for (int i = 0; i < 1000 && p.y > .04f; i++)
            {
                var before = p; v = StepVelocity(v, Time.fixedDeltaTime); p += v * Time.fixedDeltaTime;
                CollideNet(before, ref p, ref v);
            }
            landingShadow.position = new Vector3(p.x, .018f, p.z);
            landingShadow.gameObject.SetActive(flying);
            shuttle.GetComponent<TrailRenderer>().Clear();
        }
        public void Feed(string kind)
        {
            if (careerMatchId != null) return;
            SkipReplay(); replay.Clear();
            EnterTraining();
            shuttle.position = kind == "Net" ? new Vector3(0, 2, 1) : new Vector3(0, 3.4f, 4);
            velocity = kind == "Net" ? new Vector3(0, 2, -3.5f) : new Vector3(0, 7, -8);
            SpectacularMoment = false;
            flying = true; MarkLanding(); feedback = "Feed " + kind; lastFeed = Time.time;
        }
        public void BeginServe()
        {
            if (Time.timeScale == 0 || replaying || flying || (Match != null && (MatchState != "Serving" || !HumanServe))) return;
            servingCharge = true; serveChargedAt = Time.time;
        }
        public float ServePower => servingCharge ? Mathf.Clamp(Time.time - serveChargedAt, .2f, 1f) : 0;
        public void CancelServe() { servingCharge = false; serveFinger = -1; }
        public void ReleaseServe()
        {
            if (!servingCharge) return;
            float hold = ServePower;
            servingCharge = false;
            ServeWithPower(hold);
        }
        public void Serve(bool flick)
        {
            ServeWithPower(flick ? 1f : .2f);
        }
        public void ServeWithPower(float power)
        {
            if (!float.IsFinite(power) || power < 0 || power > 1) throw new System.ArgumentOutOfRangeException(nameof(power));
            if (Time.timeScale == 0 || replaying || flying || (Match != null && (MatchState != "Serving" || !HumanServe))) return;
            lastHitter = 0; lastPlayer = 0;
            if (flowStage == "Warmup" || flowStage == "GameEnd" || flowStage == "Interval" || flowStage == "ChangeEnds") return;
            flowStage = null;
            if (Match != null) { MatchState = "Rally"; serviceFlight = true; }
            shuttle.position = player.position + EndPoint(new Vector3(.35f, 1, .45f));
            float serveSide = Match == null ? (player.position.x >= 0 ? 1 : -1) : (Match.Points[0] % 2 == 0 ? 1 : -1);
            // power: 0 (short serve) to 1 (flick/deep serve)
            float targetZ = Mathf.Lerp(2.2f, 5.8f, power);
            float flightArc = Mathf.Lerp(1.1f, 1.6f, power);
            velocity = LaunchVelocity(shuttle.position, EndPoint(new Vector3(-serveSide * 1.3f, .04f, targetZ)), flightArc);
            PlaySound("racket", power < .35f ? "Net" : "Clear");
            flying = true;
            MarkLanding();
            feedback = power < .35f ? "Short serve" : power < .7f ? "Drive serve" : "Flick serve";
            contact = shuttle.position; swingAt = Time.time; lastStroke = "Serve";
            playerVisual.Present("Serve", 0, contact, Vector3.zero, jumpY, landingWeight);
        }
        public void SetAnalog(Vector2 value)
        {
            if (!float.IsFinite(value.x) || !float.IsFinite(value.y)) throw new System.ArgumentOutOfRangeException(nameof(value));
            analog = Vector2.ClampMagnitude(value, 1);
            lastAim = analog;
        }
        public void BeginShot(string kind)
        {
            if (System.Array.IndexOf(shots, kind) < 0) throw new System.ArgumentException("Invalid stroke");
            if (Time.timeScale == 0 || replaying || (Match != null && MatchState != "Rally")) return;
            stroke = kind; shotAim = analog; charging = true; chargedAt = Time.time;
            if (BootDiagnostics.Current.inputLogging) BootDiagnostics.Log("input", "charge " + kind);
        }
        public void TapShot(string kind)
        {
            if (Time.timeScale == 0 || replaying || (Match != null && MatchState != "Rally")) return;
            int index = System.Array.IndexOf(shots, kind);
            shotIntent.Queue(index, analog, .5f, Time.time);
            TryBufferedShot();
        }
        public float LastTiming { get; private set; }
        public float LastContact { get; private set; }
        public float LastGradeScore { get; private set; }
        public bool ReleaseShot()
        {
            if (Time.timeScale == 0 || replaying || !charging) return false;
            charging = false;
            if (Match != null && (MatchState != "Rally" || lastHitter == 0 || !CanReceive(0))) return false;
            var local = shuttle.position - player.position;
            bool overhead = stroke == "Smash" || stroke == "Clear" || stroke == "Drop";
            float reach = new Vector2(local.x, local.z).magnitude;
            string failure = playerVisual.ReachFailure(shuttle.position, stroke);
            if (!flying || failure.Length != 0)
            {
                feedback = "Poor | miss: " + (!flying ? "Kok tidak aktif" : failure);
                BootDiagnostics.Log("hit", feedback + " reach=" + reach.ToString("F2") + " height=" + local.y.ToString("F2")); return false;
            }

            // ponytail: local incoming-flight closest approach, capped at .20s; upgrade to stroke-specific measured windows after rally telemetry.
            var incoming = new Vector2(velocity.x, velocity.z);
            float closestAt = incoming.sqrMagnitude > .0001f ? -Vector2.Dot(new Vector2(local.x, local.z), incoming) / incoming.sqrMagnitude : 0;
            LastTiming = Mathf.Clamp01(1 - Mathf.Abs(closestAt) / .20f);
            float heightQuality = Mathf.Clamp01(1 - Mathf.Abs(local.y - (overhead ? 2.1f : 1)) / (overhead ? .7f : .9f));
            LastContact = Mathf.Min(Mathf.Clamp01((1.15f - reach) / .55f), heightQuality);
            LastGradeScore = 100 * (.5f * LastTiming + .5f * LastContact);

            float power = Mathf.Clamp01((Time.time - chargedAt) / .8f);
            var target = EndPoint(CourtPoint(shotAim, power));
            float near = stroke == "Net" ? 1f : stroke == "Drop" ? 1.6f : stroke == "Smash" ? 3.2f : 5.2f;
            float far = stroke == "Net" ? 2f : stroke == "Drop" ? 2.8f : stroke == "Smash" ? 5.6f : 6.1f;
            target.z = Mathf.Lerp(near, far, Mathf.Clamp01(.5f + shotAim.y * .35f + (power - .5f) * .3f)) * EndSign;
            // ponytail: fixed stroke envelopes, not clearance search/aim correction; refine from natural rally QA.
            float distance = Vector2.Distance(new Vector2(shuttle.position.x, shuttle.position.z), new Vector2(target.x, target.z));
            float height = shuttle.position.y;
            float duration = stroke == "Smash" ? .42f + 2.4f / Mathf.Max(1, distance)
                : stroke == "Net" ? .82f + distance * .04f
                : stroke == "Drop" ? 1f + distance * .04f
                : stroke == "Lift" ? 1.6f + distance * .03f : 1.45f + distance * .04f;
            duration += Mathf.Max(0, 1.8f - height) * .18f;
            duration = Mathf.Clamp(duration * Mathf.Lerp(1.08f, .92f, power), .2f, 3);

            target.y = .04f;
            velocity = LaunchVelocity(shuttle.position, target, duration);
            Energy = Mathf.Max(0, Energy - (stroke == "Smash" ? BootDiagnostics.Current.smashCost : BootDiagnostics.Current.shotCost));
            lastHitter = 0; lastPlayer = 0; serviceFlight = false;
            MarkLanding();
            contact = shuttle.position; swingAt = Time.time; lastStroke = stroke;
            playerVisual.Present(stroke, 0, contact, Vector3.zero, jumpY, landingWeight);
            string grade = LastGradeScore >= 85 ? "Excellent" : LastGradeScore >= 65 ? "Good" : LastGradeScore >= 40 ? "Fair" : "Poor";
            SpectacularMoment |= stroke == "Smash" && jumpY > .1f && (grade == "Excellent" || grade == "Good");
            feedback = grade + " | " + stroke; PlaySound("racket", stroke);
            BootDiagnostics.Log("hit", feedback + " power=" + power.ToString("F2") + " analog=" + analog + " contact=" + contact);
            return true;
        }
        bool ShotReachable => flying && (Match == null || MatchState == "Rally" && lastHitter != 0 && CanReceive(0))
            && playerVisual.CanReach(shuttle.position, shots[shotIntent.Kind]);
        void TryBufferedShot()
        {
            if (!shotIntent.Pending) return;
            if (Time.time > shotIntent.ExpiresAt)
            {
                string failure = playerVisual.ReachFailure(shuttle.position, shots[shotIntent.Kind]);
                feedback = "Poor | miss: " + (failure.Length == 0 ? "Kontak tidak terjadi" : failure);
                shotIntent.Clear(); return;
            }
            if (!shotIntent.TryTake(Time.time, ShotReachable, out int kind)) return;
            stroke = shots[kind]; shotAim = shotIntent.Aim; charging = true;
            chargedAt = Time.time - shotIntent.Power * .8f;
            ReleaseShot();
        }
        public void Jump()
        {
            if (!GameplayInputEnabled || replaying || (Match != null && (MatchState != "Rally" || flowStage != null))) return;
            if (Time.time - jumpAt >= .9f && jumpY <= .001f && Energy >= BootDiagnostics.Current.jumpCost)
            { jumpAt = Time.time; jumpPhase = 0; Energy -= BootDiagnostics.Current.jumpCost; }
        }
        void Update()
        {
            uiScale = CanvasScale(Screen.safeArea.size);
            if (moveFinger == -1) stickCenter = DefaultStickCenter;
            UpdateArena(); UpdateCourtAudio();
            if (!replaying && Input.GetKeyDown(KeyCode.Escape)) HandleBack();
            bool wasReplaying = replaying;
            ReadInput();
            if (replaying) { ReplayTick(); return; }
            if (wasReplaying) return;
            if (!GameplayInputEnabled) return;
            ContinuePoint(Time.deltaTime);
            if (replaying) return;
            UpdateDebug();
            // Root height belongs to fixed simulation; rendering samples its current pose.
            AnimateAthlete();
            AnimateOpponent();
            AnimateDoubles();
            if (Match != null && (MatchState == "Point" || MatchState == "MatchEnd"))
            {
                float weight = Match.Winner >= 0 ? 1 : Mathf.SmoothStep(0, 1, pointWait / .25f) * Mathf.SmoothStep(0, 1, (3 - pointWait) / .35f);
                int winner = Match.Winner >= 0 ? Match.Winner : Match.Server;
                for (int id = 0; id < (Match.Doubles ? 4 : 2); id++) Visual(id).PresentReaction(id % 2 == winner, weight);
            }
            if (ShowMatchResult)
            {
                if (careerMatchId != null && !careerResultSaved && string.IsNullOrEmpty(menuNotice)) RecordCareerResult();
                var centre = Actor(careerMatchId != null ? 0 : Match.Winner).position;
                Vector3 span = Vector3.zero;
                if (Match.Doubles) { span = Actor(Match.Winner + 2).position - centre; centre += span * .5f; }
                view.fieldOfView = 42;
                float tangent = Mathf.Tan(view.fieldOfView * Mathf.Deg2Rad * .5f);
                float distance = Mathf.Max(4.8f, (Mathf.Abs(span.x) + 1.2f) / (tangent * view.aspect)) + Mathf.Abs(span.z) * .5f;
                float end = centre.z < 0 ? 1 : -1;
                Vector3 focus = centre + Vector3.up * 1.02f - Vector3.right * end * (.4f * distance * tangent * view.aspect);
                view.transform.position = focus + new Vector3(0, .35f, -end * distance);
                view.transform.LookAt(focus); return;
            }
            ApplyGameplayCamera(false);
        }
        [System.Serializable] struct ReplayFrame
        {
            public Vector3 player, opponent, shuttle, pContact, oContact, pStep, oStep;
            public Quaternion pRotation, oRotation;
            public ExtraPose partner, second;
            public AthleteVisual.FootPose pFeet, oFeet;
            public string pStroke, oStroke;
            public float pElapsed, oElapsed, pJump, pLand, pTravel, oTravel;
        }
        readonly System.Collections.Generic.List<ReplayFrame> replay = new System.Collections.Generic.List<ReplayFrame>();
        bool replaying, pointReplayed;
        float replayAt, replayGameAt;
        double replayGameAtDouble;
        ReplayFrame liveFrame;
        Vector3 liveCameraPosition;
        Quaternion liveCameraRotation;
        public int ReplayMode { get; set; } = 1;
        public bool SpectacularMoment { get; private set; }
        public bool BeginReplay()
        {
            if (ReplayMode == 2 || replay.Count < 2 || replaying) return false;
            ClearGameplayInput();
            StopCourtAudio();
            if (MatchState == "Point" || MatchState == "MatchEnd") pointReplayed = true;
            liveFrame = CaptureFrame();
            liveCameraPosition = view.transform.position; liveCameraRotation = view.transform.rotation;
            replaying = true; replayAt = Time.unscaledTime; replayGameAt = Time.time; replayGameAtDouble = Time.timeAsDouble; return true;
        }
        public void SkipReplay()
        {
            if (!replaying) return;
            ApplyFrame(liveFrame);
            view.transform.SetPositionAndRotation(liveCameraPosition, liveCameraRotation);
            float elapsed = Time.time - replayGameAt;
            chargedAt += elapsed; jumpAt += elapsed; aiContactAt += elapsed; serveReadyAt += elapsed; swingAt += elapsed;
            observedSwing += elapsed; opponentThinkAt += elapsed; opponentRefreshAt += elapsed;
            partnerAt += elapsed; secondAt += elapsed; teamObserved += elapsed;
            double teamElapsed = Time.timeAsDouble - replayGameAtDouble;
            for (int i = 0; i < 4; i++) { teamThink[i] += teamElapsed; teamRefresh[i] += teamElapsed; }
            replaying = false;
            if (closingReplay) { closingReplay = false; NextPoint(); }
        }
        void ReplayTick()
        {
            float elapsed = Time.unscaledTime - replayAt;
            float passDuration = replay.Count * Time.fixedDeltaTime * 2;
            if (ReplayMode == 0)
            {
                view.transform.position = new Vector3(8, 4, 0);
                view.transform.LookAt(new Vector3(0, 1, 0));
                if (elapsed >= passDuration && !closingReplay)
                {
                    view.transform.SetPositionAndRotation(liveCameraPosition, liveCameraRotation);
                    elapsed -= passDuration;
                }
            }
            int index = Mathf.FloorToInt(elapsed / Time.fixedDeltaTime * .5f);
            if (index >= replay.Count) { SkipReplay(); return; }
            ApplyFrame(replay[index]);
        }
        float aiContactAt = -10, serveReadyAt;

        bool BodyContact(Vector3 from, Vector3 to, Transform athlete)
        {
            Vector3 delta = to - from;
            if (delta.sqrMagnitude < .000001f) return false;
            // ponytail: swept torso/head proxy follows original placeholder dimensions; replace with rig colliders when models change.
            var body = new Bounds(athlete.position + Vector3.up, new Vector3(.42f, 1.6f, .3f));
            return body.IntersectRay(new Ray(from, delta.normalized), out float distance) && distance <= delta.magnitude;
        }
        static bool CollideNet(Vector3 before, ref Vector3 after, ref Vector3 v)
        {
            if (before.z * after.z > 0 || before.z == after.z) return false;
            var crossing = Vector3.Lerp(before, after, -before.z / (after.z - before.z));
            float top = NetTop(crossing.x);
            if (Mathf.Abs(crossing.x) > BootDiagnostics.Current.doublesHalfWidth || crossing.y >= top || crossing.y < top - NetDepth) return false;
            after = crossing + Vector3.forward * (before.z < 0 ? -.025f : .025f);
            v = new Vector3(v.x * .15f, -1, -v.z * .08f);
            return true;
        }
        static bool NetTouchesBounds(Bounds bounds)
        {
            float width = BootDiagnostics.Current.doublesHalfWidth;
            if (bounds.min.x > width || bounds.max.x < -width || bounds.min.z > .01f || bounds.max.z < -.01f) return false;
            float left = Mathf.Max(bounds.min.x, -width), right = Mathf.Min(bounds.max.x, width);
            float top = Mathf.Max(NetTop(left), NetTop(right)), bottom = NetTop(Mathf.Clamp(0, left, right)) - NetDepth;
            return bounds.min.y <= top && bounds.max.y >= bottom;
        }
        static bool NetTouchesSegment(Vector3 a, Vector3 b, float radius)
        {
            var delta = b - a; float lo = 0, hi = 1;
            for (int axis = 0; axis <= 2; axis += 2)
            {
                float extent = axis == 0 ? BootDiagnostics.Current.doublesHalfWidth : .01f;
                extent += radius;
                if (Mathf.Abs(delta[axis]) < .000001f) { if (Mathf.Abs(a[axis]) > extent) return false; continue; }
                float enter = (-extent - a[axis]) / delta[axis], exit = (extent - a[axis]) / delta[axis];
                lo = Mathf.Max(lo, Mathf.Min(enter, exit)); hi = Mathf.Min(hi, Mathf.Max(enter, exit));
                if (lo > hi) return false;
            }
            float Height(float t) { var p = a + delta * t; return p.y - NetTop(p.x); }
            float minimum = Mathf.Min(Height(lo), Height(hi)), maximum = Mathf.Max(Height(lo), Height(hi));
            float sag = .026f / (BootDiagnostics.Current.doublesHalfWidth * BootDiagnostics.Current.doublesHalfWidth);
            if (Mathf.Abs(delta.x) > .000001f)
            {
                float peak = Mathf.Clamp((delta.y - 2 * sag * a.x * delta.x) / (2 * sag * delta.x * delta.x), lo, hi);
                maximum = Mathf.Max(maximum, Height(peak));
            }
            return minimum <= radius && maximum >= -NetDepth - radius;
        }
        bool TouchesNet(Transform athlete, Transform heldRacket)
        {
            var body = new Bounds(athlete.position + Vector3.up, new Vector3(.42f, 1.6f, .3f));
            if (NetTouchesBounds(body)) return true;
            if (heldRacket == null) return false;
            foreach (var r in heldRacket.GetComponentsInChildren<Renderer>())
            {
                if (!r.enabled) continue;
                if (r is not LineRenderer line) { if (NetTouchesBounds(r.bounds)) return true; continue; }
                // Renderer bounds include culling padding; intersect actual string/frame segments with sagged net.
                int count = line.positionCount;
                for (int i = 0; i < (line.loop ? count : count - 1); i++)
                {
                    Vector3 a = line.GetPosition(i), b = line.GetPosition((i + 1) % count);
                    if (!line.useWorldSpace) { a = line.transform.TransformPoint(a); b = line.transform.TransformPoint(b); }
                    if (NetTouchesSegment(a, b, Mathf.Max(line.startWidth, line.endWidth) * .5f)) return true;
                }
            }
            return false;
        }
        void ContactFault(int athlete, PointCause cause)
        {
            string reason = cause == PointCause.NetTouch ? "net touch" : cause == PointCause.BodyTouch ? "body touch" : "shuttle below net";
            pointCause = cause;
            flying = charging = serviceFlight = reviewAvailable = false;
            velocity = Vector3.zero; ScorePoint(1 - athlete);
            feedback = "Fault | " + reason;
            BootDiagnostics.Log("scoring", "fault " + reason + " player=" + athlete); PlaySound("whistle");
        }
        void ResetAthleteMovement()
        {
            System.Array.Clear(athleteVelocity, 0, athleteVelocity.Length);
            jumpY = landingWeight = 0; jumpPhase = 1; jumpAt = diveRecoveryUntil = brakeUntil = Time.time - 10;
            player.Find("ContactShadow").localPosition = new Vector3(0, .004f, 0);
        }
        void MoveAthlete(int id, Vector3 desired, float cap)
        {
            // ponytail: fixed-step kinematic momentum; upgrade only if measured collisions need a body solver.
            desired = Vector3.ClampMagnitude(new Vector3(desired.x, 0, desired.z), cap);
            var current = athleteVelocity[id];
            bool braking = desired.sqrMagnitude < .0001f || Vector3.Dot(current, desired) < 0;
            float response = id == 0 && jumpY > .001f ? 1 : braking ? 16.666667f : 10;
            current = Vector3.MoveTowards(current, desired, cap * response * Time.fixedDeltaTime);
            current = Vector3.ClampMagnitude(current, cap);
            var root = Actor(id); var next = root.position + current * Time.fixedDeltaTime;
            float side = id % 2 == 0 ? -EndSign : EndSign;
            float x = Mathf.Clamp(next.x, -3.4f, 3.4f), z = Mathf.Clamp(next.z * side, .25f, 7.1f) * side;
            if (x != next.x) current.x = 0;
            if (z != next.z) current.z = 0;
            if (id == 0 && jumpY <= .01f && athleteVelocity[id].magnitude > .5f && current.sqrMagnitude < athleteVelocity[id].sqrMagnitude - .01f)
                brakeUntil = Time.time + .2f;
            next.x = x; next.z = z; root.position = next; athleteVelocity[id] = current;
        }
        void FixedUpdate()
        {
            if (replaying || !GameplayInputEnabled) return;
            float oldHeight = jumpY;
            jumpPhase += Time.fixedDeltaTime;
            jumpY = jumpPhase < .7f ? Mathf.Max(0, 3.5f * jumpPhase - 5 * jumpPhase * jumpPhase) : 0;
            landingWeight = oldHeight > 0 && jumpY == 0 ? 1 : Mathf.MoveTowards(landingWeight, 0, Time.fixedDeltaTime * 4);
            var jumpPosition = player.position; jumpPosition.y = jumpY; player.position = jumpPosition;
            player.Find("ContactShadow").localPosition = new Vector3(0, .004f - jumpY, 0);
            if (Match != null && MatchState == "Rally")
            {
                if (TouchesNet(player, racket)) { ContactFault(0, PointCause.NetTouch); return; }
                if (TouchesNet(opponent, opponentVisual.StringBed)) { ContactFault(1, PointCause.NetTouch); return; }
                if (Match.Doubles && TouchesNet(partner, partnerVisual.StringBed)) { ContactFault(0, PointCause.NetTouch); return; }
                if (Match.Doubles && TouchesNet(opponentPartner, opponentPartnerVisual.StringBed)) { ContactFault(1, PointCause.NetTouch); return; }
            }
            // Rally-time energy drain per second (D-043: ~30s lowers stamina sufficiently; walking drain=0)
            if (flying && Match != null && MatchState == "Rally")
                Energy = Mathf.Max(0f, Energy - BootDiagnostics.Current.rallyDrain * Time.fixedDeltaTime);
            if (flying && Match != null && MatchState == "Rally")
            {
                OpponentEnergy = Mathf.Max(0, OpponentEnergy - BootDiagnostics.Current.rallyDrain * Time.fixedDeltaTime);
                DrainExtraAthletes(BootDiagnostics.Current.rallyDrain * Time.fixedDeltaTime);
            }
            UpdateOpponent();

            Vector2 move = assisted ? Vector2.zero : analog * EndSign;
            if (assisted && flying && shuttle.position.z * EndSign < 1 && (Match == null || lastHitter != 0))
            {
                var offset = new Vector2(shuttle.position.x - player.position.x, shuttle.position.z - player.position.z);
                move = Vector2.ClampMagnitude(offset * 2, 1);
            }
            if (Time.time < diveRecoveryUntil) move = Vector2.zero;
            float fatigueSpeed = Mathf.Lerp(.7f, 1, Mathf.Clamp01(Energy / EnergyCapacity / .75f));
            float speed = (charging ? 2.8f : 4.5f) * fatigueSpeed;
            MoveAthlete(0, new Vector3(move.x, 0, move.y) * speed, speed);
            if (!flying) return;
            TryBufferedShot();
            replay.Add(CaptureFrame());
            if (replay.Count > 1500) replay.RemoveAt(0);
            var before = shuttle.position; velocity = StepVelocity(velocity, Time.fixedDeltaTime);
            var after = before + velocity * Time.fixedDeltaTime;
            if (Match != null && MatchState == "Rally")
            {
                if (BodyContact(before, after, player)) { ContactFault(0, PointCause.BodyTouch); return; }
                if (BodyContact(before, after, opponent)) { ContactFault(1, PointCause.BodyTouch); return; }
                if (Match.Doubles && BodyContact(before, after, partner)) { ContactFault(0, PointCause.BodyTouch); return; }
                if (Match.Doubles && BodyContact(before, after, opponentPartner)) { ContactFault(1, PointCause.BodyTouch); return; }
            }
            if (Match != null && MatchState == "Rally" && before.z * after.z <= 0 && before.z != after.z)
            {
                var crossing = Vector3.Lerp(before, after, -before.z / (after.z - before.z));
                if (Mathf.Abs(crossing.x) <= BootDiagnostics.Current.doublesHalfWidth && crossing.y < NetTop(crossing.x) - NetDepth)
                { shuttle.position = crossing; ContactFault(lastHitter, PointCause.BelowNet); return; }
            }
            if (CollideNet(before, ref after, ref velocity)) { PlaySound("net"); feedback = "Net | contact below tape"; }
            if (after.y <= .04f) { after.y = .04f; flying = false; velocity = Vector3.zero; feedback += LandingInside(after) ? " | IN" : " | OUT"; BootDiagnostics.Log("collision", "ground " + after + " " + feedback);
                if (Match != null && MatchState == "Rally")
                {
                    beforePoint = JsonUtility.ToJson(Match);
                    lastLanding = after; announcedIn = serviceFlight ? LandingInside(after) : AnnounceLanding(after); reviewAvailable = true;
                    bool legalService = LegalServiceLanding(after);
                    int winner = serviceFlight ? (legalService ? lastHitter : 1 - lastHitter) : announcedIn ? (after.z * EndSign > 0 ? 0 : 1) : 1 - lastHitter;
                    if (serviceFlight && !legalService) { feedback = "Fault | service area"; reviewAvailable = false; }
                    pointCause = serviceFlight && !legalService ? PointCause.ServiceArea : announcedIn ? PointCause.In : PointCause.Out;
                    serviceFlight = false;
                    ScorePoint(winner); BootDiagnostics.Log("scoring", "point winner=" + winner); PlaySound("score");
                    shuttle.position = after;
                }
            }
            shuttle.position = after;
            if (velocity.sqrMagnitude > .01f) shuttle.rotation = Quaternion.FromToRotation(Vector3.down, velocity.normalized);
            shadow.position = new Vector3(after.x, .012f, after.z);
            float h = Mathf.Clamp(after.y, 0, 6f);
            shadow.localScale = Vector3.Lerp(new Vector3(.14f, .005f, .14f), new Vector3(.06f, .005f, .06f), h / 6f);
            landingShadow.gameObject.SetActive(flying);
        }
        Rect StickRect => new Rect(stickCenter.x - 95, stickCenter.y - 95, 190, 190);
        Rect JumpRect => ControlRect(6, new Vector2(UiWidth, UiHeight));
        Rect ShotRect(int i) => ControlRect(i + 1, new Vector2(UiWidth, UiHeight));
        static readonly string[] shots = { "Smash", "Clear", "Drop", "Net", "Lift" };
        Rect ServeRect => ControlRect(7, new Vector2(UiWidth, UiHeight));
        void PointerDown(int id, Vector2 point)
        {
            RecordTouchDown(id, point);
            if (UiPointerDown(id, point) || !GameplayInputEnabled || testerOpen) return;
            bool insideStick = (point - stickCenter).sqrMagnitude <= StickRadius * StickRadius;
            bool floatingZone = JoystickMode == "Floating" && point.x < UiWidth * .48f && point.y > 160 && !ServeRect.Contains(point);
            if ((insideStick || floatingZone) && moveFinger == -1)
            {
                moveFinger = id;
                if (JoystickMode == "Fixed")
                    SetAnalog(ShapeJoystickDelta(point - stickCenter, JoystickSensitivity));
                else
                {
                    stickCenter = point;
                    SetAnalog(Vector2.zero);
                }
            }
            else if (JumpRect.Contains(point) && jumpFinger == -1) { jumpFinger = id; jumpHeld = true; jumpStart = point; jumpDrag = point; Jump(); }
            else if (ServeRect.Contains(point) && serveFinger == -1 && !flying && (Match == null || MatchState == "Serving")) { serveFinger = id; BeginServe(); }
            else for (int i = 0; i < shots.Length; i++) if (ShotRect(i).Contains(point)) { if (shotFinger == -1) { shotFinger = id; pressedShot = i; TapShot(shots[i]); } break; }
        }
        void PointerMove(int id, Vector2 point)
        {
            menuPoint = point;
            RecordTouchMove(id, point);
            if (!GameplayInputEnabled) return;
            if (id == moveFinger)
                SetAnalog(ShapeJoystickDelta(point - stickCenter, JoystickSensitivity));
            if (id == jumpFinger) { jumpDrag = point; for (int i = 0; i < shots.Length; i++) if (ShotRect(i).Contains(point) && (!charging || stroke != shots[i])) BeginShot(shots[i]); }
        }
        void PointerUp(int id)
        {
            RecordTouchUp(id);
            if (UiPointerUp(id)) return;
            if (!GameplayInputEnabled)
            {
                if (id == moveFinger) { moveFinger = -1; SetAnalog(Vector2.zero); stickCenter = DefaultStickCenter; }
                if (id == shotFinger) shotFinger = pressedShot = -1;
                if (id == jumpFinger) { charging = false; jumpFinger = -1; jumpHeld = false; }
                if (id == serveFinger) CancelServe();
                return;
            }
            if (id == moveFinger) { moveFinger = -1; SetAnalog(Vector2.zero); stickCenter = DefaultStickCenter; }
            if (id == shotFinger) { ReleaseShot(); shotFinger = pressedShot = -1; }
            if (id == jumpFinger) { if (charging) ReleaseShot(); jumpFinger = -1; jumpHeld = false; }
            if (id == serveFinger) { ReleaseServe(); serveFinger = -1; }
        }
        void PointerCancel(int id)
        {
            RecordTouchUp(id);
            if (id == menuFinger) { menuFinger = -1; menuAction = null; }
            if (id == shotFinger || id == jumpFinger) { shotIntent.Clear(); charging = false; }
            if (id == moveFinger) { moveFinger = -1; SetAnalog(Vector2.zero); stickCenter = DefaultStickCenter; }
            if (id == shotFinger) shotFinger = pressedShot = -1;
            if (id == jumpFinger) { jumpFinger = -1; jumpHeld = false; }
            if (id == serveFinger) CancelServe();
        }
        void ReadInput()
        {
            if (testerOpen) return;
            bool wasReplaying = replaying;
            foreach (var t in Input.touches)
            {
                var point = (new Vector2(t.position.x, Screen.height - t.position.y) - UiOrigin) / uiScale;
                if (t.phase == TouchPhase.Began) PointerDown(t.fingerId, point);
                else if (t.phase == TouchPhase.Canceled) PointerCancel(t.fingerId);
                else if (t.phase == TouchPhase.Ended) { if (t.fingerId == menuFinger) menuPoint = point; PointerUp(t.fingerId); }
                else PointerMove(t.fingerId, point);
                if (wasReplaying && !replaying) return;
            }
            if (Input.touchCount == 0)
            {
                var point = (new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y) - UiOrigin) / uiScale;
                if (Input.GetMouseButtonDown(0)) PointerDown(-2, point);
                if (Input.GetMouseButton(0)) PointerMove(-2, point);
                if (Input.GetMouseButtonUp(0)) { PointerMove(-2, point); PointerUp(-2); }
                if (wasReplaying || !GameplayInputEnabled) return;
                var keysMove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                if (moveFinger == -1 && (keysMove.sqrMagnitude > 0 || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))) SetAnalog(keysMove);
            }
            if (!GameplayInputEnabled) return;
            if (Input.GetKeyDown(KeyCode.Space)) Jump();
            if (Input.GetKeyDown(KeyCode.L)) BeginServe();
            if (Input.GetKeyUp(KeyCode.L)) ReleaseServe();
            if (Input.GetKeyDown(KeyCode.K)) Serve(true);
            var keys = new[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5 };
            for (int i = 0; i < keys.Length; i++) if (Input.GetKeyDown(keys[i])) TapShot(shots[i]);
            if (Input.GetKeyDown(KeyCode.F)) Feed("Lob");
        }
        public static Color GradeColor(string text)
        {
            if (text.StartsWith("Excellent", System.StringComparison.Ordinal)) return new Color(.75f, .45f, 1);
            if (text.StartsWith("Good", System.StringComparison.Ordinal)) return new Color(.35f, 1, .5f);
            if (text.StartsWith("Fair", System.StringComparison.Ordinal)) return new Color(1, .85f, .25f);
            if (text.StartsWith("Poor", System.StringComparison.Ordinal)) return new Color(1, .35f, .3f);
            return Color.white;
        }
        public Rect ReplayToolbarRect => new Rect(24, 342, 430, 56);
        public float ControlFontSize => 26;
        public void RunMenuAction(string action)
        {
            menuNotice = null;
            try
            {
                switch (action)
                {
                    case "Save": SaveMatch(); if (careerMatchId == null || !ShowMatchResult || careerResultSaved) menuNotice = "Pertandingan disimpan."; break;
                    case "Load": LoadMatch(); menuNotice = "Pertandingan dimuat."; break;
                    case "Replay": BeginReplay(); break;
                    case "Skip": SkipReplay(); break;
                    default: throw new System.ArgumentException("Unknown menu action");
                }
            }
            catch (System.IO.FileNotFoundException e) { feedback = "File error: " + e.Message; menuNotice = "Berkas simpan tidak ditemukan."; }
            catch (System.IO.IOException e) { feedback = "File error: " + e.Message; menuNotice = "Berkas tidak dapat diakses. Coba lagi."; }
            catch (System.UnauthorizedAccessException e) { feedback = "File access denied: " + e.Message; menuNotice = "Akses berkas ditolak. Periksa izin penyimpanan."; }
            catch (System.ArgumentException e) { feedback = "Invalid save: " + e.Message; menuNotice = action == "Load" ? "Data simpan tidak valid. Pertandingan tidak diubah." : "Aksi tidak valid."; }
            catch (System.InvalidOperationException e) { feedback = e.Message; menuNotice = action == "Save" && Match == null ? "Belum ada pertandingan untuk disimpan." : "Aksi tidak dapat dijalankan."; }
        }
        bool creditsOpen;
        string CreditsText => "JUST SMASH - exhibition build\n\nCharacter: NPC Casual Set 00 (c) Chepatack\nUnity Asset Store EULA - embedded distribution only, no redistribution\nRacket and room: Joseph SARDIN / BigSoundBank (CC0)\nApplause: Dorian CLAIR / BigSoundBank (CC0)\nOther SFX, court, athletes, shuttle: original project source\nEngine: Unity 6000.3\n\nOffline: no network, no analytics, no data collection.\nSaves stay on this device.\nDistribution: personal only - do not redistribute this APK.\n\nDisplay font: Anton / Vernon Adams.\n" + Resources.Load<TextAsset>("Fonts/Anton-OFL").text;
        void DrawLegacyUi()
        {
            uiScale = CanvasScale(Screen.safeArea.size);
            GUI.matrix = Matrix4x4.TRS(UiOrigin, Quaternion.identity, Vector3.one * uiScale);
            float width = UiWidth;
            GUI.skin.label.fontSize = (int)ControlFontSize; GUI.skin.button.fontSize = (int)ControlFontSize; GUI.skin.box.fontSize = (int)ControlFontSize;
            GUI.skin.toggle.fontSize = GUI.skin.textField.fontSize = (int)ControlFontSize;
            if (testerOpen) { DrawTester(); return; }
            GUI.Box(new Rect(24, 24, 580, 120), Match == null ? "JUST SMASH | TRAINING" : "JUST SMASH | SINGLES " + Match.Points[0] + " - " + Match.Points[1] + " | Games " + Match.Games[0] + " - " + Match.Games[1]);
            GUI.contentColor = GradeColor(feedback);
            GUI.Label(new Rect(40, 55, 550, 28), feedback);
            GUI.contentColor = Color.white;
            // Sendiri: kontrol sentuh untuk badminton single
            string controlHint = servingCharge ? "Serve Power: " + ServePower.ToString("P0") : charging ? "Charge " + Mathf.Clamp01((Time.time - chargedAt) / .8f).ToString("P0") : "Joystick kiri | Ketuk tombol pukulan kanan | Tahan SERVIS";
            GUI.Label(new Rect(40, 86, 550, 28), controlHint);
            if (GUI.Button(new Rect(620, 24, 170, 48), Match == null ? "Start singles" : "Restart match")) BeginExhibition();
            if (Match != null && (FlowStage == "Warmup" || FlowStage == "Ready" || FlowStage == "GameEnd" || FlowStage == "Interval" || FlowStage == "ChangeEnds") && GUI.Button(new Rect(620, 82, 350, 48), FlowStage + " | Continue")) AdvanceFlow();
            if (Match != null && flowStage == null && MatchState == "Point" && GUI.Button(new Rect(620, 82, 170, 48), "Next point")) NextPoint();
            // Feed lob/net hanya untuk mode training (Match == null); disembunyikan di pertandingan.
            if (Match == null)
            {
                if (GUI.Button(new Rect(24, 160, 130, 48), "Feed lob")) Feed("Lob");
                if (GUI.Button(new Rect(164, 160, 130, 48), "Feed net")) Feed("Net");
            }
            if (Match == null && GUI.Button(new Rect(304, 160, 190, 48), assisted ? "Assisted ON" : "Assisted OFF")) assisted = !assisted;
            GUI.Box(ServeRect, servingCharge ? "Charging " + ServePower.ToString("P0") : "HOLD SERVE");
            if (GUI.Button(new Rect(184, 220, 110, 52), "Flick")) Serve(true);
            if (Time.timeScale == 0 && GUI.Button(new Rect(width - 390, 82, 366, 56), "Settings | Joystick: " + JoystickMode)) ConfigureJoystick(JoystickMode == "Fixed" ? "Floating" : "Fixed");
            if (GUI.Button(new Rect(width - 130, 24, 106, 48), Time.timeScale == 0 ? "Resume" : "Pause")) Time.timeScale = Time.timeScale == 0 ? 1 : 0;
            if (Match != null)
            {
                if (GUI.Button(new Rect(620, 146, 110, 56), "Save")) RunMenuAction("Save");
                if (GUI.Button(new Rect(740, 146, 110, 56), "Load")) RunMenuAction("Load");
                if (reviewAvailable && GUI.Button(new Rect(870, 146, 170, 56), "Challenge " + Match.Challenges[0])) ChallengeCall();
                GUI.Label(new Rect(620, 210, 240, 36), "Energy " + (Energy / EnergyCapacity).ToString("P0"));
            }
            if (Match != null && (flowStage != null || Match.Winner >= 0)) GUI.Box(new Rect(24, 390, Mathf.Min(width - 48, 1000), 80), FlowStage + "\n" + MatchSummary);
            if (GUI.Button(new Rect(24, 342, 170, 56), replaying ? "Skip replay" : "Replay")) RunMenuAction(replaying ? "Skip" : "Replay");
            if (GUI.Button(new Rect(204, 342, 250, 56), ReplayMode == 2 ? "Replay OFF" : ReplayMode == 1 ? "Gameplay replay" : "Replay + angle")) ReplayMode = (ReplayMode + 1) % 3;
            if (GUI.Button(new Rect(470, 342, 130, 56), creditsOpen ? "Close" : "Credits")) creditsOpen = !creditsOpen;
            if (creditsOpen)
            {
                var savedFont = GUI.skin.label.fontSize; GUI.skin.label.fontSize = 18;
                GUI.Box(new Rect(24, 410, 940, 290), CreditsText);
                GUI.skin.label.fontSize = savedFont;
            }
            GUI.Box(StickRect, "MOVE / AIM");
            var dot = StickRect.center + analog * new Vector2(65, -65); GUI.Box(new Rect(dot.x - 18, dot.y - 18, 36, 36), "+");
            for (int i = 0; i < shots.Length; i++) GUI.Box(ShotRect(i), shots[i]);
            GUI.Box(JumpRect, "Jump");
            // Panel tester hanya untuk development build; tidak tampil di APK rilis ke teman.
            if (Application.isEditor || Debug.isDebugBuild) DrawTester();
            if (DebugVisible) GUI.Label(new Rect(24, 690, width - 48, 30), ProfileSummary);
            if (jumpHeld)
            {
                GUI.Label(new Rect(width - 430, 270, 400, 36), "Hold + drag to technique; release to strike");
                for (int i = 0; i < shots.Length; i++) DrawGuide(JumpRect.center, ShotRect(i).center);
            }
        }
        void DrawGuide(Vector2 from, Vector2 to)
        {
            var saved = GUI.matrix; var d = to - from;
            GUIUtility.RotateAroundPivot(Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg, from);
            GUI.color = new Color(.1f, .9f, .9f); GUI.DrawTexture(new Rect(from.x, from.y, d.magnitude, 2), Texture2D.whiteTexture);
            GUI.matrix = saved; GUI.color = Color.white;
        }
        void OnDestroy() { Time.timeScale = 1; DestroySounds(); ReleaseArena(); ReleaseUi(); }
    }
}