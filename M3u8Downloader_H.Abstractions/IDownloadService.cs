using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Plugin
{
    public interface IDownloadService
    {
        Task Initialize(HttpClient httpClient, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken);
        void SetCryptData(string method, byte[] key, byte[] iv);
        Stream HandleData(Stream stream, CancellationToken cancellationToken);
    }
}
