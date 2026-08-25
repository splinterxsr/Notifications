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

namespace Notifications.Lambda.Adapters.Inbound
{
    public class UserLambdaHandler
    {
        private readonly ISendNotificationUseCase _useCase;
        private readonly ILogger<UserLambdaHandler> _logger;

        public UserLambdaHandler(ISendNotificationUseCase useCase, ILogger<UserLambdaHandler> logger)
        {
            _useCase = useCase;
            _logger = logger;
        }

        public UserLambdaHandler()
        {
            var services = new ServiceCollection();

            services.AddLogging(builder => builder.AddConsole());
            services.AddTransient<ISendNotificationUseCase, SendNotificationUseCase>();

            var serviceProvider = services.BuildServiceProvider();

            _useCase = serviceProvider.GetRequiredService<ISendNotificationUseCase>();
            _logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<UserLambdaHandler>();
        }

        public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            foreach (var message in sqsEvent.Records)
            {
                // Impressão síncrona forçada garantindo a exibição do log na nuvem
                context.Logger.LogInformation($"[SQS Lambda] MENSAGEM RECEBIDA (User)! Body: {message.Body}");

                try
                {
                    UserCreatedEvent userEvent = null;

                    try
                    {
                        userEvent = JsonSerializer.Deserialize<UserCreatedEvent>(message.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    catch
                    {
                        using var doc = JsonDocument.Parse(message.Body);
                        if (doc.RootElement.TryGetProperty("message", out var messageElement))
                        {
                            userEvent = JsonSerializer.Deserialize<UserCreatedEvent>(messageElement.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        }
                    }

                    if (userEvent != null && !string.IsNullOrEmpty(userEvent.UserEmail))
                    {
                        context.Logger.LogInformation($"[SQS Lambda] Usuário desserializado com sucesso: {userEvent.UserName} ({userEvent.UserEmail})");

                        var notification = new NotificationMessage
                        {
                            Recipient = userEvent.UserEmail,
                            Subject = "Bem-vindo à FCG - Cloud Games!",
                            Body = $"Olá {userEvent.UserName}, seu cadastro foi realizado com sucesso!"
                        };

                        await _useCase.ExecuteAsync(notification);
                        context.Logger.LogInformation($"[SQS Lambda] E-mail de boas-vindas simulado com sucesso!");
                    }
                    else
                    {
                        context.Logger.LogInformation("[SQS Lambda] ALERTA: Não foi possível mapear o UserCreatedEvent do body.");
                    }
                }
                catch (Exception ex)
                {
                    context.Logger.LogInformation($"[SQS Lambda] ERRO CRÍTICO no usuário: {ex.Message}");
                    throw;
                }
            }
        }
    }
}