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
    public class CaptureFramesService : CaptureFrames.CaptureFramesBase
    {
        private readonly Empty empty = new Empty();
        private readonly ILogger<ChatService> _logger;
        private readonly LocalCaptureFramesService _captureFramesService;
        private readonly LocalUsersService _usersService;

        public CaptureFramesService(ILogger<ChatService> logger, LocalUsersService usersService, LocalCaptureFramesService captureFramesService)
        {
            _logger = logger;
            _usersService = usersService;
            _captureFramesService = captureFramesService;
        }

        [AllowAnonymous]
        public override async Task CaptureFramesSubscribe(Empty request, IServerStreamWriter<CaptureFrame> responseStream, ServerCallContext context)
        {
            var peer = context.Peer;
            _logger.LogInformation($"{peer} frame captures subscribes.");

            context.CancellationToken.Register(() => _logger.LogInformation($"{peer} cancel frame captures subscription."));

            try
            {
                await _captureFramesService.FrameCapturesAsObservable()
                    .ToAsyncEnumerable()
                    .ForEachAwaitAsync(async (x) => await responseStream.WriteAsync(
                        new UserFrameCapture
                        {
                            CatureAreaGuid = x.CaptureFrameDataGuid.ToString(),
                            CaptureFrame = Google.Protobuf.ByteString.CopyFrom(x.Data)
                        }), context.CancellationToken)
                    .ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                _logger.LogError($"{peer} frame captures unsubscribed.");
            }
        }

        [Authorize]
        public override Task<Empty> SendCaptureFrame(CaptureFrame request, ServerCallContext context)
        {
            var user = GetUserFromMetadata(context.RequestHeaders);

            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"), context.RequestHeaders);

            _captureFramesService.UpdateFrameCapture(new CaptureFrameData(Guid.Parse(request.CatureAreaGuid), user.UserGuid, request.Bytes.ToByteArray()));

            return Task.FromResult(empty);
        }

        [AllowAnonymous]
        public override async Task CaptureFrameAreasSubscribe(Empty request, IServerStreamWriter<CaptureFrameArea> responseStream, ServerCallContext context)
        {
            var peer = context.Peer;
            _logger.LogInformation($"{peer} frame captures subscribes.");

            context.CancellationToken.Register(() => _logger.LogInformation($"{peer} cancel frame captures subscription."));

            try
            {
                await _captureFramesService.CaptureFrameStatesAsObservable()
                    .ToAsyncEnumerable()
                    .ForEachAwaitAsync(async (x) => await responseStream.WriteAsync(
                        new CaptureFrameArea
                        {
                            IsOn = x.IsOn,
                            CatureAreaGuid = x.FrameCaptureInfo.FrameCaptureAreaGuid.ToString()
                        }), context.CancellationToken)
                    .ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                _logger.LogError($"{peer} frame captures unsubscribed.");
            }
        }

        [Authorize]
        public override Task<CreateCaptureAreaResponse> CreateCaptureArea(Empty request, ServerCallContext context)
        {
            _logger.LogInformation($"{context.Peer} {request}");

            var user = GetUserFromMetadata(context.RequestHeaders);

            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"), context.RequestHeaders);

            Guid newAreaGuid = Guid.NewGuid();

            _captureFramesService.SwitchCaptureFrame(new CaptureFrameInfo(newAreaGuid, user.UserGuid), true);

            return Task.FromResult(new CreateCaptureAreaResponse { AreaGuid = newAreaGuid.ToString() });
        }        
        
        [Authorize]
        public override Task<Empty> DestroyCaptureArea(DestroyCaptureAreaRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"{context.Peer} {request}");

            var user = GetUserFromMetadata(context.RequestHeaders);

            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"), context.RequestHeaders);

            _captureFramesService.SwitchCaptureFrame(new CaptureFrameInfo(Guid.Parse(request.AreaGuid), user.UserGuid), false);

            return Task.FromResult(empty);
        }

        private Model.User? GetUserFromMetadata(Metadata metadata)
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
