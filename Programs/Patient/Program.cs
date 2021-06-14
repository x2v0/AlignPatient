// $Id: Program.cs 2028 2014-01-10 14:19:59Z onuchin $
// Author: Valeriy Onuchin   05.04.2011

using System;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Globalization;
using P;
namespace PatientDisplay
{

   static class Program 
   {

      static private PatientForm gForm;

      static Program()
      {
         AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
         Application.ApplicationExit += OnApplicationExit;

         //UnhandledExceptionHandler.Register();
      }

      static public void OnApplicationExit(object sender, EventArgs e) 
      {
    
         if (gForm != null) {
            gForm.Cleanup();
            gForm = null;
         }
         Session.FindAndKillProcess("PatientDisplay");
      }

      private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
      {
         Assembly executingAssembly = Assembly.GetExecutingAssembly();
         var assemblyName = new AssemblyName(args.Name);
 
         string path = "PatientDisplay." + assemblyName.Name + ".dll";

         if (assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture) == false) {
            path = String.Format(@"{0}\{1}", assemblyName.CultureInfo, path);
         }

         using (Stream stream = executingAssembly.GetManifestResourceStream(path)) {

            if (stream == null) {
                return null;
            }

            var assemblyRawBytes = new byte[stream.Length];
            stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);

            return Assembly.Load(assemblyRawBytes);
         }
      }

      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main(string[] args) 
      {
         //PatientApplicationContext context = new PatientApplicationContext();
         gForm = new PatientForm(args);

         Application.Run(gForm);
      }
   }
}