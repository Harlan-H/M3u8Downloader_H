using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Converter;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Settings;
using M3u8Downloader_H.Combiners;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Core.Downloads;
using M3u8Downloader_H.Downloader;
using M3u8Downloader_H.M3U8;

namespace M3u8Downloader_H.Core.Converters
{
    public partial class M3u8Converter : IConverter
    {
        private readonly IM3uFileInfo m3UFileInfo;
        private readonly ILog log;
        private readonly IM3u8DownloadParam downloadParamBase;
        private readonly IMergeSetting mergeSetting;
        private DownloaderClient m3UDownloaderClient = default!;
        private M3uCombinerClient m3UCombinerClient = default!;

        public M3u8Converter(IM3uFileInfo m3UFileInfo, ILog log, IM3u8DownloadParam downloadParamBase,IMergeSetting mergeSetting)
        {
            this.m3UFileInfo = m3UFileInfo;
            this.log = log;
            this.downloadParamBase = downloadParamBase;
            this.mergeSetting = mergeSetting;
        }

        public async ValueTask StartMerge(IDialogProgress progress, CancellationToken cancellationToken)
        {
            if (!m3UFileInfo.MediaFiles.Any(m => m.Uri.IsFile))
            {
                log.Warn("ts文件不是本地路径");
                return;
            }

            if (downloadParamBase.M3UKeyInfo is not null)
            {
                log.Info("开始解密数据");
                await M3u8Decrypt(downloadParamBase.M3UKeyInfo,progress, cancellationToken);
                log.Info("数据解密完成");
            }

            log.Info("开始视频合并转码");
            await M3u8Merge(progress, cancellationToken);
            log.Info("转码完成,文件路径是:{0}", Path.Combine(downloadParamBase.SavePath, downloadParamBase.VideoFullName));
        }

        private async ValueTask M3u8Decrypt(IM3uKeyInfo m3UKeyInfo, IDialogProgress progress, CancellationToken cancellationToken)
        {
            m3UDownloaderClient.DialogProgress = progress;

            m3UDownloaderClient.M3uDecrypter.Initialization(m3UKeyInfo);
            await m3UDownloaderClient.M3uDecrypter.DownloadAsync(m3UFileInfo, cancellationToken);
            
        }

        private async ValueTask M3u8Merge(IDialogProgress progress, CancellationToken cancellationToken)
        {
            m3UCombinerClient.DialogProgress = progress;

            if (m3UFileInfo.Map is not null)
            {
                m3UCombinerClient.M3u8FileMerger.Initialize(m3UFileInfo);
                await m3UCombinerClient.M3u8FileMerger.StartMerging(m3UFileInfo, cancellationToken);
            }
            else
            {
                await m3UCombinerClient.FFmpeg.ConvertToMp4(m3UFileInfo, cancellationToken);
            }
        }
    }

    public partial class M3u8Converter
    {
        public static M3u8Converter CreateM3u8Converter(
            IM3uFileInfo m3UFileInfo, 
            IM3u8DownloadParam downloadParamBase,
            IMergeSetting mergeSetting,
            ILog log)
        {
            M3u8Converter m3U8Converter = new M3u8Converter(m3UFileInfo, log, downloadParamBase, mergeSetting)
            {
                m3UDownloaderClient = new(log, downloadParamBase),
                m3UCombinerClient = new(log, downloadParamBase, mergeSetting)
            };

            return m3U8Converter;
        }
    }
}
