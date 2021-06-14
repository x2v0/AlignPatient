// ***********************************************************************
// Assembly         : DoctorDisplay
// Author           : onuchin
// Created          : 01-14-2014
//
// Last Modified By : onuchin
// Last Modified On : 01-14-2014
// ***********************************************************************
// <copyright file="Session.cs" company="PROTOM">
//     Copyright (c) PROTOM. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

// $Id: Session.cs 2549 2014-07-03 06:01:22Z onuchin $

//#define LOCAL_DEBUG

using System;
using System.Configuration;
using System.Net;
 // DllImport, to register the filters
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;
using System.Diagnostics;

using P.Net.Rtp;
using Alias = System.IO.Ports;


namespace P {

   // from ../../PCheckPosFilter/resource.h
   /// <summary>
   /// Enum PCheckPosFilterEffect
   /// </summary>
   public enum PCheckPosFilterEffect {
      /// <summary>
      /// The ID c_ RADI o_ none
      /// </summary>
      IDC_RADIO_None = 1000,
      /// <summary>
      /// The ID c_ RADI o_ snap
      /// </summary>
      IDC_RADIO_Snap = 1001,
      /// <summary>
      /// The ID c_ RADI o_ grey
      /// </summary>
      IDC_RADIO_Grey = 1002,
      /// <summary>
      /// The ID c_ RADI o_ diff
      /// </summary>
      IDC_RADIO_Diff = 1003,
      /// <summary>
      /// The ID c_ RADI o_ flip
      /// </summary>
      IDC_RADIO_Flip = 1004,    // vertical flip
      /// <summary>
      /// The ID c_ RADI o_ mirr
      /// </summary>
      IDC_RADIO_Mirr = 1005,    // mirrow flip
      /// <summary>
      /// The ID c_ RADI o_ add
      /// </summary>
      IDC_RADIO_Add  = 1006,    // accumulate mode without substract background
      /// <summary>
      /// The ID c_ RADI o_ add S
      /// </summary>
      IDC_RADIO_AddS = 1007,    // accumulate mode & substract background
      /// <summary>
      /// The ID c_ RADI o_ add D
      /// </summary>
      IDC_RADIO_AddD = 1008,    // full ("perfect") summ mode 
      /// <summary>
      /// The ID c_ RADI o_ add DD
      /// </summary>
      IDC_RADIO_AddDD = 1009,   // summ of successive frames difference
      /// <summary>
      /// The ID c_ RADI o_ blend
      /// </summary>
      IDC_RADIO_Blend = 1010,   // alfa blend
      /// <summary>
      /// The ID c_ RADI o_ last
      /// </summary>
      IDC_RADIO_Last =  1011    // last registered efect
   };

   /// <summary>
   /// Enum ELineType
   /// </summary>
   public enum ELineType { kNonLine = 0, kTopLine = -1, kBottomLine = -2, kLeftLine = -3, kRightLine = -4 };

   /// <summary>
   /// Delegate AVLogger
   /// </summary>
   /// <param name="msg">The MSG.</param>
   public delegate void AVLogger(string msg);

   /// <summary>
   /// Per-user (HKCU) registry settings
   /// </summary>
   public sealed class Reg
   {
      #region Constructor

      /// <summary>
      /// Initializes static members of the <see cref="Reg" /> class.
      /// </summary>
      static Reg()
      {
         ms = new MemoryStream();

         bf = new BinaryFormatter {
            AssemblyFormat = FormatterAssemblyStyle.Simple
         };
      }

      #endregion Constructor
      #region Static Members

      /// <summary>
      /// The bf
      /// </summary>
      public static readonly BinaryFormatter bf;
      /// <summary>
      /// The ms
      /// </summary>
      public static readonly MemoryStream ms;

      #endregion Static Members
      #region Registry Keys

