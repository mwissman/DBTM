using DBTM.Cmd.Arguments;

namespace DBTM.Cmd.Runners
{
    public interface IApplicationRunner<T> where T : IArguments
    {
        ApplicationRunnerResult Run(T arguments);
    }
}