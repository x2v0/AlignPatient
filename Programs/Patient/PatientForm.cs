// ***********************************************************************
// Assembly         : PatientDisplay
// Author           : onuchin
// Created          : 11-15-2013
//
// Last Modified By : onuchin
// Last Modified On : 11-15-2013
// ***********************************************************************
// <copyright file="PatientForm.cs" company="PROTOM">
//     Copyright (c) Valeriy Onuchin. All rights reserved.
// </copyright>
// <summary>
//
// </summary>
// ***********************************************************************

// $Id: PatientForm.cs 4359 2016-08-29 13:54:18Z onuchin $
//#define LOCAL_DEBUG

using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices; // DllImport, to register the filters
using System.Windows.Forms;

// our own classes
using P;
using P.DShow;
using P.Net;
using P.Net.Rtp;

using Point = System.Drawing.Point;

/// <summary>
/// The PatientDisplay namespace.
/// </summary>
namespace PatientDisplay
{
   /// <summary>
   /// Class PatientForm
   /// </summary>
    public partial class PatientForm {
      #region Constructor

       /// <summary>
       /// Initializes a new instance of the <see cref="PatientForm" /> class.
       /// </summary>
       /// <param name="args">The args.</param>
      public PatientForm(string[] args)
      {
         UnhandledExceptionHandler.Register();

         fgArguments = new ArgumentParser(args);
         InvokeArguments(fgArguments.Parameters);
         InitializeComponent();
      }

      /// <summary>
      /// Invokes the arguments.
      /// </summary>
      /// <param name="parameters">The parameters.</param>
      private static void InvokeArguments(StringDictionary parameters)
      {

      }

      #endregion
      #region Interop

      /// <summary>
      /// Keybd_events the specified b vk.
      /// </summary>
      /// <param name="bVk">The b vk.</param>
      /// <param name="bScan">The b scan.</param>
      /// <param name="dwFlags">The dw flags.</param>
      /// <param name="dwExtraInfo">The dw extra info.</param>
      [DllImport("user32.dll")]
      static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

      /// <summary>
      /// Systems the parameters info.
      /// </summary>
      /// <param name="uAction">The u action.</param>
      /// <param name="uParam">The u param.</param>
      /// <param name="lpvParam">The LPV param.</param>
      /// <param name="flags">The flags.</param>
      /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
      [DllImport("user32.dll")]
      private static extern bool SystemParametersInfo(int uAction, int uParam, ref int lpvParam, int flags);

      /// <summary>
      /// Registers the RTP filters.
      /// </summary>
      [DllImport("PRtpFilter.ax", EntryPoint="DllRegisterServer")]
      private static extern void RegisterRtpFilters();

      /// <summary>
      /// Registers the check pos filter.
      /// </summary>
      [DllImport("PCheckPosFilter.ax", EntryPoint = "DllRegisterServer")]
      private static extern void RegisterCheckPosFilter();

      /// <summary>
      /// Unregisters the check pos filter.
      /// </summary>
      [DllImport("PCheckPosFilter.ax", EntryPoint = "DllUnregisterServer")]
      private static extern void UnregisterCheckPosFilter();

      /// <summary>
      /// Finds the window.
      /// </summary>
      /// <param name="className">Name of the class.</param>
      /// <param name="windowText">The window text.</param>
      /// <returns>System.Int32.</returns>
      [DllImport( "user32.dll" )]
 	   private static extern int FindWindow(string className, string windowText);

      /// <summary>
      /// Shows the window.
      /// </summary>
      /// <param name="hwnd">The HWND.</param>
      /// <param name="command">The command.</param>
      /// <returns>System.Int32.</returns>
 	   [DllImport( "user32.dll" )]
 	   private static extern int ShowWindow(int hwnd, int command);

      /// <summary>
      /// Shows the cursor.
      /// </summary>
      /// <param name="bShow">if set to <c>true</c> [b show].</param>
      /// <returns>System.Int32.</returns>
      [DllImport("user32.dll")]
      static extern int ShowCursor(bool bShow);

      /// <summary>
      /// The S w_ HIDE
      /// </summary>
 	   private const int SW_HIDE = 0;
      /// <summary>
      /// The S w_ SHOW
      /// </summary>
 	   private const int SW_SHOW = 1;

