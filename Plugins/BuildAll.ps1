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

$fileList = Get-ChildItem -Directory Plugins
foreach($file in $fileList)
{
    $buildDir = "$file\bin\Publish"
    $dllpath = "$buildDir\$file.dll"
    if(Test-Path $dllpath)
    {
        Write-Host "Skipped publish, file already exists."
    }else{
        dotnet publish $PSScriptRoot/$file -o $buildDir -c Release
    }   
    Copy-Item $dllpath $targetDir/Plugins
}
Write-Host "copied to the ${targetDir}"