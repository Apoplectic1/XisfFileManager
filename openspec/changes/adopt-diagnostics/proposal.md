# Proposal: adopt-diagnostics

## Why

XFM has zero logging — every error path ends in a MessageBox and evaporates. With code issues
reported in the checked-Solver browse (astap-plate-solve), the portfolio rule is instrument first:
adopt `Astronomy.Diagnostics` (shared log + Ctrl+N observation protocol; TSM and TP already
consume it) plus the new `Astronomy.Diagnostics.WinForms` shared dialog, and instrument the solver
path so the debug session reads a log instead of reproducing blind.

## What Changes

- References: `Astronomy.Diagnostics` + `Astronomy.Diagnostics.WinForms` (csproj + sln entries).
- `Program.cs`: `Log.Init(AppLogIdentity("XisfFileManager", "xfm.log", "XFM_DIAG", DiagDefault))`
  (All in Debug / None in Release, TP's pattern) + `Log.StartNewSession()` rotation.
- `MainForm`: `ProcessCmdKey` Ctrl+N → shared `DiagnosticsDialog.ShowOrFocus`;
  `GetDiagnosticsContext()` snapshot (loaded-file count, Solver/Masters/Recurse checkbox states,
  selected tab, operation status).
- Solver instrumentation: `Log.Info` per solve (file, hinted/blind, outcome, PA, duration),
  `Log.Error` on failures with ASTAP's error text, gated `Log.Diag("SOLVER", …)` carrying full CLI
  args + `.ini` content; browse bracket lines; `Log.Error` twins on solver-path MessageBoxes.

## Capabilities

### New Capabilities

_None — tooling adoption; no XFM feature behavior changes; `skip_specs: true`._

### Modified Capabilities

_None._

## Impact

Payload gains `Astronomy.Diagnostics(.WinForms)` DLLs (gate already generic). Log area:
`%APPDATA%\XisfFileManager\Logs\xfm.log` (+ `screenshots\`). Docs: ARCHITECTURE/ROADMAP.
