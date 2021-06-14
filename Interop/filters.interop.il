
//  Microsoft (R) .NET Framework IL Disassembler.  Version 3.5.21022.8
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v2.0.50727
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 2:0:0:0
}
.assembly filters.interop
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.TypeLibVersionAttribute::.ctor(int32,
                                                                                                int32) = ( 01 00 01 00 00 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 34 35 38 64 39 35 63 61 2D 39 62 66 61   // ..$458d95ca-9bfa
                                                                                                  2D 34 61 63 37 2D 62 34 66 37 2D 33 36 31 34 64   // -4ac7-b4f7-3614d
                                                                                                  66 65 33 36 34 31 61 00 00 )                      // fe3641a..
  .custom instance void [mscorlib]System.Runtime.InteropServices.ImportedFromTypeLibAttribute::.ctor(string) = ( 01 00 0A 63 78 70 66 69 6C 74 65 72 73 00 00 )    // ...cxpfilters..
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module cxpfilters.dll
// MVID: {356BB4DA-C506-46BF-99C8-E01FB66D0231}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00FF0000


// =============== CLASS MEMBERS DECLARATION ===================

.class interface public abstract auto ansi import P.DShow.Filters.IRtpSource
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.TypeLibTypeAttribute::.ctor(int16) = ( 01 00 80 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 37 32 45 34 32 41 36 31 2D 36 41 41 31   // ..$72E42A61-6AA1
                                                                                                  2D 34 35 41 44 2D 42 41 32 31 2D 37 33 30 36 35   // -45AD-BA21-73065
                                                                                                  41 38 39 37 46 35 43 00 00 )                      // A897F5C..
  .method public hidebysig newslot abstract virtual 
          instance void  Initialize([in] object  marshal( iunknown ) pRtpStream) runtime managed internalcall
  {
  } // end of method IRtpSource::Initialize

} // end of class P.DShow.Filters.IRtpSource

.class public auto ansi import P.DShow.Filters.RtpSourceClass
       extends [mscorlib]System.Object
       implements P.DShow.Filters.IRtpSource,
                  P.DShow.Filters.RtpSource
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.TypeLibTypeAttribute::.ctor(int16) = ( 01 00 02 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 31 35 38 43 34 34 32 31 2D 39 34 35 46   // ..$158C4421-945F
                                                                                                  2D 34 38 32 36 2D 38 38 35 31 2D 32 34 35 39 44   // -4826-8851-2459D
                                                                                                  39 32 43 43 46 30 37 00 00 )                      // 92CCF07..
  .custom instance void [mscorlib]System.Runtime.InteropServices.ClassInterfaceAttribute::.ctor(int16) = ( 01 00 00 00 00 00 ) 
  .method public specialname rtspecialname 
          instance void  .ctor() runtime managed internalcall
  {
  } // end of method RtpSourceClass::.ctor

  .method public hidebysig newslot virtual 
          instance void  Initialize([in] object  marshal( iunknown ) pRtpStream) runtime managed internalcall
  {
    .override P.DShow.Filters.IRtpSource::Initialize
  } // end of method RtpSourceClass::Initialize

} // end of class P.DShow.Filters.RtpSourceClass

.class interface public abstract auto ansi import P.DShow.Filters.RtpSource
       implements P.DShow.Filters.IRtpSource
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.CoClassAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 25 4D 53 52 2E 4C 53 54 2E 4D 44 53 68 6F   // ..%MSR.LST.MDSho
                                                                                                                          77 2E 46 69 6C 74 65 72 73 2E 52 74 70 53 6F 75   // w.Filters.RtpSou
                                                                                                                          72 63 65 43 6C 61 73 73 00 00 )                   // rceClass..
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 37 32 45 34 32 41 36 31 2D 36 41 41 31   // ..$72E42A61-6AA1
                                                                                                  2D 34 35 41 44 2D 42 41 32 31 2D 37 33 30 36 35   // -45AD-BA21-73065
                                                                                                  41 38 39 37 46 35 43 00 00 )                      // A897F5C..
} // end of class P.DShow.Filters.RtpSource

.class interface public abstract auto ansi import P.DShow.Filters.IRtpRenderer
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 35 41 33 44 43 30 42 38 2D 36 33 44 35   // ..$5A3DC0B8-63D5
                                                                                                  2D 34 34 30 42 2D 38 33 34 44 2D 30 37 35 43 34   // -440B-834D-075C4
                                                                                                  42 43 35 45 37 42 37 00 00 )                      // BC5E7B7..
  .custom instance void [mscorlib]System.Runtime.InteropServices.TypeLibTypeAttribute::.ctor(int16) = ( 01 00 80 00 00 00 ) 
  .method public hidebysig newslot abstract virtual 
          instance void  Initialize([in] object  marshal( iunknown ) pRtpSender) runtime managed internalcall
  {
  } // end of method IRtpRenderer::Initialize

} // end of class P.DShow.Filters.IRtpRenderer

