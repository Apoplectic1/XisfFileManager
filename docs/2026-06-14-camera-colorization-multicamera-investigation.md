# Camera Colorization & Multi-Camera Set All — Investigation

## Status

**Investigation only — implementation deferred (2026-06-14).** No code was changed. This
records what was found, the model we agreed on, the concrete bugs in the current code, the
Astronomy.Library (AL) assessment, and the open decisions, so the work can be picked up later.

## What triggered this

`Button_KeywordCamera_SetAll_Click` bails when more than one camera is checked
(`GetCheckedCameraCount() != 1`, `MainForm/Camera.cs:212-214`). Real library targets are
heterogeneous — e.g. `E:\Photography\Astro Photography\Processing\IC405 - Flaming Star`
mixes **Z533** (L filter, `Z533G100O50@-10.0C`) and **Z183** (Ha, `Z183G111O10@-20.0C`),
each with its own gain/offset/temp. The Camera tab already renders a per-camera matrix (one
row per camera × columns Seconds/Gain/Offset/SensorTemp/Binning), so the *data* model
anticipates multi-camera; only the Set All **write** and parts of the **color feedback** lag.

The question broadened to: how should XFM handle multi-camera / -telescope / -capture-software
targets, and how should the Camera group-box colorization actually work.

## XFM's role — the normalization model

XFM reads `.xisf` files produced by several capture programs (NINA, TheSkyX, SGP, Voyager,
SharpCap), whose keyword populations are disparate and may be missing or wrong. Its job is to
turn them into **one normalized, consistent keyword + filename population** for downstream
AL-based consumers.

The Set buttons implement a **many → one → many** flow:

- **many → one** — Browse reads every file's keywords and aggregates them into the UI state
  (`FindCamera` populates each camera row's comboboxes from that camera's files).
- user edits the UI (the common, per-equipment characteristics).
- **one → many** — a Set button writes the UI state back to the matching files.

A side constraint: **minimize unneeded disk writes.** Targets hold many large files; rewriting
them blows up backup storage. XFM alphabetizes keywords and writes a file only if a keyword
actually changed vs its on-disk state (same idea drives rename). See `XisfFileUpdate.UpdateFileAsync`
("save-if-needed").

## Source-of-truth principle (decided)

**Keywords are authoritative, not folders.** The library disk convention is
`target name / camera / filter / *.xisf` (mosaics add a panel level:
`Mosaic - target / camera / panel / filter / *.xisf`), but populating that tree is error-prone —
a file can be dropped in the wrong folder. Trusting the folder to decide a write could silently
rewrite a misfiled frame (a blue-filter file in a red dir becomes red — the blue frame is
destroyed, the red pool polluted). Relying on keywords avoids this: a misfiled file's
keyword-derived name looks out of place, but downstream keyword-based apps stay correct.

Consequence for "Set All": **"matched" means matched by keyword (`INSTRUME`), never by directory
location.** A file with no recognizable camera keyword matches no camera row and is left
untouched (it cannot be safely auto-assigned). The deliberate tool for keyword-less files is
**single-camera Set All** = "I assert every loaded file is this camera," which forces identity
onto all of them. Single-camera Set All is a true override; the safeguard against mistakes is the
**color feedback + user responsibility**, not code-level keyword protection.

## Set All / Set By File today, and the gap

- **Set All** forces the (single) checked camera's row + identity keywords onto **every** file.
  Per field: a displayed single value is forced; an empty cell is skipped (`TryParse` fails), so
  that field's per-file value is preserved. This per-field force-vs-preserve already works —
  the only missing piece for multi-camera is routing each file to its **matching checked camera**
  and removing the `> 1` bail.
- **Set By File** (`MainForm/Camera.cs:283-312`) already routes per file
  (`selectedCamera ?? AllCameras.FirstOrDefault(c => c.MatchesCamera(xFile.Camera))`), preserving
  each file's own values and filling gaps from the UI.
- **Set By File trap:** after Browse, `FindCamera` auto-checks *both* cameras, so
  `GetSelectedCamera()` returns the first checked (Z533) and Set By File force-applies Z533 to
  **all** files (including Z183) unless the user manually unchecks. (User noted Set By File was
  also broken by a prior change — treat separately.)

Telescope and Capture Software use **radio buttons** (single-select), so they can't express
multi-equipment the way the Camera checkbox matrix does; handling multi-telescope / -software
the same way would need a row-style rework.

