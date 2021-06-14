// $Id: SerialPortCom.cs 1889 2013-05-23 05:52:31Z onuchin $
// Author: Valeriy Onuchin   03.02.2011

//#define LOCAL_DEBUG

using System;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using P;

namespace PatientDisplay
{
   public partial class  PatientForm : Form
   {
      #region Members

      /// <summary>
      /// fWriteStream FileStream - is used to debug COM port communication
      ///                  by saving COM data to the local file
      /// </summary>
#if LOCAL_DEBUG
      private FileStream fWriteStream;
      private short idx = 0;
#endif
      private SerialPort fComPort;
      private Thread fComThread;

      /// <summary>
      /// Total number of buffers received from serial port
      /// </summary>
      private int fComPortCounter;
      private System.Threading.Timer fComPortTimer;
      private DateTime fReadTime;

      /// <summary>
      /// COM port hardcoded buffer size
      /// </summary>
      private const int kComBufSize = 2048;
      private byte[] fComBuffer = new byte[kComBufSize];

      /// <summary>
      /// Sends the patient pulse data across the network
      /// </summary>
      private TcpClient fTcpSenderPulse;

      public static readonly string kMesureMode = "s";
      public static readonly string kCalibrMode = "a";
      private string fMode = kMesureMode;

      #endregion Members
      #region Public

      public string Mode
      {
         get { return fMode; }
         set 
         {
            if (fMode != null && fMode == value) {
               return;
            }
            fMode = value;
         }
      }
   
      #endregion Public
      #region Private
      #region CloseComPort

      private void CloseComPort()
      {

         //if (fComThread != null) {
         //   fComThread.Abort();
         //}

         if (fComPortTimer != null) {
            fComPortTimer.Dispose();
         }

        if (fComPort != null) {
            fComPort.DataReceived -= ComPortDataReceived;
            fComPort.ErrorReceived -= ComPortErrorReceived;
            fComPort.Close();
            fComPort.Dispose();
            fComPort = null;
         }

         if (fTcpSenderPulse != null) {
            fTcpSenderPulse.Close();
            fTcpSenderPulse = null;
         }
      }

      #endregion CloseComPort
      #region OpenComPort

      /// <summary>
      /// Serial port, TCP port initializations
      /// </summary>
      private void OpenComPort()
      {

         try {
            //first check if the port is already open
            //if its open then close it
            if ((fComPort != null) && fComPort.IsOpen) {
               fComPort.Close();
            }

            if (fComPort == null) {
               fComPort = new SerialPort();
            }

            //if ((fComThread != null) && fComThread.IsAlive) {
            //   fComThread.Abort();
            //}

            if (fTcpSenderPulse != null) {
               fTcpSenderPulse.Close();
            }

            // create TCP client
            fTcpSenderPulse = new TcpClient();

            //set the properties of our SerialPort Object
            fComPort.BaudRate = Session.BaudRate;
            fComPort.DataBits = Session.DataBits;
            fComPort.PortName = Session.PortName;
            fComPort.ReadBufferSize = 32768;
            fComPort.ReceivedBytesThreshold = 2048;
            fComPort.ReadTimeout  = 4000;
            fComPort.WriteTimeout = 4000;

            //fComPort.StopBits = StopBits.None; //Session.StopBits;
            //fComPort.Parity = Parity.None; //Session.Parity;
            //fComPort.Handshake = Handshake.None;  //Session.Handshake; 

            fComPort.Open();

            if (fComPort.IsOpen) {
               fComPort.DataReceived += ComPortDataReceived;
               fComPort.ErrorReceived += ComPortErrorReceived;

               // create a timer which sends "s" every 1/4 second
               fComPortTimer = new System.Threading.Timer(OnTimeElapsed, null, 20, 250);

               fComPort.Write(Mode);

            } else {
   
            }

            return;
         } catch (Exception ex) {
#if LOCAL_DEBUG
            MessageBox.Show("exception - " + ex.StackTrace);
#endif
            return;
         }
      }

      #endregion OpenComPort
      #region ComPortThreadHandler


      /// <summary>
      ///  Periodically writes the Mode symbol to COM port
      /// </summary>
      /// <param name="data">not used</param>
      void OnTimeElapsed(object data) 
      {
        if (fComPort == null || !fComPort.IsOpen) {
            return;
         }

         fComPort.Write(Mode);
      }

