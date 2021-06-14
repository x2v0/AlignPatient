// $Id: VideoWindowF.cs 2102 2014-02-26 12:47:02Z onuchin $
// Author: Valeriy Onuchin   09.07.2013

//#define DEBUG
//#define LOCAL_DEBUG

using System;
using System.Windows.Forms;

//using Microsoft.Xna.Framework;
using P.Net.Rtp;

// our own namespaces
using P;
using P.DShow;
using P.Filters;


namespace DoctorDisplay
{
   public partial class DoctorDisplayForm
   {

      /// <summary>
      /// Filtergraph on the receiving side
      /// </summary>
      private FilgraphManagerClass fFilgraphManagerF;

      private IVideoWindow fVideoWindowF;

      protected object fGraphMgrLockF = new object();

      /// <summary>
      /// Receives the video data across the network
      /// </summary>
      private RtpStream fRtpStreamF;

      /// <summary>
      /// Transform filter on the receiving side. Used to flip/mirrow frames
      /// </summary>
      private CheckPosFilter fCheckPosFilterF;

     /// <summary>
      /// Interface to transform filter on the receiving side. Used to flip/mirrow frames
      /// </summary>
      private ICheckPosFilter iCheckPosFilterF;

      /// <summary>
      /// some constants
      /// </summary>
      private const short fOffF = 50;
      private const short fVideoWindowWF = 640;
      private const short fVideoWindowHF = 480;
      private const short fVideoWindowBorderF = 4;

      /// <summary>
      /// y-positions of lines defining crop area
      /// initial values are hardcoded for a while
      /// </summary>

      private short fTopYF = fOffF; //
      private short fBotYF = fVideoWindowHF - fOffF;
      private short fPrevTopYF;
      private short fPrevBotYF;

      private short fLeftXF = fOffF;
      private short fRightXF = fVideoWindowWF - fOffF;
      private short fPrevLeftXF;
      private short fPrevRightF;

      private bool fVideoCursorCatchedF;
      private ELineType fPressedLineType = ELineType.kNonLine;
      private bool fZoomedF;
      private bool fCheckMode;
      private bool fGraphCreatedF;

      private void CreateVideoGraphF()
      {

         if (fGraphCreatedF)  {
            return;
         } else {
            fGraphCreatedF = true;
         }

         // Tell the stream we will poll it for data with our own (DShow) thread
         // Instead of receiving data through the FrameReceived event
         fRtpStreamF.IsUsingNextFrame = true;

         // Create receiving filtergraph
         fFilgraphManagerF = new FilgraphManagerClass();
         var iGraphBldF = (IGraphBuilder)fFilgraphManagerF;

#if DEBUG
         FilterGraph.AddToRot(iGraphBldF);
#endif
         var rtpSource = P.DShow.RtpSourceClass.CreateInstance();
         ((IRtpSource)rtpSource).Initialize(fRtpStreamF);

         iGraphBldF.AddFilter(rtpSource, "RtpSource Frontal");
         //iGraphBldF.Render(Filter.GetPin(rtpSource, _PinDirection.PINDIR_OUTPUT,
         //                 Guid.Empty, Guid.Empty, false, 0));

         var outPin = Filter.GetPin(rtpSource, _PinDirection.PINDIR_OUTPUT,
                                    Guid.Empty, Guid.Empty, false, 0);


          // connect transform filter just before Video Renderer
         fCheckPosFilterF = Filter.CheckPosFilter();
         iCheckPosFilterF = (ICheckPosFilter)fCheckPosFilterF.BaseFilter;

         // flip & mirrow 
         iCheckPosFilterF.Flip(Session.FlipVideo);
         iCheckPosFilterF.Mirrow(Session.FlipVideo);

         // y lines
         fBotYF = fVideoWindowHF - fOffF;
         iCheckPosFilterF.SetCropArea(0, fBotYF, (int)ELineType.kBottomLine); // blue line (bottom)
         fTopYF = fOffF;
         iCheckPosFilterF.SetCropArea(0, fTopYF, (int)ELineType.kTopLine);    // yellow line (top)

         // x lines
         fLeftXF = fOffF;
         iCheckPosFilterF.SetCropArea(fLeftXF, 0, (int)ELineType.kLeftLine);
         fRightXF = fVideoWindowWF - fOffF;
         iCheckPosFilterF.SetCropArea(fRightXF, 0, (int)ELineType.kRightLine);

         var cpf = fCheckPosFilterF.BaseFilter;
         iGraphBldF.AddFilter(cpf, "Check Position Filter Frontal");
         fCheckPosFilterF.AddedToGraph(fFilgraphManagerF);

         var cpfIn = GetInputPin(cpf);
         //var renderIn = GetInputPin(renderer);
         var cpfOut = GetOutputPin(cpf);
  
         // connect out pin to check position filter input pin
         iGraphBldF.Connect(outPin, cpfIn);
         iGraphBldF.Render(cpfOut);

         // start game pad processing
         //fPlayerIndex = PlayerIndex.One;
         //fGamePadTimer.Start();   

         VideoWindowF();
         fFilgraphManagerF.Run();
      }

