## D-064 — APPROVED: penyempitan scope oleh pemilik

### Polish terarah sebelum rilis — 2026-11-25

- Pemilik memerintahkan eksekusi feedback kontak berbasis keputusan tinggi/jangkauan yang sama, satu slot notice rally/poin, patch siku dan siluet crowd dari `JustSmash-Visual-Fixes.patch`.
- Siluet penonton sederhana menggantikan larangan penonton D-062/D-063. Gerak badan/kaki, tampilan kok, Official15 dan aturan stamina dipertahankan.
- Kandidat smash tunggal mempertahankan aim/target/power; grid 0.40–1.00 s per 0.02 s, vy awal <= 0, crossing >= NetTop(x)+0.08 m, terdekat durasi lama. Tanpa solusi mempertahankan lintasan lama yang bisa gagal. Fixture dan cermin z tercatat di [[Release-Polish]].
- Kandidat smash hanya diff untuk persetujuan, belum masuk build playtest. Bukti diminta: diff, tes Unity terarah, tiga klip sebelum–sesudah 1x dengan audio game; video pertandingan utuh menyusul. Bukan izin push atau pengujian Android.

- Gerak badan/kaki dipertahankan; pengembangan khusus ganda dihentikan tanpa menghapus mode yang ada. Tampilan kok diterima pemilik.
- Replay ditetapkan muncul setelah satu set selesai. Ini keputusan produk, belum klaim implementasi; tidak mengasumsikan replay seluruh durasi set. Review/challenge tidak diubah diam-diam.
- Penyiar ditunda. Sinkronisasi media rekaman tidak lagi dikejar sebagai gate pekerjaan. Pengujian Android dilakukan pemilik nanti.
- Investigasi penyebab kegagalan simpan diizinkan; jangan menyebut lock owner atau perbaikan terbukti tanpa bukti. Karakter/audio perlu penjelasan temuan nyata; daftar task lama tidak otomatis menjadi scope aktif.

## D-063 — APPROVED: gelombang implementasi paralel A–G, 2026-11-23

- Request/approving_user: pemilik, pesan pembagian Agent A–G + gambar image_5198af.png; approval_date:2026-11-23. Mulai delegasi independen sekarang, bukan semua35task serial.
- A: aset atlet orisinal kompatibel rig, proporsi/sportswear/grip/dua identitas. Kandidat Blender bukan final; batas kemampuan modeling dilaporkan ASSET_QUALITY_LIMIT, tidak ditutupi recolor. Integrasi tetap setelah mesh/rig/deformasi/gerak lolos. Tidak mengulang pendekatan gagal tanpa perubahan teknis yang terukur.
- B: arena kosong, lighting/material/detail venue paralel; ini mengganti dependency serial T7a pada D-062 untuk pekerjaan terisolasi, bukan izin stadion besar/dekorasi mengalahkan karakter. Tanpa penonton/tribun besar.
- C: framing/shuttle/HUD/touch/ergonomi. D: impact/step/squeak/reverb/feedback poin dari aset orisinal atau legal. Voice announcer T7c OPSIONAL; tidak menghambat audio non-voice, tanpa menambah voice sintetis sebagai pengganti terselubung.
- E animasi, F physics/contact, G AI setelah rig/kontrak siap. File/state ownership wajib eksklusif; scene/prefab/controller bersama parent-only. Parent integrasi, audit konflik, focused regression setelah perubahan konkret, visual/gameplay nyata before-after dengan kamera sama. Jumlah PASS bukan kualitas.
- Isolasi: empat feature worktree dari HEAD6f30760a27e77a8babbf6e3a3cc097de85241063 dengan overlay source dirty/untracked SHA-exact, bukan branch kosong yang mengabaikan game saat ini. Tidak commit/push/stash/reset/clean. Canonical hanya parent; Unity global satu instance, worker tidak menjalankan Unity atau menyentuh canonical.
- Batas tetap D-049/D-050, Official15, finite reach/immediate contact, PC-only/no-native-input, Rp0/no-new-dependency/account/upload, protected evidence/data, AI_RULES/UI_REFERENSI tidak diubah, WORKFLOW baru dilarang.
- Kontrak mesin: `C:/Users/Dewangga Ramadhan H/AppData/Local/hermes/cache/scratch/justsmash-parallel-20261123-1330/contract.json`. Deliverable per worker: implementasi file nyata, daftar diff, command/output terfokus, render relevan atau NOT_RUN eksplisit, blocker/deviation. Parent tidak menganggap self-report sebagai PASS.
- Supervisi: list/steer/stop langsung; slice awal dibatasi240detik dan artifact lebih dahulu. Automatic cron desktop sebelumnya gagal, tidak diklaim aktif. Hasil kerja menunggu review parent; tidak ada klaim35task selesai.

## D-062 — APPROVED_EXECUTION: seluruh plan overhaul, 2026-11-23

- Request pemilik: "gua mau lu kerjain semuanya sesuai plan. gua kasih izin emnggunakan subagent sesuai aturan. kalo ada task yg gabisa dikerjakan sekarang atua pelru keputusan dair gua dan gua ga jawab, langsung beralih ke task lain. gua mau tinggal tidur". Approving_user:pemilik; approval_date:2026-11-23.
- Dampak: batas plan-only D-061 berakhir. Seluruh 35 task pada `.hermes/plans/2026-11-22_gameplay-visual-character-overhaul.md` boleh dieksekusi, termasuk evaluasi aset gratis legal terisolasi dan integrasi hanya setelah mesh/rig/gerak lulus. Angka desain proposal boleh dikalibrasi di dalam kontrak plan, bukan diakui sebagai angka pilihan pemilik/BWF.
- Batas tetap: D-049/D-050, Official15, input/kontak langsung/finite reach, PC-only D-052, no-native-input, tanpa biaya/dependency/akun/upload baru, tanpa career/customization/multiplayer/push. Tidak menyentuh AI_RULES/UI_REFERENSI atau membuat WORKFLOW baru.
- Task butuh keputusan dicatat BLOCKED dengan pertanyaan/bukti tepat; tidak menunggu jawaban untuk task independen. Dependency gate tidak dilewati: kandidat karakter gagal tidak boleh diintegrasikan, dekorasi T7a menunggu karakter/motion. Maksimal tiga perbaikan masalah sama. Tidak menyatakan semua task selesai jika gate masih terbuka.
- Parent memiliki source/test/Unity/build/integrasi/docs. Subagent boleh paralel hanya untuk scope/file terisolasi, supervisi sesuai D-059; hasil wajib diverifikasi parent. Belum ada subagent pada pencatatan approval. Tidak ada commit/push otomatis.
- T0: baseline `evidence/shareable/20261123/overhaul-005531/baseline.json`; backup `.backups/overhaul-exec_20261123_005531/` berisi 1.203 file SHA-exact. Source 1.147 file cocok D-060; 1.348 file protected di-hash. APK lama 30.233.578 byte/SHA256 `1ec08181fe9ff0f5419e87305347696a9846ec74e88dbb98d68025f9f19f0919`. Tidak ada Unity pemilik aktif sebelum mulai.
- Tests: belum ada run overhaul pada pencatatan izin; hasil nyata ditambahkan ke TESTING/ERROR_LOG, bukan disimpulkan dari baseline lama.

## D-061 — APPROVED_PLAN_SCOPE: overhaul karakter/animasi dan arena, 2026-11-22

