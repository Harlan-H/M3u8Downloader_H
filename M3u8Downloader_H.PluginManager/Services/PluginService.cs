using M3u8Downloader_H.PluginManager.Models;
using M3u8Downloader_H.PluginManager.Utils;
using Newtonsoft.Json;
using System;
using System.IO.Packaging;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;

namespace M3u8Downloader_H.PluginManager.Services
{
    public class PluginService
    {
        private readonly HttpClient _httpClient = Http.Client;

        public PluginService()
        {

        }

        public async ValueTask DownloadAsync(
           Uri uri,
           string filePath,
           CancellationToken cancellationToken = default)
        {
            Stream stream =  await _httpClient.GetStreamAsync(uri, cancellationToken);
            await CopyToAsync(stream, filePath, cancellationToken);
        }

        public async ValueTask CopyToAsync(
            Stream stream,
           string filePath,
            CancellationToken cancellationToken = default)
        {
            using var destination = File.Create(filePath);
            await stream.CopyToAsync(destination, cancellationToken);
        }

 
    }
}
