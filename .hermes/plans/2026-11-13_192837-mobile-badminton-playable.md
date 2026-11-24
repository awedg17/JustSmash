# Just Smash — badminton mobile yang benar-benar bisa dimainkan

Tanggal: 2026-11-13. Status: **RENCANA, belum dieksekusi**. Semua kode dan command dalam dokumen ini adalah instruksi untuk eksekutor berikutnya, bukan hasil pengujian yang sudah dilakukan.

## Goal

Menghasilkan APK Android offline, landscape, singles melawan AI tiga tingkat, dengan manusia stylized yang terbaca, gerakan badminton yang jelas, kontrol bersahabat, aturan konsisten, dan UI permainan lengkap—tanpa biaya.

## KONTEKS WAJIB: keputusan terbaru dan batas kerja

Workspace tunggal: `C:/Users/Dewangga Ramadhan H/Documents/Portofolio/JustSmash`. Semua path relatif di bawah berakar di sini. Jangan mengerjakan proyek lain.

| Hal | Keputusan aktif |
|---|---|
| Otoritas desain | Pemilik menyerahkan pilihan visual, animasi, balancing, UI, dan eksperimen kepada agent. Jangan meminta pemilik memilih konstanta, teknik rigging, atau memperbaiki tes. |
| Kamera | Tetap belakang pemain, bukan broadcast samping. Gerakan dibuat lebih terbaca tanpa membengkokkan anatomi. |
| Karakter | Gunakan NPC Casual Set00 yang sudah ada, pilih varian/warna terbaik. Pakaian olahraga custom bukan requirement. Jangan membuka lagi proyek pakaian Quaternius. |
| Mode | Singles offline vs AI: Mudah, Normal, Sulit. Tidak multiplayer, doubles, career, character creator, akun, iklan, toko, cloud. |
| Biaya | Rp0. Tidak membeli aset, berlangganan, memakai paid API/credits, memulai trial berbayar, menyewa mesin, atau memesan artis. Tidak perlu akun baru untuk jalur utama. |
| Privasi | Jangan baca folder pribadi, browser/cookie, kredensial, clipboard, environment dump, sesi proyek lain, registry preferensi user, atau folder di luar workspace. Jangan upload source/aset/log ke layanan luar. |
| Toolchain | Gunakan Unity yang sudah terpasang melalui executable yang sudah diketahui. Itu bukan izin menjelajah profil Windows. Jika engine menuntut aktivasi/login/instalasi atau akses data pribadi baru, hentikan bagian tersebut dan jelaskan blocker. Cache/lisensi OS yang dikelola engine jangan diinspeksi atau diklaim sepenuhnya terkurung workspace. |
| Eksperimen | Pemilik mengizinkan banyak eksperimen. Aturan lama maksimal tiga percobaan tidak menjadi batas global sekarang. Tetap hentikan pengulangan tanpa hipotesis baru; simpan kandidat baik, bandingkan bukti, lalu ubah pendekatan. |
| Subagent | Boleh, bukan wajib. Utamakan solo untuk edit yang saling bergantung. Jika dipakai, parent wajib memeriksa aktivitas, scope, hasil, dan risiko tiap maksimal 5 menit selama child aktif. Lihat protokol di bawah. |
| Workflow/worklog | Hapus `docs/WORKFLOW.md` pada implementasi. Tidak ada file `WORKLOG.md` yang ditemukan dalam inspeksi `docs/`; jangan mengarang path atau menyamakan otomatis `ERROR_LOG.md` dengan worklog. Jangan buat penggantinya berupa ledger/status-harian baru. |
| Bukti | XML tes, capture, hash APK, dan satu ringkasan hasil bukan birokrasi tambahan: bukti diperlukan agar tidak mengklaim kualitas tanpa tes. Simpan raw output dalam `evidence/mobile/`. |
| Git | Lokal saja. Tidak `git push`, reset hard, clean, force checkout, atau memasukkan semua untracked file. Commit kecil setelah audit staged diff dan tes slice. |
| Giliran perencanaan | Hanya dokumen ini boleh ditulis. Tidak menghapus workflow, mengubah kode, membuat aset, menjalankan Unity, build, commit, download, atau mengakses HP sekarang. |

Instruksi terbaru mengganti aturan dokumen lama yang melarang memilih desain, mengunci Easy saja, mewajibkan pakaian baru, menuntut workflow/worklog, atau membatasi eksperimen tiga kali. Aturan keamanan, legalitas, menjaga pekerjaan lama, dan kejujuran tes tetap berlaku. Tidak perlu membuka kembali seluruh milestone lama untuk membuat singles ini selesai.

## Current context / temuan dari source

Nomor baris adalah snapshot inspeksi, gunakan nama method sebagai anchor setelah edit.

| Path dan lokasi | Kondisi nyata | Dampak |
|---|---|---|
| `ProjectSettings/ProjectVersion.txt:1` | Unity `6000.3.18f1`. | Jangan migrasi engine. |
| `Packages/manifest.json:3–37` | Test Framework 1.6.0 dan modul Unity bawaan; tidak ada paket UGUI/Input System/Animation Rigging tambahan. GraphicsSettings memakai built-in pipeline. | Pakai fitur native dan dependency yang ada; tidak perlu URP/HDRP. |
| `Assets/Scripts/CourtPrototype.cs:279–299` | Scene Boot membuat court runtime. | Pertahankan Boot dan public entrypoint yang diuji. |
| `Assets/Scripts/CourtPrototype.cs:366–405` | Lapangan, raket, shuttle, kamera, lighting dibuat runtime. | Perbaiki presentasi, bukan rebuild proyek dari nol. |
| `Assets/Scripts/AthleteVisual.cs:13–64` | Memuat `Resources/AthleteNPC`; Animator dimatikan; `Bone(name)` mengambil nama pertama; hanya sebagian tulang mengikuti mannequin. | Mesh manusia ada, tetapi rig-follow belum berarti gerakan manusia. |
| `Assets/Resources/AthleteNPC.prefab` | Animator punya Avatar, controller kosong. Ada nama tulang berulang seperti `RightHand`, `Hips`, `RightFoot`. | Tidak boleh mengikat skeleton memakai pencarian nama pertama. Validitas Humanoid/ikatan skin perlu dibuktikan di Unity. |
| `Assets/Editor/AthleteVisualSetup.cs:9–25` | Source `Assets/npc_casual_set_00/Prefabs/npc_csl_00_character_01m_01.prefab`; menyalin prefab dan memilih LOD0. | Jadikan sumber tetap, modifikasi hanya prefab turunan/material instance. |
| `Assets/Scripts/CourtMotion.cs:10–35,59–117` | Lengan berbagi satu pola contact/follow-through; backhand terutama twist badan. Kaki diarahkan ke target lokal, bukan plant dunia. | Perlu stroke berbeda, anticipatory pose, plant kaki, takeoff/landing. |
| `Assets/Scripts/CourtPrototype.cs:487–550` | Pukulan langsung, reach silinder; grade numerik tidak mengubah flight; dive menggeser root sampai 0.35m saat hit. | Hilangkan teleport rescue, cocokkan reach dengan raket, sembunyikan grade dari UI normal. |
| `Assets/Scripts/CourtPrototype.cs:552–569` | Jump parabola 0.7 detik; root Y ditulis Update; tidak ada landing phase khusus. | Satukan waktu jump dengan fixed simulation dan animasi. |
| `Assets/Scripts/CourtPrototype.cs:615–638` | AI mengejar posisi shuttle saat ini dan memilih satu return. | Tiga tingkat harus berbeda persepsi/taktik, bukan tiga label. |
| `Assets/Scripts/CourtPrototype.cs:571–612` | Replay hanya menyimpan posisi player/opponent/shuttle. | Tidak cukup untuk replay animasi pukulan; tambah pose/phase atau replay akan menipu. |
| `Assets/Scripts/CourtPrototype.cs:727–885` | IMGUI kotak default dan raw pointer; menu/debug menumpuk di lapangan. | Pertahankan renderer sederhana, ganti layout/skin/state dan cegah input tembus. |
| `Assets/Scripts/CourtFlow.cs:8–30` | Point auto-continue 3 detik; flow warmup/ready/interval/change-ends ada. Summary hardcode Easy. | Pakai flow existing; jangan menggandakan state skor di UI. |
| `Assets/Scripts/SinglesMatch.cs:21–35` | 15 poin, selisih dua, cap21, best-of-three; game ketiga change-ends pada8. | Pertahankan preset existing dan beri label `15 poin`, bukan klaim aturan resmi terkini. |
| `Assets/Scripts/BootDiagnostics.cs:33–35` | `moveDrain` harus >0, difficulty hanya Easy. Resource JSON juga `moveDrain:1`, meski gerak tidak dipotong pada runtime kini. | Bersihkan konfigurasi yang menyesatkan, validasi difficulty tiga pilihan. |
| `Assets/Scripts/CourtPrototype.cs:221–237`, `Assets/Scripts/CourtDiagnostics.cs:13–46` | Save/report di persistentDataPath; preferensi via PlayerPrefs. | Sebelum menjalankan host QA, alihkan data host ke workspace dan hindari membaca PlayerPrefs Windows. |
| `Assets/Editor/AndroidBuild.cs:11–46` | RunProduction membuat APK non-development ARM64; scene existing tidak ditimpa. | Reuse build path, lakukan source-bound final build. |
| `docs/TESTING.md:1–11` | Riwayat mengklaim 102/102 Play +10/10 Edit; APK 29,652,078 byte. | Riwayat, bukan hasil giliran ini atau bukti kualitas gerakan. Jangan copy label production-ready. |
| `Assets/Tests/PlayMode/LocalGateTests.cs:704–743` | Tiga pola input, timeScale15, NextPoint manual, jarak total menghitung perubahan posisi termasuk reset poin. | Bukan tiga seed AI sebenarnya, bukan video real-time, bukan pengukuran footwork murni. Perbaiki bukti. |
| `ASSET_REGISTER.csv:3–4` | NPC EULA embedded-only; racket sound BigSoundBank CC0 sudah ada. | Tidak perlu aset/persona/audio berbayar; jangan distribusikan raw NPC. |

Git berisi banyak perubahan dan source/assets untracked. Jangan rollback ke satu commit lama: itu dapat membuang game existing. `graphify-out/graph.json` tidak ditemukan; jangan membuat graph/cache untuk task ini.

## Architecture / proposed approach

Pertahankan Unity built-in, physics shuttle fixed-step, `SinglesMatch`, dan scene Boot. Pisahkan hanya ownership yang perlu: `CourtPrototype` tetap otoritas posisi/kontak/skor, `AthleteVisual` menjadi driver skeleton asli dengan clip native Unity dan IK terbatas, sedangkan partial `Assets/Scripts/CourtUi.cs` mengatur menu/HUD tanpa menduplikasi aturan pertandingan. Pakai input intent yang toleran namun tidak mengubah jenis pukulan diam-diam; AI menggunakan aturan kontak dan kapasitas fisik yang sama.

### Kontrak rasa permainan

- Tombol ditekan: feedback visual pada frame berikutnya, bukan menunggu hasil hit.
- Ketukan saat shuttle sudah dalam reach: pukul pada langkah simulasi berikutnya. Ketukan terlalu awal: antre maksimal **0.16s**; bila belum reachable ketika kedaluwarsa, tampilkan `Terlalu jauh`/`Terlalu rendah`, jangan auto-hit beberapa detik kemudian.
- Satu input menghasilkan paling banyak satu pukulan. Smash tetap Smash; tidak diam-diam diganti Clear. Backhand adalah sisi eksekusi, bukan jenis shot keenam.
- Lima jenis: Smash cepat menurun, Lob/Clear tinggi jauh, Drop pelan ke depan, Net pendek dekat net, Lift underhand tinggi keluar tekanan. Shot label harus sesuai flight dan pose.
- Arah kiri/kanan dan enam sektor tetap ada. Netral tidak otomatis selalu menembak kanan: pakai arah aim terakhir yang disengaja; setelah reset poin gunakan tengah lawan. Assisted membantu kaki saja; manual input movement selalu mengalahkan bantuan saat mode manual.
- Manual sebagai default, Assisted tersedia di Pengaturan dan Latihan. Fixed/Floating tetap ada. Tidak ada auto-teleport, reach tak terbatas, auto-aim yang mengganti target, atau power boost AI.
- Hilangkan grade `Excellent/Good/Fair/Poor` dari HUD biasa; telemetri internal boleh tetap untuk regresi. Tidak ada minigame timing/akurasi, reticle wajib, atau punishment acak pada shot valid.
- Pertahankan stamina baseline sekitar70% setelah moving rally30s, jalan/lari drain0, minimum movement70%. Tidak ada kondisi stamina0 yang membuat semua input pukulan ditolak. Samakan fatigue lawan.
- Default penentuan IN/OUT akurat. Simulasi sengaja salah-call3% tidak aktif di pertandingan baru. Challenge/replay existing tetap tersedia; jalur legacy salah-call tetap diuji tanpa menjadi perilaku default.
- Servis tetap short/flick, arah diagonal dan parity benar; charge1s opsional. Ketukan `Servis pendek` menjalankan power0.2, tahan tombol servis mengisi hingga flick. Tidak wajib presisi1detik untuk memulai rally.

