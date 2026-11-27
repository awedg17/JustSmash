using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace JustSmash
{
    public sealed partial class CourtPrototype
    {
        public Career ActiveCareer { get; private set; }
        string careerMatchId, careerOpponent, careerEvent;
        int careerSlot, careerAthlete, careerStart;
        bool careerResultSaved;
        Career[] careerSlots;
        bool[] badCareerSlots;
        AvatarAppearance draftAppearance = new AvatarAppearance(), editedAppearance;
        GameScreen appearanceReturn;
        bool previewBack;
        static readonly string[] AppearanceActions = { "Model kaus", "Warna kaus", "Warna celana", "Warna sepatu", "Warna rambut", "Warna frame", "Warna grip", "Warna senar", "Putar", "Simpan penampilan", "Kembali" };
        bool CareerMenu => screen == GameScreen.CareerSlots || screen == GameScreen.CareerCreate || screen == GameScreen.CareerCalendar || screen == GameScreen.CareerArchive || screen == GameScreen.CareerAppearance;
        int[] AppearanceChoices() => new[] { editedAppearance.jerseyStyleId, editedAppearance.jerseyColorId, editedAppearance.pantsId, editedAppearance.shoeColorId,
            editedAppearance.hairColorId, editedAppearance.racketColorId, editedAppearance.gripColorId, editedAppearance.stringColorId };
        void PoseCareerPreview()
        {
            player.SetPositionAndRotation(new Vector3(1.1f, 0, -4.7f), Quaternion.Euler(0, previewBack ? 0 : 180, 0));
            playerVisual.Present("Menu", 1, Vector3.zero, Vector3.zero, 0, 0);
            playerVisual.ApplyAppearance(editedAppearance);
            view.fieldOfView = 39;
            float distance = 3.7f;
            Vector3 center = player.position + Vector3.up * 1.02f;
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
            int index = Array.IndexOf(AppearanceActions, action);
            var a = editedAppearance;
            switch (index)
            {
                case 0: a.jerseyStyleId = (a.jerseyStyleId + 1) % AvatarAppearance.Styles.Length; break;
                case 1: a.jerseyColorId = (a.jerseyColorId + 1) % AvatarAppearance.Palette.Length; break;
                case 2: a.pantsId = (a.pantsId + 1) % AvatarAppearance.Dark.Length; break;
                case 3: a.shoeColorId = (a.shoeColorId + 1) % AvatarAppearance.Palette.Length; break;
                case 4: a.hairColorId = (a.hairColorId + 1) % AvatarAppearance.Dark.Length; break;
                case 5: a.racketColorId = (a.racketColorId + 1) % AvatarAppearance.Palette.Length; break;
                case 6: a.gripColorId = (a.gripColorId + 1) % AvatarAppearance.Dark.Length; break;
                case 7: a.stringColorId = (a.stringColorId + 1) % AvatarAppearance.Palette.Length; break;
                case 8: previewBack = !previewBack; break;
            }
            PoseCareerPreview();
        }
        void OpenCareerSlots()
        {
            ActiveCareer = null; careerSlots = new Career[2]; badCareerSlots = new bool[2];
            OpenUi(GameScreen.CareerSlots);
            for (int i = 0; i < 2; i++)
            {
                int index = i;
                if (!CareerAction(() => careerSlots[index] = CareerStore.Load(index + 1))) badCareerSlots[i] = true;
            }
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
        }
        void RecordCareerResult()
        {
            if (careerMatchId == null || careerResultSaved || !ShowMatchResult) return;
            careerResultSaved = CareerAction(() =>
            {
                string result = CareerStore.StageResult(ActiveCareer, CaptureMatch());
                ChangeCareer(next => next.CompleteMatch(careerMatchId, result));
            });
            if (!careerResultSaved) menuNotice = "Hasil belum tersimpan. " + menuNotice;
        }
        void LeaveCareerMatch()
        {
            if (Match == null || reviewAvailable || finalPointPending || replaying) return;
            if (!careerResultSaved && Match.Winner < 0 && !CareerAction(() => ChangeCareer(next => next.matchSnapshot = CaptureMatch()))) return;
            if (Match.Winner >= 0 && !careerResultSaved) return;
            careerMatchId = careerOpponent = careerEvent = null;
            FinishUiAction("Menu"); SetStaminaStat(70); OpenUi(GameScreen.CareerCalendar);
        }
        string[] CareerActions()
        {
            if (screen == GameScreen.CareerSlots) return new[] { "Slot 1", "Slot 2", "Kembali" };
            if (screen == GameScreen.CareerAppearance) return AppearanceActions;
            if (screen == GameScreen.CareerCreate) return new[] { "Atlet", "Titik awal", "Ubah Penampilan", "Buat Atlet", "Kembali" };
            if (screen == GameScreen.CareerArchive) return new[] { "Kembali" };
            var actions = new List<string>();
            if (ActiveCareer.tier >= 0) actions.Add(string.IsNullOrEmpty(ActiveCareer.matchSnapshot) ? "Main laga" : "Lanjut laga");
            else
            {
                for (int i = 0; i <= ActiveCareer.unlocked; i++) actions.Add(Career.Events[i]);
                actions.Add("Latihan atlet");
                if (ActiveCareer.skillPoints >= 3 && ActiveCareer.stamina < 100) actions.Add("Tambah stamina");
            }
            actions.Add("Ubah Penampilan"); actions.Add("Arsip"); actions.Add("Kembali"); return actions.ToArray();
        }
        void ActOnCareer(string action)
        {
            if (screen == GameScreen.CareerAppearance) { EditAppearance(action); return; }
            if (action == "Ubah Penampilan")
            {
                appearanceReturn = screen; editedAppearance = (ActiveCareer?.appearance ?? draftAppearance).Copy(); previewBack = false;
                OpenUi(GameScreen.CareerAppearance); SetShuttleVisible(false); PoseCareerPreview(); return;
            }
            if (action == "Kembali")
            {
                if (screen == GameScreen.CareerSlots) OpenUi(GameScreen.Home);
                else if (screen == GameScreen.CareerArchive) OpenUi(GameScreen.CareerCalendar);
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
            if (action == "Arsip") { OpenUi(GameScreen.CareerArchive); return; }
            CareerAction(() =>
            {
                if (action == "Main laga" || action == "Lanjut laga") StartCareerMatch(action == "Lanjut laga");
                else ChangeCareer(c =>
                {
                    if (action == "Latihan atlet") c.Train();
                    else if (action == "Tambah stamina") c.Upgrade(true);
                    else c.Register(Array.IndexOf(Career.Events, action));
                });
            });
        }
        string CareerLabel(string action)
        {
            if (screen == GameScreen.CareerAppearance)
            {
                int index = Array.IndexOf(AppearanceActions, action);
                if (index >= 0 && index < 8)
                {
                    string[] labels = index == 0 ? AvatarAppearance.Styles : index == 2 || index == 4 || index == 6 ? AvatarAppearance.DarkNames : AvatarAppearance.Colors;
                    return action + " · " + labels[AppearanceChoices()[index]];
                }
                if (action == "Putar") return previewBack ? "Lihat depan" : "Lihat belakang";
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
        string CareerRankingLabel => $"Hari {ActiveCareer.day + 1}  ·  Ranking {(ActiveCareer.Rank == 0 ? "—" : ActiveCareer.Rank.ToString())}  ·  {ActiveCareer.Points} poin  ·  {ActiveCareer.Titles} gelar  ·  Hadiah {ActiveCareer.Prize}" + (ActiveCareer.Rank == 1 ? "  ·  No. 1 dunia" : "  ·  Target: No. 1 dunia");
        void DrawCareer(float w, float h)
        {
            if (screen == GameScreen.CareerAppearance)
            {
                PanelUi(new Rect(24, 24, 674, 620));
                GUI.Label(new Rect(48, 48, 620, 80), "Ubah penampilan", titleStyle);
                return;
            }
            PanelUi(new Rect(24, 24, 990, 630));
            string title = screen == GameScreen.CareerSlots ? "Karier Tunggal" : screen == GameScreen.CareerCreate ? "Buat atlet" : screen == GameScreen.CareerArchive ? "Arsip turnamen" : ActiveCareer.Name;
            GUI.Label(new Rect(48, 40, 900, 70), title, titleStyle);
            if (ActiveCareer == null) return;
            var c = ActiveCareer;
            GUI.Label(new Rect(48, 110, 920, 42), CareerRankingLabel, smallStyle);
            GUI.Label(new Rect(48, 153, 920, 44), c.Event == null ? $"{Career.Starts[c.start]}  ·  Stamina {c.stamina}  ·  {c.skillPoints} poin latihan" : $"{c.Event}  ·  {(c.round == 0 ? "Semifinal" : "Final")} vs {c.Opponent}", bodyStyle);
            if (screen == GameScreen.CareerArchive)
                for (int i = 0; i < Mathf.Min(8, c.history.Count); i++)
                {
                    var r = c.history[c.history.Count - 1 - i];
                    GUI.Label(new Rect(50, 205 + i * 46, 920, 42), $"Hari {r.day + 1}  ·  {Career.Events[r.tier]}  ·  {(r.winner != 0 ? "Tersingkir" : r.round == 1 ? "Juara" : "Menang")} {r.games[0]}:{r.games[1]}  ·  +{r.rankingPoints} poin", smallStyle);
                }
        }
    }
}
