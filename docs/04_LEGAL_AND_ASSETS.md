## D-058 — rekaman sepatu Rp0, 2026-11-15

Pemilik mengizinkan rekaman sepatu legal dan perbaikan siku/footwork existing saja. `Walking Footsteps on Wood` oleh sillygrizzlies, Freesound635058, published2022-05-23: halaman sumber menyatakan CC0 1.0 dan deskripsi sneaker pada hardwood. Public HQ MP3 preview dipakai, bukan original WAV. URL/provenance/hashes halaman, audio dan legalcode tersimpan di `evidence/shareable/20261115/roller-polish-040359/locomotion-audio-source/sources.json`.

Empat excerpt0,26detik pada2,58/4,13/5,82/7,82detik; mono48kHz PCM16, DC removal, peak gain0,30, fade-in3ms/fade-out25ms. Detail `derived-steps.json`; total4WAV100.016byte. `Assets/Resources/Audio/step-license.txt` mencantumkan penulis, URL, lisensi dan edits; ASSET_REGISTER diperbarui. CC0 mengizinkan reuse/modifikasi/distribusi komersial; credit tidak wajib tetapi dipertahankan. Lisensi tidak membuktikan kesesuaian suara sneaker/kayu untuk court badminton; listening masih NOT_RUN, bukan kualitas natural disahkan. Tidak memperoleh model/dependency/aset lain.

## Riwayat kebijakan D-053 — 2026-11-14

D-053 mengizinkan presentasi GOR, audio legal, tunggal/ganda putra, lalu career sesuai GDD. D-052 tetap melarang akses HP; izin device historis di bawah tidak aktif. Tidak biaya/dependency baru/push/proyek lain. NPC existing dipakai apa adanya; sportswear/Quaternius bukan antrean aktif.

BigSoundBank #2363 (Dorian CLAIR, applause) dan #3515 (Joseph SARDIN, indoor crowd) menyatakan CC0 pada halaman sumber. Bukti teks/SHA sumber dan WAV turunan: `evidence/shareable/20261114/audio/sources.json` dan `derived.json`. Impor mono22050Hz: applause7,248detik; room24detik. Kredit tersedia dalam game dan ASSET_REGISTER. audio-green1 13/13 menguji event/mix bounds/pause, bukan penerimaan kualitas audible.

## Riwayat kebijakan — Quaternius exhibition, 2026-11-04
Instruksi owner menjalankan `.hermes/plans/quaternius-exhibition-review.md` sampai selesai mengizinkan akuisisi/audit Quaternius Standard gratis CC0, outfit sportswear orisinal dan integrasi hanya setelah preview/deformasi lolos. Casual Set00 hanya referensi; sumber lama dipertahankan. D-045 tetap mengizinkan kalibrasi grade/stamina dan install/update/launch HP tanpa uninstall/clear data setelah QA lokal dan APK final. Tidak M3/career, biaya, dependency baru atau push. Riwayat kebijakan lama di bawah tidak membatalkan izin terbaru. Device/visual/audio dan review owner tetap gate terpisah.

## Kebijakan aktif — D-037..D-042, 2026-11-03
Instruksi owner terbaru menggantikan aturan lama yang bertentangan; bagian checkpoint/run lama adalah riwayat, bukan izin aktif. D-044 mengizinkan eksekusi M0–M2 sampai satu APK checkpoint baru; batas docs-only sebelumnya berakhir. Otorisasi implementasi tetap M0–M2; M3+ tidak dimulai otomatis, termasuk setelah acceptance M2. Tidak ada import aset/dependency, biaya, push, destructive reset, atau install/launch APK HP tanpa izin terpisah.

Review owner hanya pengalaman modul utuh: singles lengkap; character creation lengkap; career terintegrasi; final game. Ini kelompok review, bukan penambahan scope. Tidak meminta owner menguji pukulan/animasi/tombol/fault secara granular. Agent menangani unit/integration/regression, natural automated match, inspeksi video/motion/audio, dan performance sesuai izin. Keputusan desain tetap memerlukan approval, berbeda dari testing.

