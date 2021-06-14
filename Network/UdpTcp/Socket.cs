// $Id: Socket.cs 6879 2019-02-05 06:05:57Z onuchin $
//
// Copyright (C) 2018 Valeriy Onuchin

//#define LOCAL_DEBUG

using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using MSR.LST.Net;
using PSocks = P.Net.Sockets;


namespace P.Net.Sockets
{
   //Pri1: Review for Exception Handling & Event Logging

   /// <summary>
   ///   P.Net.Sockets.Socket is a helper class that inherits from System.Net.Sockets.Socket and adds support for sending and
   ///   receiving BufferChunks.
   /// </summary>
   [ComVisible(false)]
   public class Socket : System.Net.Sockets.Socket
   {
      #region Static fields

      /// <summary>
      ///   The socks
      /// </summary>
      private static readonly Hashtable socks = Hashtable.Synchronized(new Hashtable());

      #endregion

      #region Constructors and destructors

      /// <summary>
      ///   Standard Socket constructor.  Calls the base System.Net.Sockets.Socket constructor.
      /// </summary>
      /// <param name="addressFamily">AddressFamily</param>
      /// <param name="socketType">SocketType</param>
      /// <param name="protocolType">ProtocolType</param>
      public Socket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) : 
            base(addressFamily, socketType, protocolType)
      {
      }

      #endregion

      #region  Fields

      /// <summary>
      ///   The reference count
      /// </summary>
      private int refCount;

      #endregion

      #region Public methods

      /// <summary>
      ///   Begins the receive from.
      /// </summary>
      /// <param name="bufferChunk">The buffer chunk.</param>
      /// <param name="endPoint">The end point.</param>
      /// <param name="callback">The callback.</param>
      /// <param name="state">The state.</param>
      /// <returns>IAsyncResult.</returns>
      public IAsyncResult BeginReceiveFrom(BufferChunk bufferChunk, ref EndPoint endPoint, AsyncCallback callback, object state)
      {
         return BeginReceiveFrom(bufferChunk.Buffer, 0, bufferChunk.Buffer.Length, SocketFlags.None, ref endPoint, callback, state);
      }

      /// <summary>
      ///   Calls the base class Receive.  Performs an efficient conversion from BufferChunk.
      /// </summary>
      /// <param name="bufferChunk">BufferChunk</param>
      /// <param name="socketFlags">SocketFlags</param>
      /// <example>
      ///   Socket sock = new P.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      ///   sock.Receive(bufferChunk, SocketFlags.None);
      /// </example>
      public void Receive(BufferChunk bufferChunk, SocketFlags socketFlags)
      {
         bufferChunk.Length = base.Receive(bufferChunk.Buffer, 0, bufferChunk.Buffer.Length, socketFlags);
      }

      /// <summary>
      ///   Calls the base class Receive.  Performs an efficient conversion from BufferChunk.
      /// </summary>
      /// <param name="bufferChunk">BufferChunk</param>
      /// <example>
      ///   Socket sock = new P.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      ///   sock.Receive(bufferChunk);
      /// </example>
      public void Receive(BufferChunk bufferChunk)
      {
         Receive(bufferChunk, SocketFlags.None);
      }

      /// <summary>
      ///   Calls the base class ReceiveFrom.  Performs an efficient conversion from BufferChunk.
      /// </summary>
      /// <param name="bufferChunk">BufferChunk</param>
      /// <param name="endPoint">ref EndPoint</param>
      /// <example>
      ///   Socket sock = new P.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      ///   sock.ReceiveFrom(bufferChunk, ref endPoint);
      /// </example>
      public void ReceiveFrom(BufferChunk bufferChunk, ref EndPoint endPoint)
      {
         bufferChunk.Length = base.ReceiveFrom(bufferChunk.Buffer, 0, bufferChunk.Buffer.Length, SocketFlags.None, ref endPoint);
      }

      /// <summary>
      ///   Calls the base class Send.  Performs an efficient conversion from BufferChunk.
      /// </summary>
      /// <param name="bufferChunk">BufferChunk to send.</param>
      /// <returns>int bytes sent</returns>
      /// <example>
      ///   Socket sock = new P.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      ///   sock.Send(bufferChunk);
      /// </example>
      public int Send(BufferChunk bufferChunk)
      {
         return Send(bufferChunk, SocketFlags.None);
      }

