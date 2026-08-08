# Fix double-save corruption (stale attachment geometry)

## Why

`XisfFileUpdate.UpdateFileAsync` copies the image block verbatim from the session-cached
`TargetAttachmentStart/Length`, which is set at browse and refreshed after hygiene rewrites but
**never after a save**. The second save of a file in one session — with a header-length change in
between (solve keywords added, mosaic target rename) — copies from the stale offset and silently
writes a corrupt file: N bytes of old-XML garbage before the block, block truncated by N at the
tail. This destroyed 66 files across two incidents (2026-06-08 Witch Head, 58 files, pre-refactor
writer; 2026-08-08 Clamshell, 8 files, v2.4.0 — all recovered 2026-08-08). The bug class has
survived every writer generation because nothing ever validated the copy source.

## What Changes

- **Geometry refresh after save**: a successful write updates the in-memory `XisfFile` to the
  just-written file (attachment start/length, XML string) — the same contract the hygiene path
  already honors after its rewrites.
- **Fail-fast copy gate**: before building the output, the save verifies the bytes at the cached
  copy offset in the freshly read source match the declared codec (magic bytes / structural sanity).
  Mismatch = contract violation: console error naming file + expected/found, log entry, abort the
  file's save (rule: no fallback, no warn-and-continue with a corrupt write).
- **Save-if-needed uses on-disk truth**: `KeywordsMatchXml` compares against the freshly read
  header, not the browse-time `XmlString`, so an unchanged second save skips instead of rewriting.

## Capabilities

### New Capabilities

- `save-write-integrity`: the save path's structural-integrity contract — cached geometry is
  refreshed on every write, the block-copy source is validated before writing, and unchanged files
  are not rewritten.

### Modified Capabilities

<!-- none — browse-compression-hygiene, plate-solve-stamp, scheduler-independence unaffected -->

## Impact

- `XisfFileManager/Files/XisfFileUpdate.cs` — all three changes land here (gate, refresh, skip-check).
- `XisfFileManager/Files/XisfFile.cs` — possibly a small helper/setter surface for the refresh.
- Callers (`MainForm` save loop, `Calibration`, `FluxDensity`) unchanged — behavior contract only.
- No back-compat concerns (rule 15); previously corrupted files were already recovered out-of-band.
