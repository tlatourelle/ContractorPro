using ContractorPro.Api.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ContractorPro.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IOptions<ExternalIdAuthenticationOptions> _options;

    public AuthController(IOptions<ExternalIdAuthenticationOptions> options)
    {
        _options = options;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        if (!_options.Value.Enabled)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = "auth_not_configured" });
        }

        var authenticationProperties = new AuthenticationProperties
        {
            RedirectUri = "/app/dashboard",
            IsPersistent = true
        };

        return Challenge(authenticationProperties, OpenIdConnectDefaults.AuthenticationScheme);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (_options.Value.Enabled)
        {
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        return NoContent();
    }
}
