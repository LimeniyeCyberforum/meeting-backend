using MeetingGrpc.Server.Model;

namespace MeetingGrpc.Server.Repositories
{
    public class UsersRepository : IRepository<User>
    {
        private readonly List<User> localStorage = new List<User>(); // dummy on memory storage

        public void Add(User user)
        {
            localStorage.Add(user);
        }

        public IEnumerable<User> GetAll()
        {
            return localStorage.AsReadOnly();
        }

        public void Remove(User obj)
        {
            throw new NotImplementedException();
        }

        public void Update(User obj)
        {
            throw new NotImplementedException();
        }
    }
}
