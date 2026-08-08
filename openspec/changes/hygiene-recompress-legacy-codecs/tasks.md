## 1. Criterion

- [x] 1.1 `MainForm.ReadHeadersAsync`: hygiene criterion becomes codec-based — enqueue unless
      the block is checksummed `zstd`/`zstd+sh`; update the hygiene comment block and the
      HYGIENE diag line (log the source codec)

## 2. Docs (same commit)

- [x] 2.1 ARCHITECTURE.md: hygiene criterion wording; the "stays zlib / migration forgone"
      bullet reverses (legacy codecs converge incrementally)
- [x] 2.2 DOMAIN.md: "mixed codecs are the accepted end state" paragraph reverses
- [x] 2.3 ROADMAP: shipped-entry amendment (criterion widened; incremental migration accepted)

## 3. Verification

- [x] 3.1 Warning-free `dotnet build XisfFileManager.sln -c Release`
- [ ] 3.2 Manual pass (user): browse a legacy zlib target → files recompress to `zstd+sh`
      (header check), re-browse is instant; first-browse duration roughly matches the ~8–10
      min/target expectation
