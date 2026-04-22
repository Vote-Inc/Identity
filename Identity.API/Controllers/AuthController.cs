namespace Identity.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    LoginCommandHandler loginCommandHandler,
    LogoutCommandHandler logoutCommandHandler,
    ValidateTokenQueryHandler validateTokenQueryHandler) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var result = await loginCommandHandler.Handle(
            new LoginCommand(request.Email, request.Password), ct);

        return result.Match<IActionResult>(
            onSuccess: tokens => Ok(new { token = tokens.AccessToken, expiresIn = tokens.ExpiresIn }),
            onFailure: error => error.Code switch
            {
                "identity.credentials.invalid"  => Unauthorized(error.ToResponse()),
                "identity.user.not_found"       => NotFound(error.ToResponse()),
                _                               => BadRequest(error.ToResponse())
            });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var token = HttpContext.Request.Headers.Authorization
            .ToString()
            .Replace("Bearer ", string.Empty);

        var result = await logoutCommandHandler.Handle(new LogoutCommand(token), ct);

        return result.Match<IActionResult>(
            onSuccess: NoContent,
            onFailure: error => BadRequest(error.ToResponse()));
    }

    [HttpGet("validate")]
    public async Task<IActionResult> Validate(CancellationToken ct)
    {
        var token = HttpContext.Request.Headers.Authorization
            .ToString()
            .Replace("Bearer ", string.Empty);

        if (string.IsNullOrEmpty(token))
            return Unauthorized();

        var result = await validateTokenQueryHandler.Handle(new ValidateTokenQuery(token), ct);

        return result.Match<IActionResult>(
            onSuccess: user =>
            {
                Response.Headers["X-Voter-Id"] = user.Sub;

                if (!string.IsNullOrEmpty(user.Role))
                    Response.Headers["X-Voter-Role"] = user.Role;

                return Ok();
            },
            onFailure: _ => Unauthorized()
        );
    }
}
