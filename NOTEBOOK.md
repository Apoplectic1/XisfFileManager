# Notebook

> **Charter:** Running lab notebook — chronological, empirical findings made while doing the work
> (measurements, surprises, one-off diagnostics). Newest entries at the top. Substantial standalone
> records (investigations, decisions, reviews) go to `docs/YYYY-MM-DD-<slug>.md` instead; findings
> that harden into standing truth graduate to `ARCHITECTURE.md` / `DOMAIN.md`.

## 2026-08-08 — double-save corruption: stale geometry cache, arithmetic-exact diagnosis

"Bad checksum" reports on 58 Witch Head P1of4 files (browse log) unraveled into a writer bug
present in every XFM generation: `UpdateFileAsync` copies the image block from cached
`TargetAttachmentStart/Length` — set at browse, refreshed after hygiene, **never after a save**.
The *second* save of a file in one session (header length changed between: solve keywords added,
target rename) copies from the stale offset: output = N bytes of prior-header XML tail before the
block + block truncated by N. Fingerprint that cracked it: garbage length == truncation length ==
(new offset − stale offset), byte-exact on every file (Clamshell: 8244−7020 = 1224, with the
server's healthy lz4hc copy at hlen 7005 and hygiene's `lz4hc`→`zstd` patch shrinking it by
exactly 1). Two incidents, 66 files: 2026-06-08 Witch Head (58, pre-refactor writer, damage had
synced to SKYHAWKSERVER), 2026-08-08 Clamshell P2of6 (8, v2.4.0 — every file written that
session). 20,329 other library files swept clean — single-save sessions are always correct,
which is why the bug hid for years; the AL verify/hygiene pass was the first code to ever check
the bytes. Recovery same day: Clamshell restored losslessly from the server (sync was pending —
analysis-only run had not pushed the damage); Witch Head padding-repaired (partial zlib inflate,
missing high-byte-plane tail rebuilt from the row above — ~527 px on lights, up to ~8 k px on
noisy 30–60 s Stars frames, bottom rows; user-accepted). Fix shipped as
`fix-double-save-corruption` (geometry refresh + fail-fast copy gate + on-disk skip check).

## 2026-08-07 — astap_cli.exe cannot read XISF at all; only astap.exe can

Found during the browse-compression-hygiene apply spot-check: `astap_cli.exe`'s image loader
dispatches purely on extension — FITS/TIFF/PNM/PNG — and `.xisf` falls straight to failure
("Error reading image file"; `command-line_version/unit_command_line_general.pas` `load_image`,
confirmed empirically against both a surgical temp uncompressed XISF and the uncompressed
TestData file). The uncompressed-XISF reader (`unit_xisf.pas`) is compiled **only into the GUI
binary `astap.exe`**, which accepts the same headless switches/errorlevels — how NINA drives it.
Consequences: (a) the solve feature's in-place uncompressed path shipped 2026-08-06 had *never*
worked — it just never ran, because every library file is compressed and went temp-FITS; (b) XFM
now drives `astap.exe` (`XisfConstants.AstapPath`); (c) `astap.exe` solved a surgically
uncompressed library light (PLTSOLVD=T, CRVAL ≈ header hints) — leading XML comment and
`<Metadata>` are tolerated (its parser string-searches the header), so the surgical temp needs
no header stripping.

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
