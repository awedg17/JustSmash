## Scope aktif D-062 — overhaul visual/gameplay

Plan35task disetujui2026-11-23; satu arena indoor tanpa penonton, dua karakter natural dan lima pukulan diprioritaskan. Exhibition tunggal/ganda putra tetap. History menyimpan pasangan poin side0/side1 setiap game sebelum reset, termasuk final; result menampilkan pemenang dan semua game. Tidak menebak history save lama. D-050/D-052, Official15, immediate-contact/finite-reach, grade dan seluruh setting tetap. Task blocked dilewati untuk kerja independen; tidak mengubah keputusan besar. Detail implementasi/gate di plan D-062 dan TESTING.

## Riwayat scope D-054 — 2026-11-15

Instruksi terbaru pemilik mencabut seluruh fitur/kode/menu career dengan backup terpisah; tunggal/ganda tetap. Semua requirement career/progression/world/ranking/sponsor/qualifying dalam riwayat dokumen ini bukan backlog aktif dan tidak dilanjutkan. Fokus referensi UI nyata sebelum redesign; D-052 PC-only/Rp0/no-push tetap. Bukti rollback dan QA di TESTING; keputusan di 05_DECISION_REGISTER.

## Riwayat kebijakan — D-037..D-042, 2026-11-03
Instruksi owner terbaru menggantikan aturan lama yang bertentangan; bagian checkpoint/run lama adalah riwayat, bukan izin aktif. D-044 mengizinkan eksekusi M0–M2 sampai satu APK checkpoint baru; batas docs-only sebelumnya berakhir. Otorisasi implementasi tetap M0–M2; M3+ tidak dimulai otomatis, termasuk setelah acceptance M2. Tidak ada import aset/dependency, biaya, push, destructive reset, atau install/launch APK HP tanpa izin terpisah.

Review owner hanya pengalaman modul utuh: singles lengkap; character creation lengkap; career terintegrasi; final game. Ini kelompok review, bukan penambahan scope. Tidak meminta owner menguji pukulan/animasi/tombol/fault secara granular. Agent menangani unit/integration/regression, natural automated match, inspeksi video/motion/audio, dan performance sesuai izin. Keputusan desain tetap memerlukan approval, berbeda dari testing.

Subagent dan commit lokal bersyarat tetap D-038/D-039. D-043 menyetujui NPC Casual Set00 untuk evaluasi import, default joystick Fixed (Floating tetap pilihan), servis charge1detik/power0..1/feedback konsisten/cancel aman. Grade hanya Timing + Contact; Direction/Power trace tanpa kontribusi grade. Rally sekitar30detik menurunkan stamina cukup banyak dan mulai memengaruhi performa, tetapi belum habis dan rally bisa berlanjut; jalan/lari drain0. Bobot/formula grade, biaya/kurva/efek stamina baru belum approved. Unity Personal memenuhi kondisi pribadi non-klien tanpa pendapatan yang dinyatakan owner; tidak meminta pembahasan keuangan lagi kecuali kondisi/terms relevan berubah. Target karakter/animasi/atmosfer singles tetap. Approval evaluasi NPC bukan izin import aset lain/dependency/M3 atau device action. D-044 menggantikan batas docs-only; eksekusi approved sampai APK checkpoint, tanpa mengubah proposal angka menjadi approval.

Rujukan keputusan/proposal: [Decision Register](05_DECISION_REGISTER.md). Riset aset/lisensi: [Legal dan Assets](04_LEGAL_AND_ASSETS.md). Gate bukti: [Testing](TESTING.md).

# JUST SMASH — Game Design Document (GDD)

## D-035 — approved input/stamina clarification
Technique tap strikes on press without hold. Jump hold-swipe/release remains available. Keyboard 1–5 follows immediate tap. Stat70 baseline capacity100/recovery8; stat80 capacity110/recovery8.8, linear factor `1 + (stat - 70) * 0.01`. Recovery amount, not speed, scales. Preserve timing/direction/power/position grade explanations; grade is not a guaranteed point or automatic failure. Body/net-touch included in M2-03. Career integration deferred; full M2 acceptance precedes M3. See Decision Register D-035.


