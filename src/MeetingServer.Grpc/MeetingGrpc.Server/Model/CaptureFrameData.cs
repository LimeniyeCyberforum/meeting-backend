namespace MeetingGrpc.Server.Model
{
    public class CaptureFrameData : CaptureFrameInfo
    {
        public byte[] Data { get; }

        public CaptureFrameData(Guid frameCaptureAreaGuid, Guid userGuid, byte[] data)
            : base(frameCaptureAreaGuid, userGuid)
        {
            Data = data;
        }
    }
}
