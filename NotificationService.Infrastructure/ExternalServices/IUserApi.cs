using Refit;

namespace NotificationService.Infrastructure.ExternalServices;

public record UserConfigResponse(Guid UserId, string Email, bool EnviarFalha, bool EnviarSucesso);

public interface IUserApi
{
    [Get("/users/{userId}/config")]
    Task<UserConfigResponse> GetUserConfigurationAsync(Guid userId);
}