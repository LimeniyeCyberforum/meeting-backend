using MeetingGrpc.Protos;
using System.ComponentModel.Composition;

namespace MeetingGrpc.Server.Repositories
{
    public class FrameCaptureStatesRepository : IRepository<FrameCaptureState>
    {
        private readonly List<FrameCaptureState> localStorage = new List<FrameCaptureState>(); // dummy on memory storage

        public void Add(FrameCaptureState message)
        {
            localStorage.Add(message);
        }

        public void Remove(FrameCaptureState cameraCapture)
        {
            localStorage.Remove(cameraCapture);
        }

        public IEnumerable<FrameCaptureState> GetAll()
        {
            return localStorage.AsReadOnly();
        }
    }
}
