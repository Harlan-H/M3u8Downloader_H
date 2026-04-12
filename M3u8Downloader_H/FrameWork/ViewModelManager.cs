using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Core;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.ViewModels.Downloads;
using System;


namespace M3u8Downloader_H.FrameWork;

public class ViewModelManager(SettingsService settingsService)
{
    public  DownloadViewModel CreateDownloadViewModel(
           IM3u8DownloadParam m3U8DownloadParam,
           Type? pluginType)
    {
        DownloadViewModel viewModel = new(m3U8DownloadParam)
        {
            RequestUrl = m3U8DownloadParam.RequestUrl,
            VideoName = m3U8DownloadParam.VideoName
        };
        DownloadContext downloadContext = new(Http.Client, viewModel.Log,m3U8DownloadParam, settingsService.Clone<SettingsService>());
        viewModel.downloaderCoreClient = new(downloadContext, pluginType);
        return viewModel;
    }

    public  DownloadViewModel CreateDownloadViewModel(
        IM3uFileInfo m3UFileInfo,
        IDownloadParamBase m3U8DownloadParam,
        Type? pluginType)
    {
        DownloadViewModel viewModel = new(m3U8DownloadParam)
        {
            VideoName = m3U8DownloadParam.VideoName
        };

        DownloadContext downloadContext = new(Http.Client, viewModel.Log, m3U8DownloadParam, settingsService.Clone<SettingsService>());
        viewModel.downloaderCoreClient = new(downloadContext, pluginType, m3UFileInfo);
        return viewModel;
    }

    public  DownloadViewModel CreateDownloadViewModel(
         IMediaDownloadParam mediaDownloadParam)
    {
        DownloadViewModel viewModel = new(mediaDownloadParam)
        {
            RequestUrl = mediaDownloadParam.Medias[0].Url,
            VideoName = mediaDownloadParam.VideoName
        };
        DownloadContext downloadContext = new(Http.Client, viewModel.Log, mediaDownloadParam, settingsService.Clone<SettingsService>());
        viewModel.downloaderCoreClient = new DownloaderCoreClient(downloadContext);
        return viewModel;
    }
}
