## Delta aktif D-062 — overhaul bertahap

- T4e pose adds optional FootPose(recorded/planted/airborne,left/right world plants) to main/extra replay actors. RestorePose reconstructs captured post-step plants without advancing them again; clears footfall events, preserves live Present behavior. Snapshot validation checks finite/bounded plants before mutation;v2/v3 missing fields retain legacy path. Restore size guard8,000,000characters matches existing1500frame capacity (old1M rejected valid full buffers);frame/schema limits unchanged. Tests/source-bound evidence in wave4-pose-checkpoint1.json;natural-motion/media acceptance remains separate.

- T4a Net/Lift visible follow targets dibedakan tanpa field/API/clip baru:Net sentuhan pendek,Lift naik. Active contact tetap instant;timing/recovery,physics/reach dan lowerbody unchanged. Numeric64focusedPASS;Net render obstructed in controlled closeup,visual task masih open.

- T3a ready-only breath memakai elapsed pose existing (0.8deg/3.2s/.45s blend); root/feet dan active stroke unchanged. Tidak ada save field, clock, dependency atau pose history baru. Rekonstruksi setelah pose lain locally tested; full locomotion/asset gate tetap terbuka.

- T5b physics locally verified (full motion pending): shared kinematic MoveAthlete for human and all bots, velocity stored per actor. Ground response10*cap/s accelerating,16.666667*cap/s braking, airborne human1*cap/s; no root motion/rig changes or contact timing changes. Bounds clear outward velocity. Snapshotv3 adds optional validated athleteVelocity[4]; legacy field missing→rest. Administrative ResetAthleteMovement clears jump/momentum, pause/replay preserve live values. Tests/status in TESTING; visual lunge/push-off/natural motion remains gated by character pilot.

- CourtArena membuat siluet penonton statis saat Boot. Enam mesh tribun berbagi satu material, tanpa Animator, collider, atau audio penonton. Mesh dan material dilepas bersama aset arena saat scene berganti. Officials dan scoreboard tetap. Court floor y0, painted edges40mm termasuk IN; NetTop/CollideNet dipakai mesh/collision/landing/AI prediction. CelCourt tetap, sampled grain dan shared soft contact shadows.
- SinglesMatch.GamePoints menyimpan pasangan side0/side1 berurutan sebelum game reset dan saat match selesai. Snapshot version3, baca version1/2; result version2 menambah gamePoints dan mempertahankan finalGamePoints. Slot singles-v1.json/atomic SaveMatch tetap, bukan fitur multi-slot baru.
- Restore memvalidasi history count, legal terminal score, game wins/number/winner dan history sebelum review sebelum mutasi live state. Review yang masih tersedia harus cocok dengan satu AwardPoint dari beforePoint. Migrasi lama memakai -1/-1 untuk data yang tidak ada; final current Points atau transisi review yang tersimpan dapat membuktikan game terakhir, bukan menebak game sebelumnya.
- Result tetap IMGUI/Anton/biru-kuning, dengan pemenang eksplisit, total game dan skor setiap game; unknown ditulis Data lama tidak tersedia. Karakter/motion masih baseline, belum lolos gate kandidat baru. Bukti/status aktual di [[TESTING]] dan plan D-062.
- T6e: PointCause dari adjudikasi landing/service/body/net/below-net; PointReview mencatat None/Confirmed/Overturned. Optional fields snapshotv3; v1/v2/v3 tanpa field menampilkan Sebab poin tidak tersimpan, bukan inferensi debug. Challenge sah memperbarui cause/status; reset Training/next-point membersihkan, MatchEnd mempertahankan hasil terakhir. Enum/rentang/state divalidasi sebelum mutasi. PointNotice memakai Match.Server dan state tersebut, bukan feedback internal. MenuNotice terpisah untuk outcome Save/Load di Pause/Settings. Current state verification20Edit+212Play+5visible PASS. Historical explicit-save File.Replace remains OPEN_INTERMITTENT, not a fixed storage path; see TESTING/ERROR_LOG.
- T5e: singles recovery multiplies the same AthleteFatigueSpeed(1) as chase. D-050 config/costs/capacity/recovery unchanged. Active athletes share rally-time drain; movement itself costs zero. Pause/replay freeze and once-only intergame recovery verified in both formats/ends.

