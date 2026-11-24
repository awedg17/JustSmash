## Scope aktif D-062 — overhaul badminton stylized

Eksekusi35task plan `.hermes/plans/2026-11-22_gameplay-visual-character-overhaul.md` disetujui. Prioritas karakter natural, footwork/kontak dan lima pukulan sebelum dekorasi; arena indoor tanpa penonton. Evaluasi aset gratis/legal, integrasi hanya setelah mesh/rig/motion lolos. D-050 stamina, D-052 PC-only/no-native-input, Official15, immediate contact/finite reach tetap; career/customization/multiplayer bukan scope. Result memuat pemenang, total game dan skor tiap game; save lama menampilkan data tidak tersedia bila tidak bisa dibuktikan. Bukti per-task di TESTING; approval eksekusi bukan acceptance keseluruhan.

## Riwayat scope D-055 — polish exhibition utuh

Pemilik mengizinkan eksekusi plan Roller Champions UI/match polish: UI orisinal sesuai arah referensi, grafik konsisten, animasi/gameplay responsif dan meyakinkan dari Home sampai Result. Bukan polish menu saja. Career tetap dicabut; tunggal/ganda putra, aturan/grade/stamina/finite reach existing dipertahankan. Preview arah dan pilot motion adalah gate nyata; APK/regresi bukan pengganti acceptance pengalaman. D-057 memilih blue-storyboard, court hijau dan satu font Anton Rp0/SIL OFL 1.1; selain font tersebut aset/dependency existing, PC-only/no-native-input/no-push tetap.

## Riwayat scope D-054 — 2026-11-15

Game badminton Android offline landscape, exhibition tunggal + ganda putra. **Career dibatalkan dan dicabut**, bukan fitur aktif atau antrean terakhir. Prioritas sekarang referensi UI nyata sebelum redesign; lihat UI_REFERENSI.md. D-052 PC-only, Rp0, no-push/dependency/input native tetap. Tidak menganggap rollback atau tes teknis sebagai penerimaan feel/visual/audio.

## Riwayat scope D-053 — bagian career digantikan D-054

Eksekusi pengalaman Android offline layak dibagikan: identitas UI/alur buka–main–hasil, GOR dengan ofisial/penonton/reaksi, tunggal dan ganda putra. Career masuk scope dan dikerjakan terakhir mengikuti GDD; bukan demo counter. Instruksi pemilik mendelegasikan solusi teknis, bukan penghapusan requirement. D-052 PC-only dan larangan biaya/push/dependency/input native ulang tetap. Ketentuan M0–M2/no-career/no-doubles di bawah merupakan riwayat bila bertentangan. Acceptance membutuhkan bukti runtime dan kualitas pengalaman, bukan tes/build saja.

## Riwayat kebijakan — D-037..D-042, 2026-11-03
Instruksi owner terbaru menggantikan aturan lama yang bertentangan; bagian checkpoint/run lama adalah riwayat, bukan izin aktif. D-044 mengizinkan eksekusi M0–M2 sampai satu APK checkpoint baru; batas docs-only sebelumnya berakhir. Otorisasi implementasi tetap M0–M2; M3+ tidak dimulai otomatis, termasuk setelah acceptance M2. Tidak ada import aset/dependency, biaya, push, destructive reset, atau install/launch APK HP tanpa izin terpisah.

Review owner hanya pengalaman modul utuh: singles lengkap; character creation lengkap; career terintegrasi; final game. Ini kelompok review, bukan penambahan scope. Tidak meminta owner menguji pukulan/animasi/tombol/fault secara granular. Agent menangani unit/integration/regression, natural automated match, inspeksi video/motion/audio, dan performance sesuai izin. Keputusan desain tetap memerlukan approval, berbeda dari testing.

Subagent dan commit lokal bersyarat tetap D-038/D-039. D-043 menyetujui NPC Casual Set00 untuk evaluasi import, default joystick Fixed (Floating tetap pilihan), servis charge1detik/power0..1/feedback konsisten/cancel aman. Grade hanya Timing + Contact; Direction/Power trace tanpa kontribusi grade. Rally sekitar30detik menurunkan stamina cukup banyak dan mulai memengaruhi performa, tetapi belum habis dan rally bisa berlanjut; jalan/lari drain0. Bobot/formula grade, biaya/kurva/efek stamina baru belum approved. Unity Personal memenuhi kondisi pribadi non-klien tanpa pendapatan yang dinyatakan owner; tidak meminta pembahasan keuangan lagi kecuali kondisi/terms relevan berubah. Target karakter/animasi/atmosfer singles tetap. Approval evaluasi NPC bukan izin import aset lain/dependency/M3 atau device action. D-044 menggantikan batas docs-only; eksekusi approved sampai APK checkpoint, tanpa mengubah proposal angka menjadi approval.

Rujukan keputusan/proposal: [Decision Register](05_DECISION_REGISTER.md). Riset aset/lisensi: [Legal dan Assets](04_LEGAL_AND_ASSETS.md). Gate bukti: [Testing](TESTING.md).

# Just Smash — Product Requirements

