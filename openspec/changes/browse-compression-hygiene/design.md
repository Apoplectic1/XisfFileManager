# browse-compression-hygiene — design

## Context

See `proposal.md` (Why) and `docs/2026-08-07-browse-compression-hygiene-explore.md` for the
decision record. Code facts that shape the approach (mapped 2026-08-07):

- `ReadHeadersAsync` (MainForm.cs:263-397) is a **sequential, UI-thread** loop: header parse →
  optional Verify-SHA → optional ASTAP solve per file. No `mBCancel` check exists in it today.
- `UpdateFileAsync` (XisfFileUpdate.cs:80) is a full XML **rebuild** (keyword replace,
  attachment strip, renumber) — there is no surgical rewrite primitive anywhere yet. Its
  verbatim block copy uses the **in-memory** `xFile.TargetAttachmentStart`, so any on-disk
  rewrite after header read must refresh the in-memory geometry or a later save corrupts.
- `AstapSolver.SolveAsync` (AstapSolver.cs:61) hands uncompressed files to ASTAP **in place**;
  compressed files go through pixel decode (`XisfImageReader.ReadImageAsync`, UInt16-mono-only)
  → `WriteMinimalFits` (endian swap, BZERO=32768) → temp `.fit`.
- AL `Astronomy.XISF` already owns block locate/parse/compress (`BlockCompressionInfo`,
  `XisfBlockCompression` with zstd level parameter, `XisfChecksumVerifier`) and XFM already
  consumes it.
- Cost reality (benchmark 2026-08-07): zstd-19 encodes at 2–6 MB/s single-threaded ⇒ ~10–20 s
  per 40 MB light. **This recurs every fresh night** (~100 uncompressed lights ⇒ ~25–35 min
  sequential), not just on the first library pass — the encode cost moves here from save time
  (save then copies the block verbatim and gets faster). This is why the worker pool is the
  chosen design, not an optimization.

## Goals / Non-Goals

**Goals:**
- One shared surgical rewrite primitive serving both hygiene (codec zstd-19, atomic in-place)
  and the solver's compressed-input path (codec None, temp target).
- Browse stays responsive and cancelable while rewrites run; browse completion is a hard
  barrier for all hygiene writes.
- In-memory `XisfFile` state stays coherent with the rewritten on-disk file within the session.

**Non-Goals:**
- No bulk zlib→zstd migration (compressed+checksummed files are never candidates).
- No keyword writes from hygiene; no solver-stamp persistence outside the normal save.
- No change to `UpdateFileAsync`'s own save pipeline beyond benefiting from refreshed geometry.
- No back-compat/migration machinery (portfolio rule: target state only).

## Decisions

### D1 — Surgical writer lives in AL `Astronomy.XISF`
`XisfBlockRewriter.RewriteAsync(sourcePath, targetPath, codec[, zstdLevel])`: locate the image
block from the XML, produce the block in the target codec (compress raw input; decompress
compressed input for codec None), rewrite **only** the block-affected XML attributes
(`compression`, `checksum`, `location`) plus the header-length bookkeeping, preserving all other
XML bytes; write target via temp + atomic move when target == source.
*Why AL:* it owns every ingredient (locate, `BlockCompressionInfo`, compress/decompress, SHA-1)
and both XFM call sites are XISF-format concerns, not app logic. Contract stays
consumer-agnostic (no "hygiene"/"solver" naming — callers pick codec and target).
*Alternative rejected:* XFM-local helper — duplicates AL's block grammar and leaves the solver
path importing XFM file code.
*Multi-attachment files:* swapping the main block shifts any attachment stored after it; the
writer recomputes `location` for **every** attachment following the swapped block (same
delta). If geometry can't be resolved unambiguously, fail that file loudly (spec: failure
semantics) — never write a guess.

### D2 — Pipeline shape: sequential front, pooled back, barrier at the end
The browse loop keeps its sequential UI-thread front half per file: header parse → Verify-SHA →
solve (ASTAP remains one-at-a-time, as today). Files failing the hygiene criterion are
**enqueued** to a bounded pool (`Task.Run` workers gated by `SemaphoreSlim`); solved lights
enqueue only after their solve completes (spec: solve precedes compression), unsolved files
enqueue right after their front half. After the `foreach`, browse `await`s all hygiene tasks
(the barrier) before `PopulateUiFromFiles`/`UpdateUI(ENABLED)`. Because the UI stays disabled
until the barrier passes, no Update/Calibration/FluxDensity operation can race a rewrite.
*Pool degree:* `min(6, max(2, Environment.ProcessorCount − 2))` — zstd-19 is CPU-bound; the cap
also bounds peak memory (≈ degree × (raw + compressed block), lights ~40 MB raw).
*Alternative rejected:* fully sequential in-loop compression — freezes the UI 10–20 s per file
and makes a fresh night's browse ~25–35 min with no repaint; the recurrence (every new night)
kills it.

