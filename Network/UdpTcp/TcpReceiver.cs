// $Id: TcpReceiver.cs 6879 2019-02-05 06:05:57Z onuchin $
//
// Copyright (C) 2018 Valeriy Onuchin

//#define LOCAL_DEBUG


using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using MSR.LST.Net;

//using System.Linq;

namespace P.Net
{

   #region Event stuff

   /// <summary>
   ///   Delegate OnFileReceivedDelegate
   /// </summary>
   /// <param name="sender">The sender.</param>
   /// <param name="e">The e.</param>
   public delegate void OnFileReceivedDelegate(object sender, OnFileReceivedArgs e);

   /// <summary>
   ///   Class OnFileReceivedArgs
   /// </summary>
   public class OnFileReceivedArgs : EventArgs
   {
      #region Constructors and destructors

      /// <summary>
      ///   Initializes a new instance of the <see cref="OnFileReceivedArgs" /> class.
      /// </summary>
      /// <param name="buffer">The buffer.</param>
      public OnFileReceivedArgs(byte[] buffer) : this()
      {
         fBuffer = buffer;
      }

      /// <summary>
      ///   Prevents a default instance of the <see cref="OnFileReceivedArgs" /> class from being created.
      /// </summary>
      private OnFileReceivedArgs()
      {
      }

      #endregion

      #region  Fields

      /// <summary>
      ///   The buffer
      /// </summary>
      private readonly byte[] fBuffer;

      #endregion

      #region Public properties

      /// <summary>
      ///   Gets the buffer.
      /// </summary>
      /// <value>The buffer.</value>
      public byte[] Buffer
      {
         get
         {
            return fBuffer;
         }
      }

      /// <summary>
      ///   Gets the length.
      /// </summary>
      /// <value>The length.</value>
      public int Length
      {
         get
         {
            return fBuffer.Length;
         }
      }

      #endregion
   }

   #endregion

   /// <summary>
   ///   Class TcpReceiver
   /// </summary>
   public class TcpReceiver : IDisposable
   {
      #region Constants

      /// <summary>
      ///   The DEFAULT SERVER PORT
      /// </summary>
      public const int DEFAULT_SERVER_PORT = 30043;

      #endregion

      #region Constructors and destructors

      /// <summary>
      ///   Initializes a new instance of the <see cref="TcpReceiver" /> class.
      /// </summary>
      /// <param name="port">The port.</param>
      public TcpReceiver(int port)
      {
         fConnections = new List<WeakReference>();
         fListener = new TcpListener(IPAddress.Any, port);
      }

      /// <summary>
      ///   Prevents a default instance of the <see cref="TcpReceiver" /> class from being created.
      /// </summary>
      private TcpReceiver() : this(DEFAULT_SERVER_PORT)
      {
      }

      #endregion

      #region  Fields

      /// <summary>
      ///   The list of connections
      /// </summary>
      private readonly List<WeakReference> fConnections;

      //dont want our handy connections list to stop garbage collection

      /// <summary>
      ///   The listener
      /// </summary>
      private TcpListener fListener;

      #endregion

      #region Public events

      /// <summary>
      ///   Occurs when [on file received].
      /// </summary>
      public event OnFileReceivedDelegate OnFileReceived;

      #endregion

      #region Public properties

      /// <summary>
      ///   Gets the connection count.
      /// </summary>
      /// <value>The connection count.</value>
      public int ConnectionCount
      {
         get
         {
            lock (fConnections) {
               return fConnections.Count;
            }
         }
      }

      #endregion

      #region Interface methods

      /// <summary>
      ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      public void Dispose()
      {
         if (fListener != null) {
            fListener.Stop();
            fListener = null;
         }
         KillRunningThreads();
      }

      #endregion

      #region Public methods

      /// <summary>
      ///   Starts this instance.
      /// </summary>
      public void Start()
      {
         fListener.Start();
         fListener.BeginAcceptSocket(ConnectionCallback, fListener);
      }

      /// <summary>
      ///   Stops this instance.
      /// </summary>
      public void Stop()
      {
         fListener.Stop();
         KillRunningThreads();
      }

