// $Id: DoctorDisplayCode.cs 2130 2014-03-05 18:51:19Z onuchin $

//#define DEBUG
//#define LOCAL_DEBUG

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices; // DllImport, to register the filters
//using System.Diagnostics;
using System.IO;
using System.Data;
using System.Drawing;
//using System.Text;

// our own namespaces
using System.Threading;
using System.Windows.Forms;
using P;
using P.DShow;
using P.Net;
using P.Net.Rtp;
//using Win32Util;

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Input;

using Keys = System.Windows.Forms.Keys;
using Point = System.Drawing.Point;

namespace DoctorDisplay
{
   public partial class DoctorDisplayForm
   {
      #region Interop

      [DllImport("PRtpFilter.ax", EntryPoint = "DllRegisterServer")]
      private static extern void RegisterRtpFilters();

      [DllImport("PCheckPosFilter.ax", EntryPoint = "DllRegisterServer")]
      private static extern void RegisterCheckPosFilter();

      [DllImport("PCheckPosFilter.ax", EntryPoint = "DllUnregisterServer")]
      private static extern void UnregisterCheckPosFilter();

      #endregion Interop
      #region Members

      /// <summary>
      /// command line parcer
      /// </summary>
      public static ArgumentParser gArguments;

      /// <summary>
      /// Path to the file of reference image
      /// </summary>
      public string fRefFilePath = Path.Combine(Path.GetTempPath(),
                                                "PositionReferenceImage.jpg");

      public static string gRefFileName;
      public static string[] gRefFiles; 

      public static string gHelpFile = "AlignPatient_UsersGuider.chm";

      /// <summary>
      /// Contains filter graph on the sending side
      /// </summary>
      private VideoCaptureGraph fCaptureGraph;

      /// <summary>
      /// Manages the connection to a multicast address and all the objects related to Rtp
      /// </summary>
      private RtpSession fRtpSession;

      /// <summary>
      /// Sends the video across the network
      /// </summary>
      private RtpSender fRtpSender;

     /// <summary>
      /// Sends the reference image across the network
      /// </summary>
      private TcpSender fTcpSenderImage;

     /// <summary>
      /// Recieves the reference image across the network
      /// </summary>
      private TcpReceiver fTcpReceiverImage;

      /// <summary>
      /// Doctor as participant of RtpSession
      /// </summary>    
      private RtpParticipant fParticipant;
      private RtpParticipant fPatient;

      private object fGraphMgrLock = new object();
      private FilterGraph.State fGraphMgrState = FilterGraph.State.Stopped;
      private FilterInfo fFilterInfo;
      private bool fFrontalInitiated;

      private int fRedLevel;
      private bool fBgWorkerPaused;

      ///To keep track of the current and previous state of the gamepad
      /// <summary>
      /// The current state of the controller
      /// </summary>
      //GamePadState fGamePadState;

      /// <summary>
      /// The previous state of the controller
      /// </summary>
      //GamePadState fPreviousState;

      /// <summary>
      /// Keeps track of the current controller
      /// </summary>
      //PlayerIndex fPlayerIndex = PlayerIndex.One;

      /// <summary>
      /// true if patient gamepad is connected -> ROI lines are not drawn
      /// </summary>
      bool fPatientGamePadOn;

      /// <summary>
      /// Counter for limiting the time for which the vibration motors are on.
      /// </summary>
      int fVibrationCountdown = 0;

      protected delegate void VoidDelegateInt(int idx);
      protected delegate void VoidDelegate();

      #endregion Members
      #region Rtp/Rtcp 

      private void HookRtpEvents()
      {
         RtpEvents.RtpStreamAdded += RtpStreamAdded;
         RtpStream.FirstFrameReceived += FirstFrameReceived;
         RtpEvents.RtpStreamRemoved += RtpStreamRemoved;
         RtpEvents.AppPacketReceived += AppPacketReceived;
         RtpEvents.RtpParticipantAdded += OnNewParticipant;
         RtpEvents.RtpParticipantRemoved += ParticipantRemoved;
         RtpEvents.NetworkTimeout += RaiseNetworkTimeout;
         RtpEvents.RtpParticipantTimeout += RaiseParticipantTimeout;
         RtpEvents.RtpStreamTimeout += RtpStreamTimeout;
         RtpEvents.HiddenSocketException += RaiseHiddenSocket;
      }

      private void UnhookRtpEvents()
      {
         RtpStream.FirstFrameReceived -= FirstFrameReceived;
         RtpEvents.RtpStreamAdded -= RtpStreamAdded;
         RtpEvents.RtpStreamRemoved -= RtpStreamRemoved;
         RtpEvents.AppPacketReceived -= AppPacketReceived;
         RtpEvents.RtpParticipantAdded -= OnNewParticipant;
         RtpEvents.RtpParticipantRemoved -= ParticipantRemoved;
         RtpEvents.NetworkTimeout -= RaiseNetworkTimeout;
         RtpEvents.RtpParticipantTimeout -= RaiseParticipantTimeout;
         RtpEvents.RtpStreamTimeout -= RtpStreamTimeout;
         RtpEvents.HiddenSocketException -= RaiseHiddenSocket;

         if (fTcpReceiverImage != null) {
            fTcpReceiverImage.OnFileReceived -= FileReceived;
         }
      }

      private void RaiseHiddenSocket(object sender, RtpEvents.HiddenSocketExceptionEventArgs  ea)
      {
         MessageBox.Show(Strings.NetworkBroken);
      }

      private void RtpStreamTimeout(object sender, RtpEvents.RtpStreamEventArgs ea)
      {
#if LOCAL_DEBUG
         RtpStream stream = ea.RtpStream;
         PayloadType pt = stream.PayloadType;
         SdesData prop = stream.Properties;
         string cname = prop.CName;
         string name = prop.Name;

         MessageBox.Show(String.Format("Stream timeout - {0} : {1} : {2}", cname, pt, name));
#endif
      }

