# Domain

> **Charter:** The astronomy/workflow context XFM's code assumes — image formats, FITS keyword
> semantics, the user's hardware inventory, and where XFM sits in the imaging pipeline. Read when a
> change touches meaning (what a keyword or frame type *is*) rather than mechanics (`ARCHITECTURE.md`).

## What XFM is for

XFM manages libraries of astrophotography subframes: after an imaging night, it reads XISF file
metadata in bulk, normalizes/repairs FITS keywords (camera, telescope, capture software, exposure),
renames files to a canonical scheme, and maintains a calibration-frame library.

## XISF format

XISF (Extensible Image Serialization Format) is an astronomy image format from PixInsight: an XML
metadata header carrying FITS-compatible keywords plus binary image-data attachments. XFM writes
`zstd+sh` level 19 with a SHA-1 block checksum (since 2026-08-07; readers need zstd support —
NINA 3.x, PixInsight ≥ 1.8.9-2); the legacy PixInsight/NINA-era `zlib+sh` library **converges to
zstd incrementally** — hygiene recompresses each directory's non-zstd files on its first browse
(mechanics: `ARCHITECTURE.md`).

Every Browse **repairs unhygienic files in place** (uncompressed, checksum-less, or
legacy-codec blocks → `zstd+sh` + checksum; automatic, no opt-out; Browse is not a read-only
operation — expect the *first* browse of a legacy target to spend minutes recompressing). **Gotcha —
a fresh checksum is not provenance:** compressing a file that never had a checksum *certifies the
bytes as they are now*. If silent rot had already occurred, the new SHA-1 faithfully certifies
the rotten bytes — unavoidable, since an unverifiable block is unverifiable. A block checksum
always means "intact since this checksum was written", never "intact since capture". Side
payload of the first pass: Verify-SHA coverage reaches 100% of the library.

## Plate solving: measured vs planned (2026-08-06)

A light frame's header carries two kinds of pointing/rotation truth. NINA stamps what it was *told*:
`RA`/`DEC` from the mount and `OBJCTROT` from the target's **planned** position angle (only when a
rotation was set — many frames have none, which is what TSM shows as mechanical-fallback `°(M)`).
XFM's Solver checkbox replaces plan with **measurement**: ASTAP solves the actual star field and XFM
stamps the measured centre coordinates, measured position angle (`OBJCTROT`), and the full standard
WCS solution. Measured always wins — disk is truth. The solved WCS also makes the file
self-describing for any WCS-aware software (PixInsight, astrometry tools).

## Frame types & filters

- **Light** — exposure of the target; **Dark** — thermal signal at matching exposure/temp;
  **Flat** — optical-train illumination correction; **Bias** — read-out floor.
- Filters: L, R, G, B (broadband); Ha, OIII, SII (narrowband). `SHUTTER` marks shutter-closed frames.

## Reject workflow

During stacking (PixInsight WBPP — Weighted Batch Pre-Processing), poor subframes (clouds, tracking
errors, bad seeing) are rejected into a `reject*/` subdirectory. XFM's rename prefixes such files
with `"REJECT  "` so they sort together and stand out visually. Full analysis:
`docs/2026-03-04-reject-feature-analysis.md`.

## Hardware inventory (all active)

- **Cameras:** Z533, Z183, Q178, A144. Multi-camera targets are real (e.g. one target shot with
  Z533 broadband + Z183 narrowband, each with its own gain/offset/temp).
- **Telescopes:** APM107, EvoStar150, Newtonian254 — each optionally with the Riccardi ~0.75x
  reducer (focal length and ratio are reducer-aware). Reduced focal lengths are hardcoded per scope:
  EvoStar 1000→750 and Newtonian 1100→825 are exactly 0.75x; APM107 is 700→531 (~0.759x).
- **Capture software:** NINA, TheSkyX, SGPro, Voyager, SharpCap (`SWCREATE` = NINA, TSX, SGP, VOY, SCP).

## Ecosystem position

XFM is one app in the Astronomy portfolio (see the parent `../CLAUDE.md` map). Target Scheduler
viewing/editing is owned entirely by the TSM app: XFM has **no** TS integration and never
consumes `scheduler.db` or `Catalog.db` (decided 2026-07-07; the former TS tab was removed then).
XFM's scope is the image library itself — files on disk and their keywords.
