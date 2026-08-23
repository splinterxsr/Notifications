using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Fcg.Contracts;
using Microsoft.Extensions.Logging;
using Notifications.Lambda.Core.Domain;
using Notifications.Lambda.Core.UseCases;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Notifications.Lambda.Adapters.Inbound
{
    public class PaymentLambdaHandler
    {
        private readonly ISendNotificationUseCase _useCase;
        private readonly ILogger<PaymentLambdaHandler> _logger;

        public PaymentLambdaHandler(ISendNotificationUseCase useCase, ILogger<PaymentLambdaHandler> logger)
        {
            _useCase = useCase;
            _logger = logger;
        }

        public PaymentLambdaHandler() { }

        public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            foreach (var message in sqsEvent.Records)
            {
                _logger.LogInformation($"[SQS Lambda] Processando pagamento: {message.Body}");

                var paymentEvent = JsonSerializer.Deserialize<PaymentProcessedEvent>(message.Body);

                if (paymentEvent != null && paymentEvent.Status == PaymentStatus.Approved)
                {
                    // Traduzindo o evento externo para o modelo de domínio (NotificationMessage)
                    var notification = new NotificationMessage
                    {
                        Recipient = paymentEvent.UserEmail,
                        Subject = "Confirmação de Compra - FCG",
                        Body = $"Seu pagamento para o jogo {paymentEvent.GameId} foi aprovado com sucesso!"
                    };

                    // Aciona o Core
                    await _useCase.ExecuteAsync(notification);
                }
            }
        }
    }
}