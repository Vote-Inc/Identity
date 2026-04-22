namespace Identity.Application.Auth.Queries.ValidateToken;

public sealed record ValidateTokenQuery(string AccessToken) : IRequest<Result<ValidatedUser>>;
