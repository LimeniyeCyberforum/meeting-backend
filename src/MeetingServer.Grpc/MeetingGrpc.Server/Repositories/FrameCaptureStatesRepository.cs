using MeetingGrpc.Server.Model;

namespace MeetingGrpc.Server.Repositories
{
    public class FrameCaptureStatesRepository : IRepository<ValueActionInfo<CaptureFrameInfo>>
    {
        private readonly List<ValueActionInfo<CaptureFrameInfo>> localStorage = new List<ValueActionInfo<CaptureFrameInfo>>(); // dummy on memory storage

        public void Add(ValueActionInfo<CaptureFrameInfo> message)
        {
            localStorage.Add(message);
        }

        public void Remove(ValueActionInfo<CaptureFrameInfo> cameraCapture)
        {
            localStorage.Remove(cameraCapture);
        }

        public IEnumerable<ValueActionInfo<CaptureFrameInfo>> GetAll()
        {
            return localStorage.AsReadOnly();
        }

        public void Update(ValueActionInfo<CaptureFrameInfo> obj)
        {
            throw new NotImplementedException();
        }
    }
}
