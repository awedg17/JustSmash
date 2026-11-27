using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace JustSmash
{
    [Serializable] public sealed class Career
    {
        public static readonly string[] Athletes = { "Damar Putra", "Nara Aruna", "Reno Surya" };
        public static readonly string[] Starts = { "Klub Kota", "Akademi" };
        public static readonly string[] Events = { "Open Kota", "Sirkuit Nasional", "World Circuit" };
        static readonly string[][] Seeds = {
            new[] { "Bima Santosa", "Tavi Daren", "Daru Naven" },
            new[] { "Arga Wijaya", "Mika Danta", "Lino Toran" },
            new[] { "Raka Pradana", "Kian Maren", "Aren Valen" }
        };
        [Serializable] public sealed class Result
        {
            public string id, opponent;
            public int version, winner, day, tier, round, rankingPoints, prize;
            public string rules;
            public int[] games, finalGamePoints, gamePoints;
        }
        public int version = 1, slot, athlete, start, day, eventNumber, tier = -1, round, unlocked;
        public int skillPoints, stamina = 70, technique = 70;
        public int revision;
        public List<Result> history = new List<Result>();
        public string matchSnapshot;
        public AvatarAppearance appearance = new AvatarAppearance();
        public string Name => Athletes[athlete];
        public string PendingMatchId => tier < 0 ? null : $"{slot}:{eventNumber}:{tier}:{round}";
        public string Opponent => tier < 0 ? null : Seeds[tier][round == 0 ? 0 : 1 + eventNumber % 2];
        public string Event => tier < 0 ? null : Events[tier];
        public int Titles => history.Count(r => r.winner == 0 && r.round == 1);
        // ponytail: fictional best-ten, 52-week ranking against seeded fixed benchmarks.
        public int Points => history.Where(r => r.day > day - 364).OrderByDescending(r => r.rankingPoints).Take(10).Sum(r => r.rankingPoints);
        public int Prize => history.Sum(r => r.prize);
        public int TierPoints(int entry) => entry < 0 || entry >= Events.Length ? throw new ArgumentOutOfRangeException(nameof(entry))
            : history.Where(r => r.tier == entry && r.day > day - 364).OrderByDescending(r => r.rankingPoints).Take(10).Sum(r => r.rankingPoints);
        public int TierRank(int entry)
        {
            int points = TierPoints(entry);
            return points == 0 ? 0 : 1 + Enumerable.Range(1, 64).Count(i => i * (20 << entry) >= points);
        }
        public int WorldPoints => TierPoints(2);
        public int Rank => TierRank(2);
        public void Register(int entry)
        {
            if (tier >= 0 || entry < 0 || entry > unlocked) throw new InvalidOperationException("Turnamen belum tersedia");
            tier = entry; round = 0; eventNumber++; day++;
        }
        public void CompleteMatch(string id, string json)
        {
            if (history.Any(r => r.id == id)) return;
            if (id == null || id != PendingMatchId) throw new InvalidOperationException("Laga bukan ronde aktif");
            var result = JsonUtility.FromJson<Result>(json);
            ValidateScore(result);
            result.id = id; result.opponent = Opponent; result.day = day; result.tier = tier; result.round = round;
            bool won = result.winner == 0;
            bool finished = !won || round == 1;
            result.rankingPoints = !finished ? 0 : (won ? 160 : round == 1 ? 100 : 40) << tier;
            result.prize = result.rankingPoints * 5;
            history.Add(result); skillPoints += won ? 2 : 1;
            matchSnapshot = null;
            if (finished)
            {
                if (won && tier == unlocked && unlocked < Events.Length - 1) unlocked++;
                tier = -1; round = 0; day += 2;
            }
            else { round = 1; day++; }
        }
        public void Train()
        {
            if (tier >= 0) throw new InvalidOperationException("Selesaikan turnamen dahulu");
            day++; skillPoints += start == 1 ? 3 : 2;
        }
        public void Upgrade(bool endurance)
        {
            if (tier >= 0 || skillPoints < 3 || (endurance ? stamina : technique) >= 100)
                throw new InvalidOperationException("Peningkatan belum tersedia");
            if (endurance) stamina++; else technique++;
            skillPoints -= 3;
        }
        static void ValidateScore(Result r)
        {
            if (r == null || r.version != 2 || r.rules != "Official15" || r.winner < 0 || r.winner > 1 || r.games?.Length != 2
                || r.finalGamePoints?.Length != 2 || r.games[r.winner] != 2 || r.games[1 - r.winner] < 0 || r.games[1 - r.winner] > 1)
                throw new ArgumentException("Skor Official 15 tidak valid");
            new SinglesMatch { Winner = r.winner, Games = r.games, Points = r.finalGamePoints, GameNumber = r.games.Sum(), GamePoints = r.gamePoints }.ValidateGamePoints(false);
        }
        internal void Validate(int expectedSlot)
        {
            if (version != 1 || slot != expectedSlot || slot < 1 || slot > 2 || athlete < 0 || athlete >= Athletes.Length || start < 0 || start >= Starts.Length
                || day < 0 || day > 1000000 || eventNumber < 0 || eventNumber > day || tier < -1 || tier > unlocked || unlocked < 0 || unlocked >= Events.Length
                || round < 0 || round > 1 || (tier < 0 && round != 0) || skillPoints < 0 || skillPoints > 4000000 || stamina < 70 || stamina > 100
                || technique < 70 || technique > 100 || history == null || matchSnapshot?.Length > 8000000 || (!string.IsNullOrEmpty(matchSnapshot) && tier < 0))
                throw new ArgumentException("Slot karier tidak valid");
            if (revision < 0) throw new ArgumentException("Revisi slot tidak valid");
            if (!string.IsNullOrEmpty(matchSnapshot)) CourtPrototype.CareerSnapshotResult(matchSnapshot);
            if (appearance == null) appearance = new AvatarAppearance();
            appearance.Validate();
            var ids = new HashSet<string>();
            foreach (var r in history)
            {
                ValidateScore(r);
                if (string.IsNullOrEmpty(r.id) || !ids.Add(r.id) || r.id == PendingMatchId || string.IsNullOrEmpty(r.opponent)
                    || r.day < 0 || r.day > day || r.tier < 0 || r.tier >= Events.Length || r.round < 0 || r.round > 1
                    || r.rankingPoints < 0 || r.rankingPoints > 640 || r.prize != r.rankingPoints * 5)
                    throw new ArgumentException("Arsip karier tidak valid");
            }
        }
    }

    public static class CareerStore
    {
        static string PathFor(int slot) => slot < 1 || slot > 2 ? throw new ArgumentOutOfRangeException(nameof(slot))
            : Path.Combine(LocalGameData.Root, "careers", $"slot-{slot}-v1.json");
        static Career Read(string path, int slot)
        {
            if (new FileInfo(path).Length > 16000000) throw new ArgumentException("Slot terlalu besar");
            var value = JsonUtility.FromJson<Career>(File.ReadAllText(path));
            if (value == null) throw new ArgumentException("Slot kosong");
            value.Validate(slot);
            return value;
        }
        static Career ReadSlot(string path, int slot, out bool recovered)
        {
            recovered = false;
            if (!File.Exists(path)) { recovered = File.Exists(path + ".bak"); return recovered ? Read(path + ".bak", slot) : null; }
            try { return Read(path, slot); }
            catch (ArgumentException) when (File.Exists(path + ".bak"))
            {
                recovered = true;
                return Read(path + ".bak", slot);
            }
        }
        static bool TryReadPending(string pendingPath, Career target, out string result, out string snapshot)
        {
            result = null; snapshot = null;
            if (target?.PendingMatchId == null || !File.Exists(pendingPath)) return false;
            try
            {
                var pending = Read(pendingPath, target.slot);
                if (pending.revision != target.revision || pending.PendingMatchId != target.PendingMatchId) return false;
                result = CourtPrototype.CareerSnapshotResult(pending.matchSnapshot);
                if (result == null) return false;
                snapshot = pending.matchSnapshot;
                return true;
            }
            catch (ArgumentException) { return false; }
        }
        static int TryReadRevision(string path)
        {
            if (!File.Exists(path) || new FileInfo(path).Length > 16000000) return -1;
            try
            {
                var value = JsonUtility.FromJson<Career>(File.ReadAllText(path));
                return value != null && value.revision >= 0 ? value.revision : -1;
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is ArgumentException) { return -1; }
        }
        static int NextRevision(string path, int oldRevision, bool recovered)
        {
            string dir = Path.GetDirectoryName(path);
            string name = Path.GetFileName(path);
            int highest = Math.Max(oldRevision, Math.Max(TryReadRevision(path), Math.Max(TryReadRevision(path + ".bak"), TryReadRevision(path + ".pending"))));
            int corruptCount = 0;
            if (Directory.Exists(dir))
                foreach (string file in Directory.GetFiles(dir, name + ".corrupt-*"))
                {
                    corruptCount++;
                    highest = Math.Max(highest, TryReadRevision(file));
                }
            int next = checked(highest + 1);
            return recovered ? Math.Max(next, checked(oldRevision + 2 + corruptCount)) : next;
        }
        public static Career Load(int slot)
        {
            string path = PathFor(slot);
            var value = ReadSlot(path, slot, out _);
            if (TryReadPending(path + ".pending", value, out _, out string snapshot)) value.matchSnapshot = snapshot;
            return value;
        }
        static FileStream LockSlot(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            return new FileStream(path + ".lock", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }
        static Career CheckRevision(string path, Career value, out bool recovered)
        {
            var current = ReadSlot(path, value.slot, out recovered);
            if ((current?.revision ?? 0) != value.revision)
                throw new InvalidOperationException("Slot berubah di sesi lain. Buka ulang slot.");
            return current;
        }
        internal static string StageResult(Career value, string snapshot)
        {
            string path = PathFor(value.slot);
            using var held = LockSlot(path);
            var current = CheckRevision(path, value, out _);
            string result = CourtPrototype.CareerSnapshotResult(snapshot);
            if (current?.PendingMatchId == null || current.PendingMatchId != value.PendingMatchId || result == null)
                throw new InvalidOperationException("Hasil karier belum final");
            string pendingPath = path + ".pending";
            if (TryReadPending(pendingPath, current, out string existingResult, out _)) return existingResult;
            current.matchSnapshot = snapshot;
            Write(pendingPath, current, null);
            return result;
        }
        public static void Save(Career value)
        {
            string path = PathFor(value.slot);
            value.Validate(value.slot);
            using var held = LockSlot(path);
            var current = CheckRevision(path, value, out bool recovered);
            if (TryReadPending(path + ".pending", current, out _, out _) && !value.history.Any(r => r.id == current.PendingMatchId))
                throw new InvalidOperationException("Selesaikan simpan hasil laga dahulu.");
            int oldRevision = value.revision;
            value.revision = NextRevision(path, oldRevision, recovered);
            try { Write(path, value, recovered ? path + ".corrupt-" + Guid.NewGuid().ToString("N") : path + ".bak"); }
            catch { value.revision = oldRevision; throw; }
        }
        static void Write(string path, Career value, string backup)
        {
            string temp = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                using (var stream = new FileStream(temp, FileMode.CreateNew, FileAccess.Write))
                {
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(value));
                    stream.Write(data, 0, data.Length);
                    stream.Flush(true);
                }
                if (File.Exists(path)) File.Replace(temp, path, backup);
                else File.Move(temp, path);
            }
            finally { if (File.Exists(temp)) File.Delete(temp); }
        }
    }
}
