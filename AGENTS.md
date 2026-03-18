# M3u8Downloader_H 项目文档

## 项目概述

M3u8Downloader_H 是一个功能强大的 M3U8 视频流下载器，采用 C# WPF 技术构建。该应用程序支持多线程、多任务和断点续传，能够自动解密 AES-128/192/256-CBC 加密的视频流，并支持 TS 和 FMP4 格式的下载与合并。

### 核心特性

- **多任务下载**：支持同时下载多个视频任务
- **断点续传**：下载中断后可继续下载
- **加密解密**：自动识别并解密 AES-128/192/256-CBC 加密的视频流
- **批量下载**：支持批量处理多个下载任务
- **代理支持**：可在设置中配置代理服务器
- **自定义请求头**：支持自定义 HTTP 请求头
- **插件系统**：提供插件功能，可扩展下载能力
- **HTTP API**：提供 RESTful API 接口，支持第三方调用
- **长视频支持**：支持 MP4、FLV 等长视频格式下载
- **文件夹快速合并**：支持拖拽文件夹实现快速视频合并

### 项目信息

- **当前版本**：4.0.1.0
- **目标框架**：.NET 9.0 Windows
- **开发工具**：Visual Studio 2022
- **许可证**：参见 LICENSE.txt
- **作者**：Harlan
- **GitHub**：https://github.com/Harlan-H/M3u8Downloader_H

## 技术栈

### 核心技术

- **.NET 9.0**：最新的 .NET 框架
- **WPF (Windows Presentation Foundation)**：UI 框架
- **Caliburn.Micro**：MVVM 框架，用于实现 MVVM 架构
- **MaterialDesignThemes**：Material Design 风格的 UI 库
- **PropertyChanged.Fody**：自动实现 INotifyPropertyChanged 接口
- **FFmpeg**：用于视频转码和合并

### NuGet 包

- Caliburn.Micro (4.0.230)
- MaterialDesignThemes (5.2.1)
- PropertyChanged.Fody (4.1.0)

## 项目架构

项目采用分层架构设计，包含以下模块：

### 主要模块

1. **M3u8Downloader_H**（主应用程序）
   - WPF UI 界面
   - MVVM 架构实现
   - 主窗口和仪表板视图
   - 使用 Caliburn.Micro 进行依赖注入

2. **M3u8Downloader_H.Core**（核心功能）
   - 核心下载逻辑
   - 任务管理
   - 进度跟踪

3. **M3u8Downloader_H.Downloader**（下载器）
   - M3U8 下载器实现
   - 媒体下载器（支持长视频）
   - 加密视频下载器
   - 直播流下载器
   - 插件化下载器

4. **M3u8Downloader_H.Combiners**（合并器）
   - M3U8 文件合并
   - FFmpeg 视频转码
   - 支持多种视频格式

5. **M3u8Downloader_H.M3U8**（M3U8 解析）
   - M3U8 文件解析
   - 属性读取器
   - 文件读取管理器
   - 支持插件扩展解析

6. **M3u8Downloader_H.Plugin**（插件系统）
   - 插件管理器
   - 插件加载和卸载
   - 插件接口实现

7. **M3u8Downloader_H.Plugin.Abstractions**（插件抽象层）
   - 插件接口定义
   - 下载器抽象
   - M3U8 抽象
   - 设置抽象

8. **M3u8Downloader_H.RestServer**（REST API 服务器）
   - HTTP 监听服务
   - API 接口实现
   - 请求和响应处理

9. **M3u8Downloader_H.Settings**（设置管理）
   - 应用程序设置
   - 用户偏好配置

10. **M3u8Downloader_H.Common**（公共库）
    - 公共工具类
    - 通用扩展方法

11. **M3u8Downloader_H.Extensions**（扩展功能）
    - 加密扩展
    - HTTP 扩展
    - 请求消息扩展

12. **M3u8Downloader_H.Test**（测试项目）
    - 单元测试
    - 集成测试

### 架构模式

- **MVVM (Model-View-ViewModel)**：UI 层采用 MVVM 模式
- **依赖注入**：使用 Caliburn.Micro 的 SimpleContainer 实现依赖注入
- **插件化架构**：核心功能支持插件扩展
- **分层设计**：清晰的模块划分和职责分离

## 构建和运行

### 环境要求

- **操作系统**：Windows 7 及以上（Win7 需要安装 KB4457144）
- **开发环境**：Visual Studio 2022
- **.NET 版本**：.NET 9.0 SDK
- **运行时**：.NET 9.0 Runtime（或使用自包含版本）

### 构建命令

