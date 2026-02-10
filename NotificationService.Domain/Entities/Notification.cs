using NotificationService.Domain.Enums;

namespace NotificationService.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; private set; }
        public string UserEmail { get; private set; }
        public NotificationType Type { get; private set; }
        public string Message { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Notification(Guid userId, string userEmail, NotificationType type, string message)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            UserEmail = userEmail;
            Type = type;
            Message = message;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
