using System;
using DBTM.Application;

namespace DBTM.UI
{
    public class ArgumentsProvider : IArgumentsProvider
    {
        private readonly string[] _args;

        public ArgumentsProvider(string[] args)
        {
            _args = args;
        }

        public bool HasFile
        {
            get
            {
                return (_args.Length > 0) && (!_args[0].StartsWith("-"));
            }
        }

        public string FilePath
        {
            get
            {
                return HasFile ? _args[0] : null;
            }
        }
    }
}