- Request: lampiran `pasted_content_2026-11-21_18-00-20-259_e5f6f9.txt` dan “boleh dibuat seperti itu. tolong buat plannya. terus buat animasi dan char”. Konflik plan versus implementasi langsung diselesaikan dengan klarifikasi eksplisit.
- Pertanyaan sesi: “Pesan lu meminta plan, sedangkan lampiran meminta implementasi langsung. Sesi ini selesai sampai mana?” Jawaban pemilik: **“Plan lengkap dulu, termasuk karakter dan animasi; belum ubah game”**. Alternatif plan+lalu karakter atau seluruh implementasi tidak dipilih. Status ini **bukan izin eksekusi overhaul**.
- Pertanyaan jalur karakter: sportswear membuka kembali area yang ditutup setelah enam percobaan modeling gagal. Jawaban: **“Evaluasi aset legal gratis; integrasi hanya setelah mesh, rig, dan geraknya lolos”**. Alternatif existing-only atau membuat dari nol tidak dipilih. Kandidat belum disetujui/lulus; tidak mengulang pendekatan pakaian procedural gagal. Evaluasi halaman/provenance untuk plan, tanpa download/import/modeling pada sesi ini.
- Pertanyaan stamina: lampiran berbeda dari D-050. Jawaban: **“Pertahankan D-050; perbaiki bug dan feedback saja”**. Jalan/lari drain0, recovery hanya antargame; recovery idle/akhir rally dan biaya gerak baru pada lampiran tidak diterapkan. Angka QA yang diusulkan bukan angka pilihan pemilik.
- Approving_user:pemilik; approval_date:2026-11-22. Bukti klarifikasi asli: session `20261121_235716_d9972b`, message128611, outcome submitted, ketiga respons answered. Rationale: plan lengkap tanpa diam-diam mengubah game, seleksi aset legal matang sebelum integrasi, pertahankan stamina yang sudah diputuskan.
- Arah plan: arena indoor tanpa spectator/tribun, court/net sesuai ukuran lampiran, dua identitas atlet sportswear, footwork/stroke/jump/landing/contact, physics/AI, kamera/HUD/audio/flow. Arena inti dahulu, **pilot karakter/gerak sebelum dekorasi**. Exhibition tunggal dan ganda putra tetap; tidak career/customization baru. Batas PC-only/no-native-input/no-push/Rp0 dan kontrol/aturan/grade/reach/contact langsung tetap. Instal alat/dependency, akun/upload model atau scope besar baru tidak otomatis diizinkan.
- Plan: `.hermes/plans/2026-11-22_gameplay-visual-character-overhaul.md`. Seluruh task implementasi masih pending. Acceptance kelak: RED/GREEN/regresi, bukti before-after setiap tahap, source-bound captures/APK, motion/listening/device/owner gates terpisah. **Tidak ada Unity run/test/build baru pada pencatatan plan.**
- Backup register/handoff sebelum edit: `.backups/overhaul-plan_20261122_011738/`. Sesi ini menulis plan dan pointer keputusan/handoff saja; hasil pemeriksaan hash/referensi dokumen dicatat pada handoff. Keputusan historis di bawah tetap riwayat, bukan izin mengabaikan batas plan-only terbaru.
- Lampiran kedua `pasted_content_2026-11-22_12-30-05-281_8a7edc.txt` (2026-11-22): pemilik menilai physics, gameplay, dan vibes masih kurang realistis dan meminta digabung ("gabungin aja"). Digabung **additif ke plan yang sama**: arah stylized 3D badminton simulator, prioritas satu lapangan bagus + dua karakter natural + lima animasi pukulan enak dilihat di atas stadion megah, dan bobot kerja terbesar untuk animasi + sensasi memukul dicatat pada plan §1; stamina lampiran tetap konflik D-050 dan tidak diterapkan. Plan tetap `PLAN_ONLY`; merge ini **bukan izin eksekusi**. Backup sebelum merge: `.backups/overhaul-plan-merge_20261122_200549/`.

## D-060 — APPROVED: perbaikan AI/timer berbasis reproduksi dan diagnosis lock lokal, 2026-11-21

- Pertanyaan1: "Boleh memperbaiki jalur AI/timer jika reproduksi membuktikan penyebab kegagalan durasi Easy? Aturan, stamina, grade, reach, kontak langsung, dan gate <1000 detik tetap tidak berubah." Jawaban pemilik: **"Izinkan fix berbasis reproduksi"**.
- Pertanyaan2: "Boleh memakai Windows Restart Manager untuk membaca proses pengunci file prefs saat error? API membuat sesi diagnostik sementara; tidak menghentikan proses, menghapus data, atau mengubah antivirus. Ini diperlukan untuk melanjutkan diagnosis pemilik lock." Jawaban pemilik: **"Izinkan sesi diagnosis lock lokal"**.
- Approving_user:pemilik; approval_date:2026-11-21. Memperluas batas D-055/D-059 hanya untuk dua scope tersebut. Tidak mengubah aturan Official15, stamina, grade, finite reach, kontak immediate, PC-only/no-native/no-career/no-push/Rp0. Tidak membolehkan tuning acak, sengaja membuat AI kalah, memperbesar gate, retry save tanpa sebab, menelan IOException, atau mengganti atomic save.
- Restart Manager hanya RmStartSession/RmRegisterResources/RmGetList/RmEndSession pada file terkait; tanpa RmShutdown/RmRestart/force-close, install/elevasi/OSconfig/antiviruschange. Query owner adalah snapshot saat run, bukan bukti owner error historis. Proof scratch terkendali dulu; jangan membaca isi prefs atau mencetak commandline/credential.
- Parent owns source/test/Unity/build/docs. Anak hanya scratch terisolasi. Satu Unity sekaligus; backup sebelum edit, RED reproduksi lalu fix minimum/GREEN/regresi. Stop tiga fix gagal per blocker. Acceptance motion/listening/owner tetap terpisah.
- Baseline sebelum scope baru: `finish-checkpoint1.json`, APK30.233.122byte SHA256`4dad93be0d7f1e981d2b89901521c47145926ad0230e77f73e33cea72f639ac8`,1.147input/1.348protectedidentik,209executions/205uniquetestsPASS. Backup5docs `.backups/repro-authority_20261121/`. Belum ada AI/timer/storage runtimefix pada approval.

## D-059 — APPROVED: delegasi paralel dan supervisi, 2026-11-16

- Request pemilik: "lanjutkan semua pekerjaan. bagi task ke subagent. pastikan hasil pekerjaan di cek. tiap 5 menit sekali cek apakah subagent bener bener bekerja atau cuma buang buang token ga penting". Approving_user:pemilik; approval_date:2026-11-16.
- Dampak: larangan subagent D-055 diganti khusus pekerjaan ini. Scope pekerjaan lain tetap D-055..058; permintaan ini belum menjawab izin khusus tuning AI/timer. Diagnosis/read-only/test-only boleh; tidak melonggarkan Easy<1000, aturan, stamina, grade, reach, timing kontak, PC-only/no-native-input/no-career/no-push/Rp0.
- Pembagian awal: persistence (reproduksi scratch/diagnosis File.Replace), pacing (analisis trace Easy tanpa tuning), presentation (audit source/capture dan patch usulan terisolasi). Anak hanya menulis dalam direktori scratch masing-masing; source/test/Unity/build/integrasi/docs tetap milik parent. Tidak menjalankan Unity bersamaan atau mengedit source saat Unity aktif.
- Kontrak: `C:/Users/Dewangga Ramadhan H/AppData/Local/hermes/cache/scratch/justsmash-team-20261116_081159/contract.json`, memuat1.147input source dan1.348protectedfile SHA-256. Baseline diverifikasi; backup docs `.backups/delegation_20261116_081159/`. Timeout pemeriksaan pertama tidak menghasilkan kontrak; pemeriksaan berikut selesai exit0, tanpa mengubah file produk.
- Rasional/alternatif: tiga pekerjaan independen lebih berguna dibanding beberapa agent mengulang fullsuite atau mengedit file bersama. Supervisi 5 menit memakai list/steer/stop; minta command/output/bukti dan blocker konkret, hentikan pengulangan tanpa hipotesis baru. Hasil anak bukan PASS: parent memeriksa file/diff, menjalankan cek ulang dan membandingkan manifest sebelum mengintegrasikan.
- Tests: pekerjaan subagent belum selesai pada pencatatan ini; tidak ada klaim regresi/packaging/motion/listening final.

## D-058 — APPROVED: suara langkah dan siku locomotion, 2026-11-15

- Request: "kok ada saura aneh pas lagi jaln/lari. terus tangannya aneh nekuknya". Pemilik memilih **"Perbaiki keduanya; izinkan rekaman sepatu legal Rp0"**; approving_user:pemilik; approval_date:2026-11-15.
- Scope: ritme footwork/foot-contact audio, penggantian step sintetis dengan rekaman sepatu yang lisensinya mengizinkan reuse/modifikasi/distribusi komersial Rp0, serta arah siku/ready-recovery pada rig existing. Bukan izin dependency/model/aset lain, AI, physics, reach, timing kontak, stamina, aturan, native input, HP/emulator atau push. D-055..057 lainnya tetap.
- Temuan source-bound: step noise+120Hz80ms, jarak event pi/10m (14,324event/s pada4,5m/s); backhand pole dihitung dari contact dunia lama meski stroke tidak aktif. Source tiga file sesuai manifest APK closure-build1. Replay rumus atas CSV bukan pengukuran audio; gambar kecil tidak menetapkan anatomis natural.
- Alternatif: existing audio saja / tahan perubahan; pemilik memilih rekaman legal. Tes: reproduksi RED runtime siku, cadence dan audio asset, GREEN/regresi, capture before-after dengan audio/motion, APK source-bound; numerical/media validation bukan listening/owner acceptance.
- Rollback: `.backups/locomotion-audio_20261115_212438/`,185file SHA-256 verified; protected baseline1.348file termasuk historical build evidence. Evidence baru tidak menimpa run lama. Saat approval dicatat tes/fix belum selesai; hasil teknis berikut bukan acceptance pemilik.
- Hasil lokal: `locomotion-verification1.json`,19Edit+169Play+2controlledCapture=190/190PASS,1.147source dan1.348protectedfile identik. APK30.230.650byte,4PCMsepatu+license bundled. Cadence runtime jalan19 menjadi6contacts/4s pada1,5m/s;ready elbow177,96° menjadi105,99°. ListeningNOT_RUN/continuous-motionNOT_CERTIFIED;overlap kontur tangan/grip masih terlihat dari samping. Easy949,7124sPASS run ini,tetapi historical instability belum repaired/approved untuk tuning. Detail TESTING/ERROR_LOG.