      /// <summary>
      /// Suspends the system by shutting power down. Depending on the Hibernate parameter,
      /// the system either enters a suspend (sleep) state or hibernation (S4).
      /// </summary>
      /// <param name="hibernate">If this parameter is TRUE, the system hibernates. If the parameter is FALSE, the system is suspended.</param>
      /// <param name="forceCritical">Windows Server 2003, Windows XP, and Windows 2000:  If this parameter is TRUE,
      /// the system suspends operation immediately; if it is FALSE, the system broadcasts a PBT_APMQUERYSUSPEND event to each
      /// application to request permission to suspend operation.</param>
      /// <param name="disableWakeEvent">If this parameter is TRUE, the system disables all wake events.
      /// If the parameter is FALSE, any system wake events remain enabled.</param>
      /// <returns>If the function succeeds, the return value is true.</returns>
      /// <remarks>See http://msdn.microsoft.com/en-us/library/aa373201(VS.85).aspx</remarks>
      [DllImport("Powrprof.dll", SetLastError = true)]
      static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

      /// <summary>
      /// Blocks the input.
      /// </summary>
      /// <param name="blockIt">if set to <c>true</c> [block it].</param>
      /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
      [DllImport("user32.dll")]
      static extern bool BlockInput(bool blockIt);

      /// <summary>
      /// The S c_ MONITORPOWER
      /// </summary>
      private int SC_MONITORPOWER = 0xF170;
      /// <summary>
      /// The W m_ SYSCOMMAND
      /// </summary>
      private uint WM_SYSCOMMAND = 0x0112;
      /// <summary>
      /// The HWN d_ BROADCAST
      /// </summary>
      private const int HWND_BROADCAST = 0xFFFF;

      /// <summary>
      /// Sends the message.
      /// </summary>
      /// <param name="hWnd">The h WND.</param>
      /// <param name="Msg">The MSG.</param>
      /// <param name="wParam">The w param.</param>
      /// <param name="lParam">The l param.</param>
      /// <returns>IntPtr.</returns>
      [DllImport("user32.dll")]
      static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);


      #endregion Interop
      #region Statics / App.Config overrides

      /// <summary>
      /// Initializes static members of the <see cref="PatientForm" /> class.
      /// </summary>
      static PatientForm()
      {      
         RegisterRtpFilters();
         RegisterCheckPosFilter();

         Session.AppConfig();
      }   

      #endregion Statics / App.Config overrides
      #region Members

      #region Receiving side / Playing graph

      /// <summary>
      /// Receives the data across the network
      /// </summary>
      private RtpStream fRtpStream;

      //internal int fBorderWidth;
      //internal int fBorderHeight;
      /// <summary>
      /// The f UI state
      /// </summary>
      protected int fUiState;
      #endregion Receiving side / Playing graph

      /// <summary>
      /// Check if form invoke is required and invoke the form on the UI thread
      /// (form.Invoke if not on UI thread, or on the current thread if we are
      /// on the UI thread)
      /// </summary>
      /// <param name="handle">Auxilary parameter sometimes used as switching option</param>
      protected delegate void VoidDelegateInt(int handle);
      /// <summary>
      /// Delegate VoidDelegateArr
      /// </summary>
      /// <param name="arr">The arr.</param>
      protected delegate void VoidDelegateArr(byte[] arr);
      /// <summary>
      /// Delegate VoidDelegate
      /// </summary>
      protected delegate void VoidDelegate();
      /// <summary>
      /// Delegate DelegateFileReceived
      /// </summary>
      /// <param name="e">The e.</param>
      protected delegate void DelegateFileReceived(OnFileReceivedArgs e);

      /// <summary>
      /// The last effect
      /// </summary>
      /// default effect  //IDC_RADIO_Snap = 1001 form PCheckPosFilter resource.h
      protected int fLastEffect = 1001;
      /// <summary>
      /// The f snapshot done
      /// </summary>
      protected bool fSnapshotDone;

      /// <summary>
      /// command line parcer
      /// </summary>
      private static ArgumentParser fgArguments;

      /// <summary>
      /// Disable screen saver timer
      /// </summary>
      private Timer fTimer = new Timer();

      /// <summary>
      /// The f video cursor catched
      /// </summary>
      private bool fVideoCursorCatched;
      /// <summary>
      /// The f pressed line type
      /// </summary>
      private ELineType fPressedLineType = ELineType.kNonLine;
      /// <summary>
      /// The f zoomed
      /// </summary>
      private bool fZoomed;
      /// <summary>
      /// The f check mode
      /// </summary>
      private bool fCheckMode;

      // frontal/profile camera
      /// <summary>
      /// The f frontal
      /// </summary>
      private bool fFrontal = true;

      // profile frame y location
      /// <summary>
      /// The k yoffset
      /// </summary>
      private const int kYoffset = 27;

