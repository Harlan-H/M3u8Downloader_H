param(
    [Parameter(Mandatory=$true)]
    [string]$PublishDir,

    [Parameter(Mandatory=$true)]
    [string]$IconPath,

    [Parameter(Mandatory=$true)]
    [string]$Version,

    [Parameter(Mandatory=$true)]
    [string]$ShortVersion
)

Write-Host "开始 macOS 打包..."
Write-Host "PublishDir: $PublishDir"
Write-Host "IconPath: $IconPath"
Write-Host "Version: $Version"
Write-Host "ShortVersion: $ShortVersion"

# 规范路径

$PublishDir = (Resolve-Path $PublishDir).Path

# .app 结构

$AppDir = Join-Path $PublishDir "M3u8Downloader_H.app"
$ContentsDir = Join-Path $AppDir "Contents"
$MacOSDir = Join-Path $ContentsDir "MacOS"
$ResourcesDir = Join-Path $ContentsDir "Resources"


# 创建目录结构

New-Item -ItemType Directory -Force -Path $MacOSDir | Out-Null
New-Item -ItemType Directory -Force -Path $ResourcesDir | Out-Null

Write-Host "复制 publish 文件..."

$Items = Get-ChildItem $PublishDir

foreach ($Item in $Items) {
    if ($Item.FullName -eq $AppDir) {
        continue
    }

    Move-Item $Item.FullName $MacOSDir -Force
}


Copy-Item $IconPath -Destination (Join-Path $ResourcesDir "AppIcon.icns") -Force

$TargetExe = Join-Path $MacOSDir $AppName

# 赋执行权限

chmod +x $TargetExe

Write-Host "生成 Info.plist..."

$PlistPath = Join-Path $ContentsDir "Info.plist"

$PlistContent = @"

<?xml version="1.0" encoding="UTF-8"?>

<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN"
"http://www.apple.com/DTDs/PropertyList-1.0.dtd">

<plist version="1.0">
<dict>
    <key>CFBundleName</key>
    <string>M3u8Downloader_H</string>

    <key>CFBundleDisplayName</key>
    <string>M3u8Downloader_H</string>

    <key>CFBundleIdentifier</key>
    <string>com.Harlan-H.M3u8Downloader_H</string>

    <key>CFBundleVersion</key>
    <string>$Version</string>

    <key>CFBundleShortVersionString</key>
    <string>$ShortVersion</string>

    <key>CFBundleExecutable</key>
    <string>M3u8Downloader_H</string>

    <key>CFBundleIconFile</key>
    <string>AppIcon</string>

    <key>CFBundlePackageType</key>
    <string>APPL</string>

    <key>NSHighResolutionCapable</key>
    <true/>

</dict>
</plist>
"@

$PlistContent | Out-File $PlistPath -Encoding utf8

Write-Host "修复权限..."

chmod -R +x $MacOSDir

Write-Host "macOS App 打包完成:"
Write-Host $AppDir
