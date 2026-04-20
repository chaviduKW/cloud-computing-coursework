import axios from 'axios'
import type { InternalAxiosRequestConfig } from 'axios'

// Track refresh state to avoid infinite loops
let isRefreshing = false
let failedQueue: Array<{ resolve: (token: string) => void; reject: (err: unknown) => void }> = []

const processQueue = (error: unknown, token: string | null) => {
  failedQueue.forEach(({ resolve, reject }) => {
    if (error) reject(error)
    else resolve(token!)
  })
  failedQueue = []
}

// If VITE_API_BASE_URL is set, use it directly (e.g. pointing to a remote server).
// Otherwise fall back to relative URLs so the Vite dev-server proxy handles routing.
const client = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '',
})

// Attach JWT to every request
client.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const token = localStorage.getItem('accessToken')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Auto-refresh on 401
client.interceptors.response.use(
  (res) => res,
  async (error) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean }

    if (error.response?.status === 401 && !originalRequest._retry) {
      const refreshToken = localStorage.getItem('refreshToken')

      if (!refreshToken) {
        localStorage.clear()
        window.location.href = '/login'
        return Promise.reject(error)
      }

      if (isRefreshing) {
        // Queue this request until refresh is done
        return new Promise<string>((resolve, reject) => {
          failedQueue.push({ resolve, reject })
        }).then((token) => {
          originalRequest.headers.Authorization = `Bearer ${token}`
          return client(originalRequest)
        })
      }

      originalRequest._retry = true
      isRefreshing = true

      try {
        // Use plain axios (not intercepted client) to avoid loop
        const baseURL = import.meta.env.VITE_API_BASE_URL || ''
        const { data } = await axios.post(`${baseURL}/api/auth/refresh-token`, { refreshToken })

        if (data?.accessToken) {
          localStorage.setItem('accessToken', data.accessToken)
          if (data.refreshToken) localStorage.setItem('refreshToken', data.refreshToken)
          processQueue(null, data.accessToken)
          originalRequest.headers.Authorization = `Bearer ${data.accessToken}`
          return client(originalRequest)
        }

        throw new Error('Refresh failed')
      } catch (refreshError) {
        processQueue(refreshError, null)
        localStorage.clear()
        window.location.href = '/login'
        return Promise.reject(refreshError)
      } finally {
        isRefreshing = false
      }
    }

    return Promise.reject(error)
  },
)

export default client
