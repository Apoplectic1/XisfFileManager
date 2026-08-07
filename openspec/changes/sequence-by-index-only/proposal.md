# sequence-by-index-only

## Why

Of the four sequence-numbering modes, only Index-Only is ever used — and the three weight modes
are vestigially **broken**, not merely idle: they key off `XisfFile.SSWeight`, which nothing in
the live codebase assigns since the SubframeSelector CSV import was retired to `archive/`
(Weight-Only would name every light `0000`, colliding straight into the duplicates mover). The
ByFilter/ByTime index choice is likewise ByFilter-only in practice. The adjacent Weights-removal
groupbox (strips legacy `SSWEIGHT` keywords manually) becomes redundant once `SSWEIGHT` joins
the automatic unwanted-keyword purge — it even contains a Designer-only "Calibration" radio
with no code behind it. Explored 2026-08-07 (in-conversation; scope fully mapped).

## What Changes

- **Sequence Numbering groupbox removed entirely** (`GroupBox_FileSelection_SequenceNumbering`,
  its 4 mode radios, the nested "Index" groupbox and its ByFilter/ByTime radios + tooltips).
  Renaming always sequences **by index, numbered per filter within each directory group**.
- Dead code deleted along the chain: the 4 `CheckedChanged` handlers, `eOrder` enum,
  `XisfFileRename.RenameOrder`, `FormatFileIndex` collapses to `{index:D3}`,
  `SetFileIndex(bTime)` loses its parameter (by-time branch dies with its radio),
  `XisfFile.SSWeight` (never assigned), 3 `UpdateUI` references, 2 tooltips.
- **All seven legacy weight keywords join `RemoveUnwantedKeywords`** (`SSWEIGHT`, `NWEIGHT`,
  `W_SNR`, `W_FWHM`, `W_ECC`, `W_PSFSNR`, `W_PSFS` — the manual tool's entire detection
  vocabulary; decided at apply, 2026-08-07) — purged automatically as files pass through
  keyword saves (passive convergence; no migration pass).
- **Weights groupbox removed entirely** (`GroupBox_KeywordUpdateTab_SubFrameKeywords_Weights`:
  combobox, label, All/Selected radios, Remove button, orphaned Calibration radio), plus
  `Button_KeywordSubFrameWeight_Remove_Click` and the now-consumerless `WeightKeyword` getters
  on `KeywordList` and `XisfFile`.
- **BREAKING (behavioral, accepted):** the manual SSWEIGHT-removal tool disappears; purging is
  passive-only, so a file never saved again keeps its inert `SSWEIGHT` keyword.

## Capabilities

### New Capabilities

*None.*

### Modified Capabilities

*None — rename sequencing and keyword normalization have no capability specs; this change
declares `skip_specs: true` (pure dead-code/UI removal plus one normalization-list entry).*

## Impact

- **XFM only**: `MainForm.Designer.cs` (two groupboxes + 13 controls), `FileSelection.cs`
  (handlers, rename click, `SetFileIndex`), `MainForm.cs` (init, `UpdateUI`, tooltips),
  `Globals.cs` (`eOrder`), `XisfFileRename.cs`, `XisfFile.cs`, `KeywordList.cs`
  (`WeightKeyword` out, `RemoveKeyword("SSWEIGHT")` in — case-sensitive list, uppercase matches
  what SubframeSelector wrote; the KeywordList.cs:821 solved-keywords guard is unaffected).
- **Docs**: `ARCHITECTURE.md` enum list drops `eOrder`; ROADMAP shipped entry.
- **Not affected**: AL, solver, hygiene, save pipeline semantics (UPDATE_NEW dirty-detection
  naturally treats an SSWEIGHT purge as a keyword change, which is what writes it out).
