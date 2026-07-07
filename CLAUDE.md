# XISF File Manager (XFM)

> **Charter:** Always-loaded router + load-bearing gotchas. Kept thin — mechanics, domain, and
> priorities live in the docs below.

Windows Forms (.NET 10) app for managing XISF astrophotography image libraries: bulk FITS-keyword
normalization, canonical renaming, image-block compression on save, and a calibration-frame
library. (Target Scheduler functionality was removed 2026-07-07 — TS is TSM-only; XFM never
touches `scheduler.db`.)

## Doc map

| Doc | Read it for |
|---|---|
| `ARCHITECTURE.md` | Subsystem mechanics: directory layout, XISF/compression handling, keyword contract, conventions, "adding a feature area" template |
| `DOMAIN.md` | Astronomy context: frame types/filters, hardware inventory (cameras/telescopes/software), reject workflow, ecosystem position |
| `ROADMAP.md` | Open follow-ups + recently-shipped digest (git is the changelog) |
| `VERIFICATION.md` | How to verify a change (no test project — build + manual in-app pass) |
| `NOTEBOOK.md` | Lab notebook: chronological empirical findings |
| `RELEASING.md` | dev→main flow, `vX.Y.Z` tag-triggered releases, GitHub push policy |
| `docs/` | Journal: dated records `YYYY-MM-DD-<slug>.md` (investigations, decisions) — discover via glob/grep, not enumerated here. Also holds `FITS Keyword Standards.pdf` (reference asset) |

Portfolio map (sibling repos, shared library, data-flow hubs): `../CLAUDE.md`.

## Build & run

```bash
dotnet build XisfFileManager.sln -c Release
dotnet run --project XisfFileManager/XisfFileManager.csproj
```

## Load-bearing gotchas

- **`XisfFile.FocalRatio` setter self-derives** FOCRATIO from the file's FocalLength/ApertureDiameter,
  ignoring its assigned value (`XisfFile.cs:259`; `KeywordList`'s setter just stores what it's given) —
  FOCALLEN and APTDIA must be written first.
- **`XisfFileUpdate.UpdateFileAsync` is save-if-needed:** `PROTECT` never writes (enforced inside
  the method; deliberate write paths like Calibration/FluxDensity set `FORCE` first); `UPDATE_NEW`
  writes when keywords changed **or** the block is uncompressed; `FORCE` always writes. Saves
  rewrite files in place (temp file + atomic move) — see cautions in `VERIFICATION.md`.
- **`APTAREA` is optimistic:** full circular π·r², obstructions ignored — don't trust it for
  SNR/throughput math on the Newtonian (ROADMAP follow-up #2).
- **Exposure lives in `EXPTIME`:** the `ExposureSeconds` setter writes EXPTIME (dropping any legacy
  `EXPOSURE`); on-disk legacy `EXPOSURE` is converted to EXPTIME and purged at save.
- Naming: `m` private fields, `b` booleans, `e` enums, `Type_Underscore_Names` for controls
  (full conventions in `ARCHITECTURE.md`).

## Excluded from docs governance

`TestData/` (sample inputs), `Archive/` (retired code, archival-only), `bin`/`obj` (generated),
`.claude/`.