.class public auto ansi import P.DShow.Filters.RtpRendererClass
       extends [mscorlib]System.Object
       implements P.DShow.Filters.IRtpRenderer,
                  P.DShow.Filters.RtpRenderer
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 43 43 37 43 45 39 46 37 2D 30 39 32 37   // ..$CC7CE9F7-0927
                                                                                                  2D 34 42 45 36 2D 38 34 45 37 2D 33 30 35 43 34   // -4BE6-84E7-305C4
                                                                                                  45 34 35 44 33 45 31 00 00 )                      // E45D3E1..
  .custom instance void [mscorlib]System.Runtime.InteropServices.TypeLibTypeAttribute::.ctor(int16) = ( 01 00 02 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.ClassInterfaceAttribute::.ctor(int16) = ( 01 00 00 00 00 00 ) 
  .method public specialname rtspecialname 
          instance void  .ctor() runtime managed internalcall
  {
  } // end of method RtpRendererClass::.ctor

  .method public hidebysig newslot virtual 
          instance void  Initialize([in] object  marshal( iunknown ) pRtpSender) runtime managed internalcall
  {
    .override P.DShow.Filters.IRtpRenderer::Initialize
  } // end of method RtpRendererClass::Initialize

} // end of class P.DShow.Filters.RtpRendererClass

.class interface public abstract auto ansi import P.DShow.Filters.RtpRenderer
       implements P.DShow.Filters.IRtpRenderer
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 35 41 33 44 43 30 42 38 2D 36 33 44 35   // ..$5A3DC0B8-63D5
                                                                                                  2D 34 34 30 42 2D 38 33 34 44 2D 30 37 35 43 34   // -440B-834D-075C4
                                                                                                  42 43 35 45 37 42 37 00 00 )                      // BC5E7B7..
  .custom instance void [mscorlib]System.Runtime.InteropServices.CoClassAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 27 4D 53 52 2E 4C 53 54 2E 4D 44 53 68 6F   // ..'MSR.LST.MDSho
                                                                                                                          77 2E 46 69 6C 74 65 72 73 2E 52 74 70 52 65 6E   // w.Filters.RtpRen
                                                                                                                          64 65 72 65 72 43 6C 61 73 73 00 00 )             // dererClass..
} // end of class P.DShow.Filters.RtpRenderer

.class interface public abstract auto ansi import P.DShow.Filters.IScreenScraper
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 41 44 42 43 46 33 33 46 2D 37 30 41 33   // ..$ADBCF33F-70A3
                                                                                                  2D 34 32 39 30 2D 41 38 30 31 2D 37 42 34 44 34   // -4290-A801-7B4D4
                                                                                                  33 46 46 30 42 41 43 00 00 )                      // 3FF0BAC..
  .custom instance void [mscorlib]System.Runtime.InteropServices.TypeLibTypeAttribute::.ctor(int16) = ( 01 00 80 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .method public hidebysig newslot abstract virtual 
          instance void  Handle([in] valuetype P.DShow.Filters._RemotableHandle& hWnd) runtime managed internalcall
  {
    .param [1]
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComAliasNameAttribute::.ctor(string) = ( 01 00 1F 4D 53 52 2E 4C 53 54 2E 4D 44 53 68 6F   // ...MSR.LST.MDSho
                                                                                                            77 2E 46 69 6C 74 65 72 73 2E 77 69 72 65 48 57   // w.Filters.wireHW
                                                                                                            4E 44 00 00 )                                     // ND..
  } // end of method IScreenScraper::Handle

  .method public hidebysig newslot abstract virtual 
          instance void  FrameRate([in] int32 FrameRate) runtime managed internalcall
  {
  } // end of method IScreenScraper::FrameRate

} // end of class P.DShow.Filters.IScreenScraper

.class public sequential ansi sealed beforefieldinit P.DShow.Filters._RemotableHandle
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .field public int32 fContext
  .field public valuetype P.DShow.Filters.__MIDL_IWinTypes_0009 u
} // end of class P.DShow.Filters._RemotableHandle

.class public explicit ansi sealed P.DShow.Filters.__MIDL_IWinTypes_0009
       extends [mscorlib]System.ValueType
{
  .pack 4
  .size 0
  .field [0] public int32 hInproc
  .field [0] public int32 hRemote
} // end of class P.DShow.Filters.__MIDL_IWinTypes_0009

