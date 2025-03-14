using Microsoft.Xaml.Behaviors;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;

namespace M3u8Downloader_H.Behaviors
{
    public class DragAndDropBehaviour : Behavior<TextBox>
    {
        private static string[] _filterStringArr = [];
        private string _filePath = string.Empty;
        // Using a DependencyProperty as the backing store for FilterString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterStringProperty =
            DependencyProperty.Register("FilterString", typeof(string), typeof(DragAndDropBehaviour),new PropertyMetadata(FilterStringChangedCallback));

        public string FilterString
        {
            get { return (string)GetValue(FilterStringProperty); }
            set { SetValue(FilterStringProperty, value); }
        }

        public static void FilterStringChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not DragAndDropBehaviour)
                return;
            
            string? oldValue = e.OldValue as string;
            string? newValue = e.NewValue as string;
            if (oldValue == newValue || string.IsNullOrWhiteSpace(newValue))
                return;

            _filterStringArr = newValue.Split("|");
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AllowDrop = true;
            AssociatedObject.PreviewDragEnter += DragAndDropBehaviour_PreviewDragEnter;
            AssociatedObject.PreviewDragOver += DragAndDropBehaviour_PreviewDragEnter;
            AssociatedObject.PreviewDrop += DragAndDropBehaviour_PreviewDrop;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.AllowDrop = false;
            AssociatedObject.PreviewDragEnter -= DragAndDropBehaviour_PreviewDragEnter;
            AssociatedObject.PreviewDragOver -= DragAndDropBehaviour_PreviewDragEnter;
            AssociatedObject.PreviewDrop -= DragAndDropBehaviour_PreviewDrop;
        }


        private void DragAndDropBehaviour_PreviewDrop(object sender, System.Windows.DragEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.Text = _filePath; 
                BindingExpression bindingExpression = tb.GetBindingExpression(TextBox.TextProperty);
                bindingExpression.UpdateSource();
            }
        }

        private void DragAndDropBehaviour_PreviewDragEnter(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;

            if (e.Data.GetData(DataFormats.FileDrop) is not string[] text)
                return;
            if (text.Length > 1)
                return;

            var ext = Path.GetExtension(text[0]);
            if( _filterStringArr.Contains(ext))
            {
                _filePath = text[0];
                e.Handled = true;
            }
            else 
                e.Handled = false;
        }
    }
}
