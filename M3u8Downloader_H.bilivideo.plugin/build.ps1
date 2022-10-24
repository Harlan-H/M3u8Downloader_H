$targetDir = $args[0]
if((Test-Path $targetDir) -eq $False)
{
    Write-Host "${targetDir} not found"
    exit
}

$dllpath = "$PSScriptRoot\bin\Publish\M3u8Downloader_H.bilivideo.plugin.dll"
if(Test-Path $dllpath)
{
    Write-Host "Skipped publish, file already exists."
}else{
    dotnet publish $PSScriptRoot/ -o $PSScriptRoot/bin/Publish -c Release
}

Copy-Item $PSScriptRoot/bin/Publish/M3u8Downloader_H.bilivideo.plugin.dll $targetDir/Plugins/
Write-Host "copied to the ${targetDir}"