      private void OnNewParticipant(object sender, RtpEvents.RtpParticipantEventArgs ea)
      {
         RtpParticipant p = ea.RtpParticipant;

#if LOCAL_DEBUG
         string str = "New participant ";
         str += String.Format("{0} : {1} {2}", p.CName, p.IPAddress, Session.PatientCName);
         MessageBox.Show(str);
#endif
         if (p.CName.StartsWith(Session.PatientCName)) {
            fPatient = p;
            Session.Connected = true;

            if (fTcpReceiverImage == null) {
               fTcpReceiverImage = new TcpReceiver(Session.TcpPort);
               fTcpReceiverImage.OnFileReceived += FileReceived;
               fTcpReceiverImage.Start();
            }
            ResetPatient();
            SetThreshold();
         }
      }
/*
      /// <summary>
      /// Handle function called when reference image received from patient machine
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void FileReceived(object sender, OnFileReceivedArgs e)
      {
         if (InvokeRequired) {
            //Just in case we want to play with the UI
            Invoke(new OnFileReceivedDelegate(FileReceived), sender, e);
         } else {
            string filename = "PosRefImage";
            DateTime now = DateTime.Now;
            byte cameraId = (byte)(51 - e.Buffer[0]);
            filename += string.Format("_{0:yyyy-MM-dd_HH-mm}_", now);
            filename += cameraId;
            filename += ".jpg";

            // generate file name
            if (Session.PatientName != null) {
               filename = "pref";

               if (Session.PatientId >= 0) {
                  filename += Session.PatientId + "_";
               }

               var patientsrow = fDb.Patients.Rows[0];

               filename += String.Format("{0}_{1}_{2}", patientsrow["LastName"], patientsrow["FirstName"], patientsrow["MiddleName"]);

               Session.StudyTime = now;

               filename += string.Format("_{0:yyyy-MM-dd_HH-mm}_", now);
               filename += cameraId;
               filename += ".jpg";
            }

//#if LOCAL_DEBUG
            MessageBox.Show(Strings.ReferenceFileReceived + filename);
//#endif

            // save file locally through file dialog
            if (Session.Local || Session.PatientName == null) {
               try {

                  using (var sd = new SaveFileDialog {
                     FileName = filename, 
                     Title = Strings.SaveFile,
                     Filter = Strings.SaveFileTypes + filename,
                     FilterIndex = 1,
                     InitialDirectory = Path.Combine(Session.DataBasePath, Session.HeadPath)}) {
                  
                     if (sd.ShowDialog() == DialogResult.OK) { // ask user for file path & name
                        File.WriteAllBytes(sd.FileName, e.Buffer.Skip(1).ToArray());
                     } else {
                        return;
                     }
                  }

               } catch (Exception) {
                  MessageBox.Show(this, Strings.SaveFileError, Strings.Doctor_Display,
                                  MessageBoxButtons.OK, MessageBoxIcon.Stop);
               }

               return;
            }

            if (fDb == null) {
               fDb = new Db();
            }

            using (var headAdapter = new HeadAdapter()) {

               int ret = headAdapter.FillById(fDb.Head, Session.PatientId);

               //
               Session.NewSession = true;

               if (ret != 0) {   // patient with such ID already exists
                  fDbRow = fDb.Head.Rows[0];

                  if (File.Exists(fDbRow["HeadRef"].ToString())) {
                     File.Delete(fDbRow["HeadRef"].ToString());
                  }

                  fDbRow["Date"] = Session.StudyTime;
                  fDbRow["Time"] = Session.StudyTime;
                  fDbRow["HeadRef"] = filename;
                  fDbRow["CX"] = Session.RoiX;
                  fDbRow["CY"] = Session.RoiY;
                  fDbRow["Width"] = Session.RoiWidth;
                  fDbRow["Threshold"] = Session.Threshold;
                  //fDbAdapter.Update(Session.PatientId, Session.StudyTime, Session.StudyTime, filename, 0, 0, 50, 50,
                  //                  Session.PatientId, null, null, filename, null, null, null, 50);
                  headAdapter.Update(fDbRow);
               } else {
                  // create a new row
                  ret = headAdapter.Insert(Session.PatientId, Session.StudyTime, Session.StudyTime, filename,
                                           Session.RoiX[0], Session.RoiY[0], Session.RoiWidth[0], (short)Session.Threshold);

                  if (ret == 0) {
                     MessageBox.Show(this, Strings.DbInsertError, Strings.Doctor_Display,
                                     MessageBoxButtons.OK, MessageBoxIcon.Stop);
                  }
               }
            }
 
            Session.NewSession = false;
            buttonLoad.Enabled = true;
         }
      }
*/
       private void RaiseParticipantTimeout(object sender, RtpEvents.RtpParticipantEventArgs ea)
      {
#if LOCAL_DEBUG
         RtpParticipant p = ea.RtpParticipant;
         string str = "Participant leaved ";
         str += String.Format("{0} : {1}", p.CName, p.IPAddress);
         MessageBox.Show(str);
#endif
      }

      private void RaiseNetworkTimeout(object sender, RtpEvents.NetworkTimeoutEventArgs ea)
      {
#if LOCAL_DEBUG
         string message = ea.Message;
         string str = "Network  timeout: ";
         str += message;
         MessageBox.Show(str);
#endif
      }

      private void ParticipantRemoved(object sender, RtpEvents.RtpParticipantEventArgs ea)
      {
         RtpParticipant p = ea.RtpParticipant;
#if LOCAL_DEBUG
         string str = "Participant leaved ";
         str += String.Format("{0} : {1}", p.CName, p.IPAddress);
         MessageBox.Show(str);
#endif

         if (p.CName == Session.PatientCName) {
            MessageBox.Show(Strings.PatientLeaved);

            Session.Connected = false;
            fParticipant = null;
            Session.Participant = null;

            if (fTcpReceiverImage != null) {
               fTcpReceiverImage.OnFileReceived -= FileReceived;
               //fTcpReceiverImage.Dispose();
               //fTcpReceiverImage = null;
            }

            splitContainer1.Panel1.UseWaitCursor = true;
            splitContainer1.Panel2.UseWaitCursor = true;

            buttonSnap.Enabled = false;
            toolComboMode.SelectedIndex = 0;
         }
      }

