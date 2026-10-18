using Fcg.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Moq;
using Notifications.Function;
using System.Text.Json;

namespace Notifications.Tests
{
    public class SendPaymentNotificationTests
    {
        [Fact]
        public void Run_Should_Process_Approved_Payment_Successfully()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<SendPaymentNotification>>();
            var function = new SendPaymentNotification(loggerMock.Object);

            var paymentEvent = new PaymentProcessedEvent(
                Guid.NewGuid(),
                1,
                "user@test.com",
                Guid.NewGuid().ToString(),
                PaymentStatus.Approved
            );

            var wrappedMessage = JsonSerializer.Serialize(new { message = paymentEvent });

            var contextMock = new Mock<FunctionContext>();

            // Act
            function.Run(wrappedMessage, contextMock.Object);

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("PaymentProcessedEvent desserializado com sucesso")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);
        }
    }
}