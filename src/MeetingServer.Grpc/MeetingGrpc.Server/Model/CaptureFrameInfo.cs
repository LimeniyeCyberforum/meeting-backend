namespace MeetingGrpc.Server.Model
{
    public class CaptureFrameInfo
    {
        public Guid FrameCaptureAreaGuid { get; }
        public Guid UserGuid { get; }

        public CaptureFrameInfo(Guid frameCaptureAreaGuid, Guid userGuid)
        {
            FrameCaptureAreaGuid = frameCaptureAreaGuid;
            UserGuid = userGuid;
        }
    }
}
