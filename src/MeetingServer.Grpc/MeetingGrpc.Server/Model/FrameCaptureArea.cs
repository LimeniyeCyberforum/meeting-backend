namespace MeetingGrpc.Server.Model
{
    public class FrameCaptureArea : FrameCaptureInfo
    {
        public byte[] Data { get; }

        public FrameCaptureArea(Guid frameCaptureAreaGuid, Guid userGuid, byte[] data)
            : base(frameCaptureAreaGuid, userGuid)
        {
            Data = data;
        }
    }
}
