namespace M3u8Downloader_H.Plugin
{
    public interface IPluginBuilder
    {
        /// <summary>
        /// 创建获取m3u8文件信息的类
        /// 如果你不需要处理，只要return null 即可
        /// 创建IM3u8FileInfoService实例参考CreatePluginService函数的注释
        /// </summary>
        /// <returns>返回实现了IGetM3u8InfoService接口的实例化</returns>
        IM3u8UriProvider? CreateM3u8UriProvider();

        /// <summary>
        /// 解析m3u8数据得接口,在大部分情况之下此接口是不用实现得,但是在某些情况下你不需要默认得实现方案，那么请实现此接口
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
