# Tasks — fix-double-save-corruption

## 1. Fail-fast copy gate (make corruption unwritable)

- [x] 1.1 In `UpdateFileAsync`, after reading `binaryFileData`, parse the on-disk header's
      `location` attribute and compare offset/length with `xFile.TargetAttachmentStart/Length`;
      on mismatch abort this file's save (no write, `LastUpdateOutcome = Failed`, `Log.Error`
      naming file + cached vs on-disk values, error surfaced to the UI summary path)
- [x] 1.2 Add codec plausibility check at the copy offset: zlib/zstd magic bytes for those
      codecs; for lz4/uncompressed require offset ≥ end of on-disk XML and offset+length ≤ file
      size; same abort path on failure
- [x] 1.3 Remove/route any existing silent tolerance in the same region (the
      `possibleBogusImageAttachmentLocation` warn-and-continue block) into the new gate per
      rule 16 — one contract check, no scattered guards

## 2. Geometry refresh after save (make the bug unreachable)

- [x] 2.1 After the successful atomic move in `UpdateFileAsync`, refresh
      `xFile.TargetAttachmentStart` (16 + written XML byte length + padding, taken from the
      exact written buffers), leave `TargetAttachmentLength` (verbatim copy), and set
      `xFile.XmlString` to the written XML text — in-place saves only (Calibration/FluxDensity
      write copies; the untouched source's cache stays valid)
- [x] 2.2 Confirm the three call sites (MainForm save loop, `Calibration`, `FluxDensity`) need
      no changes; remove any call-site geometry fixups made redundant (none existed; MainForm
      failure path already MessageBoxes + stops on false return)

## 3. Save-if-needed correctness

- [x] 3.1 Change `KeywordsMatchXml` to take the freshly read on-disk XML string as a parameter
      (extracted before keyword replacement) instead of reading `xFile.XmlString`
- [x] 3.2 Reorder `UpdateFileAsync` so the skip check runs after the file read (it currently
      runs before) while keeping the locked-file wait and PROTECT gate first

## 4. Verify (per VERIFICATION.md — no test project; build + manual pass on copies)

- [x] 4.1 `dotnet build XisfFileManager.sln -c Release` warning-free (0 warnings, 0 errors)
- [x] 4.2 Double-save repro on scratch copies: browse (solver on) → save → save again; second
      save must skip (no keyword change) — then force a keyword change and save again; a
      verify-SHA browse must report all files verified, zero failed
      (2026-08-08 F:\Temp pass: save #1 = 58 Written after solve added WCS, save #2 = 58
      Skipped; all files re-verified geometry+checksum clean out-of-app)
- [x] 4.3 Gate test: hex-edit a scratch copy's `location` offset to point into the XML; save
      must abort with the contract error and leave the file untouched
      (2026-08-08: covered twice — a throwaway harness driving the real UpdateFileAsync on
      library-scale scratch copies, then permanently as automated tests in group 6; both abort
      shapes verified with files byte-identical after)
- [x] 4.4 Hygiene interplay: browse a legacy-codec scratch copy (hygiene rewrites) → save →
      save; verify clean (same 2026-08-08 F:\Temp pass — 58 zlib legacies hygiene-rewrote to
      zstd, then the double save; zero failures)

## 6. Test project (scope added mid-apply, user 2026-08-08)

- [x] 6.1 Create `XisfFileManager.Tests` (xunit.v3 3.2.2 + Test.Sdk 18.6.0, warnings-as-errors —
      Library conventions), referenced fixture `TestData/Unit16_0s_200x200.xisf`, added to sln
- [x] 6.2 `SaveWriteIntegrityTests`: the four spec scenarios through the real `UpdateFileAsync` —
      corrupt-offset abort, stale-cache abort, FORCE double-save verified clean, UPDATE_NEW
      second-save skip (fixtures zstd-compressed in setup via the AL rewriter)
- [x] 6.3 Fix the latent `GetString(data, xisfStart, xisfEnd)` count bug the first test run
      exposed (header extraction overran by xisfStart bytes; hard throw on files smaller than
      xisfStart + xisfEnd)
- [x] 6.4 VERIFICATION.md rewritten (test section, "no test project" retired) + CLAUDE.md doc-map row

## 5. Docs (same commit as code — rule 4)

- [x] 5.1 CLAUDE.md: replace/extend the `UpdateFileAsync` gotcha bullet (save now refreshes
      geometry + aborts on geometry-contract violation); ROADMAP recently-shipped entry
- [x] 5.2 NOTEBOOK.md entry: the 2026-08-08 corruption incident — signature, arithmetic
      diagnosis, recovery method (server restore + padding repair), link to this change
- [ ] 5.3 Update memory `xfm-double-save-corruption-bug` to "fixed" once shipped
