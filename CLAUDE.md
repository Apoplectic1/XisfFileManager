# XISF File Manager

A Windows Forms desktop application for managing XISF (Extensible Image Serialization Format) files used in astrophotography.

Active priorities and recently-shipped work: see `ROADMAP.md`.

## Refactoring Status

All planned refactoring phases complete (.NET 10 upgrade, camera/telescope/capture-software abstractions, async/await, nullable annotations, CA cleanup). Details preserved in git history.

**Cameras supported:** Z533, Z183, Q178, A144 (all active)
**Telescopes supported:** APM107, EvoStar150, Newtonian254 (with Riccardi 0.75x reducer support)
**Capture Software supported:** NINA, TheSkyX, SGPro, Voyager, SharpCap

## Build Commands

```bash
# Build the solution
dotnet build XisfFileManager.sln -c Release

# Build for specific platform (project defaults to x64)
dotnet build XisfFileManager.sln -c Release -r win-x64

# Run the application
dotnet run --project XisfFileManager/XisfFileManager.csproj
```

## Branches & Releases

- **`dev`** — active development branch (default; new work lands here).
- **`main`** — stable/release branch; merge `dev` into `main` when cutting a release.
- Releases are tag-triggered: pushing an annotated `vX.Y.Z` tag runs `.github/workflows/release.yml`, which injects the tag into `AssemblyInformationalVersion` (shown in the window title). Latest released tag: `v1.8.0`.

## Project Architecture

### Technology Stack
- **.NET 10.0** Windows Forms application (Windows 11 SDK 26100)
- **Nullable reference types** enabled (0 warnings)
- **SQLite** via Microsoft.Data.Sqlite for Target Scheduler database
- **MathNet.Numerics** for scientific calculations
- **GeoTimeZone/TimeZoneConverter** for timezone handling

### Directory Structure
```
XisfFileManager/
├── MainForm/           # UI view layer: thin partial classes of MainForm + composition root
│   ├── MainForm.cs     # Constructor (composition root) + Browse load pipeline + UI state
│   ├── Camera.cs / Telescope.cs / CaptureSoftware.cs # Per-feature tab binding
│   ├── ImageType.Detection.cs / .SetActions.cs / .Masters.cs # Filter & frame-type tab
│   ├── TargetScheduler.Tree.cs / .Events.cs + CustomTreeView.cs # Scheduler tab
│   └── Calibration.cs / FileSelection.cs / SubFrameKeywords.cs / FluxDensity.cs
├── Models/             # Domain models + shared session state
│   ├── Workspace.cs    # Shared session state (loaded files, image lists, directory stats)
│   ├── CameraConfiguration.cs # Base camera config + PropertyAnalysis<T>
│   ├── TelescopeConfiguration.cs # Base telescope config + TelescopeAnalysis
│   ├── CaptureSoftwareConfiguration.cs # Base capture software config
│   ├── Cameras/        # Camera-specific configurations
│   │   └── Z533Camera.cs, Z183Camera.cs, Q178Camera.cs, A144Camera.cs
│   ├── Telescopes/     # Telescope-specific configurations
│   │   └── APM107Telescope.cs, EvoStar150Telescope.cs, Newtonian254Telescope.cs
│   └── CaptureSoftware/ # Capture software configurations
│       └── NinaSoftware.cs, TheSkyXSoftware.cs, etc.
├── Services/           # Business logic services
│   ├── CameraService.cs # Camera detection and analysis
│   ├── TelescopeService.cs # Telescope detection and analysis
│   └── CaptureSoftwareService.cs # Software detection and analysis
├── Helpers/            # UI and utility helpers
│   ├── UIHelpers.cs    # Common control manipulation methods
│   └── FileHelpers.cs  # File operation utilities
├── Configuration/      # Centralized constants and paths
│   ├── AppPaths.cs     # Machine-specific drive paths
│   ├── XisfConstants.cs # XISF format constants (signature size, buffer size)
│   └── DirectoryFilters.cs # Directory exclude lists
├── Data/               # Database infrastructure
│   ├── ITableMapper.cs # Generic mapper interface
│   ├── SqliteReaderExtensions.cs # Null-safe reading helpers
│   └── TableMappers.cs # Mappers for all 8 Target Scheduler tables
├── Files/              # XISF file I/O operations
│   ├── XisfFile.cs     # Core XISF file representation
│   ├── XisfXmlReader.cs # XML metadata parsing
│   ├── XisfFileRename.cs # File renaming logic
│   ├── XisfFileUpdate.cs # File modification/writing
│   ├── Buffer.cs       # Binary buffer operations
│   ├── Compression/    # Image-block codec (zlib+sh + SHA-1)
│   │   ├── XisfBlockCompression.cs # Shuffle/zlib/SHA-1 compress+decompress
│   │   └── BlockCompressionInfo.cs # compression/checksum attribute parse+format
│   └── XML/            # XML processing
│       └── Xml.cs      # XML metadata utilities
├── Keyword/            # FITS/XISF keyword handling
│   ├── Keyword.cs      # Single keyword (Name, Value, Comment)
│   └── KeywordList.cs  # Keyword collection with typed accessors
├── Calibration/        # Calibration frame library
├── TargetScheduler/    # N.I.N.A. Target Scheduler SQLite integration
│   ├── SqlLiteManager.cs   # Database orchestration
│   ├── SqlLiteReader.cs    # Read operations
│   ├── SqlLiteWriter.cs    # Write operations
│   └── Tables/         # Database table models
├── Calculations/       # Image statistics and math
├── Directories/        # Directory traversal and properties
├── Utility/            # General-purpose utilities
│   └── ToolTip.cs      # ToolTip helpers
└── Globals/            # Enums and global constants
```

