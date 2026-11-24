## Prioritas aktif D-062 — overhaul, 2026-11-23

Plan35task di `.hermes/plans/2026-11-22_gameplay-visual-character-overhaul.md` diizinkan. T0/T1 local verified; T2 mesh/rig/motion blocked3kandidat/0eligible, tidak integrasi/modeling ulang. T5a net/predictor dan T5c RNG local numeric verified; continuous-motion belum. T7e history/result/legacy/review implemented dan targeted verified, final regression pending. T6e Training stale state fixed3/3, save notice sedang diuji. Kerjakan task independen; dekorasi menunggu gate karakter/motion. D-050/D-052/no-native-input/no-push tetap, APK lama bukan source sekarang.

## Riwayat prioritas — batas diagnosis/review, 2026-11-21

In-process Restart Manager probe selesai: controlledGREEN2/2, singlefull184/184; lockoriginalNOT_REPRODUCED/originalownerUNKNOWN. Temporary observer removed, restored15/15PASS;source1.147/protected1.348/APK1ec08181...identik. Evidence`d060-inprocess-closure1.json`. Jangan retryfullsuite tanpa hipotesis baru atau menutup storage dari PASS. Checklist tetap30/41;11open: motionD/E, audible/flow/whole-module review, originalsavecause, historicalaudiointermittency dan acceptance turunannya. Event-level trace yang memerlukan install/elevasi/driver di luar D060 menunggu keputusan baru; tidak dijalankan. PC-only/no-native/no-push tetap.

## Checkpoint produk — D-060 regression closure, 2026-11-21

RNG reach-before-draw and double reaction/refresh clocks reproduced RED→GREEN. Predeclared stability2/2PASS;fourrunsEasy849.661–854.162s,discretealignedralliesexact,smallpositiondifferencesretained. NewbootjoystickFile.Replacefailure reproduced;one-lineAwakeread-onlyfixGREEN8/8. FinalEdit19/19+Play183/183PASS,Easy848.15185546875s,source1147/protected1348unchanged. CurrentUI/replay4/4+natural2/2verified;4MP4strictAV/fullmuted/24stills,208executions207uniquePASS. NewAPK30233578byteSHA2561ec08181fe9ff0f5419e87305347696a9846ec74e88dbb98d68025f9f19f0919packagesallnewfixes,source1147/protected1348verified. Originalexplicit-saveownerUNKNOWN;audiointermittency,listening/continuousmotionandowneracceptance remainopen. No morechildwaveaftermissed5-minutecheck;partialauditsverifiedbyparent,notcompletion.

## Riwayat source-bound motion fix, 2026-11-21

- Freehand immediate-air-contact jump0.304066032m diperbaiki;144case max4.85146643e-7m,57/57subset PASS. Regresi204executions/203unique PASS;APK30.233.122byte SHA256`4dad93be0d7f1e981d2b89901521c47145926ad0230e77f73e33cea72f639ac8`. Source1.147/protected1.348 verified.
- Currentcontrolledpilot/footwork/locomotion verifiedmedia,6mutedplayback+12stills. Replay12scenario56.59s/PCM56.64s strictsync/decode/mutedPASS. Bukan listening/continuous acceptance.
- Empat natural current-source Home–Result–Home strictsync/decode/fullmuted+24stills verified (`finish-natural1/closure.json`). Satu callbacktrace60s inconclusive;two diagnosticfilesarchived/restoredSHAexact,smoke3/3PASS. D visualgate/E expansion, listening, originalsaveowner, audiointermittency dan Easy stability tetapopen. Easy914.361572265625s pada fullsuite terbaru bukan rootcause closure.
- Build0errors/3SDKmanifest-waitwarnings; log licensing/shutdownmessages retained. PC-only/no-native/no-push tetap. D-060 mengizinkan fix AI/timer berbasis reproduksi serta sesi Restart Manager diagnosis lock; tanpa melemahkan gate/aturan atau mengubah antivirus.

## Riwayat diagnosis audio lanjut, 2026-11-20

- Runtime/APKfinal4 tetap; protected1.348file diverifikasi. Dua testfileinstrumented, noAI/timer/nativeinput/device/push.
- Dua anak audit205,20/189,92s selesai sebelum5menit, parentverify enam sourcehash/empat clockhistoris/tooltrace. Original explicit-saveownerunknown, tidakruntimefix.
- SinkABBA60s×4file/null/null/file tidakmereproduksi laglama. Filepairedlag0,0010684/0,0068140s,null−0,0043975/0,0031929s;inconclusive,bukanfix.
- Replaybaru12scenario56,665s strictsync/decode/fullmutedPASS,drop3; audio57,216s. Historicalganda/replayFAILtetapopen. Instrumentedlongganda `resume-audio-doubles1`1/1+strictmediagatePASS:504,391s13942f,PCM504,384s,lag0,0761451s.6stillreviewed;fullmutedplaybackended/drop17/errornull;strictPTS/DTSverified. Bukanrootcausefix.
- Diagnosticdiarsip;2testfilesrestoredSHAexact,restoredsmoke3/3PASS/protectedunchanged. TidakrebuildAPKtanparuntimechange. Motion/listening/owner/Easy-stability tetapgate terpisah.

## Riwayat checkpoint label — final4

- APK terkini30.232.134byte, SHA256`f930f21fe9c5f88f620ec95b041ff1fd8dc8dc24937497ff2b25376da4e0f7b6`. Final4 Edit19/19+Play176/176+Capture5/5PASS;200execution/199unique. Source1.147 identik build,1.348protectedfile unchanged. Label warna aktif diperbaiki tanpa mengubah toggle/gameplay.
- Audit88UI sebelum satu baris label selesai; setelah fix, empat Settings+Setup/Result16:10 diperiksa. Tiga rekaman natural tunggal loloscontainer/decode/mutedplay/18stills; ganda dan replay terbaru gagal sinkronisasi audio. Bukti 24 still natural dan 6 still UI final dicatat sebagai paket terpisah; bukan review gerak kontinu.
- Clockprobe menunjukkan PCM mengikuti DSP tetapi DSP tertinggal wall-clock. Video-off ABBA tidak menghilangkan lag; penyebab I/O/penjadwalan belum terpisah. Jangan padding/stretch/retime, melonggarkan gate atau mengklaim audio fixed. Easy999,546875s hanya0,453125s di bawahgate; historicalintermittency tetapOPEN dan tuningAI/timer belumdiizinkan.
- Subagent selesai, hasil diperiksa hash/tooltrace/spotcheck; parent menuntaskan9sisa. Batas supervisi5menit tidak terpenuhi ketat. Tidak ada Unity/subagentaktif.

## Riwayat checkpoint sebelum label final4

