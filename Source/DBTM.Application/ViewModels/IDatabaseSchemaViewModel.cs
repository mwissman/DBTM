using System.ComponentModel;
using System.Windows.Input;
using DBTM.Domain.Entities;

namespace DBTM.Application.ViewModels
{
    public interface IDatabaseSchemaViewModel : INotifyPropertyChanged
    {
        Database Database { get; set; }
        IApplicationSettings Settings { get; set; }
        ICommand OpenDatabaseCommand { get; }
        ICommand NewDatabaseCommand { get; }
        ICommand CloseWindowCommand { get; }
        ICommand SaveDatabaseAsCommand { get; }
        ICommand SaveDatabaseCommand { get; }
        ICommand MoveStatementUpCommand { get; }
        ICommand MoveStatementDownCommand { get; }
        ICommand SetConnectionStringCommand { get; }
        ICommand FullBuildCommand { get; }
        ICommand AddVersionCommand { get; }
        ICommand AddStatementCommand { get; }
        ICommand CompileVersionCommand { get; }
        ICommand CompileAllCommand { get; }
        ICommand InitializeViewCommand { get; }

    }
}