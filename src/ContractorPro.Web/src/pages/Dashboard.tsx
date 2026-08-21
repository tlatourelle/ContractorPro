import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { getTeamMe, logout, updateCompanyProfile, type TeamMeResponse } from '../api'

export default function Dashboard() {
  const navigate = useNavigate()
  const [teamMe, setTeamMe] = useState<TeamMeResponse | null>(null)
  const [loading, setLoading] = useState(true)
  const [companyName, setCompanyName] = useState('')
  const [companyTimezone, setCompanyTimezone] = useState('')
  const [isSaving, setIsSaving] = useState(false)
  const [saveMessage, setSaveMessage] = useState('')

  useEffect(() => {
    let mounted = true

    const load = async () => {
      try {
        const data = await getTeamMe()
        if (mounted) {
          setTeamMe(data)
          setCompanyName(data.contractor.name)
          setCompanyTimezone(data.contractor.timezone)
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

  const handleSaveCompany = async () => {
    if (!teamMe) {
      return
    }

    setIsSaving(true)
    setSaveMessage('')

    try {
      const response = await updateCompanyProfile({
        name: companyName,
        timezone: companyTimezone,
      })

      setTeamMe({
        ...teamMe,
        contractor: response.contractor,
      })
      setSaveMessage('Company profile saved.')
    } catch {
      setSaveMessage('Unable to save company profile. Check values and try again.')
    } finally {
      setIsSaving(false)
    }
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
          <p className="text-gray-600">
            Timezone: <span className="font-semibold text-gray-900">{teamMe.contractor.timezone}</span>
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

      <div className="mt-6 border-t border-gray-200 pt-6">
        <h3 className="text-lg font-semibold text-gray-900 mb-3">Company Profile</h3>
        <div className="grid gap-4 sm:grid-cols-2">
          <label className="text-sm text-gray-700">
            Company Name
            <input
              type="text"
              value={companyName}
              onChange={(event) => setCompanyName(event.target.value)}
              className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-gray-900"
            />
          </label>
          <label className="text-sm text-gray-700">
            Timezone
            <input
              type="text"
              value={companyTimezone}
              onChange={(event) => setCompanyTimezone(event.target.value)}
              className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-gray-900"
            />
          </label>
        </div>
        <div className="mt-4 flex items-center gap-3">
          <button
            type="button"
            onClick={handleSaveCompany}
            disabled={isSaving || !teamMe.teamMember.isOwner}
            className="rounded-md bg-gray-900 px-4 py-2 text-sm font-medium text-white disabled:cursor-not-allowed disabled:opacity-50"
          >
            {isSaving ? 'Saving...' : 'Save'}
          </button>
          {!teamMe.teamMember.isOwner && (
            <span className="text-sm text-gray-600">Only company owners can edit profile settings.</span>
          )}
          {saveMessage && <span className="text-sm text-gray-700">{saveMessage}</span>}
        </div>
      </div>

      <div className="mt-6 border-t border-gray-200 pt-6">
        <h3 className="text-lg font-semibold text-gray-900 mb-3">Your Team Profile</h3>
        <dl className="grid gap-y-2 text-sm text-gray-700 sm:grid-cols-2 sm:gap-x-8">
          <div>
            <dt className="font-medium text-gray-500">Email</dt>
            <dd>{teamMe.user.email}</dd>
          </div>
          <div>
            <dt className="font-medium text-gray-500">Role</dt>
            <dd>{teamMe.teamMember.role}</dd>
          </div>
          <div>
            <dt className="font-medium text-gray-500">Owner</dt>
            <dd>{teamMe.teamMember.isOwner ? 'Yes' : 'No'}</dd>
          </div>
          <div>
            <dt className="font-medium text-gray-500">Member Id</dt>
            <dd>{teamMe.teamMember.id}</dd>
          </div>
        </dl>
      </div>
    </div>
  )
}