      private void RtpStreamAdded(object sender, RtpEvents.RtpStreamEventArgs ea) {

         var stream = ea.RtpStream;
         var pt = stream.PayloadType;
         var prop = stream.Properties;
         var cname = prop.CName;
#if LOCAL_DEBUG
         string name = prop.Name;
         string priExns = prop.GetPrivateExtension("PROTOM_ReferenceImage");

         MessageBox.Show(String.Format("Stream added - {0} : {1} : {2}", cname, pt, name));
#endif

         if ((pt == PayloadType.dynamicVideo) &&
             (cname.StartsWith(Session.PatientCName))) {

               int idx = cname.EndsWith("1") ? 1 : 2;

               if (idx == 1) {
                  fRtpStreamF = stream;
                  fRtpStreamF.IsUsingNextFrame = true;
               
                  //SetROI();
                  //SetThreshold();
                  splitContainer1.Panel1.UseWaitCursor = false;
                  splitContainer1.Panel1.BackColor = SystemColors.GrayText;
               } else {
                  fRtpStreamP = stream;
                  fRtpStreamP.IsUsingNextFrame = true;
               
                  //SetROI();
                  //SetThreshold();
                  splitContainer1.Panel2.UseWaitCursor = false;
                  splitContainer1.Panel2.BackColor = SystemColors.GrayText;
               }

               buttonSnap.Enabled = true;
               toolComboMode.SelectedIndex = 0;

            } else if ((pt == PayloadType.dynamicAudio) &&
                      cname.StartsWith(Session.PatientCName)) {
               fRtpAudioStream = stream; // defined in Audio.cs
               fRtpAudioStream.IsUsingNextFrame = true;
            }
      }

      private void RtpStreamRemoved(object sender, RtpEvents.RtpStreamEventArgs ea)
      {

         var stream = ea.RtpStream;
         var pt = stream.PayloadType;
         var prop = stream.Properties;
         var cname = prop.CName;
#if LOCAL_DEBUG
         string name = prop.Name;

         MessageBox.Show(String.Format("Stream removed - {0} : {1} : {2}", cname, pt, name));
#endif
         if ((pt == PayloadType.dynamicVideo) &&
            cname.StartsWith(Session.PatientCName)) {

            int idx = cname.EndsWith("1") ? 1 : 2;

            buttonSnap.Enabled = false;
            toolComboMode.SelectedIndex = 0;
            //MessageBox.Show(Strings.StreamBroken);

            if (idx == 1) {   
               splitContainer1.Panel1.UseWaitCursor = true;
            } else {
               splitContainer1.Panel2.UseWaitCursor = true;
            }
         }
      }

      private void AppPacketReceived(object sender, RtpEvents.AppPacketReceivedEventArgs ea)
      {
         // Process RTCP APP packets which used for command processing

         var name  = ea.Name;
         var subtype = ea.Subtype;
         var data  = ea.Data;

         if (name == "DONE") {
#if LOCAL_DEBUG
            MessageBox.Show("DONE signal recieved");
#endif
         }

         if (name == "GPAD") {
            gamePadLabel.Visible = subtype == 0;
            fPatientGamePadOn = !gamePadLabel.Visible;

#if LOCAL_DEBUG
            MessageBox.Show("GPAD signal recieved " + subtype);
#endif
         } else if (name == "RXYW") {
            //Invoke(new VoidDelegateArr(SetROI), data);
         }
      }

      private void FirstFrameReceived(object sender, EventArgs ea)
      {

         lock (fGraphMgrLock) {
            var stream = (RtpStream)sender;
            var prop = stream.Properties;
            var name = prop.Name;
            var cname = prop.CName;
            var pt = stream.PayloadType;

#if LOCAL_DEBUG
            MessageBox.Show(String.Format("FirstFrameReceived - {0} : {1} : {2}", cname, pt, name));
#endif
            if ((pt == PayloadType.dynamicVideo) &&
               cname.StartsWith(Session.PatientCName)) { // video
               // Creating and destroying the filtergraph and the filters in it
               // must occur on same thread, so make it the UI thread
               
               int idx = cname.EndsWith("1") ? 1 : 2;

               UseWaitCursor = false;
               btnZoom.Enabled = true;
               buttonSnap.Enabled = true;

               if (idx == 1 && !fFrontalInitiated) {
                  fFrontalInitiated = true;
                  Session.FormInvoke(new VoidDelegate(CreateVideoGraphF), null);
                  splitContainer1.Panel1.UseWaitCursor = false;
               } else {
                  Session.FormInvoke(new VoidDelegate(CreateVideoGraphP), null);
                  splitContainer1.Panel2.UseWaitCursor = false;
               }

               // start game pad processing
               //fPlayerIndex = PlayerIndex.One;
               //fGamePadTimer.Start();

            } else if ((pt == PayloadType.dynamicAudio) && 
                       cname.StartsWith(Session.PatientCName)) {
               Session.FormInvoke(new VoidDelegate(CreateAudioRenderGraph), null); // defined in Audio.cs
            }
         }
      }

      private void LeaveRtpSession()
      {
         UnhookRtpEvents();

         if (fRtpSession != null) {
            fRtpSession.Dispose();
            fRtpSession = null;

            if (fRtpSender != null) {
               fRtpSender.Dispose();
               fRtpSender = null;
            }

            fRtpStreamF = null;
            fRtpStreamP = null;

            // Leave TCP sessions if any 
            if (fTcpSenderImage != null) {
               fTcpSenderImage.Dispose();
               fTcpSenderImage = null;
            }

            if (fTcpReceiverImage != null) {
               // not clear why exception is raised when it's disposed 
               //fTcpReceiverImage.Dispose();
               fTcpReceiverImage = null;
            }

         }
      }


