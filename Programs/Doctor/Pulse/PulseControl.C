// $Id: PulseControl.C 1217 2012-09-14 09:16:01Z onuchin $
// Author: Valeriy Onuchin   27.01.2011

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



const Double_t kStep = 0.001; // 1 ms = frequency 1KHz
const int kSpan = 4;          // number of buffers in accomulated buffer
const int kN = 1024;          // number of values in the input buffer
const int kNs = 256;
const int kNMAX = kN*kSpan;   // =4046 corresponds to 4096 ms
Bool_t kFirstFill = kTRUE;
const Float_t kZeroLevel = 1.3/5.*1024;  // ADC zero level
const int kPeriod = kN;


TCanvas *fCanvas = 0;
TSpectrum *fSpectrum = 0;
TH1F *fHist = 0;
TH1F *fRaw = 0;
TDatime *fTime = new TDatime();
TLatex *fTimeText = 0;
TTimer *fTimer = 0;
Bool_t fStopped = kFALSE;



///////////////////////////////////////////////////////////////////////////////
//______________________________________________________________________________
void FindPeaks(short *buf)
{
   // - fill hist by buf data
   // - find heart beat peaks

   Float_t *Y = fHist->GetArray();
   //Float_t *R = fRaw->GetArray();

   int len = kNMAX - kNs;
   int sz = sizeof(Float_t)*len;

  // move hist content by kN cells
   memmove(Y + kNs, Y, sz);
  //memmove(R + kN, R, sz);

   Float_t meanY = 0;
    // fill first kN bins in reverse order
   for (int k = 0; k < kNs; k++) {
      Y[k] = Float_t(buf[kNs - k - 1]);// - kZeroLevel;
      //R[k] = Float_t(buf[k]) - kZeroLevel;

      // zero excitations
      //Y[k] = (Y[k] > 800) || (Y[k] < -200) ? 0 : Y[k];
      meanY += Y[k];
   }

   meanY = meanY/kNs;

   if (!fSpectrum) {
      fSpectrum = new TSpectrum();
   }
 
   Float_t dest[2*kNs];

   // find peaks in ~4 seconds time interval
   int nfound = fSpectrum->SearchHighRes(Y, dest, kNs, 5, 30, kFALSE, 1, kFALSE, 3);

   // fill first kN bins with found values
   for (int i = 0; i < kNs; i++) {
      Float_t value =  dest[i] - meanY;   // zero level correction
      fHist->SetBinContent(i, value); //value);
   }

   nfound = fSpectrum->Search(fHist, 6, "", 0.6);

   float *xpeaks = fSpectrum->GetPositionX();

   int *idx = new int[nfound];
   TMath::Sort(nfound, xpeaks, idx, kFALSE);

   Float_t beat = 1; //(xpeaks[idx[nfound - 1]] - xpeaks[idx[0]])/(nfound-1);
   
   if (nfound > 0) {
      beat = xpeaks[idx[1]] - xpeaks[idx[0]];
   }

   if (beat > 0.1) {
      beat = 60/beat;   // beats per min
   } else {
      beat = 0;
   }

   TString title = Form("Pulse = %2.1f beats/min", beat);
   fHist->SetTitle(title);

   if ((beat > 150) || (beat < 50)) {
      gStyle->SetTitleTextColor(2);
   } else {
      gStyle->SetTitleTextColor(1);
   }

   fTime->Set();
   fTimeText->DrawLatex(3., 420, fTime->AsSQLString());

   if (!fStopped) {
      fCanvas->Modified();
      fCanvas->Update();
   }

   gSystem->ProcessEvents();

   delete idx;
}

//______________________________________________________________________________
void UpdateTime()
{
   //

   if (fStopped) {
      return;
   }

   fTime->Set();
   fTimeText->DrawLatex(3., 420, fTime->AsSQLString());

   fCanvas->Modified();
   fCanvas->Update();
}

