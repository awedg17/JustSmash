using System;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace JustSmash
{
    public static class LocalGameData
    {
        static bool OnDevice => Application.platform == RuntimePlatform.Android;
        public static string Root => OnDevice ? Application.persistentDataPath : Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".hermes", "local-game-data"));
        static string KeyPath(string key)
        {
            if (string.IsNullOrEmpty(key) || key.Length > 64) throw new ArgumentException("Invalid preference key", nameof(key));
            foreach (char c in key)
                if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '-' || c == '_')) throw new ArgumentException("Invalid preference key", nameof(key));
            return Path.Combine(Root, "prefs", key + ".txt");
        }
        public static string GetString(string key, string fallback = "")
        {
            if (OnDevice) return PlayerPrefs.GetString(key, fallback);
            string path = KeyPath(key);
            if (!File.Exists(path)) return fallback;
            if (new FileInfo(path).Length > 4096) throw new IOException("Preference file too large");
            return File.ReadAllText(path);
        }
        public static void SetString(string key, string value)
        {
            if (value == null || value.Length > 1024) throw new ArgumentException("Invalid preference value");
            if (OnDevice) { PlayerPrefs.SetString(key, value); return; }
            string path = KeyPath(key); Directory.CreateDirectory(Path.GetDirectoryName(path));
            string temp = path + ".tmp"; File.WriteAllText(temp, value);
            if (File.Exists(path)) File.Replace(temp, path, path + ".bak"); else File.Move(temp, path);
        }
        public static float GetFloat(string key, float fallback = 0)
        {
            if (OnDevice) return PlayerPrefs.GetFloat(key, fallback);
            return float.TryParse(GetString(key), NumberStyles.Float, CultureInfo.InvariantCulture, out float value) && float.IsFinite(value) ? value : fallback;
        }
        public static void SetFloat(string key, float value)
        {
            if (!float.IsFinite(value)) throw new ArgumentOutOfRangeException(nameof(value));
            if (OnDevice) PlayerPrefs.SetFloat(key, value); else SetString(key, value.ToString("R", CultureInfo.InvariantCulture));
        }
        public static void Save() { if (OnDevice) PlayerPrefs.Save(); }
    }
}