      // camera index 
      /// <summary>
      /// The f idx
      /// </summary>
      private int fIdx; 

      #endregion Members
      #region OnLoad/OnClosing events handlers

      /// <summary>
      /// Handles the Load event of the PatientDisplayForm control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private void PatientDisplayForm_Load(object sender, EventArgs e)
      {
         // called when form is loaded

         bool ret = Session.FindProcess("PatientDisplay");

         // new instance started
         Session.ProgramNumber++;
         fIdx = Session.ProgramNumber - 1;

         Session.PatientCName += Session.ProgramNumber;
         Session.CallingForm = this;

         HookRtpEvents();

         CreateSender();

         // resize form
         Width = Session.PDispWidth + 2;
         Height = Session.PDispHeight + kYoffset + 1;

         if(Session.Doctor == null) {
            // read IP address & port from registry
            var ip = Reg.ReadValue(Reg.Doctor, Reg.DoctorIP);

            if(ip != null) {
               Session.DoctorIP = (string)ip;
            }
            /*
            var port = Reg.ReadValue(Reg.Doctor, Reg.DoctorPort);

            if (port != null) {
               Session.PulsePort= (int)port;
            }

            // open serial port and connect it to the saved IP address
            OpenComPort();
*/
         }

         if (Session.ProgramNumber == 2) {   // profile camera
            fFrontal = false;
            ProfileFrame();
         } else {
            FrontalFrame();
         }

         //  fRefFileName defined in ReferenceImage.cs
         gRefFileName += ".jpg;" + Session.ProgramNumber;
         fRefFilePath = Path.Combine(Path.GetTempPath(), gRefFileName);

         //InitLines();

         if (fTimer != null) {
            fTimer.Enabled = true;
            //Int32 period = (GetScreenSaverTimeout() - 1)*1000;
            fTimer.Interval = 60000;   // 1 min
            fTimer.Tick += HandleTimer;
            fTimer.Start();
         }
      }

      /// <summary>
      /// Frontals the frame.
      /// </summary>
      private void FrontalFrame()
      {
         fFrontal = true;
         fVideoWindow.SetWindowPosition(-Session.ProfileWidth, 0,
                                        Session.PDispWidth, Session.PDispHeight);
      }

      /// <summary>
      /// Profiles the frame.
      /// </summary>
      private void ProfileFrame()
      {
         fFrontal = false;
         toolStrip.Hide();
         fPictureBox.Location = new Point(0, 0);

         Height -= kYoffset;
         Width = Session.ProfileWidth;
         FormBorderStyle = FormBorderStyle.None;
         Location = new Point(Session.PDispWidth - Session.ProfileWidth, kYoffset);

         fVideoWindow.SetWindowPosition((Session.ProfileWidth - Session.PDispWidth)/2, 0,
                                         Session.PDispWidth, Session.PDispHeight);

         TopMost = true;
         Focus();
         BringToFront();
      }

      /// <summary>
      /// Creates the sender.
      /// </summary>
      private void CreateSender()
      {

         int ncamera = 0;

         foreach (FilterInfo fi in Enumerable.Reverse(VideoSource.Sources())) {
            ncamera++;

            if (ncamera == Session.ProgramNumber) {
               fVideoFilterInfo = fi;  // defined in Video.cs
               break;
            }
         }

         if (ncamera != Session.ProgramNumber) {   //
            //MessageBox.Show("All video cameras assigned");
            Application.Exit();
            return;
         }

         foreach (FilterInfo fi in AudioSource.Sources()) {
            fMicFilterInfo = fi; // the very first microphone
            break;
         }

         foreach (FilterInfo fi in AudioRenderer.Renderers()) {
            fSpeakerFI = fi; // the very first renderer
            break;
         }

         Session.JoinRtpSession(Session.PatientCName);

         CreateVideoCaptureGraph(); // defined in Video.cs 
         ActivateMicrophone();      // defined in AudioP.cs

         if (ncamera==1) {
            Process.Start("PatientDisplay.exe");
         }
      }

      /// <summary>
      /// Handles the Closing event of the PatientDisplayForm control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data.</param>
      private void PatientDisplayForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {

         try {
           // Check to see if we are minimized, and if so, restore before saving settings
           // Otherwise when relaunching, the form will "appear" off screen
           if (WindowState == FormWindowState.Minimized) {
              WindowState = FormWindowState.Normal;
           }
         } catch { }

         Cleanup();
      }

      #endregion OnLoad/OnClosing events handlers
      #region Rtp/Rtcp Events Handlers