## FEEDBACK-013 — current repair checkpoint
D-036 permits post-hit landing indicator, actual-displacement movement energy, automatic next-point preparation and licensed recorded racket audio. Preserve capacity/recovery and manual player serve.79/79 local PlayMode passed; UI/trail/natural animation, moving-rally baseline and owner/device acceptance remain open. Not full M0–M2 acceptance.



## D-035 — current acceptance delta
- Immediate technique press performs one shot; pointer release cannot duplicate it. Jump hold-swipe remains functional.
- Stat70 capacity100/recovery8; stat80 capacity110/recovery8.8; save/restore preserves stat/energy and prevents repeated recovery.
- Body/net-touch awards opponent exactly once during rally and is not line-review challengeable.
- D-043: grade Timing + Contact saja; Direction/Power trace, tanpa kontribusi grade; tidak memaksa outcomes.
- M0–M2 saja; M3 tidak boleh dimulai tanpa izin baru (D-042). Incremental tests are not full acceptance.



## CLOSURE-011 — otoritas dan penutupan aktif
Instruksi pemilik terbaru mengizinkan audit dan penutupan teknis lokal seluruh M0–M2, tanpa approval ulang perubahan teknis dalam scope. D-031 menunda review manual; D-034 menyetujui world-space change-ends/player-centric camera serta 3% salah call pada landing eligible dalam 5 cm, maksimum 1/match. D-016 menetapkan joystick/manual+assisted, bukan eksperimen swipe A/B. D-017 menetapkan Official15 saja; Classic21 terkunci sampai career2028, Quick16 dihapus.

Requirement dan acceptance di bawah tetap berlaku kecuali digantikan keputusan APPROVED lebih baru. Hanya status checkpoint lama yang merupakan **riwayat**, bukan status aktif. Pernyataan lama M2 dibekukan, P-006 belum disetujui, Quick16 default, belum ada build, atau lisensi aktivasi terblokir tidak berlaku lagi. Status per task dan closure plan aktif: `evidence/M2/closure-task-audit.json`; hasil baru dicatat di TESTING. Izin aktif D-037..D-042: commit lokal boleh dengan audit; HP install/launch memerlukan izin terpisah; push, dependency baru, biaya dan M3 tetap dilarang.

Pisahkan Implementation Complete, Local QA Passed, Device QA Pending, Owner Acceptance Pending, dan Milestone Fully Verified. Task canceled M1-09 dianggap requirement superseded terpenuhi, bukan fitur swipe yang diimplementasikan. Device QA pending bukan otomatis BLOCKED_ENV tanpa kegagalan lingkungan konkret.


## M1 D-032 local additions
- M1-02/03/04/05/06/15/17 mapped to M1GapTests and evidence/M1/gaps*. Controlled flight stability is not shot accuracy, Android FPS or owner feel acceptance. Structured tester offline JSONL, no analytics service.

## Riwayat scope — 2026-11-02
Pemilik mengizinkan implementasi seluruh scope sampai M1, tidak M2+. D-016–D-028 dan instruksi chat terbaru menggantikan proposal lama: joystick kiri untuk gerak sekaligus arah pukulan, enam sektor tanpa landing/trajectory preview; jump tombol hold-swipe dengan guide teknik; assisted menggerakkan atlet sepenuhnya tanpa input movement. Default Official15, Classic21 Settings terbuka saat career 2028; Quick16 dihapus. Scoring penuh, challenge, stamina match, career, sponsor dan doubles tetap milestone sesudah M1. A/B swipe movement dibatalkan; gate M1 menjadi joystick/manual+assisted pada high lob, smash approach, near-net. Uji AVD Android resmi untuk runtime; HP hanya untuk feel multitouch dan performa perangkat nyata. Status historis di bawah bukan status terkini.


Versi 0.2 | 2026-11-01 | Spesifikasi, bukan bukti implementasi.

## Otoritas dan batas dokumen

Detail gameplay tetap di [GDD](01_GDD.md); keputusan dan status di [Decision Register](05_DECISION_REGISTER.md). Dokumen ini memetakan kebutuhan ke milestone dan tes tanpa mengganti atau menghapus isi GDD. APPROVED berlaku hanya pada keputusan yang disebut. Fitur jangka panjang bukan izin membangun semuanya sekarang.

## Produk dan pengguna

Game Android 3D badminton offline, landscape, single-player. Pengguna mengendalikan atlet secara langsung; kemampuan bermain menjadi penentu, bukan pay-to-win atau manipulasi atribut AI. Exhibition mendahului career; career satu atlet per save dengan banyak slot independen, kehidupan atlet, ranking dan pensiun sukarela.

## Batas wajib

