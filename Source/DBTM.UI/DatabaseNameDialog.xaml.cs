using System;
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

namespace DBTM.UI
{
    /// <summary>
    /// Interaction logic for DatabaseNameDialog.xaml
    /// </summary>
    public partial class DatabaseNameDialog : Window
    {
        public DatabaseNameDialog()
        {
            InitializeComponent();
            txtDatabseName.Focus();
        }

        private void btnOkay_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public string DatabaseName
        {
            get
            {
                return txtDatabseName.Text;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtDatabseName.Text = string.Empty;
            this.Close();
        }
    }
}
