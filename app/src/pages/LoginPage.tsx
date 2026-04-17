import { useState } from 'react'
import { Button, Card, Form, Input, message, Typography } from 'antd'
import { LockOutlined, MailOutlined } from '@ant-design/icons'
import { Link, useNavigate } from 'react-router-dom'
import { login } from '../api/authApi'
import { useAuth } from '../context/AuthContext'

export default function LoginPage() {
  const [loading, setLoading] = useState(false)
  const { setAuth } = useAuth()
  const navigate = useNavigate()

  const handleFinish = async (values: { email: string; password: string }) => {
    setLoading(true)
    try {
      const res = await login(values)
      if (!res.success || !res.accessToken || !res.refreshToken || !res.user) {
        void message.error(res.message || 'Login failed')
        return
      }
      setAuth(res.accessToken, res.refreshToken, res.user)
      void message.success(`Welcome back, ${res.user.firstName}!`)
      navigate('/')
    } catch {
      void message.error('Login failed. Check your credentials.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh', background: '#f0f2f5' }}>
      <Card style={{ width: 380 }}>
        <Typography.Title level={3} style={{ textAlign: 'center', marginBottom: 24 }}>
          Sign In
        </Typography.Title>
        <Form layout="vertical" onFinish={handleFinish} size="large">
          <Form.Item name="email" rules={[{ required: true, type: 'email' }]}>
            <Input prefix={<MailOutlined />} placeholder="Email" />
          </Form.Item>
          <Form.Item name="password" rules={[{ required: true }]}>
            <Input.Password prefix={<LockOutlined />} placeholder="Password" />
          </Form.Item>
          <Form.Item>
            <Button type="primary" htmlType="submit" loading={loading} block>
              Sign In
            </Button>
          </Form.Item>
        </Form>
        <Typography.Text type="secondary">
          No account? <Link to="/register">Register</Link>
        </Typography.Text>
      </Card>
    </div>
  )
}
