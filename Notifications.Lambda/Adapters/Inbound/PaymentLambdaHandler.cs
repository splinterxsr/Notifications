using System;
using System.Text.Json;
using System.Threading.Tasks;
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
                try
                {
                    _logger.LogInformation($"[SQS Lambda] Processando pagamento, mensagem bruta: {message.Body}");

                    PaymentProcessedEvent paymentEvent = null;

                    try
                    {
                        paymentEvent = JsonSerializer.Deserialize<PaymentProcessedEvent>(message.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    catch
                    {
                        using var doc = JsonDocument.Parse(message.Body);
                        if (doc.RootElement.TryGetProperty("message", out var messageElement))
                        {
                            paymentEvent = JsonSerializer.Deserialize<PaymentProcessedEvent>(messageElement.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        }
                    }

                    if (paymentEvent != null && paymentEvent.Status == PaymentStatus.Approved)
                    {
                        _logger.LogInformation($"[SQS Lambda] Pagamento processado com sucesso para o usuário: {paymentEvent.UserEmail}");

                        var notification = new NotificationMessage
                        {
                            Recipient = paymentEvent.UserEmail,
                            Subject = "Confirmação de Compra - FCG",
                            Body = $"Seu pagamento para o jogo {paymentEvent.GameId} foi aprovado com sucesso!"
                        };

                        await _useCase.ExecuteAsync(notification);
                    }
                    else
                    {
                        _logger.LogWarning("[SQS Lambda] Não foi possível mapear o PaymentProcessedEvent do body ou o status não é Approved.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[SQS Lambda] Erro ao processar mensagem de pagamento: {ex.Message}");
                    throw;
                }
            }
        }
    }
}