.class public auto ansi import P.DShow.Filters.ScreenScraperClass
       extends [mscorlib]System.Object
       implements P.DShow.Filters.IScreenScraper,
                  P.DShow.Filters.ScreenScraper
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.TypeLibTypeAttribute::.ctor(int16) = ( 01 00 02 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.ClassInterfaceAttribute::.ctor(int16) = ( 01 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 36 36 42 41 35 39 36 35 2D 33 30 39 32   // ..$66BA5965-3092
                                                                                                  2D 34 32 32 33 2D 38 36 34 39 2D 34 39 36 45 37   // -4223-8649-496E7
                                                                                                  41 42 36 37 46 32 35 00 00 )                      // AB67F25..
  .method public specialname rtspecialname 
          instance void  .ctor() runtime managed internalcall
  {
  } // end of method ScreenScraperClass::.ctor

  .method public hidebysig newslot virtual 
          instance void  Handle([in] valuetype P.DShow.Filters._RemotableHandle& hWnd) runtime managed internalcall
  {
    .param [1]
    .custom instance void [mscorlib]System.Runtime.InteropServices.ComAliasNameAttribute::.ctor(string) = ( 01 00 1F 4D 53 52 2E 4C 53 54 2E 4D 44 53 68 6F   // ...MSR.LST.MDSho
                                                                                                            77 2E 46 69 6C 74 65 72 73 2E 77 69 72 65 48 57   // w.Filters.wireHW
                                                                                                            4E 44 00 00 )                                     // ND..
    .override P.DShow.Filters.IScreenScraper::Handle
  } // end of method ScreenScraperClass::Handle

  .method public hidebysig newslot virtual 
          instance void  FrameRate([in] int32 FrameRate) runtime managed internalcall
  {
    .override P.DShow.Filters.IScreenScraper::FrameRate
  } // end of method ScreenScraperClass::FrameRate

} // end of class P.DShow.Filters.ScreenScraperClass

.class interface public abstract auto ansi import P.DShow.Filters.ScreenScraper
       implements P.DShow.Filters.IScreenScraper
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 41 44 42 43 46 33 33 46 2D 37 30 41 33   // ..$ADBCF33F-70A3
                                                                                                  2D 34 32 39 30 2D 41 38 30 31 2D 37 42 34 44 34   // -4290-A801-7B4D4
                                                                                                  33 46 46 30 42 41 43 00 00 )                      // 3FF0BAC..
  .custom instance void [mscorlib]System.Runtime.InteropServices.CoClassAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 29 4D 53 52 2E 4C 53 54 2E 4D 44 53 68 6F   // ..)MSR.LST.MDSho
                                                                                                                          77 2E 46 69 6C 74 65 72 73 2E 53 63 72 65 65 6E   // w.Filters.Screen
                                                                                                                          53 63 72 61 70 65 72 43 6C 61 73 73 00 00 )       // ScraperClass..
} // end of class P.DShow.Filters.ScreenScraper

