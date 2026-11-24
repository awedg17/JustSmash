using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class FootworkWaveTests : PreferenceFixture
{
    const string Hip = "Root/Hips";

    [UnityTest] public IEnumerator LowContactLungeUsesLocalDirectionAndReconstructsWithoutHistory()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var courtType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(courtType);
        courtType.GetMethod("ConfigureDiscipline").Invoke(court, new object[] { true });
        ((Behaviour)court).enabled = false;
        int cases = 0;
        foreach (string name in new[] { "Player", "Opponent", "Partner", "OpponentPartner" })
        foreach (float yaw in new[] { 0f, 180f })
        foreach (string shot in new[] { "Net", "Lift", "Serve" })
        foreach (float side in new[] { -.4f, .4f })
        {
            var root = GameObject.Find(name).transform;
            root.SetPositionAndRotation(new Vector3(0, 0, yaw == 0 ? -4 : 4), Quaternion.Euler(0, yaw, 0));
            var model = root.Find("AthleteVisual");
            var driver = model.GetComponents<MonoBehaviour>().Single(t => t.GetType().Name == "AthleteVisual");
            var type = driver.GetType(); var present = type.GetMethod("Present");
            var bones = model.GetComponentsInChildren<Transform>().Where(t => t != model && t.parent != null).ToArray();
            var lengths = bones.Select(t => Vector3.Distance(t.position, t.parent.position)).ToArray();
            var feet = new[] { model.Find(Hip + "/LeftUpLeg/LeftLeg/LeftFoot"), model.Find(Hip + "/RightUpLeg/RightLeg/RightFoot") };
            var contact = root.TransformPoint(new Vector3(side, 1f, .35f));
            Assert.That((bool)type.GetMethod("CanReach").Invoke(driver, new object[] { contact, shot }), Is.True, "Fixture must use legal reach");
            var position = root.position; var rotation = root.rotation; var scale = root.localScale;
            type.GetMethod("SetTravel").Invoke(driver, new object[] { .7f });
            for (int i = 0; i < 24; i++) present.Invoke(driver, new object[] { "Clear", 10f, Vector3.zero, Vector3.zero, 0f, 0f });
            var settled = feet.Select(t => t.position).ToArray();
            present.Invoke(driver, new object[] { shot, 0f, contact, Vector3.zero, 0f, 0f });
            for (int i = 0; i < feet.Length; i++)
                Assert.That(Vector3.Distance(settled[i], feet[i].position), Is.LessThan(.01f), "Contact must not teleport foot into lunge");
            var bed = (Transform)type.GetProperty("StringBed").GetValue(driver);
            Assert.That(Vector3.Distance(bed.position, contact), Is.LessThan(.05f), "Immediate stringbed contact changed");
            var previous = feet.Select(t => t.position).ToArray();
            for (int frame = 1; frame <= 39; frame++)
            {
                present.Invoke(driver, new object[] { shot, frame / 60f, contact, Vector3.zero, 0f, 0f });
                for (int i = 0; i < feet.Length; i++)
                {
                    Assert.That(Vector3.Distance(previous[i], feet[i].position), Is.LessThan(.07f), "Foot discontinuity in 60Hz pose samples");
                    previous[i] = feet[i].position;
                }
            }
            present.Invoke(driver, new object[] { shot, .18f, contact, Vector3.zero, 0f, 0f });
            int lead = side < 0 ? 0 : 1;
            var local = root.InverseTransformPoint(feet[lead].position);
            Assert.That(local.x * Mathf.Sign(side), Is.GreaterThan(.30f), "Low contact must extend lead foot toward local contact, not fixed forward");
            Assert.That(local.z, Is.GreaterThan(.10f), "Forward contact must retain forward reach");
            foreach (float elapsed in new[] { 0f, .06f, .18f, .44f, .649f })
            {
                present.Invoke(driver, new object[] { shot, elapsed, contact, Vector3.zero, 0f, 0f });
                var saved = bones.Select(t => t.localRotation).ToArray();
                var footPositions = feet.Select(t => t.position).ToArray();
                present.Invoke(driver, new object[] { "Net", .2f, root.TransformPoint(new Vector3(-side, 1f, .35f)), Vector3.zero, 0f, 0f });
                present.Invoke(driver, new object[] { "Clear", 10f, Vector3.zero, Vector3.zero, 0f, 0f });
                present.Invoke(driver, new object[] { shot, elapsed, contact, Vector3.zero, 0f, 0f });
                for (int i = 0; i < bones.Length; i++)
                {
                    Assert.That(Quaternion.Angle(saved[i], bones[i].localRotation), Is.LessThan(.05f), "Pose depends on unrelated pose: " + bones[i].name);
                    // Hips translate for crouch; every actual bone segment stays fixed.
                    if (bones[i].name != "Hips") Assert.That(Vector3.Distance(bones[i].position, bones[i].parent.position), Is.EqualTo(lengths[i]).Within(.001f), bones[i].name);
                }
                for (int i = 0; i < feet.Length; i++)
                    Assert.That(Vector3.Distance(footPositions[i], feet[i].position), Is.LessThan(.0001f), "Foot plant retains unrecorded history");
                Assert.That(root.position, Is.EqualTo(position));
                Assert.That(Quaternion.Angle(root.rotation, rotation), Is.LessThan(.001f));
                Assert.That(root.localScale, Is.EqualTo(scale));
                Assert.That((float)type.GetProperty("Travel").GetValue(driver), Is.EqualTo(.7f));
                Assert.That((int)type.GetMethod("TakeFootContacts").Invoke(driver, null), Is.Zero, "Stationary low pose must not emit gait steps");
            }
            cases++;
        }
        Assert.That(cases, Is.EqualTo(48));
        TestContext.WriteLine("Low-lunge cases=" + cases + "; four actors, both ends, three strokes, both sides");
    }

    [UnityTest] public IEnumerator MovingLowContactPreservesExistingFootPlantWithoutTeleport()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var ct = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(ct); ((Behaviour)court).enabled = false;
        var root = GameObject.Find("Player").transform; var model = root.Find("AthleteVisual");
        var driver = model.GetComponents<MonoBehaviour>().Single(t => t.GetType().Name == "AthleteVisual");
        var type = driver.GetType(); var present = type.GetMethod("Present");
        var feet = new[] { model.Find(Hip + "/LeftUpLeg/LeftLeg/LeftFoot"), model.Find(Hip + "/RightUpLeg/RightLeg/RightFoot") };
        var failures = new System.Collections.Generic.List<string>(); float maximum = 0; int cases = 0;
        foreach (float yaw in new[] { 0f, 180f }) foreach (float side in new[] { -.4f, .4f })
        foreach (var direction in new[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right })
        foreach (int ticks in new[] { 3, 8, 13 })
        {
            root.SetPositionAndRotation(new Vector3(0, 0, -4), Quaternion.Euler(0, yaw, 0));
            type.GetMethod("SetTravel").Invoke(driver, new object[] { 0f });
            for (int i = 0; i < 24; i++) present.Invoke(driver, new object[] { "Clear", 10f, Vector3.zero, Vector3.zero, 0f, 0f });
            var step = root.TransformDirection(direction) * .075f;
            for (int i = 0; i < ticks; i++)
            {
                root.position += step;
                present.Invoke(driver, new object[] { "Clear", 10f, Vector3.zero, step, 0f, 0f });
            }
            var before = feet.Select(t => t.position).ToArray(); var position = root.position;
            var contact = root.TransformPoint(new Vector3(side, 1, .35f));
            present.Invoke(driver, new object[] { "Net", 0f, contact, Vector3.zero, 0f, 0f });
            Assert.That(root.position, Is.EqualTo(position));
            for (int i = 0; i < 2; i++)
            {
                float delta = Vector3.Distance(before[i], feet[i].position); maximum = Mathf.Max(maximum, delta);
                if (delta > .081f) failures.Add(yaw + "/" + side + "/" + direction + "/" + ticks + "/" + i + "=" + delta);
            }
            cases++;
        }
        TestContext.WriteLine("Moving contact cases=" + cases + "; max foot step=" + maximum.ToString("R"));
        Assert.That(cases, Is.EqualTo(48));
        Assert.That(failures, Is.Empty, "Moving-foot gate .081m exceeded (baseline also fails): " + string.Join("; ", failures));
    }
