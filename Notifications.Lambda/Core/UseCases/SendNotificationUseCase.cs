using Microsoft.Extensions.Logging;
using Notifications.Lambda.Core.Domain;

namespace Notifications.Lambda.Core.UseCases
{
    public interface ISendNotificationUseCase
    {
        Task ExecuteAsync(NotificationMessage notification);
    }

    public class SendNotificationUseCase : ISendNotificationUseCase
    {
        private readonly ILogger<SendNotificationUseCase> _logger;

        public SendNotificationUseCase(ILogger<SendNotificationUseCase> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync(NotificationMessage notification)
        {
            _logger.LogInformation("Enviando notificação para: {Recipient}", notification.Recipient);
            _logger.LogInformation("Assunto: {Subject}", notification.Subject);
            _logger.LogInformation("Conteúdo: {Body}", notification.Body);
            _logger.LogInformation("--------------------------------------------------");

            return Task.CompletedTask;
        }
    }
}
