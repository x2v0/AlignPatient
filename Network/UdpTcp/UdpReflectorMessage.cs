// $Id: UdpReflectorMessage.cs 6297 2018-06-18 10:53:59Z onuchin $
//
// Copyright (C) 2018 Valeriy Onuchin

//#define LOCAL_DEBUG


using System;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace P.Net
{
   /// <summary>
   ///   Enum UdpReflectorMessageType
   /// </summary>
   public enum UdpReflectorMessageType
   {
      JOIN,
      LEAVE,
      PING,
      PING_REPLY
   }

   /// <summary>
   ///   Class InvalidUdpReflectorMessage
   /// </summary>
   public class InvalidUdpReflectorMessage : Exception
   {
   }

   /// <summary>
   ///   Class UdpReflectorMessage
   /// </summary>
   public class UdpReflectorMessage
   {
      #region Static fields

      /// <summary>
      ///   The header line
      /// </summary>
      private static readonly string headerLine = "P.Net 1.0";

      /// <summary>
      ///   The UTF8
      /// </summary>
      private static readonly UTF8Encoding utf8 = new UTF8Encoding();

      #endregion

      #region Constructors and destructors

      // ping messages don't need a valid ip address...
      /// <summary>
      ///   Initializes a new instance of the <see cref="UdpReflectorMessage" /> class.
      /// </summary>
      /// <param name="type">The type.</param>
      public UdpReflectorMessage(UdpReflectorMessageType type)
      {
         this.type = type;
         multicastEP = new IPEndPoint(IPAddress.Loopback, 1);
      }

      // outbound reflector messages
      /// <summary>
      ///   Initializes a new instance of the <see cref="UdpReflectorMessage" /> class.
      /// </summary>
      /// <param name="type">The type.</param>
      /// <param name="multicastEP">The multicast EP.</param>
      public UdpReflectorMessage(UdpReflectorMessageType type, IPEndPoint multicastEP)
      {
         this.type = type;
         this.multicastEP = multicastEP;
      }

      // inbound data (from the network)
      // throws InvalidUdpReflectorMessage if this is not a valid control message
      /// <summary>
      ///   Initializes a new instance of the <see cref="UdpReflectorMessage" /> class.
      /// </summary>
      /// <param name="buffer">The buffer.</param>
      /// <param name="count">The count.</param>
      /// <exception cref="InvalidUdpReflectorMessage">
      /// </exception>
      public UdpReflectorMessage(byte[] buffer, int count)
      {
         if (count > 50) {
            throw new InvalidUdpReflectorMessage();
         }
         var str = utf8.GetString(buffer, 0, count);
         var lines = str.Split(new[] {'\n'}, 2);
         if (lines.Length < 2) {
            throw new InvalidUdpReflectorMessage();
         }
         if (!lines[0].Equals(headerLine)) {
            throw new InvalidUdpReflectorMessage();
         }

         var toks = lines[1].Split(new[] {':'}, 3);
         if (toks.Length < 3) {
            throw new InvalidUdpReflectorMessage();
         }


         if (toks[0].Trim().Equals("JOIN", StringComparison.InvariantCultureIgnoreCase)) {
            type = UdpReflectorMessageType.JOIN;
         } else if (toks[0].Equals("LEAVE", StringComparison.InvariantCultureIgnoreCase)) {
            type = UdpReflectorMessageType.LEAVE;
         } else if (toks[0].Equals("PING", StringComparison.InvariantCultureIgnoreCase)) {
            type = UdpReflectorMessageType.PING;
         } else if (toks[0].Equals("PING_REPLY", StringComparison.InvariantCultureIgnoreCase)) {
            type = UdpReflectorMessageType.PING_REPLY;
         } else {
            throw new InvalidUdpReflectorMessage();
         }

         try {
            var addr = IPAddress.Parse(toks[1].Trim());
            var port = int.Parse(toks[2].Trim());

            multicastEP = new IPEndPoint(addr, port);
         } catch (Exception) {
            throw new InvalidUdpReflectorMessage();
         }
      }

      #endregion

      #region  Fields

      /// <summary>
      ///   The multicast EP
      /// </summary>
      private readonly IPEndPoint multicastEP;

      /// <summary>
      ///   The type
      /// </summary>
      private readonly UdpReflectorMessageType type;

      #endregion

      #region Public properties

      /// <summary>
      ///   Gets the multicast EP.
      /// </summary>
      /// <value>The multicast EP.</value>
      public IPEndPoint MulticastEP
      {
         get
         {
            return multicastEP;
         }
      }

      /// <summary>
      ///   Gets the type.
      /// </summary>
      /// <value>The type.</value>
      public UdpReflectorMessageType Type
      {
         get
         {
            return type;
         }
      }

      #endregion

      #region Public methods

      /// <summary>
      ///   To the buffer chunk.
      /// </summary>
      /// <returns>BufferChunk.</returns>
      public BufferChunk ToBufferChunk()
      {
         var builder = new StringBuilder();
         builder.Append(headerLine + "\n");

         if (type == UdpReflectorMessageType.JOIN) {
            builder.Append("JOIN: ");
         } else if (type == UdpReflectorMessageType.LEAVE) {
            builder.Append("LEAVE: ");
         } else if (type == UdpReflectorMessageType.PING) {
            builder.Append("PING: ");
         } else if (type == UdpReflectorMessageType.PING_REPLY) {
            builder.Append("PING_REPLY: ");
         } else {
            Debug.Assert(false);
         }

         builder.Append(multicastEP.Address + ":" + multicastEP.Port + "\n");

         var buffer = utf8.GetBytes(builder.ToString());
         return new BufferChunk(buffer);
      }

      #endregion
   }
}
