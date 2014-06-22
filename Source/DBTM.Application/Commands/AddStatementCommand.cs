using System;
using System.Windows.Input;
using DBTM.Application.Views;
using DBTM.Domain.Entities;

namespace DBTM.Application.Commands
{
    public class AddStatementCommand : ICommand
    {
        private readonly IMainWindowView _view;

        public AddStatementCommand(IMainWindowView view)
        {
            _view = view;
        }

        public void Execute(object parameter)
        {
            DatabaseVersion databaseVersion = parameter as DatabaseVersion;

            if (databaseVersion != null)
            {
                var statementToAdd = new SqlStatement(string.Empty, string.Empty, string.Empty) { IsEditable = true};

                SqlStatementCollection statements = GetStatements(databaseVersion);

                statements.Add(statementToAdd);
                statements.SetCanMoveUpDownOnAllStatements();
                
                _view.UpdateSelectedStatement(statementToAdd);
            }
        }

        private SqlStatementCollection GetStatements(DatabaseVersion databaseVersion)
        {
            SqlStatementCollection statements;
            switch (_view.SelectedSqlStatementType)
            {
                case SqlStatementType.PreDeployment:
                    statements = databaseVersion.PreDeploymentStatements;
                    break;
                case SqlStatementType.PostDeployment:
                    statements = databaseVersion.PostDeploymentStatements;
                    break;
                default:
                    throw new ArgumentException("Unknown Statement Type");
            }
            return statements;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}