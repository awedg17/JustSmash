# JUST SMASH — Game Design Document (GDD)
Versi 0.2 | 2026-11-01 | Persetujuan lanjutan tercatat di `05_DECISION_REGISTER.md`.

Keterangan status:
- **[USER-LOCKED]**: diminta/ditentukan langsung oleh pemilik game, jangan ganti tanpa izin.
- **[PROPOSED]**: rekomendasi perancangan; harus masuk tahap validasi/approval sebelum implementasi final.
- **[TO TEST]**: belum cukup bukti, buat eksperimen untuk dipilih pemilik game.
- **[APPROVED]**: pemilik menyetujui keputusan yang disebut, bukan semua usulan dalam paragraf atau milestone.

## 1. Visi dan pilar
**[USER-LOCKED]** Game mobile Android 3D badminton dengan kontrol langsung, pertandingan menantang tetapi fair, dan mode player career yang memberi ruang menjadi juara dunia sekaligus mengalami kehidupan atlet di luar lapangan. Offline dan single-player; gratis di awal; biaya produksi harus Rp0.

Pilar produk:
1. Rasa bermain badminton menjadi prioritas: gerakan, timing, ruang/posisi, momentum dan fisika shuttlecock harus dapat dipahami.
2. Kemenangan ditentukan terutama oleh keputusan dan kemampuan bermain; atribut mengubah kemampuan/konsistensi, bukan jaminan menang.
3. Karier satu atlet/satu save, siklus kompetisi dan ranking realistis, bisa dibuat banyak save terpisah, tamat saat pensiun sukarela.
4. UI bersih dan ergonomis, dunia hidup dengan berita serta aktivitas sosial ringan tanpa membengkakkan ukuran.
5. Offline, tanpa premium currency, tanpa pay-to-win, tanpa mengandalkan aset berisiko hak cipta.

## 2. Batas platform dan presentasi
- Nama: **Just Smash** [USER-LOCKED], nama dagang belum dikonfirmasi clearance mereknya.
- Android only, landscape, offline-only tahap pertama, single player [USER-LOCKED].
- RAM perangkat minimum 4 GB, target 30 FPS stabil, ukuran **keseluruhan game maksimal 1 GB** [USER-LOCKED; dikonfirmasi ulang pemilik]. Batas ini bukan hanya ukuran APK; ukur paket, instalasi beserta seluruh aset, dan data game pada skenario uji; laporkan komponennya tanpa menghitung paket distribusi dua kali sebagai bagian instalasi. Tidak berarti game boleh menggunakan 4 GB RAM sendiri.
- Engine **Unity [APPROVED]**. Versi editor, render pipeline, dan package ditetapkan lewat audit M0; izin engine tidak otomatis menyetujui layanan berbayar/cloud.
- [PROPOSED] kualitas grafis dibuat bisa dikurangi. Sasaran internal ukuran build jauh di bawah 1 GB bila memungkinkan, tanpa menjadi perubahan persyaratan.
- Referensi kamera: foto Badminton Jump Smash yang diberikan user: pemain di bawah layar menghadap net dan lawan di atas, view belakang bertinggi sedang. Pelacakan kamera saat lompat/zoom-out agar pemain, shuttle, dan area relevan tidak hilang [USER-LOCKED].
- Referensi visual: 3D stylized seperti Valorant tetapi lebih ringan. **Tidak menyalin model, artwork, animation rig, tekstur, UI, logo, atau audio Valorant.**
- Referensi UI: gambar user bertuliskan Minimalist Game GUI — turquoise/cyan, abu, tombol simpel; judul memakai tipografi sporty khas Just Smash, bukan menyalin visual aslinya satu-ke-satu.