      // There is a bug in the .NET Framework 1.1 that causes RegistryKey.OpenSubKey to throw if the key name
      //  passed in is >= 255 chars.  Instead, it's supposed to check if an individual subkey name (not the full
      //  path being passed in) is >= 255 chars.
      // To work around this bug, we open the gClientKey, and then open subkeys off of it.  This basically eliminates
      //  the chance of hitting the error, unless either the device moniker is excessively long, or the install path
      //  is > 200 chars to so.
      /// <summary>
      /// The g client key
      /// </summary>
      static RegistryKey gClientKey = Registry.CurrentUser.CreateSubKey(
                                      @"Software\PROTOM\AlignPatient\" + 
                                      System.Reflection.Assembly.GetEntryAssembly().CodeBase);

      /// <summary>
      /// The doctor
      /// </summary>
      public static readonly string Doctor = "Doctor\\";
      /// <summary>
      /// The doctor IP
      /// </summary>
      public static readonly string DoctorIP = "IP";
      /// <summary>
      /// The doctor port
      /// </summary>
      public static readonly string DoctorPort = "Port";

      /// <summary>
      /// The root key
      /// </summary>
      public static readonly string RootKey = "Settings\\";

      /// <summary>
      /// The selected devices
      /// </summary>
      public static readonly string SelectedDevices = RootKey + "SelectedDevices\\";
      /// <summary>
      /// The selected cameras
      /// </summary>
      public static readonly string SelectedCameras = SelectedDevices + "Cameras\\";
      /// <summary>
      /// The selected microphone
      /// </summary>
      public static readonly string SelectedMicrophone = SelectedDevices + "Microphone\\";
      /// <summary>
      /// The selected speaker
      /// </summary>
      public static readonly string SelectedSpeaker = SelectedDevices + "Speaker\\";
      /// <summary>
      /// The linked camera
      /// </summary>
      public static readonly string LinkedCamera = SelectedDevices + "LinkedCamera\\";

      /// <summary>
      /// The media type
      /// </summary>
      public static readonly string MediaType      = "MediaType";
      /// <summary>
      /// The format block
      /// </summary>
      public static readonly string FormatBlock    = "FormatBlock";

      /// <summary>
      /// The physical connector index
      /// </summary>
      public static readonly string PhysicalConnectorIndex  = "PhysicalConnectorIndex";
      /// <summary>
      /// The video standard index
      /// </summary>
      public static readonly string VideoStandardIndex      = "VideoStandardIndex";
      /// <summary>
      /// The microphone source index
      /// </summary>
      public static readonly string MicrophoneSourceIndex   = "MicrophoneSourceIndex";

      /// <summary>
      /// The compressor enabled
      /// </summary>
      public static readonly string CompressorEnabled = "CompressorEnabled";
      /// <summary>
      /// The audio compressor enabled
      /// </summary>
      public static readonly string AudioCompressorEnabled = "AudioCompressorEnabled";
      /// <summary>
      /// The video compressor enabled
      /// </summary>
      public static readonly string VideoCompressorEnabled = "VideoCompressorEnabled";
      /// <summary>
      /// The custom compression
      /// </summary>
      public static readonly string CustomCompression = "CustomCompression";
      /// <summary>
      /// The compressor bit rate
      /// </summary>
      public static readonly string CompressorBitRate = "CompressorBitRate";
      /// <summary>
      /// The compressor key frame rate
      /// </summary>
      public static readonly string CompressorKeyFrameRate = "CompressorKeyFrameRate";
      /// <summary>
      /// The compression media type index
      /// </summary>
      public static readonly string CompressionMediaTypeIndex = "CompressionMediaTypeIndex";

      /// <summary>
      /// The auto play remote audio
      /// </summary>
      public static readonly string AutoPlayRemoteAudio = "AutoPlayRemoteAudio";
      /// <summary>
      /// The auto play remote video
      /// </summary>
      public static readonly string AutoPlayRemoteVideo = "AutoPlayRemoteVideo";

      /// <summary>
      /// The audio buffer size
      /// </summary>
      public static readonly string AudioBufferSize = "AudioBufferSize";
      /// <summary>
      /// The audio buffer count
      /// </summary>
      public static readonly string AudioBufferCount = "AudioBufferCount";

      #endregion Registry Keys
      #region Methods

      /// <summary>
      /// Reads the value.
      /// </summary>
      /// <param name="key">The key.</param>
      /// <param name="valName">Name of the val.</param>
      /// <returns>System.Object.</returns>
      public static object ReadValue(string key, string valName)
      {
         object ret = null;

