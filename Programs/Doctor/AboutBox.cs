using System;
using System.Reflection;
using System.Windows.Forms;

namespace DoctorDisplay {
   partial class AboutBox : Form {
      public AboutBox() {
         InitializeComponent();
         Text = Strings.AboutBox_AboutBox_Контроль_положения_пациента;
         labelProductName.Text = Strings.AboutBox_AboutBox_Контроль_положения_пациента;
         labelVersion.Text = String.Format("Version {0} {0}", AssemblyVersion);
         labelCopyright.Text = Strings.AboutBox_AboutBox_onuchin_cern_ch;
         labelCompanyName.Text = Strings.AboutBox_AboutBox_ЗАО_ПРОТОМ;
         textBoxDescription.Text = "";
      }

      #region Assembly Attribute Accessors

      public string AssemblyTitle {
         get {
         var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);

            if (attributes.Length > 0) {
               var titleAttribute = (AssemblyTitleAttribute)attributes[0];

               if (titleAttribute.Title != "") {
                  return titleAttribute.Title;
               }
            }

            return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
         }
      }

      public string AssemblyVersion {
         get {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
         }
      }

      public string AssemblyDescription {
         get {
         var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);

            if (attributes.Length == 0) {
               return "";
            }
            return ((AssemblyDescriptionAttribute)attributes[0]).Description;
         }
      }

      public string AssemblyProduct {
         get {
            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (attributes.Length == 0) {
               return "";
            }
            return ((AssemblyProductAttribute)attributes[0]).Product;
         }
      }

      public string AssemblyCopyright {
         get {
            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attributes.Length == 0) {
               return "";
            }
            return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
         }
      }

      public string AssemblyCompany {
         get {
            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            if (attributes.Length == 0) {
               return "";
            }
            return ((AssemblyCompanyAttribute)attributes[0]).Company;
         }
      }
      #endregion
   }
}
