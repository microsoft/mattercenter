using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Utility
{
    public class LogTables
    {
        public string ServiceLogTable { get; set; }
        public string SPOLogTable { get; set; }
        public string EventViewerSource { get; set; }
        public string EventViewerEventID { get; set; }
        public string EventViewerLogName { get; set; }
        
        public string AzureRowKeyDateFormat { get; set; }
        public bool IsLoggingOnAzure { get; set; }
        public string ExternalAccessRequests { get; set; }
        public string MatterCenterConfiguration { get; set; }
        public string MatterRequests { get; set; }
    }
}
