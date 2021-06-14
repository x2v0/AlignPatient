// $Id: $
// Author: Valeriy Onuchin   29.12.2010

using System;

namespace P.DShow
{
    // [HKEY_CLASSES_ROOT\CLSID\{62BE5D10-60EB-11d0-BD3B-00A0C911CE86}]
    // @="System Device Enumerator"
    // "Version"=dword:00000007
    //
    // [HKEY_CLASSES_ROOT\CLSID\{62BE5D10-60EB-11d0-BD3B-00A0C911CE86}\InprocServer32]
    // @="C:\\WINDOWS\\System32\\devenum.dll"
    // "ThreadingModel"="Both"
    class CreateDeviceEnumClass
    {
        private static Guid CLSID_DeviceEnum = new Guid("62BE5D10-60EB-11d0-BD3B-00A0C911CE86");

        public static ICreateDevEnum CreateInstance()
        {
            return (ICreateDevEnum)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_DeviceEnum, true));
        }
    }

    // [HKEY_LOCAL_MACHINE\SOFTWARE\Classes\CLSID\{BF87B6E1-8C27-11D0-B3F0-00AA003761C5}]
    // @="Capture Graph Builder 2"
    //
    // [HKEY_LOCAL_MACHINE\SOFTWARE\Classes\CLSID\{BF87B6E1-8C27-11D0-B3F0-00AA003761C5}\InprocServer32]
    // @="C:\\WINDOWS\\System32\\qcap.dll"
    // "ThreadingModel"="Both"
    public abstract class CaptureGraphBuilder2Class
    {
        private static Guid CLSID_CaptureGraphBuilder2 = new Guid("BF87B6E1-8C27-11D0-B3F0-00AA003761C5");

        public static ICaptureGraphBuilder2 CreateInstance()
        {
            return (ICaptureGraphBuilder2)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_CaptureGraphBuilder2, true));
        }
    }

    // [HKEY_CLASSES_ROOT\CLSID\{CDA42200-BD88-11d0-BD4E-00A0C911CE86}]
    // @="Filter Mapper2"
    //
    // [HKEY_CLASSES_ROOT\CLSID\{CDA42200-BD88-11d0-BD4E-00A0C911CE86}\InprocServer32]
    // @="C:\\WINDOWS\\System32\\quartz.dll"
    // "ThreadingModel"="Both"
    public abstract class FilterMapper2Class
    {
        private static Guid CLSID_FilterMapper2 = new Guid("CDA42200-BD88-11d0-BD4E-00A0C911CE86");

        public static IFilterMapper2 CreateInstance()
        {
            return (IFilterMapper2)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_FilterMapper2, true));
        }
    }

    /// <summary>
    /// Class for creating the RtpRenderer DShow filter
    /// 
    /// Guid was taken from RtpRenderer.idl in the DShow\RtpFilters folder
    /// </summary>
    public abstract class RtpRendererClass
    {
        private static Guid CLSID_RtpRenderer = new Guid("A2A1DF8F-BABD-4352-9CDE-4A624E34D44A");

        public static IBaseFilter CreateInstance()
        {
            return (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_RtpRenderer, true));
        }
    }

    /// <summary>
    /// Class for creating the RtpSource DShow filter
    /// 
    /// Guid was taken from RtpSource.idl in the DShow\RtpFilters folder
    /// </summary>
    public abstract class RtpSourceClass
    {
        private static Guid CLSID_RtpSource = new Guid("6DF31D07-99DA-4840-BA42-14784733D430");

        public static IBaseFilter CreateInstance()
        {
            return (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_RtpSource, true));
        }
    }

    /// <summary>
    /// Class for creating the CLSID_ScreenScraper DShow filter
    /// 
    /// Guid was taken from CLSID_ScreenScraper.idl in the DShow\ScreenScraper folder
    /// </summary>
    public abstract class ScreenScraperClass
    {
        private static Guid CLSID_ScreenScraper = new Guid("66BA5965-3092-4223-8649-496E7AB67F25");

        public static IBaseFilter CreateInstance()
        {
            return (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_ScreenScraper, true));
        }
    }

    /// <summary>
    /// Class for creating the CheckPosition filter
    /// 
    /// Guid was taken from PCheckPosFilter.idl in the ..\..\..\PCheckPosFilter folder
    /// </summary>
    public abstract class CheckPosFilterClass
    {
       private static Guid CLSID_CheckPosFilter = new Guid("4B4C9612-E982-4ae0-B360-2D93A0F69B74");

       public static IBaseFilter CreateInstance()
       {
          return (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_CheckPosFilter, true));
       }
    }
}