Versi 0.3 | 2026-11-02 | Persetujuan lanjutan tercatat di `05_DECISION_REGISTER.md`.

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
- [DEFERRED] kualitas grafis bisa dikurangi belum disetujui; prioritas audio, hit, footwork, interaksi objek realistis dan kosmetik career. Sasaran internal ukuran build jauh di bawah 1 GB bila memungkinkan, tanpa menjadi perubahan persyaratan.
- Referensi kamera: foto Badminton Jump Smash yang diberikan user: pemain di bawah layar menghadap net dan lawan di atas, view belakang bertinggi sedang. Pelacakan kamera saat lompat/zoom-out agar pemain, shuttle, dan area relevan tidak hilang [USER-LOCKED].
- Referensi visual: 3D stylized seperti Valorant tetapi lebih ringan. **Tidak menyalin model, artwork, animation rig, tekstur, UI, logo, atau audio Valorant.**
- Referensi UI: gambar user bertuliskan Minimalist Game GUI — turquoise/cyan, abu, tombol simpel; judul memakai tipografi sporty khas Just Smash, bukan menyalin visual aslinya satu-ke-satu.

## 3. Aturan pertandingan
[USER-LOCKED] Best of 3 game/set, change ends, kamera konsisten di sisi pemain, exhibition tersedia. Semua score mode harus diuji (normal/deuce/cap).
- **Official 15-point** [USER-LOCKED]: target 15; bila 14–14 perlu unggul 2 poin; 20–20 siapa mencapai 21 menang. Mengikuti aturan resmi BWF berlaku 4 Jan 2027. Ini adalah **default dan satu-satunya aturan scoring dari awal game**.
- **Classic 21-point** [USER-LOCKED]: target 21; bila 20–20 perlu unggul 2 poin; 29–29 pemain mencapai 30 duluan menang. Opsi ini **terkunci dan baru tersedia di Settings setelah career mencapai tahun 2028**.
- Quick 16 dan preset scoring lainnya **dihapus**.
- Pergantian ujung lapangan: akhir game pertama, game kedua jika ada ketiga, dan game penentuan saat pemimpin pertama kali mencapai 8 (15-point) atau 11 (21-point). Kamera tetap berorientasi dari belakang atlet pengguna; model geometri lapangan dan arah bermain diperbarui secara konsisten.
- Servis dua varian tombol [USER-LOCKED]: [PROPOSED] pendek dan panjang/flick sesuai posisi/aturan; keduanya menggunakan mekanik timing+power yang konsisten; aturan area servis ganjil/genap.
- Foul: out, shuttle mengenai net/tidak melewati net, kontak tubuh, double hit, menyentuh net, kesalahan servis, let, fault lain yang relevan; **sebagian fault yang sangat jarang boleh ditunda hingga tahap polishing bila tertulis jelas**.
- Challenge [USER-LOCKED]: Hanya pada keputusan IN/OUT; **dua challenge gagal per game, challenge berhasil tidak memotong jatah [APPROVED, P-008]**. **[USER-LOCKED]** Line judge memiliki error rate di bawah 5%; **maksimal 1 kesalahan call per pertandingan**; kesalahan hanya terjadi pada shuttle yang jatuh sangat dekat garis (bukan kesalahan obvious). Keputusan akhir challenge selalu dari posisi jatuh shuttle sebenarnya. Replay keputusan memakai trajectory asli.
- Pause, resume, autosave pertandingan belum selesai, skip/sim pertandingan [USER-LOCKED]. Skip tetap menyimpan statistik dan mengubah ranking/dana sesuai simulasi.
- Men's singles dan men's doubles [USER-LOCKED]. Singles lebih dulu; doubles memerlukan partner AI, partner selection, positioning, serving/receiving rotation, ranking pair yang berbeda.