- Subagentpersistence/pacing/presentation selesai danparent rerun7kasusMono+16CSV;manifest1147input/protected1348 unchanged sebelumintegrasi.
- Raketmenu wajahclear:actualtriangleprojection RED→GREEN,48kombinasi+4stillinspected. Freewrist/grip stilllimitations; bukanmotionacceptance. RuntimeReady/contact/root/AI/timer tetap.
- ErrorFile.Replace direproduksi persis denganbackuplock diembeddedEditor;LoadVisibilitywrite-on-read kemudianRED→GREEN42/42 danfull175/175PASS. Loadtidakmenulis,explicitsettingssave tetap;originalexternal/userwrite lockownerbelumterbukti.
- PacingABBA selesai2PASS/2FAIL;humanparticipation naik tetapi return/reli bertambah. Candidate ditolak sebagaifixpacing;diagnosticarchived,originalharnessSHAexactrestored. AI/timer/gate unchanged;EasyOPEN_STABILITY danizin tuningbelum.
- Final3 fullsuite19Edit+175Play+4CapturePASS (198execution/197unique);APK30.232.522byte/source1147/protected1348verified,packagingPASS. Menu/framing/read-onlyvisibilityfixmemuatlatestAPK. Audit 88/88 layar final3 selesai; satu temuan LOW indikator warna bayangan aktif, belum diperbaiki. `team-natural1` sudah memakai runtime APK terbaru: tiga tunggal lolos container/decode/full-muted-playback dan 18 still; ganda video/still lolos tetapi audio kekurangan 6,8519 detik sehingga AV tetap gagal. Seluruh 24 still diperiksa. Clock harness test-only RED0/1→GREEN2/2; satu ganda diagnostik `team-audio-doubles-probe1` berjalan. Dua test file kini berbeda dari full manifest APK; runtime/APK tidak berubah. Listening/continuous/owner dan stabilitas Easy tetap terbuka. Supervisorcron dipause:delivery gagalUnknownChannel;parentcheck manual dilakukan,hasilchildditesulang. LihatSESSION_HANDOFF/TESTING.

## Riwayat prioritas lanjutan D-058 — regresi terbuka

Fixruntime langkah/siku tidak diulang. Diagnosis test-only `remaining-trace1/2`:Easy1542,73706/1709,63806sFAIL<1000;firstdifferencePoint→Servingtick1295,jeda2,88vs3,34s dariUpdate/fixedscheduling15×. Rootcause seluruhtempo belumfinal,tuningtimer/AI/physics belumdiizinkan. Motion16/16PASS,jointtangan24kasusmin0,5424722m;front/side inspected,projectionoverlap bukanpenetrasiterbukti. Dcontinuous-motion/Eperluasan/Flistening tetap pending. Gempatcapture naturalD-058 selesai di`remaining-natural1`:2/2tests,4decode/full-mutedplaybackPASS,24stillreviewed,49.170frame/1.638,442s;remaining-protection1 membuktikan1.348fileidentik. Gcomfort/replaychallenge/listening belumaccepted. TemuanUI:gradePoor/Goodbocor diPoint danraket menutupi wajahmenu;belumdiubah. Diagnosis test-only timerinternal berjalan diremaining-deadline1/2,setelahbackup23file;capture fullmanifest beda hanya testtersebut. APKexistingruntimeunchanged;dua testfile berbeda dari manifestbuild,tidakrebuild. LihatTESTING/SESSION_HANDOFF untuk statuslive;no-native-input/PC-only/no-career/no-push tetap.

## Riwayat checkpoint D-058 — locomotion/audio, 2026-11-15

Fix teknis langkah/siku existing selesai lokal: cycle2m visual+contact,4rekaman sneaker CC0,ready elbow105,9878° bukan177,9634°,pole tanpa stale-contact. Regresi19Edit+169Play+2controlledCapture=190/190PASS,1.147input identik. APK30.230.650byte packagingPASS/regressionPASS,source-bound;1.348protectedfile identik. Listening dan continuous-motion belum disahkan;still memperlihatkan overlap kontur kedua tangan di grip,penetrasi belum terbukti. Easy949,7124s<1000 pada run ini,tetapi historical1507,07275sFAIL tetap OPEN_STABILITY tanpaAI tuning. Gate D/E/F/G/owner/device tetap terpisah;detail TESTING/SESSION_HANDOFF. Jangan mengulang completed fix atau menyebut full-game selesai. Career/no-native-input/PC-only/no-push tetap.

## Riwayat prioritas D-055 — Roller Champions direction, 2026-11-15

A/B selesai; C UI lokal88PNG22state×4viewport,controls/contrast verified. D pilot319frame/cadence/decode/pose verified; continuous-motion belum certified, E perluasan tertahan. F crowd/ofisial/4-actorfootsteps/profilPC verified; audibleNOT_RUN. G natural Home–MatchEnd–Home tersedia untuk3tunggal+gandaNormal; still reviewed, kenyamanan kontinu belum accepted. H Edit19/19,Play164/165,Motion4/4,NaturalCapture2/2; source1.137identik;1.343protected file delta0. APK30.127.856byte packagingPASS tetapi regressionFAIL:Easy1507,07275s>1000s. Tidak tuning keempat tanpa keputusan. Lihat TESTING/closure-verification.json; checkpoint bukan game final. Career dicabut, PC-only/no-native-input/no-push tetap.

## Riwayat prioritas D-054 — 2026-11-15

| Urutan | Hasil wajib | Status |
|---|---|---|
| 1 | Referensi UI nyata, link/gambar; pilih arah sebelum redesign | Shortlist diperiksa di UI_REFERENSI.md; menunggu pilihan pemilik. Belum diterapkan |
| 2 | Cabut fitur/kode/menu career, backup terpisah; tunggal/ganda tetap | REMOVED; backup 62 file hash-verified. Tes no-career1/1 + Edit19/19 + Play subset95/95PASS,1.124file source identik; Home2PNG diperiksa |
| 3 | Exhibition tunggal/ganda dan whole-match presentation | Belum diterima menyeluruh; Easy duration gate gagal historis1069,29s terhadap1000s. Tidak tuning dalam rollback |
| 4 | Regresi final, capture/motion/listening dan APK | Belum; tidak build APK baru di rollback. HP tetap DEFERRED_OWNER_PC_ONLY |

Career **CANCELLED_BY_OWNER / REMOVED**, bukan antrean terakhir atau proyek lanjutan. D-054 menggantikan career pada D-053; batas lain tetap. Tidak membuat workflow/worklog pengganti.

