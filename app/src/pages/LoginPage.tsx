import { useState } from 'react'
import { Alert, Button, Card, Divider, Form, Input, Typography } from 'antd'
import { LockOutlined, MailOutlined } from '@ant-design/icons'
import { Link, useNavigate } from 'react-router-dom'
import * as yup from 'yup'
import type { Schema } from 'yup'
import { login } from '../api/authApi'
import { useAuth } from '../context/AuthContext'
import { loginSchema, yupRule, type LoginFormValues } from '../utils/validation'
import { useToast } from '../hooks/useToast'

const { Title, Text } = Typography

export default function LoginPage() {
  const [loading, setLoading] = useState(false)
  const [serverError, setServerError] = useState<string | null>(null)
  const { setAuth } = useAuth()
  const navigate = useNavigate()
  const [form] = Form.useForm<LoginFormValues>()
  const toast = useToast()

  const handleFinish = async (values: LoginFormValues) => {
    // Full schema validation with yup before hitting the API
    try {
      await loginSchema.validate(values, { abortEarly: false })
    } catch (err) {
      if (err instanceof yup.ValidationError) {
        form.setFields(
          err.inner.map((e) => ({ name: e.path as 'email' | 'password', errors: [e.message] })),
        )
        return
      }
    }

    setLoading(true)
    setServerError(null)
    try {
      const res = await login(values)
      if (!res.success || !res.accessToken || !res.refreshToken || !res.user) {
        setServerError(res.message || 'Login failed. Please check your credentials.')
        return
      }
      setAuth(res.accessToken, res.refreshToken, res.user)
      toast.success(res.message || `Welcome back, ${res.user.firstName}!`)
      navigate('/')
    } catch {
      setServerError('Unable to connect. Please try again later.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div
      style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        minHeight: '100vh',
        background: '#f0f2f5',
      }}
    >
      <Card style={{ width: 400, boxShadow: '0 4px 12px rgba(0,0,0,0.1)' }}>
        <Title level={3} style={{ textAlign: 'center', marginBottom: 4 }}>
          Sign In
        </Title>
        <Text type="secondary" style={{ display: 'block', textAlign: 'center', marginBottom: 24 }}>
          Enter your credentials to continue
        </Text>

        {serverError && (
          <Alert
            type="error"
            message={serverError}
            showIcon
            closable
            onClose={() => setServerError(null)}
            style={{ marginBottom: 16 }}
          />
        )}

        <Form form={form} layout="vertical" onFinish={handleFinish} size="large">
          <Form.Item
            name="email"
            label="Email Address"
            rules={[yupRule(loginSchema.fields.email as Schema)]}
            validateTrigger={['onBlur', 'onChange']}
          >
            <Input prefix={<MailOutlined />} placeholder="you@example.com" autoComplete="email" />
          </Form.Item>

          <Form.Item
            name="password"
            label="Password"
            rules={[yupRule(loginSchema.fields.password as Schema)]}
            validateTrigger={['onBlur', 'onChange']}
          >
            <Input.Password
              prefix={<LockOutlined />}
              placeholder="Your password"
              autoComplete="current-password"
            />
          </Form.Item>

          <Form.Item style={{ marginBottom: 8 }}>
            <Button type="primary" htmlType="submit" loading={loading} block>
              Sign In
            </Button>
          </Form.Item>
        </Form>

        <Divider plain>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Don't have an account?
          </Text>
        </Divider>

        <div style={{ textAlign: 'center' }}>
          <Text type="secondary">
            New to TechSalary?{' '}
            <Link to="/register">
              <strong>Create an account</strong>
            </Link>
          </Text>
        </div>
      </Card>
    </div>
  )
}