## 3. Aturan pertandingan
[USER-LOCKED] Best of 3 game/set, change ends, kamera konsisten di sisi pemain, exhibition tersedia. Semua score mode harus diuji (normal/deuce/cap).
- **Quick Just Smash** [USER-LOCKED]: target 16; bila 15–15 perlu unggul 2 poin; pada 19–19 yang memperoleh poin ke-20 menang. Ditujukan untuk durasi yang nyaman.
- **Classic Official 2026** [USER-LOCKED as selectable option + corrected rule]: target 21; bila 20–20 perlu unggul 2 poin; 29–29 pemain mencapai 30 duluan menang. Bukan batas 25.
- **Modern Official mulai 2027** [APPROVED additional preset, P-002]: target 15; bila 14–14 perlu unggul 2 poin; 20–20 siapa mencapai 21 menang. BWF mengumumkan berlaku 4 Jan 2027.
- Untuk career, **Quick 16 adalah default sementara** sesuai pilihan user; apakah kalender profesional pada tahun berbeda harus otomatis memakai aturan resmi adalah `APPROVAL_REQUIRED` (jangan diam-diam ubah).
- Pergantian ujung lapangan: akhir game pertama, game kedua jika ada ketiga, dan game penentuan saat pemimpin pertama kali mencapai 8 (Quick16/Modern15, usulan), atau 11 (Classic21). Kamera tetap berorientasi dari belakang atlet pengguna; model geometri lapangan dan arah bermain diperbarui secara konsisten.
- Servis dua varian tombol [USER-LOCKED]: [PROPOSED] pendek dan panjang/flick sesuai posisi/aturan; keduanya menggunakan mekanik timing+power yang konsisten; aturan area servis ganjil/genap.
- Foul: out, shuttle mengenai net/tidak melewati net, kontak tubuh, double hit, menyentuh net, kesalahan servis, let, fault lain yang relevan; **sebagian fault yang sangat jarang boleh ditunda hingga tahap polishing bila tertulis jelas**.
- Challenge [USER-LOCKED]: Hanya pada keputusan IN/OUT; **dua challenge gagal per game, challenge berhasil tidak memotong jatah [APPROVED, P-008]**. [PROPOSED untuk simulasi kesalahan call berikut:] Call awal kadang keliru dengan probabilitas yang diketahui sistem, tapi keputusan akhir selalu berasal dari posisi jatuh shuttle sebenarnya (bukan me-random hasil challenge). Replay keputusan memakai trajectory asli; di MVP gunakan line judge call deterministik dulu lalu simulasi kesalahan secara terkendali.
- Pause, resume, autosave pertandingan belum selesai, skip/sim pertandingan [USER-LOCKED]. Skip tetap menyimpan statistik dan mengubah ranking/dana sesuai simulasi.
- Men's singles dan men's doubles [USER-LOCKED]. Singles lebih dulu; doubles memerlukan partner AI, partner selection, positioning, serving/receiving rotation, ranking pair yang berbeda.

## 4. Kontrol dan responsivitas — belum diputuskan
**[TO TEST / HARD GATE]** User belum memilih antara joystick dan swipe. Agent wajib membuat prototype A/B sebelum mengunci skema kontrol final.

**A — Manual hybrid joystick (PROPOSED):** ibu jari kiri menggerakkan pemain via virtual joystick; ibu jari kanan memegang pilihan jenis pukulan; arah straight/cross dipilih dengan input target yang tidak menyita jari kiri ketika footwork (misalnya mini-flick/target sector di tombol stroke). Input jump terpisah dan swipe ke atas pada area kanan hanya tersedia ketika jendela jump timing sah. **Dilarang** menyamakan posisi avatar dengan contact otomatis yang tiba-tiba teleport/bergeser jauh.

**B — Swipe movement (TO TEST):** avatar menuju posisi target secara berbasis drag/swipe, pukulan tetap perlu timing; perlu diuji apakah pemain merasa terlambat, kontrol gerak bergantian (bukan bersamaan), atau aim menjadi tidak terbaca. Jangan membangun kompleksitas gesture penuh sebelum A/B disetujui.

