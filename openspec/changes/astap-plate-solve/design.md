# Design: astap-plate-solve

## Context

See `proposal.md` for motivation and decisions. Mechanics grounded in: XFM's read pass
(`MainForm.ReadHeadersAsync` — per-file loop with progress UI, calls
`XisfXmlReader.ReadXisfFileHeaderKeywords`), NINA's `ASTAPSolver` (invocation + `.ini` parse +
convention bridge — reference cheat-sheet), AL's shipped `XisfImageReader` /
`XisfHeaderReader` / `WcsOrientation`.

## Goals / Non-Goals

**Goals**: solve inline in the read loop (the existing per-file progress UI narrates it); everything
solver-specific in one new area; AL used for all astrometry/XISF-read work (portfolio goal).

**Non-Goals**: no auto-solve policy beyond the checkbox; no master/calibration solving; no
`RADESYS` keyword (we keep removing it; J2000 is carried by `EQUINOX`); no multi-channel or
non-UInt16 solve support in v1 (the library is UInt16 mono — anything else reports as a per-file
solve failure, not a crash); no XFM test project (unchanged — verification is field-level, see
tasks).

## Decisions

1. **New area `Solver\` (`AstapSolver.cs` + `SolveResult`)** — per ARCHITECTURE's "adding a new
   feature area" convention. Static service, UI-free; MainForm owns the checkbox and the loop hook.
2. **Hook point**: inside `ReadHeadersAsync`'s loop, after `ReadXisfFileHeaderKeywords`, gated on
   `CheckBox_Solver.Checked && !Masters_Enable.Checked && mFile.FrameType == eFrame.LIGHT`.
   Failures accumulate; one summary dialog after the loop (per-file MessageBoxes would make a
   50-frame browse unusable). The existing file-name label + progress bar narrate progress.
3. **Solver executable**: `astap_cli.exe` (no GUI, no popups), default install
   `C:\Program Files\astap\astap_cli.exe` as a `XisfConstants` constant (v1: constant, not a
   setting — promote to Properties.Settings only if a second machine ever needs it). Existence
   checked once per checked browse; missing ⇒ the browse fails loudly (spec).
4. **Hints via AL `XisfHeaderReader`** (not XFM's keyword tier): `-ra` = RaDegrees/15, `-spd` =
   DecDegrees+90, `-fov` = FieldHeightDeg, `-r 10`, `-z 0` (auto downsample); hints absent ⇒ blind
   (`-r 180`, `-fov 0`). One header read per file — cheap, and identical semantics for
   compressed/uncompressed.
5. **Input paths**: uncompressed ⇒ `-f <original>`; compressed ⇒ AL `XisfImageReader.ReadImageAsync`
   → minimal temp FITS in `Path.GetTempPath()`. **Always `-o <tempbase>`** so ASTAP's `.ini`/`.wcs`
   land in temp even when solving the original in place — the image library never gains solver
   files. Temp artifacts deleted in `finally`.
6. **Temp FITS (UInt16 mono)**: SIMPLE/BITPIX=16/NAXIS=2/NAXIS1/NAXIS2/BZERO=32768/BSCALE=1/END;
   data = big-endian signed shorts (`value − 32768`), padded to 2880-byte blocks. No keywords —
   hints ride the CLI. ~60 lines, lives in the Solver area (it is solver plumbing, not XISF I/O).
7. **Result**: parse `<tempbase>.ini` as `KEY=VALUE` lines (first `=` splits); `PLTSOLVD=T` gates;
   `ERROR`/`WARNING` captured for reporting; process exit code mapped to ASTAP's documented table
   (0 ok / 1 no solution / 2 too few stars / 16 read error / 32,33 database trouble) as the
   fallback message when the `.ini` is absent. 60 s process timeout ⇒ kill + report.
8. **Angle bridge (the NINA `ASTAPSolver` bridge, in this wrapper by design)**:
   `WcsOrientation.FromCdMatrix(CD…)` → `PA = (360 − (Rotation − 180)) mod 360`; `Flipped`
   inverted likewise. AL stays generic (its spec pins this boundary).
9. **Stamp via a new high-tier `KeywordList.SetPlateSolution(...)`** — one programmatic method (the
   AL-migration choke point) doing the low-tier writes: RA/DEC (`[deg]`, frame centre = ASTAP's
   reference pixel), `OBJCTROT` through the existing `RotatorSkyAngle` setter, CTYPE1/2
   (`'RA---TAN'`/`'DEC--TAN'`), EQUINOX=2000.0, CRVAL/CRPIX/CD/CROTA from the `.ini` verbatim
   (invariant-culture full precision). `RemoveUnwantedKeywords` already has no collisions with this
   set (verified 2026-08-06); the spec pins survival so future strip-list edits can't silently eat
   it.
10. **New `ProjectReference` to `Astronomy.Core`** (for `WcsOrientation`) + sln membership with the
    follow-up-#7 config-mapping check (`dotnet build -c Release` ⇒ `Astronomy.* -> …Release…`).

## Risks / Trade-offs

- [Solve adds ~1 s/frame to checked browses] → acceptable and user-controlled (checkbox); the
  per-file label/progress UI already narrates.
- [ASTAP bridge (180° + flip inversion) inherited empirically from NINA] → consistency with
  everything NINA ever stamped is guaranteed by construction; absolute correctness verified once
  against a known solved field (tasks).
- [Solving the original uncompressed file in place] → read-only for ASTAP; `-o` redirect keeps its
  outputs away; no mutation risk.
- [UInt16-mono-only temp FITS] → matches the entire real library; anything else surfaces as a named
  per-file failure, never a wrong solve.

## Migration Plan

None — additive feature behind an unchecked-by-default checkbox. Ships after AL release (gate armed).

## Open Questions

None — trigger scope, update-mode interplay, and stamp set were all decided 2026-08-06
(proposal); the keyword-mode redesign is deliberately parked as ROADMAP follow-up #8.
