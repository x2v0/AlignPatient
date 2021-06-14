// $Id: Video.cs 2090 2014-02-26 07:21:10Z onuchin $
 // Author: Valeriy Onuchin  09.07.2013
 
//#define DEBUG
//#define LOCAL_DEBUG

using System.Runtime.InteropServices; // marshaling

// our own classes
 
using P;
using P.DShow;
using P.Net.Rtp;
using P.Filters;
 

namespace PatientDisplay
{
   public partial class PatientForm {

      #region Sending side / Capture graph

     /// <summary>
      /// Contains filtergraph on the sending side
      /// </summary>
      /// 
      protected FilgraphManagerClass fFilgraphManager;
      protected VideoCaptureGraph fVideoCaptureGraph;
      protected ICaptureGraphBuilder2 iGraphBuilder2;
      protected IGraphBuilder iGraphBuilder;
      protected IPin iPreviewPin;
      protected IPin iCapturePin;

      protected FilgraphManagerClass fFilgraphManagerDoctor;
      protected FilterGraph.State fgmState = FilterGraph.State.Stopped;
      protected uint rotID = 0;
      protected object fGraphMngLockDoctor = new object();
      protected IVideoWindow fDoctorVideoWindow;

     /// <summary>
      /// Filter graph lock object
      /// </summary>
      protected object fGraphMngLock = new object();

      /// <summary>
      /// Video camera filter info
      /// </summary>
      protected FilterInfo fVideoFilterInfo;

      /// <summary>
      /// Sends the data across the network
      /// </summary>
      private RtpSender fRtpSender;

      /// <summary>
      /// Patient camera video window 
      /// </summary>

      protected IVideoWindow fVideoWindow;

      /// <summary>
      /// Transform check position filter 
      /// </summary>

      private CheckPosFilter fCheckPosFilter;

      /// <summary> 
      /// Interface to transform filter on the receiving side. 
      /// Used to:
      ///    - display difference between reference frame and video frames (diff , alfa blend etc.) 
      ///    - flip/mirrow video frames
      ///    - crop and stretch video frames
      /// </summary>

      private ICheckPosFilter iCheckPosFilter;

      #endregion Sending side / Capture graph

      /// <summary>
      /// Create video capture graph
      /// </summary>
      private void CreateVideoCaptureGraph() {

         if (fRtpSender == null) {
            fRtpSender = Session.CreateSender();
         }

         // Create the graph for the video device
         fVideoCaptureGraph = new VideoCaptureGraph(fVideoFilterInfo);

         fFilgraphManager = fVideoCaptureGraph.FilgraphManager;

         iGraphBuilder = (IGraphBuilder)fFilgraphManager;
         iGraphBuilder2 = CaptureGraphBuilder2Class.CreateInstance();
         iGraphBuilder2.SetFiltergraph(iGraphBuilder);

        // add check position transform filter
         fCheckPosFilter = Filter.CheckPosFilter();
         iGraphBuilder.AddFilter(fCheckPosFilter.BaseFilter, fCheckPosFilter.FriendlyName);
         iCheckPosFilter = (ICheckPosFilter)fCheckPosFilter.BaseFilter;
         iCheckPosFilter.Mirrow(1);
         fCheckPosFilter.AddedToGraph(fFilgraphManager);

         var source = fVideoCaptureGraph.Source;

         // set frames per second
         var strCfg = (IAMStreamConfig)source.OutputPin;
         var mt = Pin.GetMediaType(strCfg);
         var pvi = (VIDEOINFOHEADER)Marshal.PtrToStructure(mt.pbFormat, typeof(VIDEOINFOHEADER));
         pvi.AvgTimePerFrame = (long)(10000000/24.9);
         Marshal.StructureToPtr(pvi, mt.pbFormat, true);

         strCfg.SetFormat(ref mt);
         MediaType.Free(ref mt);

         iGraphBuilder.Connect(source.OutputPin, fCheckPosFilter.InputPin);

         var smartTee = Filter.CreateBaseFilterByName("Smart Tee");
         iGraphBuilder.AddFilter(smartTee, "Smart Tee");
         iPreviewPin = Filter.GetPinByName(smartTee, "Preview");
         iCapturePin = Filter.GetPinByName(smartTee, "Capture");
         var smartTeeInputPin = Filter.GetPinByName(smartTee, "Input");
         iGraphBuilder.Connect(fCheckPosFilter.OutputPin, smartTeeInputPin);
     

#if DEBUG
         FilterGraph.AddToRot(iGraphBuilder);
#endif

         fVideoCaptureGraph.AddCompressorNotDispose(VideoCompressor.DefaultFilterInfo(), iCapturePin);
         fVideoCaptureGraph.VideoCompressor.QualityInfo = VideoCompressor.DefaultQualityInfo;
         iGraphBuilder.Render(iPreviewPin); 
         PatientVideoWindow(false);
         fVideoCaptureGraph.RenderNetwork(fRtpSender);
         fVideoCaptureGraph.Run();
      }

      private void PatientVideoWindow(bool fullScreen)
      {

         // kill explorer
         //foreach (Process clsProcess in Process.GetProcesses()) {
         //   if (clsProcess.ProcessName.StartsWith("explorer"))  {
         //      clsProcess.Kill();
         //   }
         //}

         // hide taskbar
         int hwnd = FindWindow("Shell_TrayWnd", "");
         if (hwnd != 0) {
            //MessageBox.Show("taskbar");
            //ShowWindow(hwnd, SW_HIDE);
         }

         // hide desktop
         hwnd = FindWindow("Progman", "Program Manager");
         if (hwnd != 0) {
            //MessageBox.Show("Progman");
            //ShowWindow(hwnd, SW_HIDE);
         }

         // hide cursor
         //ShowCursor(false);

         fVideoWindow = fFilgraphManager;

         if (fullScreen) {
            fVideoWindow.FullScreenMode = -1;
         } else {
            int ws = fVideoWindow.WindowStyle;
            ws = ws & ~(0x00C00000);   // Remove caption
            ws = ws & ~(0x00800000);   // Remove WS_BORDER
            ws = ws & ~(0x00400000);   // Remove WS_DLGFRAME
            //ws = ws | (0x01000000);    // maximized
            //ws = ws | (0x8000000);     // popup
            ws = ws & ~(0x00010000);   // no maximized box
            ws = ws & ~(0x00020000);   // no minimized box
            ws = ws & ~(0x00040000);   // no size box
            //ws = ws & ~(0x00080000);   // no sys menu
            //ws = ws & ~(0x00040000);   // no thick frame

            fVideoWindow.WindowStyle = ws;
            fVideoWindow.Width = Session.PDispWidth;
            fVideoWindow.Height = Session.PDispHeight;
            fVideoWindow.SetWindowPosition(0, 0, Session.PDispWidth, Session.PDispHeight);
            fVideoWindow.Owner = fPictureBox.Handle.ToInt32();
         }
      }


      private void DisposeVideo()
      {
         if (fVideoCaptureGraph != null) {
            fVideoCaptureGraph.Stop();
            fVideoCaptureGraph.RemoveFiltersDownstreamFromSource(PayloadType.dynamicVideo);
            fVideoCaptureGraph.Dispose();
            fVideoCaptureGraph = null;
         }
      }

    }
}
 
