using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using MeetingGrpc.Protos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MeetingGrpc.Server.Model;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MeetingGrpc.Repositories.LocalServices;
using User = MeetingGrpc.Server.Model.User;
using MeetingGrpc.Server.Repositories.LocalServices;

namespace MeetingGrpc.Server.Services
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthorizationService : Authorization.AuthorizationBase
    {
        private readonly ILogger<AuthorizationService> _logger;
        private readonly IConfiguration _configuration;

        private readonly LocalUsersService _usersService;
        private readonly LocalCaptureFramesService _captureFramesService;

        public AuthorizationService(ILogger<AuthorizationService> logger, IConfiguration configuration, LocalUsersService usersService, LocalCaptureFramesService captureFramesService)
        {
            _logger = logger;
            _configuration = configuration;
            _usersService = usersService;
            _captureFramesService = captureFramesService;
        }

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
            if (request?.Username == null)
            {
                return Task.FromResult(new ConnectResponse
                {
                    IsSuccess = false
                });
            }

            User newUserNotFull = new User(Guid.NewGuid(), request.Username, null);
            var userRoles = new List<string> { "User" };
            var token = GetJwtToken(newUserNotFull, userRoles);
            var userFull = new User(newUserNotFull.UserGuid, newUserNotFull.Name, token);
            _usersService.Add(userFull);
            _captureFramesService.CreateArea(userFull.UserGuid, userFull.UserGuid, DateTime.UtcNow);

            return Task.FromResult(new ConnectResponse { IsSuccess = true, UserGuid = newUserNotFull.UserGuid.ToString(), JwtToken = token.JwtToken, Expiration = Timestamp.FromDateTime(token.Expiration) });
        }

        private Token GetJwtToken(User applicationUserDto, IEnumerable<string> roles)
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
            return new Token(new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo);
        }

        private static IEnumerable<Claim> GetApplicationUserClaims(User userDto)
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
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.UtcNow.AddHours(8)).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, issuer),
                new Claim(JwtRegisteredClaimNames.Aud, audience)
            };
        }

        private static IEnumerable<Claim> GetRolesAsClaims(IEnumerable<string> roles)
        {
            const string roleType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
            return roles.Select(x => new Claim(roleType, x));
        }
    }
}