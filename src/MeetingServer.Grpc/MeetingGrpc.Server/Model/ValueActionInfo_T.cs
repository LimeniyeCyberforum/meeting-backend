namespace MeetingGrpc.Server.Model
{
    public class ValueActionInfo<T>
    {
        public T Value { get; }
        public DateTime DateTime { get; }

        public ValueActionInfo(T value, DateTime dateTime)
        {
            Value = value;
            DateTime = dateTime;
        }
    }
}
