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
Work sequentially. Deliver milestone artifacts and ask user to validate controls before polishing. Do not hide failures. Do not claim visual or audio reference review until actually reviewed. If no physical Android device, report emulator/Editor-only limitation.

## Rule 8 — Code constraints
Unity is approved. Use C# components sized for current milestone; use event-driven message contracts only where needed. Use testable data models for career simulation. Avoid game systems tightly coupled to 3D scene. Avoid hardcoded player/athlete names and enum indices as IDs. Use deterministic RNG seed for world sim/testing. Keep autosaves atomic, versioned, backed up. Avoid unnecessary packages/plugins.

## Rule 9 — Playability and fairness
No power multipliers to AI solely due to difficulty. No collision hacks that teleport characters at contact. No unexplainable miss. No physics frame-dependent simulation. A tester must be able to see input/timing/position/shot result trace.

## Rule 10 — External content and legal
No ripped YouTube audio, copied artwork/mesh/animation, trademark/logos/names/real athlete lookalikes without clearance, copyrighted game UI reuse. A free price tag is NOT evidence of commercial reuse rights. If unclear, do not import. Legal disclaimer doesn't waive rights.

## Rule 11 — Workflow pemilik
- Sebelum coding/fix, sampaikan temuan, rencana, risiko, dan tes; tunggu persetujuan scope. Persetujuan penataan dokumen bukan izin mulai membangun game. Jangan minta ulang keputusan yang sudah disetujui.
- Maksimal 3 percobaan auto-fix untuk kegagalan yang sama; bila tetap gagal, hentikan scope tersebut, laporkan error persis, bukti, dan penyebab yang diketahui. Jangan mengarang root cause atau fallback.
- Git lokal saja. **Jangan `git push`**; pemilik meninjau dan melakukan push sendiri. Jangan init/commit pada sesi dokumen ini.
- Backup sebelum perubahan jika belum ada Git. Jangan overwrite pekerjaan user, hapus save, atau expose credential.
- Pilih tidak menulis kode jika tidak diperlukan; gunakan fitur native/stdlib/dependency yang sudah ada sebelum menambah package. Tanpa interface satu implementasi, class dummy, atau scaffolding untuk milestone jauh.
- Logika nontrivial harus punya tes runnable. PASS memerlukan hasil eksekusi; build sukses bukan bukti gameplay/performance.
- Update dokumen, status roadmap, dan bug log sesuai bukti pada sesi yang sama. Laporan singkat: perubahan, hasil tes, sisa blocker.
- Jangan menyalin warna atau workflow produk lain ke Just Smash; ikuti `DESIGN.md` dan GDD. Tidak memakai subagent tanpa permintaan pemilik.

## First instruction to coding agent (copy/paste) — hanya setelah izin implementasi
> Baca seluruh file dalam `/docs/`, laporkan audit/rencana dan konfirmasi izin implementasi bila belum ada, kemudian kerjakan **MILESTONE 0 saja** dari `02_ROADMAP.md` sampai APK Android development minimum berhasil dibuat dan diverifikasi. Jangan mulai career mode, membangun menu lengkap, membuat aset final, atau memilih kontrol joystick/swipe secara sepihak. Buat laporan audit project, structure proposal yang proporsional, build evidence, license register, dan list blocker. Semua item USER-LOCKED wajib dipertahankan. Jika terjadi konflik, tulis opsi dan tunggu persetujuan untuk scope terkait. Jangan memakai tool/aset berbayar atau mengaktifkan analytics/network SDK.
