using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.ExternalServices;

namespace NotificationService.Infrastructure.Adapters;

public class UserProfileServiceAdapter : IUserProfileService
{
    private readonly IUserApi _userApi;

    public UserProfileServiceAdapter(IUserApi userApi)
    {
        _userApi = userApi;
    }

    public async Task<bool> IsNotificationEnabledAsync(Guid userId, NotificationType type)
    {
        try
        {
            var config = await _userApi.GetUserConfigurationAsync(userId);

            return type switch
            {
                NotificationType.Error => config.EnviarFalha,
                NotificationType.VideoProcessed => config.EnviarSucesso,
                NotificationType.VideoUploaded => true, // Regra de negócio: Sempre notifica upload
                _ => false
            };
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<string> GetUserEmailAsync(Guid userId)
    {
        var config = await _userApi.GetUserConfigurationAsync(userId);
        return config.Email;
    }
}