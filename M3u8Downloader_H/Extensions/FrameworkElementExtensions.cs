using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace M3u8Downloader_H.Extensions
{ 
    public static  class FrameworkElementExtensions
    {
        public static IEnumerable<FrameworkElement> GetAllAncestors(this FrameworkElement element)
        {
            while ((element = (FrameworkElement)VisualTreeHelper.GetParent(element)) != null)
            {
                yield return element;
            }
        }

        public static T FindVisualChild<T>(this DependencyObject element) where T : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child =  VisualTreeHelper.GetChild(element, i);
                if (child is not null && child is T content)
                {
                    return content;
                }

                var result =  FindVisualChild<T>(child!);
                if (result != null) return result;
            }
            return null!;
        }
    } 
}
