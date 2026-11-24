## BLENDER-047 — BLOCKED_NOT_ELIGIBLE_FOR_INTEGRATION, 2026-11-04
- D-047 explicit owner authorization: official free Blender only; one jersey/shorts/court-shoes outfit, isolated outside Assets. No final import, APK or phone action. No additional dependency, paid asset or push.
- Official portable Blender4.5.15 LTS installed under `C:/Users/Dewangga Ramadhan H/AppData/Local/JustSmashTools/blender-4.5.15-windows-x64/`. Official ZIP398,649,826bytes; SHA256 `52dd02fd74877fa08e74b67d7854cf32468a232c02aaa19b6844c91285574f8d` matches official checksum. License/source `blender/tool-provenance.json`; costRp0. Blender license permits commercial artwork, base remains licensed Quaternius CC0.
- Backup `.backups/blender_outfit_20261104_151853/`. Source/runtime Assets/Scripts and Assets/Resources hash-identical to `blender/baseline-inputs.json`; old FBX/outfit retained. Work is `.blend` and scripts/evidence outside Unity Assets, not imported/shipped assets.
- Three modeling attempts executed and retained. Native primitives + voxel union + bounded nearest-surface fitting + barycentric skin weights, no copied Casual geometry. Attempt1 had collapsed fitting faces and footwear detachment; attempt2 normalized imported transforms and removed many degeneracies but remained bulky with shoulder deformation; attempt3 reduced silhouette/bounded fit and removed global shoulder-plane cut, still fails. Two initial script-harness errors (`NoneType world.color`, `.normalized` missing call) fixed before first rendered candidate; logs retained separately, not disguised as mesh evidence.
- Final mesh: Jersey15,616verts/31,070tris,1component,30 nonboundary nonmanifold edges,15 zero-area faces; Shorts8,937verts/17,490tris,1component,7 nonboundary nonmanifold edges,17 zero-area faces. Each shoe610verts/1,216tris,1component,closed topology. All vertex weight sums within.001; finite evaluated vertices across6 poses. These partial passes do not establish correct seam locations/rig mapping/coverage.
- Same six fixed sampled poses rendered front/back/side/diagonal: rest,lunge,overhead,jump,backhand,dive-save (24 actual PNGs per attempt). Final diagonal sheet visually inspected: FAIL shoulder/sleeve deformation (especially backhand/overhead), lunge shorts coverage and primitive court-shoe silhouette. Pose samples are NOT natural gameplay/animation acceptance; root elevation/grounded motion not validated.
- Final CPU baked ray rest coverage: Jersey213/800 misses; Shorts110/286; each shoe340/340. All poses fail coverage; exact metrics in `blender/attempt3/validation.json`. Shoe ray failure versus visible near-foot placement requires further rig/coordinate investigation; do not assert all sampled foot vertices are visibly exposed. Full closed-volume containment and same-side rig mapping audit incomplete.
- Runnable `python evidence/M2/quaternius/blender/check_gate.py` executed exit1; `gate-result.json` correctly rejects candidate. No OUTFIT_CHECK_PASS or integration eligibility. Three model candidates exhausted; no fourth trial. Whole exhibition remains blocked, old runtime regression not re-run because no runtime/Assets/test source changed in this scope.
- Proposed next workflow (NOT approved/executed): manual low-poly retopology/weight-paint by experienced character artist in existing Blender, reusing same base/pose fixtures, rather than fourth procedural generator trial. If cost cannot remainRp0, stop. Alternative asset acquisition requires separate approval/license audit; no automatic substitute.

## D-047 — APPROVED: isolated Blender outfit repair, 2026-11-04
- Owner explicitly permits official free Blender installation for one original badminton outfit: jersey, shorts, court shoes. No cost, other dependency, scope expansion or paid substitute.
- Back up before edits; preserve gameplay and old assets. Repair seams/fit/coverage/weights/deformation; agent inspects lunge, overhead, jump, backhand, dive-save poses. Maximum3 automatic fixes per same defect; stop and propose asset/workflow alternative if approach fails.
- No failed visual integration; no final import, final APK or phone action under this repair approval. Relevant gates and further required permissions remain mandatory. Deliver pose evidence and mesh/rig validation plus integration eligibility, not gameplay acceptance.
- Official license https://www.blender.org/about/license/ confirms free use and commercial artwork rights. Installation/source/hash and QA results recorded under evidence/M2/quaternius/blender/; approval does not mean validation PASS.

