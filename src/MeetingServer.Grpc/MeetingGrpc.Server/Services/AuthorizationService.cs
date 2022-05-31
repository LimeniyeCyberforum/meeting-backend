using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using MeetingGrpc.Protos;
using MeetingGrpc.Server.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MeetingGrpc.Server.Model;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MeetingGrpc.Repositories.LocalServices;

namespace GrpsServer.Services
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthorizationService : Authorization.AuthorizationBase
    {
        private readonly ILogger<AuthorizationService> _logger;
        private readonly IConfiguration _configuration;

        private readonly UsersService _usersService;

        //private readonly UsersCameraCaptureService _usersCameraCaptureService;

        public AuthorizationService(ILogger<AuthorizationService> logger, IConfiguration configuration, UsersService usersService)
        {
            _logger = logger;
            _configuration = configuration;
            _usersService = usersService;
            //_chatService = loggerTest;
            //_usersCameraCaptureService = usersCameraCaptureService;
        }

        private readonly Empty empty = new Empty();

        //private static readonly Dictionary<Guid, string> users = new Dictionary<Guid, string>();

        [AllowAnonymous]
        public override Task<CheckNameResponse> IsNameExists(CheckNameRequest request, ServerCallContext context)
        {
            return Task.FromResult(new CheckNameResponse
            {
                IsExists = request?.Username != null ? _usersService.IsNameExists(request.Username) : false
            });
        }

        [AllowAnonymous]
        public override Task<ConnectResponse> Connect(ConnectRequest request, ServerCallContext context)
        {
            if (request?.Username == null)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 .TryAdd(newUserGuid, request.Username))
            {
                return Task.FromResult(new ConnectResponse
                {
                    IsSuccess = false
                });
            }

            var newUser = new UserDto
            {
                UserGuid = Guid.NewGuid(),
                Name = request.Username,
            };

            var userRoles = new List<string>
            {
                "User"
            };
            var token = GetJwtToken(newUser, userRoles);

            _usersService.Add(newUser);

            return Task.FromResult(new ConnectResponse { IsSuccess = true, UserGuid = newUser.UserGuid.ToString(), JwtToken = token.JwtToken, Expiration = Timestamp.FromDateTime(token.Expiration) });
        }

        private TokenDto GetJwtToken(UserDto applicationUserDto, IEnumerable<string> roles)
        {
            var applicationUserClaims = GetApplicationUserClaims(applicationUserDto);
            var applicationUserRolesClaims = GetRolesAsClaims(roles);
            var jwtAuthRequiredClaims = GetJwtAuthRequiredClaims(_configuration.GetValue<string>("JwtIssuer"), _configuration.GetValue<string>("JwtAudience"));
            var claims = jwtAuthRequiredClaims.Union(applicationUserRolesClaims).Union(applicationUserClaims);
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JwtKey")));
            var signingCredential = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtHeader = new JwtHeader(signingCredential);
            var jwtPayload = new JwtPayload(claims);
            var token = new JwtSecurityToken(jwtHeader, jwtPayload);
            return new TokenDto
            {
                JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };
        }

        private static IEnumerable<Claim> GetApplicationUserClaims(UserDto userDto)
        {
            return new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userDto.Name),
                new Claim("Guid", userDto.UserGuid.ToString())
            };
        }

        private static IEnumerable<Claim> GetJwtAuthRequiredClaims(string issuer, string audience)
        {
            return new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddHours(8)).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, issuer),
                new Claim(JwtRegisteredClaimNames.Aud, audience)
            };
        }

        private static IEnumerable<Claim> GetRolesAsClaims(IEnumerable<string> roles)
        {
            const string roleType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
            return roles.Select(x => new Claim(roleType, x));
        }


        //public override async Task MessagesSubscribe(Empty request, IServerStreamWriter<MessageFromLobby> responseStream, ServerCallContext context)
        //{
        //    var peer = context.Peer;
        //    _logger.LogInformation($"{peer} subscribes.");

        //    context.CancellationToken.Register(() => _logger.LogInformation($"{peer} cancels subscription."));

        //    try
        //    {
        //        await _chatService.GetChatLogsAsObservable()
        //            .ToAsyncEnumerable()
        //            .ForEachAwaitAsync(async (x) => await responseStream.WriteAsync(x), context.CancellationToken)
        //            .ConfigureAwait(false);
        //    }
        //    catch (TaskCanceledException)
        //    {
        //        _logger.LogError($"{peer} unsubscribed.");
        //    }
        //}

        //public override Task<Empty> SendMessage(MessageRequest request, ServerCallContext context)
        //{
        //    _logger.LogInformation($"{context.Peer} {request}");

        //    if (!Guid.TryParse(request.UserGuid, out Guid guid))
        //    {
        //        _logger.LogError($"UserGuid {request.UserGuid} is not Guid.");
        //        return Task.FromResult(empty);
        //    }

        //    if (!users.TryGetValue(guid, out var username))
        //    {
        //        _logger.LogError($"User with guid {request.UserGuid} not found.");
        //        return Task.FromResult(empty);
        //    }

        //    _chatService.Add(new MessageFromLobby()
        //    {
        //        Username = username,
        //        MessageGuid = request.MessageGuid,
        //        Message = request.Message,
        //        Time = Timestamp.FromDateTime(DateTime.UtcNow),
        //        // TODO : Temporary
        //        UserGuid = request.UserGuid
        //    });

        //    return Task.FromResult(empty);
        //}

        //public override async Task CameraCaptureSubscribe(Empty reques, IServerStreamWriter<CameraCapture> responseStream, ServerCallContext context)
        //{
        //    var peer = context.Peer;
        //    _logger.LogInformation($"{peer} subscribes.");

        //    context.CancellationToken.Register(() => _logger.LogInformation($"{peer} cancels subscription."));

        //    try
        //    {
        //        await _usersCameraCaptureService.GetUserCameraCaptureFrameUpdateAsObservable()
        //            .ToAsyncEnumerable()
        //            .ForEachAwaitAsync(async (x) => await responseStream.WriteAsync(x), context.CancellationToken)
        //            .ConfigureAwait(false);
        //    }
        //    catch (TaskCanceledException)
        //    {
        //        _logger.LogError($"{peer} unsubscribed.");
        //    }
        //}

        //public override Task<Empty> SendCameraFrame(CameraCapture request, ServerCallContext context)
        //{
        //    //_logger.LogInformation($"{context.Peer} {request}");

        //    if (!Guid.TryParse(request.UserGuid, out Guid guid))
        //    {
        //        _logger.LogError($"UserGuid {request.UserGuid} is not Guid.");
        //        return Task.FromResult(empty);
        //    }

        //    if (!users.TryGetValue(guid, out var username))
        //    {
        //        _logger.LogError($"User with guid {request.UserGuid} not found.");
        //        return Task.FromResult(empty);
        //    }

        //    _usersCameraCaptureService.UpdateFrameCapture(new CameraCapture()
        //    {
        //        UserGuid = request.UserGuid,
        //        CaptureFrame = request.CaptureFrame
        //    });

        //    return Task.FromResult(empty);
        //}


























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