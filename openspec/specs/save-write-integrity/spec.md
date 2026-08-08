# save-write-integrity Specification

## Purpose
Guarantees the save path never writes a structurally corrupt XISF file: cached block geometry
always reflects the file on disk, and every block copy is validated against the source bytes
before anything is written.
## Requirements
### Requirement: Cached geometry reflects the last write

After a successful save, the in-memory state for that file SHALL describe the file as just
written — attachment offset and length, and the header the save-skip comparison reads — such that
an immediately following save of the same file is correct without an intervening re-browse.

#### Scenario: Second save after a header-length change
- **WHEN** a file is saved once (header length changes — e.g. keywords added or removed) and then
  saved again in the same session
- **THEN** the second save copies the image block from its true offset and produces a
  structurally valid file (declared geometry matches the bytes; declared checksum matches the
  stored block)

#### Scenario: Save after a hygiene rewrite
- **WHEN** a file is rewritten by browse hygiene and then saved
- **THEN** the save copies the image block from the post-hygiene offset (existing behavior,
  preserved)

### Requirement: Block-copy source is validated before writing

Before writing the output, the save SHALL verify that the bytes at the cached copy offset in the
freshly read source file are plausibly the declared image block (for a compressed block, the
codec's magic bytes; for an uncompressed block, that the offset is not inside the XML header).
On mismatch the save SHALL abort that file without writing, emit a console/UI-visible error
naming the file, the expected codec, and what was found, and record a log entry. It SHALL NOT
fall back to scanning, guessing, or writing anyway.

#### Scenario: Stale offset detected
- **WHEN** the cached offset points at bytes that do not match the declared codec (e.g. XML text)
- **THEN** no output file is written, the original file is left untouched, the failure is logged
  and reported, and processing continues with the next file

### Requirement: Unchanged files are not rewritten

In save-if-needed mode, the skip decision SHALL be made against the current on-disk header, not
against session-cached header text.

#### Scenario: Immediate re-save with no edits
- **WHEN** a file is saved and then saved again with no keyword changes in between
- **THEN** the second save is skipped (reported as skipped, file untouched)
