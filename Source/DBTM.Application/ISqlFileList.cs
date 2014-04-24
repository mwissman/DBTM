using System.Collections.Generic;

namespace DBTM.Application
{
    public interface ISqlFileList
    {
        IList<ISqlFile> Files { get; }
    }
}