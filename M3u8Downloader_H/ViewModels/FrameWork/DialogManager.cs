using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace M3u8Downloader_H.ViewModels.FrameWork
{
    public class DialogManager
    {
        public DialogManager()
        {
            
        }

        public static async Task<T?> ShowDialogAsync<T>(DialogScreen<T> dialogScreen)
        {
            UIElement view = ViewLocator.LocateForModel(dialogScreen, null, null);
            ViewModelBinder.Bind(dialogScreen, view, null);

            void OnDialogOpened(object? openSender, DialogOpenedEventArgs openArgs)
            {
                void OnScreenClosed(object? openSender,EventArgs closeArgs)
                {
                    openArgs.Session.Close();
                    dialogScreen.Closed -= OnScreenClosed;
                }
                dialogScreen.Closed += OnScreenClosed;
            }

            await DialogHost.Show(view, OnDialogOpened);

            return dialogScreen.DialogResult;
        }
    }
}
