using Identity.Domain.Exceptions;

namespace Identity.Application.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    ICognitoService cognitoService)
    : IRequestHandler<LoginCommand, Result<AuthTokens>>
{
    public async Task<Result<AuthTokens>> Handle(LoginCommand command, CancellationToken ct)
    {
        Email email;
        try
        {
            email = Email.Create(command.Email);
        }
        catch (InvalidEmailException)
        {
            return Result<AuthTokens>.Failure(IdentityErrors.InvalidCredentials);
        }

        var authResult = await cognitoService.AuthenticateAsync(email.Value, command.Password, ct);

        var user = await userRepository.FindByEmailAsync(email, ct);

        if (authResult.IsFailure)
        {
            if (user is null)
                return Result<AuthTokens>.Failure(IdentityErrors.InvalidCredentials);

            user.RecordFailedLogin();
            userRepository.Update(user);
            await userRepository.UnitOfWork.SaveEntitiesAsync(ct);

            return Result<AuthTokens>.Failure(IdentityErrors.InvalidCredentials);
        }

        if (user is null)
            return Result<AuthTokens>.Failure(IdentityErrors.UserNotFound);

        user.RecordLogin();
        userRepository.Update(user);
        await userRepository.UnitOfWork.SaveEntitiesAsync(ct);

        return Result<AuthTokens>.Success(authResult.Value!);
    }
}
