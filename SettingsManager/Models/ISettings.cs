using System;
using System.Collections.Generic;

namespace M3u8Downloader_H.Settings.Models
{
    public interface ISettings
    {
        int MaxThreadCount { get; }
        int RetryCount { get;  }       
        string SelectedFormat { get;  }                
        bool ForcedMerger { get;  }
        bool IsCleanUp { get;  }         
        bool SkipRequestError { get;  }
        Dictionary<string,string> Headers { get;  } 
        double RecordDuration { get;  }
        TimeSpan Timeouts { get;  }     
    }
}
