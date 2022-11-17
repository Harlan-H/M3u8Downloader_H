using M3u8Downloader_H.Models;
using M3u8Downloader_H.ViewModels.FrameWork;

namespace M3u8Downloader_H.ViewModels
{
    public partial class DeleteDialogViewModel : DialogScreen<DeleteDialogResult>
    {
        public string? Title { get; set; }
        public int Count { get; set; } = 0;

        public string ConfirmBtnText { get; set; } = default!;
        public string CancelBtnText { get; set; } = default!;

        public bool IsDeleteCached { get; set; }

        public void Confirm()
        {
            DeleteDialogResult deleteDialogResult = new(true, IsDeleteCached);
            Close(deleteDialogResult);
        }

        public void Cancel()
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
