using NotificationService.Domain.Enums;

namespace NotificationService.Domain.Interfaces
{
    public interface IUserProfileService
    {
        Task<bool> IsNotificationEnabledAsync(Guid userId, NotificationType type);
        Task<string> GetUserEmailAsync(Guid userId);

    }
}
