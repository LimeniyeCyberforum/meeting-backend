using MeetingGrpc.Protos;
using MeetingGrpc.Server.Model;
using System.ComponentModel.Composition;

namespace MeetingGrpc.Server.Repositories
{
    public class ChatRepository : IRepository<Message>
    {
        private readonly List<Message> localStorage = new List<Message>(); // dummy on memory storage

        public void Add(Message message)
        {
            localStorage.Add(message);
        }

        public IEnumerable<Message> GetAll()
        {
            return localStorage.AsReadOnly();
        }

        public void Remove(Message obj)
        {
            throw new NotImplementedException();
        }
    }
}
