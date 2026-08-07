# Verification

> **Charter:** How to verify a change in this repo. Honest status: there is **no automated test
> project** — verification is build + manual exercise of the running app.

## Build (code-correct)

```bash
dotnet build XisfFileManager.sln -c Release
```

Pure-managed solution — `dotnet build` is sufficient (no `.vcxproj` in the graph). Nullable is
enabled repo-wide; the bar is **0 warnings — and enforced**: `<TreatWarningsAsErrors>` since 2026-08-01
(portfolio-wide ratchet), so a new warning is a build break. Fix it, or — rarely, with a comment —
suppress it deliberately; never turn the ratchet off.

## Run (feature-correct)

```bash
dotnet run --project XisfFileManager/XisfFileManager.csproj
```

A clean build proves compile-correctness only. UI/behavioral changes need a manual pass in the
running app — typically: Browse to a folder of real XISF files, exercise the affected tab, and
Update Keywords to confirm the save path. Sample inputs live in `TestData/`
(`Unit16_0s_200x200.xisf`, a NINA profile). State explicitly what was build-verified vs. what
still needs a human in-app check.

## Cautions

- Keyword updates **rewrite XISF files in place** (temp file + atomic move). Test against copies or
  `TestData/`, not the live image library, unless the write is the thing being verified.
- **Browse itself writes since 2026-08-07** (compression hygiene): browsing a folder rewrites any
  uncompressed or checksum-less file in place. Point Browse at copies when the write side effect is
  unwanted during testing — there is no opt-out checkbox by design.

## Manual pass: browse-time compression hygiene (2026-08-07)

Against a **copy** of a mixed folder (fresh uncompressed + legacy zlib-no-checksum + legacy
zlib+checksum), verify:

1. Browse → uncompressed and no-checksum files rewritten `zstd+sh(19)` + SHA-1 (header shows
   `compression="zstd+sh:…"` `checksum="sha-1:…"`); a compressed+checksummed zlib file is
   byte-identical; one rewritten file **opens in PixInsight**.
2. Statistics line shows `Hygiene N Rewritten`; a Verify-SHA re-browse reports 100% verified.
3. Cancel mid-pass → UI restored, files read so far kept; re-browse repairs only the remainder.
4. Hygiene-then-save: Update Keywords on a rewritten file in the same session → valid file
   (checks the in-memory geometry refresh — a corrupt output here means a stale block offset).
5. Solver on a compressed backlog frame → solves via temp `.xisf` (no `.fit` in `%TEMP%`, no
   solver files beside library images); a fresh **uncompressed** light solves in place
   (exercises the `astap.exe` switch — `astap_cli` could never read XISF).

## CI

No CI test/build gate. The only workflow is `.github/workflows/release.yml`, tag-triggered on
`vX.Y.Z` (see `RELEASING.md`).
