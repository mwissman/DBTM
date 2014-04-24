using System.ComponentModel;

namespace DBTM.Application
{
    public interface IApplicationSettings : INotifyPropertyChanged
    {
        string ConnectionString { get; set; }
        string DatabaseFilePath { get; set; }
    }
}