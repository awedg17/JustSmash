# Just Smash — Product Requirements

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
| R-02 | Kontrol A/B dan assisted diuji pada scene sama; pemilik memilih kontrol final, tidak auto-lock | GDD §4, D-004, P-001/P-014 | M1 | T-02 |
| R-03 | Lima stroke, straight/cross, hold power/release timing, jump, grading dan sebab kegagalan terbaca | GDD §4, D-003/D-005 | M1–M2 | T-03 |
| R-04 | Shuttle drag/contact/net/floor, animasi, kamera belakang dan change-ends konsisten; tidak teleport | GDD §5, D-002, P-006 | M1–M2 | T-04 |
| R-05 | Quick16 cap20, Classic21 cap30 dan Modern15 cap21; deuce, best-of-3, servis/fault dan change-ends benar | GDD §3, D-007, P-002 | M2 | T-05 |
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

Sesi ini hanya penataan dokumen. Belum ada game, build, audit Unity, atau hasil tes gameplay. Implementasi pertama setelah izin adalah M0, kemudian M1 berdasarkan gate roadmap. Sasaran playable pertama tetap satu lapangan, dua placeholder, shuttle, kamera, eksperimen kontrol dan debug contact; bukan seluruh career atau menu final.

## Keputusan disetujui dan yang masih terbuka

- Unity disetujui sebagai engine (P-011); bukan persetujuan biaya/cloud atau versi editor tertentu.
- Modern15 disetujui sebagai preset tambahan (P-002); Quick16 tetap default sementara. Otomatis mengikuti tahun turnamen belum disetujui (P-003).
- Challenge IN/OUT dua gagal/game, berhasil tidak mengurangi jatah, hasil berdasarkan ground-truth disetujui (P-008). Simulasi kesalahan call dan probabilitasnya masih usulan.
- A/B kontrol, detail starting ages, ranking/progression, extra attributes, dan parameter balancing tetap mengikuti status register. Jangan menganggap persetujuan dokumen sebagai persetujuan semua PROPOSED.

## Di luar scope

Multiplayer, backend, akun pemain, monetisasi premium, coaching career setelah pensiun, trickshot, cloning aset/IP, paid publishing dan fitur di luar milestone yang disetujui.

## Kriteria selesai

Setiap R-ID memiliki hasil tes dan bukti yang terkait build nyata; seluruh gate terkait lolos; tidak ada bug kritis terbuka atau keputusan material yang dilewati. Approval gameplay subjektif dari pemilik tetap diperlukan. Checklist, dokumen lengkap, atau build sukses saja tidak membuktikan produk selesai.
