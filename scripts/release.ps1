#requires -version 5.1
# Build, pack, and (optionally) upload a XisfFileManager release to GitHub.
# Modeled on TSM's scripts/release.ps1; XFM difference: the exe is XisfFileManager.exe.
#
# Prerequisites (one-time per machine):
#   dotnet tool install -g vpk
#   $env:GITHUB_TOKEN = "<personal-access-token-with-public_repo-scope>"
#
# Per-release flow (see RELEASING.md):
#   1. git tag vX.Y.Z on main, push main + tag
#   2. .\scripts\release.ps1
#
# The script reads the latest reachable tag via `git describe --tags --abbrev=0` and uses
# that as the release version. MinVer (in XisfFileManager.csproj) reads the same tag at build
# time so the assembly version matches.

[CmdletBinding()]
param(
    # Skip the GitHub upload step (useful for local dry-runs of vpk pack).
    [switch] $NoUpload
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repoRoot = Split-Path -Parent $PSScriptRoot
Push-Location $repoRoot
try {
    $tag = git describe --tags --abbrev=0 2>$null
    if (-not $tag) {
        throw "No git tag reachable from HEAD. Tag a release first (e.g. 'git tag v2.0.0')."
    }
    $version = $tag.TrimStart('v')
    Write-Host "Releasing XisfFileManager $version (tag $tag)" -ForegroundColor Cyan

    Write-Host "`n--> dotnet publish (Release|win-x64, self-contained)" -ForegroundColor Cyan
    dotnet publish XisfFileManager/XisfFileManager.csproj -c Release -r win-x64 --self-contained true -o .\publish -nologo
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed" }

    $publish = Join-Path $repoRoot 'publish'
    if (-not (Test-Path (Join-Path $publish 'XisfFileManager.exe'))) { throw "Publish output not found at $publish" }

    # AL coordination gate (see RELEASING.md): arms on any Astronomy.* DLL in the payload.
    # ARMED 2026-08-06 by the Astronomy.XISF adoption (adopt-al-xisf-compression).
    $alDlls = @(Get-ChildItem $publish -Filter 'Astronomy.*.dll')
    if ($alDlls.Count -gt 0) {
        $alDirty = git -C (Join-Path $repoRoot '..\Library') status --porcelain
        if ($alDirty) { throw "..\Library working tree is dirty - commit and release AL first (Library\RELEASING.md)." }
        foreach ($dll in $alDlls) {
            $alVer = $dll.VersionInfo.ProductVersion
            if ($alVer -match '-alpha') { throw "Embedded $($dll.Name) stamps '$alVer' (untagged AL state) - release AL first (Library\RELEASING.md)." }
        }
    }

    Write-Host "`n--> vpk pack" -ForegroundColor Cyan
    vpk pack `
        -u XisfFileManager `
        -v $version `
        -p $publish `
        -e XisfFileManager.exe `
        -i XisfFileManager\XisfFileManager.ico `
        --packTitle 'XISF File Manager'
    if ($LASTEXITCODE -ne 0) { throw "vpk pack failed" }

    if ($NoUpload) {
        Write-Host "`nDone. Skipping upload (-NoUpload). Output is in .\Releases\" -ForegroundColor Yellow
        return
    }

    if (-not $env:GITHUB_TOKEN) {
        throw "GITHUB_TOKEN env var is not set. Either set it or re-run with -NoUpload."
    }

    Write-Host "`n--> vpk upload github (publish)" -ForegroundColor Cyan
    # --tag aligns the GitHub release tag with the git tag (vpk's default would be the bare
    # version "2.0.0", but the git/RELEASING.md convention is "v2.0.0").
    vpk upload github `
        --repoUrl 'https://github.com/Apoplectic1/XisfFileManager' `
        --token $env:GITHUB_TOKEN `
        --tag $tag `
        --publish
    if ($LASTEXITCODE -ne 0) { throw "vpk upload failed" }

    Write-Host "`nReleased XisfFileManager $version to GitHub." -ForegroundColor Green
}
finally {
    Pop-Location
}
