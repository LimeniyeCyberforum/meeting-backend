using MeetingGrpc.Server.Model;
using System.Reactive.Linq;

namespace MeetingGrpc.Server.Repositories.LocalServices
{
    public class LocalFrameCapturesService
    {
        private readonly IRepository<FrameCaptureInfo> _repository;

        private event Action<FrameCaptureInfo> FrameCaptureStreamStarted;
        private event Action<FrameCaptureInfo> FrameCaptureStreamStoped;
        private event Action<FrameCaptureArea> FrameCaptureUpdated;

        public LocalFrameCapturesService(IRepository<FrameCaptureInfo> repository)
        {
            _repository = repository;
        }

        public void ShitchFrameCapture(FrameCaptureInfo frameCaptureInfo, bool isOn)
        {
            if (isOn)
            {
                _repository.Add(frameCaptureInfo);
                FrameCaptureStreamStarted?.Invoke(frameCaptureInfo);
            }
            else
            {
                _repository.Remove(frameCaptureInfo);
                FrameCaptureStreamStoped?.Invoke(frameCaptureInfo);
            }
        }

        public void UpdateFrameCapture(FrameCaptureArea cameraCapture)
        {
            FrameCaptureUpdated?.Invoke(cameraCapture);
        }

        public IObservable<FrameCaptureInfo> CaptureFrameStatesAsObservable()
        {
            var started = Observable.FromEvent<FrameCaptureInfo>((x) => FrameCaptureStreamStarted += x, (x) => FrameCaptureStreamStarted -= x);
            var stoped = Observable.FromEvent<FrameCaptureInfo>((x) => FrameCaptureStreamStoped += x, (x) => FrameCaptureStreamStoped -= x);
            return started.Concat(stoped);
        }

        public IObservable<FrameCaptureArea> FrameCapturesAsObservable()
        {
            var newFrame = Observable.FromEvent<FrameCaptureArea>((x) => FrameCaptureUpdated += x, (x) => FrameCaptureUpdated -= x);
            return newFrame;
        }
    }
}
