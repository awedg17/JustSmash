# Just Smash — Plan overhaul gameplay, karakter, dan animasi

Tanggal plan: 2026-11-22. Status aktif: **EXECUTION_APPROVED — D-062, 2026-11-23**. Catatan plan-only di bawah adalah riwayat sesi penulisan, bukan larangan eksekusi terbaru.

Eksekusi: seluruh 35 task diizinkan pemilik, subagent sesuai aturan, task BLOCKED dilewati untuk melanjutkan task independen. D-050 dan D-052 tetap. Evidence aktif `evidence/shareable/20261123/overhaul-005531/`; backup `.backups/overhaul-exec_20261123_005531/`. Status/hasil per-task dicatat setelah bukti aktual, tidak dari intent.
Workspace: `C:/Users/Dewangga Ramadhan H/Documents/Portofolio/JustSmash`.

## D-063 — replay capture boundary verified (2026-11-23 19:55)

Test-only MobileMatchTests capture now starts game PCM at actual first recorded frame and stops after actual final frame,not before ffmpeg launch/after unrelated frame delay. Runtime audio unchanged. wave4-replay-av-boundary1:1/1UnityPASS,12controlled scenarios,1683frames56.031s;PCM56.0426667s,AVduration11.67ms vs87.67ms prior. Strict live34.52ms,packet16.24ms,decode/count/PTS/DTS PASS. No trim/retime/padding. Source/protected unchanged during run;only MobileMatchTests differs since26focused current-runtime checks.

Original video copied into replay-challenge-1x-av.mp4 with AAC audio,1683frames intact;full decodePASS,video56.031/audio56.042667s. This is controlled replay/challenge footage,not natural match or listening acceptance. Before/after recordings preserved; source-bound parent-media.json and mux-verification.json accompany output.

Browser Edge headless full1x playback56.042667s alsoPASS:1683decoded,0dropped,ended=true,error=null. MUTED;not audible/natural-motion review. `browser-playback.json` retained.