.class interface public abstract auto ansi import P.DShow.Filters.ICheckPosFilter
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 44 31 35 31 35 38 37 41 2D 39 46 44 36   // ..$D151587A-9FD6
                                                                                                  2D 34 36 44 39 2D 42 34 36 39 2D 35 42 31 35 41   // -46D9-B469-5B15A
                                                                                                  46 39 41 42 43 34 42 00 00 )                      // F9ABC4B..
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.TypeLibTypeAttribute::.ctor(int16) = ( 01 00 80 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.InterfaceTypeAttribute::.ctor(int16) = ( 01 00 01 00 00 00 ) 
  .method public hidebysig newslot abstract virtual 
          instance void  SetEffect([in] int32 effect) runtime managed internalcall
  {
  } // end of method ICheckPosFilter::SetEffect

  .method public hidebysig newslot abstract virtual 
          instance void  SetThreshold([in] int32 thresh) runtime managed internalcall
  {
  } // end of method ICheckPosFilter::SetThreshold

  .method public hidebysig newslot abstract virtual 
          instance void  SetRefImage([in] int32 sz,
                                     [in] uint8& img) runtime managed internalcall
  {
  } // end of method ICheckPosFilter::SetRefImage

  .method public hidebysig newslot abstract virtual 
          instance void  Snapshot([in] string  marshal( lpwstr) filename) runtime managed internalcall
  {
  } // end of method ICheckPosFilter::Snapshot

  .method public hidebysig newslot abstract virtual 
          instance void  Reset() runtime managed internalcall
  {
  } // end of method ICheckPosFilter::Reset

  .method public hidebysig newslot abstract virtual 
          instance void  GetEffect([out] int32& effect) runtime managed internalcall
  {
  } // end of method ICheckPosFilter::GetEffect

  .method public hidebysig newslot abstract virtual 
          instance void  GetWidth([out] int32& width) runtime managed internalcall
  {
  } // end of method ICheckPosFilter::GetWidth

  .method public hidebysig newslot abstract virtual 
          instance void  GetHeight([out] int32& height) runtime managed internalcall
  {
  } // end of method ICheckPosFilter::GetHeight

  .method public hidebysig newslot abstract virtual 
          instance void  GetThreshold([out] int32& thresh) runtime managed internalcall
  {
  } // end of method ICheckPosFilter::GetThreshold

  .method public hidebysig newslot abstract virtual 
          instance void  GetImageSize([out] int32& sz) runtime managed internalcall
  {
  } // end of method ICheckPosFilter::GetImageSize

  .method public hidebysig newslot abstract virtual 
          instance void  GetRefImage(native int img) runtime managed internalcall
  {
  } // end of method ICheckPosFilter::GetRefImage

  .method public hidebysig newslot abstract virtual 
          instance void  GetCurImage(native int img) runtime managed internalcall
  {
  } // end of method ICheckPosFilter::GetCurImage

  .method public hidebysig newslot abstract virtual 
          instance void  Flip([in] int32 'on') runtime managed internalcall
  {
  } // end of method ICheckPosFilter::Flip

  .method public hidebysig newslot abstract virtual 
          instance void  Mirrow([in] int32 'on') runtime managed internalcall
  {
  } // end of method ICheckPosFilter::Mirrow

  .method public hidebysig newslot abstract virtual 
          instance void  SetCropArea([in] int32 x,
                                     [in] int32 y,
                                     [in] int32 w) runtime managed internalcall
  {
  } // end of method ICheckPosFilter::SetCropArea

  .method public hidebysig newslot abstract virtual 
          instance void  GetCropArea([out] int32& x,
                                     [out] int32& y,
                                     [out] int32& w) runtime managed internalcall
  {
  } // end of method ICheckPosFilter::GetCropArea

  .method public hidebysig newslot abstract virtual 
          instance void  SetRefImageFile([in] string  marshal( lpwstr) filename) runtime managed internalcall
  {
  } // end of method ICheckPosFilter::SetRefImageFile

} // end of class P.DShow.Filters.ICheckPosFilter

