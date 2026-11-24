using System;
using UnityEngine;

namespace JustSmash
{
    public sealed partial class CourtPrototype
    {
        uint opponentSeed = 193, opponentRandom = 193;
        float observedSwing = -100, opponentThinkAt, opponentRefreshAt;
        Vector3 opponentTarget;
        string opponentStroke = "Serve";
        public float OpponentEnergy { get; private set; } = 100;
        public bool SimulateLineErrors { get; set; }
        public void ConfigureOpponent(string difficulty, int seed)
        {
            OpponentProfile.For(difficulty);
            if (seed < 1) throw new ArgumentOutOfRangeException(nameof(seed));
            Difficulty = difficulty; opponentSeed = opponentRandom = (uint)seed;
            observedSwing = -100; opponentRefreshAt = 0;
        }
        float OpponentNoise()
        {
            opponentRandom ^= opponentRandom << 13; opponentRandom ^= opponentRandom >> 17; opponentRandom ^= opponentRandom << 5;
            return opponentRandom / 4294967296f * 2 - 1;
        }
        void ResetOpponent()
        {
            observedSwing = -100; opponentRefreshAt = 0;
            opponentTarget = opponent.position;
        }
        void ObserveFlight(OpponentProfile profile)
        {
            // ponytail: bounded flight integration, no input/target peeking; retune horizon only from real rally evidence.
            Vector3 point = shuttle.position, v = velocity, destination = point;
            for (int i = 0; i < 65 && point.y > .04f; i++)
            {
                var before = point; v = StepVelocity(v, Time.fixedDeltaTime); point += v * Time.fixedDeltaTime;
                CollideNet(before, ref point, ref v);
                if (point.z * EndSign < .35f) continue;
                destination = point;
                float distance = Vector2.Distance(new Vector2(opponent.position.x, opponent.position.z), new Vector2(point.x, point.z));
                if (v.y < 0 && point.y < 2.2f && point.y > .6f && distance < OpponentProfile.MovementSpeed * (i + 1) * Time.fixedDeltaTime + .5f) break;
            }
            opponentTarget = destination - opponent.right * .30f - opponent.forward * .35f;
            opponentTarget.x = Mathf.Clamp(opponentTarget.x + OpponentNoise() * profile.AimError * .35f, -3.4f, 3.4f);
            opponentTarget.z = Mathf.Clamp(opponentTarget.z * EndSign, .35f, 7.1f) * EndSign;
            opponentTarget.y = 0;
        }
        void UpdateOpponent()
        {
            if (Match == null || flowStage != null) return;
            if (Match.Doubles) { UpdateDoubles(); return; }
            if (MatchState == "Serving" && Match.Server == 1)
            {
                if (Time.time < serveReadyAt) return;
                shuttle.position = opponent.position + opponent.TransformDirection(new Vector3(.35f, 1, .45f));
                float side = Match.Points[1] % 2 == 0 ? 1 : -1;
                velocity = LaunchVelocity(shuttle.position, EndPoint(new Vector3(side * 1.3f, .04f, -4.5f)), 1.5f);
                serviceFlight = flying = true; lastHitter = 1; MatchState = "Rally";
                aiContactAt = Time.time; opponentContact = shuttle.position; opponentStroke = "Serve";
                opponentVisual.Present(opponentStroke, 0, opponentContact, Vector3.zero, 0, 0);
                MarkLanding(); PlaySound("racket", "Serve");
                return;
            }
            if (MatchState != "Rally" || !flying) return;
            if (lastHitter != 0)
            {
                var home = EndPoint(new Vector3(0, 0, 3.8f));
                MoveAthlete(1, (home - opponent.position) / Time.fixedDeltaTime, OpponentProfile.MovementSpeed * AthleteFatigueSpeed(1));
                return;
            }
            var profile = OpponentProfile.For(Difficulty);
            if (observedSwing != swingAt)
            {
                observedSwing = swingAt; opponentThinkAt = Time.time + profile.Reaction;
                opponentRefreshAt = opponentThinkAt;
            }
            if (Time.time < opponentThinkAt) { MoveAthlete(1, Vector3.zero, OpponentProfile.MovementSpeed * AthleteFatigueSpeed(1)); return; }
            if (Time.time >= opponentRefreshAt)
            {
                ObserveFlight(profile); opponentRefreshAt = Time.time + profile.Refresh;
            }
            float fatigue = Mathf.Lerp(.7f, 1, Mathf.Clamp01(OpponentEnergy / EnergyCapacity / .75f));
            MoveAthlete(1, (opponentTarget - opponent.position) / Time.fixedDeltaTime, OpponentProfile.MovementSpeed * fatigue);
            if (shuttle.position.z * EndSign < .15f || velocity.y >= 0 || Time.time - aiContactAt < .3f) return;
            float height = shuttle.position.y;
            string shot = height >= 1.45f ? "Clear" : "Lift";
            if (!opponentVisual.CanReach(shuttle.position, shot)) return;
            // ponytail: one forecourt open-space choice; expand only from observed rally gaps.
            if (Difficulty != "Easy" && height >= 1 && height < 1.45f && opponent.position.z * EndSign < 2 && player.position.z * EndSign < -4) shot = "Net";
            else if (Difficulty == "Hard" && height > 1.9f && opponent.position.z * EndSign < 4.8f) shot = "Smash";
            else if (Difficulty != "Easy" && height > 1.45f && OpponentNoise() > .25f) shot = "Drop";
            float x = Difficulty == "Easy" ? -player.position.x * .45f : player.position.x >= 0 ? -1.65f : 1.65f;
            float z = shot == "Net" ? 1.2f : shot == "Drop" ? 1.5f : shot == "Smash" ? 3.4f : 5.2f;
            Vector3 target = new Vector3(Mathf.Clamp(x + OpponentNoise() * profile.AimError, -2.35f, 2.35f), .04f, -z * EndSign);
            float distance = Vector2.Distance(new Vector2(shuttle.position.x, shuttle.position.z), new Vector2(target.x, target.z));
            float duration = shot == "Net" ? .82f + .04f * distance + Mathf.Max(0, 1.8f - height) * .18f : shot == "Smash" ? .8f : shot == "Drop" ? 1.15f : 1.65f;
            velocity = LaunchVelocity(shuttle.position, target, duration);
            OpponentEnergy = Mathf.Max(0, OpponentEnergy - (shot == "Smash" ? BootDiagnostics.Current.smashCost : BootDiagnostics.Current.shotCost));
            BootDiagnostics.Log("AI", "return " + Difficulty + " " + shot);
            lastHitter = 1; serviceFlight = false; aiContactAt = Time.time; opponentContact = shuttle.position; opponentStroke = shot;
            opponentVisual.Present(opponentStroke, 0, opponentContact, Vector3.zero, 0, 0);
            MarkLanding(); PlaySound("racket", shot);
        }
    }
}
