using M3u8Downloader_H.M3U8.AttributeReaderManager;
using M3u8Downloader_H.M3U8.AttributeReaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <returns></returns>
        IDownloadService? CreatePluginService();
    }
}
