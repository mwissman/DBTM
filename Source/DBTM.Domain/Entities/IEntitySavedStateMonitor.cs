namespace DBTM.Domain.Entities
{
    public interface IEntitySavedStateMonitor
    {
        bool IsSaved { get; }
        void MarkAsSaved();
    }
}