## Riwayat arsitektur D-055 — polish tanpa migrasi

Pertahankan Unity built-in, partial CourtPrototype, IMGUI, NPC Casual Set00 dan native clip/IK existing. Perubahan presentation tidak mengambil alih root/collider, kontak, physics, scoring atau RNG. D-057 sudah mengizinkan dan menerapkan UI biru/kuning, Anton/OFL, CelCourt dan ArenaText depth-aware. Home/Setup memakai staging; Result memakai kamera winner-aware tanpa memindahkan simulation roots. Grip pada palm/socket, recovery/preparation native IK; footsteps empat aktor consumed once. Crowd empat rig foreground +44 static dan sinyal umpire terhubung skeleton. Proteksi QA mencakup semua local-game-data, bukan prefs saja; evidence run unik. Detail dependensi/gate di plan Roller Champions 2026-11-15.

## Baseline arsitektur D-054 — 2026-11-15

Runtime kembali exhibition tunggal/ganda pre-career (`home-preview-green1-source.json`). Career*.cs, CourtCareer.cs, modul world/progression/finance/life/archive serta hook per-atlet career dicabut. Shared runtime dipulihkan byte-identik, termasuk ShotIntent.Window0,16detik dan snapshot exhibition baseline. Tidak membuat stub/no-op career. Kamera, arena, reaksi, input/settings dan empat atlet ganda tetap baseline tersebut. Satu tes no-career baru adalah satu-satunya delta dalam Assets terhadap baseline.

## Riwayat arsitektur D-053 — career berikut sudah dicabut

Checkpoint atribut gameplay: `CourtCareer` memetakan empat atlet untuk Footwork/Stamina/Power/Technique. Power memodifikasi durasi solve ke target sama; career ground crossing memakai interpolasi sub-step pada marker dan actual. Technique player memakai rata-rata core/subskill kontak (Backhand/Serve terpisah), bot core sendiri; biaya/input window/presisi hanya career. `ShotIntent.QueueFor` memvalidasi window finite0..0,2detik, API Queue tetap0,16. Snapshotv2 `intentWindow` opsional0 berarti legacy0,16; validasi sebelum mutasi. Presisi deviasi geometri kontak deterministik bounded0,35m, bukan auto-aim/reach/grade. `career-technique-regression`44/44Edit+100/100UI,1.142file identik; bukan full Play. Qualifying/Balance masih terbuka.

Pertahankan Unity built-in, aset existing, partial CourtPrototype dan simulasi fixed-step. Presentation arena dipisah dari skor/contact; static geometry/material dipakai ulang, crowd tidak memakai puluhan Animator. Ganda menambah state empat atlet/servis/receiving dan snapshot yang divalidasi sebelum mutasi. Career simulation/save local dipisah dari scene dan diintegrasikan terakhir. Tidak migrasi pipeline atau tambah package. D-053 mengizinkan mode baru; D-052 dan batas lainnya tetap.

