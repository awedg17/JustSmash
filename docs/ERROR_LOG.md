## Kontak/pose/crowd release polish — 2026-11-25

- CLOSED_LOCAL_VERIFIED: buffer menebak tinggi minimum overhead 1.45 m untuk semua stroke dan release menebak jarak horizontal. Sekarang alasan berasal dari keputusan stroke-specific yang sama dengan `CanReach`. 24 kombinasi dua ujung dan buffer sukses sebelum expiry/notice priority PASS; kontak sah tetap tick sama, miss tanpa biaya energi/audio. Run focused29/29, sumber/data tetap.
- Patch siku/crowd lampiran diterapkan: ready wrist di depan, siku turun; enam mesh statis/satu material dengan lifecycle cleanup. Tes pose/motion/arena PASS; Home350s dan still render kedua ujung diperiksa. Kualitas motion/audio masih keputusan pemilik, bukan ditutup oleh tes struktur.
- Kandidat smash terpisah: grid .40–1.00/.02, vy<=0, margin .08, nearest old duration; enam fixture Unity .68/none/.66 kedua ujung. Tes6/6 setelah pembanding power memakai jam charge aktual; gagal pertama karena float-clock fixture, bukan product rollback. Tanpa kandidat mempertahankan velocity lama persis. Kandidat sudah keluar dari source aktif, hanya patch untuk persetujuan.
- Capture: PCM/video source drift56–143ms pada beberapa klip, baseline metadata `lastWall:R`. Raw/failed gate dipertahankan; parser tidak mencipta waktu. MP4 berisi audio game, tidak ditrim/retime/pad. Ini keterbatasan rekaman, bukan bug audio game yang terbukti. Detail [[Release-Polish]]. Save File.Replace lama tetap OPEN.

## D-063 — T4e foot reconstruction + snapshot capacity (2026-11-23 19:38)

Foot history defect CLOSED_LOCAL_VERIFIED for tested routes:216cases (still/stride/air,3strokes,formats/ends/actors,ApplyFrame/RestoreMatch),maxfoot error baseline0.370000362m→0. New optional plant state retains planted/airborne,left/right world targets; replay reconstruct avoids applying plant movement twice and consumes no footfall.48corrupt live/buffer foot values reject before mutation;v2/v3 missing fields accepted. ReplayTick/SkipReplay/load from replay56checksPASS. Whole T4e remains pending natural-motion/pause/accepted-character gates.

Current source guard26/26PASS in wave4-pose-capacity-green1,including snapshot216cases,48invalidcases,56replaychecks,ClosureTests/momentum. Last whole-suite wave4-pose-edit1 20/20 and wave4-pose-play1 248/250;only two knownlunge failures,source/data unchanged. This whole suite PRECEDES one-line snapshot size fix and latest test-only edits; no claim250/250 or final-source wholePASS.

Capacity defect reproduced on OLD runtime too:1500frames create1,469,686characters(singles)/1,666,635(doubles) beyond prior1,000,000 restore limit. New plant fields2,505,376/2,933,479characters. Minimal bounded8,000,000character guard now admits current1500-frame fixtures;unchanged1500frame bound and all schema checks retained. Inputs8,000,001characters and1501frames explicitly reject without mutation. No disk-save/autosave reliability claim;intermittent File.Replace remains open. Snapshot format staysv3 optional fields.

Same-camera screenshot correction: immediate manual renders were stale evidence(2changedpixels despite.22m bone drift); yielding real frame/render boundary yielded24,164changedpixels expected-vs-restored baseline,0after. CameraJSON exact;visible leg/foot reconfiguration reviewed,not motion acceptance. `wave4-pose-roundtrip-comparison/restore-rendered-before-after.png`. New source frozen; candidate restore byte-exact before capacity fix. Raw old images retained as UNACCEPTED.

Replay/challenge capture wave4-pose-lifecycle1:2/2UnityPASS,12scenarios,1678frames55.891s,live25.33ms/packet17.26msPASS,PCM55.9786667s(duration87.67msFAIL),decodePASS. No mux/trim/retime. Initial nearest replay-active still hit Skip frame;interval-midpoint replacement shows LEWATI. No continuous-motion/listening acceptance. Corrected-camera lowstroke older pair parent audited:695/696frames,22.575/22.671s;beforePCM59.67msFAIL,after49msPASS,live13.22/17.29ms andpacket13.26/29.19msPASS. Both precede foot-state runtime;not current-source videos.

## D-063 — T4e foot-state reconstruction subset (2026-11-23)

Actual CaptureFrame/ApplyFrame and CaptureMatch/RestoreMatch reproduced hidden foot-plant history: initial72cases feet drift max0.268378347m, racket/chest0. Candidate adds optional recorded left/right world-space plants plus planted/airborne flags, restores without applying plant MoveTowards twice, clears replay foot-contact flags, validates finite/bounded vectors before match mutation. Four runtime files: AthleteVisual.cs, CourtReplay.cs, CourtPrototype.cs, CourtDoubles.cs. Live Present/physics/contact/stamina unchanged.

`wave4-pose-roundtrip-green2`5/5PASS: original72cases +48corrupt live/buffer/actor/side/value combinations reject without mutation; missing-foot-state v2/v3 accepted; momentum tests pass. `wave4-pose-roundtrip-motion1`2/2PASS expands to216cases across still/stride/air,formats,ends,3strokes,actors,ApplyFrame/RestoreMatch. Earlier focused26tests24PASS/two knownlungeFAIL; no lunge repair. Whole current suite running under wave4-pose-edit1/wave4-pose-play1; not yet claimed PASS.

Same-camera baseline re-executed as wave4-pose-roundtrip-beforevisual1 with candidate saved to .backups/wave4-pose-candidate1,then four runtime files restored byte-exact. Numeric RED retained; screenshot capture currently UNACCEPTED: .22m bone difference yielded only2pixels. Vision shows feet visible but overlapping heads; immediate manual camera render may use stale skinned data, cause unproven. Do not cite image as visual proof. Natural/replay lifecycle/fullmotion/device remain open. Read-only media auditor stop requested at177s vs120s budget (within5min check); partial result not yet consumed, no worker canonical edits authorized.

## D-063 — idle visible-rig frozen CLOSED_LOCAL_VERIFIED (2026-11-23)

Ready chest stayed frozen (all8actor/end cases0deg) because visible legacy pose used distance phase; existing breath moved hidden proxy only. Minimal ready-only chest flex uses recorded elapsed; GREEN0.799169deg max offset,feet0m drift. `wave4-idle-green1`63/63,Edit20/20,fullPlay244/246 with exactly two known lunge failures. Samecamera controlled originals4.215s each pass strict50ms/decode; not whole natural animation acceptance. `wave4-idle-checkpoint1.json` binds source. First idle capture used gameplay Update, which moved camera; rejected as fixed-camera comparison. Corrected fixture disables gameplay component, calls actual AnimateAthlete, labels noHUD/noaudio. Final pair only runtime difference AthleteVisual.cs.

## D-063 — controlled marker clock remains blocked (2026-11-23)

`wave3-marker-after-font1`:1/1logic PASS,4scenarios,1424frames/24.475s,source/data unchanged. MediaFAIL:91.59464ms live-clock,84.61324ms video/sample. Biggest mismatch first two Home frames,not rally. One test-only stage probe: second frame deltaTime19.99999ms vs unscaledDeltaTime60.8356ms,encoder write max4.54ms. Runtime unchanged; exact origin of startup clock split unproven. Probe1 had int-format R error,probe2 fixed1/1;instrumentation archived and original harness restored SHA-exact to font checkpoint. Diagnosis `wave3-marker-startup-diagnosis.json`; no blind rerun/trim/retime/gate change. T6d remains partial. Next T7f independent: existing unedited1x full-match harness on current runtime; old fullmatches are no longer runtime-identical.

## D-063 — cold HUD hitch CLOSED_LOCAL_VERIFIED (2026-11-23)

Native profiler isolated first-use font work at match start: glyph adjustment61.65ms/GUI74.72ms. Native CalcSize warmup during style init removed that observed marker spike(0ms/GUI4.31ms) and cold-HUD91.31ms ->15.93ms against unchanged50ms gate. One runtime candidate; final pair corrects capture fixtures,not retries of the fix. Earlier camera-mismatched and disabled-OnGUI stills retained but rejected as HUD-pair evidence. Final camera-equal screenshot differs12pixels only. Fresh singles/doubles excerpt media now passes; historical AV deficits/marker failure are not erased or declared explained. Fullmatch/listening and broader-motion gates remain.

## D-063 — wave3 integration checkpoint (2026-11-23, 16:03)

- Checkpoint `evidence/shareable/20261123/overhaul-005531/wave3-checkpoint1.json`: SHUTTLE_LIGHTING_SUBSET_VERIFIED_FULL_GATE_BLOCKED. Parent verified1159current source inputs,1348protected files SHA-exact,28artifact hashes. Six changed source paths from probe: CelCourt.shader, CourtPrototype.cs, CourtArena.cs, ReadabilityTests.cs, AiContactWaveTests.cs, LocalGateTests.cs. No live Unity/worker, no commit/push/device/APK.
- Full integration ran once after runtime change: Edit20/20,Play239/244,source/data unchanged. Five failures retained: two known lunge regressions; two audio tests inspected obsolete `sounds` cache instead of borrowed recording arrays; wall contrast regression from prior arena finishing (.210040003 >=.207395494). Not a shuttle lighting regression; same8background PNG hashes established no shared-scene lighting change.
- Small correction: wall RGB(.16,.22,.29) to(.15,.21,.28),grayscale.20004; gate unchanged. Audio tests now assert event delta/actual clip and collect measured output through MatchEnd, not cache membership. Former FullMatchAudioEventsAreAudible renamed FullMatchProducesAudioSamplesThroughMatchEnd so test name no longer claims listening. `wave3-integration-repair1`6/6PASS,source/data unchanged; arena/court/white-face checks plus profile. No second whole-suite rerun or244/244claim. Footwork tests remain canonical unresolved.
- Completed accelerated audio-match fixture176.2296game seconds,41human contacts,8799nonzero-output samples,7587samples above.001RMS,maxRMS.04498397,racket+score observed. Before correction already produced actual output (maxRMS.04406886) despite cache assertionFAIL. This verifies signal/event flow,not listening or1x fullmatch acceptance. Separate AI baseline24cases/7tests PASS covers8serves,8returns,8misses with repeat silence.
- PC Editor profile,1280x720,no recorder,two6second windows:30target181frames,median33.29277ms,p9534.19495ms,268023676UnityallocatedBytes;60target360frames,median16.66641ms,p9517.47894ms,280631781bytes;max368drawcalls,86659triangles. Editor-inclusive/no thermal/Android/device claim. These short windows do not certify sustained mobile performance.
- `wave3-comparison/shuttle-before-after.png`:8same-camera pairs,white-face262/362dark to0/362,outline retained. `wall-before-after.png`:same test/camera,subtle darkening confirmed. Natural excerpts from white-green1 have only CourtArena.cs runtime difference from current;do not call exact-final-runtime. Singles/doubles strictclock.093839695/.06081214seconds FAIL;keep originals,no retime/mux. No new whole task closed:character,lunge,natural tracking/audio listening/squeak,full1xmatches andAndroid gates remain. Historical storage lock remains even though this run's storage test passed.
- Next independent work: diagnose measured capture timing boundary before another long1x capture; no blind rerun. Latest peak occurs Setup→Playing singles frame30 and Point transition doubles frame94; adjacent wall/game samples are retained, cause not isolated. Lunge/character failure ceiling stays in force. Optional voice is not blocker. No new worker wave until supervision delivery path is verified.

## D-063 — wave3 shuttle lighting and AI audio (2026-11-23)

- Retained tiny runtime diff: CelCourt `_Unlit` defaults0; only shared shuttle material sets1. Outline/mesh/cork/root/camera/contact/physics unchanged. Native Unlit/Color diagnostic was not integrated because it lost outline. Corrected white-face mask isolates lighting from intended overlapping outlines; same corrected test RED262/362 dark pixels, GREEN0/362. Initial265→15failure remains archived, not hidden. `wave3-shuttle-white-green2`30/30focused PASS; runtime source/data unchanged. Eight same-camera before/after poses; all eight shuttle-hidden background PNG hashes identical. Visible silhouettes clearer; distant/body-occluded and end-on dot cases still OPEN. No X-ray/scale/occlusion cheat.
- `wave3-ai-audio-baseline1`7/7PASS: extended real AI entrypoints,8serve+8return+8unreachable cases across formats/ends/bots. Legal calls increment exactly once, correct Lift/Serve recording and.55gain; repeated calls emit no extra event, unreachable paths silent. Existing behavior coverage, not audio runtime fix or listening approval.
- Gameplay excerpts from `wave3-shuttle-white-green1` are current runtime despite later test-only correction. Both recorded at1x setting but strict clock FAIL: singles.093839695s and doubles.06081214s >.05. PCM/video differences now within50ms; no retime/padding/mux. `parent-media.json`MEDIA_BLOCKED. Static8s review identifies compact shuttle above net, not temporal acceptance/full match/Android.
- Full integration regression started once after concrete shader/material diff; final results must be read from wave3-regression-edit1/play1, not inferred. Lunge known baseline failures retained; no new candidate. Protected rules untouched. Evidence base: `evidence/shareable/20261123/overhaul-005531/`; comparison `wave3-comparison/shuttle-before-after.png`. Backup `.backups/shuttle-readability_20261123_1527/`. APK still old; no phone/commit/push.

## D-063 — retained contact fix; lunge rejected (2026-11-23, 15:05)

- FINAL RETAINED runtime: four immediate-Present calls in CourtPrototype/CourtOpponent/CourtDoubles. Human16cases .652610242m → .0000006078505m max;AI return/serve stale pose RED confirmed. `wave2-contact-green2`46/46focusedPASS includes motion/audio/natural excerpt;source/data unchanged. Current runtime/assets/settings match that run exactly;only FootworkWaveTests.cs differs. Parent `wave2-contact-checkpoint1.json`status CONTACT_SUBSET_VERIFIED_LUNGE_REJECTED. No full-regression/overhaul closure.
- E initial full-foot target snap candidate rejected before import. Parent smooth elapsed-weight candidate passed stationary continuity/direction48cases and47focused tests;controlled onset .08000016m → .0000002682209m. Visual still only shallow stagger,not deep athletic lunge. New48case moving-foot probe exposed max.2250002m;restored baseline measures.117160469m. Both fail .081m gate;stationary.08m was NOT universal baseline. Candidate worsens running transition,so rolled back AthleteVisual.cs SHA-exact to56a3950d032714dc7076b34fdd546d23743869f385b251257553670216a4672e. Candidate preserved under wave2/E/deliverables/parent-continuous-candidate. No fourth attempt/no masking of failed tests. E animation remains OPEN.
- `wave2-lunge-green1`47PASS and`wave2-lunge-natural1`5PASS are REJECTED candidate evidence,not current gameplay. Last latter capture singles maxclock.050904453s andPCM/video.079s FAIL;doubles passes. Both natural excerpts contain only Serve/Clear/Smash human states,no Net/Lift;not natural-lunge evidence. No source/audio retiming or tolerance relaxation.
- Current retained runtime video is `wave2-contact-green2/CaptureWaveOneNaturalRalliesAt1x-8a969bbd/`:singles763frames/25.442s,maxclock.04167268s butPCM25.578667s (FAIL);doubles766frames/25.521s,maxclock.03446966s,PCM25.557333s (PASS). Doubles AV muxcopy exit0,ffprobe video25.521s/audio25.557s,start0/decode0stderr-empty. Strict overall media gate still BLOCKED;not audible review/full match/Android acceptance.
- Same-camera32human-contact PNG verified;`wave2-comparison/serve-before-after.png`andcontact-comparison.json. Controlled lunge comparison retained explicitly rejected. Broad-view shuttle/grip still hard to read. A quality limit,whole audio/squeak,storage intermittent lock andAndroid exclusions persist. No live workers at14:39check;no five-minute automation claim. Subset partial,not35tasks complete.

