using NotificationService.Application.DTOs;

namespace NotificationService.Application.Interfaces
{
    public interface INotificationUseCase
    {
        Task ExecuteAsync(VideoEvent videoEvent);
    }
}