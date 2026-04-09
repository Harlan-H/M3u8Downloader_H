using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using System.Linq;

namespace M3u8Downloader_H.Behaviors;

public class OpenFileDialogBehavior : OpenFileDialogBehaviorBase<OpenFileDialogBehavior,Button>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is null)
            return;

        if (AssociatedObject is Button button)
            button.Click += ButtonnOnClick;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        AssociatedObject?.Click -= ButtonnOnClick;
    }

    private async void ButtonnOnClick(object? sender, RoutedEventArgs e)
    {
        var storageProvider = TopLevel.GetTopLevel(AssociatedObject)?.StorageProvider;
        storageProvider ??= AssociatedObject!.GetSelfAndLogicalAncestors().OfType<TopLevel>().FirstOrDefault()?.StorageProvider;
        if (storageProvider is null)
            return;

        await OpenFileOrFolderAsync(storageProvider);
    }
}