         using (RegistryKey rk = gClientKey.OpenSubKey(key)) {
            if (rk != null) { // Settings exist
               ret = rk.GetValue(valName);
            }
         }
         return ret;
      }

      /// <summary>
      /// Deletes the value.
      /// </summary>
      /// <param name="key">The key.</param>
      /// <param name="valName">Name of the val.</param>
      public static void DeleteValue(string key, string valName)
      {
         using (RegistryKey rk = gClientKey.OpenSubKey(key, true)) {
            if (rk != null) {
               rk.DeleteValue(valName, false);
            }
         }
      }

      /// <summary>
      /// Writes the value.
      /// </summary>
      /// <param name="key">The key.</param>
      /// <param name="valName">Name of the val.</param>
      /// <param name="val">The val.</param>
      public static void WriteValue(string key, string valName, object val)
      {
         using (RegistryKey rk = gClientKey.CreateSubKey(key)) {
            if (rk != null)  {
               rk.SetValue(valName, val);
            }
         }
     }

      /// <summary>
      /// Values the names.
      /// </summary>
      /// <param name="key">The key.</param>
      /// <returns>System.String[][].</returns>
     public static string[] ValueNames(string key)
     {
         string[] ret = null;

         using (RegistryKey rk = gClientKey.OpenSubKey(key)) {
            if (rk != null) {
               ret = rk.GetValueNames();
            }
         }

         return ret;
      }

      #endregion Methods
   }

   /// <summary>
   /// Common properties for PatientDisplay, DoctorDisplay programs
   /// </summary>
   public sealed class Session
   {
      #region Members
      /// <summary>
      /// CallingForm, if set by the calling application, is used to marshal all events onto the Form
      /// thread, making it much easier to do things.
      /// </summary>
      private static Form gCallingForm; // == fPatientForm
      /// <summary>
      /// The build number
      /// </summary>
      private static short fBuildNumber = (short)System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build;
      /// <summary>
      /// The program number
      /// </summary>
      private static byte fProgramNumber; 
#if DEBUG
      /// <summary>
      /// The UDP port
      /// </summary>
      private static int fUdpPort = fBuildNumber <= 1024  ? (short)(fBuildNumber + 1024) : fBuildNumber;
#else
      private static int fUdpPort = 2555;
#endif
      /// <summary>
      /// number of cameras
      /// </summary>
      private const int kNcams = 2;

      /// <summary>
      /// The end point
      /// </summary>
      private static IPEndPoint fEndPoint;
      /// <summary>
      /// The patient C name
      /// </summary>
      private static string fPatientCName = "PROTOM_PatientCheckPosition";
      /// <summary>
      /// The doctor C name
      /// </summary>
      private static string fDoctorCName = "PROTOM_DoctorCheckPosition";
      /// <summary>
      /// The pult C name
      /// </summary>
      private static string fPultCName = "PROTOM_PultCheckPosition";
      /// <summary>
      /// The TCP port
      /// </summary>
      private static int fTcpPort = 30040;
      /// <summary>
      /// The exhale port
      /// </summary>
      private static int fExhalePort = fTcpPort + 10;
      /// <summary>
      /// The breath port
      /// </summary>
      private static int fBreathPort = fTcpPort + 20;

      /// <summary>
      /// The local
      /// </summary>
      private static bool fLocal;
      /// <summary>
      /// The activate microphone
      /// </summary>
      private static bool fActivateMicrophone;
      /// <summary>
      /// The new session
      /// </summary>
      private static bool fNewSession = true;
      /// <summary>
      /// The data base path
      /// </summary>
      private static string fDataBasePath = @"P:\";

      /// <summary>
      /// The path to directory with patients photos
      /// </summary>
      private static string fPhotoPath = "Foto";

      /// <summary>
      /// The path to directory with patients reference images data
      /// </summary>
      private static string fHeadPath = "Head";

