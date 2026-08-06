# Tasks: adopt-diagnostics

_Design skipped (conditional): adoption of an established shared contract; decisions in proposal._

## 1. Adoption

- [x] 1.1 References + sln entries (Diagnostics, Diagnostics.WinForms); Log.Init + StartNewSession in Program.cs
- [x] 1.2 Ctrl+N ProcessCmdKey + GetDiagnosticsContext in MainForm

## 2. Instrumentation

- [x] 2.1 AstapSolver: Info per solve (outcome/PA/duration), Error on failure, Diag("SOLVER") args + ini dump
- [x] 2.2 Read pass: browse bracket lines; Error twins on solver MessageBox paths

## 3. Docs + verify (same commit)

- [x] 3.1 ARCHITECTURE/ROADMAP notes; build clean; field check = user presses Ctrl+N and finds xfm.log populated
