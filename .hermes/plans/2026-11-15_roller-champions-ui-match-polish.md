# PLAN — JustSmash: UI sporty orisinal dan pertandingan matang

Tanggal plan: 2026-11-15. Status eksekusi terbaru: **30/41 checklist selesai; 11 masih terbuka** (2026-11-21). Eksekusi disetujui D-055; D-056..060 mengubah arah biru-kartunis, izin Anton/rekaman langkah/delegasi, serta fix AI/timer berbasis reproduksi dan diagnosis RM terbatas. Baseline/usulan di bawah dipertahankan sebagai riwayat, bukan batas izin terbaru.

Checkpoint produk `d060-checkpoint1.json`:208executions/207uniquePASS,APK30233578byte SHA256`1ec08181fe9ff0f5419e87305347696a9846ec74e88dbb98d68025f9f19f0919`,source1.147/protected1.348verified. Diagnosis tambahan `d060-inprocess-closure1.json`:controlled2/2/full184/184/restored15/15PASS, tetapi lockasliNOT_REPRODUCED/ownerUNKNOWN; bukan productfix atau penutupan DoD. Observer dicabut SHA-exact. Continuous-motion/audible/owneracceptance, historicalaudiointermittency dan originalsavecause tetap terbuka. `docs/05_DECISION_REGISTER.md` dan bagian aktif SESSION_HANDOFF/TESTING menjadi status otoritatif; PC-only/no-native/no-push tetap. Tidak ada approval perluasan traceOS baru pada pencatatan ini.

Dokumen handoff untuk eksekutor tanpa konteks chat. Saat plan dibuat, tidak ada perubahan pada runtime, aset, package, setting proyek, save atau APK. Pemeriksaan hash/XML lama bukan menjalankan ulang Unity.

## 0. KONTEKS WAJIB

### 0.1 Permintaan dan hasil yang dituju

Pemilik: “lanjutin dari task terakhir dan gua mau lu buat plan /plan kayak roller champion. karena uinya udah kayak gitu, pastikan animasi gameplay dan detail pas lagi pertandinga n juga sebagus itu”. Batas sebelumnya: **jangan jiplak; career dicabut; tunggal/ganda tetap**.

JustSmash adalah badminton 3D stylized, Android landscape, offline, pemain melawan AI. Arah: identitas turquoise/cyan–abu JustSmash, atlet menjadi fokus menu, tipografi sporty, lapangan dan pertandingan hidup. Bukan dashboard, toko, atau lobby multiplayer. UI proyek **belum** di-redesign mengikuti referensi.

Roller Champions menjadi acuan tingkat perhatian terhadap presentasi, bukan janji kesetaraan produksi AAA. Menu bagus tidak menutup animasi buruk. Kelulusan perlu bukti gerak, kontak, kamera, suasana, suara, seluruh alur pertandingan, dan penerimaan pemilik. Bila aset/pendekatan existing gagal mencapai hasil, laporkan batas dan ajukan perubahan pendekatan; jangan turunkan standar diam-diam.

### 0.2 Keputusan dan batas otorisasi

| Dasar | Keputusan aktif | Implikasi plan |
|---|---|---|
| Instruksi chat terbaru | Roller Champions dipilih sebagai arah; permintaan sekarang plan | Pilihan Nintendo pada ringkasan lama bukan arah aktif. Belum izin implementasi |
| D-054 | Career dihapus penuh; backup terpisah; tunggal/ganda dipertahankan | Jangan menghidupkan career, save career, qualifying, progression, store atau placeholder tab |
| D-053, bagian non-career | UI/alur utuh, GOR, ofisial, penonton, reaksi, audio, tunggal/ganda putra | Polishing mencakup pertandingan, bukan tombol saja |
| D-052 | PC-only | Tanpa discovery ADB/HP, install, launch, log/data HP, emulator atau port Windows baru. Device QA `DEFERRED_OWNER_PC_ONLY` |
| D-051 / penolakan tool | Tanpa native input PC, edit AI_RULES atau worklog baru | Synthetic handler tests dibedakan dari input native; jangan bypass penolakan |
| D-048 | NPC Casual Set00 existing; custom sportswear dibatalkan | Jangan memulai lagi Blender/outfit/Quaternius; model existing bukan paket animasi badminton |
| D-049 / D-050 | Grade dan stamina final | Grade Timing+Contact 50:50, threshold 85/65/40; jalan/lari drain 0. Jangan tuning demi animasi |
| Batas umum | Rp0, tanpa package/aset/akun baru, tanpa push | Font, clip, rekaman atau model baru membutuhkan izin spesifik dan provenance |

Rujukan aktif: `docs/05_DECISION_REGISTER.md:1–49`. Beberapa header lama dalam `docs/AI_RULES.md:1–15` masih menyebut Quaternius/device; keputusan lebih baru di atas yang berlaku. Jangan mengedit file aturan untuk mengatasi konflik.

### 0.3 Protokol yang harus dibawa eksekutor

1. Baca dokumen proyek dan keputusan terbaru sebelum coding. Plan ini bukan persetujuan angka/layout baru; detail di bawah merupakan usulan sampai pemilik mengizinkan eksekusi. Konflik gameplay/content yang belum diputuskan: hentikan slice terkait, bukan menebak.
2. Audit dirty/untracked. Saat audit, `Assets`, `Packages`, `ProjectSettings` masih untracked dan banyak dokumen dirty. Jangan `git reset`, `git clean`, overwrite pekerjaan pemilik, atau menganggap checkout Git cukup untuk rollback. Backup per slice dengan daftar file dan hash; restore hanya perubahan slice sendiri.
3. Tidak `git push`. Tidak subagent pada pekerjaan ini. Commit lokal hanya bila diizinkan, setelah audit secret dan file besar; bukan syarat penyelesaian plan.
4. TDD per perilaku: tes reproduksi dahulu, RED yang benar, perubahan minimum, GREEN, regresi. Tes baseline yang sudah lulus tidak perlu dipaksa gagal. Maksimum tiga percobaan per kegagalan yang sama; setelah itu laporkan error/bukti/batas diketahui.
5. Pertahankan Unity built-in, partial `CourtPrototype`, IMGUI dan tool yang sudah tersedia. Tidak migrasi URP/HDRP, Cinemachine, Animation Rigging, UI framework, atau event bus demi polish.
6. Root/collider, physics fixed-step, score, reach dan input tetap otoritatif. Animasi tidak boleh menunda hit sah, memperpanjang tulang, memperlebar reach/window, memindahkan root agar hit sukses, atau mengubah hasil pertandingan.
7. Simpan data secara atomik; validasi input/snapshot sebelum mutasi. Backup **seluruh** `.hermes/local-game-data/`, termasuk snapshot dan `.bak`, sebelum QA. `PreferenceFixture` hanya mengisolasi prefs, bukan snapshot pertandingan.
8. Pisahkan numerical PASS, rendered-still review, continuous-motion review, audible review, packaging, native input, device QA dan owner acceptance. Tidak menulis “gameplay/UI selesai” dari jumlah tes.
9. Gunakan `docs/TESTING.md`, `docs/ERROR_LOG.md`, decision register, PRD/ARCHITECTURE/DESIGN/roadmap existing untuk perubahan yang kelak disetujui. Jangan membuat `WORKFLOW.md`/worklog pengganti, mengedit `docs/AI_RULES.md`, atau menimpa `docs/UI_REFERENSI.md` yang punya penolakan overwrite sebelumnya.
10. Tidak rip aset/audio/animasi game lain atau video badminton. Screenshot referensi hanya observasi, tidak dikemas menjadi aset game. Kredensial tidak masuk command/log/artifact shareable; redaksi menjadi `[REDACTED]` bila ditemukan.
11. Jangan tutup Editor dengan pekerjaan belum tersimpan. Proses panjang memakai background+notify; tidak mengedit source/test saat Unity menjalankan suite atau build. Baca XML dan log sebenarnya sebelum memulai slice berikutnya.