Subagent dan commit lokal bersyarat tetap D-038/D-039. D-043 menyetujui NPC Casual Set00 untuk evaluasi import, default joystick Fixed (Floating tetap pilihan), servis charge1detik/power0..1/feedback konsisten/cancel aman. Grade hanya Timing + Contact; Direction/Power trace tanpa kontribusi grade. Rally sekitar30detik menurunkan stamina cukup banyak dan mulai memengaruhi performa, tetapi belum habis dan rally bisa berlanjut; jalan/lari drain0. Bobot/formula grade, biaya/kurva/efek stamina baru belum approved. Unity Personal memenuhi kondisi pribadi non-klien tanpa pendapatan yang dinyatakan owner; tidak meminta pembahasan keuangan lagi kecuali kondisi/terms relevan berubah. Target karakter/animasi/atmosfer singles tetap. Approval evaluasi NPC bukan izin import aset lain/dependency/M3 atau device action. D-044 menggantikan batas docs-only; eksekusi approved sampai APK checkpoint, tanpa mengubah proposal angka menjadi approval.

Rujukan keputusan/proposal: [Decision Register](05_DECISION_REGISTER.md). Riset aset/lisensi: [Legal dan Assets](04_LEGAL_AND_ASSETS.md). Gate bukti: [Testing](TESTING.md).

# JUST SMASH — Legal, IP, Assets, Terms & Privacy Plan
Versi 0.1 | Riset 2026-11-01 | **Risk-management checklist, bukan nasihat hukum profesional atau jaminan anti-gugatan.**

## Procedural audio provenance — M2 (riwayat sebelum rekaman D-036)
- `Assets/Scripts/CourtAudio.cs`: generated mono22050Hz waveforms from sine/noise synthesis, deterministic System.Random(37); racket/net/step/whistle/score/crowd. No downloaded samples, broadcast audio, music or commentary. Created locally during approved D-029 M2 implementation; no external asset license claimed or required for imported samples because none imported. AI-assisted source authorship is not legal clearance guarantee. Listening acceptance pending.

## Evaluasi aset D-042 — 2026-11-03; EVALUATED_NOT_IMPORTED

Harga/spec adalah klaim listing saat diperiksa, bukan hasil import/profiling. Semua kandidat di bawah belum dipilih, diunduh sebagai paket, diimport atau dibuktikan berjalan di Unity proyek. Ukuran paket bukan RAM runtime/APK. Rigged tidak otomatis Unity Humanoid Avatar valid. Sportswear tidak bisa dijamin hanya dengan recolor: celana panjang tidak menjadi shorts, armor tidak menjadi jersey.

