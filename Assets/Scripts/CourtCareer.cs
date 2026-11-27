using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace JustSmash
{
    public sealed partial class CourtPrototype
    {
        public Career ActiveCareer { get; private set; }
        internal enum CareerSaveState { None, PendingFailed, CommitFailed, Saved }
        internal CareerSaveState CareerSaveStatus { get; private set; }
        string careerMatchId, careerOpponent, careerEvent;
        int careerSlot, careerAthlete, careerStart;
        bool careerResultSaved;
        Career[] careerSlots;
        bool[] badCareerSlots;
        AvatarAppearance draftAppearance = new AvatarAppearance(), editedAppearance;
        GameScreen appearanceReturn;
        bool previewBack, previewCloseUp;
        static readonly string[] AppearanceActions = {
            "Wajah", "Gaya rambut", "Model kaus", "Warna kaus", "Warna celana", "Warna sepatu", "Warna rambut",
            "Warna frame", "Warna grip", "Warna senar", "Putar", "Lihat wajah", "Simpan penampilan", "Kembali"
        };
        bool CareerMenu => screen == GameScreen.CareerSlots || screen == GameScreen.CareerCreate
            || screen == GameScreen.CareerSummary || screen == GameScreen.CareerCalendar
            || screen == GameScreen.CareerAthlete || screen == GameScreen.CareerArchive
            || screen == GameScreen.CareerAppearance;
        static int AppearanceSwatchCount(string action) => action switch
        {
            "Wajah" => AvatarAppearance.FaceTones.Length,
            "Warna kaus" => AvatarAppearance.Palette.Length,
            "Warna celana" => AvatarAppearance.Dark.Length,
            "Warna sepatu" => AvatarAppearance.Palette.Length,
            "Warna rambut" => AvatarAppearance.Dark.Length,
            "Warna frame" => AvatarAppearance.Palette.Length,
            "Warna grip" => AvatarAppearance.Dark.Length,
            "Warna senar" => AvatarAppearance.Palette.Length,
            _ => 0
        };
        Color[] AppearanceSwatches(string action, out int active)
        {
            active = -1;
            var a = editedAppearance; if (a == null) return null;
            switch (action)
            {
                case "Wajah": active = a.faceId; return AvatarAppearance.FaceTones;
                case "Warna kaus": active = a.jerseyColorId; return AvatarAppearance.Palette;
                case "Warna celana": active = a.pantsId; return AvatarAppearance.Dark;
                case "Warna sepatu": active = a.shoeColorId; return AvatarAppearance.Palette;
                case "Warna rambut": active = a.hairColorId; return AvatarAppearance.Dark;
                case "Warna frame": active = a.racketColorId; return AvatarAppearance.Palette;
                case "Warna grip": active = a.gripColorId; return AvatarAppearance.Dark;
                case "Warna senar": active = a.stringColorId; return AvatarAppearance.Palette;
                default: return null;
            }
        }
        void ApplyAppearanceSwatch(string action, int swatch)
        {
            var a = editedAppearance;
            switch (action)
            {
                case "Wajah": a.faceId = swatch; break;
                case "Warna kaus": a.jerseyColorId = swatch; break;
                case "Warna celana": a.pantsId = swatch; break;
                case "Warna sepatu": a.shoeColorId = swatch; break;
                case "Warna rambut": a.hairColorId = swatch; break;
                case "Warna frame": a.racketColorId = swatch; break;
                case "Warna grip": a.gripColorId = swatch; break;
                case "Warna senar": a.stringColorId = swatch; break;
            }
            PoseCareerPreview();
        }
        void PoseCareerPreview()
        {
            player.SetPositionAndRotation(new Vector3(1.1f, 0, -4.7f), Quaternion.Euler(0, previewBack ? 0 : 180, 0));
            playerVisual.Present("Menu", 1, Vector3.zero, Vector3.zero, 0, 0);
            playerVisual.ApplyAppearance(editedAppearance);
            view.fieldOfView = previewCloseUp ? 28 : 39;
            float distance = previewCloseUp ? 1.65f : 3.7f;
            Vector3 center = player.position + Vector3.up * (previewCloseUp ? 1.48f : 1.02f);
            Vector3 target = center - Vector3.right * (distance * Mathf.Tan(view.fieldOfView * Mathf.Deg2Rad * .5f) * view.aspect * .57f);
            view.transform.SetPositionAndRotation(target - Vector3.forward * distance, Quaternion.identity);
        }
        void EditAppearance(string action)
        {
            if (action == "Kembali" || action == "Simpan penampilan")
            {
                if (action == "Simpan penampilan")
                {
                    if (appearanceReturn == GameScreen.CareerCreate) draftAppearance = editedAppearance.Copy();
                    else if (!CareerAction(() => ChangeCareer(next => next.appearance = editedAppearance.Copy()))) return;
                }
                playerVisual.ApplyAppearance(null); OpenUi(appearanceReturn); return;
            }
            var a = editedAppearance;
            switch (action)
            {
                case "Wajah": a.faceId = (a.faceId + 1) % AvatarAppearance.FaceTones.Length; break;
                case "Gaya rambut": a.hairStyleId = (a.hairStyleId + 1) % AvatarAppearance.HairStyles.Length; break;
                case "Model kaus": a.jerseyStyleId = (a.jerseyStyleId + 1) % AvatarAppearance.Styles.Length; break;
                case "Warna kaus": a.jerseyColorId = (a.jerseyColorId + 1) % AvatarAppearance.Palette.Length; break;
                case "Warna celana": a.pantsId = (a.pantsId + 1) % AvatarAppearance.Dark.Length; break;
                case "Warna sepatu": a.shoeColorId = (a.shoeColorId + 1) % AvatarAppearance.Palette.Length; break;
                case "Warna rambut": a.hairColorId = (a.hairColorId + 1) % AvatarAppearance.Dark.Length; break;
                case "Warna frame": a.racketColorId = (a.racketColorId + 1) % AvatarAppearance.Palette.Length; break;
                case "Warna grip": a.gripColorId = (a.gripColorId + 1) % AvatarAppearance.Dark.Length; break;
                case "Warna senar": a.stringColorId = (a.stringColorId + 1) % AvatarAppearance.Palette.Length; break;
                case "Putar": previewBack = !previewBack; break;
                case "Lihat wajah": previewCloseUp = !previewCloseUp; break;
            }
            PoseCareerPreview();
        }
        void OpenCareerSlots()
        {
            ActiveCareer = null; careerSlots = new Career[2]; badCareerSlots = new bool[2];
            OpenUi(GameScreen.CareerSlots);
            string firstNotice = null;
            for (int i = 0; i < 2; i++)
            {
                int index = i;
                if (!CareerAction(() => careerSlots[index] = CareerStore.Load(index + 1)))
                {
                    badCareerSlots[i] = true;
                    firstNotice ??= menuNotice;
                }
            }
            menuNotice = firstNotice;
        }
        bool CareerAction(Action action)
        {
            try { action(); menuNotice = null; return true; }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is ArgumentException || e is InvalidOperationException)
            {
                menuNotice = "Karier: " + (e is IOException || e is UnauthorizedAccessException ? "penyimpanan tidak dapat diakses. Coba lagi." : e.Message);
                Debug.LogWarning("[career] " + e.GetType().Name + ": " + e.Message);
                return false;
            }
        }
        void ChangeCareer(Action<Career> apply)
        {
            var next = JsonUtility.FromJson<Career>(JsonUtility.ToJson(ActiveCareer));
            apply(next); CareerStore.Save(next); ActiveCareer = next;
        }
        void StartCareerMatch(bool resume)
        {
            var career = ActiveCareer;
            if (career?.PendingMatchId == null) return;
            if (ShowMatchResult || Match == null) SetStaminaStat(career.stamina);
            ConfigureDiscipline(false); Difficulty = "Normal";
            if (resume) { RestoreMatch(career.matchSnapshot); EnterGameScreen(false); }
            else BeginExhibition();
            playerVisual.ApplyAppearance(career.appearance);
            careerMatchId = career.PendingMatchId; careerOpponent = career.Opponent; careerEvent = career.Event; careerResultSaved = false;
            CareerSaveStatus = CareerSaveState.None;
        }
        void RecordCareerResult()
        {
            if (careerMatchId == null || careerResultSaved || !ShowMatchResult) return;
            string result = null;
            if (!CareerAction(() => result = CareerStore.StageResult(ActiveCareer, CaptureMatch())))
            {
                CareerSaveStatus = CareerSaveState.PendingFailed;
                menuNotice = "Hasil belum tersimpan (gagal menulis .pending). " + menuNotice;
                return;
            }
            if (!CareerAction(() => ChangeCareer(next => next.CompleteMatch(careerMatchId, result))))
            {
                CareerSaveStatus = CareerSaveState.CommitFailed;
                menuNotice = "Hasil belum tersimpan (gagal menerapkan ke slot). " + menuNotice;
                return;
            }
            careerResultSaved = true;
            CareerSaveStatus = CareerSaveState.Saved;
        }
        void LeaveCareerMatch()
        {
            if (Match == null || reviewAvailable || finalPointPending || replaying) return;
            if (!careerResultSaved && Match.Winner < 0 && !CareerAction(() => ChangeCareer(next => next.matchSnapshot = CaptureMatch()))) return;
            if (Match.Winner >= 0 && !careerResultSaved) return;
            careerMatchId = careerOpponent = careerEvent = null; CareerSaveStatus = CareerSaveState.None;
            FinishUiAction("Menu"); SetStaminaStat(70); OpenUi(GameScreen.CareerCalendar);
        }
        string[] CareerActions()
        {
            if (screen == GameScreen.CareerSlots) return new[] { "Slot 1", "Slot 2", "Kembali" };
            if (screen == GameScreen.CareerAppearance) return AppearanceActions;
            if (screen == GameScreen.CareerCreate) return new[] { "Atlet", "Titik awal", "Ubah Penampilan", "Buat Atlet", "Kembali" };
            var actions = new List<string> { "TabRingkasan", "TabKalender", "TabAtlet", "TabArsip", "Kembali" };
            if (screen == GameScreen.CareerArchive) return actions.ToArray();
            if (screen == GameScreen.CareerSummary)
            {
                if (ActiveCareer.tier >= 0) actions.Add(string.IsNullOrEmpty(ActiveCareer.matchSnapshot) ? "Main laga" : "Lanjut laga");
                else actions.Add("Buka kalender");
                actions.Add("Ubah Penampilan");
                return actions.ToArray();
            }
            if (screen == GameScreen.CareerAthlete)
            {
                actions.Add("Latihan atlet");
                if (ActiveCareer.skillPoints >= 3 && ActiveCareer.stamina < 100) actions.Add("Tambah stamina");
                if (ActiveCareer.skillPoints >= 3 && ActiveCareer.technique < 100) actions.Add("Tambah teknik");
                actions.Add("Ubah Penampilan");
                return actions.ToArray();
            }
            if (ActiveCareer.tier >= 0) actions.Add(string.IsNullOrEmpty(ActiveCareer.matchSnapshot) ? "Main laga" : "Lanjut laga");
            else
            {
                for (int i = 0; i <= ActiveCareer.unlocked; i++) actions.Add(Career.Events[i]);
                actions.Add("Latihan atlet");
                if (ActiveCareer.skillPoints >= 3 && ActiveCareer.stamina < 100) actions.Add("Tambah stamina");
            }
            actions.Add("Ubah Penampilan"); actions.Add("Arsip"); return actions.ToArray();
        }
        void ActOnCareer(string action)
        {
            if (screen == GameScreen.CareerAppearance) { EditAppearance(action); return; }
            if (action == "TabRingkasan") { OpenUi(GameScreen.CareerSummary); return; }
            if (action == "TabKalender" || action == "Buka kalender") { OpenUi(GameScreen.CareerCalendar); return; }
            if (action == "TabAtlet") { OpenUi(GameScreen.CareerAthlete); return; }
            if (action == "TabArsip" || action == "Arsip") { OpenUi(GameScreen.CareerArchive); return; }
            if (action == "Ubah Penampilan")
            {
                appearanceReturn = screen; editedAppearance = (ActiveCareer?.appearance ?? draftAppearance).Copy(); previewBack = false; previewCloseUp = false;
                OpenUi(GameScreen.CareerAppearance); SetShuttleVisible(false); PoseCareerPreview(); return;
            }
            if (action == "Kembali")
            {
                if (screen == GameScreen.CareerSlots) OpenUi(GameScreen.Home);
                else if (screen == GameScreen.CareerArchive || screen == GameScreen.CareerSummary || screen == GameScreen.CareerAthlete) OpenUi(GameScreen.CareerCalendar);
                else OpenCareerSlots();
                return;
            }
            if (screen == GameScreen.CareerSlots)
            {
                careerSlot = action == "Slot 1" ? 1 : 2;
                if (badCareerSlots[careerSlot - 1]) { OpenCareerSlots(); return; }
                ActiveCareer = careerSlots[careerSlot - 1]; careerAthlete = careerStart = 0; draftAppearance = new AvatarAppearance();
                OpenUi(ActiveCareer == null ? GameScreen.CareerCreate : GameScreen.CareerCalendar); return;
            }
            if (screen == GameScreen.CareerCreate)
            {
                if (action == "Atlet") careerAthlete = (careerAthlete + 1) % Career.Athletes.Length;
                else if (action == "Titik awal") careerStart = (careerStart + 1) % Career.Starts.Length;
                else if (action == "Buat Atlet" && CareerAction(() =>
                {
                    var next = new Career { slot = careerSlot, athlete = careerAthlete, start = careerStart, appearance = draftAppearance.Copy() };
                    CareerStore.Save(next); ActiveCareer = next;
                })) OpenUi(GameScreen.CareerCalendar);
                return;
            }
            CareerAction(() =>
            {
                if (action == "Main laga" || action == "Lanjut laga") StartCareerMatch(action == "Lanjut laga");
                else ChangeCareer(c =>
                {
                    if (action == "Latihan atlet") c.Train();
                    else if (action == "Tambah stamina") c.Upgrade(true);
                    else if (action == "Tambah teknik") c.Upgrade(false);
                    else c.Register(Array.IndexOf(Career.Events, action));
                });
            });
        }
        string CareerLabel(string action)
        {
            if (action == "TabRingkasan") return "Ringkasan";
            if (action == "TabKalender") return "Kalender";
            if (action == "TabAtlet") return "Atlet";
            if (action == "TabArsip") return "Arsip";
            if (screen == GameScreen.CareerAppearance)
            {
                var a = editedAppearance;
                return action switch
                {
                    "Wajah" => "Wajah · " + AvatarAppearance.FaceNames[a.faceId],
                    "Gaya rambut" => "Rambut · " + AvatarAppearance.HairStyles[a.hairStyleId],
                    "Model kaus" => "Model · " + AvatarAppearance.Styles[a.jerseyStyleId],
                    "Warna kaus" => "Kaus · " + AvatarAppearance.Colors[a.jerseyColorId],
                    "Warna celana" => "Celana · " + AvatarAppearance.DarkNames[a.pantsId],
                    "Warna sepatu" => "Sepatu · " + AvatarAppearance.Colors[a.shoeColorId],
                    "Warna rambut" => "Cat · " + AvatarAppearance.DarkNames[a.hairColorId],
                    "Warna frame" => "Frame · " + AvatarAppearance.Colors[a.racketColorId],
                    "Warna grip" => "Grip · " + AvatarAppearance.DarkNames[a.gripColorId],
                    "Warna senar" => "Senar · " + AvatarAppearance.Colors[a.stringColorId],
                    "Putar" => previewBack ? "Lihat depan" : "Lihat belakang",
                    "Lihat wajah" => previewCloseUp ? "Lihat penuh" : "Lihat wajah",
                    _ => action
                };
            }
            if (screen == GameScreen.CareerSlots && action.StartsWith("Slot ", StringComparison.Ordinal))
            {
                int index = action == "Slot 1" ? 0 : 1;
                return action + " · " + (badCareerSlots[index] ? "Gagal dibaca" : careerSlots[index]?.Name ?? "Kosong");
            }
            if (action == "Atlet") return Career.Athletes[careerAthlete];
            if (action == "Titik awal") return Career.Starts[careerStart];
            return action;
        }
        string CareerRankingLabel
        {
            get
            {
                var c = ActiveCareer;
                var ranks = new List<string> { c.Rank > 0 ? $"Dunia #{c.Rank} ({c.WorldPoints} poin WC)" : "Dunia —" };
                if (c.TierRank(0) > 0) ranks.Add($"Kota #{c.TierRank(0)} ({c.TierPoints(0)} poin)");
                if (c.TierRank(1) > 0) ranks.Add($"Nasional #{c.TierRank(1)} ({c.TierPoints(1)} poin)");
                return $"Hari {c.day + 1}  ·  {string.Join("  ·  ", ranks)}  ·  {c.Titles} gelar  ·  Total {c.Points} poin  ·  Hadiah {c.Prize}" + (c.Rank == 1 ? "  ·  No. 1 dunia" : "");
            }
        }
        string CareerRequirementLabel
        {
            get
            {
                var c = ActiveCareer;
                if (c.unlocked == 0) return "Syarat Sirkuit Nasional: Juara Open Kota";
                if (c.unlocked == 1) return "Syarat World Circuit: Juara Sirkuit Nasional";
                return c.Rank == 0 ? "World Circuit terbuka · Main untuk masuk ranking dunia"
                    : c.Rank == 1 ? "Status: Peringkat No. 1 dunia" : "Target: No. 1 dunia di World Circuit";
            }
        }
        void DrawCareer(float w, float h)
        {
            if (screen == GameScreen.CareerAppearance)
            {
                PanelUi(new Rect(24, 24, 672, h - 48));
                GUI.Label(new Rect(44, 42, 620, 64), "Ubah penampilan", titleStyle);
                return;
            }
            float pw = w - 48;
            PanelUi(new Rect(24, 24, pw, h - 48));
            string title = screen == GameScreen.CareerSlots ? "Karier Tunggal"
                : screen == GameScreen.CareerCreate ? "Buat atlet"
                : screen == GameScreen.CareerSummary ? $"{ActiveCareer?.Name} · Ringkasan"
                : screen == GameScreen.CareerAthlete ? $"{ActiveCareer?.Name} · Atlet"
                : screen == GameScreen.CareerArchive ? $"{ActiveCareer?.Name} · Arsip"
                : $"{ActiveCareer?.Name} · Kalender";
            GUI.Label(new Rect(44, 34, pw - 240, 56), title, titleStyle);
            if (ActiveCareer == null) return;
            var c = ActiveCareer;
            string nextObjective = c.tier >= 0
                ? $"Laga berikutnya: {c.Event} · {(c.round == 0 ? "Semifinal" : "Final")} vs {c.Opponent} — Tekan {(string.IsNullOrEmpty(c.matchSnapshot) ? "MAIN LAGA" : "LANJUT LAGA")}"
                : $"Belum terdaftar · Pilih {Career.Events[c.unlocked]} di Kalender untuk bertanding atau Latihan atlet";
            GUI.Label(new Rect(44, 160, pw - 48, 34), CareerRankingLabel, smallStyle);
            FillUi(new Rect(44, 200, pw - 48, 92), new Color(.04f, .16f, .44f, .88f));
            GUI.Label(new Rect(58, 208, pw - 76, 36), nextObjective, bodyStyle);
            GUI.Label(new Rect(58, 250, pw - 76, 34), $"{Career.Starts[c.start]}  ·  Stamina {c.stamina}  ·  Teknik {c.technique}  ·  Poin latihan {c.skillPoints}  ·  {CareerRequirementLabel}", smallStyle);
            if (screen == GameScreen.CareerSummary)
            {
                float infoX = 44 + Mathf.Min(420, (pw - 64) * .48f) + 24, infoW = pw - infoX - 24;
                GUI.Label(new Rect(infoX, 312, infoW, 34), "RIWAYAT TERBARU", bodyStyle);
                if (c.history.Count == 0) GUI.Label(new Rect(infoX, 354, infoW, 36), "Belum ada pertandingan selesai.", smallStyle);
                for (int i = 0; i < Mathf.Min(5, c.history.Count); i++)
                {
                    var r = c.history[c.history.Count - 1 - i];
                    GUI.Label(new Rect(infoX, 354 + i * 40, infoW, 36), $"Hari {r.day + 1} · {Career.Events[r.tier]} {(r.round == 0 ? "SF" : "F")} · {(r.winner == 0 ? "Menang" : "Kalah")} {r.games[0]}:{r.games[1]} · +{r.rankingPoints} poin", smallStyle);
                }
            }
            else if (screen == GameScreen.CareerAthlete)
            {
                float infoX = 44 + Mathf.Min(420, (pw - 64) * .48f) + 24, infoW = pw - infoX - 24;
                var a = c.appearance;
                GUI.Label(new Rect(infoX, 312, infoW, 34), "PROFIL & PERALATAN", bodyStyle);
                GUI.Label(new Rect(infoX, 354, infoW, 32), $"Asal: {Career.Starts[c.start]}", smallStyle);
                GUI.Label(new Rect(infoX, 390, infoW, 32), $"Stamina: {c.stamina}/100  ·  Teknik: {c.technique}/100  ·  Poin latihan: {c.skillPoints}", smallStyle);
                GUI.Label(new Rect(infoX, 426, infoW, 32), $"Wajah: {AvatarAppearance.FaceNames[a.faceId]}  ·  Rambut: {AvatarAppearance.HairStyles[a.hairStyleId]} ({AvatarAppearance.DarkNames[a.hairColorId]})", smallStyle);
                GUI.Label(new Rect(infoX, 462, infoW, 32), $"Outfit: {AvatarAppearance.Styles[a.jerseyStyleId]} {AvatarAppearance.Colors[a.jerseyColorId]} / Celana {AvatarAppearance.DarkNames[a.pantsId]}", smallStyle);
                GUI.Label(new Rect(infoX, 498, infoW, 32), $"Raket: Frame {AvatarAppearance.Colors[a.racketColorId]} · Grip {AvatarAppearance.DarkNames[a.gripColorId]} · Senar {AvatarAppearance.Colors[a.stringColorId]}", smallStyle);
            }
            else if (screen == GameScreen.CareerArchive)
            {
                if (c.history.Count == 0) GUI.Label(new Rect(44, 312, pw - 48, 36), "Belum ada riwayat turnamen.", bodyStyle);
                for (int i = 0; i < Mathf.Min(8, c.history.Count); i++)
                {
                    var r = c.history[c.history.Count - 1 - i];
                    GUI.Label(new Rect(44, 312 + i * 42, pw - 48, 38), $"Hari {r.day + 1}  ·  {Career.Events[r.tier]} ({(r.round == 0 ? "Semifinal" : "Final")})  ·  {(r.winner != 0 ? "Tersingkir" : r.round == 1 ? "Juara" : "Menang")} {r.games[0]}:{r.games[1]}  ·  +{r.rankingPoints} poin  ·  +{r.prize} hadiah", smallStyle);
                }
            }
        }
    }
}
