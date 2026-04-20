namespace Identity.Domain.Services;

public sealed record AuthTokens(
    string AccessToken,
    string IdToken,
    string RefreshToken,
    int? ExpiresIn);
