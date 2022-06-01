using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MeetingGrpc.Protos;
using MeetingGrpc.Server.Repositories.LocalServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace MeetingGrpc.Server.Services
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FrameCaptureService : FrameCapture.FrameCaptureBase
    {
        private readonly Empty empty = new Empty();
        private readonly ILogger<ChatService> _logger;
        private readonly LocalFrameCapturesService _frameCapturesService;


        public FrameCaptureService(ILogger<ChatService> logger, LocalFrameCapturesService frameCapturesService)
        {
            _logger = logger;
            _frameCapturesService = frameCapturesService;
        }

        [AllowAnonymous]
        public override async Task FrameCapturesSubscribe(Empty request, IServerStreamWriter<UserFrameCapture> responseStream, ServerCallContext context)
        {
            var peer = context.Peer;
            _logger.LogInformation($"{peer} frame captures subscribes.");

            context.CancellationToken.Register(() => _logger.LogInformation($"{peer} cancel frame captures subscription."));

            try
            {
                await _frameCapturesService.FrameCapturesAsObservable()
                    .ToAsyncEnumerable()
                    .ForEachAwaitAsync(async (x) => await responseStream.WriteAsync(
                        new UserFrameCapture
                        {
                            UserGuid = x.UserGuid,
                            CaptureFrame = x.CaptureFrame
                        }), context.CancellationToken)
                    .ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                _logger.LogError($"{peer} frame captures unsubscribed.");
            }
        }
    }
}
