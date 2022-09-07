using CliWrap.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Core.VideoConverter;
using M3u8Downloader_H.M3U8.Infos;

namespace M3u8Downloader_H.Core.M3uCombiners
{
    public class CombinerFactory
    {
#if DEBUG
        private const string FFmpegCliFilePath = @"F:\源代码\库\ffmpeg-4.3.1-2020-11-19-full_build-shared\bin\ffmpeg.exe";
#else
        private const string FFmpegCliFilePath = "./ffmpeg.exe";
#endif
        public CombinerFactory()
        {
        }

        public static async ValueTask VideoMerge(M3UFileInfo m3UFileInfo, string fileFullPath, string videoName, bool isFile, bool ForceMerge)
        {
            using M3uCombiner m3UCombiner = isFile && m3UFileInfo.Key is not null
                ? new CryptM3uCombiner(m3UFileInfo, fileFullPath, videoName)
                : new M3uCombiner(fileFullPath, videoName);

            m3UCombiner.Initialization();
            await m3UCombiner.MegerVideoHeader(m3UFileInfo.Map);
            await m3UCombiner.Start(m3UFileInfo, ForceMerge);
        }

        public static async ValueTask Converter(string inputFile, string extension, string outputFile, bool deleteOriginalFile, IProgress<double> progress, CancellationToken cancellationToken = default)
        {
           // var tmpInputFile = inputFile + ".ts";

            FFmpeg Ffmpeg = new(FFmpegCliFilePath);
            var arguments = new ArgumentsBuilder();

            arguments.Add("-i").Add(inputFile);

            arguments.Add("-f").Add(extension);

            arguments
                .Add("-c:a").Add("copy")
                .Add("-c:v").Add("copy");

            var tmpOutputFile = Path.ChangeExtension(outputFile, extension);
            arguments
                .Add("-nostdin")
                .Add("-y").Add(tmpOutputFile);

            await Ffmpeg.ExecuteAsync(arguments.Build(), progress, cancellationToken);

            if (deleteOriginalFile)
                File.Delete(inputFile);

        }
    }
}
