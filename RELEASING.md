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
  git tag vX.Y.Z
  git push origin main vX.Y.Z
  git checkout dev
  ```
- Publish at natural completion points (a shipped unit of work, docs riding the same commit) —
  not on a schedule, and never mid-change. The working tree must be clean and the build
  warning-free at the published commit (see `VERIFICATION.md`). No tag → no push: the tag is
  what makes a `main` state a published state.

## Distribution: Velopack installers, built locally (adopted 2026-08-02)

Installers ship as GitHub Releases **packed and uploaded from this machine** via
`scripts\release.ps1` — the portfolio's one release mechanism (TSM/TP model). This replaced
the tag-triggered GitHub Actions workflow (`release.yml`, removed 2026-08-02).

One-time setup: `dotnet tool install -g vpk`, and `$env:GITHUB_TOKEN` = a PAT with
`public_repo` scope (only needed for upload; `-NoUpload` dry-runs without it).

Per-release flow:
```powershell
# on main, at the published commit (see Branch policy)
git tag vX.Y.Z
git push origin main vX.Y.Z
.\scripts\release.ps1          # publish → vpk pack → upload to GitHub Releases
```
- **Versions come from the tag:** the script reads the latest reachable tag and injects it as
  `InformationalVersion` at publish (shown in the window title) and as the Velopack package
  version. `AssemblyVersion` stays hand-set in the csproj (no MinVer here).
- **The installed app self-updates**: startup check of this repo's Releases via Velopack
  (`MainForm.CheckForUpdatesAsync`); a published release propagates to installed copies on
  their next launch.
- **Dry-run:** `.\scripts\release.ps1 -NoUpload` → artifacts in `Releases\` (gitignored, as is
  the `publish\` staging dir); run the Setup.exe there to test an install locally. vpk refuses
  to re-pack a version already present in `Releases\` — delete that folder before repeating a
  dry-run at the same tag.
- The app's `Velopack` NuGet package and the `vpk` CLI should stay on matching versions
  (both 1.2.0 as of 2026-08-02) — `vpk pack` warns on skew.

Latest released tag: **`v2.0.0`** (first script-built release; `v1.9.0` and earlier were
CI-built).

## Content rules (what is deliberately public)

- **`README.md` is the storefront** — user-facing description only (what XFM does, install,
  usage caveats, license). Development/testing minutiae stay out.
- **`TestData/` sample images are deliberately committed** and therefore public.
- **Never in the repo, so never published:** tokens/credentials (none exist).
- History publishes whole. Anything that must not be public must never be committed — there
  is no post-hoc scrub step.
