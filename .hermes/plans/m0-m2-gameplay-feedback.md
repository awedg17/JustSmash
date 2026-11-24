# Rencana revisi gameplay M0–M2

Status: PLAN ONLY. Belum ada perubahan source, asset, build, atau pengujian runtime baru. Implementasi menunggu persetujuan rencana. Instruksi terbaru membatasi pekerjaan pada M0–M2; jangan lanjut M3 otomatis.

## Bukti dan batas audit
- Video pemilik: WhatsApp Video 2026-11-03 at 13.42.23.mp4, durasi 101.93 detik. Sampel frame memperlihatkan kontrol debug padat, karakter primitif, trail tidak terbaca jelas. Energy terbaca 100% pada 0–0 dan 21% pada 1–9. Ini bukan bukti tepat nol pada poin ke-11; keluhan tersebut dicatat sebagai laporan pemilik.
- Audio belum dinilai lewat pendengaran. CourtAudio.cs:8–30 membuat satu clip racket sintetis berbasis noise dan tone 900 Hz, bukan rekaman impact raket–shuttle.
- CourtPrototype.cs:340–343: TrailRenderer dan dua marker sudah dibuat; keberadaan komponen bukan bukti terlihat pada HP.
- CourtPrototype.cs:626–635: shadow mengikuti X/Z shuttle; landingShadow dihitung ulang tiap physics tick dengan estimasi yang tidak memakai integrator drag aktual. Rumus cabang naik juga tidak sesuai persamaan ballistic. Marker bukan titik landing stabil yang diminta.
- CourtMotion.cs:34–47: lunge dipilih setelah stroke Net, bukan berdasarkan kebutuhan reach saat mengejar. Kaki primitif diputar/digeser sebagai satu bagian. Ini belum memenuhi natural jointed movement.
- CourtPrototype.cs:581: drain Rally berjalan terus berdasarkan waktu + besar input analog, bukan perpindahan nyata. Assisted movement dihitung setelah drain, sehingga biaya tidak konsisten dengan gerak aktual.
- CourtPrototype.cs:733: Next point masih memerlukan klik. CourtFlow.cs menyimpan transisi set/interval/change ends yang harus tetap dihormati.

## Batas scope
Dalam scope: M1-06/07 animasi, footwork, lunge dan sinkronisasi kontak; M1-15 visibility; M2-01/04/09/10/11/13 flow pertandingan; M2-06 stamina; M2-12 audio. UI yang menghalangi tes termasuk penyelesaian kontrol M1 dan exhibition M2, bukan menunggu M3.

Perubahan kebutuhan: marker landing sesudah shuttle terpukul menggantikan perilaku marker lama. D-016/D-032 sebelumnya melarang aim/landing preview normal. Catat pengecualian baru secara eksplisit saat rencana disetujui: indikator lintasan shuttle yang SUDAH terbang diperbolehkan; preview target sebelum pukulan dan trajectory line tetap tidak ditambahkan.

Di luar scope, jangan dikerjakan:
- Model/animasi wasit 3D. Dokumen M0–M2 menetapkan aturan line judge, challenge dan sudut kamera umpire-side, bukan karakter wasit visual. Permintaan dicatat, tidak dianggap otomatis bagian M2.
- M3-07 tuning reli 60 detik, difficulty/adaptive AI, dan paket accessibility M3. Memperbaiki perhitungan jalan dan baseline M2 tetap dalam scope.
- Full biomechanical simulation/ragdoll atau overhaul seluruh model karakter. Multi-joint animation dasar untuk lunge dan pukulan tetap termasuk M1.

## Urutan pekerjaan yang diusulkan

### 1. UI minimum agar pengujian terbaca
- Buat preview layout sebelum mengubah source: joystick kiri, cluster shot/jump kanan, skor/stamina atas, area lapangan bersih.
- Simpan Feed/Tester dalam panel development, Save/Load/Restart dan pengaturan dalam menu pause. Tidak menghapus toggle, assist, replay, atau challenge.
- Pisahkan feedback pendek dari instruksi kontrol; label sesuai mode sentuh, bukan hanya keyboard.
- Uji safe area landscape, hit target dan multi-touch pada perangkat. Tidak wajib migrasi framework UI bila penataan minimal memadai.

### 2. Trail dan titik jatuh
- Audit render/material/kontras trail pada APK yang sama dengan source. Buat ekor pendek yang terbaca tanpa menutupi shuttle atau mengubah physics.
- Jadikan satu marker utama sebagai prediksi titik ground impact. Simulasikan salinan posisi/velocity dengan timestep, drag, gravity, net collision dan ground rule yang sama; jangan memakai target pukulan sebagai hasil landing.
- Hitung dan simpan setelah serve/hit. Marker tetap di koordinat dunia itu sepanjang lintasan tak berubah, bukan mengikuti X/Z kok.
- Perbarui hanya saat pukulan/contact mengubah lintasan; bersihkan pada akhir poin/reset, dan pulihkan konsisten pada save/load/replay. Pause tidak menggeser marker.
- Hindari dua penanda yang tampak sama namun bermakna berbeda. Tidak menambahkan aim preview sebelum pukulan.
- Acceptance: marker tidak melayang/merayap selama free flight; jarak ke landing aktual dilaporkan untuk tiap teknik, arah, net collision dan change ends. Ambang kelulusan numerik ditetapkan sebelum implementasi, bukan direkayasa setelah hasil.
- Titik landing adalah petunjuk jatuh, bukan posisi badan wajib. Raket masih harus bisa menjangkau kok sebelum ground contact.

