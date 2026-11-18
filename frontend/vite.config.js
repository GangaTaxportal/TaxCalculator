import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server :{
    proxy:{
      "/api":{
                target: "https://localhost:7206",
        changeOrigin: true,
        secure: false, // accepts the local ASP.NET dev certificate
      }
    }
  }
})
