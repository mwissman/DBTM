namespace DBTM.Domain.Entities
{
    public class EmptyDatabase : Database
    {
        public EmptyDatabase()
            : base("-- Database Not Set --")
        {
            _isSaved = true;
        }

        public override bool CanFullBuild { get { return false; } }
        public override bool IsEditable { get { return false; } }
        public override int BaseLineVersionNumber
        {
            get
            {
                return int.MaxValue;
            }
        }

        public override bool HasBaseline
        {
            get { return true; }
        }
    }
}