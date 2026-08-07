## 1. Sequence Numbering removal (rename always index-per-filter)

- [x] 1.1 `FileSelection.cs`: delete the 4 `CheckedChanged` handlers
      (`RadioButton_WeightIndex/Index/Weight/IndexWeight_CheckedChanged`); in the rename click
      handler drop the `bFilter`/`bTime` reads and call `SetFileIndex()`
- [x] 1.2 `SetFileIndex`: remove the `bTime` parameter and the by-time branch — always number
      per filter within each directory group
- [x] 1.3 `XisfFileRename.cs`: delete `RenameOrder`; `FormatFileIndex` returns `$"{index:D3} "`
      unconditionally (verify trailing-space parity with the old INDEX arm)
- [x] 1.4 `Globals.cs`: delete `eOrder`; `MainForm.cs`: drop the `RenameOrder = eOrder.INDEX`
      init (MainForm.cs:68) and the two ByFilter/ByTime tooltips (MainForm.cs:80-81)
- [x] 1.5 `MainForm.Designer.cs`: remove `GroupBox_FileSelection_SequenceNumbering`, its 4
      radios, nested `GroupBox_FileSelection_Count` + ByFilter/ByTime radios (declarations,
      layout, event wiring, field declarations); retarget/remove the 3 `UpdateUI` references
      to the groupbox
- [x] 1.6 `XisfFile.cs`: delete `SSWeight` (never assigned)

## 2. SSWEIGHT auto-purge + Weights groupbox removal

- [x] 2.1 `KeywordList.RemoveUnwantedKeywords`: add all seven legacy weight keywords in the
      alphabetical list — `SSWEIGHT`, `NWEIGHT`, `W_SNR`, `W_FWHM`, `W_ECC`, `W_PSFSNR`,
      `W_PSFS` (case-sensitive matches, the spellings the graders wrote; scope decided at
      apply 2026-08-07 — purging only SSWEIGHT would orphan the other six once the tool goes)
- [x] 2.2 `FileSelection.cs`: delete `Button_KeywordSubFrameWeight_Remove_Click`
- [x] 2.3 `MainForm.Designer.cs`: remove `GroupBox_KeywordUpdateTab_SubFrameKeywords_Weights`
      and all 6 child controls (combobox, label, All/Selected radios, Remove button, the
      code-orphaned Calibration radio) + any `UpdateUI`/enable references
- [x] 2.4 `KeywordList.cs` + `XisfFile.cs`: delete the `WeightKeyword` getters (consumerless
      after 2.2)

## 3. Docs (same commit)

- [x] 3.1 `ARCHITECTURE.md`: drop `eOrder` from the enums list; adjust any rename-pipeline
      wording that mentions weight sequencing
- [x] 3.2 ROADMAP: Recently shipped entry (sequence-by-index-only; SSWEIGHT joins the passive
      purge; Weights tool removed)

## 4. Verification

- [x] 4.1 Warning-free `dotnet build XisfFileManager.sln -c Release`
- [ ] 4.2 Manual pass (user): rename a browsed folder → files numbered `NNN` per filter as
      before; Keyword Update tab shows no Weights groupbox; File Selection shows no Sequence
      Numbering groupbox; saving a file that carried `SSWEIGHT` writes it back without the
      keyword
