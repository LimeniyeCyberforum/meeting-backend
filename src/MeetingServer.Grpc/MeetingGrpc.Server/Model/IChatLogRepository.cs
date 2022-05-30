using MeetingGrpc.Protos;

namespace GrpsServer.Model
{
    public interface IChatLogRepository
    {
        void Add(MessageFromLobby chatLog);
        IEnumerable<MessageFromLobby> GetAll();
    }
}