### Target visual yang tidak boleh diterjemahkan sesuka hati

**Karakter:** manusia stylized dewasa, kepala dengan wajah/rambut, bahu tersambung, siku/lutut menekuk, tangan mencengkeram raket, dua kaki dengan sepatu. NPC kaos/celana existing diterima. Dilarang menampilkan capsule/cube/sphere sebagai kulit pemain final; proxy collision boleh ada tetapi render OFF. Jangan mewarnai kulit/rambut ikut warna baju.

**Warna:** pertahankan arah proyek turquoise/abu. Court `#246F66`, luar court `#173A38`, garis `#ECF4ED`, UI panel `#14262D`, UI text `#F4F8F9`, muted `#BDCDD0`, accent `#26D0C3`. Player kaos teal terang, opponent coral/orange; kulit, rambut, celana tetap natural. Warna label/skor dan posisi memberi pembeda tambahan, tidak hanya warna.

**Venue:** satu hall sederhana, floor matte, dinding gelap tenang, lampu dari atas/depan, bayangan kontak lembut. Tidak perlu crowd model, shiny floor, bloom, depth of field, motion blur, raytracing, logo sponsor, atau banyak light real-time. Raket punya handle/frame/string bed yang terbaca, bukan bola putih padat.

**Kamera:** pertahankan anchor awal `(0,5.2,-14.2)`, FOV48, LookAt `(0,1,-1)` sebagai baseline. Boleh kalibrasi untuk keterbacaan; POV selalu belakang pemain pada kedua ends. Tidak mengguncang/berputar saat tiap smash; zoom-out kecil hanya bila perlu agar kepala/raket/shuttle tidak terpotong. UI tidak boleh menutup contact zone.

**Animasi:** bukan semua stroke satu ayunan. Forehand menampilkan bahu terbuka dan follow-through menyilang; backhand siku memimpin di sisi non-raket, punggung tangan/raket berbalik; smash overhead cepat dengan badan menutup; lob lebih panjang ke atas; net/lift datang dari bawah. Jump menunjukkan tekuk lutut, takeoff, apex, descent, compression landing, recovery—tanpa melayang setelah kaki menyentuh lantai.

### UI final: enam tampilan, satu renderer

Gunakan IMGUI yang sudah ada dengan GUIStyle/texture warna solid yang dibuat sekali, bukan Unity default skin. Ini keputusan sengaja untuk enam tampilan tanpa package baru; jangan migrasi UI Toolkit/UGUI di tengah pengerjaan hanya demi framework. Native font Unity digunakan berukuran besar/weight berbeda; tidak memakai monospace, engraving, dashboard proyek lain, emoji, glass/neon, tagline, atau tombol dekoratif.

Koordinat desain di bawah memakai safe-area logical height720. `scale=min(safeWidth/1280,safeHeight/720)`, `W=safeWidth/scale`, `H=safeHeight/scale`. Semua Rect diturunkan dari W/H; jangan memakai720 untuk Y pada layar yang memiliki H berbeda.

| Tampilan | Konten dan placement |
|---|---|
| Home | Court manusia sebagai background diam. Panel kiri `(40,100,360,440)`: judul JUST SMASH, `Main`, `Latihan`, `Pengaturan`, `Kredit`. Tidak langsung masuk training debug. |
| Match setup | `Tunggal`, pilihan `Mudah / Normal / Sulit`, label `15 poin • terbaik dari 3 game`, tombol `Mulai`, `Kembali`. Normal default; pilihan tersimpan lokal. |
| HUD rally | Score compact top-center `(W/2-150,20,300,72)`, pause top-right `(W-104,20,80,64)`, energy bar kecil di bawah skor. Tidak ada Save/Load/Restart/Credits/Feed memenuhi court. |
| Controls | Joystick center `(128,H-132)`, radius92. Lima shot di grid kanan, Rect(i)=`(W-324+(i%3)*100,H-310+(i/3)*90,92,80)`. Jump `(W-220,H-116,92,80)`. Primary labels `Smash`, `Lob`, `Drop`, `Net`, `Lift`; text22–26. Serve mengganti area shot selama giliran servis, bukan menambah tombol menutup court. |
| Point/interval/result | Banner point pendek di top-center, jenis fault jelas. Setelah3s siap servis otomatis, bukan auto-servis player. Interval/change-ends satu tombol `Lanjut`; MatchEnd panel tengah skor game + `Main lagi`/`Menu`. Tidak menampilkan `Warmup`, `flowStage`, `Official15` sebagai istilah UI. |
| Pause/settings/credits | Modal opaque menahan input gameplay. `Lanjut`, `Pengaturan`, `Simpan`, `Muat`, `Ulangi pertandingan`, `Menu`. Restart/menu ketika match aktif meminta konfirmasi; save gagal tidak membuang match. Settings: Manual/Assisted, Fixed/Floating, Replay OFF/Gameplay/Angle, trail/shadow warna+brightness, sound volume, batas30/60FPS. Credit scrollable dengan lisensi existing. |

Tiap area interaktif minimal48dp setelah konversi DPI; jika DPI tak valid gunakan safe fallback160 dan uji di HP. Padding16–24 logical px, gap8+, sudut4–8px, border1–2px. Shadow/pressed offset kecil; tidak perlu animasi menu besar. Tombol pressed harus tetap dikenali saat pointer keluar; cancel tidak men-trigger shot/serve. Ukur safe-area dan overlap, bukan sekadar screenshot satu rasio.

## Execution method: microtasks, TDD, evidence

Task A–F adalah bundle. Pecah tiap bundle menjadi subtask bertanda .1 test (3–5 menit), .2 run RED dan inspeksi (2–3 menit aktif), .3 satu diff/keypose (3–5 menit), .4 run GREEN dan inspeksi (3–5 menit aktif), .5 commit lokal (2 menit). Ulangi .3–.4 per fungsi/clip, jangan menggabungkan refactor, balancing dan visual dalam satu diff. Lama compile, suite, encode, dan build tidak termasuk; angka pada heading adalah satu slot fokus, bukan estimasi seluruh bundle. Pekerjaan artistik memerlukan banyak slot; tidak ada janji semua animasi selesai dalam lima menit.

Untuk setiap perubahan kode: tulis test behavior terlebih dahulu, jalankan untuk melihat RED karena alasan benar, implementasikan perubahan minimum, jalankan GREEN dan suite terdampak, audit diff, commit lokal. Test baru yang langsung PASS adalah coverage baseline; jangan membuat bug agar RED. Eksperimen visual memakai before capture yang dinilai gagal, perubahan tunggal, after capture dengan skenario sama; tes numerik tidak menggantikan inspeksi motion.

### Command tes standar (Bash Windows, executable native path)

Jalankan dari root workspace. Command ini untuk implementasi nanti. Jangan pakai `-quit` pada Unity test runner; jangan `-nographics` untuk tes render. Jika Editor project ini sedang terbuka, gunakan Test Runner Editor atau minta user menutup/simpan; jangan paksa kill.

```bash
UNITY='C:/Program Files/Unity/Hub/Editor/6000.3.18f1/Editor/Unity.exe'
PROJECT='C:/Users/Dewangga Ramadhan H/Documents/Portofolio/JustSmash'
RUN="$(date +%Y%m%d_%H%M%S)"
mkdir -p "evidence/mobile/$RUN"
"$UNITY" -batchmode -projectPath "$PROJECT" -runTests -testPlatform EditMode -testResults "$PROJECT/evidence/mobile/$RUN/edit.xml" -logFile "$PROJECT/evidence/mobile/$RUN/edit.log"
```

PlayMode command, pada proses terpisah setelah EditMode selesai:

```bash
"$UNITY" -batchmode -projectPath "$PROJECT" -runTests -testPlatform PlayMode -testResults "$PROJECT/evidence/mobile/$RUN/play.xml" -logFile "$PROJECT/evidence/mobile/$RUN/play.log"
```

Untuk slice, tambahkan `-testFilter 'NamaClass.NamaTest'` dan gunakan nama XML/log unik `Txx-red`, `Txx-green`. Tool terminal untuk proses panjang memakai `background=true, notify=true`; source/test dibekukan sampai proses selesai. Jangan menjalankan dua Unity pada workspace yang sama.

Baca hasil, jangan menganggap exit0 cukup:

```bash
python -c 'import sys,xml.etree.ElementTree as E; r=E.parse(sys.argv[1]).getroot(); cases=r.findall(".//test-case"); bad=[(x.get("fullname"),x.get("result")) for x in cases if x.get("result")!="Passed"]; print("TOTAL",len(cases),"PASS",len(cases)-len(bad),"BAD",len(bad)); print(*bad,sep="\n"); assert cases and not bad, "Missing/pending/failed tests"' "evidence/mobile/$RUN/play.xml"
```

GREEN expected: `TOTAL N PASS N BAD 0`, N adalah jumlah aktual, bukan dipaksa102. RED expected: test target Failed dengan assertion kontrak yang belum terpenuhi; compile error/scene missing bukan RED behavior yang sah. Selalu simpan keluaran gagal, jangan overwrite pada retry.

Commit template setelah GREEN, explicit paths per task:

```bash
git diff --check
git diff --cached --stat
# Stage hanya file slice yang sudah diperiksa, termasuk .meta Unity-nya.
# Gunakan command git add spesifik yang disebut pada task, bukan git add .
git diff --cached --check
git commit -m "feat: playable mobile slice"
```

Jangan stage raw aset NPC berlisensi untuk publikasi, cache, APK, credential, raw log sensitif, atau pekerjaan user tak terkait. Tidak push. Sebelum baseline commit, audit untracked source yang memang belum pernah masuk Git dan simpan backup lokal; jangan mengklaim commit kecil sebagai snapshot lengkap tanpa memasukkan dependency yang diperlukan secara sadar.

## Step-by-step tasks

### A. Baseline dan workspace aman

**A1 · 3 menit — identitas, backup, tidak menyentuh output lama.**
- Read: `.gitignore`, `ProjectSettings/ProjectVersion.txt`, `Assets/Editor/AndroidBuild.cs`, status Git.
- Jalankan `git --no-optional-locks status --short` dan hash file yang akan disentuh. Backup hanya source/docs/settings ke `.backups/mobile-<RUN>/` di workspace sebelum perubahan; jangan mengikutkan Library/builds/evidence besar.
- Simpan hash input kecil ke `evidence/mobile/<RUN>/inputs.json`, bukan ledger pekerjaan. Jangan mencatat nama file pribadi atau environment.
- Expected: source/assets existing tetap ada, output lama tidak ditimpa, backup bisa dibaca. Belum menjalankan Unity karena save host belum disandbox.

**A2 · 3 menit — hentikan birokrasi aktif.**
- Paths: `docs/WORKFLOW.md`, `docs/AI_RULES.md`, `docs/00_README.md`.
- Hapus hanya `docs/WORKFLOW.md` setelah backup; hapus link aktif ke file tersebut di README dan kewajiban worklog di AI_RULES. Ganti pembuka AI_RULES dengan ringkas: scope singles, Rp0, workspace-only, no-push, test nyata, otoritas desain delegated.
- `docs/ERROR_LOG.md`, `docs/SESSION_HANDOFF.md`, roadmap lama tetap sebagai arsip, tidak wajib diperbarui setiap task. Jangan membuat `WORKLOG`, `CHANGELOG`, ledger, atau decision-ID ritual baru. Cukup `docs/TESTING.md` ringkasan final, `docs/DESIGN.md` keputusan produk final, dan asset provenance jika berubah.
- Check: `test ! -e docs/WORKFLOW.md` exit0; link aktif README/AI_RULES tidak lagi menuju file hilang. Referensi historis di arsip boleh tinggal sebagai sejarah, bukan instruksi aktif.
- Commit setelah audit: `git add -- docs/WORKFLOW.md docs/AI_RULES.md docs/00_README.md`; `git commit -m "docs: retire workflow overhead for mobile singles"`.