# Quaternius → first exhibition review — consolidated execution plan

Status: EXECUTION AUTHORIZED by owner instruction 2026-11-04: "kerjakan plan .hermes/plans/quaternius-exhibition-review.md sampai selesai". Owner chose Quaternius + original simple sportswear. Acquisition/audit and isolated outfit preview started; integration not yet authorized by passed visual gate. Scope: men's singles exhibition; no career, multiplayer, paid assets, new dependencies or push.


## Cara memakai dokumen tunggal ini
- Ini sumber plan utama: audit aset → repair outfit → integrasi → gameplay/presentation QA → regression → APK → device → review owner.
- Detail repair lengkap tertanam di bawah sebagai **detail langkah 2**, bukan task terpisah. Ikuti langkah 1–9 pada Ordered work; saat langkah2, jalankan seluruh subtask repair1–10, kemudian kembali langsung ke langkah3.
- Batas “runtime unchanged”, “jangan edit Resources”, dan “tanpa integrasi” dalam detail repair berlaku **hanya selama langkah2**. Langkah3 mengizinkan perubahan visual runtime setelah repair lolos; langkah4 mengizinkan kalibrasi sesuai D-045. Tidak ada konflik scope global.
- Permintaan penggabungan ini hanya mengubah dokumen. Instruksi owner terbaru mengizinkan eksekusi pendekatan repair gabungan; bukan izin melanjutkan generator tabung lama. Riwayat gagal dipertahankan.
- File repair bertanggal menjadi penunjuk arsip saja; agent cukup menerima dokumen ini. Tidak perlu membaca plan repair kedua.

## Finish line
Owner reviews one whole playable exhibition match after agent technical QA. Build success alone is not acceptance. Device absence or visual/audio failure remains an explicit blocker.

## Ordered work
1. Acquire/audit free Quaternius Standard candidate outside gameplay integration: preserve source URL, downloaded license, exact free model availability, file hashes; measure triangles, bones, material slots, textures and file sizes. Published ~13k triangles is a pack average, not a measured final player. Stop/report if free candidate or licensing fails. No automatic paid substitute.
2. Create original simple badminton jersey, shorts and indoor shoes on selected base; existing Casual Set00 is visual reference only, not copied mesh/texture or shipped dependency. Reuse geometry/materials across two athletes, distinct readable colors, no brands. Capture front/back/side preview before gameplay integration; verify final counts and deformation. Do not promise a size budget before measurement.
3. Integrate visual onto gameplay rig without changing scoring, hit timing, reach, movement or shuttle physics. Check anatomical facing for both ends, hand-to-racket grip, overhead/forehand/backhand contact, bent knees, foot placement, jumps/landings, lunge and recovery. Container transform.forward is not anatomical-facing proof. Remove Casual runtime dependency only after replacement passes; preserve source/reference files.
4. Close grade/stamina evidence and docs: Timing + Contact 50:50, thresholds 85/65/40; document actual implementation. Confirm 30s natural moving rally, fatigue influence without forced miss, walking drain 0, recovery/low-energy behavior and equal conditions. Existing focused energy 70/100 is a baseline, not complete balance acceptance.
5. Close playable presentation: release tester/debug UI absent, clear touch hints, Fixed joystick default with Floating option retained, serve charge/cancel and multi-touch correct. Verify shuttle/trail visibility through high flights. Review continuous motion and in-game audio; existing four CC0 racket recordings are baseline, not new synthetic replacements. Check valid-hit audio, miss silence and duplicate events. No unrelated UI redesign.
6. Run integrated regression: PlayMode/EditMode, full natural exhibition to results, score/serve/end-swap/pause/resume/save/challenge with replay on/off and exactly-once continuation. Capture actual continuous gameplay, inspect audiovisual defects; passing assertions alone cannot establish feel.
7. Build non-development APK after final runtime edits; preserve application ID/signing compatibility. Record actual APK bytes/SHA256 and input manifest. Inspect release artifact, not only build log. Measure final character and build payload; compare to old candidate only on like-for-like runtime metrics.
8. Install/update connected Android device without uninstall/clear data. Test launch, existing save/settings, safe area, simultaneous movement+shot+jump, frame timing/FPS and sustained match stability. D-045 permits install/launch; device connection still required. If unavailable, report BLOCKED_DEVICE and do not call ready.
9. Update TESTING, ERROR_LOG, WORKFLOW, decision register/current policies and handoff with exact evidence and remaining limitations. Deliver APK and short review guide. Owner tests whole exhibition experience, not individual technical cases.

