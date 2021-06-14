// $Id: TcpSender.cs 6296 2018-06-18 10:50:13Z onuchin $
//
// Copyright (C) 2018 Valeriy Onuchin

//#define LOCAL_DEBUG

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
//using System.Linq;

namespace P.Net
{
   /// <summary>
   /// Delegate OnSendFileCompleteDelegate
   /// </summary>
   /// <param name="sender">The sender.</param>
   /// <param name="e">The <see cref="SendFileCompleteEventArgs"/> instance containing the event data.</param>
   public delegate void OnSendFileCompleteDelegate(object sender, SendFileCompleteEventArgs e);

   /// <summary>
   /// Class SendFileCompleteEventArgs
   /// </summary>
   public class SendFileCompleteEventArgs : EventArgs
   {
      /// <summary>
      /// The file name
      /// </summary>
      private string fFileName;

      /// <summary>
      /// The date started
      /// </summary>
      private DateTime fDateStarted;

      /// <summary>
      /// The date completed
      /// </summary>
      private DateTime fDateCompleted;

      /// <summary>
      /// Gets the name of the file.
      /// </summary>
      /// <value>The name of the file.</value>
      public string FileName { get { return fFileName; } }

      /// <summary>
      /// Gets the started.
      /// </summary>
      /// <value>The started.</value>
      public DateTime Started
      {
         get { return fDateStarted; }
         internal set { fDateStarted = value; }
      }

      /// <summary>
      /// Gets the completed.
      /// </summary>
      /// <value>The completed.</value>
      public DateTime Completed
      {
         get { return fDateCompleted; }
         internal set { fDateCompleted = value; }
      }

      /// <summary>
      /// Gets the time taken.
      /// </summary>
      /// <value>The time taken.</value>
      public TimeSpan TimeTaken
      {
         get {
            return Completed.Subtract(Started);
         }
      }

      /// <summary>
      /// Prevents a default instance of the <see cref="SendFileCompleteEventArgs"/> class from being created.
      /// </summary>
      private SendFileCompleteEventArgs() : this(string.Empty)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="SendFileCompleteEventArgs"/> class.
      /// </summary>
      /// <param name="fileName">Name of the file.</param>
      public SendFileCompleteEventArgs(string fileName)
      {
         fFileName = fileName;
      }
   }

   /// <summary>
   /// Class TcpSender
   /// </summary>
   public class TcpSender : IDisposable
   {
      /// <summary>
      /// The DEFAULT SERVER PORT
      /// </summary>
      public const int DEFAULT_SERVER_PORT = 30043;
      /// <summary>
      /// The end point
      /// </summary>
      private IPEndPoint endPoint;

      /// <summary>
      /// Occurs when [on send file complete].
      /// </summary>
      public event OnSendFileCompleteDelegate OnSendFileComplete;

      /// <summary>
      /// Prevents a default instance of the <see cref="TcpSender"/> class from being created.
      /// </summary>
      private TcpSender()
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="TcpSender"/> class.
      /// </summary>
      /// <param name="IP">The IP.</param>
      /// <param name="port">The port.</param>
      public TcpSender(string IP, int port) : this(IPAddress.Parse(IP), port)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="TcpSender"/> class.
      /// </summary>
      /// <param name="IP">The IP.</param>
      /// <param name="port">The port.</param>
      public TcpSender(IPAddress IP, int port) : this(new IPEndPoint(IP, port))
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="TcpSender"/> class.
      /// </summary>
      /// <param name="EndPoint">The end point.</param>
      /// <exception cref="System.ArgumentNullException">EndPoint</exception>
      public TcpSender(IPEndPoint EndPoint) : this()
      {
         if (EndPoint == null) {
            throw new ArgumentNullException("EndPoint");
         }

         endPoint = EndPoint;
      }

      /// <summary>
      /// Sends the file.
      /// </summary>
      /// <param name="fileName">Name of the file.</param>
      public void SendFile(string fileName)
      {
         var func = new Func<string, SendFileCompleteEventArgs>(SendFileWorker);
         func.BeginInvoke(fileName, SendFileCallback, null);
      }

      /// <summary>
      /// Sends the file worker.
      /// </summary>
      /// <param name="fileName">Name of the file.</param>
      /// <returns>SendFileCompleteEventArgs.</returns>
      private SendFileCompleteEventArgs SendFileWorker(string fileName)
      {
         SendFileCompleteEventArgs result;

         string id = null;

         if (!String.IsNullOrEmpty(fileName) && fileName.Contains(";")) {
            var idx = fileName.IndexOf(";");
            id = fileName.Substring(idx + 1, fileName.Length - idx - 1);
         }

         using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
            using (var cli = new TcpClient()) {

               try {   
                  cli.Connect(endPoint);
               } catch (Exception ex) {
                  //MessageBox.Show(ex.Message);
                  return null;
               }

               result = new SendFileCompleteEventArgs(fileName);
               result.Started = DateTime.Now;


               using (var ns = cli.GetStream()) {
                  if (id != null) {
                     var arr = Encoding.ASCII.GetBytes(id);
                     ns.Write(arr, 0, arr.Length);
                  }

                  TcpStreamHelper.CopyStreamToStream(fs, ns, null);
                  ns.Flush();
                  ns.Close();
               }

               result.Completed = DateTime.Now;
               return result;
           }
         }
      }

      /// <summary>
      /// Sends the file callback.
      /// </summary>
      /// <param name="ar">The ar.</param>
      private void SendFileCallback(IAsyncResult ar)
      {
         var result = (AsyncResult)ar;
         var del = (Func<string, SendFileCompleteEventArgs>)result.AsyncDelegate;

         try {
            var args = del.EndInvoke(ar);

            if (args != null) {
               var evt = OnSendFileComplete;

               if (evt != null) {
                  evt(this, args);
               }
            }
         } catch {
            throw; //handle this
         }
      }

      #region IDisposable Members
      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      public void Dispose()
      {
      }
      #endregion
   }
}