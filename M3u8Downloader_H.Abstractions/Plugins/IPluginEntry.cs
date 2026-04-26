using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Abstractions.Plugins.Window;


namespace M3u8Downloader_H.Abstractions.Plugins
{
    public interface IPluginEntry
    {
        IWindowPlugin? CreateWindoPlugin();

        /// <summary>
        /// 按照url判断是否要处理这个请求
        /// 返回true则调用CreateDownloadPlugin
        /// 否则不会调用
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        bool CanHandle(Uri url);

        IDownloadPlugin? CreateDownloadPlugin();
    }
}