      /// <summary>
      /// Hooks the RTP events.
      /// </summary>
      private void HookRtpEvents()
      {
         RtpEvents.RtpParticipantAdded += OnNewParticipant;
         RtpEvents.RtpParticipantRemoved += ParticipantRemoved;
         RtpEvents.RtpStreamAdded += RtpStreamAdded;
         RtpEvents.RtpStreamRemoved += RtpStreamRemoved;
         RtpEvents.AppPacketReceived += AppPacketReceived;
         RtpStream.FirstFrameReceived += FirstFrameReceived;
         RtpEvents.HiddenSocketException += RaiseHiddenSocket;

#if LOCAL_DEBUG
         RtpEvents.InvalidPacket += RaiseInvalidPacket;
         RtpEvents.NetworkTimeout += RaiseNetworkTimeout;
         RtpEvents.RtpParticipantTimeout += RaiseParticipantTimeout;
         RtpEvents.RtpStreamTimeout += RaiseCapabilityTimeout;

         //RtpEvents.FrameOutOfSequence += new RtpEvents.FrameOutOfSequenceEventHandler(RaiseFrameOutOfSequence);
         //RtpEvents.InvalidPacketInFrame += new RtpEvents.InvalidPacketInFrameEventHandler(RaiseInvalidPacket);
         //RtpEvents.PacketOutOfSequence += new RtpEvents.PacketOutOfSequenceEventHandler(RaisePacketOutOfSequence);
         //RtpEvents.DuplicateCNameDetected += new RtpEvents.DuplicateCNameDetectedEventHandler(RaiseDuplicateIdentityDetected);
#endif
      }

      /// <summary>
      /// Raised when network connection is broken
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="ea">The <see cref="RtpEvents.HiddenSocketExceptionEventArgs" /> instance containing the event data.</param>
      private void RaiseHiddenSocket(object sender, RtpEvents.HiddenSocketExceptionEventArgs  ea)
      {
         
#if LOCAL_DEBUG
         string msg = "Hidden Socket Exception";
         //msg += String.Format("{0} : {1}", p.CName, p.IPAddress);
         MessageBox.Show(msg);
#endif
         Session.NetworkWasBroken = true;

         // turn off display power when doctor is living session
         //SendMessage((IntPtr)HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, 2);

         // unblock mouse, keyboard input
         //BlockInput(false);

         // stop video processing
         fVideoCaptureGraph.Stop();

         // sleep for 5 minutes and then terminate application
         //System.Threading.Thread.Sleep(1000*300);
         Application.Exit();
    }

#if LOCAL_DEBUG
      private void RaiseInvalidPacket(object sender, RtpEvents.InvalidPacketEventArgs ea) 
      {
         string msg = "InvalidPacket ";
         //msg += String.Format("{0} : {1}", p.CName, p.IPAddress);
         MessageBox.Show(msg);
      }

      private void RaiseParticipantTimeout(object sender, RtpEvents.RtpParticipantEventArgs ea) 
      {
         RtpParticipant p = ea.RtpParticipant;
         string msg = "ParticipantTimeout ";
         msg += String.Format("{0} : {1}", p.CName, p.IPAddress);
         MessageBox.Show(msg);
      }

      private void RaiseNetworkTimeout(object sender, RtpEvents.NetworkTimeoutEventArgs ea) 
      {
         string msg = "NetworkTimeout ";
         //msg += String.Format("{0} : {1}", p.CName, p.IPAddress);
         MessageBox.Show(msg);
      }

      private void RaiseCapabilityTimeout(object sender, RtpEvents.RtpStreamEventArgs ea) 
      {
         string msg = "StreamTimeout ";
         //msg += String.Format("{0} : {1}", p.CName, p.IPAddress);
         MessageBox.Show(msg);
      }
#endif

