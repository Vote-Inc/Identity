namespace Identity.Domain.Services;

public interface ICognitoService
{
    Task<Result<CognitoAuthTokens>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
}
