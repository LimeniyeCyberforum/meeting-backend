using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MeetingGrpc.Protos;
using MeetingGrpc.Repositories.LocalServices;
using MeetingGrpc.Server.Model;
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
        private readonly LocalUsersService _usersService;

        public FrameCaptureService(ILogger<ChatService> logger, LocalFrameCapturesService frameCapturesService, LocalUsersService usersService)
        {
            _logger = logger;
            _frameCapturesService = frameCapturesService;
            _usersService = usersService;
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
            var user = GetUserFromMetadata(context.RequestHeaders);

            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"), context.RequestHeaders);

            _frameCapturesService.UpdateFrameCapture(new UserFrameCapture { UserGuid = user.UserGuid.ToString(), CaptureFrame = request.CaptureFrame });

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

            //var user = GetUserFromMetadata(context.RequestHeaders);

            //if (user == null)
            //    throw new RpcException(new Status(StatusCode.NotFound, "User not found"), context.RequestHeaders);

            //_frameCapturesService.UpdateFrameCapture(new UserFrameCapture { UserGuid = user.UserGuid, CaptureFrame =  })

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

        private User? GetUserFromMetadata(Metadata metadata)
        {
            if (metadata == null)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Permission denied"), metadata);

            var token = metadata.FirstOrDefault(x => x.Key == "authorization");

            if (token?.Value == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Token is null"), metadata);

            return _usersService.GetUserFromToken(token.Value);
        }
    }
}
