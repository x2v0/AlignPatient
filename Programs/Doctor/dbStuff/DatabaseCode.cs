// $Id: DatabaseCode.cs 2119 2014-03-04 15:49:51Z onuchin $

using System;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using DbClass;
using P;
using P.Net;

namespace DoctorDisplay {
   public partial class DoctorDisplayForm {

      DbProcess fDb = new DbProcess();

      private string GenerateRefFilename(byte cameraId)
      {
         var filename = "RefImg";
         var now = DateTime.Now;
         Session.StudyTime = now;

         filename += string.Format("_{0:yyyy-MM-dd_HH-mm}_", now);
         filename += cameraId;
         filename += ".jpg";

         if (Session.PatientName == null) {  // do not store in database
            return filename;
         }
   
         filename = "pref";

         if (Session.PatientId >= 0) {
            filename += Session.PatientId + "_";
         }

         filename += string.Format("_{0:yyyy-MM-dd_HH-mm}_", now);
         filename += cameraId;
         filename += ".jpg";

         using (var cmd = new OdbcCommand()) {
            fDb.openConnection();
            OdbcDataReader conReader = null;
            cmd.CommandText = "Select * from Patients_tab where ID=@patientId";
            cmd.Connection = fDb.Connection;
            cmd.Transaction = fDb.Transaction;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@patientId", SqlDbType.Int).Value = Session.PatientId;

            try {
               conReader = cmd.ExecuteReader();
               conReader.Read();
               filename += conReader["LastName"] + "_" + conReader["FirstName"] + "_" + conReader["MiddleName"];
            } catch (Exception ex) {
               fDb.errorTransaction();
               throw new ApplicationException("Something wrong happened :", ex);
            } finally {
               if (conReader != null) {
                  conReader.Close();
               }
               fDb.closeConnection();
            }
         }

         return filename;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="filename"></param>
      /// <param name="camId"></param>
      /// <param name="comment"></param>
      /// <returns>true if new record added to database</returns>
      private bool  SaveRefFile2Db(string filename, int camId, string comment)
      {
       
         var select = "Select D_P, HeadRef, CX, CY, Width, Threshold, NCam, Comment " +
                      "from Head_tab where ID_P=" + Session.PatientId + " and NCam=" + camId;
         var headAdapter = new OdbcDataAdapter(select, fDb.Connection);
         var ds = new DataTable();
         int ret;

         using (var cb = new OdbcCommandBuilder(headAdapter)) {
            ret = headAdapter.Fill(ds);

            DataRow row;

            if (ret > 0) {
               // patient with such ID and camera ID  already exists -> update it
               row = ds.Rows[0];

               if (File.Exists(row["HeadRef"].ToString())) {
                  // delete exisiting file
                  File.Delete(row["HeadRef"].ToString());
               }
            } else {
               row = ds.NewRow();
            }

            row["ID_P"] = Session.PatientId;
            //row["Date"] = Session.StudyTime;
            //row["Time"] = Session.StudyTime;
            row["HeadRef"] = filename;
            row["CX"] = Session.RoiX;
            row["CY"] = Session.RoiY;
            row["Width"] = Session.RoiWidth;
            row["Threshold"] = Session.Threshold;
            row["Comment"] = comment;
            row["NCam"] = camId;

            headAdapter.Update(ds);
         }
         return (ret == 0);
      }

      /// <summary>
      /// Handle function called when reference image received from patient machine
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void FileReceived(object sender, OnFileReceivedArgs e)
      {
         if (InvokeRequired) {
            //Just in case we want to play with the UI
            Invoke(new OnFileReceivedDelegate(FileReceived), sender, e);
         } else {
            var cameraId = (byte)(51 - e.Buffer[0]);
            var filename = GenerateRefFilename(cameraId);

//#if LOCAL_DEBUG
            MessageBox.Show(Strings.ReferenceFileReceived + filename);
//#endif

            // save file locally through file dialog
            if (Session.Local || Session.PatientName == null) {
               try {

                  using (var sd = new SaveFileDialog {
                     FileName = filename, 
                     Title = Strings.SaveFile,
                     Filter = Strings.SaveFileTypes + filename,
                     FilterIndex = 1,
                     InitialDirectory = Path.Combine(Session.DataBasePath, Session.HeadPath)}) {
                  
                     if (sd.ShowDialog() == DialogResult.OK) { // ask user for file path & name
                        File.WriteAllBytes(sd.FileName, e.Buffer.Skip(1).ToArray());
                     } else {
                        return;
                     }
                  }

               } catch (Exception) {
                  MessageBox.Show(this, Strings.SaveFileError, Strings.Doctor_Display,
                                  MessageBoxButtons.OK, MessageBoxIcon.Stop);
               }

               return;
            }
 
            Session.NewSession = SaveRefFile2Db(filename, cameraId, "");

            buttonLoad.Enabled = true;
         }
      }
   }
}
