## Purpose

Automatic library hygiene during the Browse read pass: any image whose block is uncompressed or
lacks a checksum is repaired in place — compressed as `zstd+sh` level 19 and stamped with a SHA-1
block checksum — so the library converges to fully compressed, fully verifiable without a
save-mode decision or a migration pass.

## ADDED Requirements

### Requirement: Hygiene criterion and remedy
During the Browse read pass, every file whose image block is **uncompressed or lacks a block
checksum** SHALL be rewritten in place with the block compressed as `zstd+sh` level 19 and a
SHA-1 block checksum recorded. A file whose block is already compressed **and** checksummed SHALL
NOT be touched, regardless of codec — mixed codecs are the accepted end state; hygiene performs
no bulk migration. A compressed block lacking only a checksum SHALL take the same remedy
(decode + recompress as `zstd+sh` level 19 + checksum), not a checksum-only patch. The fresh
checksum certifies the bytes as written by the rewrite ("intact from now on") and carries no
claim about the block's history.

#### Scenario: Fresh uncompressed file is repaired
- **WHEN** a directory containing a freshly captured uncompressed XISF is browsed
- **THEN** the file on disk is rewritten with a `zstd+sh` level 19 block and a SHA-1 checksum

#### Scenario: Legacy compressed file without checksum is repaired
- **WHEN** a zlib+shuffle-compressed file lacking a block checksum is browsed
- **THEN** the file is rewritten with a `zstd+sh` level 19 block and a SHA-1 checksum

#### Scenario: Compressed and checksummed zlib file is untouched
- **WHEN** a zlib+shuffle-compressed file carrying a valid block checksum is browsed
- **THEN** the file's bytes on disk are unchanged after the browse

### Requirement: Always on, exempt from keyword-write protection
Hygiene SHALL run on every Browse with no enabling checkbox and no opt-out. It SHALL be exempt
from the keyword-write protection mode: a session or file marked protected still receives
hygiene rewrites; protection retains its meaning of "no keyword writes". A hygiene rewrite SHALL
NOT add, remove, or alter any FITS keyword.

#### Scenario: Protected file still gets hygiene
- **WHEN** an uncompressed file whose keyword-update mode is PROTECT is browsed
- **THEN** the file is rewritten compressed+checksummed and its keywords are byte-identical

### Requirement: Surgical rewrite semantics
A hygiene rewrite SHALL preserve the file's XISF XML header byte-for-byte except the image
block's `compression`, `checksum`, and `location` attributes (and the header-length bookkeeping
those changes force), SHALL replace only the image block bytes, and SHALL write via a temporary
file finalized by an atomic replace — at no point may the library file exist in a partially
written state. The rewritten file SHALL be readable by standard XISF consumers (PixInsight,
NINA-era readers with zstd support, the shared library reader). Operations later in the same
session (keyword save, checksum verify, solve) SHALL behave correctly against the rewritten
file — the session's in-memory picture of the file's geometry is refreshed by the rewrite.

#### Scenario: Hygiene then keyword save in one session
- **WHEN** a file is hygiene-rewritten during Browse and subsequently saved through the normal
  keyword-update path in the same session
- **THEN** the saved file is a valid XISF carrying both the compressed+checksummed block and the
  keyword changes

#### Scenario: Interrupted rewrite leaves the original intact
- **WHEN** a hygiene rewrite is interrupted before completion (crash, power loss)
- **THEN** the library file on disk is the original, unmodified file

### Requirement: Solve precedes compression
For a light frame that the checked solver pass will solve, the solve SHALL complete before that
file's hygiene rewrite begins — an uncompressed light is handed to the solver in place and MUST
NOT be compressed out from under it. Hygiene SHALL NOT persist solver stamps; solved keywords
ride the normal save path unchanged.

#### Scenario: Fresh light solves in place, then compresses
- **WHEN** an uncompressed light frame is browsed with the solver checkbox checked
- **THEN** the solver reads the uncompressed file directly, and the hygiene rewrite happens
  after the solve completes, containing no solver keywords

### Requirement: Bounded concurrency with a browse-completion barrier
Hygiene rewrites SHALL run concurrently on a bounded worker pool, off the UI thread — the UI
SHALL remain responsive (repaint, progress, cancel) while rewrites are in flight. The pool is
browse-scoped: Browse SHALL NOT report completion (and no post-browse operation may start)
until every issued rewrite has completed or been abandoned. No hygiene write SHALL occur after
the browse pass has completed.

#### Scenario: Browse completion waits for in-flight rewrites
- **WHEN** header reading finishes while hygiene rewrites are still running
- **THEN** the browse completes (UI re-enabled, results populated) only after the last rewrite
  finishes

### Requirement: Cancelable and resumable
The Browse pass SHALL be cancelable during hygiene. On cancel, in-flight rewrites complete
their atomic finalization, queued rewrites are abandoned, and the browse ends cleanly with the
UI restored. Files already repaired stay repaired; hygiene is idempotent — a subsequent Browse
of the same directory repairs only the files still failing the criterion.

#### Scenario: Cancel mid-pass, browse again
- **WHEN** a browse over many unhygienic files is canceled partway and the directory is browsed
  again
- **THEN** the second browse rewrites only the files not repaired before the cancel, and no file
  is rewritten twice

### Requirement: Progress and reporting
During hygiene the browse UI SHALL indicate the file currently being rewritten and overall
progress. On completion (or cancel) the browse status SHALL report counts of files rewritten
and failed, and each rewrite SHALL be logged; failures are logged as errors naming the file and
cause.

#### Scenario: Counts reported after the pass
- **WHEN** a browse rewrites some files and one rewrite fails
- **THEN** the completion status reports the rewritten and failed counts and the log carries a
  per-file error for the failure

### Requirement: Failure semantics
A failed hygiene rewrite (locked file, I/O error, unparseable block geometry) SHALL leave the
original file untouched, be reported per file, and SHALL NOT stop the browse pass — remaining
files are still read and repaired.

#### Scenario: One locked file doesn't stop the pass
- **WHEN** one file in a browsed directory is locked by another process during its rewrite
- **THEN** that file is reported as failed and unchanged on disk, and the remaining files are
  read and repaired normally
