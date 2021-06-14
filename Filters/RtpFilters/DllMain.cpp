// $Id: DllMain.cpp 89 2010-12-28 12:24:34Z onuchin $
// Author: Valeriy Onuchin   28.11.2010

extern "C" int __stdcall DllEntryPoint(void *, unsigned, void *);

int __stdcall DllMain( void * dllHandle, unsigned reason, void * reserved)
{
    // VC has handled the native initialization, forward on to DirectShow
    return DllEntryPoint(dllHandle, reason, reserved);
}