**A3 · 5 menit — sandbox data host sebelum QA.**
- Paths baru: `Assets/Scripts/LocalGameData.cs`, `Assets/Tests/EditMode/LocalGameDataTests.cs`; edits `Assets/Scripts/CourtPrototype.cs:221`, `Assets/Scripts/CourtDiagnostics.cs:44`, semua panggilan PlayerPrefs di `Assets/Scripts/CourtPrototype.cs` dan `Assets/Scripts/CourtDiagnostics.cs`.
- Gunakan kode lengkap pada Appendix1. Di Editor/non-Android root = workspace `/.hermes/local-game-data`; Android memakai sandbox aplikasi. Jangan membaca/migrasi PlayerPrefs host yang sudah ada. Preferences Android tetap key lama agar tidak hilang.
- Perbaiki test host sebelum full suite: `Assets/Tests/PlayMode/ApprovedControlsTests.cs:76,82` masih membaca PlayerPrefs; arahkan lewat reflection ke `LocalGameData.GetString`. `Assets/Tests/PlayMode/ClosureTests.cs:184,303` masih membaca/menulis save pribadi di persistentDataPath; ganti root dengan property `LocalGameData.Root` lewat reflection. Jangan menjalankan test lama itu dahulu, termasuk untuk mendapatkan RED, karena akses pribadi sudah diketahui dari source. RED aman memakai EditMode `LocalGameDataTests` yang belum memuat Boot.
- RED: test root containment, string/float round-trip, malformed preferences tidak menimpa file; editor command tidak boleh menyentuh registry. Test boleh memanggil reflection seperti contoh Appendix4 karena runtime sekarang Assembly-CSharp.
- GREEN: ganti `PlayerPrefs.` dengan `LocalGameData.` di dua file tersebut; SavePath dan tester report root gunakan `LocalGameData.Root`. Tambahkan ignore `/.hermes/local-game-data/` ke `.gitignore`.
- Verification: jalankan EditMode `LocalGameDataTests`; expected root di workspace, data hasil test hanya di sana. Search `PlayerPrefs|persistentDataPath|GetFolderPath` pada `Assets/Scripts` dan `Assets/Tests`: akses langsung hanya boleh tersisa di cabang Android helper. Direct access dari tests harus nol sebelum full suite. Tidak menjalankan suite lama yang masih punya akses host sebelum search tuntas.
- Commit paths explicit: helper/test/.meta + dua script + `.gitignore`.

**A4 · 3 menit — regression baseline yang jujur.**
- Jalankan dua command suite standar setelah A3. Bekukan semua file saat tes aktif.
- XML gagal tetap gagal; bedakan baseline failure dari fitur baru. Periksa daftar test dan screenshot lama hanya sebagai navigasi, bukan acceptance baru.
- Expected: hitungan aktual terbaca, tidak ada klaim UI/animasi baik dari suite hijau. Jika gagal, perbaiki harness privat/sandbox lebih dulu tanpa mengubah aturan olahraga.

**A5 · 5 menit — rekam baseline presentasi.**
- Paths: `Assets/Tests/PlayMode/AthleteVisualTests.cs`, `evidence/mobile/<RUN>/before/`.
- Reuse scene Boot, rekam idle/lateral/backhand/overhead/jump pada kamera gameplay, `Time.timeScale=1`, sound normal. Screenshot tambahan front/side untuk diagnose, bukan mengganti kamera main.
- Test lama menulis path tetap `evidence/M2/D045-integrated-court.png`: ganti output hanya untuk capture baru agar baseline tidak tertimpa. Simpan PNG dengan timestamp dan continuous recording jika capture tool tersedia. Jangan memakai desktop-wide capture berisi aplikasi pribadi.
- Expected: evidence sesuai source hash dan daftar temuan spesifik. Tidak ada render baru yang diklaim sudah dilakukan pada plan ini.

### Urutan vertikal yang wajib

A1–A5, lalu D1–D2 agar game punya pintu masuk yang dapat dicoba, lalu B1–B4+B10 untuk satu backhand yang benar, C1–C2 untuk satu intent responsif, D3–D4 untuk HUD/input yang bisa dipakai. Baru perluas B5–B9/B11, C3–C10, D5, E dan F. Bundle diurutkan menurut area agar mudah dicari, bukan instruksi membuat semua animasi dulu sebelum mencoba gameplay.

Saat layout berubah pada D3, update test Fixed joystick yang hardcode `(119,605)` di `Assets/Tests/PlayMode/ApprovedControlsTests.cs:86` menjadi pusat dari layout aktual `(128,H-132)`; tetap uji perbedaan Fixed/Floating dan reset. Saat reparent racket pada B10, test yang mencari `Hips/.../ElbowRight/Racket` harus mengecek socket dan render nyata, bukan memaksa hierarchy lama. Jangan menghapus test perilaku penting hanya karena anchor internal berpindah.

### B. Manusia dan animasi: buktikan satu karakter sebelum menduplikasi

**Pilihan teknis:** native Unity `Animation` dengan clip Generic/legacy yang di-keyframe pada skeleton existing. Tidak wajib Mecanim/Humanoid, tidak membeli mocap, tidak generate pakaian. Clip berisi pose lokal/in-place; game tetap mengendalikan root. `Animation.Sample()` mengevaluasi clip pada clock simulasi; native blending untuk transisi. IK terbatas hanya mempertemukan grip/raket/kaki dengan target yang benar-benar reachable. Satu pemilik tulang: matikan follower mannequin lama ketika jalur baru aktif.

Inspeksi prefab secara read-only menemukan skeleton utama `AthleteNPC/Root/Hips` dipakai head/body/kaos/celana/sepatu. Hair memiliki skeleton salinan di bawah `Root/Hips/Spine/Spine1/Neck/Head/npc_haircut_a_01/Root/Hips`. Jangan mengambil nama `Hips`/`RightHand` pertama untuk semua renderer. Gunakan path utama berikut untuk body; hair cukup mengikuti attachment head miliknya kecuali render membuktikan masalah.

| Bagian | Path relatif terhadap AthleteNPC |
|---|---|
| Hip | `Root/Hips` |
| Dada | `Root/Hips/Spine/Spine1` |
| Kepala | `Root/Hips/Spine/Spine1/Neck/Head` |
| Lengan kanan | `Root/Hips/Spine/Spine1/RightShoulder/RightArm` |
| Siku kanan | `Root/Hips/Spine/Spine1/RightShoulder/RightArm/RightForeArm` |
| Tangan kanan | `Root/Hips/Spine/Spine1/RightShoulder/RightArm/RightForeArm/RightHand` |
| Kaki kanan | `Root/Hips/RightUpLeg/RightLeg/RightFoot` |
| Kiri | Ganti setiap `Right` di path utama menjadi `Left`. |

**B1 · 5 menit — validate skeleton yang dirender.**
- Edit `Assets/Tests/PlayMode/AthleteVisualTests.cs`; tambah `MainSkinBonesUseExpectedSkeleton`.
- Periksa semua bone reference tiap SkinnedMeshRenderer, root, normals/material, scale, dan arah wajah dari render depan/belakang. Assert body/clothes/shoes berbagi skeleton utama; jangan menuntut hair salinan dipaksa sama.
- Tambah lookup exact path dengan error eksplisit di `Assets/Scripts/AthleteVisual.cs`; tidak silent-return ke mannequin jika missing.
- Command: PlayMode filter `AthleteVisualTests`. Expected: missing/path salah memberi assertion nama renderer+path, bukan NRE. PASS hanya rig wiring; belum motion.

**B2 · 5 menit — pilih tampilan dua atlet tanpa modifikasi mesh.**
- Paths: `Assets/Editor/AthleteVisualSetup.cs`, `Assets/Resources/AthleteNPC.prefab`, `Assets/Scripts/AthleteVisual.cs`.
- Pertahankan source `npc_csl_00_character_01m_01.prefab` sebagai baseline. Kandidat alternatif hanya jika aset existing memberi silhouette lebih jelas; tidak ganti tubuh/rig demi warna. Ambil kaos terang/player dan coral/opponent memakai `MaterialPropertyBlock` pada renderer kaos, bukan `sharedMaterial.color` di semua skin.
- RED test: `BothAthletesHaveDifferentShirtsAndUnchangedSkin` membandingkan properti kaos dan kulit setelah attach dua atlet; source shared material tak berubah.
- Inspect front/back/side dari gameplay dan close view. Expected: wajah/tangan/sepatu terlihat; kulit tidak teal; pakaian tidak robek ketika dipose; proxy MeshRenderer OFF, raket tetap terlihat.
- Commit: `git add -- Assets/Scripts/AthleteVisual.cs Assets/Editor/AthleteVisualSetup.cs Assets/Tests/PlayMode/AthleteVisualTests.cs Assets/Resources/AthleteNPC.prefab` plus .meta yang berubah; `git commit -m "fix: bind visible athlete to correct skeleton"`.

**B3 · 3 menit — satu clip idle dengan keyframe native.**
- Asset baru `Assets/Resources/AthleteMotion/Ready.anim`. Buat melalui Unity Animation window pada salinan prefab; clip legacy/in-place, looping1.2s, 30samples/s; tidak keyframe transform athlete root.
- Pose: kaki selebar bahu, lutut tekuk ringan, pinggul sedikit rendah, dada condong6–10derajat, siku tidak terkunci, raket di depan dada, tangan kiri sebagai penyeimbang. Napas hip naik/turun maksimal0.008m; tidak bounce berlebihan.
- Keyframe rest/start/end semua tulang yang kelak digerakkan; kurva quaternion, bukan Euler wrap360. Panjang tulang dan localScale tidak dianimasi.
- RED `ReadyClipContainsNoRootMotionOrScaleCurves` di `Assets/Tests/EditMode/AthleteClipTests.cs`; cek binding empty-path/root position atau scale ditolak. GREEN clip valid dan inspect still.

**B4 · 5 menit — pilot satu backhand pada native driver.**
- Paths: `Assets/Scripts/AthleteVisual.cs`, `Assets/Resources/AthleteMotion/Backhand.anim`.
- Nonaktifkan Animator existing. Tambahkan `Animation`, register `Ready` dan `Backhand`, `playAutomatically=false`; jangan biarkan `LateUpdate` follower lama menimpa clip. Driver memegang reference main bones setelah bind, bukan mencari semua Transform setiap frame.
- Gunakan kode clock/binding pada Appendix2. Pilot harus menunjukkan prep0.00–0.12s, contact0.16s, follow0.26s, recover0.46s. Contact mendekat sisi kiri pemain; elbow leading, forearm rotation, bahu berputar25–45derajat, raket kembali ke stance—bukan rotasi seluruh root180derajat.
- RED `BackhandChangesRacketSideWithoutRootMotion` di `Assets/Tests/PlayMode/AthleteVisualTests.cs`: sample kelima waktu, root tetap; arah sisi contact beda forehand; no NaN; jarak handle ke palm <0.04m.
- Capture continuous0.8s at 1x. Jika hanya terlihat lengan berkedut, perbaiki pose/kurva; jangan meneruskan pembuatan semua clip dengan fondasi gagal.

**B5 · 3 menit per arah — footwork, empat clip.**
- Assets: `MoveForward.anim`, `MoveBackward.anim`, `MoveLeft.anim`, `MoveRight.anim` di `Assets/Resources/AthleteMotion/`.
- Durasi0.48s loop, phase0/0.25/0.5/0.75/1.0. Forward/back pendek cepat; lateral memakai chasse/side-step, kaki tidak bersilangan berlebihan. Satu kaki fase stance, kaki lain swing dengan toe clearance0.05–0.10m.
- Clip tidak memindahkan root; cadence mengikuti jarak nyata pada court, bukan analog saat menekan batas. Setelah stop blend keReady0.12s, bukan skating sampai cycle selesai.
- Test `LocomotionUsesActualDisplacement` di `Assets/Tests/PlayMode/MobileMotionTests.cs`: input ke dinding tidak membuat berlari di tempat tanpa gerak; perubahan arah tidak flip badan menjauh net. Expected clip phase berubah jika actual displacement>0, diam stabil.

