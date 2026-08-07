# 2026-08-07 — Browse-time compression hygiene (explore record)

Decisions from the `/opsx:explore` session that superseded the FORCE-gated recompress plan.
ROADMAP #10 carries the compressed form; this records the reasoning. Status: **explored and
decided, not yet proposed/implemented** — next step is an openspec change
(working name `browse-compression-hygiene`). Revisit *solve-before-compress* with fresh eyes
before proposing (user request).

## The reframe

Compression becomes **automatic library hygiene in the Browse read pass** — not a save-time side
effect, not a mode-triggered action. "Compression is its own independent animal": independent of
UPDATE_NEW/FORCE/PROTECT entirely. FORCE plays no role anywhere, which (with #8's Q2) leaves
FORCE with no remaining justification at all.

## Forks settled (user, 2026-08-07)

1. **Criterion = hygiene, not migration:** block uncompressed ∨ no checksum → rewrite as
   `zstd+sh(19)` + SHA-1. Compressed+checksummed files are never touched regardless of codec —
   **mixed zlib/zstd is the accepted end state**; the ~26 GB full-migration reclaim is
   consciously forgone (not large on a quarter-filled 4 TB drive).
2. **Write = surgical:** XML byte-preserved except `compression`/`checksum`/`location`
   attributes; block swapped; temp file + atomic move. No keyword normalization, no solver-stamp
   persistence (stamps ride the normal save — a fresh solved+hygiene-fixed file is written twice
   that day, deliberately).
3. **Trigger = always-on:** no checkbox, no opt-out. **Browse stops being read-only** — stated
   decision, not accident. Compression is **exempt from Protect**; Protect narrows to "no
   keyword writes" (feeds #8's design).

## Per-file pipeline order

```
read header → verify SHA → SOLVE → HYGIENE (compress)   — never compress before solve
```

Solve-before-compress is the *entire* solve/compress synergy: a fresh uncompressed light is
already ASTAP-readable in place, so solving first costs nothing and sharing buffers would buy
~50 ms on rare paths (ASTAP is an external process — it reads files, not memory). Combining the
features beyond ordering was considered and rejected (SoC).

## Solver temp-FITS dies (rides the surgical writer)

The surgical writer is a shared primitive `(source, target, codec)`:
hygiene = codec zstd-19, atomic-replace source; solver = codec **None**, temp target.
Compressed solve inputs become **surgical temp uncompressed XISF**, unifying ASTAP's input on
one format (its uncompressed-XISF reader is field-proven by the in-place path since v2.1.0) and
deleting `WriteMinimalFits` — the endian swap, BZERO=32768 trick, and silent UInt16-only limit
go with it. FITS leaves the pipeline as a format. (Original FITS choice was rational: predates
any reusable XISF-rewrite primitive.)

## Deliberately open

- **Parallelism:** a bounded worker pool would cut a fresh-night browse ~6× (zstd-19 ≈ 2–4 MB/s
  single-threaded ⇒ ~15 s per 40 MB frame), but hygiene should essentially never recur after the
  initial library pass, so optimizing it is suspect — leaning simple/sequential; revisit with
  fresh eyes alongside solve-before-compress. If parallel: pool must be **browse-scoped**
  (barrier before browse completes) or files mutate under later operations (Update save vs
  hygiene rewrite file-lock race).
- Cancel/progress UX for the first pass over a big unhygienic directory (~90 recompresses ≈
  20–25 min sequential).

## Gotcha to carry into user docs

Recompressing a no-checksum block **launders** it: if silent rot already occurred, the fresh
SHA-1 certifies corrupt bytes. Unavoidable (unverifiable is unverifiable) — the checksum means
"intact from now on," never provenance. Side payload: hygiene converts no-checksum files
(90 of 184 in the first Verify-SHA field run) into verifiable ones — Verify SHA coverage → 100%.

## Related same-day decision: °(M) demoted to a flag (TSM-side)

The user manually removed the °(M) backlog from the library (verify via TSM's existing
ambiguity report — no new tooling). TSM will carry a simple "rotation not solved" flag instead
of mechanical-angle machinery; remedy is always "run XFM → rescan." XFM's own mechanical `M`
fallback (`RotatorPosition` in `RotationAngle`) becomes retirable after verification — open
sub-question: what the filename does for an unsolved light once the fallback dies (no S token
vs explicit marker; rule-16 flavored). `FramingAngleDegrees` (AL) is unaffected — framings
consume solved angles only.
