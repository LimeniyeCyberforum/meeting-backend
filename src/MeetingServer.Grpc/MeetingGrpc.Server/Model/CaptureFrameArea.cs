namespace MeetingGrpc.Server.Model
{
    public class CaptureFrameArea : CaptureFrameInfo
    {
        public byte[] Data { get; }

        public CaptureFrameArea(Guid frameCaptureAreaGuid, Guid userGuid, byte[] data)
            : base(frameCaptureAreaGuid, userGuid)
        {
            Data = data;
        }
    }
}
