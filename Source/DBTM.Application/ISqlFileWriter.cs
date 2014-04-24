using DBTM.Domain.Entities;

namespace DBTM.Application
{
    public interface ISqlFileWriter
    {
        void Write(ICompiledSql compiledSql, string upgradeFilePath, string rollbackFilePath);
    }
}