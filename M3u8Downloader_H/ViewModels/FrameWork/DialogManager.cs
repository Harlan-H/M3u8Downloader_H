using MaterialDesignThemes.Wpf;
using Stylet;
using System;
using System.Threading.Tasks;

namespace M3u8Downloader_H.ViewModels.FrameWork
{
    public class DialogManager
    {
        private readonly IViewManager viewManager;

        public DialogManager(IViewManager viewManager)
        {
            this.viewManager = viewManager;
        }

        public async Task<T?> ShowDialogAsync<T>(DialogScreen<T> dialogScreen)
        {
            var view = viewManager.CreateAndBindViewForModelIfNecessary(dialogScreen);

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