## D-063 — wave2 defects

- Actual RED same-call AI return/serve stringbed offsets.552630186/.652610242m:legal flight committed but pose deferred to render update. Three existing-Present calls candidate integrated;GREEN pending. E direction fixed-right-foot issue reproduced;direct-placement candidate not accepted for motion. F test name-only bone selector matched16instead8;fixture issue not runtime contact evidence. Worker correction assigned without relaxing exact8/count/reach thresholds.

## D-063 — wave1 acceptance limits

- Integrated B/C/D targeted9/9PASS,not whole feature closure. Same-harness natural recordings strict drift FAIL:before.09185637/.08039312,after.091340869/.064639797seconds against.05. Before PCM/video durations also fail. Do not mux/retime/pad,do not claim in-game audio defect from recorder diagnostics. Shuttle net-centre not identified visually;whole readability still OPEN. No character imported,no squeak or voice implemented.

## D-063 — wave1 asset and interruption review

- Original athlete candidate A rejected: shoulder/sleeve fit, shorts crotch, spherical shoes and overlapping open/gripping fingers. FBX existence and rig-name check do not establish Unity compatibility or natural deformation. ASSET_QUALITY_LIMIT; retained in isolated A worktree, not imported canonical.
- Prior full regression killed(-15) before XML/verification. Parent compared before manifest, archived two test-scene artifacts and restored exactly two difficulty prefs from SHA-matching backups. No raw preference values/log credentials published; interrupted suite remains NOT_PASS.

## Overhaul D-062 — 2026-11-23

- T7d audio lifecycle RED t7d-audio-boundary-red1:all12menu/restart/rematch transitions retain actual serve playback. StartMatch and FinishUiAction Home omitted AudioSource.Stop. Two guarded calls only;GREEN41/41,12/12stopped,CSV/source comparison verified. Visible3/3+Edit20/20PASS;fullPlay still running. Pause/replay policies unchanged;not audible-mix or whole-flow acceptance.

- T6d controlled continuous recording:Unity1/1PASS but independent media gate FAIL0.1264661s live-clock drift against0.05s. Failed `t6d-motion-visible1/media-verification.json` preserved. First unpaused segment only;frames4–5 wall119.7074ms/game34.0994ms then5–6 wall30.2353ms/game115.4088ms. Phase mismatch observed,root cause not isolated. Full decode/framecount/PTS-DTS pass,not1x acceptance. No runtime fix,threshold change,trim or blind retry.
- T7b human-contact playback new40case matrix passes first run alongside5existingtests,6/6total/280checks;baseline coverage,not a reproduced defect or audio redesign. Duplicate/rejected entrypoints do not restart stopped AudioSource. Exact PlayOneShot count,clip/gain,AI and listening remain unverified. Full current-source regression Edit20/20 + Play228/229:explicit SaveMatch IOException Unable to remove the file to be replaced reproduced at ClosureTests:190 / CourtPrototype:298. No diagnostic owner captured;original owner UNKNOWN. Parent checkpoint exit1 preserves failure,source/protected unchanged. No atomic-save change or blind rerun.

- T6d parent-reviewed finding reproduced: `t6d-resources-red1`1FAIL,2destroy/recreate cycles;cyan hint GameObject/material survive court owner destruction,ring/material already released. Shape creates unparented hint with own Mat;ReleaseArena/ReleaseUi omitted it. One guarded cleanup in ReleaseUi;`t6d-resources-green1`1/1PASS,2cycles all4references Unity-null,source/data unchanged. Before/after teardown PNG inspected:cyan oval present→absent;magenta destroyed-arena fixture is not normal gameplay presentation. Whole leak-free/runtime acceptance not claimed.
- T6d test gaps confirmed in parent review:activeInHierarchy alone misses disabled renderer;visible inspection manually invoked LateUpdate;contrast shown-hidden mask omits low-contrast glyphs. Test-only strengthening checks renderer/material/alpha/width/identity,visible natural EndOfFrame,and independent black-white text masks. `t6d-review-visible1`3/3PASS;independent pixel checker exit0,340PNG/80states/112regions/2destroycycles. Expanded contrast mask includes720pixels omitted by old selector,still minimum13.0812651energy/16.7622110notice;not proof those720pixels were invisible. No energy/notice runtime color fix. Final full20Edit+228Play+3visible PASS;parent checkpoint exit0,251executions/249unique,source/protected exact. Continuous marker/point-flow open.

- T6d score:RED64scenarios panelheight156/header absent. `CourtUi.DrawGameHud` minimal compact132,explicit POIN column/GAME header;GREEN5/5,64score scenarios plus16HUD/point PNG,contrast5.408073. Only CourtUi.cs differs RED→GREEN;control/menu/stamina/physics untouched. FullEdit20/20+Play226/226 PASS;score checker251executions/249unique exit0. Active-marker RED reproduces48missing active states and8pause/replay hint leaks. Cyan hint is FormationTarget(0),not controlled identity;AnimateDoubles skipped by Update during pause/replay. Minimal LateUpdate visibility + hollow yellow ground ring GREEN3/3,80marker states and72HUD captures;full candidate regression pending. No rule/root/RNG/energy change.

- T6b visual gate extension: first camera candidate7/7+UI3/3+Edit20/20+Play226/226 passed256executions/252unique,but16:10 crop proved sideline hidden by SMASH. Original body-only overlap gate insufficient. Expanded actual line-quad/control SAT(+3canvas margin) RED32rows/16scenarios,all1280×800. Camera localX0→.2m and focus same;GREEN7/7,64framing/768linequad checks0overlap,32high flights visible. Near/swapped16:10 crops show clear white boundary;green runoff still behind HUD. Final2 complete256executions/252unique PASS;camera checker exit0,source/protected exact. Framing locally closed;whole shuttle readability remains pending.

- T6b framing RED64scenarios/1360mesh-court rows:32fails from16reference heights .1697764<.18 viewport and headroom .4278028>.40. Baseline highFlight32/32 stays in viewport. Candidate camera height5.2→4m and lookHeight1→0;FOV48/Z/damping/jump unchanged. t6b-camera-green1 7/7 includes64framing+32highFlight,change-ends/replay/jump;full regression pending. Targets are D-062 calibrated design checks,not official rules or Android acceptance.
- T6c final closure:check_touch_checkpoint.py exit0,t6c-touch-checkpoint1.json249unique PASS (20Edit+226Play+3visible),104PNG decoded,1147source/1348protected exact. Four defects below closed locally;historical save lock and broad T6c layout/ergonomics remain open.
- Media diagnostic:default ffmpeg null mux emitted nonmonotonic DTS warnings while original MP4 packets208 had strict PTS/DTS. Decode with passthrough+1:1000 encoder timebase exited0/no diagnostics. Original media unchanged;null-output rescale warnings are not proven original packet corruption.

- T6c resize root: Update refreshes uiScale but not idle stickCenter. At1280x800 centre remainsy588 instead of668; 35px horizontal drag gives normalized35/sqrt(35^2+80^2)=.400818834, matching real .400818825 rather than .5. RED t6c-resize-red1 retained. One guarded idle reset after scale update; t6c-resize-green1 1/1 with8cases/4resolutions/2modes,centreError0,heldJump owner/cancel/pause/drawn-hitrect parity passed. Current-source visible/full regression running separately.

- T6c pause reset: RED16cases,14 stale centres up to310.0774canvas units, because ClearGameplayInput omitted stickCenter reset and carried Floating origin into next screen/mode. One-line reset added; t6c-pause-green1 targeted26/26 PASS,16cases centreError0. Charge/energy/velocity/no-ghost-release already pass in RED; do not call those newly broken.

- T6c guide draw order: actual capture RED2364cyan pixels across24button interiors/4held screenshots; draw guide before buttons => GREEN0interior pixels,422guide pixels still visible per held frame,1/1capture and8PNG. After near-doubles held frame inspected: labels clear, guide remains in gaps. No control/hitbox/font/layout change.

- T6c pointer ownership: RED16/16 stolen,8/8serve charges reset by second finger; guards jumpFinger/serveFinger==-1 fix root. GREEN25/25 relevant suite,16/16 ownership+cancel/swipe+movement preserved across format/end/Fixed-Floating. No native touch/device claim. Guide pixel RED running separately; full regression not rerun yet.

- `T5d_FULL_BLOCKED_STORAGE`: finalPlay223/224,exact old SaveMatch IOException at MobileUiTests:328/CourtPrototype:298. Source-bound checkpoint267/268 executions,245unique. Original owner unknown, no atomic-save change. Natural minimum gaps improved but<.55m interval now up to.42s; still not whole-task acceptance.

- `T5d_TARGETED_ONLY`:16case RED showed partner-owner remains.04m apart and direct corrective shift adds.04m beyond MoveAthlete:maxstep.13m vs.09m cap. Remove post-movement root shift; adjust yielding bot target before shared momentum.15/15 targeted PASS,16case minfinalgap.7m/humanMoved0.
- `T5d_BOUNDARY_RED`: expanded48case matrix failed32edge cases at.1499999m; outward escape target clipped at x3.35 so distance cannot recover. Retained first implementation. Small four-cardinal escape search chooses feasible lane only when escape falls outside existing bounds;15/15PASS,48cases minfinalgap.6999999m. Corner144matrix targeted23/23,controlledcapture1/1,Edit20/20 PASS. FullPlay223/224 fails old explicit-save path; no closure. No owner/receiver/reach/RNG/cap/D-050 change.

- `T5c_NUMERIC_VERIFIED_MEDIA_BLOCKED`: full20Edit+223Play+15targeted+1visible PASS,259executions/244unique source-bound. ContextNet RED4/48→48/48; flight16/216too thin→216/216 after planar+height duration correction. No reach/collision/difficulty-cap changes.
- `OPEN_CAPTURE_VARIANCE / terminal-frame-sampling-fixed`: original fullDecode+PTS pass but strict maxdrift.0558821/.065178s>.05 and baselineHard2missing terminalframes retained. Mandatory terminalframe capture fixed skip/exit path. Fresh matchedbefore2/terminal-green1 pair media-check3 passes8/8terminalframes anddrift.0492161/.0476743s; does not identify/fix original clock hitch. Diagnostic237render samples shows fixed scaled/unscaled offset after firstframe andmaxreadback20.8ms. Strict50ms retained; natural motion NOT_CERTIFIED.

- `T5b_PHYSICS_CLOSED_LOCAL_VERIFIED`: final20Edit+220Play+1visible PASS,241unique source-bound t5b-checkpoint1.json. Instant ground4.5→.9m/s first tick; bounded air steering,all-bot parity/snapshot/reset verified. Whole T5b remains MOTION_PENDING,not natural-motion/Android acceptance.
- `CLOSED_LOCAL_VERIFIED / brake-render-cadence`: full218/219 expected Braking but Forward. Fix1 render-delta comparison false-triggered Braking while running (30/31). Fix2 detects actual fixed-tick velocity reduction;31/31 targeted then220/220. No third fix needed.
- `HARNESS_CORRECTED / air-window-boundary`: fixed .04–.2s review window included x=-3.4 clamp then reverse. Preserve failed checker/source; segment first airborne frames until boundary,retain full interval too. media-check2 PASS,not a runtime fix or natural-motion certification.
- `FIXED_TARGETED / momentum-snapshot`: reset+restore loses next-tick position by.0720000193m. RED2/2→GREEN2/2:optional athleteVelocity[4], finite/horizontal/length/cap validation before mutation, legacy1/2/3 missing starts at rest.16format/end/air/energy cases.
- `FIXED_TARGETED / administrative-jump-reset`: NextPoint retains jumpY.0679999739; all20reset scenarios exposed stale jump or momentum. ResetAthleteMovement now used only point/new-match/Training/menu; pause/replay0/1 preserve momentum. Targeted34/34 includes reset matrix.
- `FIXED_TARGETED / bot-momentum-parity`: bot first tick.09m vs human.018m at100energy. RED2/2→GREEN2/2 with480bot samples and24chase/recovery cases; cap/difficulty/stamina unchanged. Reaction wait RED0m snap→brakes previous direction without target/RNG read;34/34 target suite PASS.
- `HARNESS_CORRECTED / fatigue-start-phase`: old LowEnergySlowsMovement test teleported root without clearing new momentum; compared fresh tick1 against tired tick2 (.03275997m). Reset through StartMatch/EnterTraining before each energy case, same thresholds retained. Targeted34/34 now PASS; no fatigue formula change.

- `CLOSED_LOCAL_VERIFIED / singles-recovery-fatigue`: recovery used base speed while chase used fatigue. RED0/1 expected .0737999678m vs .089999795m at energy30; add same AthleteFatigueSpeed(1),GREEN13/13,final20Edit+212Play+1visible PASS. No D-050 tuning. Source-bound t5e-t6e-checkpoint1.json.
- `HARNESS_CORRECTED / stamina-freeze`: swapped-end NextPoint enters ChangeEnds; Serve correctly refused. Advance bounded legal flow to Serving,assert Rally;1/1 then full212/212 PASS. Earlier Assert.Multiple compile failure fixed to supported ordinary assertions; neither failure counted as product defect.
- `STATE_CLOSED_LOCAL_VERIFIED / T6e`: typed point/review,Training/menu-state now pass current full212/212 and5visible/117PNG. This closes state slices, NOT historical explicit-save File.Replace reliability. Failed t6e-play-final3 remains OPEN_INTERMITTENT below; owner unknown.
- `RETRIEVAL_BLOCKED / animated-men-Drive`: HTTP200 body was2040byte quota-exceeded HTML,not license/FBX. Renamed failed bodies with .download-error.html; no import. Creator-linked Poly Pizza GLB signature/length verified separately,asset still unaccepted.
- `EVIDENCE_HELPER_ONLY`: repeated immutable stamina-media verifier refused existing output as designed; existing PASS retained. Ad-hoc scenario aggregation used -start event name without stripping suffix,IndexError; discarded,no speed or acceptance claim from that probe.

- Current T7e closure supersedes pending notes below: `t7e-t6e-edit-final2`20/20 + `t7e-t6e-play-final2`205/205 + visible3/3 PASS. `t7e-t6e-checkpoint2.json` source-bound228unique. History/review/result/reaction-fixture defects CLOSED_LOCAL_VERIFIED; earlier failed runs retained.
- `CLOSED_LOCAL_VERIFIED / training-stale-state`: Serving/Point/review state leaked into Training. RED0/1→GREEN3/3, then full205/205; match-only fields cleared, energy preserved, still after has no stale serve/review.
- `CLOSED_LOCAL_VERIFIED / menu-save-feedback`: successful save and errors lacking substring error were hidden. RED expected Pertandingan disimpan. vs empty; dedicated MenuNotice GREEN3/3 and full205/205, five after states inspected. Locked/missing/malformed/schema99 saves preserve live match.
- `FIXED_TARGETED / point-outcome-from-debug`: typed persisted PointCause/PointReview replace feedback parsing; training/next-point clear, actual challenge call retained, legacy unknown honest. Targeted56/56 + visible2/2/16PNG. Full current20EditPASS +207/208Play; not CLOSED due storage failure below.
- `OPEN_INTERMITTENT / explicit-match-save`: `t6e-play-final3` failed PointOutcomeSnapshotRejectsInvalidEnumsAndKeepsLegacyUnknown at MobileUiTests:328 -> SaveMatch CourtPrototype:290 -> File.Replace, IOException Unable to remove the file to be replaced. Two saves logged before failure; target/backup lock owner/HResult not captured. Same-source targeted56/56 earlier does not repair this. Source/protected data unchanged. Existing explicit-save blocker extends to match snapshots; no evidence point enum logic caused lock. No blind retry or non-atomic fallback.

