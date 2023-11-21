param
  (
    [Parameter(Position=0, Mandatory)]
    [ValidateNotNullOrEmpty()]
    [string]$buildNumber
  )

if([string]::IsNullOrWhiteSpace($buildNumber)){
    throw "Please specify the version build number to be used"
}

function Update-CsprojFile {

    param (
		[Parameter(Position=0, Mandatory)]
		[ValidateNotNullOrEmpty()]
        [string]$csprojFile,
		[Parameter(Position=1, Mandatory)]
		[ValidateNotNullOrEmpty()]
		[string]$buildNumber
    )

	$csprojXml = [xml](get-content ($csprojFile))

	$versionNode = $csprojXml.Project.PropertyGroup.Version
	if ($versionNode -eq $null) {
		# create version node if it doesn't exist

		$versionNode = $csprojXml.CreateElement("Version")
		$csprojXml.Project.PropertyGroup.AppendChild($versionNode) > $null
		#Write-Host "Version XML element added to the PropertyGroup of $($csprojFile)"

		$version = "1.0.0"
	}
	else
	{
		#Write-Host "Version XML element found in the PropertyGroup of $($csprojFile)"
		$version = $versionNode
	}

	$index = $version.LastIndexOf('.')
	if ($index -eq -1) {
		throw "$version isn't in the expected build format. $version = $($version))"
	}

	$version = $version.Substring(0, $index + 1) + $buildNumber

	Write-Host "Stamping $($csprojFile) with version number $($version)"

	$csprojXml.Project.PropertyGroup.Version = $version


	$csprojXml.Save($csprojFile)
}

#Enumerate the csproj files that are under .\src (exclude 3rdParty folder)
Get-ChildItem "./src" -Exclude "3rdParty" | 
ForEach-Object {
    
  Get-ChildItem $_.FullName -Include *.csproj -Recurse |
  ForEach-Object {
    Update-CsprojFile $_.FullName $buildNumber
  }
}
