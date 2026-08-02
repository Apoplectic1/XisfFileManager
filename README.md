# XISF File Manager

A Windows Forms (.NET 10) desktop application for managing libraries of XISF (Extensible Image
Serialization Format) astrophotography images.

## What it does

- Bulk-reads XISF metadata and normalizes/repairs FITS keywords (camera, telescope, capture
  software, exposure) across an image library
- Renames files to a canonical scheme (with `REJECT` flagging of stacked-out subframes)
- Compresses image blocks on save to the PixInsight/NINA compressed format (zlib + SHA-1)
- Maintains a calibration-frame library

## Install or build

Releases are Velopack installers published as GitHub Release assets — download the latest from the
Releases page; installed copies self-update at startup. Or build from source:

```bash
dotnet build XisfFileManager.sln -c Release
dotnet run --project XisfFileManager/XisfFileManager.csproj
```

Requires the .NET 10 SDK on Windows. Releases are tagged `vX.Y.Z` and packaged by GitHub Actions
(see `RELEASING.md`).

## Documentation

`CLAUDE.md` is the doc router: `ARCHITECTURE.md` (mechanics), `DOMAIN.md` (astronomy context),
`ROADMAP.md` (priorities), `VERIFICATION.md` (how to verify a change), `RELEASING.md` (releases),
`NOTEBOOK.md` (lab notebook), `docs/` (dated notes).

## License

[MIT](LICENSE)
