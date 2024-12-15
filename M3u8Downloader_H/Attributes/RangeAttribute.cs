using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Attributes
{
    internal class RangeAttribute(int min, int max) : BaseAttribute
    {
        private readonly int min = min;
        private readonly int max = max;

        public override void Validate(object obj, object property, object? value)
        {
            if (value is null)
                throw new InvalidDataException("传入得无效得数值");

            var val = (int)value;
            PropertyInfo tmpproperty = (PropertyInfo)property;
            if (val < min)
            {
                tmpproperty.SetValue(obj, min);
                return;
            }

            if (val > max)
            {
                tmpproperty.SetValue(obj, max);
                return;
            }
        }

    }
}
