namespace M3u8Downloader_H.Plugin
{
    public interface IPluginBuilder
    {
        // 修改后的插件将在构造函数中添加两个参数
        // HttpClient , Ilog 
        // 两个参数的位置无所谓 需要用哪个就在构造函数中添加上
        // 构造函数必须时继承自IPluginBuilder接口的构造函数,
        // 如果这两个参数都不需要可以不写构造函数
        /*  例子1：
        public class Class1 : IPluginBuilder
        {
                 public Class1(HttpClient httpClient)
                 {
                    this.httpClient = httpClient;
                 }
       }
        例子2：
        public class Class1 : IPluginBuilder
        {
                public Class1(HttpClient httpClient,ILog log)
                {
                    this.httpClient = httpClient;
                    this.log = log;
                }
        }
        例子3:
        public class Class1 : IPluginBuilder
        {
                public Class1(ILog log)
                {
                    this.log = log;
                }
        }
        */
        /// <summary>
        /// 创建获取m3u8 uri的类
        /// 如果你不需要处理，只要return null 即可
        /// </summary>
        /// <returns>返回实现了IGetM3u8InfoService接口的实例化</returns>
        IM3u8UriProvider? CreateM3u8UriProvider();

        /// <summary>
        /// 获取m3u8文件流得类
        /// 如果你需要对某项请求或者响应加密或者解密,可以实现此类
        /// </summary>
        /// <returns></returns>
        IM3u8FileInfoStreamService? CreateM3U8FileInfoStreamService();


        /// <summary>
        /// 解析m3u8数据得接口,在大部分情况之下此接口是不用实现得
        /// 在某些情况下你不需要默认得解析方案，那么请实现此接口
        /// 如果你不需要处理，直接return null
        /// </summary>
        /// <returns>返回实现了IM3uFileReader接口得实例</returns>
        IM3uFileReader? CreateM3u8FileReader();

        /// <summary>
        /// 对原始的m3u8中的一些标签进行修改，或者增加自己需要对某些标签的特别处理
        /// </summary>
        /// <param name="attributeReader">实现IAttributeReaderManager的实例</param>
        void SetAttributeReader(IAttributeReaderManager attributeReader);

        /// <summary>
        /// 如果需要对某些数据进行特殊的处理，例如需要某些流数据使用的特殊加密，那需要实现返回IDownloadService的实例
        /// 例如  
        ///       class MyDownload : IDownloadService {}
        ///       
        ///       IDownloadService? CreatePluginService()
        ///       {
        ///             return new MyDownload();
        ///       }
        ///       
        /// 如果你不需要处理得到的流，只要return null 即可
        /// </summary>
        /// <returns>返回实现了IDownloadService接口的实例化</returns>
        IDownloadService? CreatePluginService();

    }
}
