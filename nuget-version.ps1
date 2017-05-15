Param(
   [string]$packageName
)

$nugetOriginalUrl = "http://dist.nuget.org/win-x86-commandline/latest/nuget.exe"  #version 3.x
$buildDirectory = $env:BUILD_SOURCESDIRECTORY

# Helpers functions
function DownloadFile($source,$destination)
{
    $client = New-Object System.Net.WebClient
    $client.DownloadFile($source,$destination)
}

function disablePush()
{
}

function FindPackage($searchLoc, $packageFileName)
{
    $findNuget = Get-ChildItem -Path $searchLoc -Filter $packageFileName -Recurse | Sort-Object LastWriteTime | Select-Object -Last 1
    if($findNuget -eq $null)
    {
       Write-Host "Not found!" -ForegroundColor Red
       return $null       
    } 

    return $findNuget.FullName
}

function FindNuget($searchLoc)
{
    $findNuget = Get-ChildItem -Path $searchLoc -Filter "nuget.exe" -Recurse | Sort-Object LastWriteTime | Select-Object -Last 1
    if($findNuget -eq $null)
    {
       Write-Host "Not found!" -ForegroundColor Red
       return $null       
    } 

    return $findNuget.FullName
}

if(-not ($packageName -and $buildDirectory))
{
   Write-Error "Running script from outside VSO Agent is currently not supported"
   exit 1
}

Write-Host "Check if I can find default Nuget.exe" -NoNewline
$nuGetPath = ""
if(-not $nugetPath)
{
   Write-Host "Not found!" -ForegroundColor Red
   Write-Host "Check if I can find in working directory $PSScriptRoot " -NoNewline
   $nugetPath = FindNuget -searchLoc $PSScriptRoot

   if(-not $nugetPath)
   {
        Write-Host "Try to download from internet.." -NoNewline
        DownloadFile -source $nugetOriginalUrl -destination $PSScriptRoot\nuget.exe
        $nugetPath = FindNuget -searchLoc $PSScriptRoot
        if(-not $nugetPath)
        {
            disablePush
            throw("Can not find Nuget.exe")
        }
   }

   Write-Host ""
}

Write-Host "Check last version of package $packageName online"
$OutputVariable = & $nugetPath list $packageName -PreRelease -NonInteractive -ForceEnglishOutput | Out-String
$firstLine = (($OutputVariable -split '\n')[0]).ToString().Trim()
$resultsPackageName = ($firstLine -split '\s+')[0]
$resultsPackageVersion = ($firstLine -split '\s+')[1]
$packageFileName = "$resultsPackageName.$resultsPackageVersion.nupkg"

if ($firstLine -contains "error")
{
    disablePush
    throw("ERROR: $firstLine")
}

if ($firstLine -eq "No packages found.")
{
    Write-Host "Package not exists in Nuget Repository, you can push it."
    Write-Output ("##vso[task.setvariable variable=NugetEnabled;]push")
}

if ($resultsPackageName -ne $packageName)
{
    Write-Host "Package was not found. ERROR: $firstLine"
    disablePush    
}
else 
{
    Write-Host "The last version of package is $resultsPackageVersion"

    Write-Host "Check if exists the same version on build dir $buildDirectory */ $packageFileName " -NoNewline
    $packagePath = FindPackage -searchLoc $buildDirectory -packageFileName $packageFileName

    Write-Host ""

    if (-not $packagePath)
    {
        Write-Host "Package $packageName or file $packageFileName does not exists in this build, you can push it!"
        Write-Output ("##vso[task.setvariable variable=NugetEnabled;]push")
    }
    else
    {
        Write-Warning "The package already exists on the build directory, you can't push the same package."
        disablePush
    }
}