## Status aktif — D-054: tanpa career; referensi UI dahulu

Career dicabut dari source dan Home, bukan sekadar ditunda. Tunggal/ganda dipertahankan pada baseline pre-career yang hash-identik; backup `.backups/remove-career_20261115_010535/`. Tes penghapusan1/1, Edit19/19 dan Play subset95/95PASS; source1.124file identik, dua PNG Home diperiksa; save/prefs preserved. Detail di [TESTING](TESTING.md). Tidak ada APK baru. Pilihan referensi nyata di [UI_REFERENSI](UI_REFERENSI.md), belum penerapan/approval redesign. Acceptance gameplay/gerak/audio dan Easy duration tetap terbuka.

## Riwayat D-053 — career kini dibatalkan D-054

Target kini game layak dibagikan: identitas UI/alur lengkap dan atmosfer, tunggal + ganda putra, career terakhir. [Keputusan D-053](05_DECISION_REGISTER.md), [DESIGN](DESIGN.md), [TESTING](TESTING.md). Android tetap target; QA PC saja D-052. Bukti source4/APK di bawah baseline historis, bukan fitur baru.

Capture `current-presentation1`3/3PASS:10layar + controlled + natural Normal2,522,6798s empat atlet aktif,fault0. Mux/decode/Edge muted playback PASS; natural-motion/listening belum. Regresi2 Play144/145:clock-harness fixed, Easy1069,29s gagal batas1000; tuning ditahan. Fix Home preview sesudahnya RED0/1→95/95PASS dan4PNG,source1.124file identik; natural2 pre-reset Home. Full regression/APK D-053 belum. Career fondasi save/recovery/pensiun/arsip + world1.000 atlet/kalender seeded4020hari teruji Edit23/23, source1.130file identik; entry/draw/result24/24PASS, UI career/match/save/reload9/9PASS, partner data25/25PASS source1.134identik. Partner UI89/89, progression UI90/90, reward Edit28/28; finance Edit29/29 + UI91/91PASS source1.138identik dan still reviewed. Migrasi pre-finance30/30PASS; life/schema5 dan bonus hasil/cadence/expiry/clause teruji. Career statistics Edit38/38 dan UI93/93PASS source1.142identik, still arsip reviewed: filtertahun dan paging seluruh riwayat/read-only setelah pensiun. Detailarsip/capaian Edit40/40 dan UI94/94PASS source1.142identik per-run, still reviewed; Aging termasuk latihan hari90 Edit43/43 dan UI95/95PASS pada freeze1.142sama/still reviewed; Footwork empat atlet serta Stamina per-atlet/recovery/snapshot teruji career-athlete-stamina-regression Edit43/43+UI97/97PASS satu freeze1.142identik. Power/Technique/presisi kini lokal verified: career-technique-regression Edit44/44+UI100/100PASS satu freeze1.142identik, bukan full Play. Qualifying/Balance belum. Lihat [handoff](SESSION_HANDOFF.md); bukan klaim siap share.

## Baseline historis — mobile singles

Scope kini: Android offline landscape, singles vs tiga tingkat AI, kamera belakang dan NPC Casual Set00 existing. Keputusan/desain aktif: [DESIGN](DESIGN.md) dan D-051 di [Decision Register](05_DECISION_REGISTER.md). Hasil terbaru: [TESTING](TESTING.md). `docs/WORKFLOW.md` dihapus sesuai permintaan; tidak ada worklog pengganti. Riwayat di bawah bukan status build terbaru. Biaya, push, perluasan fitur, akses pribadi dan install/launch HP tanpa izin spesifik tetap dilarang.

APK Mobile terbaru: `builds/JustSmash-Mobile.apk`, 29.769.602bytes, SHA-256 `f24c3dc60e1c277fd23c38b4628cc835a244c53a1b14c888d36fde2a6800aff0`. Source4: EditMode18/18, PlayMode125/125, capture2/2 PASS; packaging/signaturev2 PASS, signer Android Debug. Android/performa **DEFERRED_OWNER_PC_ONLY** sesuai D-052: pemilik memilih "Tetap PC saja; jangan akses HP". Native input, natural-motion dan listening acceptance belum lulus; **bukan klaim modul selesai**. Status terinci di TESTING dan SESSION_HANDOFF.

