# Architecture

> **Charter:** Subsystem mechanics — how XFM works and where code lives. Read before modifying any
> subsystem. Current truth, edited in place; history lives in git.

## Technology stack

- **.NET 10.0** Windows Forms application (Windows 11 SDK 26100)
- **Nullable reference types** enabled (0 warnings)
- **MathNet.Numerics** for scientific calculations
- **Velopack** for release packaging and in-app self-update (`Program.cs` `VelopackApp.Build().Run()`;
  startup `CheckForUpdatesAsync` in `MainForm.cs` pulls from GitHub Releases — see `RELEASING.md`)
- **WindowsAPICodePack** (Core/Shell) for the folder-browse dialogs (`Directories/DirectoryOperations.cs`)
- Timezone handling uses built-in `TimeZoneInfo` (no package)

All planned refactoring phases are complete (.NET 10 upgrade, camera/telescope/capture-software
abstractions, async/await, nullable annotations, CA cleanup). Details preserved in git history.

## Directory structure

```
XisfFileManager/
├── MainForm/           # UI view layer: thin partial classes of MainForm + composition root
│   ├── MainForm.cs     # Constructor (composition root) + Browse load pipeline + UI state
│   ├── Camera.cs / Telescope.cs / CaptureSoftware.cs # Per-feature tab binding
│   ├── ImageType.Detection.cs / .SetActions.cs / .Masters.cs # Filter & frame-type tab
│   └── Calibration.cs / FileSelection.cs / SubFrameKeywords.cs / FluxDensity.cs
├── Models/             # Domain models + shared session state
│   ├── Workspace.cs    # Shared session state (loaded files, image lists, directory stats)
│   ├── CameraConfiguration.cs # Base camera config + PropertyAnalysis<T>
│   ├── TelescopeConfiguration.cs # Base telescope config + TelescopeAnalysis
│   ├── CaptureSoftwareConfiguration.cs # Base capture software config
│   ├── Cameras/        # Z533Camera.cs, Z183Camera.cs, Q178Camera.cs, A144Camera.cs
│   ├── Telescopes/     # APM107Telescope.cs, EvoStar150Telescope.cs, Newtonian254Telescope.cs
│   └── CaptureSoftware/ # NinaSoftware.cs, TheSkyXSoftware.cs, etc.
├── Services/           # Business logic services
│   ├── CameraService.cs # Camera detection and analysis
│   ├── TelescopeService.cs # Telescope detection and analysis
│   └── CaptureSoftwareService.cs # Software detection and analysis
├── Helpers/            # UIHelpers.cs (control manipulation), FileHelpers.cs (file ops)
├── Configuration/      # AppPaths.cs (machine-specific drives), XisfConstants.cs, DirectoryFilters.cs
├── Files/              # XISF file I/O operations
│   ├── XisfFile.cs     # Core XISF file representation
│   ├── XisfXmlReader.cs # XML metadata parsing
│   ├── XisfFileRename.cs / XisfFileUpdate.cs # Renaming / modification+writing
│   ├── Buffer.cs       # Binary buffer operations
│   └── XML/Xml.cs      # XML metadata utilities  (block codec: AL Astronomy.XISF.Compression since 2026-08-06)
├── Keyword/            # Keyword.cs (Name/Value/Comment), KeywordList.cs (typed accessors)
├── Solver/             # AstapSolver.cs + SolveResult (local ASTAP plate solving, read-pass feature)
├── Calibration/        # Calibration frame library
├── Calculations/       # Image statistics and math
├── Directories/        # Directory traversal and properties
├── Utility/            # ToolTip.cs and general-purpose utilities
└── Globals/            # Enums and global constants
```

## XISF file handling

XISF files contain an XML metadata header with FITS-compatible keywords, binary image data
(attachments), and optional thumbnail attachments. (Format background: `DOMAIN.md`.)

XFM treats the image data block as an **opaque byte array** — nothing in the app decodes pixels
(statistics/calculations all come from FITS keywords). On save it compresses an uncompressed block
to `zlib+sh` (plain `zlib`, no shuffle, for 1-byte samples) with a SHA-1 checksum; an
already-compressed block (any codec) is copied verbatim.

### Compression (consumed from AL — `Astronomy.XISF.Compression`)

Since 2026-08-06 (`adopt-al-xisf-compression`) the codec comes from the sibling Library repo via
`ProjectReference` — XFM's **first AL dependency**; the vendored `Files/Compression/` duplicate is
deleted. Tested in AL (`Astronomy.XISF.Tests`), not here.

