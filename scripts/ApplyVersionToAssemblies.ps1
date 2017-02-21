##-----------------------------------------------------------------------
## <copyright file="ApplyVersionToAssemblies.ps1">(c) Microsoft Corporation. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
##-----------------------------------------------------------------------
# Look for a 0.0.0 (semver) pattern in the build number. 
# Output it if found.
#
# Create mixed semver/.net version number as:
#  MAJOR.MINOR.BUILD
#
# Use MAJOR.MINOR.0.0 as the assembly version (for compatibility checking)
# Use MAJOR.MINOR.BUILD.0 as the file version (needs 4 parts for .net)
# Use full semver with build description as the informational version


# Enable -Verbose option
[CmdletBinding()]

# Regular expression pattern to find the version in the build number 
# and then apply it to the assemblies
#$VersionRegex = "\d+\.\d+\.\d+\.\d+"
# SemVer is only MAJOR.MINOR.PATCH (dotnet is MAJOR.MINOR.BUILD.REVISION)
$VersionRegex = "\d+\.\d+\.\d+"

# If this script is not running on a build server, remind user to 
# set environment variables so that this script can be debugged
if(-not ($Env:BUILD_SOURCESDIRECTORY -and $Env:BUILD_BUILDNUMBER))
{
    Write-Error "You must set the following environment variables"
    Write-Error "to test this script interactively."
    Write-Host 'For example, enter something like:'
    Write-Host '$Env:BUILD_SOURCESDIRECTORY = "C:\code\xyz-donate\XYZDonate"'
    Write-Host '$Env:BUILD_BUILDNUMBER = "Build HelloWorld_0000.00.00.0"'

    Write-Host '$Env:MAJORVERSION = "1"'
    Write-Host '$Env:MINORVERSION = "2"'
    Write-Host '$Env:BUILD_BUILDID = "3"'
    Write-Host '$Env:BUILD_SOURCEBRANCHNAME = "master"'
    Write-Host '$Env:BUILD_SOURCEVERSION = "d825102"'
    exit 1
}

# Make sure path to source code directory is available
if (-not $Env:BUILD_SOURCESDIRECTORY)
{
    Write-Error ("BUILD_SOURCESDIRECTORY environment variable is missing.")
    exit 1
}
elseif (-not (Test-Path $Env:BUILD_SOURCESDIRECTORY))
{
    Write-Error "BUILD_SOURCESDIRECTORY does not exist: $Env:BUILD_SOURCESDIRECTORY"
    exit 1
}
Write-Verbose "BUILD_SOURCESDIRECTORY: $Env:BUILD_SOURCESDIRECTORY"

# Make sure there is a build number
if (-not $Env:BUILD_BUILDNUMBER)
{
    Write-Error ("BUILD_BUILDNUMBER environment variable is missing.")
    exit 1
}
Write-Verbose "BUILD_BUILDNUMBER: $Env:BUILD_BUILDNUMBER"

Write-Verbose "MAJORVERSION: $Env:MAJORVERSION"
Write-Verbose "MINORVERSION: $Env:MINORVERSION"
Write-Verbose "BUILD_BUILDID: $Env:BUILD_BUILDID"
Write-Verbose "BUILD_SOURCEBRANCHNAME: $Env:BUILD_SOURCEBRANCHNAME"
Write-Verbose "BUILD_SOURCEVERSION: $Env:BUILD_SOURCEVERSION"

# Get and validate the version data
$VersionData = [regex]::matches($Env:BUILD_BUILDNUMBER,$VersionRegex)
switch($VersionData.Count)
{
   0        
      { 
         Write-Error "Could not find version number data in BUILD_BUILDNUMBER."
         exit 1
      }
   1 {}
   default 
      { 
         Write-Warning "Found more than instance of version data in BUILD_BUILDNUMBER." 
         Write-Warning "Will assume first instance is version."
      }
}
$NewVersion = $VersionData[0]
Write-Verbose "Version from build number: $NewVersion"
$MajorMinor = "$Env:MAJORVERSION.$Env:MINORVERSION"
$NewAssemblyVersion = "$MajorMinor.0.0"
$NewAssemblyFileVersion = "$MajorMinor.$Env:BUILD_BUILDID.0"
$NewAssemblyInformationalVersion = "$MajorMinor.$Env:BUILD_BUILDID+$Env:BUILD_SOURCEBRANCHNAME.$Env:BUILD_SOURCEVERSION"

Write-Verbose "AssemblyVersion: $NewAssemblyVersion"
Write-Verbose "AssemblyFileVersion: $NewAssemblyFileVersion"
Write-Verbose "AssemblyInformationalVersion: $NewAssemblyInformationalVersion"

$AssemblyVersionRegex = "^\s*\[\s*assembly:\s*AssemblyVersion\s*\(\s*`"([^`"])*`"\s*\)\s*\]\s*$"
$AssemblyFileVersionRegex = "^\s*\[\s*assembly:\s*AssemblyFileVersion\s*\(\s*`"([^`"])*`"\s*\)\s*\]\s*$"
$AssemblyInformationalVersionRegex = "^\s*\[\s*assembly:\s*AssemblyInformationalVersion\s*\(\s*`"([^`"])*`"\s*\)\s*\]\s*$"

# Apply the version to the assembly property files
$files = gci $Env:BUILD_SOURCESDIRECTORY -recurse -include "*Properties*","My Project" | 
    ?{ $_.PSIsContainer } | 
    foreach { gci -Path $_.FullName -Recurse -include AssemblyInfo.* }
if($files)
{
    Write-Verbose "Will apply version to $($files.count) files."

    foreach ($file in $files) {
        $fileContent = Get-Content($file)
        attrib $file -r
        $fileContent = $fileContent -replace $AssemblyVersionRegex, "[assembly:AssemblyVersion(`"$NewAssemblyVersion`")]"
        $fileContent = $fileContent -replace $AssemblyFileVersionRegex, "[assembly:AssemblyFileVersion(`"$NewAssemblyFileVersion`")]"
        $fileContent = $fileContent -replace $AssemblyInformationalVersionRegex, "[assembly:AssemblyInformationalVersion(`"$NewAssemblyInformationalVersion`")]"
		$fileContent | Out-File $file
        Write-Verbose "$file.FullName - version applied"
    }
}
else
{
    Write-Warning "Found no files."
}