Dasar: `docs/AI_RULES.md:22–64`, keputusan lebih baru di §0.2, dan batas pemilik saat ini.

### 0.4 Arsitektur dan lokasi aktif

| Lokasi | Tanggung jawab / titik perubahan minimal |
|---|---|
| `Assets/Scripts/CourtUi.cs:8–34` | Screen/action existing; Home sudah Main, Latihan, Pengaturan, Kredit |
| `Assets/Scripts/CourtUi.cs:83–109` | `UiActionRect`, sumber hitbox menu |
| `Assets/Scripts/CourtUi.cs:184–218` | Reset Home, kamera/pose preview, gaya teks |
| `Assets/Scripts/CourtUi.cs:232–373` | Tombol, `OnGUI`, settings dan HUD |
| `Assets/Scripts/AthleteVisual.cs:30–64` | `AthleteNPC` runtime, skeleton utama; Animator dimatikan |
| `Assets/Scripts/AthleteVisual.cs:91–171` | Native legacy clips, sampling pose, fixed-length IK, contact |
| `Assets/Scripts/AthleteVisual.cs:173–205` | Reaksi serta foot planting |
| `Assets/Scripts/CourtMotion.cs:59–120` | Pemanggilan visual player/opponent; proxy lama dan timer footstep masih ada |
| `Assets/Scripts/CourtDoubles.cs:119–180` | Identitas empat aktor, preview, posisi servis dan `AnimateDoubles` |
| `Assets/Scripts/CourtPrototype.cs:604–628` | Urutan update, reaksi dan kamera pertandingan |
| `Assets/Scripts/CourtPrototype.cs:725–743` | Fixed-step jump, landing, net fault dan rally drain; bukan target tuning |
| `Assets/Scripts/CourtFlow.cs:8–30` | Jeda poin 3 detik, Warmup/Ready/GameEnd/Interval/ChangeEnds |
| `Assets/Scripts/CourtReplay.cs:8–86` | Pose replay/snapshot, validasi dan kompatibilitas |
| `Assets/Scripts/CourtArena.cs:45–166` | Crowd pose baked, venue/ofisial, papan skor, reaksi |
| `Assets/Scripts/CourtAudio.cs:16–88` | Room/crowd, impact samples, SFX procedural |
| `Assets/Scripts/LocalGameData.cs:10–43` | Penyimpanan Editor workspace dan sandbox Android |
| `Assets/Tests/PlayMode/MobileMatchTests.cs:15–16` | Output harness masih hardcoded ke evidence 20261114 |
| `Assets/Editor/AndroidBuild.cs:11–45` | Build mobile, output APK, scene dan metadata Android |
| `evidence/mobile/verify_mobile_apk.py:12–31` | Verifier lama hardcoded ke source4; jangan dipakai sebagai bukti build baru tanpa penyesuaian |

Unity `6000.3.18f1`, Test Framework `1.6.0`, built-in renderer (`m_CustomRenderPipeline` fileID 0). Tidak ada `.ttf/.otf` di Assets pada audit. Python, ffmpeg dan ffprobe tersedia. `graphify-out/graph.json` tidak tersedia; audit memakai source langsung, tidak membuat graph.

## 1. BASELINE DAN PEMETAAN GAP

### 1.1 Bukti yang sudah ada — bukan hasil implementasi plan

- Manifest `evidence/shareable/20261115/remove-career-final-source.json`: **1.124 file** Assets/Packages/ProjectSettings. Pemeriksaan ulang saat planning: semua hash cocok, delta kosong.
- XML existing dibaca ulang: `remove-career-green1.xml` **1/1 PASS**, `remove-career-edit.xml` **19/19 PASS**, `remove-career-play.xml` **95/95 PASS**, failed/skipped/inconclusive 0. Play adalah subset enam class, bukan full regression.
- Baseline visual yang diperiksa: `evidence/shareable/20261115/home-no-career-doubles.png`. Atlet kecil, panel menu dominan, arena/tribun gelap, keterbacaan teks arena rendah. Ini observasi still, bukan diagnosis motion.
- Motion/audio belum diterima. Regresi natural ganda Easy historis **1069,29 detik**, gagal gate **<1000 detik**; belum rerun saat planning. Pertahankan status terbuka (`docs/TESTING.md:18`, `docs/TESTING.md:48–54`). Jangan menutupnya dengan animasi baru atau memperbesar batas tes.
- Tidak ada APK baru, gameplay recording baru, tes native input atau tes HP pada tahap plan.

### 1.2 Acuan visual dan batas adaptasi

Main menu Roller Champions menunjukkan karakter utuh besar, aksi dominan yang jelas, judul tebal dan ruang latar dingin yang memisahkan siluet. Latar screenshot tersebut adalah ruang presentasi, **bukan bukti arena pertandingan**.[3]

Mode selection menunjukkan pembeda selected/locked dan hierarki antar opsi. Still tidak membuktikan apakah klik langsung memulai permainan atau membutuhkan konfirmasi.[4]

Adaptasi JustSmash: hierarki, skala atlet, ruang bernapas, kontras, konsistensi antar layar. Jangan menyalin pink khas, grafiti, logo, karakter, pose khas, bentuk kartu/stiker, susunan tab, toko, battle pass, reward, undangan sosial atau progression lock. Timing animasi di plan adalah usulan JustSmash, **bukan hasil mengukur video Roller Champions**. Trailer yang sempat ditemukan belum diaudit geraknya; tidak menjadi dasar klaim motion.

| Gap / permintaan | Bukti source aktual | Task |
|---|---|---|
| Menu punya identitas, atlet lebih menonjol | Panel 520 unit + dim overlay seluruh layar: `Assets/Scripts/CourtUi.cs:254–278`; Home memakai kamera jauh: `Assets/Scripts/CourtUi.cs:203` | B, C |
| Swing/footwork bukan pose robot | Clips empat key pada chest/left shoulder, pilih satu pose tanpa blend transisi: `Assets/Scripts/AthleteVisual.cs:91–114`, `Assets/Scripts/AthleteVisual.cs:139–171` | D, E |
| Kontak, grip dan feet tetap benar | IK fixed-length + plants sudah ada: `Assets/Scripts/AthleteVisual.cs:116–134`, `Assets/Scripts/AthleteVisual.cs:181–205` | D, E |
| Dunia pertandingan bereaksi | Crowd berbagi satu mesh duduk; cheer memutar seluruh badan; sinyal wasit balok terpisah: `Assets/Scripts/CourtArena.cs:60–75`, `Assets/Scripts/CourtArena.cs:133–166` | F |
| Kamera, replay dan hasil koheren | Kamera bergantung tinggi jump pemain; replay menyimpan pose dan mengembalikan kamera: `Assets/Scripts/CourtPrototype.cs:625–662`, `Assets/Scripts/CourtReplay.cs:8–27` | G |
| Audio menyatu dengan gerak | Impact/room/crowd rekaman existing; step/net/whistle/score masih procedural: `Assets/Scripts/CourtAudio.cs:46–88`; langkah player timer .28s: `Assets/Scripts/CourtMotion.cs:87` | F |
| Klaim mutu punya bukti | Capture lama 15fps pada singles, kategori Capture juga berisi tes native yang dilarang: `Assets/Tests/PlayMode/MobileMatchTests.cs:27`, `Assets/Tests/PlayMode/MobileMatchTests.cs:113–167` | A, H |