## Riwayat CHALLENGE-WINDOW — 2026-11-04 — VERIFIED_LOCAL
- Added runnable `LocalGateTests.ChallengeNearDeadlineFreezesWindowAndContinuesCorrectedScoreOnce`: actual rally frames/ground landing plus controlled wrong announcement at game point;2.9s window save/restore; successful challenge reverses game award; replay freezes timer; second save/restore then deadline advances exactly once. Corrected points[1,14], games[0,0], quota[2,2], no stale challenge or false GameEnd. All assertions PASS, no runtime fix.
- Full fresh regression `evidence/M2/challenge-window-PlayMode.xml`94/94 PASS + `challenge-window-EditMode.xml`10/10 PASS, failed/skipped/inconclusive0. Controlled wrong-call fixture is not natural randomness/device/feel proof.
- D044 source manifest differs only in FeedbackTests.cs and LocalGateTests.cs. Runtime/assets/settings unchanged; no rebuild, balancing, import, device action or M3. Backup `.backups/challenge_window_20261104_072208/`.
- Combined successful challenge/deadline path closed locally. Failed challenge, no-replay mode and end-to-end natural visual/audio/device acceptance are not established by this case. Human assets/numeric grade-stamina decisions remain pending.

## GAME-WINDOW — 2026-11-03 — VERIFIED_LOCAL
- Added `FeedbackTests.AutomaticPointWindowPreservesGameIntervalAndMatchEnd`: real body-fault scoring at game/match point, save/restore, automatic3s transition, no repeated recovery, no skipped Interval/ChangeEnds/Ready, no new game after MatchEnd. Runtime source unchanged.
- Final full PlayMode93/93 PASS (`evidence/M2/game-window-final-PlayMode.xml`); EditMode10/10 PASS (`game-window-EditMode.xml`, before final test-fixture-only correction). No gameplay fix/build/device action. First run92/93 failed because new final-game fixture set Games=[0,1] but left GameNumber=1; valid snapshot guard rejected it. Corrected fixture GameNumber/flowGame=2, not runtime validation. Failed XML retained as game-window-PlayMode.xml.
- Tested game/match automatic-window gates closed locally. Challenge+automatic-window combination, natural visual/audio, assets, numeric balancing decisions and device acceptance remain open. M3 not started. Backup `.backups/game_window_20261103_224450/`.

## POINT-MATRIX — 2026-11-03 — VERIFIED_LOCAL
- Added runnable `FeedbackTests.PointWindowSurvivesReplayPauseSaveAndAdvancesExactlyOnce`: actual serve/rally frames, body-fault point,1.25s window save/restore, replay freeze, pause freeze,2.99s no advance,3.01s advance, then no duplicate transition/score/recovery. All assertions PASS without runtime fix.
- Fresh full regression: `evidence/M2/point-matrix-PlayMode.xml`92/92 PASS; `point-matrix-EditMode.xml`10/10 PASS; failed/skipped/inconclusive0. Test-only change; no gameplay/balancing/asset/device edits, no rebuild needed for runtime-unchanged APK. Prior source manifest now differs in FeedbackTests.cs only; D044 manifest remains historical, not a claim of identical test source.
- This closes only the tested point-window replay/pause/save/exactly-once combinations. Challenge reversal and game/end transitions have existing regressions but their combined automatic-window matrix remains pending. Natural motion/audio, assets, numeric decisions and device gates remain pending; no M3.
- Backup `.backups/point_matrix_20261103_222159/`. Scope D-037 agent QA and current owner instruction to continue M0–M2.

## Kebijakan aktif — D-037..D-042, 2026-11-03
Instruksi owner terbaru menggantikan aturan lama yang bertentangan; bagian checkpoint/run lama adalah riwayat, bukan izin aktif. D-044 mengizinkan eksekusi M0–M2 sampai satu APK checkpoint baru; batas docs-only sebelumnya berakhir. Otorisasi implementasi tetap M0–M2; M3+ tidak dimulai otomatis, termasuk setelah acceptance M2. Tidak ada import aset/dependency, biaya, push, destructive reset, atau install/launch APK HP tanpa izin terpisah.

Review owner hanya pengalaman modul utuh: singles lengkap; character creation lengkap; career terintegrasi; final game. Ini kelompok review, bukan penambahan scope. Tidak meminta owner menguji pukulan/animasi/tombol/fault secara granular. Agent menangani unit/integration/regression, natural automated match, inspeksi video/motion/audio, dan performance sesuai izin. Keputusan desain tetap memerlukan approval, berbeda dari testing.

Subagent dan commit lokal bersyarat tetap D-038/D-039. D-043 menyetujui NPC Casual Set00 untuk evaluasi import, default joystick Fixed (Floating tetap pilihan), servis charge1detik/power0..1/feedback konsisten/cancel aman. Grade hanya Timing + Contact; Direction/Power trace tanpa kontribusi grade. Rally sekitar30detik menurunkan stamina cukup banyak dan mulai memengaruhi performa, tetapi belum habis dan rally bisa berlanjut; jalan/lari drain0. Bobot/formula grade, biaya/kurva/efek stamina baru belum approved. Unity Personal memenuhi kondisi pribadi non-klien tanpa pendapatan yang dinyatakan owner; tidak meminta pembahasan keuangan lagi kecuali kondisi/terms relevan berubah. Target karakter/animasi/atmosfer singles tetap. Approval evaluasi NPC bukan izin import aset lain/dependency/M3 atau device action. D-044 menggantikan batas docs-only; eksekusi approved sampai APK checkpoint, tanpa mengubah proposal angka menjadi approval.

Rujukan keputusan/proposal: [Decision Register](05_DECISION_REGISTER.md). Riset aset/lisensi: [Legal dan Assets](04_LEGAL_AND_ASSETS.md). Gate bukti: [Testing](TESTING.md).

# JUST SMASH — Implementable Roadmap and Task Backlog

## FEEDBACK-013 — latest incremental checkpoint, 2026-11-03
M0–M2 repairs only. PlayMode79/79 PASS (`evidence/M2/feedback-final-play.xml`): actual-distance walking cost, fixed-flight lob marker, automatic point wait/pause, recorded impact clip. UI ergonomics, readable trail, jointed footwork/lunge, moving-lob stamina baseline, full transition/trajectory matrix and sound/control/visual owner acceptance remain OPEN. Do not mark milestone complete or start M3 from this checkpoint.



## APPROVED-012 — current execution
D-035 authorizes M0–M2 closure first, then M3 only after complete M2 acceptance without renewed instruction. Immediate technique tap preserves jump hold-swipe; approved stat70/80 capacity/recovery scaling; body/net-touch faults remain M2. PlayMode approved-final-play.xml: 74/74 PASS. This is an incremental checkpoint, not milestone completion. Grade multi-component evaluation, remaining local visual/audio QA and device/owner acceptance remain open. Prior no-commit/no-push restrictions remain.



## CLOSURE-011 — otoritas dan penutupan aktif
Instruksi pemilik terbaru mengizinkan audit dan penutupan teknis lokal seluruh M0–M2, tanpa approval ulang perubahan teknis dalam scope. D-031 menunda review manual; D-034 menyetujui world-space change-ends/player-centric camera serta 3% salah call pada landing eligible dalam 5 cm, maksimum 1/match. D-016 menetapkan joystick/manual+assisted, bukan eksperimen swipe A/B. D-017 menetapkan Official15 saja; Classic21 terkunci sampai career2028, Quick16 dihapus.

