using M3u8Downloader_H.ViewModels;
using System;


namespace M3u8Downloader_H.FrameWork;

public abstract partial class DialogViewModelBase<T> : ViewModelBase
{
    public T? DialogResult { get; private set; }

    public event EventHandler? Closed;

    public void Close(T? dialogResult = default)
    {
        DialogResult = dialogResult;
        Closed?.Invoke(this, EventArgs.Empty);
    }

}

public abstract class DialogViewModelBase : DialogViewModelBase<bool?>
{

}