      private void VideoWindowF()
      {

         if (fVideoWindowF != null) {   // dispose
            fVideoWindowF.Visible = 0;
            fVideoWindowF.Owner = 0;
            fVideoWindowF = null;
         }

         // Must render before adjusting the VideoWindow
         fVideoWindowF = fFilgraphManagerF;
         int ws = fVideoWindowF.WindowStyle;
         ws = ws & ~(0x00800000); // Remove WS_BORDER
         ws = ws & ~(0x00400000); // Remove WS_DLGFRAME
         fVideoWindowF.WindowStyle = ws;
         fVideoWindowF.Owner = splitContainer1.Panel1.Handle.ToInt32();
         fVideoWindowF.MessageDrain = splitContainer1.Panel1.Handle.ToInt32();
         fVideoWindowF.SetWindowPosition(-fVideoWindowBorderF, -fVideoWindowBorderF, 
                                         fVideoWindowWF + 2*fVideoWindowBorderF, 
                                         fVideoWindowHF + 2*fVideoWindowBorderF);
         //fVideoWindowF.HideCursor(-1);   // -1 is true
      }

      /// <summary>
      /// ZoomFrontCamera
      /// </summary>
      private void ZoomFrontCamera()
      {
         
         // front camera
         var h = fBotYF - fTopYF;

         if (h < 10 || fZoomedF) {
            return;
         }

         Session.RoiY[0] = Session.FlipVideo == 1 ? (short)(fVideoWindowHF - fBotYF) : fTopYF;
         
         var w = (short)(h * fVideoWindowWF/(float)fVideoWindowHF);
         Session.RoiX[0] = (short)((fVideoWindowWF - w)*0.5 + Session.ShiftX[0]);
         Session.RoiWidth[0] = w;

         // hide y lines
         fBotYF = fVideoWindowHF;
         iCheckPosFilterF.SetCropArea(0, fBotYF, (int)ELineType.kBottomLine);  // blue line (bottom)
         fTopYF = 0;
         iCheckPosFilterF.SetCropArea(0, fTopYF, (int)ELineType.kTopLine);       // yellow line (top)

         fZoomedF = true;
      }

      private void UnzoomFrontalCamera()
      {
         
         // frontal camera
         fVideoWindowF.SetWindowPosition(-fVideoWindowBorderF, -fVideoWindowBorderF, 
                                         fVideoWindowWF + 2*fVideoWindowBorderF, 
                                         fVideoWindowHF + 2*fVideoWindowBorderF);
         // draw lines
         fBotYF = fVideoWindowHF - fOffF;
         iCheckPosFilterF.SetCropArea(0, fBotYF, (int)ELineType.kBottomLine); // blue line (bottom)
         fTopYF = fOffF;
         iCheckPosFilterF.SetCropArea(0, fTopYF, (int)ELineType.kTopLine);    // yellow line (top)

         fLeftXF = fOffF;
         iCheckPosFilterF.SetCropArea(fLeftXF, 0, (int)ELineType.kLeftLine);
         fRightXF = fVideoWindowWF - fOffF;
         iCheckPosFilterF.SetCropArea(fRightXF, 0, (int)ELineType.kRightLine);
         fZoomedF = false;
      }

