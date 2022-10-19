using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class RangeAttribute : Attribute
    {
        private readonly int min;
        private readonly int max;

        public RangeAttribute(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public void Validate(object obj, object property, int? value)
        {
            if (value is null)
                throw new InvalidDataException("传入得无效得数值");

            PropertyInfo tmpproperty = (PropertyInfo)property;
            if (value < min)
            {
                tmpproperty.SetValue(obj, min);
                return;
            }

            if (value > max)
            {
                tmpproperty.SetValue(obj, max);
                return;
            }
        }

    }
}
