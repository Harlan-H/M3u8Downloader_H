# M3u8Downloader_H
软件分两个版本： 
  - [m3u8Downloader_H.zip](https://github.com/Harlan-H/M3u8Downloader_H/releases/latest/download/M3u8Downloader_H.zip)  需要.net 6的运行库的版本
  - [M3u8Downloader_H-SingleFile.zip](https://github.com/Harlan-H/M3u8Downloader_H/releases/latest/download/M3u8Downloader_H-SingleFile.zip)  不需要.net 6运行库  
  **如果你是win7用户 不管你用哪个版本都必须安装KB4457144 ,具体看下面的环境问题，点击上方版本直接下载**

# 下一步计划
 - 目前支持已经支持直接通过网址下载m3u8视频的有b站直播，抖音直播，vlive视频，555dd7影视站
 - 那如果你遇到没有适配得网站,同时又需要适配那么可以给我留言,如果你也是开发者你也想加入那可以提交pr
 - 后续得开发还将继续对一些网站进行适配
  
# 特点
 - 支持多线程，多任务,断点续传
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
 - 个性化的m3u8下载，可以采用xml,json等方式下载m3u8的文件内容
 - 提供http接口调用，可以使用任何语言对软件发起调用下载，具体参见帮助文档
 - 提供插件功能，可以个性化定制自己的下载需求，具体参见帮助文档->插件开发
 - 特定网站可以直接通过网页地址来下载m3u8视频

# 帮助文档
 - 在线地址：http://note.youdao.com/noteshare?id=c6ba2fb478ad300b7095c7c951556fc6
  - wiki  : https://github.com/Harlan-H/M3u8Downloader_H/wiki/

# 环境问题
 1. 程序是64位 只支持64位系统
 2. 如果提示安装.net6 可以自行下载安装，地址：https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-desktop-6.0.9-windows-x64-installer
 3. 对于win7 64用户.net6安装完成  如果出现点击程序没有任何反应(没有反应指没有任何报错也不出任何界面) 那么就需要进行第二个步骤
    - 下载一个更新KB4457144   地址：http://download.windowsupdate.com/d/msdownload/update/software/secu/2018/09/windows6.1-kb4457144-x64_5ca467d42deadc2b2f4010c4a26b4a6903790dd5.msu
    - 如果上面两个步骤都做了还是不行 这边给你提供一个参考资料  [点击跳转](https://www.cnblogs.com/simadi/p/14410536.html)


# 截图
![QQ截图20221120192851](https://user-images.githubusercontent.com/39378318/202899529-a6d8ae44-f578-4312-b6b7-77d354c874b4.png)
![setting1](https://user-images.githubusercontent.com/39378318/202899568-874bbf90-1198-42eb-b42a-d8e3df27ba5c.png)
![QQ截图20221120192700](https://user-images.githubusercontent.com/39378318/202899573-ee513782-f792-464b-b7ee-eb8b48f02ce6.png)


# 支持作者
|微信|支付宝|
|:--:|:--:|
|![weixin](https://user-images.githubusercontent.com/39378318/190890312-ab314b1e-24e8-4237-aa24-2f49752b49ab.png)|![zhi](https://user-images.githubusercontent.com/39378318/190890316-d16156a1-88bb-487a-a7a4-664cf0a5e4da.png)|
