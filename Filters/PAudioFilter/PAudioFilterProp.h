#pragma once

class PAudioFilterProperties : public CBasePropertyPage
{

public:

    static CUnknown * WINAPI CreateInstance(LPUNKNOWN lpunk, HRESULT *phr);

private:

    BOOL OnReceiveMessage(HWND hwnd,UINT uMsg,WPARAM wParam,LPARAM lParam);
    HRESULT OnConnect(IUnknown *pUnknown);
    HRESULT OnDisconnect();
    HRESULT OnActivate();
    HRESULT OnDeactivate();
    HRESULT OnApplyChanges();

    void    GetControlValues();

    PAudioFilterProperties(LPUNKNOWN lpunk, HRESULT *phr);

    BOOL m_bIsInitialized;                   // Used to ignore startup messages
    IPAudioFilter *m_pIPAudioFilter;      // The custom interface on the filter
   PAudioFilterParameters m_PAudioFilterParameters;
};

