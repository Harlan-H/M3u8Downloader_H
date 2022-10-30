using M3u8Downloader_H.M3U8.AttributeReaderManager;

namespace M3u8Downloader_H.Plugin
{
    public interface IPluginBuilder
    {
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


        /// <summary>
        /// 创建获取m3u8文件信息的类
        /// 如果你不需要处理，只要return null 即可
        /// 创建IM3u8FileInfoService实例参考CreatePluginService函数的注释
        /// </summary>
        /// <returns>返回实现了IGetM3u8InfoService接口的实例化</returns>
        IM3u8FileInfoService? CreateM3u8FileInfoService();
    }
}
