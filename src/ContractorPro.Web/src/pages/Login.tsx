import { useEffect, useMemo, useState } from 'react'

type AuthConfig = {
  enabled: boolean
  isConfigured: boolean
  canToggle: boolean
}

type AuthError = {
  error?: string
}

export default function Login() {
  const [authConfig, setAuthConfig] = useState<AuthConfig | null>(null)
  const [statusMessage, setStatusMessage] = useState<string>('')
  const [isToggling, setIsToggling] = useState(false)

  const canSignIn = useMemo(() => {
    if (!authConfig) {
      return false
    }

    return authConfig.enabled && authConfig.isConfigured
  }, [authConfig])

  useEffect(() => {
    let isCancelled = false

    const loadConfig = async () => {
      try {
        const response = await fetch('/api/v1/auth/config', {
          credentials: 'include',
        })

        if (!response.ok) {
          return
        }

        const payload = (await response.json()) as AuthConfig
        if (!isCancelled) {
          setAuthConfig(payload)
          if (!payload.isConfigured) {
            setStatusMessage('Auth is enabled but incomplete. Set Authority, ClientId, and ClientSecret in user-secrets.')
          } else if (!payload.enabled) {
            setStatusMessage('Auth is currently disabled for this API session.')
          }
        }
      } catch {
        if (!isCancelled) {
          setStatusMessage('Unable to read auth configuration from API.')
        }
      }
    }

    void loadConfig()
    return () => {
      isCancelled = true
    }
  }, [])

  const handleLogin = () => {
    if (!canSignIn) {
      return
    }

    window.location.assign('/api/v1/auth/login')
  }

  const setEnabled = async (enabled: boolean) => {
    setIsToggling(true)
    setStatusMessage('')

    try {
      const response = await fetch('/api/v1/auth/config', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include',
        body: JSON.stringify({ enabled }),
      })

      const payload = (await response.json()) as unknown
      if (!response.ok) {
        const authError = payload as AuthError
        setStatusMessage(authError.error ?? 'Could not toggle auth.')
        return
      }

      setAuthConfig(payload as AuthConfig)
      if (enabled) {
        setStatusMessage('Auth enabled. You can now sign in with Google.')
      } else {
        setStatusMessage('Auth disabled for this API session.')
      }
    } catch {
      setStatusMessage('Could not toggle auth right now.')
    } finally {
      setIsToggling(false)
    }
  }

  return (
    <div className="bg-white rounded-lg shadow p-8 max-w-md mx-auto">
      <h2 className="text-2xl font-bold text-gray-900 mb-3">Welcome to ContractorPro</h2>
      <p className="text-gray-600 mb-6">
        Sign in with Google to create or access your company workspace.
      </p>
      {statusMessage ? (
        <p className="mb-4 rounded border border-amber-200 bg-amber-50 px-3 py-2 text-sm text-amber-900">
          {statusMessage}
        </p>
      ) : null}

      {authConfig?.canToggle ? (
        <div className="mb-4 grid grid-cols-2 gap-2">
          <button
            type="button"
            onClick={() => void setEnabled(true)}
            disabled={isToggling}
            className="rounded-md border border-emerald-400 px-3 py-2 text-sm font-medium text-emerald-700 hover:bg-emerald-50 disabled:opacity-60"
          >
            Enable Auth
          </button>
          <button
            type="button"
            onClick={() => void setEnabled(false)}
            disabled={isToggling}
            className="rounded-md border border-slate-400 px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-60"
          >
            Disable Auth
          </button>
        </div>
      ) : null}

      <button
        type="button"
        onClick={handleLogin}
        disabled={!canSignIn || isToggling}
        className="w-full rounded-md bg-blue-600 px-4 py-3 text-white font-semibold hover:bg-blue-700 disabled:cursor-not-allowed disabled:bg-blue-300"
      >
        Sign in with Google
      </button>
    </div>
  )
}