      #endregion  Rtp/Rtcp
      #region Video/Audio

      private static IPin GetInputPin(IBaseFilter filt)   
      {   
         IPin pin; 
         IEnumPins pinList;
         uint fetched;

         if (filt == null) {
            return null;   
         }
 
         filt.EnumPins(out pinList);
         pinList.Reset();
         pinList.Next(1, out pin, out fetched);

         while (fetched == 1) {
            _PinInfo pinfo;
            pin.QueryPinInfo(out pinfo);

            if (pinfo.dir == _PinDirection.PINDIR_INPUT) {
               return pin;
            }
            pinList.Next(1, out pin, out fetched);
         }
         return null;
      }

      private static IPin GetOutputPin(IBaseFilter filt)
      {
         IPin pin;
         IEnumPins pinList;
         uint fetched;

         if (filt == null) {
            return null;
         }

         filt.EnumPins(out pinList);
         pinList.Reset();
         pinList.Next(1, out pin, out fetched);

         while (fetched == 1) {
            _PinInfo pinfo;
            pin.QueryPinInfo(out pinfo);

            if (pinfo.dir == _PinDirection.PINDIR_OUTPUT) {
               return pin;
            }
            pinList.Next(1, out pin, out fetched);
         }
         return null;
      }

      private static IPin ConnectedToInput(IBaseFilter filt)   
      {   
         if (filt == null) {
            return null;
         }

         var inPin = GetInputPin(filt);   
         IPin outPin;

         if (inPin == null) {
            return null;
         }

         inPin.ConnectedTo(out outPin);
       
         return outPin;   
      }

      private static bool IsRenderer(IBaseFilter filt)
      {
         if (filt == null) {
            return false;
         }

         IEnumPins  pinList;   
         int nrOutput = 0;   
         int nrInput = 0;   
         IPin  pin;
         uint  fetched;

         filt.EnumPins(out pinList);
         pinList.Reset();
         pinList.Next(1, out pin, out fetched);

         while (fetched == 1) {
            _PinInfo pinfo;
            pin.QueryPinInfo(out pinfo);

            if (pinfo.dir == _PinDirection.PINDIR_OUTPUT) {
               nrOutput++;   
            } else {
               nrInput++;
            }
            pinList.Next(1, out pin, out fetched);
        }

         // the only filters that have no outputs are renderers
         return nrOutput == 0 && nrInput == 1;
      }

      private IBaseFilter FindRenderer()
      {
         var iFG = (IFilterGraph)fFilgraphManagerF;

         IEnumFilters iEnum;
         iFG.EnumFilters(out iEnum);

         IBaseFilter iBF;
         uint fetched;
         iEnum.Next(1, out iBF, out fetched);

         while (fetched == 1) {
            if (IsRenderer(iBF)) {
               return iBF;
            }

            iEnum.Next(1, out iBF, out fetched);
         }

         return null; 
      }

      public void Cleanup()
      {
 
         fBgWorkerPaused = true;

         if (backgroundWorker1 != null) {
            backgroundWorker1.CancelAsync();
            backgroundWorker1.Dispose();
            backgroundWorker1 = null;
            GC.Collect();
         }

         lock (fGraphMgrLock) {
            CleanupF();
            CleanupP();

            DisposeDevice();
            DisposeAudio();
         }

         LeaveRtpSession();
         UnregisterCheckPosFilter();

         Session.CallingForm = Program.gForm = null;
      }

      private void DisposeDevice()
      {
         if (fCaptureGraph != null) {
            fCaptureGraph.Stop();
            fCaptureGraph.RemoveFiltersDownstreamFromSource(PayloadType.dynamicVideo);
            fCaptureGraph.Dispose();
            fCaptureGraph = null;
         }
      }

      #endregion Video, Audio
      #region Database

      /// <summary>
      ///  Load file from data base by ID specified from the command line
      /// </summary>
      private int LoadFileByID()
      {
         /*
         if (fDb == null) {
            fDb = new Db();
         }
         int ret;
         bool twoCams = false;

         using (var headAdapter = new HeadAdapter()) {

            ret = headAdapter.FillById(fDb.Head, Session.PatientId);

            //
            Session.NewSession = true;

            if (ret != 0) {
               fDbRow = fDb.Head.Rows[0];

               gRefFileName = fDbRow["HeadRef"].ToString();
               Session.RoiX[Session.ProgramNumber] = (short)fDbRow["CX"];
               Session.RoiY[Session.ProgramNumber] = (short)fDbRow["CY"];
               Session.RoiWidth[Session.ProgramNumber] = (short)fDbRow["Width"];
               Session.Threshold = (short)fDbRow["Threshold"];
               gRefFiles = gRefFileName.Split(' ');
               twoCams = gRefFiles.Count() == 2;

               if (gRefFileName != null) {
                  if (twoCams) {    // two cameras by default
                     for (var i = 0; i < 2; i++) {
                        gRefFiles[i] = Path.Combine(Session.DataBasePath, gRefFiles[i]);
                        Session.NewSession |= File.Exists(gRefFiles[i]);
                     }
                  } else {
                     Session.NewSession = File.Exists(gRefFileName);
                  }
               }
               
            }
         }
         // disable/enable load button
         buttonLoad.Enabled = !Session.NewSession || Session.Local;

         using (var patients = new PatientsAdapter()) {
            ret = patients.FillById(fDb.Patients, Session.PatientId);

            if (ret != 0) {
               DataRow patientsrow = fDb.Patients.Rows[0];
               Session.PatientName = String.Format("{0} {1} {2}", 
                                                   patientsrow["LastName"],
                                                   patientsrow["FirstName"],
                                                   patientsrow["MiddleName"]);

               labelFamilyName.Text = patientsrow["LastName"].ToString();
               labelName.Text = patientsrow["FirstName"].ToString();
               labelSndName.Text = patientsrow["MiddleName"].ToString();
               labelID.Text = patientsrow["ID"].ToString();
               String str = patientsrow["DOB"].ToString();
               int idx = str.IndexOf(" ");

               if (idx > 0) {
                  labelBirthday.Text = str.Remove(idx);
               } else {
                  labelBirthday.Text = str;
               }

               string fotoDb = Session.DataBasePath;
               fotoDb = Path.Combine(fotoDb, Session.PhotoPath);
               string path = Path.Combine(fotoDb, patientsrow["FotoRef"].ToString());

               if (File.Exists(path)) {
                  pictureFoto.Load(path);
               } else {
                  
               }

               // change window title
               if (Text == Strings.FormTitle) {
                  Text += " : " + Session.PatientName;
               }
            }

            /*if (gRefFileName != null) {
               if (Path.GetDirectoryName(gRefFileName) == null) {   // 
                  gRefFileName = Path.Combine(Session.DataBasePath, gRefFileName);
               }
               ret = Convert.ToInt32(File.Exists(gRefFileName));
            } else {
               ret = 0;
            }*/
         int ret;
         return ret;
      }
   }