      /// <summary>
      /// patient pulse data read from serial COM port 
      /// </summary>
      private static int fPulsePort = fTcpPort + 30;
      /// <summary>
      /// The port name
      /// </summary>
      private static string fPortName = "COM5";
      /// <summary>
      /// The COM data file
      /// </summary>
      private static string fComDataFile = "PulseData.dat";
      /// <summary>
      /// The baud rate
      /// </summary>
      private static int fBaudRate = 115200;
      /// <summary>
      /// The data bits
      /// </summary>
      private static int fDataBits = 8;
      /// <summary>
      /// The stop bits
      /// </summary>
      private static StopBits fStopBits = StopBits.None;
      /// <summary>
      /// The parity
      /// </summary>
      private static Parity fParity = Parity.None;
      /// <summary>
      /// The handshake
      /// </summary>
      private static Handshake fHandshake = Handshake.None;

      /// <summary>
      /// The patient id
      /// </summary>
      private static int fPatientId;
      /// <summary>
      /// The patient name
      /// </summary>
      private static string fPatientName;
      /// <summary>
      /// The doctor name
      /// </summary>
      private static string fDoctorName;
      /// <summary>
      /// The study time
      /// </summary>
      private static DateTime fStudyTime;

      /// <summary>
      /// The roi X
      /// </summary>
      public static short[] RoiX = new short[kNcams];
      /// <summary>
      /// The roi Y
      /// </summary>
      public static short[] RoiY = new short[kNcams];
      /// <summary>
      /// The roi width
      /// </summary>
      public static short[] RoiWidth = new short[kNcams];
      /// <summary>
      /// The shift X
      /// </summary>
      public static short[] ShiftX = new short[kNcams];

      /// <summary>
      /// The P disp width
      /// </summary>
      public static int PDispWidth = 1024;
      /// <summary>
      /// The P disp height
      /// </summary>
      public static int PDispHeight = 600;
      /// <summary>
      /// The profile width
      /// </summary>
      public static int ProfileWidth = 320;

      /// <summary>
      /// camera parameters
      /// </summary>
      private static int fFlipVideo = 1;
      /// <summary>
      /// The threshold
      /// </summary>
      private static int fThreshold = 50;

      /// <summary>
      /// Participant of RtpSession
      /// </summary>
      private static RtpParticipant fParticipant;
      /// <summary>
      /// The location
      /// </summary>
      private static string fLocation;

      /// <summary>
      /// Manages the connection to a multicast address and all the objects related to Rtp
      /// </summary>
      private static RtpSession fRtpSession;

      /// <summary>
      /// The network was broken
      /// </summary>
      private static bool fNetworkWasBroken;

      /// <summary>
      /// The connected
      /// </summary>
      private static bool fConnected;

      /// <summary>
      /// Doctor's Rtp SSDES data
      /// </summary>
      private static RtpParticipant fDoctor;

      /// <summary>
      /// Doctor's IP addrres
      /// </summary>
      private static string  fDoctorIP;

      #endregion Members
      #region Public

      /// <summary>
      /// Gets or sets a value indicating whether [network was broken].
      /// </summary>
      /// <value><c>true</c> if [network was broken]; otherwise, <c>false</c>.</value>
      public static bool NetworkWasBroken
      {
         get { return fNetworkWasBroken; }
         set { fNetworkWasBroken = value; }
      }

      /// <summary>
      /// Gets or sets a value indicating whether this <see cref="Session"/> is connected.
      /// </summary>
      /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
      public static bool Connected
      {
         get { return fConnected; }
         set { fConnected = value; }
      }

      /// <summary>
      /// Gets or sets the flip video.
      /// </summary>
      /// <value>The flip video.</value>
      public static int FlipVideo
      {
         get { return fFlipVideo; }
         set { fFlipVideo = value; }
      }

      /// <summary>
      /// Gets or sets a value indicating whether [activate microphone].
      /// </summary>
      /// <value><c>true</c> if [activate microphone]; otherwise, <c>false</c>.</value>
      public static bool ActivateMicrophone
      {
         get { return fActivateMicrophone; }
         set { fActivateMicrophone = value; }
      }