## D-057 — APPROVED: paket biru-kartunis, court hijau, Anton, 2026-11-15

- Pemilik memilih paket `blue-storyboard`: **"pakai ini, ubah lapangan jadi ijo"**. Gate B arah visual lolos: UI biru dominan, bidang diagonal, judul besar, tombol kuning, shading kartunis atlet/arena; lapangan hijau. Implementasi runtime C–H dilanjutkan dalam D-055, bukan acceptance game final.
- Pemilik kemudian memilih **"Izinkan Anton; lanjut implementasi"**. Satu font Anton-Regular.ttf (170.812 byte), Rp0, SIL OFL 1.1, embedding/bundling diperbolehkan dengan copyright/lisensi. Sumber Google Fonts `ofl/anton`, Vernon Adams / Anton Project Authors. Impact Windows hanya preview lokal, tidak dikemas. Body built-in dipertahankan. Tidak memberi izin aset/font/dependency lainnya.
- Approving_user: pemilik; approval_date: 2026-11-15. Alternatif font existing ditawarkan; Anton dipilih untuk bentuk padat-tebal preview. Biaya nol; lisensi/provenance wajib dikemas/dicatat.
- Rollback slice: `.backups/roller-blue_20261115/`, 148 file source/test/editor/docs/data/register terverifikasi. Tests runtime baru belum dijalankan pada pencatatan. Input/physics/reach/grade/stamina tidak dituning. Pilot motion tetap gate, PC-only/no-native-input/no-push tetap.

## D-056 — APPROVED_DIRECTION_CHANGE: lebih biru, kartunis, desain ekstrem, 2026-11-15

- Pemilik menolak storyboard3 pada gate B: **"gua mau lebih biru dan lebih kartunis. lebih ekstrem lagi desginnya itu masih ai slop"**. Approval_date:2026-11-15; approving_user:pemilik. Storyboard3 bukan arah approved; redesign runtime belum dimulai.
- Arah aktif kini biru dominan dan lebih kartunis/berani, menggantikan turquoise sebagai batas palet aktif D-055. Buat revisi paket konsep dari aset existing; bukan recolor sidebar kotak yang sama. Atlet/arena harus selaras dengan UI. Tidak menganggap feedback ini izin aset/font/dependency/biaya baru.
- Plan A–H tetap; preview revisi tetap perlu persetujuan, kualitas motion/pilot terpisah. Semua aturan gameplay/no-native-input/PC-only/no-career/no-push dan batas D-055 lain tetap.
- Bukti: storyboard3 enam panel dari capture Unity scene+overlay konsep; ditolak sebagai desain terlalu generik. Tidak ada runtime redesign atau APK baru.

## D-055 — APPROVED_EXECUTION: Roller Champions direction + whole-match polish, 2026-11-15

- Request pemilik: menjalankan `2026-11-15_roller-champions-ui-match-polish.md` dengan hasil game ber-UI sesuai referensi, bukan AI slop, grafik sepadan, animasi/gameplay langsung dan realistis. Approving user: pemilik proyek; approval_date: 2026-11-15. Ini izin eksekusi plan A–H, bukan bukti kualitas tercapai atau izin mengubah semua batas.
- Arah Roller Champions menggantikan rekomendasi Nintendo yang belum dipilih. Adaptasi hierarki dan fokus atlet; identitas turquoise/abu, GOR, badminton dan aset JustSmash sendiri. Tidak menyalin branding/aset referensi. Preview satu paket B tetap gate sebelum redesign luas; motion pilot D tetap gate sebelum perluasan.
- Dalam scope: prototipe visual/test-only, UI/alur, lighting/material existing, native pose/IK, arena/audio existing, kamera, pengujian lokal dan APK. Immediate hit, finite reach, root/physics authority, Official15, grade D-049 dan stamina D-050 dipertahankan. Easy <1000 belum closed; tidak tuning AI/physics atau melonggarkan tes tanpa keputusan terpisah.
- Batas tetap: tanpa career, aset/font/package/akun baru, biaya, input native, HP/emulator, port baru, push atau subagent. Tidak mengedit AI_RULES/UI_REFERENSI atau membuat WORKFLOW/worklog. Jika pendekatan gagal tiga kali, laporkan batas; jangan ganti standar diam-diam.
- Backup terverifikasi `.backups/roller-polish_20261115_040359/`: 1.166 file, 745.705.870 bytes, termasuk 1.124 input Assets/Packages/ProjectSettings dan seluruh local-game-data. Evidence baru `evidence/shareable/20261115/roller-polish-040359/`. Baseline still diperiksa: atlet kecil, full-screen dim menenggelamkan venue. Source: transisi pose tanpa blending dan foot-plant step per-frame; belum kesimpulan motion runtime.
- Tests: proteksi fixture/evidence RED sedang dijalankan; tidak ada runtime edit atau APK baru pada pencatatan ini. Preview, numerical/visual/audio, packaging, device dan owner acceptance dilaporkan terpisah.

## D-054 — APPROVED: cabut career; cari referensi UI sebelum redesign, 2026-11-15

- Pemilik meminta referensi UI nyata dan link, tidak menjiplak; memilih eksplisit: **"Cabut fitur, kode, dan menu career; backup terpisah. Tunggal/ganda tetap."**
- Dampak: career keluar scope aktif, bukan reset save saja atau menyembunyikan tombol. Hapus runtime/menu/integrasi/test khusus career. Tunggal/ganda dan perbaikan presentation sebelum career dipertahankan. Jangan mulai lagi career/qualifying/atribut career.
- Rollback: backup `.backups/remove-career_20261115_010535/` berisi 62 file dengan hash terverifikasi. Manifest `evidence/shareable/20261115/remove-career-backup.json`. Delapan script runtime dan satu test khusus career beserta meta dicabut; sepuluh file bersama dipulihkan dari backup yang persis cocok baseline `home-preview-green1-source.json`. Ditambah satu regresi tanpa-career pada MobileUiTests.
- Save exhibition/prefs tidak direset. Folder career lokal kosong saat audit; tidak mengakses HP. Bukti dan dokumen lama dipertahankan sebagai riwayat, bukan pekerjaan aktif.
- Referensi di `UI_REFERENSI.md`: Nintendo Switch Sports (rekomendasi), Roller Champions, Rocket League. Pilihan visual dan preview belum diterima pemilik; tidak menerapkan redesign atau menyalin aset/branding/layout persis. Source CourtUi berubah hanya rollback career.
- Tests: RED `remove-career-red1` 0/1 karena tombol Career masih ada; GREEN `remove-career-green1` 1/1; EditMode `remove-career-edit` 19/19. Regresi Play subset `remove-career-play`95/95PASS dan dua PNG Home diperiksa,1.124file freeze identik. Save/prefs dipulihkan hash-identik setelah test. Bukan penerimaan gameplay/UI menyeluruh atau APK baru.
- D-052, Rp0, workspace-only, no-push/no-dependency/no-native-input/no-AI_RULES-edit tetap. D-053 hanya superseded bagian career dan redesign sebelum referensi disetujui.

## D-053 — RIWAYAT, career dibatalkan D-054: game siap dibagikan, ganda putra dibuka, career terakhir, 2026-11-14

