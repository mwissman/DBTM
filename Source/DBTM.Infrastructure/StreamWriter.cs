using System;

namespace DBTM.Infrastructure
{
    public class StreamWriter : IStreamWriter
    {
        public void Write(string path, string contents)
        {
            using(var streamWriter =  new System.IO.StreamWriter(path, false))
            {
                streamWriter.Write(contents);   
            }
        }
    }
}