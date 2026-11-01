# JUST SMASH — Legal, IP, Assets, Terms & Privacy Plan
Versi 0.1 | Riset 2026-11-01 | **Risk-management checklist, bukan nasihat hukum profesional atau jaminan anti-gugatan.**

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
