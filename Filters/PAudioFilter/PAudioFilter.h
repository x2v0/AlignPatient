#pragma once

class PAudioFilter : public CTransformFilter,
       public IPAudioFilter,
       public ISpecifyPropertyPages,
       public CPersistStream
{

public:

    DECLARE_IUNKNOWN;
    static CUnknown * WINAPI CreateInstance(LPUNKNOWN punk, HRESULT *phr);

    // Reveals IPAudioFilter and ISpecifyPropertyPages
    STDMETHODIMP NonDelegatingQueryInterface(REFIID riid, void ** ppv);

    // CPersistStream stuff
    HRESULT ScribbleToStream(IStream *pStream);
    HRESULT ReadFromStream(IStream *pStream);
    STDMETHODIMP GetClassID(CLSID *pClsid);

    // Overrriden from CTransformFilter base class
    HRESULT Transform(IMediaSample *pIn, IMediaSample *pOut);
    HRESULT CheckInputType(const CMediaType *mtIn);
    HRESULT CheckTransform(const CMediaType *mtIn, const CMediaType *mtOut);
    HRESULT DecideBufferSize(IMemAllocator *pAlloc, ALLOCATOR_PROPERTIES *pProperties);
    HRESULT GetMediaType(int iPosition, CMediaType *pMediaType);

    // These implement the custom IPAudioFilter interface
    STDMETHODIMP get_PAudioFilter(PAudioFilterParameters *irp);
    STDMETHODIMP put_PAudioFilter(PAudioFilterParameters irp);

    // ISpecifyPropertyPages interface
    STDMETHODIMP GetPages(CAUUID *pPages);

private:

    // Constructor
    PAudioFilter(TCHAR *tszName, LPUNKNOWN punk, HRESULT *phr);
   ~PAudioFilter();

    BOOL CanPerformTransform(const CMediaType *pMediaType) const;

   CCritSec m_PAudioFilterLock;         // Private play critical section
   PAudioFilterParameters m_PAudioFilterParameters;
   CMediaType m_mt;

   HRESULT Copy(IMediaSample *pSource, IMediaSample *pDest) const;

};

