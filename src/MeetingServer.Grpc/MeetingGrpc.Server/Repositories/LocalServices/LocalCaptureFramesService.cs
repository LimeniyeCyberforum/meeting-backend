using Google.Protobuf.WellKnownTypes;
using MeetingGrpc.Server.Model;
using System.Reactive.Linq;

namespace MeetingGrpc.Server.Repositories.LocalServices
{
    public enum CaptureFrameState
    {
        Disabled,
        Enabled,
        Created,
        Removed
    }

    public class LocalCaptureFramesService
    {
        private readonly IRepository<ValueActionInfo<CaptureFrameInfo>> _repository;

        private event Action<(CaptureFrameState Action, ValueActionInfo<CaptureFrameInfo> FrameCaptureInfo)> CaptureFrameAreasChanged;  
        private event Action<CaptureFrameData> CaptureFrameUpdated;

        public LocalCaptureFramesService(IRepository<ValueActionInfo<CaptureFrameInfo>> repository)
        {
            _repository = repository;
        }

        public void SwitchCaptureFrame(ValueActionInfo<CaptureFrameInfo> captureFrameInfo, CaptureFrameState newState)
        {
            switch (newState)
            {
                case CaptureFrameState.Created:
                    _repository.Add(captureFrameInfo);
                    break;
                case CaptureFrameState.Removed:
                    _repository.Remove(captureFrameInfo);
                    break;
            }
            CaptureFrameAreasChanged?.Invoke((newState, captureFrameInfo));
        }

        public void UpdateFrameCapture(CaptureFrameData cameraCapture)
        {
            CaptureFrameUpdated?.Invoke(cameraCapture);
        }

        public IObservable<(CaptureFrameState Action, ValueActionInfo<CaptureFrameInfo> FrameCaptureInfo)> CaptureFrameStatesAsObservable()
        {
            var started = Observable.FromEvent<(CaptureFrameState Action, ValueActionInfo<CaptureFrameInfo> FrameCaptureInfo)>((x) => CaptureFrameAreasChanged += x, (x) => CaptureFrameAreasChanged -= x);
            return started;
        }

        public IObservable<CaptureFrameData> FrameCapturesAsObservable()
        {
            var newFrame = Observable.FromEvent<CaptureFrameData>((x) => CaptureFrameUpdated += x, (x) => CaptureFrameUpdated -= x);
            return newFrame;
        }
    }
}
