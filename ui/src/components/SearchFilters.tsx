import { useEffect, useState } from 'react'
import {
  Button,
  Col,
  DatePicker,
  Form,
  InputNumber,
  Row,
  Select,
  AutoComplete,
} from 'antd'
import { SearchOutlined, ClearOutlined } from '@ant-design/icons'
import dayjs from 'dayjs'
import { getCompanies, getDesignations } from '../api/searchApi'
import type { SearchQuery } from '../types'

const { RangePicker } = DatePicker

const EXPERIENCE_LEVELS = ['Junior', 'Mid', 'Senior', 'Lead', 'Principal', 'Staff', 'Manager', 'Director']

interface Props {
  onSearch: (query: SearchQuery) => void
  loading: boolean
}

interface FormValues {
  company?: string
  designation?: string
  location?: string
  experienceLevel?: string
  salaryRange?: [number | null, number | null]
  dateRange?: [dayjs.Dayjs, dayjs.Dayjs] | null
  sortBy?: 'date' | 'salary' | 'votes'
  sortOrder?: 'asc' | 'desc'
}

export default function SearchFilters({ onSearch, loading }: Props) {
  const [form] = Form.useForm<FormValues>()
  const [companies, setCompanies] = useState<string[]>([])
  const [designations, setDesignations] = useState<string[]>([])

  useEffect(() => {
    getCompanies().then(setCompanies).catch(() => {})
    getDesignations().then(setDesignations).catch(() => {})
  }, [])

  const handleFinish = (values: FormValues) => {
    const query: SearchQuery = {
      company: values.company || undefined,
      designation: values.designation || undefined,
      location: values.location || undefined,
      experienceLevel: values.experienceLevel || undefined,
      minSalary: values.salaryRange?.[0] ?? undefined,
      maxSalary: values.salaryRange?.[1] ?? undefined,
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
            <AutoComplete
              options={companies.map((c) => ({ value: c }))}
              filterOption={(input, option) =>
                (option?.value as string).toLowerCase().includes(input.toLowerCase())
              }
              placeholder="Any company"
              allowClear
            />
          </Form.Item>
        </Col>
        <Col xs={24} sm={12} md={6}>
          <Form.Item name="designation" label="Designation">
            <AutoComplete
              options={designations.map((d) => ({ value: d }))}
              filterOption={(input, option) =>
                (option?.value as string).toLowerCase().includes(input.toLowerCase())
              }
              placeholder="Any role"
              allowClear
            />
          </Form.Item>
        </Col>
        <Col xs={24} sm={12} md={6}>
          <Form.Item name="location" label="Location">
            <AutoComplete placeholder="Any country" allowClear />
          </Form.Item>
        </Col>
        <Col xs={24} sm={12} md={6}>
          <Form.Item name="experienceLevel" label="Experience Level">
            <Select placeholder="Any level" allowClear>
              {EXPERIENCE_LEVELS.map((lvl) => (
                <Select.Option key={lvl} value={lvl}>
                  {lvl}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>
        </Col>
      </Row>

      <Row gutter={16}>
        <Col xs={24} sm={12} md={6}>
          <Form.Item label="Min Salary" name={['salaryRange', 0]}>
            <InputNumber
              style={{ width: '100%' }}
              placeholder="0"
              min={0}
              formatter={(v) => (v ? `$ ${v}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',') : '')}
            />
          </Form.Item>
        </Col>
        <Col xs={24} sm={12} md={6}>
          <Form.Item label="Max Salary" name={['salaryRange', 1]}>
            <InputNumber
              style={{ width: '100%' }}
              placeholder="No limit"
              min={0}
              formatter={(v) => (v ? `$ ${v}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',') : '')}
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
              <Select.Option value="votes">Votes</Select.Option>
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