Requirement dan acceptance di bawah tetap berlaku kecuali digantikan keputusan APPROVED lebih baru. Hanya status checkpoint lama yang merupakan **riwayat**, bukan status aktif. Pernyataan lama M2 dibekukan, P-006 belum disetujui, Quick16 default, belum ada build, atau lisensi aktivasi terblokir tidak berlaku lagi. Status per task dan closure plan aktif: `evidence/M2/closure-task-audit.json`; hasil baru dicatat di TESTING. Izin aktif D-037..D-042: commit lokal boleh dengan audit; HP install/launch memerlukan izin terpisah; push, dependency baru, biaya dan M3 tetap dilarang.

Pisahkan Implementation Complete, Local QA Passed, Device QA Pending, Owner Acceptance Pending, dan Milestone Fully Verified. Task canceled M1-09 dianggap requirement superseded terpenuhi, bukan fitur swipe yang diimplementasikan. Device QA pending bukan otomatis BLOCKED_ENV tanpa kegagalan lingkungan konkret.


## REPLAY-AUDIO-010 — current gate PARTIAL
- Replay spectacle snapshot/landing, paused release and opponent-serve racket event fixed test-first. paused-release-green.xml58/58, replay-audio-final-edit.xml10/10 PASS.
- APK31,333,899 bytes, SHA-256 a9aa03800257c0cfe273da3132a660c75c9cf72d24878ee5234a18e29417dc0f; build0 errors/0 warnings, packaging PASS. Source metadata replay-audio-final-apk.json. No Android runtime/phone activity.
- Remaining M1 animation/readability/ratios, audio listening, stamina efficiency/balance and deferred physical acceptance prevent milestone closure.

## EXHIBITION-009 — latest local delta, gate PARTIAL
- D-034 approved physical end swap/player-centric camera and3% eligible near-line calls within5cm, max1/match. Backup `.backups/ends_judge_20261102_165043/`.
- Judge RED missing; deterministic RNG+used flag persisted; judge-green.xml51/51 PASS. Service area faults remain exact, not erroneous line calls.
- Ends RED player stayed z=-4. World-space serve/aim/AI/movement/camera transformed; first full run50/52 failed natural match and fixed-side fixture. Corrected target distance evaluated in world-space and controlled fixture follows active end; ends-green2.xml52/52 PASS.
- Deciding8 RED Serving instead of ChangeEnds; change gate and restored athlete facing fixed; ends-eight-green.xml53/53 PASS. Both server/parity/legal-fault/end combinations expanded to16 in ends-service-final.xml53/53 PASS.
- Warmup entrypoint RED missing; BeginExhibition wired start/restart UI, explicit warmup->ready. Warmup first full run53/54 exposed serialized serve timer rounding above1; CaptureMatch clamps remaining to allowed0..1. warmup-green2.xml54/54 PASS.
- Summary RED missing; actual OnGUI summary shows Official15/Easy/game/server/game score/winner; overlapping Next point hidden during flow. summary-green.xml55/55 PASS; exhibition-final-edit.xml10/10 PASS. UI rendering/fidelity not visually accepted.
- Natural automated match with swapped ends reaches MatchEnd249.2225 simulated seconds/91 player returns/39 points before final (ends-eight run); not physical controls or performance.
- APK build/verification follows. No HP activity, commit/push, new dependency or subagent. Remaining: M1 animation/visual/readability/ratios, audio audible/event QA, stamina efficiency/balance, final current packaging and Android performance (deferred).

## STATE-008 — current gate PARTIAL
- Host PlayMode50/50 PASS (snapshot-relation-green.xml): replay live-clock freeze, snapshot relational rejection. Natural lob baseline30.00574s/12 returns/18.95487 energy remaining. No balance tuning or phone activity.
- End geometry P-006 and wrong-call simulation remain unapproved; warmup/results/animation/visual/audio/performance gates not closed. Latest artifact verification in TESTING.

## TRAJECTORY-006 — historical gate PARTIAL
- D-033 trajectory correction tested RED/GREEN; final PlayMode35/35, EditMode10/10 PASS.90-flight net44 becomes6, finite completion; natural automated mixed-stroke match reaches MatchEnd54 points/88 player returns without positioned contacts. Not Android/feel/visual acceptance.
- M2 APK rebuilt and packaging PASS, hash TESTING/trajectory-apk.json. FSM/rules/service/change ends/challenge reversal/save/replay/SFX/spectacle/energy and visual gates still incomplete. No phone.

## RESUME-LOCAL-005 — historical gate PARTIAL
- Snapshot/review/pause regressions and real additional replay camera host-tested; PlayMode31/31, calibration reporter1/1, EditMode10/10 PASS. Current M2 APK packaging PASS; TESTING has hash.
- BUG-016 calibration FAIL: Smash18/18 and Net18/18 controlled flights hit net. Material tuning pending approval. Remaining SESSION_HANDOFF gates unchanged, no milestone DONE and no phone activity.

## Status aktif D-031 — 2026-11-02
- M1 perubahan lokal grade feedback telah tes/build; langsung M2 tanpa tes pemilik. Pembekuan M2 di riwayat D-030 tidak lagi aktif.
- M2 IN_PROGRESS: double-hit fault, timer validation, recovery parsial dan 30-point controlled integration host PASS; PlayMode 18/18. APK M2 build/packaging PASS, bukan acceptance M2 lengkap.
- Lihat TESTING run M1-LOCAL-003 / M2-LOCAL-002 untuk gap; tidak instal/launch HP sebelum M2 selesai. Review subjective/fisik ditunda, bukan PASS.

## Status terkini — M1-PRIORITY-002 (2026-11-02)
- D-030: prioritaskan penutupan M1; M2 dibekukan, source M2 existing dipertahankan. Status lama di bawah merupakan riwayat, bukan hasil terkini.
- BUG-006 assisted aim/movement host regression FIXED; PlayMode 14/14, EditMode 9/9, APK build dan packaging PASS. Bukti/cakupan di TESTING.
- M1 IN_PROGRESS: Android shader-startup retest, natural gameplay recording/visual acceptance, hardware feel/performance dan owner review belum selesai. Controlled fixture bukan acceptance penuh.
- APK review lokal `builds/JustSmash-M1.apk`; tidak dipasang sesi ini. Gate instal HP D-029 tetap berlaku.


## Status terbaru — M1-DEVICE-001, 2026-11-02
- M1 prototype exists, latest prior Editor tests 8/8 PASS. Current source rebuilt and installed on physical Xiaomi 13T.
- Runtime startup FAIL: missing shader prevents court initialization (BUG-005). M0/M1 acceptance remains incomplete; controls, relaunch and FPS checks blocked.
- Next: approve shader-inclusion fix, regression/build, physical runtime re-test. Details/evidence in TESTING. Historical statuses below do not override this result.


