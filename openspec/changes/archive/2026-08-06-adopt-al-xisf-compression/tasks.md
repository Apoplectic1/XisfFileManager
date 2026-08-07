# Tasks: adopt-al-xisf-compression

_Design artifact deliberately skipped (conditional): straight dependency swap, no architectural
decisions — the proposal + AL's `xisf-codecs-and-image-read` design carry the rationale._

## 1. Swap

- [x] 1.1 Delete `XisfFileManager\Files\Compression\` (vendored `XisfBlockCompression.cs` + `BlockCompressionInfo.cs`)
- [x] 1.2 Add `ProjectReference` to `..\..\Library\Astronomy.XISF\Astronomy.XISF.csproj` (XFM's first AL dependency)
- [x] 1.3 Retarget usings in `XisfFile.cs`, `XisfFileUpdate.cs`, `Files\XML\Xml.cs` to `Astronomy.XISF.Compression`; confirm call surface unchanged (`Compress(raw, itemSize)` zlib default, `Parse`, `None`, `ToCompressionAttribute`/`ToChecksumAttribute`)

## 2. Docs (same commit)

- [x] 2.1 `ARCHITECTURE.md`: Compression section now points at AL (`Astronomy.XISF.Compression`), notes the two deliberate deltas — `Parse` fails fast on malformed known-codec attributes (load-time `InvalidDataException` instead of lenient zeros) and `ToCompressionAttribute` throws on `Other` (unreachable: only freshly-compressed blocks reach it); enum-location note updated (`BlockCodec` now AL's, with lz4/lz4hc/zstd members XFM doesn't emit)
- [x] 2.2 `ROADMAP`/release note: AL dependency arms the conditional release gate (AL publishes before XFM releases)

## 3. Verify

- [x] 3.1 Build clean (no test project — see `VERIFICATION.md`); behavior identity argument recorded (same zlib+shuffle+SHA-1 bytes; field verification = user's next post-night run)
