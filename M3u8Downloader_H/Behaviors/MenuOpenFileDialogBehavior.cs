using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Platform.Storage;
using Avalonia.Xaml.Interactions.Core;
using Avalonia.Xaml.Interactivity;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Behaviors;

public class MenuOpenFileDialogBehavior : OpenFileDialogBehaviorBase<MenuOpenFileDialogBehavior,MenuItem>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is null)
            return;

        if (AssociatedObject is MenuItem button)
            button.Click += ButtonnOnClick;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        AssociatedObject?.Click -= ButtonnOnClick;
    }



    private async void ButtonnOnClick(object? sender, RoutedEventArgs e)
    {
        var storageProvider = TopLevel.GetTopLevel(AttachedTextBox)?.StorageProvider;
        storageProvider ??= AttachedTextBox!.GetSelfAndLogicalAncestors().OfType<TopLevel>().FirstOrDefault()?.StorageProvider;
        if (storageProvider is null)
            return;

        await OpenFileOrFolderAsync(storageProvider);
    }
}
