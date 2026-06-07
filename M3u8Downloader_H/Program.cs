using Avalonia;
using M3u8Downloader_H.Utils;
using System;
using System.Reflection;

namespace M3u8Downloader_H
{
    internal sealed class Program
    {
        private static Assembly Assembly { get; } = typeof(App).Assembly;

        public static Version Version { get; } = Assembly.GetName().Version!;
        public static string VersionString { get; } = $"v{Version.ToString(3)}";

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }catch(Exception e)
            {
                if (OperatingSystem.IsWindows())
                    _ = WinApi.MessageBox(0, e.ToString(), "错误", 0x10);

                throw;
            }

        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
#if DEBUG
                .WithDeveloperTools()
#endif
                .WithInterFont()
                .LogToTrace();
    }
}
