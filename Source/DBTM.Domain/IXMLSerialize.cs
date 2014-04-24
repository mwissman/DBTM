using System;
using System.IO;
using DBTM.Domain.Entities;

namespace DBTM.Domain
{
    public interface IXMLSerializer
    {
        void Serialize<T>(string path, T objectToSerialize);
        T Deserialize<T>(string path);
    }
}