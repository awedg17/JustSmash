# Just Smash — Error Log

Versi 0.2 | 2026-11-01

## Status awal

Belum ada bug aplikasi yang diamati: implementasi dan pengujian game belum dijalankan. Ini bukan klaim game bebas bug. Konflik spesifikasi yang diperbaiki dicatat di WORKFLOW dan Decision Register, bukan dipalsukan sebagai bug runtime.

## Aturan pencatatan

- Buat ID `BUG-001` dan seterusnya hanya saat ada temuan nyata.
- Catat error persis, repro, expected/actual, environment dan build/source identity.
- Root cause yang belum dibuktikan ditulis UNKNOWN; hipotesis tidak menjadi fakta.
- Maksimal tiga percobaan auto-fix per kegagalan sama. Setiap percobaan menyimpan perubahan dan hasil; jika masih gagal, status BLOCKED dan lapor pemilik.
- Solusi berhasil harus punya regression evidence; percobaan gagal tetap tercatat tetapi bukan solusi.
- Status OPEN, INVESTIGATING, BLOCKED, FIXED_VERIFIED atau REOPENED. Jangan tutup hanya karena compilation sukses.
- Jaga credential, data pribadi dan save user; redaksi log bila mengandung secret tanpa mengubah error teknis relevan.

## Template bug — belum merupakan temuan

| Field | Isi |
|---|---|
| ID / tanggal | ... |
| Deskripsi / severity | ... |
| Requirement / task / test | ... |
| Build hash / source snapshot / device / seed | ... |
| Langkah reproduksi | ... |
| Expected / actual / error persis | ... |
| Root cause | UNKNOWN sampai terbukti |
| Percobaan 1–3 | Perubahan, hasil, evidence tiap percobaan |
| Solusi terverifikasi | Kosong sampai ada perbaikan yang berhasil |
| File terdampak | ... |
| Regression command atau langkah | ... |
| Evidence / test run ID | ... |
| Status / sisa blocker | ... |

## Temuan aktual

### BUG-001 — Unity Editor belum aktif
- Status: BLOCKED. Severity: build blocker lingkungan, bukan bug gameplay.
- Scope: M0-03/06/17, T-01, run M0-AUDIT-001.
- Repro: jalankan Unity 6000.3.18f1 dengan `-batchmode -quit -createProject` untuk folder Game.
- Expected: project Unity dibuat. Actual: proses berhenti, return code 198.
- Error: `No valid Unity Editor license found. Please activate your license.`
- Bukti: `evidence/M0/create-project.log`, baris 84–95; entitlements kosong dan access token tidak tersedia.
- Penyebab teramati: Editor tidak menemukan lisensi valid. Status akun/eligibility pemilik belum diverifikasi.
- Percobaan: 1 eksekusi awal; tidak ada auto-fix/bypass. Pemilik perlu aktivasi lisensi melalui Unity Hub.
- Solusi/regression: belum; setelah aktivasi, ulangi createProject dan build nyata.
