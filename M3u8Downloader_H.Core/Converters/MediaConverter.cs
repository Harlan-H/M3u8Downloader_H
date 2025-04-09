using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Converter;
using M3u8Downloader_H.Abstractions.Settings;
using M3u8Downloader_H.Combiners;

namespace M3u8Downloader_H.Core.Converters
{
    public partial class MediaConverter : IConverter
    {
        private M3uCombinerClient m3UCombinerClient = default!;
        private readonly IMediaDownloadParam mediaDownloadParam;
        private readonly ILog log;

        public MediaConverter(IMediaDownloadParam mediaDownloadParam, ILog log)
        {
            this.mediaDownloadParam = mediaDownloadParam;
            this.log = log;
        }

        public async ValueTask StartMerge(IDialogProgress progress, CancellationToken cancellationToken)
        {
            if(mediaDownloadParam.Medias.Count < 1)
            {
                log.Warn("视频流不能小于1");
                return;
            }

            log.Info("开始转码");
            await M3u8Merge(progress, cancellationToken);
            log.Info("转码完成");
        }

        private async ValueTask M3u8Merge(IDialogProgress progress, CancellationToken cancellationToken)
        {
            m3UCombinerClient.FFmpeg.CachePath = Path.GetDirectoryName(mediaDownloadParam.Medias[0].Url.OriginalString) ?? throw new ArgumentException("获取缓存路径失败");
            await m3UCombinerClient.FFmpeg.ConvertToMp4(mediaDownloadParam.Medias, progress, cancellationToken);   
        }
    }

    public partial class MediaConverter
    {
        public static MediaConverter CreateMediaConverter(
            IMediaDownloadParam downloadParam,
            IMergeSetting mergeSetting,
            ILog log)
        {
            MediaConverter mediaConverter = new(downloadParam, log)
            {
                m3UCombinerClient = new(log, downloadParam, mergeSetting)
            };
            return mediaConverter; 
        }
    }
}