**C — Assisted (USER-LOCKED):** AI navigasi membantu footwork, pengguna tetap menentukan timing, power, jump, jenis serta aim pukulan. Bantuan gerak tidak boleh mengubah kualitas pukulan secara ajaib atau menyamakan skill player dengan AI.

**Sistem hit [USER-LOCKED + implementation PROPOSED]:**
- Pukulan: smash, lob/clear, drop shot, netting, lift. Backhand dicatat sebagai teknik/varian stroke yang dipilih otomatis dari sisi tubuh dan posisi racket, bukan tombol tambahan sejak awal.
- Arah: straight atau cross; ada target depth/power. Kekuatan berasal dari durasi menahan tombol, dan waktu **melepas** untuk kontak; durasi dan pelepasan harus dapat dilakukan tanpa kontradiksi (game menampilkan charge dan contact window).
- Timing kontak adalah relatif terhadap posisi shuttle **dan stroke**: overhead di atas/depan kepala untuk lob/smash; net shot dan lift menuntut kontak lebih rendah. Jangan gunakan kriteria overhead untuk semua jenis pukulan.
- Hit classification dari timing, power, arah, stance/posisi; kualitas: Excellent ungu, Good hijau, Fair kuning, Poor merah. Umpan balik akibat: miss, net, out, weak, off-target sesuai hukum gerak, bukan kesalahan acak yang tak dapat dipahami.
- Assist contact [PROPOSED] boleh menggerakkan lengan/racket dengan procedural IK untuk meminimalkan ketidakkonsistenan animasi dan collider; boleh memberi window input, **tidak** boleh auto-teleport pemain. Semua hit event loggable untuk debugging.
- Arah awal bukan tombol tak terbatas: target zone back-left/back-right/front-left/front-right; zone lebih banyak dipertimbangkan setelah uji ergonomi.
- Jump [USER-LOCKED]: tombol dengan jendela timing, gesture swipe-up dipakai jika valid (contoh referensi mekanik, tidak meniru kode/animasi game lain). Perlu fallback untuk pengguna non-swipe.

## 5. Physics, stamina, camera, AI
- Shuttlecock harus punya kurva melambat akibat drag, orientasi base-first, pantulan/berhenti setelah lantai, net collision, out-of-bounds, contact racket; **bukan bola basket parabola tanpa drag**. Dibangun dan kalibrasi dari hasil observasi badminton, bukan menyalin rekaman berlisensi ke asset game.
- [USER-LOCKED] comet-like visual highlight/shadow tipis agar shuttle tampak meski mengecil; tidak memanipulasi physics. Kamera dan indikator tetap tidak menghalangi penilaian kedalaman shuttle.
- [USER-LOCKED] animasi natural, hasil pukulan terbaca, tidak ada clip/tembus yang mengganggu gameplay. Motion, hit collider dan contact frame harus sinkron. Gunakan in-place anim, root-motion hanya jika hasil teruji; IK lengan dan transisi untuk reach, lunge, jump, recovery.
- [USER-LOCKED] stamina basis ketahanan kurang lebih 15 menit **akumulasi reli biasa per set**; smash, sprint, jump lebih boros, stamina pulih sebagian antargame sesuai athleticism. [PROPOSED] jangan membuat 15 menit reli panjang sebagai durasi wajib setiap set, karena durasi setiap poin variatif. Kalibrasi drain berdasarkan exertion dan time; tes simulasi rally 60 detik.
- [USER-LOCKED] AI 4 difficulty: easy, normal, hard, adaptive. **Tidak menaikkan atribut AI atau menurunkan atribut pengguna saat difficulty diubah.** Perbedaan lewat pilihan taktis, kesalahan yang wajar, antisipasi yang mungkin dilihat AI, dan efisiensi positioning; adaptive mempelajari distribusi kebiasaan berdasarkan observasi *aksi yang sudah terlihat*, bukan membaca input sebelum dieksekusi. Tidak perfect prediction, masih fun.
- [USER-LOCKED] replay momen spektakuler otomatis tetapi bisa skip/matikan; kamera standar dan opsional angle samping dekat umpire; rekaman event/transform ring buffer bukan render ulang fisika.