## Colorization = a visual validity check (decided)

The group-box colors are a **validity check**, not value analysis (the many → one makes per-value
analysis in the UI impractical). The question each color answers: *are all my files' keywords for
this column present, non-sentinel, and consistent?*

| Color | Meaning |
|---|---|
| **Black** | all files present & non-sentinel for this keyword, **one** distinct value (valid + uniform) |
| **Green** | all files present & non-sentinel, **more than one** distinct value (valid, but multiple — e.g. multi-camera). Still a *pass* |
| **Red** | at least one file missing / sentinel / malformed (**invalid** → intervene). The only fail |

Green is the "slight expansion" over a plain pass/fail: it confirms validity *and* flags
legitimate multiplicity. Generalized rule (per the final pseudocode): **green = >1 distinct valid
value**; for the Camera column that is ">1 camera," for a property column it is ">1 value." Note
this is an **all-files** column-level signal — in a multi-camera load, property headers like Gain
go green naturally (gains differ across cameras). Per-camera *cell* granularity (Z533 gain black
vs Z183 gain green in the same load) is a separate concern — see Open decisions.

## Code findings (current bugs)

These are durable and worth fixing regardless of the larger refactor.

| # | Finding | Location |
|---|---|---|
| 1 | `bMissingCameras` is dead — `cameraNames = mFileList.Select(c => c.Camera)` makes `cameraNames.Count == mFileList.Count` always, so it's never true; the `DarkViolet`/partial branch in `GetCheckboxColor` is unreachable | `MainForm/Camera.cs:106,110`; `Services/CameraService.cs:184` |
| 2 | No "0 cameras recognized" warning — `bNoCameras` is true only for an *empty* file list, not for "files loaded but none carry a recognizable `INSTRUME`"; that case falls through to Black | `MainForm/Camera.cs:109` |
| 3 | Cell `NoValues` renders **Black** (fall-through), indistinguishable from a clean single value; combobox cells never return Red | `Services/CameraService.cs:165-174` |
| 4 | **MISSING masks DIFFERENT** — when some files lack a value *and* the present ones disagree, `differentValues` is forced false; the cell shows one value (index 0) as if legit, which Set All would then force-collapse | `Services/CameraService.cs:102-103,193-200` |
| 5 | Header goes Red while the offending camera's cell stays Black/Violet (cells never Red) — the red header doesn't point at the bad row | tables A+C below |
| 6 | The **Camera** column header (`Label_..._Camera`) is reset but **never recolored** — always Black; the green you see in the "Camera column" is the checkbox text, not the header | `MainForm/Camera.cs:77` (reset), absent in `FindCamera` |
| 7 | Numeric getters use `Convert.ToInt32/ToDouble` → **throw** on a present-but-non-numeric value (malformed), aborting analysis instead of reporting Red. Only `ExposureSeconds` uses `TryParse` and degrades safely | `Keyword/KeywordList.cs:505,548,234,668,450,589` (safe: `:336-348`) |

### Current color sources (for reference)

- Cell / combobox: `GetComboBoxColor` (`CameraService.cs:165-174`) — Black/Green/DarkViolet, never Red.
- Property column header: `GetLabelColor` via `UpdateLabelFromAnalyses` (`CameraService.cs:153-160`, `Camera.cs:180-185`), called with `(anyNoOrMissing, false, anyDifferent)` so any No/Missing → Red.
- Camera checkbox: `GetCheckboxColor` (`CameraService.cs:179-188`).
- State partition in `AnalyzeIntProperty` / `AnalyzeDoubleProperty` / `AnalyzeTemperature` — identical algorithm; only the validity predicate differs.

## Sentinel and malformed (decided)

