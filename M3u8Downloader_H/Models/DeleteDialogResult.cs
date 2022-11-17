using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Models
{
    public class DeleteDialogResult
    {
        public bool DialogResult { get; }
        public bool IsDeleteCache { get; }

        public DeleteDialogResult(bool result,bool isdeletecache)
        {
            DialogResult = result;
            IsDeleteCache = isdeletecache;
        }
    }
}
