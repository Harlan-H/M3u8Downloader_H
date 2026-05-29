using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Common.Services;
using M3u8Downloader_H.Core;
using M3u8Downloader_H.Core.Interfaces;
using M3u8Downloader_H.Plugin.Models.Context;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.ViewModels.Downloads;
using System.Net.Http;


namespace M3u8Downloader_H.FrameWork;

public class ViewModelManager(SettingsService settingsService)
{
    public  DownloadViewModel CreateDownloadViewModel(
           HttpClient? httpClient,
           IM3u8DownloadParam m3U8DownloadParam,
           IDownloadPlugin? downloadPlugin)
    {
        DownloadViewModel viewModel = new(m3U8DownloadParam)
        {
            RequestUrl = m3U8DownloadParam.RequestUrl,
            VideoName = m3U8DownloadParam.VideoName
        };
        SettingsService settingsService1 = settingsService.Clone<SettingsService>();
        IHttpClientWrapper httpClientWrapper = new HttpClientWrapper(httpClient ?? Http.Instance.GetClient(), settingsService1.Timeouts);
        DownloadContext downloadContext = new(httpClientWrapper, viewModel.Log,m3U8DownloadParam, settingsService1);
        viewModel.downloaderCoreClient = new(viewModel.GetM3uFileInfoState,downloadContext, downloadPlugin);
        return viewModel;
    }

    public  DownloadViewModel CreateDownloadViewModel(
        HttpClient? httpClient,
        IM3uFileInfo m3UFileInfo,
        IDownloadParamBase m3U8DownloadParam,
        IDownloadPlugin? downloadPlugin)
    {
        DownloadViewModel viewModel = new(m3U8DownloadParam)
        {
            VideoName = m3U8DownloadParam.VideoName
        };
        SettingsService settingsService1 = settingsService.Clone<SettingsService>();
        IHttpClientWrapper httpClientWrapper = new HttpClientWrapper(httpClient ?? Http.Instance.GetClient(), settingsService1.Timeouts);
        DownloadContext downloadContext = new(httpClientWrapper, viewModel.Log, m3U8DownloadParam, settingsService1);
        viewModel.downloaderCoreClient = new(downloadContext,  m3UFileInfo, downloadPlugin);
        return viewModel;
    }

    public  DownloadViewModel CreateDownloadViewModel(
         HttpClient? httpClient,
         IMediaDownloadParam mediaDownloadParam)
    {
        DownloadViewModel viewModel = new(mediaDownloadParam)
        {
            RequestUrl = mediaDownloadParam.Medias[0].Url,
            VideoName = mediaDownloadParam.VideoName
        };
        SettingsService settingsService1 = settingsService.Clone<SettingsService>();
        IHttpClientWrapper httpClientWrapper = new HttpClientWrapper(httpClient ?? Http.Instance.GetClient(), settingsService1.Timeouts);
        DownloadContext downloadContext = new(httpClientWrapper, viewModel.Log, mediaDownloadParam, settingsService1);
        viewModel.downloaderCoreClient = new DownloaderCoreClient(downloadContext);
        return viewModel;
    }
}
