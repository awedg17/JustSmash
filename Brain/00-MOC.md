---
tags: [moc, justsmash]
updated: 2026-11-23
---

# JustSmash — Map of Content

Unity badminton mobile game. Offline, Android landscape, singles + ganda putra, PC-only QA (D-052). Unity aktif di root (`Assets/`, `Packages/`, `ProjectSettings/`), docs di `docs/`, evidence di `evidence/`.

Mulai dari sini tiap sesi. Urutan baca: halaman ini → [[SESSION_HANDOFF]] (checkpoint terbaru) → [[AI_RULES]] (batas kerja) → [[05_DECISION_REGISTER]] (keputusan aktif) → dokumen sesuai task.

## Dokumen sumber

| Note | Fungsi |
|---|---|
| [[PRD]] | Kebutuhan produk + traceability |
| [[01_GDD]] | Visi, gameplay, batas produk |
| [[02_ROADMAP]] | Task M0–M8, gate acceptance |
| [[ARCHITECTURE]] | Teknis Unity, state, save, simulasi |
| [[DESIGN]] | Visual/UX, kontrol, kamera |
| [[TESTING]] | Acceptance, protokol ukur, hasil tes aktual |
| [[ERROR_LOG]] | Bug, percobaan fix, bukti regresi |
| [[05_DECISION_REGISTER]] | Satu-satunya register keputusan |
| [[04_LEGAL_AND_ASSETS]] | Lisensi, aset, gate publikasi |
| [[AI_RULES]] | Aturan agent, wajib |
| [[SESSION_HANDOFF]] | Checkpoint per sesi (riwayat panjang) |
| [[00_README]] | Indeks lama + hierarki otoritas |
| [[UI_REFERENSI]] | Screenshot referensi terverifikasi |

## Status aktif

- T4e foot-state216cases max.370000362m→0;48invalid+56replaychecksPASS. Actual render boundary now proves foot pose restored:24,164pixels→0. Snapshot1500frames failed old1Mlimit;bounded8Mguard26testsPASS. Previous full20Edit+248/250Play,two knownlunge;predates guard. Replay recorder boundary fixed:87.67→11.67ms AVgap,12scenarios/1683frames decode+browser1xPASS0drops(muted). Natural-motion/listening still open. See [[SESSION_HANDOFF]].

- Wave4 Net/Lift target subset:64focusedPASS,fullEdit20/20+Play245/247;dua lunge lama tetap gagal. Net travel.37→.127m,Lift rise.18→.52m. `wave4-low-strokes-checkpoint1.json` source-bound. Penghalang closeup terbukti NetTape ketika kamera uji menyeberang net;harness-only fix sedang diverifikasi,geometri/kamera gameplay tetap. Latest naturalAVduration69/60.67msFAIL;belum wholemotion acceptance.

- Wave4 idle breathing subset:0→0.799169deg maximum offset,8actor/end cases,feet0m drift. Targeted63/63+Edit20/20;fullPlay244/246,dua lunge lama tetap gagal. Controlled1x samecamera pair strictclock/decode PASS. `wave4-idle-checkpoint1.json` memuat source terbaru;fullmatches wave3 mendahului idle change. Tidak ada karakter baru/closure seluruh T3a.

- Full singles3difficulty+doublesNormal selesai;Normal singles AV PASS;Easy/Hard/doubles PCM-duration gateFAIL74/84/55ms. Recovery input160cases baselinePASS,test-only;idle runtime change tercatat di atas. Controlled marker tetap mediaFAIL91.59ms. Baca [[SESSION_HANDOFF]]; belum overhaul selesai.

- D-063 terbaru: cold-HUD91.31→15.93ms via native glyph warmup;34UI+1cold+1natural PASS (bukan unique count). Dua excerpt strict50ms clock/AV/decode PASS; camera-equal HUD pair12pixels differ. `wave3-font-checkpoint2.json` binds source. Fullmatch/listening/lunge/asset/Android gates tetap terbuka. [[SESSION_HANDOFF]] memisahkan hasil terbaru dari histori gagal.

