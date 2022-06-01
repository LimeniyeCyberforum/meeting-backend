using MeetingGrpc.Server.Model;

namespace MeetingGrpc.Server.Repositories
{
    public interface IRepository<T>
    {
        void Add(T obj);
        void Remove(T obj);
        IEnumerable<T> GetAll();
    }
}
