using System.Text.Json;
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
                context.Logger.LogInformation($"[SQS Lambda] MENSAGEM RECEBIDA (User)! Body: {message.Body}");

                try
                {
                    UserCreatedEvent userEvent = null;
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    using (var doc = JsonDocument.Parse(message.Body))
                    {
                        var root = doc.RootElement;

                        // Se veio envelopado pelo MassTransit (tem o nó "message")
                        if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("message", out var messageElement))
                        {
                            userEvent = JsonSerializer.Deserialize<UserCreatedEvent>(messageElement.GetRawText(), options);
                        }
                        else
                        {
                            userEvent = JsonSerializer.Deserialize<UserCreatedEvent>(message.Body, options);
                        }
                    }

                    if (userEvent != null)
                    {
                        string emailFinal = userEvent.UserEmail;
                        string nomeFinal = userEvent.UserName;

                        if (string.IsNullOrEmpty(emailFinal) || !emailFinal.Contains("@"))
                        {
                            if (!string.IsNullOrEmpty(userEvent.UserName) && userEvent.UserName.Contains("@"))
                            {
                                emailFinal = userEvent.UserName;
                                nomeFinal = userEvent.UserEmail;
                            }
                        }

                        context.Logger.LogInformation($"[SQS Lambda] Mapeado com Sucesso -> Nome: {nomeFinal} | Email: {emailFinal}");

                        var notification = new NotificationMessage
                        {
                            Recipient = emailFinal,
                            Subject = "Bem-vindo à FCG - Cloud Games!",
                            Body = $"Olá {nomeFinal}, seu cadastro foi realizado com sucesso!"
                        };

                        await _useCase.ExecuteAsync(notification);
                        context.Logger.LogInformation("[SQS Lambda] E-mail de boas-vindas simulado com sucesso!");
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