- `CLOSED_TARGETED / result-reaction-fixture`: full203cases,202PASS/1FAIL `MobilePresentationTests.ReactionsTrackOverturnedCallsGameIntervalsAndMatchResults`, `Invalid game history count`. Fixture set Games=1 without history. First repair correctly scored game but exposed stale human-only serve assumption (`Expected log did not appear: [Log] Regex: \[collision\] ground.*`). Fixture now uses AwardPoint and actual legal human/bot server; `t7e-reaction-fixture-green2`1/1PASS (12scenario), no runtime relaxation. Full suite after fixture edit pending.

- `LOCAL_PARTIAL / per-game-history`: completed Points discarded; RED missingGamePoints→Edit20/20 with actual per-game array. Save validation RED null accepted; schema3 migration explicit, result2 exports history. Full final regression pending.
- `HARNESS_CORRECTED / history`: old game3 fixture changed only Games/GameNumber without history; score first two games through AwardPoint. Old version2 assertion updated to3. Deciding review fixture lastPlayer/hitter inconsistent in doubles, corrected test-only;16cases now pass. No validator relaxation.
- `CLOSED_TARGETED / result-game-labels`: RED missing rows→visible4/4,8afterPNG individually reviewed,1280x720. Winner/game scores/unavailable legacy rows visible; rematch/menu unchanged. Other-resolution/full final regression pending.
- `CLOSED_TARGETED / review-history-prefix-transition`: corrupt beforePoint15:7 vs actual15:0 and erased earlier game accepted in two RED runs. Prefix + exact one-point score transition checked before mutation; targeted54/54 PASS.
- `CLOSED_TARGETED / legacy-game-ending-review`: new validation rejected v1/v2 nonfinal game-end save because final points were unknown after reset. RED2/3 `Contradictory review game points`; recover only score proved by stored pre-point state and awarded side. GREEN54/54 includes load, corrected challenge removing that completed game, save/reload, repeated-review rejection. Unknown earlier games remain -1/-1; no fabricated scores. Full final suite pending.

- `CLOSED_TARGETED / net-shot-envelope`: Unity108case RED maxapex3,121608m vs calibration≤2,30m; net duration `.82+.04d` GREENmax2,256229m and predictionError0; related59/59. Initial RED1 was fixture before active Feed, not product proof; corrected fixture retained RED2.
- `CLOSED_TARGETED / AI-net-predictor`: prediction omitted CollideNet; RED0/1→GREEN9/9 with same net deflection as actual flight. Full T5 regression pending.
- `CLOSED_TARGETED / singles-RNG-before-reach`: impossible height consumes RNG71→18147363. One-line order change CanReach before shot RNG, RED0/1→GREEN12/12 with12paired cases; not whole AI acceptance or pacing stability closure.
- `DIAGNOSTIC_PATH_ABSENT`: graphify-out/graph.json not present; direct-source inspection used, no graph install/rebuild.

- `CLOSED_TARGETED / court-floor-and-service-paint`: T1b RED0/2, floor−0,03m dan centre strip rejected; GREEN5/5 sesudah ground-aligned40mm paint dan batas legal tepat. Capture720p reviewed; full regression Tahap1 masih pending.
- `LOCAL_NUMERIC_PASS / net-profile`: T1c RED0/3 membuktikan tape billboard, crossing tengah+3mm masih terkena net, racket di bawah mesh tetap fault. Shared NetTop/CollideNet, tape75mm dan mesh baru: GREEN50/50.
- `CLOSED_LOCAL_VERIFIED / net-normal-cancellation`: shared front/back vertices membatalkan normals, RED0/1 → GREEN51/51 setelah split. Residual near-angle aliasing muncul di capture1x; shader probe tidak membantu, MSAA2 sesuai runtime membuat still utuh. Final visible `t1c-final1`67/67 + near/side review tanpa gap besar. Tidak mengubah runtime MSAA atau shader.
- `CLOSED_LOCAL_VERIFIED / shuttle-under-net`: `t1c-under-red1`0/1 expected fault log missing; crossing di bawah net kini fault hitter sekali, kedua arah, challenge ditolak. Final67/67 PASS. Bukan continuous-motion atau Android acceptance.

- `CLOSED_TARGETED / court-surface-shadow`: material tanpa sampled detail RED0/1 → GREEN4/4; opaque shadow tanpa alpha falloff RED0/1 → GREEN5/5. Texture/material lifecycle empat role teruji, bukan motion/Android.
- `CLOSED_LOCAL_VERIFIED / raised-court-platform`: RED1/2 beda0,100000009m; HallFloor y−0,15→−0,055, beda5mm. Final2 visible22/22 + Play194/194, top/side/both ends still reviewed.
- `CLOSED_LOCAL_VERIFIED / green-court-contract`: full1 193/194 failed expected green>0,435 actual0,430000007. Blue .29→.27; final2 Edit19/19 + Play194/194 + visible22/22. Failed XML retained, no threshold relaxation.
- `CHARACTER_BLOCKED_ASSET / T2a`: tiga jalur dievaluasi, nol eligible. Parent verifier exit0 dan evidence di `t2a-parent-audit1`; tidak mengulang procedural garment gagal, tidak import kandidat yang belum lolos.

## Diagnosis D-060 in-process — 2026-11-21

- `OPEN_OWNER_UNKNOWN`: pre-registered in-process RM observer covers all SetString IOException paths. Full bounded probe184/184 PASS; only injected backup lock captured, ownerUnityPID3396/creationFILETIME verified. Catch-to-query0.991ms/query54.511ms, not a guarantee of capturing a transient historical owner. Original explicit-save failure NOT_REPRODUCED; no causal fix claimed.
- `DIAGNOSTIC_TEST_SHARING_CONFLICT`: green1 1/2 fails when test reads held backup with incompatible sharing. Assertion moved after handle release, same `older` content required; green2 2/2 and full184/184 PASS. Failed green1 retained alongside expected missing-hook RED0/1.
- `DIAGNOSTIC_REMOVED`: three files archived/restored SHA-exact; restored15/15PASS, source1.147/protected1.348/APK1ec08181...unchanged. Original storage and historical audio remain OPEN. No retry/delete-write/swallow/OS workaround. Evidence `d060-inprocess-closure1.json`; additional event-level tracing outside D060 requires owner consent.

## Checkpoint produk D-060 — 2026-11-21

`d060-checkpoint1.json`:current208executions207uniquePASS plus4strictnaturalAV/fullmuted/24stills and12scenarioreplay. NewAPK30233578bytesSHA2561ec08181fe9ff0f5419e87305347696a9846ec74e88dbb98d68025f9f19f0919verifiedpackaging/source/protected. BuildReport0errors0warnings;compilerDiagnostics[],butlicensingvalidation/D3D12infoqueue diagnosticlinesretained. KnownRNG/reactionepoch/joystickbootdefectsCLOSED_LOCAL_VERIFIED;originalexplicit-saveownerUNKNOWN,historicalAVlagOPEN_NOT_REPRODUCED,motion/listening/owneracceptancepending. No blanketno-error/allDoDclaim;failedrunsbelowretained.

## Riwayat status DoD, 2026-11-21

- `CLOSED_LOCAL_VERIFIED / freehandaircontact`: realTapShotRED0,304066032m. Fix1targetonly0,0573486164mFAIL;sourcebranchalsochangeships/chest/leftshoulder. Fix2standalone0m butmatrixlowheight.05mFAIL0,29644075m. Fix3aircontactresttargetblend:57/57PASS/144casesmax4,85146643e-7m,contact/root/grippreserved. Fullregression204executions/203unique PASS, source-boundAPK`4dad93be...` verified. Fix localnumeric closed; continuousmotion/listening not accepted. NoAI/timer/reachchange.
- `HARNESS_SELECTION_ERROR`: matrix1 pickedCaptureprofileinbatch→ExpectedFalseButTrue; rerunvisible matrix2profilePASS. Failurehistoryretained,notproductbug.
- `SUPERVISION_PARTIAL`:275,77/316,67s,onechild>5minutes;parentlist163,1s/steer,postdeliverytrace/artifactsverified. Do notclaimfullcadence.
- `OPEN_OWNER_UNKNOWN`: localVSUnitydebuggerDLLverified,parentVSwhereconfirmed;attachNOT_RUN,noexternalownercapture. No genericretry/OSchanges/storagefix.

- `OPEN_INTERMITTENT_NOT_REPRODUCED / audio`: current4naturalstrictmediaPASS;callbacktimeline60s wallMinusDsp.0037585333s/pairedlag.0000418667s,nooverflow/pause/configmarkers. Historicaldeficitnotreproduced,notfixed. Diagnosticsarchived/testsrestoredSHAexact,smoke3/3PASS.
- `DIAGNOSTIC_PRESERVATION / standalone`: flawedmetadata REDmasksoriginal; guarded8caseGREEN independentlycompiled/runparent. NotUnityintegration/nooriginalownerproof. Noadditionalproductionstoragechange.
- `EVIDENCE_SELECTION_CORRECTED / replay`:floorframeforserving-oncewasPoint;firstServing1132/39.43361sverified. ConflictingvisionJEDA/LEWATI resolvedcrop→LEWATI. Originalsamplesretained;notruntimebug.
- `BUILD_LOG_NOT_CLEAN`: BuildReport0errors/3SDKwaiting-manifestwarnings;license-token-unavailablethenentitlementresolved,shutdownCurl42. No compilerCSerrors/warnings; do notclaimalllogsnoerror.

- `REPRODUCED_LOCAL / doubles RNG before reach`: tick38→39/sourceprobe danactualUnityRED RNG71→18147363 padaheight3.6418302mimpossible. CompanionREDlaunchvelocityberbeda. Minimalreach-before-drawfixGREEN5/5+36pairs,Edit19/19;fullPlayrunning. BukanEasy-stabilityclosure,APKolduntilrebuild.

- `D060_RM_OBSERVER_VALIDATED_NOT_STORAGE_FIX`: Unity REDmissingobserver→GREEN2/2;syntheticFile.ReplaceHResult0x80070497preserved,RMUnityPID13948found. ActualvisibilitytwoexplicitwritesPASS,originalfailureNOT_REPRODUCED/originalownerUNKNOWN. Diagnosticarchived/testrestoredSHAexact,no runtimechange.

- `OPEN_STABILITY / RNG-only`: predeclared2runsEasy1273.720947265625FAIL/748.89727783203125PASS,overallexit1. RNGfixnotwholepacingfix. NewfloatclockRED15vs14ticks→doubleclockGREEN7/7,save/replay/pause7/7;fullsuitepending. No delayconstant/gate/rulechange. Countcandidate1(RNG-only)doesnotresolvegate;candidate2precisionunderverification.

- `REPRODUCED_AGAIN / explicit-save`: `d060-clock-play1`181/182;ConfigureVisibilitysecondwriteFile.ReplaceHResult0x80070497,trailBrightness.txt.tmp retained. Parent no retained stream proven, originalownerUNKNOWN. One full exception-preserving RM diagnostic `d060-storage-full1`active, no runtimestoragechange.
- `SUPERVISION_FAILED / deleg_78f71d68`:firstcheck381.5s>300s,parentblockingtool249.51s;children423.38/420.07s>180s theninterrupted. Actualaudits/scriptsverifiedparent,notclosure. Clockchildverifierfailedtuple/listJSONcomparison;preservedRED,minimumserializationfixGREEN. Archive`d060-child-review2`.

- `CLOSED_TARGETED / joystick boot write-on-read`: actualfullsuite`d060-storage-full1`181/182 failsAwake→ConfigureJoystick→File.Replace. Controlledbackup-lockRED`d060-joystick-red1`0/1 exactstack. One-lineAwakeassignment(noSave)GREEN`d060-joystick-green1`8/8 covers4bootvalues,allprefsbyte-identical,inputdefaults,explicitpersist,MatchFlowcase,visibility. Fullfinalregressionpending. Originalexplicit-savefailure remainsOPEN_OWNER_UNKNOWN.
- `LOCAL_BOUNDED_PASS / doubles reaction-clock`:two predeclaredstabilityrunsPASS849.6610717773438/852.6569213867188s;fourrunsEasy849.661–854.162. Allalignedrallyticklengths/hitter/owners/RNGexact;positionmaxcomponentdelta0.00011062000000006122m retained,notbit-identicalclaim. Historicaldurationfailures preserved.

## Riwayat diagnosis lanjut, 2026-11-20

- `OPEN_INTERMITTENT_NOT_REPRODUCED / capture`: ABBA60s file/null/null/file tidakmereproduksi laglama;pairedlagfile0,0010684/0,0068140s/null−0,0043975/0,0031929s. Replay12scenario danlongganda terbaru strictmediagatePASS;DSPwalllag0,0059717/0,0761451s. Runtimeunchanged;inconclusivecause, bukanfix. Counter-onlysupersededStream.Nullcopy/lockidentik;diagnostictestsourcearchived laluSHAexactrestored;restoredsmoke3/3PASS.
- `OPEN_OWNER_UNKNOWN / explicit-save`: parent mengonfirmasi source memakai saveempatfile berurutan, fixture menyimpanbytes, syntheticlockcleanup memakaiusing/finally. Tidakada buktipemiliklockoriginal; tidakgenericretry, delete+write, OS/antiviruschange. Proposed managedhandletrace belumdijalankan.
- `MEDIA_CHECKPOINT_NOT_FIX`: replay56,665s/1582f dan ganda504,391s/13942f strictcapture+container/decode/fullmutedPASS;drop3/17. Sepuluhstill individualverified(4replay/6natural). Runtimediagnosisunchanged; testdeltaarchived+SHAexactrestored,source1147/protected1348/APKunchanged. Tidakmenerimaaudio/motion atau menutuphistoricalfailures.
- `SUPERVISION_WITHIN_BOUNDED_WAVE`: deleg_02d67536 dua audit205,20/189,92s; parentlist99,1s, postdeliverytooltrace/artifactverified; tidak ada anak mencapai5menit. Bukan bukti cron lama diperbaiki.

## Riwayat D-059 — parent verifikasi, 2026-11-16

- `OPEN_CAPTURE / controlled replay DSP-wall drift`: final4 replay12scenarioPASS tetapi strictmediaFAIL PCM52,629333s/metadata56,75898s. DSP samaPCM,wall56,636212s;lag4,006879s. Encoder rawvideo56,112s1687frames validprobe,tidakdimux ataudinyatakanAVPASS. Audiofailure bukan khususganda. Tidak ada rootcause/fix yangdibuktikan; janganmengubahsyncgate,clockgame,OS atau isiPCM.

- `OPEN_CAPTURE / ganda PCM duration`: team-natural1 video527,801s/15838f,rawPCM521,536s,metadata528,3879s. Strictverify_capture.py:55 gagal(absdiff<1);defisit6,8519s,finalizegap0,1533s tidakcukupmenjelaskan. Three singles .19077/.0017/.0022sdeficit tidakserupa. Rootcauseawal/callbackgap/DSPvswall unknown;logtidakpunyaaudioerror eksplisit. No stretching/padding/silencemux;rawpreserved. Test-onlyclockinstrumentation RED0/1→GREEN2/2,produkuntouched. Smokesample2,56sPCM/2,5501284swall belummerepro. Instrumentedgandaprobe merepro:PCM=DSP592,7466667s,wall598,0548692s,lag5,3082025s. Startup.0052577s,maxcallbackgap.4081931s,maxwrite.0246014s;PCMsamplebytesmatchcallbackcount,48k unchanged. Missingwalltime bukanmissingDSPblocks. ABBA60s:video-on lag1,873203/2,514118s,no-video4,290657/3,453606s;encoder/readback bukanpenyebabtunggal. Filewrite/scheduling/system belumterpisah. Diagnostic diarsip/cabut,SHAexacttestsrestored;originalfailuretetapOPEN.
- `SUPERVISION_TIMING_NOT_MET / audit2`:parentlist/steer267s;anakfinal671,64/392,28/338,40s. Satu503visionrecovered,satusteertidakmasuk. Kerjanyata79still dibuktikanlog+hash;parentmenghabiskan9sisa langsung. Bukan5mincadenceguaranteed.

