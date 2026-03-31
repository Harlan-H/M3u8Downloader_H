using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using System;
using System.IO;

namespace M3u8Downloader_H.Behaviors;

public class DragAndDropBehavior : Behavior<TextBox>
{
    private static string[] _filterStringArr = [];
    private string _filePath = string.Empty;

    public static readonly StyledProperty<string> FilterStringProperty =
        AvaloniaProperty.Register<DragAndDropBehavior,string>("FilterString",string.Empty);


    public static readonly StyledProperty<bool> IsFileProperty =
        AvaloniaProperty.Register<DragAndDropBehavior, bool>("IsFile");

    public string FilterString
    {
        get => GetValue(FilterStringProperty);
        set => SetValue(FilterStringProperty, value);
    }

    public bool IsFile
    {
        get => GetValue(IsFileProperty); 
        set => SetValue(IsFileProperty, value); 
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property != FilterStringProperty)
            return;

        string? oldValue = change.OldValue as string;
        string? newValue = change.NewValue as string;
        if (oldValue == newValue || string.IsNullOrWhiteSpace(newValue))
            return;

        _filterStringArr = newValue.Split("|");
    }


    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is null)
            return;

        DragDrop.SetAllowDrop(AssociatedObject, value: true);
        AssociatedObject.AddHandler(DragDrop.DragEnterEvent, DragAndDropBehavior_DragEnter);
        AssociatedObject.AddHandler(DragDrop.DragOverEvent, DragAndDropBehavior_DragEnter);
        AssociatedObject.AddHandler(DragDrop.DropEvent, DragAndDropBehavior_Drop);

    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject is null)
            return;

        DragDrop.SetAllowDrop(AssociatedObject, value: false);
        AssociatedObject.RemoveHandler(DragDrop.DragEnterEvent, DragAndDropBehavior_DragEnter);
        AssociatedObject.RemoveHandler(DragDrop.DragOverEvent, DragAndDropBehavior_DragEnter);
        AssociatedObject.RemoveHandler(DragDrop.DropEvent, DragAndDropBehavior_Drop);
    }


    private void DragAndDropBehavior_Drop(object? sender, DragEventArgs  e)
    {
        if (sender is TextBox tb)
        {
            tb.Text = _filePath;
        }
    }

    private void DragAndDropBehavior_DragEnter(object? sender, DragEventArgs e)
    {
        e.DragEffects = DragDropEffects.None;
        if (!e.DataTransfer.Contains(DataFormat.File))
            return;

        var files = e.DataTransfer.TryGetFile();
        if (files is null)
            return;
        

        var filePath = files.Path.LocalPath;
        FileAttributes fileAttributes = File.GetAttributes(filePath);
        if((fileAttributes & FileAttributes.Directory) > 0)
        {
            _filePath = filePath;
            e.DragEffects = DragDropEffects.Copy;
            e.Handled = true;
            return;
        }

        var ext = Path.GetExtension(filePath);
        if ((fileAttributes & FileAttributes.Archive) > 0 && _filterStringArr.Contains(ext))
        {
            _filePath = filePath;
            e.DragEffects = DragDropEffects.Copy;
            e.Handled = true;
        } 
    }
}
