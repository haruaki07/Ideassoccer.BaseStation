param (
    [Parameter(ParameterSetName='NewVersionSet')]
    [switch]$Help,
    [string]$NewVersion,
    [switch]$Major,
    [switch]$Minor,
    [switch]$Patch,
    [switch]$Revision,
    [switch]$AllowSameVersion,
    [string]$CommitMessage,
    [switch]$NoTag
)

if ($Help) {
    Write-Host
    Write-Host "Updates version.txt, commit it, then create a git tag. Version format is 'v<major>.<minor>.<patch>.<revision>'."
    Write-Host
    Write-Host "Usage: bump-version.ps1 [-NewVersion <version>] [-Major] [-Minor] [-Patch] [-Revision] [-Help]"
    Write-Host
    Write-Host "Parameters:"
    Write-Host "  -NewVersion <version>   Creates a new version specified by <version>."
    Write-Host "  -Major                  Creates a new version by incrementing the major of the current version."
    Write-Host "  -Minor                  Creates a new version by incrementing the minor of the current version."
    Write-Host "  -Patch                  Creates a new version by incrementing the patch of the current version."
    Write-Host "  -Revision               Creates a new version by incrementing the revision of the current version."
    Write-Host "  -AllowSameVersion       Allow new version same as current version."
    Write-Host "  -CommitMessage          Specify commit message. Default is the new version"
    Write-Host "  -NoTag                  Skips creating the git tag."
    exit
}

# Read the current version from version.txt
$currentVersion = Get-Content -Path "version.txt"

# Determine if any increment options were specified
$incrementSpecified = $Major.IsPresent -or $Minor.IsPresent -or $Patch.IsPresent -or $Revision.IsPresent

# Check if -NewVersion is provided or any increment options are specified
if (-not $NewVersion -and -not $incrementSpecified) {
    Write-Error "You must provide either -NewVersion or at least one of the increment options (-Major, -Minor, -Patch, -Revision)."
    exit 1
}

# Check if -NewVersion and any increment options are provided together
if ($NewVersion -and $incrementSpecified) {
    Write-Error "You cannot provide both -NewVersion and any of the increment options (-Major, -Minor, -Patch, -Revision)."
    exit 1
}

# Determine the new version based on the parameters
if ($NewVersion) {
    if (-not ($NewVersion -match '^v([0-9]+)\.([0-9]+)\.([0-9]+)\.([0-9]+)')) {
        Write-Error "Invalid version format: $($NewVersion). Should be 'v<major>.<minor>.<patch>.<revision>'. Aborting..."
        exit 1
    }

    $_newVersion = $NewVersion
}
elseif ($Major) {
    $versionComponents = $currentVersion -split '\.'
    $newMajor = [int]$versionComponents[0].Substring(1) + 1
    $_newVersion = "v$newMajor.0.0.0"
}
elseif ($Minor) {
    $versionComponents = $currentVersion -split '\.'
    $newMinor = [int]$versionComponents[1] + 1
    $_newVersion = "v$($versionComponents[0].Substring(1)).$newMinor.0.0"
}
elseif ($Patch) {
    $versionComponents = $currentVersion -split '\.'
    $newPatch = [int]$versionComponents[2] + 1
    $_newVersion = "v$($versionComponents[0].Substring(1)).$($versionComponents[1]).$newPatch.0"
}
elseif ($Revision) {
    $versionComponents = $currentVersion -split '\.'
    $newRevision = [int]$versionComponents[3] + 1
    $_newVersion = "v$($versionComponents[0].Substring(1)).$($versionComponents[1]).$($versionComponents[2]).$newRevision"
}

if ($_newVersion -eq $currentVersion -and -not $AllowSameVersion) {
    Write-Error "Version is not changed. Aborting..."
    exit 1
}

# Update the version.txt file
$_newVersion | Out-File -FilePath "version.txt" -Force

# Create a git tag with the new version
if (!$NoTag) {
    try {
        if (!$CommitMessage) {
            $CommitMessage = $_newVersion;
        }

        git add version.txt
        git commit -m $CommitMessage
        if ($LASTEXITCODE -ne 0) {
            Throw [Exception]::new("An error occurred while creating git commit.")
        }

        if ($AllowSameVersion) {
            $tagOutput = git tag -a $_newVersion -m $_newVersion -f
        }
        else {
            $tagOutput = git tag -a $_newVersion -m $_newVersion
        }

        if ($LASTEXITCODE -ne 0) {
            Throw [Exception]::new("An error occurred while creating git tag.")
        }
    }
    catch {
        $currentVersion | Out-File -FilePath "version.txt" -Force
        Write-Error $_.Exception.Message + " Rolling back..."
        exit 1
    }
}

Write-Host "info Current version: $($currentVersion)"
Write-Host "info New version: $($_newVersion)"
if (!$NoTag) {
    Write-Host "info Tag created: $($tagOutput)"
}
else {
    Write-Host "info No tag was created."
}