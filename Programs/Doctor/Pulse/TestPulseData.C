// $Id: TestPulseData.C 983 2012-03-04 09:39:08Z onuchin $
// Author: Valeriy Onuchin   27.01.2011


#include "TCanvas.h"
#include "TSystem.h"
#include "TGraph.h"
#include "TAxis.h"
//#include "TH1D.h"

TCanvas *fCanvas = 0;

//______________________________________________________________________________
void initCanvas()
{

   if (fCanvas) {   // already exists
      fCanvas->Clear();
   }

   fCanvas = new TCanvas("Pulse Data Test", "Pulse Data Test", 600, 300);
   fCanvas->DrawFrame(0, 0, 100, 600);
   fCanvas->SetFillColor(17);
   fCanvas->SetFrameFillColor(33);
   fCanvas->SetGrid();
   fCanvas->SetBorderMode(0);
}

//______________________________________________________________________________
void TestPulseData()
{
   //

   FILE *fp = fopen("PulseData.dat", "rb");

   if (!fp) {
      return;
   }

   fseek(fp, 0L, SEEK_END);   // seek to the end of file 
   int sz = ftell(fp)/2;      // file size in shorts 
   fseek(fp, 0L, SEEK_SET);   // seek back to the beginning

   short *buf = new short[sz];          // buffer with pulse data
   int result = fread(buf, 2, sz, fp);  // read the whole file

   Float_t *x = new Float_t[sz];
   Float_t *y = new Float_t[sz];

   for (int i = 0; i < sz; i++) {
      x[i] = i;
      y[i] = buf[i];
   }

   initCanvas();

   TGraph *gr = new TGraph(sz, x, y);
   gr->SetMaximum(800);
   gr->SetMinimum(-300);
   gr->SetLineColor(2);
   gr->GetYaxis()->SetNdivisions(10);
   gr->GetXaxis()->SetNdivisions(8);
   gr->GetXaxis()->SetLimits(0, sz);
   gr->SetTitle("Pulse Data");
   gr->SetName("Pulse Data");

   gr->Draw("alp");
}