### D3 — Geometry refresh, not re-read
On rewrite completion the writer returns the new geometry (`BlockCompressionInfo`, attachment
start/length, new XML length); the browse loop updates the in-memory `XisfFile` in place
(`TargetAttachmentStart/Length`, compression state, the affected `mXDoc` attributes).
*Why not re-run `ReadXisfFileHeaderKeywords`:* a wholesale re-read would discard in-memory-only
state — solver stamps not yet saved. Surgical update mirrors the surgical write.
This closes the stale-`TargetAttachmentStart` corruption path in `UpdateFileAsync`'s verbatim
copy.

### D4 — Hygiene bypasses `UpdateFileAsync` entirely
Hygiene calls the AL writer directly, so the PROTECT gate inside `UpdateFileAsync` never sees
it — the Protect exemption falls out structurally instead of via a bypass flag. Keyword-mode
semantics (ROADMAP #8) are untouched by this change.

### D5 — Solver path swap (amended during apply, 2026-08-07)
`AstapSolver.SolveAsync` compressed branch becomes: `XisfBlockRewriter.RewriteAsync(filePath,
tempBase + ".xisf", codec: None)` → hand temp to ASTAP. Delete `WriteMinimalFits`, the
`XisfImageReader.ReadImageAsync` pixel decode, and the UInt16-mono-only guard (the rewriter is
format-agnostic — it moves bytes, not pixels). Temp lives in OS temp as today; cleanup unchanged.

**Amendment — XFM must drive `astap.exe`, not `astap_cli.exe`.** The apply-phase spot-check
found `astap_cli.exe` has **no XISF loader at all** (its `load_image` dispatches on extension:
FITS/TIFF/PNM/PNG only; `.xisf` → false → "Error reading image file"; verified in
`command-line_version/unit_command_line_general.pas` and empirically). The uncompressed-XISF
reader (`unit_xisf.pas`) exists only in the GUI binary `astap.exe`, which supports the same
headless CLI switches and errorlevels — NINA's own usage pattern. Empirically: `astap.exe`
solved a surgical temp uncompressed XISF (PLTSOLVD=T, CRVAL matching hints) and read the
uncompressed TestData file ("Not enough stars", i.e. reader accepted). The design's earlier
"field-proven in-place path" premise was wrong — the in-place uncompressed branch had never
actually run in the field (library files are all compressed; the solver feature shipped
2026-08-06), so the shipped in-place path was latently broken with `astap_cli`. The binary
switch (`XisfConstants.AstapPath` → `astap.exe`) fixes that latent bug and unblocks both XISF
input paths.

### D6 — Cancel and progress ride existing affordances
The browse loop checks `mBCancel` between files (front half) and the queue stops accepting;
queued-not-started jobs are canceled via a browse-scoped `CancellationTokenSource`; in-flight
rewrites finish their atomic move. Progress: the existing per-file label gains a
`"Compressing: <file>"` line (pattern: the solve label at MainForm.cs:341) driven via
UI-thread marshaling (`IProgress<T>`), and hygiene counts (`rewritten / failed`) join the
existing browse status line and `Log` output alongside verify/solve counts.

## Risks / Trade-offs

- [Rewrite races a later save on the same file] → barrier + disabled UI until browse completes;
  in-session geometry refresh (D3). No hygiene write can exist outside a browse pass.
- [Stale in-memory geometry corrupts a save] → D3 is a hard requirement with a spec scenario
  ("hygiene then keyword save in one session"); verify in the manual pass.
- [Peak memory under the pool] → degree cap (D2); largest realistic blocks (masters ~100–200 MB
  raw) × 6 stays well under the existing 1 GB single-file ceiling.
- [Laundering: a fresh SHA-1 certifies possibly-rotted bytes on no-checksum files] →
  unavoidable by definition (unverifiable is unverifiable); documented in spec + user docs:
  checksum means "intact from now on", never provenance.
- [Files with attachments after the image block (thumbnails/ICC on foreign-written files)] →
  D1 shifts their locations correctly; anything unresolvable fails loudly per file, browse
  continues.
- [UI-thread starvation from progress chatter] → marshal per file, not per buffer; counts
  accumulate on the pool side.

## Migration Plan

None (portfolio rule 15: target state only; a rewritten file is a strictly more-conformant
XISF). Rollback = revert the commit; already-rewritten files are valid XISF and need no undo.

## Open Questions

- Pool degree default may want tuning after the first fresh-night field run (measure encode
  throughput vs core count) — safely adjustable later; spec only requires "bounded".
