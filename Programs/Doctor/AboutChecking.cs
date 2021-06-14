using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DoctorDisplay {
   partial class AboutChecking : Form {
      public AboutChecking() {
         InitializeComponent();
         Text = Strings.PatientPositionVerifyMode;
         labelProductName.Text = Strings.PatientPositionVerifyText1;
         labelVersion.Text = Strings.PatientPositionVerifyText2;
         labelCopyright.Text = Strings.PatientPositionVerifyText3;
         labelCompanyName.Text = "";
         //textBoxDescription.Text = "";

         Focus();
         BringToFront();
      }

      private void checkingShown(object sender, EventArgs e) {
         ShowInTaskbar = true;
         TopMost = true;
         Focus();
         BringToFront();
         TopMost = false;
      }
   }
}
