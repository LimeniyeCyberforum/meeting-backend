using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using GrpcCommon;
using GrpsServer.Model;
using System.Reactive.Linq;

namespace GrpsServer.Services
{
    public class MeetingService : Meeting.MeetingBase
    {
        private readonly ILogger<MeetingService> _logger;

        private readonly ChatService _chatService;

        private readonly UsersCameraCaptureService _usersCameraCaptureService;

        public MeetingService(ILogger<MeetingService> logger, ChatService loggerTest, UsersCameraCaptureService usersCameraCaptureService)
        {
            _logger = logger;
            _chatService = loggerTest;
            _usersCameraCaptureService = usersCameraCaptureService;
        }

        private readonly Empty empty = new Empty();

        private static readonly Dictionary<Guid, string> users = new Dictionary<Guid, string>();


        public override Task<ConnectResponse> Connect(ConnectRequest request, ServerCallContext context)
        {
            var newUserGuid = Guid.NewGuid();
            if (!users.TryAdd(newUserGuid, request.Username))
            {
                return Task.FromResult(new ConnectResponse
                {
                    IsSuccessfully = false,
                    ErrorMessage = "Something whent wrong. Lets try again."
                });
            }

            return Task.FromResult(new ConnectResponse
            {
                IsSuccessfully = true,
                Guid = newUserGuid.ToString()
            });
        }

        public override async Task MessagesSubscribe(Empty request, IServerStreamWriter<MessageFromLobby> responseStream, ServerCallContext context)
        {
            var peer = context.Peer;
            _logger.LogInformation($"{peer} subscribes.");

            context.CancellationToken.Register(() => _logger.LogInformation($"{peer} cancels subscription."));

            try
            {
                await _chatService.GetChatLogsAsObservable()
                    .ToAsyncEnumerable()
                    .ForEachAwaitAsync(async (x) => await responseStream.WriteAsync(x), context.CancellationToken)
                    .ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                _logger.LogError($"{peer} unsubscribed.");
            }
        }

        public override Task<Empty> SendMessage(MessageRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"{context.Peer} {request}");

            if (!Guid.TryParse(request.UserGuid, out Guid guid))
            {
                _logger.LogError($"UserGuid {request.UserGuid} is not Guid.");
                return Task.FromResult(empty);
            }

            if (!users.TryGetValue(guid, out var username))
            {
                _logger.LogError($"User with guid {request.UserGuid} not found.");
                return Task.FromResult(empty);
            }

            _chatService.Add(new MessageFromLobby()
            {
                Username = username,
                MessageGuid = request.MessageGuid,
                Message = request.Message,
                Time = Timestamp.FromDateTime(DateTime.UtcNow),
                // TODO : Temporary
                UserGuid = request.UserGuid
            });

            return Task.FromResult(empty);
        }

        public override async Task CameraCaptureSubscribe(Empty reques, IServerStreamWriter<CameraCapture> responseStream, ServerCallContext context)
        {
            var peer = context.Peer;
            _logger.LogInformation($"{peer} subscribes.");

            context.CancellationToken.Register(() => _logger.LogInformation($"{peer} cancels subscription."));

            try
            {
                await _usersCameraCaptureService.GetUserCameraCaptureFrameUpdateAsObservable()
                    .ToAsyncEnumerable()
                    .ForEachAwaitAsync(async (x) => await responseStream.WriteAsync(x), context.CancellationToken)
                    .ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                _logger.LogError($"{peer} unsubscribed.");
            }
        }

        public override Task<Empty> SendCameraFrame(CameraCapture request, ServerCallContext context)
        {
            //_logger.LogInformation($"{context.Peer} {request}");

            if (!Guid.TryParse(request.UserGuid, out Guid guid))
            {
                _logger.LogError($"UserGuid {request.UserGuid} is not Guid.");
                return Task.FromResult(empty);
            }

            if (!users.TryGetValue(guid, out var username))
            {
                _logger.LogError($"User with guid {request.UserGuid} not found.");
                return Task.FromResult(empty);
            }

            _usersCameraCaptureService.UpdateFrameCapture(new CameraCapture()
            {
                UserGuid = request.UserGuid,
                CaptureFrame = request.CaptureFrame
            });

            return Task.FromResult(empty);
        }

        //public override async Task MessageStream(IAsyncStreamReader<MessageRequest> requestStream, 
        //    IServerStreamWriter<MessageReplay> responseStream, ServerCallContext context)
        //{
        //    await foreach (var request in requestStream.ReadAllAsync())
        //    {
        //        await responseStream.WriteAsync(new MessageReplay()
        //        {
        //            Username = request.Username,
        //            Message = request.Message,
        //            Time = Timestamp.FromDateTime(DateTime.UtcNow)
        //        });
        //    }
        //}

        //public override async Task CameraCaptureStream(IAsyncStreamReader<CameraCaptureTest> requestStream,
        //    IServerStreamWriter<CameraCaptureTest> responseStream, ServerCallContext context)
        //{
        //    await foreach (var request in requestStream.ReadAllAsync())
        //    {
        //        await responseStream.WriteAsync(new CameraCaptureTest()
        //        {
        //            CaptureFrame = request.CaptureFrame
        //        });
        //    }
        //}
    }
}