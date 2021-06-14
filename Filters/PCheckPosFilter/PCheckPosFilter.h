// $Id: PCheckPosFilter.h 2033 2014-01-15 11:00:01Z onuchin $
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
//      of a patient.                                                   //
//                                                                      //
//////////////////////////////////////////////////////////////////////////


#ifndef PROTOM_PCheckPosFilter
#define PROTOM_PCheckPosFilter

#pragma warning(disable:4244) // conversion from
#pragma warning(disable:4702) // unreachable code
#pragma warning(disable:4793) // on ErrMsg


#include "PCheckPosFilter_i.c"   // IID_ICheckPosFilter, CLSID_PCheckPosFilter
#include "PCheckPosFilter_h.h"   // ICheckPosFilter


#using <mscorlib.dll>
using namespace System;

//////////////////////////////////////////////////////////////////////
class PCheckPosFilter: public CTransformFilter,
                       public ICheckPosFilter,
                       public ISpecifyPropertyPages,
                       public CPersistStream {
private:
   CCritSec    fCheckPosLock;          // Private play critical section
   BYTE       *fRefImage;              // reference image (greyish)
   BYTE       *fCurImage;              // a copy of the current video frame (r,g,b)[width*heigh]
   BYTE       *fLastImage;             // a copy of the last video frame (r,g,b)[width*heigh]
   long        fNframes;               // number of frames processed
   int         fEffect;                // which effect are we processing
   int         fThreshold;             // threshold for checkup (0 ... 255). default = 32
   CRefTime    fCheckTime;             // when checkup was done
   int         fWidth;                 // width of image (fixed)
   int         fHeight;                // height of image (fixed)
   int         fVideoControlFlags;     // VideoControlFlags the same as in IAMVideoControl (mirrowed is default)
   wchar_t    *fRefImageFile;          // the path to reference image file 
   int         fCropX;                 // start X position of cropping area.
   int         fCropY;                 // start Y position of cropping area.
   int         fCropW;                 // cropping area width
   int         fCrop;                  // if !=0 - draw crop area
   int         fXtop;                  // X top coordinate for predefined crop area. fCropW must be < 0
   int         fYtop;                  // Y top coordinate for predefined crop area. fCropW must be < 0
   int         fXbot;                  // X bottom coordinate for predefined crop area. fCropW must be < 0
   int         fYbot;                  // Y bottom coordinate for predefined crop area. fCropW must be < 0
   int         fMinX;                  // minimum x of "sensitive area"
   int         fMaxX;                  // maximum x of "sensitive area"

   virtual HRESULT Diff(void *buffer, long sz);
   virtual HRESULT Blend(void *buffer, long sz);
   virtual HRESULT Grey(void *buffer, long sz);
   virtual HRESULT Checkup(void *buffer, long sz);
   virtual HRESULT DrawCropArea(void *buffer, long sz);

   BOOL     CanPerform(const CMediaType *pMediaType) const;
   HRESULT  Copy(IMediaSample *pSource, IMediaSample *pDest);
   HRESULT  Transform(IMediaSample *pMediaSample);
   void     CropAndStretch(BYTE **pSourceBuffer, BYTE **pDestBuffer, int x, int y, int w, int h);

   // Constructor/Destructor
   PCheckPosFilter(TCHAR *tszName, LPUNKNOWN punk, HRESULT *phr);
   ~PCheckPosFilter();

public:
   DECLARE_IUNKNOWN;
   static CUnknown * WINAPI CreateInstance(LPUNKNOWN punk, HRESULT *phr);

   STDMETHODIMP NonDelegatingQueryInterface(REFIID riid, void **ppv);

   // CPersistStream stuff
   HRESULT ScribbleToStream(IStream *pStream);
   HRESULT ReadFromStream(IStream *pStream);

   // Overrriden from CTransformFilter base class
   HRESULT Transform(IMediaSample *pIn, IMediaSample *pOut);
   HRESULT CheckInputType(const CMediaType *mtIn);
   HRESULT CheckTransform(const CMediaType *mtIn, const CMediaType *mtOut);
   HRESULT DecideBufferSize(IMemAllocator *pAlloc, ALLOCATOR_PROPERTIES *pProperties);
   HRESULT GetMediaType(int iPosition, CMediaType *pMediaType);

   // ISpecifyPropertyPages interface
   STDMETHODIMP GetPages(CAUUID *pPages);

   // CPersistStream override
   STDMETHODIMP GetClassID(CLSID *pClsid);

   // Custom ICheckPosFilter interface
   virtual STDMETHODIMP    SetEffect(int effect);
   virtual STDMETHODIMP    SetThreshold(int thresh);
   virtual STDMETHODIMP    SetRefImage(long sz, BYTE *img);
   virtual STDMETHODIMP    Snapshot(wchar_t *filename = 0);
   virtual STDMETHODIMP    Reset();
   virtual STDMETHODIMP    GetEffect(int *val);
   virtual STDMETHODIMP    GetWidth(int *val);
   virtual STDMETHODIMP    GetHeight(int *val);
   virtual STDMETHODIMP    GetThreshold(int *val);
   virtual STDMETHODIMP    GetImageSize(int *val);
   virtual STDMETHODIMP    GetRefImage(BYTE **img);
   virtual STDMETHODIMP    GetCurImage(BYTE **img);
   virtual STDMETHODIMP    Flip(int on);
   virtual STDMETHODIMP    Mirrow(int on);
   virtual STDMETHODIMP    SetCropArea(int x, int y, int w);
   virtual STDMETHODIMP    GetCropArea(int *x, int *y, int *w);
   virtual STDMETHODIMP    SetRefImageFile(wchar_t *filename);
};

extern void ErrMsg(LPTSTR szFormat, ...);

#endif   // PROTOM_PCheckPosFilter
