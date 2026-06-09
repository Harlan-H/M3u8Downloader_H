using M3u8Downloader_H.Progress.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace M3u8Downloader_H.Progress.Services
{
    public partial class ProgressManager(Action<DownloadStatus> ChangeStatus) : IProgressManager
    {
        public DownloadStatus Status
        {
            get => field;
            set
            {
                if (field != value)
                {
                    ChangeStatus(value);
                    field = value;
                }
            }
        }

        public long DownloadedBytes
        {
            get => field;
            set
            {
                if (field == value)
                    return;

                field = value;
                OnPropertyChanged();
            }
        }

        public double Progress
        {
            get => field;
            set
            {
                if (field == value)
                    return;

                field = value;
                OnPropertyChanged();
            }
        }

        public void Clear()
        {
            DownloadedBytes = -1;
        }

        public IProgressHandler CreateHlsHandler() => new HlsProgressHandler(this);

        public IProgressHandler CreateLiveHandler() => new LiveProgressHandler(this);

        public IProgressHandler CreateVodHandler() => new VodProgressHandler(this);

        public IProgressHandler CreateMergerHandler() => new MergerProgressHandler(this);


    }

    public partial class ProgressManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
