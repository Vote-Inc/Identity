namespace Identity.Domain.Services;

public interface ICognitoService
{
    Task<Result<AuthTokens>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result<ValidatedUser>> ValidateAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default);
    Task<Result> SignOutAsync(string accessToken, CancellationToken cancellationToken = default);
}
