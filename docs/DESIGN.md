## Desain aktif D-062 — natural badminton, arena tanpa penonton

Identitas D-057 biru/kuning/Anton dipertahankan; overhaul mengutamakan mesh/rig/motion dan kontak natural sebelum dekorasi. T1 arena inti tanpa spectator/tribun/crowd, ofisial tetap. T7e result menambah pemenang eksplisit, total game dan baris skor side0:side1 tiap game; legacy unknown ditulis Data lama tidak tersedia. Capture actual runtime sebelum/sesudah, bukan mockup pengganti. Skor numerik dan still bukan kelulusan gerak/audio. Status QA aktual di TESTING; gate aset tiga kandidat/nol eligible tetap.

### Skor gameplay terpasang — T6d

Panel skor (24,24,350,132) unit canvas,lebih pendek24unit dari baseline. Header GAME dan difficulty,kolom POIN,dua nama tim dan tally GAME dipisah. Energi tetap di bawah panel;latihan tetap panel70unit. Biru/kuning/Anton,menu,kontrol dan logika score tidak berubah. Capture64skenario/empat resolusi dan label deuce/game3 diperiksa;detail regresi di TESTING. Marker pemain aktif terpasang:ring kuning hollow32segmen,radius.36m,width.04m,groundY.018m,mengikuti X/Z pemain;disc cyan tetap saran posisi FormationTarget(0),bukan aimassist. Keduanya disembunyikan saatpause/replay/menu/result,resources dibersihkan saat owner dihancurkan. Still jump menunjukkan sedikit overlap ring/disc tetapi beda bentuk/warna;continuous-motion acceptance belum. Kontras energy/notice memakai mask glyph independen di QA;warna runtime tidak diubah.

### Kamera gameplay terpasang — T6b

Kamera lokal (.2,4,−14.2)m mengarah (.2,0,−1)m,rotasi mengikuti pergantian ujung;FOV48,damping6,jump offsets tetap. Ini kalibrasi desain D-062,bukan aturan olahraga. Framing lokal empat resolusi/dua format/dua ujung diuji,termasuk garis lapangan terhadap kontrol. Shuttle dekat tubuh/net masih perlu review keterbacaan;tidak mengklaim seluruh gerak atauAndroid lulus. Detail bukti di [[TESTING]].

## Identitas tetap D-057 — biru kartunis, lapangan hijau

Paket `blue-storyboard` disetujui dengan court hijau. Biru dominan, judul display Anton yang diizinkan, bidang diagonal, tombol utama kuning dan outline tegas; atlet besar di menu, cel shading aset NPC existing, tim biru/oranye. GOR sama, bukan scene palsu terpisah. Semua kontrol/settings tetap. Impact Windows tidak dikemas; Anton SIL OFL + copyright disertakan. Arah approved bukan kualitas akhir accepted; pilot motion D, review audio, runtime visual dan regresi tetap. D-052 PC-only/no-native-input tetap.

## Riwayat desain D-054 — referensi dahulu; tanpa career

Home hanya Main/Latihan/Pengaturan/Kredit; exhibition tunggal/ganda, kontrol/settings dan identity turquoise existing dipertahankan. Career serta seluruh tabnya dicabut. Shortlist screenshot nyata dan batas adaptasi di [UI_REFERENSI](UI_REFERENSI.md); rekomendasi Nintendo Switch Sports untuk keterbacaan, bukan izin menyalin. Tunggu pilihan arah/preview sebelum redesign. Bagian career di bawah riwayat, bukan UI yang masih tersedia.

## Riwayat desain — pengalaman D-053

Scope terbaru: Android offline landscape; identitas sporty turquoise JustSmash; alur Home → mode/difficulty → perkenalan pertandingan → servis/rally/jeda → hasil/rematch/menu. Tunggal dan ganda putra vs Easy/Normal/Hard; career terakhir setelah exhibition teruji. GOR turnamen lokal, wasit/kursi, penonton/tribun, papan skor arena dan reaksi saat poin. Kamera belakang pemain dan NPC Casual Set00 existing (kaos teal/coral, kulit/rambut tidak diwarnai). Tidak character creator baru/dependency baru/biaya/cloud/push. D-053 menggantikan larangan mode lama; D-052 PC-only tetap. `WORKFLOW.md` dihapus sesuai instruksi; hasil QA tetap di `TESTING.md`, bukan worklog pengganti. `AI_RULES.md` tidak diedit.

