# Notebook

> **Charter:** Running lab notebook — chronological, empirical findings made while doing the work
> (measurements, surprises, one-off diagnostics). Newest entries at the top. Substantial standalone
> records (investigations, decisions, reviews) go to `docs/YYYY-MM-DD-<slug>.md` instead; findings
> that harden into standing truth graduate to `ARCHITECTURE.md` / `DOMAIN.md`.

## 2026-08-07 — block-codec benchmark: zstd only wins at level ≥ 15, then it's real

Benchmarked candidate XISF block codecs over the real library (42 stratified files, 2.5 GB raw —
full record + decision: `docs/2026-08-07-compression-benchmark.md`). Surprises worth remembering:
**zstd 1–9 is a size wash vs zlib-9+sh** (±1–4% — shuffled frames leave no long-range structure
for zstd's usual wins), the payoff appears abruptly at the level-15+ strategy switch (**−11.2%
on lights at 19**), and **22 adds nothing over 19**. lz4hc is +6…+43% (out). Decode speed is a
non-factor (all ≥ 0.6 GB/s; higher zstd levels decode *faster* — smaller stream). Decision:
new writes → `zstd+sh(19)`; one-time ~26 GB library recompress deferred to a FORCE-gated pass.

## 2026-07-07 — Camera-tab missing-value sentinel audit

Checked all five value columns for the "missing masquerades as valid" pattern: only **Seconds**
was affected — `ExposureSeconds` returns 0.0 for a missing keyword and the analysis accepted
`v >= 0`, so keyword-less files looked like legitimate 0-second frames (and 0 s *is* legitimate
for bias, so presence must be tested, not the value). Gain/Offset/Binning return -1 and
SensorTemp -273 when missing, all correctly excluded by their analyses.

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
