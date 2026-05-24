using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Utilities.Models
{
    public class TimeOutOptions(TimeSpan timeSpan, long? minBytesPerSencond)
    {
        /// <summary>
        /// Header 超时
        /// </summary>
        public TimeSpan HeaderTimeout { get; } = timeSpan;

        /// <summary>
        /// Idle 超时（多久没数据算卡死）
        /// </summary>
        public TimeSpan IdleTimeout { get; } = timeSpan;

        /// <summary>
        /// 最低速度限制（字节/秒）
        /// null = 不启用
        /// </summary>
        public long? MinBytesPerSecond { get; set; } = minBytesPerSencond;

        /// <summary>
        /// 速度检测窗口
        /// </summary>
        public TimeSpan SpeedCheckWindow { get; } = timeSpan;

        public TimeOutOptions(long timeout) : this(TimeSpan.FromSeconds(timeout),null)
        {
            
        }

        public TimeOutOptions(TimeSpan timeSpan) : this(timeSpan, null) 
        { 

        }

        public TimeOutOptions() : this(TimeSpan.FromSeconds(30))
        {
            
        }
    }
}