- Setup memakai dua baris pasangan Kamu+Dimas/lawan+Fajar pada ganda; HUD,warmup,result mengikuti identitas tim. Pemilihan mode mengaktifkan dua/empat atlet preview dan judul papan arena. Kamera change-ends bebas dekor belakang; Menu memulihkan pose Home.
- Career paling akhir pada Home; slot/buat career/kalender/peringkat/arsip/kabar dan partner memakai identitas turquoise existing. Partner dipilih dari atlet dunia, negara/umur/hubungan terlihat; pairing terkunci selama entry turnamen. Warmup/HUD memakai nama pasangan pilihan, bukan Dimas/Fajar exhibition. Partner UI89/89PASS dan still reviewed. Tab Latihan memakai target previous/next, value/cap/poin/biaya, intensitas, hari/fatigue/cedera, upgrade serta pilihan playstyle; risiko latihan dan reset/refund90% memakai modal Ya/Tidak existing. UI90/90PASS dan still reviewed. Tab Keuangan menampilkan saldo/coach/biaya fiktif, empat pilihan coach dengan none gratis, travel gratis/mandiri dan ledger dua baris per halaman; pengeluaran manual memakai konfirmasi. UI91/91PASS source1.138identik dan still reviewed tanpa overlap/clipping; bukan native/device atau redesign terpisah. Tab Kehidupan menampilkan popularitas/reputasi/prestige, penawaran dan deal sponsor/kontrak, tuntutan100/110/120/125/150%, konfirmasi teks, kegiatan keluarga/teman/komunitas/wawancara/negatif/skip gratis1hari. UI92/92PASS source1.140identik/still reviewed; notice bawah dekat batas panel belum dirapikan. Bonus kemenangan dan3pembayaran/90hari teruji terpisah dari UI; angka uang/profile fiktif dan provisional. Arsip memakai filter semua waktu/tahun ini/tahun lalu, statistik hasil/gelar/streak/uang dan6riwayat per halaman, bukan10terakhir; tersedia setelah pensiun read-only. UI93/93PASS source1.142identik; still reviewed tanpa clipping. Detail per laga menampilkan identitas/pasangan/lawan/hasil/game terakhir/reward/kredit dan bagian partner. Toggle Capaian/Riwayat laga memakai data persisted; belum ada skor game sebelumnya. UI94/94PASS source1.142identik; detail/capaian still reviewed tanpa clipping. Latihan menampilkan aturan senior tinjauan90hari, perlindungan latihan/laga terbaru dan pengecualian cedera; atribut fisik turun1 secara gradual, teknik tidak auto-turun. Edit43/43 + UI95/95PASS satu freeze1.142, still aging reviewed; bukan kelulusan motion/balance.
- Home, setup, HUD, pause, settings, credits, konfirmasi keluar/restart. Safe-area canvas; lima shot + jump; settings mempertahankan Manual/Assisted, Fixed/Floating, replay, volume, warna/brightness trail-shadow dan 30/60 FPS. Target FPS bukan FPS terukur.
- Manual, Fixed, Normal default. Intent satu pukulan maksimal 0.16s; input baru mengganti lama; expiry menjelaskan miss. Tap servis memakai power minimal 0.2; hold hingga 1.0. Cancel tidak menembak. Grade internal tidak mengubah flight dan tidak tampil di HUD normal.
- Shot memakai finite reach/panjang tulang tetap; tidak ada teleport rescue. Envelopes kedalaman Net 1–2m, Drop 1.6–2.8m, Smash 3.2–5.6m, Clear/Lift 5.2–6.1m; dihitung melalui integrator drag yang sama. Net-fault raket memakai segmen geometrinya, bukan culling bounds.
- Native in-place clips dan IK pada skeleton utama NPC; root tetap milik simulasi. World-space stance reset saat pindah posisi/ganti sisi; arah gerak lokal atlet. Jump fixed-step: puncak sekitar 0.6125m, flight sekitar 0.7s, landing/recovery terpisah. Tes transform/kontak tidak membuktikan kualitas gerak.
- AI seeded berbeda reaksi/observasi/error/taktik, kapasitas gerak 4.5m/s dan finite reach sama. Preset 15 poin, selisih dua, cap21, best-of-three. Default line judge akurat; legacy wrong-call/challenge tetap diuji secara eksplisit.
- Jalan/lari drain0; fatigue 0.7–1; hitting/jump memakai biaya existing. Poin menunggu otomatis 3s tanpa auto-serve pemain. Replay/snapshot menyimpan pose/clock/RNG serta posisi; data invalid ditolak sebelum mutasi.
- Hall sederhana built-in, satu key light overhead/front, soft shadow, trail pendek; tanpa ray tracing. Empat impact rekaman CC0 existing; SFX lain procedural baseline. Realisme audio belum diterima pemilik.
- Editor data hanya `.hermes/local-game-data`; Android data app sandbox. Harness host menyimpan/memulihkan byte preferensi per tes. APK baru `builds/JustSmash-Mobile.apk`, terpisah dari Production lama.

