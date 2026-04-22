import { useState } from 'react'
import './ResultsTable.css'
import { Table, Tooltip, Button, message } from 'antd'
import { LikeOutlined, DislikeOutlined } from '@ant-design/icons'
import type { ColumnsType } from 'antd/es/table'
import { castVote } from '../api/voteApi'
import { useAuth } from '../context/AuthContext'
import type { SalaryRecord, SearchResult } from '../types'

interface Props {
  result: SearchResult | null
  loading: boolean
  onPageChange: (page: number, pageSize: number) => void
  userVotes?: Record<string, 'UpVote' | 'DownVote'>
}


export default function ResultsTable({ result, loading, onPageChange, userVotes }: Props) {
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
      title: 'Exp. Level',
      dataIndex: 'experienceLevel',
      key: 'experienceLevel',
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
      render: (_, record) => {
        const voted = userVotes && userVotes[record.id];
        return (
          <span style={{ display: 'flex', gap: 8, justifyContent: 'center', alignItems: 'center' }}>
            <Tooltip title={isAuthenticated ? 'Upvote' : 'Sign in to vote'}>
              <Button
                type="text"
                size="small"
                icon={
                  <span style={{
                    display: 'inline-flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    width: 32,
                    height: 32,
                    borderRadius: '50%',
                    background: voted === 'UpVote' ? '#e6f7ff' : 'transparent',
                  }}>
                    <LikeOutlined style={{ color: voted === 'UpVote' ? '#1890ff' : '#52c41a', fontSize: 18 }} />
                  </span>
                }
                style={{ color: voted === 'UpVote' ? '#1890ff' : '#52c41a' }}
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
                icon={
                  <span style={{
                    display: 'inline-flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    width: 32,
                    height: 32,
                    borderRadius: '50%',
                    background: voted === 'DownVote' ? '#fff1f0' : 'transparent',
                  }}>
                    <DislikeOutlined style={{ color: voted === 'DownVote' ? '#d32029' : '#ff4d4f', fontSize: 18 }} />
                  </span>
                }
                style={{ color: voted === 'DownVote' ? '#d32029' : '#ff4d4f' }}
                loading={votingId === `${record.id}-DOWNVOTE`}
                disabled={!isAuthenticated}
                onClick={() => handleVote(record, 'DOWNVOTE')}
              >
                {record.downVotes}
              </Button>
            </Tooltip>
          </span>
        );
      },
    },
    {
      title: 'Status',
      dataIndex: 'status',
      key: 'status',
      align: 'center',
      render: (status: string) => status,
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
