using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.M3U8.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace M3u8Downloader_H.ViewModels.Components
{
    public partial class MultiMediaSetupViewModel(IList<IM3uStreamInfo> m3UStreamInfos, IList<IM3uMediaManifest>? audios, IList<IM3uMediaManifest>? subtitles) : ViewModelBase
    {
        private readonly TaskCompletionSource<IList<IM3uFileInfoSource>> taskCompletionSource = new();

        [ObservableProperty]
        public partial IList<IM3uStreamInfo> M3uStreams { get; private set; }

        [ObservableProperty]
        public partial IM3uStreamInfo SelectedStream { get; set; }

        [ObservableProperty]
        public partial IList<IM3uMediaManifest>? M3uAudios { get; private set; }

        [ObservableProperty]
        public partial IM3uMediaManifest? SelectedAudio { get; set; }

        [ObservableProperty]
        public partial IList<IM3uMediaManifest>? M3uSubtitles { get; private set; }

        [ObservableProperty]
        public partial IM3uMediaManifest? SelectedSubtitle { get; set; }


        private void RefreshMedisManifest()
        {
            if (SelectedStream.Audio is not null && audios is not null)
            {
                M3uAudios = [.. audios.Where(a => a.GroupId == SelectedStream.Audio)];
                SelectedAudio = M3uAudios.First();
            }

            if (SelectedStream.Subtitles is not null && subtitles is not null)
            {
                M3uSubtitles = [.. subtitles.Where(s => s.GroupId == SelectedStream.Subtitles)];
                SelectedSubtitle = M3uSubtitles.First();
            }
        }

        public async Task<IList<IM3uFileInfoSource>> GetM3uFileInfoState()
        {
            M3uStreams = [.. m3UStreamInfos
                .OrderByDescending(m => m.Codecs)
                .ThenByDescending(m => m.Bandwidth)];

            SelectedStream = M3uStreams.First();

            RefreshMedisManifest();

            return await taskCompletionSource.Task;
        }

        partial void OnSelectedStreamChanged(IM3uStreamInfo value)
        {
            SelectedStream = value;
            RefreshMedisManifest();
        }

        [RelayCommand]
        private void Submit()
        {
            List<IM3uFileInfoSource> m3UFileInfoStates = [new M3uFileInfoSource(SelectedStream.Uri)];
            if (SelectedAudio is not null)
                m3UFileInfoStates.Add(new M3uFileInfoSource(SelectedAudio.Uri, M3uType.AUDIO));

            if (SelectedSubtitle is not null)
                m3UFileInfoStates.Add(new M3uFileInfoSource(SelectedSubtitle.Uri, M3uType.SUBTITLE));

            taskCompletionSource.SetResult(m3UFileInfoStates);
        }
    }
}
