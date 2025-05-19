import daisyui from 'daisyui'
import {defineConfig, UserConfig} from "vite";
import tailwindcss from "@tailwindcss/vite";
import react from '@vitejs/plugin-react'

/** @type {import('tailwindcss').Config} */
const viteConfig: UserConfig = {
    plugins: [
        react(),
        tailwindcss()
    ],

}

/** @type {import('tailwindcss').Config} */
export default defineConfig(viteConfig);