# plate-solve-stamp — skip-presolved-lights delta

## MODIFIED Requirements

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
