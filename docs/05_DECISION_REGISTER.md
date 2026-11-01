# JUST SMASH — Decision Register
Tanggal 2026-11-01 | **Usulan tidak boleh otomatis dianggap disetujui.**

## Sudah ditentukan pemilik (USER-LOCKED)
| ID | Keputusan |
|---|---|
| D-001 | Nama Just Smash, Android offline single player career; landscape, 4GB device RAM, keseluruhan game <=1 GB, bukan hanya APK, target 30 FPS, biaya produksi Rp0. |
| D-002 | Gameplay sebagai badminton langsung dengan skill, kamera di belakang ala gambar pengguna, physics dan animasi natural. |
| D-003 | Pukulan smash, lob, drop, netting, lift; arah straight/cross; hold power, timing kontak, kualitas purple/green/yellow/red. |
| D-004 | Kontrol belum dipilih; joystick vs swipe perlu eksperimen; assisted positioning tersedia. |
| D-005 | Jump button serta swipe-up yang aktif pada timing tepat; tidak ada trickshot. |
| D-006 | Stamina dikaitkan intensitas, antarset recovery tidak 100%; athlete attribute & growth. |
| D-007 | 16-point game best-of-3 cap20 dengan deuce, option 21-point rules; official rules dibuat benar. |
| D-008 | Men singles/doubles, exhibition, pause+resume+save, skip/sim, challenge, moments replay toggle. |
| D-009 | Multi save career, satu athlete aktif per career, start path/age custom, retire >=20, read-only career archive. |
| D-010 | News/calendar/training/awards/achievements/popularity/reputation/ offcourt events. |
| D-011 | Skill point match/training, attributes 1-100, overall cap99 floor, aging decline, coach traits, expensive respec, cosmetic equipment only. |
| D-012 | AI easy/normal/hard/adaptive, harder = smarter, no AI stat boost/user nerf; adaptive observed behavior only. |
| D-013 | 3D stylized inspired Valorant, cosmetic customizations, turquoise simple UI reference, sound effects/crowd, no music/commentator. |
| D-014 | No accounts, no premium currency, no P2W, no real-world spend, no unapproved changes. |

## Proposed for approval (not automatically locked)
| ID | Status | Proposed choice | Reason / validation |
|---|---|---|---|
| P-001 | TO TEST | Two control prototypes manual joystick hybrid vs swipe; assisted third option | Critical UX uncertainty; owner chooses after video/playing. |
| P-002 | APPROVED | Tambahan preset Modern15 target15, deuce14, cap21; Classic21 tetap target21 cap30 | Pemilik menyetujui gameplay yang diajukan pada 2026-11-01. Quick16 tetap default sementara; penggantian otomatis berdasarkan tahun belum disetujui. Klaim regulasi eksternal tetap perlu verifikasi. |
| P-003 | PROPOSED | Career default Quick16, official optional by tournament year after explicit approval | User preferred 16 for playable duration. |
| P-004 | PROPOSED | Junior start ages 13-16, U17/U19 16-18, amateur/senior 17+, pro 18+ | Game design suggestions; not universal eligibility facts. |
| P-005 | PROPOSED | Separate junior/national/world rankings, rolling 52-week best-10 results senior, entry windows, qualifying draws | Realistic ranking, maintainable simulation. |
| P-006 | PROPOSED | Player-centric camera reorients across change-ends, rally physics remain world-space | Same screen POV even if court sides switch. |
| P-007 | PROPOSED | Core stats Power/Speed/Stamina/Technique plus Reaction/Balance/Athleticism | Effects must be distinct; owner must approve additions. |
| P-008 | APPROVED | Challenge IN/OUT: 2 gagal per game; berhasil tidak memotong jatah; keputusan akhir dari posisi jatuh sebenarnya | Pemilik menyetujui gameplay yang diajukan pada 2026-11-01. Probabilitas/simulasi call awal keliru tetap PROPOSED, bukan ikut disetujui. |
| P-009 | PROPOSED | Currency only match/contract earned, social content light text-card | Zero monetization, small build. |
| P-010 | PROPOSED | Fictional athletes with completely independent identities, real sports tier *inspired* but original brand presentation | Reduce IP / personality rights risk. |
| P-011 | APPROVED (engine only) | Unity sebagai engine; Personal sesuai eligibility Rp0 | Pemilik: "unity dijadikan sebagai engine utk membuat. gua approve unity", 2026-11-01. Versi editor/URP/package belum dikunci; audit M0. Keberadaan Unity Hub belum diperiksa sesi ini. |
| P-012 | PROPOSED | RNG seeded headless career world sim with ~1000 athletes (subject profiling) | Long careers without 3D sim overhead. |
| P-013 | PROPOSED | Doubles after full singles career prototype; partner is AI, one career athlete only | Significant separate mechanic, protect gameplay quality. |
| P-014 | TO TEST | Charging uses press-and-release time at contact; zone aim from right thumb | Potential conflict with jump/aim and timely hit. |
| P-015 | PROPOSED | Coaching and stamina penalties never hard-block training: soft fatigue/injury risk | Requested unlimited training cadence. |

