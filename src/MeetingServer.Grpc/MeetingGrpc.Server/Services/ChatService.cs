using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MeetingGrpc.Protos;
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


        public ChatService(ILogger<ChatService> logger, LocalChatService chatService)
        {
            _logger = logger;
            _chatService = chatService;
        }

        [AllowAnonymous]
        public override async Task MessagesSubscribe(Empty request, IServerStreamWriter<LobbyMessage> responseStream, ServerCallContext context)
        {
            var peer = context.Peer;
            _logger.LogInformation($"{peer} messages subscribes.");

            context.CancellationToken.Register(() => _logger.LogInformation($"{peer} cancels subscription."));

            try
            {
                await _chatService.GetMessagesAsObservable()
                    .ToAsyncEnumerable()
                    .ForEachAwaitAsync(async (x) => await responseStream.WriteAsync(
                        new LobbyMessage 
                        {
                            MessageGuid = x.Guid.ToString(),
                            Message = x.Content,
                            Time = x.DateTime.ToTimestamp(),
                            UserGuid = x.User.UserGuid.ToString(),
                            Username = x.User.Name
                        }), context.CancellationToken)
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

            var user = context.GetHttpContext().User;

            throw new ArgumentException(" Should to finde user from context");

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
