using System;
using System.Linq;
using DBTM.Domain.Entities;

namespace DBTM.Domain
{
    public class DatabaseRepository : IDatabaseRepository
    {
        private readonly IXMLSerializer _xmlSerializer;

        public DatabaseRepository(IXMLSerializer xmlSerializer)
        {
            _xmlSerializer = xmlSerializer;
            
        }

        public Database Load(string path)
        {
            var database = _xmlSerializer.Deserialize<Database>(path);
           
            database.Versions.Last().IsEditable = true;

            database.Versions.ForEach(v => v.PreDeploymentStatements.SetCanMoveUpDownOnAllStatements());
            database.Versions.ForEach(v => v.BackfillStatements.SetCanMoveUpDownOnAllStatements());
            database.Versions.ForEach(v => v.PostDeploymentStatements.SetCanMoveUpDownOnAllStatements());

            database.MarkAsSaved();

            return database;
        }

        public void Save(Database database, string path)
        {
            _xmlSerializer.Serialize(path, database);
            database.MarkAsSaved();
        }
    }
}