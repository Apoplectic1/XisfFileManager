## 1. Save path: keywords only

- [x] 1.1 `XisfFileUpdate.UpdateFileAsync`: remove the `compressNow` branch and
      `ApplyCompressionAttributes` call; the image block is always the verbatim-copy buffer
      (`BinaryDataStart = xFile.TargetAttachmentStart`); delete `ApplyCompressionAttributes`
      and `SetOrRemoveAttribute` if consumerless after this
- [x] 1.2 Save-if-needed gate: drop the `&& xFile.IsImageCompressed` term — `UPDATE_NEW`
      skips purely on `KeywordsMatchXml`; update `eUpdateOutcome` members if
      `Compressed`/`AlreadyCompressed` no longer describe outcomes (check MainForm status
      counts that consume them)
- [x] 1.3 `SetImageAttachmentLocation`: remove the now-unused `newImageSize` parameter
- [x] 1.4 Remove the `Astronomy.XISF.Compression` using / `XisfConstants.CompressionZstdLevel`
      reference from `XisfFileUpdate.cs` if nothing remains that needs them

## 2. Dead code

- [x] 2.1 Delete both `ExtractValue` overloads
- [x] 2.2 Delete the `eBufferData.POSITION` case in `WriteBinaryFileAsync`,
      `Buffer.ToPosition`, and the `POSITION`/`USERDATA` enum members (vocabulary becomes
      ASCII/BINARY/ZEROS)
- [x] 2.3 `XisfFileRename.RenameFile`: drop the unused `Status` tuple member (return the
      written name, or a skipped indicator per 4.2)
- [x] 2.4 `FileSelection.cs` rename loop: delete the no-op `xFile.FilePath` self-assignment

## 3. Correctness + layering

- [x] 3.1 Write the signature XML-length field as full 32-bit little-endian (bytes 8–11),
      retiring the "< 65536" assumption and comment
- [x] 3.2 Replace every `MessageBox` in `XisfFileUpdate.cs` and `XisfFileRename.cs` with
      `Log.Error`/`Log.Warn` + returned status (write failure, rename exception,
      block-alignment warning); verify MainForm surfaces each failure path it previously
      popped (status label / capped dialogs)
- [x] 3.3 Drop `using System.Windows.Forms` from both files once UI-free

## 4. Rename polish

- [x] 4.1 Unify index formatting: all builders use `{index:D3}` + the two-space separator
      convention (byte-identical output to today — verify against an existing filename)
- [x] 4.2 Report a skipped rename (target exists) distinctly instead of claiming the new
      name; completion counts stay honest
- [x] 4.3 Comment the deliberate stale-`FilePath` pattern (rename ends the session —
      `mFileList` is cleared, so in-memory paths are never reused)

## 5. Docs (same commit)

- [x] 5.1 ARCHITECTURE.md: save path no longer "compresses as a backstop" — hygiene is sole
      compression owner; `UpdateFileAsync` described as gate → keyword rebuild → verbatim
      block copy → atomic write
- [x] 5.2 CLAUDE.md gotcha bullet: `UPDATE_NEW` writes on keyword change only
- [x] 5.3 ROADMAP: shipped entry; note the deferred AL compose-XISF primitive beside the #8
      follow-up so it resurfaces there

## 6. Verification

- [x] 6.1 Warning-free `dotnet build XisfFileManager.sln -c Release`
- [ ] 6.2 Manual pass (user): browse → Update Keywords on a real folder — unchanged files
      skip, a keyword-edited file writes with its block byte-identical (verbatim copy);
      rename pass produces identical filenames to before; a forced failure (e.g. locked
      file) reports via status/log, no popup
