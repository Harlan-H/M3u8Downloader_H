﻿using Caliburn.Micro;
using System;


namespace M3u8Downloader_H.ViewModels.FrameWork
{
    public abstract class DialogScreen<T> : PropertyChangedBase
    {

        public T? DialogResult { get; private set; }

        public event EventHandler? Closed;

        public void Close(T? dialogResult = default)
        {
            DialogResult = dialogResult;
            Closed?.Invoke(this, EventArgs.Empty);
        }

    }

    public abstract class DialogScreen : DialogScreen<bool?>
    {

    }
}
