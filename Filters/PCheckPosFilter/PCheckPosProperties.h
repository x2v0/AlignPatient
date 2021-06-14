// $Id: PCheckPosProperties.h 71 2010-12-24 14:20:32Z onuchin $
// Author: Valeriy Onuchin   06.07.2010

/*************************************************************************
 *                                                                       *
 * Copyright (C) 2010,   Valeriy Onuchin                            *
 * All rights reserved.                                                  *
 *                                                                       *
 *************************************************************************/


//////////////////////////////////////////////////////////////////////////
//                                                                      //
//  PCheckPosProperties - property pages of PCheckPosFilter             //
//                                                                      //
//////////////////////////////////////////////////////////////////////////


#ifndef PROTOM_PCheckPosProperties
#define PROTOM_PCheckPosProperties


#pragma warning(disable:4244) // conversion from
#pragma warning(disable:4702) // unreachable code

#include "PCheckPosFilter_h.h"   // ICheckPosFilter

// {FEA8370F-FB9F-48fd-BF49-0D511A0D361B}
DEFINE_GUID(CLSID_CheckPosFilterProperties, 
0xfea8370f, 0xfb9f, 0x48fd, 0xbf, 0x49, 0xd, 0x51, 0x1a, 0xd, 0x36, 0x1b);

//////////////////////////////////////////////////////////////////////
class PCheckPosProperties : public CBasePropertyPage {

public:
   static CUnknown *WINAPI CreateInstance(LPUNKNOWN lpunk, HRESULT *phr);

private:
   INT_PTR OnReceiveMessage(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
   HRESULT OnConnect(IUnknown *pUnknown);
   HRESULT OnDisconnect();
   HRESULT OnActivate();
   HRESULT OnDeactivate();
   HRESULT OnApplyChanges();

   PCheckPosProperties(LPUNKNOWN lpunk, HRESULT *phr);

   BOOL              fIsInitialized;      // Used to ignore startup messages
   ICheckPosFilter  *fInterface;          // The custom interface on the filter
}; 

#endif // PROTOM_PCheckPosProperties

