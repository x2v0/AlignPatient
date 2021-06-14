// $Id: $

using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Microsoft.Win32;


namespace P.DShow
{
    [RunInstaller(true)]
    public class Installation : Installer
    {
        public override void Commit(IDictionary savedState)
        {
            MDShowEventLog.Install();

            string oldDirectory = Directory.GetCurrentDirectory();
            FileInfo fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(fi.Directory.FullName);

            try
            {
                RegisterRtpFilters();
            }
            catch (DllNotFoundException)
            {
                RtlAwareMessageBox.Show(null, Strings.MissingRtpFiltersError, Strings.FileNotFound, 
                    MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0);
            }

            try
            {
               RegisterCheckPosFilter();
            }
            catch (DllNotFoundException)
            {
               RtlAwareMessageBox.Show(null, Strings.MissingCheckPosFilterError, Strings.FileNotFound,
                   MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0);
            }

            Directory.SetCurrentDirectory(oldDirectory);            
        }

        public override void Uninstall(IDictionary savedState)
        {
            string oldDirectory = Directory.GetCurrentDirectory();
            FileInfo fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(fi.Directory.FullName);

            try
            {
                UnregisterRtpFilters();
            }
            catch (DllNotFoundException)
            {
                RtlAwareMessageBox.Show(null, Strings.MissingRtpFiltersError, Strings.FileNotFound, 
                    MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0);
            }

            try
            {
               UnregisterCheckPosFilter();
            }
            catch (DllNotFoundException)
            {
               RtlAwareMessageBox.Show(null, Strings.MissingCheckPosFilterError, Strings.FileNotFound,
                   MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0);
            }

            Directory.SetCurrentDirectory(oldDirectory);

            MDShowEventLog.Uninstall();

            if (savedState.Count != 0)
                base.Uninstall(savedState);
        }

        [DllImport("PRtpFilter.ax", EntryPoint="DllRegisterServer")]
        private static extern void RegisterRtpFilters();

        [DllImport("PRtpFilter.ax", EntryPoint="DllUnregisterServer")]
        private static extern void UnregisterRtpFilters();

        [DllImport("PCheckPosFilter.ax", EntryPoint = "DllRegisterServer")]
        private static extern void RegisterCheckPosFilter();

        [DllImport("PCheckPosFilter.ax", EntryPoint = "DllUnregisterServer")]
        private static extern void UnregisterCheckPosFilter();

    }
}
