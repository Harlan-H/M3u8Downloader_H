using Avalonia;
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
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
