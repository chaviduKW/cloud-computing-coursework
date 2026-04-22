// ── Auth ────────────────────────────────────────────────────────────────────

export interface RegisterRequest {
  firstName: string
  lastName: string
  email: string
  password: string
  confirmPassword: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface UserDto {
  userId: string
  firstName: string
  lastName: string
  email: string
  role: string
  isActive: boolean
}

export interface AuthResponse {
  success: boolean
  message: string
  accessToken?: string
  refreshToken?: string
  user?: UserDto
  expiresAt?: string
}

// ── Salary Submission ────────────────────────────────────────────────────────

export interface SalarySubmissionRequest {
  country: string
  company: string
  role: string
  experienceLevel: string
  salaryAmount: number
  currency: string
  anonymize: boolean
}

export interface SalarySubmission {
  id: string
  country: string
  company: string
  role: string
  experienceLevel: string
  salaryAmount: number
  currency: string
  anonymize: boolean
  status: string
  createdAt: string
}

// ── Search ───────────────────────────────────────────────────────────────────

export interface SearchQuery {
  company?: string
  designation?: string
  location?: string
  experienceLevel?: string
  minSalary?: number
  maxSalary?: number
  submittedAfter?: string
  submittedBefore?: string
  sortBy?: 'date' | 'salary' | 'votes'
  sortOrder?: 'asc' | 'desc'
  page?: number
  pageSize?: number
}

export interface SalaryRecord {
  id: string
  company: string
  role: string
  salaryAmount: number
  currency: string
  country: string
  experienceLevel: string
  upVotes: number
  downVotes: number
  totalVotes: number
  createdAt: string
}

export interface SearchResult {
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
  results: SalaryRecord[]
}

// ── Stats ────────────────────────────────────────────────────────────────────

export interface StatsResponse {
  role: string
  country: string
  company: string
  experienceLevel: string
  averageSalary: number
  medianSalary: number
  p25Salary: number
  p75Salary: number
  recordCount: number
  generatedAt: string
}

// ── Vote ─────────────────────────────────────────────────────────────────────

export interface VoteRequest {
  salarySubmissionId: string
  userId: string
  voteType: 'UPVOTE' | 'DOWNVOTE'
}

export interface VoteResponse {
  salarySubmissionId: string
  totalVotes: number
}

export interface VoteDto {
  userId: string
  voteType: string
  createdAt: string
}

export interface VotesResponse {
  votes: VoteDto[]
  totalVotes: number
}
