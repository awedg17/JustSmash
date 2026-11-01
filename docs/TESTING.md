# Just Smash — Testing

Versi 0.2 | 2026-11-01 | Rencana tes dan ledger hasil aktual dalam satu dokumen.

## Status dan aturan bukti

- `NOT_RUN`: belum dijalankan. `PASS`: expected cocok hasil eksekusi dengan bukti. `FAIL`: expected tidak terpenuhi. `BLOCKED`: tidak bisa dijalankan, tulis blocker spesifik.
- Sesi penataan dokumen tidak menjalankan Unity, build APK, emulator, atau gameplay. Seluruh tes game di bawah NOT_RUN.
- Build sukses hanya membuktikan compilation/packaging. Editor/emulator tidak membuktikan performance perangkat fisik RAM 4 GB.
- Jalankan tes runnable untuk logika nontrivial, regression untuk setiap fix, lalu smoke/integrasi pada entrypoint nyata. Framework bawaan Unity digunakan bila tersedia; jangan menambah dependency tanpa kebutuhan.
- Catat error persis dan maksimal 3 percobaan auto-fix per kegagalan yang sama. Sesudah itu hentikan scope gagal, laporkan dan tunggu arahan. Catat percobaan di ERROR_LOG, bukan menimpa kegagalan dengan PASS palsu.

## Matriks acceptance

Setiap baris wajib dipecah menjadi case terukur saat milestone berjalan. Jangan PASS satu kelompok bila baru satu subcase diuji. Detail task tetap di roadmap, dan seluruh subcase wajib terkait task terkait.

| Test | PRD | Task/scope | Skenario dan expected | Status |
|---|---|---|---|---|
| T-01 | R-01 | M0-01..20 | Audit toolchain/lisensi; build development; buka landscape, exit/relaunch, scene load tanpa exception; jalankan offline | NOT_RUN |
| T-02 | R-02 | M1-08..10,16..17 | A/B/assisted pada high lob, smash approach, near-net; log input/latency/miss; pemilik menilai lalu memilih, agent tidak auto-lock | NOT_RUN |
| T-03 | R-03 | M1-06..07,11..14; M2-05 | Lima stroke, straight/cross, charge/release, jump valid/invalid, backhand; Excellent/Good/Fair/Poor dan miss/net/out sesuai posisi/timing, tanpa teleport | NOT_RUN |
| T-04 | R-04 | M1-01..07,15; M2-08 | Drag/base-first, net/ground/out/contact; kecepatan tinggi tidak tunneling; frame render berbeda tidak mengubah aturan; kamera jump/change-ends tetap terbaca | NOT_RUN |
| T-05 | R-05 | M2-01..04 | Quick16: 16–14 menang, 16–15 lanjut, 17–15 menang, 19–19 lanjut, 20–19 menang. Classic21: 21–19 menang, 21–20 lanjut, 22–20 menang, 29–29 lanjut, 30–29 menang. Modern15: 15–13 menang, 15–14 lanjut, 16–14 menang, 20–20 lanjut, 21–20 menang. Ulang untuk kedua pemain; best-of-3, service parity, let/fault dan change-ends sesuai aturan disetujui | NOT_RUN |
| T-06 | R-06 | M2-09..13; M4-07; M5-09,13 | Exhibition sampai hasil; pause freeze, save/resume mempertahankan state; skip/sim memberi statistik dan reward sekali; challenge IN/OUT gagal mengurangi kuota dari 2, sukses tidak, reset antar-game; replay trajectory asli dan toggle/skip berfungsi | NOT_RUN |
| T-07 | R-07 | M2-06..07; M3-01..08 | Reli 60 detik normal dibanding smash/sprint/jump; recovery antarset parsial; easy/normal/hard/adaptive pada atribut sama; AI hanya mengamati aksi terlihat, bukan input masa depan; trace alasan keputusan | NOT_RUN |
| T-08 | R-08 | M4-01..16 | Seed sama mereproduksi hasil dalam environment tercatat; world >10 tahun, ID unik, aging/regenerasi, expiry/ranking, entry deadline/conflict, draw frozen, payout sekali. Detail ranking/usia dan jumlah atlet harus disetujui sebelum fixture final | NOT_RUN |
| T-09 | R-09 | M5-01..13; M4-14 | Slot independen; create/load/delete; training/coach/progression; atribut 1–100, overall floor cap99; umur <20 tidak retire, >=20 konfirmasi retire; archive read-only; statistik tetap konsisten | NOT_RUN |
| T-10 | R-10 | M6-01..07 | AI partner, hit ownership/position, service/receive rotation, pergantian partner, ranking pair, camera empat badan; save tetap valid; bukan dua clone tanpa mekanik | NOT_RUN |
| T-11 | R-11 | M7-01..08 | Ledger earned money, reward tidak ganda, tidak hutang/game over finansial; opsi gratis/skip penting; kontrak/sponsor/news/offcourt bereaksi konsisten; semua equipment kosmetik tidak mengubah stat | NOT_RUN |
| T-12 | R-12 | M1 visual/input; M2-12; M8-01 | 16:9/19.5:9/20:9, notch/safe area, teks/touch/kontras, label grade, seluruh toggle; UI turquoise sesuai referensi yang telah direview; no music/commentator; SFX event sah dan berlisensi | NOT_RUN |
| T-13 | R-13 | M0-14,17; M2-15; M6-06; M8-02 | Ukur frame time/FPS/1% low, RAM peak, thermal pada singles/doubles fisik qualifying 4GB; total game <=1 GB dengan protokol ukuran; catat durasi/skenario dan konfigurasi | NOT_RUN |
| T-14 | R-14 | M0-13; M8-03..10 | Audit seluruh aset/package/permission/endpoint; runtime offline, tidak telemetry tak disetujui; save interrupted/corrupt/migration recovery tidak kehilangan backup; Terms/Privacy sesuai implementasi; hash/signature APK dan izin release | NOT_RUN |

