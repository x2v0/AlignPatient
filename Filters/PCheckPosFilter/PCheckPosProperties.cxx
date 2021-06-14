// $Id: PCheckPosProperties.cxx 1558 2012-12-25 12:44:45Z onuchin $
// Author: Valeriy Onuchin   09.07.2010

/*************************************************************************
 *                                                                       *
 * Copyright (C) 2010,   Valeriy Onuchin                                 *
 * All rights reserved.                                                  *
 *                                                                       *
 *************************************************************************/

//////////////////////////////////////////////////////////////////////////
//                                                                      //
//  PCheckPosProperties - property pages of PCheckPosFilter             //
//                                                                      //
//////////////////////////////////////////////////////////////////////////


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
#include "PCheckPosFilter.h"
#include "PCheckPosProperties.h"


#define BUFFER_SIZE 256
static char szBuffer[BUFFER_SIZE];

/////////////////////////////// helper macros /////////////////////////////////////////////////////
#define GET_IDC_SLIDER(control)\
if (HWND(lParam)==GetDlgItem(m_Dlg,IDC_SLIDER__##control)) {\
SetDirty();\
DWORD pos=SendDlgItemMessage(m_Dlg,IDC_SLIDER__##control,TBM_GETPOS,0,0);\
SendDlgItemMessage(m_Dlg,IDC_SPIN__##control,UDM_SETPOS32,0,pos);\
break;\
}\


#define SET_IDC_RADIO(control)\
if (HWND(lParam) == GetDlgItem(m_Dlg, IDC_RADIO_##control)) {\
fInterface->SetEffect(IDC_RADIO_##control);\
break;\
}\

////////////////////////////////////////////////////////////////////////////////
//______________________________________________________________________________
CUnknown *PCheckPosProperties::CreateInstance(LPUNKNOWN lpunk, HRESULT *phr)
{
   // Used by the DirectShow base classes to create instances

   ASSERT(phr);

   CUnknown *punk = new PCheckPosProperties(lpunk, phr);

   if (punk == NULL) {
      if (phr) {
         *phr = E_OUTOFMEMORY;
      }
   }

   return punk;
}

//______________________________________________________________________________
PCheckPosProperties::PCheckPosProperties(LPUNKNOWN pUnk, HRESULT *phr) :
    CBasePropertyPage(NAME("Check Position Filter Property Page"), pUnk,
                      IDD_CheckPosFilterProp, IDS_TITLE)
{
   // Constructor

   ASSERT(phr);

   fInterface = 0;
   fIsInitialized = FALSE;
}

//______________________________________________________________________________
INT_PTR PCheckPosProperties::OnReceiveMessage(HWND hwnd,
                                              UINT uMsg,
                                              WPARAM wParam,
                                              LPARAM lParam)
{
   // Handles the messages for our property window

   if (!fInterface) {
      return CBasePropertyPage::OnReceiveMessage(hwnd,uMsg,wParam,lParam);
   }

   HRESULT hr;

   switch (uMsg) {
      case WM_COMMAND:
      {
         if (fIsInitialized) {
            m_bDirty = TRUE;
            if (m_pPageSite) {
               m_pPageSite->OnStatusChange(PROPPAGESTATUS_DIRTY);
            }
         }
         switch (HIWORD(wParam)) {
            case BN_CLICKED:
            {
               if (HWND(lParam) == GetDlgItem(m_Dlg, IDC_BUTTON_Snapshot)) {
#ifdef LOCAL_DEPUG
   ErrMsg(L"Snapshot");
#endif
                  hr = fInterface->Snapshot(0);
                  if (hr != NOERROR) {
                     return (INT_PTR)FALSE;
                  }
                  break;
               }

               SET_IDC_RADIO(Grey);
               SET_IDC_RADIO(Flip);
               SET_IDC_RADIO(Diff);
               SET_IDC_RADIO(None);
            }

            break;
         }

         return (INT_PTR)TRUE;
      }
   }

   return CBasePropertyPage::OnReceiveMessage(hwnd, uMsg, wParam, lParam);
}

//______________________________________________________________________________
HRESULT PCheckPosProperties::OnConnect(IUnknown *pUnknown)
{
   // Called when we connect to a transform filter

   CheckPointer(pUnknown, E_POINTER);
   HRESULT hr = NOERROR;

   ASSERT(fInterface == NULL);

   hr = pUnknown->QueryInterface(IID_ICheckPosFilter, (void **)&fInterface);

   if (FAILED(hr)) {
      return E_NOINTERFACE;
   }

   CheckPointer(fInterface, E_FAIL);

   fIsInitialized = TRUE;
   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosProperties::OnDisconnect()
{
   //called when we disconnect from a filter

   if(fInterface) {
      fInterface->Release();
      fInterface = 0;
   }

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosProperties::OnActivate()
{
   // being activated


   HRESULT hr;
   int val; // default

   SendDlgItemMessage(m_Dlg, IDC_SLIDER_Threshold, TBM_SETRANGE, 1, MAKELONG(0,255));

   val = 50;
  if (fInterface) {
      hr = fInterface->GetThreshold(&val);
      if (hr != NOERROR) {
         val = 50;
      }
   }
   SendDlgItemMessage(m_Dlg, IDC_SLIDER_Threshold, TBM_SETPOS, 1, 50);
   SendDlgItemMessage(m_Dlg, IDC_SLIDER_Threshold, TBM_SETTICFREQ, 10, 0);

   SetDlgItemText(m_Dlg, IDC_EDIT_Threshold, L"50");
   SendMessage(GetDlgItem(m_Dlg, IDC_SPIN_Threshold), UDM_SETRANGE, 0, MAKELONG(255,0));

   val = IDC_RADIO_None; // default

   if (fInterface) {
      hr = fInterface->GetEffect(&val);
      if (hr != NOERROR) {
         val = IDC_RADIO_None;
      }
   }
   SendDlgItemMessage(m_Dlg, val, BM_SETCHECK, BST_CHECKED, 0);

   return NOERROR;
}

//______________________________________________________________________________
HRESULT PCheckPosProperties::OnDeactivate(void)
{
   // deactivate

   return NOERROR;
} 

//______________________________________________________________________________
HRESULT PCheckPosProperties::OnApplyChanges()
{
   // Apply any changes

   CheckPointer(fInterface, E_POINTER);

   return NOERROR;
}