## 6. Career world, kalender, kompetisi
**[USER-LOCKED]** Banyak slot save; setiap career mengendalikan satu atlet; umur awal dan titik awal karier bisa dipilih; ending setelah voluntary retirement yang tersedia mulai umur 20; arsip statistik permanen bisa dibuka kembali; tak ada coaching career setelah pensiun.

**[PROPOSED] starting paths dan umur**
- Junior academy (13–16), U-17/U-19 (16–18), amateur/senior qualifier (17+), pro lower-tier (18+). Ini **pilihan desain game**, bukan klaim batas legal tunggal untuk jadi atlet badminton. BWF junior internasional adalah U-19 menurut tahun kalender, dan pembinaan nasional juga memiliki kelompok lebih muda; tidak ditemukan batas umur universal untuk pertama kali menjadi atlet profesional.
- Pemula lebih muda dapat memiliki start umur 12/13 setelah review kelayakan event; pemain yang start dewasa tetap bisa bersaing; tidak ada hukuman negara asal.
- Negara: list kurasi berdasarkan subregion (Asia Timur, Tenggara, Selatan, Tengah, Barat; Eropa utara/barat/timur/selatan; Afrika; Amerika Utara/Tengah/Karibia/Selatan; Oseania). Usahakan sampai 10 negara relevan per subregion bila ada, jangan membuat negara palsu hanya untuk memenuhi jumlah. Negara asal hanyalah identitas/national selection (logika adil lintas negara), bukan bonus atribut.

**[PROPOSED] Career progression funnel:** local youth/amateur -> national circuits -> junior/senior qualifiers -> International Future/Series/Challenge -> World Tour tiers -> World Championships / multisports 4-year cycle -> World no.1, records, retire. Kesempatan turnamen tertinggi tidak hanya berdasarkan rating stat; ditentukan ranking valid + entry quotas + pilihan kalender + national selection untuk kompetisi per negara. Kejuaraan multinegara dan Olimpiade tidak otomatis tersedia tiap tahun; event 4 tahunan.

**[PROPOSED] World ranking:** catat seluruh perolehan poin per turnamen dan tanggal; pada ranking update mingguan, hitung **hasil tertinggi 10 event selama 52 minggu** untuk masing-masing kategori (singles & doubles dipisah). Hasil lama keluar otomatis; ranking menjadi dinamis meskipun player tidak bermain. Ranking junior terpisah dan berhenti relevan setelah melewati batas U-19. Sistem seed dari posisi ranking setelah daftar peserta terkonfirmasi, draw kualifikasi/main dibuat sebelum turnamen, bracket stabil sesudah dikunci. National rankings terpisah untuk pemilihan pemain regional.

**[PROPOSED] Entry flow:** berita turnamen diumumkan -> buka pendaftaran -> user mendaftar sebelum deadline -> eligibility dan ranking check -> federasi/national selection bila event terbatas -> publish daftar main draw/qualifying -> seed -> draw -> turnamen -> update ranking/financial/news. Ada notifikasi diterima/ditolak (alasan jelas). Conflict kalender: 2 turnamen pada minggu yang sama tidak dapat diikuti bersamaan, kecuali mekanik events bertahap yang benar-benar tidak tumpang tindih. Tidak ada booking otomatis tanpa pengguna.

**[PROPOSED] World simulation:** database atlet AI asli fiktif dengan umur, negara, style, ranking, skill, fatigue, cedera, pensiun. Pemain muda berkembang, pemain senior umumnya menurun tetapi variasi besar; tidak ada auto-retire dini tanpa sebab. AI vs AI cepat via simulasi poin/stats (bukan full 3D) namun draw, hasil, prestige, dan ranking konsisten. Seed deterministik agar bug bisa direproduksi. Karier boleh panjang sampai pengguna pensiun, dengan regenerasi populasi.