## Known contradictions / clarification gates
- User first stated 15 points, then explicitly revised to **16**. Later 16 wins as current user decision. BWF official rules separate presets; do not silently override Quick16.
- User said official 21 capped 25 `cmiiw`; actual classic reaches max30. Treat 25 as factual misconception, not an irrevocable wish.
- User timing 'shuttle in front/above head' is **overhead only**; net/lift contacts physically lower. Need stroke-based contact windows.
- 'Stamina 15 minutes per set' is a **calibration target**, not guarantee every set contains that much rally time.
- Single player vs men's doubles is compatible via AI partner, but doubles requires dedicated scope.
- `top 10 countries per subcontinent` may be impossible/pointless in some small groupings; propose up to10 per curated region, no invented nations; owner approval before final list.
- Player wants to face 'top dunia' and suggest 'Finting'. Real top players' identities and highly recognizable lookalikes are legal risk; no fake near-real identity without review/licensing.
- Player says 'no expense at all': final Google Play public release costs and eligible account requirement differ from building/sideloading. Roadmap excludes paid publication.
- No engine-side cloud analytics assumption may be claimed without dependency/permission audit.

## Persetujuan penataan dokumen — 2026-11-01
- Pemilik menyetujui penambahan PRD, ARCHITECTURE, DESIGN, TESTING, WORKFLOW, ERROR_LOG; rename Agent Rules menjadi AI_RULES; satu Decision Register; serta pembaruan indeks dan workflow.
- Pemilik menegaskan: "total dari keseluruhan game adalah 1gb". Batas maksimal 1 GB berlaku keseluruhan game, bukan hanya paket APK. Lihat pengukuran di `TESTING.md`.
- Implementasi game belum dimulai atau diizinkan oleh sesi penataan ini. Seluruh hasil gameplay masih NOT_RUN.
- Persetujuan gameplay terbatas pada Modern15 dan kuota challenge yang diajukan. P-001, P-003 sampai P-007, P-009/P-010, P-012 sampai P-015 tetap berstatus sebelumnya. Tidak boleh mengunci kontrol atau desain career sepihak.
- Dampak dokumen: GDD, roadmap, AI_RULES, indeks, dan enam dokumen tambahan disinkronkan. Bukti tes game: belum ada.

## Izin implementasi M0–M1 — 2026-11-01
- ID: D-015. Request: "kerjakan sesuai dengan dokumen sampai milestone 1 aja", dilanjutkan "lanjut" atas rencana audit M0, build minimum, prototype M1 dan tes.
- Status: APPROVED oleh pemilik dalam chat. Dampak: implementasi lokal diizinkan sampai M1; bukan M2/career, cloud, biaya, atau penguncian kontrol final.
- Opsi/rationale: jalankan urutan M0 kemudian M1 sesuai dependensi; rollback backup + Git lokal, tanpa push.
- Tests: TESTING run M0-AUDIT-001. Gate M0 masih BLOCKED_LICENSE; izin tidak berarti acceptance lolos.

## Agent approval request template
**Decision ID:** ...
**Existing spec:** ...
**Why blocked:** ...
**Alternative A:** ... (consequences, cost, performance, legal risk)
**Alternative B:** ...
**Recommended for testing (not automatically chosen):** ...
**Evidence to show owner:** ...
**Owner approval/status:** PENDING
