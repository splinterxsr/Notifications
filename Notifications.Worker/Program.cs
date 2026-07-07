using MassTransit;
using Microsoft.Extensions.Hosting;
using Notifications.Worker.Handlers;

var builder = Host.CreateApplicationBuilder(args);

#region MassTransit (RabbitMQ)

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserConsumer>();
    x.AddConsumer<PaymentConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var host = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        var user = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER") ?? "guest";
        var password = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS") ?? "guest";

        cfg.Host(host, "/", h =>
        {
            h.Username(user);
            h.Password(password);
        });

        var userQueue = Environment.GetEnvironmentVariable("USER_QUEUE_NAME") ?? "users-queue";
        var paymentQueue = Environment.GetEnvironmentVariable("PAYMENT_QUEUE_NAME") ?? "payments-2-queue";

        cfg.ReceiveEndpoint(userQueue, e =>
        {
            e.ConfigureConsumer<UserConsumer>(context);
        });

        cfg.ReceiveEndpoint(paymentQueue, e =>
        {
            e.ConfigureConsumer<PaymentConsumer>(context);
        });
    });
});

#endregion

var host = builder.Build();

Console.WriteLine("Aguardando mensagens... Pressione Ctrl+C para parar.");

try
{
    await host.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Erro: {ex.Message}");
}