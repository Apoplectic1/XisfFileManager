# Releasing

> **Charter:** How a release is cut and what GitHub is for. Read before merging to `main` or tagging.

## Branch model

- **`dev`** — active development branch (default; new work lands here).
- **`main`** — stable/release branch; merge `dev` into `main` when cutting a release.

## Cut a release

1. Merge `dev` → `main` locally.
2. Create an **annotated** tag `vX.Y.Z` on `main` and push `main` + the tag.
3. The push triggers `.github/workflows/release.yml`, which injects the tag into
   `AssemblyInformationalVersion` (shown in the window title), publishes a self-contained win-x64
   build, packages it with Velopack (`vpk pack`), and uploads the installer + update assets to a
   GitHub Release.
4. **Installed copies pick the release up automatically**: the app checks GitHub Releases at
   startup via Velopack (`MainForm.CheckForUpdatesAsync`) — a tag push propagates to installed
   apps on their next launch.

Latest released tag: **`v1.8.0`**.

## GitHub policy

The local tree is the source of truth; `origin` (github.com/Apoplectic1/XisfManager) is a
**distribution channel**. Push `main` and release tags only — never push or sync `dev` or feature
branches. (Two stale pre-policy branches remain on origin — `TargetScheduler` and
`C++/CLI_for_PCL_Library` — prune candidates.)
