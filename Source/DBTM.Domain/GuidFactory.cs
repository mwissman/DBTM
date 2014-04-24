using System;

namespace DBTM.Domain
{
    public class GuidFactory:IGuidFactory
    {
        public Guid Create()
        {
            return Guid.NewGuid();
        }
    }
}