      /// <summary>
      /// Gets or sets a value indicating whether [new session].
      /// </summary>
      /// <value><c>true</c> if [new session]; otherwise, <c>false</c>.</value>
      public static bool NewSession
      {
         get { return fNewSession; }
         set { fNewSession = value; }
      }

      /// <summary>
      /// Gets or sets the threshold.
      /// </summary>
      /// <value>The threshold.</value>
      public static int Threshold
      {
         get { return fThreshold; }
         set { fThreshold = value; }
      }

      /// <summary>
      /// Gets or sets the patient id.
      /// </summary>
      /// <value>The patient id.</value>
      public static int PatientId
      {
         get { return fPatientId; }
         set { fPatientId = value; }
      }

      /// <summary>
      /// Gets or sets the name of the patient.
      /// </summary>
      /// <value>The name of the patient.</value>
      public static string PatientName
      {
         get { return fPatientName; }
         set { fPatientName = value; }
      }

      /// <summary>
      /// Gets or sets the name of the doctor.
      /// </summary>
      /// <value>The name of the doctor.</value>
      public static string DoctorName
      {
         get { return fDoctorName; }
         set { fDoctorName = value; }
      }

      /// <summary>
      /// The time when reference image was created
      /// </summary>
      /// <value>The study time.</value>
      public static DateTime StudyTime
      {
         get { return fStudyTime; }
         set { fStudyTime = value; }
      }

      /// <summary>
      /// Tcp Port used to transfer reference image
      /// </summary>
      /// <value>The TCP port.</value>
      public static int TcpPort
      {
         get { return fTcpPort; }
         set { fTcpPort = value; }
      }

      /// <summary>
      /// Tcp Port used to transfer patient pulse data
      /// </summary>
      /// <value>The pulse port.</value>
      public static int PulsePort
      {
         get { return fPulsePort; }
         set { fPulsePort = value; }
      }

      /// <summary>
      /// Tcp Port used to transfer exhale detection
      /// </summary>
      /// <value>The exhale port.</value>
      public static int ExhalePort
      {
         get { return fExhalePort; }
         set { fExhalePort = value; }
      }

      /// <summary>
      /// Tcp Port used to transfer patient breath data
      /// </summary>
      /// <value>The breath port.</value>
      public static int BreathPort
      {
         get { return fBreathPort; }
         set { fBreathPort = value; }
      }

      /// <summary>
      /// Property to hold the BaudRate
      /// </summary>
      /// <value>The baud rate.</value>
      public static int BaudRate
      {
         get { return fBaudRate; }
         set { fBaudRate = value; }
      }

      /// <summary>
      /// property to hold the Parity
      /// </summary>
      /// <value>The parity.</value>
      public static Parity Parity
      {
         get { return fParity; }
         set { fParity = value; }
      }

      /// <summary>
      /// property to hold the StopBits
      /// </summary>
      /// <value>The stop bits.</value>
      public static StopBits StopBits
      {
         get { return fStopBits; }
         set { fStopBits = value; }
      }

      /// <summary>
      /// property to hold the DataBits
      /// </summary>
      /// <value>The data bits.</value>
      public static int DataBits
      {
         get { return fDataBits; }
         set { fDataBits = value; }
      }

      /// <summary>
      /// property to hold the Serial Port Name
      /// </summary>
      /// <value>The name of the port.</value>
      public static string PortName
      {
         get { return fPortName; }
         set { fPortName = value; }
      }

      /// <summary>
      /// Gets the COM data file.
      /// </summary>
      /// <value>The COM data file.</value>
      public static string ComDataFile
      {
         get { return fComDataFile; }
      }

      /// <summary>
      /// property to hold the PortName
      /// </summary>
      /// <value>The handshake.</value>
      public static Handshake Handshake
      {
         get { return fHandshake; }
         set { fHandshake = value; }
      }

      /// <summary>
      /// if Local is true the local directory is used to save/load reference images
      /// e.g. C:\Documents and Settings\onuchin\Application Data\AlignPatientDB
      /// </summary>
      /// <value><c>true</c> if local; otherwise, <c>false</c>.</value>
      public static bool Local
      {
         get { return fLocal; }
         set { fLocal = value; }
      }