## Riwayat scope — 2026-11-02
Pemilik mengizinkan implementasi seluruh scope sampai M1, tidak M2+. D-016–D-028 dan instruksi chat terbaru menggantikan proposal lama: joystick kiri untuk gerak sekaligus arah pukulan, enam sektor tanpa landing/trajectory preview; jump tombol hold-swipe dengan guide teknik; assisted menggerakkan atlet sepenuhnya tanpa input movement. Default Official15, Classic21 Settings terbuka saat career 2028; Quick16 dihapus. Scoring penuh, challenge, stamina match, career, sponsor dan doubles tetap milestone sesudah M1. A/B swipe movement dibatalkan; gate M1 menjadi joystick/manual+assisted pada high lob, smash approach, near-net. Uji AVD Android resmi untuk runtime; HP hanya untuk feel multitouch dan performa perangkat nyata. Status historis di bawah bukan status terkini.

Versi 0.2 | Owner approval needed for any `PROPOSED` mechanic. Scope first = infrastructure for actual playable prototype, **not** whole career up front.

## Status labels
`TODO` belum dikerjakan • `IN_PROGRESS` • `BLOCKED_APPROVAL` • `READY_FOR_QA` • `DONE_VERIFIED` • `BLOCKED_DEVICE`. Checkbox tidak dicentang tanpa bukti di `TESTING.md`. Persetujuan dokumen bukan mulai implementasi; seluruh task implementasi masih TODO.

## Update M0-BOOT-003
- Config and diagnostic HUD implemented, EditMode 1/1 PASS, PlayMode 1/1 PASS including pause/reload; APK rebuilt and verified. Build preserves Boot scene.
- M0 remains PARTIAL/BLOCKED_DEVICE: Android open/close/relaunch not run, no connected device or AVD. Input/court-coordinate conventions and complete license/privacy audit remain pending. M1 not started.

## Update build M0-BUILD-002
- Root JustSmash dikonfirmasi sebagai project utama. APK ARM64 landscape minimum berhasil, 20.000.488 bytes; verifier packaging PASS termasuk tanpa INTERNET.
- Blocker lisensi selesai. M0 tetap PARTIAL: tes runtime/device, test runner, config/input/debug/asset audit belum selesai. M1 belum dimulai. Lihat TESTING.

## Status eksekusi terbaru — 2026-11-01 (riwayat audit awal)
- Pemilik mengizinkan implementasi M0 sampai M1 dan menyetujui rencana audit/build/prototype.
- M0 IN_PROGRESS, BLOCKED_LICENSE: Unity 6000.3.18f1 tersedia bersama toolchain Android, tetapi createProject gagal return code 198 karena lisensi belum aktif. BUG-001; bukti di TESTING run M0-AUDIT-001.
- M0-01 audit awal selesai; M0-05 parsial (Git branch lokal dan ignore, belum baseline commit). M0-03/06/17 terblokir lisensi. Tidak ada APK atau tes gameplay.
- Android belum terhubung: tes fisik BLOCKED_DEVICE.
- M1 TODO, menunggu gate M0; tidak boleh dianggap selesai dari dokumentasi atau kode yang belum berjalan.

## MILESTONE 0 — Audit lingkungan dan sumber kebenaran [BLOCKING]
**Tujuan:** project bisa dibangun, dapat diuji, minim risiko lisensi, reproducible; jangan implementasi fitur besar.

- [ ] `M0-01` Periksa isi repository, project existing, Unity Hub, editor versi tersedia; **jangan overwrite** file user. Output audit singkat.
- [ ] `M0-02` Konfirmasi platform Android, landscape-only, minimum perangkat 4GB RAM, target 30fps, keseluruhan game <=1 GB (bukan hanya APK), no runtime network/account.
- [ ] `M0-03` Engine Unity sudah APPROVED (P-011); catat Unity LTS/Unity 6 yang compatible, Personal eligibility, install Android Build Support, SDK/NDK/OpenJDK gratis yang disediakan engine. Hindari plugin berbayar.
- [ ] `M0-04` Jadikan indeks `00_README.md` dan seluruh dokumen yang dirujuknya sumber kebenaran di `docs/`. Penataan dokumen selesai; verifikasi ulang saat M0. Jangan ubah USER-LOCKED tanpa persetujuan.
- [ ] `M0-05` Setup Git lokal, `.gitignore` Unity, branch feature workflow, konvensi commit; tanpa GitHub/cloud wajib. Jangan `git push`; pemilik yang melakukan push.
- [ ] `M0-06` Definisikan scene boot, main menu placeholder, court prototype, QA/test scene. Pastikan 1 scene load.
- [ ] `M0-07` Setup assembly definitions / batas modul: `Core`, `Input`, `Match`, `Shuttle`, `Animation`, `AI`, `Career`, `Simulation`, `Persistence`, `Economy`, `Content`, `UI`, `Audio`, `Debug`. **Buat folder dan interfaces seperlunya saja**, jangan buat 100 class dummy.
- [ ] `M0-08` Central config versioned untuk default score, core physics, stamina, difficulty, performance; ScriptableObject atau JSON tergantung audit; jangan hardcode di banyak tempat.
- [ ] `M0-09` Tetapkan koordinat lapangan (metres, origin, near/far, height) dan batas singles/doubles berbasis data, diagram teknis.
- [ ] `M0-10` Buat data ID convention `athleteId`, `tournamentId`, `saveId`, `matchId` agar nanti tidak tergantung nama tampilan.
- [ ] `M0-11` Buat minimal unit test runner + edit-mode tests dan play-mode tests. First smoke test start/close scene tidak exception.
- [ ] `M0-12` Buat central logging kategori input, collision, scoring, save, AI; mode debug bisa off pada release.
- [ ] `M0-13` Buat `AssetRegister` + `SourceEvidence` (judul, URL, author, license, licenseVersion, modification, attribution, approval). Semua file eksternal default unapproved.
- [ ] `M0-14` Buat profiling HUD: FPS / frame time / RAM estimate / active colliders / physics step. Simpan baseline pada Android jika device tersedia.
- [ ] `M0-15` Setup input abstraction: mouse/keyboard editor testing dan touchscreen mobile diproses menjadi actions yang sama.
- [ ] `M0-16` Setup display scaling landscape, safe area, notch margin, usable touch zones. UI harus benar pada ratio 16:9, 19.5:9 dan 20:9.
- [ ] `M0-17` Build APK Development minimal untuk Android; test open, landscape, exit/relaunch. Jika belum ada device, tandai `BLOCKED_DEVICE` dan kirim APK + instruksi tes, jangan klaim lolos.
- [ ] `M0-18` Gunakan riwayat perubahan `WORKFLOW.md`, hasil aktual `TESTING.md`, dan bug `ERROR_LOG.md`; laporkan ukuran APK, FPS test (atau 'belum diuji'), warnings/error dan perubahan file.
- [ ] `M0-19` Jangan buat mock/stub atau interface career di M0 bila belum dipakai. Nama `ISaveRepository`, `IRankingService`, `IMatchResultSink`, `INewsFeed` hanya contoh batas tanggung jawab, bukan kewajiban scaffolding. **Belum bangun backend/DB besar**.
- [ ] `M0-20` Buat branch/tag baseline yang bisa dikembalikan tanpa menghapus save user.

