import { App as AntdApp, ConfigProvider, theme } from 'antd'
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { AuthProvider } from './context/AuthContext'
import AppLayout from './components/AppLayout'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import SearchPage from './pages/SearchPage'
import SubmitPage from './pages/SubmitPage'
import StatsPage from './pages/StatsPage'
import PendingPage from './pages/PendingPage'

export default function App() {
  return (
    <ConfigProvider theme={{ algorithm: theme.defaultAlgorithm }}>
      <AntdApp
        notification={{ placement: 'bottomRight' }}
        message={{ maxCount: 3 }}
      >
        <AuthProvider>
          <BrowserRouter>
            <Routes>
              <Route path="/login" element={<LoginPage />} />
              <Route path="/register" element={<RegisterPage />} />
              <Route element={<AppLayout />}>
                <Route index element={<SearchPage />} />
                <Route path="/stats" element={<StatsPage />} />
                <Route path="/submit" element={<SubmitPage />} />
                <Route path="/pending" element={<PendingPage />} />
              </Route>
              <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
          </BrowserRouter>
        </AuthProvider>
      </AntdApp>
    </ConfigProvider>
  )
}
