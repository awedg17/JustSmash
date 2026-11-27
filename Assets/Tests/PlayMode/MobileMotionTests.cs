using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MobileMotionTests : PreferenceFixture
{
    const string Hip = "Root/Hips", Chest = Hip + "/Spine/Spine1";
    [UnityTest] public IEnumerator ReadyBreathingUsesReplayablePoseTimeWithoutMovingRootsOrFeet()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var ct = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(ct);
        ct.GetMethod("ConfigureDiscipline").Invoke(court, new object[] { true }); ((Behaviour)court).enabled = false;
        var errors = new System.Collections.Generic.List<string>();
        var rows = new System.Collections.Generic.List<string> { "actor,yaw,chestRangeDegrees,maxFootDriftMetres" };
        foreach (string name in new[] { "Player", "Opponent", "Partner", "OpponentPartner" }) foreach (float yaw in new[] { 0f, 180f })
        {
            var root = GameObject.Find(name).transform; root.rotation = Quaternion.Euler(0, yaw, 0);
            var model = root.Find("AthleteVisual"); var driver = model.GetComponents<MonoBehaviour>().Single(t => t.GetType().Name == "AthleteVisual");
            var present = driver.GetType().GetMethod("Present"); var chest = model.Find(Chest);
            var feet = new[] { model.Find(Hip + "/LeftUpLeg/LeftLeg/LeftFoot"), model.Find(Hip + "/RightUpLeg/RightLeg/RightFoot") };
            var bones = model.GetComponentsInChildren<Transform>().Where(t => t != model && t.parent != null).ToArray();
            void Pose(float elapsed) => present.Invoke(driver, new object[] { "Clear", elapsed, Vector3.zero, Vector3.zero, 0f, 0f });
            for (int i = 0; i < 24; i++) Pose(.65f);
            var footPositions = feet.Select(t => t.position).ToArray(); var lengths = bones.Select(t => Vector3.Distance(t.position, t.parent.position)).ToArray();
            Vector3 position = root.position; Quaternion rotation = root.rotation, initial = chest.localRotation, previous = initial;
            float range = 0, footDrift = 0;
            for (int frame = 0; frame <= 210; frame++)
            {
                float elapsed = .65f + frame / 60f; Pose(elapsed);
                range = Mathf.Max(range, Quaternion.Angle(initial, chest.localRotation));
                Assert.That(Quaternion.Angle(previous, chest.localRotation), Is.LessThan(.15f), "Breath seam " + name); previous = chest.localRotation;
                for (int i = 0; i < feet.Length; i++) footDrift = Mathf.Max(footDrift, Vector3.Distance(footPositions[i], feet[i].position));
                Assert.That(root.position, Is.EqualTo(position)); Assert.That(root.rotation, Is.EqualTo(rotation));
                Assert.That(driver.GetType().GetMethod("TakeFootContacts").Invoke(driver, null), Is.EqualTo(0));
            }
            rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2:R},{3:R}", name, yaw, range, footDrift));
            if (range <= .5f) errors.Add(name + "/" + yaw + " visible chest remains frozen: " + range);
            Assert.That(range, Is.LessThan(3f)); Assert.That(footDrift, Is.LessThan(.001f));
            foreach (float elapsed in new[] { .65f, 1f, 2f, 3.1f })
            {
                Pose(elapsed); var saved = bones.Select(t => t.localRotation).ToArray();
                present.Invoke(driver, new object[] { "Smash", .18f, root.TransformPoint(new Vector3(.3f, 2.1f, .35f)), Vector3.zero, 0f, 0f });
                Pose(elapsed);
                for (int i = 0; i < bones.Length; i++)
                {
                    Assert.That(Quaternion.Angle(saved[i], bones[i].localRotation), Is.LessThan(.05f), "Unrecorded breath phase " + bones[i].name);
                    if (bones[i].name != "Hips") Assert.That(Vector3.Distance(bones[i].position, bones[i].parent.position), Is.EqualTo(lengths[i]).Within(.001f));
                }
                Quaternion frozen = chest.localRotation; Time.timeScale = 0;
                yield return new WaitForSecondsRealtime(.02f); Pose(elapsed);
                Assert.That(Quaternion.Angle(frozen, chest.localRotation), Is.LessThan(.01f)); Time.timeScale = 1;
            }
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/idle-breath.csv", rows);
        Assert.That(rows.Count - 1, Is.EqualTo(8)); Assert.That(errors, Is.Empty, string.Join("; ", errors));
    }
    [UnityTest] public IEnumerator LocomotionElbowIgnoresExpiredContactAndRecoversWithoutSnap()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var courtType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(courtType);
        courtType.GetMethod("ConfigureDiscipline").Invoke(court, new object[] { true }); ((Behaviour)court).enabled = false;
        foreach (string name in new[] { "Player", "Opponent", "Partner", "OpponentPartner" })
        foreach (float yaw in new[] { 0f, 180f })
        {
            var root = GameObject.Find(name).transform; root.rotation = Quaternion.Euler(0, yaw, 0);
            var model = root.Find("AthleteVisual");
            var driver = model.GetComponents<MonoBehaviour>().Single(t => t.GetType().Name == "AthleteVisual");
            var present = driver.GetType().GetMethod("Present");
            var elbow = model.Find(Chest + "/RightShoulder/RightArm/RightForeArm");
            Vector3 origin = root.position, oldContact = root.TransformPoint(new Vector3(0, 2.1f, .35f));
            root.position = origin + root.right * .09f;
            present.Invoke(driver, new object[] { "Clear", 10f, oldContact, Vector3.zero, 0f, 0f });
            Vector3 before = root.InverseTransformPoint(elbow.position);
            Quaternion armBefore = elbow.parent.localRotation, forearmBefore = elbow.localRotation;
            root.position += root.right * .02f;
            present.Invoke(driver, new object[] { "Clear", 10f, oldContact, root.right * .02f, 0f, 0f });
            float jump = Vector3.Distance(before, root.InverseTransformPoint(elbow.position));
            float armTurn = Quaternion.Angle(armBefore, elbow.parent.localRotation), forearmTurn = Quaternion.Angle(forearmBefore, elbow.localRotation);
            TestContext.WriteLine(name + " yaw=" + yaw + " expired-contact elbow jump=" + jump + " upperTurn=" + armTurn + " lowerTurn=" + forearmTurn);
            Assert.That(jump, Is.LessThan(.01f), "Walking across old contact reverses elbow: " + name);
            Assert.That(armTurn, Is.LessThan(1f), "Expired contact must not rotate ready upper arm: " + name);
            Assert.That(forearmTurn, Is.LessThan(1f), "Expired contact must not twist ready forearm: " + name);
            Vector3 backhand = root.TransformPoint(new Vector3(-.4f, 2.1f, .35f));
            present.Invoke(driver, new object[] { "Clear", .649f, backhand, Vector3.zero, 0f, 0f });
            before = root.InverseTransformPoint(elbow.position);
            present.Invoke(driver, new object[] { "Clear", .651f, backhand, Vector3.zero, 0f, 0f });
            Assert.That(Vector3.Distance(before, root.InverseTransformPoint(elbow.position)), Is.LessThan(.01f), "Elbow snaps from backhand recovery to ready: " + name);
        }
    }
    [UnityTest] public IEnumerator ReadyRacketElbowStaysBelowShoulderWithoutLocking()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var courtType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(courtType);
        courtType.GetMethod("ConfigureDiscipline").Invoke(court, new object[] { true }); ((Behaviour)court).enabled = false;
        foreach (string name in new[] { "Player", "Opponent", "Partner", "OpponentPartner" })
        foreach (float yaw in new[] { 0f, 180f })
        foreach (float distance in new[] { 0f, .025f, .075f })
        {
            var root = GameObject.Find(name).transform; root.rotation = Quaternion.Euler(0, yaw, 0);
            var model = root.Find("AthleteVisual");
            var driver = model.GetComponents<MonoBehaviour>().Single(t => t.GetType().Name == "AthleteVisual");
            root.position += root.forward * distance;
            driver.GetType().GetMethod("Present").Invoke(driver, new object[] { "Clear", 10f, Vector3.zero, root.forward * distance, 0f, 0f });
            var shoulder = model.Find(Chest + "/RightShoulder/RightArm"); var elbow = shoulder.Find("RightForeArm"); var hand = elbow.Find("RightHand");
            float drop = Vector3.Dot(shoulder.position - elbow.position, root.up);
            float angle = Vector3.Angle(shoulder.position - elbow.position, hand.position - elbow.position);
            TestContext.WriteLine(name + " ready elbow drop=" + drop + " angle=" + angle);
            Assert.That(drop, Is.GreaterThan(.08f), "Ready elbow sticks out at shoulder height");
            Assert.That(angle, Is.InRange(40f, 150f), "Ready arm folds shut or locks straight");
        }
    }
    [UnityTest] public IEnumerator ReadyHandsRemainSeparatedDespiteSideViewOverlap()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var courtType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(courtType);
        courtType.GetMethod("ConfigureDiscipline").Invoke(court, new object[] { true }); ((Behaviour)court).enabled = false;
        foreach (string name in new[] { "Player", "Opponent", "Partner", "OpponentPartner" })
        foreach (float yaw in new[] { 0f, 180f })
        {
            var root = GameObject.Find(name).transform; root.rotation = Quaternion.Euler(0, yaw, 0);
            var model = root.Find("AthleteVisual");
            var driver = model.GetComponents<MonoBehaviour>().Single(t => t.GetType().Name == "AthleteVisual");
            var left = model.Find(Chest + "/LeftShoulder/LeftArm/LeftForeArm/LeftHand");
            var right = model.Find(Chest + "/RightShoulder/RightArm/RightForeArm/RightHand");
            foreach (float distance in new[] { 0f, .025f, .075f })
            {
                root.position += root.forward * distance;
                driver.GetType().GetMethod("Present").Invoke(driver, new object[] { "Clear", 10f, Vector3.zero, root.forward * distance, 0f, 0f });
                var leftJoints = left.GetComponentsInChildren<Transform>().Where(t => t.name.StartsWith("LeftHand")).ToArray();
                var rightJoints = right.GetComponentsInChildren<Transform>().Where(t => t.name.StartsWith("RightHand")).ToArray();
                Assert.That(leftJoints.Length, Is.GreaterThan(10)); Assert.That(rightJoints.Length, Is.GreaterThan(10));
                float nearest = leftJoints.Min(a => rightJoints.Min(b => Vector3.Distance(a.position, b.position)));
                TestContext.WriteLine(name + " yaw=" + yaw + " step=" + distance + " nearestHandJointMetres=" + nearest);
                Assert.That(nearest, Is.GreaterThan(.15f), "Hand skeletons intersect; image overlap cannot be dismissed as projection");
                if (name != "Player" || yaw != 0 || distance != 0) continue;
                yield return null;
                var camera = Camera.main; var position = camera.transform.position; var rotation = camera.transform.rotation;
                var old = camera.targetTexture; var active = RenderTexture.active; var target = RenderTexture.GetTemporary(1280, 720, 24);
                try
                {
                    foreach (string view in new[] { "front", "side" })
                    {
                        camera.transform.position = root.position + (view == "front" ? root.forward * 3 : root.right * 3) + Vector3.up * 1.35f;
                        camera.transform.LookAt(root.position + Vector3.up * 1.1f); camera.targetTexture = target; camera.Render(); RenderTexture.active = target;
                        var image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
                        image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); image.Apply();
                        System.IO.File.WriteAllBytes(EvidenceDirectory + "/ready-" + view + ".png", image.EncodeToPNG()); UnityEngine.Object.Destroy(image);
                    }
                }
                finally { camera.transform.SetPositionAndRotation(position, rotation); camera.targetTexture = old; RenderTexture.active = active; RenderTexture.ReleaseTemporary(target); }
            }
        }
    }
    [UnityTest] public IEnumerator ReadyRacketHasUprightReadableSilhouette()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var courtType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(courtType);
        courtType.GetMethod("EnterTraining").Invoke(court, null); yield return null;
        var root = GameObject.Find("Player").transform;
        var model = root.Find("AthleteVisual");
        var hand = model.Find(Chest + "/RightShoulder/RightArm/RightForeArm/RightHand");
        var grip = hand.Find("RacketSocket");
        Assert.That(Vector3.Dot(grip.up, root.up), Is.GreaterThan(.6f), "Ready racket lies across torso instead of above gripping hand");
        Assert.That(root.InverseTransformPoint(grip.Find("StringBed").position).y, Is.GreaterThan(1.5f));
        var palm = root.InverseTransformPoint(grip.position);
        Assert.That(palm.y, Is.LessThan(1.45f), "Ready hand is tucked beside neck instead of in front of torso");
        Assert.That(palm.z, Is.GreaterThan(.35f), "Ready grip must clear torso silhouette");
    }
    [UnityTest] public IEnumerator RacketHandleSitsInPalmAcrossCurledFingers()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var root=GameObject.Find("Player").transform; var model=root.Find("AthleteVisual");
        var hand=model.Find(Chest+"/RightShoulder/RightArm/RightForeArm/RightHand"); var grip=hand.Find("RacketSocket");
        Assert.That(Vector3.Distance(grip.position,hand.Find("RightHandProp").position),Is.LessThan(.015f),"Grip attached to wrist instead of palm");
        Assert.That(Mathf.Abs(Vector3.Dot(grip.up,hand.right)),Is.LessThan(.2f),"Handle runs along fingers instead of across palm");
        foreach(string finger in new[] {"Index","Middle","Ring","Pinky"})
        {
            var tip=hand.Find("RightHand"+finger+"1/RightHand"+finger+"2/RightHand"+finger+"3");
            Assert.That(Vector3.ProjectOnPlane(tip.position-grip.position,grip.up).magnitude,Is.LessThan(.045f),finger+" does not wrap handle");
        }
    }
    [UnityTest] public IEnumerator StrokeRecoveryHasNoHipSnapAtActiveBoundary()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var root=GameObject.Find("Player").transform; var model=root.Find("AthleteVisual");
        var driver=model.GetComponents<MonoBehaviour>().Single(t=>t.GetType().Name=="AthleteVisual");
        var present=driver.GetType().GetMethod("Present");
        foreach(string shot in new[] {"Smash","Net"}) foreach(float side in new[] {-.4f,.4f})
        {
            Vector3 contact=root.TransformPoint(new Vector3(side,shot=="Smash"?2.1f:1,.35f));
            present.Invoke(driver,new object[]{shot,.649f,contact,Vector3.zero,0f,0f});
            var hips=model.Find(Hip); Quaternion before=hips.localRotation; Vector3 position=hips.localPosition;
            var freeHand=model.Find(Chest+"/LeftShoulder/LeftArm/LeftForeArm/LeftHand"); Vector3 wrist=freeHand.position;
            present.Invoke(driver,new object[]{shot,.651f,contact,Vector3.zero,0f,0f});
            Assert.That(Quaternion.Angle(before,hips.localRotation),Is.LessThan(1),shot+" hip snaps at recovery end");
            Assert.That(Vector3.Distance(position,hips.localPosition),Is.LessThan(.005f),shot+" pelvis pops at recovery end");
            Assert.That(Vector3.Distance(wrist,freeHand.position),Is.LessThan(.01f),shot+" free arm snaps at recovery end");
        }
    }
    [UnityTest] public IEnumerator ReadyFreeArmBendsDownWithWristInFrontOfHipAtBothEnds()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var root = GameObject.Find("Player").transform;
        var model = root.Find("AthleteVisual");
        var driver = model.GetComponents<MonoBehaviour>().Single(t => t.GetType().Name == "AthleteVisual");
        var present = driver.GetType().GetMethod("Present");
        var arm = model.Find(Chest + "/LeftShoulder/LeftArm");
        var elbow = arm.Find("LeftForeArm");
        var wrist = elbow.Find("LeftHand");
        foreach (float yaw in new[] { 0f, 180f })
        {
            root.rotation = Quaternion.Euler(0, yaw, 0);
            foreach (var step in new[] { Vector3.zero, root.forward * .12f, root.right * .12f })
            {
                present.Invoke(driver, new object[] { "Ready", 1f, root.position, step, 0f, 0f });
                Vector3 localElbow = root.InverseTransformPoint(elbow.position);
                Vector3 localShoulder = root.InverseTransformPoint(arm.position);
                Vector3 localWrist = root.InverseTransformPoint(wrist.position);
                Assert.That(localWrist.z, Is.GreaterThan(.28f), "Wrist sits at the hip instead of out in front");
                Assert.That(localElbow.y, Is.LessThan(localShoulder.y - .12f), "Elbow must hang below the shoulder");
                Assert.That(localElbow.x, Is.GreaterThan(localShoulder.x - .15f), "Elbow flares out into an akimbo silhouette");
            }
        }
    }
    [UnityTest] public IEnumerator AirborneImmediateContactKeepsFreeHandContinuous()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        const BindingFlags hidden = BindingFlags.Instance | BindingFlags.NonPublic;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type); ((Behaviour)court).enabled = false;
        type.GetMethod("Feed").Invoke(court, new object[] { "High" });
        var root = GameObject.Find("Player").transform; var model = root.Find("AthleteVisual");
        var hand = model.Find(Chest + "/LeftShoulder/LeftArm/LeftForeArm/LeftHand");
        var elbow = hand.parent; var shoulder = elbow.parent;
        float upperLength = Vector3.Distance(shoulder.position, elbow.position), lowerLength = Vector3.Distance(elbow.position, hand.position);
        type.GetMethod("Jump").Invoke(court, null);
        for (int i = 0; i < 15; i++) type.GetMethod("FixedUpdate", hidden).Invoke(court, null);
        type.GetMethod("AnimateAthlete", hidden).Invoke(court, null);
        Assert.That(root.position.y, Is.GreaterThan(.3f));
        var shuttle = GameObject.Find("Shuttle").transform;
        shuttle.position = root.TransformPoint(new Vector3(.4f, 2.1f, .35f));
        type.GetField("velocity", hidden).SetValue(court, Vector3.zero);
        Vector3 before = hand.position, position = root.position; Quaternion rotation = hand.rotation;
        var torso = new[] { model.Find(Hip), model.Find(Chest), shoulder.parent };
        var torsoBefore = torso.Select(t => t.localRotation).ToArray();
        Vector3 shoulderBefore = shoulder.position;
        float energy = (float)type.GetProperty("Energy").GetValue(court), instant = Time.time;
        type.GetMethod("TapShot").Invoke(court, new object[] { "Smash" });
        float jump = Vector3.Distance(before, hand.position), turn = Quaternion.Angle(rotation, hand.rotation);
        TestContext.WriteLine("Immediate free-hand jump=" + jump.ToString("R") + " palmTurn=" + turn.ToString("R"));
        TestContext.WriteLine("ShoulderShift=" + Vector3.Distance(shoulderBefore, shoulder.position).ToString("R") + " torsoTurns=" + string.Join(",", torso.Select((t, i) => Quaternion.Angle(torsoBefore[i], t.localRotation).ToString("R"))));
        Vector3 target = root.TransformPoint(new Vector3(-.38f, 1.85f, .35f));
        TestContext.WriteLine("Reach=" + (upperLength + lowerLength).ToString("R") + " targetDistanceBefore=" + Vector3.Distance(shoulderBefore, target).ToString("R") + " targetDistanceAfter=" + Vector3.Distance(shoulder.position, target).ToString("R"));
        System.IO.File.WriteAllText(EvidenceDirectory + "/free-hand-contact.txt", "wristJumpMetres=" + jump.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + "\npalmTurnDegrees=" + turn.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
        Assert.That(Time.time, Is.EqualTo(instant));
        Assert.That((float)type.GetProperty("Energy").GetValue(court), Is.LessThan(energy), "Tap must hit now");
        Assert.That(root.position, Is.EqualTo(position));
        var grip = model.Find(Chest + "/RightShoulder/RightArm/RightForeArm/RightHand/RacketSocket");
        Assert.That(Vector3.Distance(grip.Find("StringBed").position, shuttle.position), Is.LessThan(.05f));
        Assert.That(Vector3.Distance(grip.position, grip.parent.Find("RightHandProp").position), Is.LessThan(.015f));
        Assert.That(Vector3.Distance(shoulder.position, elbow.position), Is.EqualTo(upperLength).Within(.001f));
        Assert.That(Vector3.Distance(elbow.position, hand.position), Is.EqualTo(lowerLength).Within(.001f));
        Assert.That(jump, Is.LessThan(.01f), "Free hand teleports when immediate airborne hit starts");
    }
    [UnityTest] public IEnumerator AirborneFreeHandSeamCoversBothSidesAndReconstructsWithoutHistory()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("ConfigureDiscipline").Invoke(court, new object[] { true }); ((Behaviour)court).enabled = false;
        int cases = 0; float maximum = 0;
        foreach (string name in new[] { "Player", "Opponent", "Partner", "OpponentPartner" })
        foreach (float yaw in new[] { 0f, 180f })
        foreach (float height in new[] { .05f, .3f, .6f })
        foreach (string shot in new[] { "Smash", "Clear", "Drop" })
        foreach (float side in new[] { -.4f, .4f })
        {
            var root = GameObject.Find(name).transform; root.rotation = Quaternion.Euler(0, yaw, 0);
            root.position = new Vector3(0, height, -4);
            var model = root.Find("AthleteVisual"); var driver = model.GetComponents<MonoBehaviour>().Single(t => t.GetType().Name == "AthleteVisual");
            var present = driver.GetType().GetMethod("Present");
            var hand = model.Find(Chest + "/LeftShoulder/LeftArm/LeftForeArm/LeftHand");
            var grip = model.Find(Chest + "/RightShoulder/RightArm/RightForeArm/RightHand/RacketSocket");
            Vector3 contact = root.TransformPoint(new Vector3(side, 2.1f, .35f));
            driver.GetType().GetMethod("SetTravel").Invoke(driver, new object[] { .7f });
            present.Invoke(driver, new object[] { "Clear", 10f, contact, Vector3.zero, height, 0f });
            Vector3 before = hand.position, position = root.position; Quaternion rotation = hand.rotation;
            present.Invoke(driver, new object[] { shot, 0f, contact, Vector3.zero, height, 0f });
            float gap = Vector3.Distance(before, hand.position); maximum = Mathf.Max(maximum, gap); cases++;
            TestContext.WriteLine(name + " yaw=" + yaw + " height=" + height + " " + shot + " side=" + side + " freeGap=" + gap.ToString("R"));
            Assert.That(gap, Is.LessThan(.01f)); Assert.That(Quaternion.Angle(rotation, hand.rotation), Is.LessThan(1f));
            Assert.That(Vector3.Distance(grip.Find("StringBed").position, contact), Is.LessThan(.05f));
            foreach (float elapsed in new[] { 0f, .001f, .01f, .033f, .10f, .18f, .649f, .651f })
            {
                present.Invoke(driver, new object[] { shot, elapsed, contact, Vector3.zero, height, 0f });
                Vector3 saved = hand.position; Quaternion savedRotation = hand.rotation;
                present.Invoke(driver, new object[] { "Net", .2f, root.TransformPoint(new Vector3(.4f, 1f, .35f)), Vector3.zero, 0f, 0f });
                present.Invoke(driver, new object[] { shot, elapsed, contact, Vector3.zero, height, 0f });
                Assert.That(Vector3.Distance(saved, hand.position), Is.LessThan(.0001f), "Pose depends on unrecorded history");
                Assert.That(Quaternion.Angle(savedRotation, hand.rotation), Is.LessThan(.05f));
                Assert.That(root.position, Is.EqualTo(position));
                Assert.That(Vector3.Distance(grip.position, grip.parent.Find("RightHandProp").position), Is.LessThan(.015f));
            }
        }
        TestContext.WriteLine("Free-hand cases=" + cases + " maximumGap=" + maximum.ToString("R"));
    }
    [UnityTest] public IEnumerator AirbornePreparationLoadsTorsoAndFreeArmBeforeImmediateContact()
    {
        LogAssert.Expect(LogType.Log,"[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("JustSmash.CourtPrototype")).First(t=>t!=null);
        var court=UnityEngine.Object.FindFirstObjectByType(type); ((Behaviour)court).enabled=false;
        type.GetMethod("EnterTraining").Invoke(court,null);
        var root=GameObject.Find("Player").transform; var model=root.Find("AthleteVisual");
        var chest=model.Find(Chest); var hand=model.Find(Chest+"/LeftShoulder/LeftArm/LeftForeArm/LeftHand");
        var animate=type.GetMethod("AnimateAthlete",BindingFlags.Instance|BindingFlags.NonPublic);
        animate.Invoke(court,null); var ready=chest.localRotation;
        type.GetMethod("Jump").Invoke(court,null);
        for(int i=0;i<15;i++) type.GetMethod("FixedUpdate",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(court,null);
        animate.Invoke(court,null);
        Assert.That(Quaternion.Angle(ready,chest.localRotation),Is.GreaterThan(15),"Airborne player retains rigid ready torso");
        Assert.That(root.InverseTransformPoint(hand.position).y,Is.GreaterThan(1.6f),"Free arm must counterbalance overhead preparation");
    }
    [UnityTest] public IEnumerator NetTouchAndLiftHaveDistinctFollowThroughWithoutChangingContact()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var ct = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(ct); ct.GetMethod("ConfigureDiscipline").Invoke(court, new object[] { true }); ((Behaviour)court).enabled = false;
        var errors = new System.Collections.Generic.List<string>();
        var rows = new System.Collections.Generic.List<string> { "actor,yaw,side,netTravelMetres,liftRiseMetres,separationMetres" };
        foreach (string name in new[] { "Player", "Opponent", "Partner", "OpponentPartner" })
        foreach (float yaw in new[] { 0f, 180f }) foreach (float side in new[] { -.4f, .4f })
        {
            var root = GameObject.Find(name).transform; root.rotation = Quaternion.Euler(0, yaw, 0);
            var model = root.Find("AthleteVisual"); var driver = model.GetComponents<MonoBehaviour>().Single(t => t.GetType().Name == "AthleteVisual");
            var present = driver.GetType().GetMethod("Present"); var bed = (Transform)driver.GetType().GetProperty("StringBed").GetValue(driver);
            var arm = model.Find(Chest + "/RightShoulder/RightArm"); var elbow = arm.Find("RightForeArm"); var hand = elbow.Find("RightHand");
            float upperLength = Vector3.Distance(arm.position, elbow.position), lowerLength = Vector3.Distance(elbow.position, hand.position);
            Vector3 contact = root.TransformPoint(new Vector3(side, 1, .35f)), position = root.position;
            var follows = new Vector3[2];
            for (int shot = 0; shot < 2; shot++)
            {
                string stroke = shot == 0 ? "Net" : "Lift";
                void Pose(float elapsed) => present.Invoke(driver, new object[] { stroke, elapsed, contact, Vector3.zero, 0f, 0f });
                for (int i = 0; i < 24; i++) present.Invoke(driver, new object[] { "Clear", 10f, Vector3.zero, Vector3.zero, 0f, 0f });
                Pose(0); Assert.That(Vector3.Distance(bed.position, contact), Is.LessThan(.001f));
                Vector3 previous = bed.position;
                for (int frame = 1; frame <= 40; frame++)
                {
                    Pose(frame / 60f);
                    Assert.That(Vector3.Distance(previous, bed.position), Is.LessThan(.10f), "Racket seam " + stroke); previous = bed.position;
                    Assert.That(Vector3.Distance(arm.position, elbow.position), Is.EqualTo(upperLength).Within(.001f));
                    Assert.That(Vector3.Distance(elbow.position, hand.position), Is.EqualTo(lowerLength).Within(.001f));
                    Assert.That(root.position, Is.EqualTo(position));
                }
                Pose(.18f); follows[shot] = bed.position;
                Vector3 saved = bed.position;
                present.Invoke(driver, new object[] { "Smash", .18f, root.TransformPoint(new Vector3(-side, 2.1f, .35f)), Vector3.zero, 0f, 0f });
                Pose(.18f); Assert.That(Vector3.Distance(saved, bed.position), Is.LessThan(.001f), "Follow-through reconstruction");
            }
            float netTravel = Vector3.Distance(contact, follows[0]), liftRise = Vector3.Dot(follows[1] - contact, root.up), separation = Vector3.Distance(follows[0], follows[1]);
            rows.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2:R},{3:R},{4:R},{5:R}", name, yaw, side, netTravel, liftRise, separation));
            if (netTravel > .18f || liftRise < .4f || separation < .25f) errors.Add(name + "/" + yaw + "/" + side + " net=" + netTravel + " liftRise=" + liftRise + " separation=" + separation);
        }
        System.IO.File.WriteAllLines(EvidenceDirectory + "/net-lift-follow.csv", rows);
        Assert.That(rows.Count - 1, Is.EqualTo(16)); Assert.That(errors, Is.Empty, string.Join("; ", errors));
    }
    [UnityTest] public IEnumerator SmashFollowsAcrossBodyBelowShoulderBeforeReadyRecovery()
    {
        LogAssert.Expect(LogType.Log,"[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var root=GameObject.Find("Player").transform; var model=root.Find("AthleteVisual");
        var driver=model.GetComponents<MonoBehaviour>().Single(t=>t.GetType().Name=="AthleteVisual");
        var present=driver.GetType().GetMethod("Present");
        foreach(float side in new[] {-.4f,.4f})
        {
            Vector3 contact=root.TransformPoint(new Vector3(side,2.1f,.35f));
            present.Invoke(driver,new object[]{"Smash",.18f,contact,Vector3.zero,0f,0f});
            Vector3 bed=root.InverseTransformPoint(((Transform)driver.GetType().GetProperty("StringBed").GetValue(driver)).position);
            Assert.That(bed.y,Is.LessThan(1.4f),"Smash holds racket overhead instead of following down");
            Assert.That(bed.x*Mathf.Sign(side),Is.LessThan(-.15f),"Racket does not cross body after smash");
        }
    }
    [UnityTest] public IEnumerator VisibleContactUsesFixedBoneLengthsForBothSidesAndStrokes()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var driverType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.AthleteVisual")).First(t => t != null);
        var method = driverType.GetMethod("Present");
        Assert.That(method, Is.Not.Null, "Visible animation driver missing");
        var courtType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(courtType);
        courtType.GetMethod("ConfigureDiscipline").Invoke(court, new object[] { true }); ((Behaviour)court).enabled = false;
        foreach (string name in new[] { "Player", "Opponent", "Partner", "OpponentPartner" })
        {
            var root = GameObject.Find(name).transform;
            var model = root.Find("AthleteVisual"); var driver = model.GetComponent(driverType);
            var upper = model.Find(Chest + "/RightShoulder/RightArm"); var lower = upper.Find("RightForeArm"); var hand = lower.Find("RightHand");
            float a = Vector3.Distance(upper.position, lower.position), b = Vector3.Distance(lower.position, hand.position);
            foreach (float yaw in new[] { 0f, 180f })
            foreach (string shot in new[] { "Smash", "Clear", "Drop", "Net", "Lift", "Serve" })
            foreach (float side in new[] { -.4f, .4f })
            {
                root.rotation = Quaternion.Euler(0, yaw, 0);
                var position = root.position;
                Vector3 contact = root.TransformPoint(new Vector3(side, shot == "Net" || shot == "Lift" || shot == "Serve" ? 1f : 2.1f, .35f));
                method.Invoke(driver, new object[] { shot, 0f, contact, Vector3.zero, 0f, 0f });
                Transform bed = hand.Find("RacketSocket/StringBed");
                Assert.That(Vector3.Distance(bed.position, contact), Is.LessThan(.05f), name + " " + shot + " " + side);
                Assert.That(Vector3.Distance(upper.position, lower.position), Is.EqualTo(a).Within(.001f));
                Assert.That(Vector3.Distance(lower.position, hand.position), Is.EqualTo(b).Within(.001f));
                Assert.That(root.position, Is.EqualTo(position));
                foreach (float elapsed in new[] { .10f, .18f, .44f, .649f, .651f, 1f })
                {
                    method.Invoke(driver, new object[] { shot, elapsed, contact, Vector3.zero, 0f, 0f });
                    Assert.That(Vector3.Distance(upper.position, lower.position), Is.EqualTo(a).Within(.001f));
                    Assert.That(Vector3.Distance(lower.position, hand.position), Is.EqualTo(b).Within(.001f));
                    Assert.That(root.position, Is.EqualTo(position));
                    Assert.That(Vector3.Distance(hand.Find("RacketSocket").position, hand.Find("RightHandProp").position), Is.LessThan(.015f));
                }
            }
        }
    }
    [UnityTest] public IEnumerator FootworkUsesLocalDirectionsAndResetsPlantsOnNewPoint()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var driverType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.AthleteVisual")).First(t => t != null);
        var present = driverType.GetMethod("Present");
        var root = GameObject.Find("Player").transform;
        var model = root.Find("AthleteVisual"); var driver = model.GetComponent(driverType);
        foreach (float end in new[] { 1f, -1f })
        {
            root.rotation = Quaternion.Euler(0, end > 0 ? 0 : 180, 0);
            root.position = new Vector3(.8f, 0, -4 * end);
            present.Invoke(driver, new object[] { "Clear", 10f, Vector3.zero, Vector3.zero, 0f, 0f });
            foreach (string side in new[] { "Left", "Right" })
            {
                var foot = model.Find(Hip + "/" + side + "UpLeg/" + side + "Leg/" + side + "Foot");
                Assert.That(foot.position.y, Is.EqualTo(.13f).Within(.015f), "New point must not leave old world-space foot plants: " + side + " end=" + end);
                Assert.That(Vector3.Distance(foot.position, root.position), Is.LessThan(.5f));
            }
            present.Invoke(driver, new object[] { "Clear", 10f, Vector3.zero, root.forward * .05f, 0f, 0f });
            Assert.That(driverType.GetProperty("CurrentClip").GetValue(driver), Is.EqualTo("MoveForward"), "Animation direction must use athlete-local axes on both ends");
        }
    }
    [UnityTest] public IEnumerator GroundedFeetKeepStanceAndContactShadowStaysOnFloorDuringJump()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type); ((Behaviour)court).enabled = false;
        type.GetMethod("EnterTraining").Invoke(court, null);
        var root = GameObject.Find("Player").transform; var model = root.Find("AthleteVisual");
        var driverType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.AthleteVisual")).First(t => t != null);
        var driver = model.GetComponent(driverType); var present = driverType.GetMethod("Present");
        var left = model.Find(Hip + "/LeftUpLeg/LeftLeg/LeftFoot");
        present.Invoke(driver, new object[] { "Clear", 10f, Vector3.zero, Vector3.zero, 0f, 0f });
        Vector3 plant = left.position;
        for (int i = 0; i < 6; i++)
        {
            Vector3 step = Vector3.right * .025f; root.position += step;
            present.Invoke(driver, new object[] { "Clear", 10f, Vector3.zero, step, 0f, 0f });
            Assert.That(Vector3.Distance(left.position, plant), Is.LessThan(.04f), "Stance foot must not skate with athlete root");
            Assert.That(left.position.y, Is.EqualTo(.13f).Within(.015f));
        }
        type.GetMethod("Jump").Invoke(court, null);
        var tick = type.GetMethod("FixedUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
        for (int i = 0; i < 12; i++) tick.Invoke(court, null);
        type.GetMethod("AnimateAthlete", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(court, null);
        Assert.That(root.position.y, Is.GreaterThan(.4f));
        Assert.That(root.Find("ContactShadow").position.y, Is.EqualTo(.004f).Within(.001f), "Contact shadow must not float with airborne root");
    }
    [UnityTest] public IEnumerator EveryVisibleSkinUsesTheDrivenSkeletonDuringReactions()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type);
        type.GetMethod("ConfigureDiscipline").Invoke(court, new object[] { true });
        ((Behaviour)court).enabled = false;
        foreach (string name in new[] { "Player", "Opponent", "Partner", "OpponentPartner" })
        {
            var model = GameObject.Find(name).transform.Find("AthleteVisual");
            var skull = model.Find(Chest + "/Neck/Head");
            var skin = model.GetComponentsInChildren<SkinnedMeshRenderer>().First(s => s.name.Contains("head2"));
            var headBone = skin.bones.First(b => b.name == "Head");
            Assert.That(headBone, Is.SameAs(skull), name + " face and hair use different Head skeletons");
            var baked = new Mesh(); skin.BakeMesh(baked);
            int top = 0; var vertices = baked.vertices;
            for (int i = 1; i < vertices.Length; i++) if (vertices[i].y > vertices[top].y) top = i;
            Vector3 rest = skull.InverseTransformPoint(skin.transform.TransformPoint(vertices[top]));
            foreach (float tilt in new[] { -8f, 14f, 25f })
            {
                var rotation = skull.localRotation; skull.localRotation *= Quaternion.Euler(tilt, 0, 0);
                skin.BakeMesh(baked);
                Assert.That(Vector3.Distance(skull.InverseTransformPoint(skin.transform.TransformPoint(baked.vertices[top])), rest), Is.LessThan(.005f), name + " visible scalp does not follow driven head");
                skull.localRotation = rotation;
            }
            foreach (var renderer in model.GetComponentsInChildren<SkinnedMeshRenderer>())
                foreach (var bone in renderer.bones)
                    Assert.That(bone.IsChildOf(model.Find("Root")) || bone == model.Find("Root"), Is.True, renderer.name + " retains unanimated duplicate rig");
            UnityEngine.Object.Destroy(baked);
        }
    }

    [UnityTest] public IEnumerator HairSkinFollowsHeadThroughBackhandAndUnderhand()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var driverType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.AthleteVisual")).First(t => t != null);
        foreach (string name in new[] { "Player", "Opponent" })
        {
            var root = GameObject.Find(name).transform; var model = root.Find("AthleteVisual");
            var skins = model.GetComponentsInChildren<SkinnedMeshRenderer>();
            var hair = skins.First(s => s.name.Contains("haircut")); var head = skins.First(s => s.name.Contains("head2"));
            var baked = new Mesh();
            driverType.GetMethod("Present").Invoke(model.GetComponent(driverType), new object[] { "Clear", 10f, Vector3.zero, Vector3.zero, 0f, 0f });
            var skull = model.Find(Chest + "/Neck/Head");
            hair.BakeMesh(baked); Vector3 hairRest = skull.InverseTransformPoint(hair.transform.TransformPoint(baked.bounds.center));
            foreach (string shot in new[] { "Smash", "Net", "Lift" }) foreach (float side in new[] { -.4f, .4f })
            {
                var contact = root.TransformPoint(new Vector3(side, shot == "Smash" ? 2.1f : 1, .35f));
                driverType.GetMethod("Present").Invoke(model.GetComponent(driverType), new object[] { shot, 0f, contact, Vector3.zero, 0f, 0f });
                hair.BakeMesh(baked); Vector3 hairCenter = hair.transform.TransformPoint(baked.bounds.center);
                head.BakeMesh(baked); Vector3 headCenter = head.transform.TransformPoint(baked.bounds.center);
                Assert.That(Vector3.Distance(skull.InverseTransformPoint(hairCenter), hairRest), Is.LessThan(.01f), name + " hair shifts relative to skull");
                Debug.Log("[hair] " + name + " " + shot + " hair=" + hairCenter + " head=" + headCenter);
                Assert.That(Vector3.Distance(hairCenter, headCenter), Is.LessThan(.23f), name + " " + shot + " " + side + " hair detached from head");
            }
            UnityEngine.Object.Destroy(baked);
        }
    }
    [UnityTest] public IEnumerator NetFaultUsesRacketGeometryNotRendererCullingBounds()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(type); ((Behaviour)court).enabled = false;
        var touch = type.GetMethod("TouchesNet", BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (string name in new[] { "Player", "Opponent" })
        {
            var root = GameObject.Find(name).transform;
            root.position = new Vector3(0, 0, name == "Player" ? -1.25f : 1.25f);
            var driver = root.Find("AthleteVisual").GetComponent(AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.AthleteVisual")).First(t => t != null));
            driver.GetType().GetMethod("Present").Invoke(driver, new object[] { "Clear", 10f, Vector3.zero, Vector3.zero, 0f, 0f });
            var bed = (Transform)driver.GetType().GetProperty("StringBed").GetValue(driver);
            foreach (var line in bed.GetComponentsInChildren<LineRenderer>())
            {
                for (int i = 0; i < line.positionCount; i++) Assert.That(Mathf.Abs(line.transform.TransformPoint(line.GetPosition(i)).z), Is.GreaterThan(.3f));
                line.bounds = new Bounds(Vector3.zero, Vector3.one * 10); // Culling bounds are allowed to overestimate visible geometry.
            }
            Assert.That((bool)touch.Invoke(court, new object[] { root, bed }), Is.False, name + " racket far from net");
            bed.position = new Vector3(0, 1.2f, 0);
            Assert.That((bool)touch.Invoke(court, new object[] { root, bed }), Is.True, name + " real racket contact must fault");
            root.position = new Vector3(0, 0, .05f);
            Assert.That((bool)touch.Invoke(court, new object[] { root, bed }), Is.True, name + " body contact must fault");
        }
    }
    [UnityTest] public IEnumerator JumpClockUsesFixedTicksAndLandingRecoversAtBothRenderRates()
    {
        foreach (int renderRate in new[] { 30, 60 })
        {
            LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
            yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
            var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
            var court = UnityEngine.Object.FindFirstObjectByType(type); ((Behaviour)court).enabled = false;
            type.GetMethod("EnterTraining").Invoke(court, null); type.GetMethod("Jump").Invoke(court, null);
            var tick = type.GetMethod("FixedUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
            var player = GameObject.Find("Player").transform; float apex = 0; int landed = 0;
            for (int i = 0; i < 50; i++)
            {
                tick.Invoke(court, null); apex = Mathf.Max(apex, player.position.y);
                if (i > 1 && landed == 0 && player.position.y <= .001f) landed = i + 1;
                if (i % (renderRate == 30 ? 2 : 1) == 0) type.GetMethod("AnimateAthlete", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(court, null);
            }
            Assert.That(apex, Is.EqualTo(.6125f).Within(.03f), "Jump must not depend on Update clock");
            Assert.That(landed, Is.InRange(34, 36)); Assert.That(player.position.y, Is.EqualTo(0));
            var visual = player.Find("AthleteVisual");
            Assert.That(visual.GetComponent<Animation>()["Land"], Is.Not.Null);
        }
    }
    [UnityTest] public IEnumerator VisibleHumanFacesNetAndHoldsRacketOnMainSkeleton()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var prefab = Resources.Load<GameObject>("AthleteNPC");
        var raw = UnityEngine.Object.Instantiate(prefab);
        foreach (string path in new[] { Hip, Chest, Chest + "/RightShoulder/RightArm", Chest + "/RightShoulder/RightArm/RightForeArm/RightHand", Hip + "/RightUpLeg/RightLeg/RightFoot" })
        {
            var b = raw.transform.Find(path);
            Debug.Log("[rig] " + path + " position=" + b.position.ToString("F4") + " rotation=" + b.rotation.ToString("F4"));
        }
        foreach (var s in raw.GetComponentsInChildren<SkinnedMeshRenderer>()) Debug.Log("[skin] " + s.name + " bounds=" + s.bounds + " bones=" + s.bones.Length);
        UnityEngine.Object.Destroy(raw);
        foreach (string name in new[] { "Player", "Opponent" })
        {
            var root = GameObject.Find(name).transform;
            var model = root.Find("AthleteVisual");
            Assert.That(Vector3.Dot(model.forward, root.forward), Is.GreaterThan(.99f), name + " face away from net");
            Transform hand = model.Find(Chest + "/RightShoulder/RightArm/RightForeArm/RightHand");
            var socket = hand.Find("RacketSocket");
            Assert.That(socket, Is.Not.Null, name + " racket not attached to actual palm");
            Assert.That(Vector3.Distance(socket.position, hand.position), Is.LessThan(.09f));
            Assert.That(socket.Find("StringBed"), Is.Not.Null);
            Assert.That(model.GetComponent<Animation>(), Is.Not.Null, "Native clip driver missing");
            Assert.That(root.GetComponentsInChildren<MeshRenderer>().Where(r => !r.transform.IsChildOf(model) && r.name != "ContactShadow").All(r => !r.enabled), Is.True, "Visible proxy mannequin/racket");
        }
    }
}
