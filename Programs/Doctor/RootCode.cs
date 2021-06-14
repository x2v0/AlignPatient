// $Id: RootCode.cs 2028 2014-01-10 14:19:59Z onuchin $
// Author: Valeriy Onuchin   09.07.2013

using System;

//#define LOCAL_DEBUG
using System.Windows.Forms;
using ROOT;

namespace DoctorDisplay
{
   public partial class DoctorDisplayForm
   {

      private TCanvas fECGcanvas;

      /// <summary>
      /// true if ROOT macros are loaded
      /// </summary>
      private bool fROOTloaded;

      private void InitECGcanvas(object canvas, TCanvas.CanvasCreateEventArgs args)
      {
         if (fROOTloaded) {
            TROOT.ProcessLine(String.Format("InitECGcanvas(\"{0}\")", fECGcanvas.Name));
         }
      }

      private void LoadROOT()
      {
         fROOTloaded = !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("ROOTSYS"));

         if (fROOTloaded) {
            fECGcanvas = new TCanvas();
            patientDataTable.Controls.Add(fECGcanvas,0,0);
            // 
            // fECGcanvas
            // 
            fECGcanvas.BackColor = System.Drawing.Color.White;
            fECGcanvas.BorderStyle = BorderStyle.Fixed3D;
            fECGcanvas.Dock = DockStyle.Right;
            fECGcanvas.Location = new System.Drawing.Point(337,3);
            fECGcanvas.Name = "fECGcanvas";
            fECGcanvas.Size = new System.Drawing.Size(353,260);
            fECGcanvas.TabIndex = 7;
            fECGcanvas.UseWaitCursor = true;
            fECGcanvas.OnCanvasCreated += InitECGcanvas;

            fROOTloaded = TROOT.LoadMacro("DoctorDisplay.macros", "PulseControl.C") == 0;
         }
      }
   }
}