- Instruksi pemilik: "ya gua mau lu buat game jadi yg layak gua share ke socmed, dari uuinya. alur dari buka aplikasi ke permainna. gua buka fitur double sekarang tapi double putra. sekalian careernya, career di urutan terakhir. kalo ada yg slah, cari tau kenap adna cari solusinya sendiri".
- Otorisasi: eksekusi lokal UI dan alur aplikasi sampai hasil, identitas JustSmash, GOR hidup dengan wasit/penonton/reaksi/audio, permainan tunggal dan ganda putra. Career kini masuk scope, dikerjakan sesudah exhibition tunggal/ganda. Larangan lama career/doubles/M3+ digantikan hanya untuk scope ini; spesifikasi detail existing yang tidak bertentangan tetap berlaku.
- Keputusan implementasi dan diagnosis teknis dalam scope didelegasikan; agent mencari akar masalah, membuat regresi, memperbaiki dan menguji sendiri. Tidak meminta review pemilik per tombol/pose. Maksimum tiga perbaikan gagal per blocker tetap berlaku, lalu hentikan slice itu dan lanjut pekerjaan independen.
- Arah: GOR turnamen lokal yang meyakinkan, identitas sporty turquoise existing; bukan menyalin merek/UI game lain. NPC Casual Set00 existing, kamera belakang, kontrol/settings, Official15, grade/stamina final dipertahankan. Ganda berarti empat pemain aktif, AI partner + dua lawan, servis/receiving/rotasi dan snapshot/replay benar, bukan dekorasi dua clone.
- Career mengikuti requirement GDD: multi-save satu atlet, kalender/kompetisi/progression/ranking/partner, keuangan fiktif/coach/interaksi, pensiun dan arsip. Implementasi bertahap ada; kelulusan rinci di TESTING. Scope tidak diperkecil diam-diam menjadi tombol atau counter kemenangan. Parameter proposal tetap ditandai pilihan implementer/hipotesis balancing, bukan fakta aturan resmi.
- D-052 tetap: QA PC saja; tanpa discovery/akses HP, emulator atau port baru. Penolakan input native dan edit AI_RULES tetap; tanpa push, biaya, akun/dependency baru, reset/clean atau akses proyek lain. Tidak membuat WORKFLOW/worklog baru. Tidak menambah musik/komentator, multiplayer, cloud atau transaksi sungguhan.
- Kelulusan: regresi source-bound + inspeksi runtime menu–setup–match–hasil dan kontinuitas gerak/audio, packaging APK; penerimaan pengalaman oleh pemilik terpisah. Artefak source4 lama baseline, bukan build fitur baru. Backup `.backups/shareable_20261114_121800/`.

## D-052 — APPROVED: PC saja, tanpa akses HP, 2026-11-14

- Request/keputusan pemilik melalui pilihan eksplisit: **"Tetap PC saja; jangan akses HP"**. Approving user: pemilik proyek; approval_date:2026-11-14.
- Konteks: APK Mobile source4 sudah terbentuk dan packaging/regresi lokal PASS; ditawarkan install/update serta tes khusus paket game di HP atau tetap PC. Pemilik memilih tetap PC.
- Dampak: lanjut hanya pekerjaan lokal PC yang sudah diizinkan. Jangan discovery ADB/perangkat, install/update/launch, membaca log/data atau mengukur HP. Device QA berstatus **DEFERRED_OWNER_PC_ONLY**, bukan PASS dan bukan alasan mengulang permintaan izin tanpa instruksi baru pemilik. Tidak menafsirkan pilihan ini sebagai izin membuat emulator atau port Windows baru.
- Batas lain tetap: native input PC yang ditolak tidak diulang/bypass; tanpa biaya, dependency/aset/akun baru, push, uninstall atau clear data. Motion/audio acceptance masih terpisah dari regresi dan packaging.
- Tests/evidence: `evidence/mobile/20261113_201007/final-{edit4,play4,capture4}.xml`, `evidence/mobile/build/apk-verification.json`; keputusan ini tidak menjalankan tes HP dan tidak mengubah source runtime atau APK.

## D-051 — eksekusi mobile singles disetujui

Request pemilik: "gas kerjakan langsung semuanya. selesai dengan benar. jangna sampai ga testing". Melanjutkan scope mobile yang sudah dipilih: kamera belakang dengan gerak lebih terbaca, NPC existing + warna, singles vs tiga tingkat AI. Keputusan teknis/visual dalam scope dikerjakan agent; bukan perluasan ke career, doubles atau pakaian baru. Batas Rp0, workspace saja, tanpa push, dependency/akun baru dan tanpa install/launch HP tanpa izin spesifik tetap berlaku. `docs/WORKFLOW.md` dihapus; tidak membuat worklog/ledger pengganti. Penolakan edit `docs/AI_RULES.md` dan input native tetap dihormati.

Dampak: UI penuh, kontrol toleran, rig native fixed-length, AI seeded, hall/audio existing, snapshot/replay dan APK Mobile terpisah. Default Manual/Fixed/Normal; line judge akurat (simulasi salah-call legacy hanya fixture); grade internal tersembunyi di HUD. QA aktual tercatat di `TESTING.md`; approval eksekusi tidak sama dengan penerimaan motion, audio, device atau game oleh pemilik. Dokumen desain lama yang bertentangan menjadi riwayat.

## D-048 — APPROVED: drop custom sportswear, pakai karakter default existing, 2026-11-09
Owner setuju (chat): 6 attempt procedural outfit (3 Unity generator + 3 Blender) gagal visual coverage; generation path ditutup.
Karakter runtime yang dipakai adalah model default existing di project: NPC Casual Set00 (Chepatack, Unity Asset Store
EULA — embedded distribution only, tanpa redistribusi), sudah diimport owner via D-043/D-045 sebagai AthleteNPC.prefab.
Aset Quaternius CC0 tetap audit terisolasi, TIDAK dipakai runtime. DoD M2 'sportswear' di-amend: karaktermaniawi stylized
bukan mannequin — persyaratan pakaian custom dihapus. Solusi berbayar (artis/asset) hanya bila owner memintanya nanti
sebagai keputusan terpisah. Bloker outfit statusnya CLOSED_DROPPED, bukan selesai.

## D-049 — APPROVED: final grade pukulan, 2026-11-09
Grade Timing+Contact bobot 50:50, threshold 85/65/40 (Excellent/Good/Fair/Poor). Dianggap final; tidak boleh diubah
tanpa decision baru. Direction/Power tetap trace tanpa kontribusi grade (D-043).

## D-050 — APPROVED: final stamina, 2026-11-09
Rally 30 detik drain ~30 poin (sisa ~70), fatigue multiplier 0.7..1 saat energy < 75%, jalan/lari drain 0,
recovery antargame. Dianggap final; tidak boleh diubah tanpa decision baru.

## Fase plan production-ready exhibition single putra — 2026-11-09
Owner approve rencana 6 fase (0 re-baseline, 1 keputusan, 2 gap lokal, 3 build final, 4 device QA, 5 release M8 subset).
Scope: exhibition + single putra SAJA. Career, doubles, character creation, Classic21, M3+ keluar scope.
Definition of done: regression segar PASS + 3 natural match video/audio inspected + APK jalan 30FPS di HP owner +
credits lengkap + owner main 1 exhibition match utuh dan setuju. Estimasi agent: 2-4 sesi.

## D-047 — APPROVED: isolated Blender outfit repair, 2026-11-04
- Owner explicitly permits official free Blender installation for one original badminton outfit: jersey, shorts, court shoes. No cost, other dependency, scope expansion or paid substitute.
- Back up before edits; preserve gameplay and old assets. Repair seams/fit/coverage/weights/deformation; agent inspects lunge, overhead, jump, backhand, dive-save poses. Maximum3 automatic fixes per same defect; stop and propose asset/workflow alternative if approach fails.
- No failed visual integration; no final import, final APK or phone action under this repair approval. Relevant gates and further required permissions remain mandatory. Deliver pose evidence and mesh/rig validation plus integration eligibility, not gameplay acceptance.
- Official license https://www.blender.org/about/license/ confirms free use and commercial artwork rights. Installation/source/hash and QA results recorded under evidence/M2/quaternius/blender/; approval does not mean validation PASS.

## Kebijakan aktif — Quaternius exhibition, 2026-11-04
Instruksi owner menjalankan `.hermes/plans/quaternius-exhibition-review.md` sampai selesai mengizinkan akuisisi/audit Quaternius Standard gratis CC0, outfit sportswear orisinal dan integrasi hanya setelah preview/deformasi lolos. Casual Set00 hanya referensi; sumber lama dipertahankan. D-045 tetap mengizinkan kalibrasi grade/stamina dan install/update/launch HP tanpa uninstall/clear data setelah QA lokal dan APK final. Tidak M3/career, biaya, dependency baru atau push. Riwayat kebijakan lama di bawah tidak membatalkan izin terbaru. Device/visual/audio dan review owner tetap gate terpisah.

## Kebijakan aktif — D-037..D-042, 2026-11-03
Instruksi owner terbaru menggantikan aturan lama yang bertentangan; bagian checkpoint/run lama adalah riwayat, bukan izin aktif. D-044 mengizinkan eksekusi M0–M2 sampai satu APK checkpoint baru; batas docs-only sebelumnya berakhir. Otorisasi implementasi tetap M0–M2; M3+ tidak dimulai otomatis, termasuk setelah acceptance M2. Tidak ada import aset/dependency, biaya, push, destructive reset, atau install/launch APK HP tanpa izin terpisah.

Review owner hanya pengalaman modul utuh: singles lengkap; character creation lengkap; career terintegrasi; final game. Ini kelompok review, bukan penambahan scope. Tidak meminta owner menguji pukulan/animasi/tombol/fault secara granular. Agent menangani unit/integration/regression, natural automated match, inspeksi video/motion/audio, dan performance sesuai izin. Keputusan desain tetap memerlukan approval, berbeda dari testing.

