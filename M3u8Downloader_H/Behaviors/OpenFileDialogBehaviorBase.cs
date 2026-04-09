using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Behaviors;

public abstract class OpenFileDialogBehaviorBase<TOwner, TControl> : Behavior<TControl> 
    where TOwner : AvaloniaObject
    where TControl : AvaloniaObject
{
    public static readonly StyledProperty<bool> IsFileProperty =
       AvaloniaProperty.Register<TOwner, bool>(nameof(IsFile));

    public static readonly StyledProperty<string?> FileTypeFilterProperty =
        AvaloniaProperty.Register<TOwner, string?>(nameof(FileTypeFilter));

    public static readonly StyledProperty<TextBox> AttachedTextBoxProperty =
        AvaloniaProperty.Register<TOwner, TextBox>(nameof(AttachedTextBox));


    public bool IsFile
    {
        get => GetValue(IsFileProperty);
        set => SetValue(IsFileProperty, value);
    }

    public TextBox AttachedTextBox
    {
        get => GetValue(AttachedTextBoxProperty);
        set => SetValue(AttachedTextBoxProperty, value);
    }

    public string? FileTypeFilter
    {
        get => GetValue(FileTypeFilterProperty);
        set => SetValue(FileTypeFilterProperty, value);
    }

    protected async Task OpenFileOrFolderAsync(IStorageProvider storageProvider)
    {

        if (IsFile)
        {
            await OpenFilePiackerAsync(storageProvider);
        }
        else
        {
            await OpenFolderPickerAsync(storageProvider);
        }
    }

    private async Task OpenFilePiackerAsync(IStorageProvider storageProvider)
    {
        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择要打开的文件",
            AllowMultiple = false,
            FileTypeFilter = FileTypeFilter is not null
        ? FileFilterParser.ConvertToFilePickerFileType(FileTypeFilter)
        : null
        });

        if (files.Count > 0)
        {
            AttachedTextBox.Text = files[0].Path.LocalPath;
        }
    }

    private async Task OpenFolderPickerAsync(IStorageProvider storageProvider)
    {
        IReadOnlyList<IStorageFolder> folders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "选择要打开的目录",

            AllowMultiple = false
        });

        if (folders.Count > 0)
        {
            AttachedTextBox.Text = folders[0].Path.LocalPath;
        }
    }
}


internal static class FileFilterParser
{
    public static List<FilePickerFileType>? ConvertToFilePickerFileType(string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return null;
        }

        var parts = filter.Split('|');
        if (parts.Length % 2 != 0)
        {
            return null;
        }

        var fileTypes = new List<FilePickerFileType>();

        for (var i = 0; i < parts.Length; i += 2)
        {
            var description = parts[i];
            var patternPart = parts[i + 1];
            var index = description.IndexOf(" (", StringComparison.Ordinal);
            if (index > 0)
            {
                description = description.Substring(0, index);
            }

            description = description.Trim();

            var patterns = patternPart
                .Split([';'], StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();

            fileTypes.Add(new FilePickerFileType(description) { Patterns = patterns });
        }

        return fileTypes;
    }
}