- D-063 wave3 terbaru: F/G kontak immediate dipertahankan; lunge kandidat ditolak/rollback (moving foot .225m vs baseline .117m, gate .081m). Shuttle-only unshaded white + outline: corrected mask262/362 dark pixels menjadi0/362;30focused PASS,8before-after pasangan kamera sama/background PNG hash-identik. AI audio24cases/7tests baseline PASS, bukan suara baru/listening. Full regression wave3 Edit20/20,Play239/244;3additional failures corrected/tested6/6,2lunge failures retained. No whole-suite rerun; rekaman singles/doubles masih gagal50msclock. Baca hasil terbaru di [[SESSION_HANDOFF]]; checkpoint lama di bawah bukan source terbaru.

- D-062 (2026-11-23): eksekusi overhaul visual/karakter/arena. Plan 35 task di `.hermes/plans/2026-11-22_gameplay-visual-character-overhaul.md`. Baseline + backup 1.203 file selesai (`evidence/shareable/20261123/overhaul-005531/baseline.json`).
- D-061: plan overhaul disetujui; prioritas kualitas karakter + animasi di atas dekorasi. Stamina D-050 tidak berubah.
- T1a–T1d CLOSED_LOCAL_VERIFIED (`t1-closure.json`), final Edit19/Play194/visible22/pilot1 PASS. T2a tiga kandidat/nol eligible, `CHARACTER_BLOCKED_ASSET`; tidak integrasi/modeling ulang. T5a numeric216uniquePASS (net envelope/predictor) + singles RNG fixed; motion acceptance pending. T7e result/history locally closed. T5e stamina and T6e state current20Edit+212Play+6visible PASS (t5e-t6e-checkpoint1.json); historical File.Replace remains OPEN_INTERMITTENT. New clothed Quaternius candidate quarantined for mesh/rig/motion audit,not integrated.
- T5b physics current20Edit+220Play+1visible PASS,241unique (`t5b-checkpoint1.json`); ground/air/bots/snapshot/reset verified. Whole T5b motion/lunge pending; T5c numeric+freshcontrolled media verified, historical variance open. T5d targeted144cases and controlled media verified; fullPlay223/224 storage failure, natural overlap acceptance open. T6a runtime preview closed;T6c four defects CLOSED_LOCAL_VERIFIED,249unique PASS (visible3+Edit20+Play226),104PNG/source-bound checkpoint. Whole T6c layout/ergonomics partial. T6b framing localclosed:256executions/252unique PASS,64scenarios/768linequads0overlap,32highflights;whole shuttle readability pending. T6d score subset closed251executions/249unique,144PNG/checkerexit0. MarkerGREEN3/3 with80states;cyan teardown leak reproduced/fixed2cycles. Stronger visible3/3PASS,independent contrast112regions/340PNG verified;final Edit20+Play228+visible3PASS,251executions/249unique source-bound checkpoint. Continuous1x marker/point-flow remains. Earlier checkpoint counts above are historical.
- Career dicabut (D-054). Tunggal/ganda jalan.

## Gate yang masih OPEN

- Owner whole-module acceptance belum ada.
- Continuous-motion + audible review belum terverifikasi (butuh mata/telinga manusia).
- Save `File.Replace` IOException `0x80070497`: owner lock asli UNKNOWN, bukan fixed.
- Audio AV lag historis: OPEN_INTERMITTENT_NOT_REPRODUCED.
- Easy < 1000s stability: belum tutup.
- Native input, HP/device, emulator, public signing: NOT_RUN / dilarang tanpa izin.

## Aturan keras (dari AI_RULES + memory)

- Jangan `git push`. Commit lokal hanya dengan audit.
- Backup dulu: `.backups/<scope>_<timestamp>/` sebelum sentuh file.
- Tanpa dependency baru, tanpa biaya, tanpa cloud.
- Jangan edit `Assets/`, `Packages/`, `ProjectSettings/` selama Unity aktif.
- Keputusan desain butuh approval; perubahan teknis internal ekuivalen boleh dengan tes.
- PASS regresi/build bukan acceptance pemilik. Jangan klaim modul selesai.

## Catatan kerja

- [[Session-Log]] — log per task, terbaru di atas.
- Template log ada di dalam `Session-Log.md` (komentar HTML).
- Tiap task baru: baca MOC + Session-Log dulu, tulis hasil, baru lapor ke user.
