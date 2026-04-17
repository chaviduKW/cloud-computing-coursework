import { useEffect, useState } from 'react'
import { Alert, Badge, Table, Tag, Typography } from 'antd'
import type { ColumnsType } from 'antd/es/table'
import { getPendingSubmissions } from '../api/salaryApi'
import type { SalarySubmission } from '../types'

const columns: ColumnsType<SalarySubmission> = [
  { title: 'Company', dataIndex: 'company', key: 'company', ellipsis: true },
  { title: 'Role', dataIndex: 'role', key: 'role', ellipsis: true },
  { title: 'Country', dataIndex: 'country', key: 'country' },
  {
    title: 'Level',
    dataIndex: 'experienceLevel',
    key: 'experienceLevel',
    render: (v: string) => <Tag color="blue">{v}</Tag>,
  },
  {
    title: 'Salary',
    key: 'salary',
    render: (_, r) => `${r.currency} ${r.salaryAmount.toLocaleString()}`,
    align: 'right',
  },
  {
    title: 'Status',
    dataIndex: 'status',
    key: 'status',
    render: (s: string) => (
      <Badge
        status={s === 'PENDING' ? 'processing' : s === 'APPROVED' ? 'success' : 'error'}
        text={s}
      />
    ),
  },
  {
    title: 'Anonymous',
    dataIndex: 'anonymize',
    key: 'anonymize',
    render: (v: boolean) => (v ? 'Yes' : 'No'),
  },
  {
    title: 'Submitted',
    dataIndex: 'createdAt',
    key: 'createdAt',
    render: (d: string) => new Date(d).toLocaleDateString(),
  },
]

export default function PendingPage() {
  const [submissions, setSubmissions] = useState<SalarySubmission[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    getPendingSubmissions()
      .then(setSubmissions)
      .catch(() => setError('Failed to load submissions.'))
      .finally(() => setLoading(false))
  }, [])

  return (
    <>
      <Typography.Title level={4} style={{ marginBottom: 24 }}>
        Pending Submissions
      </Typography.Title>
      {error && <Alert type="error" message={error} style={{ marginBottom: 16 }} />}
      <Table<SalarySubmission>
        rowKey="id"
        columns={columns}
        dataSource={submissions}
        loading={loading}
        scroll={{ x: 900 }}
        pagination={{ pageSize: 20, showTotal: (t) => `${t} submissions` }}
      />
    </>
  )
}