Read-only lunge auditor sa-0-50b49e3e stopped after198.8s against180s budget (inside owner's5min interval,but internal budget exceeded18.8s). No audit result accepted yet;no worker canonical edits authorized. T4e remains partial: naturalmotion/pause/accepted character and final whole-source regression open. Prior full20Edit+248/250Play remains two knownlunge failures.

## D-063 — T4e foot reconstruction + snapshot capacity (2026-11-23 19:38)

Foot history defect CLOSED_LOCAL_VERIFIED for tested routes:216cases (still/stride/air,3strokes,formats/ends/actors,ApplyFrame/RestoreMatch),maxfoot error baseline0.370000362m→0. New optional plant state retains planted/airborne,left/right world targets; replay reconstruct avoids applying plant movement twice and consumes no footfall.48corrupt live/buffer foot values reject before mutation;v2/v3 missing fields accepted. ReplayTick/SkipReplay/load from replay56checksPASS. Whole T4e remains pending natural-motion/pause/accepted-character gates.

Current source guard26/26PASS in wave4-pose-capacity-green1,including snapshot216cases,48invalidcases,56replaychecks,ClosureTests/momentum. Last whole-suite wave4-pose-edit1 20/20 and wave4-pose-play1 248/250;only two knownlunge failures,source/data unchanged. This whole suite PRECEDES one-line snapshot size fix and latest test-only edits; no claim250/250 or final-source wholePASS.

Capacity defect reproduced on OLD runtime too:1500frames create1,469,686characters(singles)/1,666,635(doubles) beyond prior1,000,000 restore limit. New plant fields2,505,376/2,933,479characters. Minimal bounded8,000,000character guard now admits current1500-frame fixtures;unchanged1500frame bound and all schema checks retained. Inputs8,000,001characters and1501frames explicitly reject without mutation. No disk-save/autosave reliability claim;intermittent File.Replace remains open. Snapshot format staysv3 optional fields.

Same-camera screenshot correction: immediate manual renders were stale evidence(2changedpixels despite.22m bone drift); yielding real frame/render boundary yielded24,164changedpixels expected-vs-restored baseline,0after. CameraJSON exact;visible leg/foot reconfiguration reviewed,not motion acceptance. `wave4-pose-roundtrip-comparison/restore-rendered-before-after.png`. New source frozen; candidate restore byte-exact before capacity fix. Raw old images retained as UNACCEPTED.

Replay/challenge capture wave4-pose-lifecycle1:2/2UnityPASS,12scenarios,1678frames55.891s,live25.33ms/packet17.26msPASS,PCM55.9786667s(duration87.67msFAIL),decodePASS. No mux/trim/retime. Initial nearest replay-active still hit Skip frame;interval-midpoint replacement shows LEWATI. No continuous-motion/listening acceptance. Corrected-camera lowstroke older pair parent audited:695/696frames,22.575/22.671s;beforePCM59.67msFAIL,after49msPASS,live13.22/17.29ms andpacket13.26/29.19msPASS. Both precede foot-state runtime;not current-source videos.

## D-063 — T4e foot-state reconstruction subset (2026-11-23)

Actual CaptureFrame/ApplyFrame and CaptureMatch/RestoreMatch reproduced hidden foot-plant history: initial72cases feet drift max0.268378347m, racket/chest0. Candidate adds optional recorded left/right world-space plants plus planted/airborne flags, restores without applying plant MoveTowards twice, clears replay foot-contact flags, validates finite/bounded vectors before match mutation. Four runtime files: AthleteVisual.cs, CourtReplay.cs, CourtPrototype.cs, CourtDoubles.cs. Live Present/physics/contact/stamina unchanged.

`wave4-pose-roundtrip-green2`5/5PASS: original72cases +48corrupt live/buffer/actor/side/value combinations reject without mutation; missing-foot-state v2/v3 accepted; momentum tests pass. `wave4-pose-roundtrip-motion1`2/2PASS expands to216cases across still/stride/air,formats,ends,3strokes,actors,ApplyFrame/RestoreMatch. Earlier focused26tests24PASS/two knownlungeFAIL; no lunge repair. Whole current suite running under wave4-pose-edit1/wave4-pose-play1; not yet claimed PASS.

Same-camera baseline re-executed as wave4-pose-roundtrip-beforevisual1 with candidate saved to .backups/wave4-pose-candidate1,then four runtime files restored byte-exact. Numeric RED retained; screenshot capture currently UNACCEPTED: .22m bone difference yielded only2pixels. Vision shows feet visible but overlapping heads; immediate manual camera render may use stale skinned data, cause unproven. Do not cite image as visual proof. Natural/replay lifecycle/fullmotion/device remain open. Read-only media auditor stop requested at177s vs120s budget (within5min check); partial result not yet consumed, no worker canonical edits authorized.

### D-063: eksekusi paralel A–G

Instruksi terbaru pemilik mengganti urutan serial: A karakter original+rig, B arena kosong, C readability/HUD, D audio non-voice berjalan independen dalam feature worktree dengan source dirty/untracked sama. Kontrak path/ownership/state/API di `C:/Users/Dewangga Ramadhan H/AppData/Local/hermes/cache/scratch/justsmash-parallel-20261123-1330/contract.json`. Tidak ada worker menulis canonical/scene/prefab/controller bersama; parent satu-satunya pemilik Unity/integrasi.

T2a bukan penghambat global: kandidat orisinal wajib dibandingkan silhouette/grip/deformasi/gerak; batas kemampuan menjadi ASSET_QUALITY_LIMIT, bukan recolor/final palsu. T7c voice OPSIONAL dan tidak memblokir impact/step/squeak/reverb/feedback poin. T7a boleh dikerjakan paralel tanpa penonton/tribun besar, bukan mengalahkan prioritas karakter/animasi.

E Animation memiliki AthleteVisual.cs/CourtMotion.cs setelah rig disepakati; F physics/contact memiliki bagian sim CourtPrototype.cs/ShotIntent.cs setelah C selesai; G AI memiliki CourtOpponent.cs/CourtDoubles.cs. Root/collider milik simulasi, pose hanya presentasi, legal hit immediate/finite/once, predictor sama dengan flight, audio hanya event sah. Tidak menambah mekanik atau melonggarkan D-049/D-050.

QA fokus setelah diff nyata; satu regression integrasi, bukan pengulangan suite sama. Video gameplay1×/visual before-after memakai camera transform/FOV/resolusi/seed sama. Screenshot tidak menutup motion, PC tidak menutup Android. FullPlay terakhir dihentikan (-15), tidak boleh dilaporkan PASS; input source/protected diperiksa sebelum capture berikutnya.

### Wave4 Net/Lift subset checkpoint terbaru

64focused+20Edit PASS,fullPlay245/247 hanya dua lunge lamaFAIL. Net travel max.126886457m,Lift rise min.519999743m,16cases immediatecontact/bone/root unchanged. `wave4-low-strokes-checkpoint1.json` predates harness-only camera repair. Net closeup occlusion isolated to NetTape: camera at z+.1 subject z-.9. Same-half lens gate RED then2/2 GREEN on corrected baseline capture;after capture next. Geometry/gameplaycamera unchanged. NaturalAVduration69/60.67msFAIL,not lowstroke acceptance. Two read-only auditors limited4calls/120s own scratch JSON only;no worker canonical/Unity/Git writes.

### Wave4 idle subset checkpoint historis

Ready-only breath di visible rig memakai recorded elapsed,0deg→0.799169deg maximum offset,foot drift0m pada8actor/end cases.63targeted+20Edit PASS;fullPlay244/246,dua lunge baselineFAIL tetap. Controlled samecamera before/after128frames/4.215s strict50ms/decode PASS;HUD/audio tidak direkam. `wave4-idle-checkpoint1.json`source terbaru. T3a wholelocomotion belum selesai;fullmatch wave3 bukan runtime terbaru setelah idle change.

### Wave3 font / media checkpoint historis

Native glyph warmup fixes measured first-HUD hitch91.31→15.93ms;34UI PASS, fresh singles/doubles excerpts strict50ms clock+AV+decode PASS. `wave3-font-checkpoint2.json` current runtime/source evidence. Earlier MEDIA_BLOCKED entries remain historical. Controlled marker/fullmatch/natural motion/listening/Android not closed. No new whole-task closure.

### Wave3 shuttle lighting / AI audio coverage (historical checkpoint1)

Shuttle-only white faces retain outline: corrected RED262/362dark→GREEN0/362,30focusedPASS;8same-camera pairs/backgrounds exact. T6b still partial:occlusion/end-on dot/natural tracking open. AI real-contact audio7/7baselinePASS (24cases),not a runtime audio fix. Both natural-excerpt clocks exceed50ms;MEDIA_BLOCKED. Fullregression once:Edit20/20,Play239/244. Two knownlungeFAIL retained;twoobsolete audio-cache assertions replaced with real event/output checks,wall contrast.21004→.20004fix. Repair/profile6/6PASS;no fullsuite rerun/closure claim. Current1159source/1348protected verified in wave3-checkpoint1.json.

### Historical Wave2 retained contact fix / rejected lunge

F/GfourimmediatePresent calls integrated;46focusedPASS+16human exact-contact cases,source/dataunchanged. E original snap candidate rejected;parent smooth candidate passedstationary butfailedmovingfoot.2250002m vsbaseline.117160469m,rolledback. Currentruntime matcheswave2-contact-green2;onlytest-onlyFootworkWaveTests differs. `wave2-contact-checkpoint1.json`binds source/evidence. Fullanimation/natural lunge/unreadable shuttle/character/audio/device gates remain. No new fullsuite/whole-task closure.

### Historical Wave1 result / wave2 active

B/C/D integrated:9focusedUnityPASS,strict media failed50ms gate;whole net-centre shuttle still visually unresolved. A originalcandidate rejected ASSET_QUALITY_LIMIT,no import. T7c voice optional;shoe squeak remains missing. Preference recovery complete;interrupted fullsuite NOT_PASS. E/F/G now isolated in scratch/justsmash-parallel-20261123-1425 with exact ownership and frozen baseline rig;small lunge/contact-parity slices,not whole animation/physics/AI closure. [[SESSION_HANDOFF]] contains measured failure values and artifact paths.

### T7d audio lifecycle subset

12UI transitions retained old serve playback;RED t7d-audio-boundary-red1. Two guardedStop calls at StartMatch/Home exit,12→0survivals;GREEN41/41,visible3/3,Edit20/20. Full currentPlay interrupted(-15);preference fixtures recovered;not wholeT7d closure or audible acceptance. Source-bound parent comparison retained;APK older than cleanup. Previous storage/marker clock failures remain open.

### Checkpoint tambahan — controlled evidence

T6d `t6d-motion-visible1`1/1logic PASS,4controlled scenarios/1419frames/24.431s;parent media check EXIT1,strict live-clock drift0.1264661s>0.05s. Decode/framecount/PTS-DTS pass;not motion acceptance. Failure/diagnostic retained,no runtime change or gate relaxation. T7b `t7b-contact-baseline1`6/6PASS,40human-contact cases/280playback-state checks;no miss/repeat/pause/replay restart. Parent source1147/protected1348 exact. Baseline coverage only,isPlaying not exact one-shot count/listening/AI proof. Two test-only source changes;full regression Edit20/20+Play228/229,SaveMatch IOException in corrupt-save case. Parent checkpoint exit1,BLOCKED_REGRESSION_AND_MEDIA,254/255executions/249unique;source/protected exact.10closed/25unfinished remains.

### Revalidasi antrean — 2026-11-23

| Task | Status dan alasan tetap diperlukan |
|---|---|
| T0/T0b/T1a–T1d/T7e | CLOSED_LOCAL_VERIFIED; tidak diulang sebagai implementasi baru. Regresi source terbaru tetap wajib |
| T6e | STATE_CLOSED_LOCAL_VERIFIED: current20Edit+212Play+5visible PASS;117PNG, Training/restored/overturned stills reviewed. Historical explicit-save File.Replace remains OPEN_INTERMITTENT, owner unknown, not fixed; keep failed checkpoint. State correctness and storage reliability tracked separately |
| T2a | BLOCKED_ASSET: initial3/0eligible; additional Male_Casual isolated Blender audit1851tri/31joints/11generic clips, hand/shoe/jersey detail not accepted. CC0 listing differs current QAL; no import. Evidence asset-research-animated-men/audit-result.json. Tidak menurunkan gate atau mengulang generator pakaian gagal |
| T2b–T2d/T3a–T3d/T4a–T4e/T7a | Masih dibutuhkan; dependency mesh/pilot/natural-motion belum lolos. Source baseline/tes numeric tidak menggantikan gate |
| T5a | Numeric verified, continuous-motion belum certified; trajectory bukan task selesai dari XML |
| T5b | PHYSICS_CLOSED_LOCAL_VERIFIED, full task MOTION_PENDING:20Edit+220Play+1visible PASS,241unique/source-bound t5b-checkpoint1.json. Ground/air/bot parity/snapshot/reset64direction cases verified; visible lunge/reach-demand and natural motion still gated by character pilot |
| T5c/T5d | T5c numeric+fresh controlled media verified,historical clock variance/natural acceptance open. T5d144cases23/23+visible1/1+Edit20/20 PASS;fullPlay223/224 explicit-save File.Replace failure. t5d-blocked-checkpoint1.json source-bound267/268executions/245unique. Gap fixture.04→.70m,maxstep.13→.09m,Manual0;natural<.55m up to.42s so no whole-task closure |
| T5e | CLOSED_LOCAL_VERIFIED: recovery AI now uses existing fatigue;24 movement,32 drain,52 freeze/recovery,48 human cost cases. Final20Edit+212Play+1visible, D-050 untouched; source-bound t5e-t6e-checkpoint1.json |
| T6a/T6b–T6d | T6a CLOSED_LOCAL_PREVIEW:8runtimePNG inspected. T6c pointer/guide/pause/idle-resize defects CLOSED_LOCAL_VERIFIED:t6c-touch-checkpoint1.json,249/249unique PASS (20Edit+226Play+3visible),104PNG,1147source/1348protected exact. T6b FRAMING_CLOSED_LOCAL_VERIFIED:t6b-camera-checkpoint2.json,256executions/252uniquePASS,64framing/768linequads0overlap,32highflights;whole shuttle readability remains. T6d score subset CLOSED_LOCAL_VERIFIED251executions/249unique/144PNG;markerGREEN3/3/80states,cyan resource teardown RED/GREEN2cycles,independent contrast112regions/340PNG parentverified;post-review visible3/3+Edit20+Play228PASS,checkpoint251executions/249unique source-bound;continuous1x marker/point-flow stillopen;full T6c touch-layout/ergonomics not closed by defect tests. 25unfinished tasks rechecked against requirements;none cancelled |
| T7b/T7c/T7d | Audit/event/audio legal dan flow masih diperlukan. Listening/voice legal/natural approach tidak terbukti dari tes score/reaction |
| T7f | Tetap final integration gate: natural matches 1×, profile, APK source-bound, docs. Android tetap DEFERRED_OWNER_PC_ONLY |

## 1. Tujuan, keputusan, dan batas sesi

Hasil yang dituju: exhibition badminton stylized 3D, atlet natural dengan pakaian olahraga, gerak badminton terbaca, kontak raket–shuttle sinkron, arena indoor profesional tanpa penonton. Android offline landscape, RAM perangkat minimum 4 GB, target 30 FPS, keseluruhan game maksimum 1.000.000.000 byte. Dua atlet pada tunggal; empat pada ganda putra.

Dasar permintaan: lampiran `pasted_content_2026-11-21_18-00-20-259_e5f6f9.txt`, kemudian tiga jawaban klarifikasi pemilik berikut. Jawaban telah dipulihkan dari hasil klarifikasi asli, bukan ditebak dari ringkasan. Lampiran kedua `pasted_content_2026-11-22_12-30-05-281_8a7edc.txt` (2026-11-22) menegaskan arah yang sama — physics, gameplay, dan vibes dinilai masih kurang realistis — dan telah digabung ke plan ini tanpa menambah task duplikat; pemetaannya di §7.3.

| Persoalan | Keputusan pemilik | Konsekuensi |
|---|---|---|
| Plan atau langsung implementasi | **“Plan lengkap dulu, termasuk karakter dan animasi; belum ubah game”** | Sesi ini hanya dokumen. Kalimat implementasi langsung pada lampiran tidak menjadi izin mengubah game sekarang |
| Jalur aset | **“Evaluasi aset legal gratis; integrasi hanya setelah mesh, rig, dan geraknya lolos”** | Aset matang menjadi pilihan awal. Tidak mengulang generator pakaian yang gagal; tidak menjanjikan kandidat lulus sebelum diuji |
| Stamina | **“Pertahankan D-050; perbaiki bug dan feedback saja”** | Jalan/lari drain 0, recovery hanya antargame. Tidak menerapkan recovery idle/akhir rally atau biaya gerak baru dari lampiran |

Keputusan otoritatif dicatat dalam `docs/05_DECISION_REGISTER.md` D-061. Tabel ini salinan kontrak eksekusi, bukan register approval baru. Detail angka baru di bawah adalah **usulan implementasi/QA**, bukan angka pilihan pemilik atau hasil pengukuran.

### Arah visual dan prioritas kualitas dari lampiran kedua

- **Stylized 3D badminton simulator**: gerakan atlet natural, lapangan proporsional, arena indoor terasa hidup, gameplay responsif. Bukan fotorealistik; bukan kartun dengan animasi kaku. Seluruh tahap dinilai terhadap arah ini.
- **Prioritas pemilik**: lebih baik satu lapangan bagus, dua karakter natural, dan lima animasi pukulan yang benar-benar enak dilihat daripada stadion megah dengan gameplay robotik. **Animasi dan sensasi memukul shuttlecock mendapat waktu pengerjaan terbesar** (Tahap 2–4 lebih dulu daripada dekorasi arena di T7a).
- Angka timing animasi, kamera, dan mekanik pada lampiran kedua adalah **target desain awal untuk dikalibrasi lewat gameplay**, bukan fakta BWF; geometri lapangan resmi tetap mengikuti angka BWF yang sudah dipakai T1b.
- Bagian stamina lampiran kedua (recovery saat diam/akhir rally, biaya gerak eksplosif) **konflik dengan D-050 dan tidak diterapkan**; stamina tetap diatur T5e.
- Referensi mood (bukan sumber untuk disalin): arena tanpa penonton dengan kedalaman cahaya; karakter stylized berproporsi/pakaian/pose meyakinkan; footwork dan lunge saat mengejar kok; smash memakai seluruh tubuh. Semua dari lampiran pemilik; jangan menyalin mesh/foto/atlet nyata (Rule 10).

### KONTEKS WAJIB

- **Belum boleh coding, modeling, download/import paket aset, build, atau menjalankan game untuk overhaul pada sesi plan ini.** Sesudah pemilik memerintahkan eksekusi, jalankan antrean plan; jangan meminta izin ulang untuk setiap langkah teknis yang sudah termasuk scope.
- Rp0. Tanpa pembelian, subscription, trial berbayar, akun/SDK jaringan, analytics, atau layanan runtime. Izin evaluasi aset tidak otomatis mengizinkan install alat/dependency atau upload model ke layanan luar.
- Tetap Unity **6000.3.18f1**, built-in rendering, C#, partial `CourtPrototype`, IMGUI. Test Framework **1.6.0** sudah tersedia. Tidak migrasi URP/HDRP, UI Toolkit, Input System, atau memasang Animation Rigging hanya demi polish.
- Official15, best-of-three, deuce/cap21, aturan servis/ganda, change-ends, challenge/replay, pause/save/load dipertahankan. Career sudah dicabut; jangan hidupkan kembali career, Classic21, adaptive AI, multiplayer, creator, toko, atau kosmetik berstat.
- Lima input pukulan: Smash, Clear/Lob, Drop, Net, Lift. Backhand otomatis; joystick tetap movement + aim. Manual/Assisted, Fixed/Floating, hold-release servis, jump hold-swipe dan fallback tetap. Tidak ada aim assist atau preview target sebelum hit.
- **Kontak legal langsung, finite reach, tulang tidak diregangkan, root/collider tetap milik simulasi.** Grade D-049 tetap Timing/Contact 50:50, threshold 85/65/40; Direction/Power bukan bobot grade. Buffer input existing bukan izin menambah delay animasi.
- D-050: rally sekitar 30 detik menguras sekitar 30 poin, fatigue 0,7–1 di bawah 75% energi, jalan/lari drain 0, recovery antargame. Nilai config runtime harus dibaca, bukan men-tune default class yang tidak dipakai.
- **D-052 PC-only tetap:** jangan discovery ADB, install/launch/log/data HP, emulator, port Windows baru, atau bypass larangan native input. Android 30 FPS/thermal/touch/installed footprint tetap `DEFERRED_OWNER_PC_ONLY`, bukan PASS. Jangan meminta izin HP berulang tanpa instruksi baru.
- Satu Unity per proyek. Jangan mengedit source saat runner aktif, mematikan Editor pemilik, atau menutup pekerjaan yang belum disimpan. Proses panjang melalui tool: `background=true, notify=true`.
- Git sudah ada tetapi banyak file dirty/untracked sejak sebelum plan. Jangan reset/clean/stash/drop pekerjaan lama. Backup slice sebelum edit dan semua data fixture; restore hanya perubahan milik task. Tidak `git push`; plan tidak melakukan commit.
- Maksimal tiga percobaan perbaikan otomatis untuk kegagalan sama. Simpan RED/failure; setelah batas, hentikan slice dan laporkan sebab yang terbukti/yang belum diketahui. Jangan mengganti pendekatan gagal dengan nama baru lalu mengulang generator sama.
- Hasil QA di `docs/TESTING.md`, bug/percobaan di `docs/ERROR_LOG.md`, keputusan di register, provenance di `ASSET_REGISTER.csv`. Jangan membuat WORKFLOW/worklog pengganti atau mengedit `docs/AI_RULES.md`/`docs/UI_REFERENSI.md`; larangan spesifik proyek tetap.
- Referensi artistik tidak memberi izin menyalin mesh/animasi/audio/merek. Sponsor board memakai identitas fiktif Just Smash; tidak meniru atlet nyata. Tidak mengirim source aset Asset Store sebagai paket aset publik.

## 2. Baseline nyata dan peta source

Pemeriksaan sesi plan: source/dokumen dibaca, jalur toolchain dicek, halaman kandidat diperiksa. **Tidak menjalankan Unity test atau gameplay baru.** Checkpoint D-060 tersimpan melaporkan 208 eksekusi / 207 tes unik PASS; itu bukti historis, bukan verifikasi overhaul.

APK baseline yang dicatat handoff: `builds/JustSmash-Mobile.apk`, 30.233.578 byte, SHA-256 `1ec08181fe9ff0f5419e87305347696a9846ec74e88dbb98d68025f9f19f0919`. Bukti di `evidence/shareable/20261115/roller-polish-040359/d060-checkpoint1.json`. Executor wajib menghitung ulang identitas sebelum perubahan. Graphify tidak tersedia pada workspace saat pemeriksaan; peta berikut berasal dari source langsung.

| Area / kondisi saat ini | Lokasi dan simbol yang dibaca | Implikasi |
|---|---|---|
| Mesh visible berasal dari NPC Casual Set00; primitive lama disembunyikan | `Assets/Editor/AthleteVisualSetup.cs:9–23`; `Assets/Scripts/AthleteVisual.cs:32–75`, `Attach`, `Bind` | Perbaikan proxy mannequin saja tidak mengubah atlet yang terlihat |
| Animator dimatikan; 15 clip legacy dibuat dari kurva torso/bahu lalu disampling manual | `Assets/Scripts/AthleteVisual.cs:102–125`, `MakeClips`; `147–218`, `Present` | Bukan sistem tanpa animasi, tetapi label clip tidak membuktikan delapan jenis stroke punya motion lengkap |
| Grip/finger/arm IK dan foot plants sudah ada | `Assets/Scripts/AthleteVisual.cs:83–100`, `127–145`, `196–217`, `227–258` | Pertahankan kontak, panjang tulang, grip, planted feet; bukan rewrite semua dari nol |
| Motion visible dipanggil lebih dahulu; proxy lama tetap digerakkan | `Assets/Scripts/CourtMotion.cs:59–121`, `AnimateOpponent`, `AnimateAthlete` | Pisahkan otoritas rig visible dan dependency collision sebelum menghapus proxy |
| Arena membangun 48 penonton: 4 rig, 44 baked; helper juga dipakai ofisial | `Assets/Scripts/CourtArena.cs:48–178`, `BakeAudience`, `BuildArena`; `179–218`, `UpdateArena` | Jangan hapus helper bersama tanpa memisahkan kebutuhan umpire/scoreboard |
| Dimensi utama sudah benar; net belum satu profil visual/collision | `Assets/Scripts/CourtPrototype.cs:394–412`, `BuildCourt`; `445–455`, `MarkLanding`; `742–794`, `FixedUpdate` | NetMesh 1,524 m, tape lurus 1,55 m dengan lebar 0,035 m; collision memakai 1,55 m konstan. Target tape 0,075 m perlu perubahan nyata |
| Shader atlet/court cel tidak punya roughness | `Assets/Resources/CelCourt.shader:1–45` | Mengubah angka material yang tidak dibaca shader tidak memenuhi permintaan vinyl/mat |
| Drag, launch solver dan IN/OUT sudah terpusat dalam partial utama | `Assets/Scripts/CourtPrototype.cs:275–303`, `StepVelocity`, `LaunchVelocity`, `InCourt` | Kalibrasi existing integrator; tidak menambah physics engine |
| Tap dan hit memakai jangkauan visible rig | `Assets/Scripts/CourtPrototype.cs:522–578`, `TapShot`, `ReleaseShot`; `Assets/Scripts/AthleteVisual.cs:139–145`, `CanReach` | Hit sah tidak boleh ditunda menunggu backswing atau diperluas untuk menyembunyikan mesh gagal |
| AI single sudah observation/reaction/recovery, tetapi variasi shot terbatas | `Assets/Scripts/CourtOpponent.cs:31–97`; `Assets/Scripts/CourtDoubles.cs:222–260` | Perbaiki state existing; difficulty bukan speed/reach cheat |
| HUD dan touch hitbox terpusat IMGUI | `Assets/Scripts/CourtUi.cs:37–49`, `ControlRect`; `403–455`, `DrawGameHud` | Rendering dan hitbox harus berubah bersama, bukan memindahkan gambar tombol saja |
| Audio impact/step sudah rekaman; crowd terpicu tiap perubahan skor | `Assets/Scripts/CourtAudio.cs:16–43`, `UpdateAtmosphere`; `47–109`, `CreateSound`, `PlaySound` | Tanpa penonton perlu mencabut crowd playback. `arena-room` lama berasal dari rekaman indoor crowd menurut Legal; bukan ambience GOR kosong |
| Skor tiap game belum disimpan sebagai riwayat lengkap | `Assets/Scripts/SinglesMatch.cs:25–42`, `AwardPoint`; `Assets/Scripts/CourtPrototype.cs:235–244`, `CaptureResult` | Result per-game perlu data nyata sebelum skor direset; tidak bisa dibuat hanya dari total kemenangan |
| Save sudah atomic tetapi load hanya file utama; beberapa pesan tidak terlihat | `Assets/Scripts/CourtPrototype.cs:246–263`; `Assets/Scripts/CourtUi.cs:374` | Temuan rilis terpisah; bukan alasan mengubah atomic save tanpa reproduksi |
| Build menulis APK/log tetap dan dapat mengubah retained shaders/settings | `Assets/Editor/AndroidBuild.cs:11–44`, `RunMobile`, `Build` | Backup output lama, verifikasi source setelah setup/teardown, jangan menimpa evidence lama |

Line number adalah snapshot. Gunakan nama simbol untuk pencarian ulang setelah edit. Temuan `File.Replace` original-owner-unknown, historical AV lag, continuous-motion/listening dan owner acceptance tetap terbuka; jangan ditutup oleh keberadaan plan atau satu run baru PASS.

## 3. Jalur karakter dan animasi

### 3.1 Rekomendasi

**Asset-first, perubahan rig minimum, animasi badminton khusus.** Pilih mesh berpakaian olahraga yang matang bila ada; gunakan library legal sebagai bahan locomotion, bukan klaim “paket badminton lengkap”. Uji satu atlet + satu rangkaian gerak dahulu. Dua identitas atlet baru diintegrasikan setelah pilot lolos; empat aktor ganda wajib ikut regresi.

| Jalur | Yang sudah diketahui | Batas / keputusan plan |
|---|---|---|
| NPC Casual Set00 existing | Sudah terpasang melalui `AthleteNPC.prefab`; jalur runtime dan bones diketahui | Aman sebagai baseline pembanding, **bukan solusi shorts/sportswear**. Pertahankan file asli dan seluruh provenance |
| Quaternius Universal Animation Library | Listing menyebut locomotion delapan arah, varian root-motion dan tanpa root-motion, Unity/retarget, CC0; seluruh kit 120+ animasi.[1] | Kandidat pertama **locomotion in-place**. Periksa daftar Standard gratis yang benar-benar diunduh; 120+ bukan jaminan isi free subset atau gerak badminton |
| Quaternius Universal Base Characters | Listing seluruh kit: enam base, 20 rambut, humanoid, rata-rata 13k triangle, CC0; hanya sebagian pack gratis.[2] | Kandidat bentuk kepala/body, bukan atlet lengkap. Audit lama proyek hanya membuktikan Superhero di Standard. Jangan menganggap Regular/Teen/Source atau jersey/shorts ikut gratis |
| MakeHuman + wardrobe satu ekosistem | Halaman system assets menyebut CC0 untuk item yang terdaftar.[4] Paket `pants03` mencantumkan shorts/swimming trunks sebagai CC-BY, bukan CC0.[5] | Cadangan riset, belum dipilih. Kelengkapan jersey/court-shoe, versi lisensi per item, fitting, exporter dan kebutuhan install belum lulus. Tidak otomatis memasang MakeHuman |
| Mixamo | FAQ Adobe mengizinkan character/animation royalty-free untuk game dan menyebut biped humanoid.[3] | Cadangan retarget/locomotion, bukan CC0 atau bukti badminton. Tanpa login/upload/akun baru dalam sesi ini; perlu izin khusus bila jalurnya membutuhkan itu |
| Model/pakaian procedural dari nol | Enam percobaan lama gagal coverage/deformation; D-048 menutup pendekatan | **Tidak diulang** sebagai default. Recolor trousers, armor menjadi jersey, atau generator tabung baru bukan pengganti aset lolos |

**Keterbatasan nyata:** riset ini belum memverifikasi satu paket gratis berisi atlet stylized + jersey + shorts + indoor shoes + rig + seluruh stroke badminton. Jangan menjual kepastian aset yang belum ada. Saat eksekusi, lakukan shortlist maksimal tiga kandidat mesh/outfit yang relevan; hentikan riset kandidat jika semuanya membutuhkan pembuatan ulang pakaian/rig. Laporkan pilihan tersisa, bukan mengimpor hasil gagal.

### 3.2 Kontrak karakter yang harus dihasilkan

- Dua identitas laki-laki fiktif, beda wajah/rambut/siluet upper-body atau detail pakaian; **bukan hanya warna jersey**. Ganda mempertahankan empat role terbaca melalui identitas kepala/detail dan warna tim. Variasi kosmetik tidak mengubah reach/stat secara diam-diam.
- Proporsi humanoid stylized, sambungan bahu/siku/wrist/hip/knee/ankle mulus. Wajah terbaca; rambut bervolume sederhana tanpa hair simulation. Tangan punya palm/thumb/finger curl yang benar pada handle.
- Jersey dengan kerah, lengan, bahu, hem dan lipatan ringan; shorts olahraga dengan crotch/hem yang benar; sepatu indoor dengan sole/heel/toe. Tidak menutup clipping menggunakan gelembung joint atau badan diperbesar.
- Rig compatible dengan jalur pose/IK; bind pose, skala unit metre, arah depan dan weight normalization diperiksa. Pertahankan panjang tulang dan physical reach. `RacketSocket` mengikuti tangan, pusat `StringBed` menjadi titik visual kontak.
- Raket oval, shaft tipis, grip jelas, stringbed ringan. Gabungkan geometri statis bila mengurangi draw calls; jangan menambah banyak material/LineRenderer per senar tanpa ukur.
- Ready: lutut fleksi, pelvis seimbang, badan condong ringan, racket di depan tanpa menutupi muka, free hand penyeimbang. Menu memakai pose santai atlet terpisah, bukan freeze pose hit.
- Budget awal yang **diusulkan**: sekitar ≤20k triangles per atlet berpakaian, ≤4 material slots, atlas 1k–2k jika cukup. Ukur mesh final, bones/weights, shared textures, skinning, draw calls, RAM dan payload; angka itu bukan jaminan 30 FPS. Jangan menurunkan kualitas target diam-diam untuk mengejar budget.

### 3.3 Feasibility gate sebelum integrasi

1. Catat source URL, versi/tanggal, isi free subset, lisensi paket, atribusi, hak modifikasi/embedded distribution, checksum dan berkas yang benar-benar dipakai. Tidak memasukkan demo/source-library seluruhnya ke `Resources`.
2. Import terisolasi setelah izin eksekusi. Jangan mengganti `AthleteNPC.prefab` aktif. Ukur mesh/material/bone/texture; cek importer/Avatar jika retarget membutuhkan Humanoid.
3. Render front/back/side dan close grip: ready, menu, lunge kiri/kanan, overhead, backhand, jump/apex/landing. Tes bind/rest displacement dan deformasi baked surface terpisah; valid Avatar bukan PASS mesh.
4. Retarget satu locomotion in-place dan satu stroke khusus ke skeleton terpilih; rekam 1× dan review slow playback tanpa frame sintetis. Tunjukkan preparation/contact/follow-through/recovery, bukan GIF kumpulan pose.
5. Lulus hanya bila tidak ada gap bahu/crotch, pakaian tembus yang mengganggu, tulang memanjang, grip lepas, racket menembus kepala/torso, atau footwork meluncur jelas. Numeric PASS + visual/motion review sama-sama perlu.
6. Jika gagal tiga perbaikan untuk masalah sama, hentikan kandidat. Pertahankan baseline aktif; laporkan `CHARACTER_BLOCKED_ASSET`, bukan “karakter selesai”. Jalankan task independen yang sudah diizinkan, bukan menghabiskan sesi pada dekorasi.

### 3.4 Sistem pose minimum

Pertahankan API `Attach`, `CanReach`, `Present`, `TakeFootContacts`, `PresentReaction` bila cukup. Ganti isi clip/pose yang lemah, bukan membuat framework animasi baru. Gunakan native Unity animation/retargeting hanya pada rig yang lolos. Jika clip humanoid harus memakai Animator/PlayableGraph native, pilih **satu** evaluator pose; jangan Animator + legacy sampling + IK saling menimpa.

Urutan evaluasi: root/kecepatan aktual dari simulasi; base locomotion; upper-body stroke; bounded hand/racket IK; foot stance correction; presentation reaction. Root motion mati. Upper/lower-body mask dipakai hanya jika benar-benar diperlukan. Path bones tetap bila rig sama; bila berbeda, resolve bone map sekali saat bind, bukan pencarian nama berulang per frame. Jangan menambah paket.

Replay/snapshot harus menyimpan atau merekonstruksi clip/phase, facing, contact, gait/plant dan transisi secara deterministik. Uji pose setelah diselingi pose lain, save/load saat swing, pause, replay skip, teleport administratif saat next-point/change-ends. Jangan mengklaim transform root saja menjamin pose replay benar.

### 3.5 Daftar motion dan kontrak input

| Kelompok | Gerak wajib | Pembeda yang dinilai |
|---|---|---|
| Ready/menu | ready, idle breathing, menu athlete pose | Breath pakai clock pose yang ikut pause; bukan hanya phase jarak sehingga berhenti saat idle |
| Footwork | split-step; shuffle L/R; forward lunge L/R; backward chasse/crossover; side recovery; return-to-base | Direction blending lokal, cadence dari kecepatan/jarak nyata, stance plant, braking/push-off berbeda |
| Serve | forehand dan backhand serve | Grip, persiapan, kontak rendah dan follow-through berbeda; short/flick tetap ditentukan power, **bukan** tombol stroke baru |
| Clear | forehand clear/lob dan backhand clear | Hip/shoulder rotation, elbow leading, racket acceleration dan follow-through berbeda |
| Drop / smash | forehand drop; ground smash; airborne smash | Drop lebih terkontrol; smash rotasi tubuh + follow-through tegas; bukan beda label/kecepatan clip saja |
| Depan / bertahan | net shot+lunge; defensive lift; defensive block | Net sentuhan pendek, lift gerak bawah-atas, block stabil meredam. Block sebagai varian konteks input Net yang ada; tidak menambah tombol/shot physics baru |
| Jump/landing | takeoff, airborne preparation, air stroke, knee-flex landing, recovery | Root gravity konsisten; tidak ada hover, mid-air teleport atau free ground-speed steering tanpa batas |
| Flow | approach serve, point pause, celebration ringan, disappointment, change-ends, result | Mengikuti hasil sah; koreksi challenge memperbarui reaksi; bukan animasi pemenang salah |

**Batas immediate hit:** preparation/backswing diperlihatkan selama antisipasi flight yang sudah terlihat, hold serve, atau jump; tidak menunggu input lalu menunda hit sampai clip mencapai marker. Saat tap legal, physics dan pose kontak harus benar pada tick/frame yang sama, lalu follow-through/recovery. Antisipasi generik tidak membaca teknik masa depan. Late input boleh memakai backswing lebih pendek; jangan mengorbankan respons demi lima fase berdurasi penuh. Jika pilot masih snapping, nyatakan konflik kualitas yang belum teratasi, jangan memanjangkan hit window atau delay.

Recovery animation tidak boleh otomatis menarik root pemain Manual ke base. Return-to-base root hanya untuk AI/Assisted; Manual memakai input sendiri. Pukulan legal berikutnya dapat menginterupsi recovery. Airborne mengunci/meredam horizontal velocity secara fisik yang diuji, bukan menunggu landing animation selesai untuk menerima kontrol.

## 4. Urutan implementasi setelah diperintahkan

**0 baseline → 1 arena inti → 2 karakter/pilot → 3 locomotion → 4 stroke/contact → 5 flight/movement/AI → 6 kamera/HUD → 7 audio/flow/final QA.** Dekorasi arena ditahan sampai karakter dan motion lolos. Tujuh tahap lampiran tetap urut; tahap 1 bukan kesempatan memoles kursi/lampu berhari-hari. Bobot terbesar untuk Tahap 2–4 sesuai prioritas lampiran kedua: animasi pukulan dan sensasi memukul adalah dua hal yang paling pantas mendapat waktu kerja; stadion megah tanpa gerak natural bukan target.

Setiap tahap wajib: kasus RED/kontrak baru terlebih dahulu, perubahan minimum, GREEN + regresi terkait, run gameplay actual source, screenshot before-after dengan framing sama, video 1× untuk motion/flow. Hash source+aset+config dan timestamp disertakan. Kegagalan satu gate tidak disamarkan oleh gate lain yang PASS.

### Tahap 0 — proteksi dan baseline

**Kondisi:** repo dirty/untracked, bukti D-060 dan APK lama harus dipertahankan; native-input Capture dilarang.

- [x] **T0** Catat `git status`, hash source/assets/packages/settings, APK dan semua `.hermes/local-game-data`; backup slice ke folder unik. Simpan keputusan D-061 dan cek scope eksekusi terbaru. Jangan mengubah data pengguna untuk membuat tes lulus.
- [x] **T0b** Siapkan folder evidence unik dan `JUSTSMASH_QA_RUN`. Pilih test name eksplisit, bukan semua Capture. Rekam baseline arena, satu gerak pilot dan satu rally ganda; boleh memakai bukti lama hanya setelah source/runtime identity cocok, dengan label tanggal lama.

**Acceptance:** daftar file baseline/backup terverifikasi, fixture restore teruji, run tidak menimpa output lama, seluruh bukti diberi source identity. Belum menyebut game berubah.

### Tahap 1 — tanpa penonton; court/net/material inti

**Source:** `BuildCourt`, `BuildArena`, `UpdateArena`, `ReleaseArena`, `MarkLanding`, `FixedUpdate`, `BootDiagnostics.Current`, `CelCourt`. Angka dasar singles/doubles sudah benar; perubahan fokus mismatch/rendering, bukan membangun ulang semuanya.

- [x] **T1a — cabut penonton.** Hapus spawn 48 spectator, tribun besar/seats, animasi clap dan alokasi mesh/material khusus crowd. Cabut applause playback serta lapisan crowd pada room ambience. Ofisial bukan spectator: pisahkan helper yang masih diperlukan; scoreboard live tidak ikut hilang. Tidak mengganti crowd dengan NPC dekorasi lain.
- [x] **T1b — court satu sumber kebenaran.** Unit metre; 13,40 × 5,18 singles / 6,10 doubles, line 0,04; short service 1,98 dari net, doubles long service 0,76 dari baseline (=5,94 dari net), center line hanya area servis. Pakai mesh/quad ground-aligned untuk garis bila billboard LineRenderer membuat tebal berubah. Definisikan edge legal dan arah ketebalan garis: boundary line termasuk IN; jangan menambah 0,02 m OUT hanya karena garis diletakkan di tengah batas numerik. Uji setiap sisi/ujung/corner dan mode ganda. Samakan visual permukaan dengan floor contact dan letak soles; offset kecil hanya untuk depth, bukan elevasi lapangan.
- [x] **T1c — net.** Top tengah 1,524 m, ujung/tiang 1,55 m, white tape tinggi 0,075 m; mesh gelap tipis terbaca gameplay. Satu profil top net digunakan rendering, crossing collision dan `MarkLanding` prediction. Native helper kecil di partial existing cukup; kurva sag adalah pilihan visual ringan, bukan klaim bentuk sag resmi. Uji center/near-post, above/below tape, high-speed crossing, net-fault tubuh/raket. Jangan hanya menurunkan config sehingga tiang ikut salah.
- [x] **T1d — material dasar (CLOSED_LOCAL_VERIFIED; `t1-closure.json`, final Edit19/Play194/visible22/pilot1 PASS).** Court hijau natural matte, garis off-white, luar biru tua, dinding charcoal/navy panel sederhana. Built-in Standard hanya pada court bila diperlukan untuk smoothness/roughness, atau shader minimal yang benar-benar membaca tekstur; tidak migrasi pipeline. Tambah detail permukaan halus dan contact shadow lembut tanpa SSAO/postprocessing berat. Atlet tetap shader stylized yang lolos; jangan mengganti semua material demi satu lantai.

**Tes diperbarui dahulu:** `MobilePresentationTests.ArenaHasOfficialsSeatedAudienceAndLiveScoreboard`, `ForegroundAudienceClapsWithFourRigsWithoutMovingSeats`, `SeatedAudienceHasKneesForwardFeetDownAndHandsOnLap`; `FeedbackTests.CrowdUsesRecordingAndRespondsOncePerPointWithPauseDucking`. Ganti ekspektasi crowd yang sudah superseded menjadi assertion **zero spectators/stands/crowd playback**; tetap uji scoreboard, ofisial yang dipertahankan, pause dan lifecycle cleanup. Bukan menghapus seluruh tes presentation.

**Acceptance:** lapangan/net sesuai angka dan IN/OUT, tidak z-fighting, spectator/tribun/crowd update nol, scoreboard berfungsi tunggal/ganda/change-ends. Capture top/near/side dan gameplay 720p kedua ujung. Profilkan biaya sebelum/sesudah; hasil tidak boleh disebut “lebih cepat di Android” dari PC. Sponsor board, roof detail dan kursi dekoratif belum prioritas.

### Tahap 2 — mesh atlet, identitas, racket, ready + motion pilot

**Source:** `AthleteVisualSetup.Run`, `AthleteVisual.Attach/Bind/MakeRacket/Present`; `Assets/Resources/AthleteNPC.prefab` dan import kandidat terisolasi.

- [ ] **T2a — CHARACTER_BLOCKED_ASSET (audit 3 kandidat selesai, 0 eligible).** Evidence `t2a-parent-audit1/verification.json` parent-run exit0. Quaternius Standard128.968.391byte/SHA256`fdbf1804c90dfc1ea03e992bff7da2dfd1a79318e13270a660180f9308455f40` berisi dua Superhero, bukan sportswear. MakeHuman belum export jersey/shorts/court-shoe terverifikasi atau izin tool tambahan. GameMog metadata singlet/spikes dan included motion restricted; tidak diimpor. Rincian sumber pada arsip `sources.json` IDs16–19 dan raw retrieved files. Ini blocker shortlist, bukan klaim semua aset gratis gagal global. Pertanyaan tertunda: izinkan satu tool/jalur outfit kompatibel setelah item dan lisensi lengkap diverifikasi? Tidak menunggu jawaban untuk task independen. T2b–T4e/T7a tetap pending gate, baseline tidak dipromosikan sebagai atlet baru.
  Jalankan shortlist dan feasibility §3.1–3.3. Pilih satu mesh/outfit yang lolos, catat alasan menolak kandidat lain. Bila tidak ada, laporkan blocker; jangan mengalihkan kewajiban modeling ke pemilik.
- [ ] **T2b** Buat dua identitas atlet dari rig kompatibel: wajah/rambut/detail jersey/shorts/sepatu. Racket socket, finger curl dan silhouette tested. Prefab varian baru belum mengganti baseline sampai semua deformasi wajib lolos.
- [ ] **T2c** Buat pilot **approach → split-step → chasse belakang → jump-smash → landing → recovery**, ditambah reach backhand rendah. Satu rangkaian 10–20 detik tanpa glow/VFX penutup; gameplay camera dan close side/front, 1× dan slow review. Kontrak contact/flight tetap existing. Pilot bukan animasi pose montage.
- [ ] **T2d** Integrasikan kandidat lolos melalui loader yang sama; pertahankan sumber NPC lama untuk rollback, tetapi jangan mengemas aset tak terpakai hanya karena masih berada di Resources. Tunggal/ganda/Home/Setup semua memakai model yang benar, bukan scene demo terpisah.

**Tes dahulu:** `AthleteVisualTests`, `MobileMotionTests` dan contact tests pada `ClosureTests`/`LocalGateTests`; tambahkan skin coverage/grip/human identity checks. Update bone-path assertion hanya setelah mapping baru dibuktikan, bukan melemahkan fixed-length/contact gate. Uji semua empat role dan dua orientasi lapangan.

**Acceptance:** kontrak mesh §3.2, 2 identitas bukan recolor, menu muka tidak tertutup racket, tidak ada fallback tersembunyi ke mannequin. Pilot numeric+continuous-motion lulus sebelum menambah seluruh clip. Screenshot atau import sukses saja belum cukup.

### Tahap 3 — locomotion dan transisi

**Source:** `AthleteVisual.MakeClips/Present/PlantFeet`, `CourtMotion.AnimateAthlete/AnimateOpponent`, `CourtDoubles.AnimateDoubles`, pose replay di `CourtReplay`.

- [ ] **T3a** Lengkapi ready/breath, split-step, shuffle kiri/kanan, lunge depan kiri/kanan, backward chasse/crossover, side recovery dan kembali base. Blend arah lokal memakai actual displacement/velocity, bukan label input atau world-Z sehingga gerak terbalik saat change-ends.
- [ ] **T3b** Phase kaki distance-based dan kecepatan clip mengikuti root. Stance target tetap world-space selama planted; swing foot punya clearance, knee pole stabil. Reset plant hanya pada reset administratif/teleport next-point/change-ends, bukan setiap frame. Stamina rendah memperlambat cadence sesuai gerak nyata.
- [ ] **T3c** Pisahkan upper-body stroke dan lower-body footwork ketika diperlukan; transisi start/stop/reverse singkat. Input tidak menunggu crossfade selesai. Split-step dipicu observasi pukulan lawan yang sudah terjadi, bukan membaca input lawan sebelum hit.
- [ ] **T3d** Reaksi jump/takeoff/landing: fleksi lutut, torso rotation, free-hand balance, airborne plant dilepas, footfall tepat landing. Jangan menghapus seam fix D-059 atau mengganti gravity dengan clip root-motion.

**Tes dahulu:** `MobileMotionTests.FootworkUsesLocalDirectionsAndResetsPlantsOnNewPoint`, existing continuity/ready/free-hand tests dan `FeedbackTests.FootfallCadenceIsBoundedAndMatchesVisibleSolePlant` / `FourAthletesSoundOnlyOnGroundedFootContacts`. Tambah translasi diagonal, reversal, stop, low-speed/fatigue, bounds pushing, dua ends, empat aktor, pause/replay. Plant-slip diukur pada stance interval yang didefinisikan, bukan saat swing foot memang bergerak.

**Acceptance:** seluruh arah/gerak tercover video; tidak ada foot skating jelas, kaki menembus permukaan atau knee flip; displacement, pose dan footfall konsisten. Usulan guard awal: drift horizontal stance ≤0,03 m per plant pada gerak stabil, penetrasi sole ≤0,01 m; jika gagal, perbaiki gerak atau jelaskan keterbatasan, jangan menaikkan toleransi diam-diam. Angka ini bukan pengganti review natural 1×. Replay/save/pause mempertahankan phase; Manual tidak dipaksa pulang base.

### Tahap 4 — stroke berbeda dan sinkronisasi kontak

**Source:** `AthleteVisual.Present/CanReach`, `CourtPrototype.TapShot/ReleaseShot/ServeWithPower`, `CourtOpponent` dan `CourtDoubles` contact paths, `CourtReplay`.

- [ ] **T4a** Bangun semua stroke §3.5: forehand/backhand serve, forehand clear, backhand clear, forehand drop, smash, net+lunge, defensive lift, defensive block. Bedakan pose preparation/backswing/contact/follow-through/recovery secara bentuk dan timing; tidak mengganti nama generic overhead saja.
- [ ] **T4b** Contact frame menjadi acuan, bukan AnimationEvent yang memerintahkan hit. Validasi finite reach + tinggi + jenis stroke lebih dulu, commit flight sekali, pose stringbed pada contact yang sama, baru feedback/audio. Miss tidak memicu impact atau berpindah arah shuttle. IK menangani variasi reachable height tanpa memanjangkan lengan.
- [ ] **T4c** Serve forehand/backhand dan block adalah varian visual/konteks pada input existing. Mapping awal yang diusulkan: backhand untuk short serve, forehand untuk flick; block memakai Net pada bola defensif yang memenuhi geometri, tetap shot envelope Net existing. Tidak menambahkan servis/button/shot physics baru tanpa keputusan tambahan.
- [ ] **T4d** Pastikan immediate tap legal, buffered legal contact, jump swipe dan AI contact semuanya menampilkan kontak sebelum perubahan arah terlihat. Set interrupt windows untuk recovery dan locomotion; hit berikutnya tetap diperiksa rules/range, bukan selalu ditolak karena clip belum selesai.
- [ ] **T4e** Simpan/reconstruct pose state minimal agar replay, pause/load, challenge-skip dan end swap tidak memutar ulang hit/audio atau melompat ke pose beda. Validasi data sebelum mutasi state; pertahankan compatibility save existing.

**Tes dahulu:** contact reach/length dalam `MobileMotionTests`, `ClosureTests`, `LocalGateTests`; `LocalGateTests`, `MobileInputTests` dan `MobileOpponentTests` untuk pose/replay/snapshot empat aktor. Matriks: fore/backhand, high/low/edge/unreachable, jump takeoff/apex/low airborne, kedua ujung, human/partner/opponents, langsung/buffered/serve. Rekam event time, actual stringbed, cork/contact point, outgoing velocity, stroke/clip phase dan actor ID; jangan pakai clock player untuk menilai lawan.

**Acceptance:** seluruh stroke bisa dibedakan dalam gameplay; contact error tidak lebih longgar dari baseline tes existing; no bone stretch/teleport/reach inflation/delay baru. Hit sekali, VFX sekali, impact sekali, miss silence. Clip yang hanya lolos numeric tetapi masih snapping/clipping tetap OPEN_VISUAL. Video terkendali dan natural dipisahkan.

### Tahap 5 — shuttle, movement feel, AI dan regresi stamina

**Source:** `CourtPrototype.StepVelocity/LaunchVelocity/FixedUpdate/Update`, `ReleaseShot`, `CourtOpponent.ObserveFlight/UpdateOpponent`, `CourtDoubles.UpdateDoublesAt`, `OpponentProfile`, config runtime.

- [ ] **T5a — shuttle.** Pertahankan fixed-step integrator drag; kalibrasi shot envelope existing melalui satu matriks seeded sebelum/sesudah. Clear tinggi/jauh, smash cepat-menukik kemudian melambat, drop turun dekat net, net pendek-rendah, lift dari depan naik ke belakang. Cork searah velocity; gunakan geometry silhouette yang terbaca, bukan trail neon besar. Landing predictor, AI predictor, actual integration dan net profile harus konsisten. Satu trajectory helper shared hanya bila menghapus duplikasi nyata; tidak physics engine kedua.
- [ ] **T5b — movement.** Respons input langsung; acceleration/deceleration cepat dengan braking/push-off yang terlihat, perubahan arah tanpa sliding berlebih. Velocity/cap berlaku sama pada human/AI sesuai fatigue dan mode. Airborne mempertahankan momentum dan batas steering, tidak full ground-speed bebas berubah arah. Lunge dihitung dari reach demand, bukan teleport saat nama shot berubah. Manual/Assisted tetap beda tanggung jawab.
- [ ] **T5c — AI.** Susun state Ready, Anticipation, Movement, Stroke Selection dan Recovery pada logic existing; tidak wajib class/state-machine framework baru. Observasi flight setelah hit, target reachable, pilihan shot sesuai tinggi/posisi/open space. Easy reaction/error lebih besar; Normal variasi kompetitif; Hard keputusan/positioning lebih baik. Max speed/reach/stat bukan bonus difficulty. No input peeking, teleport, impossible hit. Periksa legal reach sebelum konsumsi RNG yang memilih shot; jaga perbaikan clock/RNG D-060.
- [ ] **T5d — ganda.** Empat atlet punya role penerima/hitter yang sah, front-back/side-by-side/recovery terkoordinasi tanpa overlap posisi berkepanjangan. Jangan mematikan partner atau mengubah turn/serve rules agar rekaman lebih bersih. Uji setelah kedua ujung lapangan berganti.
- [x] **T5e — stamina D-050 (CLOSED_LOCAL_VERIFIED).** `t5e-t6e-checkpoint1.json`: full20Edit+212Play, recovery at energy30 .090→.0738m/tick; before/after150frames each full-decode, natural-motion not certified. Moving rally30.00235s/33.73337m/9returns/72.98644energy.  Tidak menambah idle/point recovery atau move cost. Uji idle non-rally, diam saat rally, walking/manual/assisted jarak sama, boundary pushing, 30 detik reli bergerak, biaya hit/jump, recovery antargame tepat sekali, save/replay/pause. Diam saat rally masih dapat terkena komponen rallyDrain; itu berbeda dari bug idle di menu/training. Jika drain salah, reproduksi jalur pemicunya, jangan menaikkan capacity sebagai penutup.

**Tes dahulu:** existing EditMode math/config, `MobileOpponentTests`, `MobileMatchTests`, `LocalGateTests` stamina/flight, `M1GapTests` high-speed collision, doubles/replay snapshots. Daftar baseline seed dan max batch sebelum tuning; periksa trace tick, state, hitter, RNG, reaction deadline dan first divergence. Uji 30/60 FPS rendering terhadap fixed simulation, bukan menjanjikan bit-identical lintas platform.

**Acceptance:** lima lintasan berbeda pada parameter/pose setara dan terlihat natural; IN/OUT/net/reach tetap konsisten; human/AI sama-sama tidak punya kontak mustahil. Easy duration gate existing <1000 detik **tidak dilonggarkan** untuk menutup regresi. Ini gate fixture lama, bukan target durasi semua match manusia. Tidak sengaja membuat AI miss untuk mengejar durasi. Jika target fairness/shot baru konflik dengan gate, dokumentasikan konflik untuk keputusan, bukan retry sampai hijau. Stamina persis kontrak D-050.

### Tahap 6 — kamera, touchscreen HUD dan keterbacaan

**Source:** `CourtPrototype.Update` camera, `CourtUi.ControlRect/GameplayRects/UiActionRect/DrawGameHud/PointNotice`, `CourtDoubles` marker/position cues.

- [x] **T6a — CLOSED_LOCAL_PREVIEW.** Buat satu preview dari runtime game sebenarnya untuk HUD rally tunggal/ganda dan both ends, sebelum menerapkan layout luas. Pertahankan identitas menu biru/kuning dan Anton D-057; scoreboard gameplay boleh panel gelap semi-transparan sesuai lampiran. Tidak mengganti Home menjadi dashboard atau menyalin palet proyek lain. `t6a-preview-checkpoint1.json`: 1/1capture PASS,8PNG (2format×2ends×rally/Jump-held),all inspected. Cyan guide crosses shot labels; moved into T6c defect. Preview only,not broad redesign or motion/device acceptance.
- [ ] **T6b — kamera.** Kurangi headroom, atlet lebih besar, court penting/net/shuttle/baseline tetap terlihat. Pakai framing ringan fixed + damped adjustment seperlunya; no zoom besar/no shake besar. Ukur silhouette atlet dan area aman pada 1280×720, 1280×800, 1920×1080, 2400×1080; uji clear tinggi, jump, near-net, wide defense, doubles dan change-ends.
- [ ] **T6c — touch.** Tata ulang lima shot+Jump tanpa menghapus aksi/setting/fallback; visual tombol dan pointer hit rect memakai sumber sama. Main shot lebih mudah dijangkau; Jump menonjol hanya konteks relevan tetapi tetap dapat ditemukan/diakses. Guide hold-swipe muncul saat hold, tidak diputus oleh resize/pause/cancel/multitouch. Sisakan jalur pandang aktor/shuttle; minimalkan obstruction pada capture, tidak mengklaim tak pernah occlude dari satu still.
- [ ] **T6d — skor/feedback.** Panel lebih kecil dengan skor poin versus game jelas; energy di bawah, kontras teks diukur pada background komposit. Hilangkan string debug permanen; tampilkan outcome singkat “Poin kamu — lawan keluar”, fault dan server tanpa menampilkan grade internal. Tinjau garis hanya saat eligible dan tidak menghalangi rally. Bedakan marker pemain aktif dari saran posisi partner; jangan mengubah marker menjadi aim assist.
- [x] **T6e — state correctness (STATE_CLOSED_LOCAL_VERIFIED; storage OPEN_INTERMITTENT separately).** Current full20Edit+212Play and visible5/5,117PNG; failed older storage run retained, no storage-fix claim.  HUD Latihan tidak mewarisi “Giliran servis kamu” dari match sebelumnya; score/energy/error/success/review terikat state yang benar. Sebab poin berasal dari adjudication state, bukan parsing fragmen string untuk menebak pemenang.

**Tes dahulu:** `MobileUiTests` layout/visibility/point/flow, `MobileMatchTests` actual screens, settings/pause/replay input cancellation. Simulated pointer API boleh; native mouse/keyboard automation tetap dilarang. Periksa seluruh toggle dan menu actions tidak hilang. Target sentuh **≥48dp** adalah desain yang perlu konversi density/device; unit canvas/pixel PC bukan bukti dp fisik Android.

**Acceptance:** kedua/keempat atlet dan shuttle terbaca, skor tidak bersaing dengan gameplay, tidak ada text clipping/safe-area overlap/hitbox beda visual. Preview/disain, assertion layout, screenshot dan device-touch acceptance dilaporkan terpisah. Tidak menyebut ergonomi Android lulus dari test Editor.

### Tahap 7 — atmosfer indoor, flow, hasil dan final QA

**Source:** `CourtAudio.UpdateAtmosphere/PlayFootstep/PlaySound`, `CourtArena`, `CourtFlow`, `CourtUi.DrawGameHud`, `SinglesMatch.AwardPoint`, snapshot/result/replay.

- [ ] **T7a — arena finishing.** Setelah karakter/gerak lolos: panel navy/charcoal, roof beams/lamp fixtures sederhana, court lebih terang dan sekeliling redup, board sponsor fiktif minimalis, satu scoreboard live Just Smash. Kursi/wasit sederhana opsional menurut cost. Baked lighting hanya jika geometri benar-benar authored/static; jangan menjanjikan lightmap pada objek yang baru dibuat runtime. Tanpa tribun/NPC crowd/dekor berlebihan.
- [ ] **T7b — audio.** Audit ulang empat impact dan empat step existing; bedakan smash/clear/net dari contact event yang benar. Tambah squeak pada braking/direction-change grounded, bukan tiap frame atau di udara. Ambience ruangan kosong bukan indoor crowd baseline; reverb hall halus memakai fitur native hanya bila terdengar membantu. Shuttle sound opsional jika relevan/terdengar pada mix, bukan noise kontinu. Tidak musik/komentator/crowd chant.
- [ ] **T7c — officiating.** Score/fault call singkat berbeda dari komentator. Gunakan rekaman original/CC0/izin komersial jelas dengan provenance dan izin voice. Tanpa cloning suara orang, ripping broadcast, login layanan atau TTS berbayar. Jika clip legal belum tersedia, status `AUDIO_BLOCKED_ASSET`; bunyi sine bukan pengumuman suara yang selesai. Uji semua score/fault vocabulary yang benar-benar dipakai; call tidak tumpang-tindih serve berikutnya.
- [ ] **T7d — flow.** Athlete approach sebelum serve, point pause cukup tanpa mengubah window challenge diam-diam, reaksi menang/kalah ringan, change-ends konsisten. Challenge overturned mengoreksi reaksi dan skor; replay/pause tidak menggandakan event. Return menu/rematch tidak menyisakan actor/crowd/audio/state dari match lama.
- [x] **T7e — result tiap game.** CLOSED_LOCAL_VERIFIED: `t7e-t6e-checkpoint2.json`20Edit+205Play+3visible PASS,228unique;101PNG. Runtime/source matched; Android/whole-overhaul acceptance remains separate. Simpan final points setiap game sebelum `AwardPoint` mereset skor, termasuk game terakhir. Tambahkan data minimum pada match/snapshot/result, bukan modul statistik career. Validate array/count/range/winner/nomor game dan undo challenge agar riwayat tidak ganda atau palsu. Load save lama tanpa riwayat menampilkan data lama tidak tersedia, bukan merekayasa skor dari `Games`. Hasil menampilkan tim pemenang, skor per game dan total game, termasuk kalah pengguna.
- [ ] **T7f — bukti final.** Jalankan regresi lengkap, UI/capture controlled, natural tunggal Easy/Normal/Hard dan ganda Normal utuh Home–Result–Home pada 1×, replay/challenge, profile tanpa recorder, lalu APK source-bound. Periksa sound secara audible bila tool/operator benar-benar dapat mendengar; video muted/waveform/XML bukan listening. Pemilik mereview match utuh, bukan diminta mengetes pose/tombol granular.

**Tes dahulu:** `FeedbackTests` impact/footstep/pause/duplicate guard, `MobilePresentationTests` reactions/scoreboard/umpire, EditMode `SinglesMatch` scoring dan snapshot tests, `MobileUiTests` result, `LocalGateTests`/`MatchFlowTests` replay-snapshot, `MobileMatchTests` natural captures. Pertahankan miss silence, team reaction, score reset, correct challenge quota dan exactly-once recovery.

**Acceptance:** arena dan mix terasa indoor tanpa penonton; fore/backhand/stroke/footwork bisa dinilai dalam match utuh; hasil per-game nyata dan save/load compatible. APK/log/hash/source manifest cocok. Jangan mengklaim public-release-ready selama blocker legal, UX/data, motion/listening atau device penting belum ditutup.

## 5. Bukti, QA dan perintah lokal

### 5.1 Checklist bukti setiap tahap

- Sebelum/sesudah berasal dari game actual source, skenario/seed/difficulty/camera/resolusi sama. Isi bukti: source+asset+config hash, version Unity, waktu, test name, status, video/timeline/event, still contact/ready/landing terkait.
- Screenshots wajib untuk geometri/menu/HUD; motion/physics/flow wajib video kontinu 1×. Slow-motion hanya companion, tanpa interpolasi frame atau timeScale palsu. Still/gif pose sampling tidak menggantikan motion.
- Catat cadence capture aktual dari timestamp dan max gap. Target encoder 30 FPS, rata-rata capture, render FPS, dan device frame pacing adalah empat klaim berbeda.
- Periksa media: duration, frame/PCM count, sync, PTS/DTS, decode penuh, playback utuh, event coverage dan source identity. Jangan padding/stretch/retime audio yang gagal gate. Historical AV failure dipertahankan, tidak disebut fixed dari satu capture lolos.
- Simpan failed-run dan perbaikan beserta alasan di dokumen existing. Test skip/early-return karena batchmode tidak menjadi bukti screenshot/audio yang tidak dihasilkan. Hitung test names unik versus executions secara terpisah.
- Source dibandingkan setelah runner teardown; `Assets/InitTestScene*.unity` sementara tidak dikecualikan sembarangan. Jika setup build mengubah retained shaders/settings, freeze ulang dan tes final state yang benar-benar dibuild.

### 5.2 Cara menjalankan — hanya setelah izin eksekusi

Windows tool shell adalah Git Bash. Native executable memakai `C:/...`, bukan `/c/...`. Tidak ada Unity yang memegang proyek; jangan force-close Editor. Jalankan suite berikut **satu per satu**, beri nama run unik setiap invocation. Perintah ini dicatat, **belum dijalankan** untuk overhaul.

```bash
UNITY='C:/Program Files/Unity/Hub/Editor/6000.3.18f1/Editor/Unity.exe'
PROJECT='C:/Users/Dewangga Ramadhan H/Documents/Portofolio/JustSmash'
RUN="$PROJECT/evidence/shareable/$(date +%Y%m%d)/overhaul-$(date +%H%M%S)"
export JUSTSMASH_QA_RUN="$RUN"
# os.makedirs tanpa exist_ok menolak overwrite folder evidence lama.
python -c 'import os; os.makedirs(os.environ["JUSTSMASH_QA_RUN"])'

# EditMode; setelah suite selesai, parse XML sebelum lanjut.
"$UNITY" -batchmode -nographics -projectPath "$PROJECT" \
  -runTests -testPlatform EditMode \
  -testResults "$RUN/edit.xml" -logFile "$RUN/edit.log"

# PlayMode non-Capture. Jangan menjalankan semua kategori Capture.
"$UNITY" -batchmode -projectPath "$PROJECT" \
  -runTests -testPlatform PlayMode -testCategory '!Capture' \
  -testResults "$RUN/play.xml" -logFile "$RUN/play.log"

# Visible Game View; tidak -batchmode/-nographics; tidak native input.
"$UNITY" -projectPath "$PROJECT" -runTests -testPlatform PlayMode \
  -testFilter 'MobileMatchTests.CaptureApproachJumpSmashPilot|MobileMatchTests.CaptureControlledFootworkStrokesAndJumpAtOneTimespeed|MobileMatchTests.CaptureLocomotionCloseupWithGameAudio' \
  -testResults "$RUN/motion.xml" -logFile "$RUN/motion.log"

# Natural 1x: tiga tunggal + satu ganda; verify actual count dari metadata.
"$UNITY" -projectPath "$PROJECT" -runTests -testPlatform PlayMode \
  -testFilter 'MobileMatchTests.CaptureOneTimespeedMatchesWithGameAudio|MobileMatchTests.CaptureNaturalDoublesMatchWithGameAudio' \
  -testResults "$RUN/natural.xml" -logFile "$RUN/natural.log"

# Profil tanpa readback/encoder, Editor PC bukan bukti performa HP.
"$UNITY" -projectPath "$PROJECT" -runTests -testPlatform PlayMode \
  -testFilter 'MobilePresentationTests.ProfileUnrecordedDoublesAt30And60' \
  -testResults "$RUN/profile.xml" -logFile "$RUN/profile.log"

# Setelah seluruh gate lokal dan backup APK + evidence/mobile/build.
"$UNITY" -batchmode -quit -projectPath "$PROJECT" -buildTarget Android \
  -executeMethod AndroidBuild.RunMobile -logFile "$RUN/build.log"
```

Tests tidak memakai `-quit`; Test Runner mengelola shutdown. Build memakai `-quit`. `PreferenceFixture` membaca `JUSTSMASH_QA_RUN` dan membuat folder per test unik; `-testResults` sendiri tidak mengatur file media. Audit kembali semua `File.Write*` sebelum menambah harness.

**Dilarang:** `MobileMatchTests.NativeMouseAndKeyboardUseGameView`, memilih semua Capture, discovery HP/emulator, dan rerun helper checkpoint lama yang output-nya tetap. Helper tersimpan dalam `evidence/shareable/20261115/roller-polish-040359/d060-checkpoint-tools1/` hanya referensi prosedur sampai path/hardcoded source identity diaudit dan runner baru diarahkan ke output unik.

Pemeriksaan XML berikut memakai stdlib; ulangi setiap XML, bukan exit Unity saja:

```bash
python -c 'import sys,xml.etree.ElementTree as E; r=E.parse(sys.argv[1]).getroot(); c=r.findall(".//test-case"); bad=[(x.get("fullname"),x.get("result")) for x in c if x.get("result")!="Passed"]; print({k:r.get(k) for k in ("result","total","passed","failed","skipped","inconclusive")}); print("NON_PASS",bad); assert c and len(c)==int(r.get("total")) and not bad and r.get("result")=="Passed"' "$RUN/play.xml"
```

### 5.3 UI/replay tambahan dan regresi data

Jalankan explicit test `MobileUiTests.AllRuntimeScreensAtFourApprovedResolutions`, `MobileMatchTests.FullUiAndGameAudioCaptureSmoke`, `MobileMatchTests.DoublesUiFlowCaptureUsesActualRuntimeScreens` dan `MobileMatchTests.CaptureReplayChallengeModesWithGameAudio`; verifikasi exact names sebelum perintah. Matriks lengkap: Home, Setup tunggal/ganda, Ready/Serving, Rally, Point, Pause, Settings, Credits, Confirm, GameEnd/Interval/ChangeEnds, replay gameplay/additional angle, hasil/rematch, Training sesudah match. Bukan semua kasus harus membuat harness baru: extend fixture existing.

Data: save/load serving/rally/point/game interval/replay pause, invalid snapshot ditolak sebelum mutasi, replay skip tidak mengubah RNG/score/root, correct/mistaken challenge quota tepat, per-game result rollback konsisten. Seluruh data lokal/prefs byte-identik sebelum/sesudah QA kecuali output fixture yang disengaja dan terisolasi.

### 5.4 Performance dan ukuran

Target produk: Android RAM perangkat ≥4 GB, 30 FPS, keseluruhan instalasi+aset+data ≤1 GB. **RAM 4 GB bukan budget konsumsi game.** Pada PC-only, laporkan yang dapat diukur: no-recorder frame times p50/p95/p99, CPU/GPU bila tersedia, draw calls/batches, skinning, allocated/native memory dengan label Editor-inclusive, APK bytes dan per-asset payload. Uji empat atlet dengan arena final, bukan dua atlet pada scene kosong.

Device gate tetap ditunda: steady-state pacing/1% low, frame spikes, thermal setelah match panjang, peak app RAM, multitouch/safe-area, installed footprint + growth data. Protokol di `docs/TESTING.md` tetap; tidak membuat threshold drop-frame baru agar lulus. APK kecil atau profiler PC tidak memenuhi gate Android. Jika suatu saat pemilik mengizinkan device QA, lakukan update/launch tanpa uninstall/clear data dan tanpa membaca file pribadi.

### 5.5 Arti status akhir

| Status | Bukti yang dibutuhkan |
|---|---|
| `IMPLEMENTED` | Runtime berubah dan fitur terhubung, bukan aset isolated |
| `LOCAL_QA_PASS` | Suite pada source final benar-benar PASS; failure lama tetap tercatat |
| `MESH_REVIEWED` | Pose front/back/side/grip dan deformasi actual render diperiksa |
| `MOTION_REVIEWED` | Continuous sequence 1× mencakup gerak yang diklaim; bukan playback ended saja |
| `AUDIBLE_REVIEWED` | Mix game benar-benar didengar; waveform, muted video, clip name tidak cukup |
| `APK_VERIFIED` | Artifact hash, source identity, packaging/permissions/signature/payload terbukti |
| `DEFERRED_OWNER_PC_ONLY` | Android touch/performance/thermal/installed footprint belum dikerjakan |
| `OWNER_ACCEPTED` | Pemilik menyetujui pengalaman match utuh; agent tidak mencentangnya sendiri |

Tool/operator yang hanya bisa melihat still atau membaca PCM tidak boleh menutup listening/continuous-motion dengan inferensi. Kirim artefak asli dan status pending. Ini bukan alasan berhenti mengerjakan QA teknis independen yang masih diizinkan.

## 6. Risiko, alternatif, dan batas perubahan

### 6.1 Risiko dan plan-B

| Risiko | Pemicu berhenti / bukti | Alternatif yang sah |
|---|---|---|
| Paket gratis tidak menyediakan sportswear lengkap | Lisensi/subset gagal, perlu membangun ulang pakaian/rig, atau tiga perbaikan masalah sama gagal | Tolak kandidat; coba kandidat matang berikut dalam shortlist. Bila semuanya gagal, laporkan `CHARACTER_BLOCKED_ASSET` dan minta keputusan jalur baru. Jangan menyebut recolor kasual sebagai sportswear |
| Retarget menghasilkan deformasi atau menimpa IK | Avatar valid tetapi bahu/crotch rusak, grip lepas, contact/foot plant regresi | Batasi ke satu evaluator native dan rig mapping yang teruji. Rollback slice integrasi; tidak memasang paket atau memperpanjang tulang untuk menutup kegagalan |
| Preparation terlihat patah dengan immediate hit | Pilot late tap/sudden high-low contact masih snapping | Perbaiki antisipasi/bounded pose correction dan recovery di kontrak existing. Bila tidak cukup, laporkan konflik; jangan diam-diam menambah input delay |
| Physics/AI baru mengubah rules atau pacing | First-divergence trace, impossible hit, predictor tidak sama, gate lama gagal | Pisahkan perubahan shot/movement/AI, uji satu faktor per slice. Restore slice gagal dari backup; jangan menurunkan difficulty atau melonggarkan tes agar hijau |
| Net/garis berubah visual tetapi adjudication tetap lama | Uji boundary, net center/edge atau high-speed crossing beda hasil | Gunakan helper geometri kecil bersama untuk rendering/prediction/collision; jangan mengandalkan visual prefab tanpa rule test |
| Material/karakter menaikkan biaya render | Empat atlet melampaui budget lokal, payload/texture melonjak | Kurangi material/texture duplikat, gunakan shared assets dan geometri static sederhana. Tunda dekorasi opsional, bukan menghilangkan gerak/kontrol wajib. Android tetap belum terbukti |
| Audio score/squeak tidak tersedia legal | Tidak ada izin reuse/voice atau rekaman tidak layak mix | Gunakan hanya rekaman yang lolos; catat fitur audio belum selesai. Tanpa ripping/cloning/sintesis pengganti yang diklaim realistis |
| Recorder gagal sync atau capture lambat | PCM/video clock beda, max gap besar, output tidak finalized | Pertahankan failed run, ukur recorder terpisah dari game, perbaiki harness pada slice test-only. Jangan retime/padding untuk memalsukan gate |
| Save/replay baru merusak data lama | Snapshot valid lama gagal, history score duplikat, data fixture bocor | Optional/versioned fields dengan validasi sebelum mutasi; rollback perubahan serializer, restore hanya fixture-owned delta. Jangan clear data atau mengganti save atomik tanpa reproduksi |
| Scope pelebaran akibat audit lama | Career, onboarding baru, public signing atau storage redesign masuk tanpa izin | Pisahkan daftar publikasi di §6.3. Selesaikan overhaul yang disetujui; tanya hanya keputusan yang betul-betul di luar scope |

### 6.2 DO-NOT-TOUCH

- Source/aset/scene/config/APK tidak diubah pada sesi plan. Setelah eksekusi diizinkan, ubah hanya file terkait task aktif dan test yang membuktikannya.
- Jangan edit `docs/AI_RULES.md` atau `docs/UI_REFERENSI.md`; jangan buat WORKFLOW/worklog baru, rewrite riwayat, menghapus failed-run, atau menandai acceptance owner sendiri.
- Jangan hapus sumber asset asli, lisensi, evidence lama, backup, save/prefs pengguna, signing material, `.git`, atau dirty/untracked work yang bukan milik task. Jangan mencetak credential atau connection string ke log/plan.
- Jangan mengubah Official15, deuce/cap21, grade D-049, stamina D-050, immediate input/contact, finite reach, joystick modes, lima input shot, review/challenge, atau fitur ganda untuk menyederhanakan polish.
- Jangan hidupkan career/adaptive AI/customization/multiplayer; jangan tambah SDK, paket, akun, biaya, upload model, pipeline render baru atau port lain tanpa keputusan baru.
- Jangan jalankan native input/HP/emulator, publish atau `git push`. Jangan kill proses pemilik atau restore seluruh repository untuk rollback satu slice.

### 6.3 Temuan publikasi terpisah, bukan scope otomatis

Audit sebelum plan menemukan instruksi awal hold–release/shared-stick/jump belum lengkap, akses lanjut/muat hanya lewat Pause, feedback save/load tersaring, dan backup belum dipakai oleh jalur load. Catat ini sebagai kandidat pekerjaan UX/data sebelum playtest/rilis, **bukan** izin redesign persistence di overhaul. HUD point/training/marker yang terkait langsung masuk Tahap 6. Perubahan resume/onboarding terpisah memerlukan scope pemilik; diagnosis `File.Replace`, historical AV lag, public signing dan device QA tidak otomatis selesai karena plan atau build baru.

## 7. Handoff dan keluaran sesi plan

### 7.1 Dokumen yang diperbarui sekarang

- `.hermes/plans/2026-11-22_gameplay-visual-character-overhaul.md`: antrean implementasi, karakter/animasi, acceptance dan prosedur QA; seluruh task implementasi masih unchecked.
- `docs/05_DECISION_REGISTER.md`: D-061 hanya merekam klarifikasi pemilik, **bukan approval eksekusi**.
- `docs/SESSION_HANDOFF.md`: tautan plan dan batas sesi aktif; checkpoint D-060 di bawahnya tetap historis.

Backup dua dokumen existing: `.backups/overhaul-plan_20261122_011738/`. Backup kedua sebelum merge lampiran kedua: `.backups/overhaul-plan-merge_20261122_200549/`. Baseline hash sebelum penulisan: `C:/Users/Dewangga Ramadhan H/AppData/Local/hermes/cache/scratch/justsmash-overhaul-plan-20261122/planning-baseline.json`. Validator read-only: `check_plan.py` dalam folder scratch yang sama; memeriksa headings/task IDs, referensi test/path, backup pertama dan perubahan di luar allowlist tiga dokumen. Scratch bukan source produk atau bukti gameplay.

Tidak ada download/import aset, modeling, clip baru, Unity run, APK baru, device action atau commit pada sesi ini. Hasil verifikasi dokumen/hash dicatat setelah validator benar-benar dijalankan; test D-060 tidak dihitung sebagai tes baru.

### 7.2 Ketika eksekusi nanti disetujui

1. Baca plan ini, D-061 dan handoff terbaru, lalu aturan/testing/legal proyek. Cocokkan disk terhadap baseline; jangan menganggap hash/line lama tetap berlaku.
2. Mulai T0/T0b, lalu T1 inti tanpa penonton. Prioritaskan T2 pilot karakter/gerak sebelum arena finishing. Scope aset adalah evaluasi legal gratis dan integrasi hanya setelah lolos, bukan persetujuan kandidat tertentu.
3. Sesudah setiap slice, perbarui acceptance dan bukti di dokumen existing. Saat runtime berubah, selaraskan PRD/GDD/DESIGN/ARCHITECTURE/ROADMAP/TESTING hanya pada bagian yang benar-benar berubah; jangan menulis fitur rencana sebagai kondisi terpasang.
4. Lanjut semua task independen yang diizinkan tanpa approval berulang. Berhenti pada blocker nyata, tiga kegagalan per masalah, atau batas izin; jangan menutup task dengan gambar/source lama.

**Prompt singkat untuk sesi eksekusi — digunakan pemilik hanya saat siap memberi izin:**

> Eksekusi `.hermes/plans/2026-11-22_gameplay-visual-character-overhaul.md` di workspace JustSmash. Mulai dari kondisi disk dan T0, lanjut T1 inti lalu pilot karakter/animasi T2 sebelum dekorasi. Pertahankan D-050, PC-only, kontrol/aturan aktif, Rp0 dan no-push. Aset harus legal dan lulus mesh/rig/gerak sebelum integrasi. Jalankan QA dan bukti before-after tiap tahap; laporkan blocker asli, bukan klaim dari test lama.

### 7.3 Pemetaan permintaan ke gate akhir

| Permintaan lampiran | Task / gate |
|---|---|
| Arah stylized simulator + prioritas kualitas (lampiran kedua) | Seluruh tahap dinilai terhadap arah §1; bobot kerja terbesar Tahap 2–4 |
| Arena tanpa penonton, court/net/material benar | T1a–T1d; dekorasi secukupnya T7a |
| Karakter natural, sportswear, dua identitas, grip/racket | T2a–T2d, kontrak §3.2 dan pilot §3.3 |
| Footwork, delapan kelompok stroke, jump/landing, transisi | T3a–T3d dan T4a–T4e; daftar motion §3.5 |
| Flight/contact/movement/AI | T4b–T4e dan T5a–T5d |
| Stamina | T5e; bagian lampiran yang konflik **tidak dipakai** sesuai pilihan D-050 |
| Kamera/HUD/touch/score | T6a–T6e; Android touch tetap deferred |
| Audio, serve/point/change-ends/result | T7b–T7e, per-game result data nyata |
| Bukti tiap tahap dan whole-match | §5 dan T7f; numeric/visual/motion/audio dipisahkan |
| 30 FPS perangkat ≥4 GB, game ≤1 GB | §5.4; PC/APK bisa diuji setelah izin, device tidak diklaim lulus |

**Finish line plan:** seluruh permintaan terpetakan, konflik keputusan dinyatakan, source/test refs dapat ditemukan, dan hanya dokumen berubah. **Finish line produk:** semua gate implementasi/QA yang diizinkan punya bukti aktual; device/owner acceptance yang belum dilakukan tetap terbuka.

## Sources

[1] https://quaternius.itch.io/universal-animation-library
[2] https://quaternius.com/packs/universalbasecharacters.html
[3] https://helpx.adobe.com/creative-cloud/faq/mixamo-faq.html
[4] https://static.makehumancommunity.org/assets/assetpacks/makehuman_system_assets.html
[5] https://static.makehumancommunity.org/assets/assetpacks/pants03.html
