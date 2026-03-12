using NotificationService.Infrastructure;
using NotificationService.Worker;
using Azure.Messaging.ServiceBus;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

var sbConnection = builder.Configuration["SERVICEBUS_CONNECTION"];
builder.Services.AddSingleton(new ServiceBusClient(sbConnection));

builder.Services.AddHostedService<VideoNotificationWorker>();

var host = builder.Build();
host.Run();