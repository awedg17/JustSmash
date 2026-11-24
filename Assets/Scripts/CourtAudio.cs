using UnityEngine;
namespace JustSmash
{
    public sealed partial class CourtPrototype
    {
        float volume = .7f;
        public float Volume
        {
            get => volume;
            set { if (!float.IsFinite(value) || value < 0 || value > 1) throw new System.ArgumentOutOfRangeException(nameof(value)); volume = value; if (soundSource != null) soundSource.volume = value; if (value == 0) StopCourtAudio(); }
        }
        AudioSource soundSource;
        AudioReverbFilter indoorReverb;
        readonly AudioClip[] racketRecordings = new AudioClip[4], stepRecordings = new AudioClip[4];
        readonly System.Collections.Generic.Dictionary<string, AudioClip> sounds = new System.Collections.Generic.Dictionary<string, AudioClip>();
        int footstepVariant;
        public int SoundEventCount { get; private set; }
        public AudioClip LastSoundClip { get; private set; }
        public float LastSoundGain { get; private set; }

        void InitializeCourtAudio()
        {
            if (soundSource != null) return;
            for (int i = 0; i < 4; i++)
            {
                racketRecordings[i] = Resources.Load<AudioClip>("Audio/racket-" + i);
                stepRecordings[i] = Resources.Load<AudioClip>("Audio/step-" + i);
                if (racketRecordings[i] == null || stepRecordings[i] == null)
                    throw new System.InvalidOperationException("Missing licensed court recording");
            }
            soundSource = gameObject.AddComponent<AudioSource>();
            soundSource.spatialBlend = 0; soundSource.playOnAwake = false; soundSource.volume = Volume;
            indoorReverb = gameObject.AddComponent<AudioReverbFilter>();
            // ponytail: one short native room; separate buses only after measured mix review.
            indoorReverb.reverbPreset = AudioReverbPreset.User;
            indoorReverb.dryLevel = 0; indoorReverb.room = -1800; indoorReverb.roomHF = -1200;
            indoorReverb.decayTime = .7f; indoorReverb.decayHFRatio = .65f;
            indoorReverb.reflectionsLevel = -2200; indoorReverb.reflectionsDelay = .018f;
            indoorReverb.reverbLevel = -1800; indoorReverb.reverbDelay = .028f;
            indoorReverb.diffusion = 85; indoorReverb.density = 80;

        }
        void UpdateCourtAudio()
        {
            if (!GameplayInputEnabled || replaying || Volume <= 0) StopCourtAudio();
        }
        void StopCourtAudio()
        {
            if (soundSource != null) { soundSource.Stop(); soundSource.mute = true; }
            if (indoorReverb != null) indoorReverb.enabled = false;
        }
        void OnDisable() => StopCourtAudio();
        public AudioClip CreateSound(string kind)
        {
            if (System.Array.IndexOf(new[] { "racket", "net", "step", "whistle", "score" }, kind) < 0) throw new System.ArgumentException("Unknown sound");
            if (kind == "step")
            {
                var recording = Resources.Load<AudioClip>("Audio/step-0");
                if (recording == null) throw new System.InvalidOperationException("Missing CC0 sneaker recording");
                return Instantiate(recording);
            }
            if (kind == "racket")
            {
                var recording = Resources.Load<AudioClip>("Audio/racket-0");
                if (recording == null) throw new System.InvalidOperationException("Missing CC0 racket recording");
                return Instantiate(recording);
            }
            const int rate = 22050;
            if (kind == "score")
            {
                // Original two-note scoreboard cue: not an announcer, crowd or physical impact.
                var data = new float[6615];
                for (int i = 0; i < data.Length; i++)
                {
                    float t = i / (float)rate;
                    float note = t < .14f ? t : t - .14f;
                    if (note >= .11f) continue;
                    float envelope = Mathf.Sin(Mathf.PI * note / .11f);
                    data[i] = Mathf.Sin(2 * Mathf.PI * (t < .14f ? 660 : 880) * note) * envelope * envelope * .35f;
                }
                var cue = AudioClip.Create("JustSmash-original-score", data.Length, 1, rate, false);
                cue.SetData(data, 0); return cue;
            }
            float duration = kind == "score" ? .3f : kind == "whistle" ? .2f : .08f;
            var samples = new float[Mathf.RoundToInt(rate * duration)];
            var random = new System.Random(37); float filtered = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                float t = i / (float)rate, phase = t / duration;
                float noise = (float)random.NextDouble() * 2 - 1;
                filtered = Mathf.Lerp(filtered, noise, .4f);
                float tone = Mathf.Sin(2 * Mathf.PI * (kind == "whistle" ? 2300 : kind == "score" ? (phase < .5f ? 660 : 880) : kind == "racket" ? 900 : 120) * t);
                samples[i] = (kind == "whistle" || kind == "score" ? tone : filtered * .8f + tone * .2f) * Mathf.Sin(Mathf.PI * phase) * Mathf.Exp(-phase * 4) * .55f;
            }
            var clip = AudioClip.Create("JustSmash-original-" + kind, samples.Length, 1, rate, false);
            clip.SetData(samples, 0); return clip;
        }
        void PlayFootstep(int id, AthleteVisual visual)
        {
            int contacts = visual.TakeFootContacts();
            if (contacts == 0 || !GameplayInputEnabled || replaying || Volume <= 0) return;
            float distance = Vector3.Distance(Actor(id).position, player.position);
            PlaySound("step", null, 1 / (1 + distance * .12f));
        }
        void PlaySound(string kind, string technique = null, float gain = 1)
        {
            if (!float.IsFinite(gain) || gain < 0 || gain > 1) throw new System.ArgumentOutOfRangeException(nameof(gain));
            if (!GameplayInputEnabled || replaying || Volume <= 0) { StopCourtAudio(); return; }
            if (gain == 0) return;
            InitializeCourtAudio();
            AudioClip clip;
            if (kind == "racket")
            {
                technique ??= lastHitter == 0 ? lastStroke : "Clear";
                int sample = technique == "Smash" ? 1 : technique == "Net" || technique == "Drop" ? 3 : technique == "Lift" ? 2 : 0;
                clip = racketRecordings[sample];
                gain *= technique == "Net" || technique == "Drop" ? .28f : technique == "Smash" ? .85f : .55f;
            }
            else if (kind == "step")
            {
                clip = stepRecordings[footstepVariant];
                footstepVariant = (footstepVariant + 1) % 4;
                gain *= .26f;
            }
            else
            {
                if (!sounds.TryGetValue(kind, out clip)) { clip = CreateSound(kind); sounds.Add(kind, clip); }
                // Score/whistle are original non-voice UI cues, never recordings of officials.
                gain *= kind == "score" ? .25f : kind == "whistle" ? .22f : .35f;
            }
            soundSource.mute = false; indoorReverb.enabled = true;
            LastSoundClip = clip; LastSoundGain = gain; SoundEventCount++;
            soundSource.PlayOneShot(clip, gain);
        }
        void DestroySounds()
        {
            StopCourtAudio();
            foreach (var clip in sounds.Values) Destroy(clip);
            sounds.Clear(); LastSoundClip = null;
            System.Array.Clear(racketRecordings, 0, racketRecordings.Length);
            System.Array.Clear(stepRecordings, 0, stepRecordings.Length);
        }
    }
}
