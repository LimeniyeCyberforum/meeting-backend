namespace MeetingGrpc.Server.Model
{
    public class FrameCaptureInfo
    {
        public Guid FrameCaptureAreaGuid { get; }
        public Guid UserGuid { get; }

        public FrameCaptureInfo(Guid frameCaptureAreaGuid, Guid userGuid)
        {
            FrameCaptureAreaGuid = frameCaptureAreaGuid;
            UserGuid = userGuid;
        }
    }
}
