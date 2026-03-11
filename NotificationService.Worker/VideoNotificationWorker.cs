using Azure.Messaging.ServiceBus;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using System.Text.Json;

namespace NotificationService.Worker;

public class VideoNotificationWorker : BackgroundService
{
    private readonly ServiceBusClient _client;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<VideoNotificationWorker> _logger;
    private readonly string _successQueueName;
    private readonly string _errorQueueName;

    private ServiceBusProcessor _successProcessor;
    private ServiceBusProcessor _errorProcessor;

    public VideoNotificationWorker(
        ServiceBusClient client,
        IServiceProvider serviceProvider,
        ILogger<VideoNotificationWorker> logger,
        IConfiguration configuration)
    {
        _client = client;
        _serviceProvider = serviceProvider;
        _logger = logger;

        _successQueueName = configuration["ServiceBus:SuccessQueue"] ?? "success-queue";
        _errorQueueName = configuration["ServiceBus:ErrorQueue"] ?? "error-queue";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _successProcessor = _client.CreateProcessor(_successQueueName, new ServiceBusProcessorOptions());
        _errorProcessor = _client.CreateProcessor(_errorQueueName, new ServiceBusProcessorOptions());

        _successProcessor.ProcessMessageAsync += MessageHandler;
        _successProcessor.ProcessErrorAsync += ErrorHandler;

        _errorProcessor.ProcessMessageAsync += MessageHandler;
        _errorProcessor.ProcessErrorAsync += ErrorHandler;

        await _successProcessor.StartProcessingAsync(stoppingToken);
        await _errorProcessor.StartProcessingAsync(stoppingToken);

        _logger.LogInformation("Worker iniciado. Escutando filas: {SuccessQueue} e {ErrorQueue}", _successQueueName, _errorQueueName);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        _logger.LogInformation("Mensagem recebida da fila {QueueName}: {Body}", args.EntityPath, body);

        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var videoEvent = JsonSerializer.Deserialize<VideoEvent>(body, options);

            if (videoEvent != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var useCase = scope.ServiceProvider.GetRequiredService<INotificationUseCase>();

                await useCase.ExecuteAsync(videoEvent);
            }

            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar mensagem da fila {QueueName}", args.EntityPath);
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Erro interno no Service Bus Processor para a entidade {EntityPath}", args.EntityPath);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await _successProcessor.StopProcessingAsync(stoppingToken);
        await _errorProcessor.StopProcessingAsync(stoppingToken);

        await _successProcessor.DisposeAsync();
        await _errorProcessor.DisposeAsync();

        await base.StopAsync(stoppingToken);
    }
}