使用 Visual Studio：
1. 打开 `M3u8Downloader_H.sln`
2. 选择配置（Debug 或 Release）
3. 选择平台（Any CPU、x64 或 x86）
4. 点击 "生成解决方案" 或按 F5 运行

使用命令行：
```powershell
# 还原依赖
dotnet restore

# 构建 Debug 版本
dotnet build --configuration Debug

# 构建 Release 版本
dotnet build --configuration Release

# 运行
dotnet run --project M3u8Downloader_H\M3u8Downloader_H.csproj
```

### 发布应用

```powershell
# 发布到指定目录
dotnet publish M3u8Downloader_H/ -o Publish --configuration Release

# 发布为单文件（自包含）
dotnet publish M3u8Downloader_H/ -o PublishSingleFile -c Release --self-contained true -r win-x64 -p:PublishSingleFile=true
```

### GitHub Actions

项目配置了 GitHub Actions 用于自动化构建和发布：
- 在推送到 `master` 或 `develop` 分支时自动构建
- 在创建标签时自动创建 GitHub Release
- 支持两种发布版本：
  - 需要运行库版本（附带 dotnet-9.0-runtime-x64.exe）
  - 无需运行库版本（单文件版本）

## 开发约定

### 代码规范

- 使用 **C# 10.0+** 语法特性
- 启用 **可空引用类型**（Nullable Reference Types）
- 遵循 **.NET 命名约定**：
  - 类名使用 PascalCase
  - 方法名使用 PascalCase
  - 私有字段使用 camelCase 或 _camelCase
  - 属性使用 PascalCase
- 使用 **异步编程**（async/await）处理 I/O 操作
- 适当使用 **异常处理**和日志记录

### MVVM 架构约定

- **Views**：XAML 文件，位于 `Views` 目录
- **ViewModels**：业务逻辑，位于 `ViewModels` 目录
- **Models**：数据模型，位于 `Models` 目录
- 使用 **Caliburn.Micro** 进行视图和视图模型的绑定
- ViewModel 继承自 `Screen` 或 `PropertyChangedBase`

### 插件开发约定

- 插件必须实现 `M3u8Downloader_H.Plugin.Abstractions` 中定义的接口
- 插件项目需要引用 `M3u8Downloader_H.Plugin.Abstractions`
- 插件可以通过 `IPluginManager` 与主程序交互
- 插件可以扩展：
  - M3U8 文件解析
  - 下载逻辑
  - 网站特定支持

### 测试约定

- 单元测试位于 `M3u8Downloader_H.Test` 项目
- 使用 xUnit 或 NUnit 测试框架
- 测试命名应清晰描述测试目的
- 保持测试的独立性和可重复性

### 提交规范

- 使用清晰的提交信息
- 参考 Changelog.md 中的版本更新格式
- 重大更新需要更新版本号和 Changelog.md

## 主要功能模块说明

### 下载流程

1. **解析 M3U8**：使用 `M3u8FileInfoClient` 解析 M3U8 文件
2. **创建下载任务**：通过 `DownloaderClient` 创建下载器实例
3. **执行下载**：根据视频类型选择合适的下载器：
   - 普通视频：`M3u8Downloader`
   - 加密视频：`CryptM3uDownloader`
   - 直播流：`LiveM3uDownloader`
   - 插件支持：`PluginM3u8Downloader`
   - 长视频：`MediaDownloader` 或 `LiveVideoDownloader`
4. **合并和转码**：使用 `M3uCombinerClient` 合并文件并转码为 MP4

### API 接口

REST API 服务器监听端口范围：65400-65432

**可用接口：**

1. **downloadmedias** - 下载长视频
   - 方法：POST
   - 参数：媒体 URL、保存路径、请求头等

2. **downloadbyurl** - 通过 URL 下载
   - 方法：POST
   - 参数：M3U8 URL、保存路径、请求头、插件密钥等

3. **downloadbycontent** - 通过内容下载
   - 方法：POST
   - 参数：M3U8 内容、视频名称、保存路径等

4. **downloadbyjsoncontent** - 通过 JSON 内容下载
   - 方法：POST
   - 参数：M3U8 文件信息（JSON 格式）、保存路径等

5. **getm3u8data** - 获取 M3U8 文件信息
   - 方法：POST
   - 参数：URL 和内容
   - 返回：解析后的 M3U8 文件信息

### 设置说明

设置在任务开始前生效，下载过程中修改设置仅影响后续任务：

- **线程数**：同时下载的线程数（默认：5）
- **任务数**：同时下载的任务数（默认：3）
- **重试次数**：下载失败时的重试次数（默认：1）
- **超时时间**：下载超时时间（默认：30秒）
- **代理设置**：配置 HTTP 代理
- **请求头**：自定义 HTTP 请求头
- **转码设置**：默认转码为 MP4 格式
- **缓存目录**：默认在程序同名文件夹下（不可更改）