## Gates
- Asset: free downloadable candidate + compatible license verified from files.
- Visual: useful close previews and continuous actual gameplay inspected; no unresolved severe deform/grip/facing issues.
- Gameplay: final-source regression passes and natural match reaches results correctly.
- Mobile: actual device evidence, not Editor-only claim; no data deletion.
- Release: source-bound final artifact; no tester panel; known issues disclosed.

## Execution checkpoint — BLOCKED_VISUAL_COVERAGE / BLOCKED_DEVICE (latest repair execution)
- Owner instruction this session authorizes consolidated repair approach. Backup `.backups/quaternius_repair_20261104_132305/`.
- Three new modeling candidates executed; candidate2 rejected in memory for nonManifold37/degenerate60. Candidate3 structural pass but actual front/coverage FAIL: Jersey640/912, Shorts286/314, Shoes504/824 misses. No further modeling attempt. Full evidence `evidence/M2/quaternius/repair/checkpoint.json` and docs QUATERNIUS-REPAIR.
- Independent regression completed after final source edits: EditMode10/10, PlayMode98/98; no skipped/failed/inconclusive. Gameplay/runtime/source license hashes identical baseline. adb devices empty.
- Step2 NOT PASSED; step3 integration prohibited by failed gate. Steps4–8 unfinished; regression does not establish natural motion/audio/mobile acceptance. No final APK. Whole plan remains incomplete.

## Historical checkpoint — BLOCKED_VISUAL / BLOCKED_DEVICE
- Step1 acquisition/license and static source+Unity audit executed; evidence `evidence/M2/quaternius/source-audit.json` and `unity-audit.txt`.
- Step2 isolated outfit failed visual gate after3 automatic fixes. Final `outfit-fix3-front.png`: open shoulders, central shorts gap, exposed toes. No fourth attempt; integration not started. Details `docs/ERROR_LOG.md` QUATERNIUS-046.
- Steps3–8 unfinished. adb device list empty. No final APK or readiness claim. Step9 provenance/policy/handoff updated for actual blocked checkpoint only.

## Current evidence (not final completion)
- share-final-PlayMode.xml: 98/98; share-final-EditMode.xml: 10/10.
- Existing share APK contains old candidate; not final Quaternius artifact.
- No attached Android device at last adb check.
- Trail, whole audiovisual review and NPC replacement remain unfinished.

Rollback: retain existing owner assets, source and dirty changes; backup before import/runtime changes. No git push. Stop a failing fix after 3 automatic attempts and report exact blocker.

---

# Detail langkah 2 — outfit repair

## Goal
Ganti generator outfit tabung terpisah dengan jersey tersambung, shorts bercabang tersambung, dan sepatu tertutup; buktikan fit serta deformasi pada prefab audit sebelum meneruskan plan exhibition.

