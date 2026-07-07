# Domain

> **Charter:** The astronomy/workflow context XFM's code assumes — image formats, FITS keyword
> semantics, the user's hardware inventory, and where XFM sits in the imaging pipeline. Read when a
> change touches meaning (what a keyword or frame type *is*) rather than mechanics (`ARCHITECTURE.md`).

## What XFM is for

XFM manages libraries of astrophotography subframes: after an imaging night, it reads XISF file
metadata in bulk, normalizes/repairs FITS keywords (camera, telescope, capture software, exposure),
renames files to a canonical scheme, displays plan/acquired data from the N.I.N.A. Target Scheduler
database (read-only today; graded-count write-back is a ROADMAP item), and maintains a
calibration-frame library.

## XISF format

XISF (Extensible Image Serialization Format) is an astronomy image format from PixInsight: an XML
metadata header carrying FITS-compatible keywords plus binary image-data attachments. The on-disk
compressed form used by PixInsight/NINA is `zlib+sh` (byte-shuffle + zlib) with a SHA-1 block
checksum — the format XFM targets on save (mechanics and the plain-`zlib` fallback: `ARCHITECTURE.md`).

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

XFM is one app in the Astronomy portfolio (see the parent `../CLAUDE.md` map). It reads the Target
Scheduler plugin's database directly (`schedulerdb.sqlite` on BIRDWATCHER; accepted counts —
write-back not implemented, ROADMAP follow-up #9). Long-term, the Target Scheduler tab migrates to
the TSM app and XFM consumes `Catalog.db` read-only via `Astronomy.Catalog` (ROADMAP follow-up #7).
