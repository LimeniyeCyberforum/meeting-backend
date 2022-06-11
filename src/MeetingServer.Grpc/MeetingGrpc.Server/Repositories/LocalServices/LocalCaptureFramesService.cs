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
        private readonly IRepository<Guid, ValueActionInfo<CaptureFrameInfo>> _repository;

        private event Action<(CaptureFrameState Action, ValueActionInfo<CaptureFrameInfo> FrameCaptureInfo)> CaptureFrameAreasChanged;  
        private event Action<CaptureFrameData> CaptureFrameUpdated;

        public LocalCaptureFramesService(IRepository<Guid, ValueActionInfo<CaptureFrameInfo>> repository)
        {
            _repository = repository;
        }

        public void CreateArea(Guid captureAreaGuid, Guid userGuid, DateTime time)
        {
            var newArea = new ValueActionInfo<CaptureFrameInfo>(new CaptureFrameInfo(captureAreaGuid, userGuid, false), time);
            _repository.Add(newArea.Value.FrameCaptureAreaGuid, newArea);
            CaptureFrameAreasChanged?.Invoke((CaptureFrameState.Created, newArea));
        }

        public void RemoveArea(Guid captureAreaGuid, Guid userGuid, DateTime time)
        {
            var removedAread = new ValueActionInfo<CaptureFrameInfo>(new CaptureFrameInfo(captureAreaGuid, userGuid, false), time);
            _repository.Remove(captureAreaGuid);
            CaptureFrameAreasChanged?.Invoke((CaptureFrameState.Removed, removedAread));
        }

        public void SwitchCaptureFrame(Guid captureAreaGuid, Guid userGuid, DateTime time, bool isActive)
        {
            var updatedValue = new ValueActionInfo<CaptureFrameInfo>(new CaptureFrameInfo(captureAreaGuid, userGuid, isActive), time);
            _repository.Update(captureAreaGuid, updatedValue);
            CaptureFrameAreasChanged?.Invoke((isActive? CaptureFrameState.Enabled : CaptureFrameState.Disabled, updatedValue));
        }

        public void UpdateFrameCapture(CaptureFrameData cameraCapture)
        {
            CaptureFrameUpdated?.Invoke(cameraCapture);
        }

        public IObservable<(CaptureFrameState Action, ValueActionInfo<CaptureFrameInfo> FrameCaptureInfo)> CaptureFrameStatesAsObservable()
        {
            var allElements = _repository.GetAll().Select(x => (x.Value.IsActive ? CaptureFrameState.Enabled : CaptureFrameState.Disabled, x)).ToObservable();
            var started = Observable.FromEvent<(CaptureFrameState Action, ValueActionInfo<CaptureFrameInfo> FrameCaptureInfo)>((x) => CaptureFrameAreasChanged += x, (x) => CaptureFrameAreasChanged -= x);
            return allElements.Concat(started);
        }

        public IObservable<CaptureFrameData> FrameCapturesAsObservable()
        {
            var newChanges = Observable.FromEvent<CaptureFrameData>((x) => CaptureFrameUpdated += x, (x) => CaptureFrameUpdated -= x);
            return newChanges;
        }
    }
}
