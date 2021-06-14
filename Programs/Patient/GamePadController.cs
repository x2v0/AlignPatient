// $Id: GamePadController.cs 2016 2013-12-02 09:53:20Z onuchin $
 
// Author: Valeriy Onuchin  08.11.2013
 

//#define LOCAL_DEBUG


using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using P;
using P.Net.Rtp;

namespace PatientDisplay
{
   public partial class PatientForm {


      ///To keep track of the current and previous state of the gamepad
      /// <summary>
      /// The current state of the controller
      /// </summary>
      GamePadState fGamePadState;

      /// <summary>
      /// The previous state of the controller
      /// </summary>
      GamePadState fPreviousState;

      /// <summary>
      /// Keeps track of the current controller
      /// </summary>
      PlayerIndex fPlayerIndex = PlayerIndex.One;

      /// <summary>
      /// Counter for limiting the time for which the vibration motors are on.
      /// </summary>
      // int fVibrationCountdown = 0;


      /// <summary>
      /// y-positions of lines defining crop area
      /// initial values are hardcoded for a while
      /// </summary>
      private short fTopY = 51; //
      private short fBotY = 461;
      private short fPrevTopY;
      private short fPrevBotY;

      private short fLeftX;
      private short fRightX;
      private short fPrevLeftX;
      private short fPrevRightX;

      #region GamePad Processing


      private void InitLines()
      {
         if (iCheckPosFilter == null) {
            return;
         }

         if (!fGamePadState.IsConnected) {
            HideLines();
            return;
         }

         // y lines
         fBotY = (short)Session.PDispHeight;
         iCheckPosFilter.SetCropArea(0, fBotY, (int)ELineType.kBottomLine); // blue line (bottom)
         fTopY = 0;
         iCheckPosFilter.SetCropArea(0, fTopY, (int)ELineType.kTopLine);    // yellow line (top)

         // x lines
         fLeftX = 0;
         iCheckPosFilter.SetCropArea(fLeftX, 0, (int)ELineType.kLeftLine);

         if (fFrontal) {
            fRightX = (short)(Session.PDispWidth - Session.ProfileWidth);
         }

         iCheckPosFilter.SetCropArea(fRightX, 0, (int)ELineType.kRightLine);
      }

      private void HideLines()
      {
         if (iCheckPosFilter == null) {
            return;
         }

         // y lines
         fBotY = (short)Session.PDispHeight;
         iCheckPosFilter.SetCropArea(0, fBotY, (int)ELineType.kBottomLine);  // blue line (bottom)
         fTopY = 0;
         iCheckPosFilter.SetCropArea(0, fTopY, (int)ELineType.kTopLine);     // yellow line (top)

         // x lines
         fLeftX = 0;
         iCheckPosFilter.SetCropArea(fLeftX, 0, (int)ELineType.kLeftLine);
         fRightX = (short)Session.PDispWidth;
         iCheckPosFilter.SetCropArea(fRightX, 0, (int)ELineType.kRightLine);
      }


      private void CreateGamePadController() 
      {
         // start game pad processing
         fPlayerIndex = PlayerIndex.One;

         if (fGamePadTimer != null) {
            fGamePadTimer.Start();
         }
      }

      private void Zoom()
      {

         int h = fBotY - fTopY;

         if (h < 10 || fZoomed) {
            return;
         }

         Session.RoiY[fIdx] = fTopY;
         
         var w = (short)(h * (float)Session.PDispWidth/Session.PDispHeight);
         Session.RoiX[fIdx] = (short)((Session.PDispWidth - w)*0.5 + Session.ShiftX[fIdx]);
         Session.RoiWidth[fIdx] = w;

         HideLines();

         SetXYW();

         fZoomed = true;
      }

      private void ZoomOut()
      {

         Session.RoiWidth[fIdx] = 
         Session.RoiX[fIdx] = 
         Session.RoiY [fIdx]= 0;      // unzoom

         SetXYW();
         //ResetPatient();

         HideLines();

         fZoomed = false;
         fPressedLineType = ELineType.kNonLine;
      }

      private void CheckControllerState()
      {
         fPreviousState = fGamePadState;
         fGamePadState = GamePad.GetState(fPlayerIndex);
         int connected = fGamePadState.IsConnected ? 1 : 0;

         uint ssrc = Session.Participant.SSRC;

         // GPAD subtypes:
         //    0 - disconnected
         //    1 - connected
         Session.RtpSession.SendAppPacket(ssrc, "GPAD", (byte)connected, null, Rtcp.RtcpInterval.Now);
      } 