## 2. URUTAN EKSEKUSI

**A baseline/proteksi → B preview arah satu paket → C UI → D pilot motion → E gerak lengkap → F arena/audio → G kamera/alur → H QA utuh/APK.**

D dikerjakan segera setelah preview/UI minimum, sebelum menghabiskan waktu pada dekor arena. Bila pilot gagal, hentikan perluasan motion dan laporkan batas pendekatan. Pekerjaan independen yang sudah disetujui boleh lanjut, tetapi tidak mengemas kegagalan motion sebagai hasil final.

| Task | Hasil yang harus terlihat | Gerbang |
|---|---|---|
| A | Baseline, backup, evidence output tidak menimpa histori | Hash/data aman; kegagalan existing tercatat |
| B | Satu storyboard Home–Setup–Rally–Result dengan identitas sendiri | Persetujuan arah visual satu paket, bukan QA per tombol |
| C | Semua menu/HUD konsisten dan tetap berfungsi | Pointer/safe-area/regresi + inspeksi screenshot |
| D | Satu rangkaian approach–jump smash–land–recover meyakinkan tanpa efek | Kontak numerik + video 1×/0,25×; jangan lanjut bila gagal |
| E | Seluruh stroke/footwork/reaction konsisten pada empat aktor | Matriks gerak + replay/save + capture |
| F | Penonton/ofisial/audio bereaksi sesuai kejadian, tidak terus berisik | Event tepat + review gerak dan audible |
| G | Kamera dan tempo mendukung baca pertandingan | Rally/jump/change-ends/replay/result utuh |
| H | Regresi final, rekaman pertandingan, APK source-bound | Status lokal, packaging, device dan owner terpisah |

## 3. DETAIL TASK

### A — Bekukan baseline dan lindungi hasil lama

**Kondisi sekarang:** `Assets/Tests/PlayMode/PreferenceFixture.cs:11–32` melindungi prefs saja. Snapshot ditulis melalui `Assets/Scripts/CourtPrototype.cs:246–256`. Capture memakai lokasi tanggal lama (`Assets/Tests/PlayMode/MobileMatchTests.cs:15–16`) dan beberapa encoder memakai overwrite. Packaging verifier juga terikat source4 lama.

**Perubahan setelah izin eksekusi:**
1. Backup per-file + SHA-256 source yang akan disentuh, dokumen terkait, seluruh local-game-data, build lama dan evidence hardcoded yang mungkin ditulis tes. Jangan memasukkan backup ke Assets.
2. Buat folder run unik `evidence/shareable/<tanggal>/roller-polish-<run>/`. Arahkan keluaran test yang akan dijalankan ke folder itu dengan perubahan test-only terkecil; jangan membuat framework capture baru. Audit semua `File.Write*`/encoder sebelum suite penuh, bukan hanya `CaptureEvidence`.
3. Tambah regresi isolasi fixture snapshot/prefs dan penolakan overwrite evidence sebelum memperbaiki harness. Simpan run RED. Jika jalur output lama tidak dapat diganti pada satu tes, backup, arsipkan keluaran QA, restore original, cek hash.
4. Jalankan baseline relevan setelah proteksi. Reproduksi Easy <1000 secara terpisah sebelum menilai delta nanti. Bedakan defect runtime, fixture clock, dan target durasi; jangan tuning AI/physics dalam slice polish.
5. Sesudah persetujuan plan, catat keputusan baru pada register dengan ID berikut yang benar; sinkronkan arah aktif DESIGN/PRD/roadmap/ARCHITECTURE. Tidak edit AI_RULES atau UI_REFERENSI; tidak membuat WORKFLOW baru.

**Tes dahulu:** `PreferenceFixture`, `MobileMatchTests.NaturalDoublesHarnessClockSurvivesStartUi`, regresi no-career `MobileUiTests.ExhibitionOnlyHomeHasNoCareerAndBothFormatsStillRoundTrip`.

**Selesai:** manifest baseline + backup terverifikasi; output unik; data user byte-identik sesudah QA; status Easy tidak hilang; semua perubahan test-only dibedakan dari runtime.

### B — Preview arah visual satu paket

**Kondisi sekarang:** Home sudah memiliki empat aksi, bukan fitur yang perlu dibuat ulang (`Assets/Scripts/CourtUi.cs:24–33`). Wordmark besar dan bidang kiri fixed mendominasi; screenshot baseline memperlihatkan atlet kecil dan latar redup. Referensi dipilih untuk hierarki, bukan bentuk salinan.

**Usulan desain:**
1. Pertahankan GOR JustSmash sebagai identitas. Home memakai komposisi dekat di sisi lapangan, atlet di kanan, menu di kiri; bukan court pertandingan yang sekadar dipasang di belakang overlay gelap. Tidak membuat arena kedua. Kamera preview terpisah dari kamera rally dalam state existing; pose staging Home harus direset saat mulai pertandingan.
2. Acuan canvas existing 1280×720 (`Assets/Scripts/CourtPrototype.cs:35`). Usulan Home: lajur aksi sekitar 30–34% lebar aman; atlet utama tinggi 55–65% area aman, kepala/kaki/raket tidak terpotong. Ganda menampilkan pasangan sebagai subjek bersama dengan lawan lebih sekunder, tanpa menyembunyikan bahwa formatnya 2 lawan 2. Saat Setup, keempat identitas tetap dapat dibaca.
3. Warna mengambil proyek, bukan default produk lain: panel RGB existing sekitar `#14262D`, aksen `#26D0C3`, teks `#F4F8F9`; abu lebih terang pada lantai/area atlet. Kurangi dim seluruh layar, gelapkan lokal di balik menu. Coral tetap pembeda tim lawan, bukan aksen dekor kedua. Hex merupakan turunan source/proposal, bukan token pemilik yang sudah dikunci.
4. Tipografi: judul padat tebal, sedikit miring khusus wordmark; angka skor tegak dan stabil; body tidak miring. Preview memakai font built-in dengan styling dahulu. Tidak mengunduh font. Bila hasil tidak cukup, ajukan **satu** font berlisensi jelas beserta biaya Rp0, ukuran dan izin embedding; tunggu izin sebelum import. Font generik tidak boleh dipasarkan sebagai custom typography.
5. Bentuk sederhana, bidang solid, garis lapangan/net sebagai motif terbatas. Hindari panel SaaS, neon/glass, grafiti, barcode, ikon notifikasi, deretan kartu promo, dan subtitle dekoratif. Bidang miring kecil boleh menjadi bahasa sporty milik proyek, tidak meniru kartu referensi persis.
6. Buat satu storyboard Home, Setup tunggal, Setup ganda, rally HUD, pause/settings, hasil. Pakai capture aset proyek nyata; layout komposisi baru diberi label **KONSEP — belum runtime final**. Jangan merekayasa gambar seolah hasil game sudah jadi. Dapat memakai capture harness existing tanpa input native; jangan membangun web app untuk meniru Unity.

**Gerbang:** minta persetujuan **arah visual satu paket**, bukan meminta pemilik menguji setiap tombol/pose. Boleh lanjut tugas independen yang sudah disetujui, tetapi jangan menerapkan redesign luas sebelum arah disepakati. Seluruh review teknis tetap tugas agent.