      /// <summary>
      /// Unhooks the RTP events.
      /// </summary>
      private void UnhookRtpEvents()
      {
         RtpEvents.RtpParticipantAdded -= OnNewParticipant;
         RtpEvents.RtpParticipantRemoved -= ParticipantRemoved;
         RtpEvents.RtpStreamAdded -= RtpStreamAdded;
         RtpEvents.RtpStreamRemoved -= RtpStreamRemoved;
         RtpEvents.AppPacketReceived -= AppPacketReceived;
         RtpStream.FirstFrameReceived -= FirstFrameReceived;
         RtpEvents.HiddenSocketException -= RaiseHiddenSocket;

         //RtpEvents.InvalidPacket -= new RtpEvents.InvalidPacketEventHandler(RaiseInvalidPacket);
         //RtpEvents.NetworkTimeout -= new RtpEvents.NetworkTimeoutEventHandler(RaiseNetworkTimeout);
         //RtpEvents.RtpParticipantTimeout -= new RtpEvents.RtpParticipantTimeoutEventHandler(RaiseParticipantTimeout);
         //RtpEvents.RtpStreamTimeout -= new RtpEvents.RtpStreamTimeoutEventHandler(RaiseCapabilityTimeout);
         //RtpEvents.FrameOutOfSequence -= new RtpEvents.FrameOutOfSequenceEventHandler(RaiseFrameOutOfSequence);
         //RtpEvents.InvalidPacketInFrame -= new RtpEvents.InvalidPacketInFrameEventHandler(RaiseInvalidPacket);
         //RtpEvents.PacketOutOfSequence -= new RtpEvents.PacketOutOfSequenceEventHandler(RaisePacketOutOfSequence);
         //RtpEvents.DuplicateCNameDetected -= new RtpEvents.DuplicateCNameDetectedEventHandler(RaiseDuplicateIdentityDetected);

         // unhook also TCP events
         if (fTcpSenderImage != null) {
            fTcpSenderImage.OnSendFileComplete -= SendFileComplete;
         }

         if (fTcpReceiverImage != null) {
            fTcpReceiverImage.OnFileReceived -= FileReceived;
         }
      }

      /// <summary>
      /// RTPs the stream added.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="ea">The <see cref="RtpEvents.RtpStreamEventArgs" /> instance containing the event data.</param>
      private void RtpStreamAdded(object sender, RtpEvents.RtpStreamEventArgs ea)
      {

         lock (this) {
            var stream = ea.RtpStream;
            var p = stream.Properties;
            var payload = stream.PayloadType;
            var name = p.Name;
            var cname = p.CName;

   #if LOCAL_DEBUG
            string str = "New stream added: ";
            str += "CName=" + p.CName + " Name= " + p.Name;
            str += " Payload type=" + payload.ToString();
            MessageBox.Show(str);
   #endif

            if ((payload == PayloadType.dynamicVideo) &&
                (cname == Session.DoctorCName)) {
               fRtpStream = stream;

               // Tell the stream we will poll it for data with our own (DShow) thread
               // Instead of receiving data through the FrameReceived event
               fRtpStream.IsUsingNextFrame = true;

            } else if ((payload == PayloadType.dynamicAudio) &&
                       (cname == Session.DoctorCName)) {
               fRtpAudioStream = stream; // defined in AudioP.cs
               fRtpAudioStream.IsUsingNextFrame = true;
            }
         }
      }

      /// <summary>
      /// RTPs the stream removed.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="ea">The <see cref="RtpEvents.RtpStreamEventArgs" /> instance containing the event data.</param>
      private void RtpStreamRemoved(object sender, RtpEvents.RtpStreamEventArgs ea)
      {

         var stream = ea.RtpStream;
         var p = stream.Properties;
         var pt = stream.PayloadType;
         var cname = p.CName;

#if LOCAL_DEBUG
         string str = "Stream removed: ";
         str += "CName=" + p.CName + " Name= " + p.Name;
         str += " Payload type=" + pt.ToString();
         MessageBox.Show(str);
#endif
      }

      /// <summary>
      /// Handle new participant event
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="ea">The <see cref="RtpEvents.RtpParticipantEventArgs" /> instance containing the event data.</param>
      private void OnNewParticipant(object sender, RtpEvents.RtpParticipantEventArgs ea)
      {
         RtpParticipant p = ea.RtpParticipant;

#if LOCAL_DEBUG
         string msg = "New participant ";
         msg += String.Format("{0} : {1}", p.CName, p.IPAddress);
         MessageBox.Show(msg);
#endif

         if (p.CName == Session.DoctorCName) {

           // if network connection was once broken we need to restart application (done by XYNTService) 
           if (Session.NetworkWasBroken) {
               // sleep for 5 minutes and then terminate application
               //System.Threading.Thread.Sleep(1000*300);
               Application.Exit();
            }
            Session.Participant = Session.Doctor = p;

            Invoke(new MethodInvoker(Reset), null);

            if (fTcpReceiverImage == null) {
               fTcpReceiverImage = new TcpReceiver(Session.TcpPort);
               fTcpReceiverImage.Start();
               fTcpReceiverImage.OnFileReceived += FileReceived;
            }

            // save Doctor's IP address & port to registry
            Session.DoctorIP = Session.Doctor.IPAddress.ToString();

            // read IP address & port from registry
            var ip = Reg.ReadValue(Reg.Doctor, Reg.DoctorIP);

            string regIp = null;

            if (ip != null) {
               regIp = (string)ip;
            }

            // reopen com port
            if (regIp != Session.DoctorIP) {
               OpenComPort();
            }

            Reg.WriteValue(Reg.Doctor, Reg.DoctorIP, Session.DoctorIP);
            Reg.WriteValue(Reg.Doctor, Reg.DoctorPort, Session.PulsePort);

            // block mouse, keyboard input
            //BlockInput(true);

            // restart video processing
            fVideoCaptureGraph.Run();

            // restart audio
            //ResumePlayingAudio();

            // awake monitor
            SendMessage((IntPtr)HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, -1);

            PressScrollLock();

            if (fTimer != null) {
               fTimer.Start();
            }

            if (fGamePadTimer != null) {
               fGamePadTimer.Start();
            }
         }
#if LOCAL_DEBUG
         string str = "New participant ";
         str += p.CName + " : " + p.IPAddress;
         MessageBox.Show(str);
#endif
      }

