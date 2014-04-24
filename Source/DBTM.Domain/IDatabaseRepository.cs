using System;
using DBTM.Domain.Entities;

namespace DBTM.Domain
{
    public interface IDatabaseRepository
    {
        Database Load(string path);
        void Save(Database database, string path);
    }
}