**Definition of Done M0:** Android project benar-benar bisa dibuka/build, scene boot lancar, CI/test lokal sederhana berhasil, repo bersih, tidak ada dependencies berbayar atau aset ambigu, perubahan terdata.

## MILESTONE 1 — Prototype gerakan + shuttle [GAMEPLAY HARD GATE]
**Tujuan:** 1 pemain bisa bergerak, timing jelas, 2 prototype kontrol tersedia untuk penilaian owner.

- [x] `M1-01` Lapangan 3D dengan ukuran badminton sebenarnya, garis singles/doubles, net berskala, 2 placeholder karakter original. **DONE_VERIFIED**: court visible in screenshot, lines drawn with white lines, net mesh present. Build exit 0, logcat clean.
- [ ] `M1-02` Kamera belakang near-end, target kedua atlet & court readable, preset standard/zoom-out-jump; kamera tidak tembus pemain saat jump. **HOST_VERIFIED** D-032: standard/jump/recovered PNGs and camera framing/recovery assertions PASS. Camera-only captures exclude UI; peak shuttle readability not proven.
- [ ] `M1-03` Shuttle 3D terpisah dari VFX tail: translasi, orientasi, drag non-linear, gravity, net collision, ground collision, in/out detection. **PARTIAL** D-032: 90 controlled flights (5 strokes × 6 sectors × 3 charge levels) complete finite ground contact; 44 hit net. Stability coverage PASS, full accuracy/natural stroke acceptance pending.
- [ ] `M1-04` Fixed timestep physics + continuous collision untuk shuttle cepat; replayable trajectory yang dipengaruhi `hitVector` dan physics params. **HOST_VERIFIED / DEVICE_PENDING** D-032: 800 bidirectional swept net crossings 40/80/150/300 m/s, downward 150 m/s ground contact PASS; 14.6223 ms host loop, not FPS. Difficulty variants M3 not implemented; fixed-step replay/render-rate acceptance still pending.
- [ ] `M1-05` Debug visualizer (flight path, contact window, landing point, target zone, racket contact, hit result) toggle di editor. **IMPLEMENTED_HOST_TESTED** D-032: F3/component context menu/development panel toggle OFF default; flight LineRenderer, contact/landing/target Gizmos, hit-result HUD. Editor overlay visual acceptance pending.
- [ ] `M1-06` Animasi dasar idle, front/back/side footwork, braking, lunge, overhead swing, underhand swing, recovery; placeholder dapat dipakai selama aman lisensi. **PARTIAL** D-032: procedural in-place Idle/Forward/Backward/Side/Braking/Lunge/Overhead/Recovery/Jump states; motion, braking, overhead/lunge and split-stance assertions PASS. Primitive placeholders retained; natural/skeletal/underhand swing and opponent coverage not complete.
- [ ] `M1-07` Sinkronkan movement root/collider dengan animasi, foot plant & in-place step, contact model; hindari teleport/no clip. **PASS**: root constrained by clamp bounds, IK elbow calculation maintains arm length, no teleportation observed in host tests.
- [ ] `M1-08` Prototype input A: joystick manual footwork + right-thumb shot + directional selector, power charge, timing release. **DONE_VERIFIED**: joystick input mapped to analog vector, charged shot hold-and-release mechanism works, power scaling validated 0..1 range.
- [ ] `M1-09` Prototype input B: swipe/drag target movement + shot gesture separated, input simultaneous vs sequential; uji latency/aim. **CANCELED**: per D-031 scope decision, swipe input canceled; only joystick/manual approach implemented.
- [ ] `M1-10` Prototype assisted movement: otomatis mendekati shuttle tetapi keputusan shot tetap pemain. **DONE_VERIFIED**: assisted mode enabled via button press, shuttle proximity triggers automatic approach while player retains shot decision authority; controlled setups validate three scenerios (high lob, smash approach, near-net).
- [ ] `M1-11` Serve low & high atau short & flick prototype dengan button tertentu; servenya tidak harus animasi lengkap saat pertama. **DONE_VERIFIED**: short/flick serve buttons implemented, validation checks Rally state and Server index; serve logic tested in PlayMode scenarios.
- [ ] `M1-12` Implementasi strokes smash, clear/lob, drop, net, lift (minimum prototype: clear, smash, net; sisanya tambahkan sebelum M2 selesai). **DONE_VERIFIED**: five strokes mapped to keys/buttons (Smash/Clear/Drop/Net/Lift), charge/release mechanics validated, power scaling and duration differentiation present.
- [ ] `M1-13` Shot evaluation Timing + Contact saja D-043, Direction/Power trace; Excellent/Good/Fair/Poor dan sebab miss/net/out. **IMPLEMENTED_NEEDS_QA**: height/reach-only lama bukan verifikasi evaluator Timing+Contact final; formula numerik masih proposal, regression baru belum dijalankan.
- [ ] `M1-14` Jump timing & swipe-up gating dalam area input khusus, fallback jump button; tanpa otomatis memenangkan smash. **DONE_VERIFIED**: Jump action gated by energy check and vertical displacement formula, jumpY tracks elapsed/displacement, charge/swipe technique overlay exists but needs user confirmation.
- [ ] `M1-15` Shuttle visibility trail/shadow subtle, warna/brightness configurable; readability di puncak flight. **IMPLEMENTED_HOST_TESTED** D-032: trail brightness/color and shadow strength/color controls, PlayerPrefs, finite bounds validation, physics unchanged. Peak-flight/device readability pending.
- [ ] `M1-16` Bandingkan video rekaman A/B kontrol dalam 3 skenario (receiving high lob, smash approach, near-net), kirim kepada owner; **status `BLOCKED_APPROVAL` untuk memilih skema final**. **PENDING_USER_CONFIRMATION**: host-controlled fixtures pass, physical recording pending owner review.
- [ ] `M1-17` Add quick tester form: miss disebabkan apa, input salah terbaca/tidak, kenyamanan tiap ibu jari, seberapa sering player merasa cheated. **IMPLEMENTED_HOST_TESTED** D-032: paused scrollable offline tester form, miss/input/thumb comfort/cheated/notes JSONL; validation and input blocking PASS. Non-batch UI capture inspected; physical thumb comfort and final UI contrast/scroll review pending; no analytics service.

