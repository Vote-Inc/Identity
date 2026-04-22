namespace Identity.Application.Auth.Queries.ValidateToken;

public sealed class ValidateTokenQueryHandler(ICognitoService cognitoService)
    : IRequestHandler<ValidateTokenQuery, Result<ValidatedUser>>
{
    public async Task<Result<ValidatedUser>> Handle(ValidateTokenQuery query, CancellationToken ct)
    {
        return await cognitoService.ValidateAccessTokenAsync(query.AccessToken, ct);
    }
}
