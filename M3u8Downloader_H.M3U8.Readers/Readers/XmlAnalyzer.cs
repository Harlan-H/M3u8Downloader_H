using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using M3u8Downloader_H.M3U8.Infos;

namespace M3u8Downloader_H.M3U8.Readers
{
    internal sealed class XmlAnalyzer : AnalyzerBase
    {
        private readonly Uri host;

        public XmlAnalyzer(Uri host) : base(host)
        {
            this.host = host;
        }

        public override M3UFileInfo Read()
        {
            XmlDocument xmlDocument = new();
            xmlDocument.Load(host.OriginalString);

            M3UFileInfo m3UFileInfo = new()
            {
                Key = GetM3UKeyInfo(xmlDocument)!,
                MediaFiles = GetM3UMediaInfos(xmlDocument),
                PlaylistType = "VOD"
            };
            return m3UFileInfo;
        }

        private M3UKeyInfo? GetM3UKeyInfo(XmlDocument xmlDocument)
        {
            XmlNode? keyInfoNode = xmlDocument.SelectSingleNode("root/keyinfo");
            if (keyInfoNode == null) return null;

            XmlAttributeCollection? attribute = keyInfoNode.Attributes;
            string? method = attribute?["method"]?.Value;
            string? uri = attribute?["uri"]?.Value;

            //底层会自己处理密钥数据 不管是正常数据还是base64都会进行处理
            string? key = attribute?["key"]?.Value;  
            string? iv = attribute?["iv"]?.Value;
            return GetM3UKeyInfo(method, uri, key, iv);
        }

        private IList<M3UMediaInfo> GetM3UMediaInfos(XmlDocument xmlDocument)
        {
            XmlNodeList mediaInfoNodeList = xmlDocument.SelectNodes("root/mediainfo") ?? throw new InvalidDataException("mediaInfo节点不能为空");

            List<M3UMediaInfo> m3UMediaInfos = new();
            foreach (XmlNode mediainfo in mediaInfoNodeList)
            {
                XmlAttributeCollection? attribute = mediainfo?.Attributes;
                string uri = attribute?["uri"]?.Value
                             ?? throw new InvalidDataException("没有指明uri字段");

                string? name = attribute?["name"]?.Value;

                M3UMediaInfo m3UMediaInfo = GetM3UMediaInfo(uri, name);
                m3UMediaInfos.Add(m3UMediaInfo);
            }
            return m3UMediaInfos;
        }
    }

}
