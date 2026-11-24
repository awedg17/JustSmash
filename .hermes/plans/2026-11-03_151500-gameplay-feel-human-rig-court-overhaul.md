# Plan Perbaikan Gameplay, Fisik/Animasi Manusiawi, dan Visual Lapangan

**Target File Plan:** `.hermes/plans/2026-11-03_151500-gameplay-feel-human-rig-court-overhaul.md`  
**Status Sesi:** `PLAN MODE ONLY` (hanya dokumen rencana, tanpa ubah kode/file proyek/build/device).

---

## 1. Goal
Memperbaiki gameplay bulu tangkis agar terasa wajar dan nyaman dimainkan di HP (joystick standar game mobile, stamina hanya untuk aksi pukul/lompat, penyelamatan darurat sambil jatuh, artikulasi tubuh dua sisi forehand/backhand), membersihkan UI penguji yang mengganggu, menghapus pencahayaan aneh pada lapangan, serta memperjelas jejak kok untuk ketepatan timing.

---

## 2. Current Context & Constraints
1. **Aturan Kerja Pemilik:**
   - Tanpa spekulasi: semua perubahan berbasis bukti dan pengujian nyata.
   - Sesi ini murni membuat rencana, tidak boleh menulis kode game atau mengeksekusi build.
   - Tanpa ketergantungan berbayar (Rp0); aset berhak cipta tidak boleh sembarangan dimasukkan.
2. **Kondisi Kode Saat Ini:**
   - **Joystick:** Menggunakan kotak statis di pojok kiri bawah (`StickRect` 190x190), pembacaan titik sentuh masih statis dari titik tengah kotak sehingga tidak terasa lentur seperti joystick mengambang di ML/eFootball/FC Mobile.
   - **Stamina:** Pada update terakhir, jalan masih menghabiskan stamina (`moveDrain`). Pemilik ingin stamina **hanya berkurang saat memukul dan melompat**, bukan saat berlari/berjalan biasa.
   - **Menu Tester:** Terdapat tombol `Tester / VFX` dan jendela form scrollable lokal yang tidak dibutuhkan pemilik karena feedback diberikan langsung di percakapan.
   - **Karakter & Raket:** Karakter masih susunan objek primitif tanpa sendi siku/pergelangan yang wajar, raket terikat kaku di satu sisi sehingga terlihat tidak masuk akal saat memukul sisi kiri (backhand), jalan hanya memutar paha sederhana, dan belum ada mekanisme penyelamatan kok darurat (dive/lunge-save sambil jatuh lalu bangun).
   - **Jejak Kok (Trail):** Nilai awal `trail.time = 0.22f` dan warna putih transparan 0.35 tipis, membuat pemain sulit membaca kecepatan dan posisi puncak kok.
   - **Pencahayaan Lapangan:** Ada `CourtLight` berupa Directional Light miring `(50, -20, 0)` tanpa bayangan lembut atau tata cahaya gedung badminton indoor, menghasilkan bayangan dan kilau aneh pada lapangan hijau polos.

---

