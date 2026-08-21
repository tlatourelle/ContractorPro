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
    private readonly ExternalAuthRuntimeState _runtimeState;
    private readonly IHostEnvironment _environment;

    public AuthController(
        IOptions<ExternalIdAuthenticationOptions> options,
        ExternalAuthRuntimeState runtimeState,
        IHostEnvironment environment)
    {
        _options = options;
        _runtimeState = runtimeState;
        _environment = environment;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        var options = _options.Value;

        if (!_runtimeState.Enabled)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = "auth_not_configured" });
        }

        if (!IsConfigured(options))
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = "auth_config_incomplete" });
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

        if (_runtimeState.Enabled)
        {
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        return NoContent();
    }

    [HttpGet("config")]
    public IActionResult GetConfig()
    {
        return Ok(new
        {
            enabled = _runtimeState.Enabled,
            isConfigured = IsConfigured(_options.Value),
            canToggle = _environment.IsDevelopment()
        });
    }

    [HttpPost("config")]
    public IActionResult SetConfig([FromBody] SetAuthConfigRequest request)
    {
        if (!_environment.IsDevelopment())
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { error = "auth_toggle_not_allowed" });
        }

        if (request.Enabled && !IsConfigured(_options.Value))
        {
            return BadRequest(new { error = "auth_config_incomplete" });
        }

        _runtimeState.SetEnabled(request.Enabled);

        return Ok(new
        {
            enabled = _runtimeState.Enabled,
            isConfigured = IsConfigured(_options.Value),
            canToggle = true
        });
    }

    private static bool IsConfigured(ExternalIdAuthenticationOptions options)
    {
        return !string.IsNullOrWhiteSpace(options.Authority)
            && !options.Authority.Contains('<')
            && !options.Authority.Contains('>')
            && !string.IsNullOrWhiteSpace(options.ClientId)
            && !options.ClientId.Contains('<')
            && !options.ClientId.Contains('>')
            && !string.IsNullOrWhiteSpace(options.ClientSecret)
            && !options.ClientSecret.Contains('<')
            && !options.ClientSecret.Contains('>');
    }

    public sealed class SetAuthConfigRequest
    {
        public bool Enabled { get; set; }
    }
}
