using M3u8Downloader_H.Abstractions.M3u8;

namespace M3u8Downloader_H.Abstractions.Plugins.Download
{
    public interface IM3uFileReader
    {
        /// <summary>
        /// 此函数会传入一个已经实例化后的AttributeReaderCollection
        /// 你可以拦截这个函数修改你自定义的处理逻辑
        /// 最后要调用接口的InitAttributeReade 不然会报错
        /// </summary>
        /// <param name="readers"></param>
        void InitAttributeReade(IAttributeReaderCollection readers);

        /// <summary>
        /// 获取m3u8数据接口
        /// 如果你不需要程序自己的处理方案或者是魔改m3u8
        /// 可能有些m3u8不是一个文件 是一个自己拼装的m3u8信息
        /// 那上面的InitAttributeReade 也可以不用调用接口的
        /// </summary>
        /// <param name="stream">网站或者文件流</param>
        /// <returns></returns>
        IM3uFileInfo GetM3u8FileInfo(Stream stream);
    }
}