      /// <summary>
      /// Load reference file from local directory. For example:
      /// C:\Documents and Settings\onuchin\Application Data\AlignPatientDB
      /// </summary>
      /// <returns>1- in case of success</returns>
      private int LoadLocalFile()
      {

         using (var ofd = new OpenFileDialog {
               DefaultExt = "jpg",
               Filter = Strings.OpenFileDialogFilter, 
               AddExtension = true,
               RestoreDirectory = true,
               InitialDirectory = Session.DataBasePath}) {

            // if initial directory doesn't exist -> look file  in TEMP dir
            if (!Directory.Exists(ofd.InitialDirectory)) {
               ofd.InitialDirectory = Path.GetTempPath();
               ofd.FileName = fRefFilePath;
            }

            if (ofd.ShowDialog() == DialogResult.OK) {
               if (!File.Exists(ofd.FileName)) {
                  MessageBox.Show(Strings.FailFileAccess + ofd.FileName);
                  return 0;
               }
               gRefFileName = ofd.FileName;
            }
         }

         return 1;
      }

      /// <summary>
      /// send reference image to patient
      /// </summary>
      private void SendReferenceImage()
      {

         int ret;
         fBgWorkerPaused = true;

         if (Session.Local) {
            ret = LoadLocalFile();

         } else {
            ret = LoadFileByID();

            if (ret == 0) {
               MessageBox.Show(Strings.NoRefImageInDb);
               ret = LoadLocalFile();
            }
         }

         if ((fTcpSenderImage == null) && (fPatient != null) && Session.Connected) {

#if LOCAL_DEBUG
            MessageBox.Show("Connecting ...");
#endif
            fTcpSenderImage = new TcpSender(fPatient.IPAddress, Session.TcpPort);
            fTcpSenderImage.OnSendFileComplete += OnSendFileComplete;

#if LOCAL_DEBUG
            MessageBox.Show(Strings.Connected);
#endif
         }

         if (fTcpSenderImage != null) {
#if LOCAL_DEBUG
         MessageBox.Show(Strings.SendingFile + gRefFileName);
#endif

            if (File.Exists(gRefFileName)) {
               fTcpSenderImage.SendFile(gRefFileName);
               Cursor = Cursors.WaitCursor;
            } else {
               MessageBox.Show(Strings.LoadRefImageError + gRefFileName);
            }
         }
      }

      private void OnSendFileComplete(object sender, SendFileCompleteEventArgs e)
      {
         if (InvokeRequired) {
            Invoke(new OnSendFileCompleteDelegate(OnSendFileComplete), sender, e);
         } else {
#if LOCAL_DEBUG
         MessageBox.Show(
            string.Format("Sent file {0} in {1}", e.FileName, e.TimeTaken), 
            Text, 
            MessageBoxButtons.OK, 
            MessageBoxIcon.Information);
#endif
            MessageBox.Show(Strings.RefFile + gRefFileName + Strings.Loaded);
         }

         fBgWorkerPaused = false;
      }

      /// <summary>
      /// Sends signal to patient to activate "screen saver" 
      /// </summary>
      private void ActivatePatientScreenSaver(byte on)
      {
         if (fParticipant == null || !Session.Connected) {
            return;
         }

         uint ssrc = fParticipant.SSRC;
         const string name = "BLCK";   // black screen

         // BLCK subtypes:
         //     1 - activate black screen on patient netbook
         //     0 - back to normal video 
         fRtpSession.SendAppPacket(ssrc, name, on, null, Rtcp.RtcpInterval.Now);
      }

      #endregion Database
      #region GUI events handlers

      private void DoctorDisplayForm_Load(object sender, EventArgs e)
      {
         HookRtpEvents();
         Session.JoinRtpSession(Session.DoctorCName);
         fRtpSession = Session.RtpSession;
         fParticipant = Session.Participant;

         if ((Session.PatientId != 0) &&
             (Session.Local == false)) {
            int ret = LoadFileByID();

            if (ret != 0 || Session.Local) {
               fCheckMode = true;
               buttonLoad.Enabled = true;
            } else {
               fCheckMode = false;
               buttonLoad.Enabled = false;
            }
         }

         foreach (FilterInfo fi in VideoSource.Sources()) {
            fFilterInfo = fi; // the very first camera
            break;
         }

         foreach (FilterInfo fi in AudioSource.Sources()) {
            fMicroponeFI = fi; // the very first microphone
            break;
         }

         foreach (FilterInfo fi in AudioRenderer.Renderers()) {
            fSpeakerFI = fi; // the very first renderer
            break;
         }

         if (Session.ActivateMicrophone) {
            ActivateMicrophone(); //defined in AudioD.cs
         }

         buttonSound.CheckOnClick = true;

         LoadROOT();

         Session.CallingForm = this;
      }

