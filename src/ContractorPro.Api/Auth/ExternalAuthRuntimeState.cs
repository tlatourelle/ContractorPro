using Microsoft.Extensions.Options;

namespace ContractorPro.Api.Auth;

public sealed class ExternalAuthRuntimeState
{
    private readonly object _sync = new();
    private bool _enabled;

    public ExternalAuthRuntimeState(IOptions<ExternalIdAuthenticationOptions> options)
    {
        _enabled = options.Value.Enabled;
    }

    public bool Enabled
    {
        get
        {
            lock (_sync)
            {
                return _enabled;
            }
        }
    }

    public void SetEnabled(bool enabled)
    {
        lock (_sync)
        {
            _enabled = enabled;
        }
    }
}