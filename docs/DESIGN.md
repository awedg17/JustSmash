# Just Smash — Design dan UX

Versi 0.2 | 2026-11-01 | Turunan GDD §2/4/5/9; bukan persetujuan mockup baru.

## Arah visual yang dipertahankan

- Android landscape, 3D stylized ringan; Valorant hanya referensi karakter visual, bukan sumber model, rig, texture, UI atau audio.
- Kamera belakang bertinggi sedang, pemain dekat bawah layar, net/lawan di atas; jump/zoom-out menjaga subjek dan area relevan terbaca.
- UI turquoise/cyan, abu, tombol simpel; judul sporty Just Smash. Jangan mengganti ke palette proyek lain.
- Warna kualitas: Excellent ungu, Good hijau, Fair kuning, Poor merah. Label/indikator selain warna tetap diperlukan untuk keterbacaan.
- Tidak mengunci hex, font, spacing, aset atau layout final yang belum diberikan pemilik. Semua font/aset memerlukan provenance dan lisensi.

## Status referensi

GDD menyebut gambar kamera Badminton Jump Smash dan Minimalist Game GUI. File gambar referensi tersebut belum ada dalam enam dokumen sumber; screenshot File Explorer bukan referensi desain game. Catat URL/path dan lakukan review ketika file asli tersedia, sebelum mengklaim kesetiaan visual. Jangan mengarang token dari nama referensi.

Video audio di GDD §9 merupakan referensi observasi; belum ditonton/ditandai timestamp dalam sesi penataan ini. Audio tetap tidak boleh diambil dari siaran/video tanpa hak.

## Prototype sebelum polish

M0 membuktikan build/scene minimum. M1 berisi satu lapangan, dua karakter placeholder original, shuttle dan debug contact; bukan career dashboard final. Kontrol A dan B diuji pada scene serta skenario yang sama. Assisted tersedia sebagai footwork bantuan, bukan auto-hit.

| Prototype | Gerak | Pukulan/aim | Gate |
|---|---|---|---|
| A | Joystick ibu jari kiri | Tangan kanan memilih shot/aim, charge dan release | Ukur konflik jump/aim/charge, reach dan latency |
| B | Drag/swipe menuju target | Timing pukulan tetap dari pemain | Uji kontrol simultan vs bergantian dan keterlambatan |
| Assisted | Navigasi dibantu AI | Timing, power, jump, jenis dan aim tetap pengguna | Tidak teleport atau menjamin kualitas pukulan |

Pemilik memilih kontrol sesudah mencoba atau melihat bukti. Menyetujui Modern15/challenge tidak mengunci joystick/swipe.

## HUD pertandingan

Tampilkan skor/game, charge/contact window dan feedback hasil secara terbaca tanpa menutup shuttle/contact. Jenis pukulan tetap smash, clear/lob, drop, net, lift; backhand varian teknik, bukan kewajiban tombol tambahan. Straight/cross dan kedalaman/power harus terbaca. Jumlah/penempatan tombol final menunggu uji ergonomi.

Pause, pengaturan kontrol, replay toggle/skip harus dapat diakses. Fewer buttons bukan alasan menghapus toggle wajib. Jangan membuat tombol dekoratif atau action tanpa fungsi. Konfirmasi jelas untuk tindakan destruktif seperti delete save dan pensiun; jangan menutupi konsekuensi.

## Kamera, animasi, visibility

- Transisi change-ends mempertahankan POV belakang pemain dan geometri/arah pertandingan yang benar.
- Trail/comet/shadow shuttle tipis untuk visibility, tidak menyembunyikan kedalaman atau mengubah flight.
- Contact frame, racket collider, stroke dan animasi sinkron; overhead/net/lift punya contact window berbeda.
- Tidak ada perpindahan mendadak pemain demi menyelamatkan hit. Animasi natural bukan alasan melewati validasi physics.
- Replay berasal dari kejadian aktual, dapat skip/matikan; angle umpire opsional mengikuti milestone.

## Career dan content, setelah gate gameplay

Career home menampilkan next match, kalender, news, kondisi, ranking dan earnings; scene/menu dibuat ketika M5 berjalan. UI entry menyebut accepted/rejected beserta alasan/deadline; calendar conflict tidak disembunyikan. Offcourt menggunakan kartu teks ringan, bukan perluasan scene 3D tanpa izin. Cosmetic preview tidak boleh menjanjikan boost stat.

## Audio

Tanpa musik dan komentator. SFX racket/shuttle, net, footwork/shoes, fall/court impact, whistle dan crowd mengikuti GDD. Sorakan dibuat original atau berlisensi; tidak rip audio. Mixer dan kontrol suara mengikuti scope M8. Event suara harus berasal dari event gameplay sah, bukan double trigger akibat render frame.

## Acceptance visual dan aksesibilitas

- Uji landscape 16:9, 19.5:9, 20:9; safe area/notch, teks tidak terpotong, thumb reach, target sentuh dan kontras.
- Uji receiving high lob, smash approach, near-net pada A/B; catat input terlewat, salah gesture dan rasa terlambat.
- Grade dan sebab miss/net/out terbaca tanpa hanya mengandalkan warna.
- Simpan screenshot/video build nyata dan device/build identity. Mockup bukan bukti fitur bekerja.
- Pemilik menilai fidelity/rasa bermain; agent menjalankan build, pengujian dan pengumpulan bukti. Rencana dan hasil ada di TESTING.
