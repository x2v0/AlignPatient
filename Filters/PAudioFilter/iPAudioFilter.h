#pragma once

// these GUID's need to change if you clone this filter
// 12EC712A-46F6-4A3D-A934-EFB6354906A5
DEFINE_GUID(CLSID_PAudioFilter, 
0x12ec712a, 0x46f6, 0x4a3d, 0xa9, 0x34, 0xef, 0xb6, 0x35, 0x49, 0x6, 0xa5);

// D9DFFDC6-4636-4E47-AB2C-5C1800790E0E
DEFINE_GUID(CLSID_PAudioFilterPropertyPage, 
0xd9dffdc6, 0x4636, 0x4e47, 0xab, 0x2c, 0x5c, 0x18, 0x0, 0x79, 0xe, 0xe);

// custom transform parameters exposed to property page
struct PAudioFilterParameters {
   // TODO: insert your own transform parameters here
   int param1;
   int param2;
};

#ifdef __cplusplus
extern "C" {
#endif

// our custom interface
// 4932165B-C95D-4DE4-BF7B-93BE2EC60B83
DEFINE_GUID(IID_IPAudioFilter, 
0x4932165b, 0xc95d, 0x4de4, 0xbf, 0x7b, 0x93, 0xbe, 0x2e, 0xc6, 0xb, 0x83);

DECLARE_INTERFACE_(IPAudioFilter, IUnknown)
{
    STDMETHOD(get_PAudioFilter) (THIS_
              PAudioFilterParameters *irp
             ) PURE;

    STDMETHOD(put_PAudioFilter) (THIS_
              PAudioFilterParameters irp
             ) PURE;
};

#ifdef __cplusplus
}
#endif

