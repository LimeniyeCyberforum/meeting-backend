using MeetingGrpc.Server.Model;

namespace MeetingGrpc.Server.Repositories
{
    public class FrameCaptureStatesRepository : IRepository<Guid, ValueActionInfo<CaptureFrameInfo>>
    {
        private readonly Dictionary<Guid, ValueActionInfo<CaptureFrameInfo>> localStorage = new Dictionary<Guid, ValueActionInfo<CaptureFrameInfo>>(); // dummy on memory storage

        public void Add(Guid key, ValueActionInfo<CaptureFrameInfo> message)
        {
            localStorage.Add(key, message);
        }

        public void Remove(Guid key)
        {
            localStorage.Remove(key);
        }

        public void Update(Guid key, ValueActionInfo<CaptureFrameInfo> newValue)
        {
            if (localStorage.ContainsKey(key))
            {
                localStorage[key] = newValue;
            }
            else
            {
                // TODO : Should be loggs 
            }
        }

        public IEnumerable<ValueActionInfo<CaptureFrameInfo>> GetAll()
        {
            return localStorage.Values.AsEnumerable();
        }
    }
}
