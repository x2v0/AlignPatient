// $Id: AppConfig.cs 341 2012-01-25 13:58:52Z onuchin $

using System;


namespace P.Net.Rtp
{
    internal class AppConfig
    {
        private const string RTP = "P.Rtp.";

        public const string RTP_TimeToLive  = RTP + "TimeToLive";

        private const string CD = "P.ConnectivityDetector.";

        public const string CD_UpdateIntervalSeconds = CD + "UpdateIntervalSeconds";
        public const string CD_IPAddress = CD + "IPAddress";
        public const string CD_Port = CD + "Port";
    }
}