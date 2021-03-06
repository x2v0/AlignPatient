// $Id: EventLog.cs 1586 2013-01-15 08:53:52Z onuchin $
// Author: Valeriy Onuchin   29.12.2011

using System;
using System.Diagnostics;

//#define LOCAL_DEBUG

namespace P.Net.Rtp
{
    /// <summary>
    /// Rtp EventLog helper class
    /// 
    /// EL was preferred over EventLog because it is shorter
    /// </summary>
    internal class RtpEL: PEventLog
    {
        /// <summary>
        /// Name of the EventLog log to which all entries will be written
        /// </summary>
        private static string LOG_NAME = "Rtp";

        /// <summary>
        /// An enumeration of the EventLog Sources for Rtp
        /// 
        /// Count is only meant to be used to determine the length of the enum
        /// </summary>
        public enum Source
        {
            ConnectivityDetector,
            Events,
            EventThrower,
            RtpSender,
            RtpSession,
            RtpStream,
            Reflector
        }

        
        /// <summary>
        /// ID of the event log messages in Rtp
        /// </summary>
        public enum ID
        {
            Error,
        
            RtpParticipantAdded,
            RtpStreamAdded,

            RtpStreamTimeout,
            RtpParticipantTimeout,

            PacketOutOfSequence,
            FrameOutOfSequence,
            InvalidPacket,
            InvalidPacketInFrame,
            NetworkTimeout,
            FrameReceived,
            DuplicatePacketReceived,

            AppPacketReceived,
            UnhandledRtcpType,
            DuplicateCNameEvent,
            ParticipantStatusChanged,
            SSRCConflictDetected,
            HandleCNameConflict,

            GetSharedListener,
            UnknownPayloadType,
            SendRtcpBYE,
            SEQToleranceExceeded,
            RtpStreamReceiverReport,
            RtcpHeaderException,
            CompoundPacketException,
            UnknownStream,
            RemoveStreamAttack,
            PerformanceCounterTooLong,
            EventNotHooked,
            GrowPool,
            UnboundedGrowth,

            // These have to be at the end
            RtpParticipantRemoved = 100 + RtpParticipantAdded,
            RtpStreamRemoved = 100 + RtpStreamAdded,

            ReflectorJoinFailed,
            ReflectorLeaveFailed,
            DiagnosticsError
        }

        public RtpEL(Source source)
            :base(LOG_NAME, ".", source.ToString())
        { }

        public void WriteEntry(string message, EventLogEntryType type, ID eventID)
        {
#if LOCAL_DEBUG
            base.WriteEntry(message, type, (int)eventID);
#endif
        }

        /// <summary>
        /// Installs the EventLog log and all the sources
        /// </summary>
        public static void Install()
        {
            Install(LOG_NAME, Enum.GetNames(typeof(Source)));
        }


        /// <summary>
        /// Uninstalls the EventLog log and all the sources
        /// </summary>
        public static void Uninstall()
        {
            Uninstall(LOG_NAME, Enum.GetNames(typeof(Source)));
        }

    }

}