- `XisfBlockCompression.Compress(raw, itemSize)` — unchanged call and unchanged bytes: byte-shuffle →
  zlib (`SmallestSize` ≈ zlib level 9 ≈ PixInsight "level 100") → SHA-1 over the compressed bytes.
  AL's layer also encodes/decodes lz4 / lz4hc / zstd (±shuffle) and all five XISF checksum
  algorithms; XFM emits only zlib(+sh)+SHA-1 today (`XisfConstants.CompressionCodec`).
- `BlockCompressionInfo` — same parse/emit surface, read at load time into `XisfFile.Compression` /
  `IsImageCompressed`. Two deliberate deltas vs the vendored copy: `Parse` **fails fast**
  (`InvalidDataException`) on a malformed attribute for a known codec (previously lenient zeros —
  a malformed file now refuses to load), and `ToCompressionAttribute()` throws on a foreign
  (`Other`) codec — unreachable here, since XFM only formats attributes for blocks it just
  compressed (`ApplyCompressionAttributes` runs only under `compressNow`).
- Item size (bytes/sample for the shuffle) comes from the `sampleFormat` attribute, falling back to
  `blockLength / (W×H×channels)` from `geometry`. Shuffle is applied when item size > 1 (`zlib+sh`);
  1-byte samples take the no-shuffle branch and write plain `zlib`. A wrong item size only costs
  ratio, never correctness, since it is recorded in the attribute and used on read.
- Always stores the compressed result (even if not smaller) so a block marks itself done and isn't
  re-attempted on every save.

## Diagnostics — xfm.log + Ctrl+N (adopted 2026-08-06)

