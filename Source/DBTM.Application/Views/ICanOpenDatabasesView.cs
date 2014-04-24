namespace DBTM.Application.Views
{
    public interface ICanOpenDatabasesView
    {
        string OpenFile();
        bool AskUserChangesShouldBeAbandoned();
    }
}