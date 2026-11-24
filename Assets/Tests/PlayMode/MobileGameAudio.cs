using System;
using System.IO;
using UnityEngine;

// Test-only listener tap: game mix only, never microphone or desktop audio.
public sealed class MobileGameAudio : MonoBehaviour
{
    readonly object gate = new object();
    FileStream output;
    byte[] bytes = Array.Empty<byte>();
    public int Channels { get; private set; }
    // ponytail: aggregate clocks locate startup/gap/write delay, not per-block reconstruction.
    [Serializable] sealed class CaptureClock
    {
        public double startWall, stopWall, startDsp, stopDsp, firstWall = -1, lastWall, firstDsp, lastDsp;
        public double maxWallGap, maxDspGap, maxWriteSeconds;
        public long sampleFloats;
        public int callbacks, channels, sampleRate, stopSampleRate;
    }
    CaptureClock clock; string clockPath;
    static double WallNow => (double)System.Diagnostics.Stopwatch.GetTimestamp() / System.Diagnostics.Stopwatch.Frequency;
    public void StartCapture(string path)
    {
        lock (gate)
        {
            output = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
            clockPath = path + ".clock.json";
            clock = new CaptureClock { startWall = WallNow, startDsp = AudioSettings.dspTime, sampleRate = AudioSettings.outputSampleRate };
        }
    }
    void OnAudioFilterRead(float[] data, int channels)
    {
        double wall = WallNow, dsp = AudioSettings.dspTime;
        lock (gate)
        {
            if (output == null) return;
            if (clock.callbacks == 0) { clock.firstWall = wall; clock.firstDsp = dsp; }
            else
            {
                clock.maxWallGap = Math.Max(clock.maxWallGap, wall - clock.lastWall);
                clock.maxDspGap = Math.Max(clock.maxDspGap, dsp - clock.lastDsp);
            }
            clock.lastWall = wall; clock.lastDsp = dsp; clock.callbacks++; clock.sampleFloats += data.Length;
            Channels = clock.channels = channels;
            int count = data.Length * sizeof(float);
            if (bytes.Length < count) bytes = new byte[count];
            Buffer.BlockCopy(data, 0, bytes, 0, count);
            double writeStart = WallNow;
            output.Write(bytes, 0, count);
            clock.maxWriteSeconds = Math.Max(clock.maxWriteSeconds, WallNow - writeStart);
        }
    }
    public void StopCapture()
    {
        lock (gate)
        {
            if (output == null) return;
            output.Dispose(); output = null;
            clock.stopWall = WallNow; clock.stopDsp = AudioSettings.dspTime; clock.stopSampleRate = AudioSettings.outputSampleRate;
            File.WriteAllText(clockPath, JsonUtility.ToJson(clock));
        }
    }
    void OnDestroy() { StopCapture(); }
}
