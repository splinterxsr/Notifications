using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Fcg.Contracts;
using Notifications.Lambda.Core.Domain;
using Notifications.Lambda.Core.UseCases;

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

        public PaymentLambdaHandler()
        {
            var services = new ServiceCollection();

            services.AddLogging(builder => builder.AddConsole());
            services.AddTransient<ISendNotificationUseCase, SendNotificationUseCase>();

            var serviceProvider = services.BuildServiceProvider();

            _useCase = serviceProvider.GetRequiredService<ISendNotificationUseCase>();
            _logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<PaymentLambdaHandler>();
        }

        public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            foreach (var message in sqsEvent.Records)
            {
                context.Logger.LogInformation($"[SQS Lambda] MENSAGEM RECEBIDA (Payment)! Body: {message.Body}");

                try
                {
                    PaymentProcessedEvent paymentEvent = null;
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    using (var doc = JsonDocument.Parse(message.Body))
                    {
                        var root = doc.RootElement;

                        if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("message", out var messageElement))
                        {
                            paymentEvent = JsonSerializer.Deserialize<PaymentProcessedEvent>(messageElement.GetRawText(), options);
                        }
                        else
                        {
                            paymentEvent = JsonSerializer.Deserialize<PaymentProcessedEvent>(message.Body, options);
                        }
                    }

                    if (paymentEvent != null && paymentEvent.Status == PaymentStatus.Approved)
                    {
                        context.Logger.LogInformation($"[SQS Lambda] Mapeado com Sucesso -> Pagamento aprovado para: {paymentEvent.UserEmail} | GameId: {paymentEvent.GameId}");

                        var notification = new NotificationMessage
                        {
                            Recipient = paymentEvent.UserEmail,
                            Subject = "Confirmação de Compra - FCG",
                            Body = $"Seu pagamento para o jogo {paymentEvent.GameId} foi aprovado com sucesso!"
                        };

                        await _useCase.ExecuteAsync(notification);
                        context.Logger.LogInformation("[SQS Lambda] E-mail de confirmação de pagamento simulado com sucesso!");
                    }
                    else
                    {
                        context.Logger.LogInformation($"[SQS Lambda] ALERTA: Não foi possível mapear o PaymentProcessedEvent ou status diferente de Approved. Status: {paymentEvent?.Status}");
                    }
                }
                catch (Exception ex)
                {
                    context.Logger.LogInformation($"[SQS Lambda] ERRO CRÍTICO no pagamento: {ex.Message}");
                    throw;
                }
            }
        }
    }
}