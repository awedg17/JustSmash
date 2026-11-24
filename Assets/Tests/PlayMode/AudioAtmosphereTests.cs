using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class AudioAtmosphereTests : PreferenceFixture
{
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    Type type; Component court;
    object Call(string name, params object[] args) => type.GetMethod(name).Invoke(court, args);
    object HiddenCall(string name, params object[] args) => type.GetMethod(name, Hidden).Invoke(court, args);
    object Get(string name) => type.GetProperty(name).GetValue(court);
    void Set(string name, object value) => type.GetField(name, Hidden).SetValue(court, value);
    void Play(string kind, string technique = null, float gain = 1) => HiddenCall("PlaySound", kind, technique, gain);
    IEnumerator Boot()
    {
        LogAssert.Expect(LogType.Log, "[boot] config v1; scene=Boot");
        yield return SceneManager.LoadSceneAsync("Boot"); yield return null;
        type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("JustSmash.CourtPrototype")).First(t => t != null);
        court = (Component)UnityEngine.Object.FindFirstObjectByType(type);
        Call("EnterTraining");
        ((Behaviour)court).enabled = false;
    }
    [UnityTest] public IEnumerator NativeRoomAndExactRecordedMix()
    {
        yield return Boot();
        Play("racket", "Clear");
        Assert.That(court.GetComponent<AudioReverbFilter>(), Is.Not.Null, "Indoor native reverb missing");
        string[] techniques = { "Clear", "Smash", "Lift", "Net", "Drop" };
        int[] variants = { 0, 1, 2, 3, 3 };
        float[] gains = { .55f, .85f, .55f, .28f, .28f };
        for (int i = 0; i < techniques.Length; i++)
        {
            int before = (int)Get("SoundEventCount"); Play("racket", techniques[i]);
            Assert.That(Get("SoundEventCount"), Is.EqualTo(before + 1));
            Assert.That(Get("LastSoundClip"), Is.SameAs(Resources.Load<AudioClip>("Audio/racket-" + variants[i])));
            Assert.That((float)Get("LastSoundGain"), Is.EqualTo(gains[i]).Within(.0001f));
        }
        var room = court.GetComponent<AudioReverbFilter>();
        Assert.That(room, Is.Not.Null); Assert.That(room.reverbPreset, Is.EqualTo(AudioReverbPreset.User));
        Assert.That(room.decayTime, Is.EqualTo(.7f).Within(.001f));
        Assert.That(room.room, Is.EqualTo(-1800));
        for (int i = 0; i < 4; i++)
        {
            Play("step", null, .5f);
            Assert.That(Get("LastSoundClip"), Is.SameAs(Resources.Load<AudioClip>("Audio/step-" + i)));
            Assert.That((float)Get("LastSoundGain"), Is.EqualTo(.13f).Within(.0001f));
        }
    }
    [UnityTest] public IEnumerator PauseReplayHomeAndZeroVolumeRejectEventsAndMuteTail()
    {
        yield return Boot();
        foreach (string state in new[] { "pause", "replay", "home", "zero" })
        {
            Call("EnterTraining"); type.GetProperty("Volume").SetValue(court, .7f); Play("racket", "Smash");
            int before = (int)Get("SoundEventCount");
            if (state == "pause") Time.timeScale = 0;
            if (state == "replay") Set("replaying", true);
            if (state == "home")
            {
                var field = type.GetField("screen", Hidden); field.SetValue(court, Enum.Parse(field.FieldType, "Home"));
            }
            if (state == "zero") type.GetProperty("Volume").SetValue(court, 0f);
            HiddenCall("UpdateCourtAudio"); Play("score");
            Assert.That(Get("SoundEventCount"), Is.EqualTo(before), state);
            Assert.That(court.GetComponent<AudioSource>().mute, Is.True, state);
            Assert.That(court.GetComponent<AudioReverbFilter>().enabled, Is.False, state);
            Time.timeScale = 1; Set("replaying", false);
        }
    }
    [UnityTest] public IEnumerator RealTapMissCreatesNoEventAndLegalTapSelectsRecordingOnce()
    {
        yield return Boot(); Call("Feed", "Lob");
        var player = GameObject.Find("Player").transform;
        var shuttle = GameObject.Find("Shuttle").transform;
        Set("velocity", Vector3.zero);
        Vector3 root = player.position;
        float energy = (float)Get("Energy"); int before = (int)Get("SoundEventCount");
        shuttle.position = player.TransformPoint(new Vector3(3, 2.1f, .35f));
        Call("TapShot", "Smash");
        Assert.That(Get("SoundEventCount"), Is.EqualTo(before));
        Assert.That(Get("Energy"), Is.EqualTo(energy)); Assert.That(player.position, Is.EqualTo(root));
        shuttle.position = player.TransformPoint(new Vector3(.3f, 2.1f, .35f));
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[hit\] (Excellent|Good|Fair|Poor) \| Smash.*"));
        Call("TapShot", "Smash");
        Assert.That(Get("SoundEventCount"), Is.EqualTo(before + 1));
        Assert.That(Get("LastSoundClip"), Is.SameAs(Resources.Load<AudioClip>("Audio/racket-1")));
        Assert.That((float)Get("LastSoundGain"), Is.EqualTo(.85f).Within(.0001f));
    }
    [UnityTest] public IEnumerator PointCueIsCachedOriginalNonVoiceWithBoundedMix()
    {
        yield return Boot();
        int before = (int)Get("SoundEventCount"); Play("score");
        var clip = (AudioClip)Get("LastSoundClip");
        Assert.That(clip.name, Is.EqualTo("JustSmash-original-score"));
        Assert.That((float)Get("LastSoundGain"), Is.EqualTo(.25f));
        Play("score"); Assert.That(Get("LastSoundClip"), Is.SameAs(clip));
        Assert.That(Get("SoundEventCount"), Is.EqualTo(before + 2));
        HiddenCall("DestroySounds"); yield return null;
        Assert.That(clip == null, Is.True, "Owned generated clip leaked");
        Assert.That(Resources.Load<AudioClip>("Audio/racket-0"), Is.Not.Null, "Borrowed recording destroyed");
    }
}
