namespace NotificationService.Application.DTOs
{
    public record VideoEvent
    (
        Guid UserId,
        string UserEmail,
        Guid VideoId,
        string Status,
        string? Message
    );
}