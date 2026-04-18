namespace Identity.Domain.Services;

public sealed record CognitoAuthTokens(
    string AccessToken,
    string IdToken,
    string RefreshToken,
    int ExpiresIn);
