namespace MeetingGrpc.Server.Model
{
    public class Message
    {
        public Guid Guid { get; }

        public string Content { get; }

        public DateTime DateTime { get; }

        public User User { get; }

        public Message(Guid guid, string content, DateTime dateTime, User user)
        {
            Guid = guid;
            Content = content;
            DateTime = dateTime;
            User = user;
        }
    }
}
