using System.Text.Json;
using Microsoft.Extensions.Logging;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Fcg.Contracts;
using Notifications.Lambda.Core.Domain;
using Notifications.Lambda.Core.UseCases;

namespace Notifications.Lambda.Adapters.Inbound
{
    public class UserLambdaHandler
    {
        private readonly ISendNotificationUseCase _useCase;
        private readonly ILogger<UserLambdaHandler> _logger;

        // Construtor principal para Injeção de Dependência
        public UserLambdaHandler(ISendNotificationUseCase useCase, ILogger<UserLambdaHandler> logger)
        {
            _useCase = useCase;
            _logger = logger;
        }

        // Construtor padrão exigido pelo AWS Lambda se usar reflection pura
        public UserLambdaHandler() { }

        public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            foreach (var message in sqsEvent.Records)
            {
                try
                {
                    _logger?.LogInformation($"[SQS Lambda] Mensagem bruta recebida: {message.Body}");

                    UserCreatedEvent userEvent = null;

                    // Tenta desserializar diretamente
                    try
                    {
                        userEvent = JsonSerializer.Deserialize<UserCreatedEvent>(message.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    catch
                    {
                        // Se falhar, tenta extrair de dentro do envelope do MassTransit se houver
                        using var doc = JsonDocument.Parse(message.Body);
                        if (doc.RootElement.TryGetProperty("message", out var messageElement))
                        {
                            userEvent = JsonSerializer.Deserialize<UserCreatedEvent>(messageElement.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        }
                    }

                    if (userEvent != null && !string.IsNullOrEmpty(userEvent.UserEmail))
                    {
                        _logger?.LogInformation($"[SQS Lambda] Usuário desserializado com sucesso: {userEvent.UserName} ({userEvent.UserEmail})");

                        var notification = new NotificationMessage
                        {
                            Recipient = userEvent.UserEmail,
                            Subject = "Bem-vindo à FCG - Cloud Games!",
                            Body = $"Olá {userEvent.UserName}, seu cadastro foi realizado com sucesso!"
                        };

                        if (_useCase != null)
                        {
                            await _useCase.ExecuteAsync(notification);
                        }
                        else
                        {
                            _logger?.LogWarning("[SQS Lambda] _useCase é nulo (problema de Injeção de Dependência).");
                        }
                    }
                    else
                    {
                        _logger?.LogWarning("[SQS Lambda] Não foi possível mapear o UserCreatedEvent do body.");
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"[SQS Lambda] Erro crítico ao processar mensagem: {ex.Message}");
                    throw;
                }
            }
        }
    }
}