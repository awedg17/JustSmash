# Review tunggal Normal

Scope terbaru: instruksi pemilik pada lampiran pasted_content_2026-11-25_07-23-20-283_93828a.txt. Branch: `test/singles-feel`. Tidak ada izin push pada permintaan ini.

## Batas

Pertahankan gerak badan/kaki, kok, Official 15, dan mode ganda. Penyiar ditunda. Kandidat siku non-raket/crowd perlu ditelusuri dan diperiksa, bukan otomatis dianggap diterima. Rekaman 1x dengan audio game diminta kembali; ini memperbarui penundaan bukti rekaman dalam Scope-D064, tanpa mengizinkan tuning physics/reach/timing/balancing. Save/load tetap jalur terpisah. Unity PC otomatis tidak setara playtest Android.

## Bukti yang sedang dibuat

- Baseline pertandingan tunggal Normal penuh, seed 71, kamera/HUD runtime, kontrol otomatis melalui SetAnalog/TapShot/Serve. Pemilih pukulan otomatis lama hanya Clear/Smash/Lift; tidak membuktikan penggunaan taktis Drop/Net.
- Test-only entrypoint `MobileMatchTests.CaptureNormalSinglesWithGameAudio` membatasi capture ke Normal. Mulai PCM pada frame pertama setelah encoder siap, meniru batas capture replay yang sudah ada. Tidak mengubah audio runtime.
- Run: `evidence/singles-feel/20261125-143051/`. Unity 1/1 PASS, source dan save/prefs identik sesudah teardown.
- Video review: `evidence/singles-feel/20261125-143051/JustSmash-Normal-1x-review.mp4`, 1280x720, 10524 frame, video 350.798s, audio 350.954s, 42,499,750 byte. Full decode exit 0 dengan stderr kosong. Frame 30s diperiksa: court, pemain, kok, HUD tampil. Timeline memuat MatchEnd dan kembali Home; game elapsed 350.7823s vs wall 350.9190s.
- Audio asli dimux AAC tanpa trim/padding/retime; selisih durasi sekitar 157ms. Sinkronisasi audiovisual dan listening BELUM lolos; ini baseline review, bukan release acceptance. Skenario terarah dan klip before/after belum selesai.

## Temuan source, belum bukti gameplay

- `CourtPrototype.ReleaseShot` menghitung LastTiming/LastContact/LastGradeScore, tetapi target/durasi outgoing tidak membaca nilai tersebut. Kualitas dipakai label dan SpectacularMoment. Posisi/tinggi aktual tetap memengaruhi lintasan dan eligibility. Jangan menyimpulkan semua pukulan identik.
- `CourtOpponent.UpdateOpponent`: Smash eksplisit hanya pada Hard; Normal bisa Clear/Lift/Drop/Net. Short clear yang memberi serangan Normal perlu diuji, tidak boleh dinyatakan lolos dari pertandingan selesai.

## Antrean terbatas

1. Verifikasi baseline selesai, kecepatan, audio, decode; berikan path video.
2. Controlled paired scenarios dari dua ujung: clear dalam/pendek; drop lawan belakang/depan; net awal/telat; smash bagus/sulit. Catat kontak, posisi, lintasan, AI, hasil dan kegagalan.
3. Periksa kontak audiovisual, clean/late/reach/net/out, siku/crowd kontinu dan replay akhir set. Jangan ubah tuning tanpa bukti dan izin.
4. Klip before/after hanya untuk perubahan nyata. Tidak ada klaim siap dibagi sebelum APK dan playtest yang diminta.

## Playtest teman

APK tanpa tutorial lisan. Setelah satu pertandingan tanyakan: Kenapa kalah pada poin itu? Pukulan apa dipilih untuk membuat lawan berlari? Kapan merasa bisa menyerang? Catat jawaban dan cocokkan timestamp kejadian. Belum dilakukan.