      private void HideLinesF()
      {
         // y lines
         fBotYF = fVideoWindowHF;
         iCheckPosFilterF.SetCropArea(0, fBotYF, (int)ELineType.kBottomLine);  // blue line (bottom)
         fTopYF = 0;
         iCheckPosFilterF.SetCropArea(0, fTopYF, (int)ELineType.kTopLine);       // yellow line (top)

        // x lines
         fLeftXF = 0;
         iCheckPosFilterF.SetCropArea(fLeftXF, 0, (int)ELineType.kLeftLine);
         fRightXF = fVideoWindowWF;
         iCheckPosFilterF.SetCropArea(fRightXF, 0, (int)ELineType.kRightLine);

         splitContainer1.Panel1.UseWaitCursor = false;
         toolStrip.UseWaitCursor = false;

      }

      private void CleanupF()
      {
        if (fFilgraphManagerF != null) {
            if (fRtpStreamF != null) {
               fRtpStreamF.UnblockNextFrame();
            }

            fFilgraphManagerF.Stop();
            FilterGraph.RemoveAllFilters(fFilgraphManagerF);
            fFilgraphManagerF = null;
         }
      }  

      private void OnVideoMouseDownF(object sender, MouseEventArgs e) {

         short y = (short)e.Y;
         short x = (short)e.X;

         y = (short)(fVideoWindowHF - y);
#if LOCAL_DEBUG
   MessageBox.Show("x = " + x + " y = " + y);
#endif
         fVideoCursorCatchedF = false;
         fPressedLineType = ELineType.kNonLine;;

         if (Math.Abs(y - fTopYF)  < 5) {
            fPressedLineType = ELineType.kTopLine;
            fVideoCursorCatchedF = true;
            return;
         }

         if (Math.Abs(y - fBotYF) < 5) {
            fPressedLineType = ELineType.kBottomLine;
            fVideoCursorCatchedF = true;
            return;
         }

         if (Math.Abs(x - fLeftXF) < 5) {
            fPressedLineType = ELineType.kLeftLine;
            fVideoCursorCatchedF = true;
            return;
         }

         if (Math.Abs(x - fRightXF) < 5) {
            fPressedLineType = ELineType.kRightLine;
            fVideoCursorCatchedF = true;
            return;
         }
      }

      private void OnVideoMouseUpF(object sender, MouseEventArgs e) {
         fVideoCursorCatchedF = false;
         fPressedLineType = ELineType.kNonLine;

      }

      private void OnVideoMouseMoveF(object sender, MouseEventArgs e) {

         if (!fVideoCursorCatchedF) {
            return;
         }

         var y = (short)e.Y;
         var x = (short)e.X;

         if (fZoomedF) {
            btnZoom.Enabled = true;
         }

         if (y < 10 || y > fVideoWindowHF - 10) {
            return;
         }

         y = (short)(fVideoWindowHF - y);

         switch (fPressedLineType) {
            case ELineType.kBottomLine:
               fBotYF = y;
               iCheckPosFilterF.SetCropArea(x, y, (int)ELineType.kBottomLine);
               break;
            case ELineType.kTopLine:
               fTopYF = y;
               iCheckPosFilterF.SetCropArea(x, y, (int)ELineType.kTopLine);
               break;
            case ELineType.kLeftLine:
               fLeftXF = x;
               iCheckPosFilterF.SetCropArea(x, y, (int)ELineType.kLeftLine);
               break;
            case ELineType.kRightLine:
               fRightXF = x;
               iCheckPosFilterF.SetCropArea(x, y, (int)ELineType.kRightLine);
               break;
         }
      }

   }
}