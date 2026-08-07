## 1. AL — surgical block rewriter (`Astronomy.XISF`)

- [x] 1.1 Implement `XisfBlockRewriter.RewriteAsync(sourcePath, targetPath, codec[, zstdLevel])`:
      locate image block, transform to target codec (raw→zstd+sh+SHA-1; compressed→raw for
      codec None), rewrite only `compression`/`checksum`/`location` attributes + header-length
      bookkeeping, preserve all other XML bytes, shift `location` of any attachment after the
      swapped block, temp + atomic move when target == source; return new geometry
      (`BlockCompressionInfo`, attachment start/length)
- [x] 1.2 Fail loudly (exception naming file + cause) on unresolvable geometry; never write a
      partial or guessed file
- [x] 1.3 AL tests: uncompressed→zstd-19 round-trip (reader + `XisfChecksumVerifier` pass),
      zlib-no-checksum→zstd-19, compressed→None decompress, XML-byte-preservation outside the
      three attributes, trailing-attachment location shift, in-place atomicity (temp cleaned)
- [x] 1.4 AL docs/ROADMAP entry; consumer-agnostic XML docs (no XFM terminology)

## 2. XFM — solver input path swap

- [x] 2.1 `AstapSolver.SolveAsync` compressed branch: rewrite source to `tempBase + ".xisf"`
      via `XisfBlockRewriter` (codec None) and hand that to ASTAP
- [x] 2.2 Delete `WriteMinimalFits`, `FitsCard`, the `XisfImageReader.ReadImageAsync`
      pixel-decode step, and the UInt16-mono-only guard; temp-cleanup list swaps `.fit` for
      `.xisf` — after this task no FITS-format code remains anywhere in the app
      *(apply amendment: solver binary switched to `astap.exe` — `astap_cli.exe` has no XISF
      loader; design.md D5 amendment + NOTEBOOK 2026-08-07)*
- [x] 2.3 Build + spot-check: a zlib-compressed light solves; no `.fit` produced; library
      directory gains no files *(done via scratchpad harness: surgical temp uncompressed XISF
      from a real zlib light → astap.exe → PLTSOLVD=T, CRVAL ≈ header hints; library dir clean)*

## 3. XFM — browse hygiene pass

- [x] 3.1 Surface block-checksum presence on `XisfFile` from the header read (beside
      `IsImageCompressed`) so the criterion (uncompressed ∨ no checksum) is testable per file
      *(already present: `XisfFile.Compression` is a `BlockCompressionInfo` with `HasChecksum`,
      populated by `GetImageBlockInfo` from both attributes — no code needed)*
- [x] 3.2 Hygiene queue in `ReadHeadersAsync`: bounded pool (`SemaphoreSlim`,
      degree `min(6, max(2, ProcessorCount − 2))`), enqueue after each file's front half —
      after solve completes for solved lights, immediately otherwise
- [x] 3.3 Barrier: await all hygiene tasks before `PopulateUiFromFiles`/`UpdateUI(ENABLED)`;
      UI stays disabled throughout *(barrier loop pumps the UI context every 250 ms for
      progress + late cancel)*
- [x] 3.4 Geometry refresh on completion: update the in-memory `XisfFile`
      (`TargetAttachmentStart/Length`, `Compression`, `ItemSize`) from the rewriter's returned
      geometry — no wholesale header re-read (solver stamps must survive). `mXDoc` attrs left
      alone: verified only consumed for keywords at read time; the save path re-reads XML from
      disk
- [x] 3.5 Cancel: `mBCancel` checked between files and in the barrier loop (the existing
      Cancel button); queued jobs abandon via a captured flag after gate acquire, in-flight
      rewrites finish their atomic move, browse ends cleanly with files read so far kept
- [x] 3.6 Progress + reporting: `"Compressing: <file>"` on the browse label (UI-context
      continuations — no cross-thread writes by construction); progress bar repurposed for the
      hygiene tail; `Hygiene N Rewritten` joins the Statistics line via `mHygieneSummary`
      (same repaint mechanism as Verify-SHA); `Log.Diag("HYGIENE")` per start, `Log.Info` per
      rewrite, `Log.Error` per failure; capped failures dialog
- [x] 3.7 Per-file failure handling: locked/unreadable/unresolvable-geometry files report and
      skip, original untouched (temp + atomic replace in AL; retried naturally on next browse)

## 4. Docs (same commit as code)

- [x] 4.1 ROADMAP: #10 → Recently shipped (closed-entry stub like #6; shipped digest carries the
      astap.exe discovery); #8's narrowed-Protect note already present in its entry
- [x] 4.2 ARCHITECTURE.md: hygiene subsystem section (criterion, surgical writer, pool + barrier,
      geometry refresh, cancel); solver mechanics rewritten (astap.exe + surgical temp XISF);
      stale "FORCE-gated recompress" and "targets zlib on save" wordings corrected
- [x] 4.3 User-facing gotcha in DOMAIN.md § XISF format: laundering ("intact since this checksum
      was written, never since capture"), Browse-writes notice, zstd-19 + mixed-codec end state
- [x] 4.4 VERIFICATION.md: Browse-writes caution + 5-step hygiene manual pass (compress/PI-open,
      counts + 100% Verify-SHA, cancel/resume, hygiene-then-save, solver temp-XISF + in-place)
- [x] 4.5 CLAUDE.md gotchas: `UpdateFileAsync` bullet unchanged (save-compression backstop
      remains); added "Browse writes files" and "solver is astap.exe, not astap_cli" bullets

## 5. Verification

- [x] 5.1 Warning-free `dotnet build XisfFileManager.sln -c Release` (AL graph too)
- [x] 5.2 AL tests green (`Astronomy.XISF.Tests`: 113/113 — 10 new rewriter tests + no
      regressions in the reader/verifier/compression suites the rewriter refactor touched)
- [ ] 5.3 Manual pass (user): browse a fresh uncompressed night → files rewritten
      `zstd+sh(19)` + SHA-1, PixInsight opens one; a legacy no-checksum file recompressed;
      compressed+checksummed zlib file byte-identical
- [ ] 5.4 Manual pass (user): cancel mid-pass → UI restored, re-browse repairs only the
      remainder; hygiene-then-keyword-save on one file yields a valid XISF
- [ ] 5.5 Manual pass (user): solver on a compressed backlog frame (temp `.xisf`, solves, no
      solver files near library); fresh-night wall-clock sanity vs pool expectation (~4–6 min
      per ~100 lights)