## Batas keseluruhan game 1 GB

Persetujuan pemilik: total keseluruhan game maksimal 1 GB, bukan APK saja. Catat bytes mentah dan tampilkan GB desimal (1 GB = 1.000.000.000 bytes); jangan menyamakan 1 GiB dengan 1 GB.

Pengukuran wajib memisahkan:

1. Paket distribusi lengkap: APK/split/paket aset jika ada; bukan hanya file APK terkecil.
2. Footprint terpasang: kode/native libraries dan seluruh aset bawaan/ekstraksi; tidak menghitung file yang sama dua kali.
3. Data/save/cache aktual sesudah clean install, skenario pertandingan, dan stress career >10 tahun; catat jumlah slot/atlet/durasi skenario.
4. Total footprint game pada skenario: instalasi + data/cache. Jangan menjumlahkan lagi salinan paket distribusi di folder unduhan ke footprint instalasi.

Paket lengkap dan total footprint skenario harus dilaporkan terhadap batas 1 GB; paket kecil tidak meluluskan instalasi >1 GB. Ukuran folder project Unity/Library/build-tool di PC bukan ukuran game pemain. Bukti metode ukur di perangkat diperlukan; hasil estimasi asset saja bukan PASS.

Jumlah slot dan panjang career tidak dibatasi numerik oleh GDD. Tes finite tidak membuktikan ukuran selama semua kemungkinan penggunaan; jika data tak terbatas mengancam batas, catat blocker desain dan minta keputusan tanpa menghapus save atau menetapkan cap sepihak.

## Protokol performance

Catat model perangkat, Android, RAM perangkat, Unity/build/config, resolusi/quality, hash APK, durasi, FPS/frame-time distribution, 1% low, RAM peak dan thermal. Target 30 FPS bukan klaim minimum seluruh frame. Ambang toleransi drop/pacing belum numerik; laporkan data nyata dan minta keputusan bila perlu, jangan membuat angka kelulusan sendiri. Sesi 5/15/30 menit mengikuti M3-06; singles/doubles long match mengikuti M8-02.

## Template hasil eksekusi

| Field | Isi yang harus dicatat |
|---|---|
| Run ID / waktu / pelaksana | Identitas run nyata |
| Test ID / subcase / requirement / task | Traceability lengkap |
| Commit atau source snapshot / APK SHA-256 | Build yang benar-benar diuji |
| Environment / perangkat / seed | Reproduksi |
| Command atau langkah UI | Langkah aktual, bukan command karangan |
| Expected / actual | Nilai dan perilaku yang dibandingkan |
| Status | PASS / FAIL / BLOCKED / NOT_RUN |
| Evidence path | Log, test report, screenshot/video atau artifact nyata |
| Bug / batas cakupan | ID ERROR_LOG dan bagian yang belum diuji |

## Run M0-AUDIT-001 — 2026-11-01

- Otorisasi: pemilik meminta implementasi sampai M1, lalu menyetujui rencana dengan `lanjut`.
- Environment: Windows 11, Unity 6000.3.18f1 (5ebeb53e4c07). AndroidPlayer, SDK, NDK dan OpenJDK bawaan tersedia.
- T-01 / M0-01: PASS audit awal; root hanya docs dan backup sebelum sesi. Git lokal diinisialisasi pada branch `feature/m0-m1`; tanpa remote/push.
- T-01 / M0-03, M0-06, M0-17: BLOCKED_LICENSE. Eksekusi nyata Unity `-batchmode -quit -createProject "C:/Users/Dewangga Ramadhan H/Documents/Portofolio/JustSmash/Game"` gagal sebelum project terbentuk. Log: `evidence/M0/create-project.log`, baris 94–95: `No valid Unity Editor license found. Please activate your license.` / return code 198.
- M0-17 / perangkat: BLOCKED_DEVICE. `SDK/platform-tools/adb.exe devices -l` mengembalikan daftar kosong.
- APK, scene smoke, edit/play-mode, offline runtime, FPS, RAM dan footprint game: NOT_RUN; belum ada build. M0 gate belum lolos, M1 belum dimulai.
- Tidak mencoba bypass lisensi, mengganti engine, atau membuat project/stub palsu. Aktivasi Unity oleh pemilik diperlukan sebelum retry.

Pemeriksaan konsistensi Markdown sebelumnya tetap terpisah dalam WORKFLOW; bukan bukti gameplay.
