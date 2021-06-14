using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DoctorDisplay {
   partial class AboutFixing : Form {
      public AboutFixing() {
         InitializeComponent();
         Text = Strings.PatientPositionFixMode;
         labelProductName.Text = Strings.PatientPositionFixText1;
         labelVersion.Text = Strings.PatientPositionFixText2;
         labelCopyright.Text = Strings.PatientPositionFixText3;
         labelCompanyName.Text = Strings.PatientPositionFixText4;
         //textBoxDescription.Lines[0] = "Примечания:";
         //textBoxDescription.Lines[1] = "- нижняя граниица зоны интереса должна лежать в районе подбородка";
         //textBoxDescription.Lines[2] = "- верхняя граниица зоны интереса должна лежать в бровей";
         //textBoxDescription.Lines[3] = "- для сброса зоны интереса и реперного снимка нажмите кнопку \"Сброс\"";
      }

      private void fixingShown(object sender, EventArgs e) {
         ShowInTaskbar = true;
         TopMost = true;
         Focus();
         BringToFront();
         TopMost = false;
      }
   }
}
