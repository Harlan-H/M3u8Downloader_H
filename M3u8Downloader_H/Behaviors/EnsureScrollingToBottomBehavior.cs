using Microsoft.Xaml.Behaviors;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace M3u8Downloader_H.Behaviors
{
    public class EnsureScrollingToBottomBehavior : Behavior<ItemsControl>
    {
        private ScrollViewer? _scrollViewer;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ItemsControl itemscontrol)
            {
                // 监听集合变化事件
                if (itemscontrol.ItemsSource is INotifyCollectionChanged collection)
                {
                    collection.CollectionChanged += Collection_CollectionChanged;
                }
            }
        }

        private void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // 当有新项添加时，滚动到最后一项
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (AssociatedObject.Items.Count > 0)
                {
                    ScrollToEnd();
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        public void ScrollToEnd()
        {
            _scrollViewer ??= FindVisualChild<ScrollViewer>(AssociatedObject);
            _scrollViewer?.ScrollToEnd();
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is not null && child is T scrollViewer)
                {
                    return scrollViewer;
                }

                var result = FindVisualChild<T>(child!);
                if (result != null) return result;
            }
            return null!;
        }
    }
}
