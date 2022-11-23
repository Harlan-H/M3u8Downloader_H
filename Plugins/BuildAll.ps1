$targetDir = $args[0]

if([string]::IsNullOrEmpty($targetDir))
{
    Write-Host "The parameter cannot be empty"
    exit
}
if((Test-Path $targetDir) -eq $False)
{
    Write-Host "${targetDir} not found"
    exit
}

$currentPath = Get-Location
$dirList = [System.IO.Directory]::GetDirectories("$currentPath\Plugins")
foreach($dir in $dirList)
{
    $dirName = $dir.Substring($dir.LastIndexOf("\") + 1)
    $buildDir = "$dir\bin\Publish"
    $dllpath = "$buildDir\$dirName.dll"
    if(Test-Path $dllpath)
    {
        Write-Host "Skipped publish, file already exists."
    }else{
        dotnet publish $dir -o $buildDir -c Release
    }   
    Copy-Item $dllpath $targetDir/Plugins
}
Write-Host "copied to the ${targetDir}"