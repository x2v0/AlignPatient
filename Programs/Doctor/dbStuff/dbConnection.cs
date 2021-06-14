// $Id: dbConnection.cs 2105 2014-02-27 13:06:37Z onuchin $
// db helper classes stolen from  http://www.codeproject.com/Articles/690207/Csharp-Project-on-Database-for-Beginners

using System.Data;
using System.Data.Odbc;

namespace DbClass
{
    // make this class abstract so that only dbProcess and dbUser class can access this class
    public abstract class DbConnection
    {
        public OdbcConnection Connection;
        public OdbcTransaction Transaction;

        public DbConnection()
        {
            //string strProject = "ERP001\\SQLR2"; //Enter your SQL server instance name
            //tring strDatabase = "vims"; //Enter your database name
            //string strUserID = "azmain "; // Enter your SQL Server User Name
            //string strPassword = "neelgiri"; // Enter your SQL Server Password
            //string strconn = "data source=" + strProject + ";Persist Security Info=false;database=" + strDatabase + ";user id=" + strUserID + ";password=" + strPassword + ";Connection Timeout = 0";
            Connection = new OdbcConnection(DoctorDisplay.Properties.Settings.Default.ConnectionString);
        }

        public void openConnection()
        {
            Connection.Close();
            Connection.Open();
            Transaction = Connection.BeginTransaction();
        }

        public void closeConnection()
        {
            Transaction.Commit();
            Connection.Close();
        }

        public void errorTransaction()
        {
            Transaction.Rollback();
            Connection.Close();
        }

        protected void ExecuteSql(string sSQL)
        {
            var cmdDate = new OdbcCommand(" SET DATEFORMAT dmy", Connection, Transaction);
            cmdDate.ExecuteNonQuery();
            var cmd = new OdbcCommand(sSQL, Connection, Transaction);
            cmd.ExecuteNonQuery();
        }

        protected void OnlyExecuteSql(string sSQL)
        {
            var cmd = new OdbcCommand(sSQL, Connection);
            cmd.ExecuteNonQuery();
        }

        protected DataSet FillDataSet(DataSet dset, string sSQL, string tbl)
        {
            var cmd = new OdbcCommand(sSQL, Connection);
            var adapter = new OdbcDataAdapter(cmd);

            try {
                adapter.Fill(dset, tbl);
            } finally {
                Connection.Close();
            }
            return dset;

        }

        protected DataSet FillData(string sSQL, string sTable)
        {
            var cmd = new OdbcCommand(sSQL, Connection, Transaction);
            var adapter = new OdbcDataAdapter(cmd);
            var ds = new DataSet();
            adapter.Fill(ds, sTable);
            return ds;
        }

        protected OdbcDataReader setDataReader(string sSQL)
        {
            var cmd = new OdbcCommand(sSQL, Connection, Transaction);
            cmd.CommandTimeout = 300;
            var rtnReader = cmd.ExecuteReader();
            return rtnReader;
        }  
    }
}