## 文件结构说明

```
M3u8Downloader_H/
├── M3u8Downloader_H/                 # 主应用程序
│   ├── Views/                        # XAML 视图
│   ├── ViewModels/                   # 视图模型
│   ├── Models/                       # 数据模型
│   ├── Services/                     # 服务类
│   ├── Assets/                       # 资源文件
│   └── Sounds/                       # 音效文件
├── M3u8Downloader_H.Core/            # 核心功能
├── M3u8Downloader_H.Downloader/      # 下载器
│   ├── M3uDownloaders/               # M3U8 下载器
│   └── MediaDownloads/               # 媒体下载器
├── M3u8Downloader_H.Combiners/       # 合并器
│   ├── M3uCombiners/                 # M3U8 合并器
│   └── VideoConverter/               # 视频转换器
├── M3u8Downloader_H.M3U8/            # M3U8 解析
│   ├── M3UFileReaders/               # 文件读取器
│   ├── AttributeReaders/             # 属性读取器
│   └── M3UFileReaderManangers/       # 读取管理器
├── M3u8Downloader_H.Plugin/          # 插件系统
│   ├── PluginManagers/               # 插件管理器
│   └── AttributeReaderManagers/      # 属性读取管理器
├── M3u8Downloader_H.Plugin.Abstractions/  # 插件抽象层
│   ├── Plugins/                      # 插件接口
│   ├── Downloader/                   # 下载器抽象
│   ├── M3u8/                         # M3U8 抽象
│   └── Settings/                     # 设置抽象
├── M3u8Downloader_H.RestServer/      # REST API 服务器
│   ├── Models/                       # API 模型
│   └── Utils/                        # 工具类
├── M3u8Downloader_H.Settings/        # 设置管理
├── M3u8Downloader_H.Common/          # 公共库
├── M3u8Downloader_H.Extensions/      # 扩展功能
└── M3u8Downloader_H.Test/            # 测试项目
```

## 常见问题

### 环境问题

**Q: Win7 系统双击程序没反应怎么办？**
A: 必须安装 KB4457144 补丁，详见：https://github.com/Harlan-H/M3u8Downloader_H/wiki#%E7%8E%AF%E5%A2%83%E9%97%AE%E9%A2%98

**Q: 程序启动失败提示缺少 .NET 运行时？**
A: 请安装 .NET 9.0 Runtime，或使用自包含版本（SingleFile 版本）

### 下载问题

**Q: 下载失败或速度很慢？**
A: 尝试以下方法：
- 检查网络连接
- 增加超时时间设置
- 配置代理服务器
- 检查请求头设置

**Q: 直播流下载不完整？**
A: 直播流会持续下载直到手动停止，建议：
- 设置合适的超时时间
- 使用停止按钮结束下载
- 检查直播源是否稳定

### 插件问题

**Q: 如何开发插件？**
A: 参考插件项目：https://github.com/Harlan-H/M3u8Downloader_H.Plugins

**Q: 插件加载失败？**
A: 检查：
- 插件是否实现了正确的接口
- 插件版本是否兼容
- 插件文件是否放置在正确位置

## 相关资源

- **GitHub 仓库**：https://github.com/Harlan-H/M3u8Downloader_H
- **插件仓库**：https://github.com/Harlan-H/M3u8Downloader_H.Plugins
- **在线帮助文档**：http://note.youdao.com/noteshare?id=c6ba2fb478ad300b7095c7c951556fc6
- **Wiki 文档**：https://github.com/Harlan-H/M3u8Downloader_H/wiki/
- **更新日志**：参见 Changelog.md
- **许可证**：参见 LICENSE.txt

## 贡献指南

欢迎贡献代码！请遵循以下步骤：

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

### 代码审查

- 确保代码符合项目规范
- 添加必要的注释和文档
- 确保测试通过
- 更新相关文档（如需要）

## 许可证

本项目采用开源许可证，详见 LICENSE.txt 文件。

## 支持作者

如果您觉得这个项目对您有帮助，欢迎通过微信或支付宝支持作者。

## 版本历史

详细的版本更新记录请参见 [Changelog.md](Changelog.md)。

### 最近更新

**4.0.1 (2025/05/23)**
- 修复上次更新后导致合并出现报错的问题

**4.0.0 (2025/04/15)**
- 修改界面布局
- 底层代码重构
- 增加 m3u8 合并界面，文件夹，长视频合并
- 支持长视频下载（mp4, flv 等）
- 优化多项功能和性能

---

**注意**：本文档会随着项目的发展持续更新。如有疑问或建议，请通过 GitHub Issues 反馈。