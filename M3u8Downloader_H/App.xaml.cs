using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MaterialDesignColors;

namespace M3u8Downloader_H
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Assembly Assembly { get; } = typeof(App).Assembly;

        public static Version Version { get; } = Assembly.GetName().Version!;
        public static string VersionString { get; } = Version.ToString(3);

        public static Color DeepPurpleColor { get; } = SwatchHelper.Lookup[(MaterialDesignColor)PrimaryColor.DeepPurple];
    }
}