      private void DoctorDisplayForm_Closing(object sender, CancelEventArgs e)
      {

         fBgWorkerPaused = true;

         if (backgroundWorker1 != null) {
            backgroundWorker1.CancelAsync();
	         backgroundWorker1.Dispose();
            backgroundWorker1 = null;
            GC.Collect();
         }

         Session.RoiWidth[0] = Session.RoiWidth[1] =
         Session.RoiX[0] = Session.RoiX[1] = 
         Session.RoiY[0] = Session.RoiY[1] = 0;      // unzoom

         SetROI();
         ActivatePatientScreenSaver(1);
         Cleanup();

         Session.CallingForm = Program.gForm = null;
      }

      private void buttonSnapClick(object sender, EventArgs e)
      {

         if (fParticipant == null || !Session.Connected) {
            MessageBox.Show(Strings.NoPatientConnection);
            return;
         }

         toolStripMode.UseWaitCursor = true;
         splitContainer1.Panel1.UseWaitCursor = true;
         splitContainer1.Panel2.UseWaitCursor = true;

         SetThreshold();
         SetROI();

         uint ssrc = fParticipant.SSRC;

         // SNAP subtypes:
         //     0 - make snapshot
         fRtpSession.SendAppPacket(ssrc, "SNAP", 0, null, Rtcp.RtcpInterval.Now);

         toolComboMode.SelectedIndex = 1;
         toolComboMode.Enabled = true;
         labelMode.Enabled = true;
         buttonSave.Enabled = true;

         splitContainer1.Panel1.UseWaitCursor = false;
         toolStripMode.UseWaitCursor = false;
         splitContainer1.Panel2.UseWaitCursor = false;

         // Start the background worker
         if (!backgroundWorker1.IsBusy) {
            backgroundWorker1.RunWorkerAsync();
         }
      }

      private void buttonSaveClick(object sender, EventArgs e)
      {

         if (fParticipant == null || !Session.Connected) {
            MessageBox.Show(Strings.NoPatientConnection);
            return;
         }

         toolStripMode.UseWaitCursor = true;

         uint ssrc = fParticipant.SSRC;

         // SAVE subtypes:
         //     0 - 
         fRtpSession.SendAppPacket(ssrc, "SAVE", 0, null, Rtcp.RtcpInterval.Now);

         toolStripMode.UseWaitCursor = false;
      }

      /// <summary>
      /// Send DoctorDisplay.config Threshold setting to the Patient machine
      /// </summary>
      private void SetThreshold()
      {

         if (fParticipant == null || !Session.Connected) {
            MessageBox.Show(Strings.NoPatientConnection);
            return;
         }

         uint ssrc = fParticipant.SSRC;

         // send Threshold value
         byte[] arr = BitConverter.GetBytes(Session.Threshold);
 
         fRtpSession.SendAppPacket(ssrc, "THRS", 0, arr, Rtcp.RtcpInterval.Now);
      }

      /// <summary>
      /// Send RoiX, RoiY, RoiWidth settings to the Patient machine
      /// </summary>
      private void SetROI()
      {

         if (fParticipant == null || !Session.Connected) {
            MessageBox.Show(Strings.NoPatientConnection);
            return;
         }

         uint ssrc = fParticipant.SSRC;

         // send ROI data to frontal camera
         int roi = Session.RoiWidth[0] +
                   Session.RoiX[0]*1000 +
                   Session.RoiY[0]*1000000;
         byte[] arr = BitConverter.GetBytes(roi);

         fRtpSession.SendAppPacket(ssrc, "RXYW", 0, arr, Rtcp.RtcpInterval.Now);

         // send ROI data to profile camera
         roi = Session.RoiWidth[1] +
               Session.RoiX[1]*1000 +
               Session.RoiY[1]*1000000;
         arr = BitConverter.GetBytes(roi);

         fRtpSession.SendAppPacket(ssrc, "RXYW", 1, arr, Rtcp.RtcpInterval.Now);

      }

      /*private void SetROI(byte[] roi)
      {

         if (roi == null) {
            return;
         }

         // suspend video processing
         //fVideoCaptureGraph.Stop();

         int v = BitConverter.ToInt32(roi, 0);
         Session.RoiY[Session.ProgramNumber] = (short)(v/1000000);
         Session.RoiX[Session.ProgramNumber]= (short)((v - Session.RoiY[Session.ProgramNumber]*1000000)/1000);
         Session.RoiWidth[Session.ProgramNumber] = (short)(v - Session.RoiY[Session.ProgramNumber]*1000000 -
                                                           Session.RoiX[Session.ProgramNumber]*1000);

         //iCheckPosFilterF.Reset();
         //iCheckPosFilterF.SetCropArea(x, y, w);

         // resume video processing
         //fVideoCaptureGraph.Run();
      }*/

      /// <summary>
      /// Send Reset signal to the Patient machine
      /// </summary>
      private void ResetPatient()
      {

         if (fParticipant == null || !Session.Connected) {
            //MessageBox.Show(Strings.NoPatientConnection);
            return;
         }

         uint ssrc = fParticipant.SSRC;
 
         fRtpSession.SendAppPacket(ssrc, "RSET", 0, null, Rtcp.RtcpInterval.Now);
      }