Career.cs/CareerStore menyimpan slot opaque terpisah dari exhibition; CareerWorld menangani kalender/RNG/draw/ranking; CareerCompetition menangani token pending/exactly-once result serta partnership. CourtCareer memakai menu/pointer/snapshot existing, bukan simulasi fixture terpisah. Pair ID tetap per dua anggota; partner tidak bisa diganti setelah entry sampai turnamen selesai. Ranking hasil pasangan melekat pair ID, tidak dibawa pasangan baru. CareerProgression menangani core5/subskill7, poin target/general, upgrade/caps, fatigue/injury dan reset/refund90%; schema3, migrasi v1/v2 di memori tanpa rewrite saat Load. CourtCareer menyambung tab latihan dan confirmation existing. CareerFinance menyimpan ledger bersaldo cocok, prize akhir turnamen (ganda50:50), coach opsional/30hari dan travel gratis/berbayar; semua uang fiktif. UI Keuangan menggunakan modal biaya existing dan dua transaksi per halaman. Schema4 finance memigrasi schema3 tanpa reset data/checkpoint finance atau rewrite slot saat Load. CareerLife/schema5 menyimpan negosiasi Sponsor/Contract fiktif, pembayaran bulanan bertoken, cooldown/reputasi/popularitas dan kegiatan kalender gratis. UI Kehidupan92/92PlayPASS/still reviewed; bonus kemenangan memakai token deal+match, profile naik pada committed result (Edit33/33PASS), tanpa mengubah exhibition. Migrasi pre-life1/1 baseline PASS; cadence36/36 dan lifecycle3pembayaran/expiry/clause1/1PASS. CareerArchive menghitung statistik all-time/tahun ini/tahun lalu dan paging6baris dari match/tournament/ledger persisted tanpa counter duplikat; Edit38/38, UI93/93PASS source1.142identik. CareerArchive juga membaca detail hasil/nama pasangan/lawan/reward/transaksi dan capaian main/menang/gelar pertama tanpa counter duplikat atau schema baru; Edit40/40, UI94/94PASS source1.142identik per-run, still reviewed. Tidak mengarang skor game lama. CareerProgression.AdvanceAging meninjau >35tahun per90hari, satu atribut fisik/baseline turun1 bila inactive90hari atau masih cedera; latihan/hasil terbaru melindungi, tanpa auto-growth muda. Train mencatat aktivitas sebelum kalender; Edit43/43 + UI95/95PASS satu freeze1.142. CourtCareer memetakan empat ID ke atribut career, Footwork memodulasi gerak career saja; kapasitas stamina per-atlet mengikuti rumus D-035. Snapshot v2 energyCapacities opsional menjaga replay/save dan legacy shared capacity tanpa rewrite; validasi sebelum mutasi. career-athlete-stamina-regression Edit43/43+UI97/97PASS satu freeze1.142identik. Full entry/qualifying dan Power/Technique/Balance belum lengkap. Parameter relasi/entry/point provisional; bukan data BWF.

## Riwayat kebijakan — D-037..D-042, 2026-11-03
Instruksi owner terbaru menggantikan aturan lama yang bertentangan; bagian checkpoint/run lama adalah riwayat, bukan izin aktif. D-044 mengizinkan eksekusi M0–M2 sampai satu APK checkpoint baru; batas docs-only sebelumnya berakhir. Otorisasi implementasi tetap M0–M2; M3+ tidak dimulai otomatis, termasuk setelah acceptance M2. Tidak ada import aset/dependency, biaya, push, destructive reset, atau install/launch APK HP tanpa izin terpisah.

Review owner hanya pengalaman modul utuh: singles lengkap; character creation lengkap; career terintegrasi; final game. Ini kelompok review, bukan penambahan scope. Tidak meminta owner menguji pukulan/animasi/tombol/fault secara granular. Agent menangani unit/integration/regression, natural automated match, inspeksi video/motion/audio, dan performance sesuai izin. Keputusan desain tetap memerlukan approval, berbeda dari testing.

Subagent dan commit lokal bersyarat tetap D-038/D-039. D-043 menyetujui NPC Casual Set00 untuk evaluasi import, default joystick Fixed (Floating tetap pilihan), servis charge1detik/power0..1/feedback konsisten/cancel aman. Grade hanya Timing + Contact; Direction/Power trace tanpa kontribusi grade. Rally sekitar30detik menurunkan stamina cukup banyak dan mulai memengaruhi performa, tetapi belum habis dan rally bisa berlanjut; jalan/lari drain0. Bobot/formula grade, biaya/kurva/efek stamina baru belum approved. Unity Personal memenuhi kondisi pribadi non-klien tanpa pendapatan yang dinyatakan owner; tidak meminta pembahasan keuangan lagi kecuali kondisi/terms relevan berubah. Target karakter/animasi/atmosfer singles tetap. Approval evaluasi NPC bukan izin import aset lain/dependency/M3 atau device action. D-044 menggantikan batas docs-only; eksekusi approved sampai APK checkpoint, tanpa mengubah proposal angka menjadi approval.

Rujukan keputusan/proposal: [Decision Register](05_DECISION_REGISTER.md). Riset aset/lisensi: [Legal dan Assets](04_LEGAL_AND_ASSETS.md). Gate bukti: [Testing](TESTING.md).

# Just Smash — Architecture