## Riwayat kebijakan — D-037..D-042, 2026-11-03
Instruksi owner terbaru menggantikan aturan lama yang bertentangan; bagian checkpoint/run lama adalah riwayat, bukan izin aktif. D-044 mengizinkan eksekusi M0–M2 sampai satu APK checkpoint baru; batas docs-only sebelumnya berakhir. Otorisasi implementasi tetap M0–M2; M3+ tidak dimulai otomatis, termasuk setelah acceptance M2. Tidak ada import aset/dependency, biaya, push, destructive reset, atau install/launch APK HP tanpa izin terpisah.

Review owner hanya pengalaman modul utuh: singles lengkap; character creation lengkap; career terintegrasi; final game. Ini kelompok review, bukan penambahan scope. Tidak meminta owner menguji pukulan/animasi/tombol/fault secara granular. Agent menangani unit/integration/regression, natural automated match, inspeksi video/motion/audio, dan performance sesuai izin. Keputusan desain tetap memerlukan approval, berbeda dari testing.

Subagent dan commit lokal bersyarat tetap D-038/D-039. D-043 menyetujui NPC Casual Set00 untuk evaluasi import, default joystick Fixed (Floating tetap pilihan), servis charge1detik/power0..1/feedback konsisten/cancel aman. Grade hanya Timing + Contact; Direction/Power trace tanpa kontribusi grade. Rally sekitar30detik menurunkan stamina cukup banyak dan mulai memengaruhi performa, tetapi belum habis dan rally bisa berlanjut; jalan/lari drain0. Bobot/formula grade, biaya/kurva/efek stamina baru belum approved. Unity Personal memenuhi kondisi pribadi non-klien tanpa pendapatan yang dinyatakan owner; tidak meminta pembahasan keuangan lagi kecuali kondisi/terms relevan berubah. Target karakter/animasi/atmosfer singles tetap. Approval evaluasi NPC bukan izin import aset lain/dependency/M3 atau device action. D-044 menggantikan batas docs-only; eksekusi approved sampai APK checkpoint, tanpa mengubah proposal angka menjadi approval.

Rujukan keputusan/proposal: [Decision Register](05_DECISION_REGISTER.md). Riset aset/lisensi: [Legal dan Assets](04_LEGAL_AND_ASSETS.md). Gate bukti: [Testing](TESTING.md).

# JUST SMASH — Project Handoff

## CLOSURE-011 — otoritas dan penutupan aktif
Instruksi pemilik terbaru mengizinkan audit dan penutupan teknis lokal seluruh M0–M2, tanpa approval ulang perubahan teknis dalam scope. D-031 menunda review manual; D-034 menyetujui world-space change-ends/player-centric camera serta 3% salah call pada landing eligible dalam 5 cm, maksimum 1/match. D-016 menetapkan joystick/manual+assisted, bukan eksperimen swipe A/B. D-017 menetapkan Official15 saja; Classic21 terkunci sampai career2028, Quick16 dihapus.

Requirement dan acceptance di bawah tetap berlaku kecuali digantikan keputusan APPROVED lebih baru. Hanya status checkpoint lama yang merupakan **riwayat**, bukan status aktif. Pernyataan lama M2 dibekukan, P-006 belum disetujui, Quick16 default, belum ada build, atau lisensi aktivasi terblokir tidak berlaku lagi. Status per task dan closure plan aktif: `evidence/M2/closure-task-audit.json`; hasil baru dicatat di TESTING. Izin aktif D-037..D-042: commit lokal boleh dengan audit; HP install/launch memerlukan izin terpisah; push, dependency baru, biaya dan M3 tetap dilarang.

Pisahkan Implementation Complete, Local QA Passed, Device QA Pending, Owner Acceptance Pending, dan Milestone Fully Verified. Task canceled M1-09 dianggap requirement superseded terpenuhi, bukan fitur swipe yang diimplementasikan. Device QA pending bukan otomatis BLOCKED_ENV tanpa kegagalan lingkungan konkret.