      /// <summary>
      ///   Calls the base class Send.  Performs an efficient conversion from BufferChunk.
      /// </summary>
      /// <param name="bufferChunk">BufferChunk to send</param>
      /// <param name="socketFlags">SocketFlags</param>
      /// <returns>int bytes sent</returns>
      /// <example>
      ///   Socket sock = new P.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      ///   sock.Send(bufferChunk, SocketFlags.None);
      /// </example>
      public int Send(BufferChunk bufferChunk, SocketFlags socketFlags)
      {
         return Send(bufferChunk.Buffer, bufferChunk.Index, bufferChunk.Length, socketFlags);
      }

      /// <summary>
      ///   Calls the base class SendTo.  Performs an efficient conversion from BufferChunk.
      /// </summary>
      /// <param name="bufferChunk">BufferChunk</param>
      /// <param name="endPoint">EndPoint</param>
      /// <returns>int bytes sent</returns>
      /// <example>
      ///   Socket sock = new P.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      ///   sock.SendTo(bufferChunk, endPoint);
      /// </example>
      public int SendTo(BufferChunk bufferChunk, EndPoint endPoint)
      {
         return SendTo(bufferChunk.Buffer, bufferChunk.Index, bufferChunk.Length, SocketFlags.None, endPoint);
      }

      #endregion

      #region Internal methods

      // Apparently binding to the same ports on UDPSender and UDPListener causes problems in unicast.
      // Sharing the socket though, allows us to tunnel through firewalls as data is sent and received
      // on the same endpoint.
      // This region of code enables sharing sockets between the two classes.

      /// <summary>
      ///   Gets the shared socket.
      /// </summary>
      /// <param name="endPoint">The end point.</param>
      /// <returns>SockInterfacePair.</returns>
      /// <exception cref="System.Exception"></exception>
      internal static SockInterfacePair GetSharedSocket(IPEndPoint endPoint)
      {
         lock (socks) {
            var sockObj = socks[endPoint];
            if (sockObj != null) {
               var sip = (SockInterfacePair) sockObj;
               ++sip.sock.refCount;
               return sip;
            } else {
               // Create the socket
               var sock = new Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

               // Get the External Interface, save it for future use
               var externalInterface = Utility.GetLocalRoutingInterface(endPoint.Address);

               if (externalInterface == null) {
                  // Do something more helpful here
                  throw new Exception(Strings.UnableToFindLocalRoutingInterface);
               }

               if (Utility.IsMulticast(endPoint.Address)) {
                  // Allow multiple binds to this socket, as it will still function properly
                  //  (this is only the case if it is a multicast socket.  Unicast sockets fail to
                  //   receive all data on all sockets)
                  sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, -1);

                  // We don't join the multicast group here, because we may not want to listen
                  // to our own data (halfing our throughput).
               }

               // Add the socket to the hashtable
               var sip = new SockInterfacePair();
               sip.sock = sock;
               sip.extInterface = externalInterface;
               socks.Add(endPoint, sip);

               // Increase the socket's reference count
               ++sock.refCount;

               return sip;
            }
         }
      }

      /// <summary>
      ///   Releases the shared socket.
      /// </summary>
      /// <param name="endPoint">The end point.</param>
      /// <param name="sock">The sock.</param>
      /// <exception cref="System.InvalidOperationException"></exception>
      internal static void ReleaseSharedSocket(IPEndPoint endPoint, Socket sock)
      {
         var sockObj = socks[endPoint];
         if (sockObj == null) {
            throw new InvalidOperationException(Strings.SockDoesNotExistAsASharedSocket);
         }

         lock (socks) {
            if (--sock.refCount <= 0) {
               // Leave the multicast group
               if (Utility.IsMulticast(endPoint.Address)) {
                  try {
                     if (endPoint.AddressFamily == AddressFamily.InterNetworkV6) {
                        var mo = new IPv6MulticastOption(endPoint.Address);
                        sock.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.DropMembership, mo);
                     } else {
                        var mo = new MulticastOption(endPoint.Address);
                        sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, mo);
                     }
                  } catch {
                  } // The user of the socket *may* not have joined the multicast group (?)
               }

               // Remove ourselves from the shared pool
               socks.Remove(endPoint);

               // Close the socket
               try {
                  sock.Close();
               } catch (ObjectDisposedException) {
               }
            }
         }
      }

      #endregion

      #region Nested classes

      /// <summary>
      ///   Class SockInterfacePair
      /// </summary>
      internal class SockInterfacePair
      {
         #region  Fields

         /// <summary>
         ///   The initialized
         /// </summary>
         public bool Initialized;

         /// <summary>
         ///   The ext interface
         /// </summary>
         internal IPAddress extInterface;

         /// <summary>
         ///   The sock
         /// </summary>
         internal Socket sock;

         #endregion
      }

      #endregion
   }
}