## Key Concepts

### XISF Files
XISF (Extensible Image Serialization Format) is an astronomy image format from PixInsight. Files contain:
- XML metadata header with FITS-compatible keywords
- Binary image data (attachments)
- Optional thumbnail attachments

XFM treats the image data block as an **opaque byte array** — nothing in the app decodes pixels
(statistics/calculations all come from FITS keywords). On save it compresses an uncompressed block
to `zlib+sh` with a SHA-1 checksum (see Compression below); an already-compressed block (any codec)
is copied verbatim.

### Compression (`Files/Compression/`)
- `XisfBlockCompression` — pure, UI-free codec: byte-shuffle → `System.IO.Compression.ZLibStream`
  (`SmallestSize` ≈ zlib level 9 ≈ PixInsight "level 100") → SHA-1 over the compressed bytes.
  `Compress`/`Decompress` are symmetric; `Decompress` is present for tests and future pixel I/O but
  is not yet wired into any runtime path (XFM blocks are opaque).
- `BlockCompressionInfo` — parses/formats the `compression="zlib+sh:size:itemSize"` and
  `checksum="sha-1:hex"` attributes; read at load time into `XisfFile.Compression` / `IsImageCompressed`.
- Item size (bytes/sample for the shuffle) comes from the `sampleFormat` attribute, falling back to
  `blockLength / (W×H×channels)` from `geometry`. Shuffle is always written (`zlib+sh`); a wrong item
  size only costs ratio, never correctness, since it is recorded in the attribute and used on read.
- Always stores the compressed result (even if not smaller) so a block marks itself done and isn't
  re-attempted on every save.

### Keywords
Keywords follow FITS conventions with Name/Value/Comment triplets:
- `IMAGETYP`: Frame type (Light, Dark, Flat, Bias)
- `FILTER`: Filter name (L, R, G, B, Ha, OIII, SII)
- `EXPTIME`: Exposure time in seconds (standard; legacy `EXPOSURE` is normalized to this and purged)
- `CCD-TEMP`: Sensor temperature
- `OBJECT`: Target name
- `SWCREATE`: Capture software (NINA, TSX, SGP, VOY, SCP)
- `FOCALLEN`: Focal length in mm (reducer-aware; from the selected telescope)
- `FOCRATIO`: Focal ratio, derived as FOCALLEN ÷ APTDIA (reducer-aware)
- `APTDIA`: Aperture diameter in mm (hardcoded per telescope)
- `APTAREA`: Aperture area in mm² (full circle π·r²; obstructions ignored — see note below)

Telescope keywords (`TELESCOP`, `FOCALLEN`, `APTDIA`, `APTAREA`, `FOCRATIO`) are all
written by `TelescopeConfiguration.ApplyKeywords`, invoked from the Telescope tab's
Set All / Set By File buttons. `APTAREA` uses the full circular aperture area and
deliberately ignores central/secondary-mirror obstructions, so the Newtonian's value
is optimistic — revisit before trusting it for light-gathering/SNR math.

### Enums (Globals/Globals.cs)
- `eFrame`: LIGHT, DARK, FLAT, BIAS, ALL, EMPTY
- `eFilter`: L, R, G, B, H, O, S, SHUTTER, ALL, EMPTY
- `eOrder`: File ordering (INDEX, WEIGHT, WEIGHTINDEX, INDEXWEIGHT)
- `eKeywordUpdateMode`: PROTECT, UPDATE_NEW, FORCE

### Target Scheduler Integration
Reads/writes N.I.N.A. Target Scheduler SQLite database:
- Projects, Targets, Exposure Plans
- Acquired Images tracking
- Profile Preferences

## Code Conventions