## 4. Kontrol dan responsivitas
**[USER-LOCKED]** Joystick virtual untuk movement. Arah pukulan ditentukan oleh **analog kiri yang sama** — posisi analog saat kontak menentukan arah jatuh shuttlecock. Tidak ada aim assist, preview trajectory, atau indikator visual kemana shuttle akan jatuh.

**Sistem hit [USER-LOCKED]:**
- Pukulan: smash, lob/clear, drop shot, netting, lift. Backhand dipilih otomatis dari sisi tubuh dan posisi racket, bukan tombol tambahan.
- Target zone: **6 zona** (back-left, back-right, mid-left, mid-right, front-left, front-right) diarahkan via analog kiri.
- D-035 teknik tap-immediate; jump hold-swipe tetap. D-041 servis khusus HOLD-release untuk power; detail tuning masih PROPOSED.
- Timing kontak relatif terhadap posisi shuttle **dan stroke**: overhead di atas/depan kepala untuk lob/smash; net shot dan lift menuntut kontak lebih rendah.
- D-043: grade dari Timing + Contact saja (kualitas posisi/ketinggian/reach/stance relevan stroke). Direction/Power hanya trace analisis, bukan grade. Excellent ungu, Good hijau, Fair kuning, Poor merah; sebab miss/net/out/weak/off-target mengikuti hukum gerak. Bobot/window/threshold numerik baru belum final.
- Assist contact [APPROVED]: procedural IK menyesuaikan lengan/racket agar visual contact realistis. Boleh memberi window input, **tidak** boleh auto-teleport pemain. Detail implementasi ditentukan agent agar natural. Semua hit event loggable.

**Jump [USER-LOCKED]:** tombol terpisah. Untuk jump-strike: **hold tombol jump → swipe ke arah tombol teknik pukulan** (misal swipe atas = smash). UI wajib menampilkan visual guide (panah/glow path) dari tombol jump ke tombol teknik saat hold aktif, agar user mudah membaca arah swipe. Fallback non-swipe tersedia.

**Assisted footwork [USER-LOCKED]:** AI navigasi menggerakkan pemain sepenuhnya; pengguna tidak perlu menggerakkan atlet, pengguna tetap menentukan timing, power, jump, jenis serta aim pukulan. Bantuan gerak tidak mengubah kualitas pukulan atau menyamakan skill player dengan AI.

## 5. Physics, stamina, camera, AI
- Shuttlecock harus punya kurva melambat akibat drag, orientasi base-first, pantulan/berhenti setelah lantai, net collision, out-of-bounds, contact racket; **bukan bola basket parabola tanpa drag**. Dibangun dan kalibrasi dari hasil observasi badminton, bukan menyalin rekaman berlisensi ke asset game.
- [USER-LOCKED] comet-like visual highlight/shadow tipis agar shuttle tampak meski mengecil; tidak memanipulasi physics. Kamera dan indikator tetap tidak menghalangi penilaian kedalaman shuttle.
- [USER-LOCKED] animasi natural, hasil pukulan terbaca, tidak ada clip/tembus yang mengganggu gameplay. Motion, hit collider dan contact frame harus sinkron. Gunakan in-place anim, root-motion hanya jika hasil teruji; IK lengan dan transisi untuk reach, lunge, jump, recovery.
- [USER-LOCKED] stamina basis ketahanan kurang lebih **rally sekitar30detik pukulan lob biasa** menurunkan stamina cukup banyak dan mulai memengaruhi performa, tetapi belum habis dan rally masih dapat dilanjutkan (D-043); smash dan jump lebih boros; jalan/lari/sprint stamina0 sesuai D-042, menggantikan aturan gerak lama. Recovery stamina antar set **tidak signifikan** — stamina 100 pun tidak akan membuat stamina yang habis menjadi penuh saat istirahat antar set. Baseline ini untuk stat stamina standar awal career (belum latihan). Semakin tinggi stat stamina: kapasitas lebih besar + recovery antar set lebih banyak (tapi tetap terbatas). Kalibrasi drain berdasarkan exertion dan time.
- [USER-LOCKED] AI 4 difficulty: easy, normal, hard, adaptive. **Tidak menaikkan atribut AI atau menurunkan atribut pengguna saat difficulty diubah.** Perbedaan lewat pilihan taktis, kesalahan yang wajar, antisipasi yang mungkin dilihat AI, dan efisiensi positioning; adaptive mempelajari distribusi kebiasaan berdasarkan observasi *aksi yang sudah terlihat*, bukan membaca input sebelum dieksekusi. Tidak perfect prediction, masih fun.
- [USER-LOCKED] replay momen spektakuler; **setting 3 opsi: replay angle gameplay + angle tambahan, replay angle gameplay saja, atau tidak ada replay**. Rekaman event/transform ring buffer bukan render ulang fisika.

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