**Tes/cek dahulu:** inventaris state, aksi dan toggle dari `UiActions`, `ActionLabel`, `DrawSettings`; tandai tiap elemen pada storyboard. Cek bahasa Indonesia, label panjang ganda, kontras komposit arena+panel. Target awal kontras teks kecil ≥4,5:1, elemen besar ≥3:1; ukur hasil render, bukan swatch terpisah saja.

**Selesai:** arah punya identitas JustSmash; tidak menghapus kontrol/setting; ukuran atlet/latar bisa direalisasikan dengan aset existing; keputusan font/asset yang belum diizinkan terlihat jelas.

### C — UI, HUD dan transisi menu

**Kondisi sekarang:** `UiActionRect` dan `OnGUI`/`DrawGameHud` membagi hitbox dan gambar; settings dan input nyata sudah tersedia (`Assets/Scripts/CourtUi.cs:83–171`, `Assets/Scripts/CourtUi.cs:245–373`). Jangan menulis menu fungsional kedua untuk preview.

**Perubahan minimum:**
1. Terapkan komposisi B melalui helper/gaya existing. Pertahankan action key dan alur `ActOnUi`; ubah label tampilan bila disetujui, bukan ID internal. Home tetap Main/Latihan/Pengaturan/Kredit; tidak menambah Resume/Shop/News/Career hanya demi mengisi ruang.
2. Setup: pilihan Tunggal/Ganda setara, status selected berupa bidang + label/penanda, bukan warna saja. Identitas tim, format 15 poin/best-of-three, tingkat kesulitan, satu CTA mulai yang dominan. Tidak menambahkan lock progression atau pilihan fiktif.
3. HUD rally: scoreboard ringkas berisi tim, angka poin/game dan server; energy terbaca; pause terpisah. Area net, shuttle, badan/raket, serta lajur challenge tidak tertutup. Feedback poin singkat dan jelas, bukan banner besar selama rally. Stamina, service charge dan miss explanation tetap ada; jangan menampilkan grade internal yang sebelumnya disembunyikan.
4. Pertahankan seluruh setting: Manual/Assisted, Fixed/Floating, tiga replay mode, volume, warna/kecerahan trail, warna/kekuatan shadow, 30/60 FPS. Pause/save/load/restart/menu, konfirmasi Ya/Tidak, latihan dua umpan dan kredit tetap bekerja. Android Back/background pause tidak bocor input.
5. Gunakan hitbox yang sama untuk gambar dan pointer. Target minimum usulan: menu utama ≥64 unit canvas tinggi, shot/jump ≥80, slider ≥48; jarak antartarget ≥12. Ini **unit canvas**, bukan klaim ukuran dp atau ergonomi HP. Pertahankan minimum existing bila lebih besar. Safe-area diuji termasuk inset simulasi.
6. Usulan motion UI: feedback press 70–100ms; perpindahan panel 160–220ms; fade kamera/menu 220–300ms. Input action tetap diproses sekali saat kondisi valid, bukan menunggu animasi selesai. Animasi UI memakai unscaled time agar pause/settings tidak membeku; frame loop tidak boleh menambah ulang hit/serve. Tidak bounce berlebihan atau shimmer terus-menerus.
7. Result menampilkan pemenang, format, tim dan skor game **yang tersedia**; jangan mengarang statistik/set history. Tombol tanding ulang dan menu jelas. Reaksi kemenangan tetap terlihat di belakang panel tanpa menutupi informasi.

**Tes dahulu:** perluas `MobileUiTests` untuk bounds/text fit/overlap, selected state, all-toggle round-trip, cancel drag-off, multi-pointer, transisi cepat, Back dan kembali dari ganti sisi. Pertahankan tes no-career dan semua fungsi existing. Update assertion posisi lama hanya sesuai desain yang disetujui, bukan menghapus regresinya.

**Kriteria selesai:** capture nyata Home, Setup dua mode, Warmup/Ready, Serving/Rally/Point, Challenge, Replay, Pause/Settings/Credits/Confirm, GameEnd/Interval/ChangeEnds, Result/Rematch/Home. Uji 1280×720, 1920×1080, 2400×1080 dan 1280×800, plus safe-area inset simulasi. Teks/hitbox tidak bertumpuk, selected tidak ambigu, menu tidak memicu gerak/pukulan, tidak ada career. Screenshot diperiksa; passing bounds saja belum visual PASS.

### D — Pilot motion: approach, jump smash, landing, recovery

**Kondisi sekarang:** `AthleteVisual` memakai prefab NPC, mematikan Animator, lalu membuat legacy clips dengan empat key pada chest/left shoulder. `Present` memilih satu clip dan mengoreksi lengan/kaki dengan IK. Contact sudah diposisikan pada t=0; root tidak dimiliki animasi (`Assets/Scripts/AthleteVisual.cs:40–64`, `Assets/Scripts/AthleteVisual.cs:91–171`). Ini pondasi teknis, bukan bukti animasi natural. `CourtMotion` juga masih menggerakkan proxy lama, tetapi model terlihat berasal dari `AthleteVisual`; jangan memperbaiki rig tersembunyi saja.

**Pilot terkecil:** satu rangkaian nyata 8–12 detik: ready, dua langkah pendek menuju shuttle, jump-smash yang sah, landing, kembali ready. Buat variasi forehand lalu backhand; view gameplay + samping audit. Matikan efek tambahan pada audit pose; jangan tutupi snap dengan shake/trail/cut.

**Perubahan minimum:**
1. Kerjakan di `AthleteVisual.MakeClips`, `Present`, `PlantFeet`; pakai native animation/curve/IK existing. Buat distribusi pose pelvis–torso–shoulder–elbow–wrist dan counterbalance lengan bebas. Transisi gerak memakai displacement aktual/local axis, bukan sekadar nama stroke.
2. Persiapan berasal dari ready/gerak dan posisi shuttle yang sudah terlihat. Input teknik tetap immediate; **tidak ada windup wajib baru**. Jump mengikuti fase fixed-step existing. Jika pemain tap mendadak, prioritaskan kontak sah tepat waktu; jembatani pose visual dengan shortest-path rotation, bukan memindah jadwal fisika.
3. Contact IK tetap finite fixed-length dan diaktifkan pada frame kontak; follow-through spesifik smash/lob/low shot, kemudian recovery. Blend hanya bagian yang tidak memindahkan string bed dari kontak sah. Jangan menambah IK package atau mengaktifkan root motion.
4. Usulan tuning visual: blend locomotion 80–140ms; follow-through 120–220ms; recovery 250–450ms; landing compression/recovery visual sekitar 180–280ms. Rentang ini kandidat, tidak mengubah waktu jump/physics, energy, input window atau kemampuan bergerak. Ukur pada 30/60 render; foot planting tidak menggunakan perpindahan tetap per frame.
5. Satu otoritas pose per skeleton; evaluasi lalu IK final. Jangan membiarkan pass Animator/LateUpdate kedua menimpa contact. State temporal baru harus dapat direkonstruksi untuk replay atau disimpan/direset secara tervalidasi; hindari membawa timer tersembunyi ke save.

**Tes dahulu:** perluas `MobileMotionTests.VisibleContactUsesFixedBoneLengthsForBothSidesAndStrokes`, `GroundedFeetKeepStanceAndContactShadowStaysOnFloorDuringJump`, `JumpClockUsesFixedTicksAndLandingRecoversAtBothRenderRates`; tambah pengujian fase sebelum/saat/sesudah contact dan transisi actual entrypoint, tidak hanya memanggil `Present` langsung. Pertahankan tes input/shot yang membuktikan kontak sekali dan miss tidak dibuat menjadi hit.

