using System.Text.Json;
using Fcg.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Moq;
using Notifications.Function;

namespace Notifications.Tests
{
    public class SendUserCreatedNotificationTests
    {
        [Fact]
        public void Run_Should_Process_User_Created_Successfully()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<SendUserCreatedNotification>>();
            var function = new SendUserCreatedNotification(loggerMock.Object);

            var userEvent = new UserCreatedEvent(
                1,
                "TestUser",
                "user@test.com",
                1
            );

            var wrappedMessage = JsonSerializer.Serialize(new { message = userEvent });
            var contextMock = new Mock<FunctionContext>();

            // Act
            function.Run(wrappedMessage, contextMock.Object);

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("UserCreatedEvent desserializado com sucesso")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);
        }
    }
}