**[PROPOSED] Core attributes**: Power, Footwork/Speed, Stamina, Technique, Balance/Recovery [USER-LOCKED]. Reaction dan Athleticism **dihapus**. Balance/Recovery: jelek = gampang jatuh/ga stabil pas mukul, recovery postur lambat setelah pukulan berat. Technique memiliki subskill Smash, Clear/Lob, Drop, Net, Lift, Backhand, Serve; pengaruh pada toleransi timing, presisi dan efisiensi stamina; tidak auto-success.

**[USER-LOCKED]** Skill points dari match dan latihan kalender. Match reward dari partisipasi/performa dengan anti-exploit; training memfokuskan satu atribut atau subskill dan memberi poin spesifik. Tidak ada mini-game, tidak ada hard cap frekuensi latihan, tetapi ada fatigue dan risiko cedera (konsekuensi masuk akal, bukan hukuman random tanpa peringatan). Harga upgrade meningkat per tingkat; usia tidak menaikkan harga upgrade.

**[USER-LOCKED]** Coach dengan trait bonus diskon upgrade/skill point/performance training, atau none; coach bukan pengganti skill bermain. Respec/ganti playstyle mahal dari mata uang earned dalam game atau skill points; tidak boleh premium currency. Target kurva sekitar nilai akumulasi satu tahun latihan/kompetisi adalah *hipotesis balancing* bukan hard lock sampai dites.

**[PROPOSED] Aging & potential**: atlet muda punya growth rate lebih baik, prime dengan stabilitas lebih baik, senior mengalami penurunan. **[USER-LOCKED]** Muda tetap harus latihan untuk naik — tidak ada auto-growth pesat tanpa usaha. Senior yang rajin latihan penurunannya lambat/minimal — overall tidak turun drastis jika aktif berlatih. Penurunan signifikan hanya akibat inactivity atau cedera. Gunakan kebugaran/latihan/hasil turnamen untuk mengurangi penurunan. Variation per individu. Cedera tidak secara fisik digambarkan ekstrem.

**[PROPOSED] Playstyles** bukan buff ajaib: 'Fast Net', 'Power Attacker', 'Counter Puncher', 'Endurance Rally' pada subskill threshold. Jelaskan di UI fungsi dan konsekuensi; perubahan tipografi/ikon minimal. **[USER-LOCKED]** Setelah pilih playstyle, tiap atribut punya **max potential cap** yang visible ke player — player tahu limit atribut per playstyle dari awal. Ganti playstyle mid-career: semua atribut di-reset ke baseline, pilih playstyle baru (cap baru berlaku), skill points yang sudah di-spend **dibalikin 90%** (10% hangus sebagai cost). Treatment sama untuk setiap kali ganti.

## 8. Career finances, coaches, life
**[USER-LOCKED]** pendapatan prize money, sponsor, kontrak. Pengeluaran coach, travel, fasilitas, equipment kosmetik, social life. Tidak ada hutang/financial game over; setiap kegiatan penting punya opsi gratis/skip tanpa menutup jalan karier. Sistem sponsor dipengaruhi popularitas dan reputasi, deal term/bonus/klausul dan negosiasi seperti kontrak game manajer olahraga tetapi desain dan implementasi original. **[USER-LOCKED]** Sponsor negosiasi: UI sederhana berbasis teks, info langsung kalau nego ketinggian sponsor marah/tolak. Tidak ada cutscene.

