using NotificationService.Application.DTOs;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Services
{
    public class NotificationUseCase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IEmailSender _emailSender;

        public NotificationUseCase(IUserProfileService userProfileService, IEmailSender emailSender)
        {
            _userProfileService = userProfileService;
            _emailSender = emailSender;
        }

        public async Task ExecuteAsync(VideoEvent videoEvent)
        {
            var notificationType = MapStatusToType(videoEvent.Status);

            bool isEnabled = await _userProfileService.IsNotificationEnabledAsync(videoEvent.UserId, notificationType);

            if (!isEnabled)
            {
                return;
            }

            string subject = $"FIAP X - Status do Vídeo: {videoEvent.Status}";
            string body = $"Olá! Seu vídeo {videoEvent.VideoId} está com o status: {videoEvent.Status}. \n{videoEvent.Message}";

            await _emailSender.SendEmailAsync(videoEvent.UserEmail, subject, body);
        }

        private NotificationType MapStatusToTupe(string status) => status.ToLower() switch
        {
            "uploaded" => NotificationType.VideoUploaded,
            "processed" => NotificationType.VideoProcessed,
            _ => NotificationType.Error
        };
    }
}
