using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Oracle.DataAccess.Client;
using OraTest3;

namespace OraTest3
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {

        public string connStr;

        public Login()
        {
            InitializeComponent();

            txtID.Text = App.Current.Resources["ID"].ToString();
            txtPW.Text = App.Current.Resources["PW"].ToString();
            txtServer.Text = App.Current.Resources["SERVER"].ToString();
            txtDB.Text = App.Current.Resources["DB"].ToString();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            SetGlobals();

            OracleConnection connTest = new OracleConnection();
            bool yesno = true;
            try
            {
                // Reopen the connection
                connTest.ConnectionString = DbStuff.MakeConn();
                connTest.Open();
            }
            catch (Exception ex)
            {
                yesno = false;
                DbStuff.ErrMsg(this.connStr + "\n\n" + ex.Message + "\n\n" + ex.StackTrace + "\n\n" + ex.Source);
            }
            finally
            {
                if (connTest.State != ConnectionState.Closed) { connTest.Close(); }
                connTest.Dispose();
            }
            if (yesno == true) { MessageBox.Show("Succesful connection", "Success"); }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SetGlobals();

            try
            {
                int count = 0;
                Creds c = new Creds();
                if (txtPW.Text.Length > 0) { c.Pw = txtPW.Text; }
                else { c.Pw = "nada"; }
                if (txtID.Text.Length > 0) { c.Id = txtID.Text; count++; }
                if (txtServer.Text.Length > 0) { c.Server = txtServer.Text; count++; }
                if (txtDB.Text.Length > 0) { c.Database = txtDB.Text; count++; }
                if (count != 3) { throw new Exception("Missing field."); }
                c.Save();
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                DbStuff.ErrMsg(ex.Message);
            }
        }

        private void SetGlobals()
        {
            if (txtPW.Text.Length > 0) { App.Current.Resources["PW"] = txtPW.Text; }
            else { App.Current.Resources["PW"] = "nada"; }
            if (txtID.Text.Length > 0) { App.Current.Resources["ID"] = txtID.Text; }
            if (txtServer.Text.Length > 0) { App.Current.Resources["SERVER"] = txtServer.Text; }
            if (txtDB.Text.Length > 0) { App.Current.Resources["DB"] = txtDB.Text; }
        }

        private void txtID_GotFocus(object sender, RoutedEventArgs e)
        {
            txtID.SelectAll();
        }

        private void txtPW_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPW.SelectAll();
        }

        private void txtServer_GotFocus(object sender, RoutedEventArgs e)
        {
            txtServer.SelectAll();
        }

        private void txtDB_GotFocus(object sender, RoutedEventArgs e)
        {
            txtDB.SelectAll();
        }
    }
}