Gate berbeda: regresi host, inspeksi rekaman, packaging APK, input native, Android fisik/performa dan penerimaan pemilik. Input native pernah ditolak izin; tidak diulang/bypass. Akses HP butuh izin spesifik baru setelah APK siap. Bagian bertanggal lebih lama di bawah merupakan riwayat jika bertentangan dengan scope ini.

## Riwayat kebijakan — D-037..D-042, 2026-11-03
Instruksi owner terbaru menggantikan aturan lama yang bertentangan; bagian checkpoint/run lama adalah riwayat, bukan izin aktif. D-044 mengizinkan eksekusi M0–M2 sampai satu APK checkpoint baru; batas docs-only sebelumnya berakhir. Otorisasi implementasi tetap M0–M2; M3+ tidak dimulai otomatis, termasuk setelah acceptance M2. Tidak ada import aset/dependency, biaya, push, destructive reset, atau install/launch APK HP tanpa izin terpisah.

Review owner hanya pengalaman modul utuh: singles lengkap; character creation lengkap; career terintegrasi; final game. Ini kelompok review, bukan penambahan scope. Tidak meminta owner menguji pukulan/animasi/tombol/fault secara granular. Agent menangani unit/integration/regression, natural automated match, inspeksi video/motion/audio, dan performance sesuai izin. Keputusan desain tetap memerlukan approval, berbeda dari testing.

Subagent dan commit lokal bersyarat tetap D-038/D-039. D-043 menyetujui NPC Casual Set00 untuk evaluasi import, default joystick Fixed (Floating tetap pilihan), servis charge1detik/power0..1/feedback konsisten/cancel aman. Grade hanya Timing + Contact; Direction/Power trace tanpa kontribusi grade. Rally sekitar30detik menurunkan stamina cukup banyak dan mulai memengaruhi performa, tetapi belum habis dan rally bisa berlanjut; jalan/lari drain0. Bobot/formula grade, biaya/kurva/efek stamina baru belum approved. Unity Personal memenuhi kondisi pribadi non-klien tanpa pendapatan yang dinyatakan owner; tidak meminta pembahasan keuangan lagi kecuali kondisi/terms relevan berubah. Target karakter/animasi/atmosfer singles tetap. Approval evaluasi NPC bukan izin import aset lain/dependency/M3 atau device action. D-044 menggantikan batas docs-only; eksekusi approved sampai APK checkpoint, tanpa mengubah proposal angka menjadi approval.

Rujukan keputusan/proposal: [Decision Register](05_DECISION_REGISTER.md). Riset aset/lisensi: [Legal dan Assets](04_LEGAL_AND_ASSETS.md). Gate bukti: [Testing](TESTING.md).

# Just Smash — Design dan UX

## FEEDBACK-013 — presentation status
Recorded racket sample/mix is an owner-approved candidate, not accepted realism. Four licensed impacts with technique-specific gain; no added music/commentary. Existing IMGUI layout, trail visibility and primitive articulation remain unfinished. Planned UI: joystick left, shot/jump right, score/energy top and debug/menu separated; preview required before source change. Preserve all toggles. Post-hit landing marker is flight indicator, not pre-shot aiming aid.



## D-035 — control clarification
Technique buttons strike immediately on press. Keep jump hold-swipe guide and release-to-strike path. Explain Excellent/Good/Fair/Poor with timing/direction/power/position; do not promise a winning shot. Energy HUD displays fraction of stat-dependent capacity, not raw capacity units as percent. UI fidelity remains pending.



