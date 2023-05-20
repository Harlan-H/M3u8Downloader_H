using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace M3u8Downloader_H.Behaviors
{
    internal class HeightAnimationTriggerAction : TriggerAction<Button>
    {
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetControlProperty =
            DependencyProperty.Register("TargetControl", typeof(Grid), typeof(HeightAnimationTriggerAction));

        // Using a DependencyProperty as the backing store for From.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(double), typeof(HeightAnimationTriggerAction));

        // Using a DependencyProperty as the backing store for To.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(double), typeof(HeightAnimationTriggerAction));



        public Grid TargetControl
        {
            get { return (Grid)GetValue(TargetControlProperty); }
            set { SetValue(TargetControlProperty, value); }
        }

        public double From
        {
            get { return (double)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public double To
        {
            get { return (double)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            double dTo;
            if (TargetControl.ActualHeight == From)
            {
                dTo = To;
            }
            else if (TargetControl.ActualHeight == To)
            {
                dTo = From;
            }
            else
            {
                return;
            }
            DoubleAnimation heightAnimation = new(dTo, TimeSpan.FromSeconds(0.3), FillBehavior.HoldEnd);
            TargetControl.BeginAnimation(Grid.HeightProperty, heightAnimation);
        }
    }
}
