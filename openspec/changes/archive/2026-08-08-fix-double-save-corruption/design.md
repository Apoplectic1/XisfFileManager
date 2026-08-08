# Design — fix-double-save-corruption

## Context

Motivation and incident history: see `proposal.md` — Why, and the 2026-08-08 diagnosis session
(memory: `xfm-double-save-corruption-bug`). Current mechanics:

- `UpdateFileAsync` reads the whole source file fresh (`binaryFileData`), rebuilds the XML from the
  in-memory `KeywordList`, computes the *output* attachment location via
  `SetImageAttachmentLocation`, then copies the block from
  `binaryFileData[xFile.TargetAttachmentStart .. +TargetAttachmentLength]` (`XisfFileUpdate.cs:225`).
- `TargetAttachmentStart/Length` are written only at browse (`XisfXmlReader.cs:94`, from the
  declared `location`) and after hygiene (`MainForm.cs:351`, from the AL rewrite result). A save
  changes the header length but leaves the cache pointing at the pre-save layout.
- `KeywordsMatchXml` compares `KeywordList` to `xFile.XmlString` — also browse-time state — so an
  unchanged second save is not skipped; it proceeds and corrupts.

## Goals / Non-Goals

**Goals**
- The three requirements in `specs/save-write-integrity/spec.md`, entirely inside XFM's save path.
- Make the failure mode *unreachable*, and independently make it *unwritable* (gate) if any future
  path reintroduces stale geometry.

**Non-Goals**
- No repair/migration of existing corrupt files (recovery already done out-of-band; rule 15).
- No AL/Library changes — the AL rewriter is correct; this is XFM-side cache discipline.
- No redesign of the save pipeline (buffer list, temp-file + atomic move stay as-is).

## Decisions

1. **Gate reads the source's own declared location, not just cached values.**
   After reading `binaryFileData`, parse the *on-disk* header's `location` attribute and compare
   with `xFile.TargetAttachmentStart/Length`. Then check codec magic at that offset
   (zlib `78 9c/da/01/5e`, zstd `28 B5 2F FD`; lz4 has no magic — for lz4/uncompressed, require
   offset ≥ end of the on-disk XML). Mismatch of either check → abort this file's save
   (log + console error naming file, expected, found; `LastUpdateOutcome = Failed`).
   *Alternative considered*: trust the on-disk declared location and drop the cache entirely.
   Rejected for this change — the cache also feeds other consumers and the read is where the
   contract lives; syncing + validating is the smaller, stricter change. (Follow-up candidate:
   collapse the cache once no other reader needs it.)

2. **Refresh inside `UpdateFileAsync`, immediately after the successful atomic move.**
   `TargetAttachmentStart = 16 + newXmlLength + padding` (the value `SetImageAttachmentLocation`
   already computed), `TargetAttachmentLength` unchanged (verbatim copy), and `XmlString` set to
   the just-written XML text. Owning the refresh in the writer (not call sites) mirrors how the
   hygiene path's contract is documented on `XisfBlockRewriteResult` and fixes all three callers
   (MainForm, Calibration, FluxDensity) at once.
   *Alternative*: refresh at call sites like MainForm does for hygiene. Rejected: three call sites,
   one already-missed pattern — that's how this bug happened.

3. **`KeywordsMatchXml` compares against the freshly read on-disk XML** (the `xmlString` the save
   just extracted from `binaryFileData`), passed as a parameter instead of `xFile.XmlString`.
   With decision 2 keeping `XmlString` current this is belt-and-braces, but it makes the skip
   decision correct even if the file changed outside the session (hygiene, external tools).

## Risks / Trade-offs

- [Gate false-positives on exotic-but-valid files (e.g. lz4 legacy where only a weak structural
  check is possible)] → the weak check (offset past on-disk XML end, block spans to declared end)
  still catches the actual failure signature (offset inside XML). Legacy codecs are being
  converged to zstd by hygiene anyway, where the magic check is strong.
- [Aborting a save mid-batch leaves some files updated, some not] → per-file abort with
  continue-to-next matches existing per-file failure handling (locked files); summary already
  reports failed counts.
- [Refresh math drifts from what was actually written] → derive the refreshed values from the
  exact buffers written (signature length field + padding buffer), not recomputed.

## Migration Plan

None — ship it (rule 15). Manual verification per `VERIFICATION.md`: browse a scratch copy,
save twice with a keyword change between; second save must produce a verified-clean file
(`CheckBox_VerifySha` browse pass), and a deliberately stale-offset test (hex-edited copy) must
abort with the contract error.

## Open Questions

None.
