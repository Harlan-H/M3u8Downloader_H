<# 
.SYNOPSIS
自动为 M3u8Downloader_H 程序添加端口URL ACL权限，自动提权
请使用powershell执行此脚本
#>

# 你要绑定的端口，修改这里即可
$TARGET_PORT = 65432

# 检测当前是否是管理员
$windowsPrincipal = New-Object Security.Principal.WindowsPrincipal(
    [Security.Principal.WindowsIdentity]::GetCurrent()
)
$isAdmin = $windowsPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    # 不是管理员，弹窗提示，然后用管理员身份重启本脚本
    Write-Host "当前不是管理员权限，正在请求管理员权限..." -ForegroundColor Yellow
    Start-Process powershell.exe "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`"" -Verb RunAs
    exit
}

# 已经是管理员，开始执行
Clear-Host
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "正在为M3u8Downloader_H程序添加端口 $TARGET_PORT 访问权限..." -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "`n"

try {
    # 先删除旧的冲突规则
    $oldUrls = @(
        "http://+:${TARGET_PORT}/",
        "http://*:${TARGET_PORT}/",
        "http://0.0.0.0:${TARGET_PORT}/",
        "http://localhost:${TARGET_PORT}/"
    )

    foreach ($url in $oldUrls) {
        Write-Host "正在清理旧规则: $url"
        # 删除的时候忽略错误（本来不存在也没关系）
        netsh http delete urlacl url=$url 2>&1 | Out-Null
    }

    Write-Host "`n正在添加新权限规则..."
    $user = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name
    netsh http add urlacl url=http://+:${TARGET_PORT}/ user="$user"
    if ($LASTEXITCODE -eq 0) {
        Write-Host "`n✅ 操作成功！端口 $TARGET_PORT 已经对 $user 用户开放权限" -ForegroundColor Green
        Write-Host "现在可以重新启动你的程序，不需要管理员权限了" -ForegroundColor Green
    } else {
        Write-Host "`n❌ 添加失败，错误信息：" -ForegroundColor Red
        Write-Host $result -ForegroundColor Red
    }
} catch {
    Write-Host "`n❌ 发生异常：$_" -ForegroundColor Red
}

Write-Host "`n正在配置防火墙规则..."

netsh advfirewall firewall add rule name="M3u8Downloader_H_$TARGET_PORT" `
    dir=in action=allow protocol=TCP localport=$TARGET_PORT 2>&1 | Out-Null

Write-Host "✅ 防火墙规则已添加"


Write-Host "`n按任意键关闭窗口..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

