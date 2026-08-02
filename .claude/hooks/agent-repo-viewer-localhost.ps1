param(
  [int]$Port = 8443
)
$Url = "https://localhost:$Port"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$Check = Join-Path $ScriptDir "..\scripts\check_localhost.py"
if (-not (Test-Path $Check)) { Write-Error "Missing check script at: $Check"; exit 1 }
python "$Check" $Url
exit $LASTEXITCODE
