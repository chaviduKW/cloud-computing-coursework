import { createContext, useContext, useState, useEffect, type ReactNode } from 'react'
import type { UserDto } from '../types'

interface AuthState {
  user: UserDto | null
  accessToken: string | null
  refreshToken: string | null
}

interface AuthContextValue extends AuthState {
  setAuth: (token: string, refresh: string, user: UserDto) => void
  clearAuth: () => void
  isAuthenticated: boolean
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AuthState>(() => ({
    accessToken: localStorage.getItem('accessToken'),
    refreshToken: localStorage.getItem('refreshToken'),
    user: (() => {
      const u = localStorage.getItem('user')
      return u ? (JSON.parse(u) as UserDto) : null
    })(),
  }))

  useEffect(() => {
    if (state.accessToken) localStorage.setItem('accessToken', state.accessToken)
    else localStorage.removeItem('accessToken')

    if (state.refreshToken) localStorage.setItem('refreshToken', state.refreshToken)
    else localStorage.removeItem('refreshToken')

    if (state.user) localStorage.setItem('user', JSON.stringify(state.user))
    else localStorage.removeItem('user')
  }, [state])

  const setAuth = (accessToken: string, refreshToken: string, user: UserDto) =>
    setState({ accessToken, refreshToken, user })

  const clearAuth = () => setState({ accessToken: null, refreshToken: null, user: null })

  return (
    <AuthContext.Provider
      value={{ ...state, setAuth, clearAuth, isAuthenticated: !!state.accessToken }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
