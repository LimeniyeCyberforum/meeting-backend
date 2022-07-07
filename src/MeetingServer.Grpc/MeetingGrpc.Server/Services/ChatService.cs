using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MeetingProtobuf.Protos;
using MeetingGrpc.Repositories.LocalServices;
using MeetingGrpc.Server.Model;
using MeetingGrpc.Server.Repositories.LocalServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace MeetingGrpc.Server.Services
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatService : Chat.ChatBase
    {
        private readonly Empty empty = new Empty();
        private readonly ILogger<ChatService> _logger;
        private readonly LocalChatService _chatService;
        private readonly LocalUsersService _usersService;


        public ChatService(ILogger<ChatService> logger, LocalChatService chatService, LocalUsersService localUsersService)
        {
            _logger = logger;
            _chatService = chatService;
            _usersService = localUsersService;
        }

        [AllowAnonymous]
        public override async Task MessagesSubscribe(Empty request, IServerStreamWriter<LobbyMessageResponse> responseStream, ServerCallContext context)
        {
            var peer = context.Peer;
            _logger.LogInformation($"{peer} messages subscribes.");

            context.CancellationToken.Register(() => _logger.LogInformation($"{peer} cancels subscription."));

            try
            {
                await _chatService.GetMessagesAsObservable()
                    .ToAsyncEnumerable()
                    .ForEachAwaitAsync(async (x) => await responseStream.WriteAsync(
                        new LobbyMessageResponse
                        {
                            Action = x.Action.ToProtosAction(),
                            LobbyMessage = new LobbyMessage
                            {
                                MessageGuid = x.Message.Guid.ToString(),
                                Message = x.Message.Content,
                                Time = x.Message.DateTime.ToTimestamp(),
                                UserGuid = x.Message.User.UserGuid.ToString(),
                                Username = x.Message.User.Name
                            }
                        }, context.CancellationToken))
                    .ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                _logger.LogError($"{peer} unsubscribed.");
            }
        }

        [Authorize]
        public override Task<Empty> SendMessage(MessageRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"{context.Peer} {request}");

            if (!Guid.TryParse(request.MessageGuid, out Guid messageGuid))
                throw new NotImplementedException();

            var user = GetUserFromMetadata(context.RequestHeaders);

            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"), context.RequestHeaders);


            _chatService.Add(new Message(messageGuid, request.Message, DateTime.UtcNow, user));

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
