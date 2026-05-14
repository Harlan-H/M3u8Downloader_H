using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Models
{
    public enum PluginStatus
    {
        /// <summary>
        /// 版本过低
        /// </summary>
        VersionTooLow,

        Normal,
        /// <summary>
        /// 加载中
        /// </summary>
        Loading,

        /// <summary>
        /// 安装完成
        /// </summary>
        Installed,


        Failed
    }
}
