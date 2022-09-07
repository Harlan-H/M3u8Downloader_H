using PluginInterface;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.M3U8.Infos;
using System.Collections.Generic;
using M3u8Downloader_H.Extensions;

namespace M3u8Downloader_H.Core.M3uDownloaders
{
    internal class PluginM3u8Downloader : M3u8Downloader
    {
        private readonly string PluginPath;
        private IPlugin plugin = default!;
        private readonly HttpClient http;
        private readonly IEnumerable<KeyValuePair<string, string>>? headers;
        private readonly M3UFileInfo m3UFileInfo;

        public PluginM3u8Downloader(string pluginPath, HttpClient http, IEnumerable<KeyValuePair<string, string>>? headers, M3UFileInfo m3UFileInfo, IProgress<double> progress) : base(http, headers, progress)
        {
            this.http = http;
            this.headers = headers;
            this.m3UFileInfo = m3UFileInfo;
            PluginPath = Path.IsPathRooted(pluginPath) ? pluginPath : "plugin/" + pluginPath;

        }

        private IPlugin InitPlugin()
        {
            Type[] exportTypes = Assembly.LoadFrom(PluginPath).GetExportedTypes();

            var result = exportTypes.Where(i => i.GetInterface(nameof(IPlugin)) != null).FirstOrDefault();
            if (result == null)
                throw new TypeAccessException($"{PluginPath}没有实现IPlugin接口");

            return CreatePluginInstance(result) ?? throw new InvalidDataException("没有找到合适的构造函数");
        }

        private IPlugin? CreatePluginInstance(Type type)
        {
            ConstructorInfo? constructorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, new Type[] { typeof(HttpClient), typeof(IEnumerable<KeyValuePair<string, string>>) }, null);
            IPlugin? iplugin = constructorInfo?.Invoke(new object[] { http, headers! }) as IPlugin;
            if (iplugin == null)
            {
                constructorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, Array.Empty<Type>(), null);
                iplugin = constructorInfo?.Invoke(null) as IPlugin;
            }
            return iplugin;
        }

        public override async ValueTask Initialization(CancellationToken cancellationToken)
        {
            plugin = InitPlugin();

            await plugin.Initialize(cancellationToken);

            if (m3UFileInfo.Key is null)
                return;

            if (m3UFileInfo.Key.BKey != null)
            {
                plugin.SetCryptData(m3UFileInfo.Key.Method, m3UFileInfo.Key.BKey, m3UFileInfo.Key.IV);
            }
            else if (m3UFileInfo.Key.Uri != null)
            {
                //不会在尝试解析密钥 因为可能不知道 他会遇到啥类型的密钥
                byte[] data = m3UFileInfo.Key.Uri.IsFile
                    ? await File.ReadAllBytesAsync(m3UFileInfo.Key.Uri.OriginalString, cancellationToken)
                    : await http.GetByteArrayAsync(m3UFileInfo.Key.Uri, headers, cancellationToken);
                plugin.SetCryptData(m3UFileInfo.Key.Method, data, m3UFileInfo.Key.IV);
            }
        }

        protected override Stream DownloadAfter(Stream stream, string contentType, CancellationToken cancellationToken)
        {
            return plugin.HandleData(stream, cancellationToken);
        }

    }
}
