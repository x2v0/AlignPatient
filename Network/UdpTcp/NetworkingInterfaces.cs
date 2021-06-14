// $Id: NetworkingInterfaces.cs 6879 2019-02-05 06:05:57Z onuchin $
//
// Copyright (C) 2018 Valeriy Onuchin

//#define LOCAL_DEBUG

using System;
using System.Collections;
using System.Net;

namespace P.Net
{
   /// <summary>
   ///   Delegate ReceivedFromCallback
   /// </summary>
   /// <param name="bufferChunk">The buffer chunk.</param>
   /// <param name="endPont">The end pont.</param>
   public delegate void ReceivedFromCallback(BufferChunk bufferChunk, EndPoint endPont);

   /// <summary>
   ///   Interface INetworkListener
   /// </summary>
   public interface INetworkListener : IDisposable
   {
      /// <summary>
      ///   Receives the specified packet buffer.
      /// </summary>
      /// <param name="packetBuffer">The packet buffer.</param>
      void Receive(BufferChunk packetBuffer);

      /// <summary>
      ///   Receives from.
      /// </summary>
      /// <param name="packetBuffer">The packet buffer.</param>
      /// <param name="sender">The sender.</param>
      void ReceiveFrom(BufferChunk packetBuffer, out EndPoint sender);

      /// <summary>
      ///   Asyncs the receive from.
      /// </summary>
      /// <param name="queue">The queue.</param>
      /// <param name="callback">The callback.</param>
      void AsyncReceiveFrom(Queue queue, ReceivedFromCallback callback);

      /// <summary>
      ///   Gets the external interface.
      /// </summary>
      /// <value>The external interface.</value>
      IPAddress ExternalInterface
      {
         get;
      }

#if FaultInjection
        int DropPacketsReceivedPercent { get; set; }
#endif
   }

   /// <summary>
   ///   Interface INetworkSender
   /// </summary>
   public interface INetworkSender : IDisposable
   {
      /// <summary>
      ///   Sends the specified packet buffer.
      /// </summary>
      /// <param name="packetBuffer">The packet buffer.</param>
      void Send(BufferChunk packetBuffer);

      /// <summary>
      ///   Gets the external interface.
      /// </summary>
      /// <value>The external interface.</value>
      IPAddress ExternalInterface
      {
         get;
      }

      /// <summary>
      ///   Gets or sets the delay between packets.
      /// </summary>
      /// <value>The delay between packets.</value>
      short DelayBetweenPackets
      {
         get;
         set;
      }

      /// <summary>
      ///   Disables the loopback.
      /// </summary>
      void DisableLoopback();

#if FaultInjection
        int DropPacketsSentPercent { get; set; }
#endif
   }
}