**Gerbang numerik:** existing contact error <0,05m; panjang segmen dalam ±0,001m; root tidak berubah akibat `Present`; stance drift <0,04m dalam fixture existing. Itu batas regresi, bukan definisi realisme. Perluas kasus arah/ketinggian legal, 30/60fps, kedua ujung lapangan, pause/resume dan reach-invalid. Toleransi tidak dilonggarkan untuk membuat pilot hijau.

**Gerbang visual:** rekaman runtime 1× dan potongan 0,25× tanpa interpolasi frame buatan; preparation/contact/follow-through/landing/recovery terbaca, kaki menahan berat, grip tidak lepas, siku/lutut tidak membalik, kaki tidak meluncur/menembus lantai, tidak snap dari ready ke pose ekstrem. Log timestamp cacat, inspeksi urutan rapat di sekitar kontak; still/GIF pose tidak cukup.

**Stop rule:** maksimum tiga percobaan atas cacat yang sama. Jika native pose/IK mentok, tandai `BLOCKED_MOTION_APPROACH`, simpan kegagalan, jangan lanjut memperbanyak stroke atau mengklaim natural. Ajukan satu kandidat clip badminton berlisensi/authoring workflow dengan bukti rig compatibility, biaya Rp0, payload dan preview; tidak import sebelum izin. Tidak otomatis mengulang jalur outfit yang sudah ditutup D-048.

### E — Luaskan motion ke pertandingan tunggal/ganda

**Kondisi sekarang:** player/opponent memakai driver sama, partner dan lawan kedua memanggil `Present` dengan jump/landing nol (`Assets/Scripts/CourtMotion.cs:59–85`, `Assets/Scripts/CourtDoubles.cs:173–180`). Replay menyimpan stroke, elapsed, travel dan extra pose (`Assets/Scripts/CourtReplay.cs:8–27`). Jangan menjanjikan AI jump baru melalui polishing visual.

**Perubahan setelah D lolos:**
1. Paket locomotion: ready breath kecil, split-step responsif saat lawan memukul, maju/mundur/samping, pengereman, lunge berdasarkan arah/reach aktual, recovery. Perubahan berat badan boleh visual; kecepatan/perpindahan root tetap sama. Tidak auto-teleport menuju shuttle.
2. Stroke: Serve, Smash, Clear/Lob, Drop, Net, Lift, plus forehand/backhand sesuai sisi aktual. Jadwal preparation/contact/follow-through berbeda menurut stroke dan tinggi, bukan satu gerak dengan label berbeda. Backhand tidak sekadar membalik arah twist torso.
3. Jump/landing hanya saat state gameplay benar-benar airborne; tidak memberi bot lompatan dekoratif yang membuat kaki terbang sedangkan contact/collider tetap di lantai. Penambahan bot jump/dive mekanis adalah scope gameplay terpisah.
4. Terapkan profil gerak konsisten pada Player, Opponent, Partner, OpponentPartner dengan local axis setelah change ends. Hindari foot plant kedua kaki terkunci ketika root terus bergerak; stance/swing fase sesuai displacement. Ganda tidak menjadi empat animasi serempak.
5. Reaksi poin: menang normal, poin penting, kalah ringan, akhir game/match; bobot berbeda tanpa menambah ritual panjang. Harus berhenti ketika siap servis, reset Home, serta mengikuti koreksi Challenge. Freeze saat pause; replay tidak memicu hadiah, score, sound/reaction baru.
6. Tidak refactor seluruh `CourtPrototype`. Tambah helper hanya jika benar-benar dipakai; pertahankan public entrypoint dan struktur snapshot. Jika visual state baru diperlukan, validasi finite/bounded, default aman snapshot lama, no partial mutation pada data invalid.

**Tes dahulu:** perluas motion/replay/snapshot di `MobileMotionTests`, `MobileInputTests`, `MobileUiTests` dan `MobilePresentationTests.PointReactionsFollowTeamAndSurvivePauseSaveUntilNextServe` / `ReactionsTrackOverturnedCallsGameIntervalsAndMatchResults`. Periksa HairSkin/rig/racket dan net-fault sesudah perubahan pose; jangan mengubah collider untuk menyembunyikan clipping visual.

**Selesai:** matriks ≥4 aktor × 5 stroke × 2 sisi contact × 2 arah lapangan pada kontak legal; Serve, grounded/airborne yang memang didukung, walking/running/side/back/stop, low lunge, miss, pause/replay/save/load dan Home reset. Rekam tiap kelompok yang tak muncul alami sebagai **controlled fixture**, lalu periksa pertandingan natural terpisah. Semua pemeriksaan transform dan kontinuitas gerak lolos; bukan hanya nama `CurrentClip` benar.

### F — GOR, ofisial, penonton dan audio pertandingan

**Kondisi sekarang:** `BuildArena` sudah memiliki 48 penonton (2 sisi × 3 baris × 8), umpire/kursi, service judge, papan skor dan kit bag. Mereka bukan fitur yang hilang. Namun crowd berbagi pose baked dan cheer menggoyang root; `UmpireSignal` adalah geometri terpisah (`Assets/Scripts/CourtArena.cs:60–146`, `Assets/Scripts/CourtArena.cs:148–166`). Audio menggunakan rekaman impact/room/applause existing serta SFX procedural (`Assets/Scripts/CourtAudio.cs:16–88`). Nilai kualitas suara belum dibuktikan dengan mendengar.

**Perubahan minimum:**
1. Pisahkan lapisan baca: lantai/court, atlet/net/shuttle, tribun/dinding. Atur key light/material/warna existing agar siluet jelas tanpa arena hitam atau overexposure. Raket, shuttle dan garis tetap terbaca saat kamera bergerak. Jangan menambah volumetric fog, ray tracing, post-process stack atau scene baru. Baked lighting bukan solusi otomatis untuk venue yang dibuat runtime.
2. Tetap GOR lokal, bukan stadion raksasa. Bench, kursi wasit, pagar, tribun dan papan skor membentuk ruang yang meyakinkan; rapikan prop yang melayang/berpotongan, arah teks dan skala. Jangan memenuhi court dengan dekor atau sponsor palsu.
3. Pilot crowd: batasi 2–4 figur foreground rigged dari prefab existing untuk tepuk tangan/angkat lengan/lean; crowd jauh tetap mesh/material shared, variasi pose duduk dibuat sekali, bukan BakeMesh setiap frame. Fase start reaksi berbeda per orang. Profil sebelum memperluas; tidak memasang Animator per seluruh 48 penonton. Angka ini budget kandidat, bukan pengukuran performa final.
4. Ganti sinyal wasit terpisah dengan gerak lengan yang tersambung skeleton ofisial existing; jangan mengklaim semua gesture umpire realistis tanpa referensi. Gerak hanya presentasi, bukan sumber keputusan score. Service judge bereaksi pada kejadian relevan yang memang ada; tidak menciptakan fault baru demi animasi.
5. Pakai kejadian/state existing untuk idle perhatian, rally intens, poin biasa, game point/match point dan hasil. Reaksi dan sound tepat sekali pada poin terkonfirmasi; reload/snapshot tidak menyulut applause ulang. Challenge overturned memperbarui pihak yang bereaksi, bukan menciptakan kemenangan kedua. Tidak perlu event bus baru.
6. Footstep berasal dari fase kaki mendarat saat perpindahan nyata, bukan timer kontinu; bedakan landing jump dan langkah. Terapkan pada empat atlet dengan attenuation sederhana; tidak bersuara saat kaki airborne/diam/pause. Audio hit hanya pada kontak sah, miss hening, duplicate event tidak menggandakan impact.
7. Mix existing: room rendah saat servis, rally sedikit meningkat, impact selalu jelas, applause lebih kuat pada poin penting/akhir match lalu turun sebelum servis berikut. Net/Drop tidak terdengar seperti Smash; volume 0 benar-benar diam; pause/replay tidak mengubah/duplikasi skor atau applause. Tidak musik/komentator, mikrofon, rekaman desktop, atau suara dari aplikasi lain.
8. Step/net/whistle/score procedural tetap **baseline placeholder**, bukan realistis. Jika listening gagal, ajukan rekaman legal spesifik dengan izin komersial, sumber, lisensi, ukuran dan biaya Rp0. Tanpa izin aset baru, laporkan gap audio, jangan menyebut synth sebagai pengganti setara. Lisensi existing tercatat di `ASSET_REGISTER.csv:3–8`; audit sumber kembali jika mengubah sumber/aset.