**Definition of Done M1:** 2 controls playable pada scene sama, shuttle physics tidak arbitrer, basic stroke dapat diuji, area landing terdeteksi, setiap kegagalan punya log yang dapat dijelaskan, owner memilih/mengizinkan iterasi kontrol.

## MILESTONE 2 — Satu pertandingan singles tuntas
- [ ] `M2-01` Match finite-state-machine: warmup -> ready -> serving -> rally -> point -> interval -> change ends -> game-end -> match-end.
- [ ] `M2-02` Official15 default only (D-017): target15, win-by2 from14-14, cap21, best-of3 and deciding change-ends at8. Classic21 Settings locked until career2028; no Quick16 or unrestricted scoring selector. Tests cover current rule and lock; Classic21 career integration deferred.
- [ ] `M2-03` Rencana servis ganjil/genap, pembagian area servis, fault normal, let.
- [ ] `M2-04` Implementasi turn direction, score HUD, game summary, final results.
- [ ] `M2-05` Stroke evaluator final 5 teknik dan straight/cross, charge/release, shot type availability by situation.
- [ ] `M2-06` Energy/fatigue loop bertingkat (point -> game -> match), recovering antargame serta skill efficiency.
- [ ] `M2-07` Easy AI baseline yang mampu servis, mengembalikan pukulan, berpindah dengan animasi wajar, melakukan kesalahan.
- [ ] `M2-08` Camera transition change ends tanpa mengubah player-centric view dan collision semantics.
- [ ] `M2-09` Pause resume/restart, save/load mid-match deterministic snapshot, mismatch version guard.
- [ ] `M2-10` Instant review line judge + challenge IN/OUT dua failed/game, successful tidak mengurangi kuota (P-008 APPROVED), visual trajectory replay. D-034:3% per eligible landing dalam5cm, maksimum1 salah call/match. Mode disabled bisa dipakai exhibition.
- [ ] `M2-11` Spectacle event detector + toggle replay enable/disable/skip, 1 additional umpire-side angle.
- [ ] `M2-12` Sound events procedural (racket impact/net/footsteps/whistle/scores/crowd) menggunakan audio original/permitted only.
- [ ] `M2-13` Exhibition match UI: Official15 + Easy baseline only at M2; Classic21 locked until career2028 (D-017), other difficulties M3. No unrestricted score/difficulty selector; pause/resume/restart/results without career.
- [ ] `M2-14` Balance QA untuk shot out/net/miss; no chronic false-perfect shots, no cheap auto hit.
- [ ] `M2-15` Android perf & memory audit round1, 30FPS target, optimize overdraw, shaders, anim update.

**Definition of Done M2 / review singles:** natural match vs Easy dari warmup sampai hasil, skor/fault benar, save/resume, challenge/replay; UI/kontrol/audio/atmosfer terintegrasi; karakter manusia stylized sportswear berwajah jelas bukan mannequin, animasi badminton/contact/footwork natural. Unit/integration/regression, video/audio QA agent serta device/performance sesuai izin. Pending tidak dianggap PASS. Owner pengalaman utuh saja; M3+ memerlukan izin baru. Fixed/Floating Settings D-040, HOLD servis D-041 dan grade/stamina proposal terhubung M2-05/06/13, tanpa task ID baru. Target acceptance bukan izin import.

## MILESTONE 3 — AI fair, adaptive & match feel
- [ ] `M3-01` Behaviors easy, normal, hard from timing variance, positioning, tactical shot selection; not stat multiplier.
- [ ] `M3-02` Adaptive remembers observed player tendencies (recent shot distribution, court target, reaction) within match and adapts boundedly.
- [ ] `M3-03` AI perception delay + uncertainty. Must not read user's future direction/timing input.
- [ ] `M3-04` Tactical personalities for world athlete data; equal-attribute controlled tests across difficulties.
- [ ] `M3-05` Validate no 'AI cheating': tests with same stats, sampled AI decision traces.
- [ ] `M3-06` 5/15/30-minute simulated exposure QA; AI neither inhuman nor helpless.
- [ ] `M3-07` Re-tune stamina drain for 60-second long rally under normal effort versus repeated smash.
- [ ] `M3-08` Accessibility: visual shuttle contrast settings, button size, assisted mode vs manual behavior.

**Definition of Done M3:** hard/adaptive is challenging and fun without stat rigging; written QA sample and gameplay captures.

## MILESTONE 4 — Career simulation foundation (headless-first)
**Important:** simulate world using fast data models; 3D engine only renders player matches. Must be designed and tested BEFORE making 100 menu screens.

- [ ] `M4-01` Athlete data schema: uuid, age, country, style, attributes, junior/senior ranking, fatigue, injury, life/career status.
- [ ] `M4-02` Tournament schema: id, start/end, tier, event type, eligibility, capacity, entry deadline, draw, seed slots, points matrix, prize distribution.
- [ ] `M4-03` World date/tick scheduler (week/year; explicit event calendar, no realtime waiting).
- [ ] `M4-04` Country/subregion catalog curations; nationality not gameplay buff.
- [ ] `M4-05` Fictional AI athlete generator with unique identities, skill variance, believable ages and playing styles; no real names or deliberate lookalikes.
- [ ] `M4-06` World population replenishment: junior intake, aging, peak/decline, retirement with variance and injury exceptions.
- [ ] `M4-07` Match outcome simulator for offscreen matches and skipped player matches that respects skill, fatigue, form, randomness with controlled bounds.
- [ ] `M4-08` Separate singles and doubles rankings, 52-week rolling window, best 10 eligible results for senior, junior eligibility and exclusion on age-out, weekly snapshot/history.
- [ ] `M4-09` Entry request/accept/reject reasons and deadlines; qualifying, main draw, seed placement and draws freeze after publication.
- [ ] `M4-10` National federation selection simulation for national-team-only events, calendar conflicts, quotas and eligibility. Need country neutral treatment.
- [ ] `M4-11` Tournament bracket engine, withdrawal, walkover, injury, results, trophy/points/prize awards; prevent duplicated payout.
- [ ] `M4-12` 4-year elite multisport event cycle with regional and Olympic-inspired competition, independently named; document exact qualification logic.
- [ ] `M4-13` Event feed to in-game news/calendar; announcement for qualified players and draw results, not just modal spam.
- [ ] `M4-14` Career save slot create/load/delete, archived retired careers read-only; test up to 10+ simulated years and version migration.
- [ ] `M4-15` Automated stress test 1,000+ simulated athletes (provisional quantity, scale per performance) for world stability, duplicates/expiry/season results.
- [ ] `M4-16` `PROPOSED` game career age choices, starting paths and tournament tiers reviewed/approved before final fixtures.

**Definition of Done M4:** Headless world sim stable for >10 virtual years with deterministic seed, credible ranking changes, draws not corrupt, career load/save, no visual polish required.

