using DBTM.Domain.Entities;

namespace DBTM.Application.Views
{
    public interface IMainWindowView
    {
        SetConnectionStringResult AskUserForConnectionString();
        string AskUserForNewDatabasename();
        string AskUserForNewFilePath();
        bool AskUserChangesShouldBeAbandoned();
        FullBuildDialogResults AskUserForFullBuildParameters(string databaseName);
        FullBuildDialogResults AskUserForFullBuildParameters(string databaseName, FullBuildDialogResults settings);

        void Initialize();
        void Show();
        
        void DisplayError(string message);
        void ShowBuildResultMessage(string buildMessage, string databaseName);

        void UpdateSelectedVersion(DatabaseVersion databaseVersion);
        void UpdateSelectedStatement(SqlStatement sqlStatement);

        SqlStatementType SelectedSqlStatementType { get; }
        
    }
}