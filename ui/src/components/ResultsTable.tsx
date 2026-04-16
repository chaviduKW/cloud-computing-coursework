import { useState } from 'react'
import { Table, Tag, Tooltip, Button, message } from 'antd'
import { LikeOutlined, DislikeOutlined } from '@ant-design/icons'
import type { ColumnsType } from 'antd/es/table'
import { castVote } from '../api/voteApi'
import { useAuth } from '../context/AuthContext'
import type { SalaryRecord, SearchResult } from '../types'

interface Props {
  result: SearchResult | null
  loading: boolean
  onPageChange: (page: number, pageSize: number) => void
}

const LEVEL_COLORS: Record<string, string> = {
  Junior: 'green', Mid: 'blue', Senior: 'purple', Lead: 'orange',
  Principal: 'red', Staff: 'cyan', Manager: 'volcano', Director: 'magenta',
}

export default function ResultsTable({ result, loading, onPageChange }: Props) {
  const { isAuthenticated, user } = useAuth()
  const [votingId, setVotingId] = useState<string | null>(null)

  const handleVote = async (record: SalaryRecord, voteType: 'UPVOTE' | 'DOWNVOTE') => {
    if (!user) return
    setVotingId(`${record.id}-${voteType}`)
    try {
      await castVote({ salarySubmissionId: record.id, userId: user.userId, voteType })
      void message.success('Vote recorded!')
    } catch {
      void message.error('Failed to cast vote.')
    } finally {
      setVotingId(null)
    }
  }

  const columns: ColumnsType<SalaryRecord> = [
    { title: 'Company', dataIndex: 'company', key: 'company', ellipsis: true },
    { title: 'Role', dataIndex: 'role', key: 'role', ellipsis: true },
    { title: 'Country', dataIndex: 'country', key: 'country' },
    {
      title: 'Level',
      dataIndex: 'experienceLevel',
      key: 'experienceLevel',
      render: (level: string) => <Tag color={LEVEL_COLORS[level] ?? 'default'}>{level}</Tag>,
    },
    {
      title: 'Salary',
      key: 'salary',
      align: 'right',
      render: (_, r) => <strong>{r.currency} {r.salaryAmount.toLocaleString()}</strong>,
    },
    {
      title: 'Votes',
      key: 'votes',
      align: 'center',
      render: (_, record) => (
        <span style={{ display: 'flex', gap: 8, justifyContent: 'center', alignItems: 'center' }}>
          <Tooltip title={isAuthenticated ? 'Upvote' : 'Sign in to vote'}>
            <Button
              type="text"
              size="small"
              icon={<LikeOutlined />}
              style={{ color: '#52c41a' }}
              loading={votingId === `${record.id}-UPVOTE`}
              disabled={!isAuthenticated}
              onClick={() => handleVote(record, 'UPVOTE')}
            >
              {record.upVotes}
            </Button>
          </Tooltip>
          <Tooltip title={isAuthenticated ? 'Downvote' : 'Sign in to vote'}>
            <Button
              type="text"
              size="small"
              icon={<DislikeOutlined />}
              style={{ color: '#ff4d4f' }}
              loading={votingId === `${record.id}-DOWNVOTE`}
              disabled={!isAuthenticated}
              onClick={() => handleVote(record, 'DOWNVOTE')}
            >
              {record.downVotes}
            </Button>
          </Tooltip>
        </span>
      ),
    },
    {
      title: 'Submitted',
      dataIndex: 'createdAt',
      key: 'createdAt',
      render: (d: string) => new Date(d).toLocaleDateString(),
    },
  ]

  return (
    <Table<SalaryRecord>
      rowKey="id"
      columns={columns}
      dataSource={result?.results ?? []}
      loading={loading}
      scroll={{ x: 800 }}
      pagination={
        result
          ? {
              current: result.page,
              pageSize: result.pageSize,
              total: result.totalCount,
              showSizeChanger: true,
              showTotal: (t) => `${t} records`,
              onChange: onPageChange,
            }
          : false
      }
    />
  )
}
