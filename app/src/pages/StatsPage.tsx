import { useState, useEffect } from 'react'
import { Button, Card, Col, Form, Row, Select, Statistic, Typography, Alert, Spin } from 'antd'
import { BarChartOutlined } from '@ant-design/icons'
import { getStats } from '../api/statsApi'
import { getCompanies, getDesignations } from '../api/searchApi'
import type { StatsResponse } from '../types'

const EXPERIENCE_LEVELS = ['Entry','Junior', 'Mid', 'Senior']


const COUNTRIES = [
  'Afghanistan', 'Albania', 'Algeria', 'Argentina', 'Australia', 'Austria', 'Bangladesh',
  'Belgium', 'Brazil', 'Canada', 'Chile', 'China', 'Colombia', 'Czech Republic', 'Denmark',
  'Egypt', 'Ethiopia', 'Finland', 'France', 'Germany', 'Ghana', 'Greece', 'Hungary', 'India',
  'Indonesia', 'Iran', 'Iraq', 'Ireland', 'Israel', 'Italy', 'Japan', 'Jordan', 'Kenya',
  'Malaysia', 'Mexico', 'Morocco', 'Netherlands', 'New Zealand', 'Nigeria', 'Norway', 'Pakistan',
  'Peru', 'Philippines', 'Poland', 'Portugal', 'Romania', 'Russia', 'Saudi Arabia', 'Singapore',
  'South Africa', 'South Korea', 'Spain', 'Sri Lanka', 'Sweden', 'Switzerland', 'Taiwan',
  'Thailand', 'Turkey', 'Ukraine', 'United Arab Emirates', 'United Kingdom', 'United States',
  'Venezuela', 'Vietnam',
]

function formatSalary(val: number) {
  return `$${val.toLocaleString(undefined, { maximumFractionDigits: 0 })}`
}

export default function StatsPage() {
  const [loading, setLoading] = useState(false)
  const [stats, setStats] = useState<StatsResponse | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [companies, setCompanies] = useState<string[]>([])
  const [designations, setDesignations] = useState<string[]>([])

  // useEffect(() => {
  //   getCompanies().then(setCompanies).catch(() => {})
  // }, [])
  useEffect(() => {
      Promise.allSettled([getCompanies(), getDesignations()]).then(([c, d]) => {
        if (c.status === 'fulfilled') setCompanies(c.value)
        if (d.status === 'fulfilled') setDesignations(d.value)
        setLoading(false)
      })
    }, [])

  const handleFinish = async (values: {
    role?: string
    country?: string
    company?: string
    experienceLevel?: string
  }) => {
    setLoading(true)
    setError(null)
    try {
      const data = await getStats(values)
      setStats(data)
    } catch {
      setError('Failed to load stats. Make sure the Stats API is running.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <>
      <Typography.Title level={4} style={{ marginBottom: 24 }}>
        Salary Statistics
      </Typography.Title>

      <Card style={{ marginBottom: 24 }}>
        <Form layout="vertical" onFinish={handleFinish}>
          <Row gutter={16}>
            <Col xs={24} sm={6}>
              <Form.Item name="role" label="Designation">
                <Select
                  showSearch
                  allowClear
                  loading={loading}
                  placeholder="Any role"
                  filterOption={(input, option) =>
                    (option?.value as string).toLowerCase().includes(input.toLowerCase())
                  }
                  options={designations.map((d) => ({ value: d, label: d }))}
                />
            </Form.Item>
            </Col>
            <Col xs={24} sm={6}>
              <Form.Item name="country" label="Country">
                <Select
                  showSearch
                  placeholder="Select country"
                  allowClear
                  filterOption={(input, option) =>
                    (option?.value as string).toLowerCase().includes(input.toLowerCase())
                  }
                  options={COUNTRIES.map((c) => ({ value: c, label: c }))}
                />
              </Form.Item>
            </Col>
            <Col xs={24} sm={6}>
              <Form.Item name="company" label="Company">
                <Select
                  showSearch
                  placeholder="Select company"
                  allowClear
                  filterOption={(input, option) =>
                    (option?.value as string).toLowerCase().includes(input.toLowerCase())
                  }
                  options={companies.map((c) => ({ value: c, label: c }))}
                />
              </Form.Item>
            </Col>
            <Col xs={24} sm={6}>
              <Form.Item name="experienceLevel" label="Experience Level">
                <Select placeholder="Any level" allowClear>
                  {EXPERIENCE_LEVELS.map((l) => (
                    <Select.Option key={l} value={l}>{l}</Select.Option>
                  ))}
                </Select>
              </Form.Item>
            </Col>
          </Row>
          <Button type="primary" htmlType="submit" icon={<BarChartOutlined />} loading={loading}>
            Get Statistics
          </Button>
        </Form>
      </Card>

      {loading && <Spin size="large" style={{ display: 'block', margin: '40px auto' }} />}
      {error && <Alert type="error" message={error} style={{ marginBottom: 16 }} />}

      {stats && !loading && (
        <>
          <Row gutter={[16, 16]} style={{ marginBottom: 16 }}>
            <Col xs={12} sm={6}>
              <Card>
                <Statistic title="Average Salary" value={stats.averageSalary} formatter={(v) => formatSalary(Number(v))} />
              </Card>
            </Col>
            <Col xs={12} sm={6}>
              <Card>
                <Statistic title="Median Salary" value={stats.medianSalary} formatter={(v) => formatSalary(Number(v))} />
              </Card>
            </Col>
            <Col xs={12} sm={6}>
              <Card>
                <Statistic title="25th Percentile" value={stats.p25Salary} formatter={(v) => formatSalary(Number(v))} />
              </Card>
            </Col>
            <Col xs={12} sm={6}>
              <Card>
                <Statistic title="75th Percentile" value={stats.p75Salary} formatter={(v) => formatSalary(Number(v))} />
              </Card>
            </Col>
          </Row>
          <Card>
            <Row gutter={16}>
              <Col xs={12} sm={6}>
                <Statistic title="Total Records" value={stats.recordCount} />
              </Col>
              {stats.role && (
                <Col xs={12} sm={6}>
                  <Statistic title="Role" value={stats.role} />
                </Col>
              )}
              {stats.country && (
                <Col xs={12} sm={6}>
                  <Statistic title="Country" value={stats.country} />
                </Col>
              )}
              {stats.company && (
                <Col xs={12} sm={6}>
                  <Statistic title="Company" value={stats.company} />
                </Col>
              )}
              {stats.experienceLevel && (
                <Col xs={12} sm={6}>
                  <Statistic title="Experience Level" value={stats.experienceLevel} />
                </Col>
              )}
            </Row>
            <Typography.Text type="secondary" style={{ display: 'block', marginTop: 16 }}>
              Generated at {new Date(stats.generatedAt).toLocaleString()}
            </Typography.Text>
          </Card>
        </>
      )}
    </>
  )
}
