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

        [Authorize]
        public override Task<Empty> SendFrameCapture(FrameCaptureRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"{context.Peer} {request}");

            //if (!Guid.TryParse(request., out Guid messageGuid))
            //    throw new NotImplementedException();

            var user = context.GetHttpContext().User;

            throw new NotImplementedException(" Should to finde user from context");

            //_chatService.Add(new Message(messageGuid, request.Message, Timestamp.FromDateTime(DateTime.UtcNow),
            //                    new User()
            //    Username = username,
            //    MessageGuid = request.MessageGuid,
            //    Message = request.Message,
            //    Time = ,
            //    // TODO : Temporary
            //    UserGuid = request.UserGuid
            //});

            return Task.FromResult(empty);
        }

        [AllowAnonymous]
        public override async Task FrameCaptureStatesSubscribe(Empty request, IServerStreamWriter<FrameCaptureState> responseStream, ServerCallContext context)
        {
            var peer = context.Peer;
            _logger.LogInformation($"{peer} frame captures subscribes.");

            context.CancellationToken.Register(() => _logger.LogInformation($"{peer} cancel frame captures subscription."));

            try
            {
                await _frameCapturesService.CaptureFrameStatesAsObservable()
                    .ToAsyncEnumerable()
                    .ForEachAwaitAsync(async (x) => await responseStream.WriteAsync(
                        new FrameCaptureState
                        {
                            UserGuid = x.UserGuid,
                            IsOn = x.IsOn
                        }), context.CancellationToken)
                    .ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                _logger.LogError($"{peer} frame captures unsubscribed.");
            }
        }

        [Authorize]
        public override Task<Empty> TurnOffFrameCapture(Empty request, ServerCallContext context)
        {
            _logger.LogInformation($"{context.Peer} {request}");

            //if (!Guid.TryParse(request., out Guid messageGuid))
            //    throw new NotImplementedException();

            var user = context.GetHttpContext().User;

            throw new NotImplementedException(" Should to finde user from context");

            //_chatService.Add(new Message(messageGuid, request.Message, Timestamp.FromDateTime(DateTime.UtcNow),
            //                    new User()
            //    Username = username,
            //    MessageGuid = request.MessageGuid,
            //    Message = request.Message,
            //    Time = ,
            //    // TODO : Temporary
            //    UserGuid = request.UserGuid
            //});

            return Task.FromResult(empty);
        }


        [Authorize]
        public override Task<Empty> TurnOnFrameCapture(Empty request, ServerCallContext context)
        {
            _logger.LogInformation($"{context.Peer} {request}");

            //if (!Guid.TryParse(request., out Guid messageGuid))
            //    throw new NotImplementedException();

            var user = context.GetHttpContext().User;

            throw new NotImplementedException(" Should to finde user from context");

            //_chatService.Add(new Message(messageGuid, request.Message, Timestamp.FromDateTime(DateTime.UtcNow),
            //                    new User()
            //    Username = username,
            //    MessageGuid = request.MessageGuid,
            //    Message = request.Message,
            //    Time = ,
            //    // TODO : Temporary
            //    UserGuid = request.UserGuid
            //});

            return Task.FromResult(empty);
        }
    }
}
