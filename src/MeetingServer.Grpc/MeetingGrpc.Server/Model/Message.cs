namespace MeetingGrpc.Server.Model
{
    public class Message
    {
        public Guid Guid { get; }
        public string Content { get; }
        public User User { get; }

        public Message(Guid guid, string content, User user)
        {
            Guid = guid;
            Content = content;
            User = user;
        }
    }
}
