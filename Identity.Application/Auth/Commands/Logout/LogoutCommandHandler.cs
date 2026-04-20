namespace Identity.Application.Auth.Commands.Logout;

public sealed class LogoutCommandHandler(ICognitoService cognitoService)
    : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand command, CancellationToken ct)
    {
        return await cognitoService.SignOutAsync(command.AccessToken, ct);
    }
}
