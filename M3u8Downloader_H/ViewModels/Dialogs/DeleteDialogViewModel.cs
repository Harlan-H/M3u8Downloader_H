using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using M3u8Downloader_H.FrameWork;
using M3u8Downloader_H.Models;

namespace M3u8Downloader_H.ViewModels.Dialogs
{
    public partial class DeleteDialogViewModel : DialogViewModelBase<DeleteDialogResult>
    {

        [ObservableProperty]
        public partial int Count { get; set; } = 0;

        [ObservableProperty]
        public partial string ConfirmBtnText { get; set; } = default!;

        [ObservableProperty]
        public partial string CancelBtnText { get; set; } = default!;

        [ObservableProperty]

        public partial bool IsDeleteCached { get; set; }

        [RelayCommand]
        private void Confirm()
        {
            DeleteDialogResult deleteDialogResult = new(true, IsDeleteCached);
            Close(deleteDialogResult);
        }

        [RelayCommand]
        private void Cancel()
        {
            DeleteDialogResult deleteDialogResult = new(false, IsDeleteCached);
            Close(deleteDialogResult);
        }
    }

    public partial class DeleteDialogViewModel
    {
        public static DeleteDialogViewModel CreateDeleteDialogViewModel(int count, string title = "注意",  string confirmText = "确定", string CancelText = "取消")
        {
            var view = new DeleteDialogViewModel()
            {
                Title = title,
                Count = count,
                CancelBtnText = CancelText,
                ConfirmBtnText = confirmText
            };
            return view;
        }
    }
}
