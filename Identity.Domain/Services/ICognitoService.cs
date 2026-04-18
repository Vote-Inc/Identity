namespace Identity.Domain.Services;

public interface ICognitoService
{
    Task<Result<AuthTokens>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates all tokens issued to the user via Cognito GlobalSignOut.
    /// </summary>
    Task<Result> SignOutAsync(string accessToken, CancellationToken cancellationToken = default);
}
