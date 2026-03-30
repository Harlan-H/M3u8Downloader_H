using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Core;
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
        viewModel.downloaderCoreClient = new(Http.Client, m3U8DownloadParam, settingsService.Clone<SettingsService>(), viewModel.Log, pluginType);
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

        viewModel.downloaderCoreClient = new(Http.Client, m3U8DownloadParam, settingsService.Clone<SettingsService>(), viewModel.Log, pluginType, m3UFileInfo);
        return viewModel;
    }

    public  DownloadViewModel CreateDownloadViewModel(
         IMediaDownloadParam m3U8DownloadParam)
    {
        DownloadViewModel viewModel = new(m3U8DownloadParam)
        {
            RequestUrl = m3U8DownloadParam.Medias[0].Url,
            VideoName = m3U8DownloadParam.VideoName
        };
        viewModel.downloaderCoreClient = new DownloaderCoreClient(Http.Client, m3U8DownloadParam, settingsService.Clone<SettingsService>(), viewModel.Log);
        return viewModel;
    }
}
