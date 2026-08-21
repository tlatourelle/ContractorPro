import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import AppLayout from './pages/AppLayout'
import PortalLayout from './pages/PortalLayout'
import './index.css'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/app" replace />} />
        <Route path="/app/*" element={<AppLayout />} />
        <Route path="/p/*" element={<PortalLayout />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
