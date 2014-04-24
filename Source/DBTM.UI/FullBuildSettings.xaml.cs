using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DBTM.Application;

namespace DBTM.UI
{
    /// <summary>
    /// Interaction logic for FullBuildSettings.xaml
    /// </summary>
    public partial class FullBuildSettings : Window
    {
        public FullBuildSettings(string databaseName, Window owner)
        {
            Owner = owner;
            InitializeComponent();
            txtDatabaseName.Text = databaseName;
            Settings = new FullBuildDialogResults() {WasCanceled = true};
            
        }

        public FullBuildSettings(string databaseName, Window owner, FullBuildDialogResults settings) : this(databaseName, owner)
        {
            txtDataFilePath.Text = settings.DatabaseFilePath;
            txtPassword.Text = settings.Password;
            txtServerName.Text = settings.DatabaseServer;
            txtUserName.Text = settings.DatabaseUsername;
        }

        public FullBuildDialogResults Settings {get; private set;}

        private void btnOkay_Click(object sender, RoutedEventArgs e)
        {
            bool handled = txtDatabaseName.Text.Length > 0 && txtDataFilePath.Text.Length > 0 &&
                           txtServerName.Text.Length > 0 && txtUserName.Text.Length > 0 && txtPassword.Text.Length > 0;
            e.Handled = handled;
            
            if (handled)
            {
                Settings.DatabaseFilePath = txtDataFilePath.Text;
                Settings.DatabaseName = txtDatabaseName.Text;
                Settings.DatabaseServer = txtServerName.Text;
                Settings.DatabaseUsername = txtUserName.Text;
                Settings.Password = txtPassword.Text;
                Settings.CrossDatabaseNamePrefix = txtCrossDatabaseNamePrefix.Text;
                Settings.WasCanceled = false;
                Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Settings.WasCanceled = true;
        }

        private void btnDataFile_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.ShowDialog();
                txtDataFilePath.Text = folderDialog.SelectedPath;
            }
        }
    }
}
