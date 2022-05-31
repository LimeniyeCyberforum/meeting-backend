namespace MeetingGrpc.Server.Model
{
    public class TokenDto
    {
        public string? JwtToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}
