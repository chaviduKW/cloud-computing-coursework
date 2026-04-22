import { useState } from 'react'
import { Alert, Button, Card, Divider, Form, Input, Typography } from 'antd'
import { LockOutlined, MailOutlined, UserOutlined } from '@ant-design/icons'
import { Link, useNavigate } from 'react-router-dom'
import * as yup from 'yup'
import { register } from '../api/authApi'
import { useAuth } from '../context/AuthContext'
import { registerSchema, yupRule, type RegisterFormValues } from '../utils/validation'
import { useToast } from '../hooks/useToast'

const { Title, Text } = Typography

export default function RegisterPage() {
  const [loading, setLoading] = useState(false)
  const [serverError, setServerError] = useState<string | null>(null)
  const { setAuth } = useAuth()
  const navigate = useNavigate()
  const [form] = Form.useForm<RegisterFormValues>()
  const toast = useToast()

  const handleFinish = async (values: RegisterFormValues) => {
    // Full schema validation with yup before hitting the API
    try {
      await registerSchema.validate(values, { abortEarly: false })
    } catch (err) {
      if (err instanceof yup.ValidationError) {
        form.setFields(
          err.inner.map((e) => ({
            name: e.path as (keyof RegisterFormValues),
            errors: [e.message],
          }))
        )
        return
      }
    }

    setLoading(true)
    setServerError(null)
    try {
      const res = await register(values)
      if (!res.success) {
        setServerError(res.message || 'Registration failed. Please try again.')
        return
      }
      // API returned tokens — auto-login the user
      if (res.accessToken && res.refreshToken && res.user) {
        setAuth(res.accessToken, res.refreshToken, res.user)
        toast.success(res.message || 'Account created! Welcome aboard.')
        navigate('/')
      } else {
        // Registration successful but no tokens — redirect to login
        toast.success(res.message || 'Account created! Please sign in.')
        navigate('/login')
      }
    } catch {
      setServerError('Unable to register. Please try again later.')
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
        padding: '24px 0',
      }}
    >
      <Card style={{ width: 440, boxShadow: '0 4px 12px rgba(0,0,0,0.1)' }}>
        <Title level={3} style={{ textAlign: 'center', marginBottom: 4 }}>
          Create Account
        </Title>
        <Text type="secondary" style={{ display: 'block', textAlign: 'center', marginBottom: 24 }}>
          Join TechSalary to share and explore salaries
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
          <div style={{ display: 'flex', gap: 12 }}>
            <Form.Item
              name="firstName"
              label="First Name"
              style={{ flex: 1, marginBottom: 12 }}
              rules={[yupRule(registerSchema.fields.firstName as yup.StringSchema)]}
              validateTrigger={['onBlur', 'onChange']}
            >
              <Input prefix={<UserOutlined />} placeholder="John" autoComplete="given-name" />
            </Form.Item>

            <Form.Item
              name="lastName"
              label="Last Name"
              style={{ flex: 1, marginBottom: 12 }}
              rules={[yupRule(registerSchema.fields.lastName as yup.StringSchema)]}
              validateTrigger={['onBlur', 'onChange']}
            >
              <Input prefix={<UserOutlined />} placeholder="Doe" autoComplete="family-name" />
            </Form.Item>
          </div>

          <Form.Item
            name="email"
            label="Email Address"
            rules={[yupRule(registerSchema.fields.email as yup.StringSchema)]}
            validateTrigger={['onBlur', 'onChange']}
          >
            <Input prefix={<MailOutlined />} placeholder="you@example.com" autoComplete="email" />
          </Form.Item>

          <Form.Item
            name="password"
            label="Password"
            rules={[yupRule(registerSchema.fields.password as yup.StringSchema)]}
            validateTrigger={['onBlur', 'onChange']}
            extra="Min 8 characters, one uppercase letter and one number"
          >
            <Input.Password
              prefix={<LockOutlined />}
              placeholder="Create a strong password"
              autoComplete="new-password"
            />
          </Form.Item>

          <Form.Item
            name="confirmPassword"
            label="Confirm Password"
            dependencies={['password']}
            rules={[
              { required: true, message: 'Please confirm your password' },
              ({ getFieldValue }) => ({
                validator: async (_, value) => {
                  if (!value || getFieldValue('password') === value) return
                  throw new Error('Passwords do not match')
                },
              }),
            ]}
            validateTrigger={['onBlur', 'onChange']}
          >
            <Input.Password
              prefix={<LockOutlined />}
              placeholder="Re-enter your password"
              autoComplete="new-password"
            />
          </Form.Item>

          <Form.Item style={{ marginBottom: 8 }}>
            <Button type="primary" htmlType="submit" loading={loading} block>
              Create Account
            </Button>
          </Form.Item>
        </Form>

        <Divider plain>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Already have an account?
          </Text>
        </Divider>

        <div style={{ textAlign: 'center' }}>
          <Text type="secondary">
            Sign in to your account{' '}
            <Link to="/login">
              <strong>Sign In</strong>
            </Link>
          </Text>
        </div>
      </Card>
    </div>
  )
}
