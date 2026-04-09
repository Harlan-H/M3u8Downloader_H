using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace M3u8Downloader_H.Behaviors;

class LinkageToggleButtonBehavior : Behavior<ToggleButton>
{
    public static readonly StyledProperty<ToggleButton> TargetProperty =
        AvaloniaProperty.Register<LinkageToggleButtonBehavior, ToggleButton>("Target");

    public ToggleButton Target
    {
        get { return GetValue(TargetProperty); }
        set { SetValue(TargetProperty, value); }
    }


    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject?.IsCheckedChanged += AssociatedObject_Checked;
    }

    private void AssociatedObject_Checked(object? sender, RoutedEventArgs e)
    {
        if (Target is null) return;
        if (AssociatedObject?.IsChecked is not null && AssociatedObject.IsChecked == true)
        {
            Target.IsChecked = true;
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject?.IsCheckedChanged -= AssociatedObject_Checked;
    }
}
