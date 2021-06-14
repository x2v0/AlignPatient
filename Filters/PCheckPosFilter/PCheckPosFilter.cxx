// $Id: PCheckPosFilter.cxx 2033 2014-01-15 11:00:01Z onuchin $
// Author: Valeriy Onuchin   06.07.2010

/*************************************************************************
 *                                                                       *
 * Copyright (C) 2010,   Valeriy Onuchin                                 *
 * All rights reserved.                                                  *
 *                                                                       *
 *************************************************************************/

//////////////////////////////////////////////////////////////////////////
//                                                                      //
//  PCheckPosFilter is transform filter intended to control a position  //
//      of a patient. It displays the difference between the            //
//      video frames and the reference image by using different         //
//      comparing algorithms:                                           //
//         - constant threshold on luminosity difference                // 
//         - floating level threshold                                   //
//  Filter allows flip video frames vertical and horizontal by using    //
//  SetEffect method with VideoControlFlags from IAMVideoControl        //
//                                                                      //
// TODO:                                                                //
//       1. calibration of exposition (hist balancing)                  //
//                                                                      //
//                                                                      //
//////////////////////////////////////////////////////////////////////////


#include <windows.h>
#include <streams.h>
#include <initguid.h>
#include <Strsafe.h>

#if (1100 > _MSC_VER)
#include <olectlid.h>
#else
#include <olectl.h>
#endif

#include "vcclr.h"          // for gcroot
using namespace System;
using namespace System::Runtime::InteropServices;
using namespace System::Drawing;

#include "PCheckPosProperties.h"
#include "PCheckPosFilter.h"
#include "resource.h"


//#define LOCAL_DEBUG 1 // auto snapshot after short period of time

////////////////////////////////////////////////////////////////////////////////
//       singleton used to export some functions to C#
static ICheckPosFilter *gCheckPosFilter = 0; 


////////////////////// filter registration tables //////////////////////////////
//______________________________________________________________________________
const AMOVIESETUP_MEDIATYPE sudPinTypes =
{
   &MEDIATYPE_Video,       // Major type
   &MEDIASUBTYPE_RGB24     // Minor type
};

//______________________________________________________________________________
const AMOVIESETUP_PIN sudpPins[] =
{
   {
   L"Input",             // Pins string name
   FALSE,                // Is it rendered
   FALSE,                // Is it an output
   FALSE,                // Are we allowed none
   FALSE,                // And allowed many
   &CLSID_NULL,          // Connects to filter
   NULL,                 // Connects to pin
   1,                    // Number of types
   &sudPinTypes          // Pin information
   },

   {
   L"Output",            // Pins string name
   FALSE,                // Is it rendered
   TRUE,                 // Is it an output
   FALSE,                // Are we allowed none
   FALSE,                // And allowed many
   &CLSID_NULL,          // Connects to filter
   NULL,                 // Connects to pin
   1,                    // Number of types
   &sudPinTypes          // Pin information
   }
};

//______________________________________________________________________________
const AMOVIESETUP_FILTER sudCheckPosFilter =
{
    &CLSID_PCheckPosFilter,         // Filter CLSID
    L"Check Position Fiter",        // String name
    MERIT_DO_NOT_USE,               // Filter merit
    2,                              // Number of pins
    sudpPins                        // Pin information
};

// --- COM factory table and registration code --------------
// List of class IDs and creator functions for the class factory. This
// provides the link between the OLE entry point in the DLL and an object
// being created. The class factory will call the static CreateInstance
//______________________________________________________________________________
CFactoryTemplate g_Templates[] = {
   { L"Check Position Fiter"
   , &CLSID_PCheckPosFilter
   , PCheckPosFilter::CreateInstance
   , NULL
   , &sudCheckPosFilter },

   { L"Check Position Fiter Properties"
   , &CLSID_CheckPosFilterProperties
   , PCheckPosProperties::CreateInstance }
};

int g_cTemplates = sizeof(g_Templates) / sizeof(g_Templates[0]);


///////////////////////////////// helper ///////////////////////////////////////
//______________________________________________________________________________
void ErrMsg(LPTSTR szFormat, ...)
{
   // Popup message box displaying formatted text 

   static TCHAR szBuffer[2048]={0};
   const size_t NUMCHARS = sizeof(szBuffer) / sizeof(szBuffer[0]);
   const int LASTCHAR = NUMCHARS - 1;

   // Format the input string
   va_list pArgs;
   va_start(pArgs, szFormat);

   // Use a bounded buffer size to prevent buffer overruns.  Limit count to
   // character size minus one to allow for a NULL terminating character.
   HRESULT hr = StringCchVPrintf(szBuffer, NUMCHARS - 1, szFormat, pArgs);
   va_end(pArgs);

   // Ensure that the formatted string is NULL-terminated
   szBuffer[LASTCHAR] = TEXT('\0');

   MessageBox(0, szBuffer, NULL, MB_OK | MB_ICONEXCLAMATION | MB_TASKMODAL);
}

/*
* palying with preloading
*
static int loadFilter()
{
   FILE *fp = fopen("d:\\qq.txt", "w");
   if (fp) {
      fputs("Check Position Filter loaded", fp);
      fclose(fp);
   }
   return 1;
}

static int qq = loadFilter();
*/

////////////////////////////////////////////////////////////////////////
//
// Exported entry points for registration and unregistration 
// (in this case they only call through to default implementations).
//
////////////////////////////////////////////////////////////////////////
//______________________________________________________________________________
STDAPI DllRegisterServer()
{
   // self-registration entrypoint

   return AMovieDllRegisterServer2(TRUE);
}

//______________________________________________________________________________
STDAPI DllUnregisterServer()
{
   // unregister

   return AMovieDllRegisterServer2(FALSE);
}

//______________________________________________________________________________
STDAPI MakeSnapshot()
{
   // make snapshot

#ifdef LOCAL_DEBUG
   if (LOCAL_DEBUG > 0) {
      ErrMsg(L"Debugging:Make Snapshot 0x%x", gCheckPosFilter);
   }
#endif
   if (gCheckPosFilter) {
      return gCheckPosFilter->Snapshot(0);
   }

   return E_FAIL;
}

