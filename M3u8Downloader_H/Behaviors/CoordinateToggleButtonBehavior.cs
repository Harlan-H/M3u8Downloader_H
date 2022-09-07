using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace M3u8Downloader_H.Behaviors
{
    class CoordinateToggleButtonBehavior : Behavior<ToggleButton>
    {

        // Using a DependencyProperty as the backing store for Target.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(ToggleButton), typeof(CoordinateToggleButtonBehavior));

        public ToggleButton Target
        {
            get { return (ToggleButton)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }


        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Checked += AssociatedObject_Checked;
        }

        private void AssociatedObject_Checked(object sender, RoutedEventArgs e)
        {
            if (AssociatedObject.IsChecked is not null && AssociatedObject.IsChecked == true)
            {
                Target.IsChecked = true;
                BindingExpression bindingExpression = Target.GetBindingExpression(ToggleButton.IsCheckedProperty);
                bindingExpression.UpdateSource();
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Checked -= AssociatedObject_Checked;
        }
    }
}