## Active D-032 — M1 seven gaps
- Local debug, VFX config, tester form and procedural motion added; camera/stroke/high-speed evidence in evidence/M1/gaps*. Detailed gate/status in TESTING.
- Host tests/build do not close natural-animation, Android performance or deferred owner acceptance. No phone activity this session.

## Status aktif D-031 — 2026-11-02
- M1 perubahan lokal grade feedback telah tes/build; langsung M2 tanpa tes pemilik. Pembekuan M2 di riwayat D-030 tidak lagi aktif.
- M2 IN_PROGRESS: double-hit fault, timer validation, recovery parsial dan 30-point controlled integration host PASS; PlayMode 18/18. APK M2 build/packaging PASS, bukan acceptance M2 lengkap.
- Lihat TESTING run M1-LOCAL-003 / M2-LOCAL-002 untuk gap; tidak instal/launch HP sebelum M2 selesai. Review subjective/fisik ditunda, bukan PASS.

## Status terkini — M1-PRIORITY-002 (2026-11-02)
- D-030: prioritaskan penutupan M1; M2 dibekukan, source M2 existing dipertahankan. Status lama di bawah merupakan riwayat, bukan hasil terkini.
- BUG-006 assisted aim/movement host regression FIXED; PlayMode 14/14, EditMode 9/9, APK build dan packaging PASS. Bukti/cakupan di TESTING.
- M1 IN_PROGRESS: Android shader-startup retest, natural gameplay recording/visual acceptance, hardware feel/performance dan owner review belum selesai. Controlled fixture bukan acceptance penuh.
- APK review lokal `builds/JustSmash-M1.apk`; tidak dipasang sesi ini. Gate instal HP D-029 tetap berlaku.


## Riwayat scope — 2026-11-02
Pemilik mengizinkan implementasi seluruh scope sampai M1, tidak M2+. D-016–D-028 dan instruksi chat terbaru menggantikan proposal lama: joystick kiri untuk gerak sekaligus arah pukulan, enam sektor tanpa landing/trajectory preview; jump tombol hold-swipe dengan guide teknik; assisted menggerakkan atlet sepenuhnya tanpa input movement. Default Official15, Classic21 Settings terbuka saat career 2028; Quick16 dihapus. Scoring penuh, challenge, stamina match, career, sponsor dan doubles tetap milestone sesudah M1. A/B swipe movement dibatalkan; gate M1 menjadi joystick/manual+assisted pada high lob, smash approach, near-net. Uji AVD Android resmi untuk runtime; HP hanya untuk feel multitouch dan performa perangkat nyata. Status historis di bawah bukan status terkini.

Versi: 0.2 | Tanggal baseline/revisi: 2026-11-01 | Status: spesifikasi pra-produksi

## Cara memakai dokumen
1. Baca [AI_RULES](AI_RULES.md) dan [Decision Register](05_DECISION_REGISTER.md) untuk batas kerja; instruksi langsung terbaru pemilik menggantikan ketentuan historis yang bertentangan.
2. Baca [PRD](PRD.md) dan [GDD](01_GDD.md). USER-LOCKED dipertahankan; PRD memetakan kebutuhan, GDD menyimpan detail gameplay utuh.
3. Baca [ARCHITECTURE](ARCHITECTURE.md), [DESIGN](DESIGN.md), dan [Legal/Assets](04_LEGAL_AND_ASSETS.md) sesuai task.
4. Ikuti [roadmap](02_ROADMAP.md): M0, lalu M1; jangan otomatis lompat ke career/kosmetik. Sesi ini hanya penataan dokumen, bukan izin implementasi.
5. Gunakan [TESTING](TESTING.md) untuk rencana/hasil nyata dan [ERROR_LOG](ERROR_LOG.md) untuk bug serta bukti perbaikan.

## Handoff terbaru D-043
Jawaban enam keputusan tercatat di register. Personal eligible berdasarkan kondisi owner; NPC evaluation import disetujui; default Fixed; servis1detik; grade Timing+Contact, D/P trace; stamina30detik belum habis. Angka balancing baru belum approved.

## Riwayat handoff D-037..D-042
Dokumentasi/proposal selesai; coding/import/device action belum dilakukan sesi ini. Baca Decision Register untuk keputusan/angka PROPOSED dan Legal untuk evaluasi4aset+alternatif. Gate review singles di TESTING; checkpoint/task lama adalah riwayat, bukan izin review granular.

