import publicClient from './publicClient'
import client from './client'
import type { SalarySubmission, SalarySubmissionRequest } from '../types'

const BASE = '/api/salaries'

export async function submitSalary(body: SalarySubmissionRequest): Promise<SalarySubmission> {
  const { data } = await publicClient.post<SalarySubmission>(BASE, body)
  return data
}

export async function getPendingSubmissions(): Promise<SalarySubmission[]> {
  const { data } = await client.get<SalarySubmission[]>(`${BASE}/pending`)
  return data
}

export async function approveSubmission(submissionId: string): Promise<void> {
  await client.post(`${BASE}/approve/${submissionId}`)
}

export interface SalaryFilter {
  role?: string
  country?: string
  company?: string
  experienceLevel?: string
}

export async function getApprovedSalaries(filter?: SalaryFilter): Promise<SalarySubmission[]> {
  const params: Record<string, string> = {}
  if (filter?.role) params.role = filter.role
  if (filter?.country) params.country = filter.country
  if (filter?.company) params.company = filter.company
  if (filter?.experienceLevel) params.experienceLevel = filter.experienceLevel
  const { data } = await client.get<SalarySubmission[]>(`${BASE}/approved`, { params })
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
