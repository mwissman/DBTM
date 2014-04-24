using System;
using DBTM.Application.ViewModels;
using DBTM.Application.Views;
using DBTM.Domain;

namespace DBTM.Application.Commands
{
    public class SaveDatabaseCommand : BaseSaveCommand
    {
        private readonly IDatabaseRepository _databaseRepository;

        public SaveDatabaseCommand(IMainWindowView view, IDatabaseSchemaViewModel viewModel, IDatabaseRepository databaseRepository)
            : base(view,viewModel)
        {
            _databaseRepository = databaseRepository;
        }

        public override void Execute(object parameter)
        {
            string databaseFilePath = _viewModel.Settings.DatabaseFilePath;

            if (string.IsNullOrEmpty(databaseFilePath))
            {
                AskForFilePathResult result = AskUserForFilePathAndUpdateViewModel();
                if (!result.DidUserProvidePath)
                {
                    return;
                }

                databaseFilePath = result.FilePath;
            } 

            _databaseRepository.Save(_viewModel.Database, databaseFilePath);
        }
    }
}