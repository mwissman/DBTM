namespace DBTM.Domain.Entities
{
    public enum SqlStatementType : byte
    {
        PreDeployment = 1,
        Backfill = 2,
        PostDeployment = 3,
    }
}