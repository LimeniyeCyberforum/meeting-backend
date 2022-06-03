using MeetingGrpc.Server.Model;

namespace MeetingGrpc.Server.Repositories
{
    public class FrameCaptureStatesRepository : IRepository<FrameCaptureInfo>
    {
        private readonly List<FrameCaptureInfo> localStorage = new List<FrameCaptureInfo>(); // dummy on memory storage

        public void Add(FrameCaptureInfo message)
        {
            localStorage.Add(message);
        }

        public void Remove(FrameCaptureInfo cameraCapture)
        {
            localStorage.Remove(cameraCapture);
        }

        public IEnumerable<FrameCaptureInfo> GetAll()
        {
            return localStorage.AsReadOnly();
        }
    }
}
