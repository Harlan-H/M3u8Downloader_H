using System;

namespace M3u8Downloader_H.M3U8.Extensions
{
    public static class UriExtension
    {
        public static Uri Join(this Uri host, string uri)
        {
            if (Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out Uri? relativeUri))
            {
                if (relativeUri.IsAbsoluteUri)
                {
                    return relativeUri;
                }
                else
                {
                    if (host != null)
                        return new Uri(host, relativeUri);
                }
            }
            throw new UriFormatException(uri);
        }
    }
}
