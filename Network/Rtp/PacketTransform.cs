// $Id: $
// Author: Valeriy Onuchin  29.12.2010

using System;


namespace P.Net.Rtp
{
    /// <summary>
    /// This interface defines functionality for modifying packets during send and receive,
    /// respectively.  This interface was added for encryption support, but any type
    /// of packet transformation is possible.
    /// </summary>
    public interface PacketTransform
    {
        void Encode(RtpPacket packet);
        void Decode(RtpPacket packet);
    }
}
