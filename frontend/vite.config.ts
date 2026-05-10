import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    // Proxy all /api requests to the .NET backend during development.
    // This avoids CORS issues — the browser always talks to localhost:5173.
    proxy: {
      '/api': {
        target: 'http://localhost:5241',
        changeOrigin: true,
      },
    },
  },
})
