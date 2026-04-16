import { useState } from 'react'
import { Button, Card, Col, Form, Input, InputNumber, Row, Select, Switch, message, Typography, Result } from 'antd'
import { submitSalary } from '../api/salaryApi'
import type { SalarySubmissionRequest } from '../types'

const EXPERIENCE_LEVELS = ['Junior', 'Mid', 'Senior', 'Lead', 'Principal', 'Staff', 'Manager', 'Director']
const CURRENCIES = ['USD', 'EUR', 'GBP', 'AUD', 'CAD', 'SGD', 'INR', 'JPY', 'LKR']

export default function SubmitPage() {
  const [form] = Form.useForm<SalarySubmissionRequest>()
  const [loading, setLoading] = useState(false)
  const [submitted, setSubmitted] = useState(false)

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
                <Input placeholder="e.g. Google" />
              </Form.Item>
            </Col>
            <Col xs={24} sm={12}>
              <Form.Item name="role" label="Role / Designation" rules={[{ required: true }]}>
                <Input placeholder="e.g. Software Engineer" />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col xs={24} sm={12}>
              <Form.Item name="country" label="Country" rules={[{ required: true }]}>
                <Input placeholder="e.g. United States" />
              </Form.Item>
            </Col>
            <Col xs={24} sm={12}>
              <Form.Item name="experienceLevel" label="Experience Level" rules={[{ required: true }]}>
                <Select placeholder="Select level">
                  {EXPERIENCE_LEVELS.map((l) => (
                    <Select.Option key={l} value={l}>{l}</Select.Option>
                  ))}
                </Select>
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
