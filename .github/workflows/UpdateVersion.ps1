param
  (
    [Parameter(Position=0, Mandatory)]
    [ValidateNotNullOrEmpty()]
    [string]$buildNumber
  )

if([string]::IsNullOrWhiteSpace($buildNumber)){
    throw "Please specify the version build number to be used"
}


$dotnetCountersFolder = Join-Path -Path $PSScriptRoot -ChildPath "../../src/Data.Json"
$csprojFile = Join-Path -Path $dotnetCountersFolder -ChildPath "Data.Json.csproj"

$csprojXml = [xml](get-content ($csprojFile))

$versionNode = $csprojXml.Project.PropertyGroup.Version
if ($versionNode -eq $null) {
    # create version node if it doesn't exist
    $versionNode = $csprojXml.CreateElement("Version")
    $csprojXml.Project.PropertyGroup.AppendChild($versionNode)
    Write-Host "AssemblyVersion XML tag added to $($csproj)"

    $version = "1.0.0"
}
else
{
    $version = $csprojXml.Project.PropertyGroup.Version
}

$index = $version.LastIndexOf('.')
if ($index -eq -1) {
    throw "$version isn't in the expected build format. $version = $($version))"
}

$version = $version.Substring(0, $index + 1) + $buildNumber

Write-Host "Stamping $($csproj) with version number $($version)"

$csprojXml.Project.PropertyGroup.Version = $version


$csprojXml.Save($csprojFile)