//______________________________________________________________________________
void init(Bool_t filter = kTRUE)
{

   if (fCanvas) {   // already exists
      delete fCanvas;
      fCanvas = 0;
   }

   fCanvas = new TCanvas("fCanvas", "Pulse", -600, 300);
   //fCanvas->DrawFrame(0, 0, 100, 600);
   fCanvas->SetFillColor(17);
   fCanvas->SetFrameFillColor(13);
   fCanvas->SetGrid();
   fCanvas->SetBorderMode(0);
   //fCanvas->ToggleEventStatus();

   if (!fHist) {
      fHist = new TH1F("ECG", "Pulse", kNMAX, 0, kNMAX);
      fHist->SetMaximum(400);
      fHist->SetMinimum(-200);
      fHist->GetYaxis()->SetNdivisions(10);
      fHist->GetXaxis()->SetNdivisions(16);
      fHist->GetXaxis()->SetLimits(0, kNMAX*kStep);
      TColor(200, 0, 1, 1); //204./255, 204./255);
      fHist->SetLineColor(200);
      fHist->SetLineWidth(2);
      fHist->SetFillColor(0);
      fHist->SetStats(kFALSE);
      fHist->GetXaxis()->SetTitle("sec");
      fHist->Draw();
      gStyle->SetTitleFontSize(0.07);
      gStyle->SetTitleX(0.2);
   }

   if (!fRaw) {
      fRaw = new TH1F("Raw", "Pulse", kNMAX, 0, kNMAX);
      fRaw->SetMaximum(800);
      fRaw->SetMinimum(-300);
      fRaw->GetYaxis()->SetNdivisions(10);
      fRaw->GetXaxis()->SetNdivisions(8);
      fRaw->GetXaxis()->SetLimits(0, kNMAX*kStep);
      fRaw->SetLineColor(4);
      fRaw->SetFillColor(0);
      fRaw->SetStats(kFALSE);
      fRaw->GetXaxis()->SetTitle("sec");
      fRaw->Draw();
   }

   if (filter) {
      fHist->Draw();
   } else {
      fRaw->Draw();
   }

   if (fTimeText) {
      delete fTimeText;
      fTimeText = 0;
   }

   if (!fTimeText) {
      fTimeText = new TLatex(3., 420, fTime->AsSQLString());
      fTimeText->SetLineWidth(2);
      fTimeText->SetTextColor(4);
      fTimeText->Draw();
   }

   //if (!fTimer) {
   //   fTimer = new TTimer(1000);
   //   fTimer->Connect("Timeout()", 0, 0, "UpdateTime()");
   //   fTimer->Start(1000);
   //}
}


//______________________________________________________________________________
void PulseControl()
{
   //

   Bool_t connected = kFALSE;
   int events = 0;

   init();

   TServerSocket *ss = new TServerSocket(30044, kTRUE);

   printf("Server Socket created. Wating for accept\n");

   TSocket *s0 = ss->Accept();

   printf("Socket created and accepted\n");

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

         printf("Create socket\n");

         if (!s0) {
            connected = kFALSE;
            delete ss;
            ss = 0;
            gSystem->Sleep(2000); // sleep 2 sec
            continue;
         } else {
            connected = kTRUE;
            printf("Connected\n");
         }
      }

      n = s0->RecvRaw(buf, kN*2);
      events++;

      if (n <= 0) {
         connected = kFALSE;
         delete ss;
         ss = 0;
         printf("disconnected\n");

         gSystem->ProcessEvents();
         continue;
      }

      for (int i = 0; i < kN; i += kNs) {
         FindPeaks(buf + i);
      }

      fCanvas->Print("pulse.gif+60");
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

   init();

   int result;
   int n = 0;

   while (!feof(fp)) {
      result = fread(buf, 2, kN, fp);

      FindPeaks(buf);

      fTime->Set();
      fTimeText->DrawLatex(3., 420, fTime->AsSQLString());

      fCanvas->Modified();
      fCanvas->Update();

      //fCanvas->Print("pulse.gif+60");

      gSystem->ProcessEvents();
      gSystem->Sleep(200);
      n++;
   }
   fclose(fp);
   printf("n = %d\n", n);

   // make infinite animation by adding "++" to the file name
   //fCanvas->Print("pulse.gif++");
}