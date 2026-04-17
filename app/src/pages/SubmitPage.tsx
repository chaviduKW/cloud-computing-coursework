import { useEffect, useState } from 'react'
import { Button, Card, Col, Form, InputNumber, Row, Select, Switch, message, Typography, Result } from 'antd'
import { submitSalary } from '../api/salaryApi'
import { getCompanies, getDesignations } from '../api/searchApi'
import type { SalarySubmissionRequest } from '../types'

const CURRENCIES = ['USD', 'EUR', 'GBP', 'AUD', 'CAD', 'SGD', 'INR', 'JPY', 'LKR']

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

export default function SubmitPage() {
  const [form] = Form.useForm<SalarySubmissionRequest>()
  const [loading, setLoading] = useState(false)
  const [submitted, setSubmitted] = useState(false)
  const [companies, setCompanies] = useState<string[]>([])
  const [designations, setDesignations] = useState<string[]>([])

  useEffect(() => {
    getCompanies().then(setCompanies).catch(() => {})
    getDesignations().then(setDesignations).catch(() => {})
  }, [])

  const handleFinish = async (values: SalarySubmissionRequest) => {
    setLoading(true)
    try {
      await submitSalary(values)
      setSubmitted(true)
      form.resetFields()
    } catch {
      void message.error('Submission failed. Please try again.')
    } finally {
      setLoading(false)
    }
  }

  if (submitted) {
    return (
      <Result
        status="success"
        title="Salary Submitted!"
        subTitle="Your submission is pending review and will appear in search results once approved."
        extra={
          <Button type="primary" onClick={() => setSubmitted(false)}>
            Submit Another
          </Button>
        }
      />
    )
  }

  return (
    <>
      <Typography.Title level={4} style={{ marginBottom: 24 }}>
        Submit a Salary
      </Typography.Title>
      <Card style={{ maxWidth: 700 }}>
        <Form
          form={form}
          layout="vertical"
          onFinish={handleFinish}
          initialValues={{ currency: 'USD', anonymize: true }}
          size="large"
        >
          <Row gutter={16}>
            <Col xs={24} sm={12}>
              <Form.Item name="company" label="Company" rules={[{ required: true }]}>
                <Select
                  showSearch
                  placeholder="Select company"
                  filterOption={(input, option) =>
                    (option?.value as string).toLowerCase().includes(input.toLowerCase())
                  }
                  options={companies.map((c) => ({ value: c, label: c }))}
                />
              </Form.Item>
            </Col>
            <Col xs={24} sm={12}>
              <Form.Item name="role" label="Role / Designation" rules={[{ required: true }]}>
                <Select
                  showSearch
                  placeholder="Select designation"
                  filterOption={(input, option) =>
                    (option?.value as string).toLowerCase().includes(input.toLowerCase())
                  }
                  options={designations.map((d) => ({ value: d, label: d }))}
                />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col xs={24} sm={12}>
              <Form.Item name="country" label="Country" rules={[{ required: true }]}>
                <Select
                  showSearch
                  placeholder="Select country"
                  filterOption={(input, option) =>
                    (option?.value as string).toLowerCase().includes(input.toLowerCase())
                  }
                  options={COUNTRIES.map((c) => ({ value: c, label: c }))}
                />
              </Form.Item>
            </Col>
            <Col xs={24} sm={12}>
              <Form.Item name="experienceYears" label="Years of Experience" rules={[{ required: true }]}>
                <InputNumber
                  style={{ width: '100%' }}
                  min={0}
                  max={50}
                  placeholder="e.g. 5"
                  addonAfter="yrs"
                />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col xs={24} sm={12}>
              <Form.Item name="salaryAmount" label="Annual Salary" rules={[{ required: true }]}>
                <InputNumber
                  style={{ width: '100%' }}
                  min={1}
                  placeholder="e.g. 120000"
                  formatter={(v) => (v ? String(v).replace(/\B(?=(\d{3})+(?!\d))/g, ',') : '')}
                />
              </Form.Item>
            </Col>
            <Col xs={24} sm={12}>
              <Form.Item name="currency" label="Currency" rules={[{ required: true }]}>
                <Select>
                  {CURRENCIES.map((c) => (
                    <Select.Option key={c} value={c}>{c}</Select.Option>
                  ))}
                </Select>
              </Form.Item>
            </Col>
          </Row>

          <Form.Item name="anonymize" label="Submit Anonymously" valuePropName="checked">
            <Switch />
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit" loading={loading}>
              Submit
            </Button>
          </Form.Item>
        </Form>
      </Card>
    </>
  )
}
