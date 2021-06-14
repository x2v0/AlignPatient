// $Id: ReferenceImage.cs 4359 2016-08-29 13:54:18Z onuchin $

//#define LOCAL_DEBUG

using P;
using P.Net;

namespace PatientDisplay
{
   /// <summary>
   ///    Class PatientForm
   /// </summary>
   public partial class PatientForm
   {
      #region Events

      /// <summary>
      ///    Called when [send file complete].
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="SendFileCompleteEventArgs" /> instance containing the event data.</param>
      private void SendFileComplete(object sender, SendFileCompleteEventArgs e)
      {
         if (InvokeRequired) {
            Invoke(new OnSendFileCompleteDelegate(SendFileComplete), sender, e);
         } else {
#if LOCAL_DEBUG
         MessageBox.Show(
            string.Format("Sent file {0} in {1}", e.FileName, e.TimeTaken), 
            Text, 
            MessageBoxButtons.OK, 
            MessageBoxIcon.Information);
#endif
         }
         // resume video processing
         //fVideoCaptureGraph.Run();
      }

      #endregion Events

      #region Members

      /// <summary>
      ///    The ref file name
      /// </summary>
      private static string gRefFileName = "PositionReferenceImage";

      /// <summary>
      ///    The reference image file path
      /// </summary>
      private string fRefFilePath;

      /// <summary>
      ///    Sends the reference image across the network
      /// </summary>
      private TcpSender fTcpSenderImage;

      /// <summary>
      ///    Receives the reference image across the network
      /// </summary>
      private TcpReceiver fTcpReceiverImage;

      #endregion

      #region Private Methods

      /// <summary>
      ///    Sends the reference image.
      /// </summary>
      private void SendReferenceImage()
      {
         if ((fTcpSenderImage == null) && (Session.Doctor != null)) {
            // reference image sender via TCP/IP protocol 
            fTcpSenderImage = new TcpSender(Session.Doctor.IPAddress, Session.TcpPort);
            fTcpSenderImage.OnSendFileComplete += SendFileComplete;
         }

         if (fTcpSenderImage != null && !string.IsNullOrEmpty(fRefFilePath)) {
#if LOCAL_DEBUG
            MessageBox.Show("Sending file :" + fRefFilePath + " to " + Session.Doctor.IPAddress + " " + Session.TcpPort);
#endif
            // suspend video
            //fVideoCaptureGraph.Stop();
            fTcpSenderImage.SendFile(fRefFilePath);
         }
      }

      /// <summary>
      ///    Sets the reference image.
      /// </summary>
      private void SetReferenceImage()
      {
         iCheckPosFilter.SetRefImageFile(fRefFilePath);

#if LOCAL_DEBUG
            MessageBox.Show("Reference Image Set : " + fRefFilePath);
#endif
      }

      #endregion
   }
}