**B6 · 5 menit — planted feet dan braking.**
- Paths: `Assets/Scripts/CourtMotion.cs`, `Assets/Scripts/AthleteVisual.cs`, `Assets/Tests/PlayMode/MobileMotionTests.cs`.
- Simpan world-space stance anchor setiap foot plant; lepaskan saat swing/takeoff atau reach leg maksimum tercapai. Gunakan two-bone solver fixed-length yang sudah ada sebagai basis, kali ini pada skeleton utama dengan panjang bind aktual, bukan konstanta mannequin.
- Saat stance, target sole Y=floor0.0; hip compensation maksimal0.08m. Jangan scale/stretch tulang agar mencapai target. Direction change mengakhiri stance lama, membentuk plant baru, tidak menyeret foot sejauh gerak root.
- RED: `GroundedFeetStayPlantedAndNeverPenetrateFloor`: step60Hz1detik lateral pada kedua ends; slip stance<0.04m per phase; sole penetrasi<=0.015m. Expected before fail jika target kaki mengikuti root; after pass dan continuous motion tidak sliding.

**B7 · 3–5 menit per clip — lima stroke dan serve.**
- Assets dalam `Assets/Resources/AthleteMotion/`: `Smash.anim`, `Clear.anim`, `Drop.anim`, `Net.anim`, `Lift.anim`, `Serve.anim`. Backhand dari B4 menjadi varian upper-body; net/lift boleh memakai backhand rendah terpisah `BackhandLow.anim` agar tidak memaksa overhead untuk shuttle rendah.
- Path test `Assets/Tests/EditMode/AthleteClipTests.cs`: setiap clip memiliki marker normalized contact dan length nonzero, tidak root motion/scale; contact marker tercantum dalam tabel berikut (simpan sebagai field serialized pada driver/clip table, bukan AnimationEvent yang bisa double-hit).

| Clip | Panjang | Contact | Prep / contact / follow / recover |
|---|---:|---:|---|
| Smash | 0.50s | 0.18s | Bahu terbuka, siku naik; akselerasi cepat menurun; racket menyilang pinggang; kembaliReady. |
| Clear | 0.62s | 0.24s | Tarikan lebih panjang; impact tinggi; follow mengarah atas/depan; recovery stabil. |
| Drop | 0.54s | 0.22s | Prep miripClear agar masuk akal; impact lebih lembut; follow pendek, bukanSmash lambat. |
| Net | 0.46s | 0.16s | Lunge dari kaki depan sesuai arah; racket depan/bawah; push kecil; tarik balik. |
| Lift | 0.60s | 0.22s | Raket dari bawah pinggang; siku extensi; ayunan naik; kembaliReady. |
| Backhand | 0.46s | 0.16s | Siku memimpin kiri tubuh; forearm berputar; follow pendek; kembali menghadapnet. |
| BackhandLow | 0.48s | 0.17s | Raket di sisi kiri depan bawah, bukan overhead; lutut merendah. |
| Serve | 0.52s | 0.20s | Raket+shuttle depan tubuh rendah; swing underhand; racket lanjut ke depan; release jelas. |

- Forehand/backhand ditentukan contact dalam local basis atlet, bukan worldX. Yang berubah pose varian, bukan flight intention.
- Test per clip/atlet/ends: `StrokeContactMatchesVisibleRacket`; contact distance stringbed–shuttle<=0.05m untuk contact yang dinyatakan valid, handle–palm<=0.04m, length tulang tetap±1%. Invalid reach harus miss; jangan memperlebar toleransi sampai tes hijau.
- Review slow0.25x untuk contact, lalu1x untuk keterbacaan. Tidak boleh hanya memberi labelBackhand pada animasiClear.

**B8 · 5 menit — anticipation dan contact clock.**
- Paths: `Assets/Scripts/CourtPrototype.cs:469–550`, `Assets/Scripts/CourtMotion.cs`, `Assets/Scripts/AthleteVisual.cs`, `Assets/Tests/PlayMode/MobileMotionTests.cs`.
- Incoming-flight prediction hanya mengarah stance/shoulder untuk persiapan; tidak memilih shot menggantikan input. Ketika input ada sebelum reach, mulai prep. Ketika hit valid terjadi sekarang, sample tepat pada contact marker, bukan menunda physics menungguwindup.
- Saat miss, play swing-to-recovery ringan tanpa suara impact. Saat AI hit, panggil jalur pose yang sama dengan info jenis/sisi/contact, bukan default arm oscillator.
- RED `SuccessfulShotAndVisibleContactShareSimulationTick` dan `MissHasNoImpactEvent`; expected valid hit satu perubahan velocity/satu sound; AI dan player sama. Tidak pakai AnimationEvent sebagai otoritas skor/hit.

**B9 · 5 menit — jump fixed-step.**
- Paths: `Assets/Scripts/CourtPrototype.cs:552–562`, `Assets/Scripts/CourtMotion.cs`, `Assets/Tests/PlayMode/MobileMotionTests.cs`; asset `Jump.anim` dan `Land.anim`.
- Pindah integrasi jump/rootY ke FixedUpdate sebelum contact evaluation; render menginterpolasi saja. Param awal takeoff speed3.5m/s, gravity10m/s², flight0.7s seperti baseline; tinggi puncak0.6125m. RootY>=0.
- `Jump.anim`: crouch singkat sebelum/awal takeoff, extension takeoff, legs tuck ringan, descent membentang kaki. `Land.anim`0.22s: lutut menyerap0.06–0.10m hip drop laluReady; footstep/landing sound hanya sekali perlanding, tidak ketika aerialattack mulai.
- RED `JumpAtThirtyAndSixtyRenderFpsHasSameLandingTick`; assert apex±0.03m, ground time±satu fixed tick, no repeatedjump while airborne. Movement/reach saat airborne tetap sesuai root nyata. Jangan bikin pemain berhenti di udara sambil menyelesaikanSmash.

**B10 · 5 menit — racket grip, seluruh collider consumer.**
- Paths: `Assets/Scripts/CourtPrototype.cs:378–387,648–669`, `Assets/Scripts/AthleteVisual.cs`, `Assets/Tests/PlayMode/AthleteVisualTests.cs`, `Assets/Tests/PlayMode/ClosureTests.cs`.
- Visible racket menjadi child socket tangan kanan skeleton utama. Handle berada di palm, racket frame oval/string tidak solid sphere. Gameplay proxy tetap boleh di hierarchy lama tetapi render OFF; update `racket` reference dan opponent reference agar TouchesNet mengikuti objek yang dilihat pemain.
- Jangan memakai `GetComponent<Renderer>().bounds` tanpa fallback ketika frame dibuat beberapa bagian; bentuk collider/Bounds stringbed/shaft eksplisit mengikuti socket. Body-net faults harus tetap berjalan walau racket reference hilang.
- Test forehand/backhand/high/low untuk kedua atlet; solver tidak menyentuh root. Gunakan search_files untuk semua lookup path `ElbowRight/Racket` dalam script/test sebelum reparent; ubah test mengikuti kontrak socket yang terlihat, bukan mempertahankan hierarchy palsu demi tes.

**B11 · 5 menit — satukan kedua atlet dan bersihkan old renderer.**
- Paths: `Assets/Scripts/CourtMotion.cs`, `Assets/Scripts/AthleteVisual.cs`, `Assets/Scripts/CourtPrototype.cs`, `Assets/Tests/PlayMode/AthleteVisualTests.cs`.
- Hapus writes ke visible bones dari follower lama. Proxy torso tetap collision-only; matikan `ArmIK` LineRenderer di normal play, bukan memotong tangan render diam-diam.
- Commit B bila clip/rig contact tests dan existing regression GREEN. Capture 30 detik natural moving rally at 1x dengan keduapemain. Jika mesh baik tetapi pose buruk, status visual masih FAIL, bukan `ready`.
- Commit explicit: script/test di atas, `Assets/Resources/AthleteMotion/` beserta .meta dan prefab turunan; jangan raw source NPC baru.

### C. Game feel, aturan, dan AI yang tidak curang

Setiap bundle berikut dikerjakan sebagai subtask terpisah: **.1 test perilaku (3–5 menit), .2 run RED dan baca hasil (2–3 menit aktif), .3 satu perubahan minimum (3–5 menit per diff), .4 run GREEN/regression dan audit (3–5 menit aktif), .5 commit lokal (2 menit)**. Jangan mengaku seluruh bundle selesai dalam lima menit. Bila .3 melebar, pecah berdasarkan fungsi yang disebut; source tetap dibekukan selama Unity berjalan.

**C1 — validasi batas input.**
- Paths: `Assets/Scripts/CourtPrototype.cs:449,468,552`; test `Assets/Tests/PlayMode/ApprovedControlsTests.cs`.
- `ServeWithPower` menolak NaN, Infinity, dan power di luar0..1. `SetAnalog` menolak vektor nonfinite. `Jump` hanya legal pada Training/Rally, bukan Pause/Point/MatchEnd/servis/modal.
- Test invalid input tidak mengubah skor, posisi, charge, atau energi. Nilai API invalid menghasilkan `ArgumentOutOfRangeException`; aksi pada state ilegal menghasilkan no-op. Guard lengkap tersedia pada Appendix3.
- Tambahkan `PointerCancel(int id)` terpisah. Cancel menghapus finger/charge tanpa memanggil ReleaseShot/ReleaseServe. `TouchPhase.Canceled` tidak boleh diarahkan ke `PointerUp` normal.
- Verify: PlayMode `ApprovedControlsTests`; expected semua input invalid ditolak tanpa mutasi, cancel tidak memukul. Commit hanya script/test ini.

**C2 — buffer satu intent, bukan auto-hit.**
- New: `Assets/Scripts/ShotIntent.cs`, `Assets/Tests/EditMode/ShotIntentTests.cs`; integrasi `Assets/Scripts/CourtPrototype.cs` dan `Assets/Tests/PlayMode/ApprovedControlsTests.cs`.
- Copy kode lengkap Appendix3/4 setelah RED. Touch/keyboard mengantre jenis, aim, power, createdAt dan deadline+0.16s. Hanya satu intent; input terbaru menggantikan sebelumnya.
- `ReleaseShot()` tetap immediate untuk public API lama. Wrapper input menambahkan buffer; FixedUpdate mencoba hit tepat sebelum swept body-collision saat incoming shuttle legal dan reachable. Guard harus memakai fungsi reach yang sama dengan hit, bukan nilai `true` dari fixture.
- Hit sekali mengonsumsi intent. Point, pause, replay, cancel dan wrong turn membersihkannya. Kedaluwarsa menghasilkan satu pesan posisi/tinggi, bukan pukulan terlambat atau fault akibat repeat tap.
- RED/GREEN: `ShotIntentTests` (kode lengkap), lalu PlayMode `BufferedTouchFiresOnceOnFirstReachableTick`. Uji actual PointerDown, too far, cancel, newest-wins, render30/60. Commit helper, wiring dan tests setelah kedua lapisan lulus.

**C3 — gerak dan aim tidak saling merusak.**
- Paths: `Assets/Scripts/CourtPrototype.cs:244–248,468,675–685`; `Assets/Tests/PlayMode/ApprovedControlsTests.cs`.
- Tambahkan `lastAim`; update dari analog yang magnitudenya>0.18. Joystick release menolkan movement, bukan menghapus aim. Reset poin memakai tengah lawan; hilangkan bias default selaluX+1.8.
- Pertahankan enam sektor ketika aim disengaja. Manual memakai movement analog saat ini. Assisted memakai joystick untuk aim saja dan mendekati titik intersep reachable pada incoming flight, tidak mengejar pukulannya sendiri.
- RED/GREEN `NeutralAimDoesNotForceRight`, `AimSurvivesJoystickRelease`, dan existing assisted regression. Uji kiri/kanan/netral, kedua ends, movement+jump+shot tiga finger tanpa perebutan ID.

