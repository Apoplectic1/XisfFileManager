# Notebook

> **Charter:** Running lab notebook — chronological, empirical findings made while doing the work
> (measurements, surprises, one-off diagnostics). Newest entries at the top. Substantial standalone
> records (investigations, decisions, reviews) go to `docs/YYYY-MM-DD-<slug>.md` instead; findings
> that harden into standing truth graduate to `ARCHITECTURE.md` / `DOMAIN.md`.

## 2026-07-07 — findings from the docs fan-out audit

- **TS database access is read-only in code** — the only SQL that executes is `SELECT * FROM {table}`
  (`SqlLiteReader.cs:45`); `SqlLiteWriter.WriteDatabaseFile` is uncalled `your_table` boilerplate and
  `SqlLiteUpdater` is an empty stub. Docs (and portfolio map) had claimed graded-count write-back →
  now the ROADMAP "TS graded-count write-back" follow-up (filed as #12; renumbered twice same day
  as neighbors closed — find it by name, not number).
- **`RemoveKeyword` matching is case-sensitive** (`KeywordList.cs:137`, plain `Equals`) — keyword-name
  casing must match what setters write (e.g. `"CREJECT"`, `KeywordList.cs:331`).
- **`RiccardiReductionFactor = 0.75` is dead code** — each telescope hardcodes its reduced focal
  length; APM107's 700→531 is ~0.759x, not 0.75x.
- Audit stats: 42 confirmed flags over 6 rounds / 27 agents; rounds were still yielding ~2 new
  flags at the cap, so coverage is good but not provably dry.

*(Notebook started 2026-07-07 at docs-architecture setup. Prior investigation records live in
`docs/` — see the dated notes there.)*
