namespace NotificationService.Application.DTOs
{
    public record VideoEvent
    (
        string UserId,
        string Name,
        string Email,
        string VideoId,
        string VideoTitle,
        string Status,
        string SendMailOnSuccess,
        string SendMailOnFailure
    );
}