using MeetingGrpc.Protos;
using System.Reactive.Linq;

namespace MeetingGrpc.Server.Repositories.LocalServices
{
    public class LocalFrameCapturesService
    {
        private readonly IRepository<FrameCaptureState> _repository;

        private event Action<FrameCaptureState> FrameCaptureStreamStarted;
        private event Action<FrameCaptureState> FrameCaptureStreamStoped;
        private event Action<UserFrameCapture> FrameCaptureUpdated;

        public LocalFrameCapturesService(IRepository<FrameCaptureState> repository)
        {
            _repository = repository;
        }

        public void NewFrameCaptureState(FrameCaptureState newState)
        {
            if (newState.IsOn)
            {
                _repository.Add(newState);
                FrameCaptureStreamStarted?.Invoke(newState);
            }
            else
            {
                _repository.Remove(newState);
                FrameCaptureStreamStoped?.Invoke(newState);
            }
        }

        public void UpdateFrameCapture(UserFrameCapture cameraCapture)
        {
            FrameCaptureUpdated?.Invoke(cameraCapture);
        }

        public IObservable<FrameCaptureState> CaptureFrameStatesAsObservable()
        {
            var started = Observable.FromEvent<FrameCaptureState>((x) => FrameCaptureStreamStarted += x, (x) => FrameCaptureStreamStarted -= x);
            var stoped = Observable.FromEvent<FrameCaptureState>((x) => FrameCaptureStreamStoped += x, (x) => FrameCaptureStreamStoped -= x);
            return started.Concat(stoped);
        }

        public IObservable<UserFrameCapture> FrameCapturesAsObservable()
        {
            var newFrame = Observable.FromEvent<UserFrameCapture>((x) => FrameCaptureUpdated += x, (x) => FrameCaptureUpdated -= x);
            return newFrame;
        }
    }
}
