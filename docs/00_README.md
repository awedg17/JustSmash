# JUST SMASH — Project Handoff
Versi: 0.2 | Tanggal baseline/revisi: 2026-11-01 | Status: spesifikasi pra-produksi

## Cara memakai dokumen
1. Baca [AI_RULES](AI_RULES.md), [WORKFLOW](WORKFLOW.md), dan [Decision Register](05_DECISION_REGISTER.md) untuk izin, batas kerja dan keputusan terbaru.
2. Baca [PRD](PRD.md) dan [GDD](01_GDD.md). USER-LOCKED dipertahankan; PRD memetakan kebutuhan, GDD menyimpan detail gameplay utuh.
3. Baca [ARCHITECTURE](ARCHITECTURE.md), [DESIGN](DESIGN.md), dan [Legal/Assets](04_LEGAL_AND_ASSETS.md) sesuai task.
4. Ikuti [roadmap](02_ROADMAP.md): M0, lalu M1; jangan otomatis lompat ke career/kosmetik. Sesi ini hanya penataan dokumen, bukan izin implementasi.
5. Gunakan [TESTING](TESTING.md) untuk rencana/hasil nyata dan [ERROR_LOG](ERROR_LOG.md) untuk bug serta bukti perbaikan.

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
| [WORKFLOW.md](WORKFLOW.md) | Siklus audit/approval/build/test/handoff dan riwayat perubahan |
| [ERROR_LOG.md](ERROR_LOG.md) | Bug, percobaan perbaikan dan regression evidence |

## Keputusan terbaru dan status pekerjaan

- Unity sebagai engine disetujui. Versi editor/pipeline/package masih menunggu audit M0; bukan izin biaya atau cloud.
- Preset Modern15 dan challenge IN/OUT dua gagal/game disetujui; sukses tidak memotong jatah. Quick16 tetap default sementara. Proposal lain tetap mengikuti status register.
- Batas **keseluruhan game maksimal 1 GB**, bukan hanya APK; metode pengukuran di TESTING.
- Enam dokumen sumber tetap dipertahankan, dengan Agent Rules di-rename dan perubahan disetujui diterapkan. Enam dokumen wajib ditambahkan. Backup sebelum revisi ada di `.backups/docs_20261101_225027/` relatif root proyek.
- Implementasi sampai M1 telah diizinkan (D-015). Audit M0 dan percobaan createProject dijalankan: BLOCKED_LICENSE, Unity return code 198. Belum ada build atau tes gameplay; lihat TESTING run M0-AUDIT-001.
- ASSET_REGISTER.csv dibuat saat perolehan aset/dependency pertama; belum ada entri aset yang diverifikasi. Riwayat perubahan dan hasil QA tidak membutuhkan ledger terpisah dari WORKFLOW/TESTING.

## Hierarki otoritas
- Instruksi langsung terbaru pemilik produk > pernyataan USER-LOCKED pada GDD > keputusan disetujui yang tercatat di Decision Register > task roadmap > usulan agent.
- Perubahan terhadap desain produk memerlukan persetujuan sebelum coding. Perubahan teknis internal yang ekuivalen dapat dilakukan dengan alasan dan tes.
- Kalau dua dokumen bertentangan, berhenti pada scope yang konflik dan ajukan pertanyaan terfokus; lanjutkan tugas lain yang tidak terblokir.
- Jangan menafsirkan usulan `PROPOSED` sebagai persetujuan pengguna.

## Sasaran build pertama
Satu scene Android landscape, satu lapangan, dua karakter placeholder, shuttlecock dengan physics wajar, kamera belakang, kontrol eksperimen A/B, bisa melakukan dan menerima pukulan, tampil skor debug, serta debug visualization posisi contact. Playable lebih penting daripada banyak menu.

## Status sumber dan verifikasi
Sumber diwarisi dari baseline 1 Nov 2026; belum diverifikasi ulang dalam penataan dokumen ini. Beberapa regulasi olahraga/lisensi berubah; sumber harus diperiksa ulang sebelum rilis. Dokumen di sini bukan opini hukum resmi. Tautan sumber ada di `04_LEGAL_AND_ASSETS.md` dan `01_GDD.md`.