- `CLOSED_LOCAL_VERIFIED / UI-SHADOW-VALUE`: label sebelumnya tanpa nilai aktif. RED0/1 membuktikan `#1A2424` hilang; satu baris `CourtUi.ActionLabel` menampilkan hex RGB. GREEN4/4 mencakup tiga warna/toggle/padding/hover/persist/read-onlyload; tiga still diperiksa utuh. Final4 Edit19/19+Play176/176+Capture5/5PASS; empat viewport Settings dan Setup/Result16:10 diperiksa. Fix sudah masuk APK30.232.134byte SHA256f930f21fe9c5f88f620ec95b041ff1fd8dc8dc24937497ff2b25376da4e0f7b6. Packaging bukan AV/whole-game acceptance.
- `NOT_PROVEN_DEFECT / UI-CREDITS-EDGE`: URLfont pada1280×720 terpotong vertikal di batas bawah viewport berscroll; bukan horizontal overflow atau bukti lisensi hilang. Parentvision dan `GUI.BeginScrollView` source mengoreksi penamaan temuananak; interaksi scroll belumdijalankan.

- `CLOSED_LOCAL_VERIFIED / LoadVisibility write-on-read`: team-final-play2 158/174PASS,16FAIL saatAwake→LoadVisibility→ConfigureVisibility→SetString(trailTint). Protectedbaseline delta0. RED team-load-red1 0/1 mereproduksi exactIOException denganbackupslocked;GREEN team-load-green1 42/42PASS,source-delta0. ApplyVisibility memisahkanvalidasi/render dariConfigureVisibilitysave;LoadVisibility read-only,clamp/migration/render unchanged,setting eksplisit tetapdisimpan. Test membuktikanprefsfile-set+bytes tidakberubah,backupterkunci amanuntukload. Original user-write lock tetapOPEN; tidakmenggantiFile.Replace atau mengklaimlockowner teridentifikasi.

- `CLOSED_LOCAL_VERIFIED / doubles Setup arm crop`: 1280×800 Partner mesh maxviewportX1,01285338RED;cameraSetup x−1,2→−0,9 tanpaactorrootchange. Bakedwholemesh4viewport+facetest4/4GREEN,finalcapture3still16:10/ultrawide confirmswholearmvisible. Framingfixsource-boundfinal3fullsuite+APK;motionacceptance tetap.
- `CHECKPOINT_VERIFIED / final3`:19Edit/175Play/4CapturePASS,APK30.232.522byte/source1147/protected1348verified. Checkpoint tidak menghapusEasyintermittency atauoriginalexplicit-save lock;AVacceptanceNOT_RUN/NOT_CERTIFIED.
- `CLOSED_LOCAL_STILL / menu racket-face`: target khususmenu(.45,1.35,.25),gameReady/contact unchanged. Parenttests RED→GREEN dengan48projectedtrianglecases;4Home/Setup screenshotsfacesclear. AABBcandidate43/44 failure dipertahankan; tesdiubahtriangles setelahbukti conservativebbox,baseline tetapRED. Freewrist/grip masihstiff/crowded;motionNOT_CERTIFIED.
- `DIAGNOSED_SYNTHETIC / File.Replace backup lock`: standaloneMono7cases dire-runparent;embeddedEditor exactmessage+HResult0x80070497 jugaPASS. Dataold/new/backuputuh,release+retryberhasil. Ini membuktikan failure mechanism, bukanpenyebablockoriginal. Runtime storagefix belum;fullsuitefailure tetapOPEN.
- `SUPERVISION_DELIVERY_FAILED`:cronD-059 script firsttickrun tetapioriginDiscordlama404UnknownChannel. Parentmanual list/steer menjagabataschild;hasilsemuadiverifikasi. Cronpaused,tidakmenyatakan5-minuteautodelivery berhasil.
- `REJECTED_DIAGNOSTIC / predicted human steering as pacing fix`: predeclaredABBA2PASS/2FAIL;baseline901,39905/1600,98413s,candidate930,02454/1081,86438s. Humanparticipation naik9,75%→20,24%,return/reli malah12,49→15,25,B2reli125,02s. Totalrallyturun dipengaruhi jumlahgame;stabilitasbelumfixed. Diagnostic source diarsip,originalharnessSHAexact dipulihkan. Tidak mengubah AI/timer/gate.

## Riwayat status — 2026-11-16

- `OPEN_INTERMITTENT / preference File.Replace`: fullsuite `remaining-final-play1` gagal diConfigureVisibility kedua, M1GapTests:65,LocalGameData:33,IOException `Unable to remove the file to be replaced`. Key/HResult/lockowner tidak terekam saat gagal. Tes isolated1/class7/order46PASS tidak membuktikan fixed; instrumentasi test-only kini mencatat metadata dan rethrow. Data1348file hash-identik. Tidak mengganti atomic save dengan delete+write; D-059 child mencari reproduksi/falsifikasi terarah di scratch.
- `OPEN_REGRESSION / Easy duration`: fullsuite terbaru1490,08545sFAIL<1000;19EditPASS,Play169/171PASS dengan failurepersistence terpisah. Batas tidak diubah; tuningAI/timerBLOCKED_APPROVAL. Diagnosis trace lanjutan D-059 bukan izin produk.
- `VERIFIED_CONTROLLED / replay-challenge`:12scenario PASS dalam1test;full55,998s1683frame decode/PCM/PTS/DTS/mutedplayPASS,dropped0. Score/quota/timerfreeze/skiprestore diperiksa. Natural/motion/listening dan pemilik tetap acceptance terpisah.

## Riwayat D-058 — locomotion/audio, 2026-11-15

- `CLOSED_LOCAL_VERIFIED / footfall chatter`:19contacts/4s pada1,5m/s; contact spacing pi/10m. Gait visual+contact sekarang full cycle2m,tes4atlet menghasilkan6contacts/4s berjalan dan17/4s berlari4,5m/s. Consume-once/landing/airborne/menu/pause/replay tetap teruji; tidak menyembunyikan cadence lewat cooldown audio.
- `CLOSED_LOCAL_VERIFIED / synthetic step path`: RED clip JustSmash-original-step; sekarang4rekaman sneaker CC0 berbeda. Source/derivation/license bundled; resource hashes/PCM valid dan actual controlled audio captured. ListeningNOT_RUN; legal clip tidak otomatis cocok court.
- `CLOSED_LOCAL_VERIFIED / stale-contact elbow twist`: inactive pole sebelumnya mengikuti contact dunia lama,RED1,950°/3,675°. Blend ready/recovery,active-only backhand/preparation kini lulus continuity posisi+rotasi. Tes awal posisi-only tidak menangkap twist; diperkuat tanpa melonggarkan assertion.
- `CLOSED_LOCAL_POSE / near-locked ready arm`: ready target terlalu jauh menghasilkanangle177,9634°,drop6,65cm. Target lebih dekat+pole bawah menghasilkan105,9878°,drop24,17cm di4atlet×2yaw. Still after menampilkan elbow bent, bukan forward arm nyaris lurus. Kontur kedua tangan/grip masih overlap dari samping; penetrasi tidak terbukti. Natural anatomy tetap NOT_CERTIFIED.
- `OPEN_REGRESSION / Easy duration`: checkpoint D-058 Easy949,7124sPASS bukan fix. `remaining-trace1/2` gagal1542,73706/1709,63806s terhadap<1000. `remaining-deadline1/2` gagal1387,19885/1571,91504s; source-identik,semua6matchMatchEnd,service/netFault0. Trace pertama membuktikan PointUpdate/FixedUpdate divergence; trace deadline membuktikan alignedreli2offset50 float32reactiondeadline berbeda1ULP: botkedua0,09m versus0 karenaTime.time>=teamThink. Relativeage kedua0,279999900794s. TotalRally1101,00/1303,42s melampaui1000tanpaPointwait; mempercepatPoint bukan solusi terbukti. Akar seluruhdurasi belumfinal. Bukti `remaining-diagnosis1.json` dan `remaining-deadline-diagnosis1.json`; tidak tuning timer/AI/physics atau retry-until-green. Perubahan gameplay tetap menunggu izin.
- `BASELINE_COVERAGE / hand overlap`: tes24kombinasi minimumjarak joint0,5424722m;16/16motionPASS. Readyfront menunjukkan tangan terpisah;side tetap menumpuk kontur. Tidak ada bukti meshpenetration,distance joint bukan meshcollision test. Jump-smashhand/head masih sulit dibaca pada still;continuous-motion/listening belum certified.
- Riwayat checkpoint:19Edit+169PlayPASS,2controlledcapturePASS,1.147source identik;6media decode/full-muted-playbackPASS. Latest EasyFAIL di atas tidak ditutup checkpoint ini.
- `CLOSED_LOCAL_VERIFIED / internal grade visible at Point`: rawfeedback padaHUD membocorkan Poor/Good. D-055 repair hanya `CourtUi.PointNotice` menyaring empat prefixgrade exact; internalfeedback/LastGradeScore/fault/review tetap. RED `remaining-grade-red1`0/1;GREEN `remaining-grade-green1`1/1,exit0,source-delta0. Controlled still diperiksa: `Clear | IN`,tanpaPoor/Good; kontras terhadaptribun masih kurang. Fullregression/build sesudahfix belumselesai; natural1/APK lama bukanruntime-identikUIbaru.
- `OPEN_VISUAL / menu racket-face overlap`: Home/SetupD-058 semua4capture raket tegak menutupi wajah. Runtime ready pose memenuhi tesjoint tetapi siluet belumideal; bukanbukti meshpenetration. Duafaractor ganda overlap padaRally@4,552s;player/head menumpuk scoreboard padaresultHard@381,555s. Temuanstill,bukanpenilaiancontinuous.
- `VERIFIED_CAPTURE_ONLY / natural D-058`:2/2capturePASS,4MP4utuh49.170frame/1.638,442s,24still diperiksa. Decode/PTS/DTS/moov/fullmutedplayPASS;browserdrop3/6/5/2(ganda/Easy/Normal/Hard),bukanzerodrop. 1.348protectedfileidentik. ListeningNOT_RUN/motionNOT_CERTIFIED/replaychallenge tidakdicakup,tidak finalgame.

## Riwayat D-057 — closure QA, 2026-11-15

- `CLOSED_TEST_HARNESS / batch rendering assumptions`: closure-play1 dua UI failure (head x1,02620411 dan GUIStyle null). Framing test sekarang memilih empat aspect approved secara eksplisit; label-style test Category Capture + WaitForEndOfFrame dijalankan sungguhan non-batch. Assertion tidak dilonggarkan. batch-ui-green1 1/1 dan closure-motion2 4/4PASS; bukan runtime fix baru.
- `CLOSED_CAPTURE_FRAMING / net over landing`: hanya offset kamera audit di fixture diubah(3,1.6,3)→(3,1.6,1). Pilot closure-motion2 landing6,756222/recovery6,951996s terbaca tanpa tape net.319frame/31,07258samples/s/max49,788ms;1×/0,25× decode dan browser PASS. Natural motion tetap NOT_CERTIFIED.
- `OPEN_REGRESSION / Easy duration`: closure-play1 Easy1335,8418s>1000s;closure-play2 Easy1507,07275s>1000s,164/165PASS. Ketiga difficulty mencapai MatchEnd tanpa service/net fault; runtime AI/rules/physics tidak diubah. Tuning tetap perlu keputusan terpisah D-055; hasil baseline sesekali PASS bukan fix/stabilitas. Tiga perbaikan tempo historis sudah dipakai; tidak mencoba tuning keempat.
- `CLOSED_VERIFIER / null-muxer timestamp rounding`: decode MP4 variable-rate ke null dengan timebase default mencetak non-monotonicDTS walau packetPTS/DTS actual strictly increasing. Probe5s mereproduksi13warning; passthrough+1:1000 menghilangkannya tanpa menyentuh rekaman. `verify_capture.py` kini assert stderr kosong; full decode keempatMP4 dan browserPASS dalam closure-natural2/full-decode-and-playback.json.
- `CLOSED_VERIFIER / packaging evidence versus regression`: verifier semula berhenti pada closure-play2 sebelum membacaAPK. Sekarang tetap mengecek semua source/packaging, menulis detail nonPassTests/statusBLOCKED_REGRESSION dan exit1 saat suiteFAIL; tidak melewatkan Easy. closure-apk1:packagingPASS/regressionFAIL. Backup verifier terverifikasi; source game tidak berubah.
- `CHECKPOINT_ONLY`: empat rekaman utuh1×/audio game-only dan APK30.127.856byte tersedia;1.343protected file hash-identik. Motion kontinu/audible/owner bukanPASS. Android runtime tidak dilakukan.

- `CLOSED_LOCAL_VERIFIED / pale control hover`: screenshot2400×1080 memperlihatkan GERAK: MANUAL abu pucat. `hover-red1`0/1 mereproduksi GUIStyle.hover RGBA(.9,.9,.9,1) melawan ink(.027,.098,.243,1). Set normal/hover/active/focused ink; `hover-green1`45/45PASS, source unchanged during run, screenshot normal dan matrix diperiksa. Pixel isi teks navy/off-white16,19:1; off-white/panel5,41:1. Tidak klaim touch/native input.
- `INTERRUPTED_CAPTURE / natural-capture1`: proses dihentikan sebelum teardown/XML; CSV berhenti199,3161detik saat Rally. MP431.457.328byte bukan pertandingan utuh/hasil PASS. `difficulty.txt.bak` berbeda terhadap manifest data; salinan terganggu di `.backups/roller-closure_20261115/`, original dipulihkan dari backup hash-matching; seluruh manifest proteksi kembali cocok. Tidak menghapus failed/interrupted evidence.
- `EVIDENCE_LIMIT / capture overhead`: HD pilot25,7466fps,maxgap199,265ms; controlled24,6181fps,maxgap112,570ms. PC1280×720 tanpa encoder/readback: target30 median33,34045ms/p9534,1177ms; target60 median16,66069ms/p9517,37404ms. Biaya capture bukan bukti runtime hanya25fps. Enam detik per target bukan stress/device benchmark.

## Riwayat D-057 — runtime polish findings, 2026-11-15

- `CLOSED_LOCAL_VERIFIED / result framing`: kamera hasil lama memusatkan player walaupun winner lain/dua pemain. Winner-aware midpoint/aspect + viewport regression,49/49 selected PASS. Root tidak diubah untuk framing.
- `CLOSED_LOCAL_VERIFIED / arena font`: RGB atlas font bukan warna glyph; ArenaText shader memakai vertex RGB/atlas alpha dan ZTest LEqual. Pixel brightness + occlusion regression termasuk49/49; masih bukan whole-scene acceptance.
- `CLOSED_LOCAL_VERIFIED / grip`: socket wrist memberi jarak0,075359m dari palm. Socket RightHandProp + orientasi telapak/jari + fixed-length IK, palm/contact tests PASS. Reach/timing tidak diperlebar.
- `CLOSED_LOCAL_VERIFIED / recovery hip`: akhir .65s membuat snap11,2deg; weight recovery kontinyu dengan regression Smash/Net dua sisi PASS.
- `CLOSED_TEST_HARNESS / host input contamination`: prepare-green1 gagal2 root checks; root-probe1 membuktikan raw axis mengisi analog meski fixture sudah nol. QaInputIsolation reset native axes setiap frame sebelum Update; input-isolation-green1 49/49PASS. Native-input test tetap opt-in/excluded, tidak dijalankan.
- `CLOSED_TEST_HARNESS / pilot evidence`: cadence scheduling sebelumnya19,45fps dan VFR membuang frame; pilot-audit2 schedule accumulated + passthrough1ms318/318 frame,31,026fps. Kamera audit disetel LateUpdate. Numeric pilot PASS tidak menutup motion.
- `CLOSED_LOCAL_POSE / short smash follow-through`: follow target kini y1,05m/cross-body, regression existing lulus. HD frame6,707833s (+0,184s) memperlihatkan raket di bawah bahu dan melintasi torso. Closure hanya defect pose; grip/arm near-face, framing landing tertutup net dan continuous-motion gate belum diterima. Matriks numeric96 kontak sudah diuji; bukan perluasan animasi accepted.

