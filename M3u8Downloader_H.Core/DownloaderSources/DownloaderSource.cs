using CliWrap.Builders;
using M3u8Downloader_H.Core.DownloaderManagers;
using M3u8Downloader_H.Core.M3uCombiners;
using M3u8Downloader_H.Core.M3uDownloaders;
using M3u8Downloader_H.Core.VideoConverter;
using M3u8Downloader_H.M3U8.Infos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Core.DownloaderSources
{
    internal abstract class DownloaderSource : IDownloaderSource
    {
        public Uri Url  = default!;
        public HttpClient HttpClient  = default!;
        public IEnumerable<KeyValuePair<string, string>>? Headers;
        public string VideoFullPath = default!;
        public string VideoFullName = default!;
        public M3UFileInfo M3UFileInfo = default!;
        public IProgress<double> VodProgress  = default!;
        public IProgress<double> LiveProgress = default!;
        public Action<int> SetStatusDelegate = default!;
        public Action<string> ChangeVideoNameDelegate = default!;

        protected string PluginPath = default!;
        protected int _taskNumber;
        protected int _timeouts = default!;
        protected double _maxRecordDuration;
        protected bool _skipRequestError = false;
        protected bool _forceMerge;
        protected string _formats = default!;
        protected bool _isCleanUp;
        protected bool _skipDirectoryExist;
        protected string _savePath = default!;

        private readonly FFmpeg _ffmpeg;
        private bool _firstTimeToRun = true;

        public DownloaderSource()
        {
#if DEBUG
            _ffmpeg = new(@"F:\源代码\库\ffmpeg-4.3.1-2020-11-19-full_build-shared\bin\ffmpeg.exe");
#else
            _ffmpeg = new("./ffmpeg.exe");
#endif
        }

        public IDownloaderSource WithTaskNumber(int number)
        {
            _taskNumber = number;
            return this;
        }

        public IDownloaderSource WithTimeout(int timeout)
        {
            _timeouts = timeout;
            return this;
        }

        public IDownloaderSource WithSavePath(string savepath)
        {
            _savePath = savepath;
            return this;
        }

        public IDownloaderSource WithMaxRecordDuration(double duration)
        {
            _maxRecordDuration = duration;
            return this;
        }

        public IDownloaderSource WithSkipRequestError(bool skip)
        {
            _skipRequestError = skip;
            return this;
        }

        public IDownloaderSource WithForceMerge(bool merge)
        {
            _forceMerge = merge;
            return this;
        }

        public IDownloaderSource WithFormats(string format)
        {
            _formats = format;
            return this;
        }

        public IDownloaderSource WithCleanUp(bool isCleanUp)
        {
            _isCleanUp = isCleanUp;
            return this;
        }

        public IDownloaderSource WithSkipDirectoryExist(bool isCleanUp)
        {
            _skipDirectoryExist = isCleanUp;
            return this;
        }

        public IDownloaderSource WithHeaders(IEnumerable<KeyValuePair<string, string>>? headers)
        {
            Headers ??= headers;
            return this;
        }

        protected M3u8Downloader CreateDownloader()
        {
            return !string.IsNullOrWhiteSpace(PluginPath)
                ? new PluginM3u8Downloader(PluginPath, M3UFileInfo)
                : M3UFileInfo.Key is not null
                ? new CryptM3uDownloader(M3UFileInfo)
                : new M3u8Downloader();
        }

        public virtual Task DownloadAsync(CancellationToken cancellationToken = default)
        {
            if (_firstTimeToRun)
            {
                CreateDirectory(VideoFullPath, _skipDirectoryExist);
                _firstTimeToRun = false;
            }
            
            return Task.CompletedTask;
        }

        public virtual async Task ConvertToMp4(CancellationToken cancellationToken = default)
        {
            string fileExtension = Path.GetExtension(VideoFullName).Trim('.');
            if (!(fileExtension != "mp4" && _formats == "mp4")) return;

            await Converter(cancellationToken);
        }

        protected async ValueTask VideoMerge(bool isFile)
        {
            using M3uCombiner m3UCombiner = isFile && M3UFileInfo.Key is not null
                ? new CryptM3uCombiner(M3UFileInfo, VideoFullPath, VideoFullName)
                : new M3uCombiner(VideoFullPath, VideoFullName);

            m3UCombiner.Initialization();
            await m3UCombiner.MegerVideoHeader(M3UFileInfo.Map);
            await m3UCombiner.Start(M3UFileInfo, _forceMerge);
        }

        protected async ValueTask Converter( CancellationToken cancellationToken = default)
        {
            var arguments = new ArgumentsBuilder();

            arguments.Add("-i").Add(VideoFullName);

            arguments.Add("-f").Add(_formats);

            arguments
                .Add("-c:a").Add("copy")
                .Add("-c:v").Add("copy");

            var tmpOutputFile = Path.ChangeExtension(VideoFullName, _formats);
            ChangeVideoNameDelegate(tmpOutputFile);
            arguments
                .Add("-nostdin")
                .Add("-y").Add(tmpOutputFile);

            await _ffmpeg.ExecuteAsync(arguments.Build(), VodProgress, cancellationToken);

            File.Delete(VideoFullName);
        }


        protected static void CreateDirectory(string dirPath, bool skipExist = true)
        {
            DirectoryInfo directoryInfo = new(dirPath);
            if (directoryInfo.Exists)
            {
                if (skipExist) return;

                throw new Exception($"{dirPath} 目录已经存在，程序停止");
            }
            directoryInfo.Create();
        }


        protected void RemoveCacheDirectory(string filePath, bool recursive = true)
        {
            try
            {
                if (_isCleanUp)
                    Directory.Delete(filePath, recursive);
            }
            catch (DirectoryNotFoundException)
            {

            }
        }
    }
}
