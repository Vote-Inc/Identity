namespace Identity.Domain.Services;

public sealed record ValidatedUser(string Sub, string? Role);