//______________________________________________________________________________
STDAPI Flip(int on)
{
   // vertical flip

#ifdef LOCAL_DEBUG
   if (LOCAL_DEBUG > 0) {
      ErrMsg(L"Debugging:Vertical Flip 0x%x", gCheckPosFilter);
   }
#endif
   if (gCheckPosFilter) {
      gCheckPosFilter->Flip(on);
      return NOERROR;
   }

   return E_FAIL;
}

//______________________________________________________________________________
STDAPI Mirrow(int on)
{
   // horizontal flip

#ifdef LOCAL_DEBUG
   if (LOCAL_DEBUG > 0) {
      ErrMsg(L"Debugging:Horizontal Flip 0x%x", gCheckPosFilter);
   }
#endif
   if (gCheckPosFilter) {
      gCheckPosFilter->Mirrow(on);
      return NOERROR;
   }

   return E_FAIL;
}

//______________________________________________________________________________
STDAPI SetEffect(int e)
{
   // static exported C wrapper function to set effect 

#ifdef LOCAL_DEBUG
   if (LOCAL_DEBUG > 0) {
      ErrMsg(L"Debugging:SetEffect 0x%x", gCheckPosFilter);
   }
#endif
   if (gCheckPosFilter) {
      return gCheckPosFilter->SetEffect(e);
   }

   return E_FAIL;
}

//______________________________________________________________________________
STDAPI SetThreshold(int t)
{
   // static exported C wrapper function to set threshold

#ifdef LOCAL_DEBUG
   if (LOCAL_DEBUG > 0) {
      ErrMsg(L"Debugging:SetThreshold 0x%x", gCheckPosFilter);
   }
#endif
   if (gCheckPosFilter) {
      return gCheckPosFilter->SetThreshold(t);
   }

   return E_FAIL;
}

//______________________________________________________________________________
STDAPI SetRefImage(long sz, BYTE *img)
{
   // static exported C wrapper function to set reference image

#ifdef LOCAL_DEBUG
   if (LOCAL_DEBUG > 0) {
      ErrMsg(L"Debugging:SetRefImage 0x%x 0x%x %l", gCheckPosFilter, img, sz);
   }
#endif
   if (gCheckPosFilter) {
      return gCheckPosFilter->SetRefImage(sz, img);
   }

   return E_FAIL;
}

//______________________________________________________________________________
STDAPI Reset()
{
   // static exported C wrapper function to reset reference image

#ifdef LOCAL_DEBUG
   if (LOCAL_DEBUG > 0) {
      ErrMsg(L"Debugging:Reset 0x%x", gCheckPosFilter);
   }
#endif
   if (gCheckPosFilter) {
      return gCheckPosFilter->Reset();
   }

   return E_FAIL;
}

//______________________________________________________________________________
STDAPI GetEffect(int *val)
{
   // static exported C wrapper function which returns the current effect

#ifdef LOCAL_DEBUG
   if (LOCAL_DEBUG > 0) {
      ErrMsg(L"Debugging:GetEffect 0x%x", gCheckPosFilter);
   }
#endif
   if (gCheckPosFilter) {
      return gCheckPosFilter->GetEffect(val);
   }

   return E_FAIL;
}

//______________________________________________________________________________
STDAPI GetWidth(int *val)
{
   // static exported C wrapper function which returns the image width

#ifdef LOCAL_DEBUG
   if (LOCAL_DEBUG > 0) {
      ErrMsg(L"Debugging:GetWidth 0x%x", gCheckPosFilter);
   }
#endif
   if (gCheckPosFilter) {
      return gCheckPosFilter->GetWidth(val);
   }

   return E_FAIL;
}

//______________________________________________________________________________
STDAPI GetHeight(int *val)
{
   // static exported C wrapper function which returns the image height

#ifdef LOCAL_DEBUG
   if (LOCAL_DEBUG > 0) {
      ErrMsg(L"Debugging:GetHeight 0x%x", gCheckPosFilter);
   }
#endif
   if (gCheckPosFilter) {
      return gCheckPosFilter->GetHeight(val);
   }

   return E_FAIL;
}

//______________________________________________________________________________
STDAPI GetThreshold(int *val)
{
   // static exported C wrapper function which returns the current threshold

#ifdef LOCAL_DEBUG
   if (LOCAL_DEBUG > 0) {
      ErrMsg(L"Debugging:GetThreshold 0x%x", gCheckPosFilter);
   }
#endif
   if (gCheckPosFilter) {
      return gCheckPosFilter->GetThreshold(val);
   }

   return E_FAIL;
}

//______________________________________________________________________________
STDAPI GetImageSize(int *val)
{
   // static exported C wrapper function which returns the image size

#ifdef LOCAL_DEBUG
   if (LOCAL_DEBUG > 0) {
      ErrMsg(L"Debugging:GetImageSize 0x%x", gCheckPosFilter);
   }
#endif
   if (gCheckPosFilter) {
      return gCheckPosFilter->GetImageSize(val);
   }

   return E_FAIL;
}

//______________________________________________________________________________
STDAPI GetRefImage(BYTE **img)
{
   // static exported C wrapper function which returns the reference image

#ifdef LOCAL_DEBUG
   if (LOCAL_DEBUG > 0) {
      ErrMsg(L"Debugging:GetRefImage 0x%x", gCheckPosFilter);
   }
#endif
   if (gCheckPosFilter) {
      return gCheckPosFilter->GetRefImage(img);
   }

   return E_FAIL;
}

//______________________________________________________________________________
STDAPI GetCurImage(BYTE **img)
{
   // static exported C wrapper function which returns the current image

#ifdef LOCAL_DEBUG
   if (LOCAL_DEBUG > 0) {
      ErrMsg(L"Debugging:GetCurImage 0x%x", gCheckPosFilter);
   }
#endif
   if (gCheckPosFilter) {
      return gCheckPosFilter->GetCurImage(img);
   }

   return E_FAIL;
}