**[USER-LOCKED]** Event kalender meliputi pertandingan, latihan, coach session, kontrak, sponsor, wawancara, personal life, kabar peserta kompetisi. Ada news/modal/notifikasi. Milestone, all-time/last-year/current-year stats, penghargaan dengan cutscene ringan saat juara; relationship atlet lain menurun hanya jika memilih interaksi negatif relevan, rivalitas bukan fitur wajib.

## 7. Atribut dan progression
**[USER-LOCKED]** skala atribut 1–100; overall rating 0–99 dihitung dari atribut, pembulatan `floor` bukan `ceil`; rating 99 tidak mengharuskan semua atribut 99. Maksimum rating 99. Rating hanya tampilan sintetis dan bukan pengganti kualitas input player.

**[PROPOSED] Core attributes**: Power, Footwork/Speed, Stamina, Technique, Reaction, Balance/Recovery, Athleticism. User secara eksplisit ingin Power, Speed, Stamina, Technique; tiga tambahan perlu approval. Tidak harus banyak atribut jika efeknya tidak bisa diukur. Technique memiliki subskill Smash, Clear/Lob, Drop, Net, Lift, Backhand, Serve; pengaruh pada toleransi timing, presisi dan efisiensi stamina; tidak auto-success.

**[USER-LOCKED]** Skill points dari match dan latihan kalender. Match reward dari partisipasi/performa dengan anti-exploit; training memfokuskan satu atribut atau subskill dan memberi poin spesifik. Tidak ada mini-game, tidak ada hard cap frekuensi latihan, tetapi ada fatigue dan risiko cedera (konsekuensi masuk akal, bukan hukuman random tanpa peringatan). Harga upgrade meningkat per tingkat; usia tidak menaikkan harga upgrade.

**[USER-LOCKED]** Coach dengan trait bonus diskon upgrade/skill point/performance training, atau none; coach bukan pengganti skill bermain. Respec/ganti playstyle mahal dari mata uang earned dalam game atau skill points; tidak boleh premium currency. Target kurva sekitar nilai akumulasi satu tahun latihan/kompetisi adalah *hipotesis balancing* bukan hard lock sampai dites.

**[PROPOSED] Aging & potential**: atlet muda punya growth rate lebih baik, prime dengan stabilitas lebih baik, senior mengalami penurunan meski overall pernah 99, tetapi kemenangan tetap mungkin dengan skill pemain. Gunakan kebugaran/latihan/hasil turnamen untuk mengurangi penurunan akibat inactivity. Tidak menetapkan umur pasti semua atlet jatuh; variation per individu. Cedera tidak secara fisik digambarkan ekstrem.

**[PROPOSED] Playstyles** bukan buff ajaib: 'Fast Net', 'Power Attacker', 'Counter Puncher', 'Endurance Rally' pada subskill threshold (mis. 10 untuk tier awal, butuh uji). Jelaskan di UI fungsi dan konsekuensi; perubahan tipografi/ikon minimal.

## 8. Career finances, coaches, life
**[USER-LOCKED]** pendapatan prize money, sponsor, kontrak. Pengeluaran coach, travel, fasilitas, equipment kosmetik, social life. Tidak ada hutang/financial game over; setiap kegiatan penting punya opsi gratis/skip tanpa menutup jalan karier. Sistem sponsor dipengaruhi popularitas dan reputasi, deal term/bonus/klausul dan negosiasi seperti kontrak game manajer olahraga tetapi desain dan implementasi original.

**[PROPOSED]** prestige kejuaraan, streak, interaksi fans, wawancara berefek reputasi/popularitas. Currency hanya uang fiktif earned in career; wardrobe/senarnya tidak ada boost stat, termasuk grip/tension, karena semuanya kosmetik. Event personal non-immersive ringan: keluarga, teman, hubungan, kegiatan sosial, sponsor, pengelolaan tabungan/pengeluaran; dampaknya cerita/reputasi/uang, tidak harus memakai scene 3D baru. Jangan memakai nama/identitas merek sungguhan tanpa izin.

