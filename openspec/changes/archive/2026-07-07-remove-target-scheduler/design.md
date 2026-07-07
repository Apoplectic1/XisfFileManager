# Remove Target Scheduler functionality — Design

## Context

XFM's Target Scheduler tab is a read-only viewer over the N.I.N.A. Target Scheduler SQLite
database (hardcoded UNC path to BIRDWATCHER). The 2026-07-07 docs audit established the code is
`SELECT`-only with stub writer/updater classes; TSM now owns all TS viewing and editing. The TS
surface in XFM is **fully self-contained**: the tab reads the DB and paints TreeViews; its event
handlers only refine those trees. Nothing flows into keywords, `XisfFile`, or `Workspace`, and no
other tab consumes any TS type. Decisions (2026-07-07): XFM will never consume `scheduler.db` or
`Catalog.db`; ship as v1.9.0.

Footprint (mapped in exploration):

```
UI:    TabPage_TargetScheduler (Designer ~104 lines) · TargetScheduler.Tree.cs (233)
       · TargetScheduler.Events.cs (118) · CustomTreeView.cs (87)
       · MainForm.cs ctor wiring (mSchedulerDB, mExposureTreeView, panel juggle ~55-69)
       · tab-switch guard (MainForm.cs:727)
Data:  TargetScheduler/ (SqlLiteManager/Reader/Writer/Updater + Tables/ ×8)
       · Data/ (ITableMapper, TableMappers, SqliteReaderExtensions)
Deps:  Microsoft.Data.Sqlite (csproj) · eProjectPriority (Globals.cs)
       · dead `using ...TargetScheduler.Tables` (Calibration.cs)
Data:  TestData/schedulerdb.sqlite
```

## Goals / Non-Goals

**Goals:**
- Zero TS coupling: no TS UI, types, SQLite dependency, or test data anywhere in XFM.
- Clean deletion per house rule #15 — no migration paths, gates, or compatibility shims.
- Reference docs (repo + portfolio parent) reflect the new boundary; ROADMAP #7/#8 resolved.

**Non-Goals:**
- `BuildTargetFileTree` / `TreeView_CalibrationTab_TargetFileTree` (Calibration tab's
  loaded-files tree) — TS-adjacent name, zero TS involvement; untouched.
- Any future `Astronomy.Catalog` / `Catalog.db` consumption — explicitly ruled out, not deferred.
- `TestData/5b13e2a0-….profile` — NINA profile sample, not TS; verify no TS-only use before
  leaving it in place.

## Decisions

1. **Hand-edit `MainForm.Designer.cs` rather than opening the VS Designer.** The TS controls
   form an isolated cluster (TabPage + 3 TreeViews + button/checkbox/radios/labels). Hand
   removal is deterministic and reviewable in the diff; the VS Designer rewrites unrelated
   serialization noise. Risk is managed by the existing convention that TabIndex values are
   maintained by hand anyway. Alternative (VS Designer) rejected: noisy diffs, risk of
   regenerating unrelated blocks.
2. **Delete `Data/` entirely** rather than keeping it as generic infrastructure. Its three files
   exist solely for TS table mapping; keeping unconsumed "generic" plumbing violates the
   no-dead-code stance. If a future feature needs SQLite mapping, git history holds it.
3. **Remove `Microsoft.Data.Sqlite`** in the same commit as the code (build proves it unused).
4. **Order of operations: code first, Designer last within the same commit** — delete the
   partial classes and data layer, then strip Designer/ctor references, then csproj. Build after
   each stage locally; single commit ships the whole removal (plus doc updates, house rule #4).
5. **Portfolio parent docs edited in the same working session** (the parent `Astronomy/` dir is
   a non-git container, so the edit is just a file write outside this repo's commit).
6. **Release as v1.9.0** — visible feature removal warrants a minor bump over a patch.

## Risks / Trade-offs

- [Designer surgery misses a reference → build break] → compile after the Designer pass;
  CS0103/CS1061 errors name exactly the orphaned handlers/controls.
- [Hidden TS consumer outside the mapped footprint] → post-deletion sweep:
  `grep -ri "scheduler\|sqlite\|eProjectPriority\|CustomTreeView"` over `XisfFileManager/` must
  return only Calibration-tab tree names (`TargetFileTree`) and doc mentions.
- [Tab removal shifts adjacent tab indices/UX] → verify Keywords/Calibration tabs still select
  and render correctly at app launch (feature-correct check, not just build).
- [ROADMAP renumbering goes stale against user notes] → note old→new mapping in the commit and
  conversation per collaboration rule #9.

## Migration Plan

None — house rule #15: target state only. Installed apps receive v1.9.0 via Velopack
auto-update; the tab simply disappears. Rollback = revert the commit / reinstall v1.8.2.

## Open Questions

- None blocking. (`TestData/5b13e2a0-….profile` usage check is folded into tasks.)
