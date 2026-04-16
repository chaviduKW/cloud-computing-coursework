import client from './client'
import type { VoteRequest, VoteResponse, VotesResponse } from '../types'

const BASE = '/api/vote'

export async function castVote(body: VoteRequest): Promise<VoteResponse> {
  const { data } = await client.post<VoteResponse>(BASE, body)
  return data
}

export async function getVotes(submissionId: string): Promise<VotesResponse> {
  const { data } = await client.get<VotesResponse>(`${BASE}/${submissionId}`)
  return data
}
