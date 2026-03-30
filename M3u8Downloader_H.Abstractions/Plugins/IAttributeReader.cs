using M3u8Downloader_H.Abstractions.M3u8;

namespace M3u8Downloader_H.Abstractions.Plugins
{
    public interface IAttributeReader
    {
        /// <summary>
        /// 是否提前退出，一般情况下都设置为false
        /// </summary>
        bool ShouldTerminate { get; }

        /// <summary>
        /// 匹配到的标签内容的主处理函数
        /// </summary>
        /// <param name="m3UFileInfo">m3UFileInfo数据结构</param>
        /// <param name="value">匹配到标签的冒号之后的内容</param>
        /// <param name="reader">控制当前取到的内容，或者取下一行内容的参数</param>
        /// <param name="baseUri">请求的原始的主url,就是那个.m3u8的地址</param>
        void Write(IM3uFileInfo m3UFileInfo, string value, IEnumerator<string> reader, Uri baseUri);
    }
}