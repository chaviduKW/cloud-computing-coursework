import { useState } from 'react'
import { Avatar, Button, Dropdown, Layout, Menu, Typography } from 'antd'
import {
  SearchOutlined,
  BarChartOutlined,
  FormOutlined,
  DatabaseOutlined,
  LogoutOutlined,
  UserOutlined,
  LoginOutlined,
} from '@ant-design/icons'
import { Outlet, useNavigate, useLocation, Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { logout } from '../api/authApi'

const { Header, Sider, Content } = Layout

const NAV_ITEMS = [
  { key: '/', icon: <SearchOutlined />, label: 'Search' },
  { key: '/stats', icon: <BarChartOutlined />, label: 'Statistics' },
  { key: '/submit', icon: <FormOutlined />, label: 'Submit Salary' },
  { key: '/pending', icon: <DatabaseOutlined />, label: 'Pending' },
]

export default function AppLayout() {
  const { isAuthenticated, user, clearAuth, refreshToken } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const [collapsed, setCollapsed] = useState(false)

  const handleLogout = async () => {
    if (refreshToken) {
      try { await logout(refreshToken) } catch { /* ignore */ }
    }
    clearAuth()
    navigate('/login')
  }

  const userMenuItems = [
    {
      key: 'logout',
      icon: <LogoutOutlined />,
      label: 'Logout',
      onClick: handleLogout,
    },
  ]

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider collapsible collapsed={collapsed} onCollapse={setCollapsed} theme="dark">
        <div style={{ padding: '16px', textAlign: 'center' }}>
          <Typography.Text strong style={{ color: '#fff', fontSize: collapsed ? 12 : 16 }}>
            {collapsed ? 'TS' : 'TechSalary'}
          </Typography.Text>
        </div>
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={[location.pathname]}
          items={NAV_ITEMS.map((item) => ({
            key: item.key,
            icon: item.icon,
            label: <Link to={item.key}>{item.label}</Link>,
          }))}
        />
      </Sider>

      <Layout>
        <Header
          style={{
            background: '#fff',
            padding: '0 24px',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'flex-end',
            boxShadow: '0 1px 4px rgba(0,0,0,0.1)',
          }}
        >
          {isAuthenticated && user ? (
            <Dropdown menu={{ items: userMenuItems }} placement="bottomRight">
              <span style={{ cursor: 'pointer', display: 'flex', alignItems: 'center', gap: 8 }}>
                <Avatar icon={<UserOutlined />} />
                <Typography.Text>{user.firstName} {user.lastName}</Typography.Text>
              </span>
            </Dropdown>
          ) : (
            <Button icon={<LoginOutlined />} type="primary" onClick={() => navigate('/login')}>
              Sign In
            </Button>
          )}
        </Header>

        <Content style={{ margin: 24, minHeight: 'auto' }}>
          <Outlet />
        </Content>
      </Layout>
    </Layout>
  )
}
