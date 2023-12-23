[CmdletBinding()]
param (
    [Parameter(Position = 0)]
    [string]$ProjectOrSolution = 'Boldly.sln',
    [ValidateSet('Release', 'Debug')]
    [string]$Configuration = 'Release',
    [string]$Version = '0.0.0',
    [Switch]$BuildClientApp
)

$semVersionRegExPattern = '^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$'
if ($Version -NotMatch $semVersionRegExPattern) {
    Write-Error "$Version is not a valid version."
    Exit -1
}

$buildArgs = @($ProjectOrSolution,
    "-Restore"
    "-MaxCpuCount"
    "-Target:Build"
    "-Property:Configuration=$Configuration"
    "-Property:Version=$Version"
    "-Property:BuildClientApp=$($BuildClientApp.ToString())"
    "-ConsoleLoggerParameters:Summary"
    "-Verbosity:Minimal"
)

Write-Host "`nMSBuild arguments:"
$buildArgs | Write-Host
Write-Host ""

dotnet msbuild $buildArgs

Exit $LASTEXITCODE