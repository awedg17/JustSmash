using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class AiContactWaveTests : PreferenceFixture
{
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    Type type; UnityEngine.Object court;
    object Get(string name) => type.GetField(name, Hidden).GetValue(court);
    void Set(string name, object value) => type.GetField(name, Hidden).SetValue(court, value);
    object Call(string name, params object[] args) => type.GetMethod(name, Hidden | BindingFlags.Public).Invoke(court, args);
    Transform Actor(int id) => (Transform)Call("Actor", id);
    object Visual(int id) => Call("Visual", id);
    object Property(object value, string name) => value.GetType().GetProperty(name).GetValue(value);
    Transform Bed(object visual) => (Transform)Property(visual, "StringBed");
    void Ready(object visual) => visual.GetType().GetMethod("Present").Invoke(visual,
        new object[] { "Clear", 10f, Vector3.zero, Vector3.zero, 0f, 0f });
    IEnumerator Boot()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = UnityEngine.Object.FindFirstObjectByType(type); ((Behaviour)court).enabled = false;
    }
    void Setup(bool doubles, bool swapped, int server)
    {
        Set("doublesSelected", doubles); Call("ConfigureOpponent", "Easy", 193); Call("StartMatch");
        var match = Property(court, "Match"); var mt = match.GetType();
        mt.GetField("EndsSwapped").SetValue(match, swapped); mt.GetField("Server").SetValue(match, server % 2);
        ((int[])mt.GetField("RightPlayers").GetValue(match))[server % 2] = server;
        Call("NextPoint"); Set("flowStage", null); Set("serveReadyAt", Time.time - 1);
        Array.Clear((Vector3[])Get("athleteVelocity"), 0, 4);
    }
    void ReturnSetup(bool doubles, bool swapped, int id, bool rejected)
    {
        Setup(doubles, swapped, 0); Call("Serve", true);
        Set("serviceFlight", false); Set("lastHitter", 1 - id % 2); Set("lastPlayer", 1 - id % 2);
        Set("aiContactAt", Time.time - 1);
        Set("observedSwing", Get("swingAt")); Set("opponentThinkAt", Time.time - 1);
        Set("opponentRefreshAt", float.MaxValue); Set("opponentTarget", Actor(1).position);
        Set("teamObserved", Get(id % 2 == 0 ? "aiContactAt" : "swingAt"));
        Set(id % 2 == 0 ? "nearOwner" : "farOwner", id);
        for (int i = 0; i < 4; i++)
        {
            ((double[])Get("teamThink"))[i] = 0; ((double[])Get("teamRefresh"))[i] = double.MaxValue;
            if (doubles || i < 2) ((Vector3[])Get("teamTargets"))[i] = Actor(i).position;
        }
        ((Transform)Get("shuttle")).position = Actor(id).TransformPoint(new Vector3(rejected ? 4 : .3f, 1.1f, .35f));
        Set("velocity", Vector3.down); Ready(Visual(id));
    }
    void AssertImmediate(int id, Vector3 root, Quaternion rotation, float travel, Vector3 contact, string label)
    {
        Assert.That((int)Get("lastHitter"), Is.EqualTo(id % 2), label + " legal hit not committed");
        Assert.That(Actor(id).position, Is.EqualTo(root), label + " root moved at contact");
        Assert.That(Quaternion.Angle(Actor(id).rotation, rotation), Is.LessThan(.001f), label);
        Assert.That((float)Property(Visual(id), "Travel"), Is.EqualTo(travel), label + " counted displacement twice");
        Assert.That(Vector3.Distance(Bed(Visual(id)).position, contact), Is.LessThan(.035f),
            label + " stale stringbed before LateUpdate");
    }
    [UnityTest] public IEnumerator LegalReturnsPresentBeforeRenderAtBothEndsForEveryBot()
    {
        yield return Boot();
        foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
        for (int id = 1; id <= (doubles ? 3 : 1); id++)
        {
            ReturnSetup(doubles, swapped, id, false);
            var visual = Visual(id); var root = Actor(id).position; var rotation = Actor(id).rotation;
            var contact = ((Transform)Get("shuttle")).position; float travel = (float)Property(visual, "Travel");
            Assert.That((bool)visual.GetType().GetMethod("CanReach").Invoke(visual, new object[] { contact, "Lift" }), Is.True);
            Assert.That(Vector3.Distance(Bed(visual).position, contact), Is.GreaterThan(.1f), "fixture must start stale");
            int audioBefore = (int)Property(court, "SoundEventCount");
            Call("UpdateOpponent"); // Real singles/doubles return entrypoint; no render update or yield.
            Assert.That(Property(court, "SoundEventCount"), Is.EqualTo(audioBefore + 1));
            Assert.That(Property(court, "LastSoundClip"), Is.SameAs(Resources.Load<AudioClip>("Audio/racket-2")));
            Assert.That((float)Property(court, "LastSoundGain"), Is.EqualTo(.55f));
            AssertImmediate(id, root, rotation, travel, contact, doubles + "/" + swapped + "/" + id);
            Call("UpdateOpponent");
            Assert.That(Property(court, "SoundEventCount"), Is.EqualTo(audioBefore + 1), "Same contact replayed audio");
        }
    }
    [UnityTest] public IEnumerator LegalServesPresentBeforeRenderAtBothEndsForEveryBot()
    {
        yield return Boot();
        foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
        for (int id = 1; id <= (doubles ? 3 : 1); id++)
        {
            Setup(doubles, swapped, id); var visual = Visual(id); Ready(visual);
            var root = Actor(id).position; var rotation = Actor(id).rotation;
            var contact = Actor(id).TransformPoint(new Vector3(.35f, 1, .45f));
            float travel = (float)Property(visual, "Travel"); var rng = Get("opponentRandom");
            Assert.That(Vector3.Distance(Bed(visual).position, contact), Is.GreaterThan(.1f), "fixture must start stale");
            int audioBefore = (int)Property(court, "SoundEventCount");
            Call("UpdateOpponent"); // Includes Serving guard and BotServe, not direct BotPose.
            Assert.That(Property(court, "SoundEventCount"), Is.EqualTo(audioBefore + 1));
            Assert.That(Property(court, "LastSoundClip"), Is.SameAs(Resources.Load<AudioClip>("Audio/racket-0")));
            Assert.That((float)Property(court, "LastSoundGain"), Is.EqualTo(.55f));
            Assert.That(Property(court, "MatchState"), Is.EqualTo("Rally"));
            Assert.That(Get("opponentRandom"), Is.EqualTo(rng));
            AssertImmediate(id, root, rotation, travel, contact, doubles + "/" + swapped + "/" + id);
            Call("UpdateOpponent");
            Assert.That(Property(court, "SoundEventCount"), Is.EqualTo(audioBefore + 1), "Serve audio duplicated");
        }
    }
    [UnityTest] public IEnumerator UnreachableReturnsDoNotPoseSpendEnergyOrConsumeShotRng()
    {
        yield return Boot();
        foreach (bool doubles in new[] { false, true }) foreach (bool swapped in new[] { false, true })
        for (int id = 1; id <= (doubles ? 3 : 1); id++)
        {
            ReturnSetup(doubles, swapped, id, true);
            var visual = Visual(id); var bed = Bed(visual).position; var rng = Get("opponentRandom");
            var energy = Call("AthleteEnergy", id); var hitter = Get("lastHitter"); var velocity = Get("velocity");
            Assert.That((bool)visual.GetType().GetMethod("CanReach").Invoke(visual,
                new object[] { ((Transform)Get("shuttle")).position, "Lift" }), Is.False);
            int audioBefore = (int)Property(court, "SoundEventCount");
            Call("UpdateOpponent");
            Assert.That(Property(court, "SoundEventCount"), Is.EqualTo(audioBefore), "Unreachable return emitted audio");
            Assert.That(Get("lastHitter"), Is.EqualTo(hitter)); Assert.That(Get("velocity"), Is.EqualTo(velocity));
            Assert.That(Get("opponentRandom"), Is.EqualTo(rng)); Assert.That(Call("AthleteEnergy", id), Is.EqualTo(energy));
            Assert.That(Bed(visual).position, Is.EqualTo(bed));
        }
    }
}