//______________________________________________________________________________
STDAPI ChangeGlobal(ICheckPosFilter *newGlobal, ICheckPosFilter **oldGlobal)
{
   // 

#ifdef LOCAL_DEBUG
   if (LOCAL_DEBUG > 0) {
      ErrMsg(L"Debugging:Changing global 0x%x", gCheckPosFilter);
   }
#endif

   *oldGlobal = gCheckPosFilter;
   gCheckPosFilter = newGlobal;

   return NOERROR;
}


//////////////////////////////////////////////////////////////////////
//_____________________________________________________________________________
PCheckPosFilter::PCheckPosFilter(TCHAR *tszName, LPUNKNOWN punk, HRESULT *phr) :
                 CTransformFilter(tszName, punk, CLSID_PCheckPosFilter),
                 CPersistStream(punk, phr)
{
   // Constructor

   fCheckTime  = 0;
   fCurImage   = 0;              // the current vide frame being processed 
   fRefImage   = 0;              // reference image
   fLastImage  = 0;              // the last/previous frame processed
   fNframes    = 0;              // number of frames processed
   fEffect     = IDC_RADIO_None; // which effect are we processing
   fThreshold  = 50;             // threshold for checkup (0 ... 255)
   fWidth      = -1;             // width of image
   fHeight     = -1;             // height of image
   fVideoControlFlags  = 0;      // !!!!must be fixed ==video frames are mirrowed == VideoControlFlag_FlipHorizontal
   fRefImageFile = 0;            // 
   fCropX        = 0;            // start X position of cropping area. 
   fCropY        = 0;            // start Y position of cropping area.
   fCropW        = 0;            // cropping area width. If fCropW < 0, no cropping is done only two lines are drawn
   fXtop         = 0;            // X top coordinate for predefined crop area. fCropW must be < 0
   fYtop         = 0;            // Y top coordinate for predefined crop area. fCropW must be < 0
   fXbot         = 0;            // X bottom coordinate for predefined crop area. fCropW must be < 0
   fYbot         = 0;            // Y bottom coordinate for predefined crop area. fCropW must be < 0
   fMinX         = 0;            // minimum x of "sensitive area"
   fMaxX         = 0;            //
   fCrop         = 0;            //

   if (!gCheckPosFilter) {
      gCheckPosFilter = this;
   }
}

//______________________________________________________________________________
PCheckPosFilter::~PCheckPosFilter()
{
   // destructor

   CAutoLock lock(&fCheckPosLock);

   if (fCurImage) {
      delete [] fCurImage;
   }
   fCurImage = 0;

   if (fRefImage) {
      delete [] fRefImage;
   }
   fRefImage = 0;

   if (fLastImage) {
      delete [] fLastImage;
   }
   fLastImage = 0;

   fWidth  = -1; 
   fHeight = -1;
   fEffect = IDC_RADIO_None;
   fThreshold = -1;
   fRefImageFile = 0;
   fCropX        = 0;
   fCropY        = 0;
   fCropW        = 0;
   fXtop         = 0;
   fYtop         = 0;
   fXbot         = 0;
   fYbot         = 0;
   fMinX         = 0;
   fMaxX         = 0;
   fCrop         = 0;
}

