using System;
using System.Windows.Input;
using DBTM.Application.ViewModels;
using DBTM.Application.Views;
using DBTM.Domain;

namespace DBTM.Application.Commands
{
    public class SaveDatabaseAsCommand : BaseSaveCommand
    {
        private IDatabaseRepository _databaseRepository;

        public SaveDatabaseAsCommand(IMainWindowView view, IDatabaseSchemaViewModel viewModel ,IDatabaseRepository databaseRepository) : base(view,viewModel)
        {  
            _databaseRepository = databaseRepository;  
        }

        public override void Execute(object parameter)
        {
            var result = AskUserForFilePathAndUpdateViewModel();
            if (result.DidUserProvidePath)
            {
                _databaseRepository.Save(_viewModel.Database,result.FilePath);
            }
        }
    }
}