#if UNITY_EDITOR
    // Controlled pose sequence, not natural gameplay or motion acceptance.
    [UnityTest, Category("Capture"), Timeout(30000)]
    public IEnumerator CaptureControlledLowLungeAt1x()
    {
        Assert.That(Time.captureFramerate, Is.Zero, "Use natural frame timing");
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        var courtType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        var court = UnityEngine.Object.FindFirstObjectByType(courtType);
        ((Behaviour)court).enabled = false;
        var root = GameObject.Find("Player").transform;
        root.SetPositionAndRotation(new Vector3(0, 0, -4), Quaternion.identity);
        var model = root.Find("AthleteVisual");
        var driver = model.GetComponents<MonoBehaviour>().Single(t => t.GetType().Name == "AthleteVisual");
        var type = driver.GetType(); var present = type.GetMethod("Present");
        var feet = new[] { model.Find(Hip + "/LeftUpLeg/LeftLeg/LeftFoot"), model.Find(Hip + "/RightUpLeg/RightLeg/RightFoot") };
        var camera = Camera.main; var position = camera.transform.position; var rotation = camera.transform.rotation;
        var fov = camera.fieldOfView; var aspect = camera.aspect; var old = camera.targetTexture;
        var active = RenderTexture.active; var speed = Time.timeScale;
        var target = RenderTexture.GetTemporary(1280, 720, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default, Mathf.Max(1, QualitySettings.antiAliasing));
        var image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
        var clock = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            Time.timeScale = 1;
            camera.transform.position = new Vector3(0, 1.35f, -1);
            camera.transform.LookAt(new Vector3(0, 1.1f, -4));
            camera.fieldOfView = 50; camera.aspect = 1280f / 720; camera.targetTexture = target;
            System.IO.File.WriteAllText(EvidenceDirectory + "/camera.json", "{\"position\":" + JsonUtility.ToJson(camera.transform.position) + ",\"rotation\":" + JsonUtility.ToJson(camera.transform.rotation) + ",\"fov\":50,\"width\":1280,\"height\":720,\"msaa\":" + target.antiAliasing + ",\"controlled\":true}");
            using (var hash = System.Security.Cryptography.SHA256.Create())
                System.IO.File.WriteAllText(EvidenceDirectory + "/source-sha256.txt", string.Join("\n", new[] { "Scripts/AthleteVisual.cs", "Tests/PlayMode/FootworkWaveTests.cs", "Resources/AthleteNPC.prefab" }.Select(p => p + " " + BitConverter.ToString(hash.ComputeHash(System.IO.File.ReadAllBytes(Application.dataPath + "/" + p))).Replace("-", "").ToLowerInvariant())));
            using (var csv = new System.IO.StreamWriter(EvidenceDirectory + "/frames.csv"))
            {
                csv.AutoFlush = true;
                csv.WriteLine("side,frame,pose_elapsed_s,game_s,wall_s,delta_s,left_x,left_y,left_z,right_x,right_y,right_z,left_step_m,right_step_m,png");
                foreach (float side in new[] { -.4f, .4f })
                {
                    var contact = root.TransformPoint(new Vector3(side, 1, .35f));
                    Assert.That((bool)type.GetMethod("CanReach").Invoke(driver, new object[] { contact, "Net" }), Is.True);
                    type.GetMethod("SetTravel").Invoke(driver, new object[] { .7f });
                    for (int i = 0; i < 24; i++) present.Invoke(driver, new object[] { "Clear", 10f, Vector3.zero, Vector3.zero, 0f, 0f });
                    var previous = feet.Select(f => f.position).ToArray();
                    float elapsed = -1; int frame = 0;
                    while (true)
                    {
                        if (elapsed >= 0) present.Invoke(driver, new object[] { "Net", elapsed, contact, Vector3.zero, 0f, 0f });
                        camera.Render(); RenderTexture.active = target;
                        image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); image.Apply();
                        string file = (side < 0 ? "left-" : "right-") + frame.ToString("D4") + ".png";
                        System.IO.File.WriteAllBytes(EvidenceDirectory + "/" + file, image.EncodeToPNG());
                        var l = feet[0].position; var r = feet[1].position;
                        csv.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2:R},{3:R},{4:R},{5:R},{6:R},{7:R},{8:R},{9:R},{10:R},{11:R},{12:R},{13:R},{14}", side, frame++, elapsed, Time.timeAsDouble, clock.Elapsed.TotalSeconds, Time.deltaTime, l.x, l.y, l.z, r.x, r.y, r.z, Vector3.Distance(l, previous[0]), Vector3.Distance(r, previous[1]), file));
                        previous[0] = l; previous[1] = r;
                        if (elapsed >= .65f) break;
                        yield return null;
                        elapsed = elapsed < 0 ? 0 : Mathf.Min(.65f, elapsed + Time.deltaTime);
                    }
                }
            }
            TestContext.WriteLine("Controlled still sequence with measured frame gaps; not gameplay acceptance: " + EvidenceDirectory);
        }
        finally
        {
            camera.transform.SetPositionAndRotation(position, rotation); camera.fieldOfView = fov; camera.aspect = aspect;
            camera.targetTexture = old; RenderTexture.active = active; Time.timeScale = speed;
            RenderTexture.ReleaseTemporary(target); UnityEngine.Object.Destroy(image);
        }
    }
#endif
}
