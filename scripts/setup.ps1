param(
    [Parameter(Mandatory)]
    [ValidateSet("01-sequential-pattern", "02-concurrent-pattern", "03-handoff-pattern", "04-group-chat-pattern")]
    [string]$Session
)

$REPOSITORY_ROOT = git rev-parse --show-toplevel
$SAMPLE_DIR = "$REPOSITORY_ROOT/samples/$Session/start"
$WORKSHOP_DIR = "$REPOSITORY_ROOT/workshop"

if (Test-Path $WORKSHOP_DIR) {
    Write-Host "The 'workshop' directory already exists. Rename or remove it before running this script."
    exit 1
}

Write-Host "Setting up the workshop directory for session: $Session"

New-Item -ItemType Directory -Path $WORKSHOP_DIR -Force | Out-Null
Copy-Item -Path $SAMPLE_DIR/* -Destination $WORKSHOP_DIR -Recurse -Force

Get-ChildItem -Path $WORKSHOP_DIR -Filter "post-deploy.*" -Recurse | ForEach-Object {
    (Get-Content $_.FullName -Raw) -replace "samples/$Session/start", 'workshop' | Set-Content $_.FullName -NoNewline
}

Write-Host "Workshop directory is ready at: $WORKSHOP_DIR"