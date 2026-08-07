# Skip Pre-Solved Light Frames

## Why

XFM's normal input is raw captures that have never been plate-solved, so a checked browse solving
every light frame is right for the first pass — but the current spec mandates re-solving frames that
already carry a solution, which makes every re-browse of an already-processed library pay the full
solver cost again (1–60 s per frame) for values that cannot change. The sky solution of a captured
frame is immutable; re-measuring it is pure waste.

## What Changes

- During a checked browse, a light frame already carrying a **full measured WCS solution** is
  skipped: no solver process runs and no solution keywords are stamped for that frame.
- The "already solved" test is presence of all 11 unconditional solve-only keywords
  (`CTYPE1`/`CTYPE2`, `EQUINOX`, `CRVAL1`/`CRVAL2`, `CRPIX1`/`CRPIX2`, `CD1_1`..`CD2_2`) —
  provenance-agnostic (any tool's solution counts; no ASTAP-marker requirement). `CROTA1`/`CROTA2`
  are excluded from the test because they are conditionally stamped. `RA`/`DEC`/`OBJCTROT` cannot
  discriminate — raw captures carry them as planned values.
- A partial set (any of the 11 missing) fails the test and re-solves — self-healing, no error.
- No force-re-solve path: skip is unconditional when the set is present.
- Skips are counted and reported alongside solved/failed in the browse status line and log.
- The check exists only inside the solver-enabled path; an unchecked browse remains untouched by
  the solver in every way, exactly as today.
- **BREAKING** (spec-level): inverts the existing requirement clause "re-solving frames that
  already carry a solution — measured always replaces planned" for fully-solved frames. Planned-only
  frames (no WCS set) still solve, so measured still replaces planned.

## Capabilities

### New Capabilities

(none)

### Modified Capabilities

- `plate-solve-stamp`: the checkbox-gated solving requirement changes from "re-solve every light
  frame" to "solve every light frame that does not already carry a full measured WCS solution;
  skip and count the rest." Reporting requirement gains the skipped count.

## Impact

- `XisfFileManager/MainForm/MainForm.cs` — read-pass solve branch (`ReadHeadersAsync`): add the
  presence check before `SolveAsync`, track/report the skipped count.
- `XisfFileManager/Keyword/KeywordList.cs` — likely home of the "has full WCS solution" presence
  helper next to `SetPlateSolution` (the stamped-set definition stays in one file).
- `AstapSolver.cs` unchanged. No settings, no schema, no new UI.
