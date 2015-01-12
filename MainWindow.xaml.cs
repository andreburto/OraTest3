using System;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Oracle.DataAccess.Client;
using OraTest3;

namespace OraTest3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Creds cred = new Creds();
        public List<OraTest3.TablesChoices> tbls = new List<OraTest3.TablesChoices>();
        public string connStr;
        public delegate void UpdateCombo();

        // EVENTS
        public MainWindow()
        {
            InitializeComponent();

            if (cred.IsFile)
            {
                cred.Load();
                App.Current.Resources["ID"] = cred.Id;
                App.Current.Resources["PW"] = cred.Pw;
                App.Current.Resources["SERVER"] = cred.Server;
                App.Current.Resources["DB"] = cred.Database;
            }
            else
            {
                Login l = new Login();
                l.ShowDialog();
                if (l.DialogResult == false)
                {
                    this.Close();
                    return;
                }
                else
                {
                    cred.Load();
                    App.Current.Resources["ID"] = cred.Id;
                    App.Current.Resources["PW"] = cred.Pw;
                    App.Current.Resources["SERVER"] = cred.Server;
                    App.Current.Resources["DB"] = cred.Database;
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            cred.Save();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text.Length == 0) { return; }
            DataSet ds = DbStuff.LoadTableData(txtSearch.Text);
            if (ds.Tables.Count == 0) { return; }
            if (ds.Tables[0].Rows.Count == 0) { return; }
            dgTables.ItemsSource = new DataView(ds.Tables[0]);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Login l = new Login();
            l.ShowDialog();
            if (l.DialogResult == true)
            {
                cred.Load();
                App.Current.Resources["ID"] = cred.Id;
                App.Current.Resources["PW"] = cred.Pw;
                App.Current.Resources["SERVER"] = cred.Server;
                App.Current.Resources["DB"] = cred.Database;
            }
        }

        private void dgTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // The program crashes if the text from a field in dgTables is selected and I go to another table.
            // For some reason CurRow does not get initialized.
            // This throws an exception, pops up an error, and saves the program.
            try
            {
                DataRowView CurRow = (DataRowView)dgTables.SelectedItem;
                if (CurRow["TABLE_NAME"].ToString().Length == 0) { return; }
                DataSet ds = DbStuff.LoadColumnData(CurRow["TABLE_NAME"].ToString());
                if (ds.Tables.Count == 0) { return; }
                if (ds.Tables[0].Rows.Count == 0) { return; }
                dgColumns.ItemsSource = new DataView(ds.Tables[0]);
            }
            catch (Exception ex)
            {
                DbStuff.ErrMsg(ex);
            }
        }

        private void btnFetch_Click(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text.Length == 0) { return; }
            // Get column information
            DataSet ds = DbStuff.LoadColumnData(txtSearch.Text);
            if (ds.Tables.Count == 0) { return; }
            if (ds.Tables[0].Rows.Count == 0) { return; }
            dgColumns.ItemsSource = new DataView(ds.Tables[0]);
            // Get table comments
            DataSet ds2 = DbStuff.FetchDataSet("SELECT TABLE_NAME, TABLE_TYPE, COMMENTS, OWNER FROM ALL_TAB_COMMENTS WHERE UPPER(TABLE_NAME) = UPPER('" + txtSearch.Text + "')");
            if (ds2.Tables.Count == 0) { return; }
            if (ds2.Tables[0].Rows.Count == 0) { return; }
            dgTables.ItemsSource = new DataView(ds2.Tables[0]);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            ScratchPad sp = new ScratchPad();
            sp.ShowInTaskbar = false;
            sp.ShowDialog();
        }
    }
}
