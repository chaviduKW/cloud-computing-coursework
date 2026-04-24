import { useEffect, useState } from 'react'
import {
  Button,
  Col,
  DatePicker,
  Form,
  InputNumber,
  Row,
  Select,
} from 'antd'
import { SearchOutlined, ClearOutlined } from '@ant-design/icons'
import dayjs from 'dayjs'
import { getCompanies, getDesignations } from '../api/salaryApi'
import type { SearchQuery } from '../types'

const { RangePicker } = DatePicker

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

interface Props {
  onSearch: (query: SearchQuery) => void
  loading: boolean
}

const EXPERIENCE_LEVELS = ['Entry','Junior', 'Mid', 'Senior']

interface FormValues {
  company?: string
  designation?: string
  location?: string
  experienceLevel?: string
  minSalary?: number
  maxSalary?: number
  dateRange?: [dayjs.Dayjs, dayjs.Dayjs] | null
  sortBy?: 'date' | 'salary'
  sortOrder?: 'asc' | 'desc'
}

export default function SearchFilters({ onSearch, loading }: Props) {
  const [form] = Form.useForm<FormValues>()
  const [companies, setCompanies] = useState<string[]>([])
  const [designations, setDesignations] = useState<string[]>([])
  const [loadingOptions, setLoadingOptions] = useState(true)

  useEffect(() => {
    Promise.allSettled([getCompanies(), getDesignations()]).then(([c, d]) => {
      if (c.status === 'fulfilled') setCompanies(c.value)
      if (d.status === 'fulfilled') setDesignations(d.value)
      setLoadingOptions(false)
    })
  }, [])

  const handleFinish = (values: FormValues) => {
    const query: SearchQuery = {
      company: values.company || undefined,
      designation: values.designation || undefined,
      location: values.location || undefined,
      experienceLevel: values.experienceLevel || undefined,
      minSalary: values.minSalary ?? undefined,
      maxSalary: values.maxSalary ?? undefined,
      submittedAfter: values.dateRange?.[0]?.toISOString() ?? undefined,
      submittedBefore: values.dateRange?.[1]?.toISOString() ?? undefined,
      sortBy: values.sortBy ?? 'date',
      sortOrder: values.sortOrder ?? 'desc',
      page: 1,
      pageSize: 20,
    }
    onSearch(query)
  }

  const handleReset = () => {
    form.resetFields()
    onSearch({ sortBy: 'date', sortOrder: 'desc', page: 1, pageSize: 20 })
  }

  return (
    <Form
      form={form}
      layout="vertical"
      onFinish={handleFinish}
      initialValues={{ sortBy: 'date', sortOrder: 'desc' }}
    >
      <Row gutter={16}>
        <Col xs={24} sm={12} md={6}>
          <Form.Item name="company" label="Company">
            <Select
              showSearch
              allowClear
              loading={loadingOptions}
              placeholder="Any company"
              filterOption={(input, option) =>
                (option?.value as string).toLowerCase().includes(input.toLowerCase())
              }
              options={companies.map((c) => ({ value: c, label: c }))}
            />
          </Form.Item>
        </Col>
        <Col xs={24} sm={12} md={6}>
          <Form.Item name="designation" label="Designation">
            <Select
              showSearch
              allowClear
              loading={loadingOptions}
              placeholder="Any role"
              filterOption={(input, option) =>
                (option?.value as string).toLowerCase().includes(input.toLowerCase())
              }
              options={designations.map((d) => ({ value: d, label: d }))}
            />
          </Form.Item>
        </Col>
        <Col xs={24} sm={12} md={6}>
          <Form.Item name="location" label="Location">
            <Select
              showSearch
              allowClear
              placeholder="Any country"
              filterOption={(input, option) =>
                (option?.value as string).toLowerCase().includes(input.toLowerCase())
              }
              options={COUNTRIES.map((c) => ({ value: c, label: c }))}
            />
          </Form.Item>
        </Col>
        <Col xs={24} sm={12} md={6}>
          <Form.Item name="experienceLevel" label="Experience Level">
            <Select allowClear placeholder="Any level">
              {EXPERIENCE_LEVELS.map((l) => (
                <Select.Option key={l} value={l}>{l}</Select.Option>
              ))}
            </Select>
          </Form.Item>
        </Col>
      </Row>

      <Row gutter={16}>
        <Col xs={24} sm={12} md={6}>
          <Form.Item label="Min Salary" name="minSalary">
            <InputNumber
              style={{ width: '100%' }}
              placeholder="0"
              min={0}
              formatter={(v) => (v ? `${v}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',') : '')}
            />
          </Form.Item>
        </Col>
        <Col xs={24} sm={12} md={6}>
          <Form.Item label="Max Salary" name="maxSalary">
            <InputNumber
              style={{ width: '100%' }}
              placeholder="No limit"
              min={0}
              formatter={(v) => (v ? `${v}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',') : '')}
            />
          </Form.Item>
        </Col>
        <Col xs={24} sm={12} md={6}>
          <Form.Item name="dateRange" label="Submitted Between">
            <RangePicker style={{ width: '100%' }} />
          </Form.Item>
        </Col>
        <Col xs={24} sm={12} md={3}>
          <Form.Item name="sortBy" label="Sort By">
            <Select>
              <Select.Option value="date">Date</Select.Option>
              <Select.Option value="salary">Salary</Select.Option>
            </Select>
          </Form.Item>
        </Col>
        <Col xs={24} sm={12} md={3}>
          <Form.Item name="sortOrder" label="Order">
            <Select>
              <Select.Option value="desc">Desc</Select.Option>
              <Select.Option value="asc">Asc</Select.Option>
            </Select>
          </Form.Item>
        </Col>
      </Row>

      <Row justify="end" gutter={8}>
        <Col>
          <Button icon={<ClearOutlined />} onClick={handleReset}>
            Reset
          </Button>
        </Col>
        <Col>
          <Button type="primary" htmlType="submit" icon={<SearchOutlined />} loading={loading}>
            Search
          </Button>
        </Col>
      </Row>
    </Form>
  )
}