### Naming
- Private fields: `m` prefix (e.g., `mFileList`, `mCalibration`)
- Boolean fields: `b` prefix (e.g., `bModified`, `mBCancel`)
- UI Controls: Type prefix with underscore separators (e.g., `Label_FileSelection_Statistics_OperationStatus`)
- Enums: `e` prefix (e.g., `eFrame`, `eFilter`)

### Patterns
- MainForm uses partial classes split across feature files (thin UI binding; logic lives in `Services/`)
- Shared session state lives on `Models/Workspace.cs` (`mWorkspace`), not bare MainForm fields
- The Browse handler is a named-stage pipeline: `ResetSession → TrySelectSourceFolder → ReadHeadersAsync → PopulateUiFromFiles → RefreshFeatureDetection → BuildTargetFileTree`
- Event-driven UI updates via delegates (e.g., `CalibrationTabPageEvent`)
- Keyword properties on XisfFile delegate to KeywordList
- `XisfFileUpdate.UpdateFileAsync` is **save-if-needed**: `PROTECT` never writes; `UPDATE_NEW` writes when keywords changed **or** the block is uncompressed; `FORCE` always writes. It always re-serializes the XML header and either compresses an uncompressed block or copies an already-compressed one verbatim. `LastUpdateOutcome` reports the result for status counts.

### Important Files
- `Workspace.cs`: Shared session state (loaded files, image lists, directory stats); exposed on MainForm as `mWorkspace` and read by every feature partial
- `XisfFile.cs`: Central model - all keyword access flows through here
- `KeywordList.cs`: Typed property accessors for common FITS keywords. Note: the `FocalRatio` setter self-derives FOCRATIO from the FOCALLEN/APTDIA keywords (ignoring its assigned value), so those must be written first.
- `CameraConfiguration.cs`: Base class for camera configs with temperature handling
- `CameraService.cs`: Camera detection, property analysis, and UI color helpers
- `TelescopeConfiguration.cs`: Base class for telescope configs with reducer support; `ApplyKeywords` emits TELESCOP/FOCALLEN/APTDIA/APTAREA and triggers FOCRATIO for all scopes
- `TelescopeService.cs`: Telescope detection, analysis, and UI color helpers
- `CaptureSoftwareConfiguration.cs`: Base class for capture software configs
- `CaptureSoftwareService.cs`: Software detection and analysis
- `Files/Compression/XisfBlockCompression.cs` + `BlockCompressionInfo.cs`: pure, UI-free `zlib+sh` + SHA-1 image-block codec and its attribute parser/formatter (see Compression above)
- `UIHelpers.cs`: Common UI control manipulation (ClearComboBox, ResetRadioButton, etc.)
- `MainForm.Designer.cs`: Auto-generated UI - TabIndex values manually fixed for proper navigation
- `Globals.cs`: All enums and constants
- `Configuration/AppPaths.cs`: Machine-specific paths (E:\, F:\ drives)
- `Configuration/XisfConstants.cs`: XISF signature size, max buffer size, and compression/checksum codec names (`zlib+sh`, `sha-1`)
- `Configuration/DirectoryFilters.cs`: Exclude lists for directory filtering

## Common Tasks

### Adding a New Keyword
1. Add property to `KeywordList.cs` with getter/setter
2. Optionally expose through `XisfFile.cs` as a delegating property

### Adding UI Feature
1. Add controls in Visual Studio Designer (updates MainForm.Designer.cs)
2. Create new partial class file in MainForm/ for logic
3. Wire up events in MainForm.cs constructor

### Adding a New Feature Area
Follow the **Telescope** feature as the template (`Services/TelescopeService.cs` + `Models/TelescopeConfiguration.cs` + `MainForm/Telescope.cs`). Four parts, four places:
1. **Logic** → `Services/<Feature>Service.cs`: pure and UI-free; takes domain data (e.g. `IEnumerable<XisfFile>`) and returns a `<Feature>Analysis` result type defined in `Models/`.
2. **UI binding** → a thin `MainForm/<Feature>.cs` partial: control↔model mappings, `Find<Feature>()` / `Clear<Feature>Group()`, and the button handlers. Call the service for every decision; use `Helpers/UIHelpers.cs` for control resets.
3. **Shared state** → read from `mWorkspace` (`Models/Workspace.cs`), never new MainForm fields. If the feature needs new session data, add a member to `Workspace`.
4. **Construction & wiring** → the `MainForm` constructor only (the composition root). If the feature reacts to a file load, add `Clear<Feature>Group()` to `ResetSession()` and `Find<Feature>()` to `RefreshFeatureDetection()` (both in `MainForm.cs`).

### Database Schema Changes
Table models are in `TargetScheduler/Tables/` - each maps to N.I.N.A. Target Scheduler tables.