**A sentinel value *is* "absent."** Getters return per-column sentinels when the keyword is
empty: `-1` (Seconds/Gain/Offset/Binning), `-273` (SensorTemp/FocuserTemp). So per-column
"validity" collapses to one rule — **`value == sentinel ⇒ Absent`** — and the three `Analyze*`
methods can merge into one generic `Analyze(selector, sentinel)` (temperature stops being a
special case: it's just `sentinel = -273`).

**Malformed must be folded in.** Today `Convert.To*` throws on garbage (finding #7); switching the
getters to `TryParse → sentinel` makes a malformed value degrade to the sentinel, so it lands in
the same "absent → red" bucket. No separate state or color is needed. Recommended: treat **only**
the sentinel as absent (so a genuinely out-of-range value surfaces loudly as a distinct value
rather than being silently swallowed).

## Astronomy.Library (AL) assessment

AL's `Astronomy.Catalog.Scan.ImageLibraryScanner.ScanUnitsAsync(targetDir)`
(`Library/Astronomy.Catalog/Scan/ImageLibraryScanner.cs:85`) reads exactly one library target and
returns `TargetReport`s whose `FilterAggregate`s carry `CamerasSeen` and modal `TypicalSettings`
(gain/offset/temp/binning). It does **not** simplify this work:

- **Read-only** — built on the immutable `XisfHeader`; AL has no `.xisf` writer. XFM's task is to
  *write/normalize* keywords.
- **Disk-tree-shaped** — it walks `Captures/<Camera>/<Filter>/`; XFM operates on the in-memory
  `mFileList` (an arbitrary Browse set with unsaved edits).
- **Filter-grouped** — it folds camera into per-filter aggregates; XFM needs per-camera routing.
- **No coupling today** — XFM has zero AL `ProjectReference`s, and the `Astronomy.XISF` codec
  share was reverted two commits before this investigation (`2cd23fc`).

**Decision: keep this in XFM.** But the convergence is real and worth doing later: AL's
`XisfHeader.OffsetNormalized` (`Astronomy.XISF/XisfHeader.cs:117-130`) already **duplicates** XFM's
per-camera offset divisors (÷5 / ÷40 / ÷18.33; `KeywordList.cs:553-575`), and `XisfHeaderReader`
duplicates XFM's header parse. The user confirmed AL methods *can* be added if needed — so a shared
`Astronomy.XISF` reader (and eventually a writer) is the eventual home, with the camera registry as
the single source of truth.

## Design directions explored (deferred)

- **Unify colorization on one state → color map.** Collapse the 4 analysis bools into a single
  `MatchState`; one `ColorFor(state)`; two methods — `ColumnHeaderColor(states)` (severity roll-up)
  and `RowColor(state)`; compute the Camera column's state from detection
  (`CameraColumnState`/`CameraRowState`) so its header/checkboxes are colored uniformly. This fixes
  findings 1, 2, 3, 5, 6. The final pseudocode reduced the header pass to one generic
  `SetCameraGroupBoxHeaderColumnLabelColor(files, columnName)` (validity filter → red on any
  invalid; else black if 1 distinct value, green if >1).
- **Reuse across columns.** The 5 value columns already share the same color functions; the only
  per-column variation is the validity predicate (the sentinel) and the value selector (SensorTemp
  is camera-dependent CCD-TEMP/FOCTEMP; Gain/Offset only for capable cameras). "Sentinel == absent"
  makes the analyzers fully generic.
- **Easy add/remove camera (separate, larger refactor).** Today adding a camera touches ~5 places:
  the concrete `CameraConfiguration`, `CameraService.AllCameras`, ~6 hand-keyed dictionaries in
  `InitializeCameraMappings`, the Designer controls, and the `KeywordList` per-camera switches. To
  make it a one-liner: (a) data-drive the grid (build rows from `AllCameras` into a
  `TableLayoutPanel`, deleting the Designer controls + mapping dictionaries), and (b) move the
  per-camera offset divisor + eGain onto `CameraConfiguration` (deleting the `KeywordList` switches
  and the AL duplication). The color refactor is orthogonal to this.

## Open decisions (for pickup)

1. **Multi-camera Set All write:** ≥2 checked → apply each checked camera's row to its keyword-
   matched files; files matching no checked camera → skip + report. Confirm the report mechanism.
2. **Cells vs header scope:** is colorization header-only (one label per column, all-files), or do
   the per-camera cells/dropdowns also get colored (per-camera detail)? Dropdown items, if colored,
   were agreed to inherit the cell color (option "c" — uniform, no owner-draw).
3. **Green semantics:** confirm green = ">1 distinct valid value" generically (Camera column = ">1
   camera").
4. **Whether to do the colorization fix + sentinel/`TryParse` fix now**, independent of the
   multi-camera Set All write and the larger add/remove-camera rework.
5. **Set By File:** fix the multi-checked trap (and the prior regression) in the same pass or
   separately.
