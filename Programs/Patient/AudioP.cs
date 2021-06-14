// $Id: AudioP.cs 1943 2013-07-16 12:10:04Z onuchin $
// Author: Valeriy Onuchin   08.02.2011

//#define LOCAL_DEBUG

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Globalization;

using P;
using P.DShow;
using P.Net.Rtp;


namespace PatientDisplay
{
   public partial class PatientForm {
      #region Members

      #region Sending side / Capture graph

      protected AudioCaptureGraph fAudioCaptureGraph;
      protected AVLogger fLog;

      /// <summary>
      /// Audio microphone filter info
      /// </summary>
      protected FilterInfo fMicFilterInfo;

      /// <summary>
      /// Sends the audio across the network
      /// </summary>
      private RtpSender fRtpAudioSender;

      #endregion Sending side / Capture graph
      #region Receiving side / Playing graph

      // Audio volume control related variable members
      private IBasicAudio iBasicAudio;
      private int fCurrentVolume;

      /// <summary>
      /// Audio renderer filter info 
      /// </summary>
      protected FilterInfo fSpeakerFI;
      protected FilgraphManagerClass fgmAudio;
      protected object fgmAudioLock = new object();
      protected FilterGraph.State fgmAudioState = FilterGraph.State.Stopped;
      protected uint fRotAudioID = 0;

      /// <summary>
      /// Receives the audio data across the network
      /// </summary>
      private RtpStream fRtpAudioStream;

      #endregion Receiving side / Playing graph

      #endregion Members
      #region Public

      /// <summary>
      /// Reads / Writes to the registry, whether compression is enabled or not
      /// </summary>
      public bool RegCompressorEnabled {
         get {
            bool enabled = true;

            object setting = Reg.ReadValue(DeviceKey(), Reg.CompressorEnabled);
            if (setting != null) {
               enabled = bool.Parse((string)setting);
            }

           return enabled;
         }

         set {
            Reg.WriteValue(DeviceKey(), Reg.CompressorEnabled, value);
         }
      }

      /// <summary>
      /// Reads / Writes to the registry, whether compression is enabled or not
      /// </summary>
      public bool RegAudioCompressorEnabled {
         get {
            bool enabled = true;

            object setting = Reg.ReadValue(DeviceKey(), Reg.AudioCompressorEnabled);
            if (setting != null) {
               enabled = bool.Parse((string)setting);
            }

            return enabled;
         }

         set {
            Reg.WriteValue(DeviceKey(), Reg.AudioCompressorEnabled, value);
         }
      }

      #region Sending side / Capture graph

      /// <summary>
      /// Get the CaptureGraph for this capability.  
      /// It may be a audio or a DV capture graph.
      /// </summary>
      public AudioCaptureGraph AudioCaptureGraph {
         get { return fAudioCaptureGraph; }
      }

      /// <summary>
      /// Provide device's registry path
      /// </summary>
      protected string DeviceKey()
      {
         return Reg.RootKey + fAudioCaptureGraph.Source.FriendlyName;
      }

      /// <summary>
      /// Creates the microphone (capture graph)
      /// </summary>
      public void ActivateMicrophone()
      {
#if LOCAL_DEBUG
         Log(string.Format(CultureInfo.CurrentCulture, "\r\nInitializing microphone - {0}, {1}", 
             fMicFilterInfo.DisplayName, fMicFilterInfo.Moniker));
#endif
         // Get microphone up and running
         CreateAudioCaptureGraph(fMicFilterInfo);

         RestoreMicrophoneSettings();
         RestoreAudioSettings();
         // Get Custom buffer settings: uncompressed audio only
         RestoreBufferSettings();

         LogCurrentMediaType(fAudioCaptureGraph.Source);

         // Add compressor if necessary
         AddAudioCompressor();
         LogCurrentMediaType(((IAudioCaptureGraph)fAudioCaptureGraph).AudioCompressor);
#if LOCAL_DEBUG
         // Log all the filters in the graph
         Log(FilterGraph.Debug(fAudioCaptureGraph.IFilterGraph));
#endif
         if (fRtpAudioSender == null) {
            fRtpAudioSender = Session.CreateAudioSender();
         }

         fAudioCaptureGraph.RenderNetwork(fRtpAudioSender, PayloadType.dynamicAudio);
         fAudioCaptureGraph.Run();
      }