**[PROPOSED]** prestige kejuaraan, streak, interaksi fans, wawancara berefek reputasi/popularitas [APPROVED scope]. Currency hanya uang fiktif earned in career; wardrobe/senarnya tidak ada boost stat, termasuk grip/tension, karena semuanya kosmetik. Event personal non-immersive ringan: keluarga, teman, hubungan, kegiatan sosial, sponsor, pengelolaan tabungan/pengeluaran; dampaknya cerita/reputasi/uang, tidak harus memakai scene 3D baru. Jangan memakai nama/identitas merek sungguhan tanpa izin.

## 9. Visual, audio, UX
- Character creator preset avatar; custom outfit, accessories, shoes, socks, racket, strings, grip, string/racket color, tattoo, hair/hair color. Semua kosmetik.
- No music, no commentator. SFX: racket/shuttle, hit/net, whistle, shoes, footwork, fall, court impact, crowd rise/cheer, rhythm 'ea ea' khas sorakan publik. Rekam/generate original/asset berlisensi; **jangan rip track dari video referensi**. Katalog lengkap 57 kategori SFX tercatat di `badminton-sfx-catalog.md` hasil audit 5 video referensi YouTube.
- Referensi *observasi* audio user: https://youtu.be/n20IQaOMLZQ , https://youtu.be/sWVPnyyLnFw , https://youtu.be/YanbwuQvnrU , https://www.youtube.com/live/cFW-I7T_ATY , https://youtu.be/Xvwfct8ZEO0 . Katalog awal 2026-11-02 berasal dari metadata/caption dan inferensi, bukan audit mendengarkan langsung. Timestamp dan kategori belum terverifikasi audible; tidak boleh diklaim audit selesai. Persetujuan scope SFX tetap berlaku; tanpa musik/komentator dan tanpa rip audio.
- UX [USER-LOCKED]: fewer buttons, readable, predictable navigation, no decorative action button, thumb reach landscape, visual quality status purple/green/yellow/red, pause/replay toggle and control settings accessible. UI style *inspired by* user reference, not copyrighted duplicate.

## 10. Simulasi doubles
[USER-LOCKED as eventual feature] Career supports Men's Doubles, but requires full partnership mechanics. [PROPOSED] Career creation chooses primary singles or doubles discipline; for doubles, choose/change partner (with nationality/eligibility rule), separate ranking & prize distribution, coach tactics and AI partner position, serve order and rotation. Cannot claim doubles complete if implemented only as 2 clones on court. Milestone later than singles core.

**[USER-LOCKED] Relationship system:** partner relationship dibangun lewat kegiatan realistis — muji saat interview, bonding, interaksi positif lainnya. Bukan fitur wajib rivalitas.

**[USER-LOCKED] Rotation strategies:** Top 3 strategi rotasi men's doubles diterapkan:
1. **Front-Back (Attack Formation):** satu pemain di T area, satu di baseline. Aktif saat side menyerang (smash/drop ke bawah).
2. **Side-by-Side (Defense Formation):** dua pemain bagi court kiri-kanan di mid-court. Trigger instant saat side terpaksa lift/clear.
3. **Free Rotation (Transition):** perpindahan fleksibel antara attack ↔ defense sepanjang rally. Block-and-follow: block ke net → maju jadi front, partner mundur. Drive counter: flat return → maju ke predicted reply zone.

**[USER-LOCKED] Positioning & joystick:** Player bebas gerak pakai joystick. UI menampilkan suggested position via hologram dots sesuai state rotasi. Keluar rotasi terus-terusan = minor penalty ke coach/partner relationship. Keluar rotasi tapi selamatkan momen penting = tidak kena penalti. Rotasi bukan faktor utama hubungan memburuk.

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