      #endregion

      #region Private methods

      /// <summary>
      ///   Sockets the connected.
      /// </summary>
      /// <param name="s">The s.</param>
      /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
      private static bool SocketConnected(Socket s)
      {
         return !(s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0));
      }

      /// <summary>
      ///   Connections the callback.
      /// </summary>
      /// <param name="ar">The ar.</param>
      private void ConnectionCallback(IAsyncResult ar)
      {
         var fListener = (TcpListener) ar.AsyncState;
         try {
            var s = fListener.EndAcceptSocket(ar);
            new Func<Socket, byte[]>(HandleSocketComms).BeginInvoke(s, HandleSocketCommsCallback, s);
         } catch {
            //You should handle this but this should be a _rare_ error
            throw;
         } finally {
            //Prime up TheListener to accept another inbound request
            fListener.BeginAcceptSocket(ConnectionCallback, fListener);
         }
      }

      /// <summary>
      ///   Gets the running threads.
      /// </summary>
      /// <returns>Thread[].</returns>
      private Thread[] GetRunningThreads()
      {
         lock (fConnections) {
            var result = new List<Thread>();

            foreach (var wr in fConnections) {
               if (wr.IsAlive) {
                  result.Add((Thread) wr.Target);
               }
            }

            return result.ToArray();
         }
      }

      /// <summary>
      ///   This basically blocks until the socket closes and we have
      ///   received all of the data
      /// </summary>
      /// <param name="s">The s.</param>
      /// <returns>System.Byte[][].</returns>
      private byte[] HandleSocketComms(Socket s)
      {
         //Dont do any exception handling here
         TrackThread();

         using (var ms = new MemoryStream()) //This may be a bad idea for big files :)
         {
            do {
               var szToRead = s.Available;
               var buffer = new byte[szToRead];

               var szRead = s.Receive(buffer, szToRead, SocketFlags.None);
               if (szRead > 0) {
                  ms.Write(buffer, 0, szRead);
                  Console.WriteLine(Strings.ReadData + szRead);
               }
            } while (SocketConnected(s));

            return ms.ToArray();
         }
         //return result;
      }

      /// <summary>
      ///   Handles the socket comms callback.
      /// </summary>
      /// <param name="ar">The ar.</param>
      private void HandleSocketCommsCallback(IAsyncResult ar)
      {
         byte[] file = null;
         var error = false;

         try {
            var result = (AsyncResult) ar;
            var del = (Func<Socket, byte[]>) result.AsyncDelegate;
            file = del.EndInvoke(ar);

            var s = ar.AsyncState as Socket;
            if (s != null) {
               //Sockets throw exceptions++ when disposed
               try {
                  s.Close(0);
                  ((IDisposable) s).Dispose();
               } catch {
               }
            }
         } catch (ThreadAbortException) { //When we stop our server
            //We were killed! Just let it die out
            error = true;
         } catch {
            //Might want to handle this
            error = true;
         } finally {
            RemoveThread(); //conn is closed at this point

            if (!error &&
                (file != null) &&
                (file.Length > 0)) {
               var del = OnFileReceived;

               if (del != null) {
                  del(this, new OnFileReceivedArgs(file));
               }
            }
         }
      }

      /// <summary>
      ///   Kills the running threads.
      /// </summary>
      private void KillRunningThreads()
      {
         var threads = GetRunningThreads();

         foreach (var t in threads) {
            try {
               t.Abort();
            } catch {
            }
         }
      }

      /// <summary>
      ///   Removes the thread.
      /// </summary>
      private void RemoveThread()
      {
         var t = Thread.CurrentThread;
         lock (fConnections) {
            for (var i1 = 0; i1 < fConnections.Count; i1++) {
               if (fConnections[i1].Target as Thread == t) {
                  fConnections.RemoveAt(i1);
                  break;
               }
            }
         }
      }

      /// <summary>
      ///   Tracks the thread.
      /// </summary>
      private void TrackThread()
      {
         lock (fConnections) {
            fConnections.Add(new WeakReference(Thread.CurrentThread, false));
         }
      }

      #endregion
   }
}