      /// <summary>
      /// Thread which periodically polls COM port for data available and
      /// sends to client or write to file (for debugging)
      /// </summary>
      void ComPortThreadHandler()
      {
         int total = 0;

         while (fComThread.IsAlive) {

            Thread.Sleep(1024);

            int bytes = fComPort.BytesToRead;

            // create a byte array to hold the awaiting data
            var comBuffer = new byte[bytes];

            // read the data and store it
            int nRead = fComPort.Read(comBuffer, 0, bytes);
            total += nRead;

#if LOCAL_DEBUG
            if (fWriteStream == null) {
               fWriteStream = new System.IO.FileStream(Session.ComDataFile, System.IO.FileMode.Create, 
                                               System.IO.FileAccess.Write);
            }
if (idx < 5) {
//MessageBox.Show("read = " + nRead + " bytes = " + bytes);
}
            if (fWriteStream.CanWrite) {
               fWriteStream.Write(comBuffer, 0, nRead);
               idx++;
            }

            // cleanup buffers
            //fComPort.DiscardOutBuffer();
            //fComPort.DiscardInBuffer();

            if (idx == 1) {
               MessageBox.Show("started");
            }

            if (idx == 10) {
               fWriteStream.Close();
               idx = 0;

               MessageBox.Show("closed " + Session.ComDataFile);
               CloseComPort();
               Application.Exit();
               return;
            }

            fComPort.Write(kMesureMode);
#else
            if (fTcpSenderPulse == null) {   //reentrance
               fTcpSenderPulse = new TcpClient();
            }

            if (!fTcpSenderPulse.Connected) {
               try {
                  fTcpSenderPulse.Connect(Session.DoctorIP, Session.PulsePort);

                  if ((fComPort != null) && fComPort.IsOpen) {
                     fComPort.Write(kMesureMode);
                  } else {
                     // COM port is not opened/connected sleep for connect            
                     Thread.Sleep(2000);

                     //set the properties of our SerialPort Object
                     fComPort.BaudRate = Session.BaudRate;
                     fComPort.DataBits = Session.DataBits;
                     fComPort.PortName = Session.PortName;
                     fComPort.ReadBufferSize = 32768;
                     fComPort.ReceivedBytesThreshold = 2048;
                     fComPort.ReadTimeout  = 4000;
                     fComPort.WriteTimeout = 4000;
                     fComPort.Open();
                     fComPort.Write(kMesureMode);
                     continue;
                  }
               } catch (Exception) {
                  // failed to connect to network 
                  Thread.Sleep(2000);
                  //fComPort.Open();
                  continue;
               }
            }

            try {
               // send COM data across network
               Stream strm = fTcpSenderPulse.GetStream();

               //if (fTcpSenderPulse.Connected) {
               strm.Write(comBuffer, 0, comBuffer.Length);
               strm.Flush();
               //}
               fComPort.Write(kMesureMode);

            } catch (Exception) {
               // disconnected
               fTcpSenderPulse.Close();
               fTcpSenderPulse = null;
               //fComPort.Close();
            }
#endif
         }
      }

      #endregion

      #endregion Private
      #region Events

      #region ComPortErrorReceived
      /// <summary>
      /// method that will be called when the error received
      /// </summary>
      void ComPortErrorReceived(object sender, SerialErrorReceivedEventArgs e)
      {

         switch (e.EventType)  {
             case SerialError.Frame:
                 MessageBox.Show(Strings.Framing_error_); 
                 
                 break;
             case SerialError.Overrun:
                 MessageBox.Show(Strings.Character_buffer_overrun_); 
                 
                 break;
             case SerialError.RXOver:
                 MessageBox.Show(Strings.Input_buffer_overflow_); 
                 
                 break;
             case SerialError.RXParity:
                 MessageBox.Show(Strings.Parity_error_);
                 
                 break;
             case SerialError.TXFull:
                 MessageBox.Show(Strings.Output_buffer_full_); 
                 break;
         } 
      }
      #endregion
      #region ComPortDataReceived
      /// <summary>
      /// method that will be called when there is data waiting in the buffer
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      void ComPortDataReceived(object sender, SerialDataReceivedEventArgs e)
      {

         if (fComPort == null) {
            return;
         }

         if (fTcpSenderPulse == null) {
            fTcpSenderPulse = new TcpClient();
         }

         int bytes = fComPort.BytesToRead;
 
         if (bytes < kComBufSize) {
            return;
         }

         DateTime now = DateTime.Now;
         fReadTime = now;

         //read the data and store it
         int nRead = fComPort.Read(fComBuffer, 0, kComBufSize);

         // cleanup buffers
         //fComPort.DiscardOutBuffer();
         //fComPort.DiscardInBuffer();

#if LOCAL_DEBUG
         if (fWriteStream == null) {
            fWriteStream = new System.IO.FileStream("PulseData.dat", System.IO.FileMode.Create, 
                                            System.IO.FileAccess.Write);
         }

         if (fWriteStream.CanWrite) {
            fWriteStream.Write(comBuffer, 0, bytes);
            idx++;
         }

         if (idx == 1) {
            MessageBox.Show("started");
         }

         if (idx == 200) {
            fWriteStream.Close();
            idx = 0;
            MessageBox.Show("closed");
            CloseComPort();
            Application.Exit();
         }
#else

         if (!fTcpSenderPulse.Connected) {
            try {
               fTcpSenderPulse.Connect(Session.DoctorIP, Session.PulsePort);
            } catch (Exception) {
               // failed to connect to network 
               fTcpSenderPulse.Close();
               fTcpSenderPulse = null;
               //Thread.Sleep(2000);
               return;
            }
         }

         try {
            // send COM data across network
            Stream strm = fTcpSenderPulse.GetStream();

            if (fTcpSenderPulse.Connected) {
               strm.Write(fComBuffer, 0, fComBuffer.Length);
               strm.Flush();
            }
         } catch (Exception) {
            // disconnected
            fTcpSenderPulse.Close();
            fTcpSenderPulse = null;
         }
#endif
         fComPortCounter++;
      }

      #endregion

      #endregion Events
   }
}
