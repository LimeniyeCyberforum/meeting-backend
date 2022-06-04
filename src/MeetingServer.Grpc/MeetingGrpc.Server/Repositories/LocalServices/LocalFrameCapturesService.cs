using MeetingGrpc.Server.Model;
using System.Reactive.Linq;

namespace MeetingGrpc.Server.Repositories.LocalServices
{
    public class LocalFrameCapturesService
    {
        private readonly IRepository<FrameCaptureInfo> _repository;

        private event Action<(bool IsOn, FrameCaptureInfo FrameCaptureInfo)> FrameCaptureStreamChanged;  
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
            }
            else
            {
                _repository.Remove(frameCaptureInfo);
            }

            FrameCaptureStreamChanged?.Invoke((isOn, frameCaptureInfo));
        }

        public void UpdateFrameCapture(FrameCaptureArea cameraCapture)
        {
            FrameCaptureUpdated?.Invoke(cameraCapture);
        }

        public IObservable<(bool IsOn, FrameCaptureInfo FrameCaptureInfo)> CaptureFrameStatesAsObservable()
        {
            var started = Observable.FromEvent<(bool IsOn, FrameCaptureInfo FrameCaptureInfo)>((x) => FrameCaptureStreamChanged += x, (x) => FrameCaptureStreamChanged -= x);
            return started;
        }

        public IObservable<FrameCaptureArea> FrameCapturesAsObservable()
        {
            var newFrame = Observable.FromEvent<FrameCaptureArea>((x) => FrameCaptureUpdated += x, (x) => FrameCaptureUpdated -= x);
            return newFrame;
        }
    }
}