## Riwayat D-055 — QA protection and preview findings, 2026-11-15

- `CLOSED_LOCAL_VERIFIED / QA data isolation`: PreferenceFixture hanya mem-backup prefs, bukan match snapshots. RED0/2 lalu baseline37/37; seluruh local-game-data termasuk .bak/subfolder dipulihkan, output run unik menolak direktori existing. File sets/hash identik setelah QA. Legacy test outputs di luar tiga class yang sudah diarahkan tetap perlu proteksi sebelum full suite.
- `CLOSED_PREVIEW_HARNESS / stale skin and score`: render Camera.Render langsung sesudah pose/reaction menghasilkan rambut tampak terlepas dan papan lama. Diagnosis rig1/1PASS; tidak mengubah runtime. Yield satu frame sebelum render + UpdateArena memberi rambut menempel/papan sesuai. Ini perbaikan capture, bukan peningkatan animasi gameplay.
- `CLOSED_LOCAL_VERIFIED / text depth`: defect preview ditutup ArenaText atlas-alpha/vertex-RGB/ZTest LEqual dan pixel/occlusion regression. Ganda setup dipisahkan melalui staging saja. Kualitas seluruh scene/gerak tetap gate terpisah.
- `OPEN_STABILITY / natural doubles timing`: Easy run baseline terbaru752,7991s PASS<1000 tanpa perubahan runtime, historis1069,29sFAIL dipertahankan. Normal878,2996s/Hard339,8728s. Tidak tuning/longgarkan batas dan tidak menutup ketidakstabilan dari satu run.

## D-054 — career REMOVED_BY_OWNER, 2026-11-15

Bukan perbaikan atau penyelesaian career. Pemilik memilih cabut fitur/kode/menu dengan backup, pertahankan tunggal/ganda. RED no-career0/1 menunjukkan tombol Home masih ada; GREEN1/1, Edit19/19 dan Play subset95/95PASS source-identik membuktikan penghapusan/regresi baseline yang dicakup. Save yang diubah fixture dipulihkan hash-identik; dua Home PNG diperiksa. Tidak full Play. Regresi UI/motion lama crash sebelum XML; kini source career dicabut, bukan bug crash dinyatakan fixed. Hasil Play rollback di TESTING. Temuan gameplay/animasi/audio dan Easy duration terpisah, belum ditutup.

## Riwayat D-053 — regresi ganda, 2026-11-14

### Career belum ada — PARTIAL_FOUNDATION_VERIFIED

RED career-save0/1 lalu green20/20 Edit, archive RED2/3 lalu green22/22: slot independen/backup/recovery/read-only retirement tersedia. CareerWorld RED0/1; stress green1 gagal fixture InvalidCastException pada List<RankingResult>→Array setelah simulasi dua dunia4020hari. Fixture diganti IEnumerable, runtime tidak dilonggarkan; `career-world-green2.xml`23/23PASS,source1.130file identik. Entry RED0/1 membuktikan Register belum ada → career-entry-green1 Edit24/24PASS. UI RED0/1 tombol Career width0 → career-ui-green1 Play9/9PASS, save exhibition utuh. Source/test/failed XML disimpan; bukan career lengkap. Partner RED0/1 ChoosePartner null → career-partner-green1 Edit25/25PASS source1.134file identik; pair-ranking/lock/archive/save tested. UI partner RED0/1 tab tidak reachable → career-partner-ui-green1 89/89PASS,source1.134identik; screenshot tab aktif/nama/hubungan/tombol tanpa overlap. Progression RED0/1 Train null → career-progression-green1 Edit26/26PASS; UI RED0/1 tab width0 → career-progression-ui-green1 Play90/90PASS,source1.136identik. Latihan/upgrade/risk/refund90%/reload tested. Reward result RED1/2 (reward0 FAIL,migration PASS) → career-progression-reward-green1 Edit28/28PASS,source1.136identik. Finance RED0/1 cash null→career-finance-green1 Edit29/29PASS;existing1.137unchanged+1meta menjadi1.138. Ledger/split prize/no duplicate/coach/free route tested. UI finance RED0/1 tab Keuangan width0→career-finance-ui-green1 Play91/91PASS,source1.138identik; actual UI prize/coach/cancel/confirm/reload dan bounds tested. Still finance terbaca tanpa overlap/clipping; bukan native/motion. Migrasi pre-finance RED0/1 versi tetap3→career-finance-migration-green1 Edit30/30PASS,source1.138identik; schema4 saat Load, atribut/poin/file lama utuh. Life RED0/1 NegotiateDeal null→career-life-green1 Edit31/31PASS;existing1.139unchanged+1meta menjadi1.140. UI life RED0/1 tab width0→career-life-ui-green1 Play92/92PASS,source1.140identik; still reviewed, notice dekat batas panel. Bonus menang RED0/2 tunggal/ganda expected2transaksi actual0: CompleteMatch hanya memanggil PayCareerPrize yang tidak membaca deal. Hook bonus/profile hasil→career-life-result-green1 Edit33/33PASS source1.140identik; bonus tunggal/ganda dan duplicate/reload tested. Pre-life migration1/1 baseline PASS tanpa perubahan runtime; Cadence RED0/2 accepted shifted schedule→modulo30 validator→career-life-cadence-green1 Edit36/36PASS source1.140identik. Lifecycle1/1 baseline PASS (3payments/expiry/clause/no-debt). Archive statistics RED0/1 Statistics null→career-statistics-green1 Edit38/38PASS;1.141existingunchanged+meta1.142. UI period/page RED0/1 width0→career-statistics-ui-green1 Play93/93PASS source1.142identik; still arsip reviewed tanpa overlap/clipping. Detail/capaian RED0/2 ArchiveDetail null→career-archive-detail-green1 Edit40/40PASS source1.142identik. UI RED0/1 missing Tampilan arsip→career-archive-detail-ui-green1 Play94/94PASS source1.142identik; detail/capaian still reviewed tanpa clipping. Local archive detail/capaian verified, bukan whole-career acceptance. Aging RED1/2 expected349 actual350→career-aging-green1 Edit42/42PASS source1.142identik; recent training/laga melindungi dan respec baseline tidak bypass. UI aging RED0/1 AgingStatus null→95/95PASS source1.142identik/still reviewed. Boundary latihan hari90 RED0/1 expected350 actual349→catat lastTrainingDay sebelum AdvanceDays, restore bila gagal; career-aging-boundary-green1 Edit43/43 dan boundary-ui-green1 UI95/95PASS freeze1.142sama. Aging CLOSED_LOCAL_VERIFIED, bukan balancing/whole-career acceptance. Footwork career RED0/1 method missing; green1 FAIL target fixture receiving bot sama posisi awal (actual0), diperbaiki target+guard saja→career-footwork-green2 UI96/96PASS source1.142identik. Gerak human/3bot sesuai stat, difficulty tidak mengubah stat, reload dan exhibition baseline terjaga. Legacy migration sudah PASS tanpa menulis ulang slot; bukan full career.

### Kapasitas bot career memakai stat pemain — CLOSED_LOCAL_VERIFIED

`career-athlete-stamina-red1.xml`0/1: tidak ada kapasitas per atlet. `CourtCareer` memetakan stat empat atlet; `CourtPrototype` mengisi/recover sekali per game; fatigue bot memakai kapasitas sendiri. `CourtReplay`/`CourtDoubles` memvalidasi batas per atlet sebelum mutasi; snapshot lama tanpa array tetap memakai kapasitas bersama lamanya. `career-athlete-stamina-green1.xml`1/1PASS dan `career-athlete-stamina-regression-{edit,ui}.xml`43/43 +97/97PASS,failed/skipped/inconclusive0,satu freeze1.142identik. Tidak mengubah biaya/tempo exhibition. Backup `.backups/career-athlete-stamina/`.

### Power career tidak mencapai flight — CLOSED_LOCAL_VERIFIED

Power RED1 fixture duplicate Teruskan→RED2 0/1 human Smash stat50/90 sama13,1221256m/s. `AthleteShotDuration` sekarang menyesuaikan durasi sebelum solve untuk Smash/Clear/Lift empat atlet, bukan multiply velocity. Green1 FAIL marker landing beda0,166777m karena existing ground clamp memakai akhir fixed-step sesudah menembus lantai. `CareerGroundContact` menginterpolasi potong y0,04 pada marker dan flight career saja; exhibition ditahan. Green2 1/1PASS, `career-power-regression-{edit,ui}`43/43 +98/98PASS,satu freeze1.142identik; actual marker/ground <0,002m, Net/grade/exhibition tetap. Backup `.backups/career-power/`; mapping provisional.

### Technique/presisi career belum memengaruhi kontak — CLOSED_LOCAL_VERIFIED

RED `career-technique-red1`0/1: skill40/100 biaya sama1. Runtime menghubungkan biaya per-skill, bounded input window dan snapshot opsional `intentWindow` dengan legacy0 memakai0,16detik. Green1 fixture stat100/cap Balanced tidak persistable; fixture memilih Fast Net, validasi cap tidak diubah. `career-technique-green2`1/1PASS termasuk biaya actual player + tiga bot, reach tetap terbatas dan invalid snapshot atomik. RED `career-precision-red1`0/1 titik jatuh skill rendah/tinggi sama; deviasi deterministik dari geometri kontak kini mengecil sesuai core/subskill, termasuk Backhand/Serve, maksimum0,35m sebelum faktor skill. Grade/servis0/exhibition tetap. Regresi `career-technique-regression-{edit,ui}`44/44+100/100PASS,failed/skipped/inconclusive0,satu freeze1.142identik. Backup `.backups/career-technique/`; full Play/motion/career balancing belum.

### Balance hanya stat tanpa runtime — FOCUSED_VERIFIED_REGRESSION_PENDING

RED `career-balance-red1`0/1 recovery tidak ada. Recovery per-atlet/contact/landing kini mengurangi kecepatan sementara dan memberi postur grounded; snapshot/replay memvalidasi empat timer finite0..0,8 sebelum mutasi, legacy tanpa field nol. Green1 fixture membandingkan snapshot pra-restore dengan current normalized displacement; koreksi pembanding current state tanpa melemahkan runtime. Green2 1/1PASS. RED landing0/1: overlay grounded diterapkan airborne sehingga hip0,96→0,84; batas grounded ditambahkan pada live/replay. `career-balance-landing-green1`2/2PASS,1.142file identik,zero failed/skipped/inconclusive. Regresi gabungan sedang berjalan; belum closure final. Affected CourtCareer/Prototype/Opponent/Doubles/Replay/Motion/Ui/AthleteVisual/MobileUiTests; backup `.backups/career-balance/`. Reach/grade/biaya/exhibition tidak dituning.

### Clock harness natural tertimpa UI — CLOSED_LOCAL_VERIFIED

`current-regression1-play.xml`143/144PASS; natural-ganda timeout600000ms. Test mengatur Time.timeScale15 sebelum Mulai; CourtUi.EnterGameScreen secara benar meresetnya1. RED `harness-clock-red1.xml`0/1 expected15/actual1. Koreksi hanya MobileMatchTests.cs: clock sesudah Mulai. `current-regression2-play.xml`144/145PASS,clock-test PASS dan tiga natural MatchEnd tercatat di natural-doubles8.jsonl. Satu kegagalan tersisa gate Easy1069,29053s<1000; defect tempo terpisah tetap OPEN. Backup `.backups/natural-harness-clock/`; freeze1.124file identik. Tidak ada tuning runtime atau pelonggaran batas.

### Home menyimpan pose/posisi pertandingan — CLOSED_LOCAL_VERIFIED

Natural2 frame527s menunjukkan pose selebrasi dan sisi akhir tertinggal pada Home. Menu menghapus Match sehingga update pose berhenti, tetapi sebelumnya hanya mengembalikan kamera. RED `home-preview-red1.xml`0/1: Player z4 masih swapped,expected sisi dekat. CourtUi kini reset posisi/arah kedua/empat atlet, Ready/footplant, shuttle/shadow/indikator dan kamera; pilihan discipline tetap. Backup `.backups/home-athlete-preview-reset/`; `home-preview-green1.xml`95/95PASS,failed/skipped/inconclusive0,source1.124file identik. Empat PNG tunggal/ganda × result/abandon; ganda-result full diperiksa tanpa pose selebrasi/result tertinggal. Bukan bukti motion/audio atau final whole-game regression.

- `doubles-play-green1.xml`:49/69 PASS,20FAIL. `JsonUtility` membentuk objek kosong untuk reference `doubles` pada save tunggal; validasi null menolak save sah. Guard kini memeriksa extra pose pada tunggal, validasi detail ganda tetap. `doubles-probe1.xml`:dua regresi tunggal lulus,partner masih gagal.
- Partner menjauh dari shuttle selama reaction delay karena target default formasi. Probe tick0–9 membuktikan x1,20 turun ke0,32 sebelum ownership dialihkan. Ownership kini dipilih dari trajectory teramati, gerak penerima ditahan selama reaction delay; tidak teleport atau memperlebar reach. Fixture juga menyinkronkan lastPlayer dengan lastHitter.
- Audit audio lama membaca dictionary/soundSource, sedangkan rekaman crowd sekarang channel terpisah. Harness diarahkan ke crowdSource dan memeriksa clip/output; bukan menambah sorakan duplikat.
- `doubles-play-green2.xml`:**69/69 PASS**,failed/skipped/inconclusive0; termasuk restore tunggal,partner return,empat model/UI/save dan output audio. Bukan full ganda/natural-motion acceptance. Servis empat ID tercakup EditMode `doubles-service-green1.xml`; rotasi/hint/replay/natural match masih berjalan.

### Natural ganda — OPEN

`doubles-natural1/2`:Easy reli tanpa poin1000s. Harness awal tidak menggerakkan human bila bukan owner (diperbaiki tanpa membuka reach); trace natural2 tetap membuktikan bot hanya clear hampir selalu ke tengah. `CourtDoubles` kini memilih teknik dan lane dari posisi defender yang teramati. Natural3:11poin tetapi MatchEnd gagal1000s.

Ganda juga tidak memakai fatigueSpeed/cost bot, sedangkan human/singles memakainya. RED `doubles-fatigue-red1` membuktikan per-atlet stamina absen. State empat energi/cost/rally-drain/recovery/migration/validation ditambah; test fatigue/save dan48tes lainnya PASS di `doubles-natural4`, natural tetap gagal1000s meski19poin/semua empat atlet membalas. Belum dinyatakan selesai. Diagnosis5 mempertahankan assertion1000s dan mengumpulkan ketiga difficulty lebih lama untuk membedakan livelock dari pacing; bukan melonggarkan acceptance menjadi PASS.

### Smash bot — CLOSED_LOCAL_VERIFIED; tempo keseluruhan tetap OPEN