      /// <summary>
      ///  Sends reference image to patient
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void buttonLoadClick(object sender, EventArgs e)
      {

         toolStrip.UseWaitCursor = true;
         splitContainer1.Panel1.UseWaitCursor = true;

         SendReferenceImage();

         if (fDbRow != null) {
            Session.RoiX[Session.ProgramNumber] = (short)fDbRow["CX"];
            Session.RoiY[Session.ProgramNumber] = (short)fDbRow["CY"];
            Session.RoiWidth[Session.ProgramNumber] = (short)fDbRow["Width"];
            Session.Threshold = (short)fDbRow["Threshold"];

            SetThreshold();
            SetROI();
         }

         HideLinesF();

         Cursor = Cursors.Default;
         toolComboMode.SelectedIndex = 1;
         toolComboMode.Enabled = true;
         buttonLoad.Enabled = true;
         btnZoomOut.Enabled = true;
         btnZoom.Enabled = false;
         buttonSnap.Enabled = false;
         buttonSave.Enabled = false;
         toolStrip.UseWaitCursor = false;
         splitContainer1.Panel1.UseWaitCursor = false;

         // Start the background worker
         if (!backgroundWorker1.IsBusy) {
            backgroundWorker1.RunWorkerAsync();
         }
      }

      private void selectedModeChanged(object sender, EventArgs e)
      {

         var subtype = (byte)toolComboMode.SelectedIndex;

         if (fParticipant == null || !Session.Connected) {
            MessageBox.Show(Strings.NoPatientConnection);
            return;
         }

         var ssrc = fParticipant.SSRC;

         // SHOW subtypes:
         //     0 - off = do not show difference between the current video frame and reference image
         //     1 - on = how difference between the current video frame and reference image
         //     2 - semi-transparent mode

         fRtpSession.SendAppPacket(ssrc, "SHOW", subtype, null, Rtcp.RtcpInterval.Now);
      }

      private void showHelp()
      {
         var path = Application.StartupPath;
         var fname = gHelpFile;
         var filename = fname;

         if (String.IsNullOrEmpty(Path.GetPathRoot(fname))) {
            filename = path + @"\" + fname;
         } 

         string pathRoot = Path.GetPathRoot(filename);
         var di = new DriveInfo(pathRoot);

         // 
         if (di.DriveType == DriveType.Network) {
            string tmp = Path.GetTempFileName() + ".chm";
            File.Copy(filename, tmp);
            filename = tmp;
            gHelpFile = filename;
         }

         Help.ShowHelp(this, filename);
      }

      private void buttonHelpClick(object sender, EventArgs e)
      {
         toolStrip.UseWaitCursor = true;

         showHelp();

         toolStrip.UseWaitCursor = false;
      }

      private void soundCheckedChanged(object sender, EventArgs e)
      {

         var btn = (ToolStripButton)sender;

         if (!btn.Checked) {
            buttonSound.Text = Strings.AudioOn;
            buttonSound.ToolTipText = Strings.OffAudio;
            buttonSound.BackColor = Color.GreenYellow;
            fAudioCaptureGraph.Run();
         } else {
            buttonSound.Text = Strings.OnAudio;
            buttonSound.ToolTipText = Strings.AudioOftooltip;
            fAudioCaptureGraph.Stop();
            //buttonSound.BackColor = Color.GradientInactiveCaption;
         }
      }

      private void PbOnPaint(object sender, PaintEventArgs e)
      {
         using (var myFont = new Font("Arial", 14)) {
            e.Graphics.DrawString(Strings.WaitingForNetCon, myFont, 
                                  Brushes.Cornsilk, new Point(200, 250));
         }
      }

      private void OnKeyDown(object sender, KeyEventArgs e)
      {

         if(e.KeyCode == Keys.F1) {
            toolStrip.UseWaitCursor = true;

            showHelp();

            toolStrip.UseWaitCursor = false;
         }
      }

      private void mainShown(object sender, EventArgs e)
      {

         if (Session.PatientId == 0) { // id was not specified
            return;
         }

         if (fCheckMode) {
            Thread.Sleep(1000);
            var checkInfo = new AboutChecking();
            checkInfo.ShowDialog();
         } else {
            Thread.Sleep(1000);
            var fixInfo = new AboutFixing();
            fixInfo.ShowDialog();
         }
      }

      #endregion GUI events handlers 
  
      #region Zooming

      private void OnZoomClick(object sender, EventArgs e)
      {
         btnZoomOut.Enabled = true;

         ZoomFrontCamera();
         ZoomProfileCamera();

         SetROI();

         fZoomedP = true;

         btnZoom.Enabled = false;
      }

      private void OnZoomOutClick(object sender, EventArgs e)
      {

         Session.RoiWidth[0] = Session.RoiWidth[1] =
         Session.RoiX[0] = Session.RoiX[1] = 
         Session.RoiY[0] = Session.RoiY[1] = 0;      // unzoom

         SetROI();
         //ResetPatient();

         UnzoomFrontalCamera();
         UnzoomProfileCamera();

         btnZoom.Enabled = true;
         fPressedLineType = ELineType.kNonLine;

         toolComboMode.SelectedIndex = 1;
         toolComboMode.Enabled = true;
         buttonLoad.Enabled = true;
         btnZoomOut.Enabled = true;
         btnZoom.Enabled = true;
         buttonSnap.Enabled = true;
         buttonSave.Enabled = true;
      }

      #endregion Zooming
      #region Red Progress Bar

      private void GetRedLevel()
      {
         int val;
         iCheckPosFilterF.GetImageSize(out val);
         fRedLevel = val;
         iCheckPosFilterP.GetImageSize(out val);
         fRedLevel += val;
      }

      private void bgwDoWork(object sender, DoWorkEventArgs e)
      {

         var bwAsync = sender as BackgroundWorker;

         while (Session.CallingForm != null) {
            Thread.Sleep(200);

            if (bwAsync.CancellationPending) {
               e.Cancel = true;
               return;
            }

            if (fBgWorkerPaused) {
               continue;
            }
            Session.FormInvoke(new VoidDelegate(GetRedLevel), null);
  
            // Report progress to 'UI' thread
            fRedLevel = fRedLevel >= 100 ? 100 : fRedLevel;
            bwAsync.ReportProgress(fRedLevel);
         }
      }

