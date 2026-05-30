using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M3u8Downloader_H.Behaviors
{
    public class EnsureScrollingToBottomBehavior : Behavior<ItemsControl>
    {
         protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject?.PropertyChanged += OnPropertyChanged;
            
        }

        protected override void OnDetaching()
        {
            AssociatedObject?.PropertyChanged -= OnPropertyChanged;

            base.OnDetaching();
        }

        private void OnPropertyChanged(
            object? sender,
            AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == ItemsControl.ItemCountProperty)
            {
                ScrollToBottom();
            }
        }

        private void ScrollToBottom()
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (AssociatedObject == null ||
                    AssociatedObject.ItemCount == 0)
                    return;

                AssociatedObject.ScrollIntoView(AssociatedObject.ItemCount - 1);
                
            });
        }
    }
}
