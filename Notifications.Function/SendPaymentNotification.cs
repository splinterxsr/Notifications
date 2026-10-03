using Fcg.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Notifications.Function;

public class SendPaymentNotification
{
    private readonly ILogger<SendPaymentNotification> _logger;

    public SendPaymentNotification(ILogger<SendPaymentNotification> logger)
    {
        _logger = logger;
    }

    [Function(nameof(SendPaymentNotification))]
    public void Run(
            [ServiceBusTrigger(
                topicName: "payments-topic",
                subscriptionName: "notifications-sub",
                Connection = "ServiceBusConnectionString")] string messageBody,
            FunctionContext context)
    {
        _logger.LogInformation("==========================================");
        _logger.LogInformation("🚀 [AZURE FUNCTION] Nova mensagem recebida do Service Bus!");
        _logger.LogInformation("Conteúdo bruto (messageBody): {Body}", messageBody);
        _logger.LogInformation("==========================================");

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var jsonNode = JsonNode.Parse(messageBody);

        var messageContent = jsonNode?["message"]?.ToJsonString() ?? messageBody;

        var paymentEvent = JsonSerializer.Deserialize<PaymentProcessedEvent>(messageContent, jsonOptions);

        if (paymentEvent != null)
        {
            _logger.LogInformation("💰 [PAYMENT EVENT] PaymentProcessedEvent desserializado com sucesso!");

            if (paymentEvent.Status == PaymentStatus.Approved)
            {
                _logger.LogInformation("✅ Pagamento aprovado para o usuário: {UserEmail} | GameId: {GameId}", paymentEvent.UserEmail, paymentEvent.GameId);
                _logger.LogInformation("Enviando notificação para: {UserEmail}", paymentEvent.UserEmail);
                _logger.LogInformation("Assunto: Confirmação de Compra - FCG");
                _logger.LogInformation("Conteúdo: Seu pagamento para o jogo {paymentEvent.GameId} foi aprovado com sucesso!", paymentEvent.GameId);
                _logger.LogInformation("--------------------------------------------------");
            }
            else
            {
                _logger.LogWarning("❌ Pagamento rejeitado para o usuário: {UserEmail} | GameId: {GameId}", paymentEvent.UserEmail, paymentEvent.GameId);
            }
        }
        else
        {
            _logger.LogWarning("⚠️ [PAYMENT EVENT] Falha ao desserializar PaymentProcessedEvent.");
        }
    }
}