## CLOSURE-011 — otoritas dan penutupan aktif
Instruksi pemilik terbaru mengizinkan audit dan penutupan teknis lokal seluruh M0–M2, tanpa approval ulang perubahan teknis dalam scope. D-031 menunda review manual; D-034 menyetujui world-space change-ends/player-centric camera serta 3% salah call pada landing eligible dalam 5 cm, maksimum 1/match. D-016 menetapkan joystick/manual+assisted, bukan eksperimen swipe A/B. D-017 menetapkan Official15 saja; Classic21 terkunci sampai career2028, Quick16 dihapus.

Requirement dan acceptance di bawah tetap berlaku kecuali digantikan keputusan APPROVED lebih baru. Hanya status checkpoint lama yang merupakan **riwayat**, bukan status aktif. Pernyataan lama M2 dibekukan, P-006 belum disetujui, Quick16 default, belum ada build, atau lisensi aktivasi terblokir tidak berlaku lagi. Status per task dan closure plan aktif: `evidence/M2/closure-task-audit.json`; hasil baru dicatat di TESTING. Izin aktif D-037..D-042: commit lokal boleh dengan audit; HP install/launch memerlukan izin terpisah; push, dependency baru, biaya dan M3 tetap dilarang.

Pisahkan Implementation Complete, Local QA Passed, Device QA Pending, Owner Acceptance Pending, dan Milestone Fully Verified. Task canceled M1-09 dianggap requirement superseded terpenuhi, bukan fitur swipe yang diimplementasikan. Device QA pending bukan otomatis BLOCKED_ENV tanpa kegagalan lingkungan konkret.


## M1 D-032 local additions
- Tester / VFX pauses gameplay and hides HUD; scrollable form stores local reports. Brightness/color controls preserve physics. Debug OFF default, F3/context menu/development panel. Procedural primitive poses remain placeholder; natural-animation acceptance pending.

## Riwayat scope — 2026-11-02
Pemilik mengizinkan implementasi seluruh scope sampai M1, tidak M2+. D-016–D-028 dan instruksi chat terbaru menggantikan proposal lama: joystick kiri untuk gerak sekaligus arah pukulan, enam sektor tanpa landing/trajectory preview; jump tombol hold-swipe dengan guide teknik; assisted menggerakkan atlet sepenuhnya tanpa input movement. Default Official15, Classic21 Settings terbuka saat career 2028; Quick16 dihapus. Scoring penuh, challenge, stamina match, career, sponsor dan doubles tetap milestone sesudah M1. A/B swipe movement dibatalkan; gate M1 menjadi joystick/manual+assisted pada high lob, smash approach, near-net. Uji AVD Android resmi untuk runtime; HP hanya untuk feel multitouch dan performa perangkat nyata. Status historis di bawah bukan status terkini.


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

M0 membuktikan build/scene minimum. M1 berisi satu lapangan, dua karakter placeholder original, shuttle dan debug contact; bukan career dashboard final. Eksperimen A/B dibatalkan D-016; tabel di bawah adalah riwayat, bukan acceptance aktif. Assisted tersedia sebagai footwork bantuan, bukan auto-hit.

| Prototype | Gerak | Pukulan/aim | Gate |
|---|---|---|---|
| A | Joystick ibu jari kiri | Tangan kanan memilih shot/aim, charge dan release | Ukur konflik jump/aim/charge, reach dan latency |
| B | Drag/swipe menuju target | Timing pukulan tetap dari pemain | Uji kontrol simultan vs bergantian dan keterlambatan |
| Assisted | Navigasi dibantu AI | Timing, power, jump, jenis dan aim tetap pengguna | Tidak teleport atau menjamin kualitas pukulan |

Joystick telah dipilih D-016. Fixed/Floating Settings D-040; default Fixed APPROVED D-043. Owner tidak menguji kontrol parsial.

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
- Uji receiving high lob, smash approach, near-net pada joystick manual/assisted dan Fixed/Floating; catat input terlewat, salah gesture dan rasa terlambat.
- Grade dan sebab miss/net/out terbaca tanpa hanya mengandalkan warna.
- Simpan screenshot/video build nyata dan device/build identity. Mockup bukan bukti fitur bekerja.
- Pemilik menilai fidelity/rasa bermain; agent menjalankan build, pengujian dan pengumpulan bukti. Rencana dan hasil ada di TESTING.
