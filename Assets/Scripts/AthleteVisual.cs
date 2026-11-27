using System;
using System.Collections.Generic;
using UnityEngine;

namespace JustSmash
{
    // ponytail: native in-place keyframes + fixed-length IK; improve clip art without changing physics ownership.
    public sealed class AthleteVisual : MonoBehaviour
    {
        const string Hip = "Root/Hips", Chest = Hip + "/Spine/Spine1";
        const float RacketLength = .38f, GaitCycleMetres = 2f;
        Transform athlete, hips, chest, upper, lower, hand, leftUpper, leftLower, leftHand;
        readonly Transform[] thighs = new Transform[2], knees = new Transform[2], feet = new Transform[2];
        readonly Vector3[] plants = new Vector3[2];
        readonly Quaternion[] footRest = new Quaternion[2];
        readonly Dictionary<Transform, Quaternion> bind = new Dictionary<Transform, Quaternion>();
        Vector3 hipPosition, plantRoot;
        Quaternion plantRotation;
        Animation clips;
        Material racketMaterial;
        readonly List<Material> toonMaterials = new List<Material>();
        float armLength, travel;
        bool planted, wasAirborne;
        public int FootContacts { get; private set; }
        public int TakeFootContacts() { int contacts = FootContacts; FootContacts = 0; return contacts; }
        public Transform StringBed { get; private set; }
        public Transform Grip { get; private set; }
        public string CurrentClip { get; private set; } = "Ready";
        public float ClipTime { get; private set; }
        [Serializable] public struct FootPose
        {
            public bool recorded, planted, airborne;
            public Vector3 left, right;
        }
        FootPose restoredFeet;
        public FootPose CaptureFeet() => new FootPose { recorded = true, planted = planted, airborne = wasAirborne, left = plants[0], right = plants[1] };
        public static void ValidateFeet(FootPose pose)
        {
            foreach (var v in new[] { pose.left, pose.right })
                if (!float.IsFinite(v.x) || !float.IsFinite(v.y) || !float.IsFinite(v.z) || v.sqrMagnitude > 1000000)
                    throw new ArgumentException("Invalid replay foot plant");
        }
        public void RestorePose(string stroke, float elapsed, Vector3 contact, Vector3 step, float jump, float landing, FootPose pose)
        {
            ValidateFeet(pose); restoredFeet = pose;
            try { Present(stroke, elapsed, contact, step, jump, landing); }
            finally { restoredFeet = default; FootContacts = 0; }
        }
        public float Travel => travel;
        public void SetTravel(float value) { if (!float.IsFinite(value) || value < 0) throw new ArgumentOutOfRangeException(nameof(value)); travel = value; }
        public static AthleteVisual Attach(Transform root)
        {
            var prefab = Resources.Load<GameObject>("AthleteNPC");
            if (prefab == null) throw new InvalidOperationException("AthleteNPC missing");
            var model = Instantiate(prefab, root, false); model.name = "AthleteVisual";
            model.transform.localRotation = Quaternion.identity;
            foreach (var r in root.GetComponentsInChildren<MeshRenderer>()) if (!r.transform.IsChildOf(model.transform)) r.enabled = false;
            var driver = model.AddComponent<AthleteVisual>(); driver.Bind(root); return driver;
        }
        Transform Bone(string path) => transform.Find(path) ?? throw new InvalidOperationException("Athlete bone missing: " + path);
        void Bind(Transform root)
        {
            athlete = root;
            var animator = GetComponent<Animator>(); if (animator != null) animator.enabled = false;
            hips = Bone(Hip); chest = Bone(Chest); hipPosition = hips.localPosition;
            upper = Bone(Chest + "/RightShoulder/RightArm"); lower = upper.Find("RightForeArm"); hand = lower.Find("RightHand");
            leftUpper = Bone(Chest + "/LeftShoulder/LeftArm"); leftLower = leftUpper.Find("LeftForeArm"); leftHand = leftLower.Find("LeftHand");
            armLength = Vector3.Distance(upper.position, lower.position) + Vector3.Distance(lower.position, hand.position);
            for (int i = 0; i < 2; i++)
            {
                string side = i == 0 ? "Left" : "Right";
                thighs[i] = Bone(Hip + "/" + side + "UpLeg"); knees[i] = thighs[i].Find(side + "Leg"); feet[i] = knees[i].Find(side + "Foot");
                footRest[i] = Quaternion.Inverse(root.rotation) * feet[i].rotation;
            }
            foreach (var t in GetComponentsInChildren<Transform>(true)) bind[t] = t.localRotation;
            foreach (var skin in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                skin.updateWhenOffscreen = true;
                var materials = skin.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    var material = new Material(materials[i]); material.shader = Resources.Load<Shader>("CelCourt");
                    bool pants = skin.name.Contains("pants") || skin.name.Contains("jeans");
                    material.SetFloat("_Flat", pants || skin.name.Contains("tshirt") ? 1 : 0);
                    material.SetFloat("_Outline", .008f);
                    if (pants) material.color = new Color(.04f, .13f, .30f);
                    materials[i] = material; toonMaterials.Add(material);
                }
                skin.sharedMaterials = materials;
            }
            MakeRacket(); ApplyAppearance(null); MakeClips(); Present("Ready", 1, Vector3.zero, Vector3.zero, 0, 0);
        }
        public void ApplyAppearance(AvatarAppearance look)
        {
            look?.Validate();
            var block = new MaterialPropertyBlock();
            foreach (var skin in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                bool shirt = skin.name.Contains("tshirt");
                block.Clear();
                if (look == null)
                {
                    if (shirt) block.SetColor("_Color", athlete.name == "Player" || athlete.name == "Partner" ? new Color(.12f, .40f, 1) : new Color(1, .30f, .13f));
                }
                else if (shirt)
                {
                    block.SetColor("_Color", AvatarAppearance.Palette[look.jerseyColorId]);
                    block.SetFloat("_Flat", look.jerseyStyleId == 0 ? 1 : 0);
                    if (look.jerseyStyleId != 0)
                        block.SetTexture("_MainTex", Resources.Load<Texture2D>(look.jerseyStyleId == 1 ? "CareerJersey/shoulder" : "CareerJersey/side"));
                }
                else if (skin.name.Contains("pants")) block.SetColor("_Color", AvatarAppearance.Dark[look.pantsId]);
                else if (skin.name.Contains("shoe")) block.SetColor("_Color", AvatarAppearance.Palette[look.shoeColorId]);
                else if (skin.name.Contains("haircut")) block.SetColor("_Color", AvatarAppearance.Dark[look.hairColorId]);
                skin.SetPropertyBlock(block.isEmpty ? null : block);
            }
            foreach (var line in Grip.GetComponentsInChildren<LineRenderer>())
                line.startColor = line.endColor = line.name == "Grip" ? (look == null ? new Color(.11f, .14f, .17f) : AvatarAppearance.Dark[look.gripColorId])
                    : line.name == "String" ? (look == null ? Color.white : AvatarAppearance.Palette[look.stringColorId])
                    : look == null ? (line.name == "Frame" ? new Color(.85f, .92f, .93f) : Color.white) : AvatarAppearance.Palette[look.racketColorId];
        }
        void Segment(string name, Vector3 a, Vector3 b, float width, Color color, Transform parent)
        {
            var line = new GameObject(name).AddComponent<LineRenderer>(); line.transform.SetParent(parent, false);
            line.useWorldSpace = false; line.positionCount = 2; line.SetPosition(0, a); line.SetPosition(1, b); line.startWidth = line.endWidth = width;
            line.sharedMaterial = racketMaterial; line.startColor = line.endColor = color;
        }
        void MakeRacket()
        {
            Grip = new GameObject("RacketSocket").transform; Grip.SetParent(hand, false); Grip.rotation = athlete.rotation;
            racketMaterial = new Material(Shader.Find("Sprites/Default"));
            Segment("Grip", new Vector3(0, -.05f, 0), new Vector3(0, .08f, 0), .036f, new Color(.11f, .14f, .17f), Grip);
            Segment("Shaft", new Vector3(0, .08f, 0), new Vector3(0, .27f, 0), .012f, Color.white, Grip);
            StringBed = new GameObject("StringBed").transform; StringBed.SetParent(Grip, false); StringBed.localPosition = Vector3.up * RacketLength;
            var frame = new GameObject("Frame").AddComponent<LineRenderer>(); frame.transform.SetParent(StringBed, false);
            frame.useWorldSpace = false; frame.loop = true; frame.positionCount = 32; frame.startWidth = frame.endWidth = .014f; frame.sharedMaterial = racketMaterial;
            frame.startColor = frame.endColor = new Color(.85f, .92f, .93f);
            for (int i = 0; i < 32; i++) { float a = i * Mathf.PI / 16; frame.SetPosition(i, new Vector3(Mathf.Sin(a) * .11f, Mathf.Cos(a) * .145f, 0)); }
            for (int i = -3; i <= 3; i++)
            {
                float x = i * .026f, y = .145f * Mathf.Sqrt(1 - x * x / (.11f * .11f));
                Segment("String", new Vector3(x, -y, 0), new Vector3(x, y, 0), .003f, Color.white, StringBed);
                float yy = i * .034f, xx = .11f * Mathf.Sqrt(1 - yy * yy / (.145f * .145f));
                Segment("String", new Vector3(-xx, yy, 0), new Vector3(xx, yy, 0), .003f, Color.white, StringBed);
            }
        }
        void MakeClips()
        {
            clips = gameObject.AddComponent<Animation>(); clips.playAutomatically = false; clips.cullingType = AnimationCullingType.AlwaysAnimate;
            foreach (string name in new[] { "Ready", "Backhand", "BackhandLow", "Smash", "Clear", "Drop", "Net", "Lift", "Serve", "MoveForward", "MoveBackward", "MoveLeft", "MoveRight", "Jump", "Land" })
            {
                float length = name == "Smash" ? .50f : name == "Clear" || name == "Lift" ? .62f : .48f;
                var clip = new AnimationClip { name = name, legacy = true, frameRate = 30, wrapMode = name.StartsWith("Move", StringComparison.Ordinal) || name == "Ready" ? WrapMode.Loop : WrapMode.ClampForever };
                bool back = name.StartsWith("Backhand", StringComparison.Ordinal), low = name == "Net" || name == "Lift" || name == "Serve" || name == "BackhandLow";
                float lean = name == "Smash" ? -18 : low ? 20 : name == "Ready" ? 8 : -10;
                float twist = back ? 38 : name == "Ready" || name.StartsWith("Move", StringComparison.Ordinal) ? 0 : -30;
                AddRotation(clip, chest, Chest, new[] { Vector3.zero, new Vector3(lean, twist, back ? -8 : 4), new Vector3(22, -twist * .65f, -8), new Vector3(8, 0, 0) }, length);
                AddRotation(clip, leftUpper.parent, Chest + "/LeftShoulder", new[] { Vector3.zero, new Vector3(-14, 0, -18), new Vector3(10, 0, 10), Vector3.zero }, length);
                clip.EnsureQuaternionContinuity(); clips.AddClip(clip, name);
            }
        }
        void AddRotation(AnimationClip clip, Transform bone, string path, Vector3[] angles, float duration)
        {
            var keys = new Keyframe[4][]; for (int c = 0; c < 4; c++) keys[c] = new Keyframe[angles.Length];
            for (int i = 0; i < angles.Length; i++)
            {
                Quaternion q = bind[bone] * Quaternion.Euler(angles[i]);
                for (int c = 0; c < 4; c++) keys[c][i] = new Keyframe(duration * i / (angles.Length - 1), q[c]);
            }
            for (int c = 0; c < 4; c++) clip.SetCurve(path, typeof(Transform), "localRotation." + "xyzw"[c], new AnimationCurve(keys[c]));
        }
        static void Solve(Transform a, Transform b, Transform tip, Vector3 target, Vector3 pole)
        {
            float upper = Vector3.Distance(a.position, b.position), lower = Vector3.Distance(b.position, tip.position);
            Vector3 d = target - a.position; float distance = Mathf.Clamp(d.magnitude, Mathf.Abs(upper - lower) + .0001f, upper + lower - .0001f);
            Vector3 axis = d.sqrMagnitude > .000001f ? d.normalized : Vector3.up;
            Vector3 bend = Vector3.ProjectOnPlane(pole, axis).normalized;
            if (bend.sqrMagnitude < .001f) bend = Vector3.Cross(axis, Vector3.right).normalized;
            float along = (upper * upper - lower * lower + distance * distance) / (2 * distance);
            Vector3 joint = a.position + axis * along + bend * Mathf.Sqrt(Mathf.Max(0, upper * upper - along * along));
            a.rotation = Quaternion.FromToRotation(b.position - a.position, joint - a.position) * a.rotation;
            b.rotation = Quaternion.FromToRotation(tip.position - b.position, target - b.position) * b.rotation;
        }
        public bool CanReach(Vector3 point, string shot) => ReachFailure(point, shot).Length == 0;
        public string ReachFailure(Vector3 point, string shot)
        {
            Vector3 p = athlete.InverseTransformPoint(point);
            bool high = shot == "Smash" || shot == "Clear" || shot == "Drop";
            string label = shot == "Clear" ? "lob" : shot.ToLowerInvariant();
            if (p.y < (high ? 1.45f : .25f)) return "Kok terlalu rendah untuk " + label;
            if (p.y > (high ? 2.8f : 1.9f)) return "Kok terlalu tinggi untuk " + label;
            Vector3 shoulder = athlete.TransformPoint(new Vector3(.183f, high ? 1.46f : 1.34f, -.018f));
            return Vector3.Distance(point, shoulder) < armLength + RacketLength - .012f ? "" : "Di luar jangkauan";
        }
        public void Present(string stroke, float elapsed, Vector3 contact, Vector3 displacement, float jumpHeight, float landing)
        {
            if (!float.IsFinite(elapsed) || !float.IsFinite(jumpHeight) || !float.IsFinite(displacement.sqrMagnitude)) throw new ArgumentOutOfRangeException(nameof(elapsed));
            foreach (var p in bind) p.Key.localRotation = p.Value;
            hips.localPosition = hipPosition;
            bool active = elapsed >= 0 && elapsed < .65f, back = athlete.InverseTransformPoint(contact).x < -.1f;
            bool low = stroke == "Net" || stroke == "Lift" || stroke == "Serve";
            travel += displacement.magnitude;
            Vector3 localStep = athlete.InverseTransformDirection(displacement);
            CurrentClip = active ? (back ? low ? "BackhandLow" : "Backhand" : stroke) : jumpHeight > .01f ? "Jump" : landing > 0 ? "Land" : displacement.sqrMagnitude > .00001f ? (Mathf.Abs(localStep.x) > Mathf.Abs(localStep.z) ? localStep.x > 0 ? "MoveRight" : "MoveLeft" : localStep.z > 0 ? "MoveForward" : "MoveBackward") : "Ready";
            if (clips[CurrentClip] == null) CurrentClip = "Ready";
            foreach (AnimationState state in clips) state.enabled = false;
            float airborne = Mathf.SmoothStep(0, 1, jumpHeight / .30f);
            float airborneContact = active && jumpHeight > .01f ? 1 - Mathf.SmoothStep(0, 1, elapsed / .18f) : 0;
            Quaternion preparedShoulder = Quaternion.identity;
            if (airborneContact > 0)
            {
                var prepared = clips["Jump"]; prepared.enabled = true; prepared.speed = 0; prepared.weight = 1;
                prepared.time = Mathf.Repeat(travel * .22f, prepared.length); clips.Sample(); prepared.enabled = false;
                preparedShoulder = leftUpper.parent.localRotation;
            }
            var selected = clips[CurrentClip]; selected.enabled = true; selected.speed = 0; selected.weight = 1;
            ClipTime = active ? Mathf.Min(selected.length, elapsed + selected.length / 3) : Mathf.Repeat(travel * .22f, selected.length);
            selected.time = ClipTime; clips.Sample(); selected.enabled = false; // Manual sample owns pose; automatic pass must not overwrite IK or pause.
            float recovery = active ? 1 - Mathf.SmoothStep(0, 1, (elapsed - .18f) / .47f) : 0;
            hips.localPosition = hipPosition - Vector3.up * (.035f + (low ? .12f * recovery : 0) + landing * .08f);
            if (active)
            {
                Vector3 local = athlete.InverseTransformPoint(contact);
                hips.localRotation = bind[hips] * Quaternion.Euler(0, 0, Mathf.Clamp(-local.x * 28, -16, 16) * recovery);
            }
            float preparation = !active ? Mathf.SmoothStep(0, 1, jumpHeight / .30f) : 0;
            // ponytail: small ready-only breath; recorded elapsed freezes/reconstructs it without extra pose state.
            float breath = CurrentClip == "Ready" ? .8f * Mathf.Sin((elapsed - .65f) * (2 * Mathf.PI / 3.2f)) * Mathf.SmoothStep(0, 1, (elapsed - .65f) / .45f) : 0;
            if (!active) chest.localRotation = bind[chest] * Quaternion.Euler(8 - 20 * preparation + breath, -28 * preparation, 6 * preparation);
            if (airborneContact > 0)
            {
                hips.localRotation = Quaternion.Slerp(hips.localRotation, bind[hips], airborneContact);
                chest.localRotation = Quaternion.Slerp(chest.localRotation, bind[chest] * Quaternion.Euler(8 - 20 * airborne, -28 * airborne, 6 * airborne), airborneContact);
                leftUpper.parent.localRotation = Quaternion.Slerp(leftUpper.parent.localRotation, preparedShoulder, airborneContact);
            }
            // Menu-only resting racket; gameplay ready/contact/recovery remain unchanged.
            Vector3 ready = athlete.TransformPoint(stroke == "Menu" && !active
                ? new Vector3(.45f, 1.35f, .25f) : new Vector3(.32f, 1.65f, .40f));
            Vector3 target = Vector3.Lerp(ready, athlete.TransformPoint(new Vector3(.48f, 2.02f, -.12f)), preparation);
            if (active)
            {
                Vector3 follow = stroke == "Smash" ? athlete.TransformPoint(new Vector3(back ? .42f : -.42f, 1.05f, .55f))
                    : contact + athlete.TransformDirection(stroke == "Net" ? new Vector3(back ? .08f : -.08f, .04f, .09f)
                        : stroke == "Lift" ? new Vector3(back ? .18f : -.18f, .52f, .18f)
                        : new Vector3(back ? .28f : -.30f, low ? .18f : -.12f, .12f));
                target = elapsed < .18f ? Vector3.Lerp(contact, follow, Mathf.SmoothStep(0, 1, elapsed / .18f)) : Vector3.Lerp(follow, ready, Mathf.SmoothStep(0, 1, (elapsed - .18f) / .47f));
            }
            Vector3 shaft = Vector3.Slerp(athlete.up, (target - upper.position).normalized, Mathf.Max(recovery, preparation));
            Vector3 palmUp = Vector3.ProjectOnPlane(-athlete.forward, shaft).normalized;
            if (palmUp.sqrMagnitude < .001f) palmUp = athlete.up;
            Quaternion palm = Quaternion.LookRotation(shaft, palmUp);
            var socket = hand.Find("RightHandProp");
            Vector3 handTarget = target - shaft * RacketLength - palm * socket.localPosition;
            Vector3 elbowPole = Vector3.Lerp(new Vector3(.35f, -1, -.25f), new Vector3(active && back ? -1 : 1, -.3f, -.8f), Mathf.Max(recovery, preparation));
            Solve(upper, lower, hand, handTarget, athlete.TransformDirection(elbowPole));
            hand.rotation = palm;
            Grip.SetPositionAndRotation(socket.position, Quaternion.LookRotation(-palmUp, shaft));
            foreach (string finger in new[] { "Index", "Middle", "Ring", "Pinky" })
            {
                var joint = hand.Find("RightHand" + finger + "1");
                joint.localRotation = bind[joint] * Quaternion.Euler(0, 0, 70);
                joint = joint.Find("RightHand" + finger + "2"); joint.localRotation = bind[joint] * Quaternion.Euler(0, 0, 70);
                joint = joint.Find("RightHand" + finger + "3"); joint.localRotation = bind[joint] * Quaternion.Euler(0, 0, 40);
            }
            var thumb = hand.Find("RightHandThumb1"); thumb.localRotation = bind[thumb] * Quaternion.Euler(0, -25, 20);
            PoseFreeArm(stroke, localStep, elapsed, recovery, airborne, airborneContact);
            PlantFeet(displacement, active && low, jumpHeight);
        }
        void PoseFreeArm(string stroke, Vector3 step, float elapsed, float recovery, float airborne, float airborneContact)
        {
            // Keep the wrist ahead of the hip and let the elbow hang under it.
            var target = new Vector3(-.29f, 1.23f, .39f);
            var pole = new Vector3(-.12f, -1f, -.30f);
            float stepWeight = Mathf.Clamp01(step.magnitude * 6f);
            target.z += Mathf.Sin(travel * (2 * Mathf.PI / GaitCycleMetres)) * .07f * stepWeight;
            target.x -= Mathf.Min(.06f, Mathf.Abs(step.x) * .3f) * stepWeight;

            float swing = recovery * Mathf.SmoothStep(0, 1, elapsed / .16f);
            Vector3 counterbalance;
            switch (stroke)
            {
                case "Net":
                case "Lift":
                    counterbalance = new Vector3(-.47f, 1.13f, -.08f);
                    break;
                case "Serve":
                    counterbalance = new Vector3(-.29f, 1.32f, .43f);
                    break;
                default:
                    counterbalance = new Vector3(-.38f, 1.52f, .35f);
                    break;
            }
            target = Vector3.Lerp(target, counterbalance, swing);
            // The airborne ready pose and first contact sample use the same target and pole.
            float preparation = airborne * Mathf.Clamp01(airborneContact + 1 - recovery);
            target = Vector3.Lerp(target, new Vector3(-.38f, 1.85f, .35f), preparation);
            pole = Vector3.Lerp(pole, new Vector3(-.65f, -.45f, -.50f), preparation);
            Solve(leftUpper, leftLower, leftHand, athlete.TransformPoint(target), athlete.TransformDirection(pole));
        }
        public void PresentReaction(bool won, float weight)
        {
            if (!float.IsFinite(weight) || weight < 0 || weight > 1) throw new ArgumentOutOfRangeException(nameof(weight));
            if (weight == 0) return;
            Vector3 target = athlete.TransformPoint(won ? new Vector3(-.32f, 1.9f, .20f) : new Vector3(-.32f, .98f, .24f));
            var pole = Vector3.Lerp(new Vector3(-.12f, -1f, -.30f), new Vector3(-.65f, -.45f, -.50f), won ? weight : 0);
            Solve(leftUpper, leftLower, leftHand, Vector3.Lerp(leftHand.position, target, weight), athlete.TransformDirection(pole));
            var head = Bone(Chest + "/Neck/Head");
            head.localRotation *= Quaternion.Euler((won ? -8 : 14) * weight, 0, 0);
        }
        void PlantFeet(Vector3 step, bool lunge, float height)
        {
            FootContacts = 0;
            bool reconstruct = restoredFeet.recorded;
            if (reconstruct)
            {
                plants[0] = restoredFeet.left; plants[1] = restoredFeet.right;
                planted = restoredFeet.planted; wasAirborne = restoredFeet.airborne;
                plantRoot = athlete.position; plantRotation = athlete.rotation;
            }
            bool reset = Vector3.Distance(plantRoot, athlete.position) > .5f || Quaternion.Angle(plantRotation, athlete.rotation) > 45;
            if (height <= .01f && wasAirborne && !reset) FootContacts = 3;
            else if (height <= .01f && planted && !reset && !lunge && step.sqrMagnitude > .000001f)
                for (int i = 0; i < 2; i++)
                    if (Mathf.FloorToInt(travel / GaitCycleMetres + i * .5f) > Mathf.FloorToInt((travel - step.magnitude) / GaitCycleMetres + i * .5f)) FootContacts |= 1 << i;
            wasAirborne = height > .01f;
            if (!planted || height > .01f || reset)
            {
                for (int i = 0; i < 2; i++) plants[i] = athlete.TransformPoint(new Vector3(i == 0 ? -.18f : .18f, .13f, .03f));
                planted = height <= .01f;
            }
            plantRoot = athlete.position; plantRotation = athlete.rotation;
            for (int i = 0; i < 2; i++)
            {
                float cycle = Mathf.Repeat(travel / GaitCycleMetres + i * .5f, 1);
                Vector3 rest = athlete.TransformPoint(new Vector3(i == 0 ? -.20f : .20f, .13f, lunge ? (i == 1 ? .33f : -.26f) : .03f));
                Vector3 target;
                if (height > .01f) target = rest + athlete.forward * (i == 0 ? -.08f : .08f) + athlete.up * .11f;
                else if (step.sqrMagnitude < .000001f || lunge) { if (!reconstruct) plants[i] = Vector3.MoveTowards(plants[i], rest, lunge ? .08f : .025f); target = plants[i]; target.y = .13f; }
                else if (cycle < .5f)
                {
                    target = plants[i];
                    float reach = Vector3.Distance(thighs[i].position, knees[i].position) + Vector3.Distance(knees[i].position, feet[i].position);
                    if (Vector3.Distance(target, thighs[i].position) > reach - .0001f) { plants[i] = rest; target = rest; }
                }
                else { target = rest + step.normalized * .16f + Vector3.up * Mathf.Sin((cycle - .5f) * Mathf.PI * 2) * .09f; plants[i] = new Vector3(target.x, .13f, target.z); }
                Solve(thighs[i], knees[i], feet[i], target, athlete.forward);
                feet[i].rotation = athlete.rotation * footRest[i];
            }
        }
        void OnDestroy()
        {
            if (clips != null) foreach (AnimationState state in clips) Destroy(state.clip);
            if (racketMaterial != null) Destroy(racketMaterial);
            foreach (var material in toonMaterials) Destroy(material);
        }
    }
}
