import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { getTeamMe, logout, type TeamMeResponse } from '../api'

export default function Dashboard() {
  const navigate = useNavigate()
  const [teamMe, setTeamMe] = useState<TeamMeResponse | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let mounted = true

    const load = async () => {
      try {
        const data = await getTeamMe()
        if (mounted) {
          setTeamMe(data)
        }
      } catch (error) {
        if (error instanceof Error && error.message.includes('401')) {
          navigate('/app/login', { replace: true })
          return
        }

        if (mounted) {
          setTeamMe(null)
        }
      } finally {
        if (mounted) {
          setLoading(false)
        }
      }
    }

    void load()

    return () => {
      mounted = false
    }
  }, [navigate])

  const handleLogout = async () => {
    await logout()
    navigate('/app/login', { replace: true })
  }

  if (loading) {
    return <p className="text-gray-600">Loading your workspace...</p>
  }

  if (!teamMe) {
    return null
  }

  return (
    <div className="bg-white rounded-lg shadow p-6">
      <div className="flex items-start justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold text-gray-900 mb-2">Welcome, {teamMe.user.displayName}</h2>
          <p className="text-gray-600">
            Company: <span className="font-semibold text-gray-900">{teamMe.contractor.name}</span>
          </p>
        </div>
        <button
          type="button"
          onClick={handleLogout}
          className="rounded-md border border-gray-300 px-3 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
        >
          Logout
        </button>
      </div>
    </div>
  )
}
