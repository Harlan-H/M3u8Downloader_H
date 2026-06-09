using M3u8Downloader_H.Downloader.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Downloader.MediaDownloads;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Common.Extensions;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Progress.Interfaces;


namespace M3u8Downloader_H.Downloader
{
    public class DownloaderClient(IDownloadContext context, IDownloadPlugin? downloadPlugin)
    {
        private DownloaderBase? _mediaDownloader;

        public IProgressReporter DialogProgress { get; set; } = default!;
        public IM3uFileInfo M3UFileInfo { get; set; } = default!;
        public Func<CancellationToken, Task<IM3uFileInfo>> GetLiveFileInfoFunc { get; set; } = default!;

        public IDownloadService CreateM3u8Downloader(IM3uFileInfoSource m3UFileInfoSource)
        {
            IDownloadService m3U8Downloader = new M3u8Downloader(context, DialogProgress);
            if(m3UFileInfoSource.Type == M3uType.SUBTITLE)
            {
                m3U8Downloader = new SubtitleDownloader(m3U8Downloader, context);
                return m3U8Downloader;
            }

            if (downloadPlugin is not null)
            {
                var pluginDownloader = downloadPlugin.CreateDownloadService(m3U8Downloader, context);
                if (pluginDownloader is not null)
                    return pluginDownloader;
            }

            if (!m3UFileInfoSource.M3uFile!.IsVod())
            {
                LiveM3uDownloader liveM3UDownloader = new(m3U8Downloader, context, DialogProgress)
                {
                    GetLiveFileInfoFunc = GetLiveFileInfoFunc
                };
                m3U8Downloader = liveM3UDownloader;
            }
            else if (m3UFileInfoSource.M3uFile!.Key is not null)
                m3U8Downloader = new CryptM3uDownloader(m3U8Downloader, context);

            return m3U8Downloader;
        }

        public DownloaderBase MediaDownloader
        {
            get
            {
                IMediaDownloadParam mediaDownloadParam = (IMediaDownloadParam)context.DownloadParam;
                if(_mediaDownloader is null)
                {
                    if (mediaDownloadParam.IsVideoStream)
                        _mediaDownloader = new MediaDownloader(context);
                    else
                        _mediaDownloader = new LiveVideoDownloader(context);
                }

                _mediaDownloader.DialogProgress = DialogProgress;
                return _mediaDownloader;
            }
        }
    }
}
