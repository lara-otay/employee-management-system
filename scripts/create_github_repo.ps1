param(
    [string]$remoteUrl
)

if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    Write-Error "git is not installed or not on PATH"
    exit 1
}

if (-not $remoteUrl) {
    Write-Host "Usage: .\create_github_repo.ps1 -remoteUrl 'https://github.com/you/repo.git'"
    exit 1
}

Set-Location -Path (Split-Path -Parent $PSScriptRoot)

if (-not (Test-Path -Path .git)) {
    git init
}

git add .
git commit -m "Initial: add EmployeeManagement app"

git branch -M main

git remote add origin $remoteUrl

git push -u origin main

Write-Host "Pushed to $remoteUrl"