| Kandidat | Harga / lisensi listing | Geometry / texture / paket | Rig, kustomisasi dan sportswear | Penilaian |
|---|---|---|---|---|
| [Insurgent Lite](https://assetstore.unity.com/packages/3d/characters/humanoids/humans/insurgent-lite-274543) | FREE; Standard Asset Store EULA; listing Extension Asset | LOD0 1,742tri/1,061vert; LOD1 1,316tri/827vert;5 texture2048;3mesh/3prefab;16.9MB; jumlah material tidak dinyatakan | Rigged, UE5 skeleton ready; Humanoid Unity belum eksplisit. Wajah tertutup balaclava, tactical trousers/kneepad/boots/rifle terlihat pada preview. Unmasked head, detachable clothes, sportswear dan morph belum terbukti | Paling ringan secara mesh, tetapi gagal target wajah/sportswear tanpa pekerjaan mesh yang belum diizinkan. Bukan pilihan utama |
| [Medieval Assassin Girl](https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/medieval-assassin-girl-326921) | FREE; Standard EULA; Extension Asset |54,896vertices, jumlah triangles/material belum dinyatakan; body/armor `4090` menurut listing (jangan dianggap pasti4096), weapon2048;538.8MB |49 animasi combat,93blendshapes,5set tekstur, hair/cloth simulation. Humanoid belum dikonfirmasi. Preview wajah partly masked, hood/braid/loose sleeves fantasy, bukan athlete | Detail stylized kuat tetapi paling berisiko mobile dan konversi pakaian; animasi combat bukan badminton. Tidak direkomendasikan untuk baseline Android |
| [Character Constructor](https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/character-constructor-medieval-fantasy-armor-modular-prototype-303401) | FREE; Standard EULA; Extension Asset |40kvertices untuk SEMUA80part, bukan per karakter;5material seluruh part;4Kmaps;tanpaLOD;1.3GBpaket | Male/female, facial blendshape, modular gear/decal/color, rigged; Humanoid Avatar belum dibuktikan. Preview wajah/angular anatomy terbaca, armor/hood/chainmail bukan sportswear. Cloth plugin hanya rekomendasi publisher, **tidak diperlukan/diizinkan ditambah** | Runner-up untuk fleksibilitas wajah/body, tetapi outfit olahraga tidak disertifikasi; perlu stripping/texture budget/LOD. Paket1.3GB tidak otomatis melanggar game1GB karena bukan runtime, tetap risiko payload |
| [NPC Casual Set00](https://assetstore.unity.com/packages/3d/characters/humanoids/humans/npc-casual-set-00-326131) | FREE; Standard EULA; Extension Asset |~3,500–4,000tri/character LOD0;LOD0–4;101mesh/129prefab seluruh pack;maps512 sampai2048, listing1042x512/1042x1024;273.7MB;material count tidak dinyatakan | Rigged FBX, no animations/no facial rig; detailed facial blendshapes dan10face material variants;2body types+athletic/overweight/underweight,7clothing pieces. Maya source, bukan kewajiban owner mengedit. Preview wajah terbuka, torso/lengan proporsional, shirts/jeans/trousers; shorts/court shoes belum terbukti | **Rekomendasi pertama untuk technical evaluation setelah izin**: wajah/customization/LOD paling relevan. Jangan membeli extensions; gratis belum membuktikan sportswear lengkap. AI-generated T-shirt prints diungkap publisher; hindari print itu untuk jersey original |

Semua empat preview resmi diinspeksi visual (bukan render lokal). Constructor dan NPC wajah lebih terbuka; Assassin/Insurgent menutupi sebagian/banyak wajah. Penilaian image tidak membuktikan joint deformation, grip, foot plant atau retargeting. Pipeline listing: Insurgent2022.3.62 semua; Assassin6000.0.25 semua; Constructor2022.3.23 semua dengan remap material SRP; NPC kompatibilitas6000.2.8 Built-in/URP, bukan HDRP, original2022.3.25. Tidak membuktikan kompatibilitas penuh Unity6000.3.18 proyek.

### Alternatif gratis dan animasi
- [Quaternius Universal Base Characters](https://quaternius.com/packs/universalbasecharacters.html): CC0 commercial/modification,6male/female base Regular/Teen/Superhero,20hair,Humanoid,~13ktri rata-rata,FBX/glTF. Free tier tidak sama seluruh Source/engine setup; publisher menyebut60–70%pack free. Daftar tepat subset gratis, sportswear,material/texture/LOD belum diverifikasi. Cocok baseline anatomy/retarget, belum solusi jersey/shorts otomatis. Jangan menjanjikan source/shader berbayar masuk gratis.
- [Quaternius Universal Animation Library](https://quaternius.com/packs/universalanimationlibrary.html): CC0,120+animasi keseluruhan,Humanoid/Unity retarget,8-direction locomotion. Free subset perlu diperiksa sebelum acquire. Tidak ada bukti set badminton lengkap (smash/backhand/lunge/dive/recovery/contact) dari listing.
- [Mixamo FAQ Adobe](https://helpx.adobe.com/creative-cloud/faq/mixamo-faq.html): gratis Adobe ID, character+animation royalty-free personal/commercial/game; bukan CC0, bukan izin redistribusi raw library. Biped humanoid only. Kandidat locomotion/rig, bukan klaim punya clip badminton tepat. Tidak login/upload karakter atau download pada sesi ini. Tidak mewajibkan owner Blender.

### Gate Android RAM4GB dan sportswear
Target tetap30FPS dan keseluruhan game<=1,000,000,000bytes; **belum ada kandidat lulus device QA**. RAM perangkat4GB bukan budget aplikasi. Hipotesis optimasi: dua atlet saja, pilih mesh yang dipakai,LOD aktif, batasi skinned material/draw calls, disable cloth/hair simulation yang tak dibutuhkan, strip tekstur/prefab/demo tak terpakai, kompres Android sesuai platform. Semua hipotesis perlu profiling sesudah import diizinkan. Satu texture RGBA32 2048²=16MiB,4096²=64MiB tanpa mip; dengan mip lengkap kira-kira4/3. Ini hitungan uncompressed, bukan prediksi runtime format ASTC atau RAM game.

Sebelum owner memilih: NPC paling cocok tetapi **belum ada bukti empat kandidat menyediakan sportswear badminton lengkap tanpa edit mesh**. Jangan menyebut ganti warna sebagai solusi shorts. Setelah izin, spike Avatar/retarget/skin deformation/racket contact/jersey-short-shoe completeness, baru ukur heap/native/GPU memori,drawcalls,FPS/frametime/thermal/footprint pada qualifying device sesuai izin. Jika outfit tidak tersedia, ajukan aset olahraga gratis spesifik atau pekerjaan konversi terotorisasi; jangan mengalihkan model creation ke owner.

### Lisensi Asset Store dan Unity Personal
- [Standard EULA](https://unity.com/legal/as-terms), diperiksa2026-11-03: izin aset embedded dalam Licensed Product, modifikasi sesuai EULA; bukan redistribusi mesh/texture mandiri/public asset repo. Listing keempatnya berlabel Extension Asset: pertahankan label dan periksa seat/contractor terms, jangan menganggap CC0. Bukti per-package/Third-Party Notices baru dapat diperiksa ketika acquire diizinkan; EULA bukan jaminan non-infringement.
- [Unity Editor Software Terms](https://unity.com/legal/editor-terms-of-service/software), last updated June30,2026, §1.1: Personal Total Finances **tidak melebihi US$200,000** selama trailing12bulan, dinilai setiap hari penggunaan. Individual bukan jasa klien: amount generated in connection with Unity use. Jika jasa klien: gross revenue/funding klien; legal entity memakai aggregate revenue/funding. FAQ marketing [Personal](https://unity.com/products/unity-personal) menyebut less than200K; pada batas tepat gunakan terms kontraktual.
- D-043: owner menyatakan proyek pribadi belajar/hiburan, bukan perusahaan/klien, pendapatan nol. Terms §1.1 diperiksa ulang: individual non-client memakai amount generated in connection with Software, <=US$200,000/trailing12bulan. Berdasarkan kondisi yang dinyatakan, Unity Personal gratis memenuhi syarat; blocker finansial ditutup. Tidak meminta pembahasan keuangan lagi kecuali kondisi/terms relevan berubah. Aktivasi teknis dan audit toolchain berbeda dari eligibility; tidak ada subscription berbayar.

## A. Prinsip keamanan hukum
- **Game gratis tetap tunduk hak cipta, merek, hak personalitas, kontrak, dan kebijakan platform.** 'Cuma eksperimen' dan 'dibuat AI' bukan pembebasan otomatis.
- **Mengubah nama atlet asli satu huruf bukan jaminan aman.** Nama, ciri visual, riwayat, klub, gaya representasi, dan kombinasi petunjuk bisa membuat identitas asli tetap jelas. WIPO menjelaskan perlindungan nama, likeness, dan atribut personal atlet. Buat karakter **original**; hindari membuat representasi yang secara eksplisit dikenal sebagai versi terselubung figur asli.
- **Pakai struktur olahraga, jangan duplikasi IP event.** Skor/aturan permainan/fakta olahraga dapat jadi referensi fakta, tetapi jangan meniru logo BWF, Olympic rings, logo PBSI, branding 'World Tour' tertentu seolah resmi, sponsor nyata, maskot atau dekorasi broadcast, grafis scoreboard spesifik. Nama event fiktif dan visual sendiri.
- Referensi 'Valorant', 'Badminton Jump Smash', screenshot menu dan NBA 2K/EA FC adalah **arah rasa**. Jangan clone character model, UI layout pixel-exact, fonts proprietary, logo, animasi, suara, atau source code.
- Hak cipta software/konten bukan otomatis beres dengan menambahkan Terms & Conditions: T&C mengatur hubungan dengan pengguna, bukan memperoleh lisensi materi orang lain.

## B. Nama JUST SMASH
Nama belum dibuktikan bebas hak merek. Lakukan clearance di database DJKI PDKI, Google Play, toko aplikasi lain, pencarian web global, dan kelas barang/jasa game/software (cek klasifikasi sesuai kebutuhan). Cari kemiripan ejaan, bunyi, logo, bukan hanya exact match. Hasil tidak ditemukan pun bukan jaminan legal global. Sebelum distribusi luas minta penilaian orang yang memahami IP jika mampu.
- PDKI: https://pdki-indonesia.dgip.go.id/
- DJKI FAQ merek: https://dgip.go.id/faq/daftar-faq/merek/Merek-Pelayanan%20Teknis
- UU Hak Cipta Indonesia, sumber teks resmi: https://jdih.dgip.go.id/produk_hukum/view/id/346/t/undangundang%2Brepublik%2Bindonesia%2Bnomor%2B28%2Btahun%2B2014%2Btentang%2Bhak%2Bcipta
- WIPO olahraga/personality rights: https://www.wipo.int/en/web/sports/branding
- WIPO petunjuk IP/trademark: https://www.wipo.int/web-publications/guidelines-for-esports-players/en/what-should-players-look-out-for.html

## C. Engine, lisensi, dan model biaya
- Unity Personal terdaftar gratis untuk eligibility tertentu, termasuk ambang pendapatan/pembiayaan US$200.000 menurut informasi Unity 2026, dengan Runtime Fee dibatalkan. **Wajib cek terms saat install/update/rilis**; jangan add package Pro/trial subscription. https://unity.com/products/pricing-updates
- Godot MIT bisa jadi fallback gratis bila Unity tidak cocok, tetapi **jangan memigrasi sepihak**. https://godotengine.org/license/
- Unity Asset Store license per asset tetap berlaku dan dapat punya pengecualian; tidak semua 'free' otomatis bebas untuk semua penggunaan. Terms penggunaan marketplace juga dapat memiliki persyaratan usia/akun: https://unity.com/legal/as-terms
- Google Play pendaftaran penuh US$25, akun Play Console mensyaratkan 18 tahun, serta akun personal baru umumnya closed testing minimal 12 tester 14 hari sebelum akses produksi. Pengembangan dan sideload lokal bisa tetap Rp0; jangan mendaftarkan memakai identitas palsu. Syarat distribusi tertentu untuk Android dapat berubah; cek sebelum rilis.
  - https://support.google.com/googleplay/android-developer/answer/6112435
  - https://support.google.com/googleplay/android-developer/answer/14151465?hl=id
  - https://support.google.com/android-developer-console/answer/16604405?hl=en

## D. Asset sourcing policy
Prioritas: (1) dibuat sendiri/original, (2) CC0 / public-domain dengan sumber kuat, (3) CC BY jika atribusi yang sesuai dapat dipenuhi, (4) MIT/OFL sesuai komponen, (5) Unity free assets yang EULA-nya diaudit. Jangan mengimpor aset unlicensed atau NC/ND/SA tanpa review konsekuensi; apalagi rip dari video/siaran/youtube. CC BY-NC tidak boleh otomatis diasumsikan layak untuk game yang mungkin dipasarkan di masa depan.
- Creative Commons lisensi: https://creativecommons.org/cc-licenses/
- Rekaman sorakan 'ea ea' sebaiknya **rekam suara sendiri** atau buat dari audio legal yang persyaratannya jelas; contoh video pengguna hanya referensi ritme/momen, bukan sumber file audio. Jangan mendistribusikan cuplikan voice rekaman TV pertandingan.
- Jangan pakai model/voice/face generator AI dari seseorang nyata tanpa izin. Hasil model AI dari platform tidak selalu punya hak pengguna penuh; cek terms model masing-masing jika dipakai.
- Tidak ada logos resmi pada jersey, lapangan atau nama kompetisi tanpa lisensi.

**Mandatory `ASSET_REGISTER.csv` columns:**
`id, file_path, type, description, creator, source_url, acquired_date, license_name, license_url, evidence_file, commercial_use_allowed, modification_allowed, attribution_text, consent_or_approval, reviewed_by, review_date, status`

Default status unreviewed = BLOCKED, tidak masuk build release. Simpan bukti lisensi/izin pada tanggal download karena license dapat berubah.

## E. Privacy-first and no-online design
User meminta offline/local-only tanpa akun. Itu mengurangi kebutuhan data, tetapi bukan bukti game tidak memproses data. Audit Unity packages dan permission Android: analytics, crash SDK, device identifiers, telemetry, advertising, network, file access, microphone. Jangan klaim 'tidak mengumpulkan data' jika engine/package ternyata mengirim sesuatu. Matikan analytics dan cloud bila tidak disetujui. Simpan data save hanya lokal, sediakan delete. Jangan mengirim state gameplay ke server.

## F. Draft Terms of Use (belum siap dipublikasikan tanpa pemeriksaan fakta & kelengkapan)
**Just Smash — Terms of Use (DRAFT)**
- Penerbit: `[NAMA/PENANGGUNG JAWAB YANG SAH]`; kontak: `[KONTAK]`; tanggal mulai berlaku `[TANGGAL]`.
- Just Smash adalah game hiburan fiktif yang menggambarkan simulasi badminton. Semua tokoh, sponsor, event, dan merek di dalam game (kecuali yang secara sah berlisensi dan dinyatakan) dibuat untuk keperluan fiksi; tidak terafiliasi atau didukung organisasi badminton atau atlet nyata.
- Pengguna diberi izin pribadi, tidak eksklusif, untuk menggunakan salinan aplikasi sesuai hukum dan kebijakan platform. Hak atas aset kode/visual/audio asli tetap milik pemegang hak masing-masing.
- Pengguna tidak diperbolehkan menggunakan aplikasi untuk aktivitas yang melanggar hukum atau melanggar hak pihak ketiga; tidak ada pembatasan yang bertentangan dengan hak konsumen yang tidak bisa dikesampingkan.
- Game dapat menyediakan penyimpanan save lokal; aplikasi dapat berubah dalam pembaruan. Kami berupaya menjaga kompatibilitas save, tetapi kesalahan teknis mungkin terjadi. Gunakan fitur penyimpanan sesuai instruksi.
- Kami menyediakan aplikasi sebagaimana adanya, sepanjang diperbolehkan hukum. Ketentuan ini tidak meniadakan hak hukum konsumen atau kewajiban yang tidak dapat dibatasi oleh hukum.
- Ketentuan hukum dan penyelesaian sengketa: `[YURISDIKSI/MEKANISME YANG TELAH DIREVIEW]`.
- Pertanyaan: `[KONTAK]`.

## G. Draft Privacy Notice (hanya gunakan apabila terbukti benar melalui audit)
**Just Smash — Privacy Notice (DRAFT)**
- Penerbit dan kontak: `[NAMA]`, `[KONTAK]`; tanggal efektif: `[TANGGAL]`.
- Versi aplikasi ini dirancang untuk dimainkan offline tanpa membuat akun. Data karier, preferensi kontrol, pengaturan, serta save game disimpan secara lokal pada perangkat.
- `[HANYA JIKA TERBUKTI]` Aplikasi tidak mengirim data permainan, identitas, atau informasi perangkat ke server milik penerbit atau pihak ketiga; tidak ada iklan, cloud save, maupun analytics.
- Pengguna dapat menghapus save dari dalam game dan/atau menghapus penyimpanan aplikasi dari perangkat. Menghapus aplikasi biasanya menghapus data lokal yang bersangkutan, dengan ketentuan backup sistem operasi di luar kendali aplikasi bisa berlaku.
- Jika nantinya ada SDK yang mengirim data, fungsi online, pembayaran, atau analitik, notice ini wajib direvisi sebelum update; dokumentasikan jenis data, tujuan, dasar pemrosesan, penerima, retention, dan hak pengguna sesuai hukum yang berlaku.
- Kontak privasi: `[KONTAK]`.

**Note:** Draf ini bukan pengganti review kepatuhan UU PDP, persyaratan toko aplikasi, serta data safety declaration saat penerbitan. Hindari mengisi placeholder dengan data pribadi yang tidak ingin dipublikasikan; gunakan kanal kontak profesional dengan izin pengelolanya.

## H. Publication gate
`RELEASE_BLOCKED` bila:
- ada aset tanpa lisensi/izin jelas; nama game & logo belum dicek; artis/pemain nyata di-copy atau terlalu mirip;
- T&C/Privacy placeholder masih kosong; teks privasi tidak sejalan dengan implementasi SDK;
- tidak ada persetujuan pemilik atas scope/fitur final;
- distribusi membutuhkan biaya yang bertentangan dengan Rp0;
- distribusi membutuhkan akun/usia/identitas yang belum memenuhi persyaratan.

## I. Ringkasan sumber olahraga untuk riset agent
- 3x15 mulai 2027: https://pbsi.id/2026/04/26/bwf-resmi-setujui-sistem-skor-15-x-3-berlaku-mulai-4-januari-2027/
- BWF World Ranking best 10/52 weeks; cek dokumen edisi 2026, jangan memakai tabel 2018 tanpa verifikasi: https://badscale.jp/article/watch/world-ranking/ ; arsip resmi lama https://system.bwfbadminton.com/documents/folder_1_81/Statutes/CHAPTER-5---TECHNICAL-REGULATIONS/Section%205.3.3.1%20World%20Ranking%20System.pdf
- Usia U19/junior: https://system.bwfbadminton.com/documents/folder_1_81/Statutes/CHAPTER-5---TECHNICAL-REGULATIONS/Section%205.3.3.2%20-%20World%20Junior%20Ranking%20System%20-1.pdf
- Sirkuit Nasional kelompok U17/U19/senior: https://pbsi.id/2026/04/13/surabaya-buka-rangkaian-hydroplus-sirkuit-nasional-a-2026-lebih-dari-1-100-atlet-siap-bertanding/
- Olympics 2028 qualification index: https://www.olympics.com/ioc/documents/olympic-games/los-angeles-2028-olympic-games (event fiktif jangan pakai logo resmi)