### 3. Footwork, lunge, pukulan dan sendi
- Gunakan gerak aktual untuk idle/walk/run/side/back/brake/recovery. Tambahkan articulation pinggul–lutut–pergelangan kaki dan bahu–siku–pergelangan tangan pada visual minimal yang ada.
- Lunge dipicu arah gerak, jarak reach dan ketinggian shuttle, termasuk saat mengejar ke depan/samping/ujung, bukan semata stroke Net setelah hit.
- Tampilkan kaki tumpu, lutut menekuk, tubuh merendah/berputar, lengan menjangkau, lalu kembali seimbang. Berlaku untuk pemain dan Easy AI.
- Urutkan preparation/contact/follow-through/recovery. Sinkronkan pose raket, validasi contact dan perubahan velocity shuttle; pertahankan tap immediate serta jump hold-swipe yang disetujui.
- Tidak teleport, tidak memperpanjang reach tanpa batas, tidak auto-hit. Kok yang tidak terjangkau tetap miss.
- Acceptance: rekaman lob tinggi, net rendah, chase sudut, overhead, underhand, backhand dan jump; foot sliding/clipping diperiksa visual, bukan hanya nama state atau assertion transform.

### 4. Stamina berbasis aktivitas nyata
- Reproduksi pola video dengan durasi, jarak dan jumlah aksi tercatat. Jangan memakai jumlah poin sebagai satu-satunya patokan stamina.
- Pisahkan biaya jalan, gerak cepat, lunge, pukulan dan jump. Ukur gerak aktual setelah clamp/collision, berlaku sama pada manual dan assisted; analog tertahan di batas bukan jarak tempuh.
- Jalan lebih hemat daripada gerak cepat; smash/jump lebih berat. Nilai koefisien belum diputuskan, jangan mengarang target baru.
- Evaluasi idle Rally drain yang saat ini berjalan terus. Waktu state Rally saja tidak boleh dianggap aktivitas memukul/bergerak.
- Definisi baseline reli M2: saling mengembalikan kok sambil bergerak ke posisi pukul, bukan diam dan spam pukulan. Pertahankan baseline sekitar 30 detik lob D-019; jangan mengganti menjadi 60 detik M3.
- Pertahankan kapasitas stat70=100/stat80=110 dan recovery antargame 8/8.8 yang disetujui. Jangan menambah full recovery setiap poin atau menambah kapasitas sebagai penutup bug drain.
- Acceptance: idle, jalan saja, bergerak di batas, manual vs assisted pada jarak setara, reli lob sambil bergerak, smash/jump berulang, pause/replay dan antarset. Laporkan drain tiap komponen, jarak, durasi dan sisa energy.

### 5. Audio impact
- Audit event impact terhadap frame kontak yang valid; tidak ada hit sound pada miss atau event ganda.
- Buat calon suara impact yang mendekati raket–shuttle; bedakan kuat/lemah serta net shot/smash melalui envelope dan variasi terkontrol.
- Mulai dari prosedural yang ada. Jika tidak memenuhi hasil dengar, berhenti untuk memilih rekaman original/berlisensi jelas; tidak mengambil audio video/game lain tanpa izin.
- Acceptance: pemilik membandingkan sampel audio terisolasi dan mix gameplay pada HP. Tes event/clip saja tidak membuktikan realistis.

### 6. Otomatis lanjut setelah poin
- Ganti kebutuhan klik Next point dengan jeda singkat, lalu siapkan servis berikutnya otomatis. Durasi jeda menjadi parameter yang disetujui; bukan angka final dalam rencana ini.
- Tetap sediakan kesempatan challenge. Countdown berhenti saat pause, review/challenge dan replay; lanjut tepat sekali sesudah selesai.
- Akhir set, interval, change ends dan match-end tidak boleh dilewati dengan memanggil NextPoint mentah. Match selesai tetap di results; mulai match baru tidak otomatis.
- Auto-next bukan auto-serve pemain. Pemain tetap menentukan servis sesuai kontrol yang disetujui.
- Acceptance: poin normal/fault, challenge membalik keputusan, replay selesai/skip, pause/resume, save/load saat jeda, akhir set dan akhir match. Pastikan skor tidak tercatat ganda.

## Gate verifikasi dan dokumentasi
- Backup sebelum implementasi. Untuk tiap bug: repro/test gagal, diff minimum, test lulus; maksimal tiga percobaan per kegagalan sebelum lapor blocker.
- Sinkronkan PRD, ARCHITECTURE, AI_RULES bila aturan berubah, TESTING, DESIGN, WORKFLOW, Decision Register dan ERROR_LOG. Jangan menandai milestone selesai dari regression saja.
- Local regression + capture visual/audio + build APK source-bound dahulu. Instal/tes HP hanya setelah izin sesuai gate proyek.
- Acceptance fisik pemilik diperlukan untuk trail terbaca, penanda landing berguna, animasi mudah dibaca, audio masuk akal dan kontrol nyaman.
- Tidak commit/push, tidak menambah dependency atau asset tanpa persetujuan. Tidak mengimplementasikan wasit 3D atau M3 dalam rencana ini.