## Current context / assumptions
- Workspace Windows: `C:/Users/Dewangga Ramadhan H/Documents/Portofolio/JustSmash`.
- Unity executable: `C:/Program Files/Unity/Hub/Editor/6000.3.18f1/Editor/Unity.exe`.
- Shell terminal adalah Bash, bukan PowerShell. Native programs memakai path `C:/...`.
- Baca `docs/AI_RULES.md`, `docs/05_DECISION_REGISTER.md` D-045/D-046, `docs/ERROR_LOG.md` QUATERNIUS-046, dan `.hermes/plans/quaternius-exhibition-review.md` sebelum eksekusi.
- Owner telah memberi instruksi eksekusi plan gabungan. Pendekatan mesh baru diizinkan; software/dependency baru tetap tidak diizinkan.
- Base resmi tersedia: `Assets/QuaterniusAudit/Superhero_Male_FullBody.fbx`; CC0 di `Assets/QuaterniusAudit/License_Standard.txt`. Tidak perlu download ulang. Source/hash: `evidence/M2/quaternius/source-audit.json`.
- Free Standard yang benar: Superhero male/female; Regular/Teen tidak ada dalam ZIP ini.
- FBX pria: 14.318 triangles, 8.477 vertices, 65 bones per skinned renderer, 3 renderer/material slots. Jangan samakan vertex count FBX dengan glTF 8.483.
- Mesh body bernama `SuperHero_Male`. Anatomical front menghadap **-Z**. Kaki kiri anatomis di +X. `pelvis` world kira-kira (0,.95,.04); shoulder +/- .21,1.46,.07; hand +/- .71,1.46,.07; ankle +/- .11,.09,.09; toes menuju -Z.
- Generator gagal: `Assets/Editor/QuaterniusOutfit.cs`; audit/capture: `Assets/Editor/QuaterniusAudit.cs`. Output prefab `Assets/QuaterniusAudit/AthletePreview.prefab` bukan runtime athlete.
- Runtime tetap `Assets/Resources/AthleteNPC.prefab` dan `Assets/Scripts/AthleteVisual.cs`, masih kandidat Casual. **Jangan edit keduanya dalam repair ini.**
- Initial + 3 automatic fixes gagal visual. BakeMesh displacement <0.000002m: rest bind bukan bukti fit, dan bukan bukti deformasi. Jangan lanjut koreksi radius tabung lama sebagai percobaan keempat.
- Preview final gagal: `evidence/M2/quaternius/outfit-fix3-front.png`. Bahu/sleeve tidak tersambung, waist/crotch terbuka, toes terbuka. File lama `outfit-back.png` menunjukkan anatomical front karena label kamera terbalik.
- Android kosong pada pengecekan terakhir. Device bukan prerequisite perbaikan prefab ini; tetap blocker exhibition final.

## Architecture / proposed approach
Tetap Unity Editor C#, tanpa Blender, package mesh, asset berbayar atau salinan Casual. Buat tiga mesh orisinal bertopologi eksplisit: jersey memakai torso dengan dua lubang armhole yang benar-benar dijahit ke sleeve, shorts memakai satu waistband yang bercabang menjadi dua hem melalui crotch, sepatu memakai loft tertutup dengan toe/heel/sole cap. Fit di rest space, transfer skin weight dari body base berlisensi, lalu validasi topology, containment dan pose; tidak menambal celah dengan material double-sided, kulit tersembunyi, sphere, atau sepatu raksasa.

**Baseline teknis outfit untuk repair (bukan klaim pilihan warna owner):** jersey biru polos `(.04,.34,.72)`, shorts gelap `(.025,.035,.065)`, shoes off-white `(.85,.88,.91)`, roughness saat ini dipertahankan. Short sleeve, neckline bulat sederhana, hem di sekitar y=.93; shorts hem sekitar y=.66. Tanpa logo, stripes, aksesoris, redesign wajah, animasi atau body proportions. Tidak ada budget payload yang dijanjikan sebelum pengukuran.

## Scope / file ownership — khusus langkah 2
Selama langkah 2, boleh edit hanya:
- `Assets/Editor/QuaterniusOutfit.cs`
- `Assets/Editor/QuaterniusAudit.cs`
- Baru: `Assets/Editor/QuaterniusOutfitChecks.cs` — self-check Editor tanpa dependency baru.
- Generated mesh/material/prefab di `Assets/QuaterniusAudit/` berikut `.meta`.
- Evidence baru `evidence/M2/quaternius/repair/`.
- Docs existing untuk hasil: `docs/TESTING.md`, `docs/ERROR_LOG.md`, `docs/WORKFLOW.md`, `docs/SESSION_HANDOFF.md`, `ASSET_REGISTER.csv`, status plan lama.
Tidak boleh mengubah FBX/license asli, scoring, stamina, physics, hit timing/reach, gameplay rig, runtime Resources, build/signing, atau data HP.

## Verification commands
Jalankan dari workspace root. Jangan menjalankan dua Unity pada project sama. Jika Editor milik owner terbuka, minta owner save/tutup; jangan kill.

```bash
UNITY='C:/Program Files/Unity/Hub/Editor/6000.3.18f1/Editor/Unity.exe'
PROJECT='C:/Users/Dewangga Ramadhan H/Documents/Portofolio/JustSmash'
"$UNITY" -batchmode -quit -projectPath "$PROJECT" -executeMethod QuaterniusOutfitChecks.Run -logFile 'evidence/M2/quaternius/repair/check-red.log'
```
RED expected: nonzero exit dan `OUTFIT_CHECK_FAILED` dengan area/kriteria; compile error bukan RED yang sah. GREEN expected: exit0, `OUTFIT_CHECK_PASS`, JSON check results tertulis; bukan visual acceptance.

