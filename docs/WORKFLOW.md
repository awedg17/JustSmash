# Just Smash — Workflow Pengembangan

Versi 0.2 | 2026-11-01 | Scope saat ini: dokumentasi, bukan implementasi game.

## Sumber kebenaran

Instruksi terbaru pemilik > USER-LOCKED GDD > keputusan disetujui dalam Decision Register > requirement/detail terkait > roadmap > usulan agent. Bila konflik, berhenti hanya pada scope konflik, tanyakan keputusan konkret; pekerjaan independen yang sudah diizinkan dapat lanjut. PROPOSED/TO TEST bukan APPROVED.

Baca [indeks](00_README.md), [AI_RULES](AI_RULES.md), [Decision Register](05_DECISION_REGISTER.md), GDD dan task aktif sebelum bekerja. Detail tidak boleh ditebak dari proyek lain. Dokumen merupakan kontrak acceptance; kode lama tidak mengalahkan keputusan pemilik.

## Siklus kerja

1. **Audit read-only.** Cek file, perubahan user, status Git, roadmap, bukti sebelumnya, toolchain/device yang relevan. Jangan mengaku Unity/device tersedia sebelum diperiksa.
2. **Temuan dan rencana.** Sebut requirement/decision/task, scope kecil, file yang akan berubah, risiko, rollback dan tes. Tunggu persetujuan coding/fix bila scope belum diizinkan; jangan minta ulang persetujuan yang sudah jelas.
3. **Checkpoint.** Bila belum ada Git, backup file sebelum edit. Git lokal dibuat hanya dalam scope implementasi yang diizinkan; commit lokal saja, jangan `git push`, force/reset atau menghapus pekerjaan user.
4. **Implementasi minimum.** Kerjakan satu irisan terukur pada milestone aktif. YAGNI, native/stdlib/dependency tersedia lebih dulu. Tanpa scaffolding career atau interface abstrak yang belum dibutuhkan. Tidak memakai subagent tanpa permintaan pemilik.
5. **Tes nyata.** Reproduksi/failing test sebelum fix jika memungkinkan, implementasi, regression, lalu integration/build/device sesuai acceptance. Agent menjalankan perintah, bukan menyerahkan pekerjaan pengembangan rutin ke pemilik. Pemilik melakukan penilaian rasa bermain dan feedback.
6. **Batas kegagalan.** Maksimal 3 auto-fix per error sama. Jika masih gagal, hentikan scope tersebut; laporkan error persis, percobaan, bukti, sebab yang diketahui dan opsi. Jangan mengganti engine/fitur atau mengarang hasil untuk menutupi blocker.
7. **Sinkronkan dokumen.** Hasil tes ke TESTING, bug/percobaan/solusi ke ERROR_LOG, keputusan ke satu register, task dan blocker ke roadmap, ringkasan perubahan ke riwayat di bawah. Tidak ada ledger duplikat.
8. **Handoff singkat.** Laporkan perubahan, hasil nyata sebelum/sesudah, artifact/hash, batas yang belum diuji dan keputusan yang dibutuhkan. DONE_VERIFIED hanya dengan acceptance dan evidence, bukan karena checkbox atau build berhasil.

## Urutan milestone dan gate

Task ID dan DoD lengkap tetap di [roadmap](02_ROADMAP.md), tidak diduplikasi sebagai task baru.

| Milestone | Dependensi | Deliverable dan gate |
|---|---|---|
| M0 | Izin implementasi; engine Unity sudah disetujui | Audit, lisensi, local project/test setup, development APK minimum; toolchain/version/device dibuktikan |
| M1 | M0 build lolos | Satu court, shuttle/contact/movement, A/B/assisted; pemilik memilih kontrol atau mengizinkan iterasi |
| M2 | Gate kontrol M1; aturan yang digunakan disetujui | Singles penuh, skor/servis, save/resume, challenge dan replay; tidak stuck rally |
| M3 | Pertandingan M2 valid | Difficulty fair dan rasa bermain teruji; jangan lanjut career saat gameplay rusak |
| M4 | Gameplay M3 layak; detail world sim yang dipakai disetujui | Headless world >10 tahun, ranking/draw/save konsisten |
| M5 | Simulasi M4 lolos | Career singles create sampai retire/archive; seluruh alur dapat dipakai |
| M6 | Career singles M5 layak; detail doubles disetujui | Doubles nyata, AI partner/rotation/ranking, device profiling |
| M7 | Sistem career terkait lolos dan scope content disetujui | Economy, sponsor, offcourt, customization; aset legal dan kosmetik saja |
| M8 | Fitur/content terkait selesai | Performance, total ukuran, integritas save, legal/privacy, signed APK dan keputusan distribusi |

