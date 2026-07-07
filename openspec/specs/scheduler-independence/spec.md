# scheduler-independence

XFM operates with zero Target Scheduler coupling — no TS UI, no `scheduler.db`/SQLite access, no
TS-related dependencies or test data. Established by the `remove-target-scheduler` change
(2026-07-07, v1.9.0); guards against reintroduction.

## Requirements

### Requirement: No Target Scheduler code or UI
XFM SHALL contain no Target Scheduler functionality: no Target Scheduler tab or controls, no
`TargetScheduler` namespace/folder, no TS table models or mappers, and no code path that opens
`scheduler.db`/`schedulerdb.sqlite` or any other Target Scheduler database.

#### Scenario: TS tab absent from the UI
- **WHEN** the application launches
- **THEN** the main tab control contains no "Target Scheduler" tab and all remaining tabs
  (Keywords, Calibration) select and render normally

#### Scenario: No TS symbols in source
- **WHEN** the source tree is searched for `TargetScheduler`, `SqlLite`, `schedulerdb`,
  `eProjectPriority`, or `CustomTreeView`
- **THEN** no matches exist in `XisfFileManager/` source code (Calibration-tab
  `TargetFileTree` names and documentation references are exempt)

### Requirement: No SQLite dependency
The project SHALL NOT reference `Microsoft.Data.Sqlite` (or any other SQLite package), and the
build SHALL succeed without it.

#### Scenario: Package removed and build clean
- **WHEN** `dotnet build XisfFileManager.sln -c Release` runs after the removal
- **THEN** the build succeeds with 0 warnings/0 errors and the csproj contains no SQLite
  PackageReference

### Requirement: No TS test data
The repository SHALL NOT carry Target Scheduler sample data.

#### Scenario: TestData cleaned
- **WHEN** `TestData/` is listed
- **THEN** `schedulerdb.sqlite` is absent (non-TS samples such as the XISF file remain)

### Requirement: Documentation reflects the boundary
Repo reference docs and the portfolio parent map SHALL state that Target Scheduler management is
TSM-only and SHALL NOT list XFM as a consumer of `scheduler.db` or `Catalog.db`.

#### Scenario: Repo docs updated
- **WHEN** CLAUDE.md, ARCHITECTURE.md, DOMAIN.md, VERIFICATION.md, and README.md are read
- **THEN** none describe a Target Scheduler tab, TS database access, or planned Catalog.db
  consumption by XFM

#### Scenario: Portfolio map updated
- **WHEN** the parent `Astronomy/CLAUDE.md` data-flow hub section is read
- **THEN** `scheduler.db` lists TSM (and the TS plugin) only — XFM is not a consumer