      /// <summary>
      /// Adds an audio compressor if needed (by checking the registry)
      /// </summary>
      public void AddAudioCompressor()
      {
         if (RegAudioCompressorEnabled) {

            try {
               fAudioCaptureGraph.AddCompressor(AudioCompressor.DefaultFilterInfo());
#if LOCAL_DEBUG
               Log(fAudioCaptureGraph.Compressor.Dump());
#endif
               DefaultAudioCompressorSettings();
            } catch (Exception e) {
              // If we encounter an error trying to add or configure the
              // compressor, just disable compressor and log it 
              RegAudioCompressorEnabled = false;
#if LOCAL_DEBUG
              Log(e.ToString());
#endif
            }
         }
      }


      /// <summary>
      /// Disposes the microphone (capture graph)
      /// </summary>
      public void DeactivateMicrophone()
      {
         if (fAudioCaptureGraph != null) {
#if LOCAL_DEBUG
            Log(string.Format(CultureInfo.CurrentCulture, "\r\nDisposing graph - {0}", 
                fAudioCaptureGraph.Source.FriendlyName));
#endif
            fAudioCaptureGraph.Dispose();
            fAudioCaptureGraph = null;
         }
      }


      public int CompressionMediaTypeIndex {
         get { 
            int compressionIndex = AudioCompressor.DEFAULT_COMPRESSION_INDEX;

            object setting = Reg.ReadValue(DeviceKey(), Reg.CompressionMediaTypeIndex);
            if (setting != null) {
               compressionIndex = (int)setting;
            }

            return compressionIndex;
         }
         set {
            Reg.WriteValue(DeviceKey(), Reg.CompressionMediaTypeIndex, value);
         }
      }

      #region Registry

      /// <summary>
      /// Restore the microphone's last settings from the registry.  Not applicable to DV sources
      /// </summary>
      private void RestoreMicrophoneSettings()
      {
         
         if (fAudioCaptureGraph != null) {
            object setting = Reg.ReadValue(DeviceKey(), Reg.MicrophoneSourceIndex);
            if (setting != null) {
               if ((int)setting < AudioCaptureGraph.Source.InputPins.Count) {
                  fAudioCaptureGraph.AudioSource.InputPinIndex = (int)setting;
               }
            }
         }
      }

      /// <summary>
      /// Restore custom buffer settings from the registry, if any.
      /// This doesn't seem to work with DV Sources
      /// </summary>
      private void RestoreBufferSettings()
      {
          object bufSz = Reg.ReadValue(DeviceKey(), Reg.AudioBufferSize);
          if (bufSz != null) {
              fAudioCaptureGraph.AudioSource.BufferSize = (int)bufSz;
          }

          object bufCnt = Reg.ReadValue(DeviceKey(), Reg.AudioBufferCount);
          if (bufCnt != null) {
              fAudioCaptureGraph.AudioSource.BufferCount = (int)bufCnt;
          }
      }

      /// <summary>
      /// Restore the audio stream's last settings from the registry
      /// </summary>
      private void RestoreAudioSettings()
      {
         // Read media type from registry
         var bytes = (byte[])Reg.ReadValue(DeviceKey(), Reg.MediaType);

         if (bytes != null) {
            Reg.ms.Position = 0;
            Reg.ms.Write(bytes, 0, bytes.Length);

            Reg.ms.Position = 0;
            var mt = (_AMMediaType)Reg.bf.Deserialize(Reg.ms);
             
            // Read format block from registry
            if (mt.cbFormat != 0) {
               bytes = (byte[])Reg.ReadValue(DeviceKey(), Reg.FormatBlock);
               Debug.Assert(bytes.Length == mt.cbFormat);

               mt.pbFormat = Marshal.AllocCoTaskMem((int)mt.cbFormat);
               Marshal.Copy(bytes, 0, mt.pbFormat, (int)mt.cbFormat);
#if LOCAL_DEBUG
               Log("Restoring stream settings...");
               Log(MediaType.Dump(mt));
#endif
               try {
                  // Set and free
                  fAudioCaptureGraph.Source.SetMediaType(ref mt);
               } catch(COMException ex) {
#if LOCAL_DEBUG
                  Log(DShowError._AMGetErrorText(ex.ErrorCode));
                  Log(ex.ToString());
#endif
               } catch(Exception ex) {
#if LOCAL_DEBUG
                  Log(ex.ToString());
#endif
               }
            }
         }
      }

      #endregion Registry

      #endregion Sending side / Capture graph
      #region Receiving side / Playing graph

      public FilgraphManagerClass PlayGraph
      {
            get{ return fgmAudio; }
      }