```bash
"$UNITY" -batchmode -quit -projectPath "$PROJECT" -executeMethod QuaterniusOutfit.Run -logFile 'evidence/M2/quaternius/repair/generate.log'
```
Expected: exit0; mesh/material/prefab dan preview baru. Exit0 tidak cukup tanpa inspeksi PNG.

## Ordered implementation tasks
Setiap bullet pekerjaan fokus 2–5 menit; satu grouped stage boleh membutuhkan beberapa bullet. Tidak mengaku estimasi ini sebagai total waktu selesai. Simpan RED/GREEN unik, jangan overwrite run gagal.

### 1. Checkpoint dan bekukan baseline
1. `git status --short`; audit dirty/untracked. Backup file scope + generated audit assets ke `.backups/quaternius_repair_<timestamp>/`, tanpa menghapus pekerjaan lama. Catat path.
2. Copy tiga preview terakhir, `outfit-counts.txt`, `bind-check.txt` dan generator sebelum edit ke `evidence/M2/quaternius/repair/baseline/`. Jangan mengulang generator destruktif hanya untuk baseline.
3. Catat SHA256 `Assets/Scripts/*.cs`, `Assets/Resources/AthleteNPC.prefab`, original FBX dan license dalam `baseline-inputs.json`. Setelah repair wajib identik.

### 2. Perbaiki harness capture dahulu — bukan modeling trial
1. Di `Assets/Editor/QuaterniusAudit.cs`, ubah hanya mapping arah kamera berikut. Front=-Z, back=+Z, side=+X. Kode pengganti lengkap untuk baris direction:
```csharp
var direction = view == "front" ? Vector3.back :
    view == "back" ? Vector3.forward : Vector3.right;
```
2. Jangan rename bukti lama seolah dahulu benar. Output repair masuk `evidence/M2/quaternius/repair/`; parameterize destination di `Capture` dengan optional argument `string outputDirectory = "evidence/M2/quaternius"`. Pakai `Directory.CreateDirectory(outputDirectory)` dan destination itu di `File.WriteAllBytes`.
3. Tambah view diagonal depan kiri/kanan dengan direction `new Vector3(1,0,-1).normalized` dan `new Vector3(-1,0,-1).normalized`; filenames eksplisit. Tetap simpan front/back/side.
4. Framing memakai bounds renderer dan **kedua** dimensi: orthographicSize `Mathf.Max(bounds.size.y*.6f,bounds.size.x*.6f/camera.aspect)`. Bukti lama cropping tangan tidak diterima sebagai whole-model view. Tambah crop camera chest, waist/crotch, feet; render 800x1000 atau lebih, tidak screenshot hasil mockup.
5. Generate base-only audit ke folder repair; inspeksi front menampilkan face/toes, back menampilkan punggung. Bila gagal label/framing, benahi harness sebelum menilai outfit.

### 3. Tulis checks RED yang memeriksa defect nyata
`Assets/Editor/QuaterniusOutfitChecks.cs` harus public static class dengan `Run()` untuk batchmode. Gunakan `AssetDatabase.LoadAssetAtPath<GameObject>("Assets/QuaterniusAudit/AthletePreview.prefab")`, instantiate temporary, finally DestroyImmediate. Bila check gagal, tulis JSON hasil parsial lalu throw `System.Exception("OUTFIT_CHECK_FAILED: " + criterion)`; sukses `Debug.Log("OUTFIT_CHECK_PASS")`. Jangan gunakan log warning sebagai sukses.