Subagent dan commit lokal bersyarat tetap D-038/D-039. D-043 menyetujui NPC Casual Set00 untuk evaluasi import, default joystick Fixed (Floating tetap pilihan), servis charge1detik/power0..1/feedback konsisten/cancel aman. Grade hanya Timing + Contact; Direction/Power trace tanpa kontribusi grade. Rally sekitar30detik menurunkan stamina cukup banyak dan mulai memengaruhi performa, tetapi belum habis dan rally bisa berlanjut; jalan/lari drain0. Bobot/formula grade, biaya/kurva/efek stamina baru belum approved. Unity Personal memenuhi kondisi pribadi non-klien tanpa pendapatan yang dinyatakan owner; tidak meminta pembahasan keuangan lagi kecuali kondisi/terms relevan berubah. Target karakter/animasi/atmosfer singles tetap. Approval evaluasi NPC bukan izin import aset lain/dependency/M3 atau device action. D-044 menggantikan batas docs-only; eksekusi approved sampai APK checkpoint, tanpa mengubah proposal angka menjadi approval.

Rujukan keputusan/proposal: [Decision Register](05_DECISION_REGISTER.md). Riset aset/lisensi: [Legal dan Assets](04_LEGAL_AND_ASSETS.md). Gate bukti: [Testing](TESTING.md).

# JUST SMASH — Decision Register

## D-046 — APPROVED: eksekusi Quaternius exhibition plan, 2026-11-04
- Request/owner: "kerjakan plan .hermes/plans/quaternius-exhibition-review.md sampai selesai". Memilih Quaternius Standard gratis dan original jersey/shorts/indoor shoes; Casual Set00 hanya referensi, bukan mesh/texture outfit baru.
- Impact/options/rationale: mengganti kandidat visual setelah audit lisensi, free subset, preview dan deformasi; tidak membeli Source atau menambah dependency. Integrasi menunggu visual gate, gameplay authority tetap.
- Approval bukan completion. Backup `.backups/quaternius_20261104_125223/`; evidence `evidence/M2/quaternius/`. D-045 numerical/device permission retained; no uninstall/clear data, push, M3 or costs.
- Tests: official Standard ZIP/license audit executed; isolated Unity import and front/back/side rendering executed. Outfit fit gate currently failing; final regression/APK/device not run.

## D-045 — APPROVED: selesaikan singles untuk review pertama, 2026-11-04
- Owner: "gas kerjain sampe tuntas"; tiga jawaban clarify eksplisit mengizinkan kalibrasi angka grade/stamina lewat tes dan pencatatan formula/hasil; evaluasi serta integrasi NPC Casual Set00 bila kompatibilitas/lisensi lolos; install dan launch tes HP setelah QA lokal/APK siap, tanpa uninstall/clear data/file pribadi.
- Grade Timing + Contact saja; gerak drain0; rally sekitar30s mulai melelahkan tetapi belum habis; tidak forced miss. Agen memilih angka berdasar pengukuran, bukan klaim owner memilih angka.
- Kandidat aset gagal memenuhi kebutuhan: berhenti laporkan, tanpa beli/ganti kandidat. M3/career, push, biaya dan dependency baru tetap di luar izin.
- Backup `.backups/singles_finish_20261104_074043/`; dirty/untracked dipertahankan. Approval bukan bukti selesai.

## D-044 — APPROVED: lanjut eksekusi sampai satu build, 2026-11-03
Owner: "oke berarti bisa laksanakna dan lanjutin progress kan?" lalu "lanjutin terus sampe satu build jadi". Cabut batas docs-only D-043 untuk eksekusi approved M0–M2 sampai APK checkpoint baru dan regression. Tidak berarti acceptance singles utuh; proposal angka balancing tetap bukan approved. Evaluasi import NPC Casual Set00 diizinkan D-043. Tidak ada biaya, push, M3, atau HP install/launch. Backup source/docs sebelum perubahan `.backups/build_D044_20261103_212726/`; dirty/untracked diaudit, pekerjaan lama dipertahankan.

