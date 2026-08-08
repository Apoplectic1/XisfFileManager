# hygiene-recompress-legacy-codecs

## Why

User decision 2026-08-07: hygiene should converge the library on the write codec
(`zstd+sh` 19), reversing the earlier "mixed codecs are the accepted end state / ~26 GB
migration forgone" call — the hygiene machinery (bounded pool, idempotent, resumable,
browse-scoped) makes the migration nearly free, paid incrementally per first browse of each
directory. Constraint that shapes the criterion: **the zstd level is not recorded in the file**
(the XISF `compression` attribute carries codec:size[:itemSize] only; level is encoder effort) —
so the criterion is codec-based, and existing zstd blocks are trusted as target-state (XFM is
this library's only zstd writer and has only ever written level 19).

## What Changes

- **Hygiene criterion widens**: rewrite when the block is **uncompressed ∨ checksum-less ∨ its
  codec is not `zstd`/`zstd+sh`** (plain `zstd` stays accepted — XFM's own deliberate form for
  1-byte samples). Compressed+checksummed **zlib/lz4 legacy blocks now recompress** to
  `zstd+sh(19)` + SHA-1 on first browse.
- Everything else about hygiene (surgical writer, pool + barrier, cancel/resume, Protect
  exemption, reporting, failure semantics) is unchanged.
- **Operational reality, accepted:** first browse of a legacy target runs a full recompress of
  its files (~8–10 min for a typical target with the pool); the library converges toward the
  benchmark's ~26 GB reclaim (−11% on lights) as directories get browsed; re-browses are
  instant again.

## Capabilities

### New Capabilities

*None.*

### Modified Capabilities

- `browse-compression-hygiene`: the "Hygiene criterion and remedy" requirement — codec-based
  criterion; the "compressed+checksummed files are never touched regardless of codec" clause and
  its zlib-untouched scenario are replaced (zstd-family blocks with checksums are the only
  untouched class).

## Impact

- **XFM**: one criterion expression in `MainForm.ReadHeadersAsync` (+ comments/diag log).
- **Docs**: ROADMAP (hygiene shipped-entry amendment; digest wording), ARCHITECTURE (hygiene
  section + "stays zlib" bullet), DOMAIN ("mixed codecs are the accepted end state" paragraph).
- **Not affected**: AL (`XisfBlockRewriter` already handles any-codec → zstd), solver path,
  save path.
