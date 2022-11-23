using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Common.Utils;
using M3u8Downloader_H.Plugin;
using M3u8Downloader_H.vlive.plugin.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace M3u8Downloader_H.vlive.plugin.Readers
{
    internal class M3UFileReader : IM3uFileReader
    {
        public M3UFileInfo Read(Uri baseUri, Stream stream)
        {
            StreamReader streamReader = new(stream);
            StreamInfos? streamInfos = JsonConvert.DeserializeObject<StreamInfos>(streamReader.ReadToEnd());
            if (streamInfos is null)
                throw new InvalidDataException("视频流反序列化失败");

            VideoInfo? videoInfo = streamInfos.VideoInfos
                                        .OrderByDescending(s => s.BandWidth)
                                        .FirstOrDefault();
            if(videoInfo is null)
                throw new InvalidDataException("读取视频流信息出错");

            var queryParam = $"?{streamInfos.Param}={streamInfos.Value}";
            M3UFileInfo m3UFileInfo = M3UFileInfo.CreateVodM3UFileInfo();
            m3UFileInfo.Version = To.Value<int>(videoInfo.TsVersion);
            m3UFileInfo.MediaSequence = videoInfo.MeidaSequence;
            m3UFileInfo.MediaFiles = HandleMedia(videoInfo.Source, videoInfo.TsFormat, queryParam, videoInfo.ExtInfos);
            return m3UFileInfo;
        }

        private static IList<M3UMediaInfo> HandleMedia(Uri baseUri,string format,string param, int[] ExtInfos)
        {
            var newFormat = Regex.Replace(format, @"%(\d+)([sd])", match => "{0:" + match.Groups[2].Value + match.Groups[1].Value + "}");
            int currentIndex = 0;
            List<M3UMediaInfo> m3UMediaInfos = new(ExtInfos.Length);
            for (int i = 0; i < ExtInfos.Length; i++)
            {
                m3UMediaInfos.Add(new M3UMediaInfo()
                {
                    Duration = ExtInfos[i],
                    Uri = new Uri(baseUri, string.Format(newFormat, i) + param),
                    Title = $"{++currentIndex}.tmp"
                });
            }
            return m3UMediaInfos;
        }
    }
}