      /// <summary>
      /// Called when [file received].
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The e.</param>
      private void FileReceived(object sender, OnFileReceivedArgs e)
      {

#if LOCAL_DEBUG
            MessageBox.Show(Strings.ReferenceFileReceived);
#endif

         File.WriteAllBytes(fRefFilePath, e.Buffer);
#if LOCAL_DEBUG
         MessageBox.Show(Strings.ReferenceFileWritten + fRefFilePath);
#endif

         Invoke(new MethodInvoker(SetReferenceImage), null);
      }

      /// <summary>
      /// Handle event when participant leave the session
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="ea">The <see cref="RtpEvents.RtpParticipantEventArgs" /> instance containing the event data.</param>
      private void ParticipantRemoved(object sender, RtpEvents.RtpParticipantEventArgs ea)
      {
         var p = ea.RtpParticipant;
#if LOCAL_DEBUG
         string str = "Participant leaved ";
         str += p.CName + " : " + p.IPAddress;
         MessageBox.Show(str);
#endif
         if (p.CName == Session.DoctorCName) {      
            Invoke(new MethodInvoker(Reset), null);
            CloseComPort();

            if (fTimer != null) {
               // stop disable screen saver timer
               fTimer.Stop();
            }

            if (fGamePadTimer != null) {
               fGamePadTimer.Stop();
            }

            fVideoWindow.Visible = 0;

            // turn off display power when doctor is living session
            SendMessage((IntPtr)HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, 2);

            // unblock mouse, keyboard input
            //BlockInput(true);

            // stop video processing
            fVideoCaptureGraph.Stop();

            Application.Exit();
         } 
      }

      /// <summary>
      /// Firsts the frame received.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="ea">The <see cref="EventArgs" /> instance containing the event data.</param>
      private void FirstFrameReceived(object sender, EventArgs ea)
      {
         lock (fGraphMngLock) {
            var stream = (RtpStream)sender;
            var prop = stream.Properties;
            var name = prop.Name;
            var cname = prop.CName;
            var pt = stream.PayloadType;

            var idx = cname == Session.PatientCName ? 1 : 0;

#if LOCAL_DEBUG
      MessageBox.Show(String.Format("FirstFrameReceived - {0} : {1} : {2}", cname, pt, name));
#endif
            if (pt == PayloadType.dynamicVideo) { // video

               if (cname == Session.PatientCName) {
                  //Session.FormInvoke(new VoidDelegateInt(CreateDoctorVideoGraph), new Object[] { 1 });

               } else if (cname == Session.DoctorCName) {
                  //Session.FormInvoke(new VoidDelegateInt(CreateDoctorVideoGraph), new Object[] { 0 });
               }
            } else if (pt == PayloadType.dynamicAudio) {
               if (cname == Session.DoctorCName) {
                  Session.FormInvoke(new VoidDelegate(CreateAudioRenderGraph), null); // defined in AudioP.cs
               }
            }
         }
      }

