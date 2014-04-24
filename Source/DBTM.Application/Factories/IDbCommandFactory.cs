using System;
using System.Data;

namespace DBTM.Application.Factories
{
    public interface IDbCommandFactory
    {
        IDbCommand Create(string sql);
    }
}