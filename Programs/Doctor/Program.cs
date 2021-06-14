// ***********************************************************************
// Assembly         : DoctorDisplay
// Author           : onuchin
// Created          : 02-26-2014
//
// Last Modified By : onuchin
// Last Modified On : 02-26-2014
// ***********************************************************************
// <copyright file="Program.cs" company="PROTOM">
//     Copyright (c) PROTOM. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

// $Id: Program.cs 2106 2014-02-27 13:07:28Z onuchin $

using System;
using System.Configuration;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Collections.Specialized;

using P;


namespace DoctorDisplay
{

   /// <summary>
   /// Class Program
   /// </summary>
   static class Program 
   {
      /// <summary>
      /// Singleton Main frame
      /// </summary>
      static public DoctorDisplayForm gForm;

      /// <summary>
      /// Initializes static members of the <see cref="Program"/> class.
      /// </summary>
      static Program()
      {
         AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
         Application.ApplicationExit += OnApplicationExit;

         //UnhandledExceptionHandler.Register();
      }

      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      /// <param name="args">The args.</param>
      [STAThread]
      static void Main(string[] args) 
      {
         Session.FindAndKillProcess("DoctorDisplay");

         DoctorDisplayForm.gArguments = new ArgumentParser(args);
         InvokeArguments(DoctorDisplayForm.gArguments.Parameters);

         Session.CallingForm = gForm = new DoctorDisplayForm();
         Application.Run(gForm);
      }

      /// <summary>
      /// Invokes the arguments.
      /// </summary>
      /// <param name="parameters">The parameters.</param>
      private static void InvokeArguments(StringDictionary parameters)
      {
         // set default values from App.conig file
         Session.AppConfig();

         // override values App.conig file
         if (parameters.ContainsKey("f")) {
            DoctorDisplayForm.gRefFileName = parameters["f"];
            Session.Local = true;
            Session.NewSession = false;

            if (Path.GetDirectoryName(DoctorDisplayForm.gRefFileName) == null) {  
               DoctorDisplayForm.gRefFileName = Path.Combine(Session.DataBasePath, DoctorDisplayForm.gRefFileName);
            }
         }

         if (parameters.ContainsKey("id")) {
            Session.PatientId = int.Parse(parameters["id"]);
            Session.Local = false;
         }

         if (parameters.ContainsKey("dsn")) {
            //   = parameters["dsn"];
         }

         if (parameters.ContainsKey("pname")) {
            Session.PatientName = parameters["pname"];
         }

         if (parameters.ContainsKey("dname")) {
            Session.DoctorName = parameters["dname"];
         }

         if (parameters.ContainsKey("help") || parameters.ContainsKey("h")) {
            MessageBox.Show(Strings.Usage, Strings.Usage,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
         }
#if DEBUG
         if (!Path.IsPathRooted(Session.DataBasePath)) {
            var exepath = Path.GetDirectoryName(Application.StartupPath);

            if (exepath != null) {
               Session.DataBasePath = Path.Combine(exepath, Session.DataBasePath);
            }

            Session.DataBasePath = Path.GetFullPath(Session.DataBasePath);
         }

         if (!Directory.Exists(Session.DataBasePath)) {  //
            //Directory.CreateDirectory(Session.DataBasePath);
            // more coding in case of creation a new database
         }
#if LOCAL_DEBUG
         MessageBox.Show(Session.DataBasePath);
#endif
         AppDomain.CurrentDomain.SetData("DataDirectory", Session.DataBasePath);
#endif
         Thread.CurrentThread.CurrentUICulture = new CultureInfo(ConfigurationManager.AppSettings["DefaultCulture"]);
      }

      #region Global Events

      /// <summary>
      /// Called when [application exit].
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private static void OnApplicationExit(object sender, EventArgs e) 
      {
         if (gForm != null) {
            gForm.Cleanup();
            gForm = null;
         }
      }

      /// <summary>
      /// Called when [resolve assembly].
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="args">The <see cref="ResolveEventArgs"/> instance containing the event data.</param>
      /// <returns>Assembly.</returns>
      private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
      {
         var executingAssembly = Assembly.GetExecutingAssembly();
         var assemblyName = new AssemblyName(args.Name);
 
         var path = "DoctorDisplay." + assemblyName.Name + ".dll";

         if (assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture) == false) {
            path = String.Format(@"{0}\{1}", assemblyName.CultureInfo, path);
         }

         using (var stream = executingAssembly.GetManifestResourceStream(path)) {

            if (stream == null) {
                return null;
            }

            var assemblyRawBytes = new byte[stream.Length];
            stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);

            return Assembly.Load(assemblyRawBytes);
         }
      }

      #endregion Global Events
   }
}