Fixture smash pertama tidak menyentuh shuttle; y2,55m dikoreksi ke2,10m dan guard CanReach=true. `doubles-smash-red2.xml` kemudian gagal untuk sebab produk: initialVy=+1,71708608m/s, bukan smash turun. Flight bot0,85s menjadi0,60s; descending/reach test PASS di `doubles-natural6.xml`. Natural6 selesai679,52/396,31/197,35s, tetapi Hard partner1return saja. Harness human kini mengejar hanya saat nearOwner0; ini memberi ruang partner tanpa mengubah input produk atau reach.

`doubles-natural7.xml`:73/74PASS; semua ID >1return dan serviceFaults/netFaults0 pada tiga difficulty; Easy1003,42s masih melebihi1000s, Normal414,41s,Hard257,62s. Batas tidak direvisi. Tiga perubahan produk untuk tempo (variasi teknik, fatigue, lintasan smash) belum menutup gate penuh; slice kalibrasi ditahan untuk evaluasi, pekerjaan UI/presentation independen dilanjutkan. Belum feature-complete atau APK baru.

### Kamera ganti sisi/Home — CLOSED_LOCAL_VERIFIED

Capture `doubles-ui-before`10 layar actual runtime1/1PASS menunjukkan hasil terkontrol dan Home-return terhalang bidang/tulisan besar. `camera-red1.xml`0/1 membuktikan `HallBackWall` min.z13,9 berada di depan kamera z14,2 saat ganti sisi; papan skor/stripe/nama venue juga di antara kamera dan court. Dinding dipindah z16, papan skor z15,2 dan dekor z15,8+; Menu mengembalikan pose kamera Home. `camera-green1.xml`7/7PASS,failed/skipped/inconclusive0 termasuk clearance bounds, menu pose, swapped-end gameplay dan arena/scoreboard. Source freeze `camera-green1-source.json`. `team-ui-green1.xml`14/14PASS termasuk recapture10 layar source-identik; inspeksi kolase terbaru menegaskan objek besar hilang dari result/Home, kamera memulihkan framing awal. Natural-motion tetap terpisah.

### Label ganda — CLOSED_LOCAL_VERIFIED

`team-ui-red1.xml`0/1 membuktikan HUD/result belum punya label pasangan. `CourtUi.TeamName` dipakai setup,HUD,warmup,result; nama tim dua baris terpisah dari difficulty, hasil membedakan game dari poin. `team-ui-green1.xml`14/14PASS termasuk UI controls/settings dan10screen recapture; source identik. Inspeksi kolase dan setup penuh: tidak ada clipping/overlap tim, Dimas/Fajar tampil, kamera result/Home pulih. Preview setup kemudian terkonfirmasi RED `discipline-preview-red1.xml`0/1: cache arena hanya memakai skor, ConfigureDiscipline hanya menyetel flag. ConfigureDiscipline kini memakai SetExtraAthletes existing; cache arena memasukkan MatchTitle saat belum match, marker posisi tidak muncul di menu. `discipline-preview-green1.xml`26/26PASS,failed/skipped/inconclusive0,10 PNG source-identik. Inspeksi setup full memperlihatkan empat atlet, papan GANDA PUTRA, tanpa clipping/overlap kontrol. Papan jauh kecil/kontras rendah belum ditutup sebagai kualitas. Tidak mengubah scoring atau gate natural1000s.

### Tombol review menutup atlet — CLOSED_LOCAL_VERIFIED

Capture natural1× `doubles-continuous1` pada250s memperlihatkan Challenge menutup kepala/badan atlet jauh. Akar sebab UiActionRect memakai x=canvasWidth/2-130,y204,width260. RED `challenge-hud-red1.xml`0/1 x510<956 pada1280canvas. Satu baris runtime memindahkan rect ke x=w-324,y112,width300,height64, kolom kanan di bawah Jeda. `challenge-hud-green1.xml`60/60PASS,failed/skipped/inconclusive0,source1.124file identik;2PNG actual runtime normal/swapped. Swapped diinspeksi: tombol utuh, empat atlet/net/lapangan bebas tombol, kuota berkurang sekali melalui pointer. Tidak mengubah tempo/rules atau mengesahkan input native.

### Papan fisik terlalu kecil dari kamera match — CLOSED_LOCAL_GEOMETRY_AND_STILL

Papan lama masih sulit dibaca pada natural capture, walau tes occupancy dunia lulus. RED `scoreboard-view-red1.xml`0/1 mengukur seluruh tiga baris29,1086px pada viewport471px. Akar sebab teks0,07worldunit dilihat dari sekitar29,5m, gate lama hanya mengukur proporsi terhadap panel. `CourtArena` memperbesar panel5,2×2,1m menjadi7,8×3,4m dan teks0,07→0,12; reciprocal parent scale disesuaikan, kamera/rules/skor tetap. `scoreboard-view-green1.xml`21/21PASS,failed/skipped/inconclusive0; source1.124file identik. Capture `scoreboard-readable-after1.png` full diperiksa: JUST SMASH/GAME1 0:0/14 13 dapat dibaca, pemain/net tidak terhalang; poin belum sekuat HUD, bukan acceptance motion/perangkat.

### Reaksi arena hilang pada poin penutup game — CLOSED_LOCAL_EVENT_VERIFIED

`arena-game-reaction-red1.xml`0/1 membuka applause lama terbawa rematch; fixture kemudian memisahkan applause setup agar gejala penutup game diukur sendiri. `arena-game-reaction-red2.xml`0/1: remaining -1,033328s,expected>1 setelah real ground landing menutup game dan Points reset0–0. Penyebab `UpdateArena` mensyaratkan Points nonnol, tidak membedakan match baru. Cache referensi match kini menghapus applause lama; perubahan score key dalam match yang sama memicu reaksi meskipun poin0–0. `arena-game-reaction-green1.xml`70/70PASS,failed/skipped/inconclusive0,source1.124file identik. Mencakup tunggal/ganda, umpire signal, crowd audio event, pause, rematch serta Feedback/Motion/LocalGate.2PNG fixture actual runtime; swapped ganda empat atlet/net bebas panel. Still tidak membuktikan reaksi natural atlet, tepuk tangan penonton atau listening. Tidak mengubah balancing/rules.

### Reaksi atlet dan penimpaan pose saat pause — CLOSED_LOCAL_STATE_VERIFIED

RED `athlete-point-red1.xml`0/1: tangan pemenang tetap1,16000009m (expected>1,65m). `AthleteVisual.PresentReaction` menambah angkat tangan kiri bagi pemenang dan turunkan tangan/kepala bagi pihak kalah; hanya Point/MatchEnd, bobot memakai pointWait existing. Root, hit timing, stamina, aturan dan tempo tidak diubah. Green1/probe1 menemukan pose tetap bergeser saat pause: hand(0,89;1,90;-3,79) menjadi(0,88;1,92;-3,86). Legacy AnimationState tetap enabled sesudah manual Sample, sehingga pass otomatis menimpa hasil IK meskipun waktu berhenti. Selected state kini disabled segera sesudah Sample; sampling manual tetap dipakai. Green2 motion7/7 lulus tetapi fixture save ganda invalid karena lastHitter diubah tanpa lastPlayer; fixture disinkronkan, validator tidak dilonggarkan.

`athlete-point-green3.xml`92/92PASS,failed/skipped/inconclusive0,source1.124file identik dan4PNG1102×471. Dua capture ganda diperiksa: tim pemenang sama-sama mengangkat lengan, pemain/net/HUD tetap terlihat; reaksi kalah dan grip jauh kurang tegas, bukan bukti kelancaran. Test-only matrix tambahan `athlete-state-matrix1.xml`93/93PASS,source1.124file identik:12 kombinasi tunggal/ganda × winner0/1 × review/game/match, termasuk koreksi pemenang setelah challenge, save/replay, interval dan terminal result. Freeze terbaru `athlete-state-matrix1-source.json`. Full natural video masih snapshot lama; natural-motion/listening/HP tetap belum PASS. Tidak ada APK baru.

## D-053 — gap pengalaman, 2026-11-14

| Temuan | Akar sebab terverifikasi | Solusi/status |
|---|---|---|
| Venue terasa sesi sendiri, bukan pertandingan | BuildCourt hanya hall/lapangan/net/dua atlet; wasit/tribun/penonton/papan skor fisik belum dibuat | PARTIAL; CourtArena menambah48 penonton baked shared mesh, wasit/kursi/hakim servis/papan skor; seat-green1 4/4 dan identity-green1 10/10 PASS. Render diperiksa; motion/audio/whole-match belum diterima |
| Identitas UI lemah | CourtUi memakai heading biasa dan tombol setara; setup hanya difficulty, HUD Kamu/AI | PARTIAL; event/lawan/title konsisten dan10/10 identity PASS; UI ganda/career dan capture akhir belum |
| Sorakan tidak membangun atmosfer | CourtAudio crowd sintetis0,8s; satu trigger saat match-end | PARTIAL; rekaman CC0 applause+room/event-aware/pause ducking terintegrasi; audio-green1 13/13 dan audio output di green2 PASS; kualitas listening belum |

## MOBILE-SOURCE4 — baseline historis, 2026-11-14

