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

public class OpenFileDialogBehavior : OpenFileDialogBehaviorBase<OpenFileDialogBehavior,Button>
{
//     public static readonly StyledProperty<bool> IsFileProperty =
//         AvaloniaProperty.Register<OpenFileDialogBehavior, bool>(nameof(IsFile));
// 
//     public static readonly StyledProperty<string?> FileTypeFilterProperty =
//         AvaloniaProperty.Register<OpenFileDialogBehavior, string?>(nameof(FileTypeFilter));
// 
//     public static readonly StyledProperty<TextBox> AttachedTextBoxProperty =
//         AvaloniaProperty.Register<OpenFileDialogBehavior, TextBox>(nameof(AttachedTextBox));
// 
// 
// 
//     public bool IsFile
//     {
//         get => GetValue(IsFileProperty);
//         set => SetValue(IsFileProperty, value);
//     }
// 
//     public TextBox AttachedTextBox
//     {
//         get => GetValue(AttachedTextBoxProperty);
//         set => SetValue(AttachedTextBoxProperty, value);
//     }
// 
//     public string? FileTypeFilter
//     {
//         get => GetValue(FileTypeFilterProperty);
//         set => SetValue(FileTypeFilterProperty, value);
//     }


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
//         if (IsFile)
//         {
//             await OpenFilePiackerAsync(storageProvider);
//         }
//         else
//         {
//             await OpenFolderPickerAsync(storageProvider);
//         }
    }

//     private async Task OpenFilePiackerAsync(IStorageProvider storageProvider)
//     {
//         var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
//         {
//             Title = "选择要打开的文件",
//             AllowMultiple = false,
//             FileTypeFilter = FileTypeFilter is not null
//         ? FileFilterParser.ConvertToFilePickerFileType(FileTypeFilter)
//         : null
//         });
// 
//         if(files.Count > 0)
//         {
//             AttachedTextBox.Text = files[0].Path.AbsolutePath;
//         }
//     }
// 
//     private async Task OpenFolderPickerAsync(IStorageProvider storageProvider)
//     {
//         IReadOnlyList<IStorageFolder> folders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
//         {
//             Title = "选择要打开的目录",
// 
//             AllowMultiple = false
//         });
// 
//         if (folders.Count > 0)
//         {
//             AttachedTextBox.Text = folders[0].Path.AbsolutePath;
//         }
//     }
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
