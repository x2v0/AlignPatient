// $Id: VideoWindowP.cs 2102 2014-02-26 12:47:02Z onuchin $
// Author: Valeriy Onuchin   09.07.2013

//#define DEBUG
//#define LOCAL_DEBUG

using System;
using System.Windows.Forms;


//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Input;
//using Input = Microsoft.Xna.Framework.Input;

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
      /// Receives the video data across the network
      /// </summary>
      private RtpStream fRtpStreamP;

      /// <summary>
      /// Filtergraph on the receiving side
      /// </summary>
      private FilgraphManagerClass fFilgraphManagerP;

      private IVideoWindow fVideoWindowP;

      /// <summary>
      /// Transform filter on the receiving side. Used to flip/mirrow frames
      /// </summary>
      private CheckPosFilter fCheckPosFilterP;

     /// <summary>
      /// Interface to transform filter on the receiving side. Used to flip/mirrow frames
      /// </summary>
      private ICheckPosFilter iCheckPosFilterP;

     /// <summary>
      /// some constants
      /// </summary>
      private const short fOffP = 50;
      private const short fVideoWindowWP = 640;
      private const short fVideoWindowHP = 480;
      private const short fVideoWindowBorderP = 4;

      /// <summary>
      /// y-positions of lines defining crop area
      /// initial values are hardcoded for a while
      /// </summary>

      private short fTopYP = fOffP; //
      private short fBotYP = fVideoWindowHP - fOffP;
      private short fPrevTopYP;
      private short fPrevBotYP;

      private short fLeftXP = fOffP;
      private short fRightXP = fVideoWindowWP - fOffP;
      private short fPrevLeftXP;
      private short fPrevRightP;

      private bool fVideoCursorCatchedP;
      private bool fZoomedP;
      //private bool fCheckMode;
      private bool fGraphCreatedP;

      private void CreateVideoGraphP()
      {

         if (fGraphCreatedP)  {
            return;
         } else {
            fGraphCreatedP = true;
         }

         // Tell the stream we will poll it for data with our own (DShow) thread
         // Instead of receiving data through the FrameReceived event
         fRtpStreamP.IsUsingNextFrame = true;

         // Create receiving filtergraph
         fFilgraphManagerP = new FilgraphManagerClass();
         var iGraphBldP = (IGraphBuilder)fFilgraphManagerP;

#if DEBUG
         FilterGraph.AddToRot(iGraphBldP);
#endif

         var rtpSource = P.DShow.RtpSourceClass.CreateInstance();
         ((IRtpSource)rtpSource).Initialize(fRtpStreamP);

         iGraphBldP.AddFilter(rtpSource, "RtpSource Profile");
         var outPin = Filter.GetPin(rtpSource, _PinDirection.PINDIR_OUTPUT,
                                    Guid.Empty, Guid.Empty, false, 0);

            // connect transform filter just before Video Renderer
         fCheckPosFilterP = Filter.CheckPosFilter();
         iCheckPosFilterP = (ICheckPosFilter)fCheckPosFilterP.BaseFilter;

         // flip & mirrow 
         iCheckPosFilterP.Flip(Session.FlipVideo);
         iCheckPosFilterP.Mirrow(Session.FlipVideo);

         // y lines
         fBotYP = fVideoWindowHP - fOffP;
         iCheckPosFilterP.SetCropArea(0, fBotYP, (int)ELineType.kBottomLine); // blue line (bottom)
         fTopYP = fOffP;
         iCheckPosFilterP.SetCropArea(0, fTopYP, (int)ELineType.kTopLine);    // yellow line (top)

         // x lines
         fLeftXP = fOffP;
         iCheckPosFilterP.SetCropArea(fLeftXP, 0, (int)ELineType.kLeftLine);
         fRightXP = fVideoWindowWP - fOffP;
         iCheckPosFilterP.SetCropArea(fRightXP, 0, (int)ELineType.kRightLine);

         var cpf = fCheckPosFilterP.BaseFilter;
         iGraphBldP.AddFilter(cpf, "Check Position Filter Profile");
         fCheckPosFilterP.AddedToGraph(fFilgraphManagerP);

         var cpfIn = GetInputPin(cpf);
         //var renderIn = GetInputPin(renderer);
         var cpfOut = GetOutputPin(cpf);

         // connect out pin to check position filter input pin
         iGraphBldP.Connect(outPin, cpfIn);
         iGraphBldP.Render(cpfOut);

         VideoWindowP();
         fFilgraphManagerP.Run();
      }

      private void VideoWindowP()
      {

         if (fVideoWindowP != null) {   // dispose
            fVideoWindowP.Visible = 0;
            fVideoWindowP.Owner = 0;
            fVideoWindowP = null;
         }

         // Must render before adjusting the VideoWindow
         fVideoWindowP = fFilgraphManagerP;
         int ws = fVideoWindowP.WindowStyle;
         ws = ws & ~(0x00800000); // Remove WS_BORDER
         ws = ws & ~(0x00400000); // Remove WS_DLGFRAME
         fVideoWindowP.WindowStyle = ws;
         fVideoWindowP.Owner = splitContainer1.Panel2.Handle.ToInt32();
         fVideoWindowP.MessageDrain = splitContainer1.Panel2.Handle.ToInt32();
         fVideoWindowP.SetWindowPosition(-fVideoWindowBorderP, -fVideoWindowBorderP, 
                                         fVideoWindowWP + 2*fVideoWindowBorderP, 
                                         fVideoWindowHP + 2*fVideoWindowBorderP);

         //fVideoWindowP.HideCursor(-1);   // -1 is true
      }

      /// <summary>
      /// ZoomProfileCamera
      /// </summary>
      private void ZoomProfileCamera()
      {
         
         // profile camera
         var h = fBotYP - fTopYP;

         if (h < 10 || fZoomedP) {
            return;
         }

         Session.RoiY[1] = Session.FlipVideo == 1 ? (short)(fVideoWindowHP - fBotYP) : fTopYP;
         
         var w = (short)(h * fVideoWindowWP/(float)fVideoWindowHP);
         Session.RoiX[1] = (short)((fVideoWindowWP - w)*0.5 + Session.ShiftX[1]);
         Session.RoiWidth[1] = w;

         // hide y lines
         fBotYP = fVideoWindowHP;
         iCheckPosFilterP.SetCropArea(0, fBotYP, (int)ELineType.kBottomLine);  // blue line (bottom)
         fTopYP = 0;
         iCheckPosFilterP.SetCropArea(0, fTopYP, (int)ELineType.kTopLine);       // yellow line (top)
      }

      private void UnzoomProfileCamera()
      {
         
         // profile camera
         fVideoWindowP.SetWindowPosition(-fVideoWindowBorderP, -fVideoWindowBorderP, 
                                         fVideoWindowWP + 2*fVideoWindowBorderP, 
                                         fVideoWindowHP + 2*fVideoWindowBorderP);
         // draw lines
         fBotYP = fVideoWindowHP - fOffP;
         iCheckPosFilterP.SetCropArea(0, fBotYP, (int)ELineType.kBottomLine); // blue line (bottom)
         fTopYP = fOffP;
         iCheckPosFilterP.SetCropArea(0, fTopYP, (int)ELineType.kTopLine);    // yellow line (top)

         fLeftXP = fOffP;
         iCheckPosFilterP.SetCropArea(fLeftXP, 0, (int)ELineType.kLeftLine);
         fRightXP = fVideoWindowWP - fOffP;
         iCheckPosFilterP.SetCropArea(fRightXP, 0, (int)ELineType.kRightLine);
         fZoomedP = false;
      }

      private void CleanupP()
      {

         if (fFilgraphManagerP != null) {
            if (fRtpStreamP != null) {
               fRtpStreamP.UnblockNextFrame();
            }

            fFilgraphManagerP.Stop();
            FilterGraph.RemoveAllFilters(fFilgraphManagerP);
            fFilgraphManagerP = null;
         }
      }

      private void HideLinesP()
      {
         // y lines
         fBotYP = fVideoWindowHP;
         iCheckPosFilterF.SetCropArea(0, fBotYP, (int)ELineType.kBottomLine);  // blue line (bottom)
         fTopYP = 0;
         iCheckPosFilterF.SetCropArea(0, fTopYP, (int)ELineType.kTopLine);       // yellow line (top)

        // x lines
         fLeftXP = 0;
         iCheckPosFilterF.SetCropArea(fLeftXP, 0, (int)ELineType.kLeftLine);
         fRightXP = fVideoWindowWP;
         iCheckPosFilterF.SetCropArea(fRightXP, 0, (int)ELineType.kRightLine);

         splitContainer1.Panel2.UseWaitCursor = false;
         toolStrip.UseWaitCursor = false;
      }

      private void OnVideoMouseDownP(object sender, MouseEventArgs e)
      {

         short y = (short)e.Y;
         short x = (short)e.X;

         y = (short)(fVideoWindowHP - y);
#if LOCAL_DEBUG
   MessageBox.Show("x = " + x + " y = " + y);
#endif
         fVideoCursorCatchedP = false;
         fPressedLineType = ELineType.kNonLine;;

         if (Math.Abs(y - fTopYP)  < 5) {
            fPressedLineType = ELineType.kTopLine;
            fVideoCursorCatchedP = true;
            return;
         }

         if (Math.Abs(y - fBotYP) < 5) {
            fPressedLineType = ELineType.kBottomLine;
            fVideoCursorCatchedP = true;
            return;
         }

         if (Math.Abs(x - fLeftXP) < 5) {
            fPressedLineType = ELineType.kLeftLine;
            fVideoCursorCatchedP = true;
            return;
         }

         if (Math.Abs(x - fRightXP) < 5) {
            fPressedLineType = ELineType.kRightLine;
            fVideoCursorCatchedP = true;
            return;
         }
      }

      private void OnVideoMouseUpP(object sender, MouseEventArgs e) 
      {
         fVideoCursorCatchedP = false;
         fPressedLineType = ELineType.kNonLine;
      }

      private void OnVideoMouseMoveP(object sender, MouseEventArgs e) 
      {

         if (!fVideoCursorCatchedP) {
            return;
         }

         var y = (short)e.Y;
         var x = (short)e.X;

         if (fZoomedP) {
            btnZoom.Enabled = true;
         }

         if (y < 10 || y > fVideoWindowHP - 10) {
            return;
         }

         y = (short)(fVideoWindowHP - y);

         switch (fPressedLineType) {
            case ELineType.kBottomLine:
               fBotYP = y;
               iCheckPosFilterP.SetCropArea(x, y, (int)ELineType.kBottomLine);
               break;
            case ELineType.kTopLine:
               fTopYP = y;
               iCheckPosFilterP.SetCropArea(x, y, (int)ELineType.kTopLine);
               break;
            case ELineType.kLeftLine:
               fLeftXP = x;
               iCheckPosFilterP.SetCropArea(x, y, (int)ELineType.kLeftLine);
               break;
            case ELineType.kRightLine:
               fRightXP = x;
               iCheckPosFilterP.SetCropArea(x, y, (int)ELineType.kRightLine);
               break;
         }
      }
   }
}