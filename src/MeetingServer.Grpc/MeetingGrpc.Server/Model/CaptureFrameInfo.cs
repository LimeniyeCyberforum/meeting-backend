namespace MeetingGrpc.Server.Model
{
    public class CaptureFrameInfo
    {
        public Guid FrameCaptureAreaGuid { get; }
        public Guid UserGuid { get; }

        public bool IsActive { get; }

        public CaptureFrameInfo(Guid frameCaptureAreaGuid, Guid userGuid, bool isActive)
        {
            FrameCaptureAreaGuid = frameCaptureAreaGuid;
            UserGuid = userGuid;
            IsActive = isActive;
        }
    }
}
