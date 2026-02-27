using Azure.Messaging.ServiceBus;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using System.Text.Json;

namespace NotificationService.Worker;

public class VideoNotificationWorker : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<VideoNotificationWorker> _logger;

    public VideoNotificationWorker(
        ServiceBusClient client,
        IServiceProvider serviceProvider,
        ILogger<VideoNotificationWorker> logger)
    {
        _processor = client.CreateProcessor("video-notifications", new ServiceBusProcessorOptions());
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;

        await _processor.StartProcessingAsync(stoppingToken);

        await Task.Delay(-1, stoppingToken);
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        var videoEvent = JsonSerializer.Deserialize<VideoEvent>(body);

        if (videoEvent != null)
        {
            using var scope = _serviceProvider.CreateScope();
            var useCase = scope.ServiceProvider.GetRequiredService<INotificationUseCase>();

            await useCase.ExecuteAsync(videoEvent);
        }

        await args.CompleteMessageAsync(args.Message);
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Erro no Service Bus Processor");
        return Task.CompletedTask;
    }
}