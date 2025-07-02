import {defineConfig, UserConfig} from "vite";
import tailwindcss from "@tailwindcss/vite";
import react from '@vitejs/plugin-react'
import path from 'path'

/** @type {import('tailwindcss').Config} */
const viteConfig: UserConfig = {
    plugins: [
        react(),
        tailwindcss()
    ],
    resolve: {
        alias: {
            '@/models': path.resolve(__dirname, './src/models'),
            '@/atoms': path.resolve(__dirname, './src/atoms'),
            '@/functions': path.resolve(__dirname, './src/functions'),
            '@/hooks': path.resolve(__dirname, './src/hooks'),
            '@/components': path.resolve(__dirname, './src/components'),
            '@/pages': path.resolve(__dirname, './src/pages'),
            '@/app': path.resolve(__dirname, './src/app'),
        }
    }
}

/** @type {import('tailwindcss').Config} */
export default defineConfig(viteConfig);