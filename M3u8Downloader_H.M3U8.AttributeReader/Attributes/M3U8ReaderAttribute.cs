using System;

namespace M3u8Downloader_H.M3U8.AttributeReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class M3U8ReaderAttribute : Attribute
    {
        private readonly string key;
        private readonly Type type;

        public M3U8ReaderAttribute(string key, Type type)
        {
            this.key = key;
            this.type = type;
        }

        public string Key => key;
        public Type Type => type;
    }
}
