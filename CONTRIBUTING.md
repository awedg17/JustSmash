# Konvensi kerja JustSmash

## Branch

Satu branch untuk satu fitur atau satu perbaikan. Branch dibuat dari `main`
terbaru dan dihapus setelah merge.

| Prefix | Dipakai untuk | Contoh |
|---|---|---|
| `feat/` | fitur atau perilaku baru | `feat/replay-per-set` |
| `fix/` | perbaikan bug yang sudah direproduksi | `fix/save-file-replace` |
| `test/` | menambah atau memperbaiki test saja | `test/lunge-gate` |
| `docs/` | dokumen saja | `docs/slim-handoff` |
| `chore/` | repo, build, konfigurasi, aset | `chore/apk-refresh` |

Slug: huruf kecil, tanda hubung, maksimal 4 kata, tanpa tanggal.

Aturan `main`: harus selalu bisa dibuka di Unity dan lolos smoke test.
Pekerjaan yang belum selesai tidak masuk `main`.

## Commit

Format Conventional Commits:

```
<type>(<scope>): <subject>

<body: kenapa perubahan ini ada, bukan kenapa kode ini benar>
```

- Type: `feat`, `fix`, `test`, `docs`, `chore`, `refactor`, `perf`.
- Scope: `arena`, `court`, `athlete`, `ai`, `replay`, `hud`, `audio`,
  `save`, `input`, `tests`, `docs`, `repo`, `unity`.
- Subject: imperatif, huruf kecil, tanpa titik di akhir, maksimal 72 karakter.
- Body: bungkus di 72 kolom, jelaskan alasan dan batasan. Sebut apa yang
  belum tercakup kalau ada.

Satu commit = satu alasan untuk ada. Jangan campur rapikan format dengan
ubah perilaku.

Perubahan perilaku menyertakan test-nya di commit yang sama. Dokumen ikut
di commit yang sama hanya kalau dokumen itu menjelaskan perubahan tersebut,
selain itu commit `docs:` terpisah.

Contoh:

```
fix(save): tahan retry saat File.Replace gagal karena backup terkunci

SaveMatch gagal intermiten dengan IOException "Unable to remove the file
to be replaced" ketika backup dibaca proses lain. Retry sekali setelah
handle dilepas. Penyebab kunci asli belum terbukti.
```

## Test

- Targeted test dulu, sesuai area perubahan.
- Suite penuh hanya sebelum build APK atau sebelum merge ke `main`.
- Test yang gagal permanen tidak dibiarkan merah di suite: perbaiki atau
  hapus dengan alasan tertulis di commit body.
- Jangan menjalankan dua Unity pada project yang sama.

## Larangan

- Jangan commit `evidence/`, `builds/`, video capture, `.backups/`, atau
  API key dan keystore.
- Jangan `push --force` ke `main`.
- Jangan gabungkan beberapa fitur dalam satu branch.
