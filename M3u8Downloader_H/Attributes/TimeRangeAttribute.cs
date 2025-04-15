using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Attributes
{
    internal class TimeRangeAttribute(int min, int max) : BaseAttribute
    {
        public override void Validate(object obj, object property, object? value)
        {
            if (value is null)
                throw new InvalidDataException("传入得无效得数值");

            TimeSpan val = (TimeSpan)value;
            PropertyInfo tmpproperty = (PropertyInfo)property;
            if (val < TimeSpan.FromSeconds(min))
            {
                tmpproperty.SetValue(obj, min);
                return;
            }

            if (val > TimeSpan.FromSeconds(max))
            {
                tmpproperty.SetValue(obj, max);
                return;
            }
        }
    }
}
