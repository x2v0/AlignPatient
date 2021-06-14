// $Id: PulseControl.C 1700 2013-01-30 10:39:43Z onuchin $
// Author: Valeriy Onuchin   28.01.2013

#include <vector>

#include "TServerSocket.h"
#include "TCanvas.h"
#include "TSystem.h"
#include "TGraph.h"
#include "TAxis.h"
#include "TH1D.h"
#include "TSpectrum.h"
#include "TPolyMarker.h"
#include "TList.h"
#include "TMath.h"
#include "TColor.h"
#include "TDatime.h"
#include "TLatex.h"
#include "TStyle.h"
#include "TTimer.h"
#include "TROOT.h"
#include "TFrame.h"
#include "TDatime.h"


const Bool_t LOCAL_DEBUG = kFALSE;
const Bool_t DEBUG_FindPeaks = kFALSE;
const Bool_t DEBUG_Canvas = kFALSE;

const Double_t kStep = 0.001; // 1 ms = frequency 1KHz
const int kSpan = 4;          // number of buffers in accomulated buffer
const int kN = 1024;          // number of values in the input buffer
const int kNs = 256;
const int kNMAX = kN*kSpan;   // =4046 corresponds to 4096 ms
Bool_t kFirstFill = kTRUE;
const Float_t kZeroLevel = 1.3/5.*1024;  // ADC zero level
const int kPeriod = kN;

// ECG canvas
TCanvas *fECGcanvas = 0;
TSpectrum *fSpectrum = 0;
TH1F *fHist = 0;
TH1F *fRaw = 0;
TDatime *fTime = new TDatime();
TLatex *fTimeText = 0;
TTimer *fTimer = 0;
Bool_t fStopped = kTRUE;
Bool_t fECGcreated = kFALSE;

int fPulseMode = 's';

TString fFileECG = "Pulse.ecg"; // default
FILE *fECGfp = 0;

FILE *fLogFile = 0;

int npack = 0;
int nfoundOld = 1;
float *delta = new float[100];


const int kTimeSeriesBits = 2000;   // number of bits in time series hists
const int kIntervalType = 238;      // interval type for Polar HRM2 file format
const int kMaxHR = 200;             // maximum Heart Rate (bpm)
const int kMinHR = 40;              // minimum Heart Rate (bpm)
const float kMinRR = 60./kMaxHR;    // minimum R-R interval for rejection of bad events (millisec)
const float kMaxRR = 60./kMinHR;    // maximum R-R interval for rejection of bad events (millisec)

const int kMaxY = 600;  // maximum amplitude value for rejection of bad events
const int kMinY = -200; // minimum amplitude value for rejection of bad events




////////////////////////////////////////////////////////////////////////////////
//______________________________________________________________________________
void Reset()
{
   // reset all data and histos

   if (fHist) {
      fHist->Reset();
   }

   if (fRaw) {
      fRaw->Reset();
   }

   if (fECGcanvas) {
      fECGcanvas->Modified();
      fECGcanvas->Update();
   }
}

//______________________________________________________________________________
void SetPulseMode(int mode)
{
   // sets mode: 's' - measurement, 'a' - calibration

   fPulseMode = mode;
}

//______________________________________________________________________________
void *InitECGcanvas(char *name = "fECGcanvas")
{
   // Create ElectroCardioGram canvas 

   if (DEBUG_Canvas) {
      if (!fLogFile) {
         fLogFile = fopen("qq.log", "wt");
      }

      if (!fLogFile) {
         return 0;
      }
   }

   fECGcanvas = (TCanvas*)gROOT->GetListOfCanvases()->FindObject(name);
   //fprintf(fLogFile, "Name of canvas %s %x\n", gROOT->GetListOfCanvases()->First()->GetName(), fECGcanvas);
   //fprintf(fLogFile, "Canvas w = %d h = %d\n", fECGcanvas->GetWw(),  fECGcanvas->GetWh());

   if (!fECGcanvas) {
      //fprintf(fLogFile, "%s NOT found\n", name);
      //fclose(fLogFile);
      return 0;
   }

   TVirtualPad *savPad = gPad;
   fECGcanvas->cd();

   //fECGcanvas->DrawFrame(0, 0, 100, 600);
   fECGcanvas->SetFillColor(17);
   fECGcanvas->SetFrameFillColor(13);
   fECGcanvas->SetGrid();
   fECGcanvas->SetBorderMode(0);
   fECGcanvas->SetMargin(0.01, 0.0, 0.08, 0.0);

   if (!fHist) {
      fHist = new TH1F("ECG", "", kNMAX, 0, kNMAX);
      fHist->SetMaximum(400);
      fHist->SetMinimum(-200);
      fHist->GetYaxis()->SetNdivisions(10);
      fHist->GetXaxis()->SetNdivisions(16);
      fHist->GetXaxis()->SetLimits(0, kNMAX*kStep);
      fHist->SetLineColor(7);
      fHist->SetLineWidth(2);
      fHist->SetFillColor(0);
      fHist->SetStats(kFALSE);
      //fHist->GetXaxis()->SetTitle("");
      fHist->GetXaxis()->SetLabelSize(0.05);
      fHist->GetYaxis()->SetLabelSize(0.0);
      fHist->Draw();
   }

   fHist->Draw();
 
   if (fTimeText) {
      delete fTimeText;
      fTimeText = 0;
   }

   fECGcanvas->Modified();
   //fECGcanvas->Update();

   fECGcreated = kTRUE;

   savPad->cd();

   fECGcanvas->SetEditable(kFALSE);

   return fECGcanvas;
}

