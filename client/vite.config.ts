import { defineConfig } from 'npm:vite'
import react from 'npm:@vitejs/plugin-react'
import tailwindcss from 'npm:@tailwindcss/vite'
import daisyui from 'npm:daisyui'

export default defineConfig({
  plugins: [
    react(),
    tailwindcss({
      plugins: [daisyui]
    })
  ],
  server: {
    port: 3000
  }
})