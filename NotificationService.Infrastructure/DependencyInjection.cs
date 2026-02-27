using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Services;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Adapters;
using NotificationService.Infrastructure.Configurations;
using NotificationService.Infrastructure.ExternalServices;
using Refit;

namespace NotificationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var emailSection = configuration.GetSection("AzureSettings:Email");
        services.Configure<AzureEmailSettings>(emailSection);

        services.AddSingleton(sp =>
        {
            var settings = emailSection.Get<AzureEmailSettings>();
            if (string.IsNullOrEmpty(settings?.ConnectionString))
                throw new InvalidOperationException("Azure Communication Services ConnectionString is missing.");

            return new EmailClient(settings.ConnectionString);
        });

        services.AddScoped<IEmailSender, EmailSenderAdapter>();
        services.AddScoped<IUserProfileService, UserProfileServiceAdapter>();

        var userApiUrl = configuration["ExternalApis:UserApiUrl"];
        if (string.IsNullOrEmpty(userApiUrl))
            throw new InvalidOperationException("User API URL is missing in configuration.");

        services.AddRefitClient<IUserApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(userApiUrl));

        services.AddScoped<INotificationUseCase, NotificationUseCase>();

        return services;
    }
}