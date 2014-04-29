using System;
using Autofac;
using DBTM.Cmd.Arguments;
using DBTM.Cmd.Runners;

namespace DBTM.Cmd
{
    class Program
    {
        private const string EXPECTED_COMMAND_LINE_USAGE = @"
+*****************************************************************************************+
*                                                                                         *
* DBTM Command Line Usage                                                                 *
*                                                                                         *
* DBTM.exe [-command=FullBuild|CreateVersion|CompileScripts|RunDirectory] [options]       *
*                                                                                         * 
*    FullBuild Options                                                                    *
*       -databaseName=dbName                                                              * 
*       -server=localhost                                                                 *
*       -userName=username                                                               *
*       -password=password                                                                *
*       -dataFilePath=c:\somewhere\datafile.mdf                                           *
*       -databaseFilePath=c:\somewhere\somedatabase.dbschema                              *
*       [-crossDatabaseNamePrefix=DBPrefix_]                                              *
*                                                                                         *
*    CreateVersion Options                                                                *
*       -databaseFilePath=c:\somewhere\somedatabase.dbschema                              *
*                                                                                         *
*    CompileScripts Options                                                               *
*       -databaseFilePath=c:\somewhere\somedatabase.dbschema                              *
*       -compiledScriptDirectory=c:\somewhere\sql\                                        *
*                                                                                         *
*    RunDirectory Options                                                                 *
*       -databaseName=dbName                                                              * 
*       -server=localhost                                                                 *
*       -userName=username                                                               *
*       -password=password                                                                *
*       -scriptPath=c:\some\directory\path                                                *
*                                                                                         * 
+*****************************************************************************************+
";

        static void Main(string[] args)
        {
            int exitCode = 1;
            string message = EXPECTED_COMMAND_LINE_USAGE;

            var builder = new ContainerBuilder();
            builder.RegisterModule(new CommandModule(args));

            using (IContainer applicationContainer = builder.Build())
            {
                using (var lifetime = applicationContainer.BeginLifetimeScope())
                {

                    var arguments = lifetime.Resolve<IArguments>();
                    
                    if (arguments.HasRequiredArguments)
                    {
                        var runner = lifetime.Resolve(GetRunnerType(arguments));

                        try
                        {
                            var buildResult = InvokeRun(runner, arguments);

                            message = buildResult.Message;
                            
                            if (buildResult.Succeeded)
                            {
                                exitCode = 0;
                            }
                        }
                        catch (Exception ex)
                        {
                            message = ex.ToString();
                            exitCode = 1;
                        }
                    }
                }
            }

            Console.WriteLine(message);
            Environment.Exit(exitCode);
        }

        private static Type GetRunnerType(IArguments arguments)
        {
            Type appRunnerOpenGenericType = typeof(IApplicationRunner<>);
            Type[] argumentsType = { arguments.GetType().GetInterfaces()[0] };
            Type appRunnerSpecificType = appRunnerOpenGenericType.MakeGenericType(argumentsType);
            
            return appRunnerSpecificType;
        }

        private static ApplicationRunnerResult InvokeRun(object runner, IArguments arguments)
        {
            return runner.GetType().GetMethod("Run").Invoke(runner, new object[] {arguments}) as ApplicationRunnerResult;
        }
    }
}
