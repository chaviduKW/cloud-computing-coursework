import client from './client'
import type { SalarySubmission, SalarySubmissionRequest } from '../types'

const BASE = '/api/salaries'

export async function submitSalary(body: SalarySubmissionRequest): Promise<SalarySubmission> {
  const { data } = await client.post<SalarySubmission>(BASE, body)
  return data
}

export async function getPendingSubmissions(): Promise<SalarySubmission[]> {
  const { data } = await client.get<SalarySubmission[]>(`${BASE}/pending`)
  return data
}
