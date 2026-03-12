using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Services
{
    public class NotificationUseCase : INotificationUseCase
    {
        private readonly IEmailSender _emailSender;

        public NotificationUseCase(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task ExecuteAsync(VideoEvent videoEvent)
        {
            var notificationType = MapStatusToType(videoEvent.Status);

            var notification = new Notification(
                videoEvent.UserId,
                videoEvent.Name,
                videoEvent.Email,
                videoEvent.VideoId,
                videoEvent.VideoTitle,
                notificationType,
                videoEvent.SendMailOnSuccess,
                videoEvent.SendMailOnFailure
            );

            if (!notification.ShouldSendEmail())
            {
                return; // Encerra o processamento silenciosamente se a flag for "N"
            }

            string subject = notificationType == NotificationType.Success
                ? "FIAP Hackathon - Vídeo processado com sucesso!"
                : "FIAP Hackathon - Falha ao processar seu vídeo";

            string statusText = notificationType == NotificationType.Success
                ? "foi processado com sucesso e já está disponível"
                : "apresentou um erro durante o processamento";

            string body = $"Olá {notification.UserName}!\n\nSeu vídeo '{notification.VideoTitle}' (ID: {notification.VideoId}) {statusText}.";

            await _emailSender.SendEmailAsync(notification.UserEmail, subject, body);
        }

        private NotificationType MapStatusToType(string status) => status?.ToLower() switch
        {
            "success" => NotificationType.Success,
            _ => NotificationType.Error
        };
    }
}