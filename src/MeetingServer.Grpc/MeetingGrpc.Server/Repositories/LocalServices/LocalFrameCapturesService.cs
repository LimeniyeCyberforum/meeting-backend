using MeetingGrpc.Server.Model;
using System.Reactive.Linq;

namespace MeetingGrpc.Server.Repositories.LocalServices
{
    public class LocalCaptureFramesService
    {
        private readonly IRepository<CaptureFrameInfo> _repository;

        private event Action<(bool IsOn, CaptureFrameInfo FrameCaptureInfo)> CaptureFrameAreasChanged;  
        private event Action<CaptureFrameArea> CaptureFrameUpdated;

        public LocalCaptureFramesService(IRepository<CaptureFrameInfo> repository)
        {
            _repository = repository;
        }

        public void ShitchCaptureFrame(CaptureFrameInfo captureFrameInfo, bool isOn)
        {
            if (isOn)
            {
                _repository.Add(captureFrameInfo);
            }
            else
            {
                _repository.Remove(captureFrameInfo);
            }

            CaptureFrameAreasChanged?.Invoke((isOn, captureFrameInfo));
        }

        public void UpdateFrameCapture(CaptureFrameArea cameraCapture)
        {
            CaptureFrameUpdated?.Invoke(cameraCapture);
        }

        public IObservable<(bool IsOn, CaptureFrameInfo FrameCaptureInfo)> CaptureFrameStatesAsObservable()
        {
            var started = Observable.FromEvent<(bool IsOn, CaptureFrameInfo FrameCaptureInfo)>((x) => CaptureFrameAreasChanged += x, (x) => CaptureFrameAreasChanged -= x);
            return started;
        }

        public IObservable<CaptureFrameArea> FrameCapturesAsObservable()
        {
            var newFrame = Observable.FromEvent<CaptureFrameArea>((x) => CaptureFrameUpdated += x, (x) => CaptureFrameUpdated -= x);
            return newFrame;
        }
    }
}