//______________________________________________________________________________
void FindPeaks(short *buf)
{
   // - fill hist by buf data
   // - find heart beat peaks
   // - fill R-R intervals vector, Heart Rate vector
   // - save data to files 

   if (!fECGcanvas) {
      if (!InitECGcanvas()) {
         return;
      }
   }

   if (DEBUG_FindPeaks) {
      if (!fLogFile) {
         fLogFile = fopen("FindPeaks.log", "wt");
      }
   }

   if (!fECGfp) {
      //fECGfp = fopen(fFileECG.Data(), "wb+");
   }

   if (fStopped) {
      return;
   }

   npack++;

   fECGcanvas->cd();

   Float_t *Y = fHist->GetArray();
   //Float_t *R = fRaw->GetArray();

   int len = kNMAX - kNs;
   int sz = sizeof(Float_t)*len;

  // move hist content by kN cells
   memmove(Y + kNs, Y, sz);
   //memmove(R + kN, R, sz);

   Float_t meanY = 0;

   // fill first kN bins in the reverse order
   for (int k = 0; k < kNs; k++) {
      Y[k] = Float_t(buf[kNs - k - 1]); // - kZeroLevel;
      
      //R[k] = Float_t(buf[kNs - k - 1]); // - kZeroLevel;

 	   //if (fLogFile) {
 	   //   fprintf(fLogFile, "%f %d\n", Y[k], buf[kNs - k - 1]);
 	   //}

      // zero excitations
      Y[k] = (Y[k] > 600) || (Y[k] < -200) ? 0 : Y[k];
      meanY += Y[k];
   }
 
   meanY = meanY/kNs;

   if (!fSpectrum) {
      fSpectrum = new TSpectrum();
   }

   Float_t dest[2*kNs];

   // find peaks in ~4 seconds time interval
   int nfound = fSpectrum->SearchHighRes(Y, dest, kNs, 5, 30, kFALSE, 1, kFALSE, 3);

   Float_t value[kNs];

   // fill first kN bins with found values
   for (int i = 0; i < kNs; i++) {
      value[i] =  dest[i] - meanY;   // zero level correction
      fHist->SetBinContent(i, value[i]);
   }

   if (fECGfp) {
      fwrite(value, 1, sizeof(value), fECGfp);
   }

   nfound = fSpectrum->Search(fHist, 6, "", 0.6);

   float *xpeaksNew = fSpectrum->GetPositionX();

   int *idx = new int[nfound];
   TMath::Sort(nfound, xpeaksNew, idx, kFALSE);

   Float_t beat = 1;

   if (nfound > 1) {
      beat = xpeaksNew[idx[1]] - xpeaksNew[idx[0]];

      if (beat > 0.2) {
         beat = 60./beat;   // beats per min
      } else {
         beat = 0;
      }

      TString title = Form("Pulse = %2.1f beats/min", beat);
      fHist->SetTitle(title);

      /*if ((beat > 180) || (beat < 40)) {
         gStyle->SetTitleTextColor(2);
      } else {
         gStyle->SetTitleTextColor(1);
      }*/
   }

   if (DEBUG_FindPeaks && fLogFile) {
      fprintf(fLogFile, "-------------------------------------\n");
      fprintf(fLogFile, "found %d %d\n", nfound, nfoundOld);
   }

   for (int i = 0; i < nfound - 1; i++) {
      delta[i] = xpeaksNew[idx[i + 1]] - xpeaksNew[idx[i]];

      if (DEBUG_FindPeaks && fLogFile) {
         fprintf(fLogFile, "delta[%d] = %f\n", i, delta[i]);
      }
   }

   float deltaN = 0;
   int ndiff = 0;
   int newDelta = 0;
   float lastX = 0;

   if (nfound >= nfoundOld) {
      ndiff = nfound - nfoundOld;
      newDelta = 0;

      if ((ndiff == 0) && (nfound > 1)) {
         deltaN = xpeaksNew[idx[1]] - xpeaksNew[idx[0]];

      } else {
         for (int i = 0; i < ndiff; i++) {
            deltaN = xpeaksNew[idx[i + 1]] - xpeaksNew[idx[i]];

            if (deltaN <= 0) {
               break;
            }
         }
      }
   }

   nfoundOld = nfound;

   delete idx;
}

