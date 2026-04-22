namespace Identity.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    LoginCommandHandler loginCommandHandler,
    LogoutCommandHandler logoutCommandHandler) : ControllerBase
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

    [Authorize]
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

    [Authorize]
    [HttpGet("validate")]
    public IActionResult Validate()
    {
        var voterId = User.FindFirst("sub")?.Value;
        var voterRole = User.FindFirst("cognito:groups")?.Value;

        if (string.IsNullOrEmpty(voterId))
            return Unauthorized();

        Response.Headers["X-Voter-Id"] = voterId;

        if (!string.IsNullOrEmpty(voterRole))
            Response.Headers["X-Voter-Role"] = voterRole;

        return Ok();
    }
}
