import daisyui from 'daisyui'
import {defineConfig} from "vite";
import tailwindcss from "@tailwindcss/vite";
import react from '@vitejs/plugin-react'

export default defineConfig({
    plugins: [
        react(),
        tailwindcss({
            plugins: [daisyui]
        })
    ],
    server: {
        runner: 'bun'
    }
})