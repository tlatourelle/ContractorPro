import { Routes, Route } from 'react-router-dom'

export default function PortalLayout() {
  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center">
              <h1 className="text-xl font-bold text-gray-900">ContractorPro Portal</h1>
            </div>
          </div>
        </div>
      </nav>

      <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <Routes>
          <Route path="/" element={<PortalHome />} />
        </Routes>
      </main>
    </div>
  )
}

function PortalHome() {
  return (
    <div className="bg-white rounded-lg shadow p-6">
      <h2 className="text-2xl font-bold text-gray-900 mb-4">
        Portal — Scaffold
      </h2>
      <p className="text-gray-600">
        Subcontractor and customer portal. Implementation coming in Story 1.1+.
      </p>
    </div>
  )
}
