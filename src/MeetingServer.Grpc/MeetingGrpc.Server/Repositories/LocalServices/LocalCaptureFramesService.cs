using Google.Protobuf.WellKnownTypes;
using MeetingGrpc.Server.Model;
using System.Reactive.Linq;

namespace MeetingGrpc.Server.Repositories.LocalServices
{
    public class LocalCaptureFramesService
    {
        private readonly IRepository<ValueActionInfo<CaptureFrameInfo>> _repository;

        private event Action<(bool IsOn, ValueActionInfo<CaptureFrameInfo> FrameCaptureInfo)> CaptureFrameAreasChanged;  
        private event Action<CaptureFrameData> CaptureFrameUpdated;

        public LocalCaptureFramesService(IRepository<ValueActionInfo<CaptureFrameInfo>> repository)
        {
            _repository = repository;
        }

        public void SwitchCaptureFrame(ValueActionInfo<CaptureFrameInfo> captureFrameInfo, bool isOn, bool isStandardCaptureArea)
        {
            if (isOn)
            {
                _repository.Add(captureFrameInfo);
            }
            else
            {
                if (!isStandardCaptureArea)
                {
                    _repository.Remove(captureFrameInfo);
                }
            }

            CaptureFrameAreasChanged?.Invoke((isOn, captureFrameInfo));
        }

        public void UpdateFrameCapture(CaptureFrameData cameraCapture)
        {
            CaptureFrameUpdated?.Invoke(cameraCapture);
        }

        public IObservable<(bool IsOn, ValueActionInfo<CaptureFrameInfo> FrameCaptureInfo)> CaptureFrameStatesAsObservable()
        {
            var started = Observable.FromEvent<(bool IsOn, ValueActionInfo<CaptureFrameInfo> FrameCaptureInfo)>((x) => CaptureFrameAreasChanged += x, (x) => CaptureFrameAreasChanged -= x);
            return started;
        }

        public IObservable<CaptureFrameData> FrameCapturesAsObservable()
        {
            var newFrame = Observable.FromEvent<CaptureFrameData>((x) => CaptureFrameUpdated += x, (x) => CaptureFrameUpdated -= x);
            return newFrame;
        }
    }
}
