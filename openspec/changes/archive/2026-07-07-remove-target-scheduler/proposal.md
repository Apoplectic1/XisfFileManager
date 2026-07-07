# Remove Target Scheduler functionality — Proposal

## Why

Target Scheduler viewing/management is now owned entirely by the TargetSchedulerManager (TSM)
app; XFM's read-only TS tab is redundant and will never grow into write-back (2026-07-07 audit
confirmed the write path was never implemented — stubs only). Decision made 2026-07-07: XFM will
**never** consume `scheduler.db` (nor the future `Catalog.db`), so the entire TS surface —
UI tab, data layer, SQLite dependency — is dead weight to remove.

## What Changes

- **BREAKING (UI)**: Remove the "Target Scheduler" tab and all its controls (profile/project/
  target TreeViews, open-database button, active checkbox, priority radios, exposure-plan panel).
- Remove `TargetScheduler/` (SqlLiteManager/Reader/Writer/Updater + 8 table models).
- Remove `Data/` (ITableMapper, TableMappers, SqliteReaderExtensions) — TS-only infrastructure.
- Remove `MainForm/TargetScheduler.Tree.cs`, `MainForm/TargetScheduler.Events.cs`,
  `MainForm/CustomTreeView.cs`, and the MainForm constructor wiring + tab-switch guard.
- Remove the `Microsoft.Data.Sqlite` NuGet package (TS was its only consumer).
- Remove `eProjectPriority` from `Globals.cs` (TS-only enum) and the dead
  `using XisfFileManager.TargetScheduler.Tables;` in `Calibration.cs`.
- Delete `TestData/schedulerdb.sqlite` (TS sample data).
- Update docs: ARCHITECTURE (TS section + directory tree + tech stack), CLAUDE (intro),
  DOMAIN (ecosystem position), VERIFICATION (TS caution), README (feature bullet),
  ROADMAP (#7 and #8 both resolved by this change).
- Update the portfolio parent `../CLAUDE.md` + `../ROADMAP.md` (XFM row and `scheduler.db`
  data-flow hub no longer list XFM as a consumer).
- Release as **v1.9.0** (visible feature removal → minor bump).

Explicitly **not** touched: `BuildTargetFileTree` and `TreeView_CalibrationTab_TargetFileTree`
(Calibration-tab tree of loaded files — TS-adjacent name, no TS involvement).

## Capabilities

### New Capabilities
- `scheduler-independence`: XFM operates with zero Target Scheduler coupling — no TS UI, no
  `scheduler.db`/SQLite access, no TS-related dependencies or test data. This spec defines the
  target state the removal must reach and guards against reintroduction.

### Modified Capabilities
<!-- none — no existing specs in openspec/specs/ -->

## Impact

- **Code**: ~15 files deleted (2 folders + 3 MainForm partials), Designer surgery on
  `MainForm.Designer.cs` (~104 TS-related lines, TabIndex ordering preserved), small edits in
  `MainForm.cs`, `Globals.cs`, `Calibration.cs`, csproj.
- **Dependencies**: `Microsoft.Data.Sqlite` dropped from the csproj.
- **Behavior**: TS tab gone; every other tab unaffected (TS code was fully self-contained —
  nothing flowed into keywords, files, or `Workspace`).
- **Ecosystem**: BIRDWATCHER's `schedulerdb.sqlite` now has TSM as its only manager; portfolio
  map updated accordingly. No back-compat or migration code per house rule (clean rebuild).
