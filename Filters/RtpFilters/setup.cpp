// $Id: setup.cpp 1235 2012-10-02 05:46:21Z onuchin $
// Author: Valeriy Onuchin   28.11.2010

#include <streams.h>
#include <initguid.h>
#include "RtpRenderer.h"
#include "RtpSource.h"

//______________________________________________________________________________
CFactoryTemplate g_Templates[2] =
{
    {
        L"Rtp Renderer Filter",
        &CLSID_RtpRenderer,
        CRtpRenderer::CreateInstance,
        NULL,
        NULL
    },

    {
        L"Rtp Source Filter",
        &CLSID_RtpSource,
        CRtpSource::CreateInstance,
        NULL,
        NULL
    }
};

int g_cTemplates = sizeof(g_Templates)/sizeof(g_Templates[0]);

////////////////////////////////////////////////////////////////////////
//
// Exported entry points for registration and unregistration
// (in this case they only call through to default implementations).
//
////////////////////////////////////////////////////////////////////////
STDAPI DllRegisterServer()
{
    return AMovieDllRegisterServer2(TRUE);
}

STDAPI DllUnregisterServer()
{
    return AMovieDllRegisterServer2(FALSE);
}