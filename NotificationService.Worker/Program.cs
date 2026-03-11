using NotificationService.Infrastructure;
using NotificationService.Worker;
using Azure.Messaging.ServiceBus;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSingleton(new ServiceBusClient(builder.Configuration.GetConnectionString("ServiceBus")));

builder.Services.AddHostedService<VideoNotificationWorker>();

var host = builder.Build();
host.Run();