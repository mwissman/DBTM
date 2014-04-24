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
    /// Interaction logic for ConnectionStringBuilder.xaml
    /// </summary>
    public partial class ConnectionStringBuilder : Window, IConnectionStringBuilderView
    {
        public ConnectionStringBuilder()
        {
            InitializeComponent();
        }

        public string ConnectionString
        {
            get { return txtConnectionString.Text; }
        }

        private void btnOkay_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtConnectionString.Text = string.Empty;
            DialogResult = false;
            Close();
        }
    }

    public interface IConnectionStringBuilderView
    {
        string ConnectionString { get; }

    }
}
