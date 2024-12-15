using Microsoft.Win32;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace M3u8Downloader_H.Behaviors
{
    public class OpenFileDialogBehaviour : Behavior<Button>
    {

        // Using a DependencyProperty as the backing store for AttachedTextBox.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttachedTextBoxProperty =
            DependencyProperty.Register("AttachedTextBox", typeof(TextBox), typeof(OpenFileDialogBehaviour));

        // Using a DependencyProperty as the backing store for FilterString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterStringProperty =
            DependencyProperty.Register("FilterString", typeof(string), typeof(OpenFileDialogBehaviour));

        // Using a DependencyProperty as the backing store for IsFile.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFileProperty =
            DependencyProperty.Register("IsFile", typeof(bool), typeof(OpenFileDialogBehaviour));


        public bool IsFile
        {
            get { return (bool)GetValue(IsFileProperty); }
            set { SetValue(IsFileProperty, value); }
        }


        public TextBox AttachedTextBox
        {
            get { return (TextBox)GetValue(AttachedTextBoxProperty); }
            set { SetValue(AttachedTextBoxProperty, value); }
        }


        public string FilterString
        {
            get { return (string)GetValue(FilterStringProperty); }
            set { SetValue(FilterStringProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
                AssociatedObject.Click += AssociatedObject_Click;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
                AssociatedObject.Click -= AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            
            if (IsFile)
            {
                OnOpenFileDialog();
            }
            else
            {
                OnBrowerFolderDialog();
            }
        }

        private void OnOpenFileDialog()
        {
            OpenFileDialog dlg = new()
            {
                Filter = FilterString
            };
            var ret = dlg.ShowDialog();
            if (ret != null && ret.Value)
            {
                AttachedTextBox.Text = dlg.FileName;
                BindingExpression bindingExpression = AttachedTextBox.GetBindingExpression(TextBox.TextProperty);
                bindingExpression.UpdateSource();
            }
        }
        private void OnBrowerFolderDialog()
        {

            OpenFolderDialog openFolderDialog = new();
            if (openFolderDialog.ShowDialog() is true)
            {
                AttachedTextBox.Text = openFolderDialog.FolderName;
                BindingExpression bindingExpression = AttachedTextBox.GetBindingExpression(TextBox.TextProperty);
                bindingExpression.UpdateSource();
            }
        }

    }
}