**C4 — kontak yang terlihat, tanpa teleport rescue.**
- Paths: `Assets/Scripts/CourtPrototype.cs:492–509`, `Assets/Scripts/AthleteVisual.cs`; tests `Assets/Tests/PlayMode/LocalGateTests.cs`, `Assets/Tests/PlayMode/MobileMotionTests.cs`.
- Hapus perpindahan root0.35m di dalam `diveSave`. Rescue sah hanya jika footwork/lunge pada langkah fisik sudah membawa raket ke shuttle. Invalid reach tetap miss.
- Gunakan panjang bind arm+racket dariB10 untuk kedua atlet. Prediksi input boleh membantu memilih waktu, tetapi tidak mengubah posisi shuttle, panjang tulang, atau hit volume melebihi raket yang terlihat.
- RED `EmergencyLiftCannotTeleportPlayerOrHitBeyondVisibleReach`: low reachable Lift berhasil; titik di luar reach gagal dan root tidak berubah. Ganti fixture lama yang mewajibkan teleport dengan kontrak ini; jangan menghapus semua regression dive tanpa pengganti.

**C5 — lima flight yang berbeda.**
- Paths: `Assets/Scripts/CourtPrototype.cs:260–275,530–541`; tests `Assets/Tests/PlayMode/LocalGateTests.cs:460–526`.
- Pertahankan solver `LaunchVelocity` dengan integrasi drag yang sama. Target forward-local pada sisi penerima: Net1.0..2.0m, Drop1.6..2.8m, Clear/Lift5.2..6.1m, Smash3.2..5.6m. Aim memilih kedalaman/lateral, neutral midpoint. Power mengubah envelope jenis itu, bukan mengganti jenis shot.
- Jangan mengalikan velocity setelah solver karena merusak target. Kalibrasi duration existing per shot; Smash tercepat menyeberang net, Clear/Lift apex lebih tinggi daripada Drop/Net, Net mendarat lebih dekat dibanding Clear. Kasus geometris mustahil tetap net/miss, bukan dipaksa lewat net.
- RED/GREEN matrix5shots ×3power(0,.5,1) ×3lateral(-1,0,1) ×2ends; contact awal sah tinggi2.1m untuk overhead dan0.9m untuk underhand. Record actual net crossing/landing, bukan target yang diinginkan. Tambahkan start belakang/depan sebagai iterasi berikutnya, jangan menganggap satu start membuktikan seluruh rentang.
- Angka awal boleh dikalibrasi dari rally nyata. Catat angka final singkat pada DESIGN, bukan ledger baru.

**C6 — aturan konsisten, tidak sengaja mencurangi pemain.**
- Paths: `Assets/Scripts/SinglesMatch.cs`, `Assets/Scripts/CourtPrototype.cs:44–52,701–717`; tests `Assets/Tests/EditMode/MatchTests.cs`, `Assets/Tests/PlayMode/LocalGateTests.cs`.
- Pertahankan15poin, selisihdua, cap21, best-of-three, server/parity dan change-ends. UI menyebut `15 poin`, bukan mengklaim aturan BWF terkini.
- New match memakai `simulateJudgeErrors=false`. Legacy wrong-call/challenge tetap diuji dengan fixture yang mengaktifkannya secara eksplisit. Garis IN, servis diagonal, fault net/body/double-hit konsisten. Repeat input yang tidak menghasilkan hit tidak boleh menjadi double-hit fault.
- RED/GREEN `NewMatchesNeverInventWrongLineCalls`, `BoundaryLineCountsAsIn`, `ServiceParityBothEnds`; existing deuce/cap/challenge/load tetap lulus.

**C7 — tiga difficulty dengan kemampuan fisik sama.**
- New `Assets/Scripts/OpponentProfile.cs` dan `Assets/Tests/EditMode/OpponentProfileTests.cs` (kode lengkap Appendix3/4). Edit `Assets/Scripts/BootDiagnostics.cs:14,35`, `Assets/Resources/prototype-config.json`, `Assets/Scripts/CourtFlow.cs:18`; tambah `Assets/Tests/PlayMode/MobileOpponentTests.cs`.
- DefaultNormal. Speed maksimum4.5m/s, reach, jump, dan power caps identik. Perbedaan berasal dari reaksi, frekuensi pengamatan, error aim, dan pilihan taktik.

| Profile | Reaction | Refresh | Max aim error | Taktik awal |
|---|---:|---:|---:|---|
| Easy / Mudah |0.28s|0.16s|0.35m|Clear/Lift60%, Drop/Net25%, Smash15% hanya jika tinggi legal.|
| Normal |0.18s|0.10s|0.18m|Clear35%, Drop/Net35%, Smash30%; arahkan ke ruang kosong yang teramati.|
| Hard / Sulit |0.11s|0.06s|0.08m|Clear25%, Drop/Net30%, Smash45% jika legal; recovery ke basis dan menghukum short lift.|

- Probabilitas dinormalisasi ulang terhadap shot yang legal. Tidak semua contact rendah boleh Smash. Error dipilih saat hit, bukan mengubah shuttle saat flight. Parameter adalah keputusan awal agent, bukan angka yang diklaim dipilih owner.
- Verify profile unit test dan actual-displacement PlayMode. Jangan hanya menguji konstanta kalau jalur runtime masih membaca `aiSpeed` lain.

**C8 — persepsi/intersep AI yang terbatas.**
- Pindahkan `UpdateOpponent` ke partial `Assets/Scripts/CourtOpponent.cs`; otoritas match tetap `CourtPrototype`. File test `Assets/Tests/PlayMode/MobileOpponentTests.cs`.
- .3a ekstraksi tanpa perubahan behavior; .3b simpan observed position/velocity/timestamp; .3c refresh sesuai profil; .3d intersep reachable memakai `StepVelocity`; .3e taktik dan recovery. Masing-masing diff punya test dan GREEN sendiri.
- Reaksi dihitung sejak event pukulan lawan teramati. Prediksi memakai sample observasi terakhir, bukan pending input user. Search kandidat setiap0.02s hingga2s ke depan atau ground, pilih yang masih di wilayah AI, tinggi legal, dapat dicapai dengan speed4.5m/s dan reach nyata. Jika tidak ada, bergerak ke basis defensif—tidak teleport.
- RNG xorshift32 dengan seed nonzero, state disimpan; jangan memakai UnityEngine.Random global. Simpan keputusan sampai refresh berikutnya agar gerak tidak jitter setiap frame.
- RED/GREEN `OpponentWaitsForReactionAndNeverReadsQueuedShot`, `OpponentUsesSameReachAsPlayer`, `SeededOpponentRestoresNextDecision`. Kedua atlet memakai commit-hit dan motion trigger yang sama.

**C9 — stamina simetris.**
- Paths: `Assets/Scripts/CourtPrototype.cs:71–78,542,671–685`, `Assets/Scripts/CourtOpponent.cs`, `Assets/Scripts/BootDiagnostics.cs`, `Assets/Resources/prototype-config.json`; tests `Assets/Tests/PlayMode/ApprovedControlsTests.cs`.
- `moveDrain=0`. Validator mengizinkan0 hanya untuk biaya yang memang boleh nol; dimensi, drag dan fixedStep tetap positif. Baseline: rally0.6/s, shot1, smash3, jump4, recovery antargame8.
- Opponent mendapat kapasitas/drain/recovery/fatigue yang sama; movement factor0.7..1 di bawah75% energy. Energy0 tidak mematikan legal return. Jangan menghilangkan stamina diam-diam atau memberi AI stamina tak terbatas.
- RED/GREEN `WalkingCostsNoEnergy`, `BothSidesShareFatigueRules`, `ZeroEnergyStillAllowsLegalReturn`. Moving rally30s harus punya actual displacement selama rally dan energi65..75 pada fixture yang sama; reset poin tidak dihitung sebagai gerak.

**C10 — save/replay tidak kehilangan gerakan atau keputusan.**
- Paths: `Assets/Scripts/CourtPrototype.cs:113–238,571–612`, `Assets/Scripts/CourtFlow.cs`; test `Assets/Tests/PlayMode/MatchFlowTests.cs`.
- .3a schema v2+load v1; .3b persist AI seed/observasi/energy; .3c persist input/aim/jump; .3d record dua motion-state; .3e replay sampling. Setiap subtask diuji sebelum lanjut.
- Snapshot v2: difficulty, RNG state, observed position/velocity, remaining observation/reaction timers, opponent energy, lastAim, pending intent+remaining time, jump elapsed/phase, dan motion kedua atlet. v1 fallback Easy, empty buffer dan Ready; skor/serve/energy lama tidak dihapus. Slot filename tetap `singles-v1.json`.
- Uji snapshot berisi 1500 frame yang diperkaya terhadap batas JSON 1,000,000 karakter existing. Jangan menghilangkan size guard. Jika fixture sah melampaui batas, ukur ukuran aktual, naikkan batas finite secara eksplisit atau kurangi payload berulang; tambah test oversize rejection sebelum perubahan.
- ReplayFrame menambah clipID, cliptime, gaitphase, jumpstate, contactpoint, rotation kedua atlet. Replay sample hasil yang direkam, tanpa AI/physics/impact event kedua kali. Replayv1 position-only tetap dapat dimuat tetapi jangan diklaim sebagai stroke reconstruction.
- Validasi enum/range/finiteness/count<=1500 dan relasi state sebelum mutasi live. Save gagal tidak menghapus backup atau mengeluarkan pemain keHome.
- RED/GREEN `ReplayRestoresBackhandAndLandingWithoutExtraHit`, `VersionOneSaveMigratesWithoutLosingScore`, `InvalidV2LeavesLiveMatchUnchanged`. Commit setiap subtask yang hijau, bukan satu commit besar setelah seluruhC selesai.

### D. UI benar-benar untuk bermain

**D1 · 3 menit — ekstraksi tanpa redesign.**
- File baru `Assets/Scripts/CourtUi.cs` sebagai `partial CourtPrototype`; pindahkan `OnGUI`, `DrawGuide`, Rect kontrol, dan menu presentation dari `Assets/Scripts/CourtPrototype.cs` tanpa mengubah perilaku lebih dulu. Ini satu class yang sama, bukan event bus/framework.
- Tambahkan `Assets/Tests/PlayMode/MobileUiTests.cs`; baseline tombol/Rect dan public entrypoint existing masih tersedia. Jalankan filter `ApprovedControlsTests|FeedbackTests|MobileUiTests` memakai regex filter Unity yang diverifikasi, atau jalankan seluruh PlayMode bila filtering tidak cocok.
- Expected: ekstraksi tidak mengubah flight/score/touch behavior; full suite tetap hijau. Commit refactor ini terpisah dari perubahan layout.

**D2 · 5 menit — Home dan setup berfungsi.**
- Paths `Assets/Scripts/CourtUi.cs`, `Assets/Scripts/CourtFlow.cs`, `Assets/Tests/PlayMode/MobileUiTests.cs`.
- UI state private enum `Home, Setup, Playing, Training, Pause, Settings, Credits, Confirm`. Home dimulai setelah Boot; tidak memberi feeder/physics rally otomatis. `BeginExhibition` tetap entrypoint, UI membuka Setup dahulu, lalu `Mulai` menetapkan difficulty dan memanggil BeginExhibition.
- Home/Setup membekukan input/game clock tetapi UI memakai unscaled time. Sebelum menjalankan match, set timeScale1, clear semua finger/charge/buffer. Test Boot lama yang menganggap ada Court tetap sah; test training harus memanggil entry Latihan eksplisit, bukan menganggap Boot meluncurkan rally.
- RED `HomePlayStartsSelectedDifficulty` memakai klik Rect nyata, bukan memanggil StartMatch langsung. Expected: Home, Setup, Warmup/Ready, Serving dengan difficulty terpilih; Back kembali tanpa match tersembunyi.

**D3 · 5 menit — skin dan layout readable.**
- Paths `Assets/Scripts/CourtUi.cs`, `Assets/Tests/PlayMode/MobileUiTests.cs`.
- Cache GUIStyle dan texture solid pada initialization. Body 26 logical, score 36 bold, heading 42 bold, label muted 22. Pakai palet/Rect spesifik di bagian UI final; style pressed berbeda fill/border, tidak hanya tulisan.
- Pindahkan toolbar save/load/replay/credits ke Pause. Di HUD rally hanya score, stamina, pause, kontrol; challenge hanya sesudah eligible point. Toggle visibility/sound/replay tetap ada di Settings, bukan dihapus demi bersih.
- Test `HudRectsStayInsideSafeAreaWithoutOverlap`: rasio 1280x720, 2340x1080, 2400x1080; notch kiri/right 80px, inset top 24px. Ukur Rect non-overlap dan min touch size; inspect capture dari tiap rasio dengan UI yang benar-benar dirender (Camera.Render saja tidak memasukkan IMGUI).

