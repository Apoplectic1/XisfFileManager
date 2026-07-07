# Releasing

> **Charter:** How a release is cut and what GitHub is for. Read before merging to `main` or tagging.

## Branch model

- **`dev`** — active development branch (default; new work lands here).
- **`main`** — stable/release branch; merge `dev` into `main` when cutting a release.

## Cut a release

1. Merge `dev` → `main` locally.
2. Create an **annotated** tag `vX.Y.Z` on `main` and push `main` + the tag.
3. The push triggers `.github/workflows/release.yml`, which injects the tag into
   `AssemblyInformationalVersion` (shown in the window title) and builds the release.

Latest released tag: **`v1.8.0`**.

## GitHub policy

The local tree is the source of truth; `origin` (github.com/Apoplectic1/XisfManager) is a
**distribution channel**. Push `main` and release tags only — never push or sync `dev` or feature
branches.
