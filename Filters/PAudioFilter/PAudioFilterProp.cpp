#include <windows.h>
#include <windowsx.h>
#include <streams.h>
#include <commctrl.h>
#include <olectl.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>
#include <tchar.h>
#include "resource.h"
#include "iPAudioFilter.h"
#include "PAudioFilter.h"
#include "PAudioFilterProp.h"

//
// CreateInstance
//
// Used by the DirectShow base classes to create instances
//
CUnknown *PAudioFilterProperties::CreateInstance(LPUNKNOWN lpunk, HRESULT *phr)
{
    CUnknown *punk = new PAudioFilterProperties(lpunk, phr);
    if (punk == NULL) {
   *phr = E_OUTOFMEMORY;
    }
    return punk;

}

//
// Constructor
//
PAudioFilterProperties::PAudioFilterProperties(LPUNKNOWN pUnk, HRESULT *phr) :
    CBasePropertyPage(NAME("PAudioFilter Property Page"),
                      pUnk, IDD_PAudioFilterProp,IDS_TITLE),
    m_pIPAudioFilter(NULL),
    m_bIsInitialized(FALSE)
{
    ASSERT(phr);
}

//
// OnReceiveMessage
//
// Handles the messages for our property window
//
BOOL PAudioFilterProperties::OnReceiveMessage(HWND hwnd,
                                          UINT uMsg,
                                          WPARAM wParam,
                                          LPARAM lParam)
{
    switch (uMsg)
    {
        case WM_COMMAND:
        {
            if (m_bIsInitialized)
            {
                m_bDirty = TRUE;
                if (m_pPageSite)
                {
                    m_pPageSite->OnStatusChange(PROPPAGESTATUS_DIRTY);
                }
            }
            return (LRESULT) 1;
        }

    }
    return CBasePropertyPage::OnReceiveMessage(hwnd,uMsg,wParam,lParam);

}

//
// OnConnect
//
// Called when we connect to a transform filter
//
HRESULT PAudioFilterProperties::OnConnect(IUnknown *pUnknown)
{
    ASSERT(m_pIPAudioFilter == NULL);

    HRESULT hr = pUnknown->QueryInterface(IID_IPAudioFilter, (void **) &m_pIPAudioFilter);
    if (FAILED(hr)) {
        return E_NOINTERFACE;
    }

    ASSERT(m_pIPAudioFilter);

    m_pIPAudioFilter->get_PAudioFilter(&m_PAudioFilterParameters);
    m_bIsInitialized = FALSE ;
    return NOERROR;
}

//
// OnDisconnect
//
// Likewise called when we disconnect from a filter
//
HRESULT PAudioFilterProperties::OnDisconnect()
{
    if (m_pIPAudioFilter == NULL) {
        return E_UNEXPECTED;
    }

    m_pIPAudioFilter->Release();
    m_pIPAudioFilter = NULL;
    return NOERROR;
}

//
// OnActivate
//
// We are being activated
//
HRESULT PAudioFilterProperties::OnActivate()
{
    TCHAR   sz[60];

    _stprintf(sz, TEXT("%d"), m_PAudioFilterParameters.param1);
    Edit_SetText(GetDlgItem(m_Dlg, IDC_PARAM1), sz);
    _stprintf(sz, TEXT("%d"), m_PAudioFilterParameters.param2);
    Edit_SetText(GetDlgItem(m_Dlg, IDC_PARAM2), sz);

   m_bIsInitialized = TRUE;

   return NOERROR;
}

//
// OnDeactivate
//
// We are being deactivated
//
HRESULT PAudioFilterProperties::OnDeactivate(void)
{
    ASSERT(m_pIPAudioFilter);
    m_bIsInitialized = FALSE;
    GetControlValues();
    return NOERROR;
}

//
// OnApplyChanges
//
// Apply any changes so far made
//
HRESULT PAudioFilterProperties::OnApplyChanges()
{
    GetControlValues();
    m_pIPAudioFilter->put_PAudioFilter(m_PAudioFilterParameters);

    return NOERROR;
}

//
// GetControlValues
//
void PAudioFilterProperties::GetControlValues()
{
    TCHAR sz[STR_MAX_LENGTH];

    Edit_GetText(GetDlgItem(m_Dlg, IDC_PARAM1), sz, STR_MAX_LENGTH);
   m_PAudioFilterParameters.param1 = _ttoi(sz);

    Edit_GetText(GetDlgItem(m_Dlg, IDC_PARAM2), sz, STR_MAX_LENGTH);
   m_PAudioFilterParameters.param2 = _ttoi(sz);
}
