# plate-solve-stamp Specification

## Purpose
Measuring each light frame's real sky solution with the local ASTAP solver during XFM's read pass and
stamping it into the frame's keywords — replacing planned pointing/rotation values with measured ones
so downstream consumers reconcile against truth.
## Requirements
### Requirement: Checkbox-gated solving during the read pass
When `CheckBox_Solver` is checked, the Browse/read pass SHALL plate-solve every **light** frame as it
is read **except** frames that already carry a full measured WCS solution, which SHALL be skipped —
no solver process runs and no solution keywords are stamped for a skipped frame. A frame carries a
full measured WCS solution when all eleven unconditional solve-only keywords are present:
`CTYPE1`/`CTYPE2`, `EQUINOX`, `CRVAL1`/`CRVAL2`, `CRPIX1`/`CRPIX2`, `CD1_1`/`CD1_2`/`CD2_1`/`CD2_2`.
The test is presence-based and provenance-agnostic (any tool's solution counts; no ASTAP-marker
requirement). `CROTA1`/`CROTA2` SHALL NOT participate in the test (conditionally stamped);
`RA`/`DEC`/`OBJCTROT` SHALL NOT participate (raw captures carry them as planned values). A frame
with a partial set SHALL be re-solved (self-healing, not an error). There is no force-re-solve
path. Skipped frames SHALL be counted and reported alongside solved/failed counts in the browse
status and log. Master and calibration frames SHALL NOT be solve candidates. When unchecked, the
read pass SHALL involve the solver in no way — behavior identical to before this feature,
including no presence evaluation.

#### Scenario: Checked browse solves light frames
- **WHEN** a directory of raw light frames (no WCS solution set) is browsed with the solver
  checkbox checked
- **THEN** each light frame is solved and its in-memory keywords carry the solution

#### Scenario: Fully solved frame skips
- **WHEN** a light frame carrying all of `CTYPE1`/`CTYPE2`, `EQUINOX`, `CRVAL1`/`CRVAL2`,
  `CRPIX1`/`CRPIX2`, `CD1_1`..`CD2_2` is browsed with the solver checkbox checked
- **THEN** no solver process runs for that frame, its keywords are unchanged, and it is counted
  as skipped

#### Scenario: Partial solution re-solves
- **WHEN** a light frame carrying only some of the solution set is browsed with the solver
  checkbox checked
- **THEN** the frame is solved and the full stamped set replaces the partial one

#### Scenario: Planned-only frame still solves (measured replaces planned)
- **WHEN** a frame carrying planned `RA`/`DEC`/`OBJCTROT` but no WCS solution set is browsed with
  the solver checkbox checked
- **THEN** the frame is solved and the stamped values are the measured ones

#### Scenario: Skips are reported
- **WHEN** a checked browse reads a mix of solved and unsolved light frames
- **THEN** the completion status reports solved, skipped, and failed counts

#### Scenario: Unchecked browse is untouched
- **WHEN** the same directory is browsed with the checkbox unchecked
- **THEN** no solver process runs, no solution keywords are added, and no presence check occurs

### Requirement: Solve input paths
An uncompressed XISF SHALL be handed to the solver directly. A compressed XISF SHALL be
surgically rewritten as a temporary **uncompressed XISF** — XML preserved except the block
attributes the decompression forces, block decompressed, no pixel re-encoding and no
intermediate FITS — and that temporary file handed to the solver; the solver consumes one input
format (XISF) for all cases. The rewrite SHALL NOT restrict the frame's sample format or channel
count beyond what the solver itself supports. All temporary solve artifacts (input copy and
solver output files) SHALL be created outside the image library and removed afterward, and
solver output SHALL be redirected so no solver file ever appears next to a library image.

#### Scenario: Compressed backlog frame solves
- **WHEN** a zlib+shuffle-compressed light frame is read with the solver checked
- **THEN** it solves via a temporary uncompressed XISF and the original file's directory gains
  no new files

#### Scenario: No FITS intermediate
- **WHEN** any frame is solved
- **THEN** no FITS file is produced by the application as solver input

### Requirement: Header hints drive the solve
The solve SHALL pass the frame's own header pointing (RA/Dec) and field-height hints to the solver
when present, and fall back to a blind solve when absent.

#### Scenario: Hinted solve
- **WHEN** a frame's header carries RA/DEC and the optics/geometry needed for field height
- **THEN** the solver is invoked with position and field hints

### Requirement: Stamped solution set
A successful solve SHALL stamp, via the keyword pipeline's high tier: `RA`, `DEC` (degrees, frame
centre), `OBJCTROT` (measured position angle, degrees), and the standard WCS solution —
`CTYPE1`/`CTYPE2` (tangent projection), `EQUINOX`, `CRVAL1`/`CRVAL2`, `CRPIX1`/`CRPIX2`,
`CD1_1`..`CD2_2`, `CROTA1`/`CROTA2`. Stamped solution keywords SHALL survive keyword normalization
(`RemoveUnwantedKeywords`). Position angle SHALL be derived through the shared library's generic WCS
conversion with the ASTAP convention bridge (180° offset + parity inversion) applied in this wrapper.

#### Scenario: Solution survives normalization and save
- **WHEN** a solved frame is saved through the normal update path
- **THEN** the file on disk carries the full stamped set

#### Scenario: Measured replaces planned
- **WHEN** a frame already carrying a planned `OBJCTROT` is solved
- **THEN** the stamped `OBJCTROT` is the measured value

### Requirement: Persistence follows the normal save path
Solve results SHALL live in the in-memory keyword list at read time and persist only through the
existing save/update step. Solver stamps SHALL NOT be subject to the UPDATE_NEW-vs-FORCE keyword-mode
distinction (solver enabled ⇒ the update happens); the file-level PROTECT refusal keeps its meaning —
a protected file is not saved, so its solved values never persist.

#### Scenario: Protected file stays untouched on disk
- **WHEN** a PROTECT-mode file is solved in memory and a save is attempted
- **THEN** the save is refused as today and the file on disk is unchanged

### Requirement: Failure semantics
A failed solve (no solution, too few stars, solver error) SHALL stamp nothing on that frame, SHALL be
reported per file with the solver's error text, and SHALL NOT stop the read pass. A missing or
non-runnable solver installation SHALL fail the checked browse loudly with a message naming the
expected location — no silent skip.

#### Scenario: One cloudy frame doesn't stop the batch
- **WHEN** one frame of a checked browse fails to solve
- **THEN** that frame is reported with the solver's error, gains no solution keywords, and the
  remaining frames still solve

#### Scenario: Solver not installed
- **WHEN** a checked browse starts and the solver executable is absent
- **THEN** the operation fails immediately with a message naming the expected path

