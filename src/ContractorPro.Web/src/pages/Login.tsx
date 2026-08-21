export default function Login() {
  const handleLogin = () => {
    window.location.assign('/api/v1/auth/login')
  }

  return (
    <div className="bg-white rounded-lg shadow p-8 max-w-md mx-auto">
      <h2 className="text-2xl font-bold text-gray-900 mb-3">Welcome to ContractorPro</h2>
      <p className="text-gray-600 mb-6">
        Sign in with Google to create or access your company workspace.
      </p>
      <button
        type="button"
        onClick={handleLogin}
        className="w-full rounded-md bg-blue-600 px-4 py-3 text-white font-semibold hover:bg-blue-700"
      >
        Sign in with Google
      </button>
    </div>
  )
}
