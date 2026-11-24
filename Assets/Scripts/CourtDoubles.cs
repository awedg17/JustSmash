using System;
using UnityEngine;

namespace JustSmash
{
    public sealed partial class CourtPrototype
    {
        [Serializable] struct ExtraPose
        {
            public bool present;
            public Vector3 position, contact, step;
            public Quaternion rotation;
            public string stroke;
            public float elapsed, travel;
            public AthleteVisual.FootPose feet;
        }
        [Serializable] sealed class DoublesState
        {
            public int lastPlayer, nearOwner, farOwner;
            public string formation;
            public Vector3[] targets;
            public float[] think, refresh;
            public float observed;
            public float[] extraEnergy;
        }
        ExtraPose CaptureExtra(int id)
        {
            if (!DoublesMode || partner == null) return default;
            var root = Actor(id);
            return new ExtraPose { present = true, position = root.position, rotation = root.rotation,
                contact = id == 2 ? partnerContact : secondContact, stroke = id == 2 ? partnerStroke : secondStroke,
                elapsed = Mathf.Max(0, Time.time - (id == 2 ? partnerAt : secondAt)),
                feet = Visual(id).CaptureFeet(), travel = Visual(id).Travel, step = root.position - (id == 2 ? previousPartner : previousOpponentPartner) };
        }
        static void ValidateExtra(ExtraPose p)
        {
            AthleteVisual.ValidateFeet(p.feet);
            float norm = p.rotation.x * p.rotation.x + p.rotation.y * p.rotation.y + p.rotation.z * p.rotation.z + p.rotation.w * p.rotation.w;
            if (!p.present || !Bounded(p.position) || !Bounded(p.contact) || !Bounded(p.step) || !PoseStroke(p.stroke)
                || !float.IsFinite(p.elapsed) || p.elapsed < 0 || p.elapsed > 1000000
                || !float.IsFinite(p.travel) || p.travel < 0 || p.travel > 1000000 || !float.IsFinite(norm) || Mathf.Abs(norm - 1) > .01f)
                throw new ArgumentException("Invalid doubles replay pose");
        }
        void ApplyExtra(int id, ExtraPose p)
        {
            Actor(id).SetPositionAndRotation(p.position, p.rotation);
            Visual(id).SetTravel(Mathf.Max(0, p.travel - p.step.magnitude));
            Visual(id).RestorePose(p.stroke, p.elapsed, p.contact, p.step, 0, 0, p.feet);
            if (id == 2) previousPartner = p.position; else previousOpponentPartner = p.position;
        }
        DoublesState CaptureDoubles()
        {
            if (!DoublesMode) return null;
            var state = new DoublesState { lastPlayer = lastPlayer, nearOwner = nearOwner, farOwner = farOwner, formation = Formation,
                targets = (Vector3[])teamTargets.Clone(), think = new float[4], refresh = new float[4], observed = Time.time - teamObserved,
                extraEnergy = new[] { partnerEnergy, secondEnergy } };
            double now = replaying ? replayGameAtDouble : Time.timeAsDouble;
            for (int i = 0; i < 4; i++) { state.think[i] = (float)Math.Clamp(teamThink[i] - now, 0, 1); state.refresh[i] = (float)Math.Clamp(teamRefresh[i] - now, 0, 1); }
            return state;
        }
        static void ValidateDoubles(Snapshot s)
        {
            // JsonUtility materializes empty reference fields: singles has no extra poses to restore.
            if (!s.match.Doubles)
            {
                if (s.livePose.partner.present || s.livePose.second.present) throw new ArgumentException("Unexpected doubles pose");
                return;
            }
            var d = s.doubles;
            if (s.match.RightPlayers == null || s.match.RightPlayers.Length != 2 || (s.match.RightPlayers[0] != 0 && s.match.RightPlayers[0] != 2)
                || (s.match.RightPlayers[1] != 1 && s.match.RightPlayers[1] != 3) || d == null || d.lastPlayer < 0 || d.lastPlayer > 3
                || d.lastPlayer % 2 != s.hitter || (d.nearOwner != 0 && d.nearOwner != 2) || (d.farOwner != 1 && d.farOwner != 3)
                || d.targets == null || d.targets.Length != 4 || d.think == null || d.think.Length != 4 || d.refresh == null || d.refresh.Length != 4
                || (d.formation != "Front-Back" && d.formation != "Side-by-Side" && d.formation != "Free Rotation")
                || !float.IsFinite(d.observed) || d.observed < 0 || d.observed > 1000000) throw new ArgumentException("Invalid doubles snapshot");
            foreach (var target in d.targets) if (!Bounded(target)) throw new ArgumentException("Invalid doubles target");
            foreach (var timers in new[] { d.think, d.refresh }) foreach (float time in timers)
                if (!float.IsFinite(time) || time < 0 || time > 1) throw new ArgumentException("Invalid doubles timer");
            if (d.extraEnergy != null && (d.extraEnergy.Length != 2 || Array.Exists(d.extraEnergy,
                n => !float.IsFinite(n) || n < 0 || n > 100 * (1 + (s.staminaStat - 70) * .01f) + .001f)))
                throw new ArgumentException("Invalid doubles energy");
            ValidateExtra(s.livePose.partner); ValidateExtra(s.livePose.second);
        }
        void RestoreDoubles(Snapshot s)
        {
            doublesSelected = Match.Doubles; SetExtraAthletes(Match.Doubles);
            if (!Match.Doubles) return;
            var d = s.doubles;
            // ponytail: pre-stamina doubles snapshots migrate missing extra energy to starting capacity.
            partnerEnergy = d.extraEnergy == null ? EnergyCapacity : d.extraEnergy[0];
            secondEnergy = d.extraEnergy == null ? EnergyCapacity : d.extraEnergy[1];
            lastPlayer = d.lastPlayer; nearOwner = d.nearOwner; farOwner = d.farOwner; Formation = d.formation;
            Array.Copy(d.targets, teamTargets, 4); teamObserved = Time.time - d.observed;
            for (int i = 0; i < 4; i++) { teamThink[i] = Time.timeAsDouble + d.think[i]; teamRefresh[i] = Time.timeAsDouble + d.refresh[i]; }
            partnerContact = s.livePose.partner.contact; secondContact = s.livePose.second.contact;
            partnerStroke = s.livePose.partner.stroke; secondStroke = s.livePose.second.stroke;
            partnerAt = Time.time - s.livePose.partner.elapsed; secondAt = Time.time - s.livePose.second.elapsed;
        }
        bool doublesSelected;
        Transform partner, opponentPartner, suggestedPosition;
        public Vector3 SuggestedPosition => FormationTarget(0);
        AthleteVisual partnerVisual, opponentPartnerVisual;
        Vector3 previousPartner, previousOpponentPartner, partnerContact, secondContact;
        string partnerStroke = "Serve", secondStroke = "Serve";
        float partnerAt = -10, secondAt = -10;
        float partnerEnergy = 100, secondEnergy = 100;
        float AthleteEnergy(int id) => id == 0 ? Energy : id == 1 ? OpponentEnergy : id == 2 ? partnerEnergy : secondEnergy;
        float AthleteFatigueSpeed(int id) => Mathf.Lerp(.7f, 1, Mathf.Clamp01(AthleteEnergy(id) / EnergyCapacity / .75f));
        void DrainExtraAthletes(float cost)
        {
            if (!DoublesMode) return;
            partnerEnergy = Mathf.Max(0, partnerEnergy - cost); secondEnergy = Mathf.Max(0, secondEnergy - cost);
        }
        readonly Vector3[] teamTargets = new Vector3[4];
        readonly double[] teamThink = new double[4], teamRefresh = new double[4];
        float teamObserved = -100;
        int lastPlayer, nearOwner, farOwner = 1;
        public bool DoublesMode => Match != null ? Match.Doubles : doublesSelected;
        public string Formation { get; private set; } = "Side-by-Side";
        public string PartnerName => "Dimas Putra";
        public string SecondOpponentName => "Fajar Mahendra";
        Transform Actor(int id) => id == 0 ? player : id == 1 ? opponent : id == 2 ? partner : opponentPartner;
        AthleteVisual Visual(int id) => id == 0 ? playerVisual : id == 1 ? opponentVisual : id == 2 ? partnerVisual : opponentPartnerVisual;
        public void ConfigureDiscipline(bool doubles)
        {
            if (Match != null && Match.Winner < 0) throw new InvalidOperationException("Cannot change discipline during match");
            doublesSelected = doubles; SetExtraAthletes(doubles); StageMenu();
        }
        void SetExtraAthletes(bool active)
        {
            if (active && partner == null)
            {
                partner = new GameObject("Partner").transform; partner.position = new Vector3(-1.3f, 0, -4);
                opponentPartner = new GameObject("OpponentPartner").transform; opponentPartner.position = new Vector3(1.3f, 0, 4);
                opponentPartner.rotation = Quaternion.Euler(0, 180, 0);
                partnerVisual = AthleteVisual.Attach(partner); opponentPartnerVisual = AthleteVisual.Attach(opponentPartner);
                foreach (var root in new[] { partner, opponentPartner }) AddContactShadow(root);
            }
            if (active && suggestedPosition == null)
                suggestedPosition = Shape("DoublesSuggestedPosition", PrimitiveType.Sphere, Vector3.zero, new Vector3(.48f, .008f, .48f), new Color(.15f, .82f, .76f));
            if (suggestedPosition != null) suggestedPosition.gameObject.SetActive(active && GameplayInputEnabled);
            if (partner != null) { partner.gameObject.SetActive(active); opponentPartner.gameObject.SetActive(active); }
        }
        void PrepareDoubles()
        {
            SetExtraAthletes(Match.Doubles);
            if (!Match.Doubles) return;
            for (int id = 0; id < 4; id++)
            {
                int side = id % 2;
                bool right = Match.RightPlayers[side] == id;
                var root = Actor(id);
                root.rotation = Quaternion.Euler(0, (side == 0 ? EndSign < 0 : EndSign > 0) ? 180 : 0, 0);
                root.position = EndPoint(new Vector3((side == 0 ? 1 : -1) * (right ? 1.3f : -1.3f), 0, side == 0 ? -3.2f : 3.2f));
                teamTargets[id] = root.position; teamThink[id] = teamRefresh[id] = 0;
            }
            lastPlayer = Match.ServerPlayer; lastHitter = Match.Server; nearOwner = Match.Server == 0 ? Match.ServerPlayer : Match.ReceiverPlayer;
            farOwner = Match.Server == 1 ? Match.ServerPlayer : Match.ReceiverPlayer;
            var server = Actor(Match.ServerPlayer);
            shuttle.position = server.TransformPoint(new Vector3(.35f, 1, .45f));
            previousPlayer = player.position; previousOpponent = opponent.position;
            previousPartner = partner.position; previousOpponentPartner = opponentPartner.position;
            teamObserved = -100; Formation = "Side-by-Side";
        }
        bool HumanServe => Match == null || Match.ServerPlayer == 0;
        bool CanReceive(int id) => !DoublesMode || !serviceFlight || Match == null || Match.ReceiverPlayer == id;
        bool LandingInside(Vector3 point) => InCourt(point, DoublesMode);
        bool LegalServiceLanding(Vector3 point)
        {
            if (!LandingInside(point) || DoublesMode && Mathf.Abs(point.z) > DoublesServiceLine) return false;
            float sign = Match.Points[Match.Server] % 2 == 0 ? 1 : -1;
            return Match.Server == 0 ? point.z * EndSign >= ShortServiceLine && point.x * EndSign * sign <= CourtLineWidth * .5f
                : point.z * EndSign <= -ShortServiceLine && point.x * EndSign * sign >= -CourtLineWidth * .5f;
        }
        void AnimateDoubles()
        {
            if (!DoublesMode || partner == null) return;
            suggestedPosition.position = SuggestedPosition + Vector3.up * .018f;
            partnerVisual.Present(partnerStroke, Time.time - partnerAt, partnerContact, partner.position - previousPartner, 0, 0);
            opponentPartnerVisual.Present(secondStroke, Time.time - secondAt, secondContact, opponentPartner.position - previousOpponentPartner, 0, 0);
            PlayFootstep(2, partnerVisual); PlayFootstep(3, opponentPartnerVisual);
            previousPartner = partner.position; previousOpponentPartner = opponentPartner.position;
        }
        void BotServe(int id)
        {
            var root = Actor(id); int side = id % 2; float sign = Match.Points[side] % 2 == 0 ? 1 : -1;
            shuttle.position = root.TransformPoint(new Vector3(.35f, 1, .45f));
            velocity = LaunchVelocity(shuttle.position, EndPoint(new Vector3((side == 0 ? -1 : 1) * sign * 1.3f, .04f, side == 0 ? 2.6f : -2.6f)), 1.15f);
            lastPlayer = id; lastHitter = side; serviceFlight = flying = true; MatchState = "Rally";
            BotPose(id, "Serve"); MarkLanding(); PlaySound("racket", "Serve");
        }
        void BotPose(int id, string shot)
        {
            if (id == 1) { aiContactAt = Time.time; opponentStroke = shot; opponentContact = shuttle.position; }
            else if (id == 2) { partnerStroke = shot; partnerContact = shuttle.position; partnerAt = Time.time; }
            else { secondStroke = shot; secondContact = shuttle.position; secondAt = Time.time; }
            Visual(id).Present(shot, 0, shuttle.position, Vector3.zero, 0, 0);
        }
        Vector3 PredictTeamContact(int side)
        {
            Vector3 point = shuttle.position, v = velocity, best = point;
            for (int i = 0; i < 100 && point.y > .04f; i++)
            {
                var before = point; v = StepVelocity(v, Time.fixedDeltaTime); point += v * Time.fixedDeltaTime;
                CollideNet(before, ref point, ref v);
                if ((side == 0 ? -1 : 1) * point.z * EndSign < .25f) continue;
                best = point;
                if (v.y < 0 && point.y <= 1.8f) break;
            }
            return best;
        }
        Vector3 FormationTarget(int id)
        {
            int side = id % 2; float sign = side == 0 ? -1 : 1;
            int hitter = lastPlayer % 2 == side ? lastPlayer : (side == 0 ? nearOwner : farOwner);
            string shot = lastPlayer == 0 ? lastStroke : lastPlayer == 1 ? opponentStroke : lastPlayer == 2 ? partnerStroke : secondStroke;
            bool attack = lastHitter == side && (shot == "Smash" || shot == "Drop");
            bool transition = lastHitter == side && shot == "Net";
            if (side == 0) Formation = transition ? "Free Rotation" : attack ? "Front-Back" : "Side-by-Side";
            bool front = transition ? id == hitter : id != hitter;
            return EndPoint(attack || transition ? new Vector3(id == hitter ? .6f : -.6f, 0, sign * (front ? 1.8f : 4.8f))
                : new Vector3(id < 2 ? 1.55f : -1.55f, 0, sign * 3.6f));
        }
        void UpdateDoubles() => UpdateDoublesAt(Time.timeAsDouble);
        void UpdateDoublesAt(double now)
        {
            if (Match == null || !Match.Doubles || flowStage != null) return;
            if (MatchState == "Serving")
            {
                if (!HumanServe && Time.time >= serveReadyAt) BotServe(Match.ServerPlayer);
                return;
            }
            if (MatchState != "Rally" || !flying) return;
            var profile = OpponentProfile.For(Difficulty);
            float stamp = lastPlayer == 0 ? swingAt : lastPlayer == 2 ? partnerAt : lastPlayer == 3 ? secondAt : aiContactAt;
            int receiving = 1 - lastHitter;
            if (teamObserved != stamp)
            {
                teamObserved = stamp;
                for (int i = 1; i < 4; i++) { teamThink[i] = now + profile.Reaction; teamRefresh[i] = now; }
            }
            if (now >= teamRefresh[receiving == 0 ? 2 : 1])
            {
                Vector3 intercept = PredictTeamContact(receiving);
                int owner = serviceFlight ? Match.ReceiverPlayer : Vector3.SqrMagnitude(Actor(receiving).position - intercept) <= Vector3.SqrMagnitude(Actor(receiving + 2).position - intercept) ? receiving : receiving + 2;
                if (receiving == 0) nearOwner = owner; else farOwner = owner;
                teamTargets[owner] = intercept - Actor(owner).right * .30f - Actor(owner).forward * .35f;
                teamTargets[owner].y = 0;
                for (int i = 1; i < 4; i++) teamRefresh[i] = now + profile.Refresh;
            }
            for (int id = 1; id < 4; id++)
            {
                int side = id % 2; var root = Actor(id);
                int owner = side == 0 ? nearOwner : farOwner;
                Vector3 target = side == receiving && id == owner ? now >= teamThink[id] ? teamTargets[id] : root.position : FormationTarget(id);
                // Yield through the shared movement budget; Manual human never gets pushed.
                if (id == 2 || id != owner)
                {
                    var mate = Actor(id ^ 2); Vector3 away = root.position - mate.position; away.y = 0;
                    if ((target - mate.position).sqrMagnitude < .49f || away.sqrMagnitude < .49f && Vector3.Dot(target - root.position, away) <= 0)
                    {
                        if (away.sqrMagnitude < .001f) away = FormationTarget(id) - mate.position;
                        if (away.sqrMagnitude < .001f) away = root.right;
                        target = mate.position + away.normalized * .7f;
                        float end = EndSign * (side == 0 ? -1 : 1);
                        if (Mathf.Abs(target.x) > 3.35f || target.z * end < .4f || target.z * end > 7.1f)
                        {
                            // ponytail: four free-court escape lanes; add pathing only for measured route failures.
                            float best = float.NegativeInfinity;
                            for (int lane = 0; lane < 4; lane++)
                            {
                                var direction = lane == 0 ? Vector3.right : lane == 1 ? Vector3.left : lane == 2 ? Vector3.forward : Vector3.back;
                                var candidate = mate.position + direction * .7f;
                                float score = Vector3.Dot(direction, away);
                                if (Mathf.Abs(candidate.x) > 3.35f || candidate.z * end < .4f || candidate.z * end > 7.1f || score <= best) continue;
                                best = score; target = candidate;
                            }
                        }
                    }
                }
                target.x = Mathf.Clamp(target.x, -3.35f, 3.35f);
                target.z = Mathf.Clamp(target.z * EndSign * (side == 0 ? -1 : 1), .4f, 7.1f) * EndSign * (side == 0 ? -1 : 1);
                MoveAthlete(id, (target - root.position) / Time.fixedDeltaTime, OpponentProfile.MovementSpeed * AthleteFatigueSpeed(id));
                if (side != receiving || id != owner || now < teamThink[id] || velocity.y >= 0 || !CanReceive(id)) continue;
                if (shuttle.position.z * EndSign * (side == 0 ? -1 : 1) < .15f) continue;
                string shot = shuttle.position.y >= 1.45f ? "Clear" : "Lift";
                if (!Visual(id).CanReach(shuttle.position, shot)) continue;
                float choice = (OpponentNoise() + 1) * .5f;
                if (shuttle.position.y > 1.9f && choice < (Difficulty == "Hard" ? .65f : Difficulty == "Normal" ? .30f : .10f)) shot = "Smash";
                else if (shuttle.position.y >= 1.45f && choice < (Difficulty == "Easy" ? .35f : .75f)) shot = "Drop";
                else if (shuttle.position.y < 1.45f && Mathf.Abs(root.position.z) < 2.5f && choice < .4f) shot = "Net";
                float z = shot == "Net" ? 1.3f : shot == "Drop" ? 1.6f : shot == "Smash" ? 3.4f : 5.2f;
                float x = 0, gap = -1;
                // Visible defender positions only; difficulty changes decisions, never reach or speed.
                for (int lane = -1; lane <= 1; lane++)
                {
                    Vector3 candidate = EndPoint(new Vector3(lane * 2.45f, 0, (side == 0 ? 1 : -1) * z));
                    float space = Mathf.Min((Actor(1 - side).position - candidate).sqrMagnitude, (Actor(3 - side).position - candidate).sqrMagnitude);
                    if (space > gap) { gap = space; x = candidate.x * EndSign; }
                }
                x = Mathf.Clamp(x + OpponentNoise() * profile.AimError, -2.85f, 2.85f);
                velocity = LaunchVelocity(shuttle.position, EndPoint(new Vector3(x, .04f, (side == 0 ? 1 : -1) * z)), shot == "Smash" ? .6f : shot == "Drop" || shot == "Net" ? 1.2f : 1.65f);
                float cost = shot == "Smash" ? BootDiagnostics.Current.smashCost : BootDiagnostics.Current.shotCost;
                if (id == 1) OpponentEnergy = Mathf.Max(0, OpponentEnergy - cost);
                else if (id == 2) partnerEnergy = Mathf.Max(0, partnerEnergy - cost);
                else secondEnergy = Mathf.Max(0, secondEnergy - cost);
                lastPlayer = id; lastHitter = side; serviceFlight = false;
                BotPose(id, shot); MarkLanding(); PlaySound("racket", shot);
                BootDiagnostics.Log("AI", "doubles return player=" + id + " " + shot); break;
            }
        }
    }
}
