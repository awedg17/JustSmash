## D-047 — APPROVED: isolated Blender outfit repair, 2026-11-04
- Owner explicitly permits official free Blender installation for one original badminton outfit: jersey, shorts, court shoes. No cost, other dependency, scope expansion or paid substitute.
- Back up before edits; preserve gameplay and old assets. Repair seams/fit/coverage/weights/deformation; agent inspects lunge, overhead, jump, backhand, dive-save poses. Maximum3 automatic fixes per same defect; stop and propose asset/workflow alternative if approach fails.
- No failed visual integration; no final import, final APK or phone action under this repair approval. Relevant gates and further required permissions remain mandatory. Deliver pose evidence and mesh/rig validation plus integration eligibility, not gameplay acceptance.
- Official license https://www.blender.org/about/license/ confirms free use and commercial artwork rights. Installation/source/hash and QA results recorded under evidence/M2/quaternius/blender/; approval does not mean validation PASS.

## Kebijakan aktif — Quaternius exhibition, 2026-11-04
Instruksi owner menjalankan `.hermes/plans/quaternius-exhibition-review.md` sampai selesai mengizinkan akuisisi/audit Quaternius Standard gratis CC0, outfit sportswear orisinal dan integrasi hanya setelah preview/deformasi lolos. Casual Set00 hanya referensi; sumber lama dipertahankan. D-045 tetap mengizinkan kalibrasi grade/stamina dan install/update/launch HP tanpa uninstall/clear data setelah QA lokal dan APK final. Tidak M3/career, biaya, dependency baru atau push. Riwayat kebijakan lama di bawah tidak membatalkan izin terbaru. Device/visual/audio dan review owner tetap gate terpisah.

## Kebijakan aktif — D-037..D-042, 2026-11-03
Instruksi owner terbaru menggantikan aturan lama yang bertentangan; bagian checkpoint/run lama adalah riwayat, bukan izin aktif. D-044 mengizinkan eksekusi M0–M2 sampai satu APK checkpoint baru; batas docs-only sebelumnya berakhir. Otorisasi implementasi tetap M0–M2; M3+ tidak dimulai otomatis, termasuk setelah acceptance M2. Tidak ada import aset/dependency, biaya, push, destructive reset, atau install/launch APK HP tanpa izin terpisah.

Review owner hanya pengalaman modul utuh: singles lengkap; character creation lengkap; career terintegrasi; final game. Ini kelompok review, bukan penambahan scope. Tidak meminta owner menguji pukulan/animasi/tombol/fault secara granular. Agent menangani unit/integration/regression, natural automated match, inspeksi video/motion/audio, dan performance sesuai izin. Keputusan desain tetap memerlukan approval, berbeda dari testing.

Subagent dan commit lokal bersyarat tetap D-038/D-039. D-043 menyetujui NPC Casual Set00 untuk evaluasi import, default joystick Fixed (Floating tetap pilihan), servis charge1detik/power0..1/feedback konsisten/cancel aman. Grade hanya Timing + Contact; Direction/Power trace tanpa kontribusi grade. Rally sekitar30detik menurunkan stamina cukup banyak dan mulai memengaruhi performa, tetapi belum habis dan rally bisa berlanjut; jalan/lari drain0. Bobot/formula grade, biaya/kurva/efek stamina baru belum approved. Unity Personal memenuhi kondisi pribadi non-klien tanpa pendapatan yang dinyatakan owner; tidak meminta pembahasan keuangan lagi kecuali kondisi/terms relevan berubah. Target karakter/animasi/atmosfer singles tetap. Approval evaluasi NPC bukan izin import aset lain/dependency/M3 atau device action. D-044 menggantikan batas docs-only; eksekusi approved sampai APK checkpoint, tanpa mengubah proposal angka menjadi approval.

Rujukan keputusan/proposal: [Decision Register](05_DECISION_REGISTER.md). Riset aset/lisensi: [Legal dan Assets](04_LEGAL_AND_ASSETS.md). Gate bukti: [Testing](TESTING.md).

# JUST SMASH — Agent Operating Rules
Versi 0.2 | Mandatory

## Rule 1 — Specification first
Read all `/docs/*.md` at start. Before each task, cite GDD sections/decision IDs that authorize it. Do not treat old code or your intuition as authority over user's spec. If a detail is unspecified, make a minimal reversible technical choice only when player-facing result is unchanged; otherwise propose alternatives and await approval.

## Rule 2 — No silent scope expansion
Do not create features outside approved milestone or add telemetry, accounts, ads, premium money, network dependency, unapproved AI assets, new game mechanic, new ruleset or new UI tab. If user requests extra feature, update GDD/Decision Register first, receive approval, then implement.

## Rule 3 — Risk-tag changes
- `TECHNICAL_INTERNAL`: refactor, implementation details maintaining gameplay outputs; agent may proceed with tests and report only within already approved work; otherwise present findings/plan and obtain approval first.
- `GAMEPLAY_CHANGE`: input, controls, player movement, hit physics, score rules, AI difficulty, progression, stamina; owner approval mandatory.
- `CONTENT_IP_CHANGE`: athlete names/images, logos, audio, sponsors, assets, fonts/license; owner approval and provenance mandatory.
- `COST_PRIVACY_CHANGE`: any spend, network SDK, account, analytics, external services, licensing cost; default prohibited, must disclose; paid cost cannot proceed under Rp0 requirement.
- `MILESTONE_SCOPE_CHANGE`: new priority or scope; approval mandatory.

