using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.RestServer.Attributes
{
    internal class ContainedAttribute : BaseAttribute
    {
        private readonly string[] content;
        public ContainedAttribute(string[] content)
        {
            this.content = content;
        }

        public override bool Validate(object obj, object value)
        {
            return value is not null && content.Contains(value);
        }
    }
}