## 9. Visual, audio, UX
- Character creator preset avatar; custom outfit, accessories, shoes, socks, racket, strings, grip, string/racket color, tattoo, hair/hair color. Semua kosmetik.
- No music, no commentator. SFX: racket/shuttle, hit/net, whistle, shoes, footwork, fall, court impact, crowd rise/cheer, rhythm 'ea ea' khas sorakan publik. Rekam/generate original/asset berlisensi; **jangan rip track dari video referensi**.
- Referensi *observasi* audio user: https://youtu.be/n20IQaOMLZQ , https://youtu.be/sWVPnyyLnFw , https://youtu.be/YanbwuQvnrU , https://www.youtube.com/live/cFW-I7T_ATY , https://youtu.be/Xvwfct8ZEO0 . Agent harus menonton/mencatat timestamp dan nama event yang audible sebelum memakai sebagai spesifikasi; jangan mengklaim audio sudah diaudit jika belum.
- UX [USER-LOCKED]: fewer buttons, readable, predictable navigation, no decorative action button, thumb reach landscape, visual quality status purple/green/yellow/red, pause/replay toggle and control settings accessible. UI style *inspired by* user reference, not copyrighted duplicate.

## 10. Simulasi doubles
[USER-LOCKED as eventual feature] Career supports Men's Doubles, but requires full partnership mechanics. [PROPOSED] Career creation chooses primary singles or doubles discipline; for doubles, choose/change partner (with nationality/eligibility rule), separate ranking & prize distribution, coach tactics and AI partner position, serve order and rotation. Cannot claim doubles complete if implemented only as 2 clones on court. Milestone later than singles core.

## 11. No-go and success metrics
Hard no-go: paid service, backend-required login, IAP, premium currency, pay-to-win, copyrighted broadcast/athlete likeness, stealing recorded crowd audio, blind changes to GDD, fake 'proven' physics, deterministic AI reading future input, features without QA.

Measure:
- Device 4 GB RAM; gameplay 30 FPS target (pacing; 1% lows logged), memory peak, ukuran keseluruhan game <=1 GB; ukuran APK bukan bukti tunggal kelulusan batas ukuran. Pengukuran mengikuti `TESTING.md`.
- Player can explain why miss/net/out from visual timing + power + direction; hit logs show consistent causes.
- One complete match can be finished / paused / resumed with same score and game state.
- Career world survives >10 simulated years without duplicate ID/ranking corruption and supports retirement/archive.
- No third-party asset without recorded license and provenance.

## 12. Research links and status
- PBSI transition 3×15 effective 4 Jan 2027: https://pbsi.id/2026/04/26/bwf-resmi-setujui-sistem-skor-15-x-3-berlaku-mulai-4-januari-2027/
- BWF future laws 3×15 cap 21 and historical: https://worldbadminton.com/rules/  (secondary mirror; verify official statute pre-release)
- BWF world ranking: https://system.bwfbadminton.com/documents/folder_1_81/Statutes/CHAPTER-5---TECHNICAL-REGULATIONS/Section%205.3.3.1%20World%20Ranking%20System.pdf (linked copy was marked effective 2018; **do not mistake it for current 2026 edition**; recheck on BWF official statutes before final point tables)
- BWF U19 juniors: https://system.bwfbadminton.com/documents/folder_1_81/Statutes/CHAPTER-5---TECHNICAL-REGULATIONS/Section%205.3.3.2%20-%20World%20Junior%20Ranking%20System%20-1.pdf
- PBSI 2026 Sirnas U17/U19/adult: https://pbsi.id/2026/04/13/surabaya-buka-rangkaian-hydroplus-sirkuit-nasional-a-2026-lebih-dari-1-100-atlet-siap-bertanding/
- PBSI tier national points examples: https://pbsi.id/kejuaraan/poin/
- IOC LA28 official qualification document index: https://www.olympics.com/ioc/documents/olympic-games/los-angeles-2028-olympic-games
