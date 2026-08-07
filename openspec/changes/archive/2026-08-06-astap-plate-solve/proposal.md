# Proposal: astap-plate-solve

## Why

Frames whose headers lack `OBJCTROT` (sky rotation) degrade TSM's framing reconciliation — its
framing key falls back to mechanical rotation and marks the framing `°(M)`. NINA only stamps
`OBJCTROT` when a target rotation was set, and what it stamps is the *planned* angle, not a
measurement. A local ASTAP install can measure the real solution per frame. XFM is the portfolio's
image-metadata writer (TSM's image-library scan is read-only by charter), so the solve-and-stamp
step lands here — decided 2026-08-06 after the TSM-side exploration. Both AL prerequisites shipped
the same day: `Astronomy.XISF` Tier 3 (decompress + verified image read) and
`Astronomy.Core.Astrometry.WcsOrientation` (CD matrix → position angle/parity).

**First candidates are the compressed backlog** — the very frames TSM flagged `°(M)` (user call,
2026-08-06) — so the decompress → temp-file solve path is v1 scope, not a later extension.

## What Changes

- **New XFM feature: plate-solve during the read pass, gated by `CheckBox_Solver`.** The checkbox
  (added 2026-08-06, Directory Selection group) opts the Browse/read pass into solving each XISF
  file as it is read. **Scope: all light frames** (decided 2026-08-06) — every light frame read
  while checked is solved (or re-solved: measured replaces planned), at ~1 s per frame with header
  hints. Master frames stay with their own existing path
  (`CheckBox_FileSelection_DirectorySelection_Masters_Enable`); calibration frames are never
  solve candidates. Solve results populate the in-memory `KeywordList` at read time; persistence
  stays with the normal save/update step. **Solved values always win** (decided 2026-08-06: disk
  is truth — solver enabled means do the update): the solver stamp is not subject to the
  UPDATE_NEW-vs-FORCE keyword-mode distinction. File-level PROTECT keeps its meaning — a PROTECT
  file refuses the save entirely (the centralized `UpdateFileAsync` gate), so its solved values
  simply never persist. Unchecked = zero solver involvement, today's behavior exactly.
- **Solve input per frame:** compressed → AL `XisfImageReader.ReadImageAsync` (decompress,
  checksum-verified) → minimal temporary mono FITS (deleted after) → `astap_cli`; uncompressed →
  `astap_cli` reads the `.xisf` directly (native uncompressed-XISF support). Solve hints come from
  the header (`-ra` RA/15, `-spd` Dec+90, `-fov` field height) for near-instant solves.
- **Result handling:** parse ASTAP's `.ini` (`PLTSOLVD` gate, `CRVAL`/`CRPIX`/`CD`/`CROTA`,
  `ERROR`/`WARNING`); a failed solve reports per file (message + ASTAP error/exit code) and stamps
  **nothing** — no partial writes.
- **Angle conversion:** AL `WcsOrientation.FromCdMatrix` + the ASTAP convention bridge **in this
  wrapper** (position angle `360 − (Rotation − 180)`, flip-flag inversion — NINA `ASTAPSolver`'s
  bridge, kept out of AL's generic math by design).
- **Stamp through the existing high-tier keyword pipeline** (the AL-migration choke point):
  `RA`, `DEC` (degrees, frame centre = ASTAP's reference pixel), `OBJCTROT` (measured position
  angle), plus the full standard solution — `CRVAL1/2`, `CRPIX1/2`, `CD1_1..CD2_2`, `CROTA1/2` —
  so each file carries a complete WCS any future software can read.
- **`RemoveUnwantedKeywords` whitelist fix:** the WCS set must survive keyword normalization
  (today the strip list even deletes `OBJCTRA`/`OBJCTDEC`; the new WCS keywords must be exempt).
- **Save path unchanged:** the existing `UpdateFileAsync` rewrite — for an already-compressed
  backlog frame the block passes through verbatim (header-only rewrite, no recompression); an
  uncompressed frame compresses as it does today.

## Capabilities

### New Capabilities

- `plate-solve-stamp`: the solve-and-stamp contract — candidate selection, solve input paths
  (compressed/uncompressed), hint construction, success/failure semantics (no partial stamps),
  the stamped keyword set, and normalization survival of solved keywords.

### Modified Capabilities

_None — `scheduler-independence` is untouched (no TS/scheduler data involved)._

## Impact

- **Code:** new solver wrapper area (ASTAP process invocation, `.ini` parse, temp-FITS writer,
  convention bridge); read-pass hook behind `CheckBox_Solver` (already placed in the Designer,
  uncommitted — rides with this change) + candidate filter; `KeywordList` whitelist fix.
- **Dependencies:** new `ProjectReference` to `Astronomy.Core` (for `WcsOrientation`) alongside the
  existing `Astronomy.XISF`; external runtime dependency on a local ASTAP install
  (`astap_cli.exe` + star database, present at `C:\Program Files\astap`; path a configurable with
  that default — missing install fails the action with a clear message, per the fail-fast rule).
- **Payload/release:** `Astronomy.Core.dll` joins the payload; the AL release gate (armed
  2026-08-06) already checks every `Astronomy.*.dll`. AL must be released before XFM's next
  release regardless.
- **Verification:** one-time absolute-PA sanity check against a known solved field (the
  ASTAP-bridge check); field verification = solve a batch of `°(M)` backlog frames, rescan in TSM,
  confirm the framings leave mechanical fallback.
- **Docs:** ARCHITECTURE (new solver area + keyword additions), DOMAIN (measured-vs-planned
  `OBJCTROT` semantics), ROADMAP/CHANGELOG.