## FEEDBACK-013 — runtime delta
Movement energy charged after actual clamped displacement. MarkLanding simulates copied trajectory and stores world marker at flight change. ContinuePoint uses scaled time with3s provisional threshold, stops on replay/pause/tester and persists pointWait in snapshot. CourtAudio loads four Resources/Audio/racket recordings; player shot selects variant/gain; serve passes explicit technique rather than stale lastStroke. Other event sounds remain procedural. No audio SDK/dependency/network introduced.



## APPROVED-012 — runtime delta
CourtPrototype routes technique pointer/keyboard press through TapShot; jump swipe still uses BeginShot/ReleaseShot. Tap currently uses neutral midpoint power; hold-swipe retains elapsed charge. StaminaStat 1–100 scales capacity/recovery per D-035 and persists in Snapshot; schema remains version1 with stat70 default. Restore validates energy against stat-dependent capacity before mutation and clamps serialization roundoff within0.001. Contact faults use swept body Bounds and net intersection with body/racket Bounds for current procedural placeholders; rig-specific collider coverage remains future replacement, not claimed anatomical fidelity. Assisted movement no longer chases its own outgoing shot in matches.



## Koordinat dan identitas M0-09/M0-10
- Unit: metre; origin `(0,0,0)` pada tengah net; `+Y` atas. Singles `X ±2.59`, doubles `X ±3.05`, baseline `Z ±6.70`, short service `|Z|=1.98`, doubles long service `|Z|=5.94`; net tengah 1.524 m, tape/post 1.55 m. Batas singles/doubles/baseline/net kini dibaca dari prototype-config.json; angka default tetap sama.
```text
                     +Z / far baseline +6.70
      X -3.05    -2.59     0      +2.59    +3.05
         | doubles |    singles    | doubles |
         |--------- long service +5.94 -----|
         |--------- short service +1.98 ----|
         |========== NET Z=0 ==============|
         |--------- short service -1.98 ----|
         |--------- long service -5.94 -----|
                     -Z / near baseline -6.70
```
- Default player near `-Z`, opponent far `+Z`. D-034 change-ends rotates X/Z by 180 degrees through EndPoint; camera remains behind player. Analog up means toward net on either end. `Screen.safeArea` uses bottom-left pixels, GUI uses top-left coordinates; UiOrigin converts once for both input and rendering.
- ID convention: opaque lowercase prefix + GUID N (`athlete_<32hex>`, `tournament_<32hex>`, `save_<32hex>`, `match_<32hex>`). Display names never keys. Do not create athlete/tournament/career records until their milestone. Current single exhibition slot `singles-v1.json` is stable save-slot key, not athlete identity; snapshot schema `version=1`. A future multi-slot migration must preserve this slot, add IDs without deriving them from names, and test backward compatibility. No unused ID generators or career scaffolding added.

## CLOSURE-011 — otoritas dan penutupan aktif
Instruksi pemilik terbaru mengizinkan audit dan penutupan teknis lokal seluruh M0–M2, tanpa approval ulang perubahan teknis dalam scope. D-031 menunda review manual; D-034 menyetujui world-space change-ends/player-centric camera serta 3% salah call pada landing eligible dalam 5 cm, maksimum 1/match. D-016 menetapkan joystick/manual+assisted, bukan eksperimen swipe A/B. D-017 menetapkan Official15 saja; Classic21 terkunci sampai career2028, Quick16 dihapus.

Requirement dan acceptance di bawah tetap berlaku kecuali digantikan keputusan APPROVED lebih baru. Hanya status checkpoint lama yang merupakan **riwayat**, bukan status aktif. Pernyataan lama M2 dibekukan, P-006 belum disetujui, Quick16 default, belum ada build, atau lisensi aktivasi terblokir tidak berlaku lagi. Status per task dan closure plan aktif: `evidence/M2/closure-task-audit.json`; hasil baru dicatat di TESTING. Izin aktif D-037..D-042: commit lokal boleh dengan audit; HP install/launch memerlukan izin terpisah; push, dependency baru, biaya dan M3 tetap dilarang.

Pisahkan Implementation Complete, Local QA Passed, Device QA Pending, Owner Acceptance Pending, dan Milestone Fully Verified. Task canceled M1-09 dianggap requirement superseded terpenuhi, bukan fitur swipe yang diimplementasikan. Device QA pending bukan otomatis BLOCKED_ENV tanpa kegagalan lingkungan konkret.