## 3. Architecture & Proposed Approach
Perbaikan dibagi menjadi dua lapisan yang saling mengunci:
1. **Lapisan Logika Permainan (Gameplay Engine - C#):**
   - Mengubah skema joystick menjadi *floating dynamic joystick* (muncul dan mengikuti titik sentuhan pertama jari kiri di area layar kiri) dengan zona mati dan batas radius wajar.
   - Menghentikan pengurangan energi pada pergerakan kaki; konsumsi energi hanya berjalan di pemanggilan pukulan dan lompatan.
   - Menambahkan status pergerakan darurat `EmergencyDive`: terpicu otomatis saat tombol pukulan ditekan ketika jarak kok berada di luar jangkauan lunge biasa (rendah dekat lantai), menghasilkan kualitas pukulan maksimal `Good` (tidak bisa `Excellent`), memicu animasi jatuh meluncur, dan membutuhkan jeda bangun kembali (*recovery to stance*).
   - Memperbarui sistem tangan dan raket agar secara visual berpindah luwes antara forehand dan backhand tergantung posisi kok relatif terhadap badan.
2. **Lapisan Visual & Antarmuka (Presentation & Audio):**
   - Menghapus tombol dan jendela `DrawTester()` dari layar permainan agar pandangan bersih.
   - Menghapus directional light acak; menggantinya dengan pencahayaan indoor yang merata (ambient netral + light lapangan atas yang simetris) atau menonaktifkannya sementara sampai model gedung indoor dibuat proper.
   - Mengatur jejak kok (*shuttle trail*) lebih tebal dan kontras dengan gradasi kepala-ekor yang jelas untuk membantu pembacaan timing.

---

## 4. Urutan Penanganan: Gameplay vs Grafik & Fisika

Sesuai arahan pemilik, urutan pengerjaan dibagi tegas agar fondasi permainan kokoh sebelum visual dipoles:

```
[Tahap 1: Logika Gameplay Inti] 
       ↓ 
[Tahap 2: Fisika Interaksi & Deteksi Jangkauan]
       ↓ 
[Tahap 3: Artikulasi Gerak Manusia (Rig & Animasi)]
       ↓ 
[Tahap 4: Kebersihan Layar & Tata Cahaya Lapangan]
       ↓ 
[Tahap 5: Pengujian Terpadu & Build Android]
```

---

## 5. Step-by-Step Implementation Tasks

### TAHAP 1 — KONTROL JOYSTICK & ATURAN STAMINA

#### Task 1.1: Ubah Joystick ke Skema Mengambang (Floating Joystick)
- **Target File:** `Assets/Scripts/CourtPrototype.cs`
- **Konsep:** Area kiri layar (0 s.d. 45% lebar layar) menjadi zona sentuh fleksibel. Saat jempol kiri menyentuh layar pertama kali, titik tengah joystick bergeser ke koordinat tersebut. Saat jempol digeser, arah dihitung relatif dari titik sentuh awal hingga radius maksimal 75px. Saat jempol diangkat, joystick kembali netral.
- **TDD / Tes:**
  - File: `Assets/Tests/PlayMode/FeedbackTests.cs`
  - Buat tes `FloatingJoystickCentersOnTouchDownAndClampsVector()`: pastikan analog membaca 1.0 ke arah geser tanpa tergantung koordinat kotak kaku lama.

#### Task 1.2: Hentikan Pengurangan Stamina Saat Bergerak
- **Target File:** `Assets/Scripts/CourtPrototype.cs` (dalam fungsi `FixedUpdate`)
- **Perubahan:** Hapus baris pengurangan `Energy -= distance * BootDiagnostics.Current.moveDrain`. Pastikan stamina hanya berkurang pada:
  1. `Jump()` -> berkurang sebesar `jumpCost`
  2. `TapShot()` / `ReleaseShot()` -> berkurang sesuai bobot jenis pukulan (smash paling besar, net paling kecil).
- **TDD / Tes:**
  - File: `Assets/Tests/PlayMode/FeedbackTests.cs`
  - Update tes `IdleAndBlockedMovementDoNotSpendWalkingEnergy` menjadi `WalkingAndSprintingDoNotSpendEnergyOnlyJumpAndHitsDo()`: uji pergerakan sejauh 5 meter di lapangan, buktikan nilai `Energy` tetap 100.

---

### TAHAP 2 — PENYELAMATAN JATUH (DIVE SAVE) & SISTEM TIMING

#### Task 2.1: Logika Penyelamatan Darurat Otomatis
- **Target File:** `Assets/Scripts/CourtPrototype.cs` & `Assets/Scripts/CourtMotion.cs`
- **Aturan Main:**
  - Saat pemain menekan tombol pukul (misal Lift / Net) dan kok berada pada jarak 1.1m s.d. 1.8m dari badan dengan ketinggian di bawah 0.6m dari lantai:
    - Status gerak berubah menjadi `DiveSave`.
    - Pemain meluncur maju/menyamping menjangkau kok.
    - Nilai evaluasi pukulan dibatasi: **Maksimum `Good`**, tidak pernah `Excellent`.
    - Pemain tidak bisa langsung bergerak selama jeda jatuh dan bangun (`recoveryTimer` sekitar 0.45 detik).
- **TDD / Tes:**
  - File: `Assets/Tests/PlayMode/FeedbackTests.cs`
  - Buat tes `LowDistantShuttleTriggersDiveSaveWithCappedGradeAndRecovery()`: beri kok rendah berjarak 1.4m, tekan pukul, verifikasi status menjadi `DiveSave`, grade adalah `Good`, dan pemain memasuki masa pemulihan bangun.

#### Task 2.2: Peningkatan Keterbacaan Jejak Kok (Trail & Shadow)
- **Target File:** `Assets/Scripts/CourtPrototype.cs` & `Assets/Scripts/CourtDiagnostics.cs`
- **Perubahan:**
  - Ubah parameter awal `TrailRenderer`: waktu diperpanjang dari 0.22s menjadi 0.38s, lebar awal dinaikkan ke 0.12m, warna putih bergradasi kuning cerah dengan opacity 0.75 pada pangkal.
  - Bayangan kok di lantai diperbesar sedikit dan diatur agar ketebalan kontrasnya mengikuti tinggi jatuh bebas kok sehingga mudah membedakan kok di atas net vs kok yang menukik cepat.
- **TDD / Tes:**
  - Verifikasi PlayMode memastikan komponen `TrailRenderer` memiliki `startWidth >= 0.1f` dan `time >= 0.35f`.

---

### TAHAP 3 — PERBAIKAN FISIK KARAKTER: DUA SISI & SENDI

#### Task 3.1: Penanganan Raket Forehand vs Backhand
- **Target File:** `Assets/Scripts/CourtPrototype.cs` & `Assets/Scripts/CourtMotion.cs`
- **Konsep:**
  - Hitung posisi relatif kok terhadap arah hadap pemain:
    - Jika kok di sebelah kanan tubuh (koordinat lokal X > 0): gunakan bahu kanan dan sudut ayunan forehand.
    - Jika kok di sebelah kiri tubuh (koordinat lokal X < 0): bawa lengan menyilang dada ke sisi kiri tubuh dengan orientasi kepala raket menghadap kok (posisi backhand realistis).
- **TDD / Tes:**
  - Buat tes `BackhandReachPositionsRacketOnLeftSideOfTorso()`: verifikasi posisi lokal raket bernilai negatif pada sumbu X ketika kok berada di sisi kiri tubuh.

#### Task 3.2: Penyesuaian Gerakan Tubuh (Torso, Kaki, dan Lengan)
- **Target File:** `Assets/Scripts/CourtMotion.cs`
- **Konsep:**
  - Tambahkan sendi siku dan tekukan lutut yang sinkron dengan arah gerak (tidak hanya ayunan kaku depan-belakang).
  - Saat posisi jatuh penyelamatan (`DiveSave`), badan miring sejajar lantai, satu tangan menahan lantai dan tangan lain menjulurkan raket, lalu bertransisi berdiri kembali ke posisi siap.

---

### TAHAP 4 — PEMBERSIHAN UI TESTER & PENATAAN CAHAYA

#### Task 4.1: Hapus UI Tester Lokal dari Layar Permainan
- **Target File:** `Assets/Scripts/CourtDiagnostics.cs` & `Assets/Scripts/CourtPrototype.cs`
- **Perubahan:**
  - Hapus tombol `"Tester / VFX"` dari fungsi `DrawTester()`.
  - Nonaktifkan tampilan form scrollable report penguji di layar rilis permainan. Fitur pencatatan internal tetap ada jika dipanggil via kode/shortcut khusus editor, tetapi bersih total dari pandangan pemain di layar HP.
- **TDD / Tes:**
  - Buat tes verifikasi bahwa pada mode gameplay biasa tidak ada tombol tester yang merespons sentuhan atau menutupi layar.

#### Task 4.2: Koreksi Tata Cahaya Lapangan (Lighting Realistis)
- **Target File:** `Assets/Scripts/CourtPrototype.cs` (fungsi `BuildCourt`)
- **Perubahan:**
  - Hapus directional light miring `(50, -20, 0)` yang menimbulkan bayangan outdoor aneh pada lapangan badminton tertutup.
  - Gunakan pencahayaan ambient netral lembut yang merata di seluruh lapangan badminton, atau dua titik lampu atas simulasi lampu gedung olahraga (top-down spotlight/directional dengan sudut 80-90 derajat) agar bayangan jatuh tepat di bawah pemain dan net tanpa silau aneh.
- **TDD / Tes:**
  - Buat tes PlayMode untuk memastikan tidak ada directional light dengan rotasi miring liar yang aktif di scene.

---

## 6. Review & Validasi

1. **Uji Coba PlayMode Otomatis:**
   - Menjalankan seluruh PlayMode test suite (80+ tes) untuk memastikan penyesuaian joystick, stamina, dan dive save tidak merusak logika reli dan skor pertandingan.
2. **Uji Coba Visual & Rasa (Feel Check):**
   - Merekam cuplikan gameplay gerak joystick dinamis, penyelamatan kok jatuh, ayunan forehand/backhand, dan jejak kok baru.
   - Melaporkan hasil rekaman kepada pemilik untuk persetujuan rasa kontrol sebelum lanjut ke langkah berikutnya.

---

## 7. Risiko & Catatan Penting
- **Batasan Model Karakter:** Selama belum ada model 3D manusia berlisensi gratis yang disepakati (gaya kartunis mirip Valorant), animasi bersendi ini dibangun dengan sistem prosedural hirarki objek agar gerakannya lentur dan manusiawi terlebih dahulu. Begitu model 3D siap, sistem gerak tinggal dipasangkan ke tulang (*rig*) karakter tersebut tanpa membongkar logika permainan lagi.
- **Konsistensi Skor & AI:** Gerakan jatuh (*dive save*) harus memiliki jeda bangun yang seimbang agar AI tingkat mudah tidak langsung mencetak poin murah, namun pemain tetap merasakan konsekuensi fisik karena terlambat mengantisipasi kok.
