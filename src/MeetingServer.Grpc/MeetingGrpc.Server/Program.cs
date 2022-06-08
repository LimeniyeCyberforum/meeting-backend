using MeetingGrpc.Protos;
using MeetingGrpc.Repositories.LocalServices;
using MeetingGrpc.Server.Model;
using MeetingGrpc.Server.Repositories;
using MeetingGrpc.Server.Repositories.LocalServices;
using MeetingGrpc.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidAudience = builder.Configuration.GetValue<string>("JwtAudience"),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration.GetValue<string>("JwtIssuer"),
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtKey"))),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5)
    };
});

builder.Services.AddAuthorization();
builder.Services.AddGrpc();
builder.Services.AddGrpc();
//builder.Services.AddWebEncoders();
//builder.Services.AddGrpcWeb(o => o.GrpcWebEnabled = true);

builder.Services.AddSingleton<IRepository<Message>, ChatRepository>();
builder.Services.AddSingleton<IRepository<CaptureFrameInfo>, FrameCaptureStatesRepository>();
builder.Services.AddSingleton<IRepository<MeetingGrpc.Server.Model.User>, UsersRepository>();

builder.Services.AddSingleton<LocalUsersService>();
builder.Services.AddSingleton<LocalChatService>();
builder.Services.AddSingleton<LocalCaptureFramesService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseGrpcWeb();
app.MapGrpcService<UsersService>().EnableGrpcWeb();
app.MapGrpcService<AuthorizationService>().EnableGrpcWeb();
app.MapGrpcService<ChatService>().EnableGrpcWeb();
app.MapGrpcService<CaptureFramesService>().EnableGrpcWeb();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