      /// <summary>
      /// if Local is true the local directory is used to save/load reference images
      /// e.g. C:\Documents and Settings\onuchin\Application Data\AlignPatientDB
      /// </summary>
      /// <value>The data base path.</value>
      public static string DataBasePath
      {
         get { return fDataBasePath; }
         set { fDataBasePath = value; }
      }

      public static string HeadPath
      {
         get { return fHeadPath; }
      }

      public static string PhotoPath
      {
         get { return fPhotoPath; }
      }

      /// <summary>
      /// Gets or sets the name of the patient C.
      /// </summary>
      /// <value>The name of the patient C.</value>
      public static string PatientCName
      {
         get { return fPatientCName; }
         set { fPatientCName = value; }
      }

      /// <summary>
      /// Gets the name of the doctor C.
      /// </summary>
      /// <value>The name of the doctor C.</value>
      public static string DoctorCName
      {
         get { return fDoctorCName; }
      }

      /// <summary>
      /// Gets the name of the pult C.
      /// </summary>
      /// <value>The name of the pult C.</value>
      public static string PultCName
      {
         get { return fPultCName; }
      }

      /// <summary>
      /// Gets the RTP session.
      /// </summary>
      /// <value>The RTP session.</value>
      public static RtpSession RtpSession
      {
         get { return fRtpSession; }
      }

      /// <summary>
      /// Gets or sets the calling form.
      /// </summary>
      /// <value>The calling form.</value>
      public static Form CallingForm
      {
         get { return gCallingForm; }
         set { gCallingForm = value; }
      }

      /// <summary>
      /// Location corresponds to the Rtcp property LOC for the LocalParticipant.
      /// See RFC 1889 for a definition of LOC.
      /// </summary>
      /// <value>The location.</value>
      public static string Location
      {
         get { return fLocation; }
         set { fLocation = value; }
      }

      /// <summary>
      /// Return the Participant that corresponds to the person
      /// running this instance of the application.
      /// </summary>
      /// <value>The participant.</value>
      public static RtpParticipant Participant
      {
         get { return fParticipant; }
         set { fParticipant = value; }
      }

      /// <summary>
      /// Return the Participant of doctor
      /// </summary>
      /// <value>The doctor.</value>
      public static RtpParticipant Doctor
      {
         get { return fDoctor; }
         set { fDoctor = value; }
      }

      /// <summary>
      /// Return the doctor's IP address
      /// </summary>
      /// <value>The doctor IP.</value>
      public static string DoctorIP
      {
         get { return fDoctorIP; }
         set { fDoctorIP = value; }
      }

      /// <summary>
      /// Get/Set the endpooint
      /// </summary>
      /// <value>The end point.</value>
      public static IPEndPoint EndPoint
      {
         get { return fEndPoint; }
         set { fEndPoint = value; }
      }

      /// <summary>
      /// Get/Set the udp port
      /// </summary>
      /// <value>The UDP port.</value>
      public static int UdpPort
      {
         get { return fUdpPort; }
         set { fUdpPort = value; }
      }

      /// <summary>
      /// Gets or sets the program number.
      /// </summary>
      /// <value>The program number.</value>
      public static byte ProgramNumber
      {
         get { return fProgramNumber; }
         set { fProgramNumber = value; }
      }

      #region Methods

         /// <summary>
         /// Finds and kills a process by name
         /// </summary>
         /// <param name="name">The name.</param>
         /// <returns>true - in case of success,
         /// false - if process not found</returns>
      public static bool FindAndKillProcess(string name)
      {
         //here we're going to get a list of all running processes on
         //the computer
         foreach (Process clsProcess in Process.GetProcesses()) {
	         //now we're going to see if any of the running processes
	         //match the currently running processes by using the StartsWith Method,
	         //this prevents us from incluing the .EXE for the process we're looking for.
	         //Be sure to not add the .exe to the name you provide, i.e: NOTEPAD,
	         //not NOTEPAD.EXE or false is always returned even if
	         //notepad is running
	         if (clsProcess.ProcessName.StartsWith(name) && 
               (clsProcess.Id != Process.GetCurrentProcess().Id)) {
		         //since we found the proccess we now need to use the
		         //Kill Method to kill the process. Remember, if you have
		         //the process running more than once, say IE open 4
		         //times the loop thr way it is now will close all 4,
		         //if you want it to just close the first one it finds
		         //then add a return; after the Kill
		         clsProcess.Kill();
		         //process killed, return true
		         return true;
	         }
         }
         //process not found, return false
         return false;
      }

