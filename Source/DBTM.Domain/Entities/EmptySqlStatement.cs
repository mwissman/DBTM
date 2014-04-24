namespace DBTM.Domain.Entities
{
    public class EmptySqlStatement : SqlStatement
    {
        public EmptySqlStatement() : base(string.Empty, string.Empty, string.Empty)
        {
              
        }

        public override bool IsEditable
        {
            get
            {
                return false;
            }
        }

    }
}