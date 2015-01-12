using System;
using System.Data;
using System.Collections.Generic;
using System.Windows.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Oracle.DataAccess.Client;
using OraTest3;

namespace OraTest3
{
    public static class DbStuff
    {
        public static void MakeConnection()
        {
            OracleConnection connOra = new OracleConnection();

            try
            {
                // Connect to the database
                connOra.ConnectionString = MakeConn();
                connOra.Open();
            }
            catch (Exception ex)
            {
                ErrMsg(ex.Message + "\n\n" + ex.StackTrace + "\n\n" + ex.Source);
            }
            finally
            {
                // No matterv what, close and dispose
                connOra.Close();
                connOra.Dispose();
            }
        }

        public static DataSet LoadTableData(string searchTerm)
        {
            return FetchDataSet("SELECT TABLE_NAME, TABLE_TYPE, COMMENTS, OWNER FROM ALL_TAB_COMMENTS WHERE UPPER(comments) like '%'|| UPPER('" + searchTerm + "')||'%'");
        }

        public static DataSet LoadColumnData(string tblName)
        {
            return FetchDataSet("SELECT a.COLUMN_NAME, a.DATA_TYPE, b.COMMENTS, a.TABLE_NAME, a.DATA_LENGTH, a.NULLABLE, a.DATA_DEFAULT, a.CHAR_LENGTH FROM ALL_TAB_COLUMNS a, ALL_COL_COMMENTS b WHERE a.TABLE_NAME = b.TABLE_NAME and a.COLUMN_NAME = b.COLUMN_NAME and a.TABLE_NAME = UPPER('" + tblName + "')");
        }

        public static DataSet FetchDataSet(string queryA)
        {
            DataSet ds = new DataSet();
            OracleCommand cmd;
            OracleConnection connOra = new OracleConnection();
            
            try
            {
                // Reopen the connection
                connOra.ConnectionString = MakeConn();
                connOra.Open();
                // Create the command
                cmd = new OracleCommand(queryA);
                cmd.Connection = connOra;
                cmd.CommandType = CommandType.Text;
                // Execute
                OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                ErrMsg(ex.Message + "\n\n" + ex.StackTrace + "\n\n" + ex.Source);
            }
            finally
            {
                connOra.Close();
                connOra.Dispose();
            }

            // Check for data
            if (ds.Tables.Count == 0) { ErrMsg("No tables for the DataSet."); }
            else { if (ds.Tables[0].Rows.Count == 0) { ErrMsg("No records found."); } }
            // Return
            return ds;
        }

        public static string MakeConn()
        {
            // Setup connection string
            if (App.Current.Resources["ID"].ToString().Length == 0) { throw new Exception("No id"); }
            if (App.Current.Resources["PW"].ToString().Length == 0) { throw new Exception("No password"); }
            if (App.Current.Resources["SERVER"].ToString().Length == 0) { throw new Exception("No server"); }
            if (App.Current.Resources["DB"].ToString().Length == 0) { throw new Exception("No database"); }
            return @"User ID=" + App.Current.Resources["ID"] + @";Password=" + App.Current.Resources["PW"] + @";Data Source=" + App.Current.Resources["SERVER"] + @":1521/" + App.Current.Resources["DB"];
        }

        public static void ErrMsg(string msg)
        {
            try
            {
                if (msg.Length == 0) { throw new Exception("No message passed for error"); }
                MessageBox.Show(msg, "ERROR", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                ErrMsg(ex);
            }
        }

        public static void ErrMsg(Exception ex)
        {
            ErrMsg(ex.Message + "\n\n" + ex.StackTrace + "\n\n" + ex.Source);
        }
    }
}