**D4 · 5 menit — modal menangkap input tanpa bocor.**
- Paths `Assets/Scripts/CourtUi.cs`, `Assets/Scripts/CourtPrototype.cs:732–798`, `Assets/Scripts/CourtDiagnostics.cs:57–62`, `Assets/Tests/PlayMode/MobileUiTests.cs`.
- Satu guard UI `GameplayInputEnabled`: layar Playing/Training, bukan modal/replay/pause. Guard aksi tetap terpisah: servis legal ketika Serving/Ready; pukulan dan jump hanya Rally/Training. Jangan mematikan seluruh input pada Serving karena tombol servis menjadi mati. `PointerDown/Move/Up`, ReadInput, Jump, BeginServe memakai syarat konsisten. Update membaca raw pointer sebelum OnGUI, jadi `Event.Use()` saja tidak cukup: cek Rect pause/modal/control dan kepemilikan finger pada dispatcher sebelum mengirim movement. Tidak ada input tembus ke court.
- Saat membuka modal, clear analog, jumpHeld, charging, serveCharge, semua finger dan pending intent. Android Back menutup satu lapis modal; saatPlaying membukaPause; saatHome minta exit normal.
- RED `PauseButtonNeverMovesPlayer`, `CanceledServeNeverLaunchesShuttle`, `LeavingModalDoesNotReleaseStaleShot`. Expected root/energy/score tidak berubah selama modal; Resume bukan otomatis melepas pukulan.

**D5 · 5 menit — hasil, restart, simpan/muat, settings.**
- Paths `Assets/Scripts/CourtUi.cs`, `Assets/Scripts/CourtFlow.cs`, `Assets/Scripts/CourtPrototype.cs:222–238`, `Assets/Tests/PlayMode/MobileUiTests.cs`.
- MatchEnd tampil winner, games, score terakhir, difficulty. `Main lagi` match baru pakai difficulty terakhir; `Menu` meninggalkan match dengan konfirmasi hanya ketika masih aktif. Settings menyimpan via LocalGameData; grade numerik hidden kecuali Debug.
- Save/load gagal menampilkan error yang bisa ditindaklanjuti, tetap diPause dengan match intact. Tidak clear data. Background/resume Android kembali Pause, tidak langsung melanjutkan shuttle.
- Test alur Home→Setup→Main→Pause→Settings→Resume→Point→Result→Rematch; khusus Save/Load corrupt snapshot tetap tidak memutasi state.
- Commit D explicit scripts/tests/.meta; expected semua tombol ada fungsi, tidak ada text `M0`, `M1`, `flowStage`, `PROPOSED`, atau `Local tester` pada release HUD.

### E. Venue, feedback, dan readability

**E1 · 5 menit — cahaya dan contact shadow.**
- Edit `Assets/Scripts/CourtPrototype.cs:366–405`, bukan ProjectSettings pipeline. Tambahkan floor di luar court dan hall sederhana memakai geometry native; satu directional soft shadow, ambient netral. Fisik net/court tetap ukuran existing.
- Gunakan warna spesifik bagian target visual; jangan ubah hue kulit untuk mengatasi pencahayaan. Ground shadow karakter harus menambatkan kaki dan membesar/menjauh saat jump sesuai root, bukan static blob pada lokasi spawn.
- Test `VenuePreservesCourtDimensionsAndContactHeight` di `Assets/Tests/PlayMode/MobilePresentationTests.cs`. Render front/back/deep court; no pink/missing shader. Expected kulit/wajah lebih terbaca tanpa menambah light mahal.

**E2 · 3 menit — shuttle dan camera.**
- Paths `Assets/Scripts/CourtPrototype.cs:389–400,566–569`, `Assets/Scripts/CourtDiagnostics.cs`; `Assets/Tests/PlayMode/MobilePresentationTests.cs`.
- Target minimum shuttle apparent diameter 5 px pada viewport 1080p ketika high/deep, trail tipis 0.12s awal (baseline 0.38 terlalu berpotensi mengaburkan arah), shadow aktual berbeda dari landing marker. Ini presentation saja, radius collision tidak diperbesar.
- Pastikan framing kedua ends mencakup raket overhead dan apex jump. Tambahkan fov/zoom clamp hanya setelah capture menunjukkan crop. Camera tidak membaca grade untuk menggoyang frame.
- Test WorldToViewportPoint: sample kepala/raket/shuttle dalam 0.04..0.96 untuk skenario yang dibatasi court; inspect 1x. Jangan memaksa semua flying out-of-court shuttle tetap di layar dengan kamera liar.

**E3 · 5 menit — audio menyatakan contact, bukan input.**
- Paths `Assets/Scripts/CourtAudio.cs`, `Assets/Scripts/CourtPrototype.cs`, `Assets/Scripts/CourtMotion.cs`; `Assets/Tests/PlayMode/LocalGateTests.cs`.
- Reuse empat rekaman racket CC0 existing. Valid hit satu impact sesuai shot; miss tidak impact; ground landing satu footstep/contact; no repeated sound dari replay state sample. Balance SFX agar net/lift lebih lembut daripada smash, volume user dipersist.
- Test event-count plus capture audio output dari game saja. RMS bukan bukti enak didengar; dengarkan potongan smash/lift/net/landing pada gameplay 1x. Tidak menambah musik/commentator/rekaman dari YouTube.
- Commit E setelah render/audio checks dan regression. Bukti visual tetap harus dilihat, bukan dihitung saja.

### F. Quality gate akhir, build, dan delivery

**F1 · 5 menit — natural-match fixture yang tidak memalsukan coverage.**
- Paths `Assets/Tests/PlayMode/LocalGateTests.cs:704–743`, file baru `Assets/Tests/PlayMode/MobileMatchTests.cs`.
- Pertahankan fixture accelerated lama sebagai logic stress, tetapi ubah klaimnya menjadi tiga pola input; jangan menyebut seed AI bila tidak mengubah RNG AI.
- Fixture baru memakai seed 37, 71, 109 × difficulty Easy/Normal/Hard. Start melalui entrypoint setup, maju via UI flow, biarkan point timer 3s berjalan; tidak Call NextPoint dari test. Beri input player legal, bukan posisi/velocity shuttle paksa setelah rally mulai.
- Hitung displacement hanya delta XZ antar-tick Rally yang sama, reset accumulator previous saat point/ends berubah. Ukur valid returns dua sisi, point count, panjang rally, fault reason, winner, durasi. Tidak ada teleport, no hidden score injection.
- Expected: 9 match selesai tanpa deadlock; setiap match punya kedua pihak return; tidak semua point dari service fault. Jangan mengunci tingkat menang arbitrary dari 9 match; difficulty fairness dinilai juga dari reaction/taktik dan permainan nyata.

**F2 · 5 menit per inspection batch — video nyata at 1x.**
- Paths output `evidence/mobile/<RUN>/easy-match.*`, `normal-match.*`, `hard-match.*` plus timestamped PNG clips. Rekam satu match utuh per difficulty dengan audio game, `Time.timeScale=1`, source hash yang sama.
- Rekam dari Unity game view/render target atau app sendiri. Jangan capture seluruh desktop, notifications, file browser, atau suara mikrofon. Jika tool video belum ada, gunakan Unity ScreenCapture PNG sequence ke workspace; encode hanya dengan encoder yang sudah tersedia tanpa package/layanan berbayar. Jangan mengklaim PNG sequence sebagai continuous device recording.
- Inspector menilai prep/contact/follow/recovery, forehand/backhand, low/net, jump/landing, kedua atlet, left/right/end swaps. Tuliskan temuan timestamp singkat ke hasil QA, bukan jurnal pergerakan agent.
- Expected: penonton bisa menamai smash vs backhand vs jump landing tanpa melihat debug label. Jika tidak, revisi clip dan ulang scenario yang sama. Mesin test tidak bisa membuktikan `enjoyable`.

**F3 · 5 menit — real input dan usability.**
- Paths `Assets/Tests/PlayMode/MobileUiTests.cs`, `Assets/Tests/PlayMode/ApprovedControlsTests.cs`; evidence capture.
- Uji mouse/keyboard Editor memakai jalur pointer normal dan simultaneous touch di Android nanti. Screenshot harus menunjukkan game UI, bukan test harness overlay.
- Checklist: Main tanpa membaca dokumentasi; mulai serve satu tap; kaki merespons joystick; shot yang ditekan sama dengan intent; back/menu/restart jelas; 5 shot + jump bisa dicapai ibu jari; teks/score tidak menutupi shuttle. Minimum full natural match oleh agent, lalu owner menilai modul utuh sekali, bukan menjadi tester per-bug.

**F4 · 3 menit — freeze dan regression final.**
- Hitung hash shipped inputs `Assets/Scripts`, Resources yang digunakan, scene Boot, Packages, ProjectSettings. Jalankan full EditMode lalu PlayMode, parse XML actual totals. Jangan mengedit sambil tes berjalan.
- Scene hash sebelum/after build harus sama; properti shader retention yang build method ubah harus diaudit dan dites sebelum freeze final. Gagal tes tidak boleh disembunyikan oleh build yang sukses.

**F5 · 3 menit active + build wait — APK final.**
- Path existing `Assets/Editor/AndroidBuild.cs`; tambahkan entrypoint unik agar tidak menimpa APK lama:

```csharp
public static void RunMobile()
{
    Build("builds/JustSmash-Mobile.apk", "evidence/mobile/build", BuildOptions.None);
}
```

- Ini ditambahkan ke class AndroidBuild, bukan mengganti seluruh file. Test Editor `BootTests`/`ShaderPackagingTests` masih lewat. Build command:

```bash
"$UNITY" -batchmode -quit -projectPath "$PROJECT" -buildTarget Android -executeMethod AndroidBuild.RunMobile -logFile "$PROJECT/evidence/mobile/$RUN/android-build.log"
python -c 'from pathlib import Path; import hashlib; p=Path("builds/JustSmash-Mobile.apk"); assert p.is_file() and p.stat().st_size>0; print("BYTES",p.stat().st_size); print("SHA256",hashlib.sha256(p.read_bytes()).hexdigest()); print(Path("evidence/mobile/build/build-result.txt").read_text())'
```

- Expected `result=Succeeded`, `errors=0`, file nyata dan hash 64 hex; jumlah byte ditulis aktual. Warning ditinjau, bukan otomatis diabaikan. Audit APK manifest ARM64/API26+/landscape/non-development; tidak INTERNET, external storage, microphone, contacts, atau analytics. Pakai `aapt` SDK Unity hanya melalui path tool yang diketahui, bukan pencarian profil. Jika path belum diketahui, inspector dapat membaca isi APK dan build config, tetapi gate merged manifest tetap pending sampai aapt terverifikasi.
- Installed footprint target<1GB (batas existing), working goal APK<150MB. Jangan menurunkan model menjadi primitive demi mengejar angka. Hilangkan aset audit dari build graph bila tak terpakai, bukan menghapus source asli.

**F6 · device gate — hanya setelah izin akses HP yang spesifik.**
- Batas terbaru workspace-only berarti jangan menggunakan izin HP lama secara otomatis. Minta sekali izin install/update/launch **paket game ini saja**, setelah APK siap; tidak perlu bertanya untuk desain lagi.
- Jika diizinkan dan device tersedia: `adb install -r` APK terbaru, launch package `com.justsmash.prototype`, jangan uninstall/clear-data atau akses foto/file/aplikasi pribadi. Ambil FPS/app memory/logcat terfilter PID game, bukan seluruh telepon. Jika policy mutlak melarang di luar workspace, cukup kirim APK untuk user install dan laporkan device QA belum dilakukan.
- Target 30 FPS stabil: p95 frame time<=40ms, tidak ada hitch>100ms berulang, sesi15 menit termasuk 3 match, pause/background/resume, multitouch, notch, save/relaunch. Laporkan device/OS dan measurement nyata; Editor cepat bukan Android cepat. Pilihan 60 FPS opt-in hanya jika alat kuat; jangan mengklaim semua HP 60 FPS.
- Device tidak terhubung berarti `DEVICE_QA_PENDING`, bukan `lulus`. Perbaikan independent local tetap dilanjutkan.

**F7 · 3 menit — selesai yang dapat diverifikasi.**
- Update ringkas `docs/DESIGN.md` dengan keputusan final nyata, `docs/TESTING.md` dengan current gates/evidence/artifact hash, `ASSET_REGISTER.csv` hanya jika aset/provenance berubah. Jangan membuat worklog baru.
- Local commit akhir explicit files setelah secrets/build size review. Tidak push. Deliver `builds/JustSmash-Mobile.apk` dan path video/screenshot real; sebut perbedaan, hasil tes, batas device/owner acceptance secara terpisah.
- Jangan menyebut game selesai hanya karena build. Jika grafik/animasi fail, status tetap PARTIAL meski 102+ tests pass.

