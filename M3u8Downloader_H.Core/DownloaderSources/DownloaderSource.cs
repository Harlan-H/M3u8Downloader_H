using CliWrap.Builders;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Core.M3uCombiners;
using M3u8Downloader_H.Core.M3uDownloaders;
using M3u8Downloader_H.Core.Utils.Extensions;
using M3u8Downloader_H.Core.VideoConverter;
using M3u8Downloader_H.M3U8.M3UFileReaderManangers;
using M3u8Downloader_H.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        public IDownloadService? downloadService = default!;
        public IM3UFileInfoMananger M3uReader = default!;

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
        protected IProgress<long> _downloadRate = default!;

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

        public IDownloaderSource WithDownloadRate(IProgress<long> downloadRate)
        {
            _downloadRate = downloadRate;
            return this;
        }

        public IDownloaderSource WithTaskNumber(int number)
        {
            _taskNumber = number;
            return this;
        }

        public IDownloaderSource WithTimeout(int timeout)
        {
            _timeouts = timeout * 1000;
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
            return downloadService is not null
                ? new PluginM3u8Downloader(downloadService, M3UFileInfo)
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

        protected virtual async Task Converter(bool isFile,CancellationToken cancellationToken = default)
        {
            if(_formats == "mp4")
            {
                if (M3UFileInfo.MediaFiles.Any(m => m.Duration > 0))
                    await ConvertWithM3u8File(cancellationToken);
                else
                    await ConvertWithFile(isFile, cancellationToken);
            }
            else
            {
                await VideoMerge(isFile, cancellationToken);
            }
        }

        //通过xml,目录,json等方式可能无法判断流的时长，所以采用原先的转码方案
        protected async ValueTask ConvertWithFile(bool isFile, CancellationToken cancellationToken)
        {
            await VideoMerge(isFile, cancellationToken);
            await ConverterToMp4(VideoFullName,false,cancellationToken);
            File.Delete(VideoFullName);
        }

        protected async ValueTask ConvertWithM3u8File(CancellationToken cancellationToken)
        {
            string m3u8FilePath = Path.Combine(VideoFullPath, "generated.m3u8");
            if(_forceMerge)
                M3UFileInfo.MediaFiles = M3UFileInfo.MediaFiles.Where(m => File.Exists(Path.Combine(VideoFullPath, m.Title))).ToList();
            await M3UFileInfo.WriteToAsync(m3u8FilePath, cancellationToken);
            await ConverterToMp4(m3u8FilePath, true, cancellationToken);
            File.Delete(m3u8FilePath);
        }

        protected async ValueTask VideoMerge(bool isFile, CancellationToken cancellationToken = default)
        {
            using M3uCombiner m3UCombiner = isFile && M3UFileInfo.Key is not null
                ? new CryptM3uCombiner(M3UFileInfo, VideoFullPath)
                : new M3uCombiner(VideoFullPath);

            m3UCombiner.Progress = VodProgress;
            m3UCombiner.Initialization(VideoFullName);
            await m3UCombiner.MegerVideoHeader(M3UFileInfo.Map, cancellationToken);
            await m3UCombiner.Start(M3UFileInfo, _forceMerge, cancellationToken);
        }

        protected async ValueTask ConverterToMp4(string m3u8FilePath,bool allowed_extensions, CancellationToken cancellationToken = default)
        {
            var arguments = new ArgumentsBuilder();

            if(allowed_extensions)
                arguments.Add("-allowed_extensions").Add("ALL");

            arguments.Add("-i").Add(m3u8FilePath);

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