//______________________________________________________________________________
void UpdateTime()
{
   //

   if (fStopped) {
      return;
   }

   //fTime->Set();
   //fTimeText->DrawLatex(3., 420, fTime->AsSQLString());

   fECGcanvas->Modified();
   fECGcanvas->Update();
}

//______________________________________________________________________________
void PulseControl()
{
   //

   Bool_t connected = kFALSE;
   int events = 0;

   InitECGcanvas();

   TServerSocket *ss = new TServerSocket(30044, kTRUE);

   //printf("Server Socket created. Wating for accept\n");

   TSocket *s0 = ss->Accept();

   //printf("Socket created and accepted\n");

   if (s0) {
      connected = kTRUE;
   } else {
      printf("Connection failed\n");
      return;
   }

   short buf[kN];

   int nn = kN/kPeriod;

   while (1) {
      int n = -1;

      if (!connected) {
         ss = new TServerSocket(30044, kTRUE);
         s0 = ss->Accept();

         //printf("Create socket\n");

         if (!s0) {
            connected = kFALSE;
            delete ss;
            ss = 0;
            gSystem->Sleep(2000); // sleep 2 sec
            continue;
         } else {
            connected = kTRUE;
            //printf("Connected\n");
         }
      }

      n = s0->RecvRaw(buf, kN*2);
      events++;

      if (n <= 0) {
         connected = kFALSE;
         delete ss;
         ss = 0;
         //printf("disconnected\n");

         gSystem->ProcessEvents();
         continue;
      }

      for (int i = 0; i < kN; i += kNs) {
         FindPeaks(buf + i);
      }


      if (fECGcanvas) {
         fECGcanvas->Modified();
         fECGcanvas->Update();
         //fECGcanvas->Print("pulse.gif+60");
      }

      //fECGcanvas->Print("pulse.gif+60");
   }
}

//_____________________________________________________________________________
void TestPulseData(const char *fname  = "PulseData.dat")
{
   //

   FILE *fp = fopen(fname, "rb");

   if (!fp) {  // failed to open
      return;
   }

   short buf[kN];

   InitECGcanvas();

   int result;
   int n = 0;

   while (!feof(fp)) {
      result = fread(buf, 2, kN, fp);

      FindPeaks(buf);

      fTime->Set();
      fTimeText->DrawLatex(3., 420, fTime->AsSQLString());

      fECGcanvas->Modified();
      fECGcanvas->Update();

      //fECGcanvas->Print("pulse.gif+60");

      gSystem->ProcessEvents();
      gSystem->Sleep(200);
      n++;
   }
   fclose(fp);
   printf("n = %d\n", n);

   // make infinite animation by adding "++" to the file name
   //fECGcanvas->Print("pulse.gif++");
}

//_____________________________________________________________________________
int OpenPulseFile(const char *fname = "PulseMonitor.ecg")
{
   //

   if (LOCAL_DEBUG) {
      if (!fLogFile) {
         fLogFile = fopen("qq.log", "wt");
      }

      fprintf(fLogFile, "File name - %s\n", fname);
   }

   fFileECG = fname;
   fECGfp = fopen(fFileECG, "rb");

   if (LOCAL_DEBUG && fLogFile) {
      fprintf(fLogFile, "fp - 0x%x\n", fECGfp);
   }

   if (!fECGfp) {  // failed to open
      fStopped = kTRUE;
      return -1;
   }

   return 0;
}

//_____________________________________________________________________________
int ClosePulseFile()
{
   //

   fStopped = kTRUE;

   if (!fECGfp) {
      return -1;
   }

   int ret = fclose(fECGfp);
   fECGfp = 0;


   if (LOCAL_DEBUG && fLogFile) {
      fclose(fLogFile);
   }

   return ret;
}

//_____________________________________________________________________________
void StartProcessing()
{
   //

   fStopped = kFALSE;
}


//_____________________________________________________________________________
void StopProcessing()
{
   //

   fStopped = kTRUE;
}

//_____________________________________________________________________________
int ProcessPulseData(char *data, int len)
{
   //

   short *buf = (short*)data; //( len%2 == 0 ? data + 1 : data);

   //if (!fLogFile) {
   //   fLogFile = fopen("qq.log", "wt");
   //}
   //
   //for (int i = 0; i < len; i++) {
   //   fprintf(fLogFile, "data[%d] = 0x%x %d .. %d\n", i, data[i], buf[i/2], len);
   //}


   if (fStopped) {
      return -1;
   }

   for (int i = 0; i < kN; i += kNs) {
      FindPeaks(buf + i);
   }

   if (fECGcanvas) {
      fECGcanvas->Modified();
      fECGcanvas->Update();
      //fECGcanvas->Print("pulse.gif+60");
   }

   return 0;
}
