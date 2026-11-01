# Just Smash — Architecture

Versi 0.2 | 2026-11-01 | Rancangan turunan GDD/roadmap; belum diimplementasikan.

## Keputusan teknis

Engine Unity disetujui (P-011), implementasi C#. Versi editor, URP/render pipeline, Input System, scripting backend dan SDK/NDK/OpenJDK belum diaudit; catat versi aktual sebelum build. Gunakan fitur bawaan atau dependency yang sudah tersedia sebelum menambah package. Semua pilihan wajib cocok dengan Rp0, offline, Android dan target performance.

Unity sebagai alat authoring/build tidak mengizinkan akun pemain, Unity Cloud, analytics atau runtime endpoint otomatis. Eligibility dan kebutuhan aktivasi/install harus diaudit, bukan diasumsikan sudah terpenuhi. Jangan mengganti engine jika toolchain gagal tanpa persetujuan.

## Batas tanggung jawab

Batas berikut logis, bukan perintah membuat satu folder/class/interface untuk setiap baris sekarang.

| Area | Tanggung jawab | Mulai ketika |
|---|---|---|
| Core/config/debug | Identitas stabil, config yang dipakai, seed dan logging terkontrol | M0 sesuai kebutuhan |
| Input | Input editor/touch menjadi aksi gameplay; eksperimen A/B/assisted terpisah | M0 minimal, M1 |
| Match/scoring | State pertandingan, servis, fault, skor, hasil | M2 |
| Movement/shuttle/animation/camera | Gerak fisik, contact window per stroke, visual sinkron | M1 |
| AI | Persepsi terbatas, taktik, difficulty dan partner | M2–M3, partner M6 |
| Persistence | Snapshot atomic/versioned/backup, migrasi, isolasi slot | Saat save digunakan M2/M4 |
| Career/simulation | Kalender, world sim headless, ranking, entry, progression | M4–M5 |
| Economy/content | Ledger dan reward satu kali, sponsor, cosmetic content | M7 dan kebutuhan reward sebelumnya |
| UI/audio | Menampilkan state/aksi, bukan menyimpan aturan pertandingan tersembunyi | Bertahap sesuai fitur |

Scene minimum M0 harus dapat load dan build; boot/menu placeholder/court/test scene dibuat hanya sejauh task memerlukan. Tidak membuat kerangka career sebelum gameplay gate lolos. Assembly definitions hanya jika ada pemisahan nyata.

## Aliran gameplay

```text
Touch/editor input
  -> action yang divalidasi terhadap state pertandingan
  -> gerak / charge / release / jump / stroke
  -> simulasi fixed timestep + contact evaluator
  -> hasil flight / landing / fault
  -> perubahan skor dan state
  -> HUD, SFX, replay evidence, snapshot saat diperlukan
```

Validasi state input wajib; tidak menerima stroke/jump di state ilegal. Physics tidak boleh bergantung frame rate render. Continuous collision untuk shuttle cepat perlu diuji, bukan diasumsikan menyelesaikan seluruh tunneling. Grade kualitas berasal dari timing, power, aim dan posisi; visual tidak memanipulasi physics.

## State pertandingan

Roadmap M2-01 menetapkan warmup, ready, serving, rally, point, interval, change ends, game-end, match-end. Detail transisi diimplementasikan dengan tes agar:

- Satu reli memberikan paling banyak satu hasil poin yang sah.
- Servis berikutnya mengikuti skor/server dan area yang benar.
- Deuce/cap tidak dapat membuat pertandingan berhenti terlalu dini atau tidak selesai.
- Change-ends mengubah sisi dunia tanpa membalik POV pengguna secara membingungkan.
- Pause menghentikan progres; resume memulihkan state konsisten.
- Load tidak memberi reward ulang atau mengulang event akhir pertandingan.

Angka change-ends Quick16/Modern15 masih usulan GDD §3; jangan dianggap otomatis disetujui bersama preset skor. Verifikasi aturan sebelum mengunci implementasi.

## Kontrol dan kamera

A/B tetap HARD GATE. Input dapat simultan sejauh skema yang diuji memungkinkan; jangan mengganti pilihan dengan autoplay. Assisted hanya footwork, bukan jaminan pukulan. Tidak auto-teleport ketika contact; IK/input window hanya sesuai approval. Kamera player-centric mempertahankan keterbacaan pemain, shuttle dan lapangan; metode teknis P-006 tetap provisional.

## Save dan integritas data

- Data local-only, tiap slot memakai ID stabil, bukan nama tampilan atau indeks enum.
- Format/version dan seed dicatat; schema final belum dipilih.
- Write ke temporary sibling, validasi, lalu replace atomic yang sesuai platform; simpan versi terakhir yang valid. Uji proses terputus sebelum, saat, dan setelah penulisan.
- Load memvalidasi ukuran/range/schema/version dan relasi ID. Save rusak tidak boleh menimpa backup valid; error ditampilkan, tidak dihapus diam-diam.
- Snapshot mid-match menyimpan state yang diperlukan untuk skor, servis, physics, RNG dan sistem terkait. Buktikan kontinuitas lewat tes, bukan klaim byte-identical lintas device yang belum diuji.
- Migrasi tidak menimpa satu-satunya salinan. Retired career read-only; delete dan pensiun memakai konfirmasi jelas.
- Reward/hasil memiliki identitas transaksi supaya retry/load tidak membayar dua kali.

## Career simulation

Data/simulasi terpisah dari scene 3D; pertandingan offscreen tidak merender pertandingan penuh. Seed dicatat untuk reproduksi. Kalender, entry/draw, expiry poin, aging dan population replenishment mengikuti aturan yang disetujui sebelum fixtures final dibuat. Target 1.000 atlet masih provisional dan harus diprofilkan; uji karier >10 tahun tidak boleh ditukar dengan demo beberapa tick.

## Replay dan challenge

Replay memakai event/transform ring buffer dan trajectory asli, bukan menjalankan ulang physics lalu mengklaim identik. Challenge IN/OUT memakai landing ground-truth, dua gagal/game, sukses tidak mengurangi kuota. Call awal deterministik untuk MVP; probabilitas kesalahan call membutuhkan persetujuan tersendiri. Jangan mengarang hasil challenge.

## Performance dan keamanan

Profil frame time, RAM, colliders, fixed timestep, thermal dan keseluruhan ukuran game. Hindari asset duplikat, shader/overdraw berlebihan dan log release tak terbatas. Batas ukuran mencakup game keseluruhan; metode ukur ada di TESTING. Semua permission, dependency, endpoint dan data keluar diaudit. Credential/signing key tidak ditulis dalam docs/log/source.

## Struktur rencana, bukan file yang sudah ada

```text
JustSmash/
  docs/                 spesifikasi dan ledger
  .backups/             snapshot dokumentasi sebelum revisi
  [Unity project]       lokasi ditetapkan saat audit M0
  [test/build evidence] dibuat saat pengujian sungguhan
```

Jangan init project, membuat Assets/Packages/ProjectSettings palsu, atau mengaku toolchain tersedia dari diagram ini. Audit implementasi menjadi langkah berikut setelah izin coding.
