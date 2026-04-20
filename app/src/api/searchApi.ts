import client from './client'
import type { SearchQuery, SearchResult } from '../types'

const BASE = '/api/search'

export async function search(query: SearchQuery): Promise<SearchResult> {
  const params: Record<string, string | number> = {}
  if (query.company) params.company = query.company
  if (query.designation) params.designation = query.designation
  if (query.location) params.location = query.location
  if (query.experienceLevel) params.experienceLevel = query.experienceLevel
  if (query.minSalary != null) params.minSalary = query.minSalary
  if (query.maxSalary != null) params.maxSalary = query.maxSalary
  if (query.submittedAfter) params.submittedAfter = query.submittedAfter
  if (query.submittedBefore) params.submittedBefore = query.submittedBefore
  if (query.sortBy) params.sortBy = query.sortBy
  if (query.sortOrder) params.sortOrder = query.sortOrder
  if (query.page != null) params.page = query.page
  if (query.pageSize != null) params.pageSize = query.pageSize
  const { data } = await client.get<SearchResult>(BASE, { params })
  return data
}

export async function getCompanies(): Promise<string[]> {
  const { data } = await client.get<string[]>(`${BASE}/companies`)
  return data
}

export async function getDesignations(): Promise<string[]> {
  const { data } = await client.get<string[]>(`${BASE}/designations`)
  return data
}