Implement checks berikut satu per satu, run RED setelah setiap kelompok:
1. Mesh tidak null; triangle count kelipatan3; semua indices valid; semua vertex/normal/weight finite; sum4weights dalam .001 dari1; bone index di range; no zero-area triangle (cross magnitude >1e-10 world-space). Tolak material two-sided atau transparent workaround.
2. Topology pakai **welded positions untuk adjacency** toleransi0.00001m; normal/UV seam boleh duplicate render vertices. Jersey tepat1 connected component dan boundary loops tepat4: neck, waist, left sleeve cuff, right sleeve cuff. Shorts tepat1 component dan boundary loops tepat3: waist, left/right leg hem. Dua sepatu tepat2 components, tiap component **tanpa boundary edge**. Setiap undirected edge adjacency2 kecuali approved garment boundary adjacency1; adjacency>2 fail. Komponen lama gagal stage ini; buktikan RED nyata.
3. Bind check di mesh world space: BakeMesh rest output vs intended rest vertices max displacement <=.001m; test baseline ini boleh PASS, jangan dibuat RED palsu.
4. Coverage via **CPU triangle ray intersections pada baked meshes**, bukan shader atau camera depth. Cast dari body surface outward di ROI pakaian; setiap ray harus intersect garment dalam .002–.04m. ROI torso y1.00–1.42 kecuali lubang neck; shoulder points di sekitar upperarm joint termasuk top surface; shorts y.72–.96 termasuk pusat pelvis depan/belakang; shoes mencakup seluruh foot vertices y<=.095. Jangan pilih hanya3 titik yang kebetulan tertutup. Laporan jumlah sampel, misses, max distance, coordinates failures per ROI. ROI geometric predicates ditulis eksplisit dalam check, disimpan di JSON; tidak diubah untuk menyembunyikan kegagalan.
5. Cek penutupan shoe lewat topology plus containment seluruh foot vertices di closed shoe volume, bukan ankle ring saja. Test shoes harus gagal pada toe baseline.

**Catatan implementasi:** tidak perlu NUnit assembly baru. Satu runnable Editor self-check cukup. Algoritma adjacency/BFS/ray-triangle adalah implementasi standar terbatas di file checks; jangan membuat framework general mesh processing. Copy-paste commands di atas merupakan kontrak RED/GREEN; simpan JSON berisi assertion aktual, bukan hasil expected yang dikarang.

### 4. Generator safety sebelum mesh baru
1. `Assets/Editor/QuaterniusOutfit.cs`: stop `AssetDatabase.DeleteAsset` untuk semua mesh/material setiap run. Preserve GUID dengan load existing asset lalu `EditorUtility.CopySerialized` dari newly built mesh/material; `CreateAsset` hanya kalau path belum ada. Tidak menghapus FBX, material source atau license.
2. Bangun dan validasi semua candidate meshes **di memory** dahulu. Jika check structural gagal, throw sebelum overwrite prefab/assets. Clear static lists di awal run dan finally agar retry tidak terkontaminasi state.
3. `SaveGarment` bindposes tetap `bone.worldToLocalMatrix * renderer.localToWorldMatrix`. Nearest-body skin weight boleh dipertahankan sebagai baseline, bukan dinyatakan lulus motion otomatis. Mesh tangent/UV tidak diperlukan selama material polos; jangan tambah texture.
4. Jalankan generator dua kali setelah meshes valid; GUID asset/meta identik antar run; hasil count/hash deterministik. Test ini ditunda sampai topology lolos, bukan memaksa overwrite baseline gagal.

### 5. Jersey — ganti tabung torso+sleeve terpisah, jangan adjust radius lama
1. Hapus pemanggilan tiga `RingTube` untuk Jersey. Ganti dengan connected topology dalam generator yang sama, tanpa class/package lain. Buat front/back torso panels dengan seam sides, waist hem, shoulder slopes dan neck cutout; setiap armhole adalah boundary ring yang dijahit ke sleeve root dengan **shared/welded vertices**, bukan overlap dua silinder.
2. Layout torso punya waist/chest/shoulder rows. Neck radius sekitar .085m, hem y=.93. Pertahankan chest clearance .01–.025m dari body; ukur against baked base untuk setiap ring vertex, bukan memasukkan dada ke body. Bahu mengikuti joint y1.46; jangan menempatkan sleeve top di y1.60 akibat radius berlebihan.
3. Sleeve cuff di sekitar |x|=.37, center y1.455,z.065; cuff radius ditentukan dari body slice +.015m. Hubungkan loop armhole ke cuff melalui intermediate loop, triangulate quads konsisten outward. **Di shoulder/underarm tidak boleh boundary tambahan.** Jangan cap armhole; itu menutup sleeve, bukan menjahit.
4. Run structural+shoulder coverage RED/GREEN. Capture front/back/diagonal crop bahu. Jika tidak1 component/4 approved loops atau skin tampak pada roof shoulder, stage gagal. Jangan memperbesar radius terus sebagai pengganti topology.

