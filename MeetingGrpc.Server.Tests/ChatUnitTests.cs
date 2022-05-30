using AutoFixture;
using Grpc.Core;
using GrpsServer.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit;

namespace MeetingGrpc.Server.Tests
{
    public sealed class ChatUnitTests
    {
        private readonly Fixture _fixture;

        public ChatUnitTests()
        {
            _fixture = new Fixture();
        }

        //[Fact]
        //public async Task Given_RandomName_When_CallingConnected_Return_NewGuid()
        //{
        //    // Arrange
        //    var loggerMock = new Moq.Mock<ILogger<MeetingService>>();
        //    var contextMock = new Moq.Mock<ServerCallContext>();
        //    var service = new MeetingService.(loggerMock.Object);
        //    var request = new HelloRequest() { Name = _fixture.Create<string>() };
        //    // Act
        //    var result = await service.SayHello(request, contextMock.Object);
        //    // Assert
        //    Assert.Equal($"Hello {request.Name}", result.Message);
        //}
    }
}
