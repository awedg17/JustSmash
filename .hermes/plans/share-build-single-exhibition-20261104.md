# Rencana Share Build Single Exhibition (Single Putra Only)

**Target:** Versi play-to-share ("main badminton singles exhibition ke temen") dengan gameplay playable yang solid dan visual/audio yang jelas. Tidak perlu career mode, tutorial lengkap, atau polish akhir.

---

## Bukti & kondisi saat ini

- **Stamina**: `rallyDrain=2.3` di `BootDiagnostics.cs:12` tidak terpakai. Rally natural 30s = 12 hit × shotCost(1) → energi sisa 88% (masih banyak). Target D-043: "rally 30 detik menurunkan stamina cukup banyak, mulai memengaruhi performa, tapi belum habis" → energi ~70% pada 30s (fatigue multiplier aktif <75%).

- **Grade**: Formula Timing+Contact 50:50 sudah implement (`CourtPrototype.cs:526`) threshold 85/65/40 hijau (`D045-grade-final.xml` 96/96). Belum dicatat rumus/jendela timing/formula contact di Decision Register.

- **NPC/Aset Casual Set00**: Visual import 98/98, tapi celana panjang biasa (bukan sportswear). Inspeksi: facing/raket ambigu, stance canggung. Gate gagal (ASSET_REGISTER status `IMPORTED_HUMANOID_VALID_SPORTSWEAR_GATE_FAILED`). Tidak beli/ganti kandidat lain.

- **Trail**: Time 0.38, startWidth 0.12, warna kuning lembut (1,0.95,0.6,alpha 0.85) sudah ada (line 393–394 CourtPrototype.cs). OK.

- **Audio**: Rekaman CC0 racket (0537) sudah wired ke `CourtAudio.PlaySound("racket")` (Assets/Resources/Audio/racket-*.wav). SFX prosedural untuk net/step/score masih ada. Mix belum dicek audibility.

- **Tester UI**: DrawTester() dipanggil selalu di OnGUI (line 861), tapi panel hanya muncul jika `testerOpen=true`. Trigger F2 hanya aktif di Editor/debugBuild. Dalam release APK, tester akan selalu tertutup (testerOpen=false awal, tidak ada on-screen button). Aman, tapi bisa diperjelas dengan `if (Application.isEditor)` guard.

- **APK terakhir**: `JustSmash-D044.apk` timestamp Nov 3 21:43; source grade/fatigue changed Nov 4 07:46+. Perlu rebuild.

- **Device testing**: belum ada APK baru, belum install HP. adb found di `C:/Users/Dewangga Ramadhan H/AppData/Local/Android/Sdk/platform-tools/adb.exe`.

---

## Scope pekerjaan (urutan)

### 1. Stamina rally drain (aktifkan)

**Tujuan**: Rally hidup (flying==true && MatchState=="Rally") kurangi stamina per detik → 30s → ~70 remaining. Fatigue movement speed multiplier (0.7..1) otomatis turun di bawah 75%.

**Perubahan**:
- Di `CourtPrototype.FixedUpdate()` sekitar line 680, tambahkan:
```csharp
if (flying && Match != null && MatchState == "Rally")
    Energy = Mathf.Max(0, Energy - rallyDrain * Time.fixedDeltaTime);
```
- Set `rallyDrain = 0.6f` di `BootDiagnostics.rallyDrain` (dulu unused 2.3).

**TDD**:
- Test `NaturalLobRallyMeasuresThirtySecondEnergyBaseline`: update assertion energi band 60–80, dan <75 agar fatigue engaged.
- Test `ThirtySecondFixedDrainRecordsPartialRecovery`: update JSON scope string (rally-time drain active now; idle still 100).
- Test assert `WalkingDoesNotDrainEnergyOnlyJumpAndHitsDo()` tetap pass.

**Acceptance**: Natural lob 30s → energi 60–80 (target ~70), fatigue speed <75% terukur.

---

### 2. Tester UI production-hide (guard Editor-only)

**Tujuan**: Pastikan DrawTester() tidak dieksekusi sama sekali di release APK.

**Perubahan**: Line 861 CourtPrototype.cs: `DrawTester()` → `if (Application.isEditor) DrawTester();`. Atau lebih konservatif: `if (Application.isEditor || Debug.isDebugBuild) DrawTester();` sesuai trigger F2 rule.

**Acceptance**: Release APK tidak menampilkan panel lokal tester.

---

### 3. NPC facing/bone calibration (visual inspection → fix)