.class public auto ansi import P.DShow.Filters.PCheckPosFilterClass
       extends [mscorlib]System.Object
       implements P.DShow.Filters.ICheckPosFilter,
                  P.DShow.Filters.PCheckPosFilter
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 34 42 34 43 39 36 31 32 2D 45 39 38 32   // ..$4B4C9612-E982
                                                                                                  2D 34 41 45 30 2D 42 33 36 30 2D 32 44 39 33 41   // -4AE0-B360-2D93A
                                                                                                  30 46 36 39 42 37 34 00 00 )                      // 0F69B74..
  .custom instance void [mscorlib]System.Runtime.InteropServices.ClassInterfaceAttribute::.ctor(int16) = ( 01 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComConversionLossAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.TypeLibTypeAttribute::.ctor(int16) = ( 01 00 02 00 00 00 ) 
  .method public specialname rtspecialname 
          instance void  .ctor() runtime managed internalcall
  {
  } // end of method PCheckPosFilterClass::.ctor

  .method public hidebysig newslot virtual 
          instance void  SetEffect([in] int32 effect) runtime managed internalcall
  {
    .override P.DShow.Filters.ICheckPosFilter::SetEffect
  } // end of method PCheckPosFilterClass::SetEffect

  .method public hidebysig newslot virtual 
          instance void  SetThreshold([in] int32 thresh) runtime managed internalcall
  {
    .override P.DShow.Filters.ICheckPosFilter::SetThreshold
  } // end of method PCheckPosFilterClass::SetThreshold

  .method public hidebysig newslot virtual 
          instance void  SetRefImage([in] int32 sz,
                                     [in] uint8& img) runtime managed internalcall
  {
    .override P.DShow.Filters.ICheckPosFilter::SetRefImage
  } // end of method PCheckPosFilterClass::SetRefImage

  .method public hidebysig newslot virtual 
          instance void  Snapshot([in] string  marshal( lpwstr) filename) runtime managed internalcall
  {
    .override P.DShow.Filters.ICheckPosFilter::Snapshot
  } // end of method PCheckPosFilterClass::Snapshot

  .method public hidebysig newslot virtual 
          instance void  Reset() runtime managed internalcall
  {
    .override P.DShow.Filters.ICheckPosFilter::Reset
  } // end of method PCheckPosFilterClass::Reset

  .method public hidebysig newslot virtual 
          instance void  GetEffect([out] int32& effect) runtime managed internalcall
  {
    .override P.DShow.Filters.ICheckPosFilter::GetEffect
  } // end of method PCheckPosFilterClass::GetEffect

  .method public hidebysig newslot virtual 
          instance void  GetWidth([out] int32& width) runtime managed internalcall
  {
    .override P.DShow.Filters.ICheckPosFilter::GetWidth
  } // end of method PCheckPosFilterClass::GetWidth

  .method public hidebysig newslot virtual 
          instance void  GetHeight([out] int32& height) runtime managed internalcall
  {
    .override P.DShow.Filters.ICheckPosFilter::GetHeight
  } // end of method PCheckPosFilterClass::GetHeight

  .method public hidebysig newslot virtual 
          instance void  GetThreshold([out] int32& thresh) runtime managed internalcall
  {
    .override P.DShow.Filters.ICheckPosFilter::GetThreshold
  } // end of method PCheckPosFilterClass::GetThreshold

  .method public hidebysig newslot virtual 
          instance void  GetImageSize([out] int32& sz) runtime managed internalcall
  {
    .override P.DShow.Filters.ICheckPosFilter::GetImageSize
  } // end of method PCheckPosFilterClass::GetImageSize

  .method public hidebysig newslot virtual 
          instance void  GetRefImage(native int img) runtime managed internalcall
  {
    .override P.DShow.Filters.ICheckPosFilter::GetRefImage
  } // end of method PCheckPosFilterClass::GetRefImage

  .method public hidebysig newslot virtual 
          instance void  GetCurImage(native int img) runtime managed internalcall
  {
    .override P.DShow.Filters.ICheckPosFilter::GetCurImage
  } // end of method PCheckPosFilterClass::GetCurImage

  .method public hidebysig newslot virtual 
          instance void  Flip([in] int32 'on') runtime managed internalcall
  {
    .override P.DShow.Filters.ICheckPosFilter::Flip
  } // end of method PCheckPosFilterClass::Flip

  .method public hidebysig newslot virtual 
          instance void  Mirrow([in] int32 'on') runtime managed internalcall
  {
    .override P.DShow.Filters.ICheckPosFilter::Mirrow
  } // end of method PCheckPosFilterClass::Mirrow

  .method public hidebysig newslot virtual 
          instance void  SetCropArea([in] int32 x,
                                     [in] int32 y,
                                     [in] int32 w) runtime managed internalcall
  {
    .override P.DShow.Filters.ICheckPosFilter::SetCropArea
  } // end of method PCheckPosFilterClass::SetCropArea

  .method public hidebysig newslot virtual 
          instance void  GetCropArea([out] int32& x,
                                     [out] int32& y,
                                     [out] int32& w) runtime managed internalcall
  {
    .override P.DShow.Filters.ICheckPosFilter::GetCropArea
  } // end of method PCheckPosFilterClass::GetCropArea

  .method public hidebysig newslot virtual 
          instance void  SetRefImageFile([in] string  marshal( lpwstr) filename) runtime managed internalcall
  {
    .override P.DShow.Filters.ICheckPosFilter::SetRefImageFile
  } // end of method PCheckPosFilterClass::SetRefImageFile

} // end of class P.DShow.Filters.PCheckPosFilterClass

.class interface public abstract auto ansi import P.DShow.Filters.PCheckPosFilter
       implements P.DShow.Filters.ICheckPosFilter
{
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 44 31 35 31 35 38 37 41 2D 39 46 44 36   // ..$D151587A-9FD6
                                                                                                  2D 34 36 44 39 2D 42 34 36 39 2D 35 42 31 35 41   // -46D9-B469-5B15A
                                                                                                  46 39 41 42 43 34 42 00 00 )                      // F9ABC4B..
  .custom instance void [mscorlib]System.Runtime.InteropServices.CoClassAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 2B 4D 53 52 2E 4C 53 54 2E 4D 44 53 68 6F   // ..+MSR.LST.MDSho
                                                                                                                          77 2E 46 69 6C 74 65 72 73 2E 50 43 68 65 63 6B   // w.Filters.PCheck
                                                                                                                          50 6F 73 46 69 6C 74 65 72 43 6C 61 73 73 00 00 ) // PosFilterClass..
} // end of class P.DShow.Filters.PCheckPosFilter


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file D:\cygwin\home\onuchin\wrktmp\cxp\P.DShow\Interop\filters.interop.res
