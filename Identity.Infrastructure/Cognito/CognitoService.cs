namespace Identity.Infrastructure.Cognito;

public sealed class CognitoService(
    IAmazonCognitoIdentityProvider cognitoClient,
    IOptions<CognitoSettings> settings)
    : ICognitoService
{
    private readonly CognitoSettings _settings = settings.Value;

    public async Task<Result<AuthTokens>> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new InitiateAuthRequest
            {
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                ClientId = _settings.ClientId,
                AuthParameters = new Dictionary<string, string>
                {
                    ["USERNAME"] = email,
                    ["PASSWORD"] = password
                }
            };

            var response = await cognitoClient.InitiateAuthAsync(request, cancellationToken);
            var result = response.AuthenticationResult;

            return Result<AuthTokens>.Success(new AuthTokens(
                result.AccessToken,
                result.IdToken,
                result.RefreshToken,
                result.ExpiresIn));
        }
        catch (NotAuthorizedException)
        {
            return Result<AuthTokens>.Failure(Error.Unauthorized);
        }
        catch (UserNotFoundException)
        {
            return Result<AuthTokens>.Failure(Error.Unauthorized);
        }
        catch (UserNotConfirmedException)
        {
            return Result<AuthTokens>.Failure(new Error("auth.unconfirmed", "Account email has not been verified."));
        }
        catch (PasswordResetRequiredException)
        {
            return Result<AuthTokens>.Failure(new Error("auth.password_reset_required", "A password reset is required."));
        }
    }

    public async Task<Result<ValidatedUser>> ValidateAccessTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userResponse = await cognitoClient.GetUserAsync(
                new GetUserRequest { AccessToken = accessToken }, cancellationToken);

            var sub = userResponse.UserAttributes
                .FirstOrDefault(a => a.Name == "sub")?.Value ?? userResponse.Username;

            var groupsResponse = await cognitoClient.AdminListGroupsForUserAsync(
                new AdminListGroupsForUserRequest
                {
                    UserPoolId = _settings.UserPoolId,
                    Username   = userResponse.Username
                }, cancellationToken);

            var role = groupsResponse.Groups.FirstOrDefault()?.GroupName;

            return Result<ValidatedUser>.Success(new ValidatedUser(sub, role));
        }
        catch (NotAuthorizedException)
        {
            return Result<ValidatedUser>.Failure(Error.Unauthorized);
        }
    }

    public async Task<Result> SignOutAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await cognitoClient.GlobalSignOutAsync(
                new GlobalSignOutRequest { AccessToken = accessToken },
                cancellationToken);

            return Result.Success();
        }
        catch (NotAuthorizedException)
        {
            return Result.Success();
        }
    }
}
