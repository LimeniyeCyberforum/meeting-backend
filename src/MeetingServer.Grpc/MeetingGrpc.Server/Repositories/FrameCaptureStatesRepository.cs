using MeetingGrpc.Server.Model;

namespace MeetingGrpc.Server.Repositories
{
    public class FrameCaptureStatesRepository : IRepository<CaptureFrameInfo>
    {
        private readonly List<CaptureFrameInfo> localStorage = new List<CaptureFrameInfo>(); // dummy on memory storage

        public void Add(CaptureFrameInfo message)
        {
            localStorage.Add(message);
        }

        public void Remove(CaptureFrameInfo cameraCapture)
        {
            localStorage.Remove(cameraCapture);
        }

        public IEnumerable<CaptureFrameInfo> GetAll()
        {
            return localStorage.AsReadOnly();
        }
    }
}
