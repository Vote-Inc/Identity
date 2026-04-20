namespace Identity.Application.Auth.Commands.Logout;

public sealed record LogoutCommand(string AccessToken) : IRequest<Result>;