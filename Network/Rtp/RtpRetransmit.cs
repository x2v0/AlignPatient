// $Id:$
// Author: Valeriy Onuchin   29.12.2010

using System;


namespace P.Net.Rtp
{
    /// <summary>
    /// Summary description for RtpRetransmit.
    /// </summary>
    internal class RtpRetransmit
    {
        public static void FrameIncomplete(RtpStream rtpStream, int framesLost)
        {
            // Event logging and perf counting are done in called method
            rtpStream.RaiseFrameOutOfSequenceEvent(framesLost, Strings.IncompleteFrameReceived);
        }
    }
}
