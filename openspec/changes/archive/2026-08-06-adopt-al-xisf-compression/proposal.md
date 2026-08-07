# Proposal: adopt-al-xisf-compression

## Why

XFM's XISF block-compression code (`XisfFileManager\Files\Compression\XisfBlockCompression.cs` + `BlockCompressionInfo.cs`) is a vendored byte-for-byte duplicate of the Library's `Astronomy.XISF.Compression`. AL is shipping full symmetric codec coverage (`xisf-codecs-and-image-read` change in `..\Library`, decided 2026-08-06 — zlib/lz4/lz4hc/zstd ±shuffle, all spec checksums); once that lands, the duplicate is pure liability: two copies to keep conformant, and XFM is cut off from the new codecs. Decision 2026-08-06: retire the vendored copy and consume AL's codec layer directly.

## What Changes

- Delete `XisfFileManager\Files\Compression\` (both vendored files).
- Add XFM's **first AL dependency**: `ProjectReference` to `..\..\Library\Astronomy.XISF\Astronomy.XISF.csproj`; retarget usings/call sites (`XisfFileUpdate.UpdateFileAsync` compression step, `ApplyCompressionAttributes`) to AL's API as shaped by the `xisf-image-read` design.
- Behavior is **unchanged**: same zlib+shuffle+SHA-1 compression of not-already-compressed blocks during file rewrite, same attributes written. Adopting additional codecs (lz4/zstd) for new compressions is a possible later change, not this one.

## Capabilities

### New Capabilities

_None._

### Modified Capabilities

_None — no spec-level behavior changes; `skip_specs: true` is set. `scheduler-independence` is unaffected (AL is not TS; no scheduler data involved)._

## Impact

- **Code**: `Files\Compression\*` deleted; `XisfFileUpdate.cs` (and any other codec call sites) retargeted; csproj gains the `ProjectReference`.
- **Release gate**: `Astronomy.*` DLLs now appear in XFM's payload — the conditional AL-release-ordering gate in XFM's release script **arms itself** (by design, per the portfolio's cross-repo ordering). AL must be published before XFM releases from this point on.
- **Sequencing**: blocked on AL's `xisf-codecs-and-image-read` landing (the API this retargets to). Implement immediately after — drift between the two copies is cheapest at zero.
- **Verification**: build + existing XFM keyword-update/compression paths; a round-trip sanity check that an XFM-rewritten file still reads identically (checksum verifies, PixInsight/NINA can open it).
