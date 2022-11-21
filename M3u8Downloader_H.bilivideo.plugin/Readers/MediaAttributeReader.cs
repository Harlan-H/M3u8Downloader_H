using M3u8Downloader_H.bilivideo.plugin.Extensions;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Plugin;
using System.Web;

namespace M3u8Downloader_H.bilivideo.plugin.Readers
{
    internal class MediaAttributeReader : IAttributeReader
    {
        private int currentIndex;
        private string CurrentIndex => $"{++currentIndex}.tmp";

        public bool ShouldTerminate => false;

        string? baseUriQuery;

        public MediaAttributeReader()
        {

        }

        public void Write(M3UFileInfo m3UFileInfo, string value, IEnumerator<string> reader, Uri baseUri)
        {
            m3UFileInfo.MediaFiles ??= new List<M3UMediaInfo>();

            var m3UmediaInfo = new M3UMediaInfo();
            var strArray = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length > 0)
                m3UmediaInfo.Duration = (float?)Convert.ChangeType(strArray[0], typeof(float)) ?? 0;

            if (!reader.MoveNext())
                throw new InvalidDataException("Invalid M3U file. Missing a media URI.");

            string relativeUri = ChangeQuery(reader.Current.Trim(), baseUri);
            m3UmediaInfo.Uri = new Uri(baseUri, relativeUri);
            m3UmediaInfo.Title = CurrentIndex;

            m3UFileInfo.MediaFiles.Add(m3UmediaInfo);
        }

        private string ChangeQuery(string url,Uri baseuri)
        {
            baseUriQuery ??= HandleBaseQuery(url, baseuri);
            var path = url.GetPath();
            return $"{path}?{baseUriQuery}";
        }

        private static string HandleBaseQuery(string url,Uri baseuri)
        {
            var queryCollection = HttpUtility.ParseQueryString(baseuri.Query);
            var relativeUriCollection = url.GetQuery()?.GetQueryKeyPairs();
            if(relativeUriCollection is not null)
            {
                foreach (var item in relativeUriCollection)
                {
                    queryCollection[item.Key] = item.Value;
                }
            }
            return queryCollection.ToString()!;
        }
    }
}
