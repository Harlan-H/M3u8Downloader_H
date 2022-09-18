# M3u8Downloader_H
**M3u8Downloader_H** 是一个操作非常简单，同时功能比较强大的应用程序，你只需要复制需要下载的m3u8地址或者文件既可完成自动下载，合并，转码等操作
首次运行软件如果没有环境会有提示框点击install即可自动下载，无需自己去官网下载环境，让使用本程序可以更简单

# 特点
 - 简单的ui操作
 - 自动检测下载.net环境及依赖
 - 支持多线程，多任务
 - 支持断点续传
 - 支持aes-128-cbc,aes-192-cbc,aes-256-cbc自动解密
 - 支持对m3u8的ts,fmp4格式下载
 - 支持拖拽文件夹实现快速合并
 - 支持批量下载功能
 - 支持代理，在设置中配置
 - m3u8的地址不受时效影响，具体参见帮助文档
 - 当使用m3u8文件下载时，密钥可以是磁盘地址，也可以是网络地址
 - 自动根据m3u8文件中的链接地址来识别是下载还是合并操作
 - 自动转换png,jpg,bmp等伪装格式的ts流
 - 自动识别直播流，同时下载直播流
 - 可以自定义请求头
 - 提供插件功能，可以个性化定制自己的下载需求
 - 个性化的m3u8下载，可以采用xml,json等方式下载m3u8的文件内容
 - 提供http接口调用，可以使用任何语言对软件发起调用下载，具体参见帮助文档

# 帮助文档
 - 在线地址：http://note.youdao.com/noteshare?id=c6ba2fb478ad300b7095c7c951556fc6

# 目前的问题
 当软件下载的是fmp4格式的视频之后，没有办法转码为mp4。目前的解决方案其实是有的，可以通过命令行的方式让ffmpeg去转换成mp4,但是与此同时带来的问题是，软件本身是同时支持下载和合并的，那如果用户合并的ts流他是加密的，这个时候的视频解密就需要保存到硬盘上，但是如果是ts的格式，这个时候是内存中就解密完成同时写入到主视频上，所以这就带来了一个新的性能问题。如果你有比较好的方案可以给我提议.   
 所以如果你下载的视频是fmp4的格式的，请不要转码成mp4

# 截图
![list](https://user-images.githubusercontent.com/39378318/190357782-117bb79d-a7f3-43bb-9e16-aa7af88e5da8.png)
![setting1](https://user-images.githubusercontent.com/39378318/190357793-04773f9e-e02f-4fd3-ba01-f0af040cef75.png)
![setting2](https://user-images.githubusercontent.com/39378318/190357801-0035792a-76f3-49ca-a90b-08802fd63b43.png)

# 支持作者
|微信|支付宝|
|:--:|:--:|
|![weixin](https://user-images.githubusercontent.com/39378318/190890312-ab314b1e-24e8-4237-aa24-2f49752b49ab.png)|![zhi](https://user-images.githubusercontent.com/39378318/190890316-d16156a1-88bb-487a-a7a4-664cf0a5e4da.png)|
