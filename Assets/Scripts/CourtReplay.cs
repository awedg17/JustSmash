using System;
using UnityEngine;

namespace JustSmash
{
    public sealed partial class CourtPrototype
    {
        ReplayFrame CaptureFrame() => new ReplayFrame {
            player = player.position, opponent = opponent.position, shuttle = shuttle.position,
            pRotation = player.rotation, oRotation = opponent.rotation,
            pContact = contact, oContact = opponentContact, pStroke = lastStroke, oStroke = opponentStroke,
            pElapsed = Mathf.Max(0, Time.time - swingAt), oElapsed = Mathf.Max(0, Time.time - aiContactAt),
            pJump = jumpY, pLand = landingWeight,
            pStep = player.position - previousPlayer, oStep = opponent.position - previousOpponent,
            pTravel = playerVisual.Travel, oTravel = opponentVisual.Travel,
            pFeet = playerVisual.CaptureFeet(), oFeet = opponentVisual.CaptureFeet(),
            partner = CaptureExtra(2), second = CaptureExtra(3)
        };
        void ApplyFrame(ReplayFrame f)
        {
            player.position = f.player; opponent.position = f.opponent; shuttle.position = f.shuttle;
            if (f.partner.present) { ApplyExtra(2, f.partner); ApplyExtra(3, f.second); }
            if (string.IsNullOrEmpty(f.pStroke)) return; // v1 positional replay.
            player.rotation = f.pRotation; opponent.rotation = f.oRotation;
            playerVisual.SetTravel(Mathf.Max(0, f.pTravel - f.pStep.magnitude));
            opponentVisual.SetTravel(Mathf.Max(0, f.oTravel - f.oStep.magnitude));
            playerVisual.RestorePose(f.pStroke, f.pElapsed, f.pContact, f.pStep, f.pJump, f.pLand, f.pFeet);
            opponentVisual.RestorePose(f.oStroke, f.oElapsed, f.oContact, f.oStep, 0, 0, f.oFeet);
            previousPlayer = player.position; previousOpponent = opponent.position;
        }
        static bool PoseStroke(string value) => value == "Serve" || Array.IndexOf(new[] { "Smash", "Clear", "Drop", "Net", "Lift" }, value) >= 0;
        static bool UnitVector(Vector2 v) => float.IsFinite(v.x) && float.IsFinite(v.y) && v.sqrMagnitude <= 1.0001f;
        static bool Bounded(Vector3 v) => float.IsFinite(v.x) && float.IsFinite(v.y) && float.IsFinite(v.z) && v.sqrMagnitude <= 1000000;
        static void ValidatePose(ReplayFrame f)
        {
            AthleteVisual.ValidateFeet(f.pFeet); AthleteVisual.ValidateFeet(f.oFeet);
            if (f.partner.present || f.second.present) { ValidateExtra(f.partner); ValidateExtra(f.second); }
            if (string.IsNullOrEmpty(f.pStroke)) return;
            if (!PoseStroke(f.pStroke) || !PoseStroke(f.oStroke)) throw new ArgumentException("Invalid replay stroke");
            foreach (Vector3 v in new[] { f.pContact, f.oContact, f.pStep, f.oStep })
                if (!Bounded(v)) throw new ArgumentException("Invalid replay pose vector");
            foreach (float n in new[] { f.pElapsed, f.oElapsed, f.pJump, f.pLand, f.pTravel, f.oTravel })
                if (!float.IsFinite(n) || n < 0 || n > 1000000) throw new ArgumentException("Invalid replay pose timer");
            foreach (Quaternion q in new[] { f.pRotation, f.oRotation })
            {
                float norm = q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;
                if (!float.IsFinite(norm) || Mathf.Abs(norm - 1) > .01f) throw new ArgumentException("Invalid replay rotation");
            }
            if (f.pJump > .7f || f.pLand > 1) throw new ArgumentException("Invalid replay jump");
        }
        void ValidateMobileSnapshot(Snapshot s)
        {
            if (s.version == 1) return;
            OpponentProfile.For(s.difficulty);
            if (s.opponentSeed == 0 || s.opponentRandom == 0 || !Bounded(s.opponentTarget) || !UnitVector(s.lastAim) || !UnitVector(s.shotAim) || !UnitVector(s.intentAim)
                || s.intentKind < 0 || s.intentKind >= shots.Length || !float.IsFinite(s.intentPower) || s.intentPower < 0 || s.intentPower > 1
                || !float.IsFinite(s.intentRemaining) || s.intentRemaining < 0 || s.intentRemaining > ShotIntent.Window
                || !float.IsFinite(s.opponentEnergy) || s.opponentEnergy < 0 || s.opponentEnergy > 100 * (1 + (s.staminaStat - 70) * .01f) + .001f
                || !PoseStroke(s.pStroke) || !PoseStroke(s.oStroke)) throw new ArgumentException("Invalid mobile snapshot");
            foreach (float timer in new[] { s.observationAge, s.reactionRemaining, s.refreshRemaining, s.swingElapsed })
                if (!float.IsFinite(timer) || timer < 0 || timer > 1000000) throw new ArgumentException("Invalid opponent timer");
            if (s.reactionRemaining > 1 || s.refreshRemaining > 1 || s.intentPending && s.state != "Rally") throw new ArgumentException("Contradictory mobile snapshot");
            ValidatePose(s.livePose);
            if (s.frames != null) foreach (var f in s.frames) ValidatePose(f);
        }
        void RestoreMobileSnapshot(Snapshot s)
        {
            shotIntent.Clear();
            if (s.version == 1)
            {
                Difficulty = "Easy"; opponentRandom = opponentSeed = 193; OpponentEnergy = EnergyCapacity;
                lastAim = shotAim = Vector2.zero; swingAt = aiContactAt = Time.time - 10;
                ResetOpponent(); lastStroke = "Clear"; opponentStroke = "Clear";
                playerVisual.Present("Clear", 10, Vector3.zero, Vector3.zero, jumpY, 0);
                opponentVisual.Present("Clear", 10, Vector3.zero, Vector3.zero, 0, 0);
                return;
            }
            Difficulty = s.difficulty; opponentSeed = s.opponentSeed; opponentRandom = s.opponentRandom;
            OpponentEnergy = s.opponentEnergy; opponentTarget = s.opponentTarget;
            observedSwing = Time.time - s.observationAge; opponentThinkAt = Time.time + s.reactionRemaining; opponentRefreshAt = Time.time + s.refreshRemaining;
            lastAim = s.lastAim; shotAim = s.shotAim; SimulateLineErrors = s.simulateLineErrors;
            swingAt = Time.time - s.swingElapsed; lastStroke = s.pStroke; opponentStroke = s.oStroke;
            contact = s.livePose.pContact; opponentContact = s.livePose.oContact;
            if (s.intentPending)
            {
                shotIntent.Queue(s.intentKind, s.intentAim, s.intentPower, Mathf.Max(0, Time.time - ShotIntent.Window + s.intentRemaining));
            }
            ApplyFrame(s.livePose);
        }
    }
}
