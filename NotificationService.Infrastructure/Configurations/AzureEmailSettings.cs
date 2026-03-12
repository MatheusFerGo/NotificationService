namespace NotificationService.Infrastructure.Configurations;

public class AzureEmailSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string SenderAddress { get; set; } = string.Empty;
}
