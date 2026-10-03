using Fcg.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Notifications.Function
{
    public class SendUserCreatedNotification
    {
        private readonly ILogger<SendUserCreatedNotification> _logger;

        public SendUserCreatedNotification(ILogger<SendUserCreatedNotification> logger)
        {
            _logger = logger;
        }

        [Function(nameof(SendUserCreatedNotification))]
        public void Run(
            [ServiceBusTrigger(
                topicName: "users-topic",
                subscriptionName: "notifications-sub",
                Connection = "ServiceBusConnectionString")] string messageBody,
            FunctionContext context)
        {
            _logger.LogInformation("==========================================");
            _logger.LogInformation("🚀 [AZURE FUNCTION] Nova mensagem recebida do Service Bus!");
            _logger.LogInformation("Tópico: notifications-topic | Assinatura: users-sub");
            _logger.LogInformation("Conteúdo da mensagem:");
            _logger.LogInformation("{MessageBody}", messageBody);
            _logger.LogInformation("==========================================");

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var jsonNode = JsonNode.Parse(messageBody);

            var messageContent = jsonNode?["message"]?.ToJsonString();

            if (messageContent is not null)
            {
                var userEvent = JsonSerializer.Deserialize<UserCreatedEvent>(messageContent, jsonOptions);

                if (userEvent is not null)
                {
                    _logger.LogInformation("💰 [USER CREATED EVENT] UserCreatedEvent desserializado com sucesso!");
                    _logger.LogInformation("Enviando notificação para: {UserEmail}", userEvent.UserEmail);
                    _logger.LogInformation("Assunto: Bem-vindo à FCG - Cloud Games!");
                    _logger.LogInformation("Conteúdo:Olá {UserName}, seu cadastro foi realizado com sucesso!", userEvent.UserName);
                    _logger.LogInformation("--------------------------------------------------");
                }
                else
                {
                    _logger.LogWarning("⚠️ [USER CREATED EVENT] Falha ao desserializar UserCreatedEvent.");
                }
            }
        }
    }
}
