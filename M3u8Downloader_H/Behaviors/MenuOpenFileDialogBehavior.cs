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


// internal static class FileFilterParser
// {
//     public static List<FilePickerFileType>? ConvertToFilePickerFileType(string filter)
//     {
//         if (string.IsNullOrWhiteSpace(filter))
//         {
//             return null;
//         }
// 
//         var parts = filter.Split('|');
//         if (parts.Length % 2 != 0)
//         {
//             return null;
//         }
// 
//         var fileTypes = new List<FilePickerFileType>();
// 
//         for (var i = 0; i < parts.Length; i += 2)
//         {
//             var description = parts[i];
//             var patternPart = parts[i + 1];
//             var index = description.IndexOf(" (", StringComparison.Ordinal);
//             if (index > 0)
//             {
//                 description = description.Substring(0, index);
//             }
// 
//             description = description.Trim();
// 
//             var patterns = patternPart
//                 .Split([';'], StringSplitOptions.RemoveEmptyEntries)
//                 .Select(p => p.Trim())
//                 .ToList();
// 
//             fileTypes.Add(new FilePickerFileType(description) { Patterns = patterns });
//         }
// 
//         return fileTypes;
//     }
//}