Tidak ada pekerjaan milestone implementasi yang selesai hanya karena dokumen telah dirapikan. Unity sudah dipilih; versi/pipeline belum diaudit. Modern15 dan challenge dua gagal/game disetujui; A/B kontrol dan proposal lain tidak otomatis disetujui. Full-plan approval nantinya tidak memerlukan approval ulang tiap langkah teknis, tetapi gate material/desain/biaya/privacy tetap berlaku.

## Struktur dokumen dan kepemilikan fakta

- GDD: detail gameplay/visi utuh.
- PRD: requirement dan traceability.
- ARCHITECTURE: batas sistem dan keputusan teknis, bukan klaim implementasi.
- DESIGN: visual/UX berdasarkan referensi proyek ini.
- AI_RULES: batas tindakan agent.
- TESTING: rencana dan hasil aktual; menggantikan kebutuhan QA_REPORT terpisah.
- ERROR_LOG: bug dan bukti perbaikan, termasuk percobaan gagal.
- WORKFLOW: proses dan riwayat perubahan; menggantikan kebutuhan CHANGELOG terpisah.
- 05_DECISION_REGISTER: satu-satunya ledger keputusan, bukan DECISIONS tambahan.
- 04_LEGAL_AND_ASSETS: legal, provenance, sumber riset dan publication gate.
- ASSET_REGISTER.csv dibuat saat ada asset/dependency yang perlu dicatat; gunakan kolom Legal §D. Tidak mengisi aset fiktif atau mengaku license review sudah dikerjakan.

## Batas operasional

- Scope hanya JustSmash; tidak mengubah proyek/profil lain.
- Rp0, local development, no deploy/cloud/paid publishing tanpa izin terpisah. Unity tidak membatalkan batas tersebut.
- Tidak membaca/menaruh password, token, signing key dalam chat atau ledger. Tidak overwrite source/save tanpa checkpoint.
- Simpan bukti build/test nyata secara lokal saat dibuat. Screenshot mockup bukan bukti gameplay.
- Tes fisik yang terhalang device dicatat BLOCKED_DEVICE di roadmap dan BLOCKED pada case terkait. Jangan klaim lolos dari Editor.
- Jangan mempromosikan klaim riset olahraga/legal lama sebagai verifikasi baru. Source URL dan tanggal baseline dipertahankan; lakukan cek sumber sebelum implementasi aturan atau rilis yang bergantung padanya.

## Riwayat perubahan

| Tanggal | Scope | Perubahan | Bukti / status |
|---|---|---|---|
| 2026-11-01 | Pemindahan awal | Enam Markdown dipindah ke docs tanpa mengubah isi | Enam SHA-256 sebelum/sesudah cocok |
| 2026-11-01 | Penataan disetujui pemilik | Backup enam sumber; rename Agent Rules; tambah enam dokumen wajib; sync approval Unity/Modern15/challenge, batas total 1 GB, indeks dan workflow | Backup `.backups/docs_20261101_225027/`; PASS: 12 Markdown, 30 link lokal valid, 114 task ID lama utuh, 14 pasangan requirement/test; seluruh tes game NOT_RUN. Legal byte-identical dengan backup. Bukti pemeriksaan: `.backups/docs_20261101_225027/verification.json` |

## Langkah berikut

Pemilik telah mengizinkan implementasi sampai M1 dan menyetujui rencana. Audit nyata menemukan Unity 6000.3.18f1 + toolchain Android. Backup docs dibuat di `.backups/pre_M0_20261101_231032/docs`; Git lokal branch `feature/m0-m1` dan `.gitignore` dibuat, tanpa push. Eksekusi createProject gagal karena lisensi Editor belum aktif (BUG-001, return code 198; `evidence/M0/create-project.log`). Tidak ada APK, project playable, atau klaim gameplay PASS. Android belum terhubung. Lanjut setelah pemilik aktivasi Unity Hub; ulangi createProject, tes dan M0 build sebelum M1.
