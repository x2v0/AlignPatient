// $Id: RtpRenderer.cpp 1799 2013-02-20 04:42:32Z onuchin $
// Author: Valeriy Onuchin   28.11.2010

#include <streams.h>        // DShow base classes
#include "RtpRenderer.h"

//#define LOCAL_DEBUG

//______________________________________________________________________________
CRtpRenderer::CRtpRenderer(TCHAR *tszName, LPUNKNOWN punk, HRESULT *phr)
        :   CBaseRenderer (CLSID_RtpRenderer, tszName, punk, phr),
            m_cbHeader(0)
{

#if LOCAL_DEBUG
   eventLog = gcnew EventLog("Filters", ".", "PRtpRenderer");
#endif

   // Items that get written to network
   // 0 - length of header (moot since they are all fixed size, kept to be backward compatible with 3.0)
   // 1 - SampleProperties
   // 2 - MediaType
   // 3 - FormatType
   // 4 - Sample data
   int ptrCount = 5;

   ptrs = gcnew array<IntPtr>(ptrCount);
   ptrLengths = gcnew array<Int32>(ptrCount);

   ptrs[0] = IntPtr(&m_cbHeader);
   ptrLengths[0] = sizeof(m_cbHeader);

   ptrs[1] = IntPtr(&m_SampleProps);
   ptrLengths[1] = sizeof(m_SampleProps);

   ptrs[2] = IntPtr(&m_mt);
   ptrLengths[2] = sizeof(m_mt);
}

//______________________________________________________________________________
CRtpRenderer::~CRtpRenderer()
{
   // dtor
}

//______________________________________________________________________________
STDMETHODIMP CRtpRenderer::Initialize(IUnknown *pRtpSender)
{
   //

   CAutoLock cAutolock(&m_cSharedState);

   try {
      rtpSender = dynamic_cast<RtpSender^>(Marshal::GetObjectForIUnknown(IntPtr(pRtpSender)));
   } catch (Exception^ e) {
#if LOCAL_DEBUG
      eventLog->WriteEntry(e->ToString(), EventLogEntryType::Error);
#endif
      return E_FAIL;
   }

   return NOERROR;
}

//______________________________________________________________________________
HRESULT CRtpRenderer::CheckMediaType(const CMediaType* pmt)
{
    return NOERROR;
}

//______________________________________________________________________________
HRESULT CRtpRenderer::SetMediaType(const CMediaType* pmt)
{
   //

   CAutoLock cAutolock(&m_cSharedState);

   m_mt = *pmt;

   ptrs[3] = IntPtr(m_mt.pbFormat);
   ptrLengths[3] = m_mt.cbFormat;

   m_cbHeader = IPAddress::HostToNetworkOrder((Int16)(ptrLengths[1] + ptrLengths[2] + ptrLengths[3]));

   return NOERROR;
}

//______________________________________________________________________________
HRESULT CRtpRenderer::DoRenderSample(IMediaSample *pMediaSample) {

    ValidateReadPtr(pMediaSample, sizeof(IMediaSample));

    // Retrieve the properties of this sample, to send across the wire
    IMediaSample2 *pMediaSample2 = 0;
    HRESULT hr = pMediaSample->QueryInterface(IID_IMediaSample2, (void **)&pMediaSample2);

   if (FAILED(hr)) {
#if LOCAL_DEBUG
      eventLog->WriteEntry("Failed to QI for IMediaSample2 in DoRenderSample", EventLogEntryType::Error);
#endif
      return hr;
   }

   hr = pMediaSample2->GetProperties(sizeof(m_SampleProps), (byte*)&m_SampleProps);
   pMediaSample2->Release();

   if (FAILED(hr)) {
#if LOCAL_DEBUG
      eventLog->WriteEntry("Failed to get properties in DoRenderSample", EventLogEntryType::Error);
#endif
      return hr;
   }
    
   // Add media sample data
   byte* pbPayload = 0;
   hr = pMediaSample->GetPointer(&pbPayload);

   if (FAILED(hr)) {
#if LOCAL_DEBUG
      eventLog->WriteEntry("Failed to get pointer from IMediaSample in DoRendersample", EventLogEntryType::Error);
#endif
      return hr;
   }

   ptrs[4] = IntPtr(pbPayload);
   ptrLengths[4] = pMediaSample->GetActualDataLength();

   try {
     // We don't need to prepend the lengths, because each item is a fixed length except for
     // the payload, which comes last.
     rtpSender->Send(ptrs, ptrLengths, false);
   } catch (Exception^ e) {
#if LOCAL_DEBUG
     eventLog->WriteEntry(e->ToString(), EventLogEntryType::Error);
#endif
     return E_FAIL;
   }

   return hr;
}

//______________________________________________________________________________
CUnknown *WINAPI CRtpRenderer::CreateInstance(LPUNKNOWN punk, HRESULT *phr) 
{
   //
 
   return new CRtpRenderer(NAME("RtpRenderer"), punk, phr );
}

//______________________________________________________________________________
STDMETHODIMP CRtpRenderer::NonDelegatingQueryInterface(REFIID riid, void **ppv)
{
   CheckPointer(ppv,E_POINTER);

   if (riid == IID_IRtpRenderer)  {
     return GetInterface((IRtpRenderer *) this, ppv);
   } else {
     return CBaseRenderer::NonDelegatingQueryInterface(riid, ppv);
   }
}