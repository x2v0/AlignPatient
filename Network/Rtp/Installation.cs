// $Id: $
// Author: Valeriy Onuchin   29.12.2010

using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;

using Microsoft.Win32;


namespace P.Net.Rtp
{

    /// <summary>
    /// Installer class for Rtp, handles the tasks necessary to get Rtp up and functional.  This routine:
    /// 
    ///  * Checks to ensure an Administrator is logged in.  Installation cannot proceed in a limited account.
    ///  * Uninstalls first upon installation to make sure we're in a well known, reasonably clean state.
    ///  * Opens the firewall port, something that a limited account doesn't have privs to do.
    ///  * Installs the Rtp Event Log, adjusts its maximum size, and sets it to overwrite as needed.
    ///  * Creates Performance Counters for Rtp Listener, Rtp Stream, and Rtp Sender so we can use the Performance Monitor to track application status
    ///  * Sets a registry entry that says we're properly installed
    ///  
    ///  Both installation and uninstallation are handled by this class.
    /// </summary>
    [ComVisible(false)]
    [RunInstaller(true)]
    public class Installation : Installer
    {
        const string baseRegKeyName = "SOFTWARE\\PROTOM\\Rtp";

        public static bool Installed
        {
            get
            {
                bool installed = false;
                try
                {
                    using (RegistryKey pcaKey = Registry.LocalMachine.OpenSubKey(baseRegKeyName))
                    {
                        if (pcaKey != null)
                        {
                            object o = pcaKey.GetValue("RtpInstalled");
                            if (o != null)
                                installed = Convert.ToBoolean(o, CultureInfo.InvariantCulture);
                        }
                    }
                } 
                catch {}

                return installed;
            }
            set
            {
                try
                {
                    if(value)
                    {
                        using (RegistryKey pcaKey = Registry.LocalMachine.CreateSubKey(baseRegKeyName))
                        {
                            pcaKey.SetValue("RtpInstalled", true);
                        }
                    }
                    else
                    {
                        using (RegistryKey pcaKey = Registry.LocalMachine.OpenSubKey(baseRegKeyName, true))
                        {
                            pcaKey.DeleteValue("RtpInstalled");
                        }
                    }
                }
                catch {}
            }
        }

        /// <summary>
        /// This routine should be called automatically by the MSI during setup, but it can also be called using:
        ///     "installutil.exe P.Rtp.dll"
        /// </summary>
        /// <param name="savedState">State dictionary passed in by the installer code</param>
        public override void Install (IDictionary savedState)
        {
            #region Check to make sure we're in an Administrator role
            WindowsPrincipal wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            if (wp.IsInRole(WindowsBuiltInRole.Administrator) == false)
            {
                RtlAwareMessageBox.Show(null, Strings.YouMustBeAnAdministratorToInstall, 
                    Strings.AdministratorPrivilegesRequired, MessageBoxButtons.OK, MessageBoxIcon.Stop, 
                    MessageBoxDefaultButton.Button1, 0);
                Application.Exit();
            }
            #endregion
            #region Uninstall in case we weren't uninstalled cleanly before
            IDictionary state = new Hashtable();
            Uninstall(state);
            if (state.Count != 0)
                Commit(state);
            state = null;
            #endregion
            #region Call base.Install
            base.Install(savedState);
            #endregion
            #region Install the Event Logs

            RtpEL.Install();

            #endregion
            #region Create PerfCounters

            PCInstaller.Install();

            #endregion
            #region Save the fact that we're installed to the registry
            Installed = true;
            #endregion
        }

        /// <summary>
        /// This routine should be called automatically by the MSI during Remove Programs, but it can also be called using:
        ///     "installutil.exe /u P.Rtp.dll"
        /// </summary>
        /// <param name="savedState">State dictionary passed in by the installer code</param>
        public override void Uninstall (IDictionary savedState)
        {
            #region Check to make sure we're in an Administrator role
            WindowsPrincipal wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            if (wp.IsInRole(WindowsBuiltInRole.Administrator) == false)
            {
                RtlAwareMessageBox.Show(null, Strings.YouMustBeAnAdministratorToInstall, 
                    Strings.AdministratorPrivilegesRequired, MessageBoxButtons.OK, MessageBoxIcon.Stop, 
                    MessageBoxDefaultButton.Button1, 0);
                Application.Exit();
            }
            #endregion
            #region Delete the Event Logs

            RtpEL.Uninstall();

            #endregion
            #region Whack PerfCounters

            PCInstaller.Uninstall();

            #endregion
            #region Whack registry entry saying we're installed
            Installed = false;
            #endregion
            #region Call base.Uninstall
            if (savedState.Count != 0)
                base.Uninstall(savedState);
            #endregion
        }
    }
}
