# JUST SMASH — Implementable Roadmap and Task Backlog
Versi 0.2 | Owner approval needed for any `PROPOSED` mechanic. Scope first = infrastructure for actual playable prototype, **not** whole career up front.

## Status labels
`TODO` belum dikerjakan • `IN_PROGRESS` • `BLOCKED_APPROVAL` • `READY_FOR_QA` • `DONE_VERIFIED` • `BLOCKED_DEVICE`. Checkbox tidak dicentang tanpa bukti di `TESTING.md`. Persetujuan dokumen bukan mulai implementasi; seluruh task implementasi masih TODO.

## Status eksekusi terbaru — 2026-11-01
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

- [ ] `M1-01` Lapangan 3D dengan ukuran badminton sebenarnya, garis singles/doubles, net berskala, 2 placeholder karakter original.
- [ ] `M1-02` Kamera belakang near-end, target kedua atlet & court readable, preset standard/zoom-out-jump; kamera tidak tembus pemain saat jump.
- [ ] `M1-03` Shuttle 3D terpisah dari VFX tail: translasi, orientasi, drag non-linear, gravity, net collision, ground collision, in/out detection.
- [ ] `M1-04` Fixed timestep physics + continuous collision untuk shuttle cepat; replayable trajectory yang dipengaruhi `hitVector` dan physics params.
- [ ] `M1-05` Debug visualizer (flight path, contact window, landing point, target zone, racket contact, hit result) toggle di editor.
- [ ] `M1-06` Animasi dasar idle, front/back/side footwork, braking, lunge, overhead swing, underhand swing, recovery; placeholder dapat dipakai selama aman lisensi.
- [ ] `M1-07` Sinkronkan movement root/collider dengan animasi, foot plant & in-place step, contact model; hindari teleport/no clip.
- [ ] `M1-08` Prototype input A: joystick manual footwork + right-thumb shot + directional selector, power charge, timing release.
- [ ] `M1-09` Prototype input B: swipe/drag target movement + shot gesture terpisah, input simultaneous vs sequential; uji latency/aim.
- [ ] `M1-10` Prototype assisted movement: otomatis mendekati shuttle tetapi keputusan shot tetap pemain.
- [ ] `M1-11` Serve low & high atau short & flick prototype dengan button tertentu; servenya tidak harus animasi lengkap saat pertama.
- [ ] `M1-12` Implementasi strokes smash, clear/lob, drop, net, lift (minimum prototype: clear, smash, net; sisanya tambahkan sebelum M2 selesai).
- [ ] `M1-13` Shot evaluation komponen timing/direction/power/position dan grade Excellent/Good/Fair/Poor serta sebab miss/net/out; seed deterministic.
- [ ] `M1-14` Jump timing & swipe-up gating dalam area input khusus, fallback jump button; tanpa otomatis memenangkan smash.
- [ ] `M1-15` Shuttle visibility trail/shadow subtle, warna/brightness bisa dikonfigurasi; readability di puncak flight.
- [ ] `M1-16` Bandingkan video rekaman A/B kontrol dalam 3 skenario (receiving high lob, smash approach, near-net), kirim kepada owner; **status `BLOCKED_APPROVAL` untuk memilih skema final**.
- [ ] `M1-17` Add quick tester form: miss disebabkan apa, input salah terbaca/tidak, kenyamanan tiap ibu jari, seberapa sering player merasa cheated.

**Definition of Done M1:** 2 controls playable pada scene sama, shuttle physics tidak arbitrer, basic stroke dapat diuji, area landing terdeteksi, setiap kegagalan punya log yang dapat dijelaskan, owner memilih/mengizinkan iterasi kontrol.

## MILESTONE 2 — Satu pertandingan singles tuntas
- [ ] `M2-01` Match finite-state-machine: warmup -> ready -> serving -> rally -> point -> interval -> change ends -> game-end -> match-end.
- [ ] `M2-02` Scoring engine parametris Quick16, Classic21, Modern15 (P-002 APPROVED); automated tests 0-0, 14-14/15-15/20-20/29-29, change-ends, best-of-3.
- [ ] `M2-03` Rencana servis ganjil/genap, pembagian area servis, fault normal, let.
- [ ] `M2-04` Implementasi turn direction, score HUD, game summary, final results.
- [ ] `M2-05` Stroke evaluator final 5 teknik dan straight/cross, charge/release, shot type availability by situation.
- [ ] `M2-06` Energy/fatigue loop bertingkat (point -> game -> match), recovering antargame serta skill efficiency.
- [ ] `M2-07` Easy AI baseline yang mampu servis, mengembalikan pukulan, berpindah dengan animasi wajar, melakukan kesalahan.
- [ ] `M2-08` Camera transition change ends tanpa mengubah player-centric view dan collision semantics.
- [ ] `M2-09` Pause resume/restart, save/load mid-match deterministic snapshot, mismatch version guard.
- [ ] `M2-10` Instant review line judge + challenge IN/OUT dua failed/game, successful tidak mengurangi kuota (P-008 APPROVED), visual trajectory replay. Simulasi call salah tetap PROPOSED dan perlu persetujuan sebelum implementasi. Mode disabled bisa dipakai exhibition.
- [ ] `M2-11` Spectacle event detector + toggle replay enable/disable/skip, 1 additional umpire-side angle.
- [ ] `M2-12` Sound events procedural (racket impact/net/footsteps/whistle/scores/crowd) menggunakan audio original/permitted only.
- [ ] `M2-13` Exhibition match UI (choose score format/difficulty) tanpa career.
- [ ] `M2-14` Balance QA untuk shot out/net/miss; no chronic false-perfect shots, no cheap auto hit.
- [ ] `M2-15` Android perf & memory audit round1, 30FPS target, optimize overdraw, shaders, anim update.

**Definition of Done M2:** full singles match vs AI, score valid, no stuck rally, save mid-game resumes, reusable match result JSON, kontrol final disetujui user.

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
- [ ] `M6-02` Doubles positioning, footwork priority, hit ownership, partner AI cooperation.
- [ ] `M6-03` Service positions, receive order, scoring side/rotation and fault tests.
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
