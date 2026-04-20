import client from './client'
import type { VoteRequest, VoteResponse, VotesResponse } from '../types'

const BASE = '/api/vote'

export async function castVote(body: VoteRequest): Promise<VoteResponse> {
  const { data } = await client.post<VoteResponse>(BASE, body)
  return data
}

// Get votes for a specific submission, or all votes for a user if only userId is provided
export async function getVotes(submissionId?: string, userId?: string): Promise<VotesResponse> {
  const params: Record<string, string> = {}
  if (submissionId) params.submissionId = submissionId
  if (userId) params.userId = userId
  const { data } = await client.get<VotesResponse>(BASE, { params })
  return data
}