Hasil terbaru dan artefak: [TESTING](TESTING.md#mobile-source4--status-aktif-2026-11-14). Snapshot source4 identik setelah regresi/build; APK Mobile packaging PASS. Riwayat gagal tetap disimpan; bukan klaim game bebas bug atau module acceptance selesai.

| Temuan | Penyebab dan bukti | Status terkini |
|---|---|---|
| Tombol Lompat membungkus teks | Lebar92px kurang dari kebutuhan label112,136px. `label-red1.xml` gagal; rect128×80, regression `JumpLabelFitsSingleLineWithPadding` dalam `final-play4.xml` PASS; capture4 diperiksa, satu baris | CLOSED_LOCAL_VERIFIED; ergonomi HP belum |
| Verifier memakai source2 setelah source3 berubah | `verifier-source3-red.txt`: `Inputs changed after regression`. Binding source4 mencakup seluruh Assets/ProjectSettings/Packages, suite4 serta signaturev2. `python evidence/mobile/verify_mobile_apk.py` exit0, PASS_PACKAGING_ONLY | CLOSED_LOCAL_VERIFIED untuk freshness saat build; bukan attestation reproducible-build |
| Capture natural tidak memuat jump | Timeline ketiga match source2: playerY maksimum0. Tes controlled source4 terpisah menutup bukti jump/landing, apex0,6000001m; tidak mengganti bukti natural match | EVIDENCE_GAP_PARTIALLY_COVERED; natural-motion acceptance tetap terbuka |
| Frame review lawan tidak ditemukan oleh filter strokeElapsed | CSV menyimpan strokeElapsed pemain dekat, bukan lawan. Filter awal StopIteration; pemilihan diganti transisi hitter0 ke1 tanpa menamai clock/jenis pukulan lawan | CAPTURE_REVIEW_HARNESS_CORRECTED; bukan bug runtime |
| Rekaman10–12frame/detik, detail grip kecil | Sampling capture terukur sekitar9,97–11,76frame/detik pada1102×472; pose tidak cukup untuk menyahkan gerak mulus atau genggaman | OPEN_ACCEPTANCE_GAP; tidak diasumsikan FPS game rendah |
| Native input/Android/audio belum diterima | Native input pernah ditolak; HP belum diizinkan; decode/PCM/playback muted tidak sama dengan listening | NOT_RUN/PARTIAL, bukan kegagalan produk yang direkayasa |

BuildReport summary0error/0warning tetap disertai diagnostik licensing/D3D12/clang/Symbols/mono pada log penuh. Signer Android Debug validv2, tetapi bukan signing publikasi. Tidak mengubah credential/toolchain, memperluas izin, atau menyebut diagnostik ini telah diperbaiki tanpa bukti. `docs/AI_RULES.md` tidak diedit ulang setelah penolakan.

## PRODUCTION-001 — 2026-11-09 — tidak ada bug runtime baru; 3 kegagalan fixture test diperbaiki — riwayat
- Fix1/3: `Assets/Tests/PlayMode/LocalGateTests.cs` patch pertama merusak struktur (2 assert test `FixedPhysicsIndependentOfRenderGroupingAndSnapshotContinuation` + kurung tutup terhapus) — terdeteksi dari diff, dikembalikan sebelum compile.
- Fix2/3: dua fixture challenge baru memakai skenario award point manual (`JsonUtility.FromJsonOverwrite` Points[0,14] + `AwardPoint(1)`) yang membuat game selesai 15-0 tersembunyi — ekspektasi skor salah. Diganti pola test existing: set Points[1]=14 + landing asli.
- Fix3/3: semantics P-008 salah dibaca fixture: `Match.Review` mengurangi kuota HANYA saat challenge gagal (call confirmed); berhasil (overturned) tidak mengurangi. Fixture dikoreksi, runtime tidak diubah.
- Koreksi metrik motion: `maxAbsX>2` tidak valid (posisi servis x=1.3, rally pendek tidak menuntut lari lebar) diganti total displacement `movedMetres>15`. Bukan bug gameplay.
- Tidak ada perubahan runtime selain penambahan credits UI (CourtPrototype.OnGUI + `creditsOpen`/`CreditsText`) dan `AndroidBuild.RunProduction` (editor-only). Jika regression gagal setelah ini, curigai dua file tersebut dahulu.


- D-047 explicit owner authorization: official free Blender only; one jersey/shorts/court-shoes outfit, isolated outside Assets. No final import, APK or phone action. No additional dependency, paid asset or push.
- Official portable Blender4.5.15 LTS installed under `C:/Users/Dewangga Ramadhan H/AppData/Local/JustSmashTools/blender-4.5.15-windows-x64/`. Official ZIP398,649,826bytes; SHA256 `52dd02fd74877fa08e74b67d7854cf32468a232c02aaa19b6844c91285574f8d` matches official checksum. License/source `blender/tool-provenance.json`; costRp0. Blender license permits commercial artwork, base remains licensed Quaternius CC0.
- Backup `.backups/blender_outfit_20261104_151853/`. Source/runtime Assets/Scripts and Assets/Resources hash-identical to `blender/baseline-inputs.json`; old FBX/outfit retained. Work is `.blend` and scripts/evidence outside Unity Assets, not imported/shipped assets.
- Three modeling attempts executed and retained. Native primitives + voxel union + bounded nearest-surface fitting + barycentric skin weights, no copied Casual geometry. Attempt1 had collapsed fitting faces and footwear detachment; attempt2 normalized imported transforms and removed many degeneracies but remained bulky with shoulder deformation; attempt3 reduced silhouette/bounded fit and removed global shoulder-plane cut, still fails. Two initial script-harness errors (`NoneType world.color`, `.normalized` missing call) fixed before first rendered candidate; logs retained separately, not disguised as mesh evidence.
- Final mesh: Jersey15,616verts/31,070tris,1component,30 nonboundary nonmanifold edges,15 zero-area faces; Shorts8,937verts/17,490tris,1component,7 nonboundary nonmanifold edges,17 zero-area faces. Each shoe610verts/1,216tris,1component,closed topology. All vertex weight sums within.001; finite evaluated vertices across6 poses. These partial passes do not establish correct seam locations/rig mapping/coverage.
- Same six fixed sampled poses rendered front/back/side/diagonal: rest,lunge,overhead,jump,backhand,dive-save (24 actual PNGs per attempt). Final diagonal sheet visually inspected: FAIL shoulder/sleeve deformation (especially backhand/overhead), lunge shorts coverage and primitive court-shoe silhouette. Pose samples are NOT natural gameplay/animation acceptance; root elevation/grounded motion not validated.
- Final CPU baked ray rest coverage: Jersey213/800 misses; Shorts110/286; each shoe340/340. All poses fail coverage; exact metrics in `blender/attempt3/validation.json`. Shoe ray failure versus visible near-foot placement requires further rig/coordinate investigation; do not assert all sampled foot vertices are visibly exposed. Full closed-volume containment and same-side rig mapping audit incomplete.
- Runnable `python evidence/M2/quaternius/blender/check_gate.py` executed exit1; `gate-result.json` correctly rejects candidate. No OUTFIT_CHECK_PASS or integration eligibility. Three model candidates exhausted; no fourth trial. Whole exhibition remains blocked, old runtime regression not re-run because no runtime/Assets/test source changed in this scope.
- Proposed next workflow (NOT approved/executed): manual low-poly retopology/weight-paint by experienced character artist in existing Blender, reusing same base/pose fixtures, rather than fourth procedural generator trial. If cost cannot remainRp0, stop. Alternative asset acquisition requires separate approval/license audit; no automatic substitute.

## QUATERNIUS-REPAIR — BLOCKED_VISUAL_COVERAGE, 2026-11-04
- Owner explicitly authorized consolidated plan execution, including new joined-mesh approach; max 3 failed attempts then independent queue. Historical planning-only wording no longer blocks execution. Visual acceptance still mandatory.
- Backup `.backups/quaternius_repair_20261104_132305/`; baseline images/generator and source hashes in `evidence/M2/quaternius/repair/baseline/` and `baseline-inputs.json`. Gameplay scripts, runtime AthleteNPC, original FBX/license SHA256 unchanged.
- Added runnable welded structural and CPU baked-mesh coverage checks. RED actual: Jersey 3 components/6 loops (expected1/4), Shorts boundary branching, Shoes2 components/4 loops (expected2/0). `checks-red-topology.json`. New joined panels/closed loft passed structural-only checks (`checks-green-structure.json`); this is NOT outfit acceptance.
- Candidate1: generated; inspected front rejects chest clipping and broken footwear. Diagnostic rerun of identical geometry measured bind error <0.000002m, not a new modeling attempt. Candidate2: body-fit projection rejected BEFORE overwrite: Jersey nonManifold37/degenerate60. Candidate3: normal-weighted fitting structurally accepted, but rendered front and coverage rejected. No further modeling fixes attempted. Exact root cause of remaining rendered corruption UNKNOWN; bind-rest displacement alone is insufficient.
- Candidate3 CPU ray coverage failures: Jersey640/912; Shorts286/314; Shoes504/824. Full failed coordinates/predicates and bind errors in `coverage-candidate-3.json`. Pose deformation, full-foot containment, approved boundary location, finite normal/weight/material checks, useful close crops and full view visual acceptance remain incomplete. Do not treat structural-only Run() success as OUTFIT_CHECK_PASS.
- Regression after final Editor-source edits: EditMode10/10; PlayMode98/98; failed/skipped/inconclusive0. XML `repair/final-EditMode.xml` / `repair/final-PlayMode.xml`; machine checkpoint `repair/checkpoint.json`. These verify existing runtime regression, NOT replacement character or natural audiovisual feel.
- `adb devices -l` empty. BLOCKED_DEVICE confirmed. No install, uninstall, clear data, final APK, push or added dependency. Existing APK remains old candidate.
- Three modeling candidates exhausted; stop clothing scope. Integration/release deferred because mandatory visual/coverage gate fails. Independent full regression, runtime-integrity audit and device check completed; current pending queue still includes natural moving stamina/fairness, audiovisual/full-match inspection and final artifact/device QA. No claim of whole-plan completion.
- Proposed process change ONLY (not authorized/installed): Blender garment editing with explicit seam/fit and pose inspection instead of further Unity-generator trials. Requires owner approval for added tool; no paid asset/substitute.

## QUATERNIUS-046 — BLOCKED_VISUAL, 2026-11-04
- D-046 execution started; backup `.backups/quaternius_20261104_125223/`. Gameplay scripts/Resources athlete unchanged; isolated audit assets only. No new dependency, paid asset, push or device data deletion.
- Official Standard ZIP downloaded: 128,968,391 bytes; SHA256 `fdbf1804c90dfc1ea03e992bff7da2dfd1a79318e13270a660180f9308455f40`. Packaged `License_Standard.txt` CC0 verified. Exact free body choices: Superhero male/female only, not Regular/Teen. Source inventory/hashes/PNG dimensions `evidence/M2/quaternius/source-audit.json`.
- Male glTF: 14,318 triangles /8,483 vertices /65 joints /3 materials. Unity FBX:14,318 triangles /8,477 vertices /65 bones per renderer /3 renderer-material slots; FBX/glTF vertex counts differ and are not equated. Native anatomical front is -Z, established by face render and eye/toe landmarks, not container forward. Preview filename `back` currently depicts anatomical front.
- Original ring-built jersey/shorts/shoes generated without Casual mesh/texture copying. Final isolated total15,182 triangles /9,077 vertices /6 renderer-material slots. Counts `outfit-counts.txt`; not accepted/shipped character or APK payload.
- Visual RED initial render. Attempt1 recomputed garment bind poses; fit remained failed. BakeMesh check subsequently confirmed max displacement <0.000002m, excluding a large bind offset in rest pose. Attempt2 removed opposite-winding duplicate surfaces and corrected sleeve winding; shorts front remained culled. Attempt3 accounted for tube traversal direction; shorts front improved but shoulder joins remained open, shorts waist/crotch gap and bare toes remained. Exact root cause of all remaining fit defects not established; manual ring topology does not supply joined shoulder/crotch/closed shoe surfaces. No fourth automatic fix.
- Final inspected `outfit-fix3-front.png`: FAIL shoulder continuity, shorts front gap, both toe coverage. Front/back/side files exist; deformation and gameplay rig integration NOT_RUN. Do not integrate failed candidate outfit. No final-source regression, new APK, natural match/audio or release readiness claim.
- Bundled adb actual `devices -l` returned empty. `BLOCKED_DEVICE`; install/launch/performance/settings preservation/multitouch NOT_RUN. Existing share APK remains old candidate.
- Stop at three automatic fixes per owner rule. Needs approval for changed modeling approach before further outfit fixes; device must be connected for final gate. Remaining plan steps3–9 unfinished, except current provenance/policy/evidence updates. Whole exhibition NOT READY.

## D045-INTEGRATION — 2026-11-04 — PARTIAL, NOT REVIEW READY
- Parent took over stalled NPC subagent. AthleteVisualTests first failed Player NPC visual missing (`D045-visual-red.xml`). Generated Resources/AthleteNPC.prefab from approved source; runtime AthleteVisual attaches skinned model to both gameplay rigs without replacing proxy hierarchy. Original assets retained. Full latest PlayMode98/98 PASS `D045-visual-facing.xml`; no final EditMode/build/device execution.
- Grade Timing/Contact50:50 and thresholds85/65/40 implemented after expected RED; grade does not alter flight. Dive feedback regression found and fixed Fair/Good bound. Fatigue movement multiplier0.7..1 below75% energy implemented after RED; movement drain0 and valid low-energy hits regression pass. Costs/efficiency/30s calibration remain incomplete.
- Render `D045-integrated-court.png` proves model presence ONLY. Inspection reports facing/racket-hand ambiguity and awkward stance/deformation; rotating visual root did not establish visual correctness. Do not mark visual acceptance PASS. Sportswear not modified; long casual pants remain. No current APK contains these changes.
- Owner latest authorizes continue integrated singles and takeover. Backup `.backups/singles_finish_20261104_074043/`. Subagent stop requested to prevent conflicting writes. Continue actual facing/bone calibration, grip alignment, outfit, continuous-motion QA then all remaining gates; no M3/push/new dependency.

## CHALLENGE-WINDOW — 2026-11-04 — VERIFIED_LOCAL
- Added runnable `LocalGateTests.ChallengeNearDeadlineFreezesWindowAndContinuesCorrectedScoreOnce`: actual rally frames/ground landing plus controlled wrong announcement at game point;2.9s window save/restore; successful challenge reverses game award; replay freezes timer; second save/restore then deadline advances exactly once. Corrected points[1,14], games[0,0], quota[2,2], no stale challenge or false GameEnd. All assertions PASS, no runtime fix.
- Full fresh regression `evidence/M2/challenge-window-PlayMode.xml`94/94 PASS + `challenge-window-EditMode.xml`10/10 PASS, failed/skipped/inconclusive0. Controlled wrong-call fixture is not natural randomness/device/feel proof.
- D044 source manifest differs only in FeedbackTests.cs and LocalGateTests.cs. Runtime/assets/settings unchanged; no rebuild, balancing, import, device action or M3. Backup `.backups/challenge_window_20261104_072208/`.
- Combined successful challenge/deadline path closed locally. Failed challenge, no-replay mode and end-to-end natural visual/audio/device acceptance are not established by this case. Human assets/numeric grade-stamina decisions remain pending.

## GAME-WINDOW — 2026-11-03 — VERIFIED_LOCAL
- Added `FeedbackTests.AutomaticPointWindowPreservesGameIntervalAndMatchEnd`: real body-fault scoring at game/match point, save/restore, automatic3s transition, no repeated recovery, no skipped Interval/ChangeEnds/Ready, no new game after MatchEnd. Runtime source unchanged.
- Final full PlayMode93/93 PASS (`evidence/M2/game-window-final-PlayMode.xml`); EditMode10/10 PASS (`game-window-EditMode.xml`, before final test-fixture-only correction). No gameplay fix/build/device action. First run92/93 failed because new final-game fixture set Games=[0,1] but left GameNumber=1; valid snapshot guard rejected it. Corrected fixture GameNumber/flowGame=2, not runtime validation. Failed XML retained as game-window-PlayMode.xml.
- Tested game/match automatic-window gates closed locally. Challenge+automatic-window combination, natural visual/audio, assets, numeric balancing decisions and device acceptance remain open. M3 not started. Backup `.backups/game_window_20261103_224450/`.

## FINAL-RECHECK — 2026-11-03 — CLOSED_LOCAL_VERIFIED
- Fresh regression: `evidence/M2/final-recheck-PlayMode.xml` **91/91 PASS** and `final-recheck-EditMode.xml` **10/10 PASS**, failed/skipped/inconclusive0; both Unity exit0. No gameplay source edits.
- Closed local defects: serve HUD/release mismatch and cancel; joystick origin/persistence; representative both-athlete backhand/contact; opponent body/net fault; capsule sizing; upward torso segments and idle/lunge shoe geometry. Regression fixtures pass; do not reopen these defects without new failing evidence.
- Source/assets/packages/settings match D044-artifact.json before/after tests. Existing `builds/JustSmash-D044.apk` SHA256 `7717930b147f531417750da952a9b19144bb3353e56454900a808d687fcbb605` verified unchanged. No rebuild or phone action.
- Separate unfinished gates: natural animation/visual/audio acceptance, exhaustive gameplay contact matrix, Android/device performance, human asset integration, numeric grade/stamina decisions. These do not mean the closed defects still fail. Whole singles/M2 acceptance incomplete; M3 not started.
- Supersedes contradictory OPEN labels for these resolved defects in historical entries below. Backup `.backups/final_recheck_20261103_220225/`.

## DOC-AUDIT-SERVE — CLOSED_LOCAL_VERIFIED by D-044/FINAL-RECHECK; description below historical
- Temuan read-only CourtPrototype.cs: ReleaseServe power=Clamp01(holdSeconds), HUD/ServeRect power=Clamp01(holdSeconds/0.8). Pada0.8detik HUD100%, power aktual80%. Penyebab: divisor berbeda. Belum diperbaiki; tuning D-041 PROPOSED dan sesi docs-only. Acceptance setelah izin: angka/bar sama dengan power release pada0/0.4/0.8/1/>1detik, cancel tidak memicu servis.

## VISUAL-RIG — 2026-11-03 — improved, not final realism acceptance
- Backup .backups/visual_rig_20261103_173125/. Torso segments now extend upward to next joint; socket spheres/shoulder bridge close gaps. Level shoe geometry and two-bone knee solver replace rigid thigh rotations. Racket head lies along shaft, handle meets hand.
- RED visual-red.xml: spine centre=-0.175m instead of positive. Final visual-final-play.xml89/89 and visual-final-edit.xml10/10 PASS; includes idle/lunge sole height and level-foot checks. Build visual-build.log Succeeded0 errors0 warnings. Current APK builds/JustSmash-M2.apk; no device activity.
- Actual render capture16 frames under evidence/M2/visual-sequence (Smash/Net at0..0.63sec); net-motion.gif is sampled pose sequence, not natural player gameplay video. Inspection Net-1 confirms torso/shoulder and handle/hand connected; yellow landing marker overlaps foot view, so that foot is visually ambiguous. No proof of full natural biomechanics, direction-aware world foot locking, every gameplay-contact reach or full mobile performance. M3 NOT STARTED.

## CONTACT-IK — supersedes backhand/net/mesh defects below, remaining visual gate OPEN
- Existing backhand failure resolved by analytic two-bone contact-at-time-zero pose shared by both athletes. Removed delayed cocking and discontinuous roll phases. Bone lengths fixed; no root teleport and no gameplay contact delay.
- Opponent net lookup/null guard and capsule length corrected with observed RED/GREEN regressions. Final87/87 PlayMode+10/10 EditMode; APK build0 errors0 warnings.
- Torso/shoulder geometry gaps superseded by VISUAL-RIG/FINAL-RECHECK. Remaining QA: natural motion, readable grip/foot planting and complete gameplay reachable-contact matrix. M3 remains blocked by unfinished M2 acceptance.

## MANNEQUIN-AUDIT — HISTORICAL FAILURE, logic defects CLOSED by CONTACT-IK/VISUAL-RIG/FINAL-RECHECK; visual acceptance pending
- Latest executed PlayMode `evidence/M2/mannequin-play4.xml`:83/84 PASS. Backhand actual x=0.239999995; expected x<0.
- Confirmed ineffective retry cause: test invokes Update in same frame as swingAt=Time.time. t=0, k=0, so Lerp(0,targetRoll,k)=0 for every guessed roll angle. Three failed retries exhausted; stop and discuss contact-aligned animation before further fixes.
- Gameplay contact occurs at swingAt but visual contact phase starts0.18s later. Racket/contact alignment not verified.
- Opponent net regression: FixedUpdate passes opponent.Find("Racket") after racket moved into joint hierarchy. Null guard returns false before body test, disabling opponent body/racket net faults.
- Backhand roll discontinuity: cocking ends-120, acceleration starts+60. Capsule native height2 means current joint mesh sizing exceeds intended bone lengths. Foot planting, visual realism and full acceptance remain unverified.
- No new source changes during this audit. M3 must not start with these known defects.

## CLOSURE-011 — laporan konsolidasi aktual

## FEEDBACK-013 — 2026-11-03
- Stamina drain without displacement: FIXED_LOCAL_REGRESSION. RED expected100 vs99.3399811; clamped-distance cost; feedback-final-play.xml79/79. Real-rally balancing pending.
- Landing marker chased shuttle with mismatched prediction: FIXED_LOCAL_LOB_REGRESSION, not full trajectory matrix. Integrator prediction stored at launch/contact; stable marker and <0.06m lob endpoint test passes.
- Point required manual Next: LOCAL_REGRESSION_PASS for automatic3s continuation/pause; challenge/replay/set transition matrix remains open.
- Unrealistic synthetic racket tone: replaced by licensed recordings; clip regression passes. Perceived realism OPEN_OWNER_REVIEW.
- UI crowding, trail readability and primitive whole-leg animation: OPEN; not fixed in current checkpoint.



## APPROVED-012 — new regressions and fixes
- 2026-11-03 | Technique tap waited for release | Button called BeginShot only | Pointer/keyboard now call TapShot; jump swipe unchanged | CourtPrototype.cs, ClosureTests.cs, CourtSmokeTests.cs | FIXED approved-tap-green2.xml71/71. Old input fixture changed intentionally to approved D-035.
- 2026-11-03 | Stat80 saved energy110 rejected on restore | Floating-point/JSON boundary roundoff | Validate within0.001 then clamp to computed capacity; preserve finite/stat checks | CourtPrototype.cs, ClosureTests.cs | FIXED approved-stamina-green2.xml72/72; later strengthened recovery fixture.
- 2026-11-03 | Natural lob ended at2.4078064s after net-touch enabled | Assisted followed its own outgoing shot toward net | Follow incoming opponent flight only during match; preserve training behavior | CourtPrototype.cs | FIXED approved-net-green2.xml74/74 and approved-final-play.xml74/74.
- Body/net faults previously absent; added regression-first. Geometry is placeholder Bounds, not final rig collider/anatomical acceptance.



**Scope belum tertutup penuh.** Jangan menafsirkan regression/build PASS sebagai seluruh pekerjaan teknis lokal selesai. Tidak ada HP install/launch, commit, push, dependency baru atau layanan berbayar.

### Lima gate terpisah
| Gate | Hasil |
|---|---|
| Implementation Complete | **NO** — evaluator multi-komponen dan stat/skill stamina belum diimplementasikan; keputusan rinci belum ada. |
| Local QA Passed | Regression **70/70 PlayMode + 10/10 EditMode PASS**; bukan semua acceptance QA. Visual/animasi/audio, actual render-scheduler matrix, beberapa panel/state capture belum lengkap. |
| Device QA Pending | **YES** — install/open/exit/relaunch, FPS/frame time/RAM/thermal/installed footprint/multitouch/notch belum diuji. |
| Owner Acceptance Pending | **YES** — feel/kualitas audiovisual + keputusan yang dicatat di Decision Register. |
| Milestone Fully Verified | **NO**. Belum siap full M0–M2 UAT; build boleh dipakai review terarah setelah keputusan gate lokal selesai. |

### Jumlah task (52 ID unik, diperiksa dengan Python)
| DONE_VERIFIED | IMPLEMENTED_NEEDS_QA | INCOMPLETE | BLOCKED_OWNER | BLOCKED_ENV |
|---:|---:|---:|---:|---:|
| 49 | 6 | 0 | 5 | 0 |

BLOCKED_OWNER bukan fitur selesai. IMPLEMENTED_NEEDS_QA mencakup QA lokal yang masih bisa dilakukan, bukan semuanya dipindahkan kepada pemilik. Tidak ada bukti environment failure yang membenarkan BLOCKED_ENV.

- Laporan konsolidasi 52 task, counts, bug, test/build/hash, keputusan dan closure queue: `TESTING.md` bagian CLOSURE-011; machine ledger `evidence/M2/closure-task-audit.json`.
- 70 PlayMode +10 EditMode PASS, APK source-bound tersedia; ini **bukan** Implementation Complete/Local QA seluruh acceptance/Milestone Fully Verified. Masih ada keputusan grade/stamina/fault dan QA lokal visual/audio/scheduler.
- Tidak install/launch HP, commit/push, dependency baru atau biaya. Jangan meminta owner tes M1 terpisah. Lanjut queue independen; jangan mengubah balancing tanpa approved decision.

# Just Smash — Error Log

## CLOSURE-011 — bugs dan regression
- Background pause tidak autosave/clear stale pointers: RED method missing, fixed lifecycle autosave+pause+reset input; final67/67 PASS.
- Opponent racket/locomotion missing, safe-area missing: RED2/3 failed; procedural bounded visual dan common canvas transform added; final67/67 PASS.
- IMGUI replay toolbar occluded far athlete; Assisted OFF clipped: actual capture inspected before/after; toolbar kiri dan label diperlebar. Shuttle tetap kecil, natural animation belum diterima.
- Central config red: drag field absent; dimensions red: geometry config property absent; logger red: missing input event. Default-preserving config/logger/HUD completed. Intermediate config regression58/62 failed because4 intentional new scoring/AI logs lacked expectations; exact expectations added, not blanket ignore; next62/62 PASS.
- Verifier awal keliru mensyaratkan no permissions sama sekali: APK memiliki app-defined receiver permission. Harness corrected allowlist exact custom receiver, tetap menolak INTERNET/storage/mic. Bukan permission product fix.
- Local QA gaps dan owner decisions belum selesai: lihat TESTING. Jangan mengklaim full closure.

## CLOSURE-011 — otoritas dan penutupan aktif
Instruksi pemilik terbaru mengizinkan audit dan penutupan teknis lokal seluruh M0–M2, tanpa approval ulang perubahan teknis dalam scope. D-031 menunda review manual; D-034 menyetujui world-space change-ends/player-centric camera serta 3% salah call pada landing eligible dalam 5 cm, maksimum 1/match. D-016 menetapkan joystick/manual+assisted, bukan eksperimen swipe A/B. D-017 menetapkan Official15 saja; Classic21 terkunci sampai career2028, Quick16 dihapus.

Requirement dan acceptance di bawah tetap berlaku kecuali digantikan keputusan APPROVED lebih baru. Hanya status checkpoint lama yang merupakan **riwayat**, bukan status aktif. Pernyataan lama M2 dibekukan, P-006 belum disetujui, Quick16 default, belum ada build, atau lisensi aktivasi terblokir tidak berlaku lagi. Status per task dan closure plan aktif: `evidence/M2/closure-task-audit.json`; hasil baru dicatat di TESTING. Tidak ada izin HP install/launch sebelum pekerjaan lokal selesai, commit/push, dependency baru, biaya atau M3.

Pisahkan Implementation Complete, Local QA Passed, Device QA Pending, Owner Acceptance Pending, dan Milestone Fully Verified. Task canceled M1-09 dianggap requirement superseded terpenuhi, bukan fitur swipe yang diimplementasikan. Device QA pending bukan otomatis BLOCKED_ENV tanpa kegagalan lingkungan konkret.


## REPLAY-AUDIO-010
- Saved spectacle marker absent: RED replay-landing-red.xml expected true got false. Persist marker and capture ground position before automatic replay; regression skip returns y=.04.
- AI serve missing racket event: replay-landing-green.xml failure in actual UpdateOpponent serve entrypoint; hook added. This run imported new test while preceding regression was starting; exact executed totals belong to XML, not launch-time assumption.
- Paused ReleaseShot accepted hit: paused-release-red.xml expected false got true. Guard prevents pause/replay mutation. Full paused-release-green.xml58/58 PASS; EditMode10/10 PASS. Audible/visual/device quality remains unverified.

## STATE-008 — host verification
- Replay clock RED replay-clock-red.xml: charge advanced0.166665792 during replay, expected0 +/-0.04. SkipReplay shifts live charge/jump/AI/serve/swing timers; CaptureMatch uses frozen replayGameAt. replay-clock-green.xml48/48 PASS.
- Contradictory snapshot RED snapshot-relation-red.xml accepted MatchEnd without winner. Restore now rejects state/flight/service/charge/game-count contradictions before mutation. snapshot-relation-green.xml50/50 PASS. Coverage is not exhaustive hostile schema proof.
- Natural lob baseline lob-final.xml49/49 PASS:30.00574 simulated seconds,12 returns,18.95487 energy remaining. No stamina rebalance applied.

## RULES-007
- BUG-017 service area fault ignored. RED service-red.xml; diagonal parity and snapshot plus Let covered service-let-final.xml38/38 PASS. Full final package pending.
- BUG-018 successful review never reversed point/game. RED review-reversal-red.xml expected point1 got0; pre-point state restored before actual winner awarded; save/restore supported. review-final.xml40/40 PASS including corrupt-history rejection. Random wrong calls OFF.


### BUG-012 — Snapshot accepts invalid state and captures replay display
- RED resume-snapshot-red.xml: negative jumpElapsed accepted; replay save expected live (1.65,1.55,-2.60), got playback (1.65,1.07,-3.46). Attempt1 validates elapsed/movement/stroke/challenge fields before mutation and captures liveFrame during replay. Targeted3/3 PASS.

### BUG-013 — Point review lost after restore
- RED resume-review-red.xml expected saved review available true, got false. Attempt1 persists landing/call/review gate; full regression29/29 PASS. Replay trajectory persistence and schema relationships remain incomplete.

### BUG-014 — Paused entrypoints mutate energy/serve/charge
- RED resume-angle-input-red.xml expected100 energy got96. Attempt1 guards Jump/Serve/BeginShot during pause/replay; charge rejected outside Rally. Regression31/31 PASS.

### BUG-015 — Replay additional angle was label only
- RED resume-angle-input-red.xml expected camera abs(x)>5 got0. Attempt1 adds umpire-side replay pass plus gameplay pass and restores camera on skip. Regression31/31 PASS; visual inspection pending.

### BUG-016 — All controlled Smash/Net trajectories collide with net
- Original baseline:90 cases, Smash18/18, Net18/18, Clear2/18, Drop4/18, Lift2/18 net. D-033 approved correction; attempt1 uses stroke-specific distance/height duration and pre-solver power-adjusted duration/depth; no velocity post-multiplier, collision bypass or root movement.
- RED trajectory-red.xml representative Smash lands z=-0.964169145 instead of opponent side. GREEN trajectory-attempt1.xml8/8; final PlayMode34/34. New90-case trajectory-calibration.csv: Smash4/18, Net0/18, Clear0/18, Drop2/18, Lift0/18 net, all finite. Representative Smash/Net cross, unreachable miss still rejected and front-target net remains possible. Symptom FIXED_VERIFIED in controlled host scope; natural balance pending.
- Evidence handling defect: old resume-stroke-calibration.csv overwritten by first reporter rerun before filename isolated; overwritten file relabeled trajectory-reporter-attempt1.csv. Original baseline counts retained in prior logs/TESTING, per-case old CSV unavailable. New reporter uses trajectory-calibration.csv.


### BUG-010 — New motion starts in Braking instead of Idle
- RED `gaps-form-animation-green.xml`: expected Idle, actual Braking. Cause: previousPlayer default zero interpreted spawn displacement as movement. Fix attempt 1 initializes previousPlayer after BuildCourt. Regression 25/25 PASS in gaps-final-play.xml.

### BUG-011 — Procedural lunge has no split stance
- Visual capture legs clustered; RED gaps-lunge-red.xml expected z separation >0.3, actual 0.0. Fix attempt 1 adds smoothed local-only lead/trailing leg offsets. Root/contact logic unchanged; final rerun recorded in TESTING.
- Batch screenshot did not produce tester-ui.png; harness now captures end-of-frame in non-batch Editor. Not a product startup defect.

### BUG-007 — Match player could contact own serve twice
- Host FIXED_VERIFIED. RED `evidence/M2/double-hit-red.xml` returned true; expected false. Attempt 1 rejects repeated reachable player contact, stops flight, awards opponent point; full suite `double-hit-green.xml` 15/15 PASS.

### BUG-008 — Snapshot accepted negative serve timer
- Host FIXED_VERIFIED. RED `evidence/M2/timer-red.xml` expected rejection, none thrown. Attempt 1 validates finite 0..1 serveRemaining before live mutation; `timer-green.xml` 17/17 PASS. Other snapshot schema hardening remains pending; not a claim all hostile snapshots are rejected.

### BUG-009 — No stamina recovery between games
- RED `evidence/M2/recovery-red.xml`: expected energy >60 after game, actual 60. Attempt 1 adds bounded +8 once per new game and persists energyGame. Host regression `recovery-green.xml` 18/18 PASS; recovery FIXED_VERIFIED within host scope. Baseline calibration not proven.


### BUG-006 — Assisted idle analog menggerakkan atlet
- FIXED_VERIFIED untuk host regression. Root cause: `Vector2 move = analog` tetap aktif di luar blok assisted saat shuttle tidak dekat/idle.
- RED `evidence/M1/priority-assisted-red.xml`: expected (0,0,-3.5), actual (0.18,0,-3.5).
- Attempt 1: `Vector2 move = assisted ? Vector2.zero : analog`; aim tetap memakai analog. Final PlayMode 14/14 PASS, termasuk regression aim-only idle. Hardware feel NOT_RUN.
- BUG-005 update: fix retention sudah ada sebelum sesi ini; EditMode retention PASS dan APK baru dibangun, namun Android retest belum tersedia. Status runtime tetap OPEN; jangan klaim startup fixed.


### BUG-005 — Android startup missing shader
|- Status FIXED_VERIFIED, 1 fix attempt. RED: install M1 APK on Xiaomi 13T Android 15; launch failed with ArgumentNullException at CourtPrototype.Mat -> Material.CreateWithShader. Magenta plane; court not initialized.
|- Cause: runtime material creation relied on Shader.Find("Universal Render Pipeline/Lit") and "Sprites/Default", which were absent from build unless retained in m_AlwaysIncludedShaders. Editor tests pass because Shader.Find resolves during editor session, but IL2CPP player build strips unlinked shaders.
|- Attempt 1: static cache `LitShader / SpriteShader` in CourtPrototype with fallback chain (URP Lit → Standard for main materials; Sprites/Default → UI/Default for lines). RuntimeShaders.Retain injects both into GraphicsSettings.asset before build; logs confirm injection. Build exit 0, device run shows process alive, no crash, court/atlet/net visible in screenshot.
|- Evidence: `evidence/M1/shader-device-retest/startup-user.png`, logcat absence of shader-related exceptions, test suites `shader-retest-edit.xml` 10/10 and `shader-retest-play.xml` 18/18 PASS. Acceptance requires user-led visual confirmation on physical hardware.


### BUG-004 — M1 court absent after scene reload
- Observed `play-green.xml`: type exists but instance missing after LoadSceneAsync. Cause: RuntimeInitializeOnLoadMethod runs once, not each scene reload.
- Attempt 1: subscribe sceneLoaded and initialize each Boot load. `play-retry1.xml` 2/2 PASS, `serve-green.xml` 3/3 PASS. Status FIXED_VERIFIED for reload regression.


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

### BUG-003 — Build helper recreates Boot
- Status FIXED_VERIFIED for scene preservation. Review found AndroidBuild.Run unconditionally created/saved default Boot each run, risking future scene edits.
- Fix: create only when absent; normal build leaves existing scene intact.
- Regression: SHA-256 Boot before/after actual Android rebuild identical, evidence/M0/boot-before-build.sha256, M0-BOOT-003.
- No user gameplay scene loss observed.

### BUG-002 — APK development meminta INTERNET
- Status FIXED_VERIFIED untuk permission packaging; runtime belum diuji.
- APK awal 19.998.278 bytes meminta android.permission.INTERNET walaupun forceInternetPermission=false.
- RED: VerifyApk.py AssertionError `Offline APK still requests INTERNET`.
- Percobaan 1: custom manifest menghapus INTERNET lewat tools:node=remove; rebuild sukses, verifier PASS.
- Bukti: TESTING M0-BUILD-002, apk-verification.json. Ini bukan klaim telemetry/runtime audit tuntas.

### BUG-001 — Unity Editor belum aktif
- Status: FIXED_VERIFIED pada retry createProject dan build Android M0-BUILD-002. Aktivasi kini diterima Editor; bukan bug gameplay.
- Scope: M0-03/06/17, T-01, run M0-AUDIT-001.
- Repro: jalankan Unity 6000.3.18f1 dengan `-batchmode -quit -createProject` untuk folder Game.
- Expected: project Unity dibuat. Actual: proses berhenti, return code 198.
- Error: `No valid Unity Editor license found. Please activate your license.`
- Bukti: `evidence/M0/create-project.log`, baris 84–95; entitlements kosong dan access token tidak tersedia.
- Penyebab teramati: Editor tidak menemukan lisensi valid. Status akun/eligibility pemilik belum diverifikasi.
- Percobaan: 1 eksekusi awal; tidak ada auto-fix/bypass. Pemilik perlu aktivasi lisensi melalui Unity Hub.
- Solusi/regression: belum; setelah aktivasi, ulangi createProject dan build nyata.