## Tests / validation matrix

| Gate | Bukti wajib | Tidak cukup |
|---|---|---|
| Privasi | Akses agent dalam workspace; host save/preferences sandbox; tanpa account/network SDK | Klaim umum `aman` |
| Manusia | Wajah, tangan, sepatu, deformasi badan/pakaian; primitive hanya proxy tak terlihat | Ada SkinnedMeshRenderer |
| Animasi | Backhand/smash/net berbeda, contact selaras, foot plant, jump/landing pada1x | Nama AnimationState atau satu PNG |
| Aturan | Deuce/cap, server/parity, line/service/fault, transisi tepat sekali | Label `realistis` |
| Kontrol | Actual pointer, cancel/pause/buffer; tidak salah shot/teleport | Memanggil helper langsung saja |
| AI | Sembilan seeded matches, persepsi terbatas, kapasitas fisik sama | Mengubah label difficulty |
| UI | Semua tombol bekerja, modal menahan input, tiga rasio+safe area, target48dp | Mockup atau render kamera tanpa GUI |
| Audio | Event tepat sekali dan output didengar dalam game | RMS terukur saja |
| Mobile | Identitas APK, merged manifest, profil perangkat, multitouch | Windows batch test |
| Menyenangkan | Playtest agent dan satu match utuh oleh owner ketika modul siap | Jumlah tes yang lulus |

## Protokol subagent: opsional, diperiksa maksimal 5 menit

Tidak memakai subagent pada penyusunan plan ini. Saat eksekusi, kerjakan solo sampai satu jalur rig, pukulan, dan HUD bekerja. Paralel hanya untuk pekerjaan tanpa perebutan file/Unity.

1. Parent memberi absolute workspace, allowlist file, scope kecil, command tes, larangan akses luar workspace/biaya/network/push, dan output konkret. Child tidak mengubah izin atau menutup status seluruh fitur.
2. Maksimal dua child. Contoh: childA inspect pose/aset read-only; childB review aturan/tests read-only; parent mengedit `Assets/Scripts/CourtPrototype.cs`. Jangan dua child menjalankan Unity pada project yang sama.
3. Pecah pekerjaan agar child mengembalikan hasil dalam satu window maksimal5 menit. Saat child masih aktif, parent memeriksa status via `delegate_task action=list/steer/stop` setiap<=300detik. Jangan menjalankan foreground tool panjang yang menghalangi inspeksi. Jangan busy-wait atau membaca transcript/artifact untuk menunggu hasil; runtime mengirim hasil akhir child.
4. Inspeksi mencakup: aktivitas nyata/stuck, kesesuaian goal, diff hanya allowlist, larangan biaya/data pribadi, alasan test failure, artefak dan source identity. Jika runtime tidak bisa menjamin kesempatan supervisi dalam5 menit, **jangan spawn child panjang**; kerjakan solo atau chunk kecil yang selesai dalam window.
5. Jangan membuat cron atau persistent background demi meniru supervisi di luar sesi. Saat sesi berhenti, pekerjaan child harus berhenti. Tidak menjanjikan monitoring ketika agent tidak aktif.
6. Laporan child bukan bukti. Parent memeriksa diff, XML, capture, dan regression sebelum integrasi/commit. Parent tetap pemilik integrasi dan keputusan kualitas.

## Risks, tradeoffs, open questions

- **Animasi perlu pekerjaan artistik.** Clip native dipilih karena gratis dan skeleton tersedia. Pilot backhand wajib lulus render+motion sebelum membuat semua clip. Jika masih robotik, ubah keypose/timing/IK berdasarkan capture; jangan menyatakan generic locomotion sebagai paket badminton lengkap. Alternatif clip gratis perlu lisensi jelas dan uji rig. Jangan kembali ke garment generator.
- **Task kecil bukan durasi proyek.** Angka2–5 menit adalah slot fokus. Pembuatan clip, tes engine, build, dan perbaikan bisa memerlukan banyak slot. Eksperimen tidak dibatasi jumlah, tetapi setiap percobaan harus punya hipotesis dan pembanding yang sama.
- **Responsif vs persiapan.** Anticipation menyiapkan tubuh, tetapi hit valid tidak ditunda0.2s demi menunggu clip. Hindari pose mendadak dengan persiapan lebih awal, bukan input lag.
- **Reach jujur dapat menaikkan miss.** Kalibrasi footwork, bantuan gerak, dan buffer; jangan teleport atau memperpanjang tulang. Jaga shot yang diminta, bukan menggantinya diam-diam.
- **IMGUI punya batas.** Enam tampilan sederhana tidak butuh package baru, tetapi target sentuh, scaling, input dispatch dan aksesibilitas harus diuji. Migrasi renderer hanya jika masalah nyata tidak dapat diselesaikan secara kecil, bukan selera framework.
- **Skor15 bukan klaim BWF terkini.** Pertahankan preset existing dengan label yang jujur. Verifikasi regulasi resmi dibutuhkan jika nanti memberi label Official, bukan syarat membuat singles offline ini playable.
- **Privasi toolchain.** Unity dapat memakai cache/lisensi OS secara internal. Agent tidak menginspeksi profil pribadi; isolasi OS total belum dibuktikan. Jika engine meminta login/instalasi di luar batas, jelaskan blocker, jangan memperluas izin diam-diam.
- **Rp0.** Tidak membeli aset, paid API, model baru berbayar, trial atau layanan. Tool existing saja. Jangan menjanjikan token/listrik tidak memiliki biaya operasional; yang dilarang adalah pengeluaran atau penambahan layanan berbayar untuk proyek ini.
- **Device gate.** Desain dan balancing sudah didelegasikan, tidak perlu pertanyaan baru. Izin akses HP dan model perangkat ditanyakan setelah APK siap. Jika batas workspace tetap mutlak, kirim APK dan tandai device QA belum dilakukan.
- **Do-not-touch:** source `Assets/npc_casual_set_00/` (read-only, ubah derived asset), `Assets/QuaterniusAudit/`, `evidence/M2/quaternius/`, APK lama, save user, folder lain, credential. Jangan rewrite engine, render pipeline, persistence framework, atau membuat career.

## Appendix1 — sandbox data host (kode lengkap untuk A3)

File `Assets/Scripts/LocalGameData.cs`. Di Android fungsi preferences meneruskan ke PlayerPrefs existing; di Editor tidak menyentuh registry/persistentDataPath user. Host memakai satu file per key dengan atomic replace. `Save()` host no-op karena setiap Set sudah atomic. GetFloat malformed mengembalikan fallback tanpa mengubah file rusak; test memastikan file tetap utuh.

```csharp
using System;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace JustSmash
{
    public static class LocalGameData
    {
        static bool OnDevice => Application.platform == RuntimePlatform.Android;
        public static string Root => OnDevice ? Application.persistentDataPath
            : Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".hermes", "local-game-data"));

        static string KeyPath(string key)
        {
            if (string.IsNullOrEmpty(key) || key.Length > 64)
                throw new ArgumentException("Invalid preference key", nameof(key));
            foreach (char c in key)
                if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '-' || c == '_'))
                    throw new ArgumentException("Invalid preference key", nameof(key));
            return Path.Combine(Root, "prefs", key + ".txt");
        }

        public static string GetString(string key, string fallback = "")
        {
            if (OnDevice) return PlayerPrefs.GetString(key, fallback);
            string path = KeyPath(key);
            if (!File.Exists(path)) return fallback;
            if (new FileInfo(path).Length > 4096) throw new IOException("Preference file too large");
            return File.ReadAllText(path);
        }

        public static void SetString(string key, string value)
        {
            if (value == null || value.Length > 1024) throw new ArgumentException("Invalid preference value");
            if (OnDevice) { PlayerPrefs.SetString(key, value); return; }
            string path = KeyPath(key);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            string temporary = path + ".tmp";
            File.WriteAllText(temporary, value);
            if (File.Exists(path)) File.Replace(temporary, path, path + ".bak");
            else File.Move(temporary, path);
        }

        public static float GetFloat(string key, float fallback = 0)
        {
            if (OnDevice) return PlayerPrefs.GetFloat(key, fallback);
            return float.TryParse(GetString(key), NumberStyles.Float, CultureInfo.InvariantCulture, out float value)
                && float.IsFinite(value) ? value : fallback;
        }

        public static void SetFloat(string key, float value)
        {
            if (!float.IsFinite(value)) throw new ArgumentOutOfRangeException(nameof(value));
            if (OnDevice) PlayerPrefs.SetFloat(key, value);
            else SetString(key, value.ToString("R", CultureInfo.InvariantCulture));
        }

        public static void Save() { if (OnDevice) PlayerPrefs.Save(); }
    }
}
```

Kode memakai pengecekan ASCII eksplisit yang kompatibel dengan Unity .NET Standard 2.1; tidak perlu API .NET baru atau package.

Sebelum SaveMatch atau tester report menulis, panggil `Directory.CreateDirectory(LocalGameData.Root)`; path file tetap `singles-v1.json` untuk kompatibilitas slot walaupun schema JSON naik v2. Jangan rename sekaligus menghilangkan saves existing.

## Appendix2 — driver sampling minimal dan aturan authoring

Masukkan anggota berikut pada `AthleteVisual` ketika follower lama diganti; ia mengasumsikan component berada pada instance AthleteNPC yang exact path-nya sudah diverifikasi. Semua clip harus memiliki `legacy=true` dan binding relatif terhadap instance ini. **Kode ini driver sampling lengkap, bukan paket animasi jadi**; pose asset dibuat/diinspeksi per B3–B9. Authoring aset lewat Unity Animation window tidak bisa digantikan angka Euler tebakan dari plan.

```csharp
Animation motionPlayer;

void PrepareMotionPlayer()
{
    var oldAnimator = GetComponent<Animator>();
    if (oldAnimator != null) oldAnimator.enabled = false;
    motionPlayer = GetComponent<Animation>();
    if (motionPlayer == null) motionPlayer = gameObject.AddComponent<Animation>();
    motionPlayer.playAutomatically = false;
    motionPlayer.cullingType = AnimationCullingType.AlwaysAnimate;
    foreach (var clip in Resources.LoadAll<AnimationClip>("AthleteMotion"))
    {
        if (!clip.legacy || clip.length <= 0)
            throw new System.InvalidOperationException("Invalid athlete clip: " + clip.name);
        motionPlayer.AddClip(clip, clip.name);
    }
    if (motionPlayer["Ready"] == null)
        throw new System.InvalidOperationException("Ready athlete clip missing");
}

public void SampleMotion(string clipName, float seconds, float readyWeight)
{
    if (motionPlayer == null) PrepareMotionPlayer();
    if (!float.IsFinite(seconds) || !float.IsFinite(readyWeight))
        throw new System.ArgumentOutOfRangeException(nameof(seconds));
    var selected = motionPlayer[clipName];
    if (selected == null) throw new System.ArgumentException("Unknown athlete clip", nameof(clipName));
    foreach (AnimationState state in motionPlayer) state.enabled = false;
    selected.enabled = true;
    selected.speed = 0;
    selected.weight = clipName == "Ready" ? 1 : 1 - Mathf.Clamp01(readyWeight);
    selected.time = Mathf.Clamp(seconds, 0, selected.length);
    if (clipName != "Ready" && readyWeight > 0)
    {
        var idle = motionPlayer["Ready"];
        idle.enabled = true;
        idle.speed = 0;
        idle.time = 0;
        idle.weight = Mathf.Clamp01(readyWeight);
    }
    motionPlayer.Sample();
}
```

Untuk loop, caller memberikan `Mathf.Repeat(distanceDrivenPhase, clip.length)`; untuk jump/stroke clamp. `SampleMotion` tidak memutasi root, shuttle, score, energy. Pose clip upper/lower pada jump-smash harus diblend pakai `AnimationState.AddMixingTransform` pada hip/upperbody yang divalidasi, atau keyframeclip gabungan `JumpSmash.anim`; pilih clip gabungan pertama jika lebih kecil daripada sistemlayer baru. ContactIK dijalankan setelah sample sebelum render. Tidak menulis panjang mannequin ke NPC.

