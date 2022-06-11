using MeetingGrpc.Server.Model;

namespace MeetingGrpc.Server.Repositories
{
    public class UsersRepository : IRepository<Guid, User>
    {
        private readonly Dictionary<Guid, User> localStorage = new Dictionary<Guid, User>(); // dummy on memory storage

        public void Add(Guid key, User user)
        {
            localStorage.Add(key, user);
        }

        public void Remove(Guid key)
        {
            throw new NotImplementedException();
        }

        public void Update(Guid key, User obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            return localStorage.Values.AsEnumerable();
        }
    }
}