## MILESTONE 5 — Career player loop
- [ ] `M5-01` Career creation player preset, country, starting age/path, start year, discipline; multiple independent slots.
- [ ] `M5-02` Career home: next match, calendar, news, condition, ranking, earnings; action clarity and ergonomic navigation.
- [ ] `M5-03` Entry/selection/qualifying flow integrated into career UI with confirmations and reasons.
- [ ] `M5-04` Training day focused attribute, skill point from matches, increasing upgrade cost, no mini-games.
- [ ] `M5-05` Injury/fatigue/recovery and training intensity settings; unlimited training frequency with coherent consequences and free rest option.
- [ ] `M5-06` Attribute 1–100 and overall 0–99 `floor`, progression, inactivity/aging change, senior still beatable by user skill.
- [ ] `M5-07` Coach traits none/bonus, optional paid in-game hire and default free coaching path.
- [ ] `M5-08` Playstyle threshold and expensive earned-currency respec (requires balancing/approval).
- [ ] `M5-09` Match -> career update (rating, points, money, status, milestone, news, next event).
- [ ] `M5-10` Career stats all-time, last-year, this-year; histories by event and discipline.
- [ ] `M5-11` Award scene minimal cinematics/camera/trophy/medal (no intellectual-property mimicry).
- [ ] `M5-12` Retirement button unlocked at age >=20, irreversible prompt + read-only archive; ending no post-retirement gameplay.
- [ ] `M5-13` Compare manual vs sim match reward fairness; no easy skip exploit.

**Definition of Done M5:** create athlete at selectable start -> enter tournaments -> train -> earn -> improve -> rank -> choose retire -> view archive, without dangling features.

## MILESTONE 6 — Doubles full mode (not cosmetic)
- [ ] `M6-01` Partner entity, compatibility, availability and career choice, change partner while respecting event rules.
- [ ] `M6-02` IN_PROGRESS: empat atlet/ownership/partner return, Front-Back/Side-by-Side/Net transition/hint diuji. Natural7 Easy/Normal/Hard MatchEnd1003,42/414,41/257,62s,semua ID >1return; service/netFaults0. Gate1000s tetap gagal Easy; kalibrasi ditahan setelah tiga perubahan produk. Lihat TESTING D-053.
- [ ] `M6-03` LOCAL_LOGIC_PASS_PARTIAL: EditMode19/19 termasuk empat server/receiver dan JSON; batas servis panjang/area rally/rotasi/save/replay diuji MobileUiTests. Capture natural/fault komprehensif masih, belum feature complete.
- [ ] `M6-04` Separate doubles ranking/history/entry and national eligibility, points pairing.
- [ ] `M6-05` Two human? **Not in scope**; player controls one created athlete, partner AI.
- [ ] `M6-06` Camera and HUD accommodate 4 bodies, 30 FPS profiling.
- [ ] `M6-07` QA matches/retirement/partner switch mid-career, no corrupt saves.

**Definition of Done M6:** doubles tactically different and actually playable with AI partner.

## MILESTONE 7 — Contracts, economy, off-court, content
- [ ] `M7-01` prize money and finances ledger with local currency units (in-game-only), coach/travel/facility/social spending and clear free choices.
- [ ] `M7-02` Sponsor reputation/popularity criteria, duration, bonuses, condition, proposal and counter-offer dialogue.
- [ ] `M7-03` Relationship + media interview with ethical/reputation consequences; no forced rivalry.
- [ ] `M7-04` Offcourt text/event cards (friends, family, optional romantic relationship, philanthropy, lifestyle) curated, age-appropriate and no full 3D scenes.
- [ ] `M7-05` Avatar/garment/hair/shoes/tattoos/racket/string/grip color cosmetics no stat impact, readable preview.
- [ ] `M7-06` News editorial system reacts to ranking, tournament surprises, achievements, social choices; prevent repetitive spam.
- [ ] `M7-07` Sponsor brands/tournament identities completely original; brand asset provenance confirmed.
- [ ] `M7-08` Ensure all purchases use **earned fictional career money only**, no real-money payment or premium store.

## MILESTONE 8 — Polish, performance, legal QA and release decision
- [ ] `M8-01` UI final spacing/readability/contrast/touch safe area, sound mixer, no music and no unnecessary buttons.
- [ ] `M8-02` 30 FPS on real qualifying 4GB device in long singles and doubles matches; logs for thermal throttling and memory, lower quality fallback.
- [ ] `M8-03` Asset inventory audit: every sound/font/model/texture has source, license, commercial permissions if relevant, attribution, no illicit broadcast sample.
- [ ] `M8-04` Trademark search for JUST SMASH, logo and sponsor names, athlete fictional similarity review. Legal review before public release.
- [ ] `M8-05` Offline/network/privacy audit: log every SDK and external endpoint, disable unwanted analytics, update Privacy Policy to reflect facts.
- [ ] `M8-06` Save backward-compat migrations / corrupt recovery / no data loss on app restart or update.
- [ ] `M8-07` Source code QA, smoke tests + progression stress tests + gameplay bug logs.
- [ ] `M8-08` Build signed release APK for local distribution subject to applicable platform/device requirements. No Google Play publishing without user's later explicit budget/approval and eligibility.
- [ ] `M8-09` Record source credits/Terms/Privacy inside game before distributing.
- [ ] `M8-10` Final approval from owner: performance, enjoyment, design fit, legal/asset risks, no unapproved feature scope.

## Quality gates and release criteria
- A gate is not passed by marking a checkbox alone. Require **evidence**: video, test result, build, screenshots, logs, and device information. Unknown = `NOT TESTED`, not `PASS`.
- Dokumen siap bukan izin coding. Setelah implementasi disetujui, ikuti dependensi M0 sampai M8; jangan loncat ke career.
- `M0 -> M1`: project builds and license log ready.
- `M1 -> M2`: controls tested and owner selects design or expressly authorizes iterations.
- `M2 -> M3`: full singles match valid from serve to score to result.
- `M3 -> M4`: no stat cheating and game feels good; otherwise fix gameplay first.
- `M4 -> M5`: simulation worlds persist, rankings reproducible, tournament entry believable.
- `M5 -> M6`: singles career loop genuinely enjoyable; doubles is not rushed.
- `M6/M7 -> M8`: content complete with source register and source compliance.

## Required agent report per milestone
1. Summary of achieved behavior versus approved requirements.
2. File-by-file changes and architecture decisions.
3. Tests run & results: PASS/FAIL/NOT TESTED and devices.
4. Ukuran APK dan keseluruhan game, RAM dan FPS jika benar-benar diuji; laporkan komponen, perangkat, build hash, dan skenario.
5. Screenshots/recordings of actual playable build.
6. Bugs and known limitations, severity and reproduction.
7. No-cost and third-party asset/license checklist.
8. Decisions requiring explicit owner approval before next step.