//______________________________________________________________________________
CUnknown *PCheckPosFilter::CreateInstance(LPUNKNOWN punk, HRESULT *phr)
{
   // Provide the way for COM to create a PCheckPosFilter object

   ASSERT(phr);
    
   PCheckPosFilter *pNewObject = new PCheckPosFilter(NAME("Check Position Filter"),
                                                     punk, phr);

   if (pNewObject == NULL) {
      if (phr) {
         *phr = E_OUTOFMEMORY;
      }
   }

   return pNewObject;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::NonDelegatingQueryInterface(REFIID riid, void **ppv)
{
   // Reveals ICheckPosFilter and ISpecifyPropertyPages

   CheckPointer(ppv, E_POINTER);

   if (riid == IID_ICheckPosFilter) {
      return GetInterface((ICheckPosFilter *)this, ppv);

   } else if (riid == IID_ISpecifyPropertyPages) {
      return GetInterface((ISpecifyPropertyPages *)this, ppv);

   } else {
      return CTransformFilter::NonDelegatingQueryInterface(riid, ppv);
   }
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::Transform(IMediaSample *pIn, IMediaSample *pOut)
{
   // Copy the input sample into the output sample - then transform the output
   // sample 'in place'.

   CheckPointer(pIn, E_POINTER);   
   CheckPointer(pOut, E_POINTER);   

   // Copy the properties across
   HRESULT hr = Copy(pIn, pOut);

   if (FAILED(hr)) {
      return hr;
   }

   return Transform(pOut);
}

//______________________________________________________________________________
void PCheckPosFilter::CropAndStretch(BYTE **pSourceBuffer, BYTE **pDestBuffer,
                                     int x, int y, int w, int h)
{
   // Crop and Stretch video frame with C# helper classes

   int stride = fVideoControlFlags & VideoControlFlag_FlipVertical ? -3*fWidth : 3*fWidth;
   System::IntPtr scan0 = fVideoControlFlags & VideoControlFlag_FlipVertical ?
                          (System::IntPtr)((int)*pSourceBuffer - stride*(fHeight - 1)) :
                          (System::IntPtr)*pSourceBuffer;

   // create bitmap from source buffer (cropped)
   System::Drawing::Imaging::PixelFormat format = System::Drawing::Imaging::PixelFormat::Format24bppRgb;
   System::Drawing::Bitmap^ bmp = gcnew Bitmap(fWidth, fHeight, stride, format, scan0);

   System::Drawing::Rectangle cropArea(x, y, w, h);
   System::Drawing::Bitmap^ bmpCrop = bmp->Clone(cropArea, format);

   // mirrow bitmap
   if (fVideoControlFlags & VideoControlFlag_FlipHorizontal) {
      bmpCrop->RotateFlip(System::Drawing::RotateFlipType::RotateNoneFlipX);
   }

   System::Drawing::Graphics^ g = System::Drawing::Graphics::FromImage(bmp);
   //g->InterpolationMode = System::Drawing::Drawing2D::InterpolationMode::HighQualityBicubic;
   g->DrawImage(bmpCrop, 0, 0, fWidth, fHeight);
   delete g;

   System::Drawing::Imaging::ImageLockMode mode = System::Drawing::Imaging::ImageLockMode::ReadWrite;
   System::Drawing::Rectangle rect(0, 0, fWidth, fHeight);
   System::Drawing::Imaging::BitmapData^ bmd = bmp->LockBits(rect, mode, format);

   memcpy(*pDestBuffer, (BYTE*)(void*)bmd->Scan0, 3*fWidth*fHeight);
   bmp->UnlockBits(bmd);

   delete bmp;
   delete bmpCrop;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::Copy(IMediaSample *pSource, IMediaSample *pDest)
{
   // Make destination an identical copy of source

   CheckPointer(pSource, E_POINTER);   
   CheckPointer(pDest, E_POINTER);   

   // Copy the sample data
   BYTE *pSourceBuffer, *pDestBuffer;
   long lSourceSize = pSource->GetActualDataLength();

#ifdef LOCAL_DEBUG
    long lDestSize = pDest->GetSize();
    ASSERT(lDestSize >= lSourceSize);
#endif

   pSource->GetPointer(&pSourceBuffer);
   pDest->GetPointer(&pDestBuffer);

   AM_MEDIA_TYPE *pType = &m_pInput->CurrentMediaType();
   VIDEOINFOHEADER *pvi = (VIDEOINFOHEADER *)pType->pbFormat;
   ASSERT(pvi);

   // Get the image properties from the BITMAPINFOHEADER
   int w = fWidth  = pvi->bmiHeader.biWidth;
   int h = fHeight = pvi->bmiHeader.biHeight;

   RGBTRIPLE *srgb = (RGBTRIPLE*)pSourceBuffer;
   RGBTRIPLE *drgb = (RGBTRIPLE*)pDestBuffer;
   long pos = 0;

   float hw = float(fHeight)/fWidth;
   float hh = fCropW*hw;
   hh = fCropY + hh > fHeight ? fHeight - fCropY : hh;
   fCropW = fCropX + fCropW > fWidth ? fWidth - fCropX : fCropW;

   if (fCropW > 10) {   // crop & stretch image
      CropAndStretch(&pSourceBuffer, &pDestBuffer, fCropX, fCropY, fCropW, int(hh));

   }  else if (fVideoControlFlags & VideoControlFlag_FlipHorizontal) { //mirrow
      if (fVideoControlFlags & VideoControlFlag_FlipVertical) { // vertical
         for (long y = h-1; y >= 0; y--) {
            pos = (y+1)*w;
            for (long x = 0; x < w; x++) {
               pos--;
               drgb->rgbtRed   = srgb[pos].rgbtRed;
               drgb->rgbtGreen = srgb[pos].rgbtGreen;
               drgb->rgbtBlue  = srgb[pos].rgbtBlue;
               drgb++;
            }
         }
      } else { // mirrow & not vertical
         for (long y = 0; y < h; y++) {
            pos = (y+1)*w;
            for (long x = 0; x < w; x++) {
               pos--;
               drgb->rgbtRed   = srgb[pos].rgbtRed;
               drgb->rgbtGreen = srgb[pos].rgbtGreen;
               drgb->rgbtBlue  = srgb[pos].rgbtBlue;
               drgb++;
            }
         }
      }
   } else  if (fVideoControlFlags & VideoControlFlag_FlipVertical) { // vertical & not mirrow

      for (long y = h-1; y >= 0; y--) {
         pos = y*w;
         for (long x = 0; x < w; x++) {
            drgb->rgbtRed   = srgb[pos].rgbtRed;
            drgb->rgbtGreen = srgb[pos].rgbtGreen;
            drgb->rgbtBlue  = srgb[pos].rgbtBlue;
            pos++;
            drgb++;
         }
      }
   } else { // not vertical && not mirrow
      memcpy(pDestBuffer, pSourceBuffer, lSourceSize);
   }

   // Copy the sample times
   REFERENCE_TIME timeStart, timeEnd;

   if (NOERROR == pSource->GetTime(&timeStart, &timeEnd)) {
      pDest->SetTime(&timeStart, &timeEnd);
   }

   LONGLONG mediaStart, mediaEnd;

   if (pSource->GetMediaTime(&mediaStart, &mediaEnd) == NOERROR) {
      pDest->SetMediaTime(&mediaStart, &mediaEnd);
   }

   // Copy the Sync point property
   HRESULT hr = pSource->IsSyncPoint();

   if (hr == S_OK) {
      pDest->SetSyncPoint(TRUE);
   } else if (hr == S_FALSE) {
      pDest->SetSyncPoint(FALSE);
   } else {  // an unexpected error has occured...
      return E_UNEXPECTED;
   }

   // Copy the media type
   AM_MEDIA_TYPE *pMediaType;

   pSource->GetMediaType(&pMediaType);
   pDest->SetMediaType(pMediaType);
   DeleteMediaType(pMediaType);

   // Copy the preroll property
   hr = pSource->IsPreroll();

   if (hr == S_OK) {
      pDest->SetPreroll(TRUE);
   } else if (hr == S_FALSE) {
      pDest->SetPreroll(FALSE);
   } else {  // an unexpected error has occured...
      return E_UNEXPECTED;
   }

   // Copy the discontinuity property
   hr = pSource->IsDiscontinuity();

   if (hr == S_OK) {
      pDest->SetDiscontinuity(TRUE);
   } else if (hr == S_FALSE) {
      pDest->SetDiscontinuity(FALSE);
   } else {  // an unexpected error has occured...
      return E_UNEXPECTED;
   }

   // Copy the actual data length
   long lDataLength = pSource->GetActualDataLength();
   pDest->SetActualDataLength(lDataLength);

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::Transform(IMediaSample *pMediaSample)
{
   // Transform (in place)

   CAutoLock lock(&fCheckPosLock);

   BYTE *pData;                // Pointer to the actual image buffer
   long lDataLen;              // Holds length of any given sample
   HRESULT hr = NOERROR;

   AM_MEDIA_TYPE *pType = &m_pInput->CurrentMediaType();
   VIDEOINFOHEADER *pvi = (VIDEOINFOHEADER *)pType->pbFormat;
   ASSERT(pvi);

   CheckPointer(pMediaSample, E_POINTER);
   pMediaSample->GetPointer(&pData);
   lDataLen = pMediaSample->GetSize();

   // Get the image properties from the BITMAPINFOHEADER
   int cxImage    = pvi->bmiHeader.biWidth;
   int cyImage    = pvi->bmiHeader.biHeight;

   int numPixels  = cxImage*cyImage;

   if (fWidth < 0) {
      fWidth = cxImage;
   }

   fMaxX = fMaxX == 0 ? fWidth : fMaxX;

   if (fHeight < 0) {
      fHeight = cyImage;
   }

   // assuming that video frames has a fixed size (might be "improved" later)
   ASSERT(fWidth == cxImage);
   ASSERT(fHeight == cyImage);

   if (!fLastImage && (fEffect == IDC_RADIO_Diff)) {
      fLastImage = new BYTE[lDataLen];
   }

   if (!fCurImage) {
      fCurImage = new BYTE[lDataLen];
   } else {
      if (fLastImage && (fEffect == IDC_RADIO_Diff)) { //avoid extra blitting
         memcpy(fLastImage, fCurImage, lDataLen);
      }
   }

   memcpy(fCurImage, pData, lDataLen);

   switch (fEffect) {
      case IDC_RADIO_None:
         //fVideoControlFlags = 0;
         break;
      case IDC_RADIO_Grey:
         hr = Grey(pData, numPixels);
         break;
      case IDC_RADIO_Diff:
         hr = Diff(pData, numPixels);
         break;
      case IDC_RADIO_Blend:
         hr = Blend(pData, numPixels);
         break;
      case IDC_RADIO_Mirr:
         fVideoControlFlags |= VideoControlFlag_FlipHorizontal;
         break;
      case IDC_RADIO_Flip:
         fVideoControlFlags |= VideoControlFlag_FlipVertical;
         break;
      case IDC_RADIO_Snap:
      default:
         hr = Checkup(pData, numPixels);
         break;
   }

   if (fCrop != 0) {
      hr = DrawCropArea(pData, numPixels);
   }

   fNframes++;

#ifdef LOCAL_DEBUG
   if (LOCAL_DEBUG == 2) {
      CRefTime tStart, tStop ;
      hr = pMediaSample->GetTime((REFERENCE_TIME *)&tStart, (REFERENCE_TIME *)&tStop);

      static bool done = false;
 
      if (!done && (tStart > 20000000)) { // after 1 sec
         Snapshot();
         done = true;
      }
   }
#endif

   return hr;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::CheckInputType(const CMediaType *mtIn)
{
   // Check the input type is OK - return an error otherwise

   CheckPointer(mtIn, E_POINTER);

   // check this is a VIDEOINFOHEADER type
   if (*mtIn->FormatType() != FORMAT_VideoInfo) {
      return E_INVALIDARG;
   }

   // Can we transform this type
   if (CanPerform(mtIn)) {
      return NOERROR;
   }

   return E_FAIL;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::CheckTransform(const CMediaType *mtIn,
                                        const CMediaType *mtOut)
{
   // Check a transform can be done between these formats

   CheckPointer(mtIn, E_POINTER);
   CheckPointer(mtOut, E_POINTER);

   if (CanPerform(mtIn)) {
      if (*mtIn == *mtOut) {
         return NOERROR;
      }
   }

   return E_FAIL;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::DecideBufferSize(IMemAllocator *pAlloc,
                                          ALLOCATOR_PROPERTIES *pProperties)
{
   // Tell the output pin's allocator what size buffers we
   // require. Can only do this when the input is connected.

   // Is the input pin connected
   if (m_pInput->IsConnected() == FALSE) {
      return E_UNEXPECTED;
   }

   CheckPointer(pAlloc, E_POINTER);
   CheckPointer(pProperties, E_POINTER);

   HRESULT hr = NOERROR;

   pProperties->cBuffers = 1;
   pProperties->cbBuffer = m_pInput->CurrentMediaType().GetSampleSize();

   ASSERT(pProperties->cbBuffer);

   // Ask the allocator to reserve us some sample memory, NOTE the function
   // can succeed (that is return NOERROR) but still not have allocated the
   // memory that we requested, so we must check we got whatever we wanted

   ALLOCATOR_PROPERTIES actual;

   hr = pAlloc->SetProperties(pProperties, &actual);

   if (FAILED(hr)) {
      return hr;
   }

   ASSERT(actual.cBuffers == 1);

   if (pProperties->cBuffers > actual.cBuffers ||
       pProperties->cbBuffer > actual.cbBuffer) {
      return E_FAIL;
   }

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::GetMediaType(int iPosition, CMediaType *pMediaType)
{
   // I support one type, namely the type of the input pin
   // This type is only available if my input is connected

   // Is the input pin connected
   if (m_pInput->IsConnected() == FALSE) {
      return E_UNEXPECTED;
   }

   // This should never happen
   if (iPosition < 0) {
      return E_INVALIDARG;
   }

   // Do we have more items to offer
   if (iPosition > 0) {
      return VFW_S_NO_MORE_ITEMS;
   }

   CheckPointer(pMediaType, E_POINTER);

   *pMediaType = m_pInput->CurrentMediaType();

   return NOERROR;
}

//______________________________________________________________________________
BOOL PCheckPosFilter::CanPerform(const CMediaType *pMediaType) const
{
   // Check if this is a RGB24 true color format && equal width/height

   CheckPointer(pMediaType, FALSE);

   if (IsEqualGUID(*pMediaType->Type(), MEDIATYPE_Video)) {
      if (IsEqualGUID(*pMediaType->Subtype(), MEDIASUBTYPE_RGB24)) {
         VIDEOINFOHEADER *pvi = (VIDEOINFOHEADER *)pMediaType->Format();
         return (pvi->bmiHeader.biBitCount == 24);
      }
   }

   return FALSE;
} 

//______________________________________________________________________________
HRESULT PCheckPosFilter::GetClassID(CLSID *pClsid)
{
   // The method of IPersist 

   return CBaseFilter::GetClassID(pClsid);
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::GetPages(CAUUID *pPages)
{
   // Returns the clsid's of the property pages we support

   CheckPointer(pPages,E_POINTER);

   pPages->cElems = 1;
   pPages->pElems = (GUID *) CoTaskMemAlloc(sizeof(GUID));

   if (pPages->pElems == NULL) {
      return E_OUTOFMEMORY;
   }

   *(pPages->pElems) = CLSID_CheckPosFilterProperties;

   return NOERROR;
}

/////////////////////// CPersistStream macros //////////////////////////////////
#define WRITEOUT(var)  hr = pStream->Write(&var, sizeof(var), 0); \
               if (FAILED(hr)) return hr;

#define READIN(var)    hr = pStream->Read(&var, sizeof(var), 0); \
               if (FAILED(hr)) return hr;


//______________________________________________________________________________
HRESULT PCheckPosFilter::ScribbleToStream(IStream *pStream)
{
   // Overriden to write our state into a stream  aka .GRF file

   HRESULT hr;

   WRITEOUT(fEffect);
   WRITEOUT(fThreshold);
   WRITEOUT(fNframes);
   WRITEOUT(fCheckTime);
   WRITEOUT(fWidth);
   WRITEOUT(fHeight);

   int sz = fWidth*fHeight*3;

   // write image or NULL pointer
   if (fRefImage && (sz > 0))  {
      hr = pStream->Write(fRefImage, sz, 0);
      if (FAILED(hr)) {
         return hr;
      }
   }

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::ReadFromStream(IStream *pStream)
{
   // Likewise overriden to restore our state from a stream aka .GRF file

   HRESULT hr;

   // it's volatile
   fCurImage = 0;

   READIN(fEffect);
   READIN(fThreshold);
   READIN(fNframes);
   READIN(fCheckTime);
   READIN(fWidth);
   READIN(fHeight);

   int sz = fWidth*fHeight*3;

   if (sz < 0) {  //
      fRefImage = 0;
      return NOERROR;
   }

   // read reference image
   if (!fRefImage) { // allocate an array for fRefImage
      fRefImage = new BYTE[sz];
   }

   hr = pStream->Read(fRefImage, sz, 0);

   if (FAILED(hr)) { // RefImage was not written
      delete [] fRefImage;
      fRefImage = 0;
   }

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::SetEffect(int effect)
{
   // effect setter

   CAutoLock lock(&fCheckPosLock);

   if ((effect < 1000) || (effect >= IDC_RADIO_Last)) {
      return E_INVALIDARG;
   }

   fEffect = effect;
   SetDirty(TRUE);

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::GetEffect(int *val)
{
   // effect getter 

   CAutoLock lock(&fCheckPosLock);

   CheckPointer(val, E_POINTER);

   *val = fEffect;
   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::GetThreshold(int *val)
{
   // treshold getter

   CAutoLock lock(&fCheckPosLock);

   CheckPointer(val, E_POINTER);

   *val = fThreshold;
   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::SetThreshold(int thresh)
{
   // threshold setter

   CAutoLock lock(&fCheckPosLock);

   if ((thresh < 0) || (thresh > 255)) {
      return E_INVALIDARG;
   }

   fThreshold = thresh;
   SetDirty(TRUE);

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::SetRefImage(long sz, BYTE *img)
{
   // sets a new reference image

   CAutoLock lock(&fCheckPosLock);

   CheckPointer(img, E_POINTER);
 
   RGBTRIPLE *prgb = (RGBTRIPLE*)fRefImage; 
   RGBTRIPLE *irgb = (RGBTRIPLE*)img;
   BYTE grey;
   long numPixels = sz/3;

   if (!fRefImage) {
      fRefImage = new BYTE[sz];
   } else {
      if (fWidth*fHeight != numPixels) {
         return E_INVALIDARG;
      }
   }

   for (long iPixel = 0; iPixel < numPixels ; iPixel++, prgb++, irgb++) {
       grey = (57*irgb->rgbtRed + 181*irgb->rgbtGreen + 18*irgb->rgbtBlue)>>8;
       prgb->rgbtRed = prgb->rgbtGreen = prgb->rgbtBlue = grey;
   }

   CRefTime now;
   fCheckTime = now;
   fEffect = IDC_RADIO_Snap;

   SetDirty(TRUE);

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::GetCurImage(BYTE **img)
{
   // copy the current image data to input img array
   // img - must be allocated with size equals fWidth*fHeight

   CAutoLock lock(&fCheckPosLock);

   if (!fCurImage || (fWidth < 0) || (fHeight < 0)) {
      return E_FAIL;
   }

   CheckPointer(*img, E_POINTER);

   memcpy(*img, fCurImage, fWidth*fHeight);
   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::GetRefImage(BYTE **img)
{
   // img - must be allocated with size equals fWidth*fHeight

   CAutoLock lock(&fCheckPosLock);

   if (!fRefImage || (fWidth < 0) || (fHeight < 0)) {
      return E_FAIL;
   }

   CheckPointer(*img, E_POINTER);

   memcpy(*img, fRefImage, fWidth*fHeight);
   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::Snapshot(wchar_t *filename)
{
   // make a snapshot, i.e. make the current frame to be reference one

   CAutoLock lock(&fCheckPosLock);

   if (!fCurImage || (fWidth < 0) || (fHeight < 0)) {
      return E_FAIL;
   }

   long numPixels = fWidth*fHeight;
   long sz = numPixels*3;
   
   if (fRefImage) {
      delete fRefImage;
   }
   fRefImage = new BYTE[sz];

   RGBTRIPLE *prgb = (RGBTRIPLE*)fRefImage; 
   RGBTRIPLE *irgb = (RGBTRIPLE*)fCurImage;  
   BYTE grey;

   for (long iPixel = 0; iPixel < numPixels ; iPixel++, prgb++, irgb++) {
       grey = (57*irgb->rgbtRed + 181*irgb->rgbtGreen + 18*irgb->rgbtBlue) >> 8;
       prgb->rgbtRed = prgb->rgbtGreen = prgb->rgbtBlue = grey;
   }

   CRefTime now;
   fCheckTime = now;
   fEffect = IDC_RADIO_Snap;
   SetDirty(TRUE);

   System::String^ str = gcnew String(filename);

   if ((str->Trim()->Length != 0) && !System::String::IsNullOrEmpty(str)) {
      int stride = -3*fWidth;
      System::Drawing::Imaging::PixelFormat format = 
                                 System::Drawing::Imaging::PixelFormat::Format24bppRgb;

      System::IntPtr scan0 = (System::IntPtr)((int)fCurImage - stride*(fHeight - 1));
      System::Drawing::Bitmap^ bmp = gcnew Bitmap(fWidth, fHeight, stride, format, scan0);
      bmp->Save(str);
      delete bmp;
      fRefImageFile = filename;
   }

   delete str;

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::SetRefImageFile(wchar_t *filename)
{
   // creates a reference image from file specified

   CAutoLock lock(&fCheckPosLock);

   System::String^ str = gcnew String(filename);

   if (!System::IO::File::Exists(str)) {
      delete str;
      return E_FAIL;
   }

   System::Drawing::Bitmap^ image = 
            (System::Drawing::Bitmap^)System::Drawing::Bitmap::FromFile(str);

   delete str;

   if (!image) {
      return E_FAIL;
   }

   image->RotateFlip(System::Drawing::RotateFlipType::Rotate180FlipX);

   int sz = 3*fWidth*fHeight;

   if (!fRefImage) {
     fRefImage = new BYTE[sz];
   }

   System::Drawing::Imaging::PixelFormat format = 
                           System::Drawing::Imaging::PixelFormat::Format24bppRgb;

   System::Drawing::Imaging::ImageLockMode mode = 
                              System::Drawing::Imaging::ImageLockMode::ReadWrite;

   System::Drawing::Rectangle rect(0, 0, fWidth, fHeight);
   System::Drawing::Imaging::BitmapData^ bmd = image->LockBits(rect, mode, format);

   memcpy(fRefImage, (BYTE*)(void*)bmd->Scan0, sz);
   image->UnlockBits(bmd);

   delete image;

   CRefTime now;
   fCheckTime = now;
   fEffect = IDC_RADIO_Snap;
   SetDirty(TRUE);

   return NOERROR;   
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::Reset()
{
   // delete reference image

   CAutoLock lock(&fCheckPosLock);

   if (fRefImage) {
     delete [] fRefImage;
   }

   fRefImage = 0;

   //fVideoControlFlags = 0;
   fCropX = 0;
   fCropY = 0; 
   fCropW = 0;
   fXtop  = 0;
   fYtop  = 0;
   fXbot  = 0;
   fYbot  = 0;
   fMinX  = 0;
   fMaxX  = 0;
   fCrop  = 0;

   SetDirty(TRUE);

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::GetWidth(int *val)
{
   // width getter

   CAutoLock lock(&fCheckPosLock);

   CheckPointer(val, E_POINTER);

   *val = fWidth;
   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::GetHeight(int *val)
{
   // height getter

   CAutoLock lock(&fCheckPosLock);

   CheckPointer(val, E_POINTER);

   *val = fHeight;
   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::GetImageSize(int *val)
{
   // returns red level - percentage of red pixels

   CAutoLock lock(&fCheckPosLock);

   CheckPointer(val, E_POINTER);

   if (!fCurImage || (fWidth <= 0) || (fHeight <= 0)) {
      *val = 0;
      return NOERROR;
   }

   int redLevel = 0;

   RGBTRIPLE *prgb = (RGBTRIPLE*)fCurImage;

   long numPixels = fWidth*fHeight;

   for (long iPixel = 0; iPixel < numPixels ; iPixel++, prgb++) {
      if (prgb->rgbtRed == 255 &&
          prgb->rgbtGreen == 0 &&
          prgb->rgbtBlue  == 0) {
         redLevel++;
      }
   }

   *val = int(redLevel/10.0);
 
   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::DrawCropArea(void *buffer, long numPixels)
{
   // Draw crop area - two gray lines.
   // 
   // buffer    - video frame buffer 
   // numPixels - number of pixels

   if (fCropW > 0) {
      return NOERROR;
   }

   CheckPointer(buffer, E_POINTER);

   RGBTRIPLE *prgb = (RGBTRIPLE*)buffer;   // Holds a pointer to the current pixel
   BYTE grey;
   int y;
   int x;

   for (long iPixel = 0; iPixel < numPixels; iPixel++, prgb++) {
      y = iPixel/fWidth;
      x = iPixel - y*fWidth;

      grey = (57*prgb->rgbtRed + 181*prgb->rgbtGreen + 18*prgb->rgbtBlue) >> 8;

      if (y < fYtop - 2) {          
         prgb->rgbtRed = prgb->rgbtGreen = prgb->rgbtBlue = grey*2/3;
      } else if (y >= fYtop - 2 && y < fYtop) {
         prgb->rgbtRed = 255;
         prgb->rgbtGreen = 255;
         prgb->rgbtBlue = 0; 
      } else if (y >= fYbot + 2) {
         prgb->rgbtRed = prgb->rgbtGreen = prgb->rgbtBlue = grey*2/3;
      } else if (y < fYbot + 2 && y >= fYbot) {
         prgb->rgbtRed = 0;
         prgb->rgbtGreen = 0;
         prgb->rgbtBlue = 255; 
      } else if (x < fMinX - 2) {
         prgb->rgbtRed = prgb->rgbtGreen = prgb->rgbtBlue = grey*2/3;
      } else if (x >= fMaxX + 2) {
         prgb->rgbtRed = prgb->rgbtGreen = prgb->rgbtBlue = grey*2/3;
      } else if (x >= fMaxX - 2 && x < fMaxX) {
         prgb->rgbtRed = 0;
         prgb->rgbtGreen = 255;
         prgb->rgbtBlue = 0; 
      } else if (x < fMinX + 2 && x >= fMinX) {
         prgb->rgbtRed = 0;
         prgb->rgbtGreen = 255;
         prgb->rgbtBlue = 0; 
      }
   }

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::Grey(void *buffer, long numPixels)
{
   // greyish video. 
   // buffer    - video frame buffer 
   // numPixels - number of pixels

   CheckPointer(buffer, E_POINTER);

   RGBTRIPLE *prgb = (RGBTRIPLE*)buffer;   // Holds a pointer to the current pixel
   BYTE grey;

   for (long iPixel = 0; iPixel < numPixels; iPixel++, prgb++) {
       grey = (57*prgb->rgbtRed + 181*prgb->rgbtGreen + 18*prgb->rgbtBlue)>>8;
       prgb->rgbtRed = prgb->rgbtGreen = prgb->rgbtBlue = grey;
   }

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::Checkup(void *buffer, long numPixels)
{
   // create difference current frame and reference image
   // buffer    - video frame buffer 
   // numPixels - number of pixels

   CheckPointer(fRefImage, NOERROR);   // return OK if reference image not defined
   CheckPointer(buffer, E_POINTER);

   BYTE grey = 0;
   BYTE diff = 0;
   RGBTRIPLE *prgb = (RGBTRIPLE*)buffer; 
   RGBTRIPLE *ref  = (RGBTRIPLE*)fRefImage;

   int y;
   int x;

   for (long iPixel = 0; iPixel < numPixels ; iPixel++, prgb++, ref++) {
      grey = (57*prgb->rgbtRed + 181*prgb->rgbtGreen + 18*prgb->rgbtBlue)>>8;
      diff = abs(grey - ref->rgbtRed);  // reference image is already grey
      y = iPixel/fWidth;
      x = iPixel - y*fWidth;

      if (diff > fThreshold && x > fMinX && x <= fMaxX) {   // redish
         prgb->rgbtRed   = 255;
         prgb->rgbtGreen = 0;
         prgb->rgbtBlue  = 0;
      }
   }

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::Blend(void *buffer, long numPixels)
{
   // create alfa blended image base on the current frame and reference image
   // buffer    - video frame buffer 
   // numPixels - number of pixels

   CheckPointer(fRefImage, NOERROR);   // return OK if reference image not defined
   CheckPointer(buffer, E_POINTER);

   BYTE grey = 0;
   BYTE diff = 0;
   RGBTRIPLE *prgb = (RGBTRIPLE*)buffer; 
   RGBTRIPLE *ref  = (RGBTRIPLE*)fRefImage;
   int a = 160;
   int aa = 255 - a;

   for (long iPixel = 0; iPixel < numPixels ; iPixel++, prgb++, ref++) {
      prgb->rgbtRed   = (prgb->rgbtRed*aa + ref->rgbtRed*a)>>8;
      prgb->rgbtGreen = (prgb->rgbtGreen*aa + ref->rgbtGreen*a)>>8;
      prgb->rgbtBlue  = (prgb->rgbtBlue*aa + ref->rgbtBlue*a)>>8;
   }

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::Diff(void *buffer, long numPixels)
{
   // create difference current frame and previous image
   // buffer    - video frame buffer 
   // numPixels - number of pixels

   CheckPointer(fLastImage, E_POINTER);   // return OK if reference image not defined
   CheckPointer(buffer, E_POINTER);

   BYTE grey  = 0;
   BYTE diff  = 0;
   BYTE grey2 = 0;
   RGBTRIPLE *prgb = (RGBTRIPLE*)buffer; 
   RGBTRIPLE *last  = (RGBTRIPLE*)fLastImage;

   for (long iPixel = 0; iPixel < numPixels ; iPixel++, prgb++, last++) {
      grey2 = (57*last->rgbtRed + 181*last->rgbtGreen + 18*last->rgbtBlue)>>8;
      grey = (57*prgb->rgbtRed + 181*prgb->rgbtGreen + 18*prgb->rgbtBlue)>>8;
      diff = abs(grey - grey2);  // absolute difference between  successive frames

      if (diff > fThreshold) {   // redish
         prgb->rgbtRed   = 255;
         prgb->rgbtGreen = 0;
         prgb->rgbtBlue  = 0;
      }
   }

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::Flip(int on)
{
   // vertical flip-flop video frames. 

   CAutoLock lock(&fCheckPosLock);

   if (on) {
      fVideoControlFlags |= VideoControlFlag_FlipVertical;
   } else {
      fVideoControlFlags &= ~VideoControlFlag_FlipVertical;
   }

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::Mirrow(int on)
{
   // Horizontal flip-flop video frames. 
 
   CAutoLock lock(&fCheckPosLock);

   if (on) {
      fVideoControlFlags |= VideoControlFlag_FlipHorizontal;
   } else {
      fVideoControlFlags &= ~VideoControlFlag_FlipHorizontal;
   }

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::SetCropArea(int x, int y, int w)
{
   // defines crop area of video frame and then is stretched to original size 

   CAutoLock lock(&fCheckPosLock);

   if (w < 0) {   // to draw two Y lines: x - lower y-coordinate, y - upper y-coordinate
      fCrop = w;

      if (w == -1) {          // top line
         fXtop = x;
         fYtop = y;
      } else if (w == -2) {   // bottom line
         fXbot = x;
         fYbot = y;
      } else if (w == -3) {   // left line
         fMinX = x;
      } else if (w == -4) {   // right line
         fMaxX = x;
      }

      return NOERROR;
   }

   x = x < 0 ? 0 : x;
   y = y < 0 ? 0 : y;
   w = w > fWidth ? fWidth : w;
   y = y > fHeight - 10 ? fHeight - 10 : y;
   x = x > fWidth - 10 ? fWidth - 10 : x;
   w = x + w > fWidth ? fWidth - x : w;

   fCropX = x;
   fCropY = y;
   fCropW = w;

   //ErrMsg(L"ok x = %d y = %d w = %d", fCropX, fCropY, fCropW);

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosFilter::GetCropArea(int *x, int *y, int *w)
{
   // returns scale factor 

   CAutoLock lock(&fCheckPosLock);

   CheckPointer(x, E_POINTER);
   CheckPointer(y, E_POINTER);
   CheckPointer(w, E_POINTER);

   *x = fCropX;
   *y = fCropY;
   *w = fCropW;

   return NOERROR;
}