**Tes dahulu:** `MobilePresentationTests.ArenaHasOfficialsSeatedAudienceAndLiveScoreboard`, `GameWinningPointTriggersArenaReactionDespiteScoreReset`, `PhysicalScoreboardStaysReadableFromActualMatchCamera`, `FeedbackTests.CrowdUsesRecordingAndRespondsOncePerPointWithPauseDucking`. Tambah foot-contact once-only, silence airborne, empat aktor, mute, pause/replay/load/call correction. Tes `SeatedAudienceHasKneesForwardFeetDownAndHandsOnLap` dipertahankan untuk pose idle; tambah invariant cheer, jangan menuntut tangan tetap di pangkuan saat bertepuk.

**Selesai:** papan fisik dan HUD konsisten; ofisial tidak terlepas dari kursi/tubuh; penonton tidak serempak bergoyang seperti satu blok. Video 1× menunjukkan dunia merespons poin, servis tenang, hasil lebih kuat. Listen isolated impact + mix in-game pada headphone/speaker yang diizinkan; simpan penilaian audible terpisah dari validasi PCM. Tanpa kemampuan mendengar yang nyata, status `AUDIBLE_REVIEW_PENDING`, bukan PASS. Tidak menambah props tanpa pemeriksaan biaya CPU/draw calls/memory.

### G — Kamera, replay, momentum dan alur pertandingan

**Kondisi sekarang:** kamera dari belakang mengikuti tinggi jump player dan melakukan snap antarujung agar tidak melintasi court; timer poin 3 detik dan flow manual sudah tersedia (`Assets/Scripts/CourtPrototype.cs:625–628`, `Assets/Scripts/CourtFlow.cs:8–30`). Home reset dan change-ends sudah punya regresi; jangan membukanya lagi secara tidak sengaja.

**Perubahan minimum:**
1. Pisahkan target framing Home/Setup, gameplay, replay dan result melalui state yang ada. Home dekat tidak boleh membuat servis dimulai dengan kamera atau posisi atlet preview. Pertahankan kamera belakang bertinggi sedang saat rally; kamera tidak mengorbit mendadak mengikuti shuttle.
2. Atur framing/zoom yang ringan berdasarkan batas subjek terukur: badan/raket player, shuttle legal dan sisi net relevan tetap tampak pada jump-smash, lob tinggi, dekat garis, serta ganda. Laju smoothing dalam waktu, bukan frame. Uji kedua ujung dan semua aspect ratio C. Jika perlu zoom-out, lakukan halus tanpa menipu kedalaman.
3. Cut sinematik hanya pada state non-rally yang sesuai. Intro maksimum usulan 1–1,5 detik dan dapat dilewati melalui aksi lanjut existing; tampilkan identitas tim/format. Jangan menambah cutscene wajib setiap poin atau menghentikan simulation untuk efek hit. Tidak hit-stop, slow-motion live, atau camera shake baru pada fase ini.
4. Replay mempertahankan tiga mode; pakai data yang direkam, tidak simulasi ulang. Jangan kehilangan pose IK/foot plant/jump atau empat aktor; skip mengembalikan live transform, camera, timer dan RNG tepat. Tidak memainkan kontak replay sebagai kontak gameplay baru.
5. Poin, game-end/interval/change-ends dan hasil menggunakan visual/mix berbeda tetapi state logic tetap. Pertahankan kesempatan Challenge dan deadline 3 detik; jangan mempercepat timer untuk lolos durasi Easy. Ready tidak otomatis men-servis pemain. Kamera akhir match memperlihatkan reaksi tanpa mengganti tim/pemenang.

**Tes dahulu:** `MobileUiTests.ChangeEndsCameraClearsVenueAndMenuRestoresHomeView`, `MenuReturnResetsSwappedAthletesToNeutralPreview`, `FeedbackTests.PointWindowSurvivesReplayPauseSaveAndAdvancesExactlyOnce`, `AutomaticPointWindowPreservesGameIntervalAndMatchEnd`, snapshot/replay empat atlet. Tambah assertions viewport subjek pada momen ekstrem dan race pause/menu/skip/change-ends.

**Selesai:** tidak ada court-wall occlusion, shuttle hilang karena UI/framing, kamera menembus penonton/net, atau lompatan visual akibat perpindahan state. Uji alur lengkap dan inspect video; posisi kamera valid saja belum membuktikan nyaman.

### H — QA final, rekaman utuh dan APK lokal

**Kondisi sekarang:** bukti 19/19 + 95/95 adalah baseline removal, bukan hasil polish. `MobileMatchTests` menyediakan natural singles 9 seeded match, doubles 3 difficulty dan capture audio; capture singles lama 15fps tidak cukup untuk menyatakan timing kontak halus (`Assets/Tests/PlayMode/MobileMatchTests.cs:113–178`, `Assets/Tests/PlayMode/MobileMatchTests.cs:313–419`, `Assets/Tests/PlayMode/MobileMatchTests.cs:424–493`).

