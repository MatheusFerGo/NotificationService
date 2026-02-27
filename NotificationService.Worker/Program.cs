using NotificationService.Infrastructure;
using NotificationService.Worker;
using Azure.Messaging.ServiceBus;

var builder = Host.CreateApplicationBuilder(args);

// Injetando as camadas de Infra e Aplicação
builder.Services.AddInfrastructure(builder.Configuration);

// Registro do Service Bus Client
builder.Services.AddSingleton(new ServiceBusClient(builder.Configuration.GetConnectionString("ServiceBus")));

// O Worker propriamente dito
builder.Services.AddHostedService<VideoNotificationWorker>();

var host = builder.Build();
host.Run();