## Indeks dokumen

| File | Fungsi |
|---|---|
| [00_README.md](00_README.md) | Indeks, urutan baca, hierarki dan status handoff |
| [01_GDD.md](01_GDD.md) | Detail visi, gameplay dan batas produk |
| [02_ROADMAP.md](02_ROADMAP.md) | Task M0–M8, dependensi dan acceptance gate |
| [AI_RULES.md](AI_RULES.md) | Aturan agent; pengganti nama file Agent Rules lama |
| [04_LEGAL_AND_ASSETS.md](04_LEGAL_AND_ASSETS.md) | Legal, lisensi, privasi, aset, sumber dan gate publikasi |
| [05_DECISION_REGISTER.md](05_DECISION_REGISTER.md) | Satu-satunya register keputusan dan approval |
| [PRD.md](PRD.md) | Kebutuhan produk dan traceability requirement ke tes |
| [ARCHITECTURE.md](ARCHITECTURE.md) | Rancangan teknis Unity, state, save, simulasi dan batas sistem |
| [DESIGN.md](DESIGN.md) | Visual/UX proyek, kontrol, kamera dan status referensi |
| [TESTING.md](TESTING.md) | Acceptance, protokol pengukuran dan hasil tes aktual |
| [ERROR_LOG.md](ERROR_LOG.md) | Bug, percobaan perbaikan dan regression evidence |

## Keputusan terbaru dan status pekerjaan

- Unity sebagai engine disetujui. Versi editor/pipeline/package masih menunggu audit M0; bukan izin biaya atau cloud.
- Preset Modern15 dan challenge IN/OUT dua gagal/game disetujui; sukses tidak memotong jatah. Quick16 tetap default sementara. Proposal lain tetap mengikuti status register.
- Batas **keseluruhan game maksimal 1 GB**, bukan hanya APK; metode pengukuran di TESTING.
- Enam dokumen sumber tetap dipertahankan, dengan Agent Rules di-rename dan perubahan disetujui diterapkan. Enam dokumen wajib ditambahkan. Backup sebelum revisi ada di `.backups/docs_20261101_225027/` relatif root proyek.
- Implementasi sampai M1 telah diizinkan (D-015). Audit M0 dan percobaan createProject dijalankan: BLOCKED_LICENSE, Unity return code 198. Belum ada build atau tes gameplay; lihat TESTING run M0-AUDIT-001.
- ASSET_REGISTER.csv dibuat saat perolehan aset/dependency pertama; belum ada entri aset yang diverifikasi. Hasil QA kini dicatat di TESTING; WORKFLOW telah dihapus, tanpa ledger pengganti.

## Hierarki otoritas
- Instruksi langsung terbaru pemilik produk > pernyataan USER-LOCKED pada GDD > keputusan disetujui yang tercatat di Decision Register > task roadmap > usulan agent.
- Perubahan terhadap desain produk memerlukan persetujuan sebelum coding. Perubahan teknis internal yang ekuivalen dapat dilakukan dengan alasan dan tes.
- Kalau dua dokumen bertentangan, berhenti pada scope yang konflik dan ajukan pertanyaan terfokus; lanjutkan tugas lain yang tidak terblokir.
- Jangan menafsirkan usulan `PROPOSED` sebagai persetujuan pengguna.

## Sasaran build pertama
Satu scene Android landscape, satu lapangan, dua karakter placeholder, shuttlecock dengan physics wajar, kamera belakang, kontrol eksperimen A/B, bisa melakukan dan menerima pukulan, tampil skor debug, serta debug visualization posisi contact. Playable lebih penting daripada banyak menu.

## Status sumber dan verifikasi
Sumber diwarisi dari baseline 1 Nov 2026; belum diverifikasi ulang dalam penataan dokumen ini. Beberapa regulasi olahraga/lisensi berubah; sumber harus diperiksa ulang sebelum rilis. Dokumen di sini bukan opini hukum resmi. Tautan sumber ada di `04_LEGAL_AND_ASSETS.md` dan `01_GDD.md`.
