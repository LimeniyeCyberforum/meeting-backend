using MeetingGrpc.Protos;
using MeetingGrpc.Server.Model;
using System.ComponentModel.Composition;

namespace MeetingGrpc.Server.Repositories
{
    public class ChatRepository : IRepository<Guid, Message>
    {
        private readonly Dictionary<Guid, Message> localStorage = new Dictionary<Guid, Message>(); // dummy on memory storage

        public void Add(Guid key, Message message)
        {
            localStorage.Add(key, message);
        }

        public void Remove(Guid key)
        {
            throw new NotImplementedException();
        }

        public void Update(Guid key, Message obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Message> GetAll()
        {
            return localStorage.Values.AsEnumerable();
        }
    }
}