### 6. Shorts — satu waist, dua kaki dengan crotch bridge
1. Hapus dua pemanggilan `RingTube` shorts. Buat satu closed circumference waistband y sekitar1.00 yang membungkus pelvis; turunkan ke hip row y.88. Bifurcate ke dua thigh rings dan leg hems y.66; front/back center vertices di crotch harus tersambung lewat crotch strip. Tidak memakai dua tabung kaki overlap tanpa seat/pelvis mesh.
2. Waist jersey/shorts overlap vertikal minimal .03m; jersey tidak muncul sebagai celah di antara dua waistband terpisah. Shorts bagian bawah tetap dua lubang terpisah; jangan menjahit di antara dua hem sehingga menjadi rok.
3. Skin weights crotch/waist blend pelvis+thigh memakai body reference; hindari nearest vertex dari kaki berlawanan pada center seam. Body sampling nearest dibatasi sisi untuk vertices |x|>.02; seam center memakai pelvis-dominant weights dan normalized sum.
4. Run1 component/3 boundary loops + front/back pelvis coverage. Capture waist/crotch front/back dan diagonal. Tidak lulus bila skin gap, dark backface hilang, seam membuka saat lunge.

### 7. Shoes — closed shells, bukan ankle funnels
1. Ganti `RingTube` shoes dengan closed loft tiap kaki. Long axis **Z**, toe arah -Z. Gunakan cross-section di heel, midfoot, forefoot dan toe; cap heel, toe serta sole. Sole plane ikuti minimum y body foot, bukan center ring y=.012 yang membuat sisi depan kosong.
2. Ukur bounds foot body vertices tiap sisi; clearance upper .008–.018m, toe .01–.02m; sole thickness .015–.025m. Tidak boleh x/z extents lebih1.15 kali corresponding foot bound sebagai cara menutupi clipping. Catat ratio aktual.
3. Seluruh shoe memakai foot bone sisi yang sama untuk shell utama; ankle collar boleh blend calf. Jangan nearest-bind toe ke calf atau kaki lawan. Dua sepatu berbagi mesh/material bila transform/reflection+winding benar; kalau mirror menambah risiko, dua component satu mesh masih acceptable. Catat pilihan dan count, jangan framework instancing.
4. Run closed topology/containment tests, capture front/side/sole/diagonal. Seluruh toes tertutup dan silhouette sepatu indoor, bukan box/cup. Sole tidak menembus ground saat rest; jangan mengubah base athlete untuk mengompensasi.

### 8. Deformation gate, terpisah dari rest check
1. Di `Assets/Editor/QuaterniusOutfitChecks.cs`, snapshot local transforms semua bones dan restore di finally setiap pose. Definisikan named pose fixtures dengan local relative rotations (no bone translation/scaling): rest; arms lowered45°; right overhead90°; elbow flex90°; both knee flex60°; split lunge hip35°/knee70°. Pilih sign rotasi setelah memeriksa local bone axes sekali; simpan matrix/rotation yang benar di JSON fixture dan pakai yang sama untuk semua runs, jangan ubah pose untuk meloloskan outfit.
2. Capture tiap fixture front/side/diagonal dengan baked pose. Check finite bounds/weights, garment triangle area nonzero, same-side bone indices dan coverage samples ditransform melalui bobot body. Fail jika clothing penetrates sampled body atau gap shoulder/crotch/toe muncul. Numerical tolerance .002m untuk ray origin bukan izin visual penetrasi2mm terlihat.
3. Inspeksi seluruh pose PNG: tidak ada spike/inside-out triangle, shoulder gap, exposed pelvis/toes, oversized shoe, self-intersection berat. Pose samples **bukan natural gameplay video**. Laporan harus menyebut pose sampling, bukan continuous match.
4. Bila nearest weight menimbulkan kink: koreksi weight hanya area gagal, interpolasi nearest3 body vertices dengan inverse-distance yang dibatasi sisi/body region; renormalize, paling banyak4 bone influences. Jangan mengganti whole rig atau memperpanjang bone. Satu defect sama maksimum3 fix di pendekatan baru; lalu stop dengan failures aktual.

