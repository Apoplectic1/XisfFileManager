## REMOVED Requirements

### Requirement: Hygiene criterion and remedy
**Reason**: Replaced by the codec-based criterion below — the "compressed+checksummed files are
never touched regardless of codec" clause (and its zlib-untouched scenario) is inverted by the
2026-08-07 decision to converge the library on the write codec.
**Migration**: None (no consumer of the old criterion; behavior widens, it does not break).

## ADDED Requirements

### Requirement: Codec-based hygiene criterion and remedy
During the Browse read pass, every file whose image block is **uncompressed, lacks a block
checksum, or is compressed with a codec other than `zstd`/`zstd+sh`** SHALL be rewritten in
place with the block compressed as `zstd+sh` level 19 (plain `zstd` for 1-byte samples) and a
SHA-1 block checksum recorded. A checksummed zstd-family block SHALL NOT be touched: the encoder
level is not recorded in the XISF compression attribute, so existing zstd blocks are trusted as
target-state rather than re-encoded. Legacy codecs (zlib, lz4 families) converge to the write
codec incrementally as their directories are browsed. The fresh checksum certifies the bytes as
written by the rewrite ("intact from now on") and carries no claim about the block's history.

#### Scenario: Fresh uncompressed file is repaired
- **WHEN** a directory containing a freshly captured uncompressed XISF is browsed
- **THEN** the file on disk is rewritten with a `zstd+sh` level 19 block and a SHA-1 checksum

#### Scenario: Legacy compressed file without checksum is repaired
- **WHEN** a zlib+shuffle-compressed file lacking a block checksum is browsed
- **THEN** the file is rewritten with a `zstd+sh` level 19 block and a SHA-1 checksum

#### Scenario: Legacy zlib file with checksum is recompressed
- **WHEN** a zlib+shuffle-compressed file carrying a valid block checksum is browsed
- **THEN** the file is rewritten with a `zstd+sh` level 19 block and a SHA-1 checksum

#### Scenario: Checksummed zstd file is untouched
- **WHEN** a `zstd` or `zstd+sh` compressed file carrying a block checksum is browsed
- **THEN** the file's bytes on disk are unchanged after the browse
