# skip-presolved-lights — tasks

## 1. Presence helper

- [x] 1.1 Add a "has full measured WCS solution" helper to `KeywordList` next to `SetPlateSolution`
      (`KeywordList.cs:823`) testing presence of the 11 unconditional solve-only keywords:
      CTYPE1/CTYPE2, EQUINOX, CRVAL1/CRVAL2, CRPIX1/CRPIX2, CD1_1/CD1_2/CD2_1/CD2_2 — keeping the
      stamped-set definition and its presence test in one file

## 2. Read-pass skip

- [x] 2.1 In `MainForm.ReadHeadersAsync` (`MainForm.cs:291`), inside the existing
      solver-enabled + LIGHT branch, consult the helper before `SolveAsync`: when the full set is
      present, skip the solve and stamp entirely and count the frame as skipped
- [x] 2.2 Extend reporting with the skipped count: `Log.Info` browse-done line, the
      solver status label ("Read N; Solved x, Skipped y[, z failed]"), and leave the failure
      MessageBox behavior unchanged

## 3. Verify (per VERIFICATION.md — build + manual pass)

- [x] 3.1 `dotnet build XisfFileManager.sln -c Release` warning-free
- [ ] 3.2 Manual: checked browse of raw lights → all solve; save; re-browse the saved set → all
      skip (no solver processes, status shows skipped count); unchecked browse → no solver
      involvement; a mixed directory reports solved + skipped correctly

## 4. Docs ride the commit

- [x] 4.1 Update `ROADMAP.md` (recently shipped) and any touched gotchas in `CLAUDE.md`;
      commit docs together with the code change (CLAUDE.md untouched — no new gotcha)