      /// <summary>
      /// Finds a process by name
      /// </summary>
      /// <param name="name">The name.</param>
      /// <returns>true - in case of success,
      /// false - if process not found</returns>
      public static bool FindProcess(string name)
      {
         //here we're going to get a list of all running processes on
         //the computer
         foreach (Process clsProcess in Process.GetProcesses()) {
	         if (clsProcess.ProcessName.StartsWith(name) && 
               (clsProcess.Id != Process.GetCurrentProcess().Id)) {
		         ProgramNumber++;
	         }
         }
      
         return (ProgramNumber == 0);
      }

      /// <summary>
      /// Read Threshold and Roi values from App.Config
      /// </summary>
      public static void ReadThresholdAndROI()
      {
         string setting;

         if ((setting = ConfigurationManager.AppSettings["Threshold"]) != null) {
            fThreshold = short.Parse(setting);
         }

         if ((setting = ConfigurationManager.AppSettings["RoiX"]) != null) {
            RoiX[0] = short.Parse(setting);
         }

         if ((setting = ConfigurationManager.AppSettings["RoiY"]) != null) {
            RoiY[0] = short.Parse(setting);
         }

         if ((setting = ConfigurationManager.AppSettings["RoiWidth"]) != null) {
            RoiWidth[0] = short.Parse(setting);
         }

         if ((setting = ConfigurationManager.AppSettings["ShiftX"]) != null) {
            ShiftX[0] = short.Parse(setting);
         }
      }

      /// <summary>
      /// App.Config overrides
      /// </summary>
      public static void AppConfig()
      {
         string setting;

         //if ((setting = ConfigurationManager.AppSettings["EndPoint"]) != null) {
         //   string[] args = setting.Split(new char[]{':'}, 2);
         //   fEndPoint = new IPEndPoint(IPAddress.Parse(args[0]), int.Parse(args[1], CultureInfo.InvariantCulture));
         // }

         ReadThresholdAndROI();

         if ((setting = ConfigurationManager.AppSettings["UdpPort"]) != null) {
            fUdpPort = int.Parse(setting);
         }

         EndPoint = new IPEndPoint(IPAddress.Parse("234.9.9.9"), fUdpPort);

         if ((setting = ConfigurationManager.AppSettings["TcpPort"]) != null) {
            fTcpPort = int.Parse(setting);
         }

         if ((setting = ConfigurationManager.AppSettings["PulsePort"]) != null) {
            fPulsePort = int.Parse(setting);
         }

         if ((setting = ConfigurationManager.AppSettings["BreathPort"]) != null) {
            fBreathPort = int.Parse(setting);
         }

         if ((setting = ConfigurationManager.AppSettings["ExhalePort"]) != null) {
            fExhalePort = int.Parse(setting);
         }

         if ((setting = ConfigurationManager.AppSettings["Threshold"]) != null) {
            fThreshold = short.Parse(setting);
         }

         if ((setting = ConfigurationManager.AppSettings["RoiX"]) != null) {
            RoiX[0] = short.Parse(setting);
         }

         if ((setting = ConfigurationManager.AppSettings["RoiY"]) != null) {
            RoiY[0] = short.Parse(setting);
         }

         if ((setting = ConfigurationManager.AppSettings["RoiWidth"]) != null) {
            RoiWidth[0] = short.Parse(setting);
         }

         if ((setting = ConfigurationManager.AppSettings["PDispWidth"]) != null) {
            PDispWidth = short.Parse(setting);
         }

         if ((setting = ConfigurationManager.AppSettings["PDispHeight"]) != null) {
            PDispHeight = short.Parse(setting);
         }

         if ((setting = ConfigurationManager.AppSettings["ProfileWidth"]) != null) {
            ProfileWidth = short.Parse(setting);
         }

         if ((setting = ConfigurationManager.AppSettings["FlipVideo"]) != null) {
            fFlipVideo = int.Parse(setting);
         }

         if ((setting = ConfigurationManager.AppSettings["ActivateMicrophone"]) != null) {
            fActivateMicrophone = bool.Parse(setting);
         }

         if ((setting = ConfigurationManager.AppSettings["Local"]) != null) {
            fLocal = bool.Parse(setting);

            if (!fLocal && ((setting = ConfigurationManager.AppSettings["DataBasePath"]) != null)) {
               fDataBasePath = setting;

               if ((setting = ConfigurationManager.AppSettings["HeadPath"]) != null) {
                  fHeadPath = setting;
               }

               if ((setting = ConfigurationManager.AppSettings["PhotoPath"]) != null) {
                  fPhotoPath = setting;
               }

            } else {
               fDataBasePath = Path.Combine(Environment.GetFolderPath(Environment.
                                            SpecialFolder.ApplicationData), "AlignPatientDB");
            }
         }

         if ((setting = ConfigurationManager.AppSettings["PortName"]) != null) {
               fPortName = setting;
         }

         if ((setting = ConfigurationManager.AppSettings["ComDataFile"]) != null) {
            fComDataFile = setting;
         }

         if ((setting = ConfigurationManager.AppSettings["BaudRate"]) != null) {
            fBaudRate = int.Parse(setting);
         }

         if ((setting = ConfigurationManager.AppSettings["DataBits"]) != null) {
            fDataBits = int.Parse(setting);
         }


         if ((setting = ConfigurationManager.AppSettings["Parity"]) != null) {
            fParity = (Parity)Enum.Parse(typeof(Parity), setting);
         }

         if ((setting = ConfigurationManager.AppSettings["StopBits"]) != null) {
            fStopBits = (StopBits)Enum.Parse(typeof(StopBits), setting);
         }

         if ((setting = ConfigurationManager.AppSettings["Handshake"]) != null) {
            fHandshake = (Handshake)Enum.Parse(typeof(Handshake), setting);
         }
      }

