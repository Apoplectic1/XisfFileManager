# browse-compression-hygiene

## Why

The library's compression state is inconsistent — the first Verify-SHA field run found 90 of 184
files without a block checksum, and freshly captured nights arrive uncompressed — but fixing it
today requires a keyword-update save per file, entangling compression with the
UPDATE_NEW/FORCE/PROTECT mode question (ROADMAP #8). The 2026-08-07 explore session
(`docs/2026-08-07-browse-compression-hygiene-explore.md`) decided compression is **library
hygiene, not a save-mode concern**: the Browse read pass repairs any unhygienic file
automatically, and the solver's temp-FITS path dies as a side effect (its writer is replaced by
the same new rewrite primitive).

## What Changes

- **Browse-time hygiene pass**: during Browse, any file whose image block is **uncompressed or
  lacks a checksum** is rewritten in place as `zstd+sh` level 19 with a SHA-1 block checksum.
  Always on — no checkbox, no opt-out. **BREAKING (behavioral): Browse stops being read-only**
  (stated decision, 2026-08-07). Compressed+checksummed files are never touched regardless of
  codec — mixed zlib/zstd is the accepted end state; no bulk migration.
- **Hygiene is exempt from Protect**: Protect narrows to "no keyword writes" (feeds the ROADMAP
  #8 redesign; FORCE loses its last remaining job).
- **Surgical rewrite primitive** `(source, target, codec)`: XML bytes preserved except the
  `compression`/`checksum`/`location` attributes; block swapped; temp file + atomic move. No
  keyword normalization, no solver-stamp persistence (stamps still ride the normal save).
- **Solver temp-FITS dies**: compressed solve inputs become surgical **temp uncompressed XISF**
  (codec None) via the same primitive; `WriteMinimalFits` and its endian-swap/BZERO=32768/
  UInt16-mono-only code are deleted. The UInt16-mono-only restriction on compressed solve inputs
  disappears with it.
- **Per-file order**: verify SHA → solve → hygiene. Never compress before solve (an uncompressed
  light is ASTAP-readable in place).
- **Bounded worker pool** for hygiene rewrites (user decision 2026-08-07): compression runs off
  the UI thread on a browse-scoped pool with a completion barrier before Browse finishes — no
  hygiene write may outlive the browse pass.
- **Cancel + progress**: Browse gains a cancel path (`mBCancel`, currently never checked during
  Browse) and hygiene progress/counts in the existing browse status line. Cancel is clean-stop:
  in-flight rewrites complete (atomic), queued ones are abandoned; hygiene is idempotent and
  resumable — the next Browse continues where this one stopped.

## Capabilities

### New Capabilities

- `browse-compression-hygiene`: automatic repair of unhygienic image blocks (uncompressed or
  checksum-less) during the Browse read pass — criterion, remedy, surgical write semantics,
  ordering vs solve, Protect exemption, worker pool + barrier, cancel/progress, and reporting.

### Modified Capabilities

- `plate-solve-stamp`: the "Solve input paths" requirement changes — a compressed XISF is no
  longer decoded to pixels and re-encoded as a temporary FITS; it is surgically rewritten as a
  temporary **uncompressed XISF** (same primitive as hygiene, codec None). The UInt16-mono-only
  limit on compressed solve inputs is removed.

## Impact

- **XFM**: `MainForm.ReadHeadersAsync` (browse loop — hygiene step, pool, barrier, cancel,
  progress), `Solver/AstapSolver.cs` (`WriteMinimalFits` + decode path deleted, surgical temp
  XISF substituted; solver binary switched `astap_cli.exe` → `astap.exe`, which alone carries
  the XISF reader — fixes the latently broken in-place uncompressed path, see design.md D5),
  new surgical rewrite primitive (in AL), `XisfFile` in-memory geometry refresh after rewrite
  (stale `TargetAttachmentStart` would corrupt a later save's verbatim block copy).
- **AL (`Astronomy.XISF`)**: candidate home for the surgical rewrite primitive (it already owns
  block locate/parse/compress: `XisfImageReader`, `BlockCompressionInfo`, `XisfBlockCompression`).
  Consumer-agnostic contract required if placed there.
- **Docs**: ROADMAP #10 → shipped; #8 narrows (Protect = "no keyword writes"); user-docs gotcha:
  recompressing a checksum-less block *launders* it — the fresh SHA-1 means "intact from now
  on", never provenance.
- **Not affected**: `scheduler.db` (XFM is TS-free), keyword pipeline (hygiene writes no
  keywords), existing compressed+checksummed zlib files (never touched).
