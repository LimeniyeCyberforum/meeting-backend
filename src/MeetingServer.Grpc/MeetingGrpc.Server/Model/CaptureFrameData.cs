namespace MeetingGrpc.Server.Model
{
    public class CaptureFrameData : CaptureFrameInfo
    {
        public byte[] Data { get; }
        public DateTime DateTime { get; }

        public CaptureFrameData(Guid frameCaptureAreaGuid, Guid userGuid, byte[] data, DateTime dateTime, bool isActive)
            : base(frameCaptureAreaGuid, userGuid, isActive)
        {
            Data = data;
            DateTime = dateTime;
        }
    }
}
