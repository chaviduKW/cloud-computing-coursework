import client from './client'
import type { AuthResponse, LoginRequest, RegisterRequest, UserDto } from '../types'

const BASE = '/api/auth'

export async function register(body: RegisterRequest): Promise<AuthResponse> {
  const { data } = await client.post<AuthResponse>(`${BASE}/register`, body)
  return data
}

export async function login(body: LoginRequest): Promise<AuthResponse> {
  const { data } = await client.post<AuthResponse>(`${BASE}/login`, body)
  return data
}

export async function logout(refreshToken: string): Promise<void> {
  await client.post(`${BASE}/logout`, { refreshToken })
}

export async function getMe(): Promise<UserDto> {
  const { data } = await client.get<UserDto>(`${BASE}/me`)
  return data
}
