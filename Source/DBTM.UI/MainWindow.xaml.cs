using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using DBTM.Application;
using DBTM.Application.Commands;
using DBTM.Application.ViewModels;
using DBTM.Application.Views;
using DBTM.Domain.Entities;
using ICSharpCode.AvalonEdit.Highlighting;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;
using FileDialog = Microsoft.Win32.FileDialog;
using ListBox = System.Windows.Controls.ListBox;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace DBTM.UI
{
    public partial class MainWindow : Window, IMainWindowView, ICanOpenDatabasesView, ICompileVersionView
    {
        private readonly OpenFileDialog _openFileDialog;
        private readonly SaveFileDialog _saveFileDialog;
        private readonly IDatabaseSchemaViewModel _viewModel;

        public MainWindow(OpenFileDialog openFileDialog, SaveFileDialog saveFileDialog, IDatabaseSchemaViewModel viewModel)
        {
            _openFileDialog = openFileDialog;
            _saveFileDialog = saveFileDialog;
            _viewModel = viewModel;

            DataContext = viewModel;
        }



        public SetConnectionStringResult AskUserForConnectionString()
        {
            var builder = new ConnectionStringBuilder();
            var dialogResult = builder.ShowDialog();

            return new SetConnectionStringResult(!dialogResult.Value, builder.ConnectionString);
        }

        public string AskUserForNewDatabasename()
        {
            var dialog = new DatabaseNameDialog();
            dialog.ShowDialog();

            return dialog.DatabaseName;
        }

        public string AskUserForNewFilePath()
        {
            return GetFilePathFromDialog(_saveFileDialog, "DB Schema (*.dbschema)|*.dbschema", ".dbschema");
        }

        public string AskUserForCrossDatabasePrefix()
        {
            var dialog = new CrossDatabaseNamePrefixDialog();
            dialog.ShowDialog();

            return dialog.CrossDatabaseNamePrefix;
        }

        public string AskUserForCompiledSqlFolderPath()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;
                dialog.Description = @"Please Select the folder to save the Compiled Sql Scripts";
                dialog.ShowDialog();

                return dialog.SelectedPath;
            }
        }

        public void ShowBuildResultMessage(string buildMessage, string databaseName)
        {
            MessageBox.Show(buildMessage, databaseName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        public void UpdateSelectedVersion(DatabaseVersion databaseVersion)
        {
            var indexOfNewVersion = lbVersions.Items.IndexOf(databaseVersion);
            if (indexOfNewVersion != -1)
            {
                lbVersions.SelectedIndex = indexOfNewVersion;
            }
        }

        public void UpdateSelectedStatement(SqlStatement sqlStatement)
        {
            ListBox statementsListBox = GetSelectedStatementTypeListbox();

            var indexOfNewStatement = statementsListBox.Items.IndexOf(sqlStatement);
            if (indexOfNewStatement != -1)
            {
                statementsListBox.SelectedIndex = indexOfNewStatement;
            }
        }

        public SqlStatementType SelectedSqlStatementType
        {
            get { return ((SqlStatementTabItem) tcStatementTypes.SelectedItem).StatementType; }
        }

        public void DisplayStatusMessage(string message)
        {
            tbStatusBarMessage.Text = message;
        }

        public FullBuildDialogResults AskUserForFullBuildParameters(string databaseName)
        {
            var settingsDialog = new FullBuildSettings(databaseName, this);
            settingsDialog.ShowDialog();
            return settingsDialog.Settings;
        }

        public FullBuildDialogResults AskUserForFullBuildParameters(string databaseName,FullBuildDialogResults settings)
        {
            var settingsDialog = new FullBuildSettings(databaseName, this, settings);
            settingsDialog.ShowDialog();
            return settingsDialog.Settings;
        }

        public string OpenFile()
        {
            return GetFilePathFromDialog(_openFileDialog, "DB Schema (*.dbschema)|*.dbschema", ".dbschema");
        }

        public void Initialize()
        {
            InitializeComponent();

            ExecuteCommand(_viewModel.InitializeViewCommand,null,null);

            InputBindings.Add(new KeyBinding(_viewModel.SaveDatabaseCommand, Key.S, ModifierKeys.Control));
            InputBindings.Add(new KeyBinding(_viewModel.NewDatabaseCommand, Key.N, ModifierKeys.Control));
            InputBindings.Add(new KeyBinding(_viewModel.OpenDatabaseCommand, Key.O, ModifierKeys.Control));
            InputBindings.Add(new KeyBinding(_viewModel.SaveDatabaseAsCommand, Key.F12, ModifierKeys.None));


            RegisterSyntaxHighlighting();
            HighlightingManager.Instance.GetDefinition("C#");

            txtUpgradeSqlStatement.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("DBTM TSQL");
            txtRollbackSqlStatement.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("DBTM TSQL");
        }

        private void RegisterSyntaxHighlighting()
        {
            IHighlightingDefinition customHighlighting;
            using (Stream s = typeof(MainWindow).Assembly.GetManifestResourceStream("DBTM.UI.DBTM_TSQL.xshd"))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not find embedded resource");
                }
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("DBTM TSQL", new string[] { ".sql" }, customHighlighting);
        }

        private ListBox GetSelectedStatementTypeListbox()
        {
            ListBox statementsListBox;

            switch (SelectedSqlStatementType)
            {
                case SqlStatementType.PreDeployment:
                    statementsListBox = lbPreDeploymentStatements;
                    break;
                case SqlStatementType.Backfill:
                    statementsListBox = lbBackFillStatements;
                    break;
                case SqlStatementType.PostDeployment:
                    statementsListBox = lbPostDeploymentStatements;
                    break;
                default:
                    throw new ArgumentException("Unknown Statement Type");
            }

            return statementsListBox;
        }

        private static string GetFilePathFromDialog(FileDialog fileDialog, string filter, string extension)
        {
            fileDialog.Filter = filter;
            fileDialog.DefaultExt = extension;

            bool? dialog = fileDialog.ShowDialog();
            string filePath = dialog.Value ? fileDialog.FileName : string.Empty;
            fileDialog.Reset();
            return filePath;
        }

        private void MainWindowDrop(object sender, DragEventArgs e)
        {
            string[] droppedFilePaths =e.Data.GetData(DataFormats.FileDrop, true) as string[];

            foreach (string droppedFilePath in droppedFilePaths)
            {
                if (new FileInfo(droppedFilePath).Extension.EndsWith("dbschema",true, CultureInfo.CurrentUICulture))
                {
                    ICommand openDatabaseCommand = _viewModel.OpenDatabaseCommand;
                    ExecuteCommand(openDatabaseCommand,null,droppedFilePath);
                    return;
                }
            }
        }

        private void ExecuteCommand(ICommand command, object canExecuteParameter, object executeParameter)
        {
            if (command.CanExecute(canExecuteParameter))
            {
                command.Execute(executeParameter);
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var closeWindowCommand = _viewModel.CloseWindowCommand as CloseWindowCommand;
            ExecuteCommand(closeWindowCommand,null,null);
            e.Cancel = !closeWindowCommand.CanCloseWindow;
        }

        public bool AskUserChangesShouldBeAbandoned()
        {
            return MessageBox.Show("There are unsaved changes. Do you want to abandon these changes?", 
                "Database Transition Manager", 
                MessageBoxButton.YesNo,MessageBoxImage.Exclamation) == MessageBoxResult.Yes;
        }

        public void DisplayError(string message)
        {
            MessageBox.Show(message, "Database Transition Manager",MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

      
    }
}