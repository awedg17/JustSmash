using UnityEngine;
namespace JustSmash
{
    public sealed partial class CourtPrototype
    {
        public string AnimationState { get; private set; } = "Idle";
        Vector3 previousPlayer, motion;
        Vector3 previousOpponent, opponentContact;
        float opponentGait;
        void PoseRacketArm(Transform athlete, Vector3 contactPoint, float elapsed)
        {
            var shoulder = athlete.Find("Hips/Spine/Chest/ShoulderRight");
            var elbow = shoulder.Find("ElbowRight");
            var ready = athlete.TransformPoint(new Vector3(.38f, 1.25f, .38f));
            // ponytail: immediate tap has no windup; contact starts now, then follow-through and recovery. Authored clips can replace this without delaying gameplay.
            Vector3 target = ready;
            if (elapsed >= 0 && elapsed < .65f)
            {
                var follow = contactPoint + athlete.TransformDirection(new Vector3(-.22f, -.18f, .18f));
                target = elapsed < .18f ? Vector3.Lerp(contactPoint, follow, Mathf.SmoothStep(0, 1, elapsed / .18f))
                    : Vector3.Lerp(follow, ready, Mathf.SmoothStep(0, 1, (elapsed - .18f) / .47f));
            }
            const float upper = .32f, lower = .62f; // forearm .30m + grip-to-string-centre .32m
            var delta = target - shoulder.position;
            var axis = delta.sqrMagnitude > .000001f ? delta.normalized : athlete.forward;
            float distance = Mathf.Clamp(delta.magnitude, lower - upper + .001f, upper + lower - .001f);
            target = shoulder.position + axis * distance;
            var pole = athlete.TransformDirection(new Vector3(1, -.3f, -.7f));
            var bend = Vector3.ProjectOnPlane(pole, axis).normalized;
            if (bend.sqrMagnitude < .01f) bend = Vector3.ProjectOnPlane(athlete.up, axis).normalized;
            float along = (upper * upper - lower * lower + distance * distance) / (2 * distance);
            var joint = shoulder.position + axis * along + bend * Mathf.Sqrt(Mathf.Max(0, upper * upper - along * along));
            shoulder.rotation = Quaternion.FromToRotation(Vector3.down, (joint - shoulder.position).normalized);
            elbow.rotation = Quaternion.FromToRotation(Vector3.down, (target - elbow.position).normalized);
        }
        void PlantLegs(Transform athlete, float phase, float pace, bool lunge)
        {
            var hips = athlete.Find("Hips");
            for (int i = 0; i < 2; i++)
            {
                string side = i == 0 ? "Left" : "Right";
                var hip = hips.Find("Leg" + side);
                var knee = hip.Find("Knee" + side);
                var foot = knee.Find("Foot" + side);
                float cycle = phase + i * Mathf.PI;
                float forward = lunge ? (i == 0 ? .5f : -.4f) : Mathf.Cos(cycle) * .22f * pace;
                float lift = lunge ? 0 : Mathf.Max(0, Mathf.Sin(cycle)) * .11f * pace;
                var target = athlete.TransformPoint(new Vector3(i == 0 ? -.16f : .16f, .08f + lift, forward));
                var delta = target - hip.position;
                float distance = Mathf.Clamp(delta.magnitude, .001f, .899f);
                var axis = delta.normalized;
                var bend = Vector3.ProjectOnPlane(athlete.forward, axis).normalized;
                var joint = hip.position + axis * distance * .5f + bend * Mathf.Sqrt(Mathf.Max(0, .45f * .45f - distance * distance * .25f));
                hip.rotation = Quaternion.FromToRotation(Vector3.down, (joint - hip.position).normalized);
                knee.rotation = Quaternion.FromToRotation(Vector3.down, (target - knee.position).normalized);
                foot.rotation = athlete.rotation;
            }
        }
        void AnimateOpponent()
        {
            float speed = Mathf.Min(4.5f, Vector3.Distance(opponent.position, previousOpponent) / Mathf.Max(Time.deltaTime, .001f));
            var actualStep = opponent.position - previousOpponent; actualStep.y = 0;
            previousOpponent = opponent.position;
            opponentVisual.Present(opponentStroke, Time.time - aiContactAt, opponentContact, actualStep, 0, 0);
            PlayFootstep(1, opponentVisual);
            opponentGait += speed * Time.deltaTime * 9;
            var oHips = opponent.Find("Hips");
            var spine = oHips.Find("Spine"); var chest = spine != null ? spine.Find("Chest") : null;
            var rShoulder = chest != null ? chest.Find("ShoulderRight") : null;
            float oSwing = Time.time - aiContactAt;
            if (chest != null) chest.localRotation = Quaternion.Euler(oSwing < .3f ? (opponentContact.y < 1.45f ? 20 : -18) : 0, oSwing < .3f ? -30 * Mathf.Sin(oSwing / .3f * Mathf.PI) : 0, 0);
            PlantLegs(opponent, opponentGait, Mathf.Clamp01(speed / 4.5f), false);
            PoseRacketArm(opponent, opponentContact, oSwing);
        }
        float brakeUntil, gait, landingWeight;
        void AnimateAthlete()
        {
            var step = player.position - previousPlayer; step.y = 0; previousPlayer = player.position;
            playerVisual.Present(lastStroke, Time.time - swingAt, contact, step, jumpY, landingWeight);
            var speed = step / Mathf.Max(Time.deltaTime, .001f);

            motion = speed;
            float swing = Time.time - swingAt;
            bool low = lastStroke == "Net" || lastStroke == "Lift";
            bool backhand = player.InverseTransformPoint(contact).x < 0;
            AnimationState = Time.time < diveRecoveryUntil ? "DiveSave" : swing < .3f ? (lastStroke == "Net" ? "Lunge" : low ? "Underhand" : backhand ? "Backhand" : "Overhead") : swing < .65f ? "Recovery" : jumpY > .02f ? "Jump" : Time.time < brakeUntil ? "Braking" : speed.magnitude < .1f ? "Idle" : Mathf.Abs(speed.x) > Mathf.Abs(speed.z) ? "Side" : speed.z > 0 ? "Forward" : "Backward";
            gait += speed.magnitude * Time.deltaTime * 3;
            PlayFootstep(0, playerVisual);
            float stride = Mathf.Sin(gait * 3) * Mathf.Clamp01(speed.magnitude / 4.5f) * 28;
            bool dive = AnimationState == "DiveSave";
            bool lunge = AnimationState == "Lunge" || dive;
            var hips = player.Find("Hips");
            if (hips == null) return; // jointed rig only
            var spine = hips.Find("Spine"); var chest = spine != null ? spine.Find("Chest") : null;
            var left = hips.Find("LegLeft"); var right = hips.Find("LegRight");
            var lKnee = left != null ? left.Find("KneeLeft") : null;
            var rKnee = right != null ? right.Find("KneeRight") : null;
            var rShoulder = chest != null ? chest.Find("ShoulderRight") : null;
            var rElbow = rShoulder != null ? rShoulder.Find("ElbowRight") : null;
            var lShoulder = chest != null ? chest.Find("ShoulderLeft") : null;
            var lElbow = lShoulder != null ? lShoulder.Find("ElbowLeft") : null;

            // ---- LEGS: lunge = deep front-knee bend + rear extension; run = alternating thigh swing ----
            float drop = dive ? .42f : lunge ? .2f : AnimationState == "Braking" ? .05f : 0;
            hips.localPosition = new Vector3(0, .92f - drop + (AnimationState == "Idle" ? Mathf.Sin(Time.time * 2) * .008f : 0), dive ? .25f : lunge ? .12f : 0);

            // ---- TORSO lean + rotational twist drives the racket arm ----
            float lean = dive ? 48 : AnimationState == "Lunge" ? 24 : AnimationState == "Overhead" ? -18 : AnimationState == "Braking" ? -12 : speed.z * 2;
            float twist = swing < .3f ? (backhand ? 30 : -30) * Mathf.Sin(swing / .3f * Mathf.PI) : (dive ? (backhand ? 25 : -25) : 0);
            if (spine != null) spine.localRotation = Quaternion.Lerp(spine.localRotation, Quaternion.Euler(lean * .45f, twist * .4f, 0), Time.deltaTime * 18);
            if (chest != null) chest.localRotation = Quaternion.Lerp(chest.localRotation, Quaternion.Euler(lean * .55f, twist * .6f, -speed.x * 2), Time.deltaTime * 18);

            PlantLegs(player, gait * 3, Mathf.Clamp01(speed.magnitude / 4.5f), lunge);
            PoseRacketArm(player, contact, swing);

            // non-racket arm counterbalances during the swing
            if (lShoulder != null) lShoulder.localRotation = Quaternion.Euler(swing < .3f ? -50f : 20f, 0, swing < .3f ? 40f : 12f);
            if (lElbow != null) lElbow.localRotation = Quaternion.Euler(swing < .3f ? 60f : 35f, 0, 0);

        // legacy visual helpers removed; 'racket' is now a child of the jointed right hand and follows it directly
            // ponytail: procedural joint angles on primitive mannequin; replace with licensed FBX clips after feel acceptance. Root/contact rules unchanged.
        }
    }
}
