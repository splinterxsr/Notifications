using MassTransit;
using Microsoft.Extensions.Logging;
using Notifications.Worker.Contracts;

namespace Notifications.Worker.Handlers
{
    public class UserConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly ILogger<UserConsumer> _logger;

        public UserConsumer(ILogger<UserConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var user = context.Message;

            _logger.LogInformation("✅ Novo Usuário cadastrado:");
            _logger.LogInformation("   ID: {UserId}", user.UserId);
            _logger.LogInformation("   Usuário: {UserName}", user.UserName);
            _logger.LogInformation("---------------------");

            _logger.LogInformation("🔔📧 Enviando e-mail de boas vindas para o usuário {UserName} ({UserEmail})...", user.UserName, user.UserEmail);
            _logger.LogInformation("---------------------");
        }
    }
}