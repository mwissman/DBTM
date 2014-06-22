using System;

namespace DBTM.Cmd.Arguments
{
    public class ArgumentsFactory : IArgumentsFactory
    {
        private const string COMMAND_ARGUMENT = "COMMAND";
        private const string CREATE_VERSION_COMMAND = "CREATEVERSION";
        private const string COMPILE_SCRIPTS_COMMAND = "COMPILESCRIPTS";
        private const string FULLBUILD_COMMAND = "FULLBUILD";

        public IArguments Create(string[] arguments)
        {
            if (arguments.HasArgumentValue(COMMAND_ARGUMENT)) 
            {
                string command = arguments.GetArgumentValue(COMMAND_ARGUMENT);

                if (command.Equals(CREATE_VERSION_COMMAND, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new CreateVersionArguments(arguments);
                }
                
                if (command.Equals(FULLBUILD_COMMAND, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new FullBuildArguments(arguments);
                }

                if (command.Equals(COMPILE_SCRIPTS_COMMAND, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new CompileScriptsArguments(arguments);
                }
                throw new ArgumentException("Unknown Command: {0}", command);
            }


            return new FullBuildArguments(arguments);
        }
    }
}