      /// <summary>
      /// Check if form invoke is requiered and invoke the form on the UI thread
      /// (form.Invoke if not on UI thread, or on the current thread if we are
      /// on the UI thread)
      /// </summary>
      /// <param name="del">Delegate to execute on the UI thread</param>
      /// <param name="args">The args.</param>
      public static void FormInvoke(Delegate del, object[] args)
      {
         if ((gCallingForm != null) && (gCallingForm.InvokeRequired)) {
            // The caller is on a different thread than the one the form was
            // created on. So we need to call form. Invoke to execute the delegate
            // on the thread that owns the form.

            if (gCallingForm != null) {
               gCallingForm.Invoke(del, args);
            }
         } else {
            // Otherwise we are already on the UI thread where the form was created, 
            // so call the delegate code directly on the current thread

            del.DynamicInvoke(args);
         }
      }

      #region Start the RtpSession

      /// <summary>
      /// Joins the RTP session.
      /// </summary>
      /// <param name="cname">The cname.</param>
      public static void JoinRtpSession(string cname)
      {
         fParticipant = new RtpParticipant(cname, "Video");
         fRtpSession = new RtpSession(fEndPoint, fParticipant, true, true);
      }


      #endregion Start the RtpSession

      /// <summary>
      /// Creates the sender.
      /// </summary>
      /// <returns>RtpSender.</returns>
      public static RtpSender CreateSender()
      {

         return fRtpSession.CreateRtpSender("Video", PayloadType.dynamicVideo, null);
      }

      /// <summary>
      /// Creates the audio sender.
      /// </summary>
      /// <returns>RtpSender.</returns>
      public static RtpSender CreateAudioSender()
      {
         return fRtpSession.CreateRtpSender("Audio", PayloadType.dynamicAudio, null);
      }

      /// <summary>
      /// Leaves this instance.
      /// </summary>
      public static void Leave()
      {
         if (fRtpSession != null) {
            fRtpSession.Dispose();
            fRtpSession = null;
         }
      }

      #endregion Methods

      #endregion Public
   }
}