      /// <summary>
      /// Apps the packet received.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="ea">The <see cref="RtpEvents.AppPacketReceivedEventArgs" /> instance containing the event data.</param>
      private void AppPacketReceived(object sender, RtpEvents.AppPacketReceivedEventArgs ea)
      {
         // Process RTCP APP packets which used for command processing

         uint ssrc = ea.SSRC;
         if (Session.Doctor.SSRC != ssrc) {
            return;
         }

         string name  = ea.Name;
         byte subtype = ea.Subtype;
         byte[] data  = ea.Data;

         if (name == "SNAP") {
            Invoke(new MethodInvoker(MakeVideoSnap), null);
         } else if (name == "SAVE") {
            if (fSnapshotDone == false) {
               Invoke(new MethodInvoker(MakeVideoSnap), null);
               System.Threading.Thread.Sleep(500); // sleep 0.5 sec
            }
            Invoke(new MethodInvoker(SendReferenceImage), null);
         } else if (name == "SHOW") {
            if (subtype == 1) {
               Invoke(new VoidDelegateInt(ChangeEffect), new Object[] { PCheckPosFilterEffect.IDC_RADIO_Snap });
            } else if (subtype == 2) {
               Invoke(new VoidDelegateInt(ChangeEffect), new Object[] { PCheckPosFilterEffect.IDC_RADIO_Blend } );
            } else {
               // do not display diff  //IDC_RADIO_None = 1000 form PCheckPosFilter resource.h
               Invoke(new VoidDelegateInt(ChangeEffect), new Object[] { PCheckPosFilterEffect.IDC_RADIO_None } );
            }
         } else if (name == "BLCK") {
            Invoke(new VoidDelegateInt(ScreenSave), new Object[] { subtype });
         } else if (name == "THRS") {
            Invoke(new VoidDelegateArr(SetThreshold), data);
         } else if (name == "RXYW") {
            bool invoke = (subtype == 0 && fFrontal) || (subtype == 1 && !fFrontal);

            if (invoke) {
               Invoke(new VoidDelegateArr(SetROI), data);
            }
         } else if (name == "RSET") {
            Invoke(new MethodInvoker(Reset), null);
         }
      }

      #endregion  Rtp/Rtcp Events Handlers
      #region Private Methods

      /// <summary>
      /// send reference image to doctor
      /// </summary>
      private void MakeVideoSnap()
      {

         //iCheckPosFilter.Reset();

         if (!Directory.Exists(Path.GetTempPath())) {
            try {
               Directory.CreateDirectory(Path.GetTempPath());
            } catch (IOException e) {
               MessageBox.Show(e.Message);
            }
         }

         iCheckPosFilter.Snapshot(fRefFilePath);
         fSnapshotDone = true;
      }

      /// <summary>
      /// Resets this instance.
      /// </summary>
      private void Reset()
      {
         iCheckPosFilter.Reset();
         //InitLines();
      }

      /// <summary>
      /// Changes the effect.
      /// </summary>
      /// <param name="effect">The effect.</param>
      private void ChangeEffect(int effect)
      {
         iCheckPosFilter.GetEffect(out fLastEffect);
         iCheckPosFilter.SetEffect(effect);
      }

      /// <summary>
      /// Send DoctorDisplay.config RoiXF, RoiYF, RoiWidthF settings to the Patient machine
      /// </summary>
      private void SetXYW()
      {

         //if (fParticipant == null || !Session.Connected) {
         //   MessageBox.Show(Strings.NoPatientConnection);
         //   return;
         //}

         //uint ssrc = fParticipant.SSRC;

         // send ROI data
         int roi = Session.RoiWidth[fIdx] +
                   Session.RoiX[fIdx]*1000 + 
                   Session.RoiY[fIdx]*1000000;

         byte[] arr = BitConverter.GetBytes(roi);

         uint ssrc = Session.Participant.SSRC;

         Session.RtpSession.SendAppPacket(ssrc, "RXYW", 0, arr, Rtcp.RtcpInterval.Now);

         SetROI(arr);
      }

      private void ZoomTest()
      {
         iCheckPosFilter.SetCropArea(40, 40, 200);
      }

      /// <summary>
      /// Sets the ROI.
      /// </summary>
      /// <param name="roi">The roi.</param>
      private void SetROI(byte[] roi)
      {

         if (roi == null) {
            return;
         }

         // suspend video processing
         fVideoCaptureGraph.Stop();

         int v = BitConverter.ToInt32(roi, 0);
         Session.RoiY[fIdx] = (short)(v/1000000);
         Session.RoiX[fIdx] = (short)((v - Session.RoiY[fIdx]*1000000)/1000);
         Session.RoiWidth[fIdx] = (short)(v - Session.RoiY[fIdx]*1000000 - 
                                              Session.RoiX[fIdx]*1000);

         if (Session.RoiWidth[fIdx] < 20) {
            int ww;
            iCheckPosFilter.GetWidth(out ww);

            Session.RoiWidth[fIdx] = (short)ww;
            Session.RoiX[fIdx] = 0;
            Session.RoiY[fIdx] = 0;
         }

         iCheckPosFilter.Reset();
         //MessageBox.Show("qq " + fIdx + " " + Session.RoiX[fIdx] + " " + Session.RoiY[fIdx] + " " + Session.RoiWidth[fIdx]);
         iCheckPosFilter.SetCropArea(Session.RoiX[fIdx],
                                     Session.RoiY[fIdx],
                                     Session.RoiWidth[fIdx]);
         //ZoomTest();

         // resume video processing
         fVideoCaptureGraph.Run();
      }

