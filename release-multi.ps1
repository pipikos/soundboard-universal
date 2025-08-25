param(
  [string]$Version = "v0.2.0",
  [string[]]$RIDs = @("win-x64","linux-x64","osx-x64"),
  [switch]$SelfContained
)

$ErrorActionPreference = "Stop"

# self-contained flag (no ternary to support Windows PowerShell 5.1)
$sc = "false"
if ($SelfContained) { $sc = "true" }

# find project
$proj = Get-ChildItem -Path $PSScriptRoot -Filter *.csproj | Select-Object -First 1
if (-not $proj) { throw "No .csproj found in $PSScriptRoot" }

dotnet clean | Out-Null
dotnet restore | Out-Null

$zips = @()
foreach ($rid in $RIDs) {
  Write-Host "Publishing $rid (self-contained=$sc)..."
  dotnet publish -c Release -r $rid --self-contained $sc `
    -p:PublishSingleFile=false `
    -p:IncludeNativeLibrariesForSelfExtract=true | Out-Host

  $pub = Join-Path $PSScriptRoot "bin\Release\net8.0\$rid\publish"
  if (-not (Test-Path -LiteralPath $pub)) { throw "Publish dir not found: $pub" }

  # ensure config.json is present
  $configSrc = Join-Path $PSScriptRoot "config.json"
  $configDst = Join-Path $pub "config.json"
  if ((Test-Path -LiteralPath $configSrc) -and (-not (Test-Path -LiteralPath $configDst))) {
    Copy-Item -LiteralPath $configSrc -Destination $configDst -Force
  }

  # zip
  $zip = "Soundboard.Universal-$Version-$rid.zip"
  if (Test-Path -LiteralPath $zip) { Remove-Item -LiteralPath $zip -Force }
  Compress-Archive -Path (Join-Path $pub "*") -DestinationPath $zip -Force

  $zips += (Resolve-Path $zip).Path
}

Write-Host "Built zips:"
$zips | ForEach-Object { Write-Host " - $_" }
