# Tasks: astap-plate-solve

## 1. Dependencies

- [ ] 1.1 Add `ProjectReference` to `..\..\Library\Astronomy.Core\Astronomy.Core.csproj`; add `Astronomy.Core` to `XisfFileManager.sln`; verify `dotnet build XisfFileManager.sln -c Release` shows every `Astronomy.* ->` line in a Release path (follow-up #7 check)

## 2. Solver area (`Solver\`)

- [ ] 2.1 `SolveResult` (success, RA/Dec deg, PA deg, flipped, CRVAL/CRPIX/CD/CROTA raw strings, error text) + `AstapSolver.SolveAsync(filePath, isCompressed, ct)`: existence check for `XisfConstants.AstapCliPath`, hints via AL `XisfHeaderReader` (ra/spd/fov, blind fallback), `-o` temp redirect, 60 s timeout, `.ini` parse with `PLTSOLVD` gate + exit-code fallback messages, temp cleanup in `finally`
- [ ] 2.2 Temp-FITS writer for compressed inputs (AL `XisfImageReader` → UInt16 mono big-endian BZERO-32768 FITS, 2880-block padded); non-UInt16/multi-channel input ⇒ named per-file failure
- [ ] 2.3 ASTAP convention bridge: `WcsOrientation.FromCdMatrix` → `PA = (360 − (Rotation − 180)) mod 360`, flip inversion

## 3. Keyword stamp

- [ ] 3.1 `KeywordList.SetPlateSolution(...)` high-tier method: RA/DEC `[deg]`, `OBJCTROT` via `RotatorSkyAngle`, `CTYPE1/2`, `EQUINOX`, `CRVAL1/2`, `CRPIX1/2`, `CD1_1..CD2_2`, `CROTA1/2` (invariant culture); confirm no `RemoveUnwantedKeywords` collision

## 4. Read-pass hook (`MainForm`)

- [ ] 4.1 In `ReadHeadersAsync`: gate `CheckBox_Solver.Checked && !Masters_Enable && FrameType==LIGHT`; solver-missing check up front (loud failure); per-file solve → `SetPlateSolution` or accumulate failure; summary dialog after the loop (solved / failed counts + per-file reasons); commit the user's uncommitted `CheckBox_Solver` Designer placement with a label/tooltip

## 5. Docs + verify (same commit)

- [ ] 5.1 ARCHITECTURE (Solver area + stamped-keyword additions to the Keywords section), DOMAIN (measured-vs-planned `OBJCTROT`), ROADMAP recently-shipped, CHANGELOG-equivalent notes
- [ ] 5.2 Build clean (Debug + Release); field verification plan recorded: one known-field absolute-PA sanity check, then solve a `°(M)` backlog batch → save → TSM rescan → framings leave mechanical fallback (user-verified)
