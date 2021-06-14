// $Id: dbProcess.cs 2105 2014-02-27 13:06:37Z onuchin $
// db helper classes stolen from  http://www.codeproject.com/Articles/690207/Csharp-Project-on-Database-for-Beginners

using System.Data;
using System.Data.Odbc;
using System.Windows.Forms;

namespace DbClass
{
    class DbProcess : DbConnection
    {
        public bool isRecordExists(string fldName, string tblName, string param)
        {
            var sSql = "SELECT " + fldName + " From " + tblName + " WHERE " + fldName + "= '" + param + "'";
            var dr = setDataReader(sSql);
            dr.Read();
            var isExists = ((dr.HasRows == true) ? true : false);
            dr.Close();
            dr.Dispose();
            return isExists;
        }

        public OdbcDataReader getFields(string fldName, string tblName, string condition)
        {
            var sSql = "SELECT " + fldName + " FROM " + tblName + " " + condition;
            return setDataReader(sSql);
        }

        public void addRecord(string tblName, string values)
        {
            var sSql = "INSERT INTO " + tblName + " VALUES(" + values + ")";
            ExecuteSql(sSql);
        }

        public void UpdateRecord(string tblName, string values)
        {
            var sSql = "UPDATE " + tblName + " SET " + values;
            ExecuteSql(sSql);
        }

        public void DeleteRecord(string tblName, string values)
        {
            var sSql = "DELETE FROM " + tblName + " " + values;
            ExecuteSql(sSql);
        }

        public void ExecuteSp(string SPName, string condition)
        {
            var sSql = "EXEC " + SPName + " " + condition;
            ExecuteSql(sSql);
        }

        public OdbcDataReader getSP_Record(string SPName, string condition)
        {
            var sSql = "EXEC " + SPName + " " + condition;
            return setDataReader(sSql);
        }

        public void finishSave()
        {
            closeConnection();
            MessageBox.Show("Data Saved Successfully.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public string getValue(string val, string tbl, string condition)
        {
            var sSql = "SELECT " + val + " FROM " + tbl + " WHERE " + condition;
            var dr = setDataReader(sSql);
            string sValue;
            dr.Read();

            if (dr.HasRows) {
                sValue = ((dr[0].ToString().Trim() == "Null" || dr[0].ToString().Trim() == "") ? "" : dr[0].ToString().Trim());
            }
            else sValue = "";
            dr.Close();
            dr.Dispose();
            return sValue;
        }

        public DataSet FillDataWithOpenConn(string sSQL, string sTable)
        {
            return FillData(sSQL, sTable);
        }         

    }
}
