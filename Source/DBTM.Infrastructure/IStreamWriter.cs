using System;

namespace DBTM.Infrastructure
{
    public interface IStreamWriter
    {
        void Write(string path, string contents);
    }

}