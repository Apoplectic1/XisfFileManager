# 2026-08-07 — XISF block-codec benchmark → zstd-19 write decision

**Question:** is `zlib+sh(SmallestSize)` — XFM's write codec — actually the smallest practical
choice for this library, and what would zstd/lz4hc buy? Asked as the compression follow-on to the
tabled ROADMAP #8 session (where compression was ruled unconditional: an uncompressed block always
writes, under every mode).

**Method:** `tools/CompressionBench` (kept, rerunnable — not throwaway). Stratified deterministic
sample of the real image library (`E:\Photography\Astro Photography\Processing`, 19,896 XISF /
272 GB, all blocks compressed): 12 files per stratum, seed 42. Each file's block is decoded to raw
via AL's verified reader, then recompressed with every candidate; size and single-threaded encode/
decode wall time recorded. Candidates: AL `Compress` for `zlib+sh(max)` (current write), `lz4hc+sh`,
`zstd+sh(1)` (AL's pinned level), and direct ZstdSharp 0.8.8 (AL's own package) over AL's `Shuffle`
for levels 3/9/15/19/22. Raw rows: [`2026-08-07-compression-bench-results.csv`](2026-08-07-compression-bench-results.csv).

Library composition (scan): **light-sub 18,934 (95%)** · master-flat 675 · master-dark 281 ·
other-sub 6.

## Results (actual sizes, sampled)

| stratum | files | raw | zlib+sh (current) | zstd-1 | zstd-19 | zstd-22 | lz4hc+sh |
|---|---|---|---|---|---|---|---|
| light-sub | 12 | 401.5 MB | 145.4 MB | 140.1 (−3.6%) | **129.1 (−11.2%)** | 129.0 | 208.0 (+43%) |
| master-dark | 12 | 839.3 MB | 324.0 MB | 316.2 (−2.4%) | 302.1 (−6.8%) | 302.0 | 413.2 (+28%) |
| master-flat | 12 | 908.1 MB | 522.0 MB | 529.0 (+1.3%) | 513.4 (−1.6%) | 513.2 (−1.7%) | 553.4 (+6%) |
| other-sub | 6 | 381.7 MB | 277.2 MB | 278.2 (+0.4%) | 274.7 (−0.9%) | 274.6 | 296.3 (+7%) |

Speeds (single-threaded, unshuffle included in decode): zlib enc 3–19 MB/s, dec 0.6–1.3 GB/s ·
zstd-1 enc ~400–560 MB/s · zstd-19 enc 2–6 MB/s, dec 1.0–1.7 GB/s (fastest zstd decode — level
only raises encoder effort; smaller stream walks quicker) · lz4hc dec 1.3–2.0 GB/s.

## Findings

- **Low-level zstd is a wash on size** (±1–4%) — after byte-shuffle, DEFLATE already extracts
  nearly all the structure; low bytes are incompressible noise for every codec. The folklore
  "zstd beats zlib 3–10%" did not hold at levels 1–9 on this data.
- **The win lives at level ≥ 15** (big-window search strategies): **−11.2% on light subs** — the
  stratum that is 95% of the library — −6.8% darks, −1.6% flats. zstd-22 adds nothing over 19
  (−0.02%) at worse encode speed: dead on arrival.
- **lz4hc is out** on size everywhere (+6…+43%).
- **Decode speed is a non-factor**: all candidates 0.6–2.0 GB/s; a 40 MB frame decodes in
  30–60 ms under any of them, invisible beneath solver/stacking costs.
- **Library-weighted extrapolation** (per-stratum file averages × populations reproduce the
  on-disk total to ~3%: est. 264 GB vs 272 actual): recompressing everything at zstd-19 would
  reclaim **~26 GB (≈10%)**, almost all of it from lights.
- Encode-cost reality: zstd-19 is ~1.5–2× *slower* to encode than zlib-9 (a ~4 GB fresh night:
  ~30–35 min vs ~15–20, paid once per file at first save). zstd-1 (~500 MB/s) remains the speed
  play if that ever matters more than size.

## Decisions (user, 2026-08-07)

1. **XFM writes `zstd+sh` level 19 for new saves** (option 1). Needs a small AL change first —
   `XisfBlockCompression.Compress` pins zstd at level 1 (NINA/PI producer parity), so it grows an
   optional level parameter; XFM passes 19.
2. **Library recompress is a follow-on, triggered by FORCE only** — the one-time ~26 GB reclaim
   pass runs as a deliberate FORCE action, planned when picked up. This resurrects ROADMAP #8's
   "rewrite unchanged files" verb with a genuine payload and answers its Q2: FORCE's standing
   meaning becomes *the recompress trigger*, not a routine radio state.

**Compatibility:** zstd level never affects readability (one stream format; level = encoder
effort). Readers verified: NINA 3.3 clone parses `zstd`/`zstd+sh` via ZstdSharp
(`NINA.Image/FileFormat/XISF/XISF.cs:334-339,444`); PixInsight reads zstd since 1.8.9-2 (2022);
AL reads all spec codecs (Tier 3). Pre-zstd vintages (NINA 2.x, PI pre-2022) would refuse the
blocks. ASTAP is unaffected (solver hands it a decompressed temp FITS). Existing zlib files stay
readable everywhere, unchanged until the FORCE recompress.

**Side effect:** the benchmark's AL-reader path checksum-verified all 48 sampled files (42 full
run + 6 smoke) against their declared SHA-1s — first field exercise of AL's zstd/lz4hc codecs and
weak-but-real evidence the library is intact. (The Verify SHA browse feature — ROADMAP #6 —
generalizes this.)
