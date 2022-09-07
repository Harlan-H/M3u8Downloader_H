using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace M3u8Downloader_H.Behaviors
{
    public class DragAndDropBehaviour : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewDragEnter += DragAndDropBehaviour_PreviewDragEnter;
            AssociatedObject.PreviewDragOver += DragAndDropBehaviour_PreviewDragEnter;
            AssociatedObject.PreviewDrop += DragAndDropBehaviour_PreviewDrop;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewDragEnter -= DragAndDropBehaviour_PreviewDragEnter;
            AssociatedObject.PreviewDragOver -= DragAndDropBehaviour_PreviewDragEnter;
            AssociatedObject.PreviewDrop -= DragAndDropBehaviour_PreviewDrop;
        }


        private void DragAndDropBehaviour_PreviewDrop(object sender, System.Windows.DragEventArgs e)
        {
            object text = e.Data.GetData(DataFormats.FileDrop);
            if (sender is TextBox tb)
            {
                tb.Text = ((string[])text)[0];
                BindingExpression bindingExpression = tb.GetBindingExpression(TextBox.TextProperty);
                bindingExpression.UpdateSource();
            }
        }

        private void DragAndDropBehaviour_PreviewDragEnter(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
    }
}
