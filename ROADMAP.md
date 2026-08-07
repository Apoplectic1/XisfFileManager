# XISF File Manager — Roadmap

> **Charter:** Living priority list — what's next and what recently shipped. Keep entries short.
> Code structure lives in `ARCHITECTURE.md`; full history lives in git.

## Open follow-ups

1. **Collapse the double focal-ratio derivation** — `TelescopeConfiguration.ApplyKeywords` (TelescopeConfiguration.cs:70) and the `XisfFile.FocalRatio` setter (XisfFile.cs:259) each recompute FOCALLEN ÷ APTDIA, the latter discarding its assigned value; `KeywordList.FocalRatio`'s setter just stores what it's given. Correct but redundant; pick one home if simplifying. (Audited 2026-07-07: previously miscounted as triple.)
2. **APTAREA obstruction accuracy** — `APTAREA` uses full circular area π·r² and ignores obstructions, so the Newtonian (NWT254) value is optimistic. Subtract the secondary-mirror obstruction before trusting it for light-gathering / SNR / throughput math.
3. **Focal-ratio consistency coloring** — if a FOCRATIO readout is added to the Telescope groupbox, mirror the focal-length consistency check in `TelescopeService`/`TelescopeAnalysis` (distinct-ratio detection + red/black label color).
4. **Wire `XisfBlockCompression.Decompress` into a runtime path** (was #5) — the codec is compress-only today because XFM treats image blocks as opaque. Needed when the flux feature gains real pixel read/write; at that point re-evaluate `Astronomy.PCL` for true image decode/encode (the codec here only covers the byte-block layer).
5. **Preserve (then compress) thumbnail/ICC blocks** (was #6) — saves currently strip them unconditionally: `RemoveUnwantedAttachments` deletes Thumbnail/ICCProfile/DisplayFunction elements and non-main attachments are ignored when building the output (XisfFileUpdate.cs:304,368-382). The plan needs a preserve/copy step before compression is even on the table.
6. **Verify input checksums on read** (was #7) — XFM writes SHA-1 checksums but does not validate an existing block's checksum on load; could warn on mismatch (corruption detection).
7. **AL adoption prep (sln membership)** — when `Astronomy.*` `ProjectReference`s land, also add those projects to `XisfFileManager.sln` with config mappings (`Debug|x64`/`Release|x64` ActiveCfg **and** Build.0; Any CPU → Any CPU; x86 rows ActiveCfg-only onto x64). TP lesson 2026-08-02: sln builds *unset* Configuration/Platform for non-member references → silent **Debug** AL DLLs in dev builds. XFM's release path is immune (project-level publish flows Configuration), and `release.ps1`'s conditional AL gate arms itself when `Astronomy.Core.dll` appears in the payload. Verify with `dotnet build XisfFileManager.sln -c Release`: every `Astronomy.* ->` line must say `x64\Release`.

8. **Revisit UPDATE_NEW/FORCE/PROTECT — plan what the keyword-update modes mean (user, 2026-08-06)** —
   properly define the three `eKeywordUpdateMode` semantics and how they interact with feature
   writes, then align call sites. Observations that triggered it (astap-plate-solve planning):
   PROTECT is really a **file-level refusal in the common save path** (`UpdateFileAsync`'s
   centralized gate) and should stay independent of every feature, the solver included; the
   solver's "solved values always win — disk is truth" decision bypasses the UPDATE_NEW-vs-FORCE
   distinction entirely, a hint the per-keyword mode split may not be carrying its weight. Design
   task, not a quick fix.

## Recently shipped

- **Solver failures carry ASTAP's reason (`solver-log-tail`, 2026-08-06)** — `astap_cli` now runs
  with `-log`; on `PLTSOLVD=F` or timeout the tail of ASTAP's own log lands in the `SOLVER` diag
  channel (star counts, search progress). Motivated by the IC 2087 field run: 6/11 "no solution"
  failures with nothing to diagnose them by. Companion AL fix same day: legacy `sha1` checksum
  tokens canonicalized on read (2019-era SGP files had failed 106/106 before ASTAP ever ran).
- **Diagnostics adopted: xfm.log + Ctrl+N (`adopt-diagnostics`, 2026-08-06)** — shared
  `Astronomy.Diagnostics` (+ new `.WinForms` dialog satellite, which TP also consumes now): log
  rotation at startup, `XFM_DIAG` channels, Ctrl+N observation dialog with screenshots + context
  snapshot; solver + Browse read pass instrumented (per-solve Info/Error, gated SOLVER channel with
  args + raw `.ini`). Adopted ahead of debugging the checked-Solver browse issues.
- **ASTAP plate solving in the read pass (`astap-plate-solve`, 2026-08-06)** — Directory Selection
  "Solver" checkbox: checked browses solve every light frame with the local ASTAP CLI (compressed
  frames decode through AL `Astronomy.XISF`, temp-FITS hop; uncompressed solve in place, `-o` temp
  redirect) and stamp measured `RA`/`DEC`/`OBJCTROT` + full WCS via `KeywordList.SetPlateSolution`;
  persistence rides the normal save (solved values always win; PROTECT still refuses). Second AL
  dependency (`Astronomy.Core` — `WcsOrientation`; ASTAP's 180°+parity bridge stays in
  `Solver\AstapSolver`). Field verification pending: one known-field absolute-PA sanity check, then
  a `°(M)` backlog batch → save → TSM rescan → framings leave mechanical fallback.
- **AL block codec adopted; vendored duplicate retired (`adopt-al-xisf-compression`, 2026-08-06)** —
  first AL `ProjectReference` (`Astronomy.XISF`); same zlib+sh+SHA-1 writes; `Parse` now fails fast
  on malformed known-codec attributes; release gate fixed to arm on any `Astronomy.*.dll` and now
  **ARMED** (publish AL before releasing XFM). Two-tier keyword structure + AL property-first
  migration direction documented in `ARCHITECTURE.md`.
- **Released `v2.0.1` (2026-08-02)** — ships the title/MinVer fix below; first release with a
  Velopack delta package (valid baseline: local `Releases\` still held the published 2.0.0).
- **Window title = app name + version; MinVer adopted (2026-08-02)** — title is now
  `XISF File Manager X.Y.Z` (TSM pattern, a portfolio-general rule), replacing the
  date/config/version-label form whose CI-injection heuristic showed "unknown" the moment the
  v2.0.0 tag matched the hand-set AssemblyVersion. MinVer (tag-driven, `-alpha` prereleases on
  untagged commits) replaces the hand-set AssemblyVersion and the script's InformationalVersion
  injection; `GetVersionLabel`/`GetGitBranch` deleted.
- **Released `v2.0.0` (2026-08-02)** — first script-built release (`scripts/release.ps1`): Velopack 1.2.0,
  repo-rename URL fixes, release-flow migration. Origin pruned to `main`-only the same day (default
  branch fixed to `main`; stale `TargetScheduler` + `C++/CLI_for_PCL_Library` remain as local branches).
- **Local release script replaces CI (2026-08-02)** — `scripts/release.ps1` (TSM/TP model: publish →
  `vpk pack` → upload) is now the release path; `.github/workflows/release.yml` deleted. The tag still
  versions the build (injected `InformationalVersion`), and RELEASING.md was rewritten to carry the
  portfolio's general rules (dev never pushes, ff-only `main`, no tag → no push, content rules).
  Velopack NuGet bumped 0.0.1298 → 1.2.0 to match the vpk CLI (update API unchanged; pack-time
  skew warning cleared).
- **Zero-warning ratchet (2026-08-01, portfolio-wide)** — `<TreatWarningsAsErrors>` on the project; it was
  already warning-clean in Debug and Release (verified by forced non-incremental rebuilds before the switch
  went on), so this locks in the existing state. The "0 warnings" bar in VERIFICATION.md is now enforced by
  the compiler rather than by discipline.
- **Target Scheduler functionality removed (closes former follow-ups #7 and #8; v1.9.0)** — TS is TSM-only now, and XFM will **never** consume `scheduler.db` or `Catalog.db` (decided 2026-07-07). Deleted: the TS tab + Designer controls, `TargetScheduler/` + `Data/` folders, `MainForm/TargetScheduler.*` + `CustomTreeView`, `eProjectPriority`, the `Microsoft.Data.Sqlite` package (also clearing its NU1903-vulnerable transitive dep), and `TestData/schedulerdb.sqlite`. `ExpandAllNodes` moved to MainForm.cs (the Calibration target-file tree still uses it). OpenSpec change: `openspec/changes/remove-target-scheduler/`.
- **Seconds header reddens on missing exposure keyword** — the Seconds analysis is now presence-based (`HasExposure`): a file with no EXPTIME/EXPOSURE keyword drives the red header instead of masquerading as a genuine 0-second frame (real 0s bias frames stay valid). Sentinel audit of the other four columns found Gain/Offset/SensorTemp/Binning already correct (-1/-273 sentinels are excluded from valid values).
- **Camera-header colorization for unresolved INSTRUME (closes former follow-up #8)** — the Camera column header now follows the same convention as the value headers: red = any loaded file whose `INSTRUME` is missing, blank, or matches no registered camera model; green = all resolved across 2+ cameras; black = single camera. Purely informational (Set All still leaves unmatched files untouched); supersedes the earlier status-line skipped-count idea (design settled 2026-07-07; background in `docs/2026-06-14-camera-colorization-multicamera-investigation.md` open decision #1).
- **GitHub Actions bumped to `@v5` (closes former follow-up #4)** — `actions/checkout` and `actions/setup-dotnet` off the deprecated Node 20 runtime ahead of the 2026-09-16 removal; runner stays `windows-latest`.
- **Dependency cruft dropped (closes former follow-up #11)** — removed the unused `GeoTimeZone`/`TimeZoneConverter` PackageReferences (timezone code uses built-in `TimeZoneInfo`), deleted the dead packages.config-era `packages/` folder, and pointed csproj `RepositoryUrl` at the GitHub URL instead of a stale local path.
- **PROTECT gate centralized (closes former follow-up #10)** — `UpdateFileAsync` now refuses PROTECT-mode files itself (new `eUpdateOutcome.Protected`), instead of relying on the MainForm save loop; FluxDensity declares `FORCE` for its export copies, matching Calibration's existing pattern.
- **Exposure writes standardized on EXPTIME (closes former follow-up #11)** — `KeywordList.ExposureSeconds` now writes `EXPTIME` (removing any legacy `EXPOSURE`) and reads EXPTIME-first, so Camera Set All exposure edits are no longer silently discarded at save on files that already carry EXPTIME; redundant EXPTIME re-add in Set By File dropped; backwards normalization comments fixed.
- **CREJECT keyword purge** — `CREJECT` (WBPP post-processing group marker) added to `RemoveUnwantedKeywords`, and the calibration-files ClearAll handler removes it outright instead of writing an empty value.
- **Docs-architecture adoption + full doc audit (2026-07-07)** — CLAUDE.md became a thin router; mechanics moved to `ARCHITECTURE.md`, domain context to `DOMAIN.md`; scaffolded `VERIFICATION.md`/`NOTEBOOK.md`/`RELEASING.md`/`README.md`. A 42-flag fan-out audit then corrected doc↔code drift (TS access is read-only; zlib fallback; FOCRATIO gotcha layer) and surfaced follow-ups #10-#13.
- **Released `v1.8.0`, `v1.7.1`, `v1.6.0`** — tag-triggered Velopack releases (dev → main merges).
- **Velopack release pipeline + in-app self-update** — `release.yml` publishes self-contained win-x64, packages with `vpk pack`, uploads installer/update assets to GitHub Releases; the app checks for updates at startup (`MainForm.CheckForUpdatesAsync`) and shows the version in the window title.
- **Multi-camera Set All** — Camera-tab "Set All" no longer bails when more than one camera is checked. One camera checked = unchanged (force identity onto every loaded file); two or more = each file is routed by `INSTRUME` to its matching camera's row and forced per-camera (Z533 files get the Z533 row, Z183 the Z183 row), generalizing to any number of cameras. Files matching no checked camera are left untouched (reporting them is open follow-up #9). Investigation + deferred colorization/validity-color work captured in `docs/2026-06-14-camera-colorization-multicamera-investigation.md`.
- **Tidied redundant `using` directives** — removed explicit usings already provided by `ImplicitUsings` (and an unused `System.Reflection.Metadata`) from `KeywordList.cs`, `MainForm.cs`, `XisfFileUpdate.cs`, and `Xml.cs`, clearing the CS8019/CS8933 IDE noise (closes former follow-up #3).
- **XISF image-block compression (`zlib+sh` + SHA-1)** — saves compress uncompressed blocks to the PixInsight/NINA on-disk format; already-compressed blocks copied verbatim. Pure-managed codec, no native/PCL dependency; "Update Keywords" became save-if-needed. Mechanics: `ARCHITECTURE.md` (Compression).
- **Released `v1.5.0`** — merged `dev` → `main` and tagged; tag-triggered `release.yml` build.
- **MainForm maintainability refactor** — shared session state extracted to `Models/Workspace.cs`; Browse surfaced as a named-stage pipeline; oversized partials split; "Adding a new feature area" convention documented (now in `ARCHITECTURE.md`). MVP/presenters evaluated and rejected — the `Services`/`Models` layer is already the clean separation.
- **Released `v1.4.0`** — merged `dev` → `main` and tagged (`d8d862e`); tag-triggered `release.yml` build.
- **FOCRATIO + aperture keywords for all telescopes** (`6ff0f60`, `f65e770`) — `ApplyKeywords` now emits `APTDIA`/`APTAREA` and derives reducer-aware `FOCRATIO` for APM107, EvoStar150, Newtonian254; aperture diameter/area hardcoded for all three.
- **Branch/release model documented** (`31e96c6`) — dev/main flow and tag-triggered releases (now in `RELEASING.md`).
- **Sensor keyword comments clarified** (`f3cc3ee`).
- **Exposure normalized to `EXPTIME`** (`a7b47d4`) — legacy `EXPOSURE` converted and purged.
