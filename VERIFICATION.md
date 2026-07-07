# Verification

> **Charter:** How to verify a change in this repo. Honest status: there is **no automated test
> project** — verification is build + manual exercise of the running app.

## Build (code-correct)

```bash
dotnet build XisfFileManager.sln -c Release
```

Pure-managed solution — `dotnet build` is sufficient (no `.vcxproj` in the graph). Nullable is
enabled repo-wide; the bar is **0 warnings**.

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

## CI

No CI test/build gate. The only workflow is `.github/workflows/release.yml`, tag-triggered on
`vX.Y.Z` (see `RELEASING.md`).
