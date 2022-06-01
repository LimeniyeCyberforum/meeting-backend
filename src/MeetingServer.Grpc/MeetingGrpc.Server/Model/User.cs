namespace MeetingGrpc.Server.Model
{
    public class User
    {
        public Guid UserGuid { get; }
        public string? Name { get; }
        public bool IsOnline { get; }

        public User(Guid userGuid, string? name, bool isOnline)
        {
            UserGuid = userGuid; Name = name; IsOnline = isOnline;
        }
    }
}
