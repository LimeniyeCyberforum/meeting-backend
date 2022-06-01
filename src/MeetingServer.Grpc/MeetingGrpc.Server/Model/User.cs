namespace MeetingGrpc.Server.Model
{
    public class User
    {
        public Guid UserGuid { get; }
        public string? Name { get; }
        public bool IsOnline { get; }
        public Token Token { get; }

        public User(Guid userGuid, string? name, bool isOnline, Token token)
        {
            UserGuid = userGuid; Name = name; IsOnline = isOnline; Token = token;
        }
    }
}