**Tujuan**: Athlete model sekarang menggunakan NPC Casual Set00. Perbaiki facing (arah badan), lengan/raket hand grip (forehand/backhand transition), stance leg geometry. Tidak ganti asset (gate gagal), cukup tuning script binding.

**Batasan**: Aset tidak memiliki animasi, rigged FBX tanpa rigging lengkap. AthleteVisual.Map() harus bind bone hierarchy realistis. Face direction relative to shuttle position.

**TDD**: Add `AthleteVisualTests.FacingAndGripAlignWithShuttlePosition()` — verify racket hand side changes with local X sign of shuttle vector relative to athlete forward.

**Acceptance**: Visual capture Net/X.png / Smash/X.png menunjukkan orientasi badan dan raket konsisten arah kok. Stance kaki proporsional.

---

### 4. Audio mix check (playback test + manual listen)

**Tujuan**: Verifikasi racket audio CC0 terdengar jelas dalam konteks court ambience (minimal crowd SFX). Mix gain tidak terlalu keras/loudness.

**Perubahan**: `CourtAudio.PlaySound("crowd")` existing procedural crowd. Adjust gains: racket .5–.6, net .3, score .4. Existing settings okay.

**TDD**: Test `OriginalAudioEventsProduceBoundedSamples()` sudah ada. Manual listen: run Unity batch mode playback, record log volume peaks.

**Acceptance**: Racket sounds audible, not masked. Document findings.

---

### 5. Grade formula dokumentasi

**Tujuan**: Catat formula final di `05_DECISION_REGISTER.md`.

**Format**:
- Timing window: ±X seconds dari ideal contact point (defined by shuttle Z relative to player Z minus reach distance).
- Contact quality: normalized height/distance factor (0..1) from closest approach between player hands and shuttle flight path.
- GradeScore = 0.5*timing + 0.5*contact (each 0..1 → scaled 0..100).
- Thresholds: Excellent >=85, Good >=65, Fair >=40, Poor <40.
- DiveSave cap: grade clamped <=84 (Good max).

**Acceptance**: Decision register updated; tests confirm thresholds.

---

### 6. Full regression + build Android

**Test suites**: PlayMode (full suite ~100 tests), EditMode (~10). Pass required >95%.

**Build target**: Android API level compatible with target devices (Unity default). ARM64, landscape, INTERNET=false.

**Verification**: SHA256 hash recorded; manifest JSON created.

**Acceptance**: APK size ~31MB, all tests pass, manifest matches runtime assets.

---

### 7. Device install/launch (no data deletion)

**Action**: Install APK to connected Android device via adb. Do NOT uninstall/clear data. Launch app. Verify:
- Touch response (joystick left, shots right, jump top-right)
- FPS stable (>30 avg, no drop spikes under 25)
- Safety area notch respected
- Sound output present (racket plays on hit)
- Exit/relaunch works without crash

**Acceptance**: All above met; device installation log saved.

---

### 8. Laporan review

**Content**: Summary changes vs last share build, test results (PlayMode/EditMode counts), device performance numbers, known issues/remaining gates, screenshots/video captures, SHA256 APK link to download.

**Files updated**: TESTING.md, WORKFLOW.md, SESSION_HANDOFF.md, ERROR_LOG.md.

---

## Risiko & catatan

- **Fatigue threshold**: 75% cutoff established previously (D-045). If energy goes <75%, speed multiplier drops 1→0.7. Ensure this does not force-miss shots (player still can hit at low energy). Code review confirms no energy-gate on ReleaseShot().

- **NPC animation limitation**: Without animations, motion procedural only (Idle/Lunge/Jump states exist). Visual acceptance relies on static renders (Net-*.png, Smash-*.png) not gameplay video. This is acknowledged limitation.

- **Device dependency**: ADB available, but HP must be connected and USB debugging enabled. If unavailable, report emulator limitation.

- **No tutorial required**: Version "just play" expects player familiarity with mobile controls (joystick left, tap/shoot right). Instructions in-game UI text sufficient.

---

## Execution order summary

1. Implement rally drain + TDD (stamina test suite)
2. Hide tester UI from release
3. Calibrate AthleteVisual facing/grip (visual inspection, fix bindings, new test)
4. Audio verification (playback log + note)
5. Documentation: decision register entry for grade formula
6. Full regression PlayMode/EditMode
7. Build Android APK
8. Device install/launch QA
9. Final report docs

Mulai eksekusi.
