[CmdletBinding()]
param (
    [Parameter(Position = 0)]
    [string]$ProjectOrSolution = 'ChangeBlog.sln',
    [ValidateSet('Release', 'Debug')]
    [string]$Configuration = 'Release',
    [Switch]$TestClientApp
)

$testArgs = @($ProjectOrSolution
    "-Property:Configuration=$Configuration"
    "-Property:TestClientApp=$($TestClientApp.ToString())"
    '--no-build'
)

Write-Host "`n'dotnet test' arguments:"
$testArgs | Write-Host
Write-Host ""

dotnet test $testArgs

Exit $LASTEXITCODE