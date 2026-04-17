import axios from 'axios'
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

export async function logout(refreshToken: string): Promise<AuthResponse> {
  const { data } = await client.post<AuthResponse>(`${BASE}/logout`, { refreshToken })
  return data
}

export async function getMe(): Promise<UserDto> {
  const { data } = await client.get<UserDto>(`${BASE}/me`)
  return data
}

// Uses plain axios (not intercepted client) to avoid 401 retry loop
export async function refreshToken(refreshToken: string): Promise<AuthResponse> {
  const { data } = await axios.post<AuthResponse>(`${BASE}/refresh-token`, { refreshToken })
  return data
}

export async function getUsers(): Promise<UserDto[]> {
  const { data } = await client.get<UserDto[]>(`${BASE}/users`)
  return data
}
