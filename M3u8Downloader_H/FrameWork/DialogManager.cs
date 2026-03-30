using DialogHostAvalonia;
using M3u8Downloader_H.FrameWork;
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

        public static async Task<T?> ShowDialogAsync<T>(DialogViewModelBase<T> dialogScreen)
        {
            void OnDialogOpened(object? openSender, DialogOpenedEventArgs openArgs)
            {
                void OnScreenClosed(object? openSender,EventArgs closeArgs)
                {
                    openArgs.Session.Close();
                    dialogScreen.Closed -= OnScreenClosed;
                }
                dialogScreen.Closed += OnScreenClosed;
            }

            await DialogHost.Show(dialogScreen, OnDialogOpened);

            return dialogScreen.DialogResult;
        }
    }
}
