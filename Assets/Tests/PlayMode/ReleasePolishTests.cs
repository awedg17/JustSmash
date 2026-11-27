using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class ReleasePolishTests : PreferenceFixture
{
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    Type type; Component court, visual; Transform player, shuttle;
    object Call(string name, params object[] args) => type.GetMethod(name).Invoke(court, args);
    object Field(string name) => type.GetField(name, Hidden).GetValue(court);
    void Set(string name, object value) => type.GetField(name, Hidden).SetValue(court, value);
    object Prop(string name) => type.GetProperty(name).GetValue(court);
    object PrivateProp(string name) => type.GetProperty(name, Hidden)?.GetValue(court);
    IEnumerator Boot()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = (Component)UnityEngine.Object.FindFirstObjectByType(type); ((Behaviour)court).enabled = false;
        player = GameObject.Find("Player").transform; shuttle = GameObject.Find("Shuttle").transform;
        visual = player.Find("AthleteVisual").GetComponents<MonoBehaviour>().Single(t => t.GetType().Name == "AthleteVisual");
    }
    void Rally(bool swapped)
    {
        Call("EnterTraining"); Call("ConfigureDiscipline", false); Call("StartMatch");
        var match = Prop("Match"); match.GetType().GetField("EndsSwapped").SetValue(match, swapped);
        Call("NextPoint"); Set("flowStage", null); Call("Serve", true);
        Set("lastHitter", 1); Set("lastPlayer", 1); Set("serviceFlight", false);
        Set("velocity", Vector3.down); Set("lastAim", Vector2.zero);
    }
    bool Reach(string shot) => (bool)visual.GetType().GetMethod("CanReach").Invoke(visual, new object[] { shuttle.position, shot });
    void Expire(string shot)
    {
        var intent = Field("shotIntent");
        intent.GetType().GetMethod("Queue").Invoke(intent, new object[] { Array.IndexOf(new[] { "Smash", "Clear", "Drop", "Net", "Lift" }, shot), Vector2.zero, .5f, Mathf.Max(0, Time.time - .3f) });
        type.GetMethod("TryBufferedShot", Hidden).Invoke(court, null);
    }
    [UnityTest] public IEnumerator ContactReasonMatchesStrokeEligibilityAtBothEnds()
    {
        yield return Boot(); yield return new WaitForSeconds(.35f);
        var errors = new List<string>(); int cases = 0;
        foreach (bool swapped in new[] { false, true })
        foreach (string input in new[] { "release", "expired-buffer" })
        foreach (string shot in new[] { "Lift", "Net", "Smash" })
        foreach (bool far in new[] { false, true })
        {
            Rally(swapped); shuttle.position = player.TransformPoint(new Vector3(far ? 1.3f : .3f, far && shot == "Smash" ? 2.1f : 1f, .35f));
            bool reachable = Reach(shot); float instant = Time.time, energy = (float)Prop("Energy"); int sound = (int)Prop("SoundEventCount");
            string expected = far ? "Di luar jangkauan" : shot == "Smash" ? "Kok terlalu rendah untuk smash" : "";
            Assert.That(reachable, Is.EqualTo(!far && shot != "Smash"));
            if (input == "release")
            {
                Call("BeginShot", shot); LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\].*"));
                Assert.That(Call("ReleaseShot"), Is.EqualTo(reachable));
            }
            else Expire(shot);
            string actual = PrivateProp("HudNotice") as string;
            if (input == "expired-buffer" && reachable) expected = "Kontak tidak terjadi";
            if (actual != expected) errors.Add(swapped + "/" + input + "/" + shot + "/far=" + far + " expected=" + expected + " actual=" + (actual ?? "<missing HudNotice>"));
            Assert.That(Time.time, Is.EqualTo(instant));
            if (!reachable || input == "expired-buffer")
            {
                Assert.That(Prop("Energy"), Is.EqualTo(energy)); Assert.That(Prop("SoundEventCount"), Is.EqualTo(sound));
            }
            cases++;
        }
        Assert.That(cases, Is.EqualTo(24)); File.WriteAllLines(EvidenceDirectory + "/contact-errors.txt", errors);
        Assert.That(errors, Is.Empty, string.Join("\n", errors));
    }
    [UnityTest] public IEnumerator BufferHitsNowBeforeExpiryAndPointNoticeWins()
    {
        yield return Boot();
        foreach (bool swapped in new[] { false, true })
        {
            Rally(swapped); var legal = player.TransformPoint(new Vector3(.3f, 1f, .35f)); shuttle.position = legal + player.right * 3;
            Call("TapShot", "Lift"); Assert.That(Field("lastHitter"), Is.EqualTo(1));
            shuttle.position = legal; float instant = Time.time; Vector3 root = player.position;
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\].*Lift.*"));
            type.GetMethod("TryBufferedShot", Hidden).Invoke(court, null);
            Assert.That(Field("lastHitter"), Is.EqualTo(0)); Assert.That(Field("swingAt"), Is.EqualTo(instant));
            Assert.That(Time.time, Is.EqualTo(instant)); Assert.That(player.position, Is.EqualTo(root));
            Assert.That(PrivateProp("HudNotice"), Is.EqualTo(""));
            var bed = (Transform)visual.GetType().GetProperty("StringBed").GetValue(visual);
            Assert.That(Vector3.Distance(bed.position, legal), Is.LessThan(.035f));
            Set("feedback", "Poor | miss: Di luar jangkauan");
            var cause = type.GetField("pointCause", Hidden); cause.SetValue(court, Enum.Parse(cause.FieldType, "In"));
            Prop("Match").GetType().GetMethod("AwardPoint").Invoke(Prop("Match"), new object[] { 0 });
            type.GetProperty("MatchState").SetValue(court, "Point");
            Assert.That(PrivateProp("HudNotice"), Is.EqualTo(PrivateProp("PointNotice")));
            Assert.That(PrivateProp("HudNotice"), Is.EqualTo("Poin kamu\nKok masuk"));
        }
    }
}
