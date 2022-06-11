using MeetingGrpc.Server.Model;

namespace MeetingGrpc.Server.Repositories
{
    public interface IRepository<TKey, TValue>
    {
        void Add(TKey key, TValue obj);
        void Remove(TKey key);
        void Update(TKey key, TValue value);
        IEnumerable<TValue> GetAll();
    }
}
