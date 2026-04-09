<#
.SYNOPSIS
对齐dotnet编译参数的跨平台下载ffmpeg脚本
#>
param(
    # 和dotnet编译一致的运行时标识符，直接复用dotnet的-r参数即可
    [Parameter(Mandatory = $false)]
    [Alias("r")]
    [ValidateSet("win-x64", "win-x86", "win-arm64", "linux-x64", "osx-x64", "osx-arm64")]
    [string]$RuntimeIdentifier
)

# dotnet RID 和 我们压缩包平台名的映射
$ridToFfmpegPlatform = @{
    "win-x64"       = "windows-x64"
    "win-x86"       = "windows-x86"
    "win-arm64"     = "windows-arm64"
    "linux-x64"     = "linux-x64"
    "osx-x64"       = "osx-x64"
    "osx-arm64"     = "osx-arm64"
}


# 未传参数时自动检测当前平台的RID
if (-not $RuntimeIdentifier) {
    Write-Host "未指定目标运行时，自动检测当前平台..."
    # 操作系统部分
    if ($IsWindows) { $os = "win" }
    elseif ($IsLinux) { $os = "linux" }
    elseif ($IsMacOS) { $os = "osx" }
    else {
        Write-Error "无法识别当前操作系统，请手动通过 -r 参数指定运行时，例如：-r win-x64"
        exit 1
    }
    # 架构部分
    $arch = [Environment]::GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").ToLower()
    $arch = $arch -replace "^amd64$", "x64" -replace "^x86$", "x86" -replace "^arm64$", "arm64"
    $RuntimeIdentifier = "$os-$arch"

    if (-not $ridToFfmpegPlatform.ContainsKey($RuntimeIdentifier)) {
        Write-Error "自动检测到的运行时 $RuntimeIdentifier 暂不支持，请手动指定支持的运行时：win-x64、win-x86、win-arm64、linux-x64、osx-x64、osx-arm64"
        exit 1
    }
    Write-Host "自动检测到目标运行时：$RuntimeIdentifier"
}

# ========== 核心逻辑 ==========
$ffmpegPlatform = $ridToFfmpegPlatform[$RuntimeIdentifier]
$downloadUrl = "https://github.com/Harlan-H/FFmpegBuilds/releases/download/7.1/ffmpeg-$ffmpegPlatform.zip"

# 确定可执行文件名
if ($RuntimeIdentifier.StartsWith("win")) {
    $exeName = "ffmpeg.exe"
} else {
    $exeName = "ffmpeg"
}
$ffmpegFilePath = Join-Path $PSScriptRoot $exeName

# 已存在则跳过
if (Test-Path $ffmpegFilePath) {
    Write-Host "ffmpeg文件已经存在跳过下载."
    exit 0
}

Write-Host "开始下载 ffmpeg-$RuntimeIdentifier..."
# 下载压缩包
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$wc = New-Object System.Net.WebClient
$wc.DownloadFile($downloadUrl, "$ffmpegFilePath.zip")
$wc.Dispose()

# 解压可执行文件
Add-Type -Assembly System.IO.Compression.FileSystem
$zip = [IO.Compression.ZipFile]::OpenRead("$ffmpegFilePath.zip")
$ffmpegEntry = $zip.GetEntry($exeName)

if (-not $ffmpegEntry) {
    Write-Error "解压失败：压缩包中未找到ffmpeg可执行文件 $exeName"
    $zip.Dispose()
    Remove-Item "$ffmpegFilePath.zip" -Force
    exit 1
}

[IO.Compression.ZipFileExtensions]::ExtractToFile($ffmpegEntry, $ffmpegFilePath)
$zip.Dispose()

# 删除临时压缩包
Remove-Item "$ffmpegFilePath.zip" -Force

# 给非Windows平台添加执行权限
if (-not $RuntimeIdentifier.StartsWith("win")) {
    chmod +x $ffmpegFilePath
}

Write-Host "下载ffmpeg- $RuntimeIdentifier 完成."
exit 0