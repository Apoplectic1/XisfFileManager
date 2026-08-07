# RELEASING.md — publishing XFM to GitHub

> **Charter:** the rules for pushes to the public GitHub mirror. **The local repo is ground
> truth; GitHub is the public face** — a distribution channel, never the canonical location.
> Nothing here changes how development works; it only governs what the public sees and when.

## The mirror

`origin` = https://github.com/Apoplectic1/XisfFileManager (public; renamed from `XisfManager`
2026-08-02). No other remotes. `main` is the only branch on origin (default branch; two stale
pre-policy branches pruned 2026-08-02 — their history remains on local branches).

## Branch policy

- **`dev` = working branch.** All work lands here. **`dev` never pushes.**
- **`main` = distribution-ready ref, and every push of `main` carries a tag** — `vX.Y.Z`
  (semver, `v`-prefixed). Publish = fast-forward `main` to the chosen `dev` commit, tag it,
  push both:
  ```bash
  git checkout main && git merge --ff-only dev
  git tag -a vX.Y.Z -m "one-line release summary"
  git push origin main vX.Y.Z
  git checkout dev
  ```
  Tags are **annotated** (`-a -m`, adopted 2026-08-06 with v2.2.1) so the summary shows in
  `git tag -n` and GUI clients; v2.0.0–v2.2.0 are lightweight (bare pointers) — cosmetic only,
  MinVer and the release script treat both alike.
- Publish at natural completion points (a shipped unit of work, docs riding the same commit) —
  not on a schedule, and never mid-change. The working tree must be clean and the build
  warning-free at the published commit (see `VERIFICATION.md`). No tag → no push: the tag is
  what makes a `main` state a published state.
- **AL coordination (pre-flight; staged 2026-08-02, ARMED 2026-08-06):** XFM consumes the
  sibling `..\Library` via `ProjectReference` (`Astronomy.XISF`, the shared block codec — first
  AL dependency, `adopt-al-xisf-compression`), so the installer embeds the Library **working
  tree** at pack time, unpinned. If AL is dirty or has moved past its last published tag,
  **publish AL first** (see Library `RELEASING.md`) so the payload's `Astronomy.*` DLLs stamp a
  clean `X.Y.Z` that exists on AL's public mirror. `release.ps1` checks every `Astronomy.*.dll`
  in the publish output (not just `Astronomy.Core.dll` — XFM ships `Astronomy.XISF.dll` and no
  Core).
- **Docs-only exception (2026-08-02):** a `main` push may omit the tag when the delta contains
  only documentation/images — nothing that changes the built app — so the GitHub storefront
  (README, screenshots) can update without minting a release. Any change to code or build
  inputs keeps the full no-tag-no-push rule.

## Distribution: Velopack installers, built locally (adopted 2026-08-02)

Installers ship as GitHub Releases **packed and uploaded from this machine** via
`scripts\release.ps1` — the portfolio's one release mechanism (TSM/TP model). This replaced
the tag-triggered GitHub Actions workflow (`release.yml`, removed 2026-08-02).

One-time setup: `dotnet tool install -g vpk`, and `$env:GITHUB_TOKEN` = a PAT with
`public_repo` scope (only needed for upload; `-NoUpload` dry-runs without it).

Per-release flow:
```powershell
# on main, at the published commit (see Branch policy)
git tag -a vX.Y.Z -m "one-line release summary"
git push origin main vX.Y.Z
.\scripts\release.ps1          # publish → vpk pack → upload to GitHub Releases
```
- **Versions come from the tag** via MinVer (`<MinVerTagPrefix>v</MinVerTagPrefix>`, same as
  TSM) — the same tag gates the `main` push, names the GitHub Release, stamps the assembly,
  and shows in the window title (`XISF File Manager X.Y.Z`). No version files; untagged
  commits shape as `-alpha` prereleases.
- **MinVer stamp gate (2026-08-06):** MinVer 7's build cache keys on the option list only, not
  the repo — with the AL `ProjectReference`s in the graph, identical options let the Library's
  version leak onto `XisfFileManager.exe` (v2.1.0–v2.2.0 shipped stamping AL's 1.5.x; the
  title bar read it, while Velopack's package version — from `vpk -v` — stayed correct, so
  updates applied but the title never moved). Fix: the csproj sets an explicit
  `<MinVerVerbosity>` to give this project a unique cache key; `release.ps1` aborts if the
  published exe's `ProductVersion` ≠ the tag version, so a stamp regression can't ship.
- **The installed app self-updates**: startup check of this repo's Releases via Velopack
  (`MainForm.CheckForUpdatesAsync`); a published release propagates to installed copies on
  their next launch.
- **Dry-run:** `.\scripts\release.ps1 -NoUpload` → artifacts in `Releases\` (gitignored, as is
  the `publish\` staging dir); run the Setup.exe there to test an install locally. vpk refuses
  to re-pack a version already present in `Releases\` — delete that folder before repeating a
  dry-run at the same tag.
- The app's `Velopack` NuGet package and the `vpk` CLI should stay on matching versions
  (both 1.2.0 as of 2026-08-02) — `vpk pack` warns on skew.

Latest released tag: **`v2.2.1`** (MinVer cross-repo cache fix — exe stamps XFM's version again;
first annotated tag; adds the release-script stamp gate; app unchanged otherwise, carries AL
`1.5.1`). Prior: `v2.2.0` (checked browse skips pre-solved lights — full 11-keyword WCS
set present ⇒ no solver run, re-browse is idempotent; solver checkbox now default-on; carries AL
`1.5.1` unchanged); `v2.1.1` (solver diagnosability: `astap_cli -log` + failure-tail capture
in the `SOLVER` diag channel; AL `1.5.1` — legacy `sha1` checksum tokens accepted on read,
unblocking solves on 2019-era SGP files); `v2.1.0` (ASTAP plate solving in the read pass +
shared diagnostics adoption; first release carrying AL DLLs — `Astronomy.XISF`/`Core`/`Diagnostics`(+`.WinForms`)
stamped `1.5.0`, the armed AL gate's first live pass); `v2.0.1` (`v2.0.0` was the first
script-built release; `v1.9.0` and earlier were CI-built).

## Content rules (what is deliberately public)

- **`README.md` is the storefront** — user-facing description only (what XFM does, install,
  usage caveats, license). Development/testing minutiae stay out.
- **MIT-licensed** (`LICENSE`, © 2024–2026 Dan Stark; adopted 2026-08-02 with the rest of the
  portfolio).
- **`TestData/` sample images are deliberately committed** and therefore public.
- **Never in the repo, so never published:** tokens/credentials (none exist).
- History publishes whole. Anything that must not be public must never be committed — there
  is no post-hoc scrub step.