      public void ResumePlayingAudio()
      {
         lock (fgmAudioLock) {
            if (fgmAudioState != FilterGraph.State.Running) {
               fgmAudioState = FilterGraph.State.Running;

               if (fgmAudio != null) {
                  iBasicAudio = fgmAudio;

                  if (fRtpAudioStream != null) {
                     fRtpAudioStream.BlockNextFrame();
                  }

                  fgmAudio.Run();
               }
            }
         }
      }

      #endregion Receiving side / Playing graph

      #endregion Public
      #region Private

      #region Sending side / Capture graph

      /// <summary>
      /// Creates the actual FilgraphManager with the chosen microphone
      /// </summary>
      private void CreateAudioCaptureGraph(FilterInfo fi)
      {
         Debug.Assert(fAudioCaptureGraph == null);

         // Create the graph, which creates the source filter
         fAudioCaptureGraph = new AudioCaptureGraph(fi);
#if LOCAL_DEBUG
         Log(fAudioCaptureGraph.AudioSource.Dump());
#endif
      }

      /// <summary>
      /// By default, we use the Windows Media Audio V2 compressor
      /// </summary>
      private void DefaultAudioCompressorSettings()
      {
         Debug.Assert(AudioCompressor.DefaultName == "Windows Media Audio V2");
         IAudioCaptureGraph iacg = fAudioCaptureGraph;
         iacg.AudioCompressor.SetMediaType(iacg.AudioCompressor.PreConnectMediaTypes[CompressionMediaTypeIndex]);
      }

      #endregion Sending side / Capture graph
      #region Receiving side / Playing graph

      private void DisposeFgmAudio()
      {
         lock (fgmAudioLock) {
            if (fgmAudio != null) {
               // We need to manually unblock the stream in case there is no data flowing
               if (fRtpAudioStream != null) {
                  fRtpAudioStream.UnblockNextFrame();
               }
#if DEBUG    
               FilterGraph.RemoveFromRot(fRotAudioID);
#endif
               fgmAudio.Stop();
               FilterGraph.RemoveAllFilters(fgmAudio);
               fgmAudio = null;
               fgmAudio = null;
            }
         }
      }

      /// <summary>
      /// Creation of the fgmAudio and the adding / removing of filters needs to happen on the
      /// same thread.  So make sure it all happens on the UI thread.
      /// </summary>
      private void CreateAudioRenderGraph()
      {
         lock (fgmAudioLock) {
            DisposeFgmAudio();

            Debug.Assert(fgmAudio == null);

            fgmAudio = new FilgraphManagerClass();
            var iGB = (IGraphBuilder)fgmAudio;
#if DEBUG            
            fRotAudioID = FilterGraph.AddToRot(iGB);
#endif
            IBaseFilter rtpSource = RtpSourceClass.CreateInstance();
            ((P.Filters.IRtpSource)rtpSource).Initialize(fRtpAudioStream);
            iGB.AddFilter(rtpSource, "RtpAudioSource");

            // Add the chosen audio renderer
            FilterInfo fi = fSpeakerFI;
            iGB.AddFilter(Filter.CreateBaseFilter(fi), fi.Name);

            iGB.Render(Filter.GetPin(rtpSource, _PinDirection.PINDIR_OUTPUT, Guid.Empty, 
                       Guid.Empty, false, 0));

            iBasicAudio = fgmAudio;
            iBasicAudio.Volume = 0;  // full volume
            fCurrentVolume = 0; //(int)Math.Round(Math.Pow(10.0, (2.0*(double)(iBasicAudio.Volume+10000))/10000.0));
            iBasicAudio = null;

            fgmAudioState = FilterGraph.State.Stopped;
            ResumePlayingAudio();
         }
      }

      #endregion Receiving side / Playing graph

      private void DisposeAudio()
      {
         DisposeFgmAudio();
         DeactivateMicrophone();
      }

      #endregion Private
      #region Logging Methods

      protected void LogCurrentMediaType(Filter f)
      {
         if (f != null) {
            _AMMediaType mt;
             object fb;
             f.GetMediaType(out mt, out fb);
#if LOCAL_DEBUG
             Log(string.Format(CultureInfo.CurrentCulture, "\r\nCurrent media type for {0}...", 
                 f.FriendlyName) + MediaType.Dump(mt) + MediaType.FormatType.Dump(fb));
#endif
         }
      }
      protected void Log(string msg)
      {
         if (fLog != null) {
            fLog(msg);
         }
      }

      #endregion Logging Methods
   }
}