XFM consumes the portfolio's shared diagnostics contract (`Astronomy.Diagnostics` +
`Astronomy.Diagnostics.WinForms` — third/fourth AL references): `Log.Init` in `Program.cs` writes
`%APPDATA%\XisfFileManager\Logs\xfm.log` (rotated to `.prev` each run; `XFM_DIAG` env var gates the
diag channels, default all-Debug/none-Release), and **Ctrl+N** opens the shared observation dialog
(`DiagnosticsDialog.ShowOrFocus` in `MainForm.ProcessCmdKey`) — USER_OBS_START/CAP/END/CANCEL
markers + screenshots under `Logs\screenshots\`, with `GetDiagnosticsContext()` stamping loaded-file
count, tab, and checkbox states into the END line. Instrumented so far: the Browse read pass
(bracket Info lines) and the solver (`Log.Info` per solve outcome/duration, `Log.Error` on failures,
gated `Diag("SOLVER", …)` with CLI args, raw `.ini`, and — on failure/timeout — the tail of ASTAP's
own `-log` output, which carries the star counts and search progress `PLTSOLVD=F` omits).
Convention: new error paths get a `Log.Error`
twin beside any MessageBox so dialogs are never the only record.

## Keywords

### Two-tier structure (verified 2026-08-06)

Both tiers live in `Keyword/` — cohabiting `KeywordList`, not split into two classes:

- **Low tier** — `Keyword` (Name/Value/Comment POCO) plus the raw triple CRUD on `mKeywordList`:
  `AddKeyword` / `GetKeyword` / `GetKeywordValue` / `GetKeywordComment` / `RemoveKeyword` /
  `AddKeywordKeepDuplicates`. String-in/string-out against the actual keywords.
- **High tier** — the ~35 typed properties (`ExposureSeconds`, `Gain`, `CaptureTime`, `FilterName`,
  `RotatorSkyAngle`, …) that parse/format quantities and call the low tier in both directions, plus
  workflow ops (`NormalizeExposure`, `NormalizeCaptureTime`, `SetMasterFrameKeywords`,
  `RemoveUnwantedKeywords`). Getters return sentinels when the keyword is absent
  (`double.MinValue`, `-1`, `0.0` — see the 2026-07-07 sentinel audit in `NOTEBOOK.md`).

**Migration direction (2026-08-06):** this pipeline is the named target consumer of AL's future
property-first metadata layer (Library `ROADMAP.md` → Tier 2): PixInsight is shifting from FITS
keywords toward typed XISF properties, and AL will model properties as the primary surface with
FITS keywords as a compatibility projection. The high tier is the migration **choke point** — new
features (e.g. the ASTAP solve stamp) should write through the typed properties / high tier so the
eventual swap re-plumbs one layer instead of rewriting features.

Keywords follow FITS conventions with Name/Value/Comment triplets. Keywords XFM reads/writes
(astronomy semantics behind them: `DOMAIN.md`):

- `IMAGETYP`: Frame type (Light, Dark, Flat, Bias)
- `FILTER`: Filter name — stored as L, R, G, B, H, O, S, Shutter (inbound capture-software spellings
  like Ha/OIII/SII are collapsed to the single letter by the `FilterName` setter)
- `EXPTIME`: Exposure time in seconds (standard). The `ExposureSeconds` setter writes EXPTIME and
  removes any legacy `EXPOSURE`; `NormalizeExposure` converts a legacy-only `EXPOSURE` to EXPTIME
  at save and purges it
- `CCD-TEMP`: Sensor temperature
- `OBJECT`: Target name
- `SWCREATE`: Capture software (NINA, TSX, SGP, VOY, SCP)
- `FOCALLEN`: Focal length in mm (reducer-aware; from the selected telescope)
- `FOCRATIO`: Focal ratio, derived as FOCALLEN ÷ APTDIA (reducer-aware)
- `APTDIA`: Aperture diameter in mm (hardcoded per telescope)
- `APTAREA`: Aperture area in mm² (full circle π·r²; obstructions ignored — see gotcha in `CLAUDE.md`)

Telescope keywords (`TELESCOP`, `FOCALLEN`, `APTDIA`, `APTAREA`, `FOCRATIO`) are all written by
`TelescopeConfiguration.ApplyKeywords`, invoked from the Telescope tab's Set All / Set By File buttons.

### Solved-solution keywords (astap-plate-solve, 2026-08-06)

When the Directory Selection **Solver** checkbox is checked, the Browse read pass plate-solves every
**light** frame (masters excluded — their own checkbox path; calibration never) with the local ASTAP
CLI and stamps the measured solution into the in-memory `KeywordList` via the high-tier
`SetPlateSolution`: `RA`/`DEC` (degrees, frame centre), `OBJCTROT` (**measured** position angle —
replaces NINA's planned value), `CTYPE1/2` (`RA---TAN`/`DEC--TAN`), `EQUINOX`, `CRVAL1/2`,
`CRPIX1/2`, `CD1_1..CD2_2`, `CROTA1/2`. Persistence rides the normal save step; solver stamps are
not subject to UPDATE_NEW-vs-FORCE (solved values always win — disk is truth); file-level PROTECT
still refuses the save entirely. None of these names may enter `RemoveUnwantedKeywords`.

Mechanics (`Solver/AstapSolver.cs`): uncompressed XISF → `astap_cli -f` directly (read-only);
compressed → AL `XisfImageReader` → minimal temp mono FITS (UInt16 only; else per-file failure).
Hints from AL `XisfHeaderReader` (`-ra` RA/15, `-spd` Dec+90, `-fov` field height; blind `-r 180`
fallback); always `-o <temp>` so `.ini`/`.wcs` never land beside library images; 60 s timeout;
`.ini` `PLTSOLVD` gate with ASTAP's exit-code table as fallback messages. Position angle = AL
`WcsOrientation.FromCdMatrix` + the ASTAP convention bridge (`360 − (Rotation − 180)`, parity
inverted — NINA `ASTAPSolver`'s bridge, deliberately kept out of AL's generic math). A failed solve
stamps nothing, is reported in a single post-browse summary, and never stops the pass; a missing
ASTAP install refuses a checked browse up front (expected at `XisfConstants.AstapCliPath`).

### Enums (`Globals/Globals.cs`)

- `eFrame`: LIGHT, DARK, FLAT, BIAS, ALL, EMPTY
- `eFilter`: L, R, G, B, H, O, S, SHUTTER, ALL, EMPTY
- `eOrder`: File ordering (INDEX, WEIGHT, WEIGHTINDEX, INDEXWEIGHT)
- `eKeywordUpdateMode`: PROTECT, UPDATE_NEW, FORCE
- `eUpdateOutcome`: result reporting for `XisfFileUpdate.LastUpdateOutcome`
- `eMessageMode`, `eBufferData`, `eUiState`: messaging/buffer/UI state
- Two enums live outside Globals.cs, both without the `e` prefix: `BlockCodec` (AL's
  `Astronomy.XISF.Compression` since 2026-08-06; carries lz4/lz4hc/zstd members XFM never emits) and
  `ExcludeType` (`Directories/DirectoryOperations.cs`)

## Code conventions

### Naming

- Private fields: `m` prefix (e.g., `mFileList`, `mCalibration`)
- Boolean fields: `b` prefix (e.g., `bModified`, `mBCancel`)
- UI Controls: Type prefix with underscore separators (e.g., `Label_FileSelection_Statistics_OperationStatus`)
- Enums: `e` prefix (e.g., `eFrame`, `eFilter`)

### Patterns

- MainForm uses partial classes split across feature files (thin UI binding; logic lives in `Services/`)
- Shared session state lives on `Models/Workspace.cs` (`mWorkspace`), not bare MainForm fields
- The Browse handler is a named-stage pipeline: `ResetSession → TrySelectSourceFolder →
  ReadHeadersAsync → PopulateUiFromFiles → RefreshFeatureDetection → BuildTargetFileTree`
- Event-driven UI updates via delegates (e.g., `CalibrationTabPageEvent`)
- Keyword properties on XisfFile delegate to KeywordList
- `XisfFileUpdate.UpdateFileAsync` is **save-if-needed**: `PROTECT` never writes; `UPDATE_NEW` writes
  when keywords changed **or** the block is uncompressed; `FORCE` always writes. It always
  re-serializes the XML header and either compresses an uncompressed block or copies an
  already-compressed one verbatim. `LastUpdateOutcome` reports the result for status counts
  (`Protected` = refused by the PROTECT gate). The PROTECT guard lives inside `UpdateFileAsync`;
  deliberate write paths (Calibration masters, FluxDensity export copies) set `FORCE` before calling.

### Important files

- `Models/Workspace.cs`: Shared session state (loaded files, image lists, directory stats); exposed
  on MainForm as `mWorkspace` and read by every feature partial
- `Files/XisfFile.cs`: Central model — all keyword access flows through here
- `Keyword/KeywordList.cs`: Typed property accessors for common FITS keywords. Note: the
  discard-and-recompute FOCRATIO gotcha lives one layer up — `XisfFile.FocalRatio`'s setter ignores
  its assigned value and recomputes FocalLength ÷ ApertureDiameter (`XisfFile.cs:259`); KeywordList's
  own setter stores the value it's given. FOCALLEN and APTDIA must be written first either way.
- `Models/CameraConfiguration.cs` / `Services/CameraService.cs`: camera config base + detection,
  property analysis, UI color helpers. Header/label color convention: red = missing/unresolved
  values, green = legitimately different values (e.g. multi-camera), black = uniform;
  `AnalyzeCameraResolution` drives the Camera-identity header. The Seconds analysis is
  presence-based (`HasExposure`) because the exposure getter's missing-value sentinel is 0.0,
  which collides with genuine 0 s bias frames
- `Models/TelescopeConfiguration.cs` / `Services/TelescopeService.cs`: telescope config base with
  reducer support (`ApplyKeywords` emits TELESCOP/FOCALLEN/APTDIA/APTAREA and triggers FOCRATIO) +
  detection, analysis, UI color helpers
- `Models/CaptureSoftwareConfiguration.cs` / `Services/CaptureSoftwareService.cs`: capture software
  config base + detection and analysis
- `Files/Compression/XisfBlockCompression.cs` + `BlockCompressionInfo.cs`: pure, UI-free `zlib+sh` +
  SHA-1 image-block codec and its attribute parser/formatter (see Compression above)
- `Helpers/UIHelpers.cs`: Common UI control manipulation (ClearComboBox, ResetRadioButton, etc.)
- `MainForm.Designer.cs`: Auto-generated UI — TabIndex values manually fixed for proper navigation
- `Globals/Globals.cs`: Shared enums and global constants (two enums live elsewhere — see Enums above)
- `Configuration/AppPaths.cs`: Machine-specific paths (E:\, F:\ drives)
- `Configuration/XisfConstants.cs`: XISF signature size, max buffer size, and compression/checksum
  codec names (`zlib+sh`, plain-`zlib` fallback for 1-byte samples, `sha-1`)
- `Configuration/DirectoryFilters.cs`: Exclude lists for directory filtering

## Common tasks

### Adding a new keyword

1. Add property to `KeywordList.cs` with getter/setter
2. Optionally expose through `XisfFile.cs` as a delegating property

### Adding a UI feature

1. Add controls in Visual Studio Designer (updates MainForm.Designer.cs)
2. Create new partial class file in MainForm/ for logic
3. Wire up events in MainForm.cs constructor

### Adding a new feature area

Follow the **Telescope** feature as the template (`Services/TelescopeService.cs` +
`Models/TelescopeConfiguration.cs` + `MainForm/Telescope.cs`). Four parts, four places:

1. **Logic** → `Services/<Feature>Service.cs`: pure and UI-free; takes domain data (e.g.
   `IEnumerable<XisfFile>`) and returns a `<Feature>Analysis` result type defined in `Models/`.
2. **UI binding** → a thin `MainForm/<Feature>.cs` partial: control↔model mappings,
   `Find<Feature>()` / `Clear<Feature>Group()`, and the button handlers. Call the service for every
   decision; use `Helpers/UIHelpers.cs` for control resets.
3. **Shared state** → read from `mWorkspace` (`Models/Workspace.cs`), never new MainForm fields. If
   the feature needs new session data, add a member to `Workspace`.
4. **Construction & wiring** → the `MainForm` constructor only (the composition root). If the
   feature reacts to a file load, add `Clear<Feature>Group()` to `ResetSession()` and
   `Find<Feature>()` to `RefreshFeatureDetection()` (both in `MainForm.cs`).