### 9. Full regression tanpa runtime integration
```bash
"$UNITY" -batchmode -projectPath "$PROJECT" -runTests -testPlatform EditMode -testResults 'evidence/M2/quaternius/repair/final-EditMode.xml' -logFile 'evidence/M2/quaternius/repair/final-EditMode.log'
"$UNITY" -batchmode -projectPath "$PROJECT" -runTests -testPlatform PlayMode -testResults 'evidence/M2/quaternius/repair/final-PlayMode.xml' -logFile 'evidence/M2/quaternius/repair/final-PlayMode.log'
```
Run sequential, background+notify untuk tiap Unity long process. Jangan `-quit` pada test runner jika memotong suite. Parse XML `test-run` attributes dengan Python `xml.etree.ElementTree`; failed/skipped/inconclusive harus0, passed/total dicatat aktual. Baseline98Play+10Edit hanya referensi; jangan hardcode bahwa suite baru pasti sama.

### 10. Close repair dan lanjut plan exhibition tanpa mencampur gate
1. SHA runtime/scripts/FBX/license harus identik baseline. Jika beda tanpa scope, stop dan jelaskan; jangan restore membabi-buta user changes.
2. Ukur final per mesh triangles/vertices/bones/material slots/texture assets/files; aggregate dengan Python. Ini counts prefab audit, **bukan final APK payload atau performance**.
3. Docs: `ERROR_LOG` tutup hanya defect outfit yang terbukti (`CLOSED_LOCAL_VERIFIED` dengan structural+coverage+pose+visual evidence); pertahankan history3 failed attempts lama. `TESTING` pisahkan rest/topology/deform/visual/gameplay/device; `WORKFLOW` dan `SESSION_HANDOFF` sebut runtime belum diganti.
4. `ASSET_REGISTER.csv`: Quaternius tetap licensed derivative base; outfit original generated geometry, no copied Casual. Source/license/hash tidak diubah.
5. Update `.hermes/plans/quaternius-exhibition-review.md`: step2 passed hanya bila seluruh repair gates lolos. Lanjut step3 integrasi rig sebagai pekerjaan berikutnya di bawah approval eksekusi plan utama. Jangan klaim exhibition ready, build final atau phone tests dari repair.
6. Commit lokal per grouped GREEN stage hanya setelah user dirt dipisahkan dan secret/large binary audit. **Jangan `git add .`**, jangan stage source ZIP/evidence/build besar, jangan push. Jika staging memuat user changes yang tidak bisa dipisahkan aman, skip commit dan laporkan; checkpoint tetap ada. Recommended commits: harness+RED checks; jersey GREEN; shorts/shoes GREEN; pose+final evidence/docs. RED checkpoint disimpan file/log, bukan wajib commit branch rusak.

## Acceptance / deliverable
- Prefab audit berjersey+shorts+sepatu, semua mesh original clothing dan material warna tetap.
- Connected topology pass, spatial coverage pass, bind+pose checks pass, inspected useful front/back/side/diagonal+pose renders pass.
- No file-ID churn pada rerun; source+runtime unchanged; full suites actual PASS; no new dependency/cost/push.
- Evidence unik di `evidence/M2/quaternius/repair/` dan concise report: changed files, executed counts, exact unresolved gates.
- Setelah repair, main plan masih membutuhkan integration/grip/facing/natural motion/audio/stamina/full-match/release/device. Repair bukan pengganti finish line.

## Risks / tradeoffs / open questions
- Unity custom topology lebih banyak pekerjaan daripada tubes, tetapi menghindari instal software/package baru. Jangan menjanjikan modeling ini selesai5 menit.
- Manual joined topology membutuhkan authored vertex/index layout; jangan menutup lack of skill dengan oversized overlap atau skin material recolor. Jika implementer tidak mampu memenuhi layout/coverage, report blocker tepat sebelum lebih banyak trial.
- Body transfer skin weights tidak otomatis memberi cloth simulation; tidak ada cloth physics dalam scope. Plain sportswear saja.
- Closed geometry bisa menembus body saat pose ekstrem; coverage sampling+visual wajib, numerical rest PASS tidak cukup.
- Owner belum menyetujui Blender atau external modeling tools. Bila pendekatan Unity tetap gagal3 fixes, ajukan **satu** perubahan alat/proses dengan dampak konkret; jangan install diam-diam.
- Android harus tersambung untuk main plan final; tidak meminta owner melakukan technical garment QA. Owner review tetap whole playable exhibition.
