// $Id: dbUser.cs 2105 2014-02-27 13:06:37Z onuchin $
// db helper classes stolen from  http://www.codeproject.com/Articles/690207/Csharp-Project-on-Database-for-Beginners

using System;
using System.Data;
using System.Data.Odbc;
using System.Windows.Forms;


namespace DbClass
{
    class DbUser : DbConnection
    {
        public int LoginId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool LogType { get; set; }

        public bool ValidRegLogUser()
        {
            bool userValid = false;

            using (var cmd = new OdbcCommand()) {
                openConnection();

                OdbcDataReader conReader;
                conReader = null;
                cmd.CommandText = "Select * from RegLogUser where username=@userName and Password=@UserPassword";
                cmd.Connection = Connection;
                cmd.Transaction = Transaction;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@userName", SqlDbType.VarChar).Value = UserName;
                cmd.Parameters.Add("@UserPassword", SqlDbType.VarChar).Value = Password;

                try {
                    conReader = cmd.ExecuteReader();

                    while (conReader.Read()) {
                        //MessageBox.Show(conReader.GetString(1));
                        //LoginId = Convert.ToInt32(conReader["LoginID"]);
                        //LogType = (bool)conReader["LogType"];
                        userValid = true;
                    }
                } catch (Exception ex) {
                    errorTransaction();
                    throw new ApplicationException("Something wrong happened in the Login module :", ex);
                } finally {
                   if (conReader != null) {
                      conReader.Close();
                   }
                   closeConnection();
                }
            }

            return userValid;
        }
    }
}