      private void bgwProgressChanged(object sender, ProgressChangedEventArgs e)
      {
         // The progress percentage is a property of e
         redLevelBar.Value = e.ProgressPercentage;
         redLevel.Text = e.ProgressPercentage.ToString(CultureInfo.InvariantCulture);

         if (redLevelBar.Value > 30) {
           redLevelBar.ForeColor = Color.DarkRed;
         } else if (redLevelBar.Value <= 30 && redLevelBar.Value > 20) {
           redLevelBar.ForeColor = Color.Red;
         } else if (redLevelBar.Value <= 20 && redLevelBar.Value > 10) {
            redLevelBar.ForeColor = Color.Orange;
         } else if (redLevelBar.Value <= 10 && redLevelBar.Value > 5) {
            redLevelBar.ForeColor = Color.Yellow;
         } else  {
            redLevelBar.ForeColor = Color.GreenYellow;
         }
      }

      #endregion Red Progress Bar
      #region GamePad Processing

      private void UpdateControllerState()
      {
/*
         fPreviousState = fGamePadState;
         fGamePadState = GamePad.GetState(fPlayerIndex);
         gamePadLabel.Visible = !fGamePadState.IsConnected && !fPatientGamePadOn;

         // right X line
         var rx = (short)(fGamePadState.ThumbSticks.Right.X*120);
         var dx = (short)(rx - fPrevRightX);

         if (fPrevRightX >= 0) {
            if (rx > fPrevRightX) {
               fRightXF += dx;

               if (fRightXF > 2 && fRightXF < fVideoWindowWF - 2) {
                  iCheckPosFilterF.SetCropArea(fRightXF, 0, (int)ELineType.kRightLine);
               } else {
                  fRightXF -= dx;
               }
            }
            fPrevRightX = rx;
        } else if (fPrevRightX <= 0) {
            if (rx < fPrevRightX) {
               fRightXF += dx;
               if (fRightXF > 2 && fRightXF < fVideoWindowWF - 2) {
                  iCheckPosFilterF.SetCropArea(fRightXF, 0, (int)ELineType.kRightLine);
               } else {
                  fRightXF -= dx;
               }
            }
            fPrevRightX = rx;
         }

         // left X line
         var lx = (short)(fGamePadState.ThumbSticks.Left.X*120);
         dx = (short)(lx - fPrevLeftX);

         if (fPrevLeftX >= 0) {
            if (lx > fPrevLeftX) {
               fLeftXF += dx;

               if (fLeftXF > 2 && fLeftXF < fVideoWindowWF - 2) {
                  iCheckPosFilterF.SetCropArea(fLeftXF, 0, (int)ELineType.kLeftLine);
               } else {
                  fLeftXF -= dx;
               }
            }
            fPrevLeftX = lx;
        } else if (fPrevLeftX <= 0) {
            if (lx < fPrevLeftX) {
               fLeftXF += dx;

               if (fLeftXF > 2 && fLeftXF < fVideoWindowWF - 2) {
                  iCheckPosFilterF.SetCropArea(fLeftXF, 0, (int)ELineType.kLeftLine);
               } else {
                  fLeftXF -= dx;
               }
            }
            fPrevLeftX = lx;
         }

         // bottom Y line
         var ry = (short)(fGamePadState.ThumbSticks.Right.Y*120);
         var dy = (short)(ry - fPrevBotY);

         if (fPrevBotY >= 0) {
            if (ry > fPrevBotY) {
               fBotYF += dy;

               if (fBotYF > 2 && fBotYF < fVideoWindowHF - 2) {
                  iCheckPosFilterF.SetCropArea(0, fBotYF, (int)ELineType.kBottomLine);
               } else {
                  fBotYF -= dy;
               }
            }
            fPrevBotY = ry;
        } else if (fPrevBotY <= 0) {
            if (ry < fPrevBotY) {
               fBotYF += dy;

               if (fBotYF > 2 && fBotYF < fVideoWindowHF - 2) {
                  iCheckPosFilterF.SetCropArea(0, fBotYF, (int)ELineType.kBottomLine);
               } else {
                  fBotYF -= dy;
               }
            }
            fPrevBotY = ry;
         }

         // top Y line
         var ly = (short)(fGamePadState.ThumbSticks.Left.Y*120);
         dy = (short)(ly - fPrevTopY);

         if (fPrevTopY >= 0) {
            if (ly > fPrevTopY) {
               fTopYF += dy;

               if (fTopYF > 2 && fLeftXF < fVideoWindowHF - 2) {
                  iCheckPosFilterF.SetCropArea(0, fTopYF, (int)ELineType.kTopLine);
               } else {
                  fTopYF -= dy;
               }
            }
            fPrevTopY = ly;
        } else if (fPrevTopY <= 0) {
            if (ly < fPrevTopY) {
               fTopYF += dy;

               if (fTopYF > 2 && fTopYF < fVideoWindowHF - 2) {
                  iCheckPosFilterF.SetCropArea(0, fTopYF, (int)ELineType.kTopLine);
               } else {
                  fTopYF -= dy;
               }
            }
            fPrevTopY = ly;

         }

         if (!fGamePadState.Buttons.Equals(fPreviousState.Buttons)) {

            if (fGamePadState.Buttons.LeftStick == Input.ButtonState.Pressed) {
               OnZoomOutClick(null, null);
            } else if (fGamePadState.Buttons.RightStick == Input.ButtonState.Pressed) {
               OnZoomClick(null, null);
            }

            if (fGamePadState.Buttons.A == Input.ButtonState.Pressed) {
               buttonSnapClick(null, null);
            }

            if (fGamePadState.Buttons.B == Input.ButtonState.Pressed) {
               buttonLoadClick(null, null);
            }
         }
 */
      }

      private void GamePadTimerTick(object sender, EventArgs e)
      {
         UpdateControllerState();
      }

      #endregion GamePad Processing

   }
}