## D-043 — APPROVED: jawaban owner enam keputusan, 2026-11-03
- Sumber: jawaban owner bernomor1–6 setelah laporan dokumentasi. Persetujuan desain/izin evaluasi spesifik, bukan instruksi memulai seluruh coding M0–M2. Batas berhenti dokumentasi/proposal tetap sampai instruksi eksekusi berikutnya.
- 1: NPC Casual Set00 dipilih; evaluasi teknis import diizinkan untuk kandidat itu, tanpa jaminan sportswear/Humanoid/performance. Tidak membeli extension atau mengimpor kandidat lain.
- 2: Fixed menjadi default joystick; Floating tetap tersedia melalui Settings, movement/aim D-016 tetap.
- 3: Servis charge1detik, power0..1, HOLD-release, feedback konsisten dan cancel aman disetujui sesuai D-041. HUD harus sama dengan power aktual. Belum diimplementasikan/fix sesi ini.
- 4: Grade **Timing + Contact saja**. Contact mencakup kualitas posisi/ketinggian/reach/stance kontak relevan stroke, bukan sekadar timing diganti nama. Direction dan Power tetap dicatat untuk analisis, bobot grade keduanya0. Ini menggantikan kewajiban kontribusi D/P pada D-035/GDD. Bobot T/Contact, jendela, threshold dan formula numerik belum dipilih owner; formula empat komponen lama tidak boleh dipakai atau dianggap approved. Jangan otomatis menormalisasi bobot45/25 menjadi keputusan baru.
- 5: Target stamina rally panjang sekitar30detik adalah berkurang cukup banyak dan mulai memengaruhi performa, **belum habis**, pemain masih dapat melanjutkan rally. Bukan depletion30detik. Jalan/lari0, kapasitas/recovery D-035 tetap. Besar penurunan, biaya, efficiency dan kurva/dimensi dampak performa belum final; jangan menerapkan angka konservatif atau alternatif keras sebagai approval implisit. Dampak tidak boleh menjadi forced miss/outcome lottery.
- 6: Owner menyatakan proyek pribadi untuk belajar/hiburan, bukan perusahaan/klien, belum menghasilkan pendapatan. Gunakan Unity Personal gratis. Pemeriksaan ulang [Unity Editor Software Terms §1.1](https://unity.com/legal/editor-terms-of-service/software): individual non-client dinilai amount generated in connection with Software, threshold tidak melebihiUS$200,000/trailing12bulan. Berdasarkan fakta yang dinyatakan, kondisi proyek memenuhi Personal; tidak ada blocker finansial spesifik yang ditemukan. Bukan audit akuntansi/activation baru. Jangan meminta pembahasan keuangan lagi kecuali fakta atau terms relevan berubah; tidak meminta dokumen pribadi.
- Tes/impact: dokumentasi disinkronkan; gameplay/import/device belum diuji sesi ini. Approval bukan PASS. M0-03 bagian eligibility resolved berdasarkan deklarasi+terms; task audit engine/toolchain keseluruhan tidak otomatis DONE. M3+, biaya, dependency baru, push dan HP install/launch tetap dilarang tanpa izin.

## D-037 — APPROVED: review modul utuh dan QA agent
- Owner, 2026-11-03, instruksi attachment `pasted_content_2026-11-03_13-19-05-603_37f97f.txt` §1–2. GAMEPLAY/WORKFLOW.
- Cabut jalur/kewajiban review parsial D-031/CLOSURE-011. M0–M8 tetap checkpoint internal. Review: singles utuh, character creation utuh, career terintegrasi, final game; bukan perluasan fitur.
- Agent seluruh QA teknis; owner pengalaman utuh. Persetujuan desain tetap diminta. Tes: gate TESTING; keputusan bukan bukti PASS.

## D-038 — APPROVED: subagent
- Owner, tanggal/sumber sama §3. Batas scope/file jelas, tanpa keputusan produk; agent utama integrasi, merge-audit dan regression. Menggantikan larangan lama hanya pada subagent.

## D-039 — APPROVED: commit lokal
- Owner, tanggal/sumber sama §4. Audit dirty/untracked, credential/secret/build besar sebelum staging. Commit checkpoint boleh; tidak push/force push/destructive reset/penghapusan pekerjaan. Rollback wajib tidak menghilangkan source. Sesi ini backup, tidak commit; audit secret menyeluruh belum dilakukan.

## D-040 — APPROVED: Fixed/Floating joystick Settings
- Owner, tanggal/sumber sama §5. Fixed posisi tetap; Floating pusat dari sentuhan awal di area kontrol. D-016 movement/aim/enam sektor/assisted tetap.
- D-043: default Fixed APPROVED. Alasan usulan awal: posisi konsisten untuk orientasi dan pengukuran; Floating tetap pilihan. Tidak ditemukan keputusan default eksplisit; kode floating bukan approval default. Agent QA persistensi, multitouch, cancel/pause, safe-area kedua mode sebelum review utuh.

## D-041 — APPROVED mekanik servis; detail PROPOSED
- Owner, tanggal/sumber sama §6. HOLD untuk power, release melakukan servis. D-035 tap-immediate teknik lain tetap. Aturan/fault tidak berubah.
- Usulan: `p=clamp(holdSeconds/1.0,0,1)`, rentang0..1; saturasi tanpa auto-release/oscillation. HUD bar dan angka0..100% dari nilai yang sama; bukan target prediksi landing. Cancel saat pause/replay/keluar state servis, tanpa servis tak sengaja. Durasi1detik mengikuti kode yang ada, belum dianggap tuning disetujui.
- Audit source: ReleaseServe memakai clamp(seconds) tetapi HUD `/0.8`; pada0.8detik HUD100%, aktual80%. Perlu fix setelah izin coding, bukan di sesi ini. Hold lama tidak bypass fault atau auto-serve. Konsekuensi: kontrol lebih terukur, tambahan waktu input maksimum1detik untuk power penuh. Alternatif0.8detik lebih cepat tetapi resolusi sentuh lebih sempit.

## D-042 — APPROVED batas kerja dan target visual; balancing/import tetap menunggu
- Owner, tanggal/sumber sama §7–11. Jalan/lari stamina0. Target manusia stylized proporsional dengan otot/sendi/wajah jelas dan sportswear; tidak perlu owner Blender. Karakter/animasi badminton/atmosfer termasuk acceptance singles, mannequin tidak final.
- Evaluasi empat aset + alternatif gratis di Legal; belum izin import/download paket/dependency/pengeluaran. Implementasi hanya M0–M2 sebelumnya. Jangan M3 otomatis (menggantikan kalimat D-035). HP install/launch harus izin terpisah, menggantikan izin sesi lampau.
- Sesi ini hanya dokumentasi/proposal/simulasi aritmetika; berhenti. Semua approval lain tetap kecuali eksplisit diganti. Jika penyelesaian singles membutuhkan M3+ atau scope baru, ajukan perubahan dahulu.

### Riwayat proposal balancing D-042 — angka tetap PROPOSED; empat komponen/depletion30detik ditolak D-043
Baseline yang sudah disetujui D-035 dipertahankan: stat `S=1..100`, `C=100*(1+(S-70)*0.01)` sehingga31..130; recovery sekali antar-game `R=8*C/100` sehingga2.48..10.4, `E=min(C,E+R)`. Ini jumlah, bukan kecepatan recovery. Angka baru di bawah belum diterapkan.

**Grade usulan:** validasi contact/fault tetap dulu. Untuk kontak valid `Q=100*(0.45*T+0.25*X+0.15*D+0.15*P)`, komponen0..1. Excellent>=85; Good>=65; Fair>=40; Poor<40. Tidak mengubah flight atau mengundi outcome.
- `T=clamp(1-abs(dt)/0.20,0,1)`: dt selisih waktu input dengan kontak ideal stroke dalam window reachable terdekat (bukan tinggi saja). Jendela maksimum0.20s adalah proposal, diuji frame-rate dan natural trajectory; bukan input buffer baru/auto-hit.
- `X=clamp(1-reach/1.15,0,1)`: posisi horizontal relatif kontak; tinggi stroke masih gate. Batas1.15m mengikuti kode saat ini, bukan tuning baru. Formula linear terlalu menghukum kontak reach sedang; alternatif plateau `X=1` sampai0.6m lalu turun ke0 pada1.15m. **Rekomendasi plateau** `X=clamp((1.15-reach)/0.55,0,1)`, perlu approval bersama formula. Simulasi memakai plateau ini, bukan alternatif linear.
- `D`: ketepatan arah aktual terhadap sektor yang dipilih analog, bukan apakah taktik pemain dianggap benar; `clamp(1-angleError/30deg,0,1)`. `P=clamp(1-abs(pActual-pRequested)/0.25,0,1)`. Tap menggunakan power permintaan yang memang tersedia; tidak menambah hold teknik atau target power ideal tersembunyi.
- D/P berisiko selalu1 karena solver deterministik. Trace awal wajib mengukur variance; jangan mengarang target taktik agar nilai bervariasi. Alternatif lebih jujur: grade T/X saja, D/P tetap trace (mengubah acceptance, perlu approval). Rekomendasi diskusikan definisi ini sebelum evaluator; formula belum final. Dive Good cap yang sudah ada dipertahankan, bukan membuat outcome otomatis.
- Alasan bobot:70% timing/posisi,30% pelaksanaan intent. Trade-off: skill eksekusi terbaca, tetapi D/P dapat tidak diskriminatif; perubahan angka harus dari telemetry lokal fixture, bukan klaim playtest.

**Stamina usulan konservatif:** pertahankan shot1, smash3, jump4; gerak/idle0, servis0 sesuai kode sekarang. Jangan recovery per-point/selama rally tanpa keputusan baru. Stat capacity/recovery D-035 tetap. Usulan efficiency Technique `K=clamp(1-0.003*(Technique-70),0.90,1.20)`, biaya hit `base*K`, jump tetap4. Technique1..100; skill70 faktor1, skill100 faktor0.91. Ini hanya kandidat mapping atribut yang sudah ada, bukan sistem skill/career baru; jangan implementasi career dalam M2. Alternatif tidak pakai efficiency sampai disepakati (M2-06 tetap terbuka).
- Konflik nyata dengan baseline lob30detik D-019: biaya1 per lob dan12 return/30detik hanya menghabiskan12 dari100; habis setelah100 kontak, sekitar250detik pada cadence itu. Bahkan efficiency tidak membuatnya mendekati30detik. Jangan mengklaim memenuhi baseline.
- Alternatif kalibrasi30detik tanpa drain berjalan: biaya lob `100/12=8.333333`, smash `3*8.333333=25`, jump `4*8.333333=33.333333`. Ini jauh lebih keras, jump-smash58.333333; hanya1 kombinasi penuh dari100 tanpa recovery. **Tidak direkomendasikan langsung**: perlu natural moving-lob trace baru dan keputusan apakah30detik berarti habis atau mulai lelah. Pilihan mempertahankan biaya1/3/4 mengharuskan revisi baseline30detik secara eksplisit, bukan diam-diam.
- Bukti simulasi sintetis aritmetika: `evidence/M2/docs-proposal-simulation.json`, script `evidence/M2/check-docs-proposals.py`. Bukan Unity/gameplay/performance test. Grade fixture terkontrol dan hitungan biaya hanya memeriksa batas/monotonisitas; nilai awal belum empiris terkalibrasi.

### Riwayat permintaan keputusan — dijawab D-043; bukan pertanyaan aktif
1. Pilih aset dan izin import setelah menilai sportswear/Humanoid belum terbukti; rekomendasi evaluasi teknis NPC Casual Set00 terlebih dahulu, bukan jaminan siap pakai.
2. Default joystick Fixed (rekomendasi) atau Floating.
3. Detail charge servis1detik/power0..1/feedback/cancel, termasuk fix mismatch HUD.
4. Definisi D/P grade (intent trace atau komponen skor) dan angka T/X/bobot; tidak perlu owner menjalankan QA.
5. Arti target stamina30detik serta kurva efficiency; biaya konservatif tidak memenuhi depletion30detik.
6. Konfirmasi total finances terkait penggunaan Unity selama12bulan <=US$200,000, termasuk pendanaan/proyek Unity lain dan bukan jasa klien yang perlu dinilai keuangan klien. Status pribadi/nonmonetisasi saja belum memverifikasi ini.
7. Izin device QA diminta nanti saat siap; tidak diperlukan sekarang. Perluasan M3+ hanya bila gap benar-benar membutuhkan fitur di luar M2.

> D-036 dan keputusan lama di bawah dipertahankan sebagai riwayat; izin install, larangan subagent/commit, continuation M3 dan review parsial digantikan hanya sebagaimana D-037..D-042. Balancing D-035 yang tidak diganti tetap berlaku.

## D-036 — feedback repair and recorded audio approval — 2026-11-03
- Request/approving user: owner explicitly approved repairs only within M0–M2 and latest APK installation after build, preserving data. Subsequently approved actual sound samples and mix adjustment.
- Impact: GAMEPLAY_CHANGE / CONTENT_IP_CHANGE. Landing indicator allowed after flight begins; no pre-shot aim preview. Actual movement rather than idle Rally/input magnitude drives walking energy. Auto next prepares point, never auto-serves player.
- Options/rationale: synthetic900Hz/noise baseline rejected by owner as unrealistic; choose actual racket–shuttle recording with explicit reuse license. BigSoundBank537 by Joseph SARDIN is CC0, allows commercial use/remix, no mandatory attribution. Provenance retained.
- Tests: FEEDBACK-013, PlayMode79/79. Subjective sound/animation/control acceptance remains pending.
- Implementation detail, not owner-approved tuning constant:3s point wait and sample gains are provisional review values; no claim owner selected those numbers.
- UI preview and full visual/rally acceptance pending; M3 not authorized by this checkpoint. Install permission is not phone launch/testing permission.



## D-035 — Approved execution and input/stamina/grade decisions — 2026-11-03
- Owner approved execution: finish M0–M2 first; continue M3 without renewed instruction only after full M2 acceptance. Local regression alone does not establish device/owner acceptance.
- Tap a technique strikes immediately on pointer down, no hold required. Preserve existing jump hold-swipe technique/release path. Keyboard technique press follows immediate tap semantics.
- Stamina baseline stat70: capacity100, intergame recovery8. Linear factor `1 + (stat - 70) * 0.01`; stat80 capacity110/recovery8.8. Recovery amount increases; recovery time is not reduced. Career integration remains later milestone.
- Preserve timing/direction/power/position grade components and explanatory Excellent/Good/Fair/Poor UI. Grade describes execution, never guarantees a point or forces a miss. No invented outcome lottery.
- Body-touch/net-touch faults belong to M2-03; not automatically deferred to M3.
- Existing no push/new dependency/paid service constraints remain. Prior no-commit instruction not overridden by execution approval.
- Approval is permission, not implementation/test evidence. Tests and remaining gates recorded in TESTING.

## CLOSURE-011 — historical unresolved decisions (superseded only where D-035 explicitly decides)
- Grade M1-13/M2-05: tinggi/reach sekarang menghasilkan grade; direction dan power mengubah flight tapi tidak menjadi komponen grade. Belum ada definisi target timing/power/arah ideal maupun bobot. Opsi A: grade tetap contact-only, komponen lain tampil sebagai trace (mengubah acceptance, perlu approval). Opsi B: pemilik menetapkan target/bobot per teknik, baru evaluator multi-komponen dibuat (menambah balancing terukur). Tidak mengarang rumus.
- Stamina M2-06: drain/partial recovery baseline ada, tetapi stat kapasitas/recovery dan skill efficiency belum punya nilai baseline atau kurva approved. Opsi A: tunda pengaruh atribut sampai career sambil pakai fixed baseline (mengubah acceptance). Opsi B: pemilik menentukan baseline stat, kapasitas/recovery dan kurva skill untuk fixture exhibition (memerlukan regression balancing). Tetap BLOCKED_OWNER, bukan implementation complete.
- Fault M2-03: service area/double hit/net/ground/out/Let sudah ada. Body/net-touch contact tidak ada. GDD membolehkan rare fault deferred jika dicatat; belum ada klasifikasi owner yang jelas. Opsi A: prototype mengecualikan body/net-touch; opsi B: implement contact fault dengan spesifikasi collider/priority. Tidak menganggap deferral otomatis disetujui.
- Unity Personal financial eligibility dan subjective visual/audio/control acceptance tetap owner gate; aktivasi teknis bukan bukti hak lisensi atau penerimaan.

## D-034 — Change-ends dan line-judge (2026-11-02)
- APPROVED langsung pemilik melalui form: P-006 world-space side swap, camera tetap belakang pemain; movement/aim/service mengikuti sisi baru. Wajib regression kedua sisi dan deciding game8.
- APPROVED angka line-judge:3% per eligible landing dalam5cm dari boundary singles, maksimal1 salah call/match. Challenge memakai landing sebenarnya. Bukan3% seluruh landing atau jaminan ada salah call.
- Backup `.backups/ends_judge_20261102_165043/`. Risiko transform koordinat, scoring/review/save determinism. TDD per irisan, source-bound build; approval bukan gate PASS.

## D-033 — Koreksi trajectory BUG-016 (2026-11-02)
- APPROVED langsung pemilik: "setuju" atas koreksi durasi menyesuaikan jarak/tinggi kontak dan power masuk perhitungan awal, bukan pengali velocity setelah solver.
- GAMEPLAY_CHANGE dalam GDD §4/5, M1-03/M2-05/14. Tanpa aim assist, teleport, guaranteed net clearance, dependency atau HP activity. Pilihan lama fixed-duration + post-scale ditolak karena seluruh18 Smash dan18 Net fixture kena net.
- Risiko: landing/net/speed regression. Rollback `.backups/trajectory_20261102_153026/`; acceptance controlled valid contact, invalid contact/miss/net tetap mungkin, power/direction berpengaruh, all-flight finite, regression dan APK source-bound. Approval bukan PASS; hasil di TESTING.

## D-032 — Tutup tujuh gap lokal M1 (2026-11-02)
- APPROVED langsung pemilik: lanjut M1-02,03,04,05,06,15,17. Kamera visual proof, full stroke stress, high-velocity test, debug toggle, procedural animation, trail/shadow controls, structured offline tester form.
- Debug trajectory/landing/target hanya mode development yang eksplisit OFF default; gameplay normal tetap tanpa aim preview D-016.
- Tanpa analytics/network, dependency/aset eksternal, HP install/launch, commit atau push. D-031 gate lokal dan deferred owner/device review tetap berlaku.
- Risiko: physics/input regression dan visual clipping. Checkpoint `.backups/M1_gaps_20261102_142939/`; evidence `evidence/M1/gaps*`. Tests/status aktual di TESTING.

## D-031 — Gate lokal M1 lalu langsung M2 (2026-11-02)
- APPROVED langsung oleh pemilik: "utk perubahan di m1 hanya berhenti sampe build dan testing. kemudian langsung m2. gua gamau test dulu sampe m2 selesai".
- Tuntaskan perubahan M1 dengan testing oleh agent dan build lokal; setelah gate lokal lolos, langsung lanjut implementasi/debug/testing/build M2 tanpa menunggu user mencoba M1.
- Penilaian feel kontrol dan tes manual pemilik ditunda sampai M2 selesai. Pending acceptance subjektif M1 dibawa ke review M2, bukan dianggap PASS dan bukan blocker untuk mulai M2.
- Jangan instal/launch ulang HP sebelum M2 selesai sesuai D-029. Tetap tanpa biaya, cloud, subagent atau push.
- D-031 menggantikan pembekuan M2 pada D-030; tidak membatalkan requirement gameplay M1 atau menyamakan build dengan runtime Android PASS.

## D-030 — Prioritas kembali M1 (2026-11-02)
- APPROVED: pemilik meminta task pertama diselesaikan dahulu, lalu menyetujui "oke kerjakan sesuai prioritas".
- M2 dibekukan: kode yang sudah ada dipertahankan tanpa fitur M2 baru; gate M1 tidak dilewati.
- Larangan D-029 instal/launch HP sebelum gate lokal M2 tidak dicabut secara eksplisit; sesi ini tidak instal/launch HP. Build review lokal boleh dibuat. M1 tetap belum diterima pemilik.


## D-029 — Izin lanjut sampai M2 (2026-11-02)
- APPROVED langsung oleh pemilik: "gas coba buat game dan debug debug dulu sampe m2, kalo udah selesai baru install ke hp".
- Scope: perbaiki BUG-005, tuntaskan M1 dan implementasikan M2-01..15 sesuai GDD/keputusan terbaru, tanpa M3/career. Tes/debug lokal dan build dahulu; tidak instal/launch ulang pada HP sampai gate lokal M2 terpenuhi. Performance/feel fisik baru diuji sesudahnya dan tidak boleh diklaim dari Editor.
- Gameplay tetap D-016..028; superseded Quick16/A-B swipe tidak dihidupkan lagi. Tidak ada biaya, cloud, subagent, commit atau push.
- Checkpoint `.backups/pre_M2_20261102_103941/`. Tests: TESTING run M2-LOCAL-001 (IN_PROGRESS).

Tanggal 2026-11-01 | **Usulan tidak boleh otomatis dianggap disetujui.**

## Riwayat keputusan awal (USER-LOCKED saat dicatat; override eksplisit D-016+ dan D-037..D-042 berlaku)
| ID | Keputusan |
|---|---|
| D-001 | Nama Just Smash, Android offline single player career; landscape, 4GB device RAM, keseluruhan game <=1 GB, bukan hanya APK, target 30 FPS, biaya produksi Rp0. |
| D-002 | Gameplay sebagai badminton langsung dengan skill, kamera di belakang ala gambar pengguna, physics dan animasi natural. |
| D-003 | Pukulan smash, lob, drop, netting, lift; arah straight/cross; hold power, timing kontak, kualitas purple/green/yellow/red. |
| D-004 | Kontrol belum dipilih; joystick vs swipe perlu eksperimen; assisted positioning tersedia. |
| D-005 | Jump button serta swipe-up yang aktif pada timing tepat; tidak ada trickshot. |
| D-006 | Stamina dikaitkan intensitas, antarset recovery tidak 100%; athlete attribute & growth. |
| D-007 | 16-point game best-of-3 cap20 dengan deuce, option 21-point rules; official rules dibuat benar. |
| D-008 | Men singles/doubles, exhibition, pause+resume+save, skip/sim, challenge, moments replay toggle. |
| D-009 | Multi save career, satu athlete aktif per career, start path/age custom, retire >=20, read-only career archive. |
| D-010 | News/calendar/training/awards/achievements/popularity/reputation/ offcourt events. |
| D-011 | Skill point match/training, attributes 1-100, overall cap99 floor, aging decline, coach traits, expensive respec, cosmetic equipment only. |
| D-012 | AI easy/normal/hard/adaptive, harder = smarter, no AI stat boost/user nerf; adaptive observed behavior only. |
| D-013 | 3D stylized inspired Valorant, cosmetic customizations, turquoise simple UI reference, sound effects/crowd, no music/commentator. |
| D-014 | No accounts, no premium currency, no P2W, no real-world spend, no unapproved changes. |

## Proposed for approval (not automatically locked)
| ID | Status | Proposed choice | Reason / validation |
|---|---|---|---|
| P-001 | APPROVED | Joystick virtual + assisted footwork | D-016 override. Swipe dibuang. |
| P-002 | SUPERSEDED | Tambahan preset Modern15 target15, deuce14, cap21; Classic21 tetap target21 cap30 | Diganti D-017. |
| P-003 | SUPERSEDED | Career default Quick16, official optional by tournament year after explicit approval | Diganti D-017. |
| P-004 | PROPOSED | Junior start ages 13-16, U17/U19 16-18, amateur/senior 17+, pro 18+ | Game design suggestions; not universal eligibility facts. |
| P-005 | PROPOSED | Separate junior/national/world rankings, rolling 52-week best-10 results senior, entry windows, qualifying draws | Realistic ranking, maintainable simulation. |
| P-006 | APPROVED D-034 | Player-centric camera reorients across change-ends, rally physics remain world-space | Same screen POV even if court sides switch. |
| P-007 | APPROVED | Core stats Power/Speed/Stamina/Technique/Balance-Recovery | D-020. Reaction & Athleticism dibuang. |
| P-008 | APPROVED | Challenge IN/OUT: 2 gagal per game; berhasil tidak memotong jatah; keputusan akhir dari posisi jatuh sebenarnya | Pemilik menyetujui gameplay yang diajukan pada 2026-11-01. Probabilitas/simulasi call awal keliru tetap PROPOSED, bukan ikut disetujui. |
| P-009 | PROPOSED | Currency only match/contract earned, social content light text-card | Zero monetization, small build. |
| P-010 | PROPOSED | Fictional athletes with completely independent identities, real sports tier *inspired* but original brand presentation | Reduce IP / personality rights risk. |
| P-011 | APPROVED (engine only) | Unity sebagai engine; Personal sesuai eligibility Rp0 | Pemilik: "unity dijadikan sebagai engine utk membuat. gua approve unity", 2026-11-01. Versi editor/URP/package belum dikunci; audit M0. Keberadaan Unity Hub belum diperiksa sesi ini. |
| P-012 | PROPOSED | RNG seeded headless career world sim with ~1000 athletes (subject profiling) | Long careers without 3D sim overhead. |
| P-013 | PROPOSED | Doubles after full singles career prototype; partner is AI, one career athlete only | Significant separate mechanic, protect gameplay quality. |
| P-014 | APPROVED | Charging uses press-and-release time at contact; 6-zone aim from left analog | D-016. |
| P-015 | PROPOSED | Coaching and stamina penalties never hard-block training: soft fatigue/injury risk | Requested unlimited training cadence. |

## Known contradictions / clarification gates
- User first stated 15 points, then explicitly revised to 16, then finalized back to **15-point default** following BWF official rules (D-017). Classic 21 locked behind year 2028 gate.
- User said official 21 capped 25 `cmiiw`; actual classic reaches max30. Treat 25 as factual misconception, not an irrevocable wish.
- User timing 'shuttle in front/above head' is **overhead only**; net/lift contacts physically lower. Need stroke-based contact windows.
- Stamina baseline test revised to **30 seconds rally lob biasa** (D-019).
- Single player vs men's doubles is compatible via AI partner, but doubles requires dedicated scope.
- `top 10 countries per subcontinent` may be impossible/pointless in some small groupings; propose up to10 per curated region, no invented nations; owner approval before final list.
- Player wants to face 'top dunia' and suggest 'Finting'. Real top players' identities and highly recognizable lookalikes are legal risk; no fake near-real identity without review/licensing.
- Player says 'no expense at all': final Google Play public release costs and eligible account requirement differ from building/sideloading. Roadmap excludes paid publication.
- No engine-side cloud analytics assumption may be claimed without dependency/permission audit.
- Procedural IK implementation detail: agent figures out natural approach (D-016 assist contact approved).

## Persetujuan penataan dokumen — 2026-11-01
- Pemilik menyetujui penambahan PRD, ARCHITECTURE, DESIGN, TESTING, WORKFLOW, ERROR_LOG; rename Agent Rules menjadi AI_RULES; satu Decision Register; serta pembaruan indeks dan workflow.
- Pemilik menegaskan: "total dari keseluruhan game adalah 1gb". Batas maksimal 1 GB berlaku keseluruhan game, bukan hanya paket APK. Lihat pengukuran di `TESTING.md`.
- Implementasi game belum dimulai atau diizinkan oleh sesi penataan ini. Seluruh hasil gameplay masih NOT_RUN.
- Persetujuan gameplay terbatas pada Modern15 dan kuota challenge yang diajukan. P-001, P-003 sampai P-007, P-009/P-010, P-012 sampai P-015 tetap berstatus sebelumnya. Tidak boleh mengunci kontrol atau desain career sepihak.
- Dampak dokumen: GDD, roadmap, AI_RULES, indeks, dan enam dokumen tambahan disinkronkan. Bukti tes game: belum ada.

## Izin implementasi M0–M1 — 2026-11-01
| D-015 | Izin implementasi M0–M1 approved 2026-11-01. |
| D-016 | Kontrol final: joystick virtual, arah pukulan via analog kiri yang sama (6 zona), ga ada aim assist/preview. Jump button + hold-swipe ke teknik pukulan dengan UI visual guide. Assisted footwork AI bantu gerak, player tentukan timing/power/aim. |
| D-017 | Scoring: default Official 15-point (BWF resmi). Classic 21-point terkunci, baru tersedia di Settings setelah career tahun 2028. Quick 16 dan preset lain dihapus. |
| D-018 | Challenge: error rate <5%, max 1 kesalahan call per match, hanya dekat garis (bukan obvious). Keputusan akhir dari posisi jatuh asli. |
| D-019 | Stamina: baseline rally 30 detik lob biasa. Recovery antar set tidak signifikan bahkan di stamina 100. Stat stamina tinggi = kapasitas + recovery lebih besar. |
| D-020 | Core attributes final: Power, Speed, Stamina, Technique, Balance/Recovery. Reaction dan Athleticism dihapus. |
| D-021 | Playstyle: preset cap per atribut visible ke player. Reset playstyle = atribut reset + pilih ulang + skill point balik 90%. |
| D-022 | Aging: muda tetap harus latihan untuk naik. Senior rajin latihan penurunan lambat/minimal. |
| D-023 | Finance: prestige/popularity/sponsor scope approved. Sponsor negosiasi UI sederhana, ga ada cutscene. |
| D-024 | Replay: 3 opsi setting (gameplay+additional angle, gameplay only, off). |
| D-025 | Audio: 57 SFX kategori diaudit dari 5 video referensi, katalog di badminton-sfx-catalog.md. SFX dicari/digenerate original via browser nanti. |
| D-026 | Doubles: top 3 rotasi (Front-Back, Side-by-Side, Free Rotation). Joystick free movement, UI hologram dots suggested position. Relationship via bonding/interview. Keluar rotasi = minor penalty kecuali selamatkan momen penting. |
| D-027 | Testing: Android Studio AVD emulator buat logic/stamina/AI/career testing. HP colok cuma pas testing feel kontrol & FPS benchmark. |
| D-028 | Scalable quality grafis: deferred, prioritas euforia audio + mekanik + realisme interaksi + kosmetik career. |
- Status: APPROVED oleh pemilik dalam chat. Dampak: implementasi lokal diizinkan sampai M1; bukan M2/career, cloud, biaya, atau penguncian kontrol final.
- Opsi/rationale: jalankan urutan M0 kemudian M1 sesuai dependensi; rollback backup + Git lokal, tanpa push.
- Tests: TESTING run M0-AUDIT-001. Gate M0 masih BLOCKED_LICENSE; izin tidak berarti acceptance lolos.

## Agent approval request template
**Decision ID:** ...
**Existing spec:** ...
**Why blocked:** ...
**Alternative A:** ... (consequences, cost, performance, legal risk)
**Alternative B:** ...
**Recommended for testing (not automatically chosen):** ...
**Evidence to show owner:** ...
**Owner approval/status:** PENDING
