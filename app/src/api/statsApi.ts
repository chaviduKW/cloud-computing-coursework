import client from './client'
import type { StatsResponse } from '../types'

interface StatsQuery {
  role?: string
  country?: string
  company?: string
  experienceLevel?: string
}

export async function getStats(query: StatsQuery): Promise<StatsResponse> {
  const params: Record<string, string> = {}
  if (query.role) params.role = query.role
  if (query.country) params.country = query.country
  if (query.company) params.company = query.company
  if (query.experienceLevel) params.experienceLevel = query.experienceLevel
  const { data } = await client.get<StatsResponse>('/api/stats', { params })

  return data
}
