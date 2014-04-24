namespace DBTM.Application.Views
{
    public interface ICompileVersionView : IDisplayStatusMessagesView
    {
        string AskUserForCrossDatabasePrefix();
        string AskUserForCompiledSqlFolderPath();
    }
}