using System;

namespace M3u8Downloader_H.M3U8.AttributeReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class M3U8ReaderAttribute(string key, Type type) : Attribute
    {
        private readonly string key = key;
        private readonly Type type = type;

        public string Key => key;
        public Type Type => type;
    }
}