## Rule 4 — Changes and documents
Maintain change history in `WORKFLOW.md`, test plans and actual results in `TESTING.md`, bugs in `ERROR_LOG.md`, decisions only in `05_DECISION_REGISTER.md`, and asset provenance in `ASSET_REGISTER.csv` when assets are acquired. Do not create duplicate CHANGELOG, QA_REPORT, or decision documents. Add decision record `id, request, impact, options, rationale, approval_date, approving_user, tests`. Never mark `PROPOSED` as `APPROVED` on behalf of owner. Preserve source markdown on every handoff.

## Rule 5 — Definition of ready
Before coding: requirement link, scope, dependency, acceptance criteria, rollback strategy, risks. An undefined rule is `BLOCKED_APPROVAL` rather than implemented as a guess.

## Rule 6 — Definition of done
Feature works in real playable build; automated test or manual reproducible scenario; logs and evidence; basic mobile performance; no known critical regression; asset/source record complete; QA status explicit. 'Code compiles' != completed feature.

## Rule 7 — Responsive collaboration
Gunakan checkpoint internal M0–M8; review owner hanya modul utuh sesuai D-037. Agent menjalankan QA teknis, bukan owner. Do not hide failures. Do not claim visual or audio reference review until actually reviewed. If no physical Android device, report emulator/Editor-only limitation.

## Rule 8 — Code constraints
Unity is approved. Use C# components sized for current milestone; use event-driven message contracts only where needed. Use testable data models for career simulation. Avoid game systems tightly coupled to 3D scene. Avoid hardcoded player/athlete names and enum indices as IDs. Use deterministic RNG seed for world sim/testing. Keep autosaves atomic, versioned, backed up. Avoid unnecessary packages/plugins.

## Rule 9 — Playability and fairness
No power multipliers to AI solely due to difficulty. No collision hacks that teleport characters at contact. No unexplainable miss. No physics frame-dependent simulation. A tester must be able to see input/timing/position/shot result trace.

## Rule 10 — External content and legal
No ripped YouTube audio, copied artwork/mesh/animation, trademark/logos/names/real athlete lookalikes without clearance, copyrighted game UI reuse. A free price tag is NOT evidence of commercial reuse rights. If unclear, do not import. Legal disclaimer doesn't waive rights.

## Rule 11 — Workflow pemilik
- Sebelum coding/fix, sampaikan temuan, rencana, risiko, dan tes; tunggu persetujuan scope. Persetujuan penataan dokumen bukan izin mulai membangun game. Jangan minta ulang keputusan yang sudah disetujui.
- Maksimal 3 percobaan auto-fix untuk kegagalan yang sama; bila tetap gagal, hentikan scope tersebut, laporkan error persis, bukti, dan penyebab yang diketahui. Jangan mengarang root cause atau fallback.
- Git lokal saja. **Jangan `git push`**; pemilik meninjau dan melakukan push sendiri. Commit lokal diizinkan D-039 setelah audit dirty/untracked, secret dan file build besar; rollback tanpa kehilangan pekerjaan.
- Backup sebelum perubahan jika belum ada Git. Jangan overwrite pekerjaan user, hapus save, atau expose credential.
- Pilih tidak menulis kode jika tidak diperlukan; gunakan fitur native/stdlib/dependency yang sudah ada sebelum menambah package. Tanpa interface satu implementasi, class dummy, atau scaffolding untuk milestone jauh.
- Logika nontrivial harus punya tes runnable. PASS memerlukan hasil eksekusi; build sukses bukan bukti gameplay/performance.
- Update dokumen, status roadmap, dan bug log sesuai bukti pada sesi yang sama. Laporan singkat: perubahan, hasil tes, sisa blocker.
- Jangan menyalin warna atau workflow produk lain ke Just Smash; ikuti `DESIGN.md` dan GDD. Subagent diizinkan D-038; agent utama menjaga scope/file, integrasi, merge-audit dan regression.

## First instruction to coding agent (copy/paste) — hanya setelah izin implementasi
> Baca seluruh file dalam `/docs/`, laporkan audit/rencana dan konfirmasi izin implementasi bila belum ada, kemudian kerjakan **MILESTONE 0 saja** dari `02_ROADMAP.md` sampai APK Android development minimum berhasil dibuat dan diverifikasi. Jangan mulai career mode, membangun menu lengkap, membuat aset final, atau memilih kontrol joystick/swipe secara sepihak. Buat laporan audit project, structure proposal yang proporsional, build evidence, license register, dan list blocker. Semua item USER-LOCKED wajib dipertahankan. Jika terjadi konflik, tulis opsi dan tunggu persetujuan untuk scope terkait. Jangan memakai tool/aset berbayar atau mengaktifkan analytics/network SDK.
