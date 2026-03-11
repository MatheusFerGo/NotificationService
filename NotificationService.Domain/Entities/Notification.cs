using NotificationService.Domain.Enums;

namespace NotificationService.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public string UserId { get; private set; }
        public string UserName { get; private set; }
        public string UserEmail { get; private set; }
        public string VideoId { get; private set; }
        public string VideoTitle { get; private set; }
        public NotificationType Type { get; private set; }

        public string SendMailOnSuccess { get; private set; }
        public string SendMailOnFailure { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public Notification(string userId, string userName, string userEmail, string videoId, string videoTitle, NotificationType type, string sendMailOnSuccess, string sendMailOnFailure)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            UserName = userName;
            UserEmail = userEmail;
            VideoId = videoId;
            VideoTitle = videoTitle;
            Type = type;
            SendMailOnSuccess = sendMailOnSuccess;
            SendMailOnFailure = sendMailOnFailure;
            CreatedAt = DateTime.UtcNow;
        }

        public bool ShouldSendEmail()
        {
            if (Type == NotificationType.Success && SendMailOnSuccess == "Y") return true;
            if (Type == NotificationType.Error && SendMailOnFailure == "Y") return true;
            return false;
        }
    }
}