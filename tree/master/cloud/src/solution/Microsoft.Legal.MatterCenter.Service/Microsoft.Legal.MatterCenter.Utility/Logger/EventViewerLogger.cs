using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Utility
{
    public class EventViewerLogger
    {
        /// <summary>
        ///  Logs exception in Event Viewer.
        /// </summary>
        /// <param name="eventLog"> Event description </param>
        /// <param name="type"> Type of log </param>
        internal static void LogInEventViewer(string eventLog, string type, LogTables logTables)
        {
            string sourceName = logTables.EventViewerSource;
            int eventID;
            if (!int.TryParse(logTables.EventViewerEventID, NumberStyles.Any, CultureInfo.InvariantCulture, out eventID))
            {
                eventID = ServiceConstants.DEFAULT_EVENT_ID;
            }

            // Create new event source if not exists
            if (!EventLog.SourceExists(sourceName))
            {
                EventLog.CreateEventSource(sourceName, logTables.EventViewerLogName);
            }

            switch (type)
            {
                case ServiceConstants.EVENT_WARNING:
                    EventLog.WriteEntry(sourceName, eventLog, EventLogEntryType.Warning, eventID);
                    break;
                case ServiceConstants.EVENT_ERROR:
                    EventLog.WriteEntry(sourceName, eventLog, EventLogEntryType.Error, eventID);
                    break;
                default:
                    EventLog.WriteEntry(sourceName, eventLog, EventLogEntryType.Information, eventID);
                    break;
            }
        }
    }
}