      private void UpdateControllerState()
      {

         fPreviousState = fGamePadState;
         fGamePadState = GamePad.GetState(fPlayerIndex);

         if (!fGamePadState.IsConnected) {
            HideLines();
            return;
         }

         if (!fGamePadState.Buttons.Equals(fPreviousState.Buttons)) {

            if (fGamePadState.Buttons.LeftStick == ButtonState.Pressed) {
               ZoomOut();
               //Reset(); 
            } else if (fGamePadState.Buttons.RightStick == ButtonState.Pressed) {
               Zoom();
               //iCheckPosFilter.Reset();
               //iCheckPosFilter.SetCropArea(x, y, w);
               //HideLines();
            }

            if (fGamePadState.Buttons.A == ButtonState.Pressed) {
               //buttonSnapClick(null, null);
            }

            if (fGamePadState.Buttons.B == ButtonState.Pressed) {
               //buttonLoadClick(null, null);
            }
         }

         // Important! Due to flipping video on the Doctor machine 
         //
         // bottom Y line
         var ry = (short)(fGamePadState.ThumbSticks.Right.Y*120);
         var dy = (short)(ry - fPrevBotY);
         const int h = 480;
         const int hh = 2;


         if (fPrevBotY >= 0) {
            if (ry > fPrevBotY) {
               fBotY += dy;

               if (fBotY > hh && fBotY < h - hh) {
                  iCheckPosFilter.SetCropArea(0, fBotY, (int)ELineType.kBottomLine);
               } else {
                  fBotY -= dy;
               }
            }
            fPrevBotY = ry;
        } else if (fPrevBotY <= 0) {
            if (ry < fPrevBotY) {
               fBotY += dy;

               if (fBotY > hh && fBotY < h - hh) {
                  iCheckPosFilter.SetCropArea(0, fBotY, (int)ELineType.kBottomLine);
               } else {
                  fBotY -= dy;
               }
            }
            fPrevBotY = ry;
         }

         // top Y line
         var ly = (short)(fGamePadState.ThumbSticks.Left.Y*120);
         dy = (short)(ly - fPrevTopY);

         if (fPrevTopY >= 0) {
            if (ly > fPrevTopY) {
               fTopY += dy;

               if (fTopY > hh && fLeftX < h - hh) {
                  iCheckPosFilter.SetCropArea(0, fTopY, (int)ELineType.kTopLine);
               } else {
                  fTopY -= dy;
               }
            }
            fPrevTopY = ly;
        } else if (fPrevTopY <= 0) {
            if (ly < fPrevTopY) {
               fTopY += dy;

               if (fTopY > hh && fTopY < h - hh) {
                  iCheckPosFilter.SetCropArea(0, fTopY, (int)ELineType.kTopLine);
               } else {
                  fTopY -= dy;
               }
            }
            fPrevTopY = ly;
         }

         if (fFrontal == false) {
            return;
         }

         // right X line
         var rx = (short)(fGamePadState.ThumbSticks.Left.X*120);
         var dx = (short)(rx - fPrevRightX);
         const int w = 640;
         const int ww = 2;

         if (fPrevRightX >= 0) {
            if (rx > fPrevRightX) {
               fRightX += dx;

               if (fRightX > ww && fRightX < w - ww) {
                  iCheckPosFilter.SetCropArea(fRightX, 0, (int)ELineType.kRightLine);
               } else {
                  fRightX -= dx;
               }
            }
            fPrevRightX = rx;
        } else if (fPrevRightX <= 0) {
            if (rx < fPrevRightX) {
               fRightX += dx;
               if (fRightX > ww && fRightX < w - ww) {
                  iCheckPosFilter.SetCropArea(fRightX, 0, (int)ELineType.kRightLine);
               } else {
                  fRightX -= dx;
               }
            }
            fPrevRightX = rx;
         }

         // Important! Due to flipping video on the Doctor machine  
         // left line procesed by Right.X button and vice versa (.. code cleanup must be done  ASAP) 
         // left X line
         var lx = (short)(fGamePadState.ThumbSticks.Right.X*120);
         dx = (short)(lx - fPrevLeftX);

         if (fPrevLeftX >= 0) {
            if (lx > fPrevLeftX) {
               fLeftX += dx;

               if (fLeftX > ww && fLeftX < w - ww) {
                  iCheckPosFilter.SetCropArea(fLeftX, 0, (int)ELineType.kLeftLine);
               } else { 
                  fLeftX -= dx;
               }
            }
            fPrevLeftX = lx;
        } else if (fPrevLeftX <= 0) {
            if (lx < fPrevLeftX) {
               fLeftX += dx;

               if (fLeftX > ww && fLeftX < w - ww) {
                  iCheckPosFilter.SetCropArea(fLeftX, 0, (int)ELineType.kLeftLine);
               } else {
                  fLeftX -= dx;
               }
            }
            fPrevLeftX = lx;
         }
      }

      private void GamePadTimerTick(object sender, EventArgs e)
      {
         UpdateControllerState();
      }

      #endregion GamePad Processing

    }
}