## M1 D-032 local additions
- CourtPrototype partial files: CourtMotion visual-only in-place poses; CourtDiagnostics development debug, VFX PlayerPrefs and offline tester JSONL. Physics/hit authority remains FixedUpdate/ReleaseShot. No new packages/network/assets.

## Riwayat scope — 2026-11-02
Pemilik mengizinkan implementasi seluruh scope sampai M1, tidak M2+. D-016–D-028 dan instruksi chat terbaru menggantikan proposal lama: joystick kiri untuk gerak sekaligus arah pukulan, enam sektor tanpa landing/trajectory preview; jump tombol hold-swipe dengan guide teknik; assisted menggerakkan atlet sepenuhnya tanpa input movement. Default Official15, Classic21 Settings terbuka saat career 2028; Quick16 dihapus. Scoring penuh, challenge, stamina match, career, sponsor dan doubles tetap milestone sesudah M1. A/B swipe movement dibatalkan; gate M1 menjadi joystick/manual+assisted pada high lob, smash approach, near-net. Uji AVD Android resmi untuk runtime; HP hanya untuk feel multitouch dan performa perangkat nyata. Status historis di bawah bukan status terkini.


Versi 0.2 | 2026-11-01 | Rancangan turunan GDD/roadmap; belum diimplementasikan.

## Keputusan teknis

Engine Unity disetujui (P-011), implementasi C#. Versi editor, URP/render pipeline, Input System, scripting backend dan SDK/NDK/OpenJDK belum diaudit; catat versi aktual sebelum build. Gunakan fitur bawaan atau dependency yang sudah tersedia sebelum menambah package. Semua pilihan wajib cocok dengan Rp0, offline, Android dan target performance.

Unity sebagai alat authoring/build tidak mengizinkan akun pemain, Unity Cloud, analytics atau runtime endpoint otomatis. Eligibility dan kebutuhan aktivasi/install harus diaudit, bukan diasumsikan sudah terpenuhi. Jangan mengganti engine jika toolchain gagal tanpa persetujuan.

## Batas tanggung jawab

Batas berikut logis, bukan perintah membuat satu folder/class/interface untuk setiap baris sekarang.

| Area | Tanggung jawab | Mulai ketika |
|---|---|---|
| Core/config/debug | Identitas stabil, config yang dipakai, seed dan logging terkontrol | M0 sesuai kebutuhan |
| Input | Input editor/touch menjadi aksi gameplay; eksperimen A/B/assisted terpisah | M0 minimal, M1 |
| Match/scoring | State pertandingan, servis, fault, skor, hasil | M2 |
| Movement/shuttle/animation/camera | Gerak fisik, contact window per stroke, visual sinkron | M1 |
| AI | Persepsi terbatas, taktik, difficulty dan partner | M2–M3, partner M6 |
| Persistence | Snapshot atomic/versioned/backup, migrasi, isolasi slot | Saat save digunakan M2/M4 |
| Career/simulation | Kalender, world sim headless, ranking, entry, progression | M4–M5 |
| Economy/content | Ledger dan reward satu kali, sponsor, cosmetic content | M7 dan kebutuhan reward sebelumnya |
| UI/audio | Menampilkan state/aksi, bukan menyimpan aturan pertandingan tersembunyi | Bertahap sesuai fitur |

Scene minimum M0 harus dapat load dan build; boot/menu placeholder/court/test scene dibuat hanya sejauh task memerlukan. Tidak membuat kerangka career sebelum gameplay gate lolos. Assembly definitions hanya jika ada pemisahan nyata.

## Aliran gameplay

```text
Touch/editor input
  -> action yang divalidasi terhadap state pertandingan
  -> gerak / charge / release / jump / stroke
  -> simulasi fixed timestep + contact evaluator
  -> hasil flight / landing / fault
  -> perubahan skor dan state
  -> HUD, SFX, replay evidence, snapshot saat diperlukan
```

Validasi state input wajib; tidak menerima stroke/jump di state ilegal. Physics tidak boleh bergantung frame rate render. Continuous collision untuk shuttle cepat perlu diuji, bukan diasumsikan menyelesaikan seluruh tunneling. Grade kualitas berasal dari timing, power, aim dan posisi; visual tidak memanipulasi physics.

