using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MeetingProtobuf.Protos;
using MeetingGrpc.Repositories.LocalServices;
using MeetingGrpc.Server.Repositories.LocalServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace MeetingGrpc.Server.Services
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class UsersService : Users.UsersBase
    {
        private readonly ILogger<UsersService> _logger;

        private readonly LocalUsersService _usersService;

        public UsersService(ILogger<UsersService> logger, LocalUsersService usersService)
        {
            _logger = logger;
            _usersService = usersService;
        }

        [AllowAnonymous]
        public override async Task UsersSubscribe(Empty request, IServerStreamWriter<UserOnlineStatusResponse> responseStream, ServerCallContext context)
        {
            var peer = context.Peer;
            _logger.LogInformation($"[UsersSubscribe] : {peer} subscribes.");

            context.CancellationToken.Register(() => _logger.LogInformation($"{peer} cancels users subscription."));

            try
            {
                await _usersService.GetUsersAsObservable()
                    .ToAsyncEnumerable()
                    .ForEachAwaitAsync(async (x) => await responseStream.WriteAsync(
                        new UserOnlineStatusResponse
                        {
                            Action = x.Action.ToProtosAction(),
                            User = new User { Name = x.User.Name, UserGuid = x.User.UserGuid.ToString() }
                        }), context.CancellationToken)
                    .ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                _logger.LogError($"{peer} unsubscribed.");
            }
        }

        //[Authorize]
        //public override Task<OnlineTimerRefreshResponse> RefreshMyOnlineTimer(Empty request, ServerCallContext context)
        //{
        //    throw new NotFiniteNumberException();
        //}
    }
}
