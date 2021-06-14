// $Id: UdpReflectorJoiner.cs 6879 2019-02-05 06:05:57Z onuchin $
// Author: Valeriy Onuchin   29.12.2010

using System.Diagnostics;
using System.Net;
using System.Threading;

namespace P.Net
{
   public class UdpReflectorJoiner
   {
      #region Constants

      //private readonly int REFLECTOR_JOIN_ATTEMPTS = 4;
      private const int JOIN_MESSAGE_DELAY = 2000; // ms

      #endregion

      #region Constructors and destructors

      public UdpReflectorJoiner(IPEndPoint reflectorEP, IPEndPoint multicastEP)
      {
         this.multicastEP = multicastEP;
         this.reflectorEP = reflectorEP;
      }

      ~UdpReflectorJoiner()
      {
         alive = false;
      }

      #endregion

      #region  Fields

      private readonly IPEndPoint multicastEP;

      private readonly IPEndPoint reflectorEP;

      private volatile bool alive = true;

      #endregion

      #region Public methods

      public void Start()
      {
         var thread = new Thread(SendJoinMessages);
         thread.IsBackground = true;
         thread.Start();
      }

      //public UdpReflectorJoiner(IPEndPoint reflectorEP, IPEndPoint multicastEP,Socket socket)
      //{
      //    this.multicastEP = multicastEP;
      //    this.reflectorEP = reflectorEP;
      //    this.socket = socket;
      //}

      public void Terminate()
      {
         alive = false;
      }

      #endregion

      #region Private methods

      /// <summary>
      ///   Send  UDP join messages to the reflector.  There is no acknoweledgement, so we send
      ///   a series of these messages, pausing briefly in between.
      ///   A bit of ugliness: the joiner works over both C#'s built in socket, as well as
      ///   UDPSender.  If the class was initialized without a Socket, then we create a locale UdpSender.
      ///   This is necessary because the client uses UdpSender, whereas the reflector uses raw sockets.
      /// </summary>
      private void SendJoinMessages()
      {
         Debug.Assert(reflectorEP != null);
         Debug.Assert(multicastEP != null);

         UdpSender sender = null;

         try {
            sender = new UdpSender(reflectorEP, 64);
            sender.DisableLoopback();

            while (alive) {
               var rjm = new UdpReflectorMessage(UdpReflectorMessageType.JOIN, multicastEP);
               var bufferChunk = rjm.ToBufferChunk();


               // UdpSender, as used by Client
               sender.Send(bufferChunk);

               Thread.Sleep(JOIN_MESSAGE_DELAY);
            }
         } catch {
         } finally {
            if (sender != null) {
               sender.Dispose();
            }
         }
      }

      #endregion
   }
}
