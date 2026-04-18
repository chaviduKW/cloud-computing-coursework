import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  const apiTarget = env.API_TARGET || 'http://localhost:5000'

  return {
    plugins: [react()],
    server: {
      proxy: {
        // All API traffic routed through the API Gateway
        // Override the target by setting API_TARGET in your .env file
        '/api': { target: apiTarget, changeOrigin: true },
      },
    },
  }
})
