# refactor-file-writers

## Why

`XisfFileUpdate.cs` and `XisfFileRename.cs` carry sediment from three retired eras: dead
utilities with zero callers, a save-time compression branch that browse-hygiene made
near-dead (every file reaching a save was already compressed during the browse — the branch
serves only the hygiene-failed tail, which the next browse repairs anyway), pre-Diagnostics
`MessageBox` calls in the file layer, and a latent corruption bug (the save writes the XML
length into only 2 signature bytes — a keyword-heavy header crossing 64 KiB would corrupt
silently; AL's rewriter already writes the spec's 4-byte field). Explored 2026-08-07
(in-conversation); this lands as mechanical cleanup *before* ROADMAP #8 redesigns the same
method's mode semantics.

## What Changes

- **Save path owns keywords only** — the `compressNow` branch, `ApplyCompressionAttributes`,
  the fresh-bytes buffer arm, and `SetImageAttachmentLocation`'s size parameter are removed;
  the image block is always copied verbatim. The save-if-needed gate drops its
  `IsImageCompressed` term (`UPDATE_NEW` skips purely on keyword match). **Behavioral,
  accepted:** a file whose hygiene rewrite failed saves uncompressed; the next browse repairs
  it — compression has exactly one owner (browse hygiene).
- **Dead code deleted:** `ExtractValue` (both overloads), `eBufferData.POSITION` write branch +
  `Buffer.ToPosition` + `USERDATA` enum member, `RenameFile`'s unused `Status` return, the
  rename loop's no-op `FilePath` self-assignment.
- **4-byte XML-length fix:** the signature's length field is written as full 32-bit
  little-endian (parity with AL `XisfBlockRewriter`), retiring the "< 65536" assumption.
- **File layer goes UI-free:** `MessageBox` calls in both files (rename exception, write
  failure, POSITION error, block-alignment warning) become `Log.Error`/`Log.Warn` + returned
  status — MainForm already surfaces failures. **Behavioral, accepted:** errors report via
  `xfm.log` + status line instead of modal popups.
- **Rename polish:** index formatting mechanism unified across builders (identical output);
  a skipped rename (target name already exists) is reported honestly instead of claiming the
  new name; a comment documents the deliberate stale-`FilePath`-because-rename-ends-the-session
  pattern.
- **Deferred, recorded here:** an AL "compose monolithic XISF (xmlText, block)" primitive that
  would absorb `WriteBinaryFileAsync`'s buffer machinery — real scope, new AL API; revisit when
  a second consumer appears or at ROADMAP #8 if it wants it.

## Capabilities

### New Capabilities

*None.*

### Modified Capabilities

*None — no capability spec covers the save/rename mechanics; `skip_specs: true`. The two
deliberate behavior changes (no save-time compression backstop; popups → log) are stated
above and ride the reference docs.*

## Impact

- **XFM only**: `Files/XisfFileUpdate.cs` (~140 lines removed), `Files/XisfFileRename.cs`,
  `Files/Buffer.cs`, `Globals/Globals.cs` (`eBufferData`), `MainForm/FileSelection.cs`
  (rename loop). No AL changes.
- **Docs**: ARCHITECTURE.md — save-path description loses the "compresses as a backstop"
  wording (hygiene is sole owner; `UpdateFileAsync` bullet in CLAUDE.md gotchas updates
  likewise: `UPDATE_NEW` writes on keyword change only); ROADMAP shipped entry.
- **Interaction with ROADMAP #8**: deliberately sequenced first; #8's PROTECT/FORCE redesign
  then operates on a file whose only remaining jobs are gate → keyword rebuild → write.