## State pertandingan

Roadmap M2-01 menetapkan warmup, ready, serving, rally, point, interval, change ends, game-end, match-end. Detail transisi diimplementasikan dengan tes agar:

- Satu reli memberikan paling banyak satu hasil poin yang sah.
- Servis berikutnya mengikuti skor/server dan area yang benar.
- Deuce/cap tidak dapat membuat pertandingan berhenti terlalu dini atau tidak selesai.
- Change-ends mengubah sisi dunia tanpa membalik POV pengguna secara membingungkan.
- Pause menghentikan progres; resume memulihkan state konsisten.
- Load tidak memberi reward ulang atau mengulang event akhir pertandingan.

Angka change-ends Quick16/Modern15 masih usulan GDD §3; jangan dianggap otomatis disetujui bersama preset skor. Verifikasi aturan sebelum mengunci implementasi.

## Kontrol dan kamera

A/B tetap HARD GATE. Input dapat simultan sejauh skema yang diuji memungkinkan; jangan mengganti pilihan dengan autoplay. Assisted hanya footwork, bukan jaminan pukulan. Tidak auto-teleport ketika contact; IK/input window hanya sesuai approval. Kamera player-centric mempertahankan keterbacaan pemain, shuttle dan lapangan; metode teknis P-006 tetap provisional.

## Save dan integritas data

- Data local-only, tiap slot memakai ID stabil, bukan nama tampilan atau indeks enum.
- Format/version dan seed dicatat; schema final belum dipilih.
- Write ke temporary sibling, validasi, lalu replace atomic yang sesuai platform; simpan versi terakhir yang valid. Uji proses terputus sebelum, saat, dan setelah penulisan.
- Load memvalidasi ukuran/range/schema/version dan relasi ID. Save rusak tidak boleh menimpa backup valid; error ditampilkan, tidak dihapus diam-diam.
- Snapshot mid-match menyimpan state yang diperlukan untuk skor, servis, physics, RNG dan sistem terkait. Buktikan kontinuitas lewat tes, bukan klaim byte-identical lintas device yang belum diuji.
- Migrasi tidak menimpa satu-satunya salinan. Retired career read-only; delete dan pensiun memakai konfirmasi jelas.
- Reward/hasil memiliki identitas transaksi supaya retry/load tidak membayar dua kali.

## Career simulation

Data/simulasi terpisah dari scene 3D; pertandingan offscreen tidak merender pertandingan penuh. Seed dicatat untuk reproduksi. Kalender, entry/draw, expiry poin, aging dan population replenishment mengikuti aturan yang disetujui sebelum fixtures final dibuat. Target 1.000 atlet masih provisional dan harus diprofilkan; uji karier >10 tahun tidak boleh ditukar dengan demo beberapa tick.

## Replay dan challenge

Replay memakai event/transform ring buffer dan trajectory asli, bukan menjalankan ulang physics lalu mengklaim identik. Challenge IN/OUT memakai landing ground-truth, dua gagal/game, sukses tidak mengurangi kuota. Call awal deterministik untuk MVP; probabilitas kesalahan call membutuhkan persetujuan tersendiri. Jangan mengarang hasil challenge.

## Performance dan keamanan

Profil frame time, RAM, colliders, fixed timestep, thermal dan keseluruhan ukuran game. Hindari asset duplikat, shader/overdraw berlebihan dan log release tak terbatas. Batas ukuran mencakup game keseluruhan; metode ukur ada di TESTING. Semua permission, dependency, endpoint dan data keluar diaudit. Credential/signing key tidak ditulis dalam docs/log/source.

## Struktur rencana, bukan file yang sudah ada

```text
JustSmash/
  docs/                 spesifikasi dan ledger
  .backups/             snapshot dokumentasi sebelum revisi
  [Unity project]       lokasi ditetapkan saat audit M0
  [test/build evidence] dibuat saat pengujian sungguhan
```

Jangan init project, membuat Assets/Packages/ProjectSettings palsu, atau mengaku toolchain tersedia dari diagram ini. Audit implementasi menjadi langkah berikut setelah izin coding.
