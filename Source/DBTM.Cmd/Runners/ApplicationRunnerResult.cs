namespace DBTM.Cmd.Runners
{
    public class ApplicationRunnerResult
    {
        public ApplicationRunnerResult(bool succeeded, string message)
        {
            Succeeded = succeeded;
            Message = message;
        }

        public bool Succeeded { get; private set; }
        public string Message { get; private set; }

        public bool Equals(ApplicationRunnerResult other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Succeeded.Equals(Succeeded) && Equals(other.Message, Message);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ApplicationRunnerResult)) return false;
            return Equals((ApplicationRunnerResult) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Succeeded.GetHashCode()*397) ^ (Message != null ? Message.GetHashCode() : 0);
            }
        }
    }
}