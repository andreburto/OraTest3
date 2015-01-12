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

namespace OraTest3
{
    /// <summary>
    /// Interaction logic for ScratchPad.xaml
    /// </summary>
    public partial class ScratchPad : Window
    {
        public ScratchPad()
        {
            InitializeComponent();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (txtScratch.Text.Length == 0) { return; }
            string qry = txtScratch.Text;
            if (qry.Substring(0, 6).ToUpper() != "SELECT")
            {
                DbStuff.ErrMsg("You can only perform SELECT queries.");
                return;
            }
            if (qry.Substring(qry.Length - 1, 1) == ";")
            {
                qry = qry.Substring(0, qry.Length - 1);
            }
            DataSet ds = DbStuff.FetchDataSet(qry);
            if (ds.Tables.Count == 0) { return; }
            if (ds.Tables[0].Rows.Count == 0) { return; }
            dgScratch.ItemsSource = new DataView(ds.Tables[0]);
        }
    }
}
