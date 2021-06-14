// $Id:$
// Author: Valeriy Onuchin   29.12.2010

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

using Microsoft.Win32;


namespace P
{
    /// <summary>
    /// This wrapper class was created in order to simplify the code using event
    /// logs, as well as provide a central location for adding enhancements or debug code.
    /// </summary>
    /// <remarks>
    /// This class is meant to be overrided such that it is extremely easy to use throughout
    /// your entire application (or for a single assembly, depending on your app's design).
    /// See the example below.
    /// </remarks>
    /// <example>
    ///    internal class ArchiveServiceEventLog: P.PEventLog
    ///    {
    ///        /// <summary>
    ///        /// Name of the EventLog log to which all entries will be written
    ///        /// </summary>
    ///        private static string LOG_NAME = "Archive Service";
    ///
    ///        /// <summary>
    ///        /// An enumeration of the EventLog Sources.
    ///        /// </summary>
    ///        public enum Source
    ///        {
    ///            RtpRecorder,
    ///            StreamManager,
    ///            BufferManager,
    ///            DBHelper,
    ///            RtpPlayer,
    ///            StreamPlayer,
    ///            BufferPlayer
    ///        }
    ///
    ///        
    ///        /// <summary>
    ///        /// ID of the event log messages.
    ///        /// </summary>
    ///        public enum ID
    ///        {
    ///            BufferNotAvailable,
    ///            BadStreamInDB,
    ///            DBOpFailed,
    ///            IndiciesFailedToSave,
    ///            FrameDropped,
    ///            GrowingBuffers,
    ///            EmptyBuffersInPlayback,
    ///            ImproperPopulateCall,
    ///            BufferTooSmall,
    ///        }
    ///
    ///        public ArchiveServiceEventLog(Source source)
    ///            :base(LOG_NAME, ".", source.ToString())
    ///        { }
    ///
    ///        public void WriteEntry(string message, EventLogEntryType type, ID eventID)
    ///        {
    ///            base.WriteEntry(message, type, (int)eventID);
    ///        }
    ///
    ///        #region Install/Uninstall
    ///        /// <summary>
    ///        /// Installs the EventLog log and all the sources
    ///        /// </summary>
    ///        internal static void Install()
    ///        {
    ///             Install(LOG_NAME, Enum.GetNames(typeof(Source)));
    ///        }
    ///
    ///
    ///        /// <summary>
    ///        /// Uninstalls the EventLog log and all the sources
    ///        /// </summary>
    ///        internal static void Uninstall()
    ///        {
    ///            Uninstall(LOG_NAME, Enum.GetNames(typeof(Source)));
    ///        }
    ///        #endregion
    ///
    ///    }
    /// </example>
    [ComVisible(false)]
    public class PEventLog
    {
       private EventLog eventLog;

        // Static "defaults"
        static readonly bool TRACE_ON_ERROR = true;
        static readonly bool TRACE_ON_WARNING = true;
        readonly static bool TRACE_ON_INFO;
        static readonly long TICKS_BETWEEN_ENTRY = -1; // 1 second (in 100 nanosecond intervals)

        // Settings per instance
        private bool traceOnError;
        private bool traceOnWarning;
        private bool traceOnInfo;
        private long ticksBetweenEntry;

        // Properties for those instance settings
        public bool WriteTraceOnError { get { return traceOnError; } set { traceOnError = value; } }
        public bool WriteTraceOnWarning { get { return traceOnWarning; } set { traceOnWarning = value; } }
        public bool WriteTraceOnInfo { get { return traceOnInfo; } set { traceOnInfo = value; } }
        public long TickBetweenEachEntry { get { return ticksBetweenEntry; } set { ticksBetweenEntry = value; } }

        // We use a hashtable to prevent us from writing any given event to the eventLog more than a certain amt of time per second.
        //  This assists us from recovering under large loads (i.e. we don't want to make the problem worse).
        private System.Collections.Hashtable timeTable = new System.Collections.Hashtable();

        static PEventLog()
        {
            string settingsNamespace = "P.EventLog";
            System.Collections.Specialized.NameValueCollection config = System.Configuration.ConfigurationManager.AppSettings;
            string setting;

            setting = config[settingsNamespace + ".TraceOnError"];
            if (setting != null)
                TRACE_ON_ERROR = Boolean.Parse(setting);

            setting = config[settingsNamespace + ".TraceOnWarning"];
            if (setting != null)
                TRACE_ON_WARNING = Boolean.Parse(setting);

            setting = config[settingsNamespace + ".TraceOnInfo"];
            if (setting != null)
                TRACE_ON_INFO = Boolean.Parse(setting);

            setting = config[settingsNamespace + ".TicksBetweenEntry"];
            if (setting != null)
                TICKS_BETWEEN_ENTRY = Int64.Parse(setting, CultureInfo.InvariantCulture);
        }

        public PEventLog(string logName, string machineName, string source)
        {
            try
            {
                eventLog = new EventLog(logName, machineName, source);
            }
            catch
            {
                // Write to isolated storage, so we can track failed eventlog creation
            }
        }

        public virtual void WriteEntry(string message, EventLogEntryType type, int eventID)
        {
            // Make Trace more readable by doing this:
            if (message.Length > 80)
                message += '\n';

            if (type == EventLogEntryType.Error)
            {
                if (traceOnError)
                {
                    Trace.WriteLine("ERROR: " + message);
                }
            }
            else if (type == EventLogEntryType.Warning)
            {
                if (traceOnWarning)
                {
                    Trace.WriteLine("WARNING: " + message);
                }
            }
            else if (type == EventLogEntryType.Information)
            {
                if (traceOnInfo)
                {
                    Trace.WriteLine("INFO: " + message);
                }
            }

            long lastTime = (timeTable.ContainsKey(eventID)) ? (long)timeTable[eventID] : 0;
            if (lastTime < (DateTime.Now.Ticks - ticksBetweenEntry))
            {
                timeTable[eventID] = DateTime.Now.Ticks;

                try
                {
                    eventLog.WriteEntry(message, type, eventID);
                }
                catch
                {
                    // Write to isolated storage, so we can track failed eventlog writes
                }
            }
        }

        #region Install/Uninstall
        /// <summary>
        /// Installs the EventLog log and all the sources
        /// </summary>
        public static void Install(string logName, string[] sourceNames)
        {
            // Create sources (and log)
            foreach (string source in sourceNames)
            {
                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, logName);
                }
            }

            //Set the size & retention of the log
            string eventLogRegistryLoc = "SYSTEM\\CurrentControlSet\\Services\\EventLog\\" + logName;
            using (RegistryKey el = Registry.LocalMachine.CreateSubKey(eventLogRegistryLoc))
            {
                el.SetValue("MaxSize", 1024 * 1024); // 1 MB
                el.SetValue("Retention", 0);
            }
        }


        /// <summary>
        /// Uninstalls the EventLog log and all the sources
        /// </summary>
        public static void Uninstall(string logName, string[] sourceNames)
        {
            // Remove sources
            foreach (string source in sourceNames)
            {
                if (EventLog.SourceExists(source))
                {
                    EventLog.DeleteEventSource(source);
                }
            }

            // Remove log
            if (EventLog.Exists(logName))
            {
                EventLog.Delete(logName);
            }
        }
        #endregion

    }

}