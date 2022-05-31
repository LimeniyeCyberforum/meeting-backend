using MeetingGrpc.Server.Model;

namespace MeetingGrpc.Server.Repositories
{
    public interface IRepository<T>
    {
        void Add(T obj);
        IEnumerable<T> GetAll();
    }
}