- Engine Unity disetujui; versi/pipeline/package ditentukan saat audit M0. Personal hanya bila eligibility dan terms mendukung biaya produksi Rp0.
- Perangkat minimum RAM 4 GB; target 30 FPS stabil. Ini bukan izin memakai 4 GB RAM untuk aplikasi.
- **Keseluruhan game maksimal 1 GB**, bukan hanya APK. Ukur paket distribusi, footprint terpasang beserta seluruh aset, serta data/cache pada skenario uji. Protokol dan batas klaim ada di [TESTING](TESTING.md).
- Tidak ada akun pemain, runtime network wajib, analytics tanpa izin, premium currency, IAP, pay-to-win, atau biaya produksi.
- Local development dan sideload. Tidak deploy/cloud/publikasi toko tanpa persetujuan terpisah; tidak `git push` oleh agent.
- Semua identitas, artwork, audio dan merek harus original atau berlisensi sah. Referensi bukan lisensi.

## Kebutuhan dan traceability

| ID | Kebutuhan / acceptance inti | Sumber | Milestone | Test |
|---|---|---|---|---|
| R-01 | Build Android landscape dapat dibuka, ditutup, dan dijalankan ulang secara offline | GDD §2, D-001 | M0 | T-01 |
| R-02 | Joystick movement/aim D-016 dan assisted; Fixed/Floating di Settings D-040, default menunggu approval | GDD §4, D-004, P-001/P-014 | M1 | T-02 |
| R-03 | Lima stroke tap-immediate D-035, straight/cross, jump hold-swipe; servis HOLD-release D-041; grade dan sebab kegagalan terbaca | GDD §4, D-003/D-005 | M1–M2 | T-03 |
| R-04 | Shuttle drag/contact/net/floor, animasi, kamera belakang dan change-ends konsisten; tidak teleport | GDD §5, D-002, P-006 | M1–M2 | T-04 |
| R-05 | Official15 target15, deuce14, cap21; best-of-3, servis/fault/change-ends benar; Classic21 terkunci sampai career2028, Quick16 dihapus | GDD §3, D-007, P-002 | M2 | T-05 |
| R-06 | Exhibition lengkap, pause/resume/save mid-match, skip/sim, challenge IN/OUT dua gagal/game dan replay toggle | GDD §3/5, D-008, P-008 | M2, integrasi skip M4–M5 | T-06 |
| R-07 | Stamina menurut exertion; AI empat difficulty tanpa stat cheat atau membaca input masa depan | GDD §5, D-006/D-012 | M2–M3 | T-07 |
| R-08 | World sim, kalender, entry, ranking, aging dan regenerasi konsisten; detail provisional tidak dikunci tanpa izin | GDD §6, P-004/P-005/P-012 | M4 | T-08 |
| R-09 | Career multi-save, progression, training, coach, stats, pensiun umur >=20 dan archive read-only | GDD §6–8, D-009/D-010/D-011 | M4–M5 | T-09 |
| R-10 | Men's doubles benar-benar playable dengan AI partner, serving rotation dan ranking pasangan | GDD §10, D-008, P-013 | M6 | T-10 |
| R-11 | Economy earned-only, opsi gratis untuk aktivitas penting, sponsor/offcourt dan kosmetik tanpa stat boost | GDD §8–9, D-010/D-011/D-014 | M7 | T-11 |
| R-12 | Visual stylized ringan, UI turquoise/cyan, thumb reach, SFX/crowd; tanpa musik/komentator | GDD §2/9, D-013 | M1–M8 | T-12 |
| R-13 | 30 FPS target pada perangkat memenuhi minimum; total game <=1 GB; metrik nyata dan tidak dipalsukan | GDD §2/11, D-001 | M0 baseline, M2, M8 | T-13 |
| R-14 | Lisensi/provenance, audit privacy/offline, save migration/recovery dan release approval terpenuhi | GDD §11–12, Legal §D–H | M0–M8 | T-14 |

## Scope saat ini

Sesi ini dokumentasi/proposal saja. Implementasi dan bukti M0–M2 sudah ada, belum fully verified. Singles review wajib karakter/animasi/atmosfer lengkap; placeholder hanya alat internal. M3+ tidak diizinkan.

## Keputusan disetujui dan yang masih terbuka

- Unity disetujui sebagai engine (P-011); bukan persetujuan biaya/cloud atau versi editor tertentu.
- D-017 menggantikan P-002/P-003: Official15 default, Classic21 terkunci sampai career2028; Quick16 dihapus.
- Challenge IN/OUT dua gagal/game, berhasil tidak mengurangi jatah, hasil berdasarkan ground-truth disetujui (P-008). Simulasi kesalahan call dan probabilitasnya masih usulan.
- A/B kontrol, detail starting ages, ranking/progression, extra attributes, dan parameter balancing tetap mengikuti status register. Jangan menganggap persetujuan dokumen sebagai persetujuan semua PROPOSED.

## Di luar scope

Multiplayer, backend, akun pemain, monetisasi premium, coaching career setelah pensiun, trickshot, cloning aset/IP, paid publishing dan fitur di luar milestone yang disetujui.

## Kriteria selesai

Setiap R-ID memiliki hasil tes dan bukti yang terkait build nyata; seluruh gate terkait lolos; tidak ada bug kritis terbuka atau keputusan material yang dilewati. Approval gameplay subjektif dari pemilik tetap diperlukan. Checklist, dokumen lengkap, atau build sukses saja tidak membuktikan produk selesai.