      /// <summary>
      /// Sets the threshold.
      /// </summary>
      /// <param name="thresh">The thresh.</param>
      private void SetThreshold(byte[] thresh)
      {

         if (thresh == null) {
            return;
         }

         int value = BitConverter.ToInt32(thresh, 0);
         iCheckPosFilter.SetThreshold(value);
      }

      /// <summary>
      /// Screens the save.
      /// </summary>
      /// <param name="on">The on.</param>
      private void ScreenSave(int on)
      {
         if (on != 0) {
            if (fTimer != null) {
               fTimer.Stop();
            }

            //SendMessage((IntPtr) HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, 2);
         } else {
            SendMessage((IntPtr) HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, -1);

            if (fTimer != null) {
               fTimer.Start();
            }
         }
      }

      /// <summary>
      /// Gets the screen saver timeout.
      /// </summary>
      /// <returns>Int32.</returns>
      private static Int32 GetScreenSaverTimeout()
      {
         Int32 value = 0;
         SystemParametersInfo(14, 0, ref value, 0);
         return value;
      }

      /// <summary>
      /// Presses the scroll lock.
      /// </summary>
      private static void PressScrollLock()
      {
         const byte vkScroll = 0x91;
         const byte keyeventfKeyup = 0x2;

         keybd_event(vkScroll, 0x45, 0, (UIntPtr)0);
         keybd_event(vkScroll, 0x45, keyeventfKeyup, (UIntPtr)0);
      }

      /// <summary>
      /// Handles the timer.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
      private static void HandleTimer(object sender, EventArgs e)
      {
 	      PressScrollLock();
 	      PressScrollLock();
 	   }

      #endregion Private
      #region Dispose/Clean Methods

      /// <summary>
      /// Disposes of the resources (other than memory) used by the <see cref="T:System.Windows.Forms.Form" />.
      /// </summary>
      /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing) {
            if (components != null) {
              components.Dispose();
            }
         }
         base.Dispose(disposing);
         Cleanup();
      }

      /// <summary>
      /// Leaves the session.
      /// </summary>
      private void LeaveSession()
      {

         UnhookRtpEvents();

         Session.Leave();

         if (fRtpSender != null) {
            fRtpSender.Dispose();
            fRtpSender = null;
         }

         if (fTcpSenderImage != null) {
            fTcpSenderImage.Dispose();
            fTcpSenderImage = null;
         }

         if (fTcpReceiverImage != null) {
            fTcpReceiverImage.Dispose();
            fTcpReceiverImage = null;
         } 

         CloseComPort();

         fRtpStream = null;
      }

      /// <summary>
      /// Cleanups the sender.
      /// </summary>
      private void CleanupSender()
      {
         lock (fGraphMngLock) {
            if (fFilgraphManager != null) {
               fFilgraphManager.Stop();
               FilterGraph.RemoveAllFilters(fFilgraphManager);
               //fFilgraphManager.Dispose();
               fFilgraphManager = null;
            }
         }
         DisposeVideo();
         DisposeAudio();
      }

      /// <summary>
      /// Cleanups the reciever.
      /// </summary>
      private void CleanupReciever()
      {
         lock (fGraphMngLockDoctor) {
            if (fFilgraphManagerDoctor != null) {
               if (fRtpStream != null) {
                  fRtpStream.UnblockNextFrame();
               }

               fFilgraphManagerDoctor.Stop();
               FilterGraph.RemoveAllFilters(fFilgraphManagerDoctor);
               fFilgraphManagerDoctor = null;
            }
         }
      }

      #endregion
      #region Public Methods

      /// <summary>
      /// Cleanups this instance.
      /// </summary>
      public void Cleanup()
      {
         LeaveSession();
         CleanupSender();
         CleanupReciever();

         //UnregisterCheckPosFilter();
      }

      #endregion Public

      private void OnClosing(object sender, FormClosingEventArgs e)
      {

      }

      private void OnClosed(object sender, FormClosedEventArgs e)
      {

      }

   }
}
