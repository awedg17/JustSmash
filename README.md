# JustSmash

Game bulu tangkis Android, Unity 6000.3.18f1, PC-first development.
Repo ini sumber kebenaran kondisi terkini proyek untuk dibaca agent maupun
manusia. Artefak besar tetap lokal, tidak di-commit.

## Status

| Area | Kondisi |
|---|---|
| Build | APK terakhir 21 Nov 2026 (`builds/JustSmash-Mobile.apk`, 30.2 MB). Build tidak masuk git. |
| Gameplay | Singles + ganda Normal/Easy/Hard playable di PC. AI, aturan skor, stamina, replay, HUD, save lokal. |
| Test | 31 file test EditMode/PlayMode. Sebagian suite masih punya kegagalan lunge yang diketahui. |
| Aset karakter | Terbuka. Set pihak ketiga yang diuji tidak lolos kualitas, tidak diintegrasikan. |
| Save | Terbuka. `File.Replace` gagal intermiten, pemilik lock belum terbukti. |
| Android | APK lama, belum diuji di perangkat pada source sekarang. |

Rincian keputusan dan riwayat panjang ada di `docs/05_DECISION_REGISTER.md`
dan `docs/SESSION_HANDOFF.md`. Scope yang berlaku sekarang: `Brain/Scope-D064.md`.

## Struktur

```
Assets/
  Scripts/        16 script runtime: CourtPrototype, CourtDoubles, CourtOpponent,
                  AthleteVisual, CourtReplay, CourtUi, CourtAudio, LocalGameData
  Tests/          31 file test EditMode + PlayMode
  Resources/      prefab, shader, font, audio, prototype-config.json
  Scenes/         Boot.unity
  Editor/         build APK, setup boot, audit aset
  QuaterniusAudit/ hasil audit set karakter pihak ketiga
docs/             PRD, GDD, roadmap, arsitektur, testing, error log, keputusan
Brain/            MOC Obsidian, session log, scope aktif
.hermes/plans/    plan historis dan plan overhaul yang sedang dijalankan
ProjectSettings/  konfigurasi Unity
Packages/         manifest paket
```

## Yang sengaja tidak ada di repo

| Item | Ukuran | Alasan |
|---|---|---|
| `evidence/` | 16 GB | capture video, PNG, JSON verifikasi lokal |
| `Assets/npc_casual_set_00/` | 527 MB | set karakter pihak ketiga, tidak diintegrasikan |
| `builds/` | 215 MB | APK |
| `*.mp4`, `*.zip`, `.backups/` | ~3.2 GB | capture dan backup lokal |

Clone repo ini cukup untuk membaca seluruh source, test, dan dokumen.
Untuk menjalankan Unity, folder di atas harus ada di disk.

## Konvensi kerja

Branch dan commit: lihat [CONTRIBUTING.md](CONTRIBUTING.md).

## Build dan test

Build APK dari CLI Unity (lihat `Assets/Editor/AndroidBuild.cs`):

```bash
"/c/Program Files/Unity/Hub/Editor/6000.3.18f1/Editor/Unity.exe" \
  -batchmode -quit -projectPath . \
  -executeMethod JustSmash.EditorTools.AndroidBuild.BuildMobile -logFile build.log
```

Test PlayMode butuh Editor terbuka atau `-runTests -testPlatform PlayMode`.
Jangan menjalankan dua Unity pada project yang sama.
