using M3u8Downloader_H.Abstractions.M3u8;

namespace M3u8Downloader_H.Abstractions.Plugins.Download
{
    public interface IM3uFileReader
    {
        /// <summary>
        /// 此函数会传入一个已经实例化后的AttributeReaderCollection
        /// 你可以拦截这个函数修改你自定义的处理逻辑
        /// </summary>
        /// <param name="readers"></param>
        void InitAttributeReade(IAttributeReaderCollection readers);

        /// <summary>
        /// 获取m3u8数据接口
        /// </summary>
        /// <param name="stream">网站或者文件流</param>
        /// <returns></returns>
        IM3uFileInfo GetM3u8FileInfo(Stream stream);
    }
}
