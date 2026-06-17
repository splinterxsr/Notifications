using MassTransit;
using Microsoft.Extensions.Logging;
using Notifications.Worker.Contracts;

namespace Notifications.Worker.Handlers
{
    public class PaymentConsumer : IConsumer<PaymentProcessedEvent>
    {
        private readonly ILogger<PaymentConsumer> _logger;

        public PaymentConsumer(ILogger<PaymentConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            var order = context.Message;

            _logger.LogInformation("💳 Pagamento Processado:");
            _logger.LogInformation("   Transaction ID: {TransactionId}", order.TransactionId);
            _logger.LogInformation("   User: {UserId}", order.UserId);
            _logger.LogInformation("   Game: {GameId}", order.GameId);
            _logger.LogInformation("   Status: {Status}", order.Status);

            if (order.Status == PaymentStatus.Approved)
            {
                _logger.LogInformation("✅ Pagamento aprovado com sucesso!");
                _logger.LogInformation("---");

                _logger.LogInformation("🔔📧 Enviando e-mail de confirmação de compra ao usuário {UserId} ({UserEmail})...", order.UserId, order.UserEmail);
                _logger.LogInformation("---------------------");
            }
        }
    }
}