**Pelaksanaan:**
1. Freeze final Assets/Packages/ProjectSettings **termasuk tes**; jalankan seluruh EditMode dan PlayMode non-Capture. XML per run unik, daftar kasus/count, exit/log dan hash before/after wajib. Jika ada source/test edit sesudahnya, rerun cakupan terdampak; jangan menyebut semua run identik tanpa manifest.
2. Jalankan natural 9 seeded singles dan 3 doubles existing. Tambah save/pause/replay boundary coverage yang belum ada tanpa mengganti aturan. Easy <1000 tetap gate existing; jika masih gagal, status final regression PARTIAL/FAIL. Diagnosis root cause dahulu; tuning gameplay membutuhkan persetujuan terpisah, bukan mengubah threshold atau membuat AI sengaja kalah.
3. Rekam pertandingan **1× utuh**, bukan montase: singles Easy/Normal/Hard dan ganda Normal dari Home sampai Result dan kembali Home. Doubles Easy/Hard tetap numeric natural; jika masalah/keunikan hanya terjadi di sana, tambahkan capture kasus itu. Jalankan skenario replay aktif dan challenge secara terpisah bila capture natural existing mematikan replay. Jangan mengklaim cakupan dari kejadian yang tidak muncul.
4. Capture 30fps sasaran, log timestamp nyata, jumlah frame, selisih antarcapture, realSeconds/gameSeconds, sample rate/channels audio. Bedakan FPS capture dari FPS render. Jika throughput rendah, tandai kualitas bukti terbatas dan lakukan potongan audit pendek pada cadence memadai; tidak menggandakan frame atau interpolasi untuk mengaku 30fps nyata.
5. Controlled capture: semua stroke/arah, footwork, jump, landing, grip dan reaksi. Tandai saat fixture mengatur shuttle/root; itu bukan natural interception. Inspeksi 1× untuk rasa, 0,25× tanpa optical-flow untuk cacat, dan frame rapat sekitar contact. Catat timestamp sebelum/sesudah setiap bug penting.
6. Audio ditangkap dari `MobileGameAudio` listener game saja. Mux memakai clock capture sebenarnya, bukan memaksa sinkron dengan durasi rekaan; cek awal, beberapa impact tengah dan akhir. Decode seluruh MP4/PCM, lalu **dengar** mix. Media valid/nonzero waveform bukan audible acceptance.
7. Setelah QA lokal cukup, buat APK baru dengan `AndroidBuild.RunMobile`. Backup output lama karena path fixed. Verifikasi package, ARM64, landscape, min SDK, izin offline, debug flags, signature, bytes, SHA-256 dan input freeze. Perbaiki verifier existing agar menerima run/input manifest baru dengan argumen minimum; jangan menjadikan `final-source4.json` pembanding build baru. Scene Boot tidak di-regenerate bila sudah ada; catat delta settings dari build hook dan ulangi bila shipped inputs berubah.
8. Sinkronkan hasil aktual di TESTING/ERROR_LOG/roadmap/handoff. Pemilik menilai pengalaman **modul exhibition utuh**, tidak diminta menjalankan QA granular agent. Tidak install atau tes HP. APK lokal boleh berstatus packaging verified, tetapi jangan klaim release/30 FPS Android/ergonomi/performance sudah diterima.

**Selesai tahap lokal:** requirement UI+motion+arena+audio+flow punya bukti source-bound, tidak ada regresi kritis, masalah Easy ditutup dengan bukti atau tetap jelas sebagai blocker, hasil packaging benar, save/prefs pulih hash-identik. `Device QA` dan `Owner Acceptance` tetap status terpisah, tidak otomatis PASS.

## 4. TESTING, PERINTAH DAN BUKTI

### 4.1 Matriks penerimaan

| Area | Bukti wajib | Bukan bukti pengganti |
|---|---|---|
| UI/identitas | Screenshot seluruh state, bounds/text/contrast, pointer routing, empat viewport + safe area | Satu Home screenshot atau nama style |
| Motion/contact | Pilot + matriks gerak + 1×/0,25×, error contact/bone/root/plant terukur | Clip bernama Smash, bone assertions saja |
| Ganda | Empat aktor aktif, servis/receiving/rotasi, pose/replay/save, 3 natural difficulty | Empat mesh berdiri di court |
| Kamera | Framing momen ekstrem, change-ends/replay/Home round-trip | Transform kamera tanpa melihat video |
| Arena/audio | Reaksi sesuai event, listen mix game, sync kontak, no duplicate/miss sound | Mesh/audio clip tersedia, WAV nonzero |
| Data | Snapshot lama/baru, invalid atomicity, prefs/save hash restored | Fixture prefs PASS saja |
| Packaging | APK nyata + manifest/signature/hash + source freeze | Build exit 0 atau PlayerSettings saja |
| Performa | PC frame-time/CPU/draw calls/Unity allocated memory, capture overhead dipisah | Application.targetFrameRate atau ukuran RAM perangkat |
| Android | Tetap `DEFERRED_OWNER_PC_ONLY` | Screenshot desktop, PC FPS atau APK parse |
| Owner | Review satu pengalaman exhibition utuh | Persetujuan plan/desain saja |

Target produk tetap 30 FPS pada Android minimum RAM perangkat 4 GB dan keseluruhan game ≤1 GB (`docs/01_GDD.md:40–43`, `docs/TESTING.md:598`). RAM perangkat bukan allowance aplikasi. Ukuran APK bukan total footprint terpasang. Saat PC-only, ukur paket/payload dan biaya PC; installed footprint, thermal, touch/native dan qualifying-device performance tetap belum diuji. Jangan membuat ambang drop/pacing baru seolah dipilih pemilik.

### 4.2 Perintah lokal — contoh eksekusi SETELAH izin, bukan sudah dijalankan

Shell host: Git Bash; path untuk native executable harus `C:/...`, bukan `/c/...`. Variabel berikut dipasang pada sesi eksekusi, folder run unik wajib belum ada. Semua langkah proteksi A selesai sebelum membuka Unity. Proses Unity proyek ini harus tidak sedang dipakai pemilik; jangan mematikannya paksa.

```bash
UNITY='C:/Program Files/Unity/Hub/Editor/6000.3.18f1/Editor/Unity.exe'
PROJECT='C:/Users/Dewangga Ramadhan H/Documents/Portofolio/JustSmash'
RUN="$PROJECT/evidence/shareable/$(date +%Y%m%d)/roller-polish-$(date +%H%M%S)"
mkdir -p "$RUN"

# Buka Editor lokal bila dibutuhkan; tidak melakukan input native.
"$UNITY" -projectPath "$PROJECT"
```

Jalankan runner ketika Editor interaktif proyek sudah ditutup secara aman. Jangan menjalankan perintah Editor interaktif dan runner bersamaan. Melalui tool, tugas panjang gunakan `background=true, notify=true`. Tests tidak memakai `-quit`; runner mengelola shutdown.

```bash
# EditMode lengkap
"$UNITY" -batchmode -nographics -projectPath "$PROJECT" \
  -runTests -testPlatform EditMode \
  -testResults "$RUN/edit.xml" -logFile "$RUN/edit.log"

# PlayMode non-Capture; Capture berisi satu tes native yang dilarang.
"$UNITY" -batchmode -projectPath "$PROJECT" \
  -runTests -testPlatform PlayMode -testCategory '!Capture' \
  -testResults "$RUN/play.xml" -logFile "$RUN/play.log"

# UI + controlled motion capture, visible Game View, tanpa -batchmode/-nographics.
"$UNITY" -projectPath "$PROJECT" -runTests -testPlatform PlayMode \
  -testFilter 'MobileMatchTests.FullUiAndGameAudioCaptureSmoke|MobileMatchTests.DoublesUiFlowCaptureUsesActualRuntimeScreens|MobileMatchTests.CaptureControlledFootworkStrokesAndJumpAtOneTimespeed' \
  -testResults "$RUN/capture-controlled.xml" -logFile "$RUN/capture-controlled.log"

# Natural capture 1x. Jalankan sesudah output harness sudah diarahkan ke run unik.
"$UNITY" -projectPath "$PROJECT" -runTests -testPlatform PlayMode \
  -testFilter 'MobileMatchTests.CaptureOneTimespeedMatchesWithGameAudio|MobileMatchTests.CaptureNaturalDoublesMatchWithGameAudio' \
  -testResults "$RUN/capture-natural.xml" -logFile "$RUN/capture-natural.log"

# APK hanya sesudah gate lokal; backup APK/log lama dahulu.
"$UNITY" -batchmode -quit -projectPath "$PROJECT" -buildTarget Android \
  -executeMethod AndroidBuild.RunMobile -logFile "$RUN/build.log"
```

Dukungan negasi `!` filter tersedia di Test Framework terpasang (CHANGELOG paket editor, bagian 1.1.17). Periksa nama test/category ketika mengeksekusi karena source dapat berubah. **Jangan memilih seluruh kategori Capture:** `MobileMatchTests.NativeMouseAndKeyboardUseGameView` tetap tidak boleh dijalankan/bypass. Capture memerlukan `WaitForEndOfFrame`/Game View; `Camera.Render()` tidak merekam IMGUI.

Perintah di atas bukan pengganti penyesuaian output capture pada A: argumen `-testResults` hanya mengatur XML, **tidak** mengubah `CaptureEvidence` atau semua `File.Write*` di test. Audit guard `Application.isBatchMode`: beberapa tes di luar kategori Capture melewati screenshot melalui `yield break`/conditional dan dapat tercatat PASS tanpa bukti visual. Daftar capture wajib harus dibuktikan file + inspeksi secara non-batch; skipped=0 dalam XML tidak cukup. Jangan menjalankan `WaitForEndOfFrame` tanpa guard di batch.

Periksa XML dengan stdlib, bukan exit code saja:

```bash
python -c 'import sys,xml.etree.ElementTree as E; r=E.parse(sys.argv[1]).getroot(); cases=r.findall(".//test-case"); print({k:r.get(k) for k in ("result","total","passed","failed","skipped","inconclusive")}); bad=[(c.get("fullname"),c.get("result")) for c in cases if c.get("result")!="Passed"]; print("NON_PASS",bad); assert cases and len(cases)==int(r.get("total")) and not bad and r.get("result")=="Passed"' "$RUN/edit.xml"
```

Ulangi parser untuk setiap XML; jika suite memang gagal known issue, simpan kegagalan dan beri status benar, jangan ubah parser menjadi lulus. Jumlah test baru dapat berubah; jangan mengunci 19/95 sebagai target palsu.

### 4.3 Paket bukti final

Dalam folder run unik: manifest source sebelum/sesudah, XML/log RED dan final, screenshot state, MP4+game PCM/mux, timeline/capture metadata, natural-match result, ukuran/hash/manifest/signature APK. Bukti review berupa timestamp dan temuan pada TESTING/ERROR_LOG existing, bukan worklog baru. Jangan menimpa failed-run atau evidence tanggal lama. Redaksi credential pada salinan shareable, bukan menghapus bukti kegagalan.

Status harus dilaporkan terpisah:
- `IMPLEMENTED` — kode fitur ada.
- `LOCAL_REGRESSION_PASS` — suite benar-benar lolos pada manifest final.
- `VISUAL_MOTION_REVIEWED` / `AUDIBLE_REVIEWED` — inspeksi nyata dilakukan dan cacat yang ditemukan ditutup.
- `APK_PACKAGING_VERIFIED` — artifact nyata diperiksa.
- `NATIVE_INPUT_NOT_RUN_PERMISSION_DENIED` dan `DEFERRED_OWNER_PC_ONLY` — tetap sampai izin berubah.
- `OWNER_ACCEPTANCE_PENDING` — belum dinilai pemilik sebagai pengalaman utuh.

## 5. JANGAN DISENTUH

- Career runtime/menu/data, Quaternius audit dan jalur outfit yang ditutup; backup removal tidak dipakai menghidupkan fitur kembali.
- Jangan edit `docs/AI_RULES.md` atau overwrite `docs/UI_REFERENSI.md`. Jangan membuat ulang `docs/WORKFLOW.md` yang telah dihapus. Tidak bypass penolakan lewat script/tool lain.
- Rules Official15, best-of-three/deuce/cap/change ends, grade/stamina D-049/D-050, shot buffer/reach, drag/flight, root movement dan AI difficulty. Perubahan bug gameplay terpisah memerlukan keputusan, bukan kedok polish.
- Semua toggle, bahasa/aksi kontrol yang telah disetujui, save/prefs user dan snapshot backward compatibility.
- Packages/pipeline, layanan jaringan/analytics/cloud, musik/komentator, akun/billing, native input, HP/emulator, proyek lain, remote Git.
- `.backups/`, evidence historis, source dirty/untracked pemilik. Tidak reset massal, `git clean`, penghapusan Library, atau reimport besar untuk mencoba keberuntungan.

## 6. RISIKO DAN RENCANA CADANGAN

| Risiko | Deteksi | Tindakan / batas |
|---|---|---|
| Native clips+IK tidak mencapai motion target | Pilot D tetap snap, gerak tanpa berat, foot sliding | Stop setelah 3 percobaan cacat sama; ajukan pendekatan/aset berlisensi spesifik. Tidak tambah VFX untuk menyembunyikan |
| Immediate hit bertabrakan dengan windup panjang | Input-to-contact tertunda atau raket belum sampai | Pertahankan hit; siapkan ready pose antisipatif dan blend visual terbatas. Bila tidak cukup, laporkan konflik, jangan ubah kontrol diam-diam |
| Font built-in tidak menghasilkan identitas yang diinginkan | Storyboard dinilai generik / text fit buruk | Ajukan satu kandidat font legal; tanpa izin, jangan impor atau klaim typography final |
| Model casual membatasi tampilan atlet | Siluet/outfit tidak sepadan target | D-048 tetap; laporkan batas. Tidak mengulang proyek pakaian tanpa keputusan baru |
| Crowd/ofisial menambah biaya besar | Profil draw calls/CPU/memory dibanding baseline | Batasi foreground articulated, sisanya shared mesh; jika masih gagal, tahan perubahan crowd, bukan turunkan setting user diam-diam |
| Audio procedural tidak terdengar badminton | Listening menemukan step/net/whistle artificial | Ajukan rekaman legal; tanpa izin/rekaman cocok, tandai audio unfinished |
| Menu staging merusak match/restore | Home/camera/actor positions bocor ke Serving | Pisahkan pose preview dan reset lewat jalur existing; round-trip tests sebelum luas rollout |
| Temporal animation tidak cocok replay/save | Resume pose snap, plant/timer berbeda | Utamakan state derivable; jika perlu field baru, validate+default legacy dan uji before-mutation |
| Easy natural masih >1000 detik | Repro source-bound | Bedakan harness vs gameplay. Jangan longgarkan gate; minta keputusan tuning bila diperlukan |
| Capture terlalu jarang / drift audio | Timestamp gap, clock/PCM berbeda, impact tidak sinkron | Audit capture test-only dan run ulang segmen; jangan retime agar tampak normal atau klaim capture FPS sebagai render FPS |
| Scope PC-only tidak membuktikan Android | Tidak ada physical runtime/performance data | Laporkan PC dan packaging saja. Jangan minta ulang izin HP tanpa arahan pemilik |

Rollback per task: arsipkan file/evidence gagal, restore hanya path dari manifest slice setelah memeriksa tidak ada edit pemilik sesudah backup, verifikasi hash dan regresi terdampak. Jangan membuang test/bukti untuk menghasilkan status hijau.

## 7. BATAS PENYELESAIAN PLAN

Plan selesai ketika file ini memuat arah, source anchors yang valid, task/tes/risiko/rollback dan batas izin; **bukan** ketika game dianggap selesai. Seluruh kriteria pada task B–H masih pekerjaan usulan. Persetujuan eksekusi tidak otomatis menyetujui aset/dependency/biaya/device baru. Gate visual preview dan perubahan pendekatan yang material tetap dihormati.

Langkah pertama setelah pemilik mengizinkan eksekusi: A, lalu preview B. Tidak mulai career, tidak build APK dari source lama untuk memberi kesan task polish selesai.

## Sources

[3] https://interfaceingame.com/screenshots/roller-champions-main-menu
[4] https://interfaceingame.com/screenshots/roller-champions-game-mode-selection
