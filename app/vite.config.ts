import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      // IdentityApi (not in gateway)
      '/api/auth': { target: 'http://localhost:5178', changeOrigin: true },
      // StatsApi (not in gateway)
      '/api/stats': { target: 'http://localhost:5178', changeOrigin: true },
      // Everything else via API Gateway
      '/api': { target: 'http://localhost:5045', changeOrigin: true },
    },
  },
})