## Appendix3 — kode minimum untuk input dan profil AI

### `Assets/Scripts/ShotIntent.cs`

Ini hanya buffer satu intent, bukan sistem combat baru. `TryTake` dipanggil dengan `reachable` hasil validasi nyata yang juga digunakan player/AI; jangan mengoper `true` hanya supaya pukulan berhasil. Saat menerima intent, copy aim/power ke kalkulasi shot tanpa mengubah movement analog permanen. Untuk snapshot v2 simpan remaining time, bukan timestamp absolut lintas sesi.

```csharp
using System;
using UnityEngine;

namespace JustSmash
{
    public sealed class ShotIntent
    {
        public const float Window = .16f;
        public bool Pending { get; private set; }
        public int Kind { get; private set; }
        public Vector2 Aim { get; private set; }
        public float Power { get; private set; }
        public float CreatedAt { get; private set; }
        public float ExpiresAt { get; private set; }

        public void Queue(int kind, Vector2 aim, float power, float now)
        {
            if (kind < 0 || kind > 4) throw new ArgumentOutOfRangeException(nameof(kind));
            if (!float.IsFinite(aim.x) || !float.IsFinite(aim.y))
                throw new ArgumentOutOfRangeException(nameof(aim));
            if (!float.IsFinite(power) || power < 0 || power > 1)
                throw new ArgumentOutOfRangeException(nameof(power));
            if (!float.IsFinite(now) || now < 0 || now > float.MaxValue - Window)
                throw new ArgumentOutOfRangeException(nameof(now));
            Kind = kind;
            Aim = Vector2.ClampMagnitude(aim, 1);
            Power = power;
            CreatedAt = now;
            ExpiresAt = now + Window;
            Pending = true;
        }

        public bool TryTake(float now, bool reachable, out int kind)
        {
            if (!float.IsFinite(now) || now < 0) throw new ArgumentOutOfRangeException(nameof(now));
            kind = -1;
            if (!Pending) return false;
            if (now > ExpiresAt) { Clear(); return false; }
            if (now < CreatedAt || !reachable) return false;
            kind = Kind;
            Pending = false;
            return true;
        }

        public void Clear() { Pending = false; }
    }
}
```

`Clear()` tidak mengubah last accepted aim/power; hanya meniadakan intent. PointerUp/cancel/reset flow harus membedakan release yang memang diminta dari cancel. Jangan enqueue pada setiap frame ketika jari masih ditahan.

### `Assets/Scripts/OpponentProfile.cs`

```csharp
using System;

namespace JustSmash
{
    public readonly struct OpponentProfile
    {
        public readonly float Reaction, Refresh, AimError;
        public const float MovementSpeed = 4.5f;

        OpponentProfile(float reaction, float refresh, float aimError)
        { Reaction = reaction; Refresh = refresh; AimError = aimError; }

        public static OpponentProfile For(string difficulty)
        {
            switch (difficulty)
            {
                case "Easy": return new OpponentProfile(.28f, .16f, .35f);
                case "Normal": return new OpponentProfile(.18f, .10f, .18f);
                case "Hard": return new OpponentProfile(.11f, .06f, .08f);
                default: throw new ArgumentException("Unknown difficulty", nameof(difficulty));
            }
        }
    }
}
```

`BootDiagnostics` menerima tiga string tepat itu; label Indonesia hanya presentation. Konstanta speed sama tidak membuktikan AI fair jika jalur movement lain masih memakai speed tersembunyi—test displacement aktual tetap wajib.

### Guards pada public entrypoint existing

Di awal `ServeWithPower(float power)`:

```csharp
if (!float.IsFinite(power) || power < 0 || power > 1)
    throw new System.ArgumentOutOfRangeException(nameof(power));
```

Ganti tubuh `SetAnalog` dengan:

```csharp
public void SetAnalog(Vector2 value)
{
    if (!float.IsFinite(value.x) || !float.IsFinite(value.y))
        throw new System.ArgumentOutOfRangeException(nameof(value));
    analog = Vector2.ClampMagnitude(value, 1);
}
```

Tambahkan guard berikut pada awal `Jump`; pemeriksaan UI-modal ditambahkan D4 setelah state UI ada:

```csharp
if (Time.timeScale == 0 || replaying || testerOpen ||
    (Match != null && MatchState != "Rally")) return;
```

Jangan menaruh helper validation dalam class baru. Tetap cek incoming reachable/type/rally sebelum memanggil `ReleaseShot`, bukan hanya mengandalkan guard ini.

## Appendix4 — tests lengkap untuk slice kode awal

Runtime belum punya asmdef yang direferensikan test, sehingga tests existing menggunakan reflection. Pertahankan cara ini agar tidak perlu restrukturisasi seluruh assembly demi tiga test. Missing runtime type menghasilkan assertion RED, bukan compile error. Buat setiap file/test dahulu, jalankan filter RED, kemudian tambahkan helper terkait. Test lain yang disebut pada task adalah behavior contract integrasi yang harus ditulis pada slice masing-masing, bukan ditumpuk semuanya sebelum runtime berubah.

### `Assets/Tests/EditMode/ShotIntentTests.cs`

```csharp
using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class ShotIntentTests
{
    static Type Kind => AppDomain.CurrentDomain.GetAssemblies()
        .Select(a => a.GetType("JustSmash.ShotIntent")).FirstOrDefault(t => t != null);

    static object Create()
    {
        Assert.That(Kind, Is.Not.Null, "ShotIntent missing");
        return Activator.CreateInstance(Kind);
    }

    static object Call(object intent, string name, params object[] args) =>
        Kind.GetMethod(name).Invoke(intent, args);

    static bool Take(object intent, float now, bool reachable, out int shot)
    {
        object[] args = { now, reachable, -1 };
        bool result = (bool)Call(intent, "TryTake", args);
        shot = (int)args[2];
        return result;
    }

    [Test] public void EarlyInputFiresOnceWhenReachable()
    {
        var intent = Create();
        Call(intent, "Queue", 0, Vector2.left, .5f, 1f);
        Assert.That(Take(intent, 1.04f, false, out _), Is.False);
        Assert.That(Take(intent, 1.12f, true, out int shot), Is.True);
        Assert.That(shot, Is.EqualTo(0));
        Assert.That(Take(intent, 1.13f, true, out _), Is.False);
    }

    [Test] public void ExpiredInputNeverBecomesALateHit()
    {
        var intent = Create();
        Call(intent, "Queue", 2, Vector2.right, .5f, 1f);
        Assert.That(Take(intent, 1.161f, true, out _), Is.False);
        Assert.That((bool)Kind.GetProperty("Pending").GetValue(intent), Is.False);
    }

    [Test] public void NewerIntentWinsAndCancelRemovesIt()
    {
        var intent = Create();
        Call(intent, "Queue", 0, Vector2.left, .5f, 1f);
        Call(intent, "Queue", 4, Vector2.right, .8f, 1.01f);
        Assert.That(Take(intent, 1.05f, true, out int shot), Is.True);
        Assert.That(shot, Is.EqualTo(4));
        Assert.That((Vector2)Kind.GetProperty("Aim").GetValue(intent), Is.EqualTo(Vector2.right));
        Call(intent, "Queue", 1, Vector2.zero, .5f, 2f);
        Call(intent, "Clear");
        Assert.That(Take(intent, 2.01f, true, out _), Is.False);
    }

    [Test] public void InvalidAimCannotReplacePendingInput()
    {
        var intent = Create();
        Call(intent, "Queue", 0, Vector2.left, .5f, 1f);
        var error = Assert.Throws<TargetInvocationException>(() =>
            Call(intent, "Queue", 1, new Vector2(float.NaN, 0), .5f, 1.01f));
        Assert.That(error.InnerException, Is.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(Take(intent, 1.05f, true, out int shot), Is.True);
        Assert.That(shot, Is.EqualTo(0));
    }
}
```

### `Assets/Tests/EditMode/OpponentProfileTests.cs`

```csharp
using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

public class OpponentProfileTests
{
    [Test] public void DifficultyChangesPerceptionNotMovementCapacity()
    {
        var type = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetType("JustSmash.OpponentProfile")).FirstOrDefault(t => t != null);
        Assert.That(type, Is.Not.Null, "OpponentProfile missing");
        object Profile(string level) => type.GetMethod("For").Invoke(null, new object[] { level });
        float Value(object p, string field) => (float)type.GetField(field).GetValue(p);
        var easy = Profile("Easy"); var normal = Profile("Normal"); var hard = Profile("Hard");
        Assert.That(Value(easy, "Reaction"), Is.GreaterThan(Value(normal, "Reaction")));
        Assert.That(Value(normal, "Reaction"), Is.GreaterThan(Value(hard, "Reaction")));
        Assert.That(Value(easy, "AimError"), Is.GreaterThan(Value(hard, "AimError")));
        Assert.That((float)type.GetField("MovementSpeed").GetRawConstantValue(), Is.EqualTo(4.5f));
        var error = Assert.Throws<TargetInvocationException>(() => Profile("Impossible"));
        Assert.That(error.InnerException, Is.TypeOf<ArgumentException>());
    }
}
```

### `Assets/Tests/EditMode/LocalGameDataTests.cs`

```csharp
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class LocalGameDataTests
{
    [Test] public void HostPreferencesStayInsideWorkspaceAndPreserveMalformedValue()
    {
        var type = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetType("JustSmash.LocalGameData")).FirstOrDefault(t => t != null);
        Assert.That(type, Is.Not.Null, "LocalGameData missing");
        string root = (string)type.GetProperty("Root").GetValue(null);
        string expected = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".hermes", "local-game-data"));
        Assert.That(root, Is.EqualTo(expected));
        string key = "test-" + Guid.NewGuid().ToString("N");
        string path = Path.Combine(root, "prefs", key + ".txt");
        object Call(string name, params object[] args) => type.GetMethod(name).Invoke(null, args);
        try
        {
            Call("SetFloat", key, .75f);
            Assert.That((float)Call("GetFloat", key, 0f), Is.EqualTo(.75f));
            Call("SetString", key, "malformed");
            Assert.That((float)Call("GetFloat", key, .5f), Is.EqualTo(.5f));
            Assert.That(File.ReadAllText(path), Is.EqualTo("malformed"));
            var error = Assert.Throws<TargetInvocationException>(() => Call("SetString", "../escape", "bad"));
            Assert.That(error.InnerException, Is.TypeOf<ArgumentException>());
        }
        finally
        {
            foreach (string suffix in new[] { "", ".tmp", ".bak" })
                if (File.Exists(path + suffix)) File.Delete(path + suffix);
        }
    }
}
```

### Exact RED/GREEN command contoh

Jalankan setelah variabel UNITY/PROJECT diset; prefix stage berbeda setiap percobaan. Kedua invocation memakai source yang dibekukan. Command RED dijalankan sebelum menambahkan `ShotIntent.cs`, command GREEN setelah helper dan wiring slice siap.

```bash
RUN="$(date +%Y%m%d_%H%M%S)"
mkdir -p "evidence/mobile/$RUN"
"$UNITY" -batchmode -projectPath "$PROJECT" -runTests -testPlatform EditMode -testFilter ShotIntentTests -testResults "$PROJECT/evidence/mobile/$RUN/C2-red.xml" -logFile "$PROJECT/evidence/mobile/$RUN/C2-red.log"
# Expected RED: tests fail at assertion "ShotIntent missing", not compiler failure.
# Implement only ShotIntent.cs after inspecting that result, then run:
"$UNITY" -batchmode -projectPath "$PROJECT" -runTests -testPlatform EditMode -testFilter ShotIntentTests -testResults "$PROJECT/evidence/mobile/$RUN/C2-green.xml" -logFile "$PROJECT/evidence/mobile/$RUN/C2-green.log"
# Expected GREEN: 4 passed, 0 failed/skipped/inconclusive.
```

Bukan izin menulis kode pada giliran plan. Sesudah eksekusi, catat hasil aktual dari XML; angka expected tidak boleh ditempel sebagai output nyata.

## Finish line

Satu APK `builds/JustSmash-Mobile.apk` dengan seluruh UI, singles vs tiga difficulty, model manusia, stroke/footwork/jump yang terbaca, sound, aturan dan save yang bekerja. Tidak ada fitur tambahan sampai kualitas modul ini terbukti. Owner menerima satu game utuh untuk dimainkan, bukan daftar eksperimen yang masih harus mereka debug.

