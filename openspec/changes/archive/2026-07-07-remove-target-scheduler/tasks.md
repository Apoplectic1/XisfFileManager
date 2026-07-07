# Remove Target Scheduler functionality — Tasks

## 1. Pre-flight

- [x] 1.1 Confirm clean git tree on `dev` (commit/stash any in-flight work first)
- [x] 1.2 Verify `TestData/5b13e2a0-….profile` has no TS-only consumer (grep for its name/usage); leave it if used elsewhere, delete alongside if TS-only

## 2. Delete the data layer

- [x] 2.1 `git rm -r XisfFileManager/TargetScheduler` (SqlLiteManager/Reader/Writer/Updater + Tables/)
- [x] 2.2 `git rm -r XisfFileManager/Data` (ITableMapper, TableMappers, SqliteReaderExtensions)
- [x] 2.3 Remove dead `using XisfFileManager.TargetScheduler.Tables;` from `Calibration/Calibration.cs`
- [x] 2.4 Remove `eProjectPriority` from `Globals/Globals.cs`

## 3. Delete the UI layer

- [x] 3.1 `git rm` `MainForm/TargetScheduler.Tree.cs`, `MainForm/TargetScheduler.Events.cs`, `MainForm/CustomTreeView.cs`
- [x] 3.2 MainForm.cs: remove `mSchedulerDB` field + construction, `mExposureTreeView` field, the TabPage panel wiring in the constructor (~lines 55–69), and the `TabPage_TargetScheduler` tab-switch guard (~line 727)
- [x] 3.3 MainForm.Designer.cs: remove `TabPage_TargetScheduler` and all `SchedulerTab` controls (TreeViews, button, checkbox, priority radios, labels) — declarations, initialization, layout, event hookups; keep remaining TabIndex ordering coherent
- [x] 3.4 Build after Designer surgery; fix any orphaned references the compiler names

## 4. Dependencies & test data

- [x] 4.1 Remove `Microsoft.Data.Sqlite` PackageReference from the csproj
- [x] 4.2 `git rm TestData/schedulerdb.sqlite`
- [x] 4.3 Post-deletion sweep: `grep -ri "TargetScheduler\|SqlLite\|schedulerdb\|eProjectPriority\|CustomTreeView\|Sqlite"` over `XisfFileManager/` — only Calibration `TargetFileTree` names may remain

## 5. Documentation (same commit as code)

- [x] 5.1 ARCHITECTURE.md: drop the TS integration section, TargetScheduler/+Data/ from the directory tree, SQLite from the tech stack
- [x] 5.2 CLAUDE.md: intro no longer mentions the TS viewer; check doc-map rows and gotchas
- [x] 5.3 DOMAIN.md: ecosystem position → TS is TSM-only; XFM never consumes scheduler.db/Catalog.db
- [x] 5.4 VERIFICATION.md: remove the TS-database caution
- [x] 5.5 README.md: remove the TS feature bullet
- [x] 5.6 ROADMAP.md: close #7 and #8 (both resolved by removal) → Recently shipped entry; renumber remaining follow-ups and note old→new mapping
- [x] 5.7 Parent `../CLAUDE.md` + `../ROADMAP.md`: XFM row and scheduler.db data-flow hub — remove XFM as consumer (file edit outside this repo)

## 6. Verify & ship

- [x] 6.1 `dotnet build XisfFileManager.sln -c Release` — 0 warnings / 0 errors
- [x] 6.2 Launch the app: TS tab gone; Keywords + Calibration tabs select and render; Browse a folder to confirm the Calibration target-file tree still populates (feature-correct check)
- [x] 6.3 Run the spec scenarios in `specs/scheduler-independence/spec.md` as a checklist
- [x] 6.4 Commit code + docs together on `dev`
- [x] 6.5 Release v1.9.0: bump RELEASING.md latest tag, merge `dev`→`main`, annotated tag, push `main`+tag, watch release.yml, confirm Velopack assets
