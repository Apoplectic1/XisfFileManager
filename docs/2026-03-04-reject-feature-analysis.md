# REJECT Feature Analysis

## Overview

The "REJECT" prefix in `XisfFileRename.cs` is a visual file-sorting marker for subframes that failed quality checks during image stacking.

In astrophotography workflows (particularly with PixInsight's WBPP -- Weighted Batch Pre-Processing), when you integrate/stack subframes, some individual exposures get **rejected** due to poor quality (bad seeing, clouds, tracking errors, etc.). These rejected frames typically end up in a subdirectory named something like `rejected/` or `reject/`.

## Filename Prefix (`XisfFileRename.cs:31-33`)

The rename logic checks whether the file's parent directory path contains the word "reject". If it does, the renamed file gets a `"REJECT  "` prefix prepended to its name. This serves as a **visual flag** so that when you sort or browse files alphabetically, rejected frames are immediately obvious -- they all sort together at "R" and the prefix jumps out visually.

```csharp
if (sourceFileDirectory.Contains("reject", StringComparison.OrdinalIgnoreCase))
{
    newFileName = "REJECT  " + newFileName;
}
```

## Related Reject Concepts in the Codebase

### Rejection Map Image Blocks (`XisfFileUpdate.cs:155-159`, `XisfXmlReader.cs:74-78`)

PixInsight embeds `rejection_high` and `rejection_low` image blocks in integrated master files. The code strips these out during read and update to reduce file size since they're not needed downstream.

### Rejection Algorithm Tracking (`KeywordList.cs:748-763`, `Xml.cs:496`)

Tracks which pixel rejection algorithm was used during integration. Stored in the `MSTRALG` keyword and displayed in the Masters UI group. Known algorithms:

| Abbreviation | Algorithm |
|---|---|
| WSC | Winsorized Sigma Clipping |
| LFC | Linear Fit Clipping |
| ESD | Generalized Extreme Studentized Deviate Clipping |

### CREJECT Keyword (`KeywordList.cs:312-315`)

A WBPP post-processing group keyword (`CREJECT`) for calibration reject tracking, part of the calibration file keyword set (alongside `CDARK`, `CFLAT`, `CBIAS`, `CPANEL`, `CSTARS`).

### Target Scheduler Reject Reason (`AcquiredImage.cs:25`)

N.I.N.A. Target Scheduler database field (`rejectreason`) tracking why an acquired image was rejected by the scheduler.
