# Polish sebelum rilis

Arahan pemilik: feedback kontak, kandidat smash terpisah, lalu patch siku dan crowd. Tidak push. Kandidat smash belum boleh masuk build playtest sebelum persetujuan. Android dan video pertandingan utuh menyusul.

## Batas tetap

Gerak badan/kaki, tampilan kok, Official15, stamina, aim/target/power dan waktu kontak dipertahankan. Tidak mengubah smash menjadi lob atau menjamin semua kontak lolos net. Tidak ada paket baru.

## Penerimaan

- Kontak: kok rendah sama sah Lift/Net dan ditolak Smash; di luar jangkauan; buffer sukses sebelum .16 s. Kedua ujung. HUD satu pesan rally yang dibuktikan keputusan CanReach; hasil poin mengambil slot sesudah rally. Kontak sah tetap tick sama, miss tidak mengurangi energi atau memutar audio.
- Kandidat smash: front (0,2.10,-1.50), rear (0,2.10,-5.50), rear-high (0,2.40,-5.50), target (0,.04,4.40), aim nol/power .5. Cermin z untuk ujung lain. Durasi .40–1.00 per .02, vy awal <=0, crossing interpolasi >=NetTop(x)+.08. Pilih terdekat durasi lama; tanpa solusi gunakan velocity lama persis.
- Visual: JustSmash-Visual-Fixes.patch dari lampiran. Bandingkan Home pada 350 s JustSmash-Normal-1x-review.mp4. Rekam ready, overhead, recovery dan crowd dari kamera pertandingan kedua ujung. Tidak menganggap tes pose sebagai persetujuan natural motion.
- Bukti: diff terpisah, hasil tes Unity, klip terarah 1x dengan audio game. Fixture otomatis diberi label controlled, bukan playtest manual.

## Hasil lokal

Kontak dan visual terpasang di branch `fix/release-polish`, tidak commit/push. Diff terpisah ada di `evidence/release-polish/deliverables/`. Kandidat smash sudah dikeluarkan dari source aktif beserta tes khususnya; `02-smash-candidate-held.patch` bisa diterapkan sesudah patch kontak/visual dan menunggu persetujuan sebelum build. Pekerjaan lama `MobileMatchTests.cs` byte-identik dengan backup awal; `Singles-Feel-Review.md` tidak disentuh.

- Kontak/visual: `green-contact-visual` 29/29 Unity PASS, sourceDelta=[], dataChanged=false. Termasuk 24 kombinasi kontak/release/expired-buffer, buffer menjadi terjangkau kedua ujung, miss tanpa biaya energi/audio dan notice poin mengalahkan pesan rally. Pukulan sah tidak ditunda dari tick semula.
- Kandidat: `green-smash-fixture2` 6/6 tes PASS, termasuk enam fixture smash. Kedua ujung: depan .68 s, vy -0.0174839571, crossing 1.97913134 m; belakang 2.10 m tanpa kandidat, fallback crossing 1.49966717 m; belakang 2.40 m .66 s, vy -1.23211694, crossing 1.63292062 m. Batas crossing 1.604 m. Fallback diuji persis terhadap power charge aktual, bukan jam float yang dianggap tepat .5.
- Home referensi 350 s dibandingkan render sesudah: siku non-raket lebih dekat badan dan wrist di depan badan; ini review still, bukan persetujuan motion. Kamera pertandingan merekam ready/overhead/recovery dan crowd kedua ujung.
- Tiga MP4 paired: kontak 24.042 s, smash 29.077333 s, siku/crowd 9.727333 s. Urutan sebelum lalu sesudah. Controlled PC fixtures, audio game asli, tanpa retime/padding. Full decode dan packet PTS/DTS PASS; browser full 1x muted ended/error-null pada ketiganya, dropped frames 5/14/0 saat playback paralel. Tidak ada klaim audible review.
- Gate AV historis tetap FAIL pada beberapa source clip: baseline gap 81.67/103.33/118.67 ms (kontak/smash/visual), after kontak/visual 20.67/18/56 ms; smash kandidat 132 ms. Metadata baseline lastWall rusak dipertahankan, bukan dibuat-buat. Ini artefak rekaman, bukan bukti bug audio game atau blocker produk D-064.
- Runtime aktif hash-identik dengan run 29/29 dan capture after-contact-visual. Patch reconstruction dari HEAD + patch kontak/visual menghasilkan runtime identik, lalu patch kandidat menghasilkan runtime yang diuji Unity. Tidak ada APK baru atau whole-suite/release/device/owner acceptance.

## Status awal

Branch fix/release-polish. Backup awal .backups/release-polish-start. Perubahan owner MobileMatchTests.cs dan Brain/Singles-Feel-Review.md tidak ikut commit patch.

RED Unity evidence/release-polish/red-contact-visual: 0/6 PASS, enam gagal sesuai missing HUD/pose/crowd; sourceDelta=[], dataChanged=false. Baseline reference Home pada evidence/release-polish/home-reference-350.png: siku free arm menjorok ke samping. Simulasi float32 eksternal cocok .68/tanpa solusi/.66 